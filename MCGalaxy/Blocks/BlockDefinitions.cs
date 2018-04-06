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
        
        public ushort BlockID; // really raw block ID
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
        
        public BlockID_ GetBlock() { return Block.FromRaw(BlockID); }
        public void SetBlock(BlockID_ b) { BlockID = Block.ToRaw(b); }
        
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
            def.InventoryOrder = InventoryOrder;
            return def;
        }
        
        
        public static BlockDefinition[] Load(bool global, string mapName) {
            BlockDefinition[] defs = null;
            string path = global ? GlobalPath : "blockdefs/lvl_" + mapName + ".json";
            try {
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    defs = JsonConvert.DeserializeObject<BlockDefinition[]>(json);
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            if (defs == null) return new BlockDefinition[Block.ExtendedCount];
            
            for (int i = 0; i < defs.Length; i++) {
                if (defs[i] != null && defs[i].Name == null) defs[i] = null;
                BlockDefinition def = defs[i];
                if (def == null) continue;
                
                if (!def.Version2) {
                    def.Version2 = true;
                    def.SetSideTex(def.SideTex);
                }
            }
            
            // Need to adjust index of raw block ID
            BlockDefinition[] adjDefs = new BlockDefinition[Block.ExtendedCount];
            for (int b = 0; b < defs.Length; b++) {
                BlockDefinition def = defs[b];
                if (def == null) continue;
                
                BlockID_ block = def.GetBlock();
                if (block >= adjDefs.Length) {
                    Logger.Log(LogType.Warning, "Invalid block ID: " + block);
                } else {
                    adjDefs[block] = def;
                }
            }
            return adjDefs;
        }
        
        public static void Save(bool global, Level lvl) {
            BlockDefinition[] defs = global ? GlobalDefs : lvl.CustomBlockDefs;
            // We don't want to save global blocks in the level's custom blocks list
            if (!global) {
                BlockDefinition[] realDefs = new BlockDefinition[defs.Length];
                for (int i = 0; i < defs.Length; i++) {
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
        
        static void UpdateLoadedLevels(BlockDefinition[] oldGlobalDefs) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                for (int b = 0; b < lvl.CustomBlockDefs.Length; b++) {
                    if (lvl.CustomBlockDefs[b] != oldGlobalDefs[b]) continue;
                    
                    // Can't use normal lvl.HasCustomProps here because we changed global list
                    if ((lvl.Props[b].ChangedScope & 2) == 0) {
                        lvl.Props[b] = Block.Props[b];
                    }
                    lvl.UpdateCustomBlock((BlockID_)b, GlobalDefs[b]);
                }
            }
        }
        
        
        public static void Add(BlockDefinition def, BlockDefinition[] defs, Level level) {
            BlockID_ block = def.GetBlock();
            bool global = defs == GlobalDefs;
            if (global) UpdateGlobalCustom(block, def);
            
            defs[block] = def;
            if (global) Block.SetDefaultNames();
            if (!global) level.UpdateCustomBlock(block, def);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (!pl.hasBlockDefs || def.BlockID > pl.MaxRawBlock) continue;
                if (global && pl.level.CustomBlockDefs[block] != GlobalDefs[block]) continue;
                
                pl.Send(def.MakeDefinePacket(pl));
                pl.SendCurrentBlockPermissions();
            }
            Save(global, level);
        }
        
        public static void Remove(BlockDefinition def, BlockDefinition[] defs, Level level) {
            BlockID_ block = def.GetBlock();
            bool global = defs == GlobalDefs;
            if (global) UpdateGlobalCustom(block, null);
            
            defs[block] = null;
            if (global) Block.SetDefaultNames();
            if (!global) level.UpdateCustomBlock(block, null);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (!pl.hasBlockDefs || def.BlockID > pl.MaxRawBlock) continue;
                if (global && pl.level.CustomBlockDefs[block] != null) continue;
                
                pl.Send(Packet.UndefineBlock(def, pl.hasExtBlocks));
            }
            Save(global, level);
        }
        
        public static void UpdateOrder(BlockDefinition def, bool global, Level level) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (!pl.Supports(CpeExt.InventoryOrder) || def.BlockID > pl.MaxRawBlock) continue;
                SendLevelInventoryOrder(pl);
            }
        }
        
        static void UpdateGlobalCustom(BlockID_ block, BlockDefinition def) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                if (lvl.CustomBlockDefs[block] != GlobalDefs[block]) continue;
                lvl.UpdateCustomBlock(block, def);
            }
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
                if (def == null || def.BlockID > pl.MaxRawBlock) continue;
                pl.Send(def.MakeDefinePacket(pl));
            }
        }
        
        internal unsafe static void SendLevelInventoryOrder(Player pl) {
            BlockDefinition[] defs = pl.level.CustomBlockDefs;
            
            int count = pl.MaxRawBlock + 1;
            int* order_to_blocks = stackalloc int[Block.ExtendedCount];
            int* block_to_orders = stackalloc int[Block.ExtendedCount];
            for (int b = 0; b < Block.ExtendedCount; b++) {
                order_to_blocks[b] = -1;
                block_to_orders[b] = -1;
            }
            
            // Fill slots with explicit order
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def == null || def.BlockID > pl.MaxRawBlock) continue;
                if (def.InventoryOrder == -1) continue;
                
                if (def.InventoryOrder != 255) {
                    if (order_to_blocks[def.InventoryOrder] != -1) continue;
                    order_to_blocks[def.InventoryOrder] = def.BlockID;
                }
                block_to_orders[def.BlockID] = def.InventoryOrder;
            }
            
            // Put blocks into their default slot if slot is unused
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                int raw = def != null ? def.BlockID : i;
                if (raw > pl.MaxRawBlock || (def == null && raw >= Block.CpeCount)) continue;
                
                if (def != null && def.InventoryOrder >= 0) continue;              
                if (order_to_blocks[raw] == -1) {
                    order_to_blocks[raw] = raw;
                    block_to_orders[raw] = raw;
                }
            }
            
            // Push blocks whose slots conflict with other blocks into free slots at end
            for (int i = defs.Length - 1; i >= 0; i--) {
                BlockDefinition def = defs[i];
                int raw = def != null ? def.BlockID : i;
                if (raw > pl.MaxRawBlock || (def == null && raw >= Block.CpeCount)) continue;
                
                if (block_to_orders[raw] != -1) continue;
                for (int slot = count - 1; slot >= 1; slot--) {
                    if (order_to_blocks[slot] != -1) continue;
                    
                    block_to_orders[raw]  = slot;
                    order_to_blocks[slot] = raw;
                    break;
                }
            }
            
            for (int raw = 0; raw < count; raw++) {
                int order = block_to_orders[raw];
                if (order == -1) order = 255;
                
                BlockDefinition def = defs[Block.FromRaw((BlockID_)raw)];
                if (def == null && raw >= Block.CpeCount) continue;
                // Special case, don't want 255 getting hidden by default
                if (raw == 255 && def.InventoryOrder == -1) continue;
                
                pl.Send(Packet.SetInventoryOrder((BlockID_)raw, (BlockID_)order, pl.hasExtBlocks));
            }
        }
        
        public byte[] MakeDefinePacket(Player pl) {
            if (pl.Supports(CpeExt.BlockDefinitionsExt, 2) && Shape != 0) {
                return Packet.DefineBlockExt(this, true, pl.hasCP437, pl.hasExtBlocks);
            } else if (pl.Supports(CpeExt.BlockDefinitionsExt) && Shape != 0) {
                return Packet.DefineBlockExt(this, false, pl.hasCP437, pl.hasExtBlocks);
            } else {
                return Packet.DefineBlock(this, pl.hasCP437, pl.hasExtBlocks);
            }
        }
        
        public static void UpdateFallback(bool global, BlockID_ block, Level level) {
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
