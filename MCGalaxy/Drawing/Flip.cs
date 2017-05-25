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

namespace MCGalaxy.Drawing {
    public static class Flip {
        
        public static CopyState RotateX(CopyState state, int angle) {
            CopyState newState = Clone(state);
            newState.Height = angle == 180 ? state.Height : state.Length;
            newState.Length = angle == 180 ? state.Length : state.Height;
            
            int[] m = new int[] { posX, negZ, posY };
            if (angle == 180) { m[1] = negY; m[2] = negZ; }
            if (angle == 270) { m[1] = posZ; m[2] = negY; }
            return Rotate(state, newState, m);
        }
        
        public static CopyState RotateY(CopyState state, int angle) {
            CopyState newState = Clone(state);
            newState.Width = angle == 180 ? state.Width : state.Length;
            newState.Length = angle == 180 ? state.Length : state.Width;

            int[] m = new int[] { negZ, posY, posX };
            if (angle == 180) { m[0] = negX; m[2] = negZ; }
            if (angle == 270) { m[0] = posZ; m[2] = negX; }
            return Rotate(state, newState, m);
        }
        
        public static CopyState RotateZ(CopyState state, int angle) {
            CopyState newState = Clone(state);
            newState.Width = angle == 180 ? state.Width : state.Height;
            newState.Height = angle == 180 ? state.Height : state.Width;
            
            int[] m = new int[] { posY, negX, posZ };
            if (angle == 180) { m[0] = negX; m[1] = negY; }
            if (angle == 270) { m[0] = negY; m[1] = posX; }
            return Rotate(state, newState, m);
        }

        static CopyState Rotate(CopyState state, CopyState flipped, int[] m) {
		    int volume = state.Volume;
            for (int i = 0; i < volume; i++) {
                ushort x, y, z;
                state.GetCoords(i, out x, out y, out z);
                ExtBlock block = state.Get(i);
                
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
        
        const int posX = 0x100, negX = 0x200, posY = 0x010, negY = 0x020, posZ = 0x001, negZ = 0x002;
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
        
        
        public static void MirrorX(CopyState state) {
            int midZ = state.Length / 2, maxZ = state.Length - 1;
            state.OriginZ = state.OppositeOriginZ;
            state.Offset.Z = -state.Offset.Z;
            
            for (int y = 0; y < state.Height; y++) {
                for (int z = 0; z < midZ; z++) {
                    int endZ = maxZ - z;
                    int start = state.GetIndex(0, y, z);
                    int end = state.GetIndex(0, y, endZ);
                    for (int x = 0; x < state.Width; x++) {
                        ExtBlock blockA = state.Get(start), blockB = state.Get(end);
                        state.Set(blockB, start); state.Set(blockA, end);
                        start++; end++;
                    }
                }
            }
        }
        
        public static void MirrorY(CopyState state) {
            int midY = state.Height / 2, maxY = state.Height - 1;
            state.OriginY = state.OppositeOriginY;
            state.Offset.Y = -state.Offset.Y;
            
            for (int y = 0; y < midY; y++) {
                int endY = maxY - y;
                int start = state.GetIndex(0, y, 0);
                int end = state.GetIndex(0, endY, 0);
                for (int z = 0; z < state.Length; z++) {
                    for (int x = 0; x < state.Width; x++) {
                        ExtBlock blockA = state.Get(start), blockB = state.Get(end);
                        state.Set(blockB, start); state.Set(blockA, end);
                        start++; end++;
                    }
                }
            }
        }
        
        public static void MirrorZ(CopyState state) {
            int midX = state.Width / 2, maxX = state.Width - 1;
            state.OriginX = state.OppositeOriginX;
            state.Offset.X = -state.Offset.X;
            
            for (int y = 0; y < state.Height; y++) {
                for (int z = 0; z < state.Length; z++) {
                    for (int x = 0; x < midX; x++) {
                        int endX = maxX - x;
                        int start = state.GetIndex(x, y, z);
                        int end = state.GetIndex(endX, y, z);
                        
                        ExtBlock blockA = state.Get(start), blockB = state.Get(end);
                        state.Set(blockB, start); state.Set(blockA, end);
                    }
                }
            }
        }
    }
}
