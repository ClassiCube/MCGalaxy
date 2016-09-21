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

        // Change anys door blocks nearby into air forms
        public static void Door(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte block = C.data.Value2;
            bool instant = block == Block.air_door || block == Block.air_switch;
            
            ActivateablePhysics.DoDoors(lvl, (ushort)(x + 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, (ushort)(x - 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z + 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z - 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y - 1), z, instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y + 1), z, instant);
            
            if (block == Block.door_green && lvl.physics != 5) {
                ActivateablePhysics.DoNeighbours(lvl, C.b, x, y, z);
            }
        }
        
        public static void oDoor(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            int oneY = lvl.Width * lvl.Length;
            if (x > 0) ActivateODoor(lvl, ref C, C.b - 1);
            if (x < lvl.Width - 1) ActivateODoor(lvl, ref C, C.b + 1);
            if (y > 0) ActivateODoor(lvl, ref C, C.b - oneY);
            if (y < lvl.Height - 1) ActivateODoor(lvl, ref C, C.b + oneY);
            if (z > 0) ActivateODoor(lvl, ref C, C.b - lvl.Width);
            if (z < lvl.Length - 1) ActivateODoor(lvl, ref C, C.b + lvl.Width);
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        static void ActivateODoor(Level lvl, ref Check C, int index) {
        	byte block = Block.odoor(lvl.blocks[index]);
            if (block == lvl.blocks[C.b]) {
                lvl.AddUpdate(index, block, true);
            }
        }
        
        
        public static void tDoor(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            int oneY = lvl.Width * lvl.Length;
            if (x > 0) ActivateTDoor(lvl, C.b - 1);
            if (x < lvl.Width - 1) ActivateTDoor(lvl, C.b + 1);
            if (y > 0) ActivateTDoor(lvl, C.b - oneY);
            if (y < lvl.Height - 1) ActivateTDoor(lvl, C.b + oneY);
            if (z > 0) ActivateTDoor(lvl, C.b - lvl.Width);
            if (z < lvl.Length - 1) ActivateTDoor(lvl, C.b + lvl.Width);
        }
        
        static void ActivateTDoor(Level lvl, int index) {
            byte block = lvl.blocks[index];
            if (!Block.Props[block].IsTDoor) return;
            
            PhysicsArgs args = default(PhysicsArgs);
            args.Type1 = PhysicsArgs.Wait; args.Value1 = 16;
            args.Type2 = PhysicsArgs.Revert; args.Value2 = block;
            args.Door = true;
            lvl.AddUpdate(index, Block.air, false, args);
        }
    }
}