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
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ipban") }; }
        }
        public CmdBanip() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            message = ModActionCmd.FindIP(p, message, "IP ban", "banip");
            if (message == null) return;

            IPAddress ip;
            if (!IPAddress.TryParse(message, out ip)) { Player.Message(p, "\"{0}\" is not a valid IP.", message); return; }
            if (IPAddress.IsLoopback(ip)) { Player.Message(p, "You cannot IP ban the server."); return; }
            if (p != null && p.ip == message) { Player.Message(p, "You cannot IP ban yourself."); return; }
            if (Server.bannedIP.Contains(message)) { Player.Message(p, "{0} is already IP banned.", message); return; }
            // Check if IP is shared by any other higher ranked accounts
            if (!CheckIP(p, message)) return;

            string banner = p == null ? "(console)" : p.ColoredName;
            string normMsg = String.Format("An IP was &8banned %Sby {0}%S.", banner);
            string opsMsg = String.Format("{1} was &8IP banned %Sby {0}%S.", banner, message);
            
            Server.IRC.Say(normMsg, false);
            Server.IRC.Say(opsMsg, true);            
            int seeIPperm = CommandOtherPerms.GetPerm(Command.all.Find("whois"));
            Chat.MessageWhere(normMsg, pl => (int)pl.Rank < seeIPperm);
            Chat.MessageWhere(opsMsg, pl => (int)pl.Rank >= seeIPperm);
            Server.s.Log("IP-BANNED: " + message.ToLower() + " by " + banner + ".");
            
            Server.bannedIP.Add(message);
            Server.bannedIP.Save();
        }
        
        static bool CheckIP(Player p, string ip) {
            if (p == null) return true;
            List<string> accounts = PlayerInfo.FindAccounts(ip);
            if (accounts == null || accounts.Count == 0) return true;
            
            foreach (string name in accounts) {
                Group grp = Group.findPlayerGroup(name);
                if (grp == null || grp.Permission < p.Rank) continue;
                
                Player.Message(p, "You can only IP ban IPs used by players with a lower rank.");
                Player.Message(p, name + "(" + grp.ColoredName + "%S) uses that IP.");
                Server.s.Log(p.name + "failed to ipban " + ip + " - IP is also used by: " + name + "(" + grp.name + ")");
                return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/banip [ip/player]");
            Player.Message(p, "%HBans an IP, or the IP the given player is on.");
        }
    }
}
