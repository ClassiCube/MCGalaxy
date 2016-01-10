/*
    Copyright 2011 MCGalaxy
        
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
using System.Collections.Generic;

namespace MCGalaxy {
    
    internal static class FindReference {
        
        public static ushort WriteLetter(Level l, Player p, char c, ushort x, ushort y, ushort z, byte type, byte extType, int dir) {
            if( (int)c >= 256 || letters[(int)c] == null ) {
                Player.SendMessage(p, "\"" + c + "\" is not currently supported, replacing with space.");
                if (dir == 0) x += 4; 
                else if (dir == 1) x -= 4;
                else if (dir == 2) z += 4; 
                else if (dir == 3) z -= 4;
            } else {
                byte[] flags = letters[(int)c];
                for( int i = 0; i < flags.Length; i++ ) {
                    byte yUsed = flags[i];
                    for (int j = 0; j < 8; j++) {
                        if ((yUsed & (1 << j)) == 0) continue;
                        
                        PlaceBlock(l, p, x, (ushort)(y + j), z, type, extType);
                    }
                    
                    if (dir == 0) x++; 
                    else if (dir == 1) x--;
                    else if (dir == 2) z++;
                    else if (dir == 3) z--;
                }
            }

            if (dir == 0) return (ushort)(x + 1);
            else if (dir == 1) return (ushort)(x - 1);
            else if (dir == 2) return (ushort)(z + 1);
            else if (dir == 3) return (ushort)(z - 1);
            return 0;
        }

        public static List<ColorBlock> popRefCol(byte popType)
        {
            ColorBlock tempref = new ColorBlock();
            List<ColorBlock> refCol = new List<ColorBlock>();
            refCol.Clear();
            //FRONT LAYER BLOCKS
            if (popType == 1)   //poptype 1 = 2-layer color image
            {
                //FRONT LAYER BLOCKS
                tempref.r = 128;
                tempref.g = 86;
                tempref.b = 57;
                tempref.type = 3;
                refCol.Add(tempref);
                tempref.r = 162;
                tempref.g = 129;
                tempref.b = 75;
                tempref.type = 5;
                refCol.Add(tempref);
                tempref.r = 244;
                tempref.g = 237;
                tempref.b = 174;
                tempref.type = 12;
                refCol.Add(tempref);
                tempref.r = 226;
                tempref.g = 31;
                tempref.b = 38;
                tempref.type = 21;
                refCol.Add(tempref);
                tempref.r = 223;
                tempref.g = 135;
                tempref.b = 37;
                tempref.type = 22;
                refCol.Add(tempref);
                tempref.r = 230;
                tempref.g = 241;
                tempref.b = 25;
                tempref.type = 23;
                refCol.Add(tempref);
                tempref.r = 127;
                tempref.g = 234;
                tempref.b = 26;
                tempref.type = 24;
                refCol.Add(tempref);
                tempref.r = 25;
                tempref.g = 234;
                tempref.b = 20;
                tempref.type = 25;
                refCol.Add(tempref);
                tempref.r = 31;
                tempref.g = 234;
                tempref.b = 122;
                tempref.type = 26;
                refCol.Add(tempref);
                tempref.r = 27;
                tempref.g = 239;
                tempref.b = 225;
                tempref.type = 27;
                refCol.Add(tempref);
                tempref.r = 99;
                tempref.g = 166;
                tempref.b = 226;
                tempref.type = 28;
                refCol.Add(tempref);
                tempref.r = 111;
                tempref.g = 124;
                tempref.b = 235;
                tempref.type = 29;
                refCol.Add(tempref);
                tempref.r = 126;
                tempref.g = 34;
                tempref.b = 218;
                tempref.type = 30;
                refCol.Add(tempref);
                tempref.r = 170;
                tempref.g = 71;
                tempref.b = 219;
                tempref.type = 31;
                refCol.Add(tempref);
                tempref.r = 227;
                tempref.g = 39;
                tempref.b = 225;
                tempref.type = 32;
                refCol.Add(tempref);
                tempref.r = 234;
                tempref.g = 39;
                tempref.b = 121;
                tempref.type = 33;
                refCol.Add(tempref);
                tempref.r = 46;
                tempref.g = 68;
                tempref.b = 47;
                tempref.type = 34;
                refCol.Add(tempref);
                tempref.r = 135;
                tempref.g = 145;
                tempref.b = 130;
                tempref.type = 35;
                refCol.Add(tempref);
                tempref.r = 230;
                tempref.g = 240;
                tempref.b = 225;
                tempref.type = 36;
                refCol.Add(tempref);
                tempref.r = 163;
                tempref.g = 163;
                tempref.b = 163;
                tempref.type = 43; //doubleStair
                refCol.Add(tempref);
                /*
                tempref.r = 217; Turns out the back layer blocks are handled awfully.
                tempref.g = 131;
                tempref.b = 155;
                tempref.type = 55; //lightPink
                refCol.Add(tempref);
                tempref.r = 56;
                tempref.g = 77;
                tempref.b = 24;
                tempref.type = 56; //forestGreen
                refCol.Add(tempref);
                tempref.r = 86;
                tempref.g = 51;
                tempref.b = 28;
                tempref.type = 57; //brownWool
                refCol.Add(tempref);
                tempref.r = 39;
                tempref.g = 51;
                tempref.b = 154;
                tempref.type = 58; //deepBlue
                refCol.Add(tempref);
                tempref.r = 39;
                tempref.g = 117;
                tempref.b = 149;
                tempref.type = 59; //turk oys
                refCol.Add(tempref);
                 */
                // BACK LAYER BLOCKS

                tempref.r = 57;
                tempref.g = 38;
                tempref.b = 25;
                tempref.type = 3;
                refCol.Add(tempref);
                tempref.r = 72;
                tempref.g = 57;
                tempref.b = 33;
                tempref.type = 5;
                refCol.Add(tempref);
                tempref.r = 109;
                tempref.g = 105;
                tempref.b = 77;
                tempref.type = 12;
                refCol.Add(tempref);
                tempref.r = 41;
                tempref.g = 31;
                tempref.b = 16;
                tempref.type = 17;
                refCol.Add(tempref);
                tempref.r = 101;
                tempref.g = 13;
                tempref.b = 16;
                tempref.type = 21;
                refCol.Add(tempref);
                tempref.r = 99;
                tempref.g = 60;
                tempref.b = 16;
                tempref.type = 22;
                refCol.Add(tempref);
                tempref.r = 102;
                tempref.g = 107;
                tempref.b = 11;
                tempref.type = 23;
                refCol.Add(tempref);
                tempref.r = 56;
                tempref.g = 104;
                tempref.b = 11;
                tempref.type = 24;
                refCol.Add(tempref);
                tempref.r = 11;
                tempref.g = 104;
                tempref.b = 8;
                tempref.type = 25;
                refCol.Add(tempref);
                tempref.r = 13;
                tempref.g = 104;
                tempref.b = 54;
                tempref.type = 26;
                refCol.Add(tempref);
                tempref.r = 12;
                tempref.g = 106;
                tempref.b = 100;
                tempref.type = 27;
                refCol.Add(tempref);
                tempref.r = 44;
                tempref.g = 74;
                tempref.b = 101;
                tempref.type = 28;
                refCol.Add(tempref);
                tempref.r = 49;
                tempref.g = 55;
                tempref.b = 105;
                tempref.type = 29;
                refCol.Add(tempref);
                tempref.r = 56;
                tempref.g = 15;
                tempref.b = 97;
                tempref.type = 30;
                refCol.Add(tempref);
                tempref.r = 75;
                tempref.g = 31;
                tempref.b = 97;
                tempref.type = 31;
                refCol.Add(tempref);
                tempref.r = 101;
                tempref.g = 17;
                tempref.b = 100;
                tempref.type = 32;
                refCol.Add(tempref);
                tempref.r = 104;
                tempref.g = 17;
                tempref.b = 54;
                tempref.type = 33;
                refCol.Add(tempref);
                tempref.r = 20;
                tempref.g = 30;
                tempref.b = 21;
                tempref.type = 34;
                refCol.Add(tempref);
                tempref.r = 60;
                tempref.g = 64;
                tempref.b = 58;
                tempref.type = 35;
                refCol.Add(tempref);
                tempref.r = 102;
                tempref.g = 107;
                tempref.b = 100;
                tempref.type = 36;
                refCol.Add(tempref);
                tempref.r = 0;
                tempref.g = 0;
                tempref.b = 0;
                tempref.type = 49;
                refCol.Add(tempref);
                
            }
            else if (popType == 2) // poptype 2 = 1 layer color image
            {
                tempref.r = 128;
                tempref.g = 86;
                tempref.b = 57;
                tempref.type = 3;
                refCol.Add(tempref);
                tempref.r = 162;
                tempref.g = 129;
                tempref.b = 75;
                tempref.type = 5;
                refCol.Add(tempref);
                tempref.r = 244;
                tempref.g = 237;
                tempref.b = 174;
                tempref.type = 12;
                refCol.Add(tempref);
                tempref.r = 93;
                tempref.g = 70;
                tempref.b = 38;
                tempref.type = 17;
                refCol.Add(tempref);
                tempref.r = 226;
                tempref.g = 31;
                tempref.b = 38;
                tempref.type = 21;
                refCol.Add(tempref);
                tempref.r = 223;
                tempref.g = 135;
                tempref.b = 37;
                tempref.type = 22;
                refCol.Add(tempref);
                tempref.r = 230;
                tempref.g = 241;
                tempref.b = 25;
                tempref.type = 23;
                refCol.Add(tempref);
                tempref.r = 127;
                tempref.g = 234;
                tempref.b = 26;
                tempref.type = 24;
                refCol.Add(tempref);
                tempref.r = 25;
                tempref.g = 234;
                tempref.b = 20;
                tempref.type = 25;
                refCol.Add(tempref);
                tempref.r = 31;
                tempref.g = 234;
                tempref.b = 122;
                tempref.type = 26;
                refCol.Add(tempref);
                tempref.r = 27;
                tempref.g = 239;
                tempref.b = 225;
                tempref.type = 27;
                refCol.Add(tempref);
                tempref.r = 99;
                tempref.g = 166;
                tempref.b = 226;
                tempref.type = 28;
                refCol.Add(tempref);
                tempref.r = 111;
                tempref.g = 124;
                tempref.b = 235;
                tempref.type = 29;
                refCol.Add(tempref);
                tempref.r = 126;
                tempref.g = 34;
                tempref.b = 218;
                tempref.type = 30;
                refCol.Add(tempref);
                tempref.r = 170;
                tempref.g = 71;
                tempref.b = 219;
                tempref.type = 31;
                refCol.Add(tempref);
                tempref.r = 227;
                tempref.g = 39;
                tempref.b = 225;
                tempref.type = 32;
                refCol.Add(tempref);
                tempref.r = 234;
                tempref.g = 39;
                tempref.b = 121;
                tempref.type = 33;
                refCol.Add(tempref);
                tempref.r = 46;
                tempref.g = 68;
                tempref.b = 47;
                tempref.type = 34;
                refCol.Add(tempref);
                tempref.r = 135;
                tempref.g = 145;
                tempref.b = 130;
                tempref.type = 35;
                refCol.Add(tempref);
                tempref.r = 230;
                tempref.g = 240;
                tempref.b = 225;
                tempref.type = 36;
                refCol.Add(tempref);
                tempref.r = 0;
                tempref.g = 0;
                tempref.b = 0;
                tempref.type = 49;
                refCol.Add(tempref);
            }
            else if (popType == 3) //2-Layer Gray Scale
            {
                //FRONT LAYER
                tempref.r = 46;
                tempref.g = 68;
                tempref.b = 47;
                tempref.type = 34;
                refCol.Add(tempref);
                tempref.r = 135;
                tempref.g = 145;
                tempref.b = 130;
                tempref.type = 35;
                refCol.Add(tempref);
                tempref.r = 230;
                tempref.g = 240;
                tempref.b = 225;
                tempref.type = 36;
                refCol.Add(tempref);
                //BACK LAYER
                tempref.r = 20;
                tempref.g = 30;
                tempref.b = 21;
                tempref.type = 34;
                refCol.Add(tempref);
                tempref.r = 60;
                tempref.g = 64;
                tempref.b = 58;
                tempref.type = 35;
                refCol.Add(tempref);
                tempref.r = 102;
                tempref.g = 107;
                tempref.b = 100;
                tempref.type = 36;
                refCol.Add(tempref);
                tempref.r = 0;
                tempref.g = 0;
                tempref.b = 0;
                tempref.type = 49;
                refCol.Add(tempref);
            }
            else if (popType == 4) //1-Layer grayscale
            {
                tempref.r = 46;
                tempref.g = 68;
                tempref.b = 47;
                tempref.type = 34;
                refCol.Add(tempref);
                tempref.r = 135;
                tempref.g = 145;
                tempref.b = 130;
                tempref.type = 35;
                refCol.Add(tempref);
                tempref.r = 230;
                tempref.g = 240;
                tempref.b = 225;
                tempref.type = 36;
                refCol.Add(tempref);
                tempref.r = 0;
                tempref.g = 0;
                tempref.b = 0;
                tempref.type = 49;
                refCol.Add(tempref);
            }
            else if (popType == 5) // Black and white 1 layer
            {
                tempref.r = 255;
                tempref.g = 255;
                tempref.b = 255;
                tempref.type = 36;
                refCol.Add(tempref);
                tempref.r = 0;
                tempref.g = 0;
                tempref.b = 0;
                tempref.type = 49;
                refCol.Add(tempref);
            }
            return refCol;
        }

        public static void PlaceBlock(Level l, Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            if (p == null)
                l.Blockchange(x, y, z, type, extType);
            else
                l.Blockchange(p, x, y, z, type, extType);
        }

        public struct ColorBlock
        {
            public ushort x, y, z; public byte type, r, g, b, a;
        }
        
        static byte[][] letters;
        static FindReference() {
            letters = new byte[256][];
            // each set bit indicates to place a block with a y offset equal to the bit index.
            // e.g. for 0x3, indicates to place a block at 'y = 0' and 'y = 1'
            letters['A'] = new byte[] { 0x0F, 0x14, 0x0F };
            letters['B'] = new byte[] { 0x1F, 0x15, 0x0A };
            letters['C'] = new byte[] { 0x0E, 0x11, 0x11 };
            letters['D'] = new byte[] { 0x1F, 0x11, 0x0E };
            letters['E'] = new byte[] { 0x1F, 0x15, 0x15 };
            letters['F'] = new byte[] { 0x1F, 0x14, 0x14 };
            letters['G'] = new byte[] { 0x0E, 0x11, 0x17 };
            letters['H'] = new byte[] { 0x1F, 0x04, 0x1F };
            letters['I'] = new byte[] { 0x11, 0x1F, 0x11 };
            letters['J'] = new byte[] { 0x11, 0x11, 0x1E };
            letters['K'] = new byte[] { 0x1F, 0x04, 0x1B };
            letters['L'] = new byte[] { 0x1F, 0x01, 0x01 };
            letters['M'] = new byte[] { 0x1F, 0x08, 0x04, 0x08, 0x1F };
            letters['N'] = new byte[] { 0x1F, 0x08, 0x04, 0x02, 0x1F };
            letters['O'] = new byte[] { 0x0E, 0x11, 0x0E };
            letters['P'] = new byte[] { 0x1F, 0x14, 0x08 };
            letters['Q'] = new byte[] { 0x0E, 0x11, 0x13, 0x0F };
            letters['R'] = new byte[] { 0x1F, 0x14, 0x0B };
            letters['S'] = new byte[] { 0x09, 0x15, 0x12 };
            letters['T'] = new byte[] { 0x10, 0x1F, 0x10 };
            letters['U'] = new byte[] { 0x1E, 0x01, 0x1E };
            letters['V'] = new byte[] { 0x18, 0x06, 0x01, 0x06, 0x18 };
            letters['W'] = new byte[] { 0x1F, 0x02, 0x04, 0x02, 0x1F };
            letters['X'] = new byte[] { 0x1B, 0x04, 0x1B };
            letters['Y'] = new byte[] { 0x10, 0x08, 0x07, 0x08, 0x10 };
            letters['Z'] = new byte[] { 0x11, 0x13, 0x15, 0x19, 0x11 };
            letters['0'] = new byte[] { 0x0E, 0x13, 0x15, 0x19, 0x0E };
            letters['1'] = new byte[] { 0x09, 0x1F, 0x01 };
            letters['2'] = new byte[] { 0x17, 0x15, 0x1D };
            letters['3'] = new byte[] { 0x15, 0x15, 0x1F };
            letters['4'] = new byte[] { 0x1E, 0x02, 0x07, 0x02 };
            letters['5'] = new byte[] { 0x1D, 0x15, 0x17 };
            letters['6'] = new byte[] { 0x1F, 0x15, 0x17 };
            letters['7'] = new byte[] { 0x10, 0x10, 0x1F };
            letters['8'] = new byte[] { 0x1F, 0x15, 0x1F };
            letters['9'] = new byte[] { 0x1D, 0x15, 0x1F };
            
            letters[' '] = new byte[] { 0x00 };
            letters['!'] = new byte[] { 0x1D };
            letters['"'] = new byte[] { 0x18, 0x00, 0x18 };
            // # is missing
            // $ is missing
            // % is missing
            // & is missing
            letters['\''] = new byte[] { 0x18 };
            letters['('] = new byte[] { 0x0E, 0x11 };
            letters[')'] = new byte[] { 0x11, 0x0E };
            // * is missing
            letters['+'] = new byte[] { 0x04, 0x0E, 0x04 };
            letters[','] = new byte[] { 0x01, 0x03 };
            letters['-'] = new byte[] { 0x04, 0x04, 0x04 };
            letters['.'] = new byte[] { 0x01 };
            letters['/'] = new byte[] { 0x01, 0x02, 0x04, 0x08, 0x10 };
            letters[':'] = new byte[] { 0x0A };
            letters[';'] = new byte[] { 0x10, 0x0A };
            letters['\\'] = new byte[] { 0x10, 0x08, 0x04, 0x02, 0x01 };
            letters['<'] = new byte[] { 0x04, 0x0A, 0x11 };
            letters['='] = new byte[] { 0x0A, 0x0A, 0x0A };
            letters['>'] = new byte[] { 0x11, 0x0A, 0x04 };
            // '?' is missing
            // '@' is missing
            letters['['] = new byte[] { 0x1F, 0x11 };
            letters['\''] = new byte[] { 0x18 };
            letters[']'] = new byte[] { 0x11, 0x1F };
            letters['_'] = new byte[] { 0x01, 0x01, 0x01, 0x01 };
            letters['`'] = new byte[] { 0x10, 0x08 };
            letters['{'] = new byte[] { 0x04, 0x1B, 0x11 };
            letters['|'] = new byte[] { 0x1F };
            letters['}'] = new byte[] { 0x11, 0x1B, 0x04 };
            letters['~'] = new byte[] { 0x04, 0x08, 0x04, 0x08 };           
        }
    }
}
