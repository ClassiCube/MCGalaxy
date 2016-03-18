/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Commands
{
    public sealed class CmdCuboid : DrawCmd
    {
        public override string name { get { return "cuboid"; } }
        public override string shortcut { get { return "z"; } }

        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            GetRealBlock(type, extType, p, ref cpos);
            DrawOp drawOp = null;
            Func<BrushArgs, Brush> constructor = null;

            switch (cpos.mode) {
                case DrawMode.solid:
                    drawOp = new CuboidDrawOp();
                    constructor = SolidBrush.Process; break;
                case DrawMode.normal:
                    drawOp = new CuboidDrawOp(); break;
                case DrawMode.hollow:
                    drawOp = new CuboidHollowsDrawOp(); break;
                case DrawMode.walls:
                    drawOp = new CuboidWallsDrawOp(); break;
                case DrawMode.holes:
                    drawOp = new CuboidDrawOp(); 
                    constructor = CheckeredBrush.Process; break;
                case DrawMode.wire:
                    drawOp = new CuboidWireframeDrawOp(); break;
                case DrawMode.random:
                    drawOp = new CuboidDrawOp();
                    constructor = RandomBrush.Process; break;
            }
            
            int brushOffset = cpos.mode == DrawMode.normal ? 0 : 1;
            Brush brush = GetBrush(p, cpos, brushOffset, constructor);
            if (brush == null) return;
            if (!DrawOp.DoDrawOp(drawOp, brush, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected override DrawMode ParseMode(string msg) {
            if (msg == "solid") return DrawMode.solid;
            else if (msg == "hollow") return DrawMode.hollow;
            else if (msg == "walls") return DrawMode.walls;
            else if (msg == "holes") return DrawMode.holes;
            else if (msg == "wire") return DrawMode.wire;
            else if (msg == "random") return DrawMode.random;
            return DrawMode.normal;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/cuboid [brush args] <mode>");
            Player.SendMessage(p, "%HDraws a cuboid between two points.");
            Player.SendMessage(p, "   %HFor help about brushes, type %T/help brush%H.");
            Player.SendMessage(p, "   %HMode can be: solid/hollow/walls/holes/wire/random");
        }
    }
}
