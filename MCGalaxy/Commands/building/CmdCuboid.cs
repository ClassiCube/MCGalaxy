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
        public override string name { get { return "Cuboid"; } }
        public override string shortcut { get { return "z"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("cw", "wire"),
                    new CommandAlias("ch", "hollow"), new CommandAlias("Walls", "walls"),
                    new CommandAlias("box"), new CommandAlias("hbox", "hollow") }; }
        }
        
        protected override DrawMode GetMode(string[] parts) {
            string msg = parts[0];
            if (msg == "solid")  return DrawMode.solid;
            if (msg == "hollow") return DrawMode.hollow;
            if (msg == "walls")  return DrawMode.walls;
            if (msg == "holes")  return DrawMode.holes;
            if (msg == "wire")   return DrawMode.wire;
            if (msg == "random") return DrawMode.random;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            switch (dArgs.Mode) {
                case DrawMode.hollow: return new CuboidHollowsDrawOp();
                case DrawMode.walls:  return new CuboidWallsDrawOp();
                case DrawMode.holes:  return new CuboidDrawOp();
                case DrawMode.wire:   return new CuboidWireframeDrawOp();
                case DrawMode.random: return new CuboidDrawOp();
            }
            return new CuboidDrawOp();
        }
        
        protected override void GetBrush(DrawArgs dArgs) {
            if (dArgs.Mode == DrawMode.solid)  dArgs.BrushName = "Normal";
            if (dArgs.Mode == DrawMode.holes)  dArgs.BrushName = "Checkered";
            if (dArgs.Mode == DrawMode.random) dArgs.BrushName = "Random";
            dArgs.BrushArgs = dArgs.Message.Splice(dArgs.ModeArgsCount, 0);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Cuboid <brush args>");
            p.Message("&HDraws a cuboid between two points.");
            p.Message("&T/Cuboid [mode] <brush args>");
            p.Message("&HModes: &fsolid/hollow/walls/holes/wire/random");
            p.Message(BrushHelpLine);
        }
    }
}
