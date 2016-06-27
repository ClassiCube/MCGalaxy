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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {    
    public sealed class CmdLine : DrawCmd {      
        public override string name { get { return "line"; } }
        public override string shortcut { get { return "l"; } }      
        protected override string PlaceMessage { get { return "Place two blocks to determine the endpoints."; } }
        
        protected override DrawMode ParseMode(string msg) {
            if (msg == "normal") return DrawMode.solid;
            else if (msg == "walls") return DrawMode.walls;
            else if (msg == "straight") return DrawMode.straight;
            return DrawMode.normal;
        }
        
        protected override void OnUse(Player p, string msg, string[] parts, ref DrawArgs cpos) {
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
            if (parts.Length == 1 || !ushort.TryParse(parts[parts.Length - 1], out len)) 
                return DrawMode.normal;
            return ParseMode(parts[parts.Length - 2]);
        }

        protected override bool DoDraw(Player p, Vec3S32[] m, object state, byte type, byte extType) {
            DrawArgs cpos = (DrawArgs)state;
            GetRealBlock(type, extType, p, ref cpos);

            if (cpos.mode == DrawMode.straight) { 
            	int dx = Math.Abs(m[0].X - m[1].X), dy = Math.Abs(m[0].Y - m[1].Y), dz = Math.Abs(m[0].Z - m[1].Z);

                if (dx > dy && dx > dz) {
            		m[1].Y = m[0].Y; m[1].Z = m[0].Z;
                } else if (dy > dx && dy > dz) {
                    m[1].X = m[0].X; m[1].Z = m[0].Z;
                } else if (dz > dy && dz > dx) {
                    m[1].X = m[0].X; m[1].Y = m[0].Y;
                }
            }
            
            LineDrawOp drawOp = new LineDrawOp();
            drawOp.WallsMode = cpos.mode == DrawMode.walls;
            int brushOffset = cpos.mode == DrawMode.normal ? 0 : 1;
            if (cpos.data != null) {
                drawOp.MaxLength = (ushort)cpos.data; brushOffset++;
            }
            Brush brush = GetBrush(p, cpos, brushOffset);
            if (brush == null) return false;
            return DrawOp.DoDrawOp(drawOp, brush, p, m);
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
