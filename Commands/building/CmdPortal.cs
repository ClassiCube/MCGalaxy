/*
    Copyright 2011 MCGalaxy
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    
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

            if (args.Length >= 2) {
                if (args[1].ToLower() == "multi") {
                    data.Multi = true;
                } else {
                    Player.SendMessage(p, "Invalid parameters"); return;
                }
            }

            if (block == "blue" || block == "") { data.type = Block.blue_portal; }
            else if (block == "orange") { data.type = Block.orange_portal; }
            else if (block == "air") { data.type = Block.air_portal; }
            else if (block == "water") { data.type = Block.water_portal; }
            else if (block == "lava") { data.type = Block.lava_portal; }
            else if (block == "show") { ShowPortals(p); return; }
            else { Help(p); return; }

            Player.SendMessage(p, "Place an &aEntry block" + Server.DefaultColor + " for the portal");
            p.ClearBlockchange();
            data.entries = new List<PortalPos>();
            p.blockchangeObject = data;
            p.Blockchange += new Player.BlockchangeEventHandler(EntryChange);
        }

        void EntryChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            p.ClearBlockchange();
            PortalData bp = (PortalData)p.blockchangeObject;

            if (bp.Multi && type == Block.red && bp.entries.Count > 0) { ExitChange(p, x, y, z, type, extType); return; }

            p.level.Blockchange(p, x, y, z, bp.type);
            p.SendBlockchange(x, y, z, Block.green);
            PortalPos Port;

            Port.mapName = p.level.name;
            Port.x = x; Port.y = y; Port.z = z;
            bp.entries.Add(Port);
            p.blockchangeObject = bp;

            if (!bp.Multi) {
                p.Blockchange += ExitChange;
                Player.SendMessage(p, "&aEntry block placed");
            } else {
                p.Blockchange += EntryChange;
                Player.SendMessage(p, "&aEntry block placed. &cRed block for exit");
            }
        }
        
        void ExitChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            PortalData bp = (PortalData)p.blockchangeObject;

            foreach (PortalPos pos in bp.entries) {
                //safe against SQL injections because no user input is given here
                DataTable Portals = Database.fillData("SELECT * FROM `Portals" + pos.mapName + "` WHERE EntryX=" + (int)pos.x + " AND EntryY=" + (int)pos.y + " AND EntryZ=" + (int)pos.z);
                Portals.Dispose();

                if (Portals.Rows.Count == 0)
                {//safe against SQL injections because no user input is given here
                    Database.executeQuery("INSERT INTO `Portals" + pos.mapName + "` (EntryX, EntryY, EntryZ, ExitMap, ExitX, ExitY, ExitZ) VALUES (" 
                                          + (int)pos.x + ", " + (int)pos.y + ", " + (int)pos.z + ", '" + p.level.name + "', " + (int)x + ", " + (int)y + ", " + (int)z + ")");
                }
                else
                {//safe against SQL injections because no user input is given here
                    Database.executeQuery("UPDATE `Portals" + pos.mapName + "` SET ExitMap='" + p.level.name + "', ExitX=" + (int)x + ", ExitY=" + (int)y + ", ExitZ=" + 
                                          (int)z + " WHERE EntryX=" + (int)pos.x + " AND EntryY=" + (int)pos.y + " AND EntryZ=" + (int)pos.z);
                }
                //DB

                if (pos.mapName == p.level.name) 
                    p.SendBlockchange(pos.x, pos.y, pos.z, bp.type);
            }

            Player.SendMessage(p, "&3Exit" + Server.DefaultColor + " block placed");
            if (!p.staticCommands)
                return;
            bp.entries.Clear(); 
            p.blockchangeObject = bp; 
            p.Blockchange += EntryChange;
        }

        struct PortalData { public List<PortalPos> entries; public byte type; public bool Multi; }
        struct PortalPos { public ushort x, y, z; public string mapName; }

        void ShowPortals(Player p) {
            p.showPortals = !p.showPortals;
            //safe against SQL injections because no user input is given here
            DataTable Portals = Database.fillData("SELECT * FROM `Portals" + p.level.name + "`");

            if (p.showPortals) {
                for (int i = 0; i < Portals.Rows.Count; i++) {
                    DataRow row = Portals.Rows[i];
                    Server.s.Log( row["EntryX"].GetType().ToString() );
                    if (row["ExitMap"].ToString() == p.level.name)
                        p.SendBlockchange(U16(row["ExitX"]), U16(row["ExitY"]), U16(row["ExitZ"]), Block.red);
                    p.SendBlockchange(U16(row["EntryX"]), U16(row["EntryY"]), U16(row["EntryZ"]), Block.green);
                }

                Player.SendMessage(p, "Now showing &a" + Portals.Rows.Count + Server.DefaultColor + " portals.");
            } else {
                for (int i = 0; i < Portals.Rows.Count; i++) {
                    DataRow row = Portals.Rows[i];
                    if (Portals.Rows[i]["ExitMap"].ToString() == p.level.name)
                        p.RevertBlock(U16(row["ExitX"]), U16(row["ExitY"]), U16(row["ExitZ"]));
                    p.RevertBlock(U16(row["EntryX"]), U16(row["EntryY"]), U16(row["EntryZ"]));
                }
                Player.SendMessage(p, "Now hiding portals.");
            }
            Portals.Dispose();
        }
        
        static ushort U16(object x) { return Convert.ToUInt16(x); }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/portal [orange/blue/air/water/lava] [multi] - Activates Portal mode.");
            Player.SendMessage(p, "/portal [type] multi - Place Entry blocks until exit is wanted.");
            Player.SendMessage(p, "/portal show - Shows portals, green = in, red = out.");
        }
    }
}
