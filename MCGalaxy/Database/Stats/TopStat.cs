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

namespace MCGalaxy.DB 
{ 
    /// <summary> Formats a raw stat value </summary>
    public delegate string TopStatFormatter(string input);
    
    public struct TopResult { public string Name, Value; }
    
    /// <summary> Outputs ordered stats from an underlying data source </summary>
    /// <example> Most TopStats are read from a column in a database table </example>
    public abstract class TopStat 
    {    
        public string Identifier, Title;
        public TopStatFormatter Formatter;
        
        public TopStat(string identifier, string title, TopStatFormatter formatter) {
            Identifier = identifier;
            Title      = title;
            Formatter  = formatter;
        }
        
        
        public virtual string FormatName(Player p, string name) {
            return p.FormatNick(name);
        }
        
        /// <summary> Retrieves unformatted results from the underlying data source </summary>
        public abstract List<TopResult> GetResults(int maxResults, int offset);
        
        
        public static TopStat Find(string name) {
            foreach (TopStat stat in stats) 
            {
                if (stat.Identifier.CaselessEq(name)) return stat;
            }
            return null;
        }
        
        public static void List(Player p) {
            p.Message("&f" + stats.Join(stat => stat.Identifier));
        }
        
        public static void Register(TopStat stat) {
            stats.Add(stat);
        }
        
        public static void Unregister(TopStat stat) {
            stats.Remove(stat);
        }
        
        
        static List<TopStat> stats = new List<TopStat>() {
            new DBTopStat("Logins", "Most logins", "Players",
                        PlayerData.ColumnLogins, FormatInteger),
            new DBTopStat("Deaths", "Most deaths", "Players",
                        PlayerData.ColumnDeaths, FormatInteger),
            new DBTopStat("Money", "Most $currency", "Players",
                        PlayerData.ColumnMoney, FormatInteger),            
            new DBTopStat("Oldest", "Oldest players", "Players", 
                        PlayerData.ColumnFirstLogin, FormatDate, true),
            new DBTopStat("Newest", "Newest players", "Players",
                        PlayerData.ColumnFirstLogin, FormatDate),
            new DBTopStat("Recent", "Most recent players", "Players",
                        PlayerData.ColumnLastLogin, FormatDate),
            new DBTopStat("Least-Recent", "Least recent players", "Players",
                        PlayerData.ColumnLastLogin, FormatDate, true),
            new DBTopStat("Kicked", "Most times kicked", "Players", 
                        PlayerData.ColumnKicked, FormatInteger),
            new DBTopStat("Modified", "Most blocks modified", "Players",
                        PlayerData.ColumnBlocks + " & " + PlayerData.LoBitsMask, FormatInteger),
            new DBTopStat("Drawn", "Most blocks drawn", "Players",
                        PlayerData.ColumnDrawn + " & " + PlayerData.LoBitsMask, FormatInteger),
            new DBTopStat("Placed", "Most blocks placed", "Players",
                        // TODO: Check if this works on MySQL too
                        PlayerData.ColumnBlocks + " >> " + PlayerData.HiBitsShift + " & " + PlayerData.HiBitsMask,
                        FormatInteger),
            new DBTopStat("Deleted", "Most blocks deleted", "Players",
                        PlayerData.ColumnDrawn  + " >> " + PlayerData.HiBitsShift + " & " + PlayerData.HiBitsMask,
                        FormatInteger),
            new DBTopStat("TimeSpent", "Most time spent", "Players",
                        PlayerData.ColumnTimeSpent, FormatTimespan,
                        false, " CAST(TimeSpent as unsigned) "),
            new DBTopStat("Messages", "Most messages written", "Players",
                        PlayerData.ColumnMessages, FormatInteger),
        };
        
        
        public static string FormatInteger(string input) {
            long value = PlayerData.ParseLong(input);
            return value.ToString("N0");
        }
        
        public static string FormatTimespan(string input) {
            long value = PlayerData.ParseLong(input);
            return TimeSpan.FromSeconds(value).Shorten(true);
        }
        
        public static string FormatDate(string input) {
            DateTime time  = Database.ParseDBDate(input);
            TimeSpan delta = DateTime.Now - time;
            return string.Format("{0:H:mm} on {0:d} ({1} ago)", time, delta.Shorten());
        }
    }
    
    public class DBTopStat : TopStat
    {    
        public string Table, Column, OrderBy;
        
        public DBTopStat(string identifier, string title, string table, string column,
                         TopStatFormatter formatter, bool ascending = false, string orderBy = null) 
                        : base(identifier, title, formatter) {
            Table  = table;
            Column = column;
            
            OrderBy = orderBy;
            if (OrderBy == null)
                OrderBy = " " + column + " ";
            OrderBy += (ascending ? "asc" : "desc");
        }
        
        
        public override List<TopResult> GetResults(int maxResults, int offset) {
            string limit = " LIMIT " + offset + "," + maxResults;
            List<TopResult> stats = new List<TopResult>();
            
            Database.ReadRows(Table, "DISTINCT Name, " + Column, 
                              record => stats.Add(ParseRow(record)),
                              "ORDER BY" + OrderBy + limit);
            return stats;
        }
        
        static TopResult ParseRow(ISqlRecord record) {
            TopResult result;
            result.Name  = record.GetStringValue(0);
            result.Value = record.GetStringValue(1);
            return result;
        }     
    }
}