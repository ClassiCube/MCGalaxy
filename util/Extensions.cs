/*
	Copyright 2011 MCForge
	
	Author: fenderrock87
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MCGalaxy {
	
    public static class Extensions {
		
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

        public static string Capitalize(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return String.Empty;
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        public static string Concatenate<T>(this List<T> list)
        {
            return list.Concatenate(String.Empty);
        }
        public static string Concatenate<T>(this List<T> list, string separator)
        {
            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (T obj in list)
                    sb.Append(separator + obj.ToString());
                sb.Remove(0, separator.Length);
                return sb.ToString();
            }
            return String.Empty;
        }
        public static string MCCharFilter(this string str)
        {
            // Allowed chars are any ASCII char between 20h/32 and 7Dh/125 inclusive, except for 26h/38 (&) and 60h/96 (`)
            str = Regex.Replace(str, @"[^\u0000-\u007F]", "");

            if (String.IsNullOrEmpty(str.Trim()))
                return str;

            StringBuilder sb = new StringBuilder();

            foreach (char b in Encoding.ASCII.GetBytes(str))
            {
                if (b != 38 && b != 96 && b >= 32 && b <= 125)
                    sb.Append(b);
                /*else
                    sb.Append("*");*/
            }

            return sb.ToString();
        }

        public static void DeleteLine(string file, string line)
        {
            var complete = from selectLine in File.ReadAllLines(file) where selectLine != line select selectLine;
            File.WriteAllLines(file, complete.ToArray());
        }

        public static void DeleteLineWord(string file, string word)
        {
            var complete = from selectLine in File.ReadAllLines(file) where !selectLine.Contains(word) select selectLine;
            File.WriteAllLines(file, complete.ToArray());
        }

        public static void DeleteExactLineWord(string file, string word)
        {
            var complete = from selectLine in File.ReadAllLines(file) where !selectLine.Equals(word) select selectLine;
            File.WriteAllLines(file, complete.ToArray());
        }

        public static void UncapitalizeAll(string file)
        {
            string[] complete = File.ReadAllLines(file);
            for (int i = 0; i < complete.Length; i++)
                complete[i] = complete[i].ToLower();
            File.WriteAllLines(file, complete);
        }
        
        public static bool ContainsInsensitive(this List<string> list, string value) {
            foreach (string item in list)
                if (item.Equals(value, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }     
        
        public static string ToDBTime(this TimeSpan value) {
        	return value.Days + " " + value.Hours + " " + value.Minutes + " " + value.Seconds;
        }
        
        public static TimeSpan ParseDBTime(this string value) {
        	string[] parts = value.Split(' ');
        	return new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 
        	                    int.Parse(parts[2]), int.Parse(parts[3]));
        }
        
        public static bool CaselessEq(this string a, string b) {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }
        
        public static bool CaselessStarts(this string a, string b) {
            return a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
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
    }
}
