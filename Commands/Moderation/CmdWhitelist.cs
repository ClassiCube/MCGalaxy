/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
using System.Collections.Generic;

namespace MCGalaxy.Commands {
    public sealed class CmdWhitelist : Command {
        public override string name { get { return "whitelist"; } }
        public override string shortcut { get { return "w"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdWhitelist() { }

        public override void Use(Player p, string message) {
            if (!Server.useWhitelist) { Player.Message(p, "Whitelist is not enabled."); return; }
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');

            if (args[0].CaselessEq("add")) {
                if (args.Length < 2) { Help(p); return; }
                Add(p, args[1]);
            } else if (args[0].CaselessEq("del") || args[0].CaselessEq("remove")) {
                if (args.Length < 2) { Help(p); return; }
                Remove(p, args[1]);
            } else if (args[0].CaselessEq("list")) {
                List(p, args);
            } else if (args.Length == 1) {
                Add(p, args[0]);
            } else {
                Help(p);
            }
        }
        
        static void Add(Player p, string player) {
            if (Server.whiteList.Contains(player)) {
                Player.Message(p, player + " %Sis already on the whitelist!"); return;
            }
            
            Server.whiteList.Add(player);
            string src = p == null ? "(console)" : p.ColoredName;
            Chat.MessageOps(src + " %Sadded &f" + player + " %Sto the whitelist.");
            Server.whiteList.Save();
            Server.s.Log("WHITELIST: Added " + player);
        }
        
        static void Remove(Player p, string player) {
            if (!Server.whiteList.Contains(player)) {
                Player.Message(p, player + " %Sis not on the whitelist!"); return;
            }
            
            Server.whiteList.Remove(player);
            string src = p == null ? "(console)" : p.ColoredName;
            Chat.MessageOps(src + " %Sremoved &f" + player + " %Sfrom the whitelist.");
            Server.whiteList.Save();
            Server.s.Log("WHITELIST: Removed " + player);
        }
        
        static void List(Player p, string[] args) {
            List<string> list = Server.whiteList.All();
            string modifier = args.Length > 1 ? args[1] : "";
            
            if (list.Count == 0) {
                Player.Message(p, "There are no whitelisted players.");
            } else {
                Player.Message(p, "Whitelisted players:");
                MultiPageOutput.Output(p, list, 
                                       (name, i) => PlayerInfo.GetColoredName(p, name),
                                       "whitelist list", "players", modifier, false);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/whitelist add/del [player]");
            Player.Message(p, "%HAdds or removes [player] from the whitelist.");
            Player.Message(p, "%T/whitelist list");
            Player.Message(p, "%HLists all players who are on the whitelist.");
        }
    }
}
