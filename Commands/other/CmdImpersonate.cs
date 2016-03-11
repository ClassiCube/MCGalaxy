/*
    Copyright 2011 MCForge
        
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
namespace MCGalaxy.Commands
{
    public sealed class CmdImpersonate : Command
    {
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        public override bool museumUsable { get { return true; } }
        public override string name { get { return "impersonate"; } }
        public override string shortcut { get { return "imp"; } }
        public override string type { get { return CommandTypes.Other; } }
        static char[] trimChars = { ' ' };
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(trimChars, 2);
            Player who = PlayerInfo.FindOrShowMatches(p, args[0]);
            if (who == null) return;
            
            if (p == null || p == who || p.group.Permission > who.group.Permission) {
                Player.SendChatFrom(who, args[1]);
            } else {
                Player.SendMessage(p, "You cannot impersonate a player of equal or greater rank.");
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/impersonate <player> <message> - Sends a message as if it came from <player>");
        }
    }
}
