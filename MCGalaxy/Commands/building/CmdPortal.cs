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

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPortal : Command {
        public override string name { get { return "portal"; } }
        public override string shortcut { get { return "o"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdPortal() { }

        public override void Use(Player p, string message) {
            PortalData data;
            data.Multi = false;
            string[] args = message.Split(' ');
            string block = message == "" ? "" : args[0].ToLower();

            if (args.Length >= 2 && args[1].CaselessEq("multi")) {
                data.Multi = true;
            } else if (args.Length >= 2) {
                Help(p); return;
            }

            data.Block = GetBlock(p, block, out data.ExtBlock);
            if (data.Block == Block.Invalid) return;

            Player.Message(p, "Place an &aEntry block %Sfor the portal");
            p.ClearBlockchange();
            data.Entries = new List<PortalPos>();
            p.blockchangeObject = data;
            p.Blockchange += EntryChange;
        }
        
        byte GetBlock(Player p, string name, out byte extBlock) {
            extBlock = 0;
            byte id = Block.Byte(name);
            if (Block.Props[id].IsPortal) return id;
            if (name == "show") { ShowPortals(p); return Block.Invalid; }

            id = BlockDefinition.GetBlock(name, p);
            if (p.level.CustomBlockProps[id].IsPortal) {
                extBlock = id; return Block.custom_block;
            }
            
            // Hardcoded aliases for backwards compatibility
            id = Block.Invalid;
            if (name == "") id = Block.blue_portal;
            if (name == "blue") id = Block.blue_portal;        
            if (name == "orange") id = Block.orange_portal;
            if (name == "air") id = Block.air_portal;
            if (name == "water") id = Block.water_portal;
            if (name == "lava") id = Block.lava_portal;
            
            if (!Block.Props[id].IsPortal) { Help(p); return Block.Invalid; }
            return id;
        }

        void EntryChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            PortalData data = (PortalData)p.blockchangeObject;
            byte old = p.level.GetTile(x, y, z);
            if (!p.level.CheckAffectPermissions(p, x, y, z, old, data.Block, data.ExtBlock)) {
                p.RevertBlock(x, y, z); return;
            }
            p.ClearBlockchange();

            if (data.Multi && type == Block.red && data.Entries.Count > 0) { ExitChange(p, x, y, z, type, extType); return; }

            p.level.Blockchange(p, x, y, z, data.Block, data.ExtBlock);
            p.SendBlockchange(x, y, z, Block.green);
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
                Player.Message(p, "&aEntry block placed. &cRed block for exit");
            }
        }
        
        void ExitChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            PortalData bp = (PortalData)p.blockchangeObject;

            foreach (PortalPos P in bp.Entries) {
                //safe against SQL injections because no user input is given here
                string lvlName = P.Map;
                object locker = ThreadSafeCache.DBCache.Get(lvlName);
                
                lock (locker) {
                    Database.Backend.CreateTable("Portals" + lvlName, LevelDB.createPortals);

                    int count = 0;
                    using (DataTable portals = Database.Backend.GetRows("Portals" + lvlName, "*",
                                                                        "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", P.x, P.y, P.z)) {
                        count = portals.Rows.Count;
                    }
                    
                    string syntax = count == 0 ?
                        "INSERT INTO `Portals" + lvlName + "` (EntryX, EntryY, EntryZ, ExitX, ExitY, ExitZ, ExitMap) VALUES (@0, @1, @2, @3, @4, @5, @6)"
                        : "UPDATE `Portals" + lvlName + "` SET ExitMap=@6, ExitX=@3, ExitY=@4, ExitZ=@5 WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2";
                    Database.Execute(syntax, P.x, P.y, P.z, x, y, z, p.level.name);
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
                    p.SendBlockchange(U16(row["ExitX"]), U16(row["ExitY"]), U16(row["ExitZ"]), Block.red);
                p.SendBlockchange(U16(row["EntryX"]), U16(row["EntryY"]), U16(row["EntryZ"]), Block.green);
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
        
        static bool Check(BlockProps props, byte id, string name) {
            if (props.BlockId != id) return false;
            if (props.Name == "unknown") return false; // custom blocks
            id = Block.Byte(name);
            return !Block.Props[id].IsPortal;
        }
                
        public override void Help(Player p) {
            Player.Message(p, "%T/portal [block]");
            Player.Message(p, "%HPlace a block for the entry, then another block for exit.");
            Player.Message(p, "%T/portal [block] multi");
            Player.Message(p, "%HPlace multiple blocks for entries, then a red block for exit.");
            
            var allProps = Block.Props;
            Player.Message(p, "%H  Supported blocks: %S{0}",
                           allProps.Join(props => Format(props)));
            Player.Message(p, "%T/portal show %H- Shows portals (green = entry, red = exit)");
        }
    }
}
