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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {    
    public sealed class CmdPaste : Command {        
        public override string name { get { return "paste"; } }
        public override string shortcut { get { return "v"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
        	get { return new[] { new CommandAlias("pn", "not") }; }
        }
        
        public override void Use(Player p, string message) {
            if (p.CopyBuffer == null) { Player.Message(p, "You haven't copied anything yet"); return; }
            
            CatchPos cpos = default(CatchPos);
            cpos.message = message;
            p.blockchangeObject = cpos;
            Player.Message(p, "Place a block in the corner of where you want to paste.");
            p.ClearBlockchange();
            p.Blockchange += PlacedOrigin;
        }

        void PlacedOrigin(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            RevertAndClearState(p, x, y, z);
            int x1 = p.copyoffset[0] + x, y1 = p.copyoffset[1] + y, z1 = p.copyoffset[2] + z;
            CopyState state = p.CopyBuffer;
            if (state.X != state.OriginX) x1 -= (state.Width - 1);
            if (state.Y != state.OriginY) y1 -= (state.Height - 1);
            if (state.Z != state.OriginZ) z1 -= (state.Length - 1);

            DrawOp op;
            if (cpos.message == "") {
                op = new SimplePasteDrawOp();
                ((SimplePasteDrawOp)op).CopyState = p.CopyBuffer;
            } else {
                op = new PasteDrawOp();
                ((PasteDrawOp)op).CopyState = p.CopyBuffer;
                string[] args = cpos.message.Split(' ');
                if (args[0].CaselessEq("not"))
                    ((PasteDrawOp)op).Exclude = ReplaceBrush.GetBlocks(p, 1, args.Length, args);
                else
                    ((PasteDrawOp)op).Include = ReplaceBrush.GetBlocks(p, 0, args.Length, args);
            }
            Vec3S32[] marks = { new Vec3S32(x1, y1, z1) };
            if (!DrawOp.DoDrawOp(op, null, p, marks))
                return;
            if (p.staticCommands) p.Blockchange += PlacedOrigin;
        } 

        struct CatchPos { public string message; }
        
        public override void Help(Player p) {
            Player.Message(p, "/paste - Pastes the stored copy.");
            Player.Message(p, "/paste [block] [block2].. - Pastes only the specified blocks from the copy.");
            Player.Message(p, "/paste not [block] [block2].. - Pastes all blocks from the copy, except for the specified blocks.");
            Player.Message(p, "&4BEWARE: %SThe blocks will always be pasted in a set direction");
        }
    }
}
