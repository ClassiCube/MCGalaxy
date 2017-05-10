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
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Maths;

namespace MCGalaxy.Blocks {
    
    internal static class DeleteBehaviour {

        internal static void RocketStart(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics < 2 || p.level.physics == 5) { p.RevertBlock(x, y, z); return; }
            
            int dx = 0, dy = 0, dz = 0;
            p.RevertBlock(x, y, z);
            DirUtils.EightYaw(p.Rot.RotY, out dx, out dz);
            DirUtils.Pitch(p.Rot.HeadX, out dy);

            // Looking straight up or down
            byte pitch = p.Rot.HeadX;
            if (pitch >= 192 && pitch <= 196 || pitch >= 60 && pitch <= 64) { dx = 0; dz = 0; }

            byte b1 = p.level.GetBlock(x + dx * 2, y + dy * 2, z + dz * 2);
            byte b2 = p.level.GetBlock(x + dx    , y + dy,     z + dz);
            if ( b1 == Block.air && b2 == Block.air && p.level.CheckClear((ushort)( x + dx * 2 ), (ushort)( y + dy * 2 ), (ushort)( z + dz * 2 ))
                && p.level.CheckClear((ushort)( x + dx ), (ushort)( y + dy ), (ushort)( z + dz )) ) {
                p.level.Blockchange((ushort)( x + dx * 2 ), (ushort)( y + dy * 2 ), (ushort)( z + dz * 2 ), Block.rockethead);
                p.level.Blockchange((ushort)( x + dx ), (ushort)( y + dy ), (ushort)( z + dz ), Block.lava_fire);
            }
        }
        
        internal static void Firework(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0 || p.level.physics == 5) { p.RevertBlock(x, y, z); return; }
            
            Random rand = new Random();
            ushort x2 = (ushort)(x + rand.Next(0, 2) - 1);
            ushort z2 = (ushort)(z + rand.Next(0, 2) - 1);
            byte b1 = p.level.GetBlock(x2, y + 2, z2);
            byte b2 = p.level.GetBlock(x2, y + 1, z2);
            
            if (b1 == Block.air && b2 == Block.air && p.level.CheckClear(x2, (ushort)(y + 1), z2)
                && p.level.CheckClear(x2, (ushort)(y + 2), z2)) {
                p.level.Blockchange(x2, (ushort)(y + 2), z2, Block.firework);
                PhysicsArgs args = default(PhysicsArgs);
                args.Type1 = PhysicsArgs.Wait; args.Value1 = 1;
                args.Type2 = PhysicsArgs.Dissipate; args.Value2 = 100;
                p.level.Blockchange(x2, (ushort)(y + 1), z2, Block.lavastill, false, args);
            }
            p.RevertBlock(x, y, z);
        }
        
        internal static void C4Det(Player p, byte block, ushort x, ushort y, ushort z) {
            int index = p.level.PosToInt(x, y, z);
            C4Physics.BlowUp(index, p.level);
            p.ChangeBlock(x, y, z, Block.air, 0);
        }
        
        internal static void RevertDoor(Player p, byte block, ushort x, ushort y, ushort z) {
            p.RevertBlock(x, y, z);
        }
        
        internal static void Door(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics != 0) {
                bool isExt = false;
                if (block == Block.custom_block) {
                    isExt = true;
                    block = p.level.GetExtTile(x, y, z);
                }
                
                byte physForm;
                PhysicsArgs args = ActivateablePhysics.GetDoorArgs(block, isExt, out physForm);
                p.level.Blockchange(x, y, z, physForm, false, args);
            } else {
                p.RevertBlock(x, y, z);
            }
        }
        
        internal static void ODoor(Player p, byte block, ushort x, ushort y, ushort z) {
            if (block == Block.odoor8 || block == Block.odoor8_air) {
                p.level.Blockchange(x, y, z, Block.Props[block].ODoorId, 0);
            } else {
                p.RevertBlock(x, y, z);
            }
        }
        
        internal static void DoPortal(Player p, byte block, ushort x, ushort y, ushort z) {
            if (Portal.Handle(p, x, y, z)) {
                p.RevertBlock(x, y, z);
            } else {
                p.ChangeBlock(x, y, z, Block.air, 0);
            }
        }
        
        internal static void DoMessageBlock(Player p, byte block, ushort x, ushort y, ushort z) {
            if (MessageBlock.Handle(p, x, y, z, true)) {
                p.RevertBlock(x, y, z);
            } else {
                p.ChangeBlock(x, y, z, Block.air, 0);
            }
        }
        
        internal static void CustomBlock(Player p, byte block, ushort x, ushort y, ushort z) {
            byte extBlock = p.level.GetExtTile(x, y, z);
            if (p.level.CustomBlockProps[extBlock].IsPortal) {
                DoPortal(p, block, x, y, z);
            } else if (p.level.CustomBlockProps[extBlock].IsMessageBlock) {
                DoMessageBlock(p, block, x, y, z);
            } else if (p.level.CustomBlockProps[extBlock].IsTDoor) {
                RevertDoor(p, block, x, y, z);
            } else if (p.level.CustomBlockProps[extBlock].IsDoor) {
                Door(p, block, x, y, z);
            } else {
                p.ChangeBlock(x, y, z, Block.air, 0);
            }
        }
    }
}
