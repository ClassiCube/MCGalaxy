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
using System.Collections.Generic;

namespace MCGalaxy 
{
    /// <summary> Extension methods relating to dates. </summary>
    public static class DateExts 
    {
        /// <summary> Origin point in time for Unix time (Midnight January 1, 1970) </summary>
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static DateTime FromUnixTime(this long offset) {
            return UnixEpoch.AddTicks(offset * TimeSpan.TicksPerSecond);
        }
        
        /// <summary> Converts the given DateTime instance to Unix time </summary>
        /// <remarks> Unix time is the number of seconds since Midnight January 1, 1970 </remarks>
        public static long ToUnixTime(this DateTime time) {
            return (long)(time.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }
        
        public static bool AddSpamEntry(this List<DateTime> log, int maxEntries, TimeSpan checkInterval) {
            DateTime now = DateTime.UtcNow;
            if (log.Count > 0 && log.Count >= maxEntries)
                log.RemoveAt(0);
            log.Add(now);
            
            if (log.Count < maxEntries) return true;
            TimeSpan oldestDelta = now - log[0];
            return oldestDelta > checkInterval;
        }
    }
}
