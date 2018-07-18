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

namespace MCGalaxy.Games {
    internal static class MovementCheck {
        
        public static bool InRange(Player a, Player b, int dist) {
            int dx = Math.Abs(a.Pos.X - b.Pos.X);
            int dy = Math.Abs(a.Pos.Y - b.Pos.Y);
            int dz = Math.Abs(a.Pos.Z - b.Pos.Z);
            return dx <= dist && dy <= dist && dz <= dist;
        }
        static TimeSpan interval = TimeSpan.FromSeconds(5);
        
        public static bool DetectNoclip(Player p, Position newPos) {
            if (p.Game.Referee || Hacks.CanUseNoclip(p, p.level)) return false;
            if (!p.IsInsideBlock() || p.Game.NoclipLog.AddSpamEntry(5, interval)) return false;
            
            Warn(ref p.Game.LastNoclipWarn, p, "noclip");
            return false;
        }
        
        public static bool DetectSpeedhack(Player p, Position newPos, float moveDist) {
            if (p.Game.Referee || Hacks.CanUseSpeed(p, p.level)) return false;
            int dx = Math.Abs(p.Pos.X - newPos.X), dz = Math.Abs(p.Pos.Z - newPos.Z);
            
            int maxMove = (int)(moveDist * 32);
            bool speedhacking = dx >= maxMove || dz >= maxMove;         
            if (!speedhacking || p.Game.SpeedhackLog.AddSpamEntry(5, interval)) return false;
            
            Warn(ref p.Game.LastSpeedhackWarn, p, "speedhack");
            p.SendPos(Entities.SelfID, p.Pos, p.Rot);
            return true;
        }
        
        static void Warn(ref DateTime last, Player p, string action) {
            DateTime now = DateTime.UtcNow;
            if (now < last) return;
            
            p.Message("&4Do not {0} %W- ops have been warned.", action);
            Chat.MessageFromOps(p, "λNICK &4appears to be " + action + "ing");
            Logger.Log(LogType.SuspiciousActivity, "{0} appears to be {1}ing", p.name, action);
            last = now.AddSeconds(5);
        }
    }
}
