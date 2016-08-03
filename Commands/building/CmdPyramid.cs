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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPyramid : DrawCmd {
        public override string name { get { return "pyramid"; } }
        public override string shortcut { get { return "pd"; } }

        protected override bool DoDraw(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            DrawArgs cpos = (DrawArgs)state;
            cpos.block = type; cpos.extBlock = extType;
            DrawOp op = null;
            int brushOffset = cpos.mode == DrawMode.normal ? 0 : 1;
            Brush brush = GetBrush(p, cpos, brushOffset);
            if (brush == null) return false;
            
            if (marks[0].Y != marks[1].Y) {
                Player.Message(p, "The two edges of the pyramid must be on the same level"); return false;
            }

            switch (cpos.mode) {
                case DrawMode.solid:
                case DrawMode.normal:
                    op = new PyramidSolidDrawOp(); break;
                case DrawMode.hollow:
                    op = new PyramidHollowDrawOp(); break;
                case DrawMode.reverse:
                    op = new PyramidReverseDrawOp(); break;
            }           
            return DrawOp.DoDrawOp(op, brush, p, marks);
        }
        
        protected override DrawMode ParseMode(string msg) {
            if (msg == "solid") return DrawMode.solid;
            else if (msg == "hollow") return DrawMode.hollow;
            else if (msg == "reverse") return DrawMode.reverse;
            return DrawMode.normal;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/pyramid [brush args] <mode>");
            Player.Message(p, "%HDraws a square pyramid, using two points for the base.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
            Player.Message(p, "   %HModes: &fsolid/hollow/reverse");            
        }
    }
}
