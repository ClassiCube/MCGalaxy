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
using System.Net;
using System.Net.Sockets;
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Network;
using MCGalaxy.Tasks;

namespace MCGalaxy.Modules.Security
{
    public sealed class IPThrottler : Plugin 
    {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.Version; } }
        public override string name { get { return "IPThrottler"; } }
        
        SchedulerTask clearTask;        
        readonly Dictionary<string, IPThrottleEntry> ips = new Dictionary<string, IPThrottleEntry>();
        readonly object ipsLock = new object();
        
        public override void Load(bool startup) {
            OnPlayerStartConnectingEvent.Register(HandleConnecting, Priority.System_Level);
            OnConnectionReceivedEvent.Register(HandleConnectionReceived, Priority.System_Level);
            clearTask = Server.Background.QueueRepeat(CleanupTask, null, TimeSpan.FromMinutes(10));
        }
        
        public override void Unload(bool shutdown) {
            OnPlayerStartConnectingEvent.Unregister(HandleConnecting);
            OnConnectionReceivedEvent.Unregister(HandleConnectionReceived);
            Server.Background.Cancel(clearTask);
        }
        
        void HandleConnectionReceived(Socket s, ref bool cancel) {
            IPAddress ip = SocketUtil.GetIP(s);
            if (!Server.Config.IPSpamCheck || IPAddress.IsLoopback(ip)) return;
            
            DateTime now = DateTime.UtcNow;
            string ipStr = ip.ToString();
            
            lock (ipsLock) {
                IPThrottleEntry entry;
                if (!ips.TryGetValue(ipStr, out entry)) {
                    entry = new IPThrottleEntry();
                    ips[ipStr] = entry;
                }
                
                // Check if that IP is repeatedly trying to connect
                if (entry.BlockedUntil < now) {
                    if (!entry.AddSpamEntry(Server.Config.IPSpamCount, Server.Config.IPSpamInterval)) {
                        entry.BlockedUntil = now.Add(Server.Config.IPSpamBlockTime);
                    }
                    return;
                }
                
                // If still connecting despite getting kick message 10 times, 
                //  treat this as an automated DOS attempt by a bot
                entry.FailedLogins++;
                if (entry.FailedLogins > 10) cancel = true;
            }
        }
        
        void HandleConnecting(Player p, string mppass) {
            if (!Server.Config.IPSpamCheck) return;
            DateTime now = DateTime.UtcNow;
            DateTime blockedUntil;
            
            // Most of work is done on initial connection
            lock (ipsLock) {
                IPThrottleEntry entry;
                if (!ips.TryGetValue(p.ip, out entry)) return;
                
                blockedUntil = entry.BlockedUntil;
                if (blockedUntil < now) return;
            }
            
            // do this outside lock since we want to minimise time spent locked
            TimeSpan delta = blockedUntil - now;
            p.Leave("Too many connections too quickly! Wait " + delta.Shorten(true) + " before joining");            
            p.cancelconnecting = true;
        }
        
        
        class IPThrottleEntry : List<DateTime> {
            public DateTime BlockedUntil;
            public int FailedLogins;
        }
        
        void CleanupTask(SchedulerTask task) {
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
