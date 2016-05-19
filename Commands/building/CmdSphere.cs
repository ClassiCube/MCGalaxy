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
using System.Collections.Generic;
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Commands {
    public sealed class CmdSphere : DrawCmd {
        
        public override string name { get { return "sphere"; } }
        public override string shortcut { get { return "sp"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
        	get { return new[] { new CommandAlias("sphereh", null, "hollow"), new CommandAlias("sph", null, "hollow") }; }
        }
        protected override string PlaceMessage { get { return "Place a block for the centre, then another for the radius."; } }
        
        protected override DrawMode ParseMode(string msg) {
            if (msg == "solid") return DrawMode.solid;
            else if (msg == "hollow") return DrawMode.hollow;
            return DrawMode.normal;
        }

        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            GetRealBlock(type, extType, p, ref cpos);

            int dx = cpos.x - x, dy = cpos.y - y, dz = cpos.z - z;
            int radius = (int)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            Vec3S32[] marks = { new Vec3S32(cpos.x - radius, cpos.y - radius, cpos.z - radius),
                new Vec3S32(cpos.x + radius, cpos.y + radius, cpos.z + radius) };
            
            DrawOp op = null;
            Func<BrushArgs, Brush> constructor = null;
            switch (cpos.mode) {
                case DrawMode.solid:
                    op = new AdvSphereDrawOp();
                    constructor = SolidBrush.Process; break;
                case DrawMode.hollow:
                    op = new AdvHollowSphereDrawOp(); break;
                default:
                    op = new AdvSphereDrawOp(); break;
            }         
            int brushOffset = cpos.mode == DrawMode.normal ? 0 : 1;
            Brush brush = GetBrush(p, cpos, brushOffset, constructor);
            if (brush == null) return;
            
            if (!DrawOp.DoDrawOp(op, brush, p, marks))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/sphere [brush args] <mode>");
            Player.Message(p, "%HCreates a sphere, using the first point as the centre, and the second point as the radius.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
            Player.Message(p, "   %HModes: &fsolid/hollow");
        }
    }
}
