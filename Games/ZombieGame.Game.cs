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

namespace MCGalaxy {
    
    public sealed partial class ZombieGame : IGame {
        
        public override bool HandlesManualChange(Player p, ushort x, ushort y, ushort z, 
		                                         byte action, byte tile, byte b) {
            if (action == 1 && Server.noPillaring && !p.referee) {
                if (p.lastYblock == y - 1 && p.lastXblock == x && p.lastZblock == z ) {
                    p.blocksStacked++;
                } else {
                    p.blocksStacked = 0;
                }
                
                if (p.blocksStacked == 2 ) {
                    p.SendMessage("You are pillaring! Stop before you get kicked!");
                }
                if (p.blocksStacked == 4 ) {
                    Command.all.Find("kick").Use(null, p.name + " No pillaring allowed!");
                    return true;
                }
            }
            p.lastXblock = x; p.lastYblock = y; p.lastZblock = z;
            
            if (action == 1 || (action == 0 && p.painting)) {
                if (p.level.name != Server.zombie.currentLevelName || p.referee) return false;
                
                if (p.blockCount == 0 ) {
                    p.SendMessage("You have no blocks left.");
                    p.RevertBlock(x, y, z); return true;
                }

                p.blockCount--;
                if ((p.blockCount % 10) == 0 || (p.blockCount >= 0 && p.blockCount <= 10)) {
                    p.SendMessage("Blocks Left: " + c.maroon + p.blockCount);
                }
            }
            return false;
        }
    }
}
