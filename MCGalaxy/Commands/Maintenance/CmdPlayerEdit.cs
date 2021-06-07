/*
    Copyright 2015 MCGalaxy
    
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
using System.Net;
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Maintenance {
    public sealed class CmdPlayerEdit : Command2 {
        public override string name { get { return "PlayerEdit"; } }
        public override string shortcut { get { return "pe"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("SetInfo") }; }
        }

        delegate void DBSetter(string name, string column, string data);
        const int type_norm = 0, type_lo = 1, type_hi = 2;

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            args[0] = PlayerInfo.FindMatchesPreferOnline(p, args[0]);
            
            if (args[0] == null) return;
            Player who = PlayerInfo.FindExact(args[0]);
            if (args.Length == 1) {
                p.Message("&WYou must specify a type to modify.");
                MessageValidTypes(p); return;
            }
            
            string opt = args[1].ToLower();
            if (opt == "firstlogin") {
                SetDate(p, args, PlayerData.ColumnFirstLogin, who,
                        v => who.FirstLogin = v);
            } else if (opt == "lastlogin") {
                SetDate(p, args, PlayerData.ColumnLastLogin, who,
                        v => who.LastLogin = v);
            } else if (opt == "logins") {
                SetInteger(p, args, PlayerData.ColumnLogins, 1000000000, who,
                           v => who.TimesVisited = v, type_norm);
            } else if (opt == "deaths") {
                SetInteger(p, args, PlayerData.ColumnDeaths, short.MaxValue, who,
                           v => who.TimesDied = v, type_norm);
            } else if (opt == "money") {
                SetInteger(p, args, PlayerData.ColumnMoney, 100000000, who,
                           v => who.money = v, type_norm);
            } else if (opt == "title") {
                if (args.Length < 3) {
                    p.Message("Title can be up to 20 characters. Use \"null\" to remove the title"); return;
                }
                if (args[2].Length >= 20) { p.Message("Title must be under 20 characters"); return; }
                if (args[2] == "null") args[2] = "";
                
                if (who != null) {
                    who.title = args[2];
                    who.SetPrefix();
                }
                
                PlayerDB.Update(args[0], PlayerData.ColumnTitle, args[2].UnicodeToCp437());
                MessageDataChanged(p, args[0], args[1], args[2]);
            } else if (opt == "ip") {
                if (args.Length < 3) {
                    p.Message("A new IP address must be provided."); return;
                }
                
                IPAddress ip;
                if (!IPAddress.TryParse(args[2], out ip)) {
                    p.Message("&W\"{0}\" is not a valid IP address.", args[2]); return;
                }
                
                if (who != null) who.SetIP(ip);
                PlayerDB.Update(args[0], PlayerData.ColumnIP, args[2]);
                MessageDataChanged(p, args[0], args[1], args[2]);
            }  else if (opt == "modified") {
                SetInteger(p, args, PlayerData.ColumnBlocks, int.MaxValue, who,
                           v => who.TotalModified = v, type_lo);
            } else if (opt == "drawn") {
                SetInteger(p, args, PlayerData.ColumnDrawn, int.MaxValue, who,
                           v => who.TotalDrawn = v, type_lo);
            } else if (opt == "placed") {
                SetInteger(p, args, PlayerData.ColumnBlocks, int.MaxValue, who,
                           v => who.TotalPlaced = v, type_hi);
            } else if (opt == "deleted") {
                SetInteger(p, args, PlayerData.ColumnDrawn, int.MaxValue, who,
                           v => who.TotalDeleted = v, type_hi);
            } else if (opt == "totalkicked") {
                SetInteger(p, args, PlayerData.ColumnKicked, 16777215, who,
                           v => who.TimesBeenKicked = v, type_norm);
            } else if (opt == "messages") {
                SetInteger(p, args, PlayerData.ColumnMessages, 16777215, who,
                           v => who.TotalMessagesSent = v, type_norm);
            }  else if (opt == "timespent") {
                SetTimespan(p, args, PlayerData.ColumnTimeSpent, who,
                            v => who.TotalTime = v);
            } else if (opt == "color") {
                SetColor(p, args, PlayerData.ColumnColor, who,
                         v => who.UpdateColor(v.Length == 0 ? who.group.Color : v));
            } else if (opt == "titlecolor") {
                SetColor(p, args, PlayerData.ColumnTColor, who,
                         v => who.titlecolor = v);
            } else {
                p.Message("&WInvalid type");
                MessageValidTypes(p);
            }
        }
        
        
        static void SetColor(Player p, string[] args, string column, Player who, Action<string> setter) {
            if (args.Length < 3) {
                p.Message("Color format: color name, or \"null\" to reset to default color."); return;
            }
            
            string col = args[2] == "null" ? "" : Matcher.FindColor(p, args[2]);
            if (col == null) return;
            
            if (who != null) {
                setter(col);
                who.SetPrefix();
                args[0] = who.name;
            }
            
            PlayerDB.Update(args[0], column, col);
            MessageDataChanged(p, args[0], args[1], args[2]);
        }
        
        static void SetDate(Player p, string[] args, string column, Player who, Action<DateTime> setter) {
            if (args.Length < 3) {
                p.Message("Dates must be in the format: " + Database.DateFormat);
                return;
            }
            
            DateTime date;
            if (!DateTime.TryParseExact(args[2], Database.DateFormat, null, 0, out date)) {
                p.Message("Invalid date. It must be in format: " + Database.DateFormat);
                return;
            }
            
            if (who != null) setter(date);
            PlayerDB.Update(args[0], column, args[2]);
            MessageDataChanged(p, args[0], args[1], args[2]);
        }
        
        static void SetTimespan(Player p, string[] args, string column, Player who, Action<TimeSpan> setter) {
            if (args.Length < 3) {
                p.Message("Timespan must be in the format: <number><quantifier>..");
                p.Message(CommandParser.TimespanHelp, "set time spent to");
                return;
            }
            
            TimeSpan span = TimeSpan.Zero;
            if (!CommandParser.GetTimespan(p, args[2], ref span, "set time spent to", "m")) return;
            
            if (who != null) {
                setter(span);
            } else {
                long secs = (long)span.TotalSeconds;
                PlayerDB.Update(args[0], column, secs.ToString());
            }
            MessageDataChanged(p, args[0], args[1], span.Shorten(true));
        }
        
        static object ReadLong(IDataRecord record, object arg) { return record.GetInt64(0); }
        static long GetLong(string name, string column) {
            return (long)Database.ReadRows("Players", column, null, ReadLong,
        	                               "WHERE Name=@0", name);
        }
        
        static void SetInteger(Player p, string[] args, string column, int max, Player who,
                               Action<int> setter, int type) {
            if (args.Length < 3) {
                p.Message("You must specify a positive integer, which can be {0} at most.", max); return;
            }
            
            int value = 0;
            if (!CommandParser.GetInt(p, args[2], "Amount", ref value, 0, max)) return;
            
            if (who != null) {
                setter(value);
            } else {
                string dbValue = args[2];
                // special case handling for packed forms of totalBlocks and totalCuboided
                if (type == 1) {
                    long packed = GetLong(args[0], column) & ~PlayerData.LoBitsMask; // hi value only
                    packed |= (uint)value;
                    dbValue = packed.ToString();
                } else if (type == 2) {
                    long packed = GetLong(args[0], column) & PlayerData.LoBitsMask; // lo value only
                    packed |= ((long)value) << PlayerData.HiBitsShift;
                    dbValue = packed.ToString();
                }
                PlayerDB.Update(args[0], column, dbValue);
            }
            MessageDataChanged(p, args[0], args[1], args[2]);
        }

        
        static void MessageDataChanged(Player p, string name, string type, string value) {
            name = p.FormatNick(name);
            if (value.Length == 0) {
                p.Message("The {1} data for &b{0} &Shas been reset.", name, type);
            } else {
                p.Message("The {1} data for &b{0} &Shas been updated to &a{2}&S.", name, type, value);
            }
        }

        static void MessageValidTypes(Player p) {
            p.Message("&HValid types: &SFirstLogin, LastLogin, Logins, Title, IP, Deaths, Money, " +
                      "Modified, Drawn, Placed, Deleted, TotalKicked, TimeSpent, Color, TitleColor, Messages ");
        }
        
        public override void Help(Player p) {
            p.Message("&T/PlayerEdit [username] [type] <value>");
            p.Message("&HEdits an online or offline player's information. Use with caution!");
            MessageValidTypes(p);
            p.Message("&HTo see value format for a specific type, leave <value> blank.");
        }
    }
}
