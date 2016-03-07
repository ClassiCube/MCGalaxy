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
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands {
    
    public sealed class CmdReplaceNot : ReplaceCmd {
        
        public override string name { get { return "replacenot"; } }
        public override string shortcut { get { return "rn"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        protected override void BeginReplace(Player p, ExtBlock[] toAffect, ExtBlock target) {
        	CatchPos cpos = default(CatchPos);
            cpos.toAffect = toAffect;
            cpos.target = target;
            p.blockchangeObject = cpos;
            
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            ReplaceNotDrawOp drawOp = new ReplaceNotDrawOp();
            drawOp.ToReplace = cpos.toAffect;
            drawOp.Target = cpos.target;
            
            if (!DrawOp.DoDrawOp(drawOp, null, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos { public ushort x, y, z; public ExtBlock[] toAffect; public ExtBlock target; }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/rn [block] [block2].. [new] - replace everything but [block] with [new] inside a selected cuboid");
            Player.SendMessage(p, "If multiple [block]s are specified they will all be ignored.");
        }
    }
}
