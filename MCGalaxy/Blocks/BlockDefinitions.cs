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
using BlockID_ = System.UInt16;
using BlockRaw = System.Byte;

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
        
        public int InventoryOrder = -1;
        
        public const string GlobalPath = "blockdefs/global.json", GlobalBackupPath = "blockdefs/global.json.bak";
        
        public static BlockDefinition[] GlobalDefs;
        public static BlockProps[] GlobalProps = new BlockProps[Block.Count];
        internal static readonly object GlobalPropsLock = new object();
        
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
            def.InventoryOrder = InventoryOrder;
            return def;
        }
        
        
        internal static BlockDefinition[] Load(bool global, Level lvl) {
            BlockDefinition[] defs = null;
            string path = global ? GlobalPath : "blockdefs/lvl_" + lvl.MapName + ".json";
            try {
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    defs = JsonConvert.DeserializeObject<BlockDefinition[]>(json);
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            if (defs == null) defs = new BlockDefinition[Block.Count];
            
            // File was probably manually modified - fix it up
            if (defs.Length < Block.Count) {
                Logger.Log(LogType.Warning, "Expected " + Block.Count + " blocks in " + path + ", but only had " + defs.Length);
                Array.Resize(ref defs, Block.Count);
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
            GlobalDefs[Block.Air] = null;
            
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
                BlockID_ block = Block.FromRaw((byte)i);
                GlobalProps[i] = BlockProps.MakeDefault();
                GlobalProps[i] = DefaultProps(block);
            }
            BlockProps.Load("global", GlobalProps, GlobalPropsLock, false);
        }
        
        internal static BlockProps DefaultProps(BlockID_ block) {
            BlockRaw raw = (BlockRaw)block;
            if (Block.IsPhysicsType(block)) {
                return Block.Props[block];
            } else if (block < Block.Extended && GlobalDefs[raw] == null) {
                return Block.Props[raw];
            } else {
                return GlobalProps[raw];
            }
        }
        
        static void UpdateLoadedLevels(BlockDefinition[] oldGlobalDefs) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                for (int i = 0; i < lvl.CustomBlockDefs.Length; i++) {
                    if (lvl.CustomBlockDefs[i] != oldGlobalDefs[i]) continue;
                    
                    BlockID_ block = Block.FromRaw((byte)i);
                    lvl.Props[block] = DefaultProps(block);
                    lvl.UpdateCustomBlock((BlockRaw)block, GlobalDefs[i]);
                }
            }
        }
        
        
        public static void Add(BlockDefinition def, BlockDefinition[] defs, Level level) {
            BlockRaw raw = def.BlockID;
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
                if (pl.Supports(CpeExt.BlockPermissions))
                    pl.Send(Packet.BlockPermission(def.BlockID, pl.level.CanPlace, pl.level.CanDelete));
            }
            Save(global, level);
        }
        
        public static void Remove(BlockDefinition def, BlockDefinition[] defs, Level level) {
            BlockRaw raw = def.BlockID;
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
        
        public static void UpdateOrder(BlockDefinition def, bool global, Level level) {
            if (def.InventoryOrder == -1) return;
            byte raw = def.BlockID, order = (byte)def.InventoryOrder;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (global && pl.level.CustomBlockDefs[raw] != GlobalDefs[raw]) continue;

                if (!pl.Supports(CpeExt.InventoryOrder)) continue;
                pl.Send(Packet.SetInventoryOrder(raw, order));
            }
        }
        
        static void UpdateGlobalCustom(BlockRaw raw, BlockDefinition def) {
            Level[] loaded = LevelInfo.Loaded.Items;          
            foreach (Level lvl in loaded) {
                if (lvl.CustomBlockDefs[raw] != GlobalDefs[raw]) continue;
                lvl.UpdateCustomBlock(raw, def);
            }
        }
        
 
        public static int GetBlock(string msg, BlockDefinition[] defs) {
            for (int i = 1; i < Block.Count; i++) {
                BlockDefinition def = defs[i];
                if (def == null) continue;
                if (def.Name.Replace(" ", "").CaselessEq(msg))
                    return def.BlockID;
            }
            
            BlockRaw id;
            if (!BlockRaw.TryParse(msg, out id) || defs[id] == null) return -1;
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
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def != null) pl.Send(def.MakeDefinePacket(pl));
            }
        }
        
        internal static void SendLevelInventoryOrder(Player pl) {
            BlockDefinition[] defs = pl.level.CustomBlockDefs;
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def != null && def.InventoryOrder >= 0) {
                    pl.Send(Packet.SetInventoryOrder((byte)i, (byte)def.InventoryOrder));
                }
            }
        }
        
        public byte[] MakeDefinePacket(Player pl) {
            if (pl.Supports(CpeExt.BlockDefinitionsExt, 2) && Shape != 0) {
                return Packet.DefineBlockExt(this, true, pl.hasCP437);
            } else if (pl.Supports(CpeExt.BlockDefinitionsExt) && Shape != 0) {
                return Packet.DefineBlockExt(this, false, pl.hasCP437);
            } else {
                return Packet.DefineBlock(this, pl.hasCP437);
            }
        }
        
        public static void UpdateFallback(bool global, BlockRaw raw, Level level) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (pl.hasBlockDefs) continue;
                
                // if custom block is replacing core block, need to always reload for fallback
                if (raw >= Block.CpeCount && !pl.level.MayHaveCustomBlocks) continue;
                
                LevelActions.ReloadMap(pl, pl, false);
            }
        }
    }
}
