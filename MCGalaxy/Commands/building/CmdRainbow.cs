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
    public sealed class CmdRainbow : DrawCmd {
        public override string name { get { return "rainbow"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        protected override DrawOp GetDrawOp(DrawArgs dArgs) { return new RainbowDrawOp(); }
        
        protected override string GetBrush(DrawArgs dArgs, ref int offset) { return "normal"; }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/rainbow");
            Player.Message(p, "%HTaste the rainbow");
        }
    }
}