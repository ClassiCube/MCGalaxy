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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {    
    public sealed class CmdDraw : DrawCmd {
        public override string name { get { return "draw"; } }
        public override string shortcut { get { return ""; } }     
        protected override string PlaceMessage { get { return "Place a block to determine the origin."; } }
        public override int MarksCount { get { return 1; } }
        
        protected override DrawMode ParseMode(string msg) {
            if (msg == "cone") return DrawMode.cone;
            else if (msg == "hcone") return DrawMode.hcone;
            else if (msg == "icone") return DrawMode.icone;
            else if (msg == "hicone") return DrawMode.hicone;
            else if (msg == "pyramid") return DrawMode.pyramid;
            else if (msg == "hpyramid") return DrawMode.hpyramid;
            else if (msg == "ipyramid") return DrawMode.ipyramid;
            else if (msg == "hipyramid") return DrawMode.hipyramid;
            else if (msg == "sphere") return DrawMode.sphere;
            else if (msg == "hsphere") return DrawMode.hsphere;
            else if (msg == "volcano") return DrawMode.volcano;
            return DrawMode.normal;
        }
        
        protected override bool DoDraw(Player p, Vec3S32[] m, object state, byte type, byte extType) {
            CatchPos cpos = (CatchPos)state;
            GetRealBlock(type, extType, p, ref cpos);
            AdvDrawOp op = null;
            switch (cpos.mode) {
                case DrawMode.cone:
                    op = new AdvConeDrawOp(); break;
                case DrawMode.hcone:
                    op = new AdvHollowConeDrawOp(); break;
                case DrawMode.icone:
                    op = new AdvConeDrawOp(); op.Invert = true; break;
                case DrawMode.hicone:
                    op = new AdvHollowConeDrawOp(); op.Invert = true; break;
                case DrawMode.pyramid:
                    op = new AdvPyramidDrawOp(); break;
                case DrawMode.hpyramid:
                    op = new AdvHollowPyramidDrawOp(); break;
                case DrawMode.ipyramid:
                    op = new AdvPyramidDrawOp(); op.Invert = true; break;
                case DrawMode.hipyramid:
                    op = new AdvHollowPyramidDrawOp(); op.Invert = true; break;
                case DrawMode.sphere:
                    op = new AdvSphereDrawOp(); op.Invert = true; break;
                case DrawMode.hsphere:
                    op = new AdvHollowSphereDrawOp(); op.Invert = true; break;
                case DrawMode.volcano:
                    op = new AdvVolcanoDrawOp(); break;
                default:
                    Help(p); return false;
            }
            ushort radius = 0, height = 0;
            string[] args = cpos.message.Split(' ');
            if ((op.UsesHeight && !CheckTwoArgs(p, ref radius, ref height, args)) || 
                (!op.UsesHeight && !CheckOneArg(p, ref radius, args))) return false;
            
            int brushOffset = op.UsesHeight ? 3 : 2;
            Brush brush = GetBrush(p, cpos, brushOffset);
            if (brush == null) return false;
            
            Vec3S32[] marks = {
            	new Vec3S32(m[0].X - radius, m[0].Y, m[0].Z - radius),
            	new Vec3S32(m[0].X + radius, m[0].Y, m[0].Z + radius) };
            if (op.UsesHeight) {
                marks[1].Y += height;
            } else {
                marks[0].Y -= radius; marks[1].Y += radius;
            }           
            return DrawOp.DoDrawOp(op, brush, p, marks);
        }
        
        bool CheckTwoArgs(Player p, ref ushort radius, ref ushort height, string[] parts) {
            if (parts.Length < 3) { Help(p); return false; }
            if (!ushort.TryParse(parts[parts.Length - 3], out height) || height > 2000 ||
                !ushort.TryParse(parts[parts.Length - 2], out radius) || radius > 2000) {
                Player.Message(p, "Radius and height must be positive integers less than 2000."); return false;
            }
            return true;
        }
        
        bool CheckOneArg(Player p, ref ushort radius, string[] parts) {
            if (parts.Length < 2) { Help(p); return false; }
            if (!ushort.TryParse(parts[parts.Length - 2], out radius) || radius > 2000) { 
                Player.Message(p, "Radius must be a positive integer less than 2000."); return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/draw [brush args] <height> <baseradius> <mode>");
            Player.Message(p, "%T/draw [brush args] <radius> <mode>");
            Player.Message(p, "%HDraws an object at the specified point.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
            Player.Message(p, "   %HObjects: &fcone/hcone/icone/hicone");
            Player.Message(p, "     &fpyramid/hpyramid/ipyramid/hipyramid/volcano");
            Player.Message(p, "   %HObjects with only radius: &fsphere/hsphere");
            Player.Message(p, "   %HNote 'h' means hollow, 'i' means inverse");
        }
    }
}
