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

namespace MCGalaxy.Drawing.Brushes {
    
    public abstract class FrequencyBrush : Brush {
        
        protected static ExtBlock[] GetBlocks(Player p, string[] parts,
                                              int[] count, Predicate<string> filter,
                                              Predicate<string> handler) {
            ExtBlock[] blocks = new ExtBlock[parts.Length];
            for (int i = 0; i < blocks.Length; i++) {
                blocks[i].Type = Block.Zero;
                count[i] = filter(parts[i]) ? 1 : 0;
            }
            
            for (int i = 0; i < parts.Length; i++ ) {
                // Brush specific args
                if (!filter(parts[i])) {
                    if (!handler(parts[i])) return null;
                    continue;
                }
                
                byte extType = 0;
                int sepIndex = parts[i].IndexOf('/');
                string block = sepIndex >= 0 ? parts[i].Substring(0, sepIndex) : parts[i];
                byte type = DrawCmd.GetBlock(p, block, out extType);
                if (type == Block.Zero) return null;
                
                blocks[i].Type = type; blocks[i].ExtType = extType;
                if (sepIndex < 0) continue;
                int chance;
                if (!int.TryParse(parts[i].Substring(sepIndex + 1), out chance)
                    || chance <= 0 || chance > 10000) {
                    Player.Message(p, "frequency must be an integer between 1 and 10,000."); return null;
                }
                count[i] = chance;
            }
            return blocks;
        }
        
        protected static ExtBlock[] Combine(ExtBlock[] toAffect, int[] count) {
            int sum = 0;
            for (int i = 0; i < count.Length; i++) sum += count[i];
            if (toAffect.Length == 1) sum += 1;
            
            ExtBlock[] blocks = new ExtBlock[sum];
            for (int i = 0, index = 0; i < toAffect.Length; i++) {
                for (int j = 0; j < count[i]; j++)
                    blocks[index++] = toAffect[i];
            }
            // For one block argument, leave every other block untouched.
            if (toAffect.Length == 1)
                blocks[blocks.Length - 1] = new ExtBlock(Block.Zero, 0);
            return blocks;
        }
    }
}
