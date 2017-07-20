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
    public static class DoorPhysics {

        // Change anys door blocks nearby into air forms
        public static void Door(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte block = C.data.Value2;
            bool instant = !C.data.ExtBlock && (block == Block.Door_Air || block == Block.Door_AirActivatable);
            
            ActivateablePhysics.DoDoors(lvl, (ushort)(x + 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, (ushort)(x - 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z + 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z - 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y - 1), z, instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y + 1), z, instant);
            
            if (block == Block.Door_Green && lvl.physics != 5) {
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
            byte block = Block.Props[lvl.blocks[index]].ODoorId;
            if (block == lvl.blocks[C.b]) {
                lvl.AddUpdate(index, block, true);
            }
        }
        
        
        public static void tDoor(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            ActivateTDoor(lvl, (ushort)(x - 1), y, z);
            ActivateTDoor(lvl, (ushort)(x + 1), y, z);
            ActivateTDoor(lvl, x, (ushort)(y - 1), z);
            ActivateTDoor(lvl, x, (ushort)(y + 1), z);
            ActivateTDoor(lvl, x, y, (ushort)(z -1));
            ActivateTDoor(lvl, x, y, (ushort)(z + 1));
        }
        
        static void ActivateTDoor(Level lvl, ushort x, ushort y, ushort z) {
            int index;
            ExtBlock block = lvl.GetBlock(x, y, z, out index);
            
            if (index >= 0 && lvl.BlockProps[block.Index].IsTDoor) {
                PhysicsArgs args = ActivateablePhysics.GetTDoorArgs(block);
                lvl.AddUpdate(index, Block.Air, false, args);
            }         
        }
    }
}