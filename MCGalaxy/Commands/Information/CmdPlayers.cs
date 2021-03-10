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
            int totalPlayers = 0;
            if (message.Length > 0) {
                Group grp = Matcher.FindRanks(p, message);
                if (grp == null) return;
                
                GroupPlayers rankPlayers = Make(p, data, grp, ref totalPlayers);
                if (totalPlayers == 0) {
                    p.Message("There are no players of that rank online.");
                } else {
                    Output(rankPlayers, p, false);
                }
                return;
            }
            
            List<GroupPlayers> allPlayers = new List<GroupPlayers>();
            foreach (Group grp in Group.GroupList) {
                allPlayers.Add(Make(p, data, grp, ref totalPlayers));
            }
            
            if (totalPlayers == 1) {
                p.Message("There is &a1 &Splayer online.");
            } else {
                p.Message("There are &a" + totalPlayers + " &Splayers online.");
            }
            
            for (int i = allPlayers.Count - 1; i >= 0; i--) {
                Output(allPlayers[i], p, Server.Config.ListEmptyRanks);
            }
        }
        
        struct GroupPlayers { public Group group; public StringBuilder builder; }      
        static GroupPlayers Make(Player p, CommandData data, Group group, ref int totalPlayers) {
            GroupPlayers list;
            list.group = group;
            list.builder = new StringBuilder();
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                if (pl.group != group || !p.CanSee(pl, data.Rank)) continue;
                
                totalPlayers++;
                Append(p, list, pl);
            }
            return list;
        }
        
        static void Append(Player target, GroupPlayers list, Player p) {
            StringBuilder data = list.builder;
            data.Append(' ');
            if (p.voice) { data.Append("&f+").Append(list.group.Color); }
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
        
        static string GetPlural(string name) {
            if (name.Length < 2) return name;
            
            string last2 = name.Substring(name.Length - 2).ToLower();
            if ((last2 != "ed" || name.Length <= 3) && last2[1] != 's')
                return name + "s";
            return name;
        }
        
        static void Output(GroupPlayers list, Player p, bool showWhenEmpty) {
            StringBuilder data = list.builder;
            if (data.Length == 0 && !showWhenEmpty) return;
            if (data.Length > 0) data.Remove(data.Length - 1, 1);
            
            string title = ":" + list.group.Color + GetPlural(list.group.Name) + ":";
            p.Message(title + data.ToString());
        }
        
        public override void Help(Player p) {
            p.Message("&T/Players");
            p.Message("&HLists name and rank of all online players");
            p.Message("&T/Players [rank]");
            p.Message("&HLists all online players who have that rank");
        }
    }
}
