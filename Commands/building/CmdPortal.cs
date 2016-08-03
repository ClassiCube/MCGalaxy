/*
    Copyright 2011 MCForge
        
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

            if (args.Length >= 2) {
                if (args[1].CaselessEq("multi")) {
                    data.Multi = true;
                } else {
                    Player.Message(p, "Invalid parameters"); return;
                }
            }

            if (block == "blue" || block == "") { data.type = Block.blue_portal; }
            else if (block == "orange") { data.type = Block.orange_portal; }
            else if (block == "air") { data.type = Block.air_portal; }
            else if (block == "water") { data.type = Block.water_portal; }
            else if (block == "lava") { data.type = Block.lava_portal; }
            else if (block == "show") { ShowPortals(p); return; }
            else { Help(p); return; }

            Player.Message(p, "Place an &aEntry block %Sfor the portal");
            p.ClearBlockchange();
            data.entries = new List<PortalPos>();
            p.blockchangeObject = data;
            p.Blockchange += EntryChange;
        }

        void EntryChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {          
            PortalData bp = (PortalData)p.blockchangeObject;            
            if (!p.level.CheckAffectPermissions(p, x, y, z, type, extType)) {
                p.RevertBlock(x, y, z); return;
            }
            p.ClearBlockchange();

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
                Player.Message(p, "&aEntry block placed");
            } else {
                p.Blockchange += EntryChange;
                Player.Message(p, "&aEntry block placed. &cRed block for exit");
            }
        }
        
        void ExitChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            PortalData bp = (PortalData)p.blockchangeObject;

            foreach (PortalPos pos in bp.entries) {
                //safe against SQL injections because no user input is given here
                DataTable Portals = Database.Fill("SELECT * FROM `Portals" + pos.mapName + 
                                                  "` WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", pos.x, pos.y, pos.z);
                Portals.Dispose();
                
                string syntax = Portals.Rows.Count == 0 ?
                    "INSERT INTO `Portals" + pos.mapName + "` (EntryX, EntryY, EntryZ, ExitX, ExitY, ExitZ, ExitMap) VALUES (@0, @1, @2, @3, @4, @5, @6)"
                    : "UPDATE `Portals" + pos.mapName + "` SET ExitMap=@6, ExitX=@3, ExitY=@4, ExitZ=@5 WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2";
                Database.Execute(syntax, pos.x, pos.y, pos.z, x, y, z, p.level.name);

                if (pos.mapName == p.level.name) 
                    p.SendBlockchange(pos.x, pos.y, pos.z, bp.type);
            }

            Player.Message(p, "&3Exit %Sblock placed");
            if (!p.staticCommands) return;
            bp.entries.Clear(); 
            p.blockchangeObject = bp; 
            p.Blockchange += EntryChange;
        }

        struct PortalData { public List<PortalPos> entries; public byte type; public bool Multi; }
        struct PortalPos { public ushort x, y, z; public string mapName; }

        void ShowPortals(Player p) {
            p.showPortals = !p.showPortals;
            //safe against SQL injections because no user input is given here
            DataTable Portals = Database.Fill("SELECT * FROM `Portals" + p.level.name + "`");

            if (p.showPortals) {
                foreach (DataRow row in Portals.Rows) {
                    if (row["ExitMap"].ToString() == p.level.name)
                        p.SendBlockchange(U16(row["ExitX"]), U16(row["ExitY"]), U16(row["ExitZ"]), Block.red);
                    p.SendBlockchange(U16(row["EntryX"]), U16(row["EntryY"]), U16(row["EntryZ"]), Block.green);
                }

                Player.Message(p, "Now showing &a" + Portals.Rows.Count + " %Sportals.");
            } else {
                foreach (DataRow row in Portals.Rows) {
                    if (row["ExitMap"].ToString() == p.level.name)
                        p.RevertBlock(U16(row["ExitX"]), U16(row["ExitY"]), U16(row["ExitZ"]));
                    p.RevertBlock(U16(row["EntryX"]), U16(row["EntryY"]), U16(row["EntryZ"]));
                }
                Player.Message(p, "Now hiding portals.");
            }
            Portals.Dispose();
        }
        
        static ushort U16(object x) { return Convert.ToUInt16(x); }
        
        public override void Help(Player p) {
            Player.Message(p, "/portal [orange/blue/air/water/lava] [multi] - Activates Portal mode.");
            Player.Message(p, "/portal [type] multi - Place Entry blocks until exit is wanted.");
            Player.Message(p, "/portal show - Shows portals, green = in, red = out.");
        }
    }
}
