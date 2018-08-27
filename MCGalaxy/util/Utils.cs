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
using System.Globalization;
using System.IO;
using System.Text;

namespace MCGalaxy { 
    public static class Utils {

        /// <summary> The absolute path on disc of the folder MCGalaxy.exe is currently running from. </summary>
        public static string FolderPath { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        
        public static string Hex(byte r, byte g, byte b) {
             return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
        
        public static unsafe void memset(IntPtr srcPtr, byte value, int startIndex, int bytes) {
            byte* srcByte = (byte*)srcPtr + startIndex;
            // Make sure we do an aligned write/read for the bulk copy
            while (bytes > 0 && (startIndex & 0x7) != 0) {
                *srcByte = value; srcByte++; bytes--;
                startIndex++;
            }
            uint valueInt = (uint)((value << 24) | (value << 16) | (value << 8) | value );
            
            if (IntPtr.Size == 8) {
                ulong valueLong = ((ulong)valueInt << 32) | valueInt;
                ulong* srcLong = (ulong*)srcByte;
                while (bytes >= 8) {
                    *srcLong = valueLong; srcLong++; bytes -= 8;
                }
                srcByte = (byte*)srcLong;
            } else {
                uint* srcInt = (uint*)srcByte;
                while (bytes >= 4) {
                    *srcInt = valueInt; srcInt++; bytes -= 4;
                }
                srcByte = (byte*)srcInt;
            }
            
            for (int i = 0; i < bytes; i++) {
                *srcByte = value; srcByte++;
            }
        }


        public static int Clamp(int value, int lo, int hi) {
            return Math.Max(Math.Min(value, hi), lo);
        }
        
        /// <summary> Divides by 16, rounding up if there is a remainder. </summary>
        public static int CeilDiv16(int x) { return (x + 15) / 16; }
        
        const NumberStyles style = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite 
            | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
            
         // Not all languages use . as their decimal point separator    
        public static bool TryParseSingle(string s, out float result) {
            if (s != null && s.IndexOf(',') >= 0) s = s.Replace(',', '.');
            result = 0; float temp;

            if (!Single.TryParse(s, style, NumberFormatInfo.InvariantInfo, out temp)) return false;
            if (Single.IsInfinity(temp) || Single.IsNaN(temp)) return false;
            result = temp;
            return true;
        }
        
        public static bool TryParseDouble(string s, out double result) {
            if (s != null && s.IndexOf(',') >= 0) s = s.Replace(',', '.');
            result = 0; double temp;
            
            if (!Double.TryParse(s, style, NumberFormatInfo.InvariantInfo, out temp)) return false;
            if (Double.IsInfinity(temp) || Double.IsNaN(temp)) return false;
            result = temp;
            return true;
        }

                
        public static List<string> ReadAllLinesList(string path) {
            List<string> lines = new List<string>();
            using (StreamReader r = new StreamReader(path, Encoding.UTF8)) {
                string line;
                while ((line = r.ReadLine()) != null) { lines.Add(line); }
            }
            return lines;
        }
    }
}
