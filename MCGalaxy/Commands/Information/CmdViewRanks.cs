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

namespace MCGalaxy.Commands.Info {
    public sealed class CmdViewRanks : Command2 {
        public override string name { get { return "ViewRanks"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Ops", "operator"), new CommandAlias("Admins", "superop"),
                    new CommandAlias("Banned", "banned"), new CommandAlias("BanList", "banned") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(2);
            if (message.Length == 0) { 
                p.Message("Available ranks: " + Group.GroupList.Join(g => g.ColoredName)); return; 
            }
            string modifer = args.Length > 1 ? args[1] : "";
            
            Group grp = message.CaselessEq("banned") ? Group.BannedRank : Matcher.FindRanks(p, args[0]);
            if (grp == null) return;

            List<string> list = grp.Players.All();
            if (list.Count == 0) {
                p.Message("No one has the rank of " + grp.ColoredName);
            } else {
                p.Message("People with the rank of " + grp.ColoredName + ":");
                MultiPageOutput.Output(p, list, (name) => name,
                                       "ViewRanks " + args[0], "players", modifer, false);
            }
        }
        
        public override void Help(Player p) {
            p.Message("%T/viewranks [rank] %H- Shows all players who have [rank]");
            p.Message("%T/viewranks banned %H- Shows all players who are banned");
            p.Message("Available ranks: " + Group.GroupList.Join(g => g.ColoredName));
        }
    }
}
