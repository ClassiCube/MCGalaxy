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
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Maintenance {
    public sealed class CmdPlayerEdit : Command {
        public override string name { get { return "playeredit"; } }
        public override string shortcut { get { return "pe"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("setinfo") }; }
        }
        public CmdPlayerEdit() { }

        delegate void DBSetter(string name, string column, string data);

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            args[0] = PlayerInfo.FindMatchesPreferOnline(p, args[0]);
            
            if (args[0] == null) return;
            Player who = PlayerInfo.FindExact(args[0]);
            if (args.Length == 1) {
                Player.Message(p, Colors.red + "You must specify a type to modify.");
                MessageValidTypes(p); return;
            }
            
            switch (args[1].ToLower()) {
                case "firstlogin":
                    SetDate(p, args, PlayerData.ColumnFirstLogin, who, 
                            v => who.firstLogin = v); break;
                case "lastlogin":
                    SetDate(p, args, PlayerData.ColumnLastLogin, who, 
                            v => who.timeLogged = v); break;
                    
                case "logins":
                case "totallogin":
                case "totallogins":
                    SetInteger(p, args, PlayerData.ColumnLogins, 1000000000, who,
                               v => who.totalLogins = v, UpdateDB); break;
                case "deaths":
                case "totaldeaths":
                    SetInteger(p, args, PlayerData.ColumnDeaths, short.MaxValue, who,
                               v => who.overallDeath = v, UpdateDB); break;
                case "money":
                    SetInteger(p, args, PlayerData.ColumnMoney, 100000000, who,
                               v => who.money = v, UpdateDB); break;

                case "title":
                    if (args.Length < 3) {
                        Player.Message(p, "Title can be up to 20 characters. Use \"null\" to remove the title"); return;
                    }
                    if (args[2].Length >= 20) { Player.Message(p, "Title must be under 20 characters"); return; }
                    if (args[2] == "null") args[2] = "";
                    
                    if (who != null) {
                        who.title = args[2];
                        who.SetPrefix();
                    }
                    UpdateDB(args[0], args[2], PlayerData.ColumnTitle);
                    MessageDataChanged(p, args[0], args[1], args[2]); break;

                    
                case "modified":
                case "totalblocks":
                    SetInteger(p, args, PlayerData.ColumnTotalBlocks, int.MaxValue, who,
                               v => who.overallBlocks = v, UpdateDBLo); break;
                case "drawn":
                case "totalcuboided":
                case "totalcuboid":
                    SetInteger(p, args, PlayerData.ColumnTotalCuboided, int.MaxValue, who,
                               v => who.TotalDrawn = v, UpdateDBLo); break;
                case "placed":
                    SetInteger(p, args, PlayerData.ColumnTotalBlocks, int.MaxValue, who,
                               v => who.TotalPlaced = v, UpdateDBHi); break;
                case "deleted":
                    SetInteger(p, args, PlayerData.ColumnTotalCuboided, int.MaxValue, who,
                               v => who.TotalDeleted = v, UpdateDBHi); break;
                    
                case "totalkicked":
                    SetInteger(p, args, PlayerData.ColumnKicked, 1000000000, who,
                               v => who.totalKicked = v, UpdateDB); break;
                case "timespent":
                    SetTimespan(p, args, PlayerData.ColumnTimeSpent, who, v => who.time = v); break;
                case "color":
                    SetColor(p, args, PlayerData.ColumnColor, who, v => who.color = (v == "" ? who.group.color : v)); break;
                case "titlecolor":
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
            UpdateDB(args[0], col, column);
            MessageDataChanged(p, args[0], args[1], args[2]);
        }
        
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";
        static void SetDate(Player p, string[] args, string column, Player who, Action<DateTime> setter) {
            if (args.Length < 3) {
                Player.Message(p, "Dates must be in the format: yyyy-mm-dd hh:mm:ss");
                return;
            }
            
            DateTime date;
            if (!DateTime.TryParseExact(args[2], dateFormat, null, 0, out date)) {
                Player.Message(p, "Invalid date. (must be in format: yyyy-mm-dd hh:mm:ss");
                return;
            }
            
            if (who != null)
                setter(date);
            UpdateDB(args[0], args[2], column);
            MessageDataChanged(p, args[0], args[1], args[2]);
        }
        
        static void SetTimespan(Player p, string[] args, string column, Player who, Action<TimeSpan> setter) {
            if (args.Length < 3) {
                Player.Message(p, "Timespan must be in the format: <number><quantifier>..");
                Player.Message(p, CommandParser.TimespanHelp, "set time spent to");
                return;
            }
            
            TimeSpan span = TimeSpan.Zero;
            if (!CommandParser.GetTimespan(p, args[2], ref span, "set time spent to", 'm')) return;
            
            if (who != null) {
                setter(span);
            } else {
                long secs = (long)span.TotalSeconds;
                UpdateDB(args[0], secs.ToString(), column);
            }
            MessageDataChanged(p, args[0], args[1], span.Shorten(true));
        }
        
        static void SetInteger(Player p, string[] args, string column, int max, Player who,
                               Action<int> setter, DBSetter dbSetter) {
            if (args.Length < 3) {
                Player.Message(p, "You must specify a positive integer, which can be {0} at most.", max); return;
            }
            
            int value = 0;
            if (!CommandParser.GetInt(p, args[2], "Amount", ref value, 0, max)) return;
            
            if (who != null) {
                setter(value);
            } else {
                dbSetter(args[0], args[2], column);
            }
            MessageDataChanged(p, args[0], args[1], args[2]);
        }
        
        
        static void UpdateDB(string name, string value, string column) {
            Database.Backend.UpdateRows("Players", column + " = @1", "WHERE Name = @0", name, value.UnicodeToCp437());
        }
        
        // special case handling for packed forms of totalBlocks and totalCuboided
        static void UpdateDBLo(string name, string value, string column) {
            long loValue = long.Parse(value);
            // OR with existing high bits of value in DB
            using (DataTable results = Database.Backend.GetRows("Players", column, "WHERE Name = @0", name)) {
                if (results.Rows.Count > 0) {
                    long curValue = PlayerData.ParseLong(results.Rows[0][column].ToString());
                    loValue |= (curValue & ~PlayerData.LowerBitsMask);
                }
            }
            Database.Backend.UpdateRows("Players", column + " = @1", "WHERE Name = @0", name, loValue);
        }
        
        static void UpdateDBHi(string name, string value, string column) {
            long hiValue = long.Parse(value) << PlayerData.LowerBits;
            // OR with existing low bits of value in DB
            using (DataTable results = Database.Backend.GetRows("Players", column, "WHERE Name = @0", name)) {
                if (results.Rows.Count > 0) {
                    long curValue = PlayerData.ParseLong(results.Rows[0][column].ToString());
                    hiValue |= (curValue & PlayerData.LowerBitsMask);
                }
            }            
            Database.Backend.UpdateRows("Players", column + " = @1", "WHERE Name = @0", name, hiValue);
        }
        
        static void MessageDataChanged(Player p, string name, string type, string value) {
            name = PlayerInfo.GetColoredName(p, name);
            string msg = value == "" ? String.Format("The {1} data for &b{0} %Shas been reset.", name, type)
                : String.Format("The {1} data for &b{0} %Shas been updated to &a{2}%S.", name, type, value);
            Player.Message(p, msg);
        }

        static void MessageValidTypes(Player p) {
            Player.Message(p, "%HValid types: %SFirstLogin, LastLogin, Logins, Title, Deaths, Money, " +
                           "Modified, Drawn, Placed, Deleted, TotalKicked, TimeSpent, Color, TitleColor ");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/pe [username] [type] <value>");
            Player.Message(p, "%HEdits an online or offline player's information. Use with caution!");
            MessageValidTypes(p);
            Player.Message(p, "%HTo see value format for a specific type, leave <value> blank.");
        }
    }
}
