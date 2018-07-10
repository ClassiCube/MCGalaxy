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

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdVIP : Command2 {
        public override string name { get { return "VIP"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();
            
            if (args[0].CaselessEq("add")) {
                if (args.Length < 2) { Help(p); return; }
                AddVIP(p, args[1]);
            } else if (IsDeleteCommand(args[0])) {
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
                p.Message(PlayerInfo.GetColoredName(p, name) + " %Sis already a VIP.");
            } else {
                Server.vip.Add(name);
                Server.vip.Save(false);
                p.Message(PlayerInfo.GetColoredName(p, name) + " %Sis now a VIP.");
                
                Player vip = PlayerInfo.FindExact(name);
                if (vip != null) vip.Message("You are now a VIP!");
            }
        }
        
        void RemoveVIP(Player p, string name) {
            name = PlayerInfo.FindMatchesPreferOnline(p, name);
            if (name == null) return;
            
            if (!Server.vip.Contains(name)) {
                p.Message(PlayerInfo.GetColoredName(p, name) + " %Sis not a VIP.");
            } else {
                Server.vip.Remove(name);
                Server.vip.Save(false);
                p.Message(PlayerInfo.GetColoredName(p, name) + " %Sis no longer a VIP.");
                
                Player vip = PlayerInfo.FindExact(name);
                if (vip != null) vip.Message("You are no longer a VIP!");
            }
        }
        
        static void ListVIPs(Player p, string[] args) {
            List<string> list = Server.vip.All();
            string modifier = args.Length > 1 ? args[1] : "";
            
            if (list.Count == 0) {
                p.Message("There are no VIPs.");
            } else {
                p.Message("VIPs:");
                MultiPageOutput.Output(p, list, 
                                       (name) => PlayerInfo.GetColoredName(p, name),
                                       "VIP list", "players", modifier, false);
            }
        }

        public override void Help(Player p) {
            p.Message("%T/VIP add/remove [player]");
            p.Message("%HAdds or removes [player] from the VIP list.");
            p.Message("%T/VIP list");
            p.Message("%HLists all players who are on the VIP list.");
            p.Message("%H  VIPs can join regardless of the player limit.");
        }
    }
}
