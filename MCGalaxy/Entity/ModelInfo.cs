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
using System.Collections.Generic;
using MCGalaxy.Blocks;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy {
    
    public struct ModelInfo {
        
        public readonly string Model;
        public readonly Vec3S32 BaseSize;
        public readonly int EyeHeight;
        
        public ModelInfo(string model, int sizeX, int sizeY, int sizeZ, int eyeHeight) {
            Model      = model;
            BaseSize.X = sizeX;
            BaseSize.Y = sizeY;
            BaseSize.Z = sizeZ;
            EyeHeight  = eyeHeight;
        }
        
        // Eye height may be 1-2 pixels different from client's eye height,
        // because we're really after the centre of the entity's head
        public static List<ModelInfo> Models = new List<ModelInfo>() {
            new ModelInfo("humanoid",    18,56,18, 28),
            new ModelInfo("sit",         18,56,18, 18),
            new ModelInfo("chicken",     16,24,16, 12),
            new ModelInfo("creeper",     16,52,16, 22),
            new ModelInfo("chibi",        8,40,8,  14),
            new ModelInfo("head",        31,31,31,  4),
            new ModelInfo("pig",         28,28,28, 12),
            new ModelInfo("sheep",       20,40,20, 19),
            new ModelInfo("sheep_nofur", 20,40,20, 19),
            new ModelInfo("skeleton",    16,56,16, 19),
            new ModelInfo("spider",      30,24,30,  8),
        };
            
        public static ModelInfo Get(string model) {
            foreach (ModelInfo m in Models) {
                if (m.Model.CaselessEq(model)) return m;
            }
            return Models[0];
        }
        
        public static string GetRawModel(string model) {
            int sep = model.IndexOf('|');
            return sep == -1 ? model : model.Substring(0, sep);
        }
        
        public static float GetRawScale(string model) {
            int sep = model.IndexOf('|');
            string str = sep == -1 ? null : model.Substring(sep + 1);
            
            float scale;
            if (!Utils.TryParseSingle(str, out scale)) scale = 1.0f;
            if (scale < 0.01f) scale = 0.01f;
            
            // backwards compatibility
            if (model.CaselessEq("giant")) scale *= 2;
            return scale;
        }
        
        public static float DefaultMaxScale(string model) {
            return model.CaselessEq("chibi") ? 3 : 2;
        }
        
        public static float MaxScale(Entity entity) {
            if (!entity.RestrictsScale) return float.MaxValue;
            return DefaultMaxScale(GetRawModel(entity.Model));
        }
        
        public static AABB CalcAABB(Entity entity) {
            string model = GetRawModel(entity.Model);
            float scale  = GetRawScale(entity.Model);
            
            AABB bb;
            BlockID raw;
            if (BlockID.TryParse(model, out raw) && raw <= Block.MaxRaw) {
                BlockID block = Block.FromRaw(raw);
                bb = Block.BlockAABB(block, entity.Level);
                bb = bb.Offset(-16, 0, -16); // centre around [-16, 16] instead of [0, 32]
            } else {
                bb = AABB.Make(new Vec3S32(0, 0, 0), Get(model).BaseSize);
            }
            bb = bb.Expand(-1); // adjust the model AABB inwards slightly
            
            float scaleX = scale, scaleY = scale, scaleZ = scale;
            if (entity.ScaleX != 0) scaleX *= entity.ScaleX;
            if (entity.ScaleY != 0) scaleY *= entity.ScaleY;
            if (entity.ScaleZ != 0) scaleZ *= entity.ScaleZ;
            
            // always limit max scale for collisions performance 
            float max = DefaultMaxScale(model);
            scaleX = Math.Min(scaleX, max);
            scaleY = Math.Min(scaleY, max);
            scaleZ = Math.Min(scaleZ, max);
            
            bb.Min.X = (int)(bb.Min.X * scaleX); bb.Max.X = (int)(bb.Max.X * scaleX);
            bb.Min.Y = (int)(bb.Min.Y * scaleY); bb.Max.Y = (int)(bb.Max.Y * scaleY);
            bb.Min.Z = (int)(bb.Min.Z * scaleZ); bb.Max.Z = (int)(bb.Max.Z * scaleZ);
            
            return bb;
        }
        
        /// <summary> Gives distance (in half-pixel world units) from feet to camera height </summary>
        public static int CalcEyeHeight(string model) {
            float scale = GetRawScale(model);
            model = GetRawModel(model);
            BlockID raw;
            if (BlockID.TryParse(model, out raw) && raw <= Block.MaxRaw) return 16; //lazily return middle of full block if it thinks it's a block ID.
            
            float eyeHeight = Get(model).EyeHeight;
            eyeHeight *= scale;
            eyeHeight *= 2f; //multiply by two because world positions are measured in half-pixels
            return (int)eyeHeight;
        }
    }
}