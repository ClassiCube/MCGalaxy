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

namespace MCGalaxy.Commands
{
    public sealed class CmdPyramid : DrawCmd
    {
        public override string name { get { return "pyramid"; } }
        public override string shortcut { get { return "pd"; } }

        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            GetRealBlock(type, extType, p, ref cpos);
            DrawOp drawOp = null;
            int brushOffset = cpos.mode == DrawMode.normal ? 0 : 1;
            Brush brush = GetBrush(p, cpos, brushOffset);
            if (brush == null) return;
            
            if (y != cpos.y) {
                Player.SendMessage(p, "The two edges of the pyramid must be on the same level");
                return;
            }

            switch (cpos.mode) {
                case DrawMode.solid:
                case DrawMode.normal:
                    drawOp = new PyramidSolidDrawOp(); break;
                case DrawMode.hollow:
                    drawOp = new PyramidHollowDrawOp(); break;
                case DrawMode.reverse:
                    drawOp = new PyramidReverseDrawOp(); break;
            }
            
            if (!DrawOp.DoDrawOp(drawOp, brush, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected override DrawMode ParseMode(string msg) {
        	if (msg == "solid") return DrawMode.solid;
            else if (msg == "hollow") return DrawMode.hollow;
            else if (msg == "reverse") return DrawMode.reverse;
            return DrawMode.normal;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/pyramid [type] <solid/hollow/reverse> - create a square pyramid of blocks.");
        }
    }
}
