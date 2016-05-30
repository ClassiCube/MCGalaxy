/*
Copyright 2011 MCForge
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
using System.IO;
using System.Text;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdTempRank : Command {
        public override string name { get { return "temprank"; } }
        public override string shortcut { get { return "tr"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }        
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("deltemprank", null, "delete"),
                    new CommandAlias("dtr", null, "delete"), new CommandAlias("temprankinfo", null, "info"),
                    new CommandAlias("trl", null, "list"), new CommandAlias("tempranklist", null, "list") }; }
        }
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        
        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length >= 3) {
                Assign(p, args);
            } else if (args.Length == 1) {
                if (args[0].CaselessEq("list")) {
                    List(p);
                } else {
                    Help(p);
                }
            } else if (args[1].CaselessEq("delete") || args[1].CaselessEq("remove")) {
                Delete(p, args[0]);
            } else if (args[1].CaselessEq("information") || args[1].CaselessEq("info")) {
                Info(p, args[0]);
            } else {
                Help(p);
            }
        }
        
        static void Assign(Player p, string[] args) {
            string player = args[0], rank = args[1], period = args[2];
            Player who = PlayerInfo.Find(player);
            if (who == null) {
                player = PlayerInfo.FindOfflineName(player);
                if (player == null) { Player.Message(p, "&cPlayer &a" + args[0] + "&c not found."); return; }
            } else {
                player = who.name;
            }
            
            Group group = Group.FindOrShowMatches(p, rank);
            if (group == null) return;
            int periodTime;
            if (!Int32.TryParse(period, out periodTime)) {
                Player.Message(p, "&cThe period needs to be a number."); return;
            }

            foreach (string line in Server.TempRanks.Find(player)) {
                Player.Message(p, "&cThe player already has a temporary rank assigned!"); return;
            }
            
            if (p != null && who != null && p == who) {
                Player.Message(p, "&cYou cannot assign yourself a temporary rank."); return;
            }
            Group pGroup = who != null ? who.group : Group.findPlayerGroup(player);
            if (p != null && pGroup.Permission >= p.group.Permission) {
                Player.Message(p, "Cannot change the temporary rank of someone equal or higher to yourself."); return;
            }
            if (p != null && group.Permission >= p.group.Permission) {
                Player.Message(p, "Cannot change the temporary rank to a higher rank than yourself."); return;
            }

            DateTime now = DateTime.Now;
            string assigner = p == null ? "Console" : p.name;
            string data = player + " " + rank + " " + pGroup.name + " " + period + " " + now.Minute + " " +
                now.Hour + " " + now.Day + " " + now.Month + " " + now.Year + " " + assigner;
            Server.TempRanks.Append(data);
            
            Command.all.Find("setrank").Use(null, player + " " + group.name + " assigning temp rank");
            Player.Message(p, "Temp ranked {0} to {1}%S for {2} hours", player, group.ColoredName, period);
            if (who != null)
                Player.Message(who, "You have been temp ranked to {0}%S for {1} hours", group.ColoredName, period);
        }
        
        static void Delete(Player p, string name) {
            bool assigned = false;
            StringBuilder all = new StringBuilder();
            Player who = PlayerInfo.Find(name);
            
            foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                if (!line.StartsWith(name, comp)) { all.AppendLine(line); continue; }
                
                string[] parts = line.Split(' ');
                Group newgroup = Group.Find(parts[2]);
                Command.all.Find("setrank").Use(null, name + " " + newgroup.name + " temp rank unassigned");
                Player.Message(p, "&eTemp rank of &a{0}&e has been unassigned", name);
                if (who != null)
                    Player.Message(who, "&eYour temp rank has been unassigned");
                assigned = true;
            }
            
            if (!assigned) {
                Player.Message(p, "&a{0}&c has not been assigned a temp rank.", name); return;
            }
            File.WriteAllText("text/tempranks.txt", all.ToString());
        }
        
        static void Info(Player p, string name) {
            foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                if (!line.StartsWith(name, comp)) continue;
                PrintTempRankInfo(p, line); return;
            }
            Player.Message(p, "&cPlayer &a{0}&chas not been assigned a temporary rank.", name);
        }        
        
        static void List(Player p) {
            int count = 0;
            foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                if (count == 0)
                    Player.Message(p, "&ePlayers with a temporary rank assigned:");
                PrintTempRankInfo(p, line);
                count++;
            }
            if (count == 0)
                Player.Message(p, "&cThere are no players with a temporary rank assigned.");
        }        
        
        static void PrintTempRankInfo(Player p, string line) {
            string[] args = line.Split(' ');
            string temprank = args[1], oldrank = args[2], tempranker = args[9];
            int minutes = Convert.ToInt32(args[4]), hours = Convert.ToInt32(args[5]);
            int days = Convert.ToInt32(args[6]), months = Convert.ToInt32(args[7]);
            int years = Convert.ToInt32(args[8]);
            
            int period = Convert.ToInt32(args[3]);
            Group oldGrp = Group.Find(oldrank), tempGrp = Group.Find(temprank);
            string oldCol = oldGrp == null ? "" : oldGrp.color;
            string tempCol = tempGrp == null ? "" : tempGrp.color;
            
            DateTime assignmentDate = new DateTime(years, months, days, hours, minutes, 0);
            DateTime expireDate = assignmentDate.AddHours(Convert.ToDouble(period));
            Player.Message(p, "Temp rank information for {0}:", args[0]);
            Player.Message(p, "  From {0} %Sto {1}%S, by {2} on &a{3}%S, expires on &a{4}",
                           oldCol + oldrank, tempCol + temprank, tempranker,
                           assignmentDate, expireDate);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/temprank <player> <rank> <period(hours)>");
            Player.Message(p, "%HSets a temporary rank for the specified player.");
            Player.Message(p, "%T/temprank <player> info");
            Player.Message(p, "%HLists information about the temp rank for the given player.");
            Player.Message(p, "%T/temprank <player> delete %H- Removes player's temp rank.");
            Player.Message(p, "%T/temprank list %H- Lists all current temp ranks.");
        }
    }
}
