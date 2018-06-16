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
        
        protected bool TryMessageAction(Player p, string name, string msg, bool messageWho) {
            if (name.Length == 0) { Help(p); return false; }
            Player target = PlayerInfo.FindMatches(p, name);
            if (target == null) return false;

            string reciever = p == target ? "themselves" : target.ColoredName;
            if (!TryMessage(p, msg.Replace("λTARGET", reciever))) return false;

            if (messageWho && p != target && !Chat.Ignoring(target, p)) {
                string giver = (p == null) ? "(console)" : p.ColoredName;
                msg = msg.Replace("λNICK", giver);
                Player.Message(target, msg.Replace("λTARGET", "you"));
            }
            return true;
        }
        
        protected bool TryMessage(Player p, string msg) {
            if (!CanSpeak(p, name)) return false;
            Chat.MessageFrom(p, msg, null);
            
            p.CheckForMessageSpam();
            return true;
        }
        
        public static bool CanSpeak(Player p, string cmd) {
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
            TryMessageAction(p, message, "λNICK %Sjust highfived λTARGET", true);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/High5 [player]");
            Player.Message(p, "%HHigh five someone! :D");
        }
    }
}
