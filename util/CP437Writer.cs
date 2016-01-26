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
    
    public class CP437Writer : StreamWriter {
        
        public CP437Writer(string file) : base(file, false) {
        }
        
        public CP437Writer(string file, bool append) : base(file, append) {
        }
        
        const string newline = "\r\n";
        public override void WriteLine() {
            base.Write(newline);
        }
        
        public override void Write(string text) {
            if (text != null && text.Length > 0) {
                char[] c = text.ToCharArray();
                for (int i = 0; i < text.Length; i++)
                	c[i] = ConvertChar(text[i]);
                base.Write(c);
            }
        }
        
        public override void WriteLine(string text) {
            Write(text);
            base.Write(newline);
        }
        
        public static void WriteAllLines(string file, string[] lines) {
             using (CP437Writer writer = new CP437Writer(file)) {
                for (int i = 0; i < lines.Length; i++) {
                    writer.Write(lines[i]);
                    writer.WriteLine();
                }
            }
        }
        
        public static void WriteAllText(string file, string text) {
            using (CP437Writer writer = new CP437Writer(file))
                writer.Write(text);
        }
        
        public static string ConvertToUnicode(string text) {
        	if (text == null) return null;
        	if (text.Length == 0) return "";
        	
        	bool hasEmotes = false;
        	for (int i = 0; i < text.Length; i++) {
        		if (text[i] < 0x20 || text[i] >= 0x7F)
        			hasEmotes = true;
        	}
        	if( !hasEmotes ) return text;
        	
        	char[] c = text.ToCharArray();
        	for (int i = 0; i < text.Length; i++)
        		c[i] = ConvertChar(text[i]);
        	return new String( c );
        }
        
        static char ConvertChar(char c) {
            if( c < 0x20 ) {
                return FullCP437Handler.ControlCharReplacements[c];
            } else if( c < 0x7F ) {
                return c;
            } else if( c <= 0xFF ) {
                return FullCP437Handler.ExtendedCharReplacements[c - 0x7F];
            }
            return '?';
        }
    }
}
