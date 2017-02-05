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
namespace MCGalaxy.Commands {
    
    public abstract class MessageCmd : Command {
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        
        protected bool TryMessageAction(Player p, string target, string message, bool messageWho) {
            if (target == "") { Help(p); return false; }
            Player who = PlayerInfo.FindMatches(p, target);
            if (who == null) return false;

            string giver = (p == null) ? "(console)" : p.ColoredName;
            if (!TryMessage(p, string.Format(message, giver, who.ColoredName))) return false;
            
            string giverRaw = (p == null) ? "(console)" : p.name;
            if (messageWho && !who.listignored.Contains(giverRaw) && !who.ignoreAll)
                Player.Message(who, string.Format(message, giver, "you"));
            return true;
        }
        
        protected bool TryMessage(Player p, string message) { return TryMessage(p, message, name); }
        
        protected static bool TryMessage(Player p, string message, string cmd) {
            if (!CanSpeak(p, cmd)) return false;
            
            if (p.level.worldChat) {
                Player.SendChatFrom(p, message, false);
            } else {
                Chat.GlobalChatLevel(p, "<Level>" + message, false);
            }
            p.CheckForMessageSpam();
            return true;
        }
        
        internal static bool CanSpeak(Player p, string cmd) {
            if (p == null) return true;
            
            if (p.muted) { 
                Player.Message(p, "Cannot use %T/{0} %Swhile muted.", cmd); return false; 
            }
            if (Server.chatmod && !p.voice) { 
                Player.Message(p, "Cannot use %T/{0} %Swhile chat moderation is on.", cmd); return false; 
            }
            return true;
        }
    }
    
    public sealed class CmdHigh5 : MessageCmd {
        public override string name { get { return "high5"; } }
        
        public override void Use(Player p, string message) {
            TryMessageAction(p, message, "{0} %Sjust highfived {1}", true);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/high5 [player]");
            Player.Message(p, "%HHigh five someone! :D");
        }
    }
}
