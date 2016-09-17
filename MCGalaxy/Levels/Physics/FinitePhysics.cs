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
using System.Collections.Generic;

namespace MCGalaxy.BlockPhysics {
	
	public static class FinitePhysics {
		
		public unsafe static void DoWaterOrLava(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;			
			ushort x, y, z;
			lvl.IntToPos(C.b, out x, out y, out z);
			
			byte tileBelow = lvl.GetTile(x, (ushort)(y - 1), z);
			if (tileBelow == Block.air) {
				lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), lvl.blocks[C.b], false, C.data);
				lvl.AddUpdate(C.b, Block.air);
				C.data.ResetTypes();
			} else if (tileBelow == Block.waterstill || tileBelow == Block.lavastill) {
				lvl.AddUpdate(C.b, Block.air);
				C.data.ResetTypes();
			} else {
				const int count = 25;
				int* indices = stackalloc int[count];
				for (int i = 0; i < count; ++i)
					indices[i] = i;

				for (int k = count - 1; k > 1; --k) {
					int randIndx = rand.Next(k);
					int temp = indices[k];
					indices[k] = indices[randIndx]; // move random num to end of list.
					indices[randIndx] = temp;
				}

				for (int j = 0; j < count; j++) {
					int i = indices[j];
					ushort posX = (ushort)(x + (i / 5) - 2);
					ushort posZ = (ushort)(z + (i % 5) - 2);
					if (lvl.GetTile(posX, (ushort)(y - 1), posZ) == Block.air &&
					    lvl.GetTile(posX, y, posZ) == Block.air)
					{
						if (posX < x)
							posX = (ushort)(Math.Floor((double)(posX + x) / 2));
						else
							posX = (ushort)(Math.Ceiling((double)(posX + x) / 2));
						if (posZ < z)
							posZ = (ushort)(Math.Floor((double)(posZ + z) / 2));
						else
							posZ = (ushort)(Math.Ceiling((double)(posZ + z) / 2));

						int index = lvl.PosToInt(posX, y, posZ);
						if (index >= 0 && lvl.blocks[index] == Block.air &&
						    lvl.AddUpdate(index, lvl.blocks[C.b], false, C.data))
						{
							lvl.AddUpdate(C.b, Block.air);
							C.data.ResetTypes();
							return;
						}
					}
				}
			}
		}
		
		static bool Expand(Level lvl, int x, int y, int z) {
			int index = lvl.PosToInt((ushort)x, (ushort)y, (ushort)z);
			if (index >= 0 && lvl.blocks[index] == Block.air &&
			    lvl.AddUpdate(index, Block.finiteWater)) {
				return true;
			}
			return false;
		}
		
		public unsafe static void DoFaucet(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;			
			ushort x, y, z;
			lvl.IntToPos(C.b, out x, out y, out z);
			
			const int count = 6;
			int* indices = stackalloc int[count];
			for (int i = 0; i < count; ++i)
				indices[i] = i;

			for (int k = count - 1; k > 1; --k) {
				int randIndx = rand.Next(k);
				int temp = indices[k];
				indices[k] = indices[randIndx]; // move random num to end of list.
				indices[randIndx] = temp;
			}

			for (int j = 0; j < count; j++) {
				int i = indices[j];
				switch (i) {
					case 0:
						if (Expand(lvl, x - 1, y, z ) ) return;
						break;
					case 1:
						if (Expand(lvl, x + 1, y, z ) ) return;
						break;
					case 2:
						if (Expand(lvl, x, y - 1, z ) ) return;
						break;
					case 3:
						if (Expand(lvl, x, y + 1, z ) ) return;
						break;
					case 4:
						if (Expand(lvl, x, y, z - 1 ) ) return;
						break;
					case 5:
						if (Expand(lvl, x, y, z + 1 ) ) return;
						break;
				}
			}
		}
	}
}