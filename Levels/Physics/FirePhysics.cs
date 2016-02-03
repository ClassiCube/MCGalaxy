/*
	Copyright 2015 MCGalaxy
	Original level physics copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
		
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
	
	public static class FirePhysics {
		
		static bool ExpandSimple(Level lvl, int x, int y, int z) {
			int index = lvl.PosToInt((ushort)x, (ushort)y, (ushort)z);
			if (index < 0 || lvl.blocks[index] != Block.air) return false;
			
			lvl.AddUpdate(index, Block.fire);
			return true;
		}
		
		static void ExpandDiagonal(Level lvl, ushort x, ushort y, ushort z,
		                           int xOffset, int yOffset, int zOffset) {
			if (!Block.FireKill(lvl.GetTile((ushort)(x + xOffset),
			                                (ushort)(y + yOffset), (ushort)(z + zOffset))))
				return;
			
			if (xOffset != 0)
				lvl.AddUpdate(lvl.PosToInt((ushort)(x + xOffset), y, z), Block.fire);
			if (yOffset != 0)
				lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y + yOffset), z), Block.fire);
			if (zOffset != 0)
				lvl.AddUpdate(lvl.PosToInt(x, y, (ushort)(z + zOffset)), Block.fire);
		}
		
		static void ExpandAvanced(Level lvl, int x, int y, int z) {
			int index = lvl.PosToInt((ushort)x, (ushort)y, (ushort)z);
			if (index < 0) return;
			byte block = lvl.blocks[index];
			
			if (Block.FireKill(block))
				lvl.AddUpdate(index, Block.fire);
			else if (block == Block.tnt)
				lvl.MakeExplosion((ushort)x, (ushort)y, (ushort)z, -1);
		}
		
		public static void Do(Level lvl, Check C, Random rand) {
			ushort x, y, z;
			lvl.IntToPos(C.b, out x, out y, out z);
			if (C.time < 2) {
				C.time++;
				return;
			}

			if (rand.Next(1, 20) == 1 && C.time % 2 == 0) {
				int max = rand.Next(1, 18);

				if (max <= 3 && ExpandSimple(lvl, x - 1, y, z)) {
				} else if (max <= 6 && ExpandSimple(lvl, x + 1, y, z)) {
				} else if (max <= 9 && ExpandSimple(lvl, x, y - 1, z)) {
				} else if (max <= 12 && ExpandSimple(lvl, x, y + 1, z)) {
				} else if (max <= 15 && ExpandSimple(lvl, x, y, z - 1)) {
				} else if (max <= 18 && ExpandSimple(lvl, x, y, z + 1)) {
				}
			}
			for (int yy = -1; yy <= 1; yy++ ) {
				ExpandDiagonal(lvl, x, y, z, -1, yy, -1);
				ExpandDiagonal(lvl, x, y, z, +1, yy, -1);
				ExpandDiagonal(lvl, x, y, z, -1, yy, +1);
				ExpandDiagonal(lvl, x, y, z, +1, yy, +1);
			}

			if (lvl.physics >= 2) {
				if (C.time < 4) {
					C.time++;
					return;
				}
				
				ExpandAvanced(lvl, x - 1, y, z);
				ExpandAvanced(lvl, x + 1, y, z);
				ExpandAvanced(lvl, x, y - 1, z);
				ExpandAvanced(lvl, x, y + 1, z);
				ExpandAvanced(lvl, x, y, z - 1);
				ExpandAvanced(lvl, x, y, z + 1);
			}

			C.time++;
			if (C.time > 5) {
				int dropType = rand.Next(1, 10);
				if (dropType <= 2) {
					lvl.AddUpdate(C.b, Block.coal);
					C.data = "drop 63 dissipate 10";
				} else if (dropType <= 4) {
					lvl.AddUpdate(C.b, Block.obsidian);
					C.data = "drop 63 dissipate 10";
				} else if (dropType <= 8)
					lvl.AddUpdate(C.b, Block.air);
				else
					C.time = 3;
			}
		}
	}
}
