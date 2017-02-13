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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.SQL;

namespace MCGalaxy.Blocks {
    internal static class WalkthroughBehaviour {

        internal static bool Door(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0) return true;
            bool isExt = false;
            if (block == Block.custom_block) {
                isExt = true;
                block = p.level.GetExtTile(x, y, z);
            }
            
            byte physForm;
            PhysicsArgs args = ActivateablePhysics.GetDoorArgs(block, isExt, out physForm);
            p.level.Blockchange(x, y, z, physForm, false, args);
            return true;
        }
        
        internal static bool Train(Player p, byte block, ushort x, ushort y, ushort z) {
            if (!p.trainInvincible) p.HandleDeath(Block.train, 0);
            return true;
        }
        
        internal static bool CustomBlock(Player p, byte block, ushort x, ushort y, ushort z) {
            byte extBlock = p.level.GetExtTile(x, y, z);
            BlockDefinition def = p.level.CustomBlockDefs[extBlock];
            if (def == null) return false; // custom block was removed      
            if (def.CollideType == 2) return false;
            
            if (p.level.CustomBlockProps[extBlock].IsPortal) {
                return DoPortal(p, block, x, y, z);
            } else if (p.level.CustomBlockProps[extBlock].IsMessageBlock) {
                return DoMessageBlock(p, block, x, y, z);
            } else if (p.level.CustomBlockProps[extBlock].IsDoor) {
                return Door(p, block, x, y, z);
            }
            return false;
        }
        
        
        internal static bool DoPortal(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.PosToInt(x, y, z) == p.lastWalkthrough) return true;
            Portal.Handle(p, x, y, z);
            return true;
        }
        
        internal static bool DoMessageBlock(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.PosToInt(x, y, z) == p.lastWalkthrough) return true;
            MessageBlock.Handle(p, x, y, z, false);
            return true;
        }
        
        internal static bool Checkpoint(Player p, byte block, ushort x, ushort y, ushort z) {
            p.useCheckpointSpawn = true;
            p.checkpointX = x; p.checkpointY = (ushort)(y + 1); p.checkpointZ = z;
            p.checkpointRotX = p.rot[0]; p.checkpointRotY = p.rot[1];
            
            int index = p.level.PosToInt(x, y, z);
            if (index != p.lastCheckpointIndex) {
                p.SpawnEntity(p, Entities.SelfID, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]);
                p.lastCheckpointIndex = index;
            }
            return true;
        }
    }
}
