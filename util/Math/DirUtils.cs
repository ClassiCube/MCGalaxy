/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy {
    
    public static class DirUtils {

		/* How yaw works:              * How pitch works:
         *         64 | +X             *         192 | +Y
         *         ___|___             *             |
         *        /   |   \            *    flipped  |
         *       /    |    \           *     heads   |
         * 128  |     |     |    0     *  128        |           0
         *------------+-----------     * ------------+------------
         *  +Z  |     |     |   -Z     *  Y=0        |         Y=0
         *       \    |    /           *    flipped  |
         *        \___|___/            *     heads   |
         *            |                *             |
         *        192 | -X             *          64 | -Y
         *                             */
        
        public static void EightYaw(byte yaw, out int dirX, out int dirZ) {
            dirX = 0; dirZ = 0;
            const byte extent = 48;
            
            if (yaw < (0 + extent) || yaw > (256 - extent))
                dirZ = -1;
            else if (yaw > (128 - extent) && yaw < (128 + extent))
                dirZ = 1;

            if (yaw > (64 - extent) && yaw < (64 + extent))
                dirX = 1;
            else if (yaw > (192 - extent) && yaw < (192 + extent))
                dirX = -1;
        }
        
        public static void FourYaw(byte yaw, out int dirX, out int dirZ) {
            dirX = 0; dirZ = 0;
            const byte quadrant = 32;
            
            if (yaw <= (0 + quadrant) || yaw >= (256 - quadrant))
                dirZ = -1;
            else if (yaw <= (128 - quadrant))
                dirX = 1;
            else if (yaw <= (128 + quadrant))
                dirZ = 1;
            else 
                dirX = -1;
        }        
    }
}
