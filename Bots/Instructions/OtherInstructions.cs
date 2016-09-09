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

namespace MCGalaxy.Bots {
    
    /// <summary> Causes the bot to reset to the and execute first instruction. </summary>
    public sealed class ResetInstruction : BotInstruction {
        public override string Name { get { return "Reset"; } }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            bot.cur = 0; return false;
        }
    }
    
    /// <summary> Causes the bot to be removed from the world. </summary>
    public sealed class RemoveInstruction : BotInstruction {
        public override string Name { get { return "Remove"; } }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            PlayerBot.Remove(bot); return true;
        }
    }
    
    /// <summary> Causes the bot to switch to a different AI. </summary>
    public sealed class LinkScriptInstruction : BotInstruction {
        public override string Name { get { return "LinkScript"; } }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            string script = (string)data.Metadata;
            if (File.Exists("bots/" + script)) {
                Command.all.Find("botset").Use(null, bot.name + " " + script);
                return true;
            }
            bot.NextInstruction(); return true;
        }
        
        public override InstructionData Parse(string[] args) {
            InstructionData data = default(InstructionData);
            data.Metadata = args[1];
            return data;
        }
    }
    
    /// <summary> Causes the bot to wait/do nothing for a certain interval. </summary>
    public sealed class WaitInstruction : BotInstruction {
        public override string Name { get { return "Wait"; } }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            if (bot.countdown == 0) {
                bot.countdown = (short)data.Metadata;
                return true;
            }
            
            bot.countdown--;
            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
        
        public override InstructionData Parse(string[] args) {
            InstructionData data = default(InstructionData);
            data.Metadata = short.Parse(args[1]);
            return data;
        }
    }
}
