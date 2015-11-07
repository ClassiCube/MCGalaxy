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
using System.IO;
using System.Text;

namespace MCGalaxy {
    
    public class CP437Reader : StreamReader {
        
        public CP437Reader(string file) : base(file, Encoding.UTF8) {
        }
        
        public override string ReadLine() {
            string line = base.ReadLine();
            if (line == null) return null;
            if (line.Length == 0) return "";
            
            return ConvertLine(line);
        }
        
        public override string ReadToEnd() {
            string line = base.ReadToEnd();
            if (line == null) return null;
            if (line.Length == 0) return "";
            
            return ConvertLine(line);
        }
        
        public static List<string> ReadAllLines(string file) {
            using (CP437Reader reader = new CP437Reader(file)) {
                List<string> lines = new List<string>();
                string line = null;
                
                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);
                return lines;
            }
        }
        
        public static string ReadAllText(string file) {
            using (CP437Reader reader = new CP437Reader(file)) {
                return reader.ReadToEnd();
            }
        }
        
        static unsafe string ConvertLine(string line) {
            fixed (char* ptr = line) {
                for (int i = 0; i < line.Length; i++)
                    ptr[i] = ConvertChar(ptr[i]);
            }
            return line;
        }
        
        static char ConvertChar(char c) {
            int cpIndex = 0;
            if (c >= ' ' && c <= '~') {
                return c;
            } else if ((cpIndex = FullCP437Handler.ControlCharReplacements.IndexOf(c)) >= 0 ) {
                return (char)cpIndex;
            } else if ((cpIndex = FullCP437Handler.ExtendedCharReplacements.IndexOf(c)) >= 0 ) {
                return (char)(cpIndex + 127);
            }
            return c;
        }
    }
}
