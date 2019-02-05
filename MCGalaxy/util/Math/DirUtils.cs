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

namespace MCGalaxy.Maths {
    
    public static class DirUtils {

        /* How yaw works:              * How pitch works:
         *                             *
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
            const byte extent = (64 / 4) * 3;
            
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
            const byte quadrant = 64 / 2;
            
            if (yaw <= (0 + quadrant) || yaw >= (256 - quadrant))
                dirZ = -1;
            else if (yaw <= (128 - quadrant))
                dirX = 1;
            else if (yaw <= (128 + quadrant))
                dirZ = 1;
            else 
                dirX = -1;
        }

        public static void Pitch(byte pitch, out int dirY) {
            dirY = 0;
            const byte quadrant = 32;
            
            if (pitch >= 192 && pitch <= (192 + quadrant))
                dirY = 1;
            else if (pitch >= (64 - quadrant) && pitch <= 64)
                dirY = -1;
        }
        
        static Vec3F32 GetDirVector(double yaw, double pitch) {
            double x =  Math.Sin(yaw) * Math.Cos(pitch);
            double y = -Math.Sin(pitch);
            double z = -Math.Cos(yaw) * Math.Cos(pitch);
            return new Vec3F32((float)x, (float)y, (float)z);
        }
        
        public static Vec3F32 GetDirVectorExt(ushort yaw, ushort pitch) {
            const double packed2Rad = (2 * Math.PI) / 65536.0;
            return GetDirVector(yaw * packed2Rad, pitch * packed2Rad);
        }
        
        public static Vec3F32 GetDirVector(byte yaw, byte pitch) {
            const double packed2Rad = (2 * Math.PI) / 256.0;
            return GetDirVector(yaw * packed2Rad, pitch * packed2Rad);
        }
        
        public static void GetYawPitch(Vec3F32 dir, out byte yaw, out byte pitch) {
            // y = -sin(pitch) -> pitch = arcsin(-y)
            // x = sin(yaw)    -> yaw = arcsin(x)
            // z = -cos(yaw)   -> yaw = arccos(-z)
            
            // We ignore the cos(pitch) multiplication by the x/z components, since
            // this does not affect the resulting yaw
            const double rad2Packed = 256.0 / (2 * Math.PI);
            
            // NOTE: This conversion method **does** lose information
            // a) If x and z are 0, yaw cannot be properly recalculated
            // b) Pitch will always be from 0-64 or 192-256, therefore flipped heads lost
            //    However since we have X/Z, this problem does not occur for yaw,
            //    as we can use both values to determine which side of unit circle yaw is in
            // c) Resulting yaw/pitch may be 1 or 2 values off due to rounding            
            yaw = (byte)(Math.Atan2(dir.X, -dir.Z) * rad2Packed);
            pitch = (byte)(Math.Asin(-dir.Y) * rad2Packed);
        }
    }
}
