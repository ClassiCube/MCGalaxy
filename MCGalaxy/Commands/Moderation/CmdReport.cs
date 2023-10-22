/*
 * Written By Jack1312

    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
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
using MCGalaxy.Events;

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
            if (IsListAction(cmd)) {
                HandleList(p, args, data);
            } else if (cmd.CaselessEq("clear")) {
                HandleClear(p, args, data);
            } else if (IsDeleteAction(cmd)) {
                HandleDelete(p, args, data);
            } else if (IsInfoAction(cmd)) {
                HandleCheck(p, args, data);
            } else {
                HandleAdd(p, args);
            }
        }
        
        void HandleList(Player p, string[] args, CommandData data) {
            if (!CheckExtraPerm(p, data, 1)) return;
            string[] users = GetReportedUsers();
            
            if (users.Length > 0) {
                p.Message("The following players have been reported:");
                string modifier = args.Length > 1 ? args[1] : "";
                Paginator.Output(p, users, pl => p.FormatNick(pl),
                                 "Review list", "players", modifier);
                
                p.Message("Use &T/Report check [player] &Sto view report details.");
                p.Message("Use &T/Report delete [player] &Sto delete a report");
            } else {
                p.Message("No players have been reported currently.");
            }
        }
        
        void HandleCheck(Player p, string[] args, CommandData data) {
            if (args.Length != 2) {
                p.Message("You need to provide a player's name."); return;
            }
            if (!CheckExtraPerm(p, data, 1)) return;
            
            string target = PlayerDB.MatchNames(p, args[1]);
            if (target == null) return;
            string nick = p.FormatNick(target);
            
            if (!HasReports(target)) {
                p.Message("{0} &Shas not been reported.", nick); return;
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
            string nick = p.FormatNick(target);
            
            if (!HasReports(target)) {
                p.Message("{0} &Shas not been reported.", nick); return;
            }
            if (!Directory.Exists("extra/reportedbackups"))
                Directory.CreateDirectory("extra/reportedbackups");
            
            DeleteReport(target);
            p.Message("Reports on {0} &Swere deleted.", nick);
            Chat.MessageFromOps(p, "λNICK &Sdeleted reports on " + nick);
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
            string nick = p.FormatNick(target);

            List<string> reports = new List<string>();
            if (HasReports(target)) {
                reports = Utils.ReadAllLinesList(ReportPath(target));
            }
            ItemPerms checkPerms = CommandExtraPerms.Find(name, 1);
            
            if (reports.Count >= 5) {
                p.Message("{0} &Walready has 5 reports! Please wait until an {1} &Whas reviewed these reports first!",
                          nick, CommandExtraPerms.Find(name, 1).Describe());
                return;
            }
            
            string reason = ModActionCmd.ExpandReason(p, args[1]);
            if (reason == null) return;
            
            reports.Add(reason + " - Reported by " + p.name + " at " + DateTime.Now);
            File.WriteAllLines(ReportPath(target), reports.ToArray());
            p.Message("&aReport sent! It should be viewed when a {0} &ais online", 
                      checkPerms.Describe());
            
            ModAction action = new ModAction(target, p, ModActionType.Reported, reason);
            OnModActionEvent.Call(action);
            if (!action.Announce) return;
            
            string opsMsg = "λNICK &Sreported " + nick + "&S. Reason: " + reason;
            Chat.MessageFrom(ChatScope.Perms, p, opsMsg, checkPerms, null, true);
            string allMsg = "Use &T/Report check " + target + " &Sto see all of their reports";
            Chat.MessageFrom(ChatScope.Perms, p, allMsg, checkPerms, null, true);
        }
        
        
        static bool HasReports(string user) {
            return File.Exists(ReportPath(user));
        }
        static string ReportPath(string user) {
            return "extra/reported/" + user + ".txt";
        }
                
        static string[] GetReportedUsers() {
            string[] users = Directory.GetFiles("extra/reported", "*.txt");
            for (int i = 0; i < users.Length; i++) {
                users[i] = Path.GetFileNameWithoutExtension(users[i]);
            }
            return users;
        }
        
        static void DeleteReport(string user) {
            string backup = "extra/reportedbackups/" + user + ".txt";
            AtomicIO.TryDelete(backup);           
            File.Move(ReportPath(user), backup);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Report list &H- Lists all reported players.");
            p.Message("&T/Report check [player] &H- Views reports for that player.");
            p.Message("&T/Report delete [player] &H- Deletes reports for that player.");
            p.Message("&T/Report clear &H- Clears &call&H reports.");
            p.Message("&T/Report [player] [reason] &H- Reports that player for the given reason.");
        }
    }
}
