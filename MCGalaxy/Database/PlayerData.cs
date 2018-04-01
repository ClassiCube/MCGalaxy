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
using MCGalaxy.SQL;

namespace MCGalaxy.DB {
    
    /// <summary> Retrieves or sets player stats in the database. </summary>
    public class PlayerData {
        
        public const string DBTable = "Players";
        public const string ColumnDeaths = "totalDeaths";
        public const string ColumnLogins = "totalLogin";
        public const string ColumnMoney = "Money";
        public const string ColumnKicked = "totalKicked";
        
        public const string ColumnColor = "color";
        public const string ColumnTitle = "title";
        public const string ColumnTColor = "title_color";
        
        public const string ColumnFirstLogin = "FirstLogin";
        public const string ColumnLastLogin = "LastLogin";
        public const string ColumnTimeSpent = "TimeSpent";
        
        public const string ColumnTotalBlocks = "totalBlocks";
        public const string ColumnTotalCuboided = "totalCuboided";
        public const string ColumnMessages = "Messages";
        
        public string Name, Color, Title, TitleColor, IP;
        public DateTime FirstLogin, LastLogin;
        public int DatabaseID, Money, Deaths, Logins, Kicks, Messages;
        public long TotalModified, TotalDrawn, TotalPlaced, TotalDeleted;
        public TimeSpan TotalTime;
        
        internal static void Create(Player p) {
            p.prefix = "";
            p.color = p.group.Color;         
            p.FirstLogin = DateTime.Now;
            p.TimesVisited = 1;
            
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");            
            Database.Backend.AddRow(DBTable, "Name, IP, FirstLogin, LastLogin, totalLogin, Title, " +
                                    "totalDeaths, Money, totalBlocks, totalKicked, Messages, TimeSpent",
                                    p.name, p.ip, now, now, 1, "", 0, 0, 0, 0, 0, (long)p.TotalTime.TotalSeconds);
            
            using (DataTable ids = Database.Backend.GetRows(DBTable,
                                                            "ID", "WHERE Name = @0", p.name)) {
                if (ids.Rows.Count > 0) {
                    string id = ids.Rows[0]["ID"].ToString();
                    p.DatabaseID = PlayerData.ParseInt(id);
                } else {
                    p.DatabaseID = NameConverter.InvalidNameID(p.name);
                }
            }
        }
        
        internal static void Load(DataTable playerDb, Player p) {
            PlayerData data = PlayerData.Fill(playerDb.Rows[0]);
            p.TimesVisited = data.Logins + 1;
            p.TotalTime = data.TotalTime;
            p.DatabaseID = data.DatabaseID;
            p.FirstLogin = data.FirstLogin;
            
            p.title = data.Title;
            p.titlecolor = data.TitleColor;
            p.color = data.Color;
            if (p.color.Length == 0) p.color = p.group.Color;
                       
            p.TotalModified = data.TotalModified;
            p.TotalDrawn = data.TotalDrawn;
            p.TotalPlaced = data.TotalPlaced;
            p.TotalDeleted = data.TotalDeleted;
            
            p.TimesDied = data.Deaths;
            p.TotalMessagesSent = data.Messages;            
            p.money = data.Money;
            p.TimesBeenKicked = data.Kicks;
        }
        
        public static PlayerData Fill(DataRow row) {
            PlayerData data = new PlayerData();
            data.Name = row["Name"].ToString().Trim();
            data.IP = row["IP"].ToString().Trim();
            data.DatabaseID = ParseInt(row["ID"].ToString());
            
            try {
                long secs = PlayerData.ParseLong(row[ColumnTimeSpent].ToString());
                data.TotalTime = TimeSpan.FromSeconds(secs);
            } catch {
                data.TotalTime = row[ColumnTimeSpent].ToString().ParseDBTime();
            }
            
            data.FirstLogin = ParseDate(row[ColumnFirstLogin]);
            data.LastLogin  = ParseDate(row[ColumnLastLogin]);
            
            data.Title = row[ColumnTitle].ToString().Trim();
            data.Title.Cp437ToUnicodeInPlace();
            data.TitleColor = ParseColor(row[ColumnTColor]);
            data.Color = ParseColor(row[ColumnColor]);
            
            data.Money    = ParseInt(row[ColumnMoney].ToString());
            data.Deaths   = ParseInt(row[ColumnDeaths].ToString());
            data.Logins   = ParseInt(row[ColumnLogins].ToString());
            data.Kicks    = ParseInt(row[ColumnKicked].ToString());
            data.Messages = ParseInt(row[ColumnMessages].ToString());
            
            long blocks   = ParseLong(row[ColumnTotalBlocks].ToString());
            long cuboided = ParseLong(row[ColumnTotalCuboided].ToString());
            data.TotalModified = blocks & LowerBitsMask;
            data.TotalPlaced   = blocks >> LowerBits;
            data.TotalDrawn    = cuboided & LowerBitsMask;
            data.TotalDeleted  = cuboided >> LowerBits;
            return data;
        }
        
        internal static DateTime ParseDate(object value) {
            if (value is DateTime) return (DateTime)value;
            return DateTime.Parse(value.ToString());
        }
        
        internal static long ParseLong(string value) {
            return (value.Length == 0 || value.CaselessEq("null")) ? 0 : long.Parse(value);
        }
        
        internal static int ParseInt(string value) {
            return (value.Length == 0 || value.CaselessEq("null")) ? 0 : int.Parse(value);
        }
        
        static string ParseColor(object value) {
            string col = value.ToString().Trim();
            if (col.Length == 0) return col;
            
            // Try parse color name, then color code
            string parsed = Colors.Parse(col);
            if (parsed.Length > 0) return parsed;
            return Colors.Name(col).Length == 0 ? "" : col;
        }
        
        
        internal static long BlocksPacked(long placed, long modified) {
            return placed << LowerBits | modified;
        }
        
        internal static long CuboidPacked(long deleted, long drawn) {
            return deleted << LowerBits | drawn;
        }

        public const int LowerBits = 38;
        public const long LowerBitsMask = (1L << LowerBits) - 1;
        
        
        public static string FindDBColor(Player p) {
             using (DataTable colors = Database.Backend.GetRows(DBTable,
                                                            "Color", "WHERE ID = @0", p.DatabaseID)) {
                if (colors.Rows.Count > 0) {
                    string col = ParseColor(colors.Rows[0]["Color"]);
                    if (col.Length > 0) return col;
                }
                return "";
            }
        }
    }
}
