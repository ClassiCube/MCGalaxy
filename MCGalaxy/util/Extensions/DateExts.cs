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

namespace MCGalaxy {
    public static class DateExts {
        
        public static string ToDBTime(this TimeSpan value) {
            return value.Days + " " + value.Hours + " " + value.Minutes + " " + value.Seconds;
        }
        
        public static TimeSpan ParseDBTime(this string value) {
            string[] parts = value.Split(' ');
            return new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]),
                                int.Parse(parts[2]), int.Parse(parts[3]));
        }
        
        public static bool AddSpamEntry(this List<DateTime> log, int maxEntries, int checkInterval) {
            DateTime now = DateTime.UtcNow;
            if (log.Count > 0 && log.Count >= maxEntries)
                log.RemoveAt(0);
            log.Add(now);
            
            if (log.Count < maxEntries) return true;
            TimeSpan oldestDelta = now - log[0];
            return oldestDelta.TotalSeconds > checkInterval;
        }
    }
}
