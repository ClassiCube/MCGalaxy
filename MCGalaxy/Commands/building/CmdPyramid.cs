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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPyramid : DrawCmd {
        public override string name { get { return "Pyramid"; } }
        public override string shortcut { get { return "pd"; } }
        
        protected override DrawMode GetMode(string[] parts) {
            string mode = parts[0];
            if (mode == "solid")   return DrawMode.solid;
            if (mode == "hollow")  return DrawMode.hollow;
            if (mode == "reverse") return DrawMode.reverse;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            switch (dArgs.Mode) {
                case DrawMode.hollow: return new PyramidHollowDrawOp();
                case DrawMode.reverse: return new PyramidReverseDrawOp();
            }
            return new PyramidSolidDrawOp();
        }
        
        public override void Help(Player p) {
            p.Message("&T/Pyramid <brush args>");
            p.Message("&HDraws a square pyramid, using two points for the base.");
            p.Message("&T/Pyramid [mode] <brush args>");
            p.Message("&HModes: &fsolid/hollow/reverse");
            p.Message(BrushHelpLine);
        }
    }
}
