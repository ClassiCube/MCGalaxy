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
        
        public const string ColumnBlocks   = "totalBlocks";
        public const string ColumnDrawn    = "totalCuboided";
        public const string ColumnMessages = "Messages";
        
        public string Name, Color, Title, TitleColor, IP;
        public DateTime FirstLogin, LastLogin;
        public int DatabaseID, Money, Deaths, Logins, Kicks, Messages;
        public long TotalModified, TotalDrawn, TotalPlaced, TotalDeleted;
        public TimeSpan TotalTime;
        
        static object ReadID(IDataRecord record, object arg) { return record.GetInt32(0); }
        internal static void Create(Player p) {
            p.prefix = "";
            p.SetColor(p.group.Color);
            p.FirstLogin = DateTime.Now;
            p.TimesVisited = 1;
            
            string now = DateTime.Now.ToString(Database.DateFormat);
            Database.AddRow("Players", "Name, IP, FirstLogin, LastLogin, totalLogin, Title, " +
                            "totalDeaths, Money, totalBlocks, totalKicked, Messages, TimeSpent",
                            p.name, p.ip, now, now, 1, "", 0, 0, 0, 0, 0, (long)p.TotalTime.TotalSeconds);
            
            object id = Database.ReadRows("Players", "ID", null, ReadID, "WHERE Name=@0", p.name);
            if (id != null) {
                p.DatabaseID = (int)id;
            } else {
                p.DatabaseID = NameConverter.InvalidNameID(p.name);
            }
        }
        
        /// <summary> Initialises the given player's stats from this instance. </summary>
        public void ApplyTo(Player p) {
            p.TimesVisited = Logins + 1;
            p.TotalTime = TotalTime;
            p.DatabaseID = DatabaseID;
            p.FirstLogin = FirstLogin;
            
            p.title = Title;
            p.titlecolor = TitleColor;
            
            string col = Color;
            if (col.Length == 0) col = p.group.Color;
            p.SetColor(col);
            
            p.TotalModified = TotalModified;
            p.TotalDrawn = TotalDrawn;
            p.TotalPlaced = TotalPlaced;
            p.TotalDeleted = TotalDeleted;
            
            p.TimesDied = Deaths;
            p.TotalMessagesSent = Messages;
            p.money = Money;
            p.TimesBeenKicked = Kicks;
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
            data.TitleColor = ParseColor(record.GetText(ColumnTColor));
            data.Color = ParseColor(record.GetText(ColumnColor));
            
            data.Money    = record.GetInt(ColumnMoney);
            data.Deaths   = record.GetInt(ColumnDeaths);
            data.Logins   = record.GetInt(ColumnLogins);
            data.Kicks    = record.GetInt(ColumnKicked);
            data.Messages = record.GetInt(ColumnMessages);
            
            long blocks = record.GetLong(ColumnBlocks);
            long drawn  = record.GetLong(ColumnDrawn);
            data.TotalModified = UnpackLo(blocks);
            data.TotalPlaced   = UnpackHi(blocks);
            data.TotalDrawn    = UnpackLo(drawn);
            data.TotalDeleted  = UnpackHi(drawn);
            return data;
        }
        internal static object Read(IDataRecord record, object arg) { return Parse(record); }
        
        internal static long ParseLong(string value) {
            return (value.Length == 0 || value.CaselessEq("null")) ? 0 : long.Parse(value);
        }
        
        internal static int ParseInt(string value) {
            return (value.Length == 0 || value.CaselessEq("null")) ? 0 : int.Parse(value);
        }
        
        internal static string ParseColor(string raw) {
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
        
        
        internal static long UnpackHi(long value) {
            return (value >> HiBitsShift) & HiBitsMask;
        }
        internal static long UnpackLo(long value) {
            return value & LoBitsMask;
        }
        internal static long Pack(long hi, long lo) {
            return hi << HiBitsShift | lo; 
        }

        public const int HiBitsShift = 38;
        public const long LoBitsMask = (1L << HiBitsShift) - 1;
        // convert negative to positive after shifting
        public const long HiBitsMask = (1L << 26) - 1;
    }
}
