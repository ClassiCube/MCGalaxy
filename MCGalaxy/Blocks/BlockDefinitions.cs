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
using MCGalaxy.Blocks;
using MCGalaxy.Network;
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
        public static BlockProps[] GlobalProps = new BlockProps[Block.Count];
        
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
        
        
        internal static BlockDefinition[] Load(bool global, Level lvl) {
            BlockDefinition[] defs = new BlockDefinition[Block.Count];
            string path = global ? GlobalPath : "blockdefs/lvl_" + lvl.MapName + ".json";
            try {
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    defs = JsonConvert.DeserializeObject<BlockDefinition[]>(json);
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
                defs = new BlockDefinition[Block.Count];
            }
            
            for (int i = 0; i < Block.Count; i++) {
                if (defs[i] != null && defs[i].Name == null)
                    defs[i] = null;
            }
            
            for (int i = 0; i < Block.Count; i++) {
                BlockDefinition def = defs[i];
                if (def == null) continue;
                
                if (!def.Version2) {
                    def.Version2 = true;
                    def.SetSideTex(def.SideTex);
                }
            }
            return defs;
        }
        
        internal static void Save(bool global, Level lvl) {
            BlockDefinition[] defs = global ? GlobalDefs : lvl.CustomBlockDefs;
            // We don't want to save global blocks in the level's custom blocks list
            if (!global) {
                BlockDefinition[] realDefs = new BlockDefinition[Block.Count];
                for (int i = 0; i < Block.Count; i++) {
                    realDefs[i] = defs[i] == GlobalDefs[i] ? null : defs[i];
                }
                defs = realDefs;
            }
            
            string json = JsonConvert.SerializeObject(defs, Formatting.Indented);
            string path = global ? GlobalPath : "blockdefs/lvl_" + lvl.MapName + ".json";
            File.WriteAllText(path, json);
        }
        
        public static void LoadGlobal() {
            BlockDefinition[] oldDefs = GlobalDefs;
            GlobalDefs = Load(true, null);
            GlobalDefs[Block.Air] = DefaultSet.MakeCustomBlock(Block.Air);
            GlobalDefs[Block.Air].Name = "Air fallback";
            
            try {
                if (File.Exists(GlobalPath)) {
                    File.Copy(GlobalPath, GlobalBackupPath, true);
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            
            Save(true, null);
            // As the BlockDefinition instances in levels will now be different
            // to the instances in GlobalDefs, we need to update them.
            if (oldDefs != null) UpdateLoadedLevels(oldDefs);
        }
        
        public static void UpdateGlobalBlockProps() {
            for (int i = 0; i < GlobalProps.Length; i++) {
                ExtBlock block = ExtBlock.FromRaw((byte)i);
                GlobalProps[i] = BlockProps.MakeDefault();
                GlobalProps[i] = DefaultProps(block);
            }
            BlockProps.Load("global", GlobalProps, false);
        }
        
        internal static BlockProps DefaultProps(ExtBlock block) {
            if (block.IsPhysicsType) {
                return Block.Props[block.Index];
            } else if (!block.IsCustomType && GlobalDefs[block.RawID] == null) {
                return Block.Props[block.RawID];
            } else {
                return GlobalProps[block.RawID];
            }
        }
        
        static void UpdateLoadedLevels(BlockDefinition[] oldGlobalDefs) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                for (int i = 0; i < lvl.CustomBlockDefs.Length; i++) {
                    if (lvl.CustomBlockDefs[i] != oldGlobalDefs[i]) continue;
                    
                    ExtBlock block = ExtBlock.FromRaw((byte)i);
                    lvl.BlockProps[block.Index] = DefaultProps(block);
                    lvl.UpdateCustomBlock(block.RawID, GlobalDefs[i]);
                }
            }
        }
        
        
        public static void Add(BlockDefinition def, BlockDefinition[] defs, Level level) {
            byte raw = def.BlockID;
            bool global = defs == GlobalDefs;
            if (global) UpdateGlobalCustom(raw, def);
            
            defs[raw] = def;
            if (global) Block.SetDefaultNames();
            if (!global) level.UpdateCustomBlock(raw, def);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (!pl.hasBlockDefs) continue;
                if (global && pl.level.CustomBlockDefs[raw] != GlobalDefs[raw]) continue;
                
                pl.Send(def.MakeDefinePacket(pl));
                if (pl.HasCpeExt(CpeExt.BlockPermissions))
                    pl.Send(Packet.BlockPermission(def.BlockID, pl.level.CanPlace, pl.level.CanDelete));
            }
            Save(global, level);
        }
        
        public static void Remove(BlockDefinition def, BlockDefinition[] defs, Level level) {
            byte raw = def.BlockID;
            bool global = defs == GlobalDefs;
            if (global) UpdateGlobalCustom(raw, null);
            
            defs[raw] = null;
            if (global) Block.SetDefaultNames();
            if (!global) level.UpdateCustomBlock(raw, null);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (global && pl.level.CustomBlockDefs[raw] != null) continue;
                
                if (pl.hasBlockDefs)
                    pl.Send(Packet.UndefineBlock(raw));
            }
            Save(global, level);
        }
        
        static void UpdateGlobalCustom(byte raw, BlockDefinition def) {
            Level[] loaded = LevelInfo.Loaded.Items;          
            foreach (Level lvl in loaded) {
                if (lvl.CustomBlockDefs[raw] != GlobalDefs[raw]) continue;
                lvl.UpdateCustomBlock(raw, def);
            }
        }
        
        
        public static byte GetBlock(string msg, Player p) {
            return GetBlock(msg, p.level.CustomBlockDefs);
        }
        
        public static byte GetBlock(string msg, BlockDefinition[] defs) {
            for (int i = 1; i < Block.Invalid; i++) {
                BlockDefinition def = defs[i];
                if (def == null) continue;
                if (def.Name.Replace(" ", "").CaselessEq(msg))
                    return def.BlockID;
            }
            
            byte id;
            if (!byte.TryParse(msg, out id) || defs[id] == null)
                return Block.Invalid;
            return id;
        }
        
        public void SetAllTex(byte id) {
            SideTex = id; TopTex = id; BottomTex = id;
            LeftTex = id; RightTex = id; FrontTex = id; BackTex = id;
        }
        
        public void SetSideTex(byte id) {
            SideTex = id;
            LeftTex = id; RightTex = id; FrontTex = id; BackTex = id;
        }
        
        
        internal static void SendLevelCustomBlocks(Player pl) {
            BlockDefinition[] defs = pl.level.CustomBlockDefs;
            for (int i = 1; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def != null) pl.Send(def.MakeDefinePacket(pl));
            }
        }
        
        public byte[] MakeDefinePacket(Player pl) {
            if (pl.HasCpeExt(CpeExt.BlockDefinitionsExt, 2) && Shape != 0) {
                return Packet.DefineBlockExt(this, true, pl.hasCP437);
            } else if (pl.HasCpeExt(CpeExt.BlockDefinitionsExt) && Shape != 0) {
                return Packet.DefineBlockExt(this, false, pl.hasCP437);
            } else {
                return Packet.DefineBlock(this, pl.hasCP437);
            }
        }
        
        public static void UpdateFallback(bool global, byte block, Level level) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (pl.hasBlockDefs) continue;
                
                // if custom block is replacing core block, need to always reload for fallback
                if (block >= Block.CpeCount && !pl.level.MayHaveCustomBlocks) continue;
                
                LevelActions.ReloadMap(pl, pl, false);
            }
        }
    }
}
