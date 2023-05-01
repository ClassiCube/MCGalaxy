/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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

namespace MCGalaxy.Commands.Info 
{
    public sealed class CmdViewRanks : Command2 
    {
        public override string name { get { return "ViewRanks"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Ops", "@80"), new CommandAlias("Admins", "@100"),
                    new CommandAlias("Banned", "banned"), new CommandAlias("BanList", "banned") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(2);
            if (message.Length == 0) { 
                p.Message("Available ranks: " + Group.GroupList.Join(g => g.ColoredName)); return; 
            }
            string rankName = args[0];
            Group grp;

            if (rankName.CaselessEq("banned")) {
                grp = Group.BannedRank;
            } else if (!rankName.StartsWith("@")) {
                grp = Matcher.FindRanks(p, rankName);
            } else {
                // /viewranks @[permission level]
                int perm = 0;
                rankName = rankName.Substring(1);
                if (!CommandParser.GetInt(p, rankName, "Permission level", ref perm)) return;

                grp = Group.Find((LevelPermission)perm);
                if (grp == null) p.Message("&WThere is no rank with permission level \"{0}\"", rankName);
            }
            
            if (grp == null) return;
            string modifier = args.Length > 1 ? args[1] : "";
            grp.Players.OutputPlain(p, "players ranked " + grp.ColoredName,
                                    "ViewRanks " + grp.Name, modifier);
        }
        
        public override void Help(Player p) {
            p.Message("&T/viewranks [rank] &H- Shows all players who have [rank]");
            p.Message("&T/viewranks banned &H- Shows all players who are banned");
            p.Message("Available ranks: " + Group.GroupList.Join(g => g.ColoredName));
        }
    }
}
