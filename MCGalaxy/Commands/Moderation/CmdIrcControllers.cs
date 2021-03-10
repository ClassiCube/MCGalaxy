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
    public sealed class CmdIrcControllers : Command2 {
        public override string name { get { return "IRCControllers"; } }
        public override string shortcut { get { return "IRCCtrl"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] parts = message.SplitSpaces();
            
            switch (parts[0].ToLower()) {
                case "reload":
                    Server.ircControllers = PlayerList.Load("ranks/IRC_Controllers.txt");
                    p.Message("IRC Controllers reloaded!");
                    break;
                    
                case "add":
                    if (parts.Length < 2) { p.Message("You need to provide a name to add."); return; }
                    
                    if (!Server.ircControllers.Add(parts[1])) {
                        p.Message(parts[1] + " is already an IRC controller.");
                    } else {
                        Server.ircControllers.Save();
                        p.Message(parts[1] + " added to the IRC controller list.");
                    }
                    break;
                    
                case "remove":
                    if (parts.Length < 2) { p.Message("You need to provide a name to remove."); return; }
                    
                    if (!Server.ircControllers.Remove(parts[1])) {
                        p.Message(parts[1] + " is not an IRC controller.");
                    } else {
                        Server.ircControllers.Save();
                        p.Message(parts[1] + " removed from the IRC controller list.");
                    }
                    break;
                    
                case "list":
                    string modifier = parts.Length > 1 ? parts[1] : "";
                    Server.ircControllers.OutputPlain(p, "IRC controllers", "IRCControllers list", modifier);
                    break;
                    
                case "rank":
                    if (parts.Length < 2) {
                        p.Message("IRC controllers have the rank {0}",
                                  Group.GetColoredName(Server.Config.IRCControllerRank));
                        return;
                    }
                    
                    Group grp = Matcher.FindRanks(p, parts[1]);
                    if (grp == null) return;
                    if (Server.Config.IRCControllerRank > data.Rank) {
                        p.Message("Cannot change the IRC controllers rank, as it is currently a rank higher than yours."); return;
                    }
                    if (grp.Permission > data.Rank) {
                        p.Message("Cannot set the IRC controllers rank to a rank higher than yours."); return;
                    }
                    
                    Server.Config.IRCControllerRank = grp.Permission;
                    SrvProperties.Save();
                    p.Message("Set IRC controller rank to {0}&S.", grp.ColoredName);
                    break;
                    
                default:
                    Help(p); break;
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/IRCControllers add/remove [name]");
            p.Message("&HAdds or removes [name] from list of IRC controllers");
            p.Message("&T/IRCControllers reload/list");
            p.Message("&HReloads or outputs list of IRC controllers");
            p.Message("&T/IRCControllers rank [rank]");
            p.Message("&HSets which rank IRC controllers are treated as having");
        }
    }
}
