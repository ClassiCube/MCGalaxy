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
using MCGalaxy.Config;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy {
    public sealed class BlockDefinition {
        
        [ConfigUShort("BlockID", null)]
        public ushort RawID;
        [ConfigString] public string Name;
        [ConfigFloat]  public float Speed;
        [ConfigByte]   public byte CollideType;
        [ConfigUShort] public ushort TopTex;
        [ConfigUShort] public ushort BottomTex;
        
        [ConfigBool] public bool BlocksLight;
        [ConfigByte] public byte WalkSound;
        [ConfigBool] public bool FullBright;
        [ConfigByte] public byte Shape;
        [ConfigByte] public byte BlockDraw;
        [ConfigByte] public byte FallBack;
        
        [ConfigByte] public byte FogDensity;
        [ConfigByte] public byte FogR;
        [ConfigByte] public byte FogG;
        [ConfigByte] public byte FogB;
        
        // BlockDefinitionsExt fields
        [ConfigByte] public byte MinX;
        [ConfigByte] public byte MinY;
        [ConfigByte] public byte MinZ;
        [ConfigByte] public byte MaxX;
        [ConfigByte] public byte MaxY;
        [ConfigByte] public byte MaxZ;
        
        // BlockDefinitionsExt version 2 fields
        [ConfigUShort] public ushort LeftTex;
        [ConfigUShort] public ushort RightTex;
        [ConfigUShort] public ushort FrontTex;
        [ConfigUShort] public ushort BackTex;
        
        [ConfigInt(null, null, -1, -1)]
        public int InventoryOrder = -1;
        
        public BlockID GetBlock() { return Block.FromRaw(RawID); }
        public void SetBlock(BlockID b) { RawID = Block.ToRaw(b); }
        
        public const string GlobalPath = "blockdefs/global.json", GlobalBackupPath = "blockdefs/global.json.bak";
        
        public static BlockDefinition[] GlobalDefs;
        
        public BlockDefinition Copy() {
            BlockDefinition def = new BlockDefinition();
            def.RawID = RawID; def.Name = Name;
            def.Speed = Speed; def.CollideType = CollideType; 
            def.TopTex = TopTex; def.BottomTex = BottomTex;
            
            def.BlocksLight = BlocksLight; def.WalkSound = WalkSound;
            def.FullBright = FullBright; def.Shape = Shape;
            def.BlockDraw = BlockDraw; def.FallBack = FallBack;
            
            def.FogDensity = FogDensity;
            def.FogR = FogR; def.FogG = FogG; def.FogB = FogB;          
            def.MinX = MinX; def.MinY = MinY; def.MinZ = MinZ;
            def.MaxX = MaxX; def.MaxY = MaxY; def.MaxZ = MaxZ;
            
            def.LeftTex = LeftTex; def.RightTex = RightTex;
            def.FrontTex = FrontTex; def.BackTex = BackTex;            
            def.InventoryOrder = InventoryOrder;
            return def;
        }
        
        static ConfigElement[] elems;
        public static BlockDefinition[] Load(bool global, string mapName) {
            BlockDefinition[] defs = new BlockDefinition[Block.ExtendedCount];
            string path = global ? GlobalPath : "blockdefs/lvl_" + mapName + ".json";
            if (!File.Exists(path)) return defs;
            if (elems == null) elems = ConfigElement.GetAll(typeof(BlockDefinition));
            
            try {
                JsonContext ctx = new JsonContext();
                ctx.Val = File.ReadAllText(path);
                JsonArray array = (JsonArray)Json.ParseStream(ctx);
                if (array == null) return defs;
                
                foreach (object raw in array) {
                    JsonObject obj = (JsonObject)raw;
                    if (obj == null) continue;
                    
                    BlockDefinition def = new BlockDefinition();
                    obj.Deserialise(elems, def);
                    if (String.IsNullOrEmpty(def.Name)) continue;
                    
                    BlockID block = def.GetBlock();
                    if (block >= defs.Length) {
                        Logger.Log(LogType.Warning, "Invalid block ID: " + def.RawID);
                    } else {
                        defs[block] = def;
                    }
                }
            } catch (Exception ex) {
                Logger.LogError("Error Loading block defs from " + path, ex);
            }
            return defs;
        }
        
        public static void Save(bool global, Level lvl) {
            if (elems == null) elems = ConfigElement.GetAll(typeof(BlockDefinition));
            string path = global ? GlobalPath : "blockdefs/lvl_" + lvl.MapName + ".json";
            BlockDefinition[] defs = global ? GlobalDefs : lvl.CustomBlockDefs;
            
            using (StreamWriter w = new StreamWriter(path)) {
                w.WriteLine("[");
                SaveEntries(w, global, defs);
                w.WriteLine();
                w.WriteLine("]");
            }
        }
        
        static void SaveEntries(StreamWriter w, bool global, BlockDefinition[] defs) {
            bool first = true;
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                // don't want to save global blocks in the level's custom blocks list
                if (!global && def == GlobalDefs[i]) def = null;               
                if (def == null) continue;
                
                // need to add ',' from last element
                if (!first) w.WriteLine(", ");
                Json.Serialise(w, elems, def);
                first = false;
            }
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
                Logger.LogError("Error backing up global block defs", ex);
            }
            
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
                    lvl.UpdateCustomBlock((BlockID)b, GlobalDefs[b]);
                }
            }
        }
        
        
        public static void Add(BlockDefinition def, BlockDefinition[] defs, Level level) {
            BlockID block = def.GetBlock();
            bool global = defs == GlobalDefs;
            if (global) UpdateGlobalCustom(block, def);
            
            defs[block] = def;
            if (global) Block.SetDefaultNames();
            if (!global) level.UpdateCustomBlock(block, def);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (!pl.hasBlockDefs || def.RawID > pl.MaxRawBlock) continue;
                if (global && pl.level.CustomBlockDefs[block] != GlobalDefs[block]) continue;
                
                pl.Send(def.MakeDefinePacket(pl));
                pl.SendCurrentBlockPermissions();
            }
            Save(global, level);
        }
        
        public static void Remove(BlockDefinition def, BlockDefinition[] defs, Level level) {
            BlockID block = def.GetBlock();
            bool global = defs == GlobalDefs;
            if (global) UpdateGlobalCustom(block, null);
            
            defs[block] = null;
            if (global) Block.SetDefaultNames();
            if (!global) level.UpdateCustomBlock(block, null);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (!pl.hasBlockDefs || def.RawID > pl.MaxRawBlock) continue;
                if (global && pl.level.CustomBlockDefs[block] != null) continue;
                
                pl.Send(Packet.UndefineBlock(def, pl.hasExtBlocks));
            }
            Save(global, level);
        }
        
        public static void UpdateOrder(BlockDefinition def, bool global, Level level) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (!pl.Supports(CpeExt.InventoryOrder) || def.RawID > pl.MaxRawBlock) continue;
                SendLevelInventoryOrder(pl);
            }
        }
        
        static void UpdateGlobalCustom(BlockID block, BlockDefinition def) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                if (lvl.CustomBlockDefs[block] != GlobalDefs[block]) continue;
                lvl.UpdateCustomBlock(block, def);
            }
        }
        
        public void SetAllTex(ushort id) {
            SetSideTex(id);
            TopTex = id; BottomTex = id;
        }
        
        public void SetSideTex(ushort id) {
            LeftTex = id; RightTex = id; FrontTex = id; BackTex = id;
        }
        
        
        internal static void SendLevelCustomBlocks(Player pl) {
            BlockDefinition[] defs = pl.level.CustomBlockDefs;
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def == null || def.RawID > pl.MaxRawBlock) continue;
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
                if (def == null || def.RawID > pl.MaxRawBlock) continue;
                if (def.InventoryOrder == -1) continue;
                
                if (def.InventoryOrder != 0) {
                    if (order_to_blocks[def.InventoryOrder] != -1) continue;
                    order_to_blocks[def.InventoryOrder] = def.RawID;
                }
                block_to_orders[def.RawID] = def.InventoryOrder;
            }
            
            // Put blocks into their default slot if slot is unused
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                int raw = def != null ? def.RawID : i;
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
                int raw = def != null ? def.RawID : i;
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
                if (order == -1) order = 0;
                
                BlockDefinition def = defs[Block.FromRaw((BlockID)raw)];
                if (def == null && raw >= Block.CpeCount) continue;
                // Special case, don't want 255 getting hidden by default
                if (raw == 255 && def.InventoryOrder == -1) continue;
                
                pl.Send(Packet.SetInventoryOrder((BlockID)raw, (BlockID)order, pl.hasExtBlocks));
            }
        }
        
        public byte[] MakeDefinePacket(Player pl) {
            if (pl.Supports(CpeExt.BlockDefinitionsExt, 2) && Shape != 0) {
                return Packet.DefineBlockExt(this, true, pl.hasCP437, pl.hasExtBlocks, pl.hasExtTexs);
            } else if (pl.Supports(CpeExt.BlockDefinitionsExt) && Shape != 0) {
                return Packet.DefineBlockExt(this, false, pl.hasCP437, pl.hasExtBlocks, pl.hasExtTexs);
            } else {
                return Packet.DefineBlock(this, pl.hasCP437, pl.hasExtBlocks, pl.hasExtTexs);
            }
        }
        
        public static void UpdateFallback(bool global, BlockID block, Level level) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!global && pl.level != level) continue;
                if (pl.hasBlockDefs) continue;
                
                // if custom block is replacing core block, need to always reload for fallback
                if (block >= Block.CpeCount && !pl.level.MayHaveCustomBlocks) continue;
                
                LevelActions.ReloadFor(pl, pl, false);
            }
        }
    }
}
