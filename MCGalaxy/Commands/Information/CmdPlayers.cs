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
    public sealed class CmdPlayers : Command {
        public override string name { get { return "Players"; } }
        public override string shortcut { get { return "Who"; } }
        public override string type { get { return CommandTypes.Information; } }

        public override void Use(Player p, string message) {
            if (message.Length > 0) {
                Group grp = Matcher.FindRanks(p, message);
                if (grp == null) return;
                string title = ":" + grp.Color + GetPlural(grp.Name) + ":";
                Section rankSec = MakeSection(grp, title);
                
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.group != grp) continue;
                    if (p == pl || Entities.CanSee(p, pl)) {
                        string name = Colors.Strip(pl.DisplayName);
                        AddStates(pl, ref name);
                        rankSec.Append(pl, name);
                    }
                }
                
                if (rankSec.Empty) {
                    Player.Message(p, "There are no players of that rank online.");
                } else {
                    rankSec.Print(p, false);
                }
                return;
            }
            
            List<Section> playerList = new List<Section>();
            foreach (Group grp in Group.GroupList) {
                string title = ":" + grp.Color + GetPlural(grp.Name) + ":";
                playerList.Add(MakeSection(grp, title));
            }

            int totalPlayers = 0;
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                if (p == pl || Entities.CanSee(p, pl)) {
                    string name = Colors.Strip(pl.DisplayName);
                    AddStates(pl, ref name);
                    totalPlayers++;
                    playerList.Find(grp => grp.group == pl.group).Append(pl, name);
                }
            }
            
            if (totalPlayers == 1) {
                Player.Message(p, "There is &a1 %Splayer online.");
            } else {
                Player.Message(p, "There are &a" + totalPlayers + " %Splayers online.");
            }
            
            for (int i = playerList.Count - 1; i >= 0; i--) {
                playerList[i].Print(p, ServerConfig.ListEmptyRanks);
            }
        }
        
        static void AddStates(Player pl, ref string name) {
            if (pl.hidden) name += "-hidden";
            if (pl.muted) name += "-muted";
            if (pl.frozen) name += "-frozen";
            if (pl.Game.Referee) name += "-ref";
            if (pl.IsAfk) name += "-afk";
        }
        
        struct Section {
            public Group group;
            public StringBuilder builder;
            public string title;
            
            public bool Empty { get { return builder.Length == 0; } }
            
            public void Print(Player p, bool showEmpty) {
                if (builder.Length == 0 && !showEmpty) return;
                
                if (builder.Length > 0)
                    builder.Remove(builder.Length - 1, 1);
                string message = title + builder.ToString();
                Player.Message(p, message);
            }
            
            public void Append(Player pl, string name) {
                builder.Append(' ');
                if (pl.voice) {
                    builder.Append("&f+").Append(group.Color);
                }
                
                builder.Append(name);
                string lvlName = Colors.Strip(pl.level.name); // for museums
                builder.Append(" (").Append(lvlName).Append("),");
            }
        }
        
        static Section MakeSection(Group group, string title) {
            Section sec;
            sec.group = group;
            sec.builder = new StringBuilder();
            sec.title = title;
            return sec;
        }
        
        static string GetPlural(string groupName) {
            if (groupName.Length < 2)
                return groupName;
            string last2 = groupName.Substring(groupName.Length - 2).ToLower();
            if ((last2 != "ed" || groupName.Length <= 3) && last2[1] != 's')
                return groupName + "s";
            return groupName;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Players");
            Player.Message(p, "%HLists name and rank of all online players");
            Player.Message(p, "%T/Players [rank]");
            Player.Message(p, "%HLists all online players who have that rank");
        }
    }
}
