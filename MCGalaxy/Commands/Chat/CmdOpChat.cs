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
namespace MCGalaxy.Commands {
    public sealed class CmdOpChat : Command {
        public override string name { get { return "opchat"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can read opchat messages") }; }
        }
        public CmdOpChat() { }

        public override void Use(Player p, string message) {
            if (message != "") { ChatModes.MessageOps(p, message); return; }
            
            p.opchat = !p.opchat;
            if (p.opchat) Player.Message(p, "All messages will now be sent to OPs only");
            else Player.Message(p, "OP chat turned off");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/opchat [message]");
            Player.Message(p, "%HSends a message to online OPs");
            Player.Message(p, "%T/opchat");
            Player.Message(p, "%HMakes all messages sent go to OPs by default");
        }
    }
}
