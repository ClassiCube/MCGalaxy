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
        public override string Name { get { return "spin"; } }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            Metadata meta = (Metadata)data.Metadata;
            if (bot.countdown == 0) { bot.countdown = meta.Seconds; return true; }
            bot.countdown--;

            byte speed = meta.Speed;
            if (bot.rot[0] + speed > 255) bot.rot[0] = 0;
            else if (bot.rot[0] + speed < 0) bot.rot[0] = 255;
            else bot.rot[0] += speed;

            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
        
        protected struct Metadata { public short Seconds; public byte Speed; }
        
        public override InstructionData Parse(string[] args) {
            InstructionData data = default(InstructionData);
            Metadata meta;
            meta.Seconds = short.Parse(args[1]);
            meta.Speed = byte.Parse(args[2]);
            data.Metadata = meta;
            return data;
        }
        
       public override void Output(Player p, string[] args, StreamWriter w) {
            string time = args.Length > 3 ? args[3] : "10";
            string speed = args.Length > 4 ? args[4] : "2";
            w.WriteLine(Name + " " + short.Parse(time) + " " + byte.Parse(speed));
        }
    }
    
    /// <summary> Causes the bot to nod down up and down for a certain interval. </summary>
    public sealed class NodInstruction : SpinInstruction {
        public override string Name { get { return "nod"; } }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            Metadata meta = (Metadata)data.Metadata;
            if (bot.countdown == 0) { bot.countdown = meta.Seconds; return true; }
            bot.countdown--;

            byte speed = meta.Speed;
            if (bot.nodUp) {
                if (bot.rot[1] > 32 && bot.rot[1] < 128) {
                    bot.nodUp = !bot.nodUp;
                } else {
                    if (bot.rot[1] + speed > 255) bot.rot[1] = 0;
                    else bot.rot[1] += speed;
                }
            } else {
                if (bot.rot[1] > 128 && bot.rot[1] < 224) {
                    bot.nodUp = !bot.nodUp;
                } else {
                    if (bot.rot[1] - speed < 0) bot.rot[1] = 255;
                    else bot.rot[1] -= speed;
                }
            }

            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
    }
}
