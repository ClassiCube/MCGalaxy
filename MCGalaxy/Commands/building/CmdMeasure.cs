/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building 
{
    public sealed class CmdMeasure : Command2 
    {
        public override string name { get { return "Measure"; } }
        public override string shortcut { get { return "ms"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message, CommandData data) {
            List<BlockID> toCount = null;
            if (message.Length > 0) {
                string[] args = message.SplitSpaces();
                toCount = new List<BlockID>(args.Length);
                
                for (int i = 0; i < args.Length; i++) 
                {
                    int count = CommandParser.GetBlocks(p, args[i], toCount, false);
                    if (count == 0) return;
                }
            }
            
            p.Message("Place or break two blocks to determine the edges.");
            p.MakeSelection(2, "Selecting region for &SMeasure", toCount, DoMeasure);
        }
        
        bool DoMeasure(Player p, Vec3S32[] m, object state, BlockID block) {
            List<BlockID> toCount = (List<BlockID>)state;
            Vec3S32 min  = Vec3S32.Min(m[0], m[1]);
            Vec3S32 max  = Vec3S32.Max(m[0], m[1]);
            int[] counts = new int[Block.SUPPORTED_COUNT];
            
            for (ushort y = (ushort)min.Y; y <= (ushort)max.Y; y++)
                for (ushort z = (ushort)min.Z; z <= (ushort)max.Z; z++)
                    for (ushort x = (ushort)min.X; x <= (ushort)max.X; x++)
            {
                counts[p.level.FastGetBlock(x, y, z)]++;
            }

            int width = max.X - min.X + 1, height = max.Y - min.Y + 1, length = max.Z - min.Z + 1;
            int volume = width * height * length;
            p.Message("Measuring from &a({0}) &Sto &a({1})", min, max);
            p.Message("  &b{0} &Swide, &b{1} &Shigh, &b{2} &Slong, {3} blocks",
                           width, height, length, volume);
            
            string title = "Block types: ";
            if (toCount == null) {
                toCount = MostFrequentBlocks(counts);
                title   = "Top " + toCount.Count + " block types: ";
            }
            
            string blocks = toCount.Join(bl => Block.GetName(p, bl) + FormatCount(counts[bl], volume));
            p.Message(title + blocks);
            return true;
        }
        
        static List<BlockID> MostFrequentBlocks(int[] countsRaw) {
            BlockID[] blocks = new BlockID[Block.SUPPORTED_COUNT];
            int[] counts = new int[Block.SUPPORTED_COUNT]; // copy array as Sort works in place
            int total = 0;
            
            for (int i = 0; i < blocks.Length; i++) {
                blocks[i] = (BlockID)i;
                counts[i] = countsRaw[i];
                if (counts[i] > 0) total++;
            }
            
            Array.Sort(counts, blocks);
            if (total > 5) total = 5;
            
            List<BlockID> mostFrequent = new List<BlockID>(total);
            for (int i = 0; i < total; i++) 
            {
                mostFrequent.Add(blocks[blocks.Length - 1 - i]);
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
