/*
    Ported from Forester.py by dudecon
    Original at http://peripheralarbor.com/minecraft/Forester.py
    From the website: "The scripts are all available to download for free. If you'd like to make your own based on the code, go right ahead."
    
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

namespace MCGalaxy.Generator.Foilage {

    public sealed class BambooTree : Tree {
        
        public override int DefaultValue(Random rnd) { return rnd.Next(4, 8); }
        
        public override void SetData(Random rnd, int value) {
            height = (byte)value;
            size = 1;
            this.rnd = rnd;
        }        
        
        public override void Generate(ushort x, ushort y, ushort z, TreeOutput output) {
            for (int dy = 0; dy <= height; dy++) {
                ushort yy = (ushort)(y + dy);
                if (dy < height) output(x, yy, z, Block.trunk);
                
                for (int i = 0; i < 2; i++) {
                    int dx = rnd.NextDouble() >= 0.5 ? 1 : -1;
                    int dz = rnd.NextDouble() >= 0.5 ? 1 : -1;
                    output((ushort)(x + dx), yy, (ushort)(z + dz), Block.leaf);
                }
            }
        }
    }
    
    public sealed class PalmTree : Tree {
        
        public override int DefaultValue(Random rnd) { return rnd.Next(4, 8); }

        public override void SetData(Random rnd, int value) {
            height = (byte)value;
            size = 2;
            this.rnd = rnd;
        }        
        
        public override void Generate(ushort x, ushort y, ushort z, TreeOutput output) {
            for (int dy = 0; dy <= height; dy++)
                if (dy < height) output(x, (ushort)(y + dy), z, Block.trunk);
            
            for (int dz = -2; dz <= 2; dz++)
                for (int dx = -2; dx <= 2; dx++)
            {
                if (Math.Abs(dx) != Math.Abs(dz)) continue;
                output((ushort)(x + dx), (ushort)(y + height), (ushort)(z + dz), Block.leaf);
            }
        }
    }
}
