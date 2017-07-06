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
    public sealed class CmdDelay : Command {
        public override string name { get { return "delay"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            TimeSpan duration = TimeSpan.Zero;
            if (!CommandParser.GetTimespan(p, message, ref duration, "wait for", 's')) return;
            
            if (duration.TotalSeconds > 60) {
                Player.Message(p, "Can only wait for a minute at most."); return;
            }
            
            if (Interlocked.CompareExchange(ref p.UsingDelay, 1, 0) == 1) {
                Player.Message(p, "You are already using /delay."); return;
            }
            
            try {
                Thread.Sleep((int)duration.TotalMilliseconds);
            } finally {
                Interlocked.Exchange(ref p.UsingDelay, 0);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/delay [timespan]");
            Player.Message(p, "%HWaits for a certain amount of time.");
            Player.Message(p, "%HThis is mainly designed for use in %T/mb%H, to run a command after a certain delay.");
            Player.Message(p, "%H  e.g. %T/mb air /delay 10 |/help me");
        }
    }
}
