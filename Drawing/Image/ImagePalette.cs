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
        
        internal static string[] Names = {
            null, "2-layer color", "1-layer color", "2-layer grayscale",
            "1-layer grayscale", "Black and White", "Mathematical grayscale"
        };
        public static PaletteEntry[] GetPalette(byte type) {
            if (type == 1) return Color_2;
            if (type == 2) return Color_1;
            if (type == 3) return Grayscale_2;
            if (type == 4) return Grayscale_1;
            if (type == 5) return BlackWhite_1;
            return null;
        }
        
        internal static PaletteEntry[] Color_2 = {
            // front layer
            new PaletteEntry(128, 86, 57, Block.dirt),
            new PaletteEntry(162, 129, 75, Block.wood),
            new PaletteEntry(244, 237, 174, Block.sand),
            new PaletteEntry(226, 31, 38, Block.red),
            new PaletteEntry(223, 135, 37, Block.orange),
            new PaletteEntry(230, 241, 25, Block.yellow),
            new PaletteEntry(127, 234, 26, Block.lightgreen),
            new PaletteEntry(25, 234, 20, Block.green),
            new PaletteEntry(31, 234, 122, Block.aquagreen),
            new PaletteEntry(27, 239, 225, Block.cyan),
            new PaletteEntry(99, 166, 226, Block.lightblue),
            new PaletteEntry(111, 124, 235, Block.blue),
            new PaletteEntry(126, 34, 218, Block.purple),
            new PaletteEntry(170, 71, 219, Block.lightpurple),
            new PaletteEntry(227, 39, 225, Block.pink),
            new PaletteEntry(234, 39, 121, Block.darkpink),
            new PaletteEntry(46, 68, 47, Block.darkgrey),
            new PaletteEntry(135, 145, 130, Block.lightgrey),
            new PaletteEntry(230, 240, 225, Block.white),
            new PaletteEntry(163, 163, 163, Block.staircasefull),
            /*Turns out the back layer blocks are handled awfully.
            new PaletteEntry(217, 131, 155, 55),
            new PaletteEntry(56, 77, 24, 56),
            new PaletteEntry(86, 51, 28, 57),
            new PaletteEntry(39, 51, 154, 58),
            new PaletteEntry(39, 117, 149, 59),*/

            // back layer
            new PaletteEntry(57, 38, 25, Block.dirt),
            new PaletteEntry(72, 57, 33, Block.wood),
            new PaletteEntry(109, 105, 77, Block.sand),
            new PaletteEntry(41, 31, 16, Block.trunk),
            new PaletteEntry(101, 13, 16, Block.red),
            new PaletteEntry(99, 60, 16, Block.orange),
            new PaletteEntry(102, 107, 11, Block.yellow),
            new PaletteEntry(56, 104, 11, Block.lightgreen),
            new PaletteEntry(11, 104, 8, Block.green),
            new PaletteEntry(13, 104, 54, Block.aquagreen),
            new PaletteEntry(12, 106, 100, Block.cyan),
            new PaletteEntry(44, 74, 101, Block.lightblue),
            new PaletteEntry(49, 55, 105, Block.blue),
            new PaletteEntry(56, 15, 97, Block.purple),
            new PaletteEntry(75, 31, 97, Block.lightpurple),
            new PaletteEntry(101, 17, 100, Block.pink),
            new PaletteEntry(104, 17, 54, Block.darkpink),
            new PaletteEntry(20, 30, 21, Block.darkgrey),
            new PaletteEntry(60, 64, 58, Block.lightgrey),
            new PaletteEntry(102, 107, 100, Block.white),
            new PaletteEntry(0, 0, 0, Block.obsidian),
        };
        
        internal static PaletteEntry[] Color_1 = {
            new PaletteEntry(128, 86, 57, Block.dirt),
            new PaletteEntry(162, 129, 75, Block.wood),
            new PaletteEntry(244, 237, 174, Block.sand),
            new PaletteEntry(93, 70, 38, Block.trunk),
            new PaletteEntry(226, 31, 38, Block.red),
            new PaletteEntry(223, 135, 37, Block.orange),
            new PaletteEntry(230, 241, 25, Block.yellow),
            new PaletteEntry(127, 234, 26, Block.lightgreen),
            new PaletteEntry(25, 234, 20, Block.green),
            new PaletteEntry(31, 234, 122, Block.aquagreen),
            new PaletteEntry(27, 239, 225, Block.cyan),
            new PaletteEntry(99, 166, 226, Block.lightblue),
            new PaletteEntry(111, 124, 235, Block.blue),
            new PaletteEntry(126, 34, 218, Block.purple),
            new PaletteEntry(170, 71, 219, Block.lightpurple),
            new PaletteEntry(227, 39, 225, Block.pink),
            new PaletteEntry(234, 39, 121, Block.darkpink),
            new PaletteEntry(46, 68, 47, Block.darkgrey),
            new PaletteEntry(135, 145, 130, Block.lightgrey),
            new PaletteEntry(230, 240, 225, Block.white),
            new PaletteEntry(0, 0, 0, Block.obsidian),
        };
        
        internal static PaletteEntry[] Grayscale_2 = {
            // front layer
            new PaletteEntry(46, 68, 47, Block.darkgrey),
            new PaletteEntry(135, 145, 130, Block.lightgrey),
            new PaletteEntry(230, 240, 225, Block.white),
            // back layer
            new PaletteEntry(20, 30, 21, Block.darkgrey),
            new PaletteEntry(60, 64, 58, Block.lightgrey),
            new PaletteEntry(102, 107, 100, Block.white),
            new PaletteEntry(0, 0, 0, Block.obsidian),
        };
        
        internal static PaletteEntry[] Grayscale_1 = {
            new PaletteEntry(46, 68, 47, Block.darkgrey),
            new PaletteEntry(135, 145, 130, Block.lightgrey),
            new PaletteEntry(230, 240, 225, Block.white),
            new PaletteEntry(0, 0, 0, Block.obsidian),
        };
        
        internal static PaletteEntry[] BlackWhite_1 = {
            new PaletteEntry(255, 255, 255, Block.white),
            new PaletteEntry(0, 0, 0, Block.obsidian),
        };
    }
    
    public struct PaletteEntry  {
        public byte R, G, B, Block;
        
        public PaletteEntry(byte r, byte g, byte b, byte block) {
            R = r; G = g; B = b; Block = block;
        }
    }
}
