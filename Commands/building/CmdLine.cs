/*
    Copyright 2011 MCGalaxy
        
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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Commands {
    
    public sealed class CmdLine : DrawCmd {
        
        public override string name { get { return "line"; } }
        public override string shortcut { get { return "l"; } }
        public CmdLine() {}
        
        protected override int MaxArgs { get { return 3; } }
        
        protected override SolidType GetType(string msg) {
            if (msg == "walls")
                return SolidType.walls;
            else if (msg == "straight")
                return SolidType.straight;
            return SolidType.solid;
        }
        
        protected override void OnUse(Player p, string msg, string[] parts, ref CatchPos cpos) {
            if (parts.Length == 2 || parts.Length == 3) {
                string arg = parts[parts.Length - 1];
                ushort len;
                if (!ushort.TryParse(arg, out len)) {
                    if (arg == "walls" || arg == "straight" || arg == "normal") return;                
                    Player.SendMessage(p, msg + " is not valid length, assuming maximum length allowed.");
                } else {
                    cpos.data = len;
                }
            } 
        }

        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            GetRealBlock(type, extType, p, ref cpos);

            if (cpos.solid == SolidType.straight) { 
                int dx = Math.Abs(cpos.x - x);
                int dy = Math.Abs(cpos.y - y);
                int dz = Math.Abs(cpos.z - z);

                if (dx > dy && dx > dz) {
                    y = cpos.y; z = cpos.z;
                } else if (dy > dx && dy > dz) {
                    x = cpos.x; z = cpos.z;
                } else if (dz > dy && dz > dx) {
                    y = cpos.y; x = cpos.x;
                }
            }
            
            LineDrawOp drawOp = new LineDrawOp();
            drawOp.WallsMode = cpos.solid == SolidType.walls;
            if (cpos.data != null)
                drawOp.MaxLength = (ushort)cpos.data;
            Brush brush = new SolidBrush(cpos.type, cpos.extType);
                      
            if (!DrawOp.DoDrawOp(drawOp, brush, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/line [block type] <mode> <length>");
            Player.SendMessage(p, "   %HCreates a line between two points.");
            Player.SendMessage(p, "   %HMode can be: normal/walls/straight");
            Player.SendMessage(p, "   %HLength specifies the max number of blocks in the line.");
        }
    }
}
