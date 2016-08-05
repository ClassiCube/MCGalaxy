/*
    Copyright 2015 MCGalaxy team
    
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
using System.Data;
using System.Text.RegularExpressions;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {    
    public sealed class CmdPlayerEditDB : Command {        
        public override string name { get { return "playeredit"; } }
        public override string shortcut { get { return "pe"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdPlayerEditDB() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            Player who = PlayerInfo.Find(args[0]);
            if (who == null) {
                string target = PlayerInfo.FindOfflineNameMatches(p, args[0]);
                if (target == null) return;
                args[0] = target;
            }
            if (args.Length == 1) {
                Player.Message(p, Colors.red + "You must specify a type to modify.");
                MessageValidTypes(p); return;
            }
            
            switch (args[1].ToLower()) {
                case "firstlogin":
                    SetDate(p, args, "FirstLogin", who, v => who.firstLogin = v); break;
                case "lastlogin":
                    SetDate(p, args, "LastLogin", who, v => who.timeLogged = v); break;
                case "totallogin":
                case "totallogins":
                    SetInteger(p, args, "totalLogin", 1000000000, who, v => who.totalLogins = v); break;
                case "title":
                    if (args.Length < 3) {
                        Player.Message(p, "Title can be up to 17 characters. Use \"null\" to remove the title"); return;
                    }
                    if (args[2].Length > 17) { Player.Message(p, "Title must be under 17 characters"); return; }
                    if (args[2] == "null") args[2] = "";
                    
                    if (who != null) {
                        who.title = args[2];
                        who.SetPrefix();
                    }
                    UpdateDB(p, args[0], args[1], args[2], "Title");
                    MessageDataChanged(p, args[0], args[1], args[2]); break;
                case "totaldeaths":
                    SetInteger(p, args, "TotalDeaths", 1000000, who, v => who.overallDeath = v); break;
                case "money":
                    SetInteger(p, args, "Money", 100000000, who, v => who.money = v); break;
                case "totalblocks":
                    SetInteger(p, args, "totalBlocks", int.MaxValue, who, v => who.overallBlocks = v); break;
                case "totalcuboided":
                case "totalcuboid":
                    SetInteger(p, args, "totalCuboided", int.MaxValue, who, v => who.TotalDrawn = v); break;
                case "totalkicked":
                    SetInteger(p, args, "totalKicked", 1000000000, who, v => who.totalKicked = v); break;
                case "timespent":
                    SetTimespan(p, args, "TimeSpent", who, v => who.time = v.ParseDBTime()); break;
                case "color":
                    SetColor(p, args, "color", who, v => who.color = (v == "" ? who.group.color : v)); break;
                case "titlecolor":
                    SetColor(p, args, "title_color", who, v => who.titlecolor = (v == "" ? "" : v)); break;
                default:
                    Player.Message(p, Colors.red + "Invalid type.");
                    MessageValidTypes(p); break;
            }
        }
        
        static void SetColor(Player p, string[] args, string column, Player who, Action<string> setter) {
            if (args.Length < 3) {
                Player.Message(p, "Color format: color name, or \"null\" to reset to default color."); return;
            }
            
        	string col = args[2] == "null" ? "" : Colors.Parse(args[2]);
        	if (col == "" && args[2] != "null") {
                Player.Message(p, "There is no color \"" + args[2] + "\"."); return;
            }
            
            if (who != null) {
                setter(col);
                who.SetPrefix();
                args[0] = who.name;
            }
            UpdateDB(p, args[0], args[1], col, column);
            MessageDataChanged(p, args[0], args[1], args[2]);
        }
        
        static void SetDate(Player p, string[] args, string column, Player who, Action<DateTime> setter) {
            if (args.Length < 3) {
                Player.Message(p, "Dates must be in the format: yyyy-mm-dd_hh:mm:ss");
                Player.Message(p, "Do not include spaces or other special characters other than what you see above.");
                return;
            }
            args[2] = args[2].Replace('_', ' ');
            if (!Regex.IsMatch(args[2], @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}")) {
                Player.Message(p, "Dates must be in the format: yyyy-mm-dd_hh:mm:ss"); return;
            }
            
            DateTime date;
            if (!DateTime.TryParse(args[2], out date)) {
                Player.Message(p, "Invalid format.");
                Player.Message(p, "Your date must be possible via the Western Calender");
                Player.Message(p, "Time cannot exceede increments of 23h, 59m, 59s");
                return;
            }
            
            if (who != null)
                setter(date);
            UpdateDB(p, args[0], args[1], args[2], column);
            MessageDataChanged(p, args[0], args[1], args[2]);
        }
        
        static void SetTimespan(Player p, string[] args, string column, Player who, Action<string> setter) {
            if (args.Length < 3) {
                Player.Message(p, "Timespan must be in the format: <number><quantifier>..");
                Player.Message(p, TimeUtils.Help, "set time spent to");
                return;
            }
            
            TimeSpan timeFrame;
            if (!args[2].TryParseShort(p, 'm', "set time spent to", out timeFrame)) return;
            
            string time = timeFrame.ToDBTime();
            if (who != null) {
                setter(time);
            } else {
                UpdateDB(p, args[0], args[1], time, column);
            }
            MessageDataChanged(p, args[0], args[1], timeFrame.Shorten(true));
        }
        
        static void SetInteger(Player p, string[] args, string column, int max, Player who, Action<int> setter) {
            if (args.Length < 3) {
                max--;
                int digits = 1; max /= 10;
                while (max > 0) {
                    digits++; max /= 10;
                }
                Player.Message(p, "You must specify a number, which can be " + digits + " digits at most."); return;
            }
            
            int value = 0;
            if (!Int32.TryParse(args[2], out value)) {
                Player.Message(p, "&c" + args[2] + " is either not a number, or is larger than " + int.MaxValue); return;
            }
            if (value < 0 || value >= max) {
                Player.Message(p, "&cNumber must be less than " + max + " and cannot be negative."); return;
            }
            
            if (who != null)
                setter(value);
            else
                UpdateDB(p, args[0], args[1], args[2], column);
            MessageDataChanged(p, args[0], args[1], args[2]);
        }
        
        static void UpdateDB(Player p, string name, string type, string value, string column) {
            if (value != "") {
                Database.Execute("UPDATE Players SET " + column + "=@1 WHERE Name=@0", name, value);
            } else {
                Database.Execute("UPDATE Players SET " + column + "='' WHERE Name=@0", name);
            }
        }
        
        static void MessageDataChanged(Player p, string name, string type, string value) {
            string msg = value == "" ? String.Format("The {1} data for &b{0} %Shas been reset.", name, type)
                : String.Format("The {1} data for &b{0} %Shas been updated to &a{2}%S.", name, type, value);
            Player.Message(p, msg);
        }

        static void MessageValidTypes(Player p) {
            Player.Message(p, "%HValid types: %SFirstLogin, LastLogin, TotalLogins, Title, TotalDeaths, Money, " +
                               "TotalBlocks, TotalCuboid, TotalKicked, TimeSpent, Color, TitleColor ");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/pe <username> <type> <value>");
            Player.Message(p, "%HEdits an online or offline player's information. Use with caution!");
            MessageValidTypes(p);
            Player.Message(p, "%HTo see value format for a specific type, leave <value> blank.");
        }
    }
}
