/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdOutline : DrawCmd {
        public override string name { get { return "outline"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            if (dArgs.Message.Length == 0) {
                Player.Message(dArgs.Player, "Block name is required."); return null;
            }
            
            ExtBlock target;
            string[] parts = dArgs.Message.SplitSpaces(2);
            if (!CommandParser.GetBlockIfAllowed(dArgs.Player, parts[0], out target)) return null;
            
            OutlineDrawOp op = new OutlineDrawOp();
            op.Target = target;
            return op;
        }
        
        protected override string GetBrushArgs(DrawArgs dArgs, int usedFromEnd) {
            string[] parts = dArgs.Message.SplitSpaces(2);
            return parts.Length == 1 ? "" : parts[1];
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/outline [block] <brush args>");
            Player.Message(p, "%HOutlines [block] with output of your current brush.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}
