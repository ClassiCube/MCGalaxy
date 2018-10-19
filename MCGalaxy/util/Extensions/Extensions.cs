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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace MCGalaxy {
    
    /// <summary> Converts an object into a string. </summary>
    public delegate string StringFormatter<T>(T value);
    
    public static class Extensions {
        
        static char[] space = new char[] { ' ' };
        
        /// <summary> Same as value.Split(' '), but doesn't allocate ' ' each time. </summary>
        /// <example> "abc def xyz".SplitSpaces() becomes "abc", "def", "xyz" </example>
        public static string[] SplitSpaces(this string value) {
            return value.Split(space);
        }
        
        /// <summary> Same as value.Split(' ', maxParts), but doesn't allocate ' ' each time. </summary>
        /// <example> "abc def xyz".SplitSpaces(2) becomes "abc", "def xyz" </example>
        public static string[] SplitSpaces(this string value, int maxParts) {
            return value.Split(space, maxParts);
        }
        
        /// <summary> Works like value.Split(' '), removing first 'startCount' and last 'endCount' elements, 
        /// then joining the leftover elements together again. </summary>
        /// <example> "abc def ghi xyz".Splice(1, 1) becomes "def ghi" </example>
        /// <example> "abc def ghi xyz".Splice(0, 3) becomes "abc" </example>
        /// <example> "abc def ghi xyz".Splice(3, 3) becomes "" </example>
        public static string Splice(this string value, int startCount, int endCount) {
            int start = 0;
            for (int i = 0; i < startCount; i++) {
                start = value.IndexOf(' ', start) + 1;
                if (start == 0) return "";
            }
            value = value.Substring(start);
            
            int end = value.Length;
            for (int i = 0; i < endCount; i++) {
                end = value.LastIndexOf(' ', end - 1);
                if (end == -1) return "";
            }
            return value.Substring(0, end);
        }
        
        /// <summary> Works like value.Split(splitter), setting elements to null if result.Length is less than split.Length. </summary>
        /// <example> "abc def".FixedSplit(new string[3], ' ') results in "abc", "def", null </example>
        public static void FixedSplit(this string value, string[] split, char splitter) {
            int start = 0, i = 0;
            for (; i < split.Length && start <= value.Length; i++) {
                int end = value.IndexOf(splitter, start);
                if (end == -1) end = value.Length;
                
                split[i] = value.Substring(start, end - start);
                start = end + 1;
            }
            
            // If not enough split strings, set remaining to null
            for (; i < split.Length; i++) { split[i] = null; }
        }
                
        static char[] comma = new char[] { ',' };
        static string[] emptyStrs = new string[0];
        
        /// <summary> Trims spaces then calls Split(','). However, returns an empty array on empty input, 
        /// instead of a array consisting of "" that a .Split() call would. </summary>
        public static string[] SplitComma(this string str) {
            // Don't want an array of one entry of empty string
            if (str.Length == 0) return emptyStrs;
            
            if (str.IndexOf(' ') >= 0) str = str.Replace(" ", "");
            if (str.Length == 0) return emptyStrs;
 
            return str.Split(comma);
        }
        
        public static void Separate(this string str, out string prefix, out string suffix) {
            int index = str.IndexOf(' ');
            prefix = index == -1 ? str : str.Substring(0, index);
            suffix = index == -1 ? ""  : str.Substring(index + 1);
        }
        
        
        public static byte[] GZip(this byte[] bytes) {
            using (MemoryStream ms = new MemoryStream()) {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true)) {
                    gs.Write(bytes, 0, bytes.Length);
                }
                
                ms.Position = 0;
                return ms.ToArray();
            }
        }
        
        public static byte[] Decompress(this byte[] gzip, int capacity = 16) {
            using (GZipStream src = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress)) {
                const int size = 4096;
                byte[] buffer = new byte[size];
                
                using (MemoryStream dst = new MemoryStream(capacity)) {
                    int count = 0;
                    while ((count = src.Read(buffer, 0, size)) > 0) {
                        dst.Write(buffer, 0, count);
                    }
                    return dst.ToArray();
                }
            }
        }
        
        // Duplicated to avoid the memory allocation overhead from formatter delegate
        public static string Join(this IEnumerable<string> items, string separator = ", ") {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (string value in items) {
                if (value == null) continue;
                 
                if (!first) builder.Append(separator);
                builder.Append(value);
                first = false;
            }
            return builder.ToString();
        }
        
        public static string Join<T>(this IEnumerable<T> items,
                                     StringFormatter<T> formatter, string separator = ", ") {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (T item in items) {
                string value = formatter(item);
                if (value == null) continue;
                 
                if (!first) builder.Append(separator);
                builder.Append(value);
                first = false;
            }
            return builder.ToString();
        }
        
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        public static bool CaselessEq(this string a, string b) { return a.Equals(b, comp); }       
        public static bool CaselessStarts(this string a, string b) { return a.StartsWith(b, comp); }        
        public static bool CaselessEnds(this string a, string b) { return a.EndsWith(b, comp); }
        public static bool CaselessContains(this string a, string b) { return a.IndexOf(b, comp) >= 0; }
        
        public static bool CaselessContains(this List<string> items, string value) {
            foreach (string item in items) {
                if (item.Equals(value, comp)) return true;
            }
            return false;
        }
        
        public static bool CaselessContains(this string[] items, string value) {
            for (int i = 0; i < items.Length; i++) {
                if (items[i].Equals(value, comp)) return true;
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
        
        public static int CaselessIndexOf(this List<string> items, string value) {
            for (int i = 0; i < items.Count; i++) {
                if (items[i].Equals(value, comp)) return i;
            }
            return -1;
        }
    }
}
