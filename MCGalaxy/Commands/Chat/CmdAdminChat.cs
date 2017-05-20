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
namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdAdminChat : Command {
        public override string name { get { return "adminchat"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can read adminchat messages") }; }
        }

        public override void Use(Player p, string message) {
            if (message != "") { ChatModes.MessageAdmins(p, message); return; }
            
            p.adminchat = !p.adminchat;
            if (p.adminchat) Player.Message(p, "All messages will now be sent to Admins only");
            else Player.Message(p, "Admin chat turned off");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/adminchat [message]");
            Player.Message(p, "%HSends a message to online Admins");
            Player.Message(p, "%T/adminchat");
            Player.Message(p, "%HMakes all messages sent go to Admins by default");
        }
    }
}


