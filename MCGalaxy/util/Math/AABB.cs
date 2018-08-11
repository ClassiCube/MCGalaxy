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
using BlockID = System.UInt16;

namespace MCGalaxy.Maths {
    
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
        
        public AABB OffsetPosition(Position pos) {
            return Offset(pos.X, pos.Y - Entities.CharacterHeight, pos.Z);
        }
        
        public AABB Offset(int x, int y, int z) {
            AABB bb = this;
            bb.Min.X += x; bb.Min.Y += y; bb.Min.Z += z;
            bb.Max.X += x; bb.Max.Y += y; bb.Max.Z += z;
            return bb;
        }
        
        public AABB Adjust(int x, int y, int z) {
            AABB bb = this;
            if (x >= 0) { bb.Max.X += x; } else { bb.Min.X += x; }
            if (y >= 0) { bb.Max.Y += y; } else { bb.Min.Y += y; }
            if (z >= 0) { bb.Max.Z += z; } else { bb.Min.Z += z; }
            return bb;
        }
        
        public AABB Expand(int amount) {
            AABB bb = this;
            bb.Min.X -= amount; bb.Min.Y -= amount; bb.Min.Z -= amount;
            bb.Max.X += amount; bb.Max.Y += amount; bb.Max.Z += amount;
            return bb;
        }
        
        /// <summary> Determines whether this bounding box intersects
        /// the given bounding box on any axes. </summary>
        public static bool Intersects(ref AABB a, ref AABB b) {
            if (a.Max.X >= b.Min.X && a.Min.X <= b.Max.X) {
                if (a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y) {
                    return false;
                }
                return a.Max.Z >= b.Min.Z && a.Min.Z <= b.Max.Z;
            }
            return false;
        }

        public override string ToString() { return Min + " : " + Max; }
        
        
        public static AABB ModelAABB(Entity entity, Level lvl) {
            string model = entity.Model;
            float scale = GetScaleFrom(ref model);
            
            AABB bb;
            BlockID raw;
            if (BlockID.TryParse(model, out raw) && raw <= Block.MaxRaw) {
                BlockID block = Block.FromRaw(raw);
                bb = Block.BlockAABB(block, lvl);
                bb = bb.Offset(-16, 0, -16); // centre around [-16, 16] instead of [0, 32]
            } else {
                bb = AABB.Make(new Vec3S32(0, 0, 0), BaseSize(model));
            }
            bb = bb.Expand(-1); // adjust the model AABB inwards slightly

            float max = model.CaselessEq("chibi") ? 3 : 2;
            float scaleX = scale, scaleY = scale, scaleZ = scale;
            if (entity.ScaleX != 0) scaleX = Math.Min(entity.ScaleX * scale, max);
            if (entity.ScaleY != 0) scaleY = Math.Min(entity.ScaleY * scale, max);
            if (entity.ScaleZ != 0) scaleZ = Math.Min(entity.ScaleZ * scale, max);
            
            bb.Min.X = (int)(bb.Min.X * scaleX); bb.Max.X = (int)(bb.Max.X * scaleX);
            bb.Min.Y = (int)(bb.Min.Y * scaleY); bb.Max.Y = (int)(bb.Max.Y * scaleY);
            bb.Min.Z = (int)(bb.Min.Z * scaleZ); bb.Max.Z = (int)(bb.Max.Z * scaleZ);
            
            return bb;
        }
        
        internal static float GetScaleFrom(ref string model) {
            int sep = model.IndexOf('|');
            string scaleStr = sep == -1 ? null : model.Substring(sep + 1);
            model = sep == -1 ? model : model.Substring(0, sep);
            
            float scale;
            if (!Utils.TryParseSingle(scaleStr, out scale)) scale = 1.0f;
            if (scale < 0.01f) scale = 0.01f;
            
            float max = model.CaselessEq("chibi") ? 3 : 2;
            return Math.Min(scale, max);
        }
        
        static Vec3S32 BaseSize(string model) {
            if (model.CaselessEq("chicken"))  return new Vec3S32(16, 24, 16);
            if (model.CaselessEq("creeper"))  return new Vec3S32(16, 52, 16);
            if (model.CaselessEq("chibi"))    return new Vec3S32(8,  40,  8);
            if (model.CaselessEq("head"))     return new Vec3S32(31, 31, 31);
            if (model.CaselessEq("pig"))      return new Vec3S32(28, 28, 28);
            if (model.CaselessEq("sheep"))    return new Vec3S32(20, 40, 20);
            if (model.CaselessEq("skeleton")) return new Vec3S32(16, 56, 16);
            if (model.CaselessEq("spider"))   return new Vec3S32(30, 24, 30);
            
            return new Vec3S32(18, 56, 18); // default humanoid size
        }
        
        public static bool IntersectsSolidBlocks(AABB bb, Level lvl) {
            Vec3S32 min = bb.BlockMin, max = bb.BlockMax;

            for (int y = min.Y; y <= max.Y; y++)
                for (int z = min.Z; z <= max.Z; z++)
                    for (int x = min.X; x <= max.X; x++)
            {
                BlockID block = lvl.GetBlock((ushort)x, (ushort)y, (ushort)z);
                
                AABB blockBB = lvl.blockAABBs[block].Offset(x * 32, y * 32, z * 32);
                if (!AABB.Intersects(ref bb, ref blockBB)) continue;
                
                BlockDefinition def = lvl.GetBlockDef(block);
                if (def != null) {
                    if (CollideType.IsSolid(def.CollideType)) return true;
                } else if (block == Block.Invalid) {
                    if (y < lvl.Height) return true;
                } else if (!Block.Walkthrough(Block.Convert(block))) {
                    return true;
                }
            }
            return false;
        }
        
        public static int FindIntersectingSolids(AABB bb, Level lvl, ref AABB[] aabbs) {
            Vec3S32 min = bb.BlockMin, max = bb.BlockMax;
            int volume = (max.X - min.X + 1) * (max.Y - min.Y + 1) * (max.Z - min.Z + 1);
            if (volume > aabbs.Length) aabbs = new AABB[volume];
            int count = 0;
            
            for (int y = min.Y; y <= max.Y; y++)
                for (int z = min.Z; z <= max.Z; z++)
                    for (int x = min.X; x <= max.X; x++)
            {
                BlockID block = lvl.GetBlock((ushort)x, (ushort)y, (ushort)z);
                AABB blockBB = lvl.blockAABBs[block];
                
                blockBB.Min.X += x * 32; blockBB.Min.Y += y * 32; blockBB.Min.Z += z * 32;
                blockBB.Max.X += x * 32; blockBB.Max.Y += y * 32; blockBB.Max.Z += z * 32;
                if (!AABB.Intersects(ref bb, ref blockBB)) continue;
                
                BlockDefinition def = lvl.GetBlockDef(block);
                bool solid = false;
                
                if (def != null) {
                    solid = CollideType.IsSolid(def.CollideType);
                } else {
                    solid = block == Block.Invalid || !Block.Walkthrough(Block.Convert(block));
                }
                if (!solid) continue;
                
                aabbs[count] = blockBB;
                count++;
            }
            return count;
        }
    }
}