/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System;
using System.Collections.Generic;
using System.Text;

namespace MCGalaxy.Commands {
    public sealed class CmdViewRanks : Command {
        public override string name { get { return "viewranks"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ops", "operator"), new CommandAlias("admins", "superop"),
                    new CommandAlias("banned", "banned"), new CommandAlias("banlist", "banned") }; }
        }
        public CmdViewRanks() { }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(2);
            if (message == "") { 
                Player.Message(p, "Available ranks: " + Group.concatList()); return; 
            }
            string modifer = args.Length > 1 ? args[1] : "";
            
            Group grp = message.CaselessEq("banned") ? Group.BannedRank : Matcher.FindRanks(p, args[0]);
            if (grp == null) return;

            List<string> list = grp.playerList.All();
            if (list.Count == 0) {
                Player.Message(p, "No one has the rank of " + grp.ColoredName);
            } else {
                Player.Message(p, "People with the rank of " + grp.ColoredName + ":");
                MultiPageOutput.Output(p, list, (name, i) => name,
                                       "viewranks " + args[0], "players", modifer, false);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/viewranks [rank] - Shows all players who have [rank]");
            Player.Message(p, "/viewranks banned - Shows all players who are banned.");
            Player.Message(p, "Available ranks: " + Group.concatList());
        }
    }
}
