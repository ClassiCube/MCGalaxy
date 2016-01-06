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
            Brush brush = null;

            switch (cpos.solid) {
                case SolidType.solid:
                    drawOp = new CuboidDrawOp(); break;
                case SolidType.hollow:
                    drawOp = new CuboidHollowsDrawOp(); break;
                case SolidType.walls:
                    drawOp = new CuboidWallsDrawOp(); break;
                case SolidType.holes:
                    drawOp = new CuboidHolesDrawOp(); break;
                case SolidType.wire:
                    drawOp = new CuboidWireframeDrawOp(); break;
                case SolidType.random:
                    drawOp = new CuboidDrawOp();
                    brush = new RandomBrush(cpos.type, cpos.extType); break;
            }
            
            if (brush == null) brush = new SolidBrush(cpos.type, cpos.extType);
            ushort x1 = Math.Min(cpos.x, x), x2 = Math.Max(cpos.x, x);
            ushort y1 = Math.Min(cpos.y, y), y2 = Math.Max(cpos.y, y);
            ushort z1 = Math.Min(cpos.z, z), z2 = Math.Max(cpos.z, z);            
            if (!DrawOp.DoDrawOp(drawOp, brush, p, x1, y1, z1, x2, y2, z2))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected override SolidType GetType(string msg) {
            if (msg == "solid")
                return SolidType.solid;
            else if (msg == "hollow")
                return SolidType.hollow;
            else if (msg == "walls")
                return SolidType.walls;
            else if (msg == "holes")
                return SolidType.holes;
            else if (msg == "wire")
                return SolidType.wire;
            else if (msg == "random")
                return SolidType.random;
            return SolidType.Invalid;
        }
        
        public override void Help(Player p) {
        	Player.SendMessage(p, "%T/cuboid [block type] <mode>");
        	Player.SendMessage(p, "   %HCreates a cuboid between two points.");
        	Player.SendMessage(p, "   %HMode can be: solid/hollow/walls/holes/wire/random");
        }
    }
}
