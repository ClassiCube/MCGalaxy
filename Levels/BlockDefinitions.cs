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
using System.Text;
using Newtonsoft.Json;

namespace MCGalaxy {
    
    public sealed class BlockDefinition {
        
        public byte BlockID;
        public string Name;
        public byte CollideType;
        public float Speed;
        public byte TopTex, SideTex, BottomTex;
        public bool BlocksLight;
        public byte WalkSound;
        public bool FullBright;
        public byte Shape;
        public byte BlockDraw;
        public byte FogDensity, FogR, FogG, FogB;
        public byte FallBack;
        // BlockDefinitionsExt fields
        public byte MinX, MinY, MinZ;
        public byte MaxX, MaxY, MaxZ;
        // BlockDefinitionsExt version 2 fields
        public bool Version2;
        public byte LeftTex, RightTex, FrontTex, BackTex;
        
        public const string GlobalPath = "blockdefs/global.json", GlobalBackupPath = "blockdefs/global.json.bak";
        
        public static BlockDefinition[] GlobalDefs;
        
        public BlockDefinition Copy() {
            BlockDefinition def = new BlockDefinition();
            def.BlockID = BlockID; def.Name = Name;
            def.CollideType = CollideType; def.Speed = Speed;
            def.TopTex = TopTex; def.SideTex = SideTex;
            def.BottomTex = BottomTex; def.BlocksLight = BlocksLight;
            def.WalkSound = WalkSound; def.FullBright = FullBright;
            def.Shape = Shape; def.BlockDraw = BlockDraw;
            def.FogDensity = FogDensity; def.FogR = FogR;
            def.FogG = FogG; def.FogB = FogB;
            def.FallBack = FallBack;
            def.MinX = MinX; def.MinY = MinY; def.MinZ = MinZ;
            def.MaxX = MaxX; def.MaxY = MaxY; def.MaxZ = MaxZ;
            def.Version2 = Version2;
            def.LeftTex = LeftTex; def.RightTex = RightTex;
            def.FrontTex = FrontTex; def.BackTex = BackTex;
            return def;
        }
        
        public static void LoadGlobal() {
            GlobalDefs = Load(true, null);
            GlobalDefs[0] = new BlockDefinition();
            GlobalDefs[0].Name = "Air fallback";
            
            try {
                if (File.Exists(GlobalPath) && File.Exists(GlobalBackupPath)) {
                    File.Delete(GlobalBackupPath);
                    File.Copy(GlobalPath, GlobalBackupPath);
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
            Save(true, null);
        }
        
        internal static BlockDefinition[] Load(bool global, Level lvl) {
            BlockDefinition[] defs = new BlockDefinition[256];
            string path = global ? GlobalPath : "blockdefs/lvl_" + lvl.name + ".json";
            try {
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    defs = JsonConvert.DeserializeObject<BlockDefinition[]>(json);
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                defs = new BlockDefinition[256];
            }
            
            for (int i = 0; i < 256; i++) {               
                if (defs[i] != null && defs[i].Name == null)
                    defs[i] = null;
            }
            
            for (int i = 0; i < 256; i++) {
                BlockDefinition def = defs[i];
                if (def == null) continue;
                
                if (!def.Version2) {
                    def.Version2 = true;
                    def.LeftTex = def.SideTex; def.RightTex = def.SideTex;
                    def.FrontTex = def.SideTex; def.BackTex = def.SideTex;
                }
            }
            return defs;
        }
        
        internal static void Save(bool global, Level lvl) {
            BlockDefinition[] defs = global ? GlobalDefs : lvl.CustomBlockDefs;
            // We don't want to save global blocks in the level's custom blocks list
            if (!global) {
                BlockDefinition[] realDefs = new BlockDefinition[256];
                for (int i = 0; i < 256; i++)
                    realDefs[i] = defs[i] == GlobalDefs[i] ? null : defs[i];
                defs = realDefs;
            }
           
            string json = JsonConvert.SerializeObject(defs);
            string path = global ? GlobalPath : "blockdefs/lvl_" + lvl.name + ".json";
            File.WriteAllText(path, json);
        }
        
        public static void Add(BlockDefinition def, BlockDefinition[] defs, Level level) {
            byte id = def.BlockID;
            bool global = defs == GlobalDefs; 
            if (global) {
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[id] == null)
                        lvl.CustomBlockDefs[id] = def;
                }
            }
            defs[id] = def;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (!pl.hasBlockDefs) continue;
                if (global && pl.level.CustomBlockDefs[id] != GlobalDefs[id]) continue;
                
                if (pl.HasCpeExt(CpeExt.BlockDefinitionsExt, 2) && def.Shape != 0)
                    SendDefineBlockExt(pl, def, true);
                else if (pl.HasCpeExt(CpeExt.BlockDefinitionsExt) && def.Shape != 0)
                    SendDefineBlockExt(pl, def, false);
                else
                    SendDefineBlock(pl, def);
                
                if (pl.HasCpeExt(CpeExt.BlockPermissions))
                    pl.SendSetBlockPermission(def.BlockID, pl.level.CanPlace, pl.level.CanDelete);
            }
            Save(global, level);
        }
        
