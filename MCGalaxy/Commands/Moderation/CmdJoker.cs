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
using MCGalaxy.Commands.Chatting;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdJoker : Command2 {       
        public override string name { get { return "Joker"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            bool stealth = false;
            if (message[0] == '#') {
                message = message.Remove(0, 1).Trim();
                stealth = true;
                Logger.Log(LogType.UserActivity, "Stealth joker attempted");
            }

            Player who = PlayerInfo.FindMatches(p, message);
            if (who == null) return;
            if (!CheckRank(p, data, who, "joker", true)) return;
            if (!MessageCmd.CanSpeak(p, name)) return;

            if (!who.joker) {
                if (stealth) { 
                    Chat.MessageFromOps(who, "λNICK &Sis now STEALTH jokered."); 
                } else {
                    Chat.MessageFrom(who, "λNICK &Sis now a &aJ&bo&ck&5e&9r&S.", null, true);
                }
            } else {
                if (stealth) { 
                    Chat.MessageFromOps(who, "λNICK &Sis now STEALTH unjokered.");
                } else {
                    Chat.MessageFrom(who, "λNICK &Sis no longer a &aJ&bo&ck&5e&9r&S.", null, true);
                }
            }
            who.joker = !who.joker;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Joker [player]");
            p.Message("&HMakes that player become a joker!");
            p.Message("&T/Joker #[player]");
            p.Message("&HMakes that player silently become a joker!");
        }
    }
}
