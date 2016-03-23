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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Commands {
	
	/*public sealed class CmdTriangle : DrawCmd { TODO: why is this having issues?
        public override string name { get { return "triangle"; } }
        public override string shortcut { get { return "tri"; } }
		protected override string PlaceMessage {
			get { return "Place three blocks to determine the edges."; }
		} 

        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
        	RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x2 = x; bp.y2 = y; bp.z2 = z;
            p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange3);
        }
        
        void Blockchange3(Player p, ushort x, ushort y, ushort z, byte type, byte extType) { 
        	RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            GetRealBlock(type, extType, p, ref cpos);
            Vec3U16[] marks = { new Vec3U16(cpos.x, cpos.y, cpos.z),
            	new Vec3U16(cpos.x2, cpos.y2, cpos.z2), new Vec3U16(x, y, z) };
            
            Brush brush = GetBrush(p, cpos, 0, null);
            if (brush == null) return;
            if (!DrawOp.DoDrawOp(new TriangleDrawOp(), brush, p, marks))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected override DrawMode ParseMode(string msg) {
            return DrawMode.normal;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/triangle [brush args]");
            Player.SendMessage(p, "%HDraws a triangle between three points.");
            Player.SendMessage(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }*/
}
