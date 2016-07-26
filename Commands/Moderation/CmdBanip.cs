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
using System.Collections.Generic;
using System.Net;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdBanip : Command {
        public override string name { get { return "banip"; } }
        public override string shortcut { get { return "bi"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBanip() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            message = GetIP(p, message, true);
            if (message == null) return;

            IPAddress ip;
            if (!IPAddress.TryParse(message, out ip)) { Player.Message(p, "\"{0}\" is not a valid IP.", message); return; }
            if (IPAddress.IsLoopback(ip)) { Player.Message(p, "You cannot ip-ban the server."); return; }
            if (p != null && p.ip == message) { Player.Message(p, "You cannot ip-ban yourself."); return; }
            if (Server.bannedIP.Contains(message)) { Player.Message(p, "{0} is already ip-banned.", message); return; }
            // Check if IP is shared by any other higher ranked accounts
            if (!CheckIP(p, message)) return;

            string banner = p == null ? "(console)" : p.ColoredName;
            Server.IRC.Say(message.ToLower() + " was ip-banned by " + banner + "%S.");
            Server.s.Log("IP-BANNED: " + message.ToLower() + " by " + banner + ".");
            Player.GlobalMessage(message + " was &8ip-banned %Sby " + banner + "%S.");
            
            Server.bannedIP.Add(message);
            Server.bannedIP.Save();
        }
        
        internal static string GetIP(Player p, string message, bool ban) {
            IPAddress ip;
            // TryParse returns "0.0.0.123" for "123", we do not want that behaviour
            if (IPAddress.TryParse(message, out ip) && message.Split('.').Length == 4) {
            	string account = Server.ClassicubeAccountPlus ? message + "+" : message;
                if (PlayerInfo.FindName(account) == null) return message;

                // Some classicube.net accounts can be parsed as valid IPs, so warn in this case.
                Player.Message(p, "Note: \"{0}\" is an IP, but is also an account name. "
                               + "If you meant to {1} the account, use %T/{2} @{0}",
                               message, ban ? "ip-ban" : "un-ip-ban", ban ? "banip" : "unbanip");
                return message;
            }
            
            if (message[0] == '@') message = message.Remove(0, 1);
            Player who = PlayerInfo.FindMatches(p, message);
            if (who != null) return who.ip;
            
            Player.Message(p, "Searching PlayerDB..");
            OfflinePlayer target = PlayerInfo.FindOfflineMatches(p, message);
            return target == null ? null : target.ip;
        }
        
        static bool CheckIP(Player p, string ip) {
            if (p == null) return true;
            List<string> alts = PlayerInfo.FindAccounts(ip);
            if (alts == null || alts.Count == 0) return true;
            
            foreach (string name in alts) {
                Group grp = Group.findPlayerGroup(name);
                if (grp == null || grp.Permission < p.Rank) continue;
                
                Player.Message(p, "You can only ipban IPs used by players with a lower rank.");
                Player.Message(p, name + "(" + grp.ColoredName + "%S) uses that IP.");
                Server.s.Log(p.name + "failed to ipban " + ip + " - IP is also used by: " + name + "(" + grp.name + ")");
                return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/banip <ip/player>");
            Player.Message(p, "%HBans an IP, or the IP the given player is on.");
        }
    }
}
