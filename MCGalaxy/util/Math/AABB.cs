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
    
    public struct AABB {
        
        /// <summary> Fixed-point min coordinate of this bounding box. </summary>
        public Vec3S32 Min;
        
        /// <summary> Fixed-point max coordinate of this bounding box. </summary>
        public Vec3S32 Max;

        /// <summary> World/block coordinate of the min coordinate of this bounding box. </summary>        
        public Vec3S32 BlockMin { get { return new Vec3S32(Min.X >> 5, Min.Y >> 5, Min.Z >> 5); } }

        /// <summary> World/block coordinate of the max coordinate of this bounding box. </summary>            
        public Vec3S32 BlockMax { get { return new Vec3S32(Max.X >> 5, Max.Y >> 5, Max.Z >> 5); } }        
        
        
        public AABB(int x1, int y1, int z1, int x2, int y2, int z2) {
            Min.X = x1; Min.Y = y1; Min.Z = z1;
            Max.X = x2; Max.Y = y2; Max.Z = z2;
        }

        public AABB(Vec3S32 min, Vec3S32 max) {
            Min = min;
            Max = max;
        }
        
        public static AABB Make(Vec3S32 pos, Vec3S32 size) {
            return new AABB(pos.X - size.X / 2, pos.Y, pos.Z - size.Z / 2,
                            pos.X + size.X / 2, pos.Y + size.Y, pos.Z + size.Z / 2);
        }
        
        /// <summary> Returns a new bounding box, with the minimum and maximum coordinates
        /// of the original bounding box scaled away from origin the given value. </summary>
        public AABB Scale(float scale) {
            return new AABB(Min * scale, Max * scale);
        }

        /// <summary> Determines whether this bounding box intersects
        /// the given bounding box on any axes. </summary>
        public bool Intersects(AABB other) {
            if (Max.X >= other.Min.X && Min.X <= other.Max.X) {
                if (Max.Y < other.Min.Y || Min.Y > other.Max.Y) {
                    return false;
                }
                return Max.Z >= other.Min.Z && Min.Z <= other.Max.Z;
            }
            return false;
        }
        
        /// <summary> Determines whether this bounding box entirely contains
        /// the given bounding box on all axes. </summary>
        public bool Contains(AABB other) {
            return other.Min.X >= Min.X && other.Min.Y >= Min.Y && other.Min.Z >= Min.Z &&
                other.Max.X <= Max.X && other.Max.Y <= Max.Y && other.Max.Z <= Max.Z;
        }
        
        /// <summary> Determines whether this bounding box entirely contains
        /// the coordinates on all axes. </summary>
        public bool Contains(Vec3S32 P) {
            return P.X >= Min.X && P.Y >= Min.Y && P.Z >= Min.Z &&
                P.X <= Max.X && P.Y <= Max.Y && P.Z <= Max.Z;
        }
        
        public override string ToString() {
            return Min + " : " + Max;
        }
    }
}