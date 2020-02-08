/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy.Undo {
    
    public sealed class UndoDrawOpEntry {
        public string DrawOpName;
        public string LevelName;
        public DateTime Start, End;
        
        public void Init(string op, string map) {
            DrawOpName = op; LevelName = map;
            // Use same time method as DoBlockchange writing to undo buffer
            int timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
            Start = Server.StartTime.AddTicks(timeDelta * TimeSpan.TicksPerSecond);
        }
        
        public void Finish(Player p) {
            int timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds + 1;
            End = Server.StartTime.AddTicks(timeDelta * TimeSpan.TicksPerSecond);
            
            p.DrawOps.Add(this);
            if (p.DrawOps.Count > 200) p.DrawOps.RemoveFirst();
        }
    }
}