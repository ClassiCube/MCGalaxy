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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {  
    public class CmdReplace : DrawCmd {        
        public override string name { get { return "Replace"; } }
        public override string shortcut { get { return "r"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        protected virtual bool ReplaceNot { get { return false; } }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            return new CuboidDrawOp();
        }
        
        protected override string GetBrush(DrawArgs dArgs, ref int offset) {
            return ReplaceNot ? "replacenot" : "replace";
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Replace [block] [block2].. [new]");
            Player.Message(p, "%HReplaces [block] with [new] between two points.");
            Player.Message(p, "%H  If more than one [block] is given, they are all replaced.");
            Player.Message(p, "%H  If only [block] is given, replaces with your held block.");
        }
    }
    
    public sealed class CmdReplaceNot : CmdReplace {        
        public override string name { get { return "ReplaceNot"; } }
        public override string shortcut { get { return "rn"; } }
        protected override bool ReplaceNot { get { return true; } }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ReplaceNot [block] [block2].. [new]");
            Player.Message(p, "%HReplaces everything but [block] with [new] between two points.");
            Player.Message(p, "%H  If more than one [block] is given, they are all skipped.");
            Player.Message(p, "%H  If only [block] is given, replaces with your held block.");
        }
    }
}
