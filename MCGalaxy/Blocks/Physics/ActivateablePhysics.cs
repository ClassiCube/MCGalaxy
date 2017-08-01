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

namespace MCGalaxy.Blocks.Physics {
    public static class ActivateablePhysics {
        
        /// <summary> Activates fireworks, rockets, and TNT in 1 block radius around (x, y, z) </summary>
        public static void DoNeighbours(Level lvl, int index, ushort x, ushort y, ushort z) {
            for (int yy = -1; yy <= 1; yy++)
                for (int zz = -1; zz <= 1; zz++)
                    for (int xx = -1; xx <= 1; xx++)
            {
                byte b = lvl.GetTile(lvl.IntOffset(index, xx, yy, zz));
                if (b == Block.RocketStart) {
                    int b1 = lvl.IntOffset(index, xx * 3, yy * 3, zz * 3);
                    int b2 = lvl.IntOffset(index, xx * 2, yy * 2, zz * 2);
                    bool unblocked = lvl.GetTile(b1) == Block.Air && lvl.GetTile(b2) == Block.Air &&
                        !lvl.listUpdateExists.Get(x + xx * 3, y + yy * 3, z + zz * 3) &&
                        !lvl.listUpdateExists.Get(x + xx * 2, y + yy * 2, z + zz * 2);
                    
                    if (unblocked) {
                        lvl.AddUpdate(b1, Block.RocketHead);
                        lvl.AddUpdate(b2, Block.LavaFire);
                    }
                } else if (b == Block.Fireworks) {
                    int b1 = lvl.IntOffset(index, xx, yy + 1, zz);
                    int b2 = lvl.IntOffset(index, xx, yy + 2, zz);
                    bool unblocked = lvl.GetTile(b1) == Block.Air && lvl.GetTile(b2) == Block.Air &&
                        !lvl.listUpdateExists.Get(x + xx, y + yy + 1, z + zz) &&
                        !lvl.listUpdateExists.Get(x + xx, y + yy + 2, z + zz);
                    
                    if (unblocked) {
                        lvl.AddUpdate(b2, Block.Fireworks);
                        PhysicsArgs args = default(PhysicsArgs);
                        args.Type1 = PhysicsArgs.Dissipate; args.Value1 = 100;
                        lvl.AddUpdate(b1, Block.StillLava, false, args);
                    }
                } else if (b == Block.TNT) {
                    lvl.MakeExplosion((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz), 0);
                }
            }
        }

        /// <summary> Activates doors, tdoors and toggles odoors at (x, y, z) </summary>
        public static void DoDoors(Level lvl, ushort x, ushort y, ushort z, bool instant) {
            int index;
            ExtBlock block = lvl.GetBlock(x, y, z, out index);
            if (index == -1) return;
            
            int i = block.Index;
            if (lvl.BlockProps[i].IsDoor) {
                byte physForm;
                PhysicsArgs args = GetDoorArgs(block, out physForm);
                if (!instant) lvl.AddUpdate(index, physForm, false, args);
                else lvl.Blockchange(index, (ExtBlock)physForm, false, args);
            } else if (lvl.BlockProps[i].IsTDoor) {
                PhysicsArgs args = GetTDoorArgs(block);
                lvl.AddUpdate(index, Block.Air, false, args);
            } else {
                byte oDoor = lvl.BlockProps[i].oDoorId;
                if (oDoor != Block.Invalid)
                    lvl.AddUpdate(index, oDoor, true);
            }
        }
        
        
        internal static PhysicsArgs GetDoorArgs(ExtBlock block, out byte physForm) {
            PhysicsArgs args = default(PhysicsArgs);
            args.Type1 = PhysicsArgs.Wait; args.Value1 = 16 - 1;
            args.Type2 = PhysicsArgs.Revert; args.Value2 = block.RawID;
            args.Door = true;
            args.ExtBlock = block.BlockID == Block.custom_block;
            
            physForm = Block.Door_Log_air; // air
            if (block.BlockID == Block.Door_Air || block.BlockID == Block.Door_AirActivatable) {
                args.Value1 = 4 - 1;
            } else if (block.BlockID == Block.Door_Green) {
                physForm = Block.Door_Green_air; // red wool
            } else if (block.BlockID == Block.Door_TNT) {
                args.Value1 = 4 - 1; physForm = Block.Door_TNT_air; // lava
            }
            return args;
        }
        
        internal static PhysicsArgs GetTDoorArgs(ExtBlock block) {
            PhysicsArgs args = default(PhysicsArgs);
            args.Type1 = PhysicsArgs.Wait; args.Value1 = 16;
            args.Type2 = PhysicsArgs.Revert; args.Value2 = block.RawID;
            args.Door = true;
            args.ExtBlock = block.BlockID == Block.custom_block;
            return args;
        }
        
        
        internal static void CheckNeighbours(Level lvl, ushort x, ushort y, ushort z) {
            CheckAt(lvl, lvl.PosToInt((ushort)(x + 1), y, z));
            CheckAt(lvl, lvl.PosToInt((ushort)(x - 1), y, z));
            CheckAt(lvl, lvl.PosToInt(x, y, (ushort)(z + 1)));
            CheckAt(lvl, lvl.PosToInt(x, y, (ushort)(z - 1)));
            CheckAt(lvl, lvl.PosToInt(x, (ushort)(y + 1), z));
            // ommission of y-1 to match original behaviour
        }
        
        internal static void CheckAt(Level lvl, int index) {
            if (index == -1) return;
            byte block = lvl.blocks[index];
            byte convBlock = Block.Convert(block);
            if (convBlock == Block.Water || convBlock == Block.Lava ||
                (block >= Block.Red && block <= Block.White)) {
                lvl.AddCheck(index); return;
            }

            switch (block) {
                    //case Block.water:
                    //case Block.lava:
                case Block.Sapling:
                case Block.Sand:
                case Block.Gravel:
                case Block.Leaves:
                case Block.FloatWood:
                    /*case Block.lava_fast:
                    case Block.WaterDown:
                    case Block.LavaDown:
                    case Block.deathlava:
                    case Block.deathwater:
                    case Block.geyser:
                    case Block.magma:*/
                    lvl.AddCheck(index);
                    break;
                default:
                    break;
            }
        }
    }
}