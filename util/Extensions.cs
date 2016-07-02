/*
    Copyright 2011 MCForge
    
    Author: fenderrock87
    
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MCGalaxy {
    
    public static class Extensions {
        
        static char[] trimChars = {' '};
        public static string[] SplitSpaces(this string value, int maxParts) {
            return value.Split(trimChars, maxParts);
        }
        
        public static string Truncate(this string source, int maxLength) {
            if (source.Length > maxLength)
                source = source.Substring(0, maxLength);
            return source;
        }
        
        public static byte[] GZip(this byte[] bytes) {
            using (MemoryStream ms = new MemoryStream()) {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true))
                    gs.Write(bytes, 0, bytes.Length);
                
                ms.Position = 0;
                return ms.ToArray();
            }
        }
        
        public static byte[] GZip(this byte[] bytes, out int length) {
            using (MemoryStream ms = new MemoryStream()) {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true))
                    gs.Write(bytes, 0, bytes.Length);
                length = (int)ms.Length;
                return ms.GetBuffer();
            }
        }
        
        public static byte[] Decompress(this byte[] gzip, int capacity = 16)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream(capacity))
                {
                    int count = 0;
                    do {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                            memory.Write(buffer, 0, count);
                    } while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        public static string Capitalize(this string str) {
            if (String.IsNullOrEmpty(str))
                return String.Empty;
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        
        public static string Concatenate<T>(this IEnumerable<T> list, string separator = ", ") {
            return String.Join(separator, list);
        }
        
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        public static bool CaselessEq(this string a, string b) { return a.Equals(b, comp); }
        
        public static bool CaselessStarts(this string a, string b) { return a.StartsWith(b, comp);  }
        
        public static bool CaselessContains(this List<string> items, string value) {
            foreach (string item in items) {
                if (item.Equals(value, comp)) return true;
            }
            return false;
        }
        
        public static bool CaselessRemove(this List<string> items, string value) {
            for (int i = 0; i < items.Count; i++) {
                if (!items[i].Equals(value, comp)) continue;
                items.RemoveAt(i); return true;
            }
            return false;
        }
        
        public static bool CheckHex(Player p, ref string arg) {
            if (arg.Length > 0 && arg[0] == '#')
                arg = arg.Substring(1);
            if (arg.Length != 6 || !IsValidHex(arg)) {
                Player.Message(p, "\"#{0}\" is not a valid HEX color.", arg); return false;
            }
            return true;
        }

        static bool IsValidHex(string hex) {
            for (int i = 0; i < hex.Length; i++) {
                if (!Colors.IsStandardColor(hex[i])) return false;
            }
            return true;
        }
        
        public static unsafe void memset( IntPtr srcPtr, byte value, int startIndex, int bytes ) {
            byte* srcByte = (byte*)srcPtr + startIndex;
            // Make sure we do an aligned write/read for the bulk copy
            while( bytes > 0 && ( startIndex & 0x7 ) != 0  ) {
                *srcByte = value; srcByte++; bytes--;
                startIndex++;
            }
            uint valueInt = (uint)( ( value << 24 ) | ( value << 16 ) | ( value << 8 ) | value );
            
            if( IntPtr.Size == 8 ) {
                ulong valueLong = ( (ulong)valueInt << 32 ) | valueInt;
                ulong* srcLong = (ulong*)srcByte;
                while( bytes >= 8 ) {
                    *srcLong = valueLong; srcLong++; bytes -= 8;
                }
                srcByte = (byte*)srcLong;
            } else {
                uint* srcInt = (uint*)srcByte;
                while( bytes >= 4 ) {
                    *srcInt = valueInt; srcInt++; bytes -= 4;
                }
                srcByte = (byte*)srcInt;
            }
            
            for( int i = 0; i < bytes; i++ ) {
                *srcByte = value; srcByte++;
            }
        }
        
        public static T FindMatches<T>(Player pl, string name, out int matches, IEnumerable items,
                                             Predicate<T> filter, Func<T, string> nameGetter, string type, int limit = 5)  {
            T match = default(T); matches = 0;
            name = name.ToLower();
            StringBuilder matchNames = new StringBuilder();

            foreach (T item in items) {
                if (!filter(item)) continue;
                string itemName = nameGetter(item);
                if (itemName.Equals(name, comp)) { matches = 1; return item; }
                if (itemName.IndexOf(name, comp) < 0) continue;
                
                match = item; matches++;
                if (matches <= limit)
                    matchNames.Append(itemName).Append(", ");
                else if (matches == limit + 1)
                    matchNames.Append("(and more)").Append(", ");
            }
            
            if (matches == 0) {
                Player.Message(pl, "No " + type + " match \"" + name + "\"."); return default(T);
            } else if (matches == 1) {
                return match;
            } else {
                string count = matches > limit ? limit + "+ " : matches + " ";
                string names = matchNames.ToString(0, matchNames.Length - 2);
                Player.Message(pl, count + type + " match \"" + name + "\":");
                Player.Message(pl, names); return default(T);
            }
        }
    }
}
