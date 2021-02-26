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
        public override string name { get { return "Rainbow"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        protected override void GetBrush(DrawArgs dArgs) { dArgs.BrushName = "Normal"; }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            string args = dArgs.Message;
            RainbowDrawOp op = new RainbowDrawOp();
            if (args.Length > 0 && !CommandParser.GetBool(dArgs.Player, args, ref op.AllowAir)) return null;
            return op;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Rainbow <replace air>");
            p.Message("&HReplaces blocks with a rainbow between two points.");
            p.Message("&H<replace air> if given, also replaces over air.");
        }
    }
}