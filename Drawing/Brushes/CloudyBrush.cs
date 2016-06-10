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
    
    public sealed class CloudyBrush : FrequencyBrush {
        readonly ExtBlock[] blocks;
        readonly int[] counts;
        readonly float[] thresholds;
        readonly ImprovedNoise noise;
        
        public CloudyBrush(ExtBlock[] blocks, int[] counts, NoiseArgs n) {
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
        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1/frequency] [block2] <args>..",
            "%HDraws by selecting blocks from the given [blocks] using perlin noise.",
            "%Hfrequency is optional (defaults to 1), and specifies the number of times " +
                "the block should appear (as a fraction of the total of all the frequencies).",
            "%HOptional args format: %T<first letter of argument>_<value>",
            "%HArguments: %Ta%Hmplitude, %Tf%Hrequency (scale), %Ts%Heed, " +
            "%To%Hctaves, %Tp%Hersistence (turbulence), %Tl%Hacunarity",
        };
        
        public static Brush Process(BrushArgs args) {
            NoiseArgs n = default(NoiseArgs);
            // Constants borrowed from fCraft to match it
            n.Amplitude = 1;
            n.Frequency = 0.08f;
            n.Octaves = 3;
            n.Seed = int.MinValue;
            n.Persistence = 0.75f;
            n.Lacunarity = 2;
            
            int[] count;
            ExtBlock[] toAffect = GetBlocks(args, out count,
                                            Filter, arg => Handler(arg, args.Player, ref n));
            
            if (toAffect == null) return null;
            return new CloudyBrush(toAffect, count, n);
        }
        
        // We want to handle non block options.
        static bool Filter(string arg) {
            if (arg.Length < 2) return true;
            return arg[1] != '_';
        }
        
        static bool Handler(string arg, Player p, ref NoiseArgs args) {
            char opt = arg[0];
            arg = arg.Substring(arg.IndexOf('_') + 1);
            
            if (opt == 'l') {
                if (float.TryParse(arg, out args.Lacunarity)) return true;
                Player.Message(p, "\"{0}\" was not a valid decimal.", arg);
            } else if (opt == 'a') {
                if (float.TryParse(arg, out args.Amplitude)) return true;
                Player.Message(p, "\"{0}\" was not a valid decimal.", arg);
            } else if (opt == 'f') {
                if (float.TryParse(arg, out args.Frequency)) return true;
                Player.Message(p, "\"{0}\" was not a valid decimal.", arg);
            } else if (opt == 'p') {
                if (float.TryParse(arg, out args.Persistence)) return true;
                Player.Message(p, "\"{0}\" was not a valid decimal.", arg);
            } else if (opt == 'o') {
                if (byte.TryParse(arg, out args.Octaves)
                    && args.Octaves > 0 && args.Octaves <= 16) return true;
                Player.Message(p, "\"{0}\" was not an integer between 1 and 16.", arg);
            } else if (opt == 's') {
                if (int.TryParse(arg, out args.Seed)) return true;
                Player.Message(p, "\"{0}\" was not a valid integer.", arg);
            } else {
                Player.Message(p, "\"{0}\" was not a valid argument name.", opt);
            }
            return false;
        }
        
        public unsafe override void Configure(DrawOp op, Player p) {
            Player.Message(p, "Calculating noise distribution...");
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
            Player.Message(p, "Finished calculating, now drawing.");
        }
        
        int next;
        public override byte NextBlock(DrawOp op) {
            float N = noise.NormalisedNoise(op.Coords.X, op.Coords.Y, op.Coords.Z);
            N = (N + 1) * 0.5f; // rescale to [0, 1];
            N = N < 0 ? 0 : N;
            N = N > 1 ? 1 : N;
            
            next = blocks.Length - 1;
            for (int i = 0; i < thresholds.Length; i++) {
                if (N <= thresholds[i]) { next = i; break; }
            }
            return blocks[next].Type;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return blocks[next].ExtType;
        }
    }
    
    public struct NoiseArgs {
        public byte Octaves;
        public int Seed;
        public float Frequency, Amplitude, Persistence, Lacunarity;
    }
}
