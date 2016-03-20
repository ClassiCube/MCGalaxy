/*
    Copyright 2011 MCForge
        
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
using System.Collections.Generic;

namespace MCGalaxy.Drawing {
    
    internal static class ImagePalette {
        
        public static ColorBlock[] GetPalette(byte type) {
            if (type == 1) return Color_2;      // 2-Layer color
            if (type == 2) return Color_1;      // 1-Layer color
            if (type == 3) return Grayscale_2;  // 2-Layer grayscale
            if (type == 4) return Grayscale_1;  // 1-Layer grayscale
            if (type == 5) return BlackWhite_1; // 1-Layer Black and white
            return null;
        }
        
        internal static ColorBlock[] Color_2 = {
            //FRONT LAYER BLOCKS
            new ColorBlock(128, 86, 57, 3),
            new ColorBlock(162, 129, 75, 5),
            new ColorBlock(244, 237, 174, 12),
            new ColorBlock(226, 31, 38, 21),
            new ColorBlock(223, 135, 37, 22),
            new ColorBlock(230, 241, 25, 23),
            new ColorBlock(127, 234, 26, 24),
            new ColorBlock(25, 234, 20, 25),
            new ColorBlock(31, 234, 122, 26),
            new ColorBlock(27, 239, 225, 27),
            new ColorBlock(99, 166, 226, 28),
            new ColorBlock(111, 124, 235, 29),
            new ColorBlock(126, 34, 218, 30),
            new ColorBlock(170, 71, 219, 31),
            new ColorBlock(227, 39, 225, 32),
            new ColorBlock(234, 39, 121, 33),
            new ColorBlock(46, 68, 47, 34),
            new ColorBlock(135, 145, 130, 35),
            new ColorBlock(230, 240, 225, 36),
            new ColorBlock(163, 163, 163, 43),
            /*Turns out the back layer blocks are handled awfully.
            new ColorBlock(217, 131, 155, 55),
            new ColorBlock(56, 77, 24, 56),
            new ColorBlock(86, 51, 28, 57),
            new ColorBlock(39, 51, 154, 58),
            new ColorBlock(39, 117, 149, 59),*/

            // BACK LAYER BLOCKS
            new ColorBlock(57, 38, 25, 3),
            new ColorBlock(72, 57, 33, 5),
            new ColorBlock(109, 105, 77, 12),
            new ColorBlock(41, 31, 16, 17),
            new ColorBlock(101, 13, 16, 21),
            new ColorBlock(99, 60, 16, 22),
            new ColorBlock(102, 107, 11, 23),
            new ColorBlock(56, 104, 11, 24),
            new ColorBlock(11, 104, 8, 25),
            new ColorBlock(13, 104, 54, 26),
            new ColorBlock(12, 106, 100, 27),
            new ColorBlock(44, 74, 101, 28),
            new ColorBlock(49, 55, 105, 29),
            new ColorBlock(56, 15, 97, 30),
            new ColorBlock(75, 31, 97, 31),
            new ColorBlock(101, 17, 100, 32),
            new ColorBlock(104, 17, 54, 33),
            new ColorBlock(20, 30, 21, 34),
            new ColorBlock(60, 64, 58, 35),
            new ColorBlock(102, 107, 100, 36),
            new ColorBlock(0, 0, 0, 49),
        };
        
        internal static ColorBlock[] Color_1 = {
            new ColorBlock(128, 86, 57, 3),
            new ColorBlock(162, 129, 75, 5),
            new ColorBlock(244, 237, 174, 12),
            new ColorBlock(93, 70, 38, 17),
            new ColorBlock(226, 31, 38, 21),
            new ColorBlock(223, 135, 37, 22),
            new ColorBlock(230, 241, 25, 23),
            new ColorBlock(127, 234, 26, 24),
            new ColorBlock(25, 234, 20, 25),
            new ColorBlock(31, 234, 122, 26),
            new ColorBlock(27, 239, 225, 27),
            new ColorBlock(99, 166, 226, 28),
            new ColorBlock(111, 124, 235, 29),
            new ColorBlock(126, 34, 218, 30),
            new ColorBlock(170, 71, 219, 31),
            new ColorBlock(227, 39, 225, 32),
            new ColorBlock(234, 39, 121, 33),
            new ColorBlock(46, 68, 47, 34),
            new ColorBlock(135, 145, 130, 35),
            new ColorBlock(230, 240, 225, 36),
            new ColorBlock(0, 0, 0, 49),
        };
        
        internal static ColorBlock[] Grayscale_2 = {
            //FRONT LAYER
            new ColorBlock(46, 68, 47, 34),
            new ColorBlock(135, 145, 130, 35),
            new ColorBlock(230, 240, 225, 36),
            //BACK LAYER
            new ColorBlock(20, 30, 21, 34),
            new ColorBlock(60, 64, 58, 35),
            new ColorBlock(102, 107, 100, 36),
            new ColorBlock(0, 0, 0, 49),
        };
        
        internal static ColorBlock[] Grayscale_1 = {
            new ColorBlock(46, 68, 47, 34),
            new ColorBlock(135, 145, 130, 35),
            new ColorBlock(230, 240, 225, 36),
            new ColorBlock(0, 0, 0, 49),
        };
        
        internal static ColorBlock[] BlackWhite_1 = {
            new ColorBlock(255, 255, 255, 36),
            new ColorBlock(0, 0, 0, 49),
        };

        public struct ColorBlock  {
            public ushort x, y, z; public byte type, r, g, b, a;
            
            public ColorBlock(byte r, byte g, byte b, byte type) {
                this.r = r; this.g = g; this.b = b; this.type = type;
                x = 0; y = 0; z = 0; a = 0;
            }
        }
    }
}
