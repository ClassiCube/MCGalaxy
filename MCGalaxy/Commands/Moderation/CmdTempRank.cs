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
using MCGalaxy.Events;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdTempRank : Command {
        public override string name { get { return "TempRank"; } }
        public override string shortcut { get { return "tr"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("DelTempRank", null, "delete"),
                    new CommandAlias("dtr", null, "delete"), new CommandAlias("TempRankInfo", null, "info"),
                    new CommandAlias("trl", null, "list"), new CommandAlias("TempRankList", null, "list") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(4);
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

            Group newRank = Matcher.FindRanks(p, args[1]);
            if (newRank == null) return;
            TimeSpan duration = TimeSpan.Zero;
            if (!CommandParser.GetTimespan(p, args[2], ref duration, "temp rank for", "h")) return;

            if (Server.tempRanks.Contains(target)) {
                Player.Message(p, "&cThe player already has a temporary rank assigned!"); return;
            }
            
            if (p != null && who != null && p == who) {
                Player.Message(p, "&cYou cannot assign yourself a temporary rank."); return;
            }
            Group curRank = who != null ? who.group : Group.GroupIn(target);
            string reason = args.Length > 3 ? args[3] : "assigning temp rank";
            if (!CmdSetRank.CanChangeRank(target, curRank, newRank, who, p, ref reason)) return;
            
            ModAction action = new ModAction(target, p, ModActionType.Rank, reason, duration);
            action.targetGroup = curRank;
            action.Metadata = newRank;
            OnModActionEvent.Call(action);
        }
        
        static void Delete(Player p, string target) {
            string line = Server.tempRanks.FindData(target);
            if (line == null) {
                Player.Message(p, "{0}&c has not been assigned a temp rank.",
                               PlayerInfo.GetColoredName(p, target));
                return;
            }
            
            string[] parts = line.SplitSpaces();
            Player who = PlayerInfo.FindExact(target);
            Group curRank = who != null ? who.group : Group.GroupIn(target);
            
            Group oldRank = Group.Find(parts[4 - 1]); // -1 because data, not whole line
            if (oldRank == null) return;
            
            string reason = "temp rank unassigned";
            if (!CmdSetRank.CanChangeRank(target, curRank, oldRank, who, p, ref reason)) return;
            
            ModAction action = new ModAction(target, p, ModActionType.Rank, reason);
            action.Metadata = oldRank;
            action.targetGroup = curRank;
            OnModActionEvent.Call(action);
        }
        
        static void Info(Player p, string target) {
            string line = Server.tempRanks.FindData(target);
            if (line == null) {
                Player.Message(p, "{0}&c has not been assigned a temp rank.",
                               PlayerInfo.GetColoredName(p, target));
            } else {
                PrintTempRankInfo(p, line);
            }
        }
        
        static void List(Player p) {
            List<string> lines = Server.tempRanks.AllLines();
            if (lines.Count == 0) {
                Player.Message(p, "&cThere are no players with a temporary rank assigned.");
            } else {
                Player.Message(p, "&ePlayers with a temporary rank assigned:");
                foreach (string line in lines) {
                    PrintTempRankInfo(p, line);
                }
            }
        }
        
        static void PrintTempRankInfo(Player p, string line) {
            string[] args = line.SplitSpaces();
            if (args.Length < 4) return;
            
            string assigner = args[1];
            DateTime assigned = long.Parse(args[2]).FromUnixTime();
            DateTime expiry   = long.Parse(args[3]).FromUnixTime();
            string oldRank    = Group.GetColoredName(args[4]);
            string tempRank   = Group.GetColoredName(args[5]);
            
            TimeSpan assignDelta = DateTime.UtcNow - assigned;
            TimeSpan expireDelta = expiry - DateTime.UtcNow;
            Player.Message(p, "Temp rank information for {0}:",
                           PlayerInfo.GetColoredName(p, args[0]));
            Player.Message(p, "  From {0} %Sto {1}%S, by {2} &a{3} %Sago, expires in &a{4}",
                           oldRank, tempRank, assigner,
                           assignDelta.Shorten(), expireDelta.Shorten());
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/TempRank [player] [rank] [timespan] <reason>");
            Player.Message(p, "%HSets a temporary rank for the specified player.");
            Player.Message(p, "%T/TempRank [player] info");
            Player.Message(p, "%HLists information about the temp rank for the given player.");
            Player.Message(p, "%T/TempRank [player] delete %H- Removes player's temp rank.");
            Player.Message(p, "%T/TempRank list %H- Lists all current temp ranks.");
        }
    }
}
