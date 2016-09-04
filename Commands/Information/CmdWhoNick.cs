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
using System.Data;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
    
    public sealed class CmdWhoNick : Command {
        
        public override string name { get { return "whonick"; } }
        public override string shortcut { get { return "realname"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdWhoNick() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            Player nick = PlayerInfo.FindNick(p, message);
            if (nick == null) {
                Player.Message(p, "The player is not online."); return;
            }
            Player.Message(p, "This player's real username is " + nick.name);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/whonick [nickname]");
            Player.Message(p, "%HDisplays the player's real username");
        }
    }
}
