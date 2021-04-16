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
    public abstract class MessageCmd : Command2 {
        public override string type { get { return CommandTypes.Chat; } }
        
        protected bool TryMessageAction(Player p, string name, string msg, bool messageWho) {
            if (name.Length == 0) { Help(p); return false; }
            Player target = PlayerInfo.FindMatches(p, name);
            if (target == null) return false;

            string reciever = p == target ? "themselves" : target.ColoredName;
            if (!TryMessage(p, msg.Replace("λTARGET", reciever))) return false;

            if (messageWho && p != target && !Chat.Ignoring(target, p)) {
                msg = msg.Replace("λNICK", target.FormatNick(p));
                target.Message(msg.Replace("λTARGET", "you"));
            }
            return true;
        }
        
        protected bool TryMessage(Player p, string msg) { return TryMessage(p, msg, false); }
        
        protected bool TryMessage(Player p, string msg, bool relay) {
            if (!CanSpeak(p, name)) return false;
            Chat.MessageFrom(p, msg, null, relay);
            
            p.CheckForMessageSpam();
            return true;
        }
        
        public static bool CanSpeak(Player p, string cmd) {
            return p.CheckCanSpeak("use &T/" + cmd);
        }
    }
    
    public sealed class CmdHigh5 : MessageCmd {
        public override string name { get { return "High5"; } }
        
        public override void Use(Player p, string message, CommandData data) {
            TryMessageAction(p, message, "λNICK &Sjust highfived λTARGET", true);
        }

        public override void Help(Player p) {
            p.Message("&T/High5 [player]");
            p.Message("&HHigh five someone! :D");
        }
    }
}
