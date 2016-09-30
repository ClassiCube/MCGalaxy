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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdCuboid : DrawCmd {
        public override string name { get { return "cuboid"; } }
        public override string shortcut { get { return "z"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("cw", null, "wire"),
                    new CommandAlias("ch", null, "hollow"), new CommandAlias("walls", null, "walls"),
                    new CommandAlias("box"), new CommandAlias("hbox", null, "hollow") }; }
        }
        
        protected override DrawMode GetMode(string[] parts) {
            string msg = parts[parts.Length - 1];
            if (msg == "solid") return DrawMode.solid;
            else if (msg == "hollow") return DrawMode.hollow;
            else if (msg == "walls") return DrawMode.walls;
            else if (msg == "holes") return DrawMode.holes;
            else if (msg == "wire") return DrawMode.wire;
            else if (msg == "random") return DrawMode.random;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            switch (dArgs.Mode) {
                case DrawMode.hollow: return new CuboidHollowsDrawOp();
                case DrawMode.walls: return new CuboidWallsDrawOp();
                case DrawMode.holes: return new CuboidDrawOp();
                case DrawMode.wire: return new CuboidWireframeDrawOp();
                case DrawMode.random: return new CuboidDrawOp();
            }
            return new CuboidDrawOp();
        }
        
        protected override string GetBrush(Player p, DrawArgs dArgs, ref int offset) {
            offset = dArgs.Mode == DrawMode.normal ? 0 : 1;
            if (dArgs.Mode == DrawMode.solid) return "normal";
            if (dArgs.Mode == DrawMode.holes) return "checkered";
            if (dArgs.Mode == DrawMode.random) return "random";
            return p.BrushName;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/cuboid [brush args] <mode>");
            Player.Message(p, "%HDraws a cuboid between two points.");
            Player.Message(p, "   %HModes: &fsolid/hollow/walls/holes/wire/random");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}
