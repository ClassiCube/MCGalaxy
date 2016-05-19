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

namespace MCGalaxy.Commands {
    
    public sealed class CmdDraw : DrawCmd {
        public override string name { get { return "draw"; } }
        public override string shortcut { get { return ""; } }     
        protected override string PlaceMessage { get { return "Place a block to determine the origin."; } }
        
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
        
        protected override void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
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
                    Help(p); return;
            }
            string[] args = cpos.message.Split(' ');
            if ((op.UsesHeight && !CheckTwoArgs(p, op, args)) || 
                (!op.UsesHeight && !CheckOneArg(p, op, args))) return;
            
            int brushOffset = op.UsesHeight ? 3 : 2;
            Brush brush = GetBrush(p, cpos, brushOffset);
            if (brush == null) return;
            
            Vec3S32[] marks = {
            	new Vec3S32(x - op.Radius, y, z - op.Radius),
                new Vec3S32(x + op.Radius, y, z + op.Radius) };
            if (op.UsesHeight) {
                marks[1].Y += op.Height;
            } else {
                marks[0].Y -= op.Radius; 
                marks[1].Y += op.Radius;
            }
            
            if (!DrawOp.DoDrawOp(op, brush, p, marks))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) { }
        
        bool CheckTwoArgs(Player p, AdvDrawOp op, string[] parts) {
            if (parts.Length < 3) { Help(p); return false; }
            if (!ushort.TryParse(parts[parts.Length - 3], out op.Height) || op.Height > 2000 ||
                !ushort.TryParse(parts[parts.Length - 2], out op.Radius) || op.Radius > 2000) {
                Player.Message(p, "Radius and height must be positive integers less than 2000."); return false;
            }
            return true;
        }
        
        bool CheckOneArg(Player p, AdvDrawOp op, string[] parts) {
            if (parts.Length < 2) { Help(p); return false; }
            if (!ushort.TryParse(parts[parts.Length - 2], out op.Radius) || op.Radius > 2000) { 
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