        public static void Remove(BlockDefinition def, BlockDefinition[] defs, Level level) {
            byte id = def.BlockID;
            bool global = defs == GlobalDefs;
            if (global) {
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    if (lvl.CustomBlockDefs[id] == GlobalDefs[id])
                        lvl.CustomBlockDefs[id] = null;
                }
            }
            defs[id] = null;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (global && pl.level.CustomBlockDefs[id] != null) continue;
                
                if (pl.hasBlockDefs)
                    pl.SendRaw(Opcode.CpeRemoveBlockDefinition, id);
            }
            Save(global, level);
        }
        
        internal static void SendLevelCustomBlocks(Player pl) {
            BlockDefinition[] defs = pl.level.CustomBlockDefs;
            for (int i = 1; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def == null) continue;
                if (pl.HasCpeExt(CpeExt.BlockDefinitionsExt, 2) && def.Shape != 0)
                    SendDefineBlockExt(pl, def, true);
                else if (pl.HasCpeExt(CpeExt.BlockDefinitionsExt) && def.Shape != 0)
                    SendDefineBlockExt(pl, def, false);
                else
                    SendDefineBlock(pl, def);
                
                if (pl.HasCpeExt(CpeExt.BlockPermissions))
                    pl.SendSetBlockPermission(def.BlockID, pl.level.CanPlace, pl.level.CanDelete);
            }
        }
        
        public static byte GetBlock(string msg, Player p) {
            BlockDefinition[] defs = p.level.CustomBlockDefs;
            for (int i = 1; i < 255; i++) {
                BlockDefinition def = defs[i];
                if (def == null) continue;             
                if (def.Name.Replace(" ", "").CaselessEq(msg))
                    return def.BlockID;
            }
            
            byte extBlock;
            if (!byte.TryParse(msg, out extBlock) || defs[extBlock] == null)
                return Block.Zero;
            return extBlock;
        }
        
        static void SendDefineBlock(Player p, BlockDefinition def) {
            byte[] buffer = new byte[80];
            int index = 0;
            buffer[index++] = Opcode.CpeDefineBlock;
            MakeDefineBlockStart(def, buffer, ref index, false);
            buffer[index++] = def.Shape;
            MakeDefineBlockEnd(def, ref index, buffer);
            p.Send(buffer);
        }
        
        static void SendDefineBlockExt(Player p, BlockDefinition def, bool uniqueSideTexs) {
            byte[] buffer = new byte[uniqueSideTexs ? 88 : 85];
            int index = 0;
            buffer[index++] = Opcode.CpeDefineBlockExt;
            MakeDefineBlockStart(def, buffer, ref index, uniqueSideTexs);
            buffer[index++] = def.MinX;
            buffer[index++] = def.MinZ;
            buffer[index++] = def.MinY;
            buffer[index++] = def.MaxX;
            buffer[index++] = def.MaxZ;
            buffer[index++] = def.MaxY;
            MakeDefineBlockEnd(def, ref index, buffer);
            p.Send(buffer);
        }
        
        static void MakeDefineBlockStart(BlockDefinition def, byte[] buffer, ref int index, bool uniqueSideTexs) {
            // speed = 2^((raw - 128) / 64);
            // therefore raw = 64log2(speed) + 128
            byte rawSpeed = (byte)(64 * Math.Log(def.Speed, 2) + 128);
            buffer[index++] = def.BlockID;
            NetUtils.WriteAscii(def.Name, buffer, index);
            index += 64;      
            buffer[index++] = def.CollideType;
            buffer[index++] = rawSpeed;
            
            buffer[index++] = def.TopTex;
            if (uniqueSideTexs) {
                buffer[index++] = def.LeftTex;
                buffer[index++] = def.RightTex;
                buffer[index++] = def.FrontTex;
                buffer[index++] = def.BackTex;
            } else {
                buffer[index++] = def.SideTex;
            }
            
            buffer[index++] = def.BottomTex;
            buffer[index++] = (byte)(def.BlocksLight ? 0 : 1);
            buffer[index++] = def.WalkSound;
            buffer[index++] = (byte)(def.FullBright ? 1 : 0);
        }
        
        static void MakeDefineBlockEnd(BlockDefinition def, ref int index, byte[] buffer) {
            buffer[index++] = def.BlockDraw;
            buffer[index++] = def.FogDensity;
            buffer[index++] = def.FogR;
            buffer[index++] = def.FogG;
            buffer[index++] = def.FogB;
        }
    }
}
