/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Threading;

namespace MCGalaxy.Commands.World {
    public sealed class CmdDelay : Command2 {
        public override string name { get { return "Delay"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            TimeSpan duration = TimeSpan.Zero;
            if (!CommandParser.GetTimespan(p, message, ref duration, "wait for", "ms")) return;
            
            if (duration.TotalSeconds > 60) {
                p.Message("Can only wait for a minute at most."); return;
            }
            
            if (Interlocked.CompareExchange(ref p.UsingDelay, 1, 0) == 1) {
                p.Message("You are already using /delay."); return;
            }
            
            try {
                Thread.Sleep((int)duration.TotalMilliseconds);
            } finally {
                Interlocked.Exchange(ref p.UsingDelay, 0);
            }
        }
        
        public override void Help(Player p) {
            p.Message("%T/Delay [timespan]");
            p.Message("%HWaits for a certain amount of time.");
            p.Message("%HMain use is to run a command after a certain delay in a %T/MB");
            p.Message("%H  e.g. %T/MB air /Delay 1000ms |/Help Me %Hruns %T/Help Me " +
                           "%H1000 milliseconds (1 second) after the MB is clicked");
        }
    }
}
