/*
    Copyright 2015 MCGalaxy
        
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
    public sealed class CmdSphere : DrawCmd {
        public override string name { get { return "sphere"; } }
        public override string shortcut { get { return "sp"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("sphereh", null, "hollow"), new CommandAlias("sph", null, "hollow") }; }
        }
        protected override string PlaceMessage { get { return "Place a block for the centre, then another for the radius."; } }
        
        protected override DrawMode ParseMode(string msg) {
            if (msg == "solid") return DrawMode.solid;
            else if (msg == "hollow") return DrawMode.hollow;
            else if (msg == "circle") return DrawMode.circle;
            return DrawMode.normal;
        }

        protected override bool DoDraw(Player p, Vec3S32[] m, object state, byte type, byte extType) {
            DrawArgs cpos = (DrawArgs)state;
            cpos.type = type; cpos.extType = extType;
            
            DrawOp op = null;
            Func<BrushArgs, Brush> constructor = null;
            switch (cpos.mode) {
                case DrawMode.solid:
                    op = new AdvSphereDrawOp();
                    constructor = SolidBrush.Process; break;
                case DrawMode.hollow:
                    op = new AdvHollowSphereDrawOp(); break;
                case DrawMode.circle:
                    op = new EllipsoidDrawOp(); break;
                default:
                    op = new AdvSphereDrawOp(); break;
            }
            int brushOffset = cpos.mode == DrawMode.normal ? 0 : 1;
            Brush brush = GetBrush(p, cpos, brushOffset, constructor);
            if (brush == null) return false;

            Vec3S32 p0 = m[0];
            Vec3S32 radius = GetRadius(cpos.mode, m);
            m[0] = p0 - radius; m[1] = p0 + radius;
            return DrawOp.DoDrawOp(op, brush, p, m);
        }
        
        static Vec3S32 GetRadius(DrawMode mode, Vec3S32[] m) {
            int dx = Math.Abs(m[0].X - m[1].X);
            int dy = Math.Abs(m[0].Y - m[1].Y);
            int dz = Math.Abs(m[0].Z - m[1].Z);

            if (mode != DrawMode.circle) {
                int R = (int)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                return new Vec3S32(R, R, R);
            } else if (dx >= dy && dz >= dy) {
                int R = (int)Math.Sqrt(dx * dx + dz * dz);
                return new Vec3S32(R, 0, R);
            } else if (dz >= dx) {
                int R = (int)Math.Sqrt(dy * dy + dz * dz);
                return new Vec3S32(0, R, R);
            } else {
                int R = (int)Math.Sqrt(dx * dx + dy * dy);
                return new Vec3S32(R, R, 0);                
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/sphere [brush args] <mode>");
            Player.Message(p, "%HCreates a sphere, with the first point as the centre, " +
                           "and second being the radius.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
            Player.Message(p, "   %HModes: &fsolid/hollow/circle");
        }
    }
}
