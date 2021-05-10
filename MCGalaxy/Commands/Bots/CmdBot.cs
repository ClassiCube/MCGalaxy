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
    public sealed class CmdBot : Command2 {
        public override string name { get { return "Bot"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] {
                    new CommandAlias("BotAdd", "add"),
                    new CommandAlias("BotRemove", "remove"),
                    new CommandAlias("BotInfo", "info")
                }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can modify bots that do not belong to them") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            if (args[0].CaselessEq("info")) { BotInfo(p, args.Length < 2 ? "" : args[1]); return; }
            
            if (args.Length < 2) { Help(p); return; }
            
            if (!Formatter.ValidName(p, args[1], "bot")) return;
            if (!LevelInfo.Check(p, data.Rank, p.level, "modify bots in this level")) return;
            
            string bot = args[1], value = args.Length > 2 ? args[2] : null;
            if (args[0].CaselessEq("add")) {
                AddBot(p, bot);
            } else if (IsDeleteCommand(args[0])) {
                RemoveBot(p, bot, value);
            } else if (args[0].CaselessEq("text")) {
                SetBotText(p, bot, value, data.Rank);
            } else if (args[0].CaselessEq("deathmsg") || args[0].CaselessEq("deathmessage")) {
                SetDeathMessage(p, bot, value);
            } else if (args[0].CaselessEq("rename")) {
                RenameBot(p, bot, value);
            } else if (args[0].CaselessEq("copy")) {
                CopyBot(p, bot, value);
            } else {
                Help(p);
            }
        }
        
        void AddBot(Player p, string botName) {
            botName = botName.Replace(' ', '_');
            PlayerBot bot = new PlayerBot(botName, p.level);
            bot.Owner = p.name;
            TryAddBot(p, bot);
        }
        
        void TryAddBot(Player p, PlayerBot bot) {
            if (BotExists(p.level, bot.name, null)) {
                p.Message("A bot with that name already exists."); return;
            }
            if (p.level.Bots.Count >= Server.Config.MaxBotsPerLevel) {
                p.Message("Reached maximum number of bots allowed on this map."); return;
            }
            
            bot.SetInitialPos(p.Pos);
            bot.SetYawPitch(p.Rot.RotY, 0);
            
            p.Message("You added the bot " + bot.ColoredName);
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
        
        void RemoveBot(Player p, string botName, string extArgs) {
            if (botName.CaselessEq("all")) {
                //bot remove all[botname] griefer[extArgs]
                if (extArgs != null) {
                    string ownerName = PlayerInfo.FindMatchesPreferOnline(p, extArgs);
                    if (ownerName == null) { return; }
                    if (PlayerBot.CanEditAny(p) || ownerName.CaselessEq(p.name)) {
                        int removedCount = PlayerBot.RemoveBotsOwnedBy(p, ownerName, p.level, false);
                        if (removedCount == 0) {
                            p.Message("There are no bots owned by {0}&S in this level.", p.FormatNick(ownerName));
                        } else {
                            p.Message("Removed {0} bot{1} belonging to {2}&S.", removedCount, removedCount.Plural(), p.FormatNick(ownerName));
                            BotsFile.Save(p.level);
                        }
                    } else {
                        p.Message("&WYou cannot remove all bots belonging to {0}&W unless you are the owner of this map.", p.FormatNick(ownerName));
                    }
                    return;
                }
                
                if (PlayerBot.CanEditAny(p)) {
                    int removedCount = PlayerBot.RemoveLoadedBots(p.level, false);
                    if (removedCount == 0) {
                        p.Message("There are no bots in this level.");
                    } else {
                        p.Message("Removed {0} bot{1}.", removedCount, removedCount.Plural());
                        BotsFile.Save(p.level);
                    }
                } else {
                    p.Message("&WYou cannot remove all bots unless you are the owner of this map.");
                }

            } else {
                PlayerBot bot = Matcher.FindBots(p, botName);
                if (bot == null) return;
                if (!bot.EditableBy(p, "remove")) return;
                
                PlayerBot.Remove(bot);
                p.Message("Removed bot {0}", bot.ColoredName);
            }
        }
        
        void SetBotText(Player p, string botName, string text, LevelPermission plRank) {
            PlayerBot bot = Matcher.FindBots(p, botName);
            if (bot == null) return;
            if (!bot.EditableBy(p, "set the text of")) return;
            
            if (text == null) {
                p.Message("Removed text shown when bot {0} &Sclicked on", bot.ColoredName);
                bot.ClickedOnText = null;
            } else {
                bool allCmds = HasExtraPerm(p, "MB", plRank, 1);
                if (!MessageBlock.Validate(p, text, allCmds)) return;
                
                p.Message("Set text shown when bot {0} &Sis clicked on to {1}", bot.ColoredName, text);
                bot.ClickedOnText = text;
            }
            BotsFile.Save(p.level);
        }
        
        void SetDeathMessage(Player p, string botName, string text) {
            PlayerBot bot = Matcher.FindBots(p, botName);
            if (bot == null) return;
            if (!bot.EditableBy(p, "set the death message of")) return;
            
            if (text == null) {
                p.Message("Reset shown when bot {0} &Skills someone", bot.ColoredName);
                bot.DeathMessage = null;
            } else {
                p.Message("Set message shown when bot {0} &Skills someone to {1}", bot.ColoredName, text);
                bot.DeathMessage = text;
            }
            BotsFile.Save(p.level);
        }
        
        void RenameBot(Player p, string botName, string newName) {
            if (newName == null) { p.Message("New name of bot required."); return; }
            if (!Formatter.ValidName(p, newName, "bot")) return;
            
            PlayerBot bot = Matcher.FindBots(p, botName);
            if (bot == null) return;
            if (!bot.EditableBy(p, "rename")) { return; }
            if (BotExists(p.level, newName, bot)) {
                p.Message("A bot with the new name already exists."); return;
            }
            
            p.Message("Renamed bot {0}", bot.ColoredName);
            if (bot.DisplayName == bot.name) {
                bot.DisplayName = newName;
                bot.GlobalDespawn();
                bot.GlobalSpawn();
            }
            
            bot.name = newName;
            BotsFile.Save(p.level);
        }
        
        void CopyBot(Player p, string botName, string newName) {
            if (newName == null) { p.Message("Name of new bot required."); return; }
            if (!Formatter.ValidName(p, newName, "bot")) return;
            
            PlayerBot bot = Matcher.FindBots(p, botName);
            if (bot == null) return;
            
            PlayerBot clone = new PlayerBot(newName, p.level);
            BotProperties props = new BotProperties();
            props.FromBot(bot);
            props.ApplyTo(clone);
            clone.Owner = p.name;
            clone.SetModel(clone.Model);
            BotsFile.LoadAi(props, clone);
            // Preserve custom name tag
            if (bot.DisplayName == bot.name) clone.DisplayName = newName;
            TryAddBot(p, clone);
        }
        
        void BotInfo(Player p, string botName) {
            if (botName.Length == 0) {
                if (!p.Supports(CpeExt.PlayerClick)) {
                    p.Message("Your client does not support clicking on entities.");
                    p.Message("You must type &T/botinfo [name] &Sto see info.");
                    p.Message("To help find the name of a nicknamed bot, try");
                    p.Message("&T/whonick bot [nickname]");
                    return;
                }
                p.checkingBotInfo = true;
                p.Message("Left, right, or middle click a bot to display its information.");
                return;
            }
            PlayerBot bot = Matcher.FindBots(p, botName);
            if (bot == null) return;
            bot.DisplayInfo(p);
            if (p.checkingBotInfo) { p.checkingBotInfo = false; p.Message("Note: pending click-to-check bot info has been cancelled."); }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Bot add [name] &H- Adds a new bot at your position");
            p.Message("&T/Bot remove [name] &H- Removes the bot with that name");
            p.Message("&T/Bot remove all &H- Removes all bots on your map.");
            p.Message("&T/Bot remove all [owner]");
            p.Message("&HRemoves all bots owned by [owner] on your map.");
            p.Message("&T/Bot text [name] <text>");
            p.Message("&HSets the text shown when a player clicks on this bot");
            p.Message("&HSee &T/Help mb &Hfor more details on <text>");
            p.Message("&T/Bot deathmessage [name] <message>");
            p.Message("&HSets the message shown when this bot kills a player");
            p.Message("&T/Bot rename [name] [new name] &H- Renames a bot");
            p.Message("&H  Note: To only change name tag of a bot, use &T/Nick bot");
            p.Message("&T/Bot copy [name] [new name] &H- Clones an existing bot");
            p.Message("&T/Bot info &H- Displays info of the next bot you click");
            p.Message("&T/Bot info [name] &H- Displays info of bot with that name");
        }
    }
}
