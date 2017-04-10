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
using System.Collections.Generic;

namespace MCGalaxy.Commands {
    public sealed class CmdVIP : Command {
        public override string name { get { return "vip"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdVIP() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            
            if (args[0].CaselessEq("add")) {
                if (args.Length < 2) { Help(p); return; }
                AddVIP(p, args[1]);
            } else if (args[0].CaselessEq("remove")) {
                if (args.Length < 2) { Help(p); return; }
                RemoveVIP(p, args[1]);
            } else if (args[0].CaselessEq("list")) {
                ListVIPs(p, args);
            } else if (args.Length == 1) {
                AddVIP(p, args[0]);
            } else {
                Help(p);
            }
        }
        
        static void AddVIP(Player p, string name) {
            name = PlayerInfo.FindMatchesPreferOnline(p, name);
            if (name == null) return;
            
            if (Server.vip.Contains(name)) {
                Player.Message(p, PlayerInfo.GetColoredName(p, name) + " %Sis already a VIP.");
            } else {
                Server.vip.Add(name);
                Server.vip.Save(false);
                Player.Message(p, PlayerInfo.GetColoredName(p, name) + " %Sis now a VIP.");
                
                Player vip = PlayerInfo.FindExact(name);
                if (vip != null) Player.Message(vip, "You are now a VIP!");
            }
        }
        
        void RemoveVIP(Player p, string name) {
            name = PlayerInfo.FindMatchesPreferOnline(p, name);
            if (name == null) return;
            
            if (!Server.vip.Contains(name)) {
                Player.Message(p, PlayerInfo.GetColoredName(p, name) + " %Sis not a VIP.");
            } else {
                Server.vip.Remove(name);
                Server.vip.Save(false);
                Player.Message(p, PlayerInfo.GetColoredName(p, name) + " %Sis no longer a VIP.");
                
                Player vip = PlayerInfo.FindExact(name);
                if (vip != null) Player.Message(vip, "You are no longer a VIP!");
            }
        }
        
        static void ListVIPs(Player p, string[] args) {
            List<string> list = Server.vip.All();
            string modifier = args.Length > 1 ? args[1] : "";
            
            if (list.Count == 0) {
                Player.Message(p, "There are no VIPs.");
            } else {
                Player.Message(p, "VIPs:");
                MultiPageOutput.Output(p, list, 
                                       (name) => PlayerInfo.GetColoredName(p, name),
                                       "vip list", "players", modifier, false);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/vip add/remove [player]");
            Player.Message(p, "%HAdds or removes [player] from the VIP list.");
            Player.Message(p, "%T/vip list");
            Player.Message(p, "%HLists all players who are on the VIP list.");
            Player.Message(p, "%H  VIPs can join regardless of the player limit.");
        }
    }
}
