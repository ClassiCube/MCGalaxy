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

namespace MCGalaxy.Levels.IO {
    public static class CwFile {

        public static Level Load(Stream stream, string name) {
            NbtFile file = new NbtFile();
            file.LoadFromStream(stream);
            
            Level lvl;
            ImportData(file.RootTag, name, out lvl);
            if (file.RootTag.Contains("Metadata"))
                ImportMetadata((NbtCompound)file.RootTag["Metadata"], lvl);
            return lvl;
        }
        
        static void ImportData(NbtCompound root, string name, out Level lvl) {
            if (root["FormatVersion"].ByteValue > 1)
                throw new NotSupportedException("Only version 1 of ClassicWorld format is supported.");
            
            short x = root["X"].ShortValue, y = root["Y"].ShortValue, z = root["Z"].ShortValue;
            if (x <= 0 || y <= 0 || z <= 0)
                throw new InvalidDataException("Level dimensions must be > 0.");
            
            lvl = new Level(name, (ushort)x, (ushort)y, (ushort)z);
            lvl.blocks = root["BlockArray"].ByteArrayValue;
            FcmFile.ConvertExtended(lvl);
            
            if (!root.Contains("Spawn")) return;
            NbtTag spawn = root["Spawn"];
            lvl.spawnx = (ushort)spawn["X"].ShortValue;
            lvl.spawny = (ushort)spawn["Y"].ShortValue;
            lvl.spawnz = (ushort)spawn["Z"].ShortValue;
            lvl.rotx = spawn["H"].ByteValue;
            lvl.roty = spawn["P"].ByteValue;
        }
        
        static void ImportMetadata(NbtCompound root, Level lvl) {
            if (!root.Contains("CPE")) return;
            NbtCompound cpe = (NbtCompound)root["CPE"];
            
            if (cpe.Contains("EnvWeatherType"))
                lvl.Weather = cpe["EnvWeatherType"]["WeatherType"].ByteValue;
            if (cpe.Contains("EnvMapAppearance"))
                ParseEnvMapAppearance(cpe, lvl);
            if (cpe.Contains("EnvColors"))
                ParseEnvColors(cpe, lvl);
            if (cpe.Contains("BlockDefinitions"))
                ParseBlockDefinitions(cpe, lvl);
            
            foreach(NbtTag tag in cpe.Tags)
                Server.s.Log(tag.Name + " - " + tag.TagType);
        }
        
        static void ParseEnvMapAppearance(NbtCompound cpe, Level lvl) {
            NbtCompound comp = (NbtCompound)cpe["EnvMapAppearance"];
            lvl.HorizonBlock = comp["EdgeBlock"].ByteValue;
            lvl.EdgeBlock = comp["SideBlock"].ByteValue;
            lvl.EdgeLevel = comp["SideLevel"].ShortValue;
            
            if (lvl.EdgeLevel == -1)
                lvl.EdgeLevel = (short)(lvl.Height / 2);
            if (!comp.Contains("TextureURL")) return;
            
            string url = comp["TextureURL"].StringValue;
            if (url.CaselessEnds(".png"))
                lvl.terrainUrl = url == Server.defaultTerrainUrl ? "" : url;
            else
                lvl.texturePackUrl = url == Server.defaultTextureUrl ? "" : url;
        }
        
        static void ParseEnvColors(NbtCompound cpe, Level lvl) {
            NbtCompound comp = (NbtCompound)cpe["EnvColors"];
            lvl.SkyColor = GetColor(comp, "Sky");
            lvl.CloudColor = GetColor(comp, "Cloud");
            lvl.FogColor = GetColor(comp, "Fog");
            lvl.LightColor = GetColor(comp, "Sunlight");
            lvl.ShadowColor = GetColor(comp, "Ambient");
        }
        
        static string GetColor(NbtCompound comp, string type) {
            if (!comp.Contains(type)) return "";
            NbtCompound rgb = (NbtCompound)comp[type];            
            short r = rgb["R"].ShortValue, g = rgb["G"].ShortValue, b = rgb["B"].ShortValue;
            
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255) return "";
            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
		
		static void ParseBlockDefinitions(NbtCompound cpe, Level lvl) {
			
		}
    }
}