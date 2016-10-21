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

namespace MCGalaxy.Commands.Building {
    public sealed class CmdHollow : Command {
        public override string name { get { return "hollow"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdHollow() { }

        public override void Use(Player p, string message) {
            byte skip = Block.Invalid;
            if (message != "") {
                skip = Block.Byte(message);
                if (skip == Block.Invalid) { Player.Message(p, "Cannot find block entered."); return; }
            }

            Player.Message(p, "Place two blocks to determine the edges.");
            p.MakeSelection(2, skip, DoHollow);
        }
        
        bool DoHollow(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            HollowDrawOp op = new HollowDrawOp();
            op.Skip = (byte)state;
            return DrawOp.DoDrawOp(op, null, p, marks);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/hollow");
            Player.Message(p, "%HHollows out an area without flooding it");
            Player.Message(p, "%T/hollow [block]");
            Player.Message(p, "%HHollows around [block]");
        }
    }
}
