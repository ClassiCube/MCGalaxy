/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using MCGalaxy.Bots;

namespace MCGalaxy.Commands.Bots {
    public sealed class CmdBotSet : Command {
        public override string name { get { return "BotSet"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can set bots to be killer") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();
            PlayerBot bot = Matcher.FindBots(p, args[0]);
            if (bot == null) return;            
            if (!LevelInfo.ValidateAction(p, p.level, "change AI of bots in this level")) return;
                
            if (args.Length == 1) {
                bot.Instructions.Clear();
                bot.kill = false;
                bot.hunt = false;
                bot.AIName = "";
                UpdateBot(p, bot, "'s AI was turned off.");
                return;
            } else if (args.Length != 2) {
                Help(p); return;
            }

            string ai = args[1].ToLower();
            if (ai.CaselessEq("hunt")) {
                bot.hunt = !bot.hunt;
                bot.Instructions.Clear();
                bot.AIName = "";
                UpdateBot(p, bot, "'s hunt instinct: " + bot.hunt);
                return;
            } else if (ai.CaselessEq("kill")) {
                if (!CheckExtraPerm(p, 1)) return;
                bot.kill = !bot.kill;
                UpdateBot(p, bot, "'s kill instinct: " + bot.kill);
                return;
            }
            
            if (!ScriptFile.Parse(p, bot, "bots/" + ai)) return;
            bot.AIName = ai;
            UpdateBot(p, bot, "'s AI was set to " + ai);
        }
        
        static void UpdateBot(Player p, PlayerBot bot, string msg) {
            Player.Message(p, bot.ColoredName + "%S" + msg);
            Logger.Log(LogType.UserActivity, bot.name + msg);
            BotsFile.Save(bot.level);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/BotSet [bot] <AI script>");
            Player.Message(p, "%HMakes [bot] do the instructions in <AI script>");
            Player.Message(p, "%H  Special AI scripts: Kill and Hunt");
            Player.Message(p, "%HIf <AI script> is not given, turns off the bot's AI.");
        }
    }
}
