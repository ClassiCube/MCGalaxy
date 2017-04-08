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
using MCGalaxy.Blocks;

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
        
        public AABB OffsetPosition(ushort[] pos) {
            return Offset(pos[0], pos[1] - Entities.CharacterHeight, pos[2]);
        }
        
        public AABB Offset(int x, int y, int z) {
            AABB bb = this;
            bb.Min.X += x; bb.Min.Y += y; bb.Min.Z += z;
            bb.Max.X += x; bb.Max.Y += y; bb.Max.Z += z;
            return bb;
        }
        
        public AABB Expand(int amount) {
            AABB bb = this;
            bb.Min.X -= amount; bb.Min.Y -= amount; bb.Min.Z -= amount;
            bb.Max.X += amount; bb.Max.Y += amount; bb.Max.Z += amount;
            return bb;
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
        
        
        public static AABB ModelAABB(string model, Level lvl) {
            int sep = model.IndexOf('|');
            string scaleStr = sep == -1 ? null : model.Substring(sep + 1);
            model = sep == -1 ? model : model.Substring(0, sep);
            
            AABB baseBB;
            byte raw;
            if (byte.TryParse(model, out raw)) {
                byte block, extBlock;
                Block.Unpack(raw, out block, out extBlock);
                
                baseBB = Block.BlockAABB(block, extBlock, lvl);
                baseBB = baseBB.Offset(-16, 0, -16); // centre around [-16, 16] instead of [0, 32]
            } else {
                baseBB = AABB.Make(new Vec3S32(0, 0, 0), BaseSize(model));
            }
            baseBB = baseBB.Expand(-1); // adjust the model AABB inwards slightly
            
            float scale;
            if (!Utils.TryParseDecimal(scaleStr, out scale)) return baseBB;
            if (scale < 0.25f) scale = 0.25f;
            float maxScale = model.CaselessEq("chibi") ? 3 : 2;
            if (scale > maxScale) scale = maxScale;
            
            return baseBB.Scale(scale);
        }
        
        static Vec3S32 BaseSize(string model) {
            if (model.CaselessEq("chicken"))  return new Vec3S32(16, 24, 16);
            if (model.CaselessEq("creeper"))  return new Vec3S32(16, 52, 16);
            if (model.CaselessEq("chibi"))    return new Vec3S32(8,  40,  8);
            if (model.CaselessEq("head"))     return new Vec3S32(31, 31, 31);
            if (model.CaselessEq("pig"))      return new Vec3S32(28, 28, 28);
            if (model.CaselessEq("sheep"))    return new Vec3S32(20, 40, 20);
            if (model.CaselessEq("skeleton")) return new Vec3S32(16, 60, 16);
            if (model.CaselessEq("spider"))   return new Vec3S32(30, 24, 30);
            
            return new Vec3S32(16, 56, 16); // default humanoid size
        }
        
        public static bool IntersectsSolidBlocks(AABB bb, Level lvl) {
            Vec3S32 min = bb.BlockMin, max = bb.BlockMax;
            
            for (int y = min.Y; y <= max.Y; y++)
                for (int z = min.Z; z <= max.Z; z++)
                    for (int x = min.X; x <= max.X; x++)
            {
                ushort xP = (ushort)x, yP = (ushort)y, zP = (ushort)z;
                byte block = lvl.GetTile(xP, yP, zP), extBlock = 0;
                if (block == Block.custom_block)
                    extBlock = lvl.GetExtTileNoCheck(xP, yP, zP);
                
                AABB blockBB = Block.BlockAABB(block, extBlock, lvl)
                    .Offset(x * 32, y * 32, z * 32);
                if (!bb.Intersects(blockBB)) continue;
                
                BlockDefinition def = lvl.GetBlockDef(block, extBlock);
                if (def != null) {
                    if (def.CollideType == CollideType.Solid) return true;
                } else {
                    if (block == Block.Invalid || !Block.Walkthrough(Block.Convert(block))) return true;
                }
            }
            return false;
        }
    }
}