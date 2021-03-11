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

namespace MCGalaxy.DB {
    
    public delegate void OfflineStatPrinter(Player p, PlayerData who);
    
    /// <summary> Prints stats for an offline player in /info. </summary>
    public static class OfflineStat {

        /// <summary> List of stats that can be output to /info. </summary>
        public static List<OfflineStatPrinter> Stats = new List<OfflineStatPrinter>() {
            CoreLine,
            (p, who) => OnlineStat.MiscLine(p, who.Name, who.Deaths, who.Money),
            BlocksModifiedLine,
            (p, who) => OnlineStat.BlockStatsLine(p, who.TotalPlaced, who.TotalDeleted, who.TotalDrawn),
            TimeSpentLine,
            LoginLine,
            (p, who) => OnlineStat.LoginsLine(p, who.Logins, who.Kicks),
            (p, who) => OnlineStat.BanLine(p, who.Name),
            (p, who) => OnlineStat.SpecialGroupLine(p, who.Name),
            (p, who) => OnlineStat.IPLine(p, who.Name, who.IP),            
        };
        
        public static void CoreLine(Player p, PlayerData data) {
            Group group = Group.GroupIn(data.Name);
            string color = data.Color.Length == 0 ? group.Color : data.Color;
            string prefix = data.Title.Length == 0 ? "" : color + "[" + data.TitleColor + data.Title + color + "] ";
            string fullName = prefix + color + data.Name.RemoveLastPlus();
            OnlineStat.CommonCoreLine(p, fullName, data.Name, group, data.Messages);
        }
        
        public static void BlocksModifiedLine(Player p, PlayerData who) {
            p.Message("  Modified &a{0} &Sblocks", who.TotalModified);
        }
        
        public static void TimeSpentLine(Player p, PlayerData who) {
            p.Message("  Spent &a{0} &Son the server", who.TotalTime.Shorten());
        }
                
        public static void LoginLine(Player p, PlayerData who) {
            p.Message("  First login &a{0}&S, last login &a{1}",
                                 who.FirstLogin.ToString("yyyy-MM-dd"), who.LastLogin.ToString("yyyy-MM-dd"));
        }
    }
}