/*
    Copyright 2015-2024 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at

    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html

    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */

namespace MCGalaxy.Commands {
    public abstract class EntityPropertyCmd : Command2 {
        
        protected void UseBotOrOnline(Player p, CommandData data, string message, string type) {
            if (message.CaselessStarts("bot ")) {
                UseBot(p,    data, message, type);
            } else {
                UseOnline(p, data, message, type);
            }
        }
        
        protected void UseBotOrPlayer(Player p, CommandData data, string message, string type) {
            if (message.CaselessStarts("bot ")) {
                UseBot(p,    data, message, type);
            } else {
                UsePlayer(p, data, message, type);
            }
        }
        
        void UseBot(Player p, CommandData data, string message, string type) {
            string[] args = message.SplitSpaces(3);
            PlayerBot bot = Matcher.FindBots(p, args[1]);
            
            if (bot == null) return;
            if (!CheckExtraPerm(p, data, 2)) return;
            
            if (!LevelInfo.Check(p, data.Rank, p.level, "change the " + type + " of that bot")) return;
            if (!bot.EditableBy(p, "change the " + type + " of")) { return; }
            SetBotData(p, bot, args.Length > 2 ? args[2] : "");
        }

        public const string OTHER_FLAG = "-other";
        public const string OWN_FLAG = "-own";
        bool ProcessArgs(Player p, string message, string dataType, out string target, out string value) {
            string[] args = message.SplitSpaces(2);

            if (args[0].CaselessEq(OTHER_FLAG)) {
                if (args.Length == 1) {
                    target = null; value = null;
                    p.Message("You must provide the name of the player that are you are changing the {0} of.", dataType);
                    return false;
                }
                string[] otherArgs = args[1].SplitSpaces(2);
                target = otherArgs[0];
                value = otherArgs.Length > 1 ? otherArgs[1] : "";
                return true;
            }
            if (p.IsSuper) {
                target = args[0];
                value = args.Length > 1 ? args[1] : "";
                return true;
            }

            if (args[0].CaselessEq(OWN_FLAG)) {
                target = p.name;
                value = args.Length > 1 ? args[1] : "";
            } else {
                target = p.name;
                value = message;
            }

            string firstWord = value.SplitSpaces(2)[0];

            if (value.Length > 1) {
                int matches;
                Player maybe = firstWord.Length < 3 ? null : PlayerInfo.FindMatches(p, firstWord, out matches, false, false);
                if (maybe != null) {
                    string tipModel = args.Length > 1 ? args[1] : "";
                    string action = tipModel == "" ? "remove" : "change";
                    if (maybe == p) {
                        p.Message("&WTIP:");
                        p.Message("&H  To "+action+" your own {0}, use /{1} {2}", dataType, name.ToLower(), tipModel);
                    } else {
                        if (HasExtraPerm(p, p.Rank, 1)) {
                            p.Message("&WTIP:");
                            p.Message("&H  To "+action+" &Wother&H player's {0}, use /O{1} [player] {2}", dataType, name, tipModel);
                        }
                    }
                }
            }
            return true;
        }

        protected void UseOnline(Player p, CommandData data, string message, string type) {
            //if (message.Length == 0) { Help(p); return; }
            string target, value;
            if (!ProcessArgs(p, message, type, out target, out value)) return;
            
            Player who = PlayerInfo.FindMatches(p, target);
            if (who == null) return;
            
            if (p != who && !CheckExtraPerm(p, data, 1)) return;
            if (!CheckRank(p, data, who, "change the " + type + " of", true)) return;
            SetOnlineData(p, who, value);
        }
        
        protected void UsePlayer(Player p, CommandData data, string message, string type) {
            //if (message.Length == 0) { Help(p); return; }
            string target, value;
            if (!ProcessArgs(p, message, type, out target, out value)) return;

            target = PlayerInfo.FindMatchesPreferOnline(p, target);
            if (target == null) return;            
            if (p.name != target && !CheckExtraPerm(p, data, 1)) return;
            
            LevelPermission rank = Group.GroupIn(target).Permission;
            if (!CheckRank(p, data, target, rank, "change the " + type + " of", true)) return;
            SetPlayerData(p, target, value);
        }

        protected virtual void SetBotData(Player p, PlayerBot bot,    string args) { }      
        protected virtual void SetOnlineData(Player p, Player who,    string args) { }       
        protected virtual void SetPlayerData(Player p, string target, string args) { }
    }
}
