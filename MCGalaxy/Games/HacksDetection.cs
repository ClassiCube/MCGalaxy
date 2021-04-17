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
using System.Collections.Generic;

namespace MCGalaxy.Games {
    
    public abstract class HacksDetector {
        protected List<DateTime> log = new List<DateTime>(5);
        protected DateTime lastWarn;
        protected Player player;
        
        public HacksDetector(Player p) { player = p; }
        
        protected void Warn(string action) {
            DateTime now = DateTime.UtcNow;
            if (now < lastWarn) return;
            
            player.Message("&4Do not {0} &W- ops have been warned.", action);
            Chat.MessageFromOps(player, "λNICK &4appears to be " + action + "ing");
            Logger.Log(LogType.SuspiciousActivity, "{0} appears to be {1}ing", player.name, action);
            lastWarn = now.AddSeconds(5);
        }
        
        protected static TimeSpan interval = TimeSpan.FromSeconds(5);
    }
    
    public sealed class SpeedhackDetector : HacksDetector {   

        public SpeedhackDetector(Player p) : base(p) { }
        
        public bool Detect(Position newPos, float moveDist) {
            Player p = player;
            if (p.Game.Referee || Hacks.CanUseSpeed(p)) return false;
            int dx = Math.Abs(p.Pos.X - newPos.X), dz = Math.Abs(p.Pos.Z - newPos.Z);
            
            int maxMove = (int)(moveDist * 32);
            bool speeding = dx >= maxMove || dz >= maxMove;         
            if (!speeding || log.AddSpamEntry(5, interval)) return false;
            
            Warn("speedhack");
            p.SendPos(Entities.SelfID, p.Pos, p.Rot);
            return true;
        }
    }
       
    public sealed class NoclipDetector : HacksDetector {
        
        public NoclipDetector(Player p) : base(p) { }
        
        public bool Detect(Position newPos) {
            Player p = player;
            if (p.Game.Referee || Hacks.CanUseNoclip(p)) return false;
            if (!p.IsLikelyInsideBlock() || log.AddSpamEntry(5, interval)) return false;
            
            Warn("noclip");
            return false;
        }
    }
}
