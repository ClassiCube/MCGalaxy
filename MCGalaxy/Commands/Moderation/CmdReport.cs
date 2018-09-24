/*
 * Written By Jack1312

    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
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
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdReport : Command2 {
        public override string name { get { return "Report"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage reports") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("Reports", "list") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            if (!Directory.Exists("extra/reported"))
                Directory.CreateDirectory("extra/reported");

            string cmd = args[0];
            if (IsListCommand(cmd)) {
                HandleList(p, args, data);
            } else if (cmd.CaselessEq("clear")) {
                HandleClear(p, args, data);
            } else if (IsDeleteCommand(cmd)) {
                HandleDelete(p, args, data);
            } else if (IsInfoCommand(cmd)) {
                HandleCheck(p, args, data);
            } else {
                HandleAdd(p, args);
            }
        }
        
        static string[] GetReportedUsers() {
            string[] users = Directory.GetFiles("extra/reported", "*.txt");
            for (int i = 0; i < users.Length; i++) {
                users[i] = Path.GetFileNameWithoutExtension(users[i]);
            }
            return users;
        }
        
        static void DeleteReport(string user) {
            if (File.Exists("extra/reportedbackups/" + user + ".txt")) {
                File.Delete("extra/reportedbackups/" + user + ".txt");
            }            
            File.Move("extra/reported/" + user + ".txt", 
                      "extra/reportedbackups/" + user + ".txt");
        }
        
        void HandleList(Player p, string[] args, CommandData data) {
            if (!CheckExtraPerm(p, data, 1)) return;
            string[] users = GetReportedUsers();
            
            if (users.Length > 0) {
                p.Message("The following players have been reported:");
                string modifier = args.Length > 1 ? args[1] : "";
                MultiPageOutput.Output(p, users, pl => PlayerInfo.GetColoredName(p, pl),
                                       "Review list", "players", modifier, false);
                
                p.Message("Use %T/Report check [Player] %Sto view report details.");
                p.Message("Use %T/Report delete [Player] %Sto delete a report");
            } else {
                p.Message("No reports were found.");
            }
        }
        
        void HandleCheck(Player p, string[] args, CommandData data) {
            if (args.Length != 2) {
                p.Message("You need to provide a player's name."); return;
            }
            if (!CheckExtraPerm(p, data, 1)) return;
            string target = PlayerDB.MatchNames(p, args[1]);
            if (target == null) return;
            
            if (!File.Exists("extra/reported/" + target + ".txt")) {
                p.Message("The player you specified has not been reported."); return;
            }
            
            string[] reports = File.ReadAllLines("extra/reported/" + target + ".txt");
            p.MessageLines(reports);
        }
        
        void HandleDelete(Player p, string[] args, CommandData data) {
            if (args.Length != 2) {
                p.Message("You need to provide a player's name."); return;
            }
            if (!CheckExtraPerm(p, data, 1)) return;
            string target = PlayerDB.MatchNames(p, args[1]);
            if (target == null) return;
            
            if (!File.Exists("extra/reported/" + target + ".txt")) {
                p.Message("The player you specified has not been reported."); return;
            }
            if (!Directory.Exists("extra/reportedbackups"))
                Directory.CreateDirectory("extra/reportedbackups");
            
            DeleteReport(target);
            string targetName = PlayerInfo.GetColoredName(p, target);
            p.Message("Reports on {0} %Swere deleted.", targetName);
            Chat.MessageFromOps(p, "λNICK %Sdeleted reports on " + targetName);
            Logger.Log(LogType.UserActivity, "Reports on {1} were deleted by {0}", p.name, target);
        }
        
        void HandleClear(Player p, string[] args, CommandData data) {
            if (!CheckExtraPerm(p, data, 1)) return;
            if (!Directory.Exists("extra/reportedbackups"))
                Directory.CreateDirectory("extra/reportedbackups");
            
            string[] users = GetReportedUsers();
            foreach (string user in users) { DeleteReport(user); }
            
            p.Message("&aYou have cleared all reports!");
            Chat.MessageFromOps(p, "λNICK &ccleared ALL reports!");
            Logger.Log(LogType.UserActivity, p.name + " cleared ALL reports!");
        }
        
        void HandleAdd(Player p, string[] args) {
            if (args.Length != 2) {
                p.Message("You need to provide a reason for the report."); return;
            }
            string target = PlayerDB.MatchNames(p, args[0]);
            if (target == null) return;

            List<string> reports = new List<string>();
            if (File.Exists("extra/reported/" + target + ".txt")) {
                reports = Utils.ReadAllLinesList("extra/reported/" + target + ".txt");
            }
            
            ItemPerms checkPerms = CommandExtraPerms.Find(name, 1);
            if (reports.Count >= 5) {
                p.Message("{0} %Walready has 5 reports! Please wait until an {1} %Whas reviewed these reports first!",
                               PlayerInfo.GetColoredName(p, target), checkPerms.Describe());
                return;
            }
            
            string reason = args[1];
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;
            
            reports.Add(reason + " - Reported by " + p.name + " at " + DateTime.Now);
            File.WriteAllLines("extra/reported/" + target + ".txt", reports.ToArray());
            p.Message("&aReport sent! It should be viewed when a {0} &ais online", 
                           checkPerms.Describe());
            
            string opsMsg = "λNICK %Smade a report, view it with %T/Report check " + target;
            Chat.MessageFrom(ChatScope.Perms, p, opsMsg, checkPerms, null, true);
        }
        
        public override void Help(Player p) {
            p.Message("%T/Report list %H- Lists all reported players.");
            p.Message("%T/Report check [player] %H- Views reports for that player.");
            p.Message("%T/Report delete [player] %H- Deletes reports for that player.");
            p.Message("%T/Report clear %H- Clears &call%H reports.");
            p.Message("%T/Report [player] [reason] %H- Reports that player for the given reason.");
        }
    }
}
