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
using MCGalaxy.Bots;
using MCGalaxy.Tasks;

namespace MCGalaxy {
    
    public static class BotsScheduler {
        
        static Scheduler instance;
        static readonly object activateLock = new object();
        
        public static void Activate() {
            lock (activateLock) {
                if (instance != null) return;
                
                instance = new Scheduler("MCG_BotsScheduler");
                instance.QueueRepeat(BotsTick, null,
                                     TimeSpan.FromMilliseconds(100));
            }
        }
        
        static void BotsTick(SchedulerTask task) {
            Level[] levels = LevelInfo.Loaded.Items;
            for (int i = 0; i < levels.Length; i++) {
                PlayerBot[] bots = levels[i].Bots.Items;
                for (int j = 0; j < bots.Length; j++) { BotTick(bots[j]); }
            }
        }

        static BotInstruction killInstruction = new KillInstruction();
        static BotInstruction huntInstruction = new HuntInstruction();
        static void BotTick(PlayerBot bot) {
            if (bot.kill) {
                // The kill instruction should not interfere with the bot AI
                int actualCur = bot.cur;
                killInstruction.Execute(bot);
                bot.cur = actualCur;
            }
            bot.movement = false;

            if (bot.Instructions.Count == 0) {
                if (bot.hunt) huntInstruction.Execute(bot);
            } else {
                bool doNextInstruction = !bot.Instructions[bot.cur].Execute(bot);
                if (bot.cur == bot.Instructions.Count) bot.cur = 0;
                
                if (doNextInstruction) {
                    bot.Instructions[bot.cur].Execute(bot);
                    if (bot.cur == bot.Instructions.Count) bot.cur = 0;
                }
            }
            
            if (bot.curJump > 0) DoJump(bot);
        }
        
        static void DoJump(PlayerBot bot) {            
            Position pos = bot.Pos;
            switch (bot.curJump) {
                case 1: pos.Y += 24; break;
                case 2: pos.Y += 12; break;
                case 3: break;
                case 4: pos.Y -= 12; break;
                case 5: pos.Y -= 24; bot.curJump = -1; break;
            }
            bot.curJump++;
            bot.Pos = pos;
        }
    }
}