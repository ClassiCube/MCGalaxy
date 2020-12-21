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
using System.IO;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing {
    /// <summary> Utility methods for rotating and mirroring a CopyState. </summary>
    public static class Flip {

        static string[] rotX_90_270 = new string[] { "NS", "UD" };
        static string[] rotX_180 = new string[] { "N", "S",  "NE", "SE",  "NW", "SW" };
        public static CopyState RotateX(CopyState state, int angle, BlockDefinition[] defs) {
            CopyState newState = Clone(state);
            newState.Height = angle == 180 ? state.Height : state.Length;
            newState.Length = angle == 180 ? state.Length : state.Height;
            
            BlockID[] transform;
            if (angle == 90 || angle == 270) transform = Transform(defs, rotX_90_270, null);
            else if (angle == 180)           transform = Transform(defs, rotX_180,    null);
            else                             transform = Transform(defs, null,        null);
            
            int[] m = new int[] { posX, negZ, posY };
            if (angle == 180) { m[1] = negY; m[2] = negZ; }
            if (angle == 270) { m[1] = posZ; m[2] = negY; }
            return Rotate(state, newState, m, transform);
        }

        static string[] rotY_90 = new string[]  { "N", "E", "S", "W",  "NE", "SE", "SW", "NW",  "WE", "NS", "WE", "NS" };
        static string[] rotY_180 = new string[] { "W", "E", "N", "S",  "NE", "SW", "NW", "SE"                          };
        static string[] rotY_270 = new string[] { "N", "W", "S", "E",  "NE", "NW", "SW", "SE",  "WE", "NS", "WE", "NS" };
        public static CopyState RotateY(CopyState state, int angle, BlockDefinition[] defs) {
            CopyState newState = Clone(state);
            newState.Width = angle == 180 ? state.Width : state.Length;
            newState.Length = angle == 180 ? state.Length : state.Width;
            
            BlockID[] transform;
            if (angle == 90)       transform = Transform(defs, null,     rotY_90);
            else if (angle == 180) transform = Transform(defs, rotY_180, null);
            else if (angle == 270) transform = Transform(defs, null,     rotY_270);
            else                   transform = Transform(defs, null,     null);

            int[] m = new int[] { negZ, posY, posX };
            if (angle == 180) { m[0] = negX; m[2] = negZ; }
            if (angle == 270) { m[0] = posZ; m[2] = negX; }
            return Rotate(state, newState, m, transform);
        }
        
        static string[] rotZ_90_270 = new string[] { "WE", "UD" };
        static string[] rotZ_180 = new string[] { "W", "E",  "NW", "NE",  "SW", "SE" };
        public static CopyState RotateZ(CopyState state, int angle, BlockDefinition[] defs) {
            CopyState newState = Clone(state);
            newState.Width = angle == 180 ? state.Width : state.Height;
            newState.Height = angle == 180 ? state.Height : state.Width;
            
            BlockID[] transform;
            if (angle == 90 || angle == 270) transform = Transform(defs, rotZ_90_270, null);
            else if (angle == 180) transform = Transform(defs, rotZ_180,null);
            else transform = Transform(defs, null, null);
            
            int[] m = new int[] { posY, negX, posZ };
            if (angle == 180) { m[0] = negX; m[1] = negY; }
            if (angle == 270) { m[0] = negY; m[1] = posX; }
            return Rotate(state, newState, m, transform);
        }

        static CopyState Rotate(CopyState state, CopyState flipped, int[] m, BlockID[] transform) {
            int volume = state.Volume;
            for (int i = 0; i < volume; i++) {
                ushort x, y, z;
                state.GetCoords(i, out x, out y, out z);
                BlockID block = transform[state.Get(i)];
                
                flipped.Set(block,
                            Rotate(m[0], x, y, z, state),
                            Rotate(m[1], x, y, z, state),
                            Rotate(m[2], x, y, z, state));
            }
            
            int oX = state.OriginX - state.X, oY = state.OriginY - state.Y, oZ = state.OriginZ - state.Z;
            flipped.OriginX = state.X + Rotate(m[0], oX, oY, oZ, state);
            flipped.OriginY = state.Y + Rotate(m[1], oX, oY, oZ, state);
            flipped.OriginZ = state.Z + Rotate(m[2], oX, oY, oZ, state);
            
            // Offset is relative to Origin
            oX += state.Offset.X; oY += state.Offset.Y; oZ += state.Offset.Z;
            flipped.Offset.X = state.X + Rotate(m[0], oX, oY, oZ, state) - flipped.OriginX;
            flipped.Offset.Y = state.Y + Rotate(m[1], oX, oY, oZ, state) - flipped.OriginY;
            flipped.Offset.Z = state.Z + Rotate(m[2], oX, oY, oZ, state) - flipped.OriginZ;
            return flipped;
        }
        
        const int posX = 0, negX = 1, posY = 2, negY = 3, posZ = 4, negZ = 5;
        static int Rotate(int row, int x, int y, int z, CopyState state) {
            switch (row) {
                    case posX: return x;
                    case negX: return (state.Width - 1 - x);
                    case posY: return y;
                    case negY: return (state.Height - 1 - y);
                    case posZ: return z;
                    case negZ: return (state.Length - 1 - z);
            }
            return 0;
        }
        
        static CopyState Clone(CopyState state) {
            CopyState newState = new CopyState(state.X, state.Y, state.Z,
                                               state.Width, state.Height, state.Length);
            newState.UsedBlocks = state.UsedBlocks;
            newState.PasteAir = state.PasteAir;
            return newState;
        }
        
        static string[] mirrorX = new string[] { "W", "E",  "NW", "NE",  "SW", "SE" };
        public static void MirrorX(CopyState state, BlockDefinition[] defs) {
            // ceiling division by 2, because for odd length, we still want to
            // mirror the middle row to rotate directional blocks
            int midX = (state.Width + 1) / 2, maxX = state.Width - 1;
            state.OriginX  = state.OppositeOriginX;
            state.Offset.X = -state.Offset.X;
            BlockID[] transform = Transform(defs, mirrorX, null);
            
            for (int y = 0; y < state.Height; y++) {
                for (int z = 0; z < state.Length; z++) {
                    for (int x = 0; x < midX; x++) {
                        int endX = maxX - x;
                        int beg  = state.GetIndex(x, y, z);
                        int end  = state.GetIndex(endX, y, z);
                        
                        BlockID blockA = transform[state.Get(beg)];
                        BlockID blockB = transform[state.Get(end)];
                        state.Set(blockB, beg); state.Set(blockA, end);
                    }
                }
            }
        }
        
        static string[] mirrorY = new string[] { "D", "U" };
        public static void MirrorY(CopyState state, BlockDefinition[] defs) {
            int midY = (state.Height + 1) / 2, maxY = state.Height - 1;
            state.OriginY = state.OppositeOriginY;
            state.Offset.Y = -state.Offset.Y;
            BlockID[] transform = Transform(defs, mirrorY, null);
            
            for (int y = 0; y < midY; y++) {
                int endY = maxY - y;
                int beg  = state.GetIndex(0, y, 0);
                int end  = state.GetIndex(0, endY, 0);
                
                for (int z = 0; z < state.Length; z++) {
                    for (int x = 0; x < state.Width; x++) {
                        BlockID blockA = transform[state.Get(beg)];
                        BlockID blockB = transform[state.Get(end)];
                        state.Set(blockB, beg); state.Set(blockA, end);
                        beg++; end++;
                    }
                }
            }
        }
        
        static string[] mirrorZ = new string[] { "N", "S",  "NW", "SW",  "NE", "SE" };
        public static void MirrorZ(CopyState state, BlockDefinition[] defs) {
            int midZ = (state.Length + 1) / 2, maxZ = state.Length - 1;
            state.OriginZ  = state.OppositeOriginZ;
            state.Offset.Z = -state.Offset.Z;
            BlockID[] transform = Transform(defs, mirrorZ, null);
            
            for (int y = 0; y < state.Height; y++) {
                for (int z = 0; z < midZ; z++) {
                    int endZ = maxZ - z;
                    int beg  = state.GetIndex(0, y, z);
                    int end  = state.GetIndex(0, y, endZ);
                    
                    for (int x = 0; x < state.Width; x++) {
                        BlockID blockA = transform[state.Get(beg)];
                        BlockID blockB = transform[state.Get(end)];
                        state.Set(blockB, beg); state.Set(blockA, end);
                        beg++; end++;
                    }
                }
            }
        }
 
        
        static BlockID[] Transform(BlockDefinition[] defs, string[] mirrorDirs, string[] rotateDirs) {
            BlockID[] transform = new BlockID[Block.ExtendedCount];
            for (int i = 0; i < transform.Length; i++) {
                transform[i] = (BlockID)i;
            }
            if (mirrorDirs == null && rotateDirs == null) return transform;
            
            // Rotate/Mirror directional blocks
            for (int i = 0; i < defs.Length; i++) {
                if (defs[i] == null) continue;
                int dirIndex = defs[i].Name.LastIndexOf('-');
                if (dirIndex == -1) continue;
                
                BlockDefinition transformed = null;
                if (mirrorDirs != null) {
                    transformed = MirrorTransform(defs, i, dirIndex, mirrorDirs);
                } else {
                    transformed = RotateTransform(defs, i, dirIndex, rotateDirs);
                }
                
                if (transformed == null) continue;
                BlockID src = defs[i].GetBlock();
                transform[src] = transformed.GetBlock();
            }
            return transform;
        }
        
        static BlockDefinition MirrorTransform(BlockDefinition[] defs, int i, int dirIndex, string[] mirrorDirs) {
            string dir = defs[i].Name.Substring(dirIndex + 1);
            for (int j = 0; j < mirrorDirs.Length; j++) {
                if (!mirrorDirs[j].CaselessEq(dir)) continue;
                
                // Find the mirrored directional block opposite to this one
                string name = defs[i].Name.Substring(0, dirIndex);
                int mirrorIdx = (j & 1) == 0 ? 1 : -1;
                string mirrorDir = mirrorDirs[j + mirrorIdx];
                return Find(defs, name + "-" + mirrorDir);
            }
            return null;
        }
        
        static BlockDefinition RotateTransform(BlockDefinition[] defs, int i, int dirIndex, string[] rotateDirs) {
            string dir = defs[i].Name.Substring(dirIndex + 1);
            for (int j = 0; j < rotateDirs.Length; j++) {
                if (!rotateDirs[j].CaselessEq(dir)) continue;
                
                // Find the next directional block to this one in sequence
                // Each sequence is a group of 4 directional blocks
                string name = defs[i].Name.Substring(0, dirIndex);
                int sequence = (j / 4) * 4;
                string rotateDir = rotateDirs[sequence + ((j + 1) % 4)];
                return Find(defs, name + "-" + rotateDir);
            }
            return null;
        }
        
        static BlockDefinition Find(BlockDefinition[] defs, string name) {
            for (int i = 0; i < defs.Length; i++) {
                if (defs[i] != null && defs[i].Name.CaselessEq(name)) return defs[i];
            }
            return null;
        }
    }
}
