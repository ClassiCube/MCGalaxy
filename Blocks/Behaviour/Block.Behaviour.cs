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
using MCGalaxy.BlockBehaviour;

namespace MCGalaxy {
    
    public sealed partial class Block {
        
        /// <summary> Returns whether this block handles the player placing a block at the given coordinates. </summary>
        /// <remarks>If this returns true, the usual 'dirt/grass below' behaviour and 'adding to the BlockDB' is skipped. </remarks>
        public delegate bool HandleDelete(Player p, byte block, ushort x, ushort y, ushort z);
        internal static HandleDelete[] deleteHandlers = new HandleDelete[256];
        
        /// <summary> Returns whether this block handles the player deleting a block at the given coordinates. </summary>
        /// <remarks>If this returns true, the usual 'checking dirt/grass below' and 'adding to the BlockDB' is skipped. </remarks>
        public delegate bool HandlePlace(Player p, byte block, ushort x, ushort y, ushort z);
        internal static HandlePlace[] placeHandlers = new Block.HandlePlace[256];
        
        /// <summary> Returns whether this block handles the player walking through this block at the given coordinates. </summary>
        /// <remarks>If this returns true, the usual 'death check' behaviour is skipped. </remarks>
        public delegate bool HandleWalkthrough(Player p, byte block, ushort x, ushort y, ushort z);
        internal static HandleWalkthrough[] walkthroughHandlers = new Block.HandleWalkthrough[256];
        
        static void SetupCoreHandlers() {
            deleteHandlers[Block.rocketstart] = DeleteBehaviour.RocketStart;
            deleteHandlers[Block.firework] = DeleteBehaviour.Firework;
            walkthroughHandlers[Block.checkpoint] = WalkthroughBehaviour.Checkpoint;
            deleteHandlers[Block.c4det] = DeleteBehaviour.C4Det;
            placeHandlers[Block.dirt] = PlaceBehaviour.Dirt;
            placeHandlers[Block.staircasestep] = PlaceBehaviour.Stairs;
            
            for (int i = 0; i < 256; i++) {
                if (Block.mb((byte)i)) {
                    walkthroughHandlers[i] = WalkthroughBehaviour.MessageBlock;
                    deleteHandlers[i] = WalkthroughBehaviour.MessageBlock;
                } else if (Block.portal((byte)i)) {
                    walkthroughHandlers[i] = WalkthroughBehaviour.Portal;
                    deleteHandlers[i] = WalkthroughBehaviour.Portal;
                }
                
                byte doorAir = Block.DoorAirs((byte)i); // if not 0, means i is a door block
                if (Block.tDoor((byte)i)) {
                    deleteHandlers[i] = DeleteBehaviour.RevertDoor;
                } else if (Block.odoor((byte)i) != Block.Zero) {
                    deleteHandlers[i] = DeleteBehaviour.ODoor;
                } else if (doorAir != 0) {
                    deleteHandlers[doorAir] = DeleteBehaviour.RevertDoor;
                    deleteHandlers[i] = DeleteBehaviour.Door;
                }
            }
        }
    }
}
