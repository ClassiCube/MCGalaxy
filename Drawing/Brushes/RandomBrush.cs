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
    
    public sealed class Random2Brush : Brush {
        readonly ExtBlock[] blocks;
        readonly Random rnd;
        
        public Random2Brush(ExtBlock[] blocks) {
            this.blocks = blocks;
            rnd = new Random();
        }
        
        public override string Name { get { return "Random"; } }
        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1] [block2]..",
            "%HDraws by randomly selecting blocks from the given [blocks].",
        };
        
        public static Brush Process(BrushArgs args) {
            string[] parts = args.Message.Split(' ');
            ExtBlock[] toAffect = GetBlocks(args.Player, parts.Length, parts);
            return new Random2Brush(toAffect);
        }
        
        internal static ExtBlock[] GetBlocks(Player p, int max, string[] parts) {
            ExtBlock[] blocks = new ExtBlock[max];
            for (int i = 0; i < blocks.Length; i++)
                blocks[i].Type = Block.Zero;
            for (int i = 0; i < max; i++ ) {
                byte extType = 0;
                byte type = DrawCmd.GetBlock(p, parts[i], out extType);
                if (type == Block.Zero) continue;
                blocks[i].Type = type; blocks[i].ExtType = extType;
            }
            return blocks;
        }
        
        int next;
        public override byte NextBlock(DrawOp op) {
        	next = rnd.Next() % blocks.Length;
            return blocks[next].Type;
        }
        
        public override byte NextExtBlock(DrawOp op) {
        	return blocks[next].ExtType;
        }
    }
}
