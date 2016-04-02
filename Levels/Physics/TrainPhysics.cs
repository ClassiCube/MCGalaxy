/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
		
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
	
	public static class TrainPhysics {
		
		public static void Do(Level lvl, Check C, Random rand) {
			int dirX = rand.Next(1, 10) <= 5 ? 1 : -1;
			int dirY = rand.Next(1, 10) <= 5 ? 1 : -1;
			int dirZ = rand.Next(1, 10) <= 5 ? 1 : -1;
			ushort x, y, z;
			lvl.IntToPos(C.b, out x, out y, out z);

			for (int cx = -dirX; cx != 2 * dirX; cx += dirX)
				for (int cy = -dirY; cy != 2 * dirY; cy += dirY)
					for (int cz = -dirZ; cz != 2 * dirZ; cz += dirZ)
			{
				byte tileBelow = lvl.GetTile((ushort)(x + cx),(ushort)(y + cy - 1), (ushort)(z + cz));
				byte tile = lvl.GetTile((ushort)(x + cx),(ushort)(y + cy), (ushort)(z + cz));
				
				if ((tileBelow == Block.red || tileBelow == Block.op_air) &&
				    (tile == Block.air || tile == Block.water)) {
					lvl.AddUpdate(lvl.PosToInt((ushort)(x + cx), 
					                           (ushort)(y + cy), (ushort)(z + cz)), Block.train);
					lvl.AddUpdate(C.b, Block.air);
					
					byte newBlock = tileBelow == Block.red ? Block.obsidian : Block.glass;
					lvl.AddUpdate(lvl.IntOffset(C.b, 0, -1, 0), newBlock, true,
					          "wait 5 revert " + tileBelow.ToString());
					return;
				}
			}
		}
	}
}
