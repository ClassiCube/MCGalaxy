/*
    Copyright 2011 MCForge
        
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
        
        protected override DrawMode ParseMode(string msg) {
            if (msg == "normal") return DrawMode.solid;
            else if (msg == "walls") return DrawMode.walls;
            else if (msg == "straight") return DrawMode.straight;
            return DrawMode.normal;
        }
        
        protected override void OnUse(Player p, string msg, string[] parts, ref CatchPos cpos) {
            if (parts.Length < 2 || cpos.mode == DrawMode.normal) return;
            string arg = parts[parts.Length - 1];
            ushort len;
            if (ushort.TryParse(arg, out len))
                cpos.data = len;
        }
        
        protected override DrawMode GetMode(string message, string[] parts) {
            if (message == "") return DrawMode.normal;
            DrawMode mode = ParseMode(parts[parts.Length - 1]);
            if (mode != DrawMode.normal) return mode;
            
            // May be in the format <brush args> <mode> <max_length>
            ushort len;
            if (!ushort.TryParse(parts[parts.Length - 1], out len)) return DrawMode.normal;
            return ParseMode(parts[parts.Length - 2]);
        }

        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            GetRealBlock(type, extType, p, ref cpos);

            if (cpos.mode == DrawMode.straight) { 
                int dx = Math.Abs(cpos.x - x), dy = Math.Abs(cpos.y - y), dz = Math.Abs(cpos.z - z);

                if (dx > dy && dx > dz) {
                    y = cpos.y; z = cpos.z;
                } else if (dy > dx && dy > dz) {
                    x = cpos.x; z = cpos.z;
                } else if (dz > dy && dz > dx) {
                    y = cpos.y; x = cpos.x;
                }
            }
            
            LineDrawOp drawOp = new LineDrawOp();
            drawOp.WallsMode = cpos.mode == DrawMode.walls;
            int brushOffset = cpos.mode == DrawMode.normal ? 0 : 1;
            if (cpos.data != null) {
                drawOp.MaxLength = (ushort)cpos.data; brushOffset++;
            }
            Brush brush = GetBrush(p, cpos, brushOffset);
            if (brush == null) return;
                      
            if (!DrawOp.DoDrawOp(drawOp, brush, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/line [brush args] <mode> <length>");
            Player.Message(p, "%HCreates a line between two points.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");            
            Player.Message(p, "   %HModes: &fnormal/walls/straight");
            Player.Message(p, "   %HLength specifies the max number of blocks in the line.");
        }
    }
}
