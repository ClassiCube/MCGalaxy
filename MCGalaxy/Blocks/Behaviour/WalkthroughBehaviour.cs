﻿/*
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

        internal static bool Door(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0) return true;

            byte physForm;
            PhysicsArgs args = ActivateablePhysics.GetDoorArgs(block, out physForm);
            p.level.Blockchange(x, y, z, (ExtBlock)physForm, false, args);
            return true;
        }
        
        internal static bool Train(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (!p.trainInvincible) p.HandleDeath((ExtBlock)Block.train);
            return true;
        }       
        
        internal static bool DoPortal(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.PosToInt(x, y, z) == p.lastWalkthrough) return true;
            return Portal.Handle(p, x, y, z);
        }
        
        internal static bool DoMessageBlock(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.PosToInt(x, y, z) == p.lastWalkthrough) return true;
            return MessageBlock.Handle(p, x, y, z, false);
        }
        
        internal static bool Checkpoint(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            p.useCheckpointSpawn = true;
            p.checkpointX = x; p.checkpointY = (ushort)(y + 1); p.checkpointZ = z;
            p.checkpointRotX = p.Rot.RotY; p.checkpointRotY = p.Rot.HeadX;
            
            int index = p.level.PosToInt(x, y, z);
            if (index != p.lastCheckpointIndex) {
                Entities.Spawn(p, p);
                p.lastCheckpointIndex = index;
            }
            return true;
        }
    }
}
