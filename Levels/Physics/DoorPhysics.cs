/*
    Copyright 2015 MCGalaxy
    Original level physics copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
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
        
        public static void Do(Level lvl, Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            switch (lvl.blocks[C.b]) {

                    //Change any door blocks nearby into door_air
                case Block.door_tree__air:
                case Block.door_obsidian_air:
                case Block.door_glass_air:
                case Block.door_stone_air:
                case Block.door_leaves_air:
                case Block.door_sand_air:
                case Block.door_wood_air:
                case Block.door_green_air:
                case Block.door_stair_air:
                case Block.water_door_air:
                case Block.lava_door_air:
                case Block.door_iron_air:
                case Block.door_gold_air:
                case Block.door_cobblestone_air:
                case Block.door_red_air:
                case Block.door_darkpink_air:
                case Block.door_darkgrey_air:
                case Block.door_lightgrey_air:
                case Block.door_white_air:

                case Block.door_dirt_air:
                case Block.door_grass_air:
                case Block.door_blue_air:
                case Block.door_book_air:
                    AnyDoor(lvl, C, x, y, z, 16);
                    break;
                case Block.air_switch_air:
                case Block.air_door_air:
                    AnyDoor(lvl, C, x, y, z, 4, true);
                    break;
                case Block.door_tnt_air:
                    AnyDoor(lvl, C, x, y, z, 4);
                    break;

                case Block.odoor1_air:
                case Block.odoor2_air:
                case Block.odoor3_air:
                case Block.odoor4_air:
                case Block.odoor5_air:
                case Block.odoor6_air:
                case Block.odoor7_air:
                case Block.odoor8_air:
                case Block.odoor9_air:
                case Block.odoor10_air:
                case Block.odoor11_air:
                case Block.odoor12_air:

                case Block.odoor1:
                case Block.odoor2:
                case Block.odoor3:
                case Block.odoor4:
                case Block.odoor5:
                case Block.odoor6:
                case Block.odoor7:
                case Block.odoor8:
                case Block.odoor9:
                case Block.odoor10:
                case Block.odoor11:
                case Block.odoor12:
                    odoorPhysics(lvl, C);
                    break;
                default:
                    //non special blocks are then ignored, maybe it would be better to avoid getting here and cutting down the list
                    if (!(C.data is string) || !((string)C.data).Contains("wait"))
                        C.time = 255;
                    break;
            }
        }
        
        public static void odoorPhysics(Level lvl, Check C) {
            if (C.time != 0) {
                C.time = 0;
                return;
            }
            
            odoorNeighbour(lvl, C, lvl.IntOffset(C.b, -1, 0, 0));
            odoorNeighbour(lvl, C, lvl.IntOffset(C.b, +1, 0, 0));
            odoorNeighbour(lvl, C, lvl.IntOffset(C.b, 0, -1, 0));
            odoorNeighbour(lvl, C, lvl.IntOffset(C.b, 0, +1, 0));
            odoorNeighbour(lvl, C, lvl.IntOffset(C.b, 0, 0, -1));
            odoorNeighbour(lvl, C, lvl.IntOffset(C.b, 0, 0, +1));
            C.time++;
        }
        
        static void odoorNeighbour(Level lvl, Check C, int offset) {
            int index = C.b + offset;
            byte block = Block.odoor(lvl.GetTile(index));
            
            if (block == lvl.blocks[C.b])
                lvl.AddUpdate(index, block, true);
        }
        
        static void AnyDoor(Level lvl, Check C, ushort x, ushort y, ushort z, int timer, bool instaUpdate = false) {
            if (C.time != 0) {
                CheckDoorRevert(lvl, C, timer); return;
            }
            PhysDoor(lvl, (ushort)(x + 1), y, z, instaUpdate);
            PhysDoor(lvl, (ushort)(x - 1), y, z, instaUpdate);
            PhysDoor(lvl, x, y, (ushort)(z + 1), instaUpdate);
            PhysDoor(lvl, x, y, (ushort)(z - 1), instaUpdate);
            PhysDoor(lvl, x, (ushort)(y - 1), z, instaUpdate);
            PhysDoor(lvl, x, (ushort)(y + 1), z, instaUpdate);
            
            if (lvl.blocks[C.b] != Block.door_green_air) {
                CheckDoorRevert(lvl, C, timer); return;
            }
            
            if (lvl.physics != 5)
                ActivateNeighbours(lvl, C, x, y, z);           
            CheckDoorRevert(lvl, C, timer);
        }
        
        static void ActivateNeighbours(Level lvl, Check C, ushort x, ushort y, ushort z) {
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
                        lvl.AddUpdate(b1, Block.lavastill, false, "dissipate 100");
                    }
                } else if (b == Block.tnt) {
                    lvl.MakeExplosion((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz), 0);
                }
            }
        }

        static void CheckDoorRevert(Level lvl, Check C, int timer) {
            if (C.time < timer) {
                C.time++;
            } else {
                lvl.AddUpdate(C.b, Block.SaveConvert(lvl.blocks[C.b])); //turn back into door
                C.time = 255;
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

            if (Block.tDoor(rawBlock))
                lvl.AddUpdate(index, Block.air, false, "wait 16 door 1 revert " + rawBlock.ToString());
            byte oDoor = Block.odoor(rawBlock);
            if (oDoor != Block.Zero)
                lvl.AddUpdate(index, oDoor, true);
        }
    }
}