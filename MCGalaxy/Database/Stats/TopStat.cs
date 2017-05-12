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
using System.IO;

namespace MCGalaxy.DB {
    
    /// <summary> Outputs ordered stats from a column in a database table. </summary>
    public sealed class TopStat {
        
        public readonly string Identifier, Table, Column, OrderBy;
        public readonly Func<string> Title;
        public readonly Func<string, string> Formatter;
        
        public TopStat(string identifier, string table, string col, Func<string> title,
                       Func<string, string> formatter, bool ascending = false, string orderBy = null) {
            Identifier = identifier;
            Table = table;
            Column = col;
            Title = title;
            Formatter = formatter;
            
            OrderBy = orderBy;
            if (OrderBy == null)
                OrderBy = " " + col + " ";            
            OrderBy += (ascending ? " asc" : " desc");
        }
        
        /// <summary> List of stats that can be ordered. </summary>
        public static List<TopStat> Stats = new List<TopStat>() {
            new TopStat("Logins", PlayerData.DBTable, 
                        PlayerData.ColumnLogins,
                        () => "Most logins", FormatInteger),
            new TopStat("Deaths", PlayerData.DBTable, 
                        PlayerData.ColumnDeaths,
                        () => "Most deaths", FormatInteger),
            new TopStat("Money", PlayerData.DBTable, 
                        PlayerData.ColumnMoney,
                        () => "Most " + Server.moneys, FormatInteger),
            
            new TopStat("Oldest", PlayerData.DBTable, 
                        PlayerData.ColumnFirstLogin,
                        () => "Oldest players", FormatDate, true),
            new TopStat("Recent", PlayerData.DBTable, 
                        PlayerData.ColumnLastLogin,
                        () => "Most recent players", FormatDate),
            new TopStat("Newest", PlayerData.DBTable,
                        PlayerData.ColumnFirstLogin,
                        () => "Newest players", FormatDate),
            new TopStat("Least-Recent", PlayerData.DBTable,
                        PlayerData.ColumnLastLogin,
                        () => "Least Recent players", FormatDate, true),

            new TopStat("Kicked", PlayerData.DBTable, 
                        PlayerData.ColumnKicked,
                        () => "Most times kicked", FormatInteger),
            new TopStat("Modified", PlayerData.DBTable, 
                        PlayerData.ColumnTotalBlocks + " & " + PlayerData.LowerBitsMask,
                        () => "Most blocks modified", FormatInteger),
            new TopStat("Drawn", PlayerData.DBTable,
                        PlayerData.ColumnTotalCuboided + " & " + PlayerData.LowerBitsMask,
                        () => "Most blocks drawn", FormatInteger),
            new TopStat("Placed", PlayerData.DBTable,
                        PlayerData.ColumnTotalBlocks + " >> " + PlayerData.LowerBits,
                        () => "Most blocks placed", FormatInteger),
            new TopStat("Deleted", PlayerData.DBTable, 
                        PlayerData.ColumnTotalCuboided + " >> " + PlayerData.LowerBits,
                        () => "Most blocks deleted", FormatInteger),
            new TopStat("TimeSpent", PlayerData.DBTable, 
                        PlayerData.ColumnTimeSpent,
                        () => "Most time spent", FormatTimespan,
                        false, " CAST(TimeSpent as BIGINT) "),
        };
        
        public static string FormatInteger(string input) {
            long value = PlayerData.ParseLong(input);
            return value.ToString("N0");
        }
        
        public static string FormatTimespan(string input) {
            long value = PlayerData.ParseLong(input);
            return TimeSpan.FromSeconds(value).Shorten(true, true);
        }
        
        public static string FormatDate(string input) {
            DateTime time = DateTime.Parse(input);
            TimeSpan delta = DateTime.Now - time;
            return String.Format("{0:H:mm} on {0:d} ({1} ago)", time, delta.Shorten());
        }
    }
}