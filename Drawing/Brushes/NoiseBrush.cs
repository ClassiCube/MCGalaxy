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
using System.Collections.Generic;
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Generator;

namespace MCGalaxy.Drawing.Brushes {
    
    public sealed class NoiseBrush : FrequencyBrush {
        readonly ExtBlock[] blocks;
        readonly ImprovedNoise noise;
        
        public NoiseBrush(ExtBlock[] blocks) {
            this.blocks = blocks;
            noise = new ImprovedNoise(new Random());
            noise.Octaves = 4;
        }
        
        public override string Name { get { return "Noise"; } }
        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1/frequency] [block2]..",
            "%HDraws by selecting blocks from the given [blocks] using perlin noise.",
            "%Hfrequency is optional (defaults to 1), and specifies the number of times " +
            "the block should appear (as a fraction of the total of all the frequencies).",
        };
        
        public static Brush Process(BrushArgs args) {
            if (args.Message == "")
                return new NoiseBrush(new[] { new ExtBlock(args.Type, args.ExtType), 
                                               new ExtBlock(Block.Zero, 0) });
            
            string[] parts = args.Message.Split(' ');
            int[] count = new int[parts.Length];
            ExtBlock[] toAffect = GetBlocks(args.Player, parts, count, P => true, null);
            if (toAffect == null) return null;
            
            ExtBlock[] blocks = Combine(toAffect, count);
            return new NoiseBrush(blocks);
        }
        
        int next;
        public override byte NextBlock(DrawOp op) {
            float N = noise.NormalisedNoise(op.Coords.X, op.Coords.Y, op.Coords.Z);
            N = (N + 1) * 0.5f; // rescale to [0, 1];
            next = (int)(N * blocks.Length);
            
            if (next < 0) next = 0;
            if (next >= blocks.Length) next = blocks.Length - 1;
            return blocks[next].Type;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return blocks[next].ExtType;
        }
    }
}
