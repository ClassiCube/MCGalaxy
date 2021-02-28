/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Network;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdClear : Command2 {
        public override string name { get { return "Clear"; } }
        public override string shortcut { get { return "cls"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("PlayerCLS"), new CommandAlias("GlobalCLS", "global"), new CommandAlias("gcls", "global") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "can clear chat for everyone") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {        
            if (!message.CaselessEq("global")) {
                ClearChat(p);
                p.Message("&4Chat cleared.");
            } else {
                if (!CheckExtraPerm(p, data, 1)) return;
                
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    ClearChat(pl);
                }
                Chat.MessageAll("&4Global Chat cleared.");
            }
        }
        
        static void ClearChat(Player p) {
            for (int i = 0; i < 30; i++) {
                p.Send(Packet.BlankMessage());
            }
        }

        public override void Help(Player p) {
            p.Message("&T/Clear &H- Clears your chat.");
            p.Message("&T/Clear global &H- Clears chat of all users.");
        }
    }
}
