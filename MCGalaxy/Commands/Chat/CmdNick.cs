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
        public override string name { get { return "Nick"; } }
        public override string shortcut { get { return "Nickname"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the nick of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the nick of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("xnick", "-own") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (!MessageCmd.CanSpeak(p, name)) return;
            UseBotOrPlayer(p, data, message, "nick");
        }

        protected override void SetBotData(Player p, PlayerBot bot, string nick) {
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
            if (Colors.Strip(nick).Length >= 30) { p.Message("Nick must be under 30 letters."); return; }
            Player who = PlayerInfo.FindExact(target);
            
            if (nick.Length == 0) {
                MessageAction(p, target, who, "λACTOR &Sremoved λTARGET nick");
                nick = target.RemoveLastPlus();
            } else {
                // TODO: select color from database?
                string color = who != null ? who.color : Group.GroupIn(target).Color;
                MessageAction(p, target, who, "λACTOR &Schanged λTARGET nick to " + color + nick);
            }
            
            if (who != null) who.DisplayName = nick;
            if (who != null) TabList.Update(who, true);
            PlayerDB.SetNick(target, nick);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Nick [player] [nick]");
            p.Message("&HSets the nick of that player.");
            p.Message("&H  If [nick] is not given, reverts [player]'s nick to their account name.");
            p.Message("&T/Nick bot [bot] [name]");
            p.Message("&HSets the name shown above that bot in game.");
            p.Message("&H  If [name] is \"empty\", the bot will not have a name shown.");
        }
    }
}

