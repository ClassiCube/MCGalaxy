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
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdJoker : Command {       
        public override string name { get { return "Joker"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public static string keywords { get { return ""; } }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            bool stealth = false;
            if (message[0] == '#') {
                message = message.Remove(0, 1).Trim();
                stealth = true;
                Logger.Log(LogType.UserActivity, "Stealth joker attempted");
            }

            Player who = PlayerInfo.FindMatches(p, message);
            if (who == null) return;
            if (p != null && who.Rank > p.Rank) { 
                MessageTooHighRank(p, "joker", true); return;
            }

            if (!who.joker) {
                if (stealth) { 
                    Chat.MessageOps(who.ColoredName + " %Sis now STEALTH jokered."); 
                } else {
                    Chat.MessageGlobal(who, who.ColoredName + " %Sis now a &aJ&bo&ck&5e&9r%S.", false);              
                }
                OnPlayerActionEvent.Call(p, PlayerAction.Joker, null, stealth);
            } else {
                if (stealth) { 
                    Chat.MessageOps(who.ColoredName + " %Sis now STEALTH unjokered.");
                } else {
                    Chat.MessageGlobal(who, who.ColoredName + " %Sis no longer a &aJ&bo&ck&5e&9r%S.", false);
                }
                OnPlayerActionEvent.Call(p, PlayerAction.Unjoker, null, stealth);
            }
            who.joker = !who.joker;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Joker [player]");
            Player.Message(p, "%HMakes that player become a joker!");
            Player.Message(p, "%T/Joker #[player]");
            Player.Message(p, "%HMakes that player silently become a joker!");
        }
    }
}
