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
using BlockID = System.UInt16;

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
            BlockID block = lvl.GetBlock(x, y, z);
             
            ActivateODoor(lvl, block, (ushort)(x - 1), y, z);
            ActivateODoor(lvl, block, (ushort)(x + 1), y, z);
            ActivateODoor(lvl, block, x, (ushort)(y - 1), z);
            ActivateODoor(lvl, block, x, (ushort)(y + 1), z);
            ActivateODoor(lvl, block, x, y, (ushort)(z - 1));
            ActivateODoor(lvl, block, x, y, (ushort)(z + 1));
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        static void ActivateODoor(Level lvl, ushort oDoor, ushort x, ushort y, ushort z) {
            int index;
            BlockID block = lvl.GetBlock(x, y, z, out index);
            block = lvl.Props[block].oDoorBlock;
            
            if (index >= 0 && oDoor == block) {
                lvl.AddUpdate(index, oDoor, true);
            }
        }
        
        
        public static void tDoor(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            ActivateTDoor(lvl, (ushort)(x - 1), y, z);
            ActivateTDoor(lvl, (ushort)(x + 1), y, z);
            ActivateTDoor(lvl, x, (ushort)(y - 1), z);
            ActivateTDoor(lvl, x, (ushort)(y + 1), z);
            ActivateTDoor(lvl, x, y, (ushort)(z - 1));
            ActivateTDoor(lvl, x, y, (ushort)(z + 1));
        }
        
        static void ActivateTDoor(Level lvl, ushort x, ushort y, ushort z) {
            int index;
            BlockID block = lvl.GetBlock(x, y, z, out index);
            
            if (index >= 0 && lvl.Props[block].IsTDoor) {
                PhysicsArgs args = ActivateablePhysics.GetTDoorArgs(block);
                lvl.AddUpdate(index, Block.Air, args);
            }
        }
    }
}