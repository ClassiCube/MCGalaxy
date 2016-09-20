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
            
            if (time == "") time = seconds ? "0s" : "0m";
            return negate ? "-" + time : time;
        }
        
        public static TimeSpan ParseShort(this string value, char defUnit) {
            int num = 0;
            long amount = 0, total = 0;
            
            foreach (char c in value) {
                if (c == ' ') continue;
                if (c >= '0' && c <= '9') {
                    num = checked(num * 10); num += (c - '0');
                    continue;
                }
                
                amount = GetTicks(num, c);
                total = checked(total + amount);
                num = 0;
            }
            
            amount = GetTicks(num, defUnit);
            total = checked(total + amount);
            return TimeSpan.FromTicks(total);
        }
        
        public static bool TryParseShort(this string value, Player p, char defUnit,
                                         string action, out TimeSpan span) {
            span = TimeSpan.Zero;
            try {
                span = ParseShort(value, defUnit);
                return true;
            } catch (OverflowException) {
                Player.Message(p, "Timespan given is too big.");
            } catch (FormatException ex) {
                Player.Message(p, "{0} is not a valid quantifier.", ex.Message);
                Player.Message(p, Help, action);
            }
            return false;
        }
        
        public const string Help = "For example, to {0} 25 and a half hours, use \"1d1h30m\".";
        
        static void Add(ref string time, int amount, char suffix, bool spaces) {
            if (amount == 0) return;
            
            if (time == "") 
                time = "" + amount + suffix;
            else 
            	time = time + (spaces ? " " : "") + amount + suffix;
        }
        
        static long GetTicks(int num, char unit) {
            if (unit == 's' || unit == 'S')
                return num * TimeSpan.TicksPerSecond;
            if (unit == 'm' || unit == 'M')
                return num * TimeSpan.TicksPerMinute;
            if (unit == 'h' || unit == 'H')
                return num * TimeSpan.TicksPerHour;
            if (unit == 'd' || unit == 'D')
                return num * TimeSpan.TicksPerDay;
            if (unit == 'w' || unit == 'W')
                return num * TimeSpan.TicksPerDay * 7;
            throw new FormatException(unit.ToString());
        }
    }
}
