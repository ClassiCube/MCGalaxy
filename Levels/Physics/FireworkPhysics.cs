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

namespace MCGalaxy.BlockPhysics {
    
    public static class FireworkPhysics {
        
        public static void Do(Level lvl, Check C, Random rand) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (lvl.GetTile(x, (ushort)(y - 1), z) != Block.lavastill)
                return;
            
            if (lvl.GetTile(x, (ushort)(y + 1), z) == Block.air) {
                bool keepGoing = true;
                if ((lvl.Height * 80 / 100) < y)
                    keepGoing = rand.Next(1, 20) > 1;

                if (keepGoing) {
                    int bAbove = lvl.PosToInt(x, (ushort)(y + 1), z);
                    bool unblocked = !lvl.ListUpdate.Exists(u => u.b == bAbove);
                    if (unblocked) {
                        lvl.AddUpdate(bAbove, Block.firework, false);
                        lvl.AddUpdate(C.b, Block.lavastill, false, "wait 1 dissipate 100");
                        C.extraInfo = "wait 1 dissipate 100";
                        return;
                    }
                }
            }
            Firework(x, y, z, 4, lvl, rand);
        }
        
        static void Firework(ushort x, ushort y, ushort z, int size, Level lvl, Random rand) {
            if (lvl.physics < 1 || lvl.physics == 5) return;
            int rand1 = rand.Next(Block.red, Block.white);
            int rand2 = rand.Next(Block.red, Block.white);
            int min = Math.Min(rand1, rand2), max = Math.Max(rand1, rand2);
            // Not using override, since override = true makes it more likely that a colored block will be
            // generated with no extraInfo, because it sets a Check for that position with no extraInfo.
            lvl.AddUpdate(lvl.PosToInt(x, y, z), Block.air);
            
            for (ushort yy = (ushort)(y - (size + 1)); yy <= (ushort)(y + (size + 1)); ++yy)
                for (ushort zz = (ushort)(z - (size + 1)); zz <= (ushort)(z + (size + 1)); ++zz)
                    for (ushort xx = (ushort)(x - (size + 1)); xx <= (ushort)(x + (size + 1)); ++xx)
            {
                if (lvl.GetTile(xx, yy, zz) == Block.air && rand.Next(1, 40) < 2)
                    lvl.AddUpdate(lvl.PosToInt(xx, yy, zz), (byte)rand.Next(min, max), 
                                  false, "drop 100 dissipate 25");
            }
        }
    }
}
