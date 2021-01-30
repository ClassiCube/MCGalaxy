/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Tasks;

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdTimer : Command2 {
        public override string name { get { return "Timer"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (p.cmdTimer) { p.Message("Can only have one timer at a time. Use /abort to cancel your previous timer."); return; }
            if (message.Length == 0) { Help(p); return; }

            int TotalTime = 0;
            try
            {
                TotalTime = int.Parse(message.SplitSpaces()[0]);
                message = message.Substring(message.IndexOf(' ') + 1);
            }
            catch
            {
                TotalTime = 60;
            }

            if (TotalTime > 300) { p.Message("Cannot have more than 5 minutes in a timer"); return; }

            TimerArgs args = new TimerArgs();
            args.Message = message;
            args.Repeats = (int)(TotalTime / 5) + 1;
            args.Player = p;
            
            p.cmdTimer = true;
            p.level.Message("Timer lasting for " + TotalTime + " seconds has started.");
            p.level.Message(args.Message);
            Server.MainScheduler.QueueRepeat(TimerCallback, args, TimeSpan.FromSeconds(5));
        }
        
        class TimerArgs {
            public string Message;
            public int Repeats;
            public Player Player;
        }
        
        static void TimerCallback(SchedulerTask task) {
            TimerArgs args = (TimerArgs)task.State;            
            Player p = args.Player;

            args.Repeats--;
            if (args.Repeats == 0 || !p.cmdTimer) {
                p.Message("Timer ended.");
                p.cmdTimer = false;
                task.Repeating = false;
            } else {
                p.level.Message(args.Message);
                p.level.Message("Timer has " + (args.Repeats * 5) + " seconds remaining.");
            }
        }
        
        public override void Help(Player p)  {
            p.Message("&T/Timer [time] [message]");
            p.Message("&HStarts a timer which repeats [message] every 5 seconds.");
            p.Message("&HRepeats constantly until [time] has passed");
        }
    }
}
