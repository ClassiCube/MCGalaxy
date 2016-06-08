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

namespace MCGalaxy.Commands {
    public sealed class CmdIrcControllers : Command {        
        public override string name { get { return "irccontrollers"; } }
        public override string shortcut { get { return "ircctrl"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdIrcControllers() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] parts = message.Split(' ');
            
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
                    string names = Server.ircControllers.All().Concatenate(", ");
                    Player.Message(p, "IRC controllers list:");
                    Player.Message(p, names);
                    break;
            }            
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/ircctrl <reload/add/remove/list> [name]");
        }
    }
}
