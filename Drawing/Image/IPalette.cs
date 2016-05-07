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
using System.Drawing;

namespace MCGalaxy.Drawing {

    public interface IPalette {
        
        /// <summary> Sets the blocks available for this palette to pick from. </summary>
        void SetAvailableBlocks(ColorBlock[] blocks);
        
        /// <summary> Returns the best matching block for the given color,
        /// based on this palette's colourspace. </summary>
        byte BestMatch(ColorBlock cur, out int position);
    }
    
    public sealed class GrayscalePalette : IPalette {
        
        public void SetAvailableBlocks(ColorBlock[] blocks) { }
        
        public byte BestMatch(ColorBlock cur, out int position) {
            int brightness = (cur.r + cur.g + cur.b) / 3; position = -1;
            if (brightness < (256 / 4))
                return Block.obsidian;
            else if (brightness >= (256 / 4) && brightness < (256 / 4) * 2)
                return Block.darkgrey;
            else if (brightness >= (256 / 4) * 2 && brightness < (256 / 4) * 3)
                return Block.lightgrey;
            else
                return Block.white;
        }
    }
    
    public sealed class RGBPalette : IPalette {
        
        ColorBlock[] palette;
        public void SetAvailableBlocks(ColorBlock[] blocks) {
            this.palette = blocks;
        }
        
        public byte BestMatch(ColorBlock cur, out int position) {
            int minimum = int.MaxValue; position = 0;
            for (int i = 0; i < palette.Length; i++) {
                ColorBlock pixel = palette[i];
                int dist = (cur.r - pixel.r) * (cur.r - pixel.r)
                    + (cur.g - pixel.g) * (cur.g - pixel.g)
                    + (cur.b - pixel.b) * (cur.b - pixel.b);
                
                if (dist < minimum) {
                    minimum = dist; position = i;
                }
            }
            return palette[position].type;
        }
    }
}
