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

namespace MCGalaxy.Commands {    
    public class CmdNick : Command {       
        public override string name { get { return "nick"; } }
        public override string shortcut { get { return "nickname"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the nick of other players") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xnick", "-own") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            bool isBot = message.CaselessStarts("bot ");
            string[] args = message.SplitSpaces(isBot ? 3 : 2);
            if (args[0].CaselessEq("-own")) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "player name"); return; }
                args[0] = p.name;
            }
            
            Player who = null;
            PlayerBot bot = null;            
            if (isBot) bot = PlayerBot.FindMatchesPreferLevel(p, args[1]);
            else who = PlayerInfo.FindMatches(p, args[0]);
            if (bot == null && who == null) return;
            
            if (p != null && who != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "change the nick of", true); return;
            }
            if ((isBot || who != p) && !CheckExtraPerm(p)) { MessageNeedExtra(p, "change the nick of others."); return; }
            if (isBot) SetBotNick(p, bot, args);
            else SetNick(p, who, args);
        }
        
        static void SetBotNick(Player p, PlayerBot bot, string[] args) {
            string newName = args.Length > 2 ? args[2] : "";
            if (newName == "") {
                bot.DisplayName = bot.name;
                Chat.MessageAll("Bot {0}'s %Sreverted to their original name.", bot.ColoredName);
            } else {
                string nameTag = newName.CaselessEq("empty") ? "" : newName;
                if (newName.Length >= 30) { Player.Message(p, "Name must be under 30 letters."); return; }
                Chat.MessageAll("Bot {0}'s %Sname was set to {1}%S.", bot.ColoredName, nameTag);
                bot.DisplayName = newName;
            }
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.UpdateBot(bot);
        }
        
        static void SetNick(Player p, Player who, string[] args) {
            string newName = args.Length > 1 ? args[1] : "";
            if (newName == "") {
                who.DisplayName = who.truename;
                Player.SendChatFrom(who, who.FullName + " %Sreverted their nick to their original name.", false);
            } else {
                if (newName.Length >= 30) { Player.Message(p, "Nick must be under 30 letters."); return; }                
                Player.SendChatFrom(who, who.FullName + " %Schanged their nick to " + who.color + newName + "%S.", false);
                who.DisplayName = newName;
            }
            
            Entities.GlobalDespawn(who, false);
            Entities.GlobalSpawn(who, false);
            PlayerDB.Save(who);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/nick [player] [nick]");
            Player.Message(p, "%HSets the nick of that player.");
            Player.Message(p, "%HIf no [nick] is given, reverts player's nick to their original name.");
            Player.Message(p, "%T/nick bot [bot] [name]");
            Player.Message(p, "%HSets the name shown above that bot in game.");
            Player.Message(p, "%H  If [name] is \"empty\", the bot will not have a name shown.");
        }
    }
}

