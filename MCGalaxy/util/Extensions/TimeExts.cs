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

namespace MCGalaxy {

    /// <summary> Extension methods relating to timespans. </summary>
    public static class TimeExts {
        
        public static string Shorten(this TimeSpan value, 
                                     bool seconds = false, bool spaces = true) {
            string time = "";
            bool negate = value.TotalSeconds < 0;
            if (negate) value = -value;
            
            Add(ref time, value.Days, 'd', spaces);
            Add(ref time, value.Hours, 'h', spaces);
            Add(ref time, value.Minutes, 'm', spaces);
            if (value.TotalMinutes <= 1 || (seconds && value.Days < 1))
                Add(ref time, value.Seconds, 's', spaces);
            
            if (time.Length == 0) time = seconds ? "0s" : "0m";
            return negate ? "-" + time : time;
        }
        
        public static long SecondsLong(this TimeSpan value) {
            return value.Ticks / TimeSpan.TicksPerSecond;
        }
        
        static void Add(ref string time, int amount, char suffix, bool spaces) {
            if (amount == 0) return;
            
            if (time.Length == 0) 
                time = "" + amount + suffix;
            else 
                time = time + (spaces ? " " : "") + amount + suffix;
        }

        
        public static TimeSpan ParseShort(this string value, string defaultUnit) {
            int num = 0;
            long amount = 0, total = 0;
            
            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
                if (c == ' ') continue;
                
                if (c >= '0' && c <= '9') {
                    num = checked(num * 10); num += (c - '0');
                    continue;
                }
                
                amount = GetTicks(num, GetUnit(value, i));
                total = checked(total + amount);
                num = 0;
            }
            
            amount = GetTicks(num, defaultUnit);
            total = checked(total + amount);
            return TimeSpan.FromTicks(total);
        }
        
        static long GetTicks(int num, string unit) {
            if (unit.CaselessEq("s")) return num * TimeSpan.TicksPerSecond;
            if (unit.CaselessEq("m")) return num * TimeSpan.TicksPerMinute;
            if (unit.CaselessEq("h")) return num * TimeSpan.TicksPerHour;
            if (unit.CaselessEq("d")) return num * TimeSpan.TicksPerDay;
            if (unit.CaselessEq("w")) return num * TimeSpan.TicksPerDay * 7;
            
            if (unit.CaselessEq("ms")) return num * TimeSpan.TicksPerMillisecond;
            throw new FormatException(unit);
        }
        
        static string GetUnit(string value, int i) {
            string unit = "";
            // Find all alphabetical chars
            for (; i < value.Length; i++) {
                char c = value[i];
                if (c == ' ') continue;
                
                if (c >= '0' && c <= '9') break;                
                unit += value[i];
            }
            return unit;
        }
    }
}
