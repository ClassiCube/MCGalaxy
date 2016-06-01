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

namespace MCGalaxy.Commands {
    public sealed class CmdBotSet : Command {
        public override string name { get { return "botset"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can set bots to be killer") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            PlayerBot bot = PlayerBot.FindOrShowMatches(p, args[0]);
            if (bot == null) return;
                
            if (args.Length == 1) {             
                try { bot.Waypoints.Clear(); } catch { }
                bot.kill = false;
                bot.hunt = false;
                bot.AIName = "";
                Player.Message(p, bot.color + bot.name + "%S's AI was turned off.");
                Server.s.Log(bot.name + "'s AI was turned off.");
                return;
            } else if (args.Length != 2) {
                Help(p); return;
            }

            string ai = args[1].ToLower();

            if (ai == "hunt") {
                bot.hunt = !bot.hunt;
                try { bot.Waypoints.Clear(); }
                catch { }
                bot.AIName = "";
                if (p != null) Chat.GlobalChatLevel(p, bot.color + bot.name + "%S's hunt instinct: " + bot.hunt, false);
                Server.s.Log(bot.name + "'s hunt instinct: " + bot.hunt);
                BotsFile.UpdateBot(bot);
                return;
            } else if (ai == "kill") {
                if (!CheckExtraPerm(p)) { MessageNeedPerms(p, "can toggle a bot's killer instinct."); return; }
                bot.kill = !bot.kill;
                if (p != null) Chat.GlobalChatLevel(p, bot.color + bot.name + "%S's kill instinct: " + bot.kill, false);
                Server.s.Log(bot.name + "'s kill instinct: " + bot.kill);
                BotsFile.UpdateBot(bot);
                return;
            }
            
            if (!ScriptFile.Parse(p, bot, "bots/" + ai)) return;
            bot.AIName = ai;
            if (p != null) Chat.GlobalChatLevel(p, bot.color + bot.name + "%S's AI is now set to " + ai, false);
            Server.s.Log(bot.name + "'s AI was set to " + ai);
            BotsFile.UpdateBot(bot);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/botset <bot> <AI script> - Makes <bot> do <AI script>");
            Player.Message(p, "Special AI scripts: Kill and Hunt");
        }
    }
}
