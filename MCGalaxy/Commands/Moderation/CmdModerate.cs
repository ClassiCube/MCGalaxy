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
namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdModerate : Command {
        public override string name { get { return "moderate"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdModerate() { }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }

            if (Server.chatmod) {
                Chat.MessageGlobal("Chat moderation has been disabled. Everyone can now speak.");
            } else {
                Chat.MessageGlobal("Chat moderation engaged! Silence the plebians!");
            }
            Server.chatmod = !Server.chatmod;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/moderate %H- Toggles chat moderation status.");
            Player.Message(p, "%HWhen enabled, only players with %T/voice %Hmay speak.");
        }
    }
}
