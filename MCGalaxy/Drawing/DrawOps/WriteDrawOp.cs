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
                if ((int)c >= 256 || letters[c] == 0) {
                    blocks += (4 * Scale) * (4 * Scale);
                } else {
                    // first 'vertical line flags' is at highest position
                    ulong flags = letters[c]; int shift = 56;
                    while (flags != 0) {
                        // get the current flags
                        byte yUsed = (byte)(flags >> shift);
                        // clear current flags and move to next
                        flags &= (1UL << shift) - 1; shift -= 8;
                        
                        blocks += Scale * Scale * CountBits(yUsed);
                    }
                }
            }
            return blocks;
        }
        
        Vec3S32 dir, pos;
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3S32 p1 = marks[0], p2 = marks[1];
            if (Math.Abs(p2.X - p1.X) > Math.Abs(p2.Z - p1.Z)) {
                dir.X = p2.X > p1.X ? 1 : -1;
            } else {
                dir.Z = p2.Z > p1.Z ? 1 : -1;
            }
            
            pos = p1;            
            foreach (char c in Text) { DrawLetter(Player, c, brush, output); }
        }
        
        void DrawLetter(Player p, char c, Brush brush, DrawOpOutput output) {
            if ((int)c >= 256 || letters[c] == 0) {
                if (c != ' ') p.Message("\"{0}\" is not currently supported, replacing with space.", c);
                pos += dir * (4 * Scale);
            } else {
                ulong flags = letters[c]; int shift = 56;
                while (flags != 0) {
                    byte yUsed = (byte)(flags >> shift);  
                    flags &= (1UL << shift) - 1; shift -= 8;
                    
                    for (int j = 0; j < 8; j++) {
                        if ((yUsed & (1 << j)) == 0) continue;
                        
                        for (int ver = 0; ver < Scale; ver++)
                            for (int hor = 0; hor < Scale; hor++) 
                        {
                            int x = pos.X + dir.X * hor, y = pos.Y + j * Scale + ver, z = pos.Z + dir.Z * hor;
                            output(Place((ushort)x, (ushort)y, (ushort)z, brush));
                        }
                    }
                    pos += dir * Scale;
                }
            }
            pos += dir * Spacing;
        }
        
        static int CountBits(int value) {
            int bits = 0;
            while (value > 0) {
                value >>= 1; bits++;
            }
            return bits;
        }
        
        static ulong[] letters;
        static WriteDrawOp() {
            letters = new ulong[256];
            // Each letter is represented as 8 bytes
            // Each byte represents a vertical line in that letter.
            // For each byte, each set bit means place a block at y offset equal to the index of that bit.
            
            // For example, take the letter 'A', which is 0x0F140F0000000000UL
            // Taking each byte in the 'A' until value is 0, we get 0x0F 0x14 0x0F, which becomes
            //       y = 7
            //       y = 6
            //       y = 5
            //  █    y = 4
            // █ █   y = 3
            // ███   y = 2
            // █ █   y = 1
            // █ █   y = 0
            
            letters['A']  = 0x0F140F0000000000UL;
            letters['B']  = 0x1F150A0000000000UL;
            letters['C']  = 0x0E11110000000000UL;
            letters['D']  = 0x1F110E0000000000UL;
            letters['E']  = 0x1F15150000000000UL;
            letters['F']  = 0x1F14140000000000UL;
            letters['G']  = 0x0E11170000000000UL;
            letters['H']  = 0x1F041F0000000000UL;
            letters['I']  = 0x111F110000000000UL;
            letters['J']  = 0x11111E0000000000UL;
            letters['K']  = 0x1F041B0000000000UL;
            letters['L']  = 0x1F01010000000000UL;
            letters['M']  = 0x1F0804081F000000UL;
            letters['N']  = 0x1F0804021F000000UL;
            letters['O']  = 0x0E110E0000000000UL;
            letters['P']  = 0x1F14080000000000UL;
            letters['Q']  = 0x0E11130F00000000UL;
            letters['R']  = 0x1F140B0000000000UL;
            letters['S']  = 0x0915120000000000UL;
            letters['T']  = 0x101F100000000000UL;
            letters['U']  = 0x1E011E0000000000UL;
            letters['V']  = 0x1806010618000000UL;
            letters['W']  = 0x1F0204021F000000UL;
            letters['X']  = 0x1B041B0000000000UL;
            letters['Y']  = 0x1807180000000000UL;
            letters['Z']  = 0x1113151911000000UL;
            letters['0']  = 0x0E1315190E000000UL;
            letters['1']  = 0x091F010000000000UL;
            letters['2']  = 0x17151D0000000000UL;
            letters['3']  = 0x15151F0000000000UL;
            letters['4']  = 0x1E02070200000000UL;
            letters['5']  = 0x1D15170000000000UL;
            letters['6']  = 0x1F15170000000000UL;
            letters['7']  = 0x10101F0000000000UL;
            letters['8']  = 0x1F151F0000000000UL;
            letters['9']  = 0x1D151F0000000000UL;
            
            letters['!']  = 0x1D00000000000000UL;
            letters['"']  = 0x1800180000000000UL;
            letters['#']  = 0x0A1F0A1F0A000000UL;
            // $ is missing
            letters['%']  = 0x1102040811000000UL;
            // & is missing
            letters['\''] = 0x1800000000000000UL;
            letters['(']  = 0x0E11000000000000UL;
            letters[')']  = 0x110E000000000000UL;
            // * is missing
            letters['+']  = 0x040E040000000000UL;
            letters[',']  = 0x0103000000000000UL;
            letters['-']  = 0x0404040000000000UL;
            letters['.']  = 0x0100000000000000UL;
            letters['/']  = 0x0102040810000000UL;
            letters[':']  = 0x0A00000000000000UL;
            letters[';']  = 0x100A000000000000UL;
            letters['\\'] = 0x1008040201000000UL;
            letters['<']  = 0x040A110000000000UL;
            letters['=']  = 0x0A0A0A0000000000UL;
            letters['>']  = 0x110A040000000000UL;
            letters['?']  = 0x1015080000000000UL;
            letters['@']  = 0x0E11150D00000000UL;
            letters['[']  = 0x1F11000000000000UL;
            letters['\''] = 0x1800000000000000UL;
            letters[']']  = 0x111F000000000000UL;
            letters['_']  = 0x0101010100000000UL;
            letters['`']  = 0x1008000000000000UL;
            letters['{']  = 0x041B110000000000UL;
            letters['|']  = 0x1F00000000000000UL;
            letters['}']  = 0x111B040000000000UL;
            letters['~']  = 0x0408040800000000UL;
        }
    }
}
