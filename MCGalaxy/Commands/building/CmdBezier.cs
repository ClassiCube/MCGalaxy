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

namespace MCGalaxy.Commands.Building {
    public sealed class CmdBezier : DrawCmd {
        public override string name { get { return "Bezier"; } }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("Curve") }; }
        }
        
        protected override int MarksCount { get { return 3; } }
        protected override string SelectionType { get { return "points"; } }
        protected override string PlaceMessage { get { return "Place or break two blocks to determine the endpoints, then another for the control point"; } }

        protected override DrawOp GetDrawOp(DrawArgs dArgs) { return new BezierDrawOp(); }
        
        public override void Help(Player p) {
            p.Message("&T/Bezier <brush args>");
            p.Message("&HDraws a quadratic bezier curve.");
            p.Message("&HFirst two points specify the endpoints, then another point specifies the control point.");
            p.Message(BrushHelpLine);
        }
    }
}
