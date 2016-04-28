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
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "Lowest rank which can check, view and delete reports") }; }
        }
        static char[] trimChars = {' '};

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(trimChars, 2);
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
        	if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can see the list of reports."); return; }

            bool foundone = false;
            FileInfo[] fi = new DirectoryInfo("extra/reported").GetFiles("*.txt");
            Player.SendMessage(p, "The following players have been reported:");
            foreach (FileInfo file in fi) {
                foundone = true;
                string parsed = file.Name.Replace(".txt", "");
                Player.SendMessage(p, "- %c" + parsed);
            }
            
            if (foundone) {
                Player.SendMessage(p, "Use %T/report check [Player] %Sto view report info.");
                Player.SendMessage(p, "Use %T/report delete [Player] %Sto delete a report");
            } else {
                Player.SendMessage(p, "No reports were found.");
            }
        }
        
        void HandleCheck(Player p, string[] args) {
            if (args.Length != 2) {
                Player.SendMessage(p, "You need to provide a player's name."); return;
            }
            if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can view the details of a report."); return; }
            if (!Player.ValidName(args[1])) {
                Player.SendMessage(p, "\"" + args[1] + "\" is not a valid player name."); return;
            }
            
            if (!File.Exists("extra/reported/" + args[1] + ".txt")) {
                Player.SendMessage(p, "The player you specified has not been reported."); return;
            }
            string details = File.ReadAllText("extra/reported/" + args[1] + ".txt");
            Player.SendMessage(p, details);
        }
        
        void HandleDelete(Player p, string[] args) {
            if (args.Length != 2) {
                Player.SendMessage(p, "You need to provide a player's name."); return;
            }
            if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can delete reports."); return; }
            if (!Player.ValidName(args[1])) {
                Player.SendMessage(p, "\"" + args[1] + "\" is not a valid player name."); return;
            }
            
            if (!File.Exists("extra/reported/" + args[1] + ".txt")) {
                Player.SendMessage(p, "The player you specified has not been reported."); return;
            }
            if (!Directory.Exists("extra/reportedbackups"))
                Directory.CreateDirectory("extra/reportedbackups");
            if (File.Exists("extra/reportedbackups/" + args[1] + ".txt"))
                File.Delete("extra/reportedbackups/" + args[1] + ".txt");
            
            File.Move("extra/reported/" + args[1] + ".txt", "extra/reportedbackups/" + args[1] + ".txt");
            Player.SendMessage(p, "%a" + args[1] + "'s report has been deleted.");
            Chat.GlobalMessageOps(p.ColoredName + " %Sdeleted " + args[1] + "'s report.");
            Server.s.Log(args[1] + "'s report has been deleted by " + p.name);
        }
        
        void HandleClear(Player p, string[] args) {
            if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can clear the list of reports."); return; }
            
            FileInfo[] fi = new DirectoryInfo("extra/reported").GetFiles("*.txt");
            foreach (FileInfo file in fi) {
                if (File.Exists("extra/reportedbackups/" + file.Name))
                    File.Delete("extra/reportedbackups/" + file.Name);
                file.MoveTo("extra/reportedbackups/" + file.Name);
            }
            Player.SendMessage(p, "%aYou have cleared all reports!");
            Chat.GlobalMessageOps(p.ColoredName + "%c cleared ALL reports!");
            Server.s.Log(p.name + " cleared ALL reports!");
        }
        
        void HandleAdd(Player p, string[] args) {
            if (args.Length != 2) {
                Player.SendMessage(p, "You need to provide a reason for the report."); return;
            }
            string target = args[0].ToLower();
            string reason = args[1];

            if (File.Exists("extra/reported/" + target + ".txt")) {
                File.WriteAllText("extra/reported/" + target + "(2).txt", reason + " - Reported by " + p.name + "." + " DateTime: " + DateTime.Now);
                Player.SendMessage(p, "%aYour report has been sent, it should be viewed when an operator is online!");
                return;
            }
            if (File.Exists("extra/reported/" + target + "(2).txt")) {
                Player.SendMessage(p, "%cThe player you've reported has already been reported 2 times! Please wait patiently untill an OP+ has reviewed the reports!");
                return;
            }
            File.WriteAllText("extra/reported/" + target + ".txt", reason + " - Reported by " + p.name + " on " + DateTime.Now);
            Player.SendMessage(p, "%aYour report has been sent, it should be viewed when an operator is online!");
            Chat.GlobalMessageOps(p.ColoredName + " %Shas made a report, view it with %T/report check " + target);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/report [Player] [Reason] %H- Reports that player for the given reason.");
            if (!CheckAdditionalPerm(p)) return;
            Player.SendMessage(p, "%T/report list %H- Outputs the list of reported players.");
            Player.SendMessage(p, "%T/report check [Player] %H- View the report for the given player.");
            Player.SendMessage(p, "%T/report delete [Player] %H- Deletes the report for the given player.");
            Player.SendMessage(p, "%T/report clear %H- Clears &call%H reports.");
        }
    }
}
