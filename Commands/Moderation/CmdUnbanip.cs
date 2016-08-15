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
using System.Net;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdUnbanip : Command {
        public override string name { get { return "unbanip"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdUnbanip() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            message = CmdBanip.GetIP(p, message, false);
            if (message == null) return;

            IPAddress ip;
            if (!IPAddress.TryParse(message, out ip)) { Player.Message(p, "\"{0}\" is not a valid IP.", message); return; }
            if (p != null && p.ip == message) { Player.Message(p, "You cannot un-ip-ban yourself."); return; }
            if (!Server.bannedIP.Contains(message)) { Player.Message(p, message + " is not a banned IP."); return; }

            string unbanner = p == null ? "(console)" : p.ColoredName;
            Server.IRC.Say(message.ToLower() + " was un-ip-banned by " + unbanner + ".");
            Server.s.Log("IP-UNBANNED: " + message.ToLower() + " by " + unbanner + ".");
            Chat.MessageAll("{0} was &8un-ip-banned %Sby {1}%S.", message, unbanner);
            
            Server.bannedIP.Remove(message);
            Server.bannedIP.Save();
        }
        
        public override void Help(Player p)  {
            Player.Message(p, "%T/unbanip <ip/player>");
            Player.Message(p, "%HUn-bans an IP, or the IP the given player is on.");
        }
    }
}
