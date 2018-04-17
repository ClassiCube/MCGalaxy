/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Bots;

namespace MCGalaxy.Commands.Bots {
    public sealed class CmdBot : Command {
        public override string name { get { return "Bot"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("BotAdd", "add"), new CommandAlias("BotRemove", "remove") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return; }
            if (!Formatter.ValidName(p, args[1], "bot")) return;
            
            if (args[0].CaselessEq("add")) {
                AddBot(p, args[1]);
            } else if (args[0].CaselessEq("remove")) {
                RemoveBot(p, args[1]);
            } else if (args[0].CaselessEq("text")) {
                string text = args.Length > 2 ? args[2] : null;
                SetBotText(p, args[1], text);
            } else if (args[0].CaselessEq("deathmsg") || args[0].CaselessEq("deathmessage")) {
                string text = args.Length > 2 ? args[2] : null;
                SetDeathMessage(p, args[1], text);
            } else if (args[0].CaselessEq("rename")) {
                string newName = args.Length > 2 ? args[2] : null;
                RenameBot(p, args[1], newName);
            } else {
                Help(p);
            }
        }
        
        void AddBot(Player p, string botName) {
            if (!LevelInfo.ValidateAction(p, p.level.name, "add bots to this level")) return;
            
            if (BotExists(p.level, botName, null)) {
                Player.Message(p, "A bot with that name already exists."); return;
            }
            if (p.level.Bots.Count >= ServerConfig.MaxBotsPerLevel) {
                Player.Message(p, "Reached maximum number of bots allowed on this map."); return;
            }
            
            PlayerBot bot = new PlayerBot(botName, p.level);
            bot.Pos = p.Pos; bot.lastPos = bot.Pos;
            bot.SetYawPitch(p.Rot.RotY, 0);
            
            Player.Message(p, "You added the bot " + bot.ColoredName );
            PlayerBot.Add(bot);
        }
        
        static bool BotExists(Level lvl, string name, PlayerBot skip) {
            PlayerBot[] bots = lvl.Bots.Items;
            foreach (PlayerBot bot in bots) {
                if (bot == skip) continue;
                if (bot.name.CaselessEq(name)) return true;
            }
            return false;
        }
        
        void RemoveBot(Player p, string botName) {
            if (!LevelInfo.ValidateAction(p, p.level.name, "remove bots from this level")) return;
            
            if (botName.CaselessEq("all")) {
                PlayerBot.RemoveLoadedBots(p.level, false);
                BotsFile.Save(p.level);
            } else {
                PlayerBot bot = Matcher.FindBots(p, botName);
                if (bot == null) return;
                
                PlayerBot.Remove(bot);
                Player.Message(p, "Removed bot {0}", bot.ColoredName);
            }
        }
        
        void RenameBot(Player p, string botName, string newName) {
            if (!LevelInfo.ValidateAction(p, p.level.name, "rename bots on this level")) return;
            if (newName == null) { Player.Message(p, "New name of bot required."); return; }            
            if (!Formatter.ValidName(p, newName, "bot")) return;
            
            PlayerBot bot = Matcher.FindBots(p, botName);
            if (bot == null) return;
            if (BotExists(p.level, newName, bot)) {
                Player.Message(p, "A bot with the new name already exists."); return;
            }
            
            Player.Message(p, "Renamed bot {0}", bot.ColoredName);
            if (bot.DisplayName == bot.name) {
                bot.DisplayName = newName;
                bot.GlobalDespawn();
                bot.GlobalSpawn();
            }
            
            bot.name = newName;
            BotsFile.Save(bot.level);
        }
        
        void SetBotText(Player p, string botName, string text) {
            PlayerBot bot = Matcher.FindBots(p, botName);
            if (bot == null) return;
            if (!LevelInfo.ValidateAction(p, p.level.name, "set bot text of that bot")) return;
            
            if (text == null) {
                Player.Message(p, "Removed text shown when bot {0} %Sclicked on", bot.ColoredName);
                bot.ClickedOnText = null;
            } else {                
                if (!MessageBlock.Validate(p, text, false)) return;
                Player.Message(p, "Set text shown when bot {0} %Sis clicked on to {1}", bot.ColoredName, text);
                bot.ClickedOnText = text;
            }
            BotsFile.Save(bot.level);
        }
        
        void SetDeathMessage(Player p, string botName, string text) {
            PlayerBot bot = Matcher.FindBots(p, botName);
            if (bot == null) return;
            if (!LevelInfo.ValidateAction(p, p.level.name, "set kill message of that bot")) return;
            
            if (text == null) {
                Player.Message(p, "Reset shown when bot {0} %Skills someone", bot.ColoredName);
                bot.DeathMessage = null;
            } else {                
                if (!MessageBlock.Validate(p, text, false)) return;
                Player.Message(p, "Set message shown when bot {0} %Skills someone to {1}", bot.ColoredName, text);
                bot.DeathMessage = text;
            }
            BotsFile.Save(bot.level);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Bot add [name] %H- Adds a new bot at your position");
            Player.Message(p, "%T/Bot remove [name] %H- Removes the bot with that name");
            Player.Message(p, "%H  If [name] is \"all\", removes all bots on your map");
            Player.Message(p, "%T/Bot text [name] <text>");
            Player.Message(p, "%HSets the text shown when a player clicks on this bot");
            Player.Message(p, "%HSee %T/Help mb %Hfor more details on <text>");
            Player.Message(p, "%T/Bot deathmessage [name] <message>");
            Player.Message(p, "%HSets the message shown when this bot kills a player");
            Player.Message(p, "%T/Bot rename [name] [new name] %H- Renames a bot");
            Player.Message(p, "%H  Note: To only change name tag of a bot, use %T/Nick bot");
        }
    }
}
