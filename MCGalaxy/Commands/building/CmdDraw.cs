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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdDraw : DrawCmd {
        public override string name { get { return "draw"; } }
        public override string shortcut { get { return ""; } }
        protected override string PlaceMessage { get { return "Place a block to determine the origin."; } }
        public override int MarksCount { get { return 1; } }
        
        protected override DrawMode GetMode(string[] parts) {
            string msg = parts[parts.Length - 1];
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
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            AdvDrawOp op = null;
            switch (dArgs.Mode) {
                case DrawMode.cone: op = new AdvConeDrawOp(); break;
                case DrawMode.hcone: op = new AdvHollowConeDrawOp(); break;
                case DrawMode.icone: op = new AdvConeDrawOp(true); break;
                case DrawMode.hicone: op = new AdvHollowConeDrawOp(true); break;
                case DrawMode.pyramid: op = new AdvPyramidDrawOp(); break;
                case DrawMode.hpyramid: op = new AdvHollowPyramidDrawOp(); break;
                case DrawMode.ipyramid: op = new AdvPyramidDrawOp(true); break;
                case DrawMode.hipyramid: op = new AdvHollowPyramidDrawOp(true); break;
                case DrawMode.sphere: op = new AdvSphereDrawOp(); break;
                case DrawMode.hsphere: op = new AdvHollowSphereDrawOp(); break;
                case DrawMode.volcano: op = new AdvVolcanoDrawOp(); break;
            }            
            if (op == null) { Help(dArgs.Player); return null; }
            
            // Validate radius/height when the user first uses the command 
            ushort radius = 0, height = 0;
            string[] args = dArgs.Message.Split(' ');            
            if ((op.UsesHeight && !CheckTwoArgs(dArgs.Player, ref radius, ref height, args)) ||
                (!op.UsesHeight && !CheckOneArg(dArgs.Player, ref radius, args)))
                return null;
            return op;
        }
        
        protected override void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) {
            ushort radius = 0, height = 0;
            string[] args = dArgs.Message.Split(' ');
            AdvDrawOp op = (AdvDrawOp)dArgs.Op;
            
            if ((op.UsesHeight && !CheckTwoArgs(dArgs.Player, ref radius, ref height, args)) ||
                (!op.UsesHeight && !CheckOneArg(dArgs.Player, ref radius, args))) {
                m = null; return;
            }
            
            Vec3S32 P = m[0];
            m = new [] {
                new Vec3S32(P.X - radius, P.Y, P.Z - radius),
                new Vec3S32(P.X + radius, P.Y, P.Z + radius),
            };

            if (op.UsesHeight) {
                m[1].Y += height;
            } else {
                m[0].Y -= radius; m[1].Y += radius;
            }
        }
        
        protected override string GetBrush(Player p, DrawArgs dArgs, ref int offset) {
            offset = ((AdvDrawOp)dArgs.Op).UsesHeight ? 3 : 2;
            return p.BrushName;
        }
        
        bool CheckTwoArgs(Player p, ref ushort radius, ref ushort height, string[] parts) {
            if (parts.Length < 3) { 
                Player.Message(p, "You need to provide the radius and the height for the {0}.", parts[parts.Length - 1]); return false;
            }
            if (!ushort.TryParse(parts[parts.Length - 3], out height) || height > 2000 ||
                !ushort.TryParse(parts[parts.Length - 2], out radius) || radius > 2000) {
                Player.Message(p, "Radius and height must be positive integers less than 2000."); return false;
            }
            return true;
        }
        
        bool CheckOneArg(Player p, ref ushort radius, string[] parts) {
            if (parts.Length < 2) { 
                Player.Message(p, "You need to provide the radius for the {0}.", parts[parts.Length - 1]); return false;
            }
            if (!ushort.TryParse(parts[parts.Length - 2], out radius) || radius > 2000) {
                Player.Message(p, "Radius must be a positive integer less than 2000."); return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/draw <brush args> [height] [baseradius] [mode]");
            Player.Message(p, "%T/draw <brush args> [radius] [mode]");
            Player.Message(p, "%HDraws an object at the specified point.");
            Player.Message(p, "   %HObjects: &fcone/hcone/icone/hicone");
            Player.Message(p, "     &fpyramid/hpyramid/ipyramid/hipyramid/volcano");
            Player.Message(p, "   %HObjects with only radius: &fsphere/hsphere");
            Player.Message(p, "   %HNote 'h' means hollow, 'i' means inverse");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}
