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
using MCGalaxy.Commands;
 
namespace MCGalaxy.Modules.Relay {
    public abstract class BotControllersCmd : Command2 {
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        protected void Use(Player p, string message, RelayBot bot, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] parts = message.SplitSpaces();
            string cmd = parts[0].ToLower();
            string arg = parts.Length > 1 ? parts[1] : "";
            
            switch (parts[0].ToLower()) {
                case "reload":
                    bot.Controllers = PlayerList.Load("ranks/IRC_Controllers.txt");
                    p.Message("{0} Controllers reloaded!", bot.RelayName);
                    break;
                    
                case "add":
                    if (arg.Length == 0) { p.Message("You need to provide a name to add."); return; }
                    
                    if (!bot.Controllers.Add(arg)) {
                        p.Message("{0} is already an {1} controller.", arg, bot.RelayName);
                    } else {
                        bot.Controllers.Save();
                        p.Message("{0} added to the IRC controller list.", arg, bot.RelayName);
                    }
                    break;
                    
                case "remove":
                    if (arg.Length == 0) { p.Message("You need to provide a name to remove."); return; }
                    
                    if (!bot.Controllers.Remove(arg)) {
                        p.Message("{0} is not an {1} controller.", arg, bot.RelayName);
                    } else {
                        bot.Controllers.Save();
                        p.Message("{0} removed from the {0} controller list.", arg, bot.RelayName);
                    }
                    break;
                    
                case "list":
                    bot.Controllers.OutputPlain(p, bot.RelayName + " controllers", 
                                                name + " list", arg);
                    break;
                    
                case "rank":
                    if (arg.Length == 0) {
                        p.Message("{0} controllers have the rank {1}", bot.RelayName,
                                  Group.GetColoredName(Server.Config.IRCControllerRank));
                        return;
                    }
                    
                    Group grp = Matcher.FindRanks(p, arg);
                    if (grp == null) return;
                    if (Server.Config.IRCControllerRank > data.Rank) {
                        p.Message("Cannot change the {0} controllers rank, " +
                    	          "as it is currently a rank higher than yours.", bot.RelayName); return;
                    }
                    if (grp.Permission > data.Rank) {
                        p.Message("Cannot set the {0} controllers rank to a rank higher than yours.", bot.RelayName); return;
                    }
                    
                    Server.Config.IRCControllerRank = grp.Permission;
                    SrvProperties.Save();
                    p.Message("Set {0} controller rank to {1}&S.", bot.RelayName, grp.ColoredName);
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
