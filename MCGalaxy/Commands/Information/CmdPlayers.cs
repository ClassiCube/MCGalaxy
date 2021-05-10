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

namespace MCGalaxy.Commands.Info {
    public sealed class CmdPlayers : Command2 {
        public override string name { get { return "Players"; } }
        public override string shortcut { get { return "Who"; } }
        public override string type { get { return CommandTypes.Information; } }

        public override void Use(Player p, string message, CommandData data) {
            List<PlayerInfo.ListEntry> all = PlayerInfo.GetOnlineList(p, data.Rank);
            if (message.Length > 0) { ListOfRank(p, message, all); return; }
            
            int total = 0;
            foreach (PlayerInfo.ListEntry e in all) {
                total += e.players.Count;
            }
            p.Message("There are &a{0} &Splayer{1} online.", 
                      total, total != 0 ? "s" : "");
            
            for (int i = all.Count - 1; i >= 0; i--) {
                Output(all[i], p, Server.Config.ListEmptyRanks);
            }
        }
        
        static void ListOfRank(Player p, string name, List<PlayerInfo.ListEntry> all) {
            Group grp = Matcher.FindRanks(p, name);
            if (grp == null) return;
            PlayerInfo.ListEntry rank = all.Find(e => e.group == grp);
            
            if (rank == null || rank.players.Count == 0) {
                p.Message("There are no {0} &Sonline.", 
            	          rank.group.GetFormattedName());
            } else {
                Output(rank, p, false);
            }
            return;
        }
        
        static void Append(Player target, StringBuilder data, Player p, Group group) {
            data.Append(' ');
            if (p.voice) { data.Append("&f+").Append(group.Color); }
            data.Append(Colors.StripUsed(target.FormatNick(p)));
            
            if (p.hidden)       data.Append("-hidden");
            if (p.muted)        data.Append("-muted");
            if (p.frozen)       data.Append("-frozen");
            if (p.Game.Referee) data.Append("-ref");
            if (p.IsAfk)        data.Append("-afk");
            if (p.Unverified)   data.Append("-unverified");
            
            string lvlName = Colors.Strip(p.level.name); // for museums
            data.Append(" (").Append(lvlName).Append("),");
        }
        
        static void Output(PlayerInfo.ListEntry e, Player p, bool showWhenEmpty) {            
            if (e.players.Count == 0 && !showWhenEmpty) return;
            StringBuilder data = new StringBuilder();
            
            foreach (Player pl in e.players) {
                Append(p, data, pl, e.group);
            }
            
            // remove , from end
            data.Remove(data.Length - 1, 1);
            p.Message(":{0}:{1}", e.group.GetFormattedName(), data);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Players");
            p.Message("&HLists name and rank of all online players");
            p.Message("&T/Players [rank]");
            p.Message("&HLists all online players who have that rank");
        }
    }
}
