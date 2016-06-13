/*
    Copyright 2011 MCForge
    
    Author: fenderrock87
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Linq;

namespace MCGalaxy {    
    public static class TimeUtils {
        
        public static string Shorten(this TimeSpan value, bool seconds = false) {
            string time = "";
            if (value.Days >= 1) time = value.Days + "d " + value.Hours + "h " + value.Minutes + "m";
            else if (value.Hours >= 1) time = value.Hours + "h " + value.Minutes + "m";
            else time = value.Minutes + "m";
            if (seconds && value.Seconds != 0) time += " " + value.Seconds + "s";
            return time;
        }
        
        public static TimeSpan ParseShort(this string value) {
            int num = 0;
            long total = 0;
            
            foreach (char c in value) {
                long amount = 0;
                if (c == ' ') continue;                
                if (c >= '0' && c <= '9') {
                    num = checked(num * 10); num += (c - '0'); 
                    continue;
                }
                
                if (c == 's' || c == 'S') {
                    amount = num * TimeSpan.TicksPerSecond;
                } else if (c == 'm' || c == 'M') {
                    amount = num * TimeSpan.TicksPerMinute;
                } else if (c == 'h' || c == 'H') {
                    amount = num * TimeSpan.TicksPerHour;
                } else if (c == 'd' || c == 'D') {
                    amount = num * TimeSpan.TicksPerDay;
                } else {
                    throw new FormatException(c.ToString());
                }
                total = checked(total + amount);
                num = 0;
            }
            return TimeSpan.FromTicks(total);
        }
        
        public static bool TryParseShort(this string value, Player p, 
                                         string action, out TimeSpan span) {
		    span = TimeSpan.Zero;
            try {
                span = ParseShort(value);
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
        
        public static string ToDBTime(this TimeSpan value) {
            return value.Days + " " + value.Hours + " " + value.Minutes + " " + value.Seconds;
        }
        
        public static TimeSpan ParseDBTime(this string value) {
            string[] parts = value.Split(' ');
            return new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]),
                                int.Parse(parts[2]), int.Parse(parts[3]));
        }
    }
}
