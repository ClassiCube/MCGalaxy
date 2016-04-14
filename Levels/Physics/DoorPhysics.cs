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

namespace MCGalaxy.BlockPhysics {
    
    public static class DoorPhysics {
        
        public static void odoorPhysics(Level lvl, ref Check C) {
            if (C.data.Data != 0) { C.data.Data = 0; return; }
            
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, -1, 0, 0));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, +1, 0, 0));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, 0, -1, 0));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, 0, +1, 0));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, 0, 0, -1));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, 0, 0, +1));
            C.data.Data++;
        }
        
        static void odoorNeighbour(Level lvl, ref Check C, int offset) {
            int index = C.b + offset;
            byte block = Block.odoor(lvl.GetTile(index));
            
            if (block == lvl.blocks[C.b])
                lvl.AddUpdate(index, block, true);
        }
        
		//Change any door blocks nearby into door_air
        public static void AnyDoor(Level lvl, ref Check C, int timer, bool instaUpdate = false) {
			ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            if (C.data.Data != 0) {
                CheckDoorRevert(lvl, ref C, timer); return;
            }
            
            PhysDoor(lvl, (ushort)(x + 1), y, z, instaUpdate);
            PhysDoor(lvl, (ushort)(x - 1), y, z, instaUpdate);
            PhysDoor(lvl, x, y, (ushort)(z + 1), instaUpdate);
            PhysDoor(lvl, x, y, (ushort)(z - 1), instaUpdate);
            PhysDoor(lvl, x, (ushort)(y - 1), z, instaUpdate);
            PhysDoor(lvl, x, (ushort)(y + 1), z, instaUpdate);
            
            if (lvl.blocks[C.b] != Block.door_green_air) {
                CheckDoorRevert(lvl, ref C, timer); return;
            }
            
            if (lvl.physics != 5)
                ActivateNeighbours(lvl, ref C, x, y, z);           
            CheckDoorRevert(lvl, ref C, timer);
        }
        
        static void ActivateNeighbours(Level lvl, ref Check C, ushort x, ushort y, ushort z) {
            for (int xx = -1; xx <= 1; xx++)
                for (int yy = -1; yy <= 1; yy++)
                    for (int zz = -1; zz <= 1; zz++)
            {
                byte b = lvl.GetTile(lvl.IntOffset(C.b, xx, yy, zz));
                if (b == Block.rocketstart) {
                    int b1 = lvl.IntOffset(C.b, xx * 3, yy * 3, zz * 3);
                    int b2 = lvl.IntOffset(C.b, xx * 2, yy * 2, zz * 2);
                    bool unblocked = lvl.GetTile(b1) == Block.air && lvl.GetTile(b2) == Block.air &&
                        !lvl.listUpdateExists.Get(x + xx * 3, y + yy * 3, z + zz * 3) &&
                        !lvl.listUpdateExists.Get(x + xx * 2, y + yy * 2, z + zz * 2);
                    
                    if (unblocked) {
                        lvl.AddUpdate(b1, Block.rockethead);
                        lvl.AddUpdate(b2, Block.fire);
                    }
                } else if (b == Block.firework) {
                    int b1 = lvl.IntOffset(C.b, xx, yy + 1, zz);
                    int b2 = lvl.IntOffset(C.b, xx, yy + 2, zz);
                    bool unblocked = lvl.GetTile(b1) == Block.air && lvl.GetTile(b2) == Block.air &&
                        !lvl.listUpdateExists.Get(x + xx, y + yy + 1, z + zz) &&
                        !lvl.listUpdateExists.Get(x + xx, y + yy + 2, z + zz);
                    
                    if (unblocked) {
                        lvl.AddUpdate(b2, Block.firework);
                        PhysicsArgs args = default(PhysicsArgs);
					    args.Type1 = PhysicsArgs.Dissipate; args.Value1 = 100;
                        lvl.AddUpdate(b1, Block.lavastill, false, args);
                    }
                } else if (b == Block.tnt) {
                    lvl.MakeExplosion((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz), 0);
                }
            }
        }

        static void CheckDoorRevert(Level lvl, ref Check C, int timer) {
            if (C.data.Data < timer) {
                C.data.Data++;
            } else {
				lvl.AddUpdate(C.b, Block.Properties[lvl.blocks[C.b]].DoorId);
                C.data.Data = 255;
            }
        }

        static void PhysDoor(Level lvl, ushort x, ushort y, ushort z, bool instaUpdate) {
            int index = lvl.PosToInt(x, y, z);
            if (index < 0) return;
            byte rawBlock = lvl.blocks[index];
            
            byte airDoor = Block.DoorAirs(rawBlock);
            if (airDoor != 0) {
                if (!instaUpdate)
                    lvl.AddUpdate(index, airDoor);
                else
                    lvl.Blockchange(x, y, z, airDoor);
                return;
            }

            if (Block.Properties[rawBlock].IsTDoor) {
            	PhysicsArgs args = default(PhysicsArgs);
                args.Type1 = PhysicsArgs.Wait; args.Value1 = 16;
                args.Type2 = PhysicsArgs.Revert; args.Value2 = rawBlock;
                args.Door = true;
                lvl.AddUpdate(index, Block.air, false, args);
            }
            byte oDoor = Block.odoor(rawBlock);
            if (oDoor != Block.Zero)
                lvl.AddUpdate(index, oDoor, true);
        }
    }
}