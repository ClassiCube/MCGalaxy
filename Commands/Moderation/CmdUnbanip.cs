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

namespace MCGalaxy.Commands {
    public sealed class CmdUnbanip : Command {
        public override string name { get { return "unbanip"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdUnbanip() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (message[0] == '@') {
                message = message.Remove(0, 1).Trim();
                Player who = PlayerInfo.Find(message);
                if (who == null) {
                    string ip = PlayerInfo.FindIP(message);
                    if (ip == null) { Player.Message(p, "Unable to find an IP address for that user."); return; }
                    message = ip;
                } else {
                    message = who.ip;
                }
            }

            if (message.IndexOf('.') == -1) { Player.Message(p, "Not a valid ip!"); return; }
            if (p != null) if (p.ip == message) { Player.Message(p, "You shouldn't be able to use this command..."); return; }
            if (!Server.bannedIP.Contains(message)) { Player.Message(p, message + " is not a banned IP."); return; }
            Server.bannedIP.Remove(message);
            Server.bannedIP.Save();

            string name = p == null ? "(console)" : p.name;
            string fullName = p == null ? "(console)" : p.ColoredName;
            Server.IRC.Say(message.ToLower() + " was un-ip-banned by " + name + ".");
            Server.s.Log("IP-UNBANNED: " + message.ToLower() + " by " + name + ".");
            Player.GlobalMessage(message + " was &8un-ip-banned %Sby " + fullName + "%S.");
        }
        
        public override void Help(Player p)  {
            Player.Message(p, "%T/unbanip <ip/player>");
            Player.Message(p, "%HUn-bans an ip. Also accepts a player name when you use @ before the name.");
        }
    }
}
