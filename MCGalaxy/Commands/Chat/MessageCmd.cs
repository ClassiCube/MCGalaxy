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
namespace MCGalaxy.Commands.Chatting {  
    public abstract class MessageCmd : Command {
        public override string type { get { return CommandTypes.Chat; } }
        
        protected bool TryMessageAction(Player p, string name, string message, bool messageWho) {
            if (name.Length == 0) { Help(p); return false; }
            Player target = PlayerInfo.FindMatches(p, name);
            if (target == null) return false;

            string giver = (p == null) ? "(console)" : p.ColoredName;
            string reciever = p == target ? "themselves" : target.ColoredName;
            if (!TryMessage(p, string.Format(message, giver, reciever))) return false;

            if (messageWho && p != target && Chat.NotIgnoring(target, p)) {
                Player.Message(target, string.Format(message, giver, "you"));
            }
            return true;
        }
        
        protected bool TryMessage(Player p, string message) { return TryMessage(p, message, name); }
        
        protected static bool TryMessage(Player p, string message, string cmd) {
            if (!CanSpeak(p, cmd)) return false;
            Chat.MessageGlobalOrLevel(p, message, null);
            
            p.CheckForMessageSpam();
            return true;
        }
        
        internal static bool CanSpeak(Player p, string cmd) {
            if (p == null) return true;
            
            if (p.muted) { 
                Player.Message(p, "Cannot use %T/{0} %Swhile muted.", cmd); return false; 
            }
            if (Server.chatmod && !p.voice) { 
                Player.Message(p, "Cannot use %T/{0} %Swhile chat moderation is on without %T/Voice%S.", cmd); return false; 
            }
            return true;
        }
    }
    
    public sealed class CmdHigh5 : MessageCmd {
        public override string name { get { return "High5"; } }
        
        public override void Use(Player p, string message) {
            TryMessageAction(p, message, "{0} %Sjust highfived {1}", true);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/High5 [player]");
            Player.Message(p, "%HHigh five someone! :D");
        }
    }
}
