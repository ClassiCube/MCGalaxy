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
        
        public static CopyState RotateX(CopyState state) {
            CopyState newState = new CopyState(state.X, state.Y, state.Z,
                                               state.Width, state.Length, state.Height);
            int[] m = { 0x100, 0x002, 0x010 };
            return Rotate(state, newState, m);
        }
        
        public static CopyState RotateY(CopyState state) {
            CopyState newState = new CopyState(state.X, state.Y, state.Z,
                                               state.Length, state.Height, state.Width);
            int[] m = { 0x002, 0x010, 0x100 };
            return Rotate(state, newState, m);
        }
        
        public static CopyState RotateZ(CopyState state) {
            CopyState newState = new CopyState(state.X, state.Y, state.Z,
                                               state.Height, state.Width, state.Length);
            int[] m = { 0x010, 0x200, 0x001 };
            return Rotate(state, newState, m);
        }
        
        static CopyState Rotate(CopyState state, CopyState newState, int[] m) {
            byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
            for (int i = 0; i < blocks.Length; i++) {
                ushort x, y, z;
                state.GetCoords(i, out x, out y, out z);
                newState.Set(blocks[i], extBlocks[i],
                             Rotate(m[0], x, y, z, state),
                             Rotate(m[1], x, y, z, state),
                             Rotate(m[2], x, y, z, state));
            }
            
            int oX = state.OriginX - state.X, oY = state.OriginY - state.Y, oZ = state.OriginZ - state.Z;
            newState.SetOrigin(
                state.X + Rotate(m[0], oX, oY, oZ, state),
                state.Y + Rotate(m[1], oX, oY, oZ, state),
                state.Z + Rotate(m[2], oX, oY, oZ, state));
            return newState;
        }
        
        static int Rotate(int row, int x, int y, int z, CopyState state) {
            switch (row) {
                case 0x100: return x;
                case 0x200: return (state.Width - 1 - x);
                case 0x010: return y;
                case 0x020: return (state.Height - 1 - y);
                case 0x001: return z;
                case 0x002: return (state.Length - 1 - z);
            }
            return 0;
        }
        
        public static void MirrorX(CopyState state) {
            int midZ = state.Length / 2, maxZ = state.Length - 1;
            byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
            state.OriginZ = state.OppositeOriginZ;
            
            for (int y = 0; y < state.Height; y++) {
                for (int z = 0; z < midZ; z++) {
                    int endZ = maxZ - z;
                    int start = state.GetIndex(0, y, z);
                    int end = state.GetIndex(0, y, endZ);
                    for (int x = 0; x < state.Width; x++) {
                        Swap(blocks, extBlocks, start, end);
                        start++; end++;
                    }
                }
            }
        }
        
        public static void MirrorY(CopyState state) {
            int midY = state.Height / 2, maxY = state.Height - 1;
            byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
            state.OriginY = state.OppositeOriginY;
            
            for (int y = 0; y < midY; y++) {
                int endY = maxY - y;
                int start = state.GetIndex(0, y, 0);
                int end = state.GetIndex(0, endY, 0);
                for (int z = 0; z < state.Length; z++) {
                    for (int x = 0; x < state.Width; x++) {
                        Swap(blocks, extBlocks, start, end);
                        start++; end++;
                    }
                }
            }
        }
        
        public static void MirrorZ(CopyState state) {
            int midX = state.Width / 2, maxX = state.Width - 1;
            byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
            state.OriginX = state.OppositeOriginX;
            
            for (int y = 0; y < state.Height; y++) {
                for (int z = 0; z < state.Length; z++) {
                    for (int x = 0; x < midX; x++) {
                        int endX = maxX - x;
                        int start = state.GetIndex(x, y, z);
                        int end = state.GetIndex(endX, y, z);
                        Swap(blocks, extBlocks, start, end);
                    }
                }
            }
        }

        static void Swap(byte[] blocks, byte[] extBlocks, int a, int b) {
            byte tmp = blocks[a]; blocks[a] = blocks[b]; blocks[b] = tmp;
            tmp = extBlocks[a]; extBlocks[a] = extBlocks[b]; extBlocks[b] = tmp;
        }
    }
}
