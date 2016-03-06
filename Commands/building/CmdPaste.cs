/*
    Copyright 2011 MCGalaxy
        
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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands {
    
    public sealed class CmdPaste : Command {
        
        public override string name { get { return "paste"; } }
        public override string shortcut { get { return "v"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            if (p.CopyBuffer == null) {
                Player.SendMessage(p, "You haven't copied anything yet"); return;
            }
            
            p.blockchangeObject = default(CatchPos);
            Player.SendMessage(p, "Place a block in the corner of where you want to paste."); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            int offX = p.copyoffset[0] + x, offY = p.copyoffset[1] + y, offZ = p.copyoffset[2] + z;
            CopyState state = p.CopyBuffer;
            if (state.X != state.OriginX) offX -= (state.Width - 1);
            if (state.Y != state.OriginY) offY -= (state.Height - 1);
            if (state.Z != state.OriginZ) offZ -= (state.Length - 1);

            SimplePasteDrawOp drawOp = new SimplePasteDrawOp();          
            drawOp.CopyState = p.CopyBuffer;
            if (!DrawOp.DoDrawOp(drawOp, null, p, (ushort)offX, (ushort)offY, (ushort)offZ, 0, 0, 0))
                return;
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos { }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/paste - Pastes the stored copy.");
            Player.SendMessage(p, "&4BEWARE: %SThe blocks will always be pasted in a set direction");
        }
    }
}
