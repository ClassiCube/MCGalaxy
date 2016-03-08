/*
    Copyright 2011 MCGalaxy
        
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

namespace MCGalaxy.Commands {
    
    public sealed class CmdSpheroid : DrawCmd {
        
        public override string name { get { return "spheroid"; } }
        public override string shortcut { get { return "e"; } }

        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            GetRealBlock(type, extType, p, ref cpos);
            DrawOp drawOp = null;
            Brush brush = new SolidBrush(cpos.type, cpos.extType);

            switch (cpos.solid) {
                case SolidType.solid:
                    drawOp = new EllipsoidDrawOp(); break;
                case SolidType.hollow:
                    drawOp = new EllipsoidHollowDrawOp(); break;
                case SolidType.vertical:
                    drawOp = new CylinderDrawOp(); break;
            }
                      
            if (!DrawOp.DoDrawOp(drawOp, brush, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected override SolidType GetType(string msg) {
            if (msg == "solid") return SolidType.solid;
            else if (msg == "hollow") return SolidType.hollow;
            else if (msg == "vertical") return SolidType.vertical;
            return SolidType.Invalid;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/spheroid [type] <solid/hollow/vertical> - create a spheroid of blocks.");
            Player.SendMessage(p, "If <vertical> is used, a vertical tube will be created");
        }
    }
}

