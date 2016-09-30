/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
    public sealed class CmdSpheroid : DrawCmd {        
        public override string name { get { return "spheroid"; } }
        public override string shortcut { get { return "e"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("eh", null, "hollow"), new CommandAlias("cylinder", null, "vertical") }; }
        }
        
        protected override string GetBrush(Player p, DrawArgs dArgs, ref int offset) {
            offset = dArgs.Mode == DrawMode.normal ? 0 : 1;
            if (dArgs.Mode == DrawMode.solid) return "normal";
            return p.BrushName;
        }
        
        protected override DrawMode GetMode(string[] parts) {
            string msg = parts[parts.Length - 1];
            if (msg == "solid") return DrawMode.solid;
            else if (msg == "hollow") return DrawMode.hollow;
            else if (msg == "vertical") return DrawMode.vertical;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            switch (dArgs.Mode) {
                case DrawMode.hollow: return new EllipsoidHollowDrawOp();
                case DrawMode.vertical: return new CylinderDrawOp();
            }
            return new EllipsoidDrawOp();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/spheroid [brush args] <mode>");
            Player.Message(p, "%HDraws a spheroid between two points.");
            Player.Message(p, "   %HModes: &fsolid/hollow/vertical(a vertical tube)");    
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}