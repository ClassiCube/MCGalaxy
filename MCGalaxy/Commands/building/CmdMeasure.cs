/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdMeasure : Command2 {      
        public override string name { get { return "Measure"; } }
        public override string shortcut { get { return "ms"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message, CommandData data) {
            BlockID[] toCount = null;
            if (message.Length > 0) {
                string[] args = message.SplitSpaces();
                toCount = new BlockID[args.Length];
                for (int i = 0; i < toCount.Length; i++) {
                    if (!CommandParser.GetBlock(p, args[i], out toCount[i])) return;
                }
            }
            
            p.Message("Place or break two blocks to determine the edges.");
            p.MakeSelection(2, "Selecting region for &SMeasure", toCount, DoMeasure);
        }
        
        bool DoMeasure(Player p, Vec3S32[] m, object state, BlockID block) {
            BlockID[] toCount = (BlockID[])state;
            Vec3S32 min = Vec3S32.Min(m[0], m[1]), max = Vec3S32.Max(m[0], m[1]);
            int[] counts = new int[Block.ExtendedCount];
            
            for (ushort y = (ushort)min.Y; y <= (ushort)max.Y; y++)
                for (ushort z = (ushort)min.Z; z <= (ushort)max.Z; z++)
                    for (ushort x = (ushort)min.X; x <= (ushort)max.X; x++)
            {
                counts[p.level.GetBlock(x, y, z)]++;
            }

            int width = max.X - min.X + 1, height = max.Y - min.Y + 1, length = max.Z - min.Z + 1;
            int volume = width * height * length;
            p.Message("Measuring from &a({0}) &Sto &a({1})", min, max);
            p.Message("  &b{0} &Swide, &b{1} &Shigh, &b{2} &Slong, {3} blocks",
                           width, height, length, volume);
            
            string title = "Block types: ";
            if (toCount == null) {
                toCount = MostFrequentBlocks(counts);
                title = "Top " + toCount.Length + " block types: ";
            }
            
            string blocks = toCount.Join(bl => Block.GetName(p, bl) + FormatCount(counts[bl], volume));
            p.Message(title +  blocks);
            return true;
        }
        
        static BlockID[] MostFrequentBlocks(int[] countsRaw) {
            BlockID[] blocks = new BlockID[Block.ExtendedCount];
            int[] counts = new int[Block.ExtendedCount]; // copy array as Sort works in place
            int total = 0;
            
            for (int i = 0; i < blocks.Length; i++) {
                blocks[i] = (BlockID)i;
                counts[i] = countsRaw[i];
                if (counts[i] > 0) total++;
            }
            Array.Sort(counts, blocks);
            
            if (total > 5) total = 5;
            BlockID[] mostFrequent = new BlockID[total];
            for (int i = 0; i < total; i++) {
                mostFrequent[i] = blocks[blocks.Length - 1 - i];
            }
            return mostFrequent;
        }
        
        static string FormatCount(int count, int volume) {
            return ": " + count + " (" + (int)(count * 100.0 / volume) + "%)";
        }
        
        public override void Help(Player p) {
            p.Message("&T/Measure <block1> <block2>..");
            p.Message("&HMeasures all the blocks between two points");
            p.Message("&HShows information such as dimensions, most frequent blocks");
            p.Message("&H  <blocks> optionally indicates which blocks to only count");
        }
    }
}
