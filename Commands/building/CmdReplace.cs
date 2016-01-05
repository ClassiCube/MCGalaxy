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
    
    public sealed class CmdReplace : ReplaceCmd {
        
        public override string name { get { return "replace"; } }
        public override string shortcut { get { return "r"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdReplace() { }
        
        protected override void BeginReplace(Player p) {
            p.blockchangeObject = default(CatchPos);
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z;
            p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            ushort x1 = Math.Min(cpos.x, x), x2 = Math.Max(cpos.x, x);
            ushort y1 = Math.Min(cpos.y, y), y2 = Math.Max(cpos.y, y);
            ushort z1 = Math.Min(cpos.z, z), z2 = Math.Max(cpos.z, z);
            ReplaceDrawOp drawOp = new ReplaceDrawOp();
            drawOp.ToReplace = toAffect;
            drawOp.Target = target;
            
            if (!DrawOp.DoDrawOp(drawOp, null, p, x1, y1, z1, x2, y2, z2))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        struct CatchPos { public ushort x, y, z; }
        
        public override void Help(Player p) {
            p.SendMessage("/replace [block,block2,...] [new] - replace block with new inside a selected cuboid");
            p.SendMessage("If more than one block is specified, they will all be replaced.");
        }
    }
}
