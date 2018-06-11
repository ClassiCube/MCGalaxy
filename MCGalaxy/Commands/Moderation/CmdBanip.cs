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
using MCGalaxy.Events;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdBanip : Command {
        public override string name { get { return "BanIP"; } }
        public override string shortcut { get { return "bi"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("IPBan") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }            
            string[] args = message.SplitSpaces(2);
            args[0] = ModActionCmd.FindIP(p, args[0], "IP ban", "banip");
            if (args[0] == null) return;

            IPAddress ip;
            if (!IPAddress.TryParse(args[0], out ip)) { Player.Message(p, "\"{0}\" is not a valid IP.", args[0]); return; }
            if (IPAddress.IsLoopback(ip)) { Player.Message(p, "You cannot IP ban the server."); return; }
            if (p != null && p.ip == args[0]) { Player.Message(p, "You cannot IP ban yourself."); return; }
            if (Server.bannedIP.Contains(args[0])) { Player.Message(p, "{0} is already IP banned.", args[0]); return; }
            // Check if IP is shared by any other higher ranked accounts
            if (!CheckIP(p, args[0])) return;
            
            string reason = args.Length > 1 ? args[1] : "";
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;
            
            ModAction action = new ModAction(args[0], p, ModActionType.BanIP, reason);
            OnModActionEvent.Call(action);
        }
        
        static bool CheckIP(Player p, string ip) {
            if (p == null) return true;
            List<string> accounts = PlayerInfo.FindAccounts(ip);
            if (accounts == null || accounts.Count == 0) return true;
            
            foreach (string name in accounts) {
                Group grp = PlayerInfo.GetGroup(name);
                if (grp == null || grp.Permission < p.Rank) continue;
                
                Player.Message(p, "You can only IP ban IPs used by players with a lower rank.");
                Player.Message(p, name + "(" + grp.ColoredName + "%S) uses that IP.");
                
                Logger.Log(LogType.SuspiciousActivity, 
                           "{0} failed to ipban {1} - IP is also used by: {2}({3})", p.name, ip, name, grp.Name);
                return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/BanIP [ip/player] <reason>");
            Player.Message(p, "%HBans an IP, or the IP the given player is on.");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
