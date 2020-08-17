/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.IO;
using fNbt;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Levels.IO {
    public sealed class CwImporter : IMapImporter {

        public override string Extension { get { return ".cw"; } }
        public override string Description { get { return "ClassiCube/ClassicalSharp map"; } }

        public override Vec3U16 ReadDimensions(Stream src) {
            throw new NotSupportedException();
        }
        
        public override Level Read(Stream src, string name, bool metadata) {
            NbtFile file = new NbtFile();
            file.LoadFromStream(src);
            
            Level lvl;
            ReadData(file.RootTag, name, out lvl);
            if (!metadata) return lvl;
            
            if (file.RootTag.Contains("Metadata"))
                ReadMetadata((NbtCompound)file.RootTag["Metadata"], lvl);
            return lvl;
        }
        
        static void ReadData(NbtCompound root, string name, out Level lvl) {
            if (root["FormatVersion"].ByteValue > 1)
                throw new NotSupportedException("Only version 1 of ClassicWorld format is supported.");
            
            ushort width  = (ushort)root["X"].ShortValue;
            ushort height = (ushort)root["Y"].ShortValue;
            ushort length = (ushort)root["Z"].ShortValue;
            byte[] blocks = root["BlockArray"].ByteArrayValue;            
            lvl = new Level(name, width, height, length, blocks);
            
            ReadSpawn(root, lvl);           
            #if TEN_BIT_BLOCKS
            // Can't use ConvertCustom, as that changes lvl.blocks
            // (aka the array containing the lower 8 bits of block ids)
            if (root.Contains("BlockArray2")) {
                ReadExtBlocks(root, lvl); return;
            }
            #endif
            ConvertCustom(lvl);
        }
        
        #if TEN_BIT_BLOCKS
        static void ReadExtBlocks(NbtCompound root, Level lvl) {
            byte[] lo = root["BlockArray"].ByteArrayValue;
            byte[] hi = root["BlockArray2"].ByteArrayValue;
            
            for (int i = 0; i < lo.Length; i++) {
                if (hi[i] == 0 && lo[i] <= Block.CpeMaxBlock) continue;
                ushort x, y, z;
                
                lvl.IntToPos(i, out x, out y, out z);
                int b = ((hi[i] << 8) | lo[i]) + Block.Extended;
                lvl.SetBlock(x, y, z, (BlockID)b);
            }
        }
        #endif
        
        static void ReadSpawn(NbtCompound root, Level lvl) {
            if (!root.Contains("Spawn")) return;
            NbtTag spawn = root["Spawn"];
            lvl.spawnx = (ushort)spawn["X"].ShortValue;
            lvl.spawny = (ushort)spawn["Y"].ShortValue;
            lvl.spawnz = (ushort)spawn["Z"].ShortValue;
            lvl.rotx = spawn["H"].ByteValue;
            lvl.roty = spawn["P"].ByteValue;
        }
        
        static void ReadMetadata(NbtCompound root, Level lvl) {
            if (!root.Contains("CPE")) return;
            NbtCompound cpe = (NbtCompound)root["CPE"];
            
            if (cpe.Contains("EnvWeatherType"))
                lvl.Config.Weather = cpe["EnvWeatherType"]["WeatherType"].ByteValue;
            if (cpe.Contains("EnvMapAppearance"))
                ParseEnvMapAppearance(cpe, lvl);
            if (cpe.Contains("EnvColors"))
                ParseEnvColors(cpe, lvl);
            if (cpe.Contains("BlockDefinitions"))
                ParseBlockDefinitions(cpe, lvl);
        }
        
        
        static void ParseEnvMapAppearance(NbtCompound cpe, Level lvl) {
            NbtCompound comp = (NbtCompound)cpe["EnvMapAppearance"];
            lvl.Config.HorizonBlock = Block.FromRaw(comp["EdgeBlock"].ByteValue);
            lvl.Config.EdgeBlock    = Block.FromRaw(comp["SideBlock"].ByteValue);
            lvl.Config.EdgeLevel    = comp["SideLevel"].ShortValue;
            if (!comp.Contains("TextureURL")) return;
            
            string url = comp["TextureURL"].StringValue;
            if (url.CaselessEnds(".png"))
                lvl.Config.Terrain = url == Server.Config.DefaultTerrain ? "" : url;
            else
                lvl.Config.TexturePack = url == Server.Config.DefaultTexture ? "" : url;
        }
        
        static void ParseEnvColors(NbtCompound cpe, Level lvl) {
            NbtCompound comp = (NbtCompound)cpe["EnvColors"];
            lvl.Config.SkyColor    = GetColor(comp, "Sky");
            lvl.Config.CloudColor  = GetColor(comp, "Cloud");
            lvl.Config.FogColor    = GetColor(comp, "Fog");
            lvl.Config.LightColor  = GetColor(comp, "Sunlight");
            lvl.Config.ShadowColor = GetColor(comp, "Ambient");
        }
        
        static string GetColor(NbtCompound comp, string type) {
            if (!comp.Contains(type)) return "";
            NbtCompound rgb = (NbtCompound)comp[type];
            short r = rgb["R"].ShortValue, g = rgb["G"].ShortValue, b = rgb["B"].ShortValue;
            
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255) return "";
            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
        
        static void ParseBlockDefinitions(NbtCompound cpe, Level lvl) {
            NbtCompound blocks = (NbtCompound)cpe["BlockDefinitions"];
            bool hasBlockDefs = false;
            
            foreach (NbtTag tag in blocks) {
                if (tag.TagType != NbtTagType.Compound) continue;
                
                NbtCompound props = (NbtCompound)tag;
                BlockDefinition def = new BlockDefinition();
                def.RawID = props["ID"].ByteValue;
                // can't change "ID" to short since backwards compatibility
                if (props.Contains("ID2")) def.RawID = (ushort)props["ID2"].ShortValue;
                
                def.Name = props["Name"].StringValue;
                def.CollideType = props["CollideType"].ByteValue;
                def.Speed = props["Speed"].FloatValue;
                
                def.BlocksLight = props["TransmitsLight"].ByteValue == 0;
                def.WalkSound = props["WalkSound"].ByteValue;
                def.FullBright = props["FullBright"].ByteValue != 0;
                def.Shape = props["Shape"].ByteValue;
                def.BlockDraw = props["BlockDraw"].ByteValue;
                
                byte[] fog = props["Fog"].ByteArrayValue;
                def.FogDensity = fog[0];
                // Fix for older ClassicalSharp versions which saved wrong value for density = 0
                if( def.FogDensity == 0xFF ) def.FogDensity = 0;
                def.FogR = fog[1]; def.FogG = fog[2]; def.FogB = fog[3];
                
                byte[] tex = props["Textures"].ByteArrayValue;
                ImportTexs(def, tex, 0);
                if (tex.Length > 6) ImportTexs(def, tex, 6);

                byte[] coords = props["Coords"].ByteArrayValue;
                def.MinX = coords[0]; def.MinZ = coords[1]; def.MinY = coords[2];
                def.MaxX = coords[3]; def.MaxZ = coords[4]; def.MaxY = coords[5];
                
                BlockID block = def.GetBlock();
                if (block >= Block.ExtendedCount) {
                    Logger.Log(LogType.Warning, "Cannot import custom block {0} (ID {1})",
                               def.Name, def.RawID);
                    continue;
                }
                
                // Don't define level custom block if same as global custom block
                BlockDefinition globalDef = BlockDefinition.GlobalDefs[block];
                if (PropsEquals(def, globalDef)) continue;
                
                lvl.UpdateCustomBlock(block, def);
                hasBlockDefs = true;
            }
            
            if (hasBlockDefs)
                BlockDefinition.Save(false, lvl);
        }
        
        static void ImportTexs(BlockDefinition def, byte[] tex, int i) {
            int s = i == 0 ? 0 : 8;
            def.TopTex   |= (ushort)(tex[i+0] << s); def.BottomTex |= (ushort)(tex[i+1] << s);
            def.LeftTex  |= (ushort)(tex[i+2] << s); def.RightTex  |= (ushort)(tex[i+3] << s);
            def.FrontTex |= (ushort)(tex[i+4] << s); def.BackTex   |= (ushort)(tex[i+5] << s);
        }
        
        static bool PropsEquals(BlockDefinition a, BlockDefinition b) {
            if (b == null || b.Name == null) return false;
            return a.Name == b.Name && a.CollideType == b.CollideType && a.Speed == b.Speed && a.TopTex == b.TopTex
                && a.BottomTex == b.BottomTex && a.BlocksLight == b.BlocksLight && a.WalkSound == b.WalkSound
                && a.FullBright == b.FullBright && a.Shape == b.Shape && a.BlockDraw == b.BlockDraw
                && a.FogDensity == b.FogDensity && a.FogR == b.FogR && a.FogG == b.FogG && a.FogB == b.FogB
                && a.MinX == b.MinX && a.MinY == b.MinY && a.MinZ == b.MinZ && a.MaxX == b.MaxX
                && a.MaxY == b.MaxY && a.MaxZ == b.MaxZ && a.LeftTex == b.LeftTex && a.RightTex == b.RightTex
                && a.FrontTex == b.FrontTex && a.BackTex == b.BackTex;
        }
    }
}