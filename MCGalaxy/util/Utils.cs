/*
    Copyright 2015-2024 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace MCGalaxy 
{
    public static class Utils 
    {
        public static string Hex(byte r, byte g, byte b) {
            return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
        
        public static string ToHexString(byte[] data) {
            char[] hex = new char[data.Length * 2];

            for (int i = 0; i < data.Length; i++)
            {
                int value = data[i];
                hex[i * 2 + 0] = HexEncode(value >> 4);
                hex[i * 2 + 1] = HexEncode(value & 0x0F);
            }
            return new string(hex);
        }

        static char HexEncode(int i) {
            return i < 10 ? (char)(i + '0') : (char)((i - 10) + 'a');
        }


        public static int Clamp(int value, int lo, int hi) {
            return Math.Max(Math.Min(value, hi), lo);
        }
        
        /// <summary> Divides x by y, rounding up if there is a remainder. </summary>
        public static int CeilDiv(int x, int y) { 
            return (x + (y - 1)) / y;
        }

        
        public static List<string> ReadAllLinesList(string path) {
            List<string> lines = new List<string>();
            using (StreamReader r = new StreamReader(path, Encoding.UTF8)) {
                string line;
                while ((line = r.ReadLine()) != null) { lines.Add(line); }
            }
            return lines;
        }

 
        public static void SetBackgroundMode(Thread thread) {
            // Throws an exception when called on a dead thread,
            //  which can very rarely happen
            try { thread.IsBackground = true; } catch { }
        }
    }
    
    public static class MemUtils
    {
        /// <summary> Reads an unsigned 16 bit little endian integer. </summary>
        public static ushort ReadU16_LE(byte[] array, int offset) {
            return (ushort)(array[offset] | array[offset + 1] << 8);
        }
        
        /// <summary> Reads a signed 16 bit big endian integer. </summary>
        public static short ReadI16_BE(byte[] array, int offset) {
            return (short)(array[offset] << 8 | array[offset + 1]);
        }

        /// <summary> Reads an unsigned 16 bit big endian integer. </summary>
        public static ushort ReadU16_BE(byte[] array, int offset) {
            return (ushort)(array[offset] << 8 | array[offset + 1]);
        }

        /// <summary> Reads a signed 32 bit big endian integer. </summary>
        public static int ReadI32_BE(byte[] array, int offset) {
            return array[offset] << 24 | array[offset + 1] << 16
                | array[offset + 2] << 8 | array[offset + 3];
        }
        
        
        public static unsafe void memset(IntPtr srcPtr, byte value, int startIndex, int bytes) {
            byte* srcByte = (byte*)srcPtr + startIndex;
            // Make sure that aligned write/read is used for the bulk copy
            while (bytes > 0 && (startIndex & 0x7) != 0) {
                *srcByte = value; srcByte++; bytes--;
                startIndex++;
            }
            uint valueU32 = (uint)((value << 24) | (value << 16) | (value << 8) | value );
            
            if (IntPtr.Size == 8) {
                ulong valueU64 = ((ulong)valueU32 << 32) | valueU32;
                ulong* srcU64  = (ulong*)srcByte;
                while (bytes >= 8) {
                    *srcU64 = valueU64; srcU64++; bytes -= 8;
                }
                srcByte = (byte*)srcU64;
            } else {
                uint* srcU32 = (uint*)srcByte;
                while (bytes >= 4) {
                    *srcU32 = valueU32; srcU32++; bytes -= 4;
                }
                srcByte = (byte*)srcU32;
            }
            
            for (int i = 0; i < bytes; i++) {
                *srcByte = value; srcByte++;
            }
        }
    }
    
    public static class StreamUtils
    {
        /// <summary> Reads the given number of bytes from the given stream </summary>
        /// <remarks> Throws EndOfStreamException if unable to read sufficient bytes </remarks>
        public static void ReadFully(Stream s, byte[] data, int offset, int count) {
            while (count > 0) {
                int read = s.Read(data, offset, count);
                
                if (read == 0) throw new EndOfStreamException("End of stream reading data");
                offset += read; count -= read;
            }
        }
    }
}
