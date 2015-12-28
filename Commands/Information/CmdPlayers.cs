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

namespace MCGalaxy.Commands
{
    public sealed class CmdPlayers : Command
    {
        public override string name { get { return "players"; } }
        public override string shortcut { get { return "who"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdPlayers() { }

        public override void Use(Player p, string message) {
            DisplayPlayers(p, message, text => Player.SendMessage(p, text));
        }
        
        public static void DisplayPlayers(Player p, string message, Action<string> output) {
            if (message != "") {
                Group grp = Group.Find(message);
                if( grp == null ) {
                    output("There is no rank with that name"); return;
                }
                string title = ":" + grp.color + GetPlural(grp.trueName) + ":";
                Section rankSec = MakeSection(grp, title);                
                
                foreach (Player pl in Player.players) {                    
                    if (pl.group != grp) continue;
                    if (!pl.hidden || p == null || p.group.Permission > LevelPermission.Operator) {
                        string name = Player.StripColours(pl.DisplayName);
                        if (Server.afkset.Contains(pl.name)) name += "-afk";
                        if (pl.muted) name += "[muted]";
                        rankSec.Append(pl, name);
                    }
                } 
                
                if (rankSec.Empty) {
                    output( "There are no players of that rank online.");
                } else {
                    rankSec.Print(output, false);
                }
                return;
            }
            
            List<Section> playerList = new List<Section>();
            foreach (Group grp in Group.GroupList) {
                if (grp.name == "nobody")
                    continue;
                string title = ":" + grp.color + GetPlural(grp.trueName) + ":";
                playerList.Add(MakeSection(grp, title));
            }

            Section devSec = MakeSection("#%9MCGalaxy Devs:" + Server.DefaultColor);
            Section modsSec = MakeSection("#%2MCGalaxy Mods:" + Server.DefaultColor);
            Section gcModsSec = MakeSection("#%6MCGalaxy GCMods:" + Server.DefaultColor);
            int totalPlayers = 0;
            
            foreach (Player pl in Player.players) {
                if (!pl.hidden || p == null || p.group.Permission > LevelPermission.Operator) {
                    totalPlayers++;
                    string name = Player.StripColours(pl.DisplayName);

                    if (Server.afkset.Contains(pl.name)) name += "-afk";
                    if (pl.muted) name += "[muted]";

                    if (pl.isDev) devSec.Append(pl, name);
                    if (pl.isMod) modsSec.Append(pl, name);
                    if (pl.isGCMod) gcModsSec.Append(pl, name);
                    playerList.Find(grp => grp.group == pl.group).Append(pl, name);
                }
            }

            output("There are %a" + totalPlayers + Server.DefaultColor + " players online.");
            bool hasStaff = !devSec.Empty || !modsSec.Empty || !gcModsSec.Empty;
            if (hasStaff)
                output("%cMCGalaxy Staff Online:");
            devSec.Print(output, false);
            modsSec.Print(output, false);
            gcModsSec.Print(output, false);
            
            if (hasStaff)
                output( "%aNormal Players Online:");
            for (int i = playerList.Count - 1; i >= 0; i--)
                playerList[i].Print(output, Server.showEmptyRanks);
        }
        
        struct Section {
            public Group group;
            public StringBuilder builder;
            public string title;
            
            public bool Empty { get { return builder.Length == 0; } }
            
            public void Print(Action<string> output, bool showEmpty) {
                if (builder.Length == 0 && !showEmpty)
                    return;
                
                if (builder.Length > 0)
                    builder.Remove(builder.Length - 2, 2);
                string message = title + builder.ToString();
                output(message);
            }
            
            public void Append(Player pl, string name) {
                builder.Append(' ');
                if (pl.voice)
                    builder.Append("&f+" + Server.DefaultColor);
                
                builder.Append(name);
                builder.Append(" (" + pl.level.name + "), ");
            }
        }
        
        static Section MakeSection(string title) { return MakeSection(Group.standard, title); }
        
        static Section MakeSection(Group group, string title) {
            Section sec;
            sec.group = group;
            sec.builder = new StringBuilder();
            sec.title = title;
            return sec;
        }
        
        static void AppendName(Player pl, string foundName, ref string target) {
            if (pl.voice)
                target += " " + "&f+" + Server.DefaultColor + foundName + " (" + pl.level.name + "),";
            else
                target += " " + foundName + " (" + pl.level.name + "),";
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
            Player.SendMessage(p, "/players [rank] - Shows name and general rank of all players");
        }
    }
}
