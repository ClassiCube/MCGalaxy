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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {   
    public sealed class CmdTorus : DrawCmd {       
        public override string name { get { return "Torus"; } }
        public override string shortcut { get { return "tor"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Donut"), new CommandAlias("Bagel") }; }
        }
        protected override string PlaceMessage { get { return "Place a block for the centre, then another for the radius."; } }
        
        protected override void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) {
            int dx = m[0].X - m[1].X, dy = m[0].Y - m[1].Y, dz = m[0].Z - m[1].Z;
            int horR = (int)Math.Sqrt(dx * dx + dz * dz), verR = Math.Abs(dy);
            
            Vec3S32 p0 = m[0];
            m[0] = new Vec3S32(p0.X - horR, p0.Y - verR, p0.Z - horR);
            m[1] = new Vec3S32(p0.X + horR, p0.Y + verR, p0.Z + horR);
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            return new TorusDrawOp();
        }
        
        public override void Help(Player p) {
            p.Message("&T/Torus <brush args>");
            p.Message("&HDraws a torus(circular tube), with the first point as the centre, " +
                           "and second being the radius.");
            p.Message("   &HNote: radius of the tube itself is the vertical difference between the two points.");            
            p.Message(BrushHelpLine);
        }
    }
}

