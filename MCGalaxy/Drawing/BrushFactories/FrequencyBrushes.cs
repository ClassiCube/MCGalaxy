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
using MCGalaxy.Commands;
using MCGalaxy.Commands.Building;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes {
    
    /// <summary> Contains helper methods for brushes that have blocks with
    /// optional frequency counts (e.g. random and cloudy brushes) </summary>
    public static class FrequencyBrush {
        
        public static BlockID[] GetBlocks(BrushArgs args, out int[] count,
                                          Predicate<string> argFilter, Predicate<string> argHandler) {
            string[] parts = args.Message.SplitSpaces();
            Player p = args.Player;
            BlockID[] blocks;
            GetRaw(parts, argFilter, args, out blocks, out count);
            
            // check if we're allowed to place the held block
            if (blocks[0] != Block.Invalid && !CommandParser.IsBlockAllowed(p, "draw with", blocks[0])) return null;
            
            for (int i = 0, j = 0; i < parts.Length; i++ ) {
                if (parts[i].Length == 0) continue;
                
                // Brush specific args
                if (argFilter(parts[i])) {
                    if (!argHandler(parts[i])) return null;
                    continue;
                }
                
                int sepIndex = parts[i].IndexOf('/'); 
                string arg = sepIndex >= 0 ? parts[i].Substring(0, sepIndex) : parts[i];
                if (!CommandParser.GetBlockIfAllowed(p, arg, out blocks[j], true)) return null;
                
                if (sepIndex >= 0) {
                    arg = parts[i].Substring(sepIndex + 1);
                    if (!CommandParser.GetInt(p, arg, "Frequency", ref count[j], 1, 10000)) return null;
                }
                j++;
            }
            return blocks;
        }
        
        static void GetRaw(string[] parts, Predicate<string> filter, BrushArgs args,
                           out BlockID[] blocks, out int[] count) {
            int bCount = 0;
            for (int i = 0; i < parts.Length; i++) {
                if (parts[i].Length == 0 || filter(parts[i])) continue;
                bCount++;
            }
            
            // For 0 or 1 blocks given, treat second block as 'unchanged'.
            blocks = new BlockID[Math.Max(2, bCount)];
            count = new int[blocks.Length];
            for (int i = 0; i < count.Length; i++) {
                count[i] = 1;
                blocks[i] = Block.Invalid;
            }
            
            // No blocks given, assume first is held block
            if (bCount == 0) blocks[0] = args.Block;
        }
        
        public static BlockID[] Combine(BlockID[] toAffect, int[] count) {
            int sum = 0;
            for (int i = 0; i < count.Length; i++) sum += count[i];
            
            BlockID[] blocks = new BlockID[sum];
            for (int i = 0, index = 0; i < toAffect.Length; i++) {
                for (int j = 0; j < count[i]; j++)
                    blocks[index++] = toAffect[i];
            }
            return blocks;
        }
    }
    
    public sealed class RandomBrushFactory : BrushFactory {
        public override string Name { get { return "Random"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: [block1/frequency] [block2]..",
            "&HDraws by randomly selecting blocks from the given [blocks].",
            "&Hfrequency is optional (defaults to 1), and specifies the number of times " +
            "the block should appear (as a fraction of the total of all the frequencies).",
        };
        
        public override Brush Construct(BrushArgs args) {
            int[] count;
            BlockID[] toAffect = FrequencyBrush.GetBlocks(args, out count, P => false, null);
            
            if (toAffect == null) return null;
            BlockID[] blocks = FrequencyBrush.Combine(toAffect, count);
            return new RandomBrush(blocks);
        }
    }
}
