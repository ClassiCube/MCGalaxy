/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdDraw : DrawCmd {
        public override string name { get { return "Draw"; } }
        
        protected override int MarksCount { get { return 1; } }
        protected override string SelectionType { get { return "origin"; } }
        protected override string PlaceMessage { get { return "Place a block to determine the origin."; } }
        
        protected override DrawMode GetMode(string[] parts) {
            string msg = parts[0];
            if (msg == "cone")      return DrawMode.cone;
            if (msg == "hcone")     return DrawMode.hcone;
            if (msg == "icone")     return DrawMode.icone;
            if (msg == "hicone")    return DrawMode.hicone;
            if (msg == "pyramid")   return DrawMode.pyramid;
            if (msg == "hpyramid")  return DrawMode.hpyramid;
            if (msg == "ipyramid")  return DrawMode.ipyramid;
            if (msg == "hipyramid") return DrawMode.hipyramid;
            if (msg == "sphere")    return DrawMode.sphere;
            if (msg == "hsphere")   return DrawMode.hsphere;
            if (msg == "volcano")   return DrawMode.volcano;
            if (msg == "cylinder")  return DrawMode.hollow;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            DrawOp op = null;
            switch (dArgs.Mode) {
                case DrawMode.cone:   op = new ConeDrawOp(); break;
                case DrawMode.hcone:  op = new AdvHollowConeDrawOp(); break;
                case DrawMode.icone:  op = new ConeDrawOp(true); break;
                case DrawMode.hicone: op = new AdvHollowConeDrawOp(true); break;
                case DrawMode.pyramid:   op = new AdvPyramidDrawOp(); break;
                case DrawMode.hpyramid:  op = new AdvHollowPyramidDrawOp(); break;
                case DrawMode.ipyramid:  op = new AdvPyramidDrawOp(true); break;
                case DrawMode.hipyramid: op = new AdvHollowPyramidDrawOp(true); break;
                case DrawMode.sphere:  op = new AdvSphereDrawOp(); break;
                case DrawMode.hsphere: op = new AdvHollowSphereDrawOp(); break;
                case DrawMode.volcano: op = new AdvVolcanoDrawOp(); break;
                case DrawMode.hollow:  op = new CylinderDrawOp(); break;
            }
            if (op == null) { Help(dArgs.Player); return null; }

            AdvDrawMeta meta = new AdvDrawMeta();
            bool success = false;
            string[] args = dArgs.Message.SplitSpaces();
            Player p = dArgs.Player;
            
            if (UsesHeight(dArgs)) {
                if (args.Length < 3) {
                    p.Message("You need to provide the radius and the height for the {0}.", args[0]);
                } else {
                    success = CommandParser.GetInt(p, args[1], "radius", ref meta.radius, 0, 2000)
                        && CommandParser.GetInt(p, args[2], "height", ref meta.height, 0, 2000);
                }
            } else {
                if (args.Length < 2) {
                    p.Message("You need to provide the radius for the {0}.", args[0]);
                } else {
                    success = CommandParser.GetInt(p, args[1], "radius", ref meta.radius, 0, 2000);
                }
            }
            
            if (!success) return null;
            dArgs.Meta = meta;
            return op;
        }
        
        protected override void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) {
            AdvDrawMeta meta = (AdvDrawMeta)dArgs.Meta;
            int radius = meta.radius;
            
            Vec3S32 P = m[0];
            m = new Vec3S32[] {
                new Vec3S32(P.X - radius, P.Y, P.Z - radius),
                new Vec3S32(P.X + radius, P.Y, P.Z + radius),
            };

            if (UsesHeight(dArgs)) {
                m[1].Y += meta.height - 1;
            } else {
                m[0].Y -= radius; m[1].Y += radius;
            }
        }
        
        protected override void GetBrush(DrawArgs dArgs) {
            int argsUsed = UsesHeight(dArgs) ? 3 : 2;
            dArgs.BrushArgs = dArgs.Message.Splice(argsUsed, 0);
        }
        
        class AdvDrawMeta { public int radius, height; }

        static bool UsesHeight(DrawArgs args) {
            DrawMode mode = args.Mode;
            return !(mode == DrawMode.sphere || mode == DrawMode.hsphere);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Draw [object] [baseradius] [height] <brush args>");
            p.Message("&T/Draw [object] [radius] <brush args>");
            p.Message("&HDraws an object at the specified point.");
            p.Message("   &HObjects: &fcone/hcone/icone/hicone/cylinder/");
            p.Message("     &fpyramid/hpyramid/ipyramid/hipyramid/volcano");
            p.Message("   &HObjects with only radius: &fsphere/hsphere");
            p.Message("   &HNote 'h' means hollow, 'i' means inverse");
            p.Message(BrushHelpLine);
        }
    }
}
