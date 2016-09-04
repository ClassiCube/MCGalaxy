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
using System.IO;

namespace MCGalaxy.Commands {
    public sealed class CmdReport : Command {	
        public override string name { get { return "report"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can check, view and delete reports") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("reports", "list") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            if (!Directory.Exists("extra/reported"))
                Directory.CreateDirectory("extra/reported");

            switch (args[0]) {
                case "list":
                    HandleList(p, args); break;
                case "view":
                case "read":
                case "check":
                    HandleCheck(p, args); break;
                case "delete":
                case "remove":
                    HandleDelete(p, args); break;
                case "clear":
                    HandleClear(p, args); break;
                default:
                    HandleAdd(p, args); break;
            }
        }

        void HandleList(Player p, string[] args) {
        	if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "see the list of reports."); return; }

            bool foundone = false;
            string[] files = Directory.GetFiles("extra/reported", "*.txt");
            Player.Message(p, "The following players have been reported:");
            foreach (string file in files) {
                foundone = true;
                Player.Message(p, "- %c" + Path.GetFileNameWithoutExtension(file));
            }
            
            if (foundone) {
                Player.Message(p, "Use %T/report check [Player] %Sto view report info.");
                Player.Message(p, "Use %T/report delete [Player] %Sto delete a report");
            } else {
                Player.Message(p, "No reports were found.");
            }
        }
        
        void HandleCheck(Player p, string[] args) {
            if (args.Length != 2) {
                Player.Message(p, "You need to provide a player's name."); return;
            }
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "view the details of a report."); return; }
            if (!Formatter.ValidName(p, args[1], "player")) return;
            
            if (!File.Exists("extra/reported/" + args[1] + ".txt")) {
                Player.Message(p, "The player you specified has not been reported."); return;
            }
            string details = File.ReadAllText("extra/reported/" + args[1] + ".txt");
            Player.Message(p, details);
        }
        
        void HandleDelete(Player p, string[] args) {
            if (args.Length != 2) {
                Player.Message(p, "You need to provide a player's name."); return;
            }
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "delete reports."); return; }
            if (!Formatter.ValidName(p, args[1], "player")) return;
            
            if (!File.Exists("extra/reported/" + args[1] + ".txt")) {
                Player.Message(p, "The player you specified has not been reported."); return;
            }
            if (!Directory.Exists("extra/reportedbackups"))
                Directory.CreateDirectory("extra/reportedbackups");
            if (File.Exists("extra/reportedbackups/" + args[1] + ".txt"))
                File.Delete("extra/reportedbackups/" + args[1] + ".txt");
            
            File.Move("extra/reported/" + args[1] + ".txt", "extra/reportedbackups/" + args[1] + ".txt");
            Player.Message(p, "&a" + args[1] + "'s report has been deleted.");
            Chat.MessageOps(p.ColoredName + " %Sdeleted " + args[1] + "'s report.");
            Server.s.Log(args[1] + "'s report has been deleted by " + p.name);
        }
        
        void HandleClear(Player p, string[] args) {
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "clear the list of reports."); return; }
           if (!Directory.Exists("extra/reportedbackups"))
                Directory.CreateDirectory("extra/reportedbackups");            
            string[] files = Directory.GetFiles("extra/reported", "*.txt");
            
            foreach (string path in files) {
                string name = Path.GetFileName(path);
                if (File.Exists("extra/reportedbackups/" + name))
                    File.Delete("extra/reportedbackups/" + name);
                File.Move(path, "extra/reportedbackups/" + name);
            }
            Player.Message(p, "%aYou have cleared all reports!");
            Chat.MessageOps(p.ColoredName + "%c cleared ALL reports!");
            Server.s.Log(p.name + " cleared ALL reports!");
        }
        
        void HandleAdd(Player p, string[] args) {
            if (args.Length != 2) {
                Player.Message(p, "You need to provide a reason for the report."); return;
            }
            string target = args[0].ToLower();
            string reason = args[1];

            if (File.Exists("extra/reported/" + target + ".txt")) {
                File.WriteAllText("extra/reported/" + target + "(2).txt", reason + " - Reported by " + p.name + "." + " DateTime: " + DateTime.Now);
                Player.Message(p, "%aYour report has been sent, it should be viewed when an operator is online!");
                return;
            }
            if (File.Exists("extra/reported/" + target + "(2).txt")) {
                Player.Message(p, "%cThe player you've reported has already been reported 2 times! Please wait patiently untill an OP+ has reviewed the reports!");
                return;
            }
            File.WriteAllText("extra/reported/" + target + ".txt", reason + " - Reported by " + p.name + " on " + DateTime.Now);
            Player.Message(p, "%aYour report has been sent, it should be viewed when an operator is online!");
            Chat.MessageOps(p.ColoredName + " %Shas made a report, view it with %T/report check " + target);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/report [Player] [Reason] %H- Reports that player for the given reason.");
            if (!CheckExtraPerm(p)) return;
            Player.Message(p, "%T/report list %H- Outputs the list of reported players.");
            Player.Message(p, "%T/report check [Player] %H- Views report for that player.");
            Player.Message(p, "%T/report delete [Player] %H- Deletes report for that player.");
            Player.Message(p, "%T/report clear %H- Clears &call%H reports.");
        }
    }
}
