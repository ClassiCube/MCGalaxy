/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Commands {
    public sealed class CmdWaypoint : Command {
        public override string name { get { return "waypoint"; } }
        public override string shortcut { get { return "wp"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdWaypoint() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            string[] args = message.ToLower().SplitSpaces();
            string cmd = args[0];
            if (cmd == "") { Help(p); return; }
            
            if (args.Length == 1 && cmd == "list") {
                Player.Message(p, "Waypoints:");
                foreach (Warp wp in p.Waypoints.Items) {
                    if (LevelInfo.FindExact(wp.lvlname) != null)
                        Player.Message(p, wp.name + " : " + wp.lvlname);
                }
                return;
            } else if (args.Length == 1) {
                if (!p.Waypoints.Exists(cmd)) { Player.Message(p, "That waypoint does not exist"); return; }
                p.Waypoints.Goto(cmd, p);
                return;
            }
            
            string name = args[1];
            if (cmd == "create" || cmd == "new" || cmd == "add") {
                if (p.Waypoints.Exists(name)) { Player.Message(p, "That waypoint already exists"); return; }
                p.Waypoints.Create(name, p);
                Player.Message(p, "Created waypoint");
            } else if (cmd == "goto") {
                if (!p.Waypoints.Exists(name)) { Player.Message(p, "That waypoint does not exist"); return; }
                p.Waypoints.Goto(name, p);
            } else if (cmd == "replace" || cmd == "update" || cmd == "edit") {
                if (!p.Waypoints.Exists(name)) { Player.Message(p, "That waypoint does not exist"); return; }
                p.Waypoints.Update(name, p);
                Player.Message(p, "Updated waypoint");
            } else if (cmd == "delete" || cmd == "remove") {
                if (!p.Waypoints.Exists(name)) { Player.Message(p, "That waypoint does not exist"); return; }
                p.Waypoints.Remove(name, p);
                Player.Message(p, "Deleted waypoint");
            } else {
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/waypoint create [name] %H- Create a new waypoint");
            Player.Message(p, "%T/waypoint update [name] %H- Update a waypoint");
            Player.Message(p, "%T/waypoint remove [name] %H- Remove a waypoint");
            Player.Message(p, "%T/waypoint list %H- Shows a list of waypoints");
            Player.Message(p, "%T/waypoint goto [name] %H- Goto a waypoint");
            Player.Message(p, "%T/waypoint [name] %H- Goto a waypoint");
        }
    }
}
