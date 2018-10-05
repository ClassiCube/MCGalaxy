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
using MCGalaxy.Network;
using MCGalaxy.Tasks;

namespace MCGalaxy.Core {

    internal static class IPThrottler {
        
        static readonly Dictionary<string, IPThrottleEntry> ips = new Dictionary<string, IPThrottleEntry>();
        static readonly object ipsLock = new object();
        
        internal static bool CheckIP(Player p) {
            if (!Server.Config.IPSpamCheck || HttpUtil.IsLocalIP(p.ip)) return true;
            DateTime blockedUntil, now = DateTime.UtcNow;
            
            lock (ipsLock) {
                IPThrottleEntry entry;
                if (!ips.TryGetValue(p.ip, out entry)) {
                    entry = new IPThrottleEntry();
                    ips[p.ip] = entry;
                }
                blockedUntil = entry.BlockedUntil;
                
                if (blockedUntil < now) {
                    if (!entry.AddSpamEntry(Server.Config.IPSpamCount, Server.Config.IPSpamInterval)) {
                        entry.BlockedUntil = now.Add(Server.Config.IPSpamBlockTime);
                    }
                    return true;
                }
            }
            
            // do this outside lock since we want to minimise time spent locked
            TimeSpan delta = blockedUntil - now;
            p.Leave("Too many connections too quickly! Wait " + delta.Shorten(true) + " before joining");            
            return false;
        }
        
        class IPThrottleEntry : List<DateTime> {
            public DateTime BlockedUntil;
        }
        
        internal static void CleanupTask(SchedulerTask task) {
            lock (ipsLock) {
                if (!Server.Config.IPSpamCheck) { ips.Clear(); return; }
                
                // Find all connections which last joined before the connection spam check interval
                DateTime threshold = DateTime.UtcNow.Add(-Server.Config.IPSpamInterval);
                List<string> expired = null;
                foreach (var kvp in ips) {
                    DateTime lastJoin = kvp.Value[kvp.Value.Count - 1];
                    if (lastJoin >= threshold) continue;
                    
                    if (expired == null) expired = new List<string>();
                    expired.Add(kvp.Key);
                }
                
                if (expired == null) return;
                foreach (string ip in expired) {
                    ips.Remove(ip);
                }
            }
        }
    }
}
