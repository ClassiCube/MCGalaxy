/*
	Copyright 2015 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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

namespace MCGalaxy.BlockPhysics {

	public static class AIPhysics {
		
		public static Player ClosestPlayer(Level lvl, Check C) {
			if (!lvl.ai ) return null;
			
			int closestDist = 75;
			Player closetPlayer = null;
			ushort x, y, z;
			lvl.IntToPos(C.b, out x, out y, out z);
			
			Player.players.ForEach(
				delegate(Player p)
				{
					if (p.level == lvl && !p.invincible) {
						int curDist = Math.Abs((p.pos[0] / 32) - x) +
							Math.Abs((p.pos[1] / 32) - y) +
							Math.Abs((p.pos[2] / 32) - z);
						
						if (curDist < closestDist) {
							closestDist = curDist;
							closetPlayer = p;
						}
					}
				}
			);
			return closetPlayer;
		}
	}
}
