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
using System.Collections.Generic;
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
            string target = PlayerInfo.FindMatchesPreferOnline(p, args[0]);
            if (target == null) return;
            Player who = PlayerInfo.FindExact(target);

            Group group = Matcher.FindRanks(p, args[1]);
            if (group == null) return;
            TimeSpan delta;
            if (!args[2].TryParseShort(p, 'h', "temp rank for", out delta)) return;

            foreach (string line in Server.TempRanks.Find(target)) {
                Player.Message(p, "&cThe player already has a temporary rank assigned!"); return;
            }
            
            if (p != null && who != null && p == who) {
                Player.Message(p, "&cYou cannot assign yourself a temporary rank."); return;
            }
            Group pGroup = who != null ? who.group : Group.findPlayerGroup(target);
            if (p != null && pGroup.Permission >= p.Rank) {
                Player.Message(p, "Cannot change the temporary rank of someone equal or higher to yourself."); return;
            }
            if (p != null && group.Permission >= p.Rank) {
                Player.Message(p, "Cannot change the temporary rank to a higher rank than yourself."); return;
            }
            AssignTempRank(p, who, delta, pGroup, group, target);
        }
        
        static void AssignTempRank(Player p, Player who, TimeSpan delta,
                                   Group pGroup, Group group, string target) {
            DateTime now = DateTime.Now;
            string assigner = p == null ? "(console)" : p.name;
            int hours = delta.Days * 24 + delta.Hours;
            
            string data = target + " " + group.name + " " + pGroup.name + " " + hours + " " + now.Minute + " " +
                now.Hour + " " + now.Day + " " + now.Month + " " + now.Year + " " + assigner + " " + delta.Minutes;
            Server.TempRanks.Append(data);
            
            Command.all.Find("setrank").Use(null, target + " " + group.name + " assigning temp rank");
            Player.Message(p, "Temp ranked {0} to {1} %Sfor {2}", target, group.ColoredName, delta.Shorten());
            if (who != null)
                Player.Message(who, "You have been temp ranked to {0} %Sfor {1}", group.ColoredName, delta.Shorten());
        }
        
        static void Delete(Player p, string name) {
            bool assigned = false;
            StringBuilder all = new StringBuilder();
            Player who = PlayerInfo.FindExact(name);
            
            foreach (string line in File.ReadAllLines(Paths.TempRanksFile)) {
                if (!line.CaselessStarts(name)) { all.AppendLine(line); continue; }
                
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
            File.WriteAllText(Paths.TempRanksFile, all.ToString());
        }
        
        static void Info(Player p, string name) {            
            List<string> rankings = Server.TempRanks.FindMatches(p, name, "temp rank");
            if (rankings == null) return;
            
            foreach (string line in rankings) {
                PrintTempRankInfo(p, line); return;
            }
        }        
        
        static void List(Player p) {
            int count = 0;
            foreach (string line in File.ReadAllLines(Paths.TempRanksFile)) {
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
            string tempRanker = args[9];
            string tempRank = Group.GetColoredName(args[1]);
            string oldRank = Group.GetColoredName(args[2]);
            
            int min = int.Parse(args[4]), hour = int.Parse(args[5]);
            int day = int.Parse(args[6]), month = int.Parse(args[7]), year = int.Parse(args[8]);
            int periodH = int.Parse(args[3]), periodM = 0;
            if (args.Length > 10) periodM = int.Parse(args[10]);
            
            DateTime assigned = new DateTime(year, month, day, hour, min, 0);
            DateTime expiry = assigned.AddHours(periodH).AddMinutes(periodM);
            TimeSpan delta = DateTime.Now - assigned;
            TimeSpan expireDelta = expiry - DateTime.Now;
            
            Player.Message(p, "Temp rank information for {0}:", PlayerInfo.GetColoredName(p, args[0]));
            Player.Message(p, "  From {0} %Sto {1}%S, by {2} &a{3} %Sago, expires in &a{4}",
                           oldRank, tempRank, tempRanker,
                           delta.Shorten(), expireDelta.Shorten());
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/temprank [player] [rank] [timespan]");
            Player.Message(p, "%HSets a temporary rank for the specified player.");
            Player.Message(p, "%H e.g. to temprank for 90 minutes, [timespan] would be %S1h30m");
            Player.Message(p, "%T/temprank [player] info");
            Player.Message(p, "%HLists information about the temp rank for the given player.");
            Player.Message(p, "%T/temprank [player] delete %H- Removes player's temp rank.");
            Player.Message(p, "%T/temprank list %H- Lists all current temp ranks.");
        }
    }
}
