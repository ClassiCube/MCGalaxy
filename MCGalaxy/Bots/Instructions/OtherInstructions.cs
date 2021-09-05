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
using System;
using System.IO;

namespace MCGalaxy.Bots 
{  
    /// <summary> Causes the bot to reset to the and execute first instruction </summary>
    public sealed class ResetInstruction : BotInstruction 
    {
        public override bool Execute(PlayerBot bot) {
            bot.cur = 0; return false;
        }
        
        
        public override string Serialise() { return "reset"; }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] { 
            "&T/BotAI add [name] reset",
            "&HCauses the bot to go back to the first instruction",
        };
    }
    
    /// <summary> Causes the bot to be removed from the world </summary>
    public sealed class RemoveInstruction : BotInstruction 
    {
        public override bool Execute(PlayerBot bot) {
            PlayerBot.Remove(bot); return true;
        }
        
        
        public override string Serialise() { return "remove"; }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] { 
            "&T/BotAI add [name] remove",
            "&HCauses the bot to be removed from the world",
        };
    }
    
    /// <summary> Causes the bot to switch to a different AI </summary>
    public sealed class LinkScriptInstruction : BotInstruction 
    {
        public string AI;

        public override bool Execute(PlayerBot bot) {
            if (File.Exists("bots/" + AI)) {
                ScriptFile.Parse(Player.Console, bot, AI);
                return true;
            }
            bot.NextInstruction(); return true;
        }
        
        
        public override string Serialise() { return "linkscript " + AI;  }
        
        public override void Deserialise(string value) {
            AI = value;
        }
        
        public override void Output(Player p, string[] args, TextWriter w) {
            string script = args.Length > 3 ? args[3] : "";
            if (script.Length == 0) {
                p.Message("LinkScript requires a script name as a parameter");
            } else {
                AI = script;
            }
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] { 
            "&T/BotAI add [name] linkscript [ai name]",
            "&HCauses the bot to switch to the given AI, and execute that AI's instructions instead.",
        };
    }
    
    /// <summary> Causes the bot to wait/do nothing for a certain interval. </summary>
    public sealed class WaitInstruction : BotInstruction 
    {
        public short Interval = 10;

        public override bool Execute(PlayerBot bot) {
            if (bot.countdown == 0) {
                bot.countdown = Interval;
                return true;
            }
            
            bot.countdown--;
            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
        
        
        public override string Serialise() { return "wait " + Interval; }
        
        public override void Deserialise(string value) {
            Interval = short.Parse(value);
        }
        
        public override void Output(Player p, string[] args, TextWriter w) {
            string time = args.Length > 3 ? args[3] : "10";
            Interval = short.Parse(time);
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] { 
            "&T/BotAI add [name] wait <interval>",
            "&HCauses the bot to stay still for a period of time.",
            "&H  <interval> is in tenths of a second, so an interval of 20 means " +
            "stay still for two seconds. (defaults to 1 second)",
        };
    }
}
