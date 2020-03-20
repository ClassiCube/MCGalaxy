/*
    Copyright 2015 MCGalaxy
    
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
using System.Collections.Generic;
using MCGalaxy.SQL;

namespace MCGalaxy.DB {
    
    /// <summary> Retrieves a title for a column. </summary>
    /// <remarks> Title is displayed on a line before the values of that column. </remarks>
    public delegate string TopStatTitle();
    
    /// <summary> Formats a value in a column. </summary>
    public delegate string TopStatFormatter(string input);
    
    
    /// <summary> Outputs ordered stats from a column in a database table. </summary>
    public sealed class TopStat {
        
        public readonly string Identifier, Table, Column, OrderBy;
        public readonly TopStatTitle Title;
        public readonly TopStatFormatter Formatter;
        
        public TopStat(string identifier, string table, string col, TopStatTitle title,
                       TopStatFormatter formatter, bool ascending = false, string orderBy = null) {
            Identifier = identifier;
            Table = table;
            Column = col;
            Title = title;
            Formatter = formatter;
            
            OrderBy = orderBy;
            if (OrderBy == null)
                OrderBy = " " + col + " ";            
            OrderBy += (ascending ? "asc" : "desc");
        }
        
        /// <summary> List of stats that can be ordered. </summary>
        public static List<TopStat> Stats = new List<TopStat>() {
            new TopStat("Logins", "Players",
                        PlayerData.ColumnLogins, MostLogins, FormatInteger),
            new TopStat("Deaths", "Players",
                        PlayerData.ColumnDeaths, MostDeaths, FormatInteger),
            new TopStat("Money", "Players",
                        PlayerData.ColumnMoney, MostMoney, FormatInteger),            
            new TopStat("Oldest", "Players", 
                        PlayerData.ColumnFirstLogin, MostOldest, FormatDate, true),
            new TopStat("Newest", "Players",
                        PlayerData.ColumnFirstLogin, MostNewest, FormatDate),
            new TopStat("Recent", "Players",
                        PlayerData.ColumnLastLogin, MostRecent, FormatDate),
            new TopStat("Least-Recent", "Players",
                        PlayerData.ColumnLastLogin, MostNotRecent, FormatDate, true),
            new TopStat("Kicked", "Players", 
                        PlayerData.ColumnKicked, MostKicked, FormatInteger),
            new TopStat("Modified", "Players",
                        PlayerData.ColumnBlocks + " & " + PlayerData.LoBitsMask,
                        MostModified, FormatInteger),
            new TopStat("Drawn", "Players",
                        PlayerData.ColumnDrawn + " & " + PlayerData.LoBitsMask,
                        MostDrawn, FormatInteger),
            new TopStat("Placed", "Players",
                        // TODO: Check if this works on MySQL too
                        PlayerData.ColumnBlocks + " >> " + PlayerData.HiBitsShift + " & " + PlayerData.HiBitsMask,
                        MostPlaced, FormatInteger),
            new TopStat("Deleted", "Players",
                        PlayerData.ColumnDrawn  + " >> " + PlayerData.HiBitsShift + " & " + PlayerData.HiBitsMask,
                        MostDeleted, FormatInteger),
            new TopStat("TimeSpent", "Players",
                        PlayerData.ColumnTimeSpent, MostTime, FormatTimespan,
                        false, " CAST(TimeSpent as unsigned) "),
            new TopStat("Messages", "Players",
                        PlayerData.ColumnMessages, MostMessages, FormatInteger),
        };
        
        static string MostLogins()    { return "Most logins"; }
        static string MostDeaths()    { return "Most deaths"; }
        static string MostMoney()     { return "Most " + Server.Config.Currency; }
        static string MostNewest()    { return "Newest players"; }
        static string MostOldest()    { return "Oldest players"; }
        static string MostRecent()    { return "Most recent players"; }
        static string MostNotRecent() { return "Least recent players"; }
        static string MostKicked()    { return "Most times kicked"; }
        static string MostModified()  { return "Most blocks modified"; }
        static string MostDrawn()     { return "Most blocks drawn"; }
        static string MostPlaced()    { return "Most blocks placed"; }
        static string MostDeleted()   { return "Most blocks deleted"; }
        static string MostTime()      { return "Most time spent"; }
        static string MostMessages()  { return "Most messages written"; }
        
        public static string FormatInteger(string input) {
            long value = PlayerData.ParseLong(input);
            return value.ToString("N0");
        }
        
        public static string FormatTimespan(string input) {
            long value = PlayerData.ParseLong(input);
            return TimeSpan.FromSeconds(value).Shorten(true);
        }
        
        public static string FormatDate(string input) {
            DateTime time = input.ParseDBDate();
            TimeSpan delta = DateTime.Now - time;
            return string.Format("{0:H:mm} on {0:d} ({1} ago)", time, delta.Shorten());
        }
    }
}