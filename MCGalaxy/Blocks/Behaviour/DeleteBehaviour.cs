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

        internal static void RocketStart(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.physics < 2 || p.level.physics == 5) { p.RevertBlock(x, y, z); return; }
            
            int dx = 0, dy = 0, dz = 0;
            p.RevertBlock(x, y, z);
            DirUtils.EightYaw(p.Rot.RotY, out dx, out dz);
            DirUtils.Pitch(p.Rot.HeadX, out dy);

            // Looking straight up or down
            byte pitch = p.Rot.HeadX;
            if (pitch >= 192 && pitch <= 196 || pitch >= 60 && pitch <= 64) { dx = 0; dz = 0; }
            Vec3U16 head = new Vec3U16((ushort)(x + dx * 2), (ushort)(y + dy * 2), (ushort)(z + dz * 2));
            Vec3U16 tail = new Vec3U16((ushort)(x + dx), (ushort)(y + dy), (ushort)(z + dz));
            
            bool headFree = p.level.IsAirAt(head.X, head.Y, head.Z) && p.level.CheckClear(head.X, head.Y, head.Z);
            bool tailFree = p.level.IsAirAt(tail.X, tail.Y, tail.Z) && p.level.CheckClear(tail.X, tail.Y, tail.Z);
            if (headFree && tailFree) {
                p.level.Blockchange(head.X, head.Y, head.Z, (ExtBlock)Block.rockethead);
                p.level.Blockchange(tail.X, tail.Y, tail.Z, (ExtBlock)Block.lava_fire);
            }
        }
        
        internal static void Firework(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0 || p.level.physics == 5) { p.RevertBlock(x, y, z); return; }
            
            Random rand = new Random();
            // Offset the firework randomly
            Vec3U16 pos = new Vec3U16(0, 0, 0);
            pos.X = (ushort)(x + rand.Next(0, 2) - 1);
            pos.Z = (ushort)(z + rand.Next(0, 2) - 1);
            ushort headY = (ushort)(y + 2), tailY = (ushort)(y + 1);

            bool headFree = p.level.IsAirAt(pos.X, headY, pos.Z) && p.level.CheckClear(pos.X, headY, pos.Z);
            bool tailFree = p.level.IsAirAt(pos.X, tailY, pos.Z) && p.level.CheckClear(pos.X, tailY, pos.Z);            
            if (headFree && tailFree) {
                p.level.Blockchange(pos.X, headY, pos.Z, (ExtBlock)Block.firework);
                
                PhysicsArgs args = default(PhysicsArgs);
                args.Type1 = PhysicsArgs.Wait; args.Value1 = 1;
                args.Type2 = PhysicsArgs.Dissipate; args.Value2 = 100;
                p.level.Blockchange(pos.X, tailY, pos.Z, (ExtBlock)Block.lavastill, false, args);
            }
            p.RevertBlock(x, y, z);
        }
        
        internal static void C4Det(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            int index = p.level.PosToInt(x, y, z);
            C4Physics.BlowUp(index, p.level);
            p.ChangeBlock(x, y, z, ExtBlock.Air);
        }
        
        internal static void RevertDoor(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            p.RevertBlock(x, y, z);
        }
        
        internal static void Door(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.physics != 0) {             
                byte physForm;
                PhysicsArgs args = ActivateablePhysics.GetDoorArgs(block, out physForm);
                p.level.Blockchange(x, y, z, (ExtBlock)physForm, false, args);
            } else {
                p.RevertBlock(x, y, z);
            }
        }
        
        internal static void ODoor(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (block.BlockID == Block.odoor8 || block.BlockID == Block.odoor8_air) {
                p.level.Blockchange(x, y, z, (ExtBlock)p.level.BlockProps[block.Index].ODoorId);
            } else {
                p.RevertBlock(x, y, z);
            }
        }
        
        internal static void DoPortal(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (Portal.Handle(p, x, y, z)) {
                p.RevertBlock(x, y, z);
            } else {
                p.ChangeBlock(x, y, z, ExtBlock.Air);
            }
        }
        
        internal static void DoMessageBlock(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (MessageBlock.Handle(p, x, y, z, true)) {
                p.RevertBlock(x, y, z);
            } else {
                p.ChangeBlock(x, y, z, ExtBlock.Air);
            }
        }
    }
}
