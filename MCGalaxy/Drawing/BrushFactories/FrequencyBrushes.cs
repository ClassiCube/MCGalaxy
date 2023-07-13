/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using MCGalaxy.Commands;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes 
{  
    /// <summary> Contains helper methods for brushes that have blocks with
    /// optional frequency counts (e.g. random and cloudy brushes) </summary>
    public static class FrequencyBrush 
    {       
        public static bool GetBlocks(BrushArgs args, 
                                     out List<BlockID> blocks, out List<int> freqs,
                                     Predicate<string> argFilter, 
                                     Predicate<string> argHandler) {
            string[] parts = args.Message.SplitSpaces(); // out List blocks
            Player p = args.Player;
            
            int minArgs = Math.Max(2, parts.Length);
            blocks = new List<BlockID>(minArgs);
            freqs  = new List<int>(minArgs);
            
            for (int i = 0; i < parts.Length; i++) 
            {
                if (parts[i].Length == 0) continue;
                
                // Brush specific arguments
                if (argFilter(parts[i])) {
                    if (!argHandler(parts[i])) return false;
                    continue;
                }
                
                // Arguments use the format of either:
                //  1) block
                //  2) block/frequency                
                int sepIndex = parts[i].IndexOf('/'); 
                string arg   = sepIndex >= 0 ? parts[i].Substring(0, sepIndex) : parts[i];
                
                int count = CommandParser.GetBlocksIfAllowed(p, arg, "draw with", blocks, true);
                if (count == 0) return false;
                
                int freq = 1;
                if (sepIndex >= 0) {
                    arg = parts[i].Substring(sepIndex + 1);
                    if (!CommandParser.GetInt(p, arg, "Frequency", ref freq, 1, 1000)) return false;
                }
                
                for (int j = 0; j < count; j++)
                    freqs.Add(freq);
            }
            
            // treat 0 arguments as the same as if it was 1 argument of "held block"
            if (blocks.Count == 0) {
                // Check if allowed to place the held block
                if (!CommandParser.IsBlockAllowed(p, "draw with", args.Block)) return false;
                
                blocks.Add(args.Block);
                freqs.Add(1);
            }
            
            // if only 1 block given, treat second block as 'unchanged'/'skip'
            if (blocks.Count == 1) {
                blocks.Add(Block.Invalid);
                freqs.Add(1);
            }
            return true;
        }
        
        /// <summary> Combines list of block IDs and weights/frequencies into a single array of block IDs </summary>
        /// <example> [DIRT, GRASS] and [2, 1] becomes [DIRT, DIRT, GRASS] </example>
        public static BlockID[] Combine(List<BlockID> toAffect, List<int> freqs) {
            int sum = 0;
            foreach (int freq in freqs) sum += freq;
            
            BlockID[] blocks = new BlockID[sum];
            for (int i = 0, index = 0; i < toAffect.Count; i++) 
            {
                for (int j = 0; j < freqs[i]; j++)
                    blocks[index++] = toAffect[i];
            }
            return blocks;
        }
    }
    
    public sealed class RandomBrushFactory : BrushFactory 
    {
        public override string Name { get { return "Random"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: [block1/frequency] [block2]..",
            "&HDraws by randomly selecting blocks from the given [blocks].",
            "&Hfrequency is optional (defaults to 1), and specifies the number of times " +
            "the block should appear (as a fraction of the total of all the frequencies).",
        };
        
        public override Brush Construct(BrushArgs args) {
            List<BlockID> toAffect;
            List<int> freqs;
            
            bool ok = FrequencyBrush.GetBlocks(args, out toAffect, out freqs, 
                                               P => false, null);
            if (!ok) return null;

            BlockID[] blocks = FrequencyBrush.Combine(toAffect, freqs);
            return new RandomBrush(blocks);
        }
    }

    public sealed class GradientBrushFactory : BrushFactory 
    {
        public override string Name { get { return "Gradient"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: <axis> [block1/frequency] [block2]..",
            "&HDraws by linearly selecting blocks from the given [blocks].",
            "&Hfrequency is optional (defaults to 1), and specifies the number of times " +
            "the block should appear (as a fraction of the total of all the frequencies).",
        };
        
        public override Brush Construct(BrushArgs args) {
            CustomModelAnimAxis axis = GetAxis(ref args);
            List<BlockID> toAffect;
            List<int> freqs;
            
            bool ok = FrequencyBrush.GetBlocks(args, out toAffect, out freqs, 
                                               P => false, null);
            if (!ok) return null;

            BlockID[] blocks = FrequencyBrush.Combine(toAffect, freqs);
            return new GradientBrush(blocks, axis);
        }

        // TODO: Need to unify axis parsing code across MCGalaxy
        static CustomModelAnimAxis GetAxis(ref BrushArgs args) {
            CustomModelAnimAxis axis = (CustomModelAnimAxis)200;
            string msg = args.Message;

            if (msg.CaselessStarts("X ")) {
                axis = CustomModelAnimAxis.X;
            } else if (msg.CaselessStarts("Y ")) {
                axis = CustomModelAnimAxis.Y;
            } else if (msg.CaselessStarts("Z ")) {
                axis = CustomModelAnimAxis.Z;
            }

            if (axis <= CustomModelAnimAxis.Z) 
                args.Message = msg.Substring(2);
            return axis;
        }
    }
}
