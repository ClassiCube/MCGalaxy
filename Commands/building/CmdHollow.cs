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
using System.Collections.Generic;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands {
    public sealed class CmdHollow : Command {
        public override string name { get { return "hollow"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdHollow() { }

        public override void Use(Player p, string message) {
            CatchPos cpos = default(CatchPos);
            cpos.other = Block.Zero;
            if (message != "") {
                cpos.other = Block.Byte(message.ToLower());
                if (cpos.other == Block.Zero) { Player.Message(p, "Cannot find block entered."); return; }
            }

            p.blockchangeObject = cpos;
            Player.Message(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            
            HollowDrawOp op = new HollowDrawOp();
            op.Skip = cpos.other;
            if (!DrawOp.DoDrawOp(op, null, p, x, y, z, cpos.x, cpos.y, cpos.z )) 
                return;
            if (p.staticCommands) 
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos {
            public ushort x, y, z; public byte other;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/hollow - Hollows out an area without flooding it");
            Player.Message(p, "/hollow [block] - Hollows around [block]");
        }
    }
}
