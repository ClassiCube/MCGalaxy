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
using System;
using MCGalaxy;
using MCGalaxy.Bots;
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Chatting {    
    public class CmdNick : EntityPropertyCmd {       
        public override string name { get { return "nick"; } }
        public override string shortcut { get { return "nickname"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the nick of others"),
                    new CommandPerm(LevelPermission.Operator, "+ can change the nick of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xnick", "-own") }; }
        }
        
        public override void Use(Player p, string message)
        {
            if (!MessageCmd.CanSpeak(p, name)) return;
            UseBotOrPlayer(p, message, "nick");
        }

        protected override void SetBotData(Player p, PlayerBot bot, string nick) {
            if (nick == "") {
                bot.DisplayName = bot.name;
                Chat.MessageLevel(bot.level, "Bot " + bot.ColoredName + " %Sreverted to their original name.");
            } else {
                string nameTag = nick.CaselessEq("empty") ? "" : nick;
                if (nick.Length > 62) { Player.Message(p, "Name must be 62 or fewer letters."); return; }
                
                Player.Message(p, "You changed the name of bot " + bot.ColoredName + " %Sto &c" + nameTag);
                bot.DisplayName = Colors.EscapeColors(nick);
            }
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.UpdateBot(bot);
        }
        
        protected override void SetPlayerData(Player p, Player who, string nick) {
            if (nick == "") {                
                Chat.MessageGlobal(who, who.FullName + " %Sreverted their nick to their original name.", false);
                who.DisplayName = who.truename;
            } else {
                if (nick.Length >= 30) { Player.Message(p, "Nick must be under 30 letters."); return; }     
                
                Chat.MessageGlobal(who, who.FullName + " %Shad their nick changed to " + who.color + nick + "%S.", false);
                who.DisplayName = nick;
            }
            
            PlayerDB.Save(who);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/nick [player] [nick]");
            Player.Message(p, "%HSets the nick of that player.");
            Player.Message(p, "%H  If [nick] is not given, reverts [player]'s nick to their account name.");
            Player.Message(p, "%T/nick bot [bot] [name]");
            Player.Message(p, "%HSets the name shown above that bot in game.");
            Player.Message(p, "%H  If [name] is \"empty\", the bot will not have a name shown.");
        }
    }
}

