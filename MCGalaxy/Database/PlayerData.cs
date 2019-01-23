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
        public const string ColumnMoney  = "Money";
        public const string ColumnKicked = "totalKicked";
        
        public const string ColumnColor  = "color";
        public const string ColumnTitle  = "title";
        public const string ColumnTColor = "title_color";
        
        public const string ColumnName = "Name";
        public const string ColumnIP   = "IP";
        public const string ColumnID   = "ID";
        
        public const string ColumnFirstLogin = "FirstLogin";
        public const string ColumnLastLogin  = "LastLogin";
        public const string ColumnTimeSpent  = "TimeSpent";
        
        public const string ColumnTotalBlocks   = "totalBlocks";
        public const string ColumnTotalCuboided = "totalCuboided";
        public const string ColumnMessages      = "Messages";
        
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
            
            string now = DateTime.Now.ToString(Database.DateFormat);
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
            data.Name = record.GetText(ColumnName);
            data.IP   = record.GetText(ColumnIP);
            data.DatabaseID = record.GetInt(ColumnID);
            
            // Backwards compatibility with old format
            string rawTime = record.GetText(ColumnTimeSpent);
            try {
                long secs = long.Parse(rawTime);
                data.TotalTime = TimeSpan.FromSeconds(secs);
            } catch {
                data.TotalTime = rawTime.ParseOldDBTimeSpent();
            }
            
            data.FirstLogin = ParseDateTime(record, ColumnFirstLogin);
            data.LastLogin  = ParseDateTime(record, ColumnLastLogin);
            
            data.Title = record.GetText(ColumnTitle);
            data.Title = data.Title.Cp437ToUnicode();
            data.TitleColor = ParseCol(record.GetText(ColumnTColor));
            data.Color = ParseCol(record.GetText(ColumnColor));
            
            data.Money    = record.GetInt(ColumnMoney);
            data.Deaths   = record.GetInt(ColumnDeaths);
            data.Logins   = record.GetInt(ColumnLogins);
            data.Kicks    = record.GetInt(ColumnKicked);
            data.Messages = record.GetInt(ColumnMessages);
            
            long blocks   = record.GetLong(ColumnTotalBlocks);
            long cuboided = record.GetLong(ColumnTotalCuboided);
            data.TotalModified = blocks & LowerBitsMask;
            data.TotalPlaced   = blocks >> LowerBits;
            data.TotalDrawn    = cuboided & LowerBitsMask;
            data.TotalDeleted  = cuboided >> LowerBits;
            return data;
        }
        internal static object Read(IDataRecord record, object arg) { return Parse(record); }
        
        internal static long ParseLong(string value) {
            return (value.Length == 0 || value.CaselessEq("null")) ? 0 : long.Parse(value);
        }
        
        internal static int ParseInt(string value) {
            return (value.Length == 0 || value.CaselessEq("null")) ? 0 : int.Parse(value);
        }
        
        internal static string ParseCol(string raw) {
            if (raw.Length == 0) return raw;
            
            // Try parse color name, then color code
            string col = Colors.Parse(raw);
            if (col.Length > 0) return col;
            return Colors.Name(raw).Length == 0 ? "" : raw;
        }
        
        static DateTime ParseDateTime(IDataRecord record, string name) {
            int i = record.GetOrdinal(name);
            // dates are a major pain
            try {
                string raw = record.GetStringValue(i);
                return DateTime.ParseExact(raw, Database.DateFormat, null);
            } catch {
                try {
                    return record.GetDateTime(i);
                } catch (Exception ex) {
                    Logger.LogError("Error parsing date", ex);
                    return DateTime.MinValue;
                }
            }
        }
        
        
        internal static long BlocksPacked(long placed, long modified) {
            return placed << LowerBits | modified;
        }
        
        internal static long CuboidPacked(long deleted, long drawn) {
            return deleted << LowerBits | drawn;
        }

        public const int LowerBits = 38;
        public const long LowerBitsMask = (1L << LowerBits) - 1;
    }
}
