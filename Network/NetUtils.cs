/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MCGalaxy.Drawing;
using MCGalaxy.SQL;

namespace MCGalaxy {
	
    public static class NetUtils {

        public static void WriteAscii(string str, byte[] array, int offset) {
            for (int i = 0; i < 64; i++)
                array[offset + i] = (byte)' ';

            for (int i = 0; i < Math.Min(str.Length, 64); i++) {
            	char raw = str[i];
            	array[offset + i] = raw >= '\u0080' ? (byte)'?' : (byte)raw;
            }          
        }

		public static void WriteCP437(string str, byte[] array, int offset) {
            for (int i = 0; i < 64; i++)
                array[offset + i] = (byte)' ';

            for (int i = 0; i < Math.Min(str.Length, 64); i++) {
                array[offset + i] = (byte)str[i];
            }
        }
		
		public static short ReadI16(byte[] array, int offset) {
			return (short)(array[offset] << 8 | array[offset + 1]);
		}
        
        public static ushort ReadU16(byte[] array, int offset) {
			return (ushort)(array[offset] << 8 | array[offset + 1]);
		}
        
        public static int ReadI32(byte[] array, int offset) {
			return array[offset] << 24 | array[offset + 1] << 16 
				| array[offset + 2] << 8 | array[offset + 3];
		}
        
        public static void WriteI16(short value, byte[] array, int index) {
			array[index++] = (byte)(value >> 8);
			array[index++] = (byte)(value);
		}
        
        public static void WriteU16(ushort value, byte[] array, int index) {
			array[index++] = (byte)(value >> 8);
			array[index++] = (byte)(value);
		}
		
		public static void WriteI32(int value, byte[] array, int index) {
			array[index++] = (byte)(value >> 24);
			array[index++] = (byte)(value >> 16);
			array[index++] = (byte)(value >> 8);
			array[index++] = (byte)(value);
		}
		
		public static byte[] GetPositionPacket(byte id, ushort[] pos, ushort[] oldpos,
		                                       byte[] rot, byte[] oldrot, byte realPitch, bool bot) {
			bool posChanged = false, oriChanged = false, absPosUpdate = false;
        	if (oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2])
        		posChanged = true;
        	if (oldrot[0] != rot[0] || oldrot[1] != rot[1])
        		oriChanged = true;
        	if (Math.Abs(pos[0] - oldpos[0]) > 32 || Math.Abs(pos[1] - oldpos[1]) > 32 || Math.Abs(pos[2] - oldpos[2]) > 32)
        		absPosUpdate = true;
        	// TODO: not sure why this is necessary for bots
        	if (bot) 
        		absPosUpdate = true;

        	byte[] buffer = null;
        	if (absPosUpdate) {
        		buffer = new byte[10];
        		buffer[0] = Opcode.EntityTeleport;
        		buffer[1] = id;
        		NetUtils.WriteU16(pos[0], buffer, 2);
        		NetUtils.WriteU16(pos[1], buffer, 4);
        		NetUtils.WriteU16(pos[2], buffer, 6);
        		buffer[8] = rot[0];
        		buffer[9] = realPitch;
        	} else if (posChanged && oriChanged) {
        		buffer = new byte[7];
        		buffer[0] = Opcode.RelPosAndOrientationUpdate;
        		buffer[1] = id;
        		buffer[2] = (byte)(pos[0] - oldpos[0]);
        		buffer[3] = (byte)(pos[1] - oldpos[1]);
        		buffer[4] = (byte)(pos[2] - oldpos[2]);
        		buffer[5] = rot[0];
        		buffer[6] = realPitch;
        	} else if (posChanged) {
        		buffer = new byte[5]; 
        		buffer[0] = Opcode.RelPosUpdate;
        		buffer[1] = id;
        		buffer[2] = (byte)(pos[0] - oldpos[0]);
        		buffer[3] = (byte)(pos[1] - oldpos[1]);
        		buffer[4] = (byte)(pos[2] - oldpos[2]);
        	} else if (oriChanged) {
        		buffer = new byte[4];
        		buffer[0] = Opcode.OrientationUpdate;
        		buffer[1] = id;
        		buffer[2] = rot[0];
        		buffer[3] = realPitch;
        	}
        	return buffer;
		}
    }
}
