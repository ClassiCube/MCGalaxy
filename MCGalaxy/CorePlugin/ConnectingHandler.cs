/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.Security.Cryptography;
using MCGalaxy.Events;

namespace MCGalaxy.Core {
    internal static class ConnectingHandler {
        
        internal static void HandleConnecting(Player p, string mppass) {
            bool success = HandleConnectingCore(p, mppass);
            if (success) return;
            Plugin.CancelPlayerEvent(PlayerEvents.PlayerConnecting, p);
        }
        
        static bool HandleConnectingCore(Player p, string mppass) {
            if (p.truename.Length > 16) {
                p.Leave(null, "Usernames must be 16 characters or less", true); return false;
            }
            if (!Player.ValidName(p.truename)) {
                p.Leave(null, "Invalid player name", true); return false;
            }
      
            if (!VerifyName(p, mppass)) return false;
            if (!IPThrottler.CheckIP(p)) return false;
            if (!CheckPendingAlts(p)) return false;
            if (!CheckTempban(p)) return false;

            bool whitelisted = CheckWhitelist(p.name, p.ip);
            if (!whitelisted) {
                p.Leave(null, "This is a private server!", true);
                return false;
            }
            
            Group group = Group.findPlayerGroup(p.name);
            if (!CheckBanned(group, p, whitelisted)) return false;
            if (!CheckPlayersCount(group, p)) return false;
            if (!CheckOnlinePlayers(p)) return false;    
            p.group = group;
            return true;
        }
        
        static System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        static object md5Lock = new object();        
        
        static bool CheckPendingAlts(Player p) {
            int altsCount = 0;
            lock (Player.pendingLock) {
                DateTime now = DateTime.UtcNow;
                foreach (Player.PendingItem item in Player.pendingNames) {
                    if (item.Name == p.truename && (now - item.Connected).TotalSeconds <= 60)
                        altsCount++;
                }
                Player.pendingNames.Add(new Player.PendingItem(p.truename));
            }
            
            if (altsCount > 0) {
                p.Leave(null, "Already logged in!", true); return false;
            }
            return true;
        }
        
        static bool VerifyName(Player p, string mppass) {
            p.verifiedName = false;
            if (!Server.verify) return true;
            
            byte[] hash = null;
            lock (md5Lock)
                hash = md5.ComputeHash(enc.GetBytes(Server.salt + p.truename));
            
            string hashHex = BitConverter.ToString(hash);
            if (!mppass.CaselessEq(hashHex.Replace("-", ""))) {
                if (!Player.IPInPrivateRange(p.ip)) {
                    p.Leave(null, "Login failed! Close the game and sign in again.", true); return false;
                }
            } else {
                p.verifiedName = true;
            }
            return true;
        }
        
        static bool CheckTempban(Player p) {
            try {
                string data = Server.tempBans.FindData(p.name);
                if (data == null) return true;
                
                string banner, reason;
                DateTime expiry;
                Ban.UnpackTempBanData(data, out reason, out banner, out expiry);
                
                if (expiry < DateTime.UtcNow) {
                    Server.tempBans.Remove(p.name);
                    Server.tempBans.Save();
                } else {
                    reason = reason == "" ? "" :" (" + reason + ")";
                    string delta = (expiry - DateTime.UtcNow).Shorten(true);
                    
                    p.Kick(null, "Banned by " + banner + " for another " + delta + reason, true);
                    return false;
                }
            } catch { }
            return true;
        }

        static bool CheckWhitelist(string name, string ip) {
            if (!Server.useWhitelist) return true;
            if (Server.verify) return Server.whiteList.Contains(name);
            
            // Verify names is off, check if the player is on the same IP.
            return Server.whiteList.Contains(name) && PlayerInfo.FindAccounts(ip).Contains(name);
        }
        
        static bool CheckPlayersCount(Group group, Player p) {
            if (Server.vip.Contains(p.name)) return true;
            
            Player[] online = PlayerInfo.Online.Items;
            if (online.Length >= Server.players && !Player.IPInPrivateRange(p.ip)) {
                p.Leave(null, "Server full!", true); return false;
            }
            if (group.Permission > LevelPermission.Guest) return true;
            
            online = PlayerInfo.Online.Items;
            int guests = 0;
            foreach (Player pl in online) {
                if (pl.Rank <= LevelPermission.Guest) guests++;
            }
            if (guests < Server.maxGuests) return true;
            
            if (Server.guestLimitNotify) Chat.MessageOps("Guest " + p.truename + " couldn't log in - too many guests.");
            Server.s.Log("Guest " + p.truename + " couldn't log in - too many guests.");
            p.Leave(null, "Server has reached max number of guests", true);
            return false;
        }
        
        static bool CheckOnlinePlayers(Player p) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.name != p.name) continue;
                
                if (Server.verify) {
                    string reason = pl.ip == p.ip ? "(Reconnecting)" : "(Reconnecting from a different IP)";
                    pl.Leave(reason); break;
                } else {
                    p.Leave(null, "Already logged in!", true);
                    return false;
                }
            }
            return true;
        }
        
        static bool CheckBanned(Group group, Player p, bool whitelisted) {
            if (Server.bannedIP.Contains(p.ip) && (!Server.useWhitelist || !whitelisted)) {
                p.Kick(null, Server.defaultBanMessage, true);
                return false;
            }
            if (group.Permission == LevelPermission.Banned) {
                string[] data = Ban.GetBanData(p.name);
                if (data != null) {
                    p.Kick(null, Ban.FormatBan(data[0], data[1]), true);
                } else {
                    p.Kick(null, Server.defaultBanMessage, true);
                }
                return false;
            }
            return true;
        }
    }
}
