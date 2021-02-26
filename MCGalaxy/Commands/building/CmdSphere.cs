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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdSphere : DrawCmd {
        public override string name { get { return "Sphere"; } }
        public override string shortcut { get { return "sp"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("SphereH", "hollow"),
                    new CommandAlias("sph", "hollow"), new CommandAlias("Circle", "circle" ),
                    new CommandAlias("CircleH", "hollowcircle") }; }
        }
        protected override string PlaceMessage { get { return "Place a block for the centre, then another for the radius."; } }
        
        protected override DrawMode GetMode(string[] parts) {
            string msg = parts[0];
            if (msg == "solid")        return DrawMode.solid;
            if (msg == "hollow")       return DrawMode.hollow;
            if (msg == "circle")       return DrawMode.circle;
            if (msg == "hollowcircle") return DrawMode.hcircle;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            switch (dArgs.Mode) {
                case DrawMode.hollow:  return new AdvHollowSphereDrawOp();
                case DrawMode.circle:  return new EllipsoidDrawOp();
                case DrawMode.hcircle: return new EllipsoidHollowDrawOp();
            }
            return new AdvSphereDrawOp();
        }
        
        protected override void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) {
            Vec3S32 p0 = m[0];
            Vec3S32 radius = GetRadius(dArgs.Mode, m);
            m[0] = p0 - radius; m[1] = p0 + radius;
        }
        
        
        protected override void GetBrush(DrawArgs dArgs) {
            if (dArgs.Mode == DrawMode.solid) dArgs.BrushName = "Normal";
            dArgs.BrushArgs = dArgs.Message.Splice(dArgs.ModeArgsCount, 0);
        }
        
        static Vec3S32 GetRadius(DrawMode mode, Vec3S32[] m) {
            int dx = Math.Abs(m[0].X - m[1].X);
            int dy = Math.Abs(m[0].Y - m[1].Y);
            int dz = Math.Abs(m[0].Z - m[1].Z);

            bool circle = mode == DrawMode.circle || mode == DrawMode.hcircle;
            if (!circle) {
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
            p.Message("&T/Sphere <brush args>");
            p.Message("&HCreates a sphere, with first point as centre, and second for radius");
            p.Message("&T/Sphere [mode] <brush args>");
            p.Message("&HModes: &fsolid/hollow/circle/hollowcircle");
            p.Message(BrushHelpLine);
        }
    }
}
