/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdMe : Command2 {
        public override string name { get { return "Me"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool MessageBlockRestricted { get { return true; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { p.Message("You"); return; }
            if (p.joker) { p.Message("Cannot use /me while jokered."); return; }         
            if (!MessageCmd.CanSpeak(p, "Me")) return;
            
            string msg = p.color + "*" + Colors.StripUsed(p.DisplayName) + " " + message;
            Chat.MessageChat(p, msg, null, true);
        }
        
        public override void Help(Player p) {
            p.Message("What do you need help with, m'boy?! Are you stuck down a well?!");
        }
    }
}