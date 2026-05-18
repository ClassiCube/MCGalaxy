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
using MCGalaxy.Bots;

namespace MCGalaxy.Commands.Chatting 
{    
    public class CmdNick : EntityPropertyCmd 
    {       
        public override string name { get { return "Nick"; } }
        public override string shortcut { get { return "Nickname"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the nick of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the nick of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] {
                new CommandAlias("XNick"),
                new CommandAlias("ONick", OTHER_FLAG)
            }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            UseBotOrPlayer(p, data, message, "nick");
        }

        protected override void SetBotData(Player p, PlayerBot bot, string nick) {
            if (!MessageCmd.CanSpeak(p, name)) return;
            
            if (nick.Length == 0) {
                bot.DisplayName = bot.name;
                p.level.Message("Bot " + bot.ColoredName + " &Sreverted to their original name.");
            } else {
                string nameTag = nick.CaselessEq("empty") ? "" : nick;
                if (nick.Length > 62) { p.Message("Name must be 62 or fewer letters."); return; }
                
                p.Message("You changed the name of bot " + bot.ColoredName + " &Sto &c" + nameTag);
                bot.DisplayName = Colors.Escape(nick);
            }
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.Save(p.level);
        }
        
        protected override void SetPlayerData(Player p, string target, string nick) {
            PlayerOperations.SetNick(p, target, nick);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Nick <nick>");
            p.Message("&H Sets your nickname");
            p.Message("&T/ONick [player] <nick>");
            p.Message("&H Sets the nickname of other player");
            p.Message("&T/Nick bot [bot] <nick>");
            p.Message("&H Sets the nickname of that bot.");
            p.Message("&H  Leave <nick> blank to remove it.");
            p.Message("&H  If [name] is \"empty\", the bot will not have a name shown.");
        }
    }
}

