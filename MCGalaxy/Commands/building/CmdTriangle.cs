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
    public sealed class CmdTriangle : DrawCmd {
        public override string name { get { return "Triangle"; } }
        public override string shortcut { get { return "tri"; } }
        
        protected override int MarksCount { get { return 3; } }       
        protected override string PlaceMessage { get { return "Place three blocks to determine the edges."; } }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) { return new TriangleDrawOp(); }
        
        public override void Help(Player p) {
            p.Message("&T/Triangle <brush args>");
            p.Message("&HDraws a triangle between three points.");
            p.Message(BrushHelpLine);
        }
    }
}