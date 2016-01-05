/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Collections.Generic;

namespace MCGalaxy.Commands {
    
    public sealed class CmdReplaceAll : ReplaceCmd {
        
        public override string name { get { return "replaceall"; } }
        public override string shortcut { get { return "ra"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdReplaceAll() { }
        
        protected override void BeginReplace(Player p) {
            ushort x2 = (ushort)(p.level.Width - 1);
            ushort y2 = (ushort)(p.level.Height - 1);
            ushort z2 = (ushort)(p.level.Length - 1);
            ReplaceDrawOp drawOp = new ReplaceDrawOp();
            drawOp.ToReplace = toAffect;
            drawOp.Target = target;
            
            if (!DrawOp.DoDrawOp(drawOp, null, p, 0, 0, 0, x2, y2, z2))
                return;
            Player.SendMessage(p, "&4/replaceall finished!");
        }

        public override void Help(Player p) {
            p.SendMessage("/replaceall [block,block2,...] [new] - Replaces all of [block] with [new] in a map");
            p.SendMessage("If more than one block is specified, they will all be replaced.");
        }
    }
}
