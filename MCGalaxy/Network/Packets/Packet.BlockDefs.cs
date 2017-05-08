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

namespace MCGalaxy.Network {

    public static partial class Packet {
        
        public static byte[] DefineBlock(BlockDefinition def, bool hasCP437) {
            byte[] buffer = new byte[80];
            int i = 0;
            buffer[i++] = Opcode.CpeDefineBlock;
            MakeDefineBlockStart(def, buffer, ref i, false, hasCP437);
            buffer[i++] = def.Shape;
            MakeDefineBlockEnd(def, ref i, buffer);
            return buffer;
        }
        
        public static byte[] UndefineBlock(byte block) {
            return new byte[] { Opcode.CpeRemoveBlockDefinition, block };
        }
        
        public static byte[] DefineBlockExt(BlockDefinition def, bool uniqueSideTexs, bool hasCP437) {
            byte[] buffer = new byte[uniqueSideTexs ? 88 : 85];
            int i = 0;
            buffer[i++] = Opcode.CpeDefineBlockExt;
            MakeDefineBlockStart(def, buffer, ref i, uniqueSideTexs, hasCP437);
            buffer[i++] = def.MinX; buffer[i++] = def.MinZ; buffer[i++] = def.MinY;
            buffer[i++] = def.MaxX; buffer[i++] = def.MaxZ; buffer[i++] = def.MaxY;
            MakeDefineBlockEnd(def, ref i, buffer);
            return buffer;
        }
        
        static void MakeDefineBlockStart(BlockDefinition def, byte[] buffer, ref int i, 
                                         bool uniqueSideTexs, bool hasCP437) {
            // speed = 2^((raw - 128) / 64);
            // therefore raw = 64log2(speed) + 128
            byte rawSpeed = (byte)(64 * Math.Log(def.Speed, 2) + 128);
            buffer[i++] = def.BlockID;
            NetUtils.Write(def.Name, buffer, i, hasCP437);
            i += NetUtils.StringSize;
            buffer[i++] = def.CollideType;
            buffer[i++] = rawSpeed;
            
            buffer[i++] = def.TopTex;
            if (uniqueSideTexs) {
                buffer[i++] = def.LeftTex;  buffer[i++] = def.RightTex;
                buffer[i++] = def.FrontTex; buffer[i++] = def.BackTex;
            } else {
                buffer[i++] = def.SideTex;
            }
            
            buffer[i++] = def.BottomTex;
            buffer[i++] = (byte)(def.BlocksLight ? 0 : 1);
            buffer[i++] = def.WalkSound;
            buffer[i++] = (byte)(def.FullBright ? 1 : 0);
        }
        
        static void MakeDefineBlockEnd(BlockDefinition def, ref int i, byte[] buffer) {
            buffer[i++] = def.BlockDraw;
            buffer[i++] = def.FogDensity;
            buffer[i++] = def.FogR; buffer[i++] = def.FogG; buffer[i++] = def.FogB;
        }
    }
}
