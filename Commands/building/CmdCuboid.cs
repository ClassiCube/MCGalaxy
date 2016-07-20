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

namespace MCGalaxy.Commands.Building {
    public sealed class CmdCuboid : DrawCmd {
        public override string name { get { return "cuboid"; } }
        public override string shortcut { get { return "z"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("cw", null, "wire"),
                    new CommandAlias("ch", null, "hollow"), new CommandAlias("walls", null, "walls"),
                    new CommandAlias("box"), new CommandAlias("hbox", null, "hollow") }; }
        }

        protected override bool DoDraw(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            DrawArgs cpos = (DrawArgs)state;
            cpos.block = type; cpos.extBlock = extType;
            DrawOp op = null;
            Func<BrushArgs, Brush> constructor = null;

            switch (cpos.mode) {
                case DrawMode.solid:
                    op = new CuboidDrawOp();
                    constructor = SolidBrush.Process; break;
                case DrawMode.normal:
                    op = new CuboidDrawOp(); break;
                case DrawMode.hollow:
                    op = new CuboidHollowsDrawOp(); break;
                case DrawMode.walls:
                    op = new CuboidWallsDrawOp(); break;
                case DrawMode.holes:
                    op = new CuboidDrawOp(); 
                    constructor = CheckeredBrush.Process; break;
                case DrawMode.wire:
                    op = new CuboidWireframeDrawOp(); break;
                case DrawMode.random:
                    op = new CuboidDrawOp();
                    constructor = RandomBrush.Process; break;
            }
            
            int brushOffset = cpos.mode == DrawMode.normal ? 0 : 1;
            Brush brush = GetBrush(p, cpos, brushOffset, constructor);
            if (brush == null) return false;
            return DrawOp.DoDrawOp(op, brush, p, marks);
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
            Player.Message(p, "%T/cuboid [brush args] <mode>");
            Player.Message(p, "%HDraws a cuboid between two points.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
            Player.Message(p, "   %HModes: &fsolid/hollow/walls/holes/wire/random");
        }
    }
}
