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
        public static void Door(Level lvl, ref Check C, int timer, bool instant = false) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            if (C.data.Data != 0) {
                CheckDoorRevert(lvl, ref C, timer); return;
            }
            
            ActivateablePhysics.DoDoors(lvl, (ushort)(x + 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, (ushort)(x - 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z + 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z - 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y - 1), z, instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y + 1), z, instant);
            
            if (lvl.blocks[C.b] == Block.door_green_air && lvl.physics != 5) {
                ActivateablePhysics.DoNeighbours(lvl, C.b, x, y, z);
            }
            CheckDoorRevert(lvl, ref C, timer);
        }
        
        static void CheckDoorRevert(Level lvl, ref Check C, int timer) {
            if (C.data.Data < timer) {
                C.data.Data++;
            } else {
                lvl.AddUpdate(C.b, Block.Props[lvl.blocks[C.b]].DoorId);
                C.data.Data = PhysicsArgs.RemoveFromChecks;
            }
        }
        
        
        public static void oDoor(Level lvl, ref Check C) {
            // TODO: perhaps do proper bounds checking
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, -1, 0, 0));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, +1, 0, 0));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, 0, -1, 0));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, 0, +1, 0));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, 0, 0, -1));
            odoorNeighbour(lvl, ref C, lvl.IntOffset(C.b, 0, 0, +1));
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        static void odoorNeighbour(Level lvl, ref Check C, int index) {
            byte block = Block.odoor(lvl.GetTile(index));
            if (block == lvl.blocks[C.b]) {
                lvl.AddUpdate(index, block, true);
            }
        }
        
        
        public static void tDoor(Level lvl, ref Check C) {
            // TODO: perhaps do proper bounds checking
            tdoorNeighbour(lvl, lvl.IntOffset(C.b, -1, 0, 0));
            tdoorNeighbour(lvl, lvl.IntOffset(C.b, 1, 0, 0));
            tdoorNeighbour(lvl, lvl.IntOffset(C.b, 0, -1, 0));
            tdoorNeighbour(lvl, lvl.IntOffset(C.b, 0, 1, 0));
            tdoorNeighbour(lvl, lvl.IntOffset(C.b, 0, 0, -1));
            tdoorNeighbour(lvl, lvl.IntOffset(C.b, 0, 0, 1));
        }
        
        static void tdoorNeighbour(Level lvl, int index) {
            if (index < 0 || index >= lvl.blocks.Length) return;
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