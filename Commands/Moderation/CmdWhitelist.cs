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
            int sep = message.IndexOf(' ');
            string action = sep >= 0 ? message.Substring(0, sep) : message;
            string player = sep >= 0 ? message.Substring(sep + 1) : "";
            
            if (action.CaselessEq("list")) {
                string names = Server.whiteList.All().Concatenate(", ");
                Player.Message(p, "Whitelist: &f" + names); return;
            }
            if (player == "") { Help(p); return; }
            
            if (action.CaselessEq("add")) {
                if (Server.whiteList.Contains(player)) {
                    Player.Message(p, "&f" + player + " %Sis already on the whitelist!"); return;
                }
                
                Server.whiteList.Add(player);
                Chat.GlobalMessageOps(p.ColoredName + " %Sadded &f" + player + " %Sto the whitelist.");
                Server.whiteList.Save();
                Server.s.Log("WHITELIST: Added " + player);
            } else if (action.CaselessEq("del")) {
                if (!Server.whiteList.Contains(player)) {
                    Player.Message(p, "&f" + player + " %Sis not on the whitelist!"); return;
                }
                
                Server.whiteList.Remove(player);
                Chat.GlobalMessageOps(p.ColoredName + " %Sremoved &f" + player + " %Sfrom the whitelist.");
                Server.whiteList.Save();
                Server.s.Log("WHITELIST: Removed " + player);
            } else {
                Help(p);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/whitelist <add/del> [player]");
            Player.Message(p, "%HAdds or removes [player] from the whitelist.");
            Player.Message(p, "%T/whitelist list");
            Player.Message(p, "%HLists all players who are on the whitelist.");
        }
    }
}
