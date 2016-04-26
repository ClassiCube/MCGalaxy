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
using System.IO;
using System.Text;

namespace MCGalaxy.Commands {
    
    public sealed class CmdRankReqs : Command {
        
        public override string name { get { return "rankreqs"; } }
        public override string shortcut { get { return "reqs"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "Lowest rank able to set rank requirements."),
                }; }
        }
        static char[] trimChars = new[] { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") {
                if (p == null) {
                    Player.SendMessage(p, "Console must provide a rank name."); return;
                }
                
                Group next = null;
                int index = Group.GroupList.IndexOf(p.group);
                if (index < Group.GroupList.Count - 1) {
                    Group nextHigher = Group.GroupList[index + 1];
                    if (nextHigher.Permission < LevelPermission.Nobody)
                        next = nextHigher;
                }
                
                if (next == null) {
                    Player.SendMessage(p, "You are already the highest rank.");
                } else {
                    ShowRequirements(p, next);
                }
            } else if (message.CaselessStarts("set ")) {
                string[] args = message.Split(trimChars, 3);
                Group grp = Group.Find(args[1]);
                if (grp == null) { Player.SendMessage(p, "Could not find group"); return; }
                
                string path = "text/rankreqs/" + grp.name + ".txt";
                if (args.Length == 2) {
                    File.Delete(path);
                    Player.SendMessage(p, "Deleted rank requirements for " + grp.ColoredName + "%S.");
                } else {
                    string[] lines = args[2].Split(new [] { "\\n" },
                                                   StringSplitOptions.RemoveEmptyEntries);
                    CP437Writer.WriteAllLines(path, lines);
                    Player.SendMessage(p, "Updated rank requirements for " + grp.ColoredName + "%S.");
                }
            } else {
                Group grp = Group.Find(message);
                if (grp == null) { Player.SendMessage(p, "Could not find group"); return; }
                ShowRequirements(p, grp);
            }
        }
        
        void ShowRequirements(Player p, Group grp) {
            string path = "text/rankreqs/" + grp.name + ".txt";
            if (!File.Exists(path)) {
                Player.SendMessage(p, "No rank requirements have been specified yet."); return;
            }
            
            List<string> lines = CP437Reader.ReadAllLines(path);
            Player.SendMessage(p, "Rank requirements for " + grp.ColoredName + "%S:");
            foreach (string line in lines)
                Player.SendMessage(p, line);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/rankreqs [rank]");
            Player.SendMessage(p, "%HOutputs the requirements to earn that rank. " +
                               "If no rank name is given, the rank above yours is used.");
            Player.SendMessage(p, "%T/rankreqs set [rank]");
            Player.SendMessage(p, "%HSets the requirements needed to earn that rank. " +
                               "Note a \"\\n\" causes the following text to start on the next line.");
        }
    }
}
