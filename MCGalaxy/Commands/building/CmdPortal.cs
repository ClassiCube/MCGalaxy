/*
    Copyright 2011 MCForge
        
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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.Blocks;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPortal : Command {
        public override string name { get { return "portal"; } }
        public override string shortcut { get { return "o"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdPortal() { }

        public override void Use(Player p, string message)
        {
            PortalData data;
            data.Multi = false;
            string[] args = message.SplitSpaces();
            string block = message == "" ? "" : args[0].ToLower();

            if (args.Length >= 2 && args[1].CaselessEq("multi")) {
                data.Multi = true;
            } else if (args.Length >= 2) {
                Help(p); return;
            }

            data.Block = GetBlock(p, block, out data.ExtBlock);
            if (data.Block == Block.Invalid) return;
            if (!CommandParser.IsBlockAllowed(p, "place a portal of ", data.Block)) return;

            Player.Message(p, "Place an &aEntry block %Sfor the portal");
            p.ClearBlockchange();
            data.Entries = new List<PortalPos>();
            p.blockchangeObject = data;
            p.Blockchange += EntryChange;
        }
        
        byte GetBlock(Player p, string name, out byte extBlock) {
            extBlock = 0;
            byte block = Block.Byte(name);
            if (Block.Props[block].IsPortal) return block;
            if (name == "show") { ShowPortals(p); return Block.Invalid; }

            block = BlockDefinition.GetBlock(name, p);
            if (p.level.CustomBlockProps[block].IsPortal) {
                extBlock = block; return Block.custom_block;
            }
            
            // Hardcoded aliases for backwards compatibility
            block = Block.Invalid;
            if (name == "") block = Block.blue_portal;
            if (name == "blue") block = Block.blue_portal;
            if (name == "orange") block = Block.orange_portal;
            if (name == "air") block = Block.air_portal;
            if (name == "water") block = Block.water_portal;
            if (name == "lava") block = Block.lava_portal;
            
            if (!Block.Props[block].IsPortal) { Help(p); return Block.Invalid; }
            return block;
        }

        void EntryChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            PortalData data = (PortalData)p.blockchangeObject;
            byte old = p.level.GetTile(x, y, z);
            if (!p.level.CheckAffectPermissions(p, x, y, z, old, data.Block, data.ExtBlock)) {
                p.RevertBlock(x, y, z); return;
            }
            p.ClearBlockchange();

            if (data.Multi && type == Block.red && data.Entries.Count > 0) { ExitChange(p, x, y, z, type, extType); return; }

            p.level.UpdateBlock(p, x, y, z, data.Block, data.ExtBlock);
            p.SendBlockchange(x, y, z, Block.green, 0);
            PortalPos Port;

            Port.Map = p.level.name;
            Port.x = x; Port.y = y; Port.z = z;
            data.Entries.Add(Port);
            p.blockchangeObject = data;

            if (!data.Multi) {
                p.Blockchange += ExitChange;
                Player.Message(p, "&aEntry block placed");
            } else {
                p.Blockchange += EntryChange;
                Player.Message(p, "&aEntry block placed. &c{0} block for exit", Block.Name(Block.red));
            }
        }
        
        void ExitChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            PortalData bp = (PortalData)p.blockchangeObject;
            string dstMap = p.level.name.UnicodeToCp437();

            foreach (PortalPos P in bp.Entries) {
                string lvlName = P.Map;
                object locker = ThreadSafeCache.DBCache.Get(lvlName);
                
                lock (locker) {
                    Database.Backend.CreateTable("Portals" + lvlName, LevelDB.createPortals);
                    Level map = LevelInfo.FindExact(P.Map);
                    if (map != null) map.hasPortals = true;

                    int count = 0;
                    using (DataTable portals = Database.Backend.GetRows("Portals" + lvlName, "*",
                                                                        "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", P.x, P.y, P.z)) {
                        count = portals.Rows.Count;
                    }
                    
                    if (count == 0) {
                        Database.Backend.AddRow("Portals" + lvlName, "EntryX, EntryY, EntryZ, ExitX, ExitY, ExitZ, ExitMap",
                                                P.x, P.y, P.z, x, y, z, dstMap);
                    } else {
                        Database.Backend.UpdateRows("Portals" + lvlName, "ExitMap=@6, ExitX=@3, ExitY=@4, ExitZ=@5",
                                                    "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", P.x, P.y, P.z, x, y, z, dstMap);
                    }
                }
                if (P.Map == p.level.name)
                    p.SendBlockchange(P.x, P.y, P.z, bp.Block, bp.ExtBlock);
            }

            Player.Message(p, "&3Exit %Sblock placed");
            if (!p.staticCommands) return;
            bp.Entries.Clear();
            p.blockchangeObject = bp;
            p.Blockchange += EntryChange;
        }

        struct PortalData { public List<PortalPos> Entries; public byte Block, ExtBlock; public bool Multi; }
        struct PortalPos { public ushort x, y, z; public string Map; }

        
        void ShowPortals(Player p) {
            p.showPortals = !p.showPortals;
            using (DataTable table = Database.Backend.GetRows("Portals" + p.level.name, "*")) {
                if (p.showPortals) {
                    ShowPortals(p, table);
                } else {
                    HidePortals(p, table);
                }
            }
        }
        
        static void ShowPortals(Player p, DataTable table) {
            foreach (DataRow row in table.Rows) {
                if (row["ExitMap"].ToString() == p.level.name)
                    p.SendBlockchange(U16(row["ExitX"]), U16(row["ExitY"]), U16(row["ExitZ"]), Block.red, 0);
                p.SendBlockchange(U16(row["EntryX"]), U16(row["EntryY"]), U16(row["EntryZ"]), Block.green, 0);
            }

            Player.Message(p, "Now showing &a" + table.Rows.Count + " %Sportals.");
        }
        
        static void HidePortals(Player p, DataTable table) {
            foreach (DataRow row in table.Rows) {
                if (row["ExitMap"].ToString() == p.level.name)
                    p.RevertBlock(U16(row["ExitX"]), U16(row["ExitY"]), U16(row["ExitZ"]));
                p.RevertBlock(U16(row["EntryX"]), U16(row["EntryY"]), U16(row["EntryZ"]));
            }
            Player.Message(p, "Now hiding portals.");
        }
        
        static ushort U16(object x) { return Convert.ToUInt16(x); }
        
        
        static string Format(BlockProps props) {
            if (!props.IsPortal) return null;
            
            // We want to use the simple aliases if possible
            if (Check(props, Block.orange_portal, "orange")) return "orange";
            if (Check(props, Block.blue_portal, "blue")) return "blue";
            if (Check(props, Block.air_portal, "air")) return "air";
            if (Check(props, Block.lava_portal, "lava")) return "lava";
            if (Check(props, Block.water_portal, "water")) return "water";
            return props.Name;
        }
        
        static bool Check(BlockProps props, byte block, string name) {
            if (props.BlockId != block) return false;
            if (props.Name == "unknown") return false; // custom blocks
            
            block = Block.Byte(name);
            return !Block.Props[block].IsPortal;
        }
        
        static string FormatCustom(Level lvl, BlockProps props) {
            if (!props.IsPortal) return null;
            BlockDefinition def = lvl.CustomBlockDefs[props.BlockId];
            return def == null ? null : def.Name.Replace(" ", "");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/portal [block]");
            Player.Message(p, "%HPlace a block for the entry, then another block for exit.");
            Player.Message(p, "%T/portal [block] multi");
            Player.Message(p, "%HPlace multiple blocks for entries, then a red block for exit.");
            
            string blocks = Block.Props.Join(props => Format(props));
            if (!Player.IsSuper(p)) {
                string custom = p.level.CustomBlockProps.Join(props => FormatCustom(p.level, props));
                if (blocks != "" && custom != "")
                    blocks = blocks + ", " + custom;
            }

            Player.Message(p, "%H  Supported blocks: %S{0}", blocks);
            Player.Message(p, "%T/portal show %H- Shows portals (green = entry, red = exit)");
        }
    }
}
