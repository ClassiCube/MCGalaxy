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
        
        static object ReadID(IDataRecord record, object arg) { return record.GetInt32(0); }        
        internal static void Create(Player p) {
            p.prefix = "";
            p.color = p.group.Color;
            p.FirstLogin = DateTime.Now;
            p.TimesVisited = 1;
            
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Database.Backend.AddRow("Players", "Name, IP, FirstLogin, LastLogin, totalLogin, Title, " +
                                    "totalDeaths, Money, totalBlocks, totalKicked, Messages, TimeSpent",
                                    p.name, p.ip, now, now, 1, "", 0, 0, 0, 0, 0, (long)p.TotalTime.TotalSeconds);
            
            object id = Database.Backend.ReadRows("Players", "ID", null, ReadID, "WHERE Name=@0", p.name);
            if (id != null) {
                p.DatabaseID = (int)id;
            } else {
                p.DatabaseID = NameConverter.InvalidNameID(p.name);
            }
        }
        
        internal static void Apply(PlayerData data, Player p) {
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
        
        internal static PlayerData Parse(IDataRecord record) {
        	PlayerData data = new PlayerData();
            data.Name = record.GetString("Name");
            data.IP   = record.GetString("IP");
            data.DatabaseID = record.GetInt32("ID");
            
            // Backwards compatibility with old format
            string rawTime = record.GetString(ColumnTimeSpent);
            try {
                long secs = long.Parse(rawTime);
                data.TotalTime = TimeSpan.FromSeconds(secs);
            } catch {
                data.TotalTime = rawTime.ParseDBTime();
            }
            
            data.FirstLogin = record.GetDateTime(ColumnFirstLogin);
            data.LastLogin  = record.GetDateTime(ColumnLastLogin);
            
            data.Title = record.GetString(ColumnTitle);
            data.Title = data.Title.Cp437ToUnicode();
            data.TitleColor = ParseCol(record.GetString(ColumnTColor));
            data.Color = ParseCol(record.GetString(ColumnColor));
            
            data.Money    = record.GetInt32(ColumnMoney);
            data.Deaths   = record.GetInt32(ColumnDeaths);
            data.Logins   = record.GetInt32(ColumnLogins);
            data.Kicks    = record.GetInt32(ColumnKicked);
            data.Messages = record.GetInt32(ColumnMessages);
            
            long blocks   = record.GetInt64(ColumnTotalBlocks);
            long cuboided = record.GetInt64(ColumnTotalCuboided);
            data.TotalModified = blocks & LowerBitsMask;
            data.TotalPlaced   = blocks >> LowerBits;
            data.TotalDrawn    = cuboided & LowerBitsMask;
            data.TotalDeleted  = cuboided >> LowerBits;
            return data;
        }
        
        internal static object Read(IDataRecord record, object arg) { return Parse(record); }
        
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
        
        static string ParseCol(string raw) {
            if (raw.Length == 0) return raw;
            
            // Try parse color name, then color code
            string col = Colors.Parse(raw);
            if (col.Length > 0) return col;
            return Colors.Name(raw).Length == 0 ? "" : raw;
        }
        
        
        internal static long BlocksPacked(long placed, long modified) {
            return placed << LowerBits | modified;
        }
        
        internal static long CuboidPacked(long deleted, long drawn) {
            return deleted << LowerBits | drawn;
        }

        public const int LowerBits = 38;
        public const long LowerBitsMask = (1L << LowerBits) - 1;
        
        
        public static void Update(string name, string column, string value) {
            Database.Backend.UpdateRows("Players", column + "=@1", "WHERE Name=@0", name, value);
        }
        
        public static string FindDBColor(Player p) {
            string raw = Database.ReadString("Players", "Color", "WHERE ID=@0", p.DatabaseID);
            if (raw == null) return "";
            return ParseCol(raw);
        }
    }
}
