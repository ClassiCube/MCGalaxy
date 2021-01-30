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
    
    /// <summary> Causes the bot to nod spin around for a certain interval. </summary>
    public class SpinInstruction : BotInstruction {
        public SpinInstruction() { Name = "spin"; }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            Metadata meta = (Metadata)data.Metadata;
            if (bot.countdown == 0) { bot.countdown = meta.Seconds; return true; }
            bot.countdown--;

            Orientation rot = bot.Rot;
            rot.RotY += (byte)meta.Speed;
            bot.Rot = rot;

            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
        
        protected struct Metadata { public short Seconds, Speed; }
        
        public override InstructionData Parse(string[] args) {
            InstructionData data = default(InstructionData);
            Metadata meta;
            meta.Seconds  = short.Parse(args[1]);
            meta.Speed    = short.Parse(args[2]);
            data.Metadata = meta;
            return data;
        }
        
       public override void Output(Player p, string[] args, TextWriter w) {
            string time  = args.Length > 3 ? args[3] : "10";
            string speed = args.Length > 4 ? args[4] : "2";
            w.WriteLine(Name + " " + short.Parse(time) + " " + short.Parse(speed));
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] { "&T/BotAI add [name] spin <interval> <speed>",
            "&HCauses the bot to spin around for a period of time.",
            "&H  <interval> is in tenths of a second, so an interval of 20 means " +
            "spin for two seconds. (defaults to 1 second)",
            "&H  <speed> sets how fast the bot spins. (defaults to 2)",
        };
    }
    
    /// <summary> Causes the bot to nod down up and down for a certain interval. </summary>
    public sealed class NodInstruction : SpinInstruction {
        public NodInstruction() { Name = "nod"; }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            Metadata meta = (Metadata)data.Metadata;
            if (bot.countdown == 0) { bot.countdown = meta.Seconds; return true; }
            bot.countdown--;

            byte speed = (byte)meta.Speed;
            Orientation rot = bot.Rot;
            if (bot.nodUp) {
                if (rot.HeadX > 32 && rot.HeadX < 128) {
                    bot.nodUp = !bot.nodUp;
                } else {
                    if (rot.HeadX + speed > 255) rot.HeadX = 0;
                    else rot.HeadX += speed;
                }
            } else {
                if (rot.HeadX > 128 && rot.HeadX < 224) {
                    bot.nodUp = !bot.nodUp;
                } else {
                    if (rot.HeadX - speed < 0) rot.HeadX = 255;
                    else rot.HeadX -= speed;
                }
            }            
            bot.Rot = rot;

            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] { "&T/BotAI add [name] nod <interval> <speed>",
            "&HCauses the bot to nod up and down for a period of time.",
            "&H  <interval> is in tenths of a second, so an interval of 20 means " +
            "nod for two seconds. (defaults to 1 second)",
            "&H  <speed> sets how fast the bot nods. (defaults to 2)",
        };
    }
}
