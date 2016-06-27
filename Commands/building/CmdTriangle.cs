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
	public sealed class CmdTriangle : DrawCmd {
        public override string name { get { return "triangle"; } }
        public override string shortcut { get { return "tri"; } }
        public override int MarksCount { get { return 3; } }
		protected override string PlaceMessage {
			get { return "Place three blocks to determine the edges."; }
		} 

        protected override bool DoDraw(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            DrawArgs cpos = (DrawArgs)state;
            cpos.type = type; cpos.extType = extType;
            
            Brush brush = GetBrush(p, cpos, 0, null);
            if (brush == null) return false;
            return DrawOp.DoDrawOp(new TriangleDrawOp(), brush, p, marks);
        }
        
        protected override DrawMode ParseMode(string msg) { return DrawMode.normal; }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/triangle [brush args]");
            Player.Message(p, "%HDraws a triangle between three points.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}
