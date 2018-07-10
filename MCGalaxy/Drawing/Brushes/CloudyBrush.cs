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
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes {
    
    public sealed class CloudyBrush : Brush {
        readonly BlockID[] blocks;
        readonly int[] counts;
        readonly float[] thresholds;
        readonly ImprovedNoise noise;
        
        public CloudyBrush(BlockID[] blocks, int[] counts, NoiseArgs n) {
            this.blocks = blocks;
            this.counts = counts;
            this.thresholds = new float[counts.Length];
            Random r = n.Seed == int.MinValue ? new Random() : new Random(n.Seed);
            noise = new ImprovedNoise(r);
            
            noise.Frequency = n.Frequency;
            noise.Amplitude = n.Amplitude;
            noise.Octaves = n.Octaves;
            noise.Lacunarity = n.Lacunarity;
            noise.Persistence = n.Persistence;
        }
        
        public override string Name { get { return "Cloudy"; } }
        
        public unsafe override void Configure(DrawOp op, Player p) {
            if (!p.Ignores.DrawOutput) {
                p.Message("Calculating noise distribution...");
            }
            
            // Initalise our noise histogram
            const int accuracy = 10000;
            int* values = stackalloc int[accuracy];
            for (int i = 0; i < accuracy; i++)
                values[i] = 0;
            
            // Fill the histogram with the distribution of the noise
            for (int x = op.Min.X; x <= op.Max.X; x++)
                for (int y = op.Min.Y; y <= op.Max.Y; y++)
                    for (int z = op.Min.Z; z <= op.Max.Z; z++)
            {
                float N = noise.NormalisedNoise(x, y, z);
                N = (N + 1) * 0.5f; // rescale to [0, 1]
                
                int index = (int)(N * accuracy);
                index = index < 0 ? 0 : index;
                index = index >= accuracy ? accuracy - 1 : index;
                values[index]++;
            }
            
            // Calculate the coverage of blocks
            float* coverage = stackalloc float[counts.Length];
            int totalBlocks = 0;
            for (int i = 0; i < counts.Length; i++)
                totalBlocks += counts[i];
            float last = 0;
            for (int i = 0; i < counts.Length; i++) {
                coverage[i] = last + (counts[i] / (float)totalBlocks);
                last = coverage[i];
            }
            
            // Map noise distribution to block coverage
            int volume = (op.Max.X - op.Min.X + 1)
                * (op.Max.Y - op.Min.Y + 1) * (op.Max.Z - op.Min.Z + 1);
            float sum = 0;
            for (int i = 0; i < accuracy; i++) {
                // Update the thresholds
                // So for example if sum is 0.2 and coverage is [0.25, 0.5, 0.75, 1]
                //   then the threshold for all blocks is set to this.
                // If sum was say 0.8 instead, then only the threshold for the
                //   very last block would be increased.
                for (int j = 0; j < counts.Length; j++) {
                    if (sum <= coverage[j])
                        thresholds[j] = i / (float)accuracy;
                }
                sum += values[i] / (float)volume;
            }
            thresholds[blocks.Length - 1] = 1;
            
            if (!p.Ignores.DrawOutput) {
                p.Message("Finished calculating, now drawing.");
            }
        }
        
        int next;
        public override BlockID NextBlock(DrawOp op) {
            float N = noise.NormalisedNoise(op.Coords.X, op.Coords.Y, op.Coords.Z);
            N = (N + 1) * 0.5f; // rescale to [0, 1];
            N = N < 0 ? 0 : N;
            N = N > 1 ? 1 : N;
            
            next = blocks.Length - 1;
            for (int i = 0; i < thresholds.Length; i++) {
                if (N <= thresholds[i]) { next = i; break; }
            }
            return blocks[next];
        }
    }
}
