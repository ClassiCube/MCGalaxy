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
using MCGalaxy.Commands.Building;

namespace MCGalaxy.Drawing.Brushes {
    
    /// <summary> Contains helper methods for brushes that have blocks with 
    /// optional frequency counts (e.g. random and cloudy brushes) </summary>
    public static class FrequencyBrush {
        
        public static ExtBlock[] GetBlocks(BrushArgs args, out int[] count,
                                           Predicate<string> filter, Predicate<string> handler) {
            string[] parts = args.Message.Split(' ');
            Player p = args.Player;
            ExtBlock[] blocks;
            GetRaw(parts, filter, args, out blocks, out count);
            
            for (int i = 0, j = 0; i < parts.Length; i++ ) {
                if (parts[i] == "") continue;
                
                // Brush specific args
                if (!filter(parts[i])) {
                    if (!handler(parts[i])) return null;
                    continue;
                }
                
                byte extType = 0;
                int sepIndex = parts[i].IndexOf('/');
                string block = sepIndex >= 0 ? parts[i].Substring(0, sepIndex) : parts[i];
                int type = DrawCmd.GetBlock(p, block, out extType);
                if (type == -1) return null;
                
                blocks[j].Block = (byte)type; blocks[j].Ext = extType;
                if (sepIndex < 0) { j++; continue; }
                
                int chance;
                if (!int.TryParse(parts[i].Substring(sepIndex + 1), out chance)
                    || chance <= 0 || chance > 10000) {
                    Player.Message(p, "frequency must be an integer between 1 and 10,000."); return null;
                }
                count[j] = chance;
                j++;
            }
            return blocks;
        }
        
        static void GetRaw(string[] parts, Predicate<string> filter, BrushArgs args,
                           out ExtBlock[] blocks, out int[] count) {;
            int bCount = 0;
            for (int i = 0; i < parts.Length; i++) {
                if (parts[i] == "" || !filter(parts[i])) continue;
                bCount++;
            }
            
            // For 0 or 1 blocks given, treat second block as 'unchanged'.
            blocks = new ExtBlock[Math.Max(2, bCount)];
            count = new int[blocks.Length];
            for (int i = 0; i < count.Length; i++) {
                count[i] = 1;
                blocks[i] = new ExtBlock(Block.Zero, 0);
            }
            
            // No blocks given, assume first is held block
            if (bCount == 0)
                blocks[0] = new ExtBlock(args.Block, args.ExtBlock);
        }
        
        public static ExtBlock[] Combine(ExtBlock[] toAffect, int[] count) {
            int sum = 0;
            for (int i = 0; i < count.Length; i++) sum += count[i];
            
            ExtBlock[] blocks = new ExtBlock[sum];
            for (int i = 0, index = 0; i < toAffect.Length; i++) {
                for (int j = 0; j < count[i]; j++)
                    blocks[index++] = toAffect[i];
            }
            return blocks;
        }
    }
}
