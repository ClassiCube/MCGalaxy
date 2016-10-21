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
using MCGalaxy.BlockPhysics;

namespace MCGalaxy.Blocks {
    
    internal static class DeleteBehaviour {

        internal static bool RocketStart(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics < 2 || p.level.physics == 5) { p.RevertBlock(x, y, z); return true; }
            
            int dx = 0, dy = 0, dz = 0;
            p.RevertBlock(x, y, z);
            DirUtils.EightYaw(p.rot[0], out dx, out dz);
            DirUtils.Pitch(p.rot[1], out dy);

            // Looking straight up or down
            if (p.rot[1] >= 192 && p.rot[1] <= 196 || p.rot[1] >= 60 && p.rot[1] <= 64) { dx = 0; dz = 0; }

            byte b1 = p.level.GetTile((ushort)( x + dx * 2 ), (ushort)( y + dy * 2 ), (ushort)( z + dz * 2 ));
            byte b2 = p.level.GetTile((ushort)( x + dx ), (ushort)( y + dy ), (ushort)( z + dz ));
            if ( b1 == Block.air && b2 == Block.air && p.level.CheckClear((ushort)( x + dx * 2 ), (ushort)( y + dy * 2 ), (ushort)( z + dz * 2 ))
                && p.level.CheckClear((ushort)( x + dx ), (ushort)( y + dy ), (ushort)( z + dz )) ) {
                p.level.Blockchange((ushort)( x + dx * 2 ), (ushort)( y + dy * 2 ), (ushort)( z + dz * 2 ), Block.rockethead);
                p.level.Blockchange((ushort)( x + dx ), (ushort)( y + dy ), (ushort)( z + dz ), Block.lava_fire);
            }
            return true;
        }
        
        internal static bool Firework(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0 || p.level.physics == 5) { p.RevertBlock(x, y, z); return true; }
            
            Random rand = new Random();
            ushort x2 = (ushort)(x + rand.Next(0, 2) - 1);
            ushort z2 = (ushort)(z + rand.Next(0, 2) - 1);
            byte b1 = p.level.GetTile(x2, (ushort)(y + 2), z2);
            byte b2 = p.level.GetTile(x2, (ushort)(y + 1), z2);
            
            if (b1 == Block.air && b2 == Block.air && p.level.CheckClear(x2, (ushort)(y + 1), z2)
                && p.level.CheckClear(x2, (ushort)(y + 2), z2)) {
                p.level.Blockchange(x2, (ushort)(y + 2), z2, Block.firework);
                PhysicsArgs args = default(PhysicsArgs);
                args.Type1 = PhysicsArgs.Wait; args.Value1 = 1;
                args.Type2 = PhysicsArgs.Dissipate; args.Value2 = 100;
                p.level.Blockchange(x2, (ushort)(y + 1), z2, Block.lavastill, false, args);
            }
            p.RevertBlock(x, y, z); return true;
        }
        
        internal static bool C4Det(Player p, byte block, ushort x, ushort y, ushort z) {
            C4Physics.BlowUp(new ushort[] { x, y, z }, p.level);
            p.ChangeBlock(x, y, z, Block.air, 0);
            return false;
        }
        
        internal static bool RevertDoor(Player p, byte block, ushort x, ushort y, ushort z) {
            p.RevertBlock(x, y, z);
            return true;
        }
        
        internal static bool Door(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics != 0) {
                byte physForm;
                PhysicsArgs args = ActivateablePhysics.GetDoorArgs(block, out physForm);
                p.level.Blockchange(x, y, z, physForm, false, args);
            } else {
                p.RevertBlock(x, y, z);
            }
            return true;
        }
        
        internal static bool ODoor(Player p, byte block, ushort x, ushort y, ushort z) {
            if (block == Block.odoor8 || block == Block.odoor8_air) {
                p.level.Blockchange(x, y, z, Block.Props[block].ODoorId, 0);
            } else {
                p.RevertBlock(x, y, z);
            }
            return true;
        }
        
        internal static bool CustomBlock(Player p, byte block, ushort x, ushort y, ushort z) {
            byte extBlock = p.level.GetExtTile(x, y, z);
            if (p.level.CustomBlockProps[extBlock].IsPortal) {
                return WalkthroughBehaviour.Portal(p, block, x, y, z, false);
            } else if (p.level.CustomBlockProps[extBlock].IsMessageBlock) {
                return WalkthroughBehaviour.MessageBlock(p, block, x, y, z, false);
            }
            
            p.ChangeBlock(x, y, z, Block.air, 0);
            return false;
        }
    }
}
