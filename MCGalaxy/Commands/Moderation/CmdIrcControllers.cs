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
using System.Collections.Generic;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdIrcControllers : Command {
        public override string name { get { return "IRCControllers"; } }
        public override string shortcut { get { return "IRCCtrl"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            string[] parts = message.SplitSpaces();
            
            switch (parts[0].ToLower()) {
                case "reload":
                    Server.ircControllers = PlayerList.Load("IRC_Controllers.txt");
                    Player.Message(p, "IRC Controllers reloaded!");
                    break;
                    
                case "add":
                    if (parts.Length < 2) { Player.Message(p, "You need to provide a name to add."); return; }
                    if (Server.ircControllers.Contains(parts[1])) {
                        Player.Message(p, parts[1] + " is already an IRC controller."); return;
                    }
                    
                    Server.ircControllers.Add(parts[1]);
                    Server.ircControllers.Save();
                    Player.Message(p, parts[1] + " added to the IRC controller list.");
                    break;
                    
                case "remove":
                    if (parts.Length < 2) { Player.Message(p, "You need to provide a name to remove."); return; }
                    if (!Server.ircControllers.Contains(parts[1])) {
                        Player.Message(p, parts[1] + " is not an IRC controller."); return;
                    }
                    
                    Server.ircControllers.Remove(parts[1]);
                    Server.ircControllers.Save();
                    Player.Message(p, parts[1] + " removed from the IRC controller list.");
                    break;
                    
                case "list":
                    string names = Server.ircControllers.All().Join();
                    Player.Message(p, "IRC controllers list:");
                    Player.Message(p, names);
                    break;
                    
                case "rank":
                    if (parts.Length < 2) { 
                        Player.Message(p, "IRC controllers have the rank {0}", 
                                       Group.GetColoredName(ServerConfig.IRCControllerRank));
                        return;
                    }
                    
                    Group grp = Matcher.FindRanks(p, parts[1]);
                    if (grp == null) return;                  
                    if (p != null && ServerConfig.IRCControllerRank > p.Rank) {
                        Player.Message(p, "Cannot change the IRC controllers rank, as it is currently a rank higher than yours."); return;
                    }
                    if (p != null && grp.Permission > p.Rank) {
                        Player.Message(p, "Cannot set the IRC controllers rank to a rank higher than yours."); return;
                    }
                    
                    ServerConfig.IRCControllerRank = grp.Permission;
                    SrvProperties.Save();
                    Player.Message(p, "Set IRC controller rank to {0}%S.", grp.ColoredName);
                    break;
                    
                default:
                    Help(p); break;
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/IRCControllers add/remove [name]");
            Player.Message(p, "%HAdds or removes [name] from list of IRC controllers");
            Player.Message(p, "%T/IRCControllers reload/list");
            Player.Message(p, "%HReloads or outputs list of IRC controllers");
            Player.Message(p, "%T/IRCControllers rank [rank]");
            Player.Message(p, "%HSets which rank IRC controllers are treated as having in-game.");
        }
    }
}
