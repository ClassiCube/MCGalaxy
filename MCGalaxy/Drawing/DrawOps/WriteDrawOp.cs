﻿/*
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
using System;
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {   
    public class WriteDrawOp : DrawOp { 
        public override string Name { get { return "Write"; } }        
        public string Text;
        public byte Scale, Spacing;
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            int blocks = 0;
            foreach (char c in Text) {
                if ((int)c >= 256 || letters[(int)c] == null) {
                    blocks += (4 * Scale) * (4 * Scale);
                } else {
                    byte[] flags = letters[(int)c];
                    for (int i = 0; i < flags.Length; i++)
                        blocks += Scale * Scale * HighestBit(flags[i]);
                }
            }
            return blocks;
        }
        
        int dirX, dirZ;
        Vec3U16 pos;
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(marks[0]), p2 = Clamp(marks[1]);
            if (Math.Abs(p2.X - p1.X) > Math.Abs(p2.Z - p1.Z))
                dirX = p2.X > p1.X? 1 : -1;
            else
                dirZ = p2.Z > p1.Z ? 1 : -1;
            pos = p1;
            
            foreach (char c in Text) {
                DrawLetter(Player, c, brush, output);
            }
        }
        
        void DrawLetter(Player p, char c, Brush brush, DrawOpOutput output) {
            if ((int)c >= 256 || letters[(int)c] == null) {
                Player.Message(p, "\"" + c + "\" is not currently supported, replacing with space.");
                pos.X = (ushort)(pos.X + dirX * 4 * Scale);
                pos.Z = (ushort)(pos.Z + dirZ * 4 * Scale);
            } else {
                byte[] flags = letters[(int)c];
                for (int i = 0; i < flags.Length; i++) {
                    byte yUsed = flags[i];
                    for (int j = 0; j < 8; j++) {
                        if ((yUsed & (1 << j)) == 0) continue;
                        
                        for (int ver = 0; ver < Scale; ver++)
                            for (int hor = 0; hor < Scale; hor++) 
                        {
                            int x = pos.X + dirX * hor, y = pos.Y + j * Scale + ver, z = pos.Z + dirZ * hor;
                            output(Place((ushort)x, (ushort)y, (ushort)z, brush));
                        }
                    }
                    pos.X = (ushort)(pos.X + dirX * Scale);
                    pos.Z = (ushort)(pos.Z + dirZ * Scale);
                }
            }
            pos.X = (ushort)(pos.X + dirX * Spacing);
            pos.Z = (ushort)(pos.Z + dirZ * Spacing);
        }
        
        static int HighestBit(int value) {
            int bits = 0;
            while (value > 0) {
                value >>= 1; bits++;
            }
            return bits;
        }
        
        static byte[][] letters;
        static WriteDrawOp() {
            letters = new byte[256][];
            // each set bit means place a block at y offset equal to that bit index.
            // e.g. 0x0A means place a block at 'y = 0' and at 'y = 3'
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
            letters['#'] = new byte[] { 0x0A, 0x1F, 0x0A, 0x1F, 0x0A };
            // $ is missing
            letters['%'] = new byte[] { 0x11, 0x02, 0x04, 0x08, 0x11 };
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
            letters['?'] = new byte[] { 0x10, 0x15, 0x08 };
            letters['@'] = new byte[] { 0x0E, 0x11, 0x15, 0x0D };
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
