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
        	get { return new[] { new CommandAlias("pastenot", "not"), new CommandAlias("pn", "not") }; }
        }
        
        public override void Use(Player p, string message) {
            if (p.CopyBuffer == null) { Player.Message(p, "You haven't copied anything yet"); return; }
            Player.Message(p, "Place a block in the corner of where you want to paste.");
            p.MakeSelection(1, message, DoPaste);
        }

        bool DoPaste(Player p, Vec3S32[] m, object state, byte type, byte extType) {
        	string message = (string)state;
        	m[0] += p.copyoffset;
            CopyState cState = p.CopyBuffer;
            if (cState.X != cState.OriginX) m[0].X -= (cState.Width - 1);
            if (cState.Y != cState.OriginY) m[0].Y -= (cState.Height - 1);
            if (cState.Z != cState.OriginZ) m[0].Z -= (cState.Length - 1);

            DrawOp op;
            if (message == "") {
                op = new SimplePasteDrawOp();
                ((SimplePasteDrawOp)op).CopyState = p.CopyBuffer;
            } else {
                op = new PasteDrawOp();
                ((PasteDrawOp)op).CopyState = p.CopyBuffer;
                string[] args = message.Split(' ');
                if (args[0].CaselessEq("not"))
                    ((PasteDrawOp)op).Exclude = ReplaceBrush.GetBlocks(p, 1, args.Length, args);
                else
                    ((PasteDrawOp)op).Include = ReplaceBrush.GetBlocks(p, 0, args.Length, args);
            }
            return DrawOp.DoDrawOp(op, null, p, m);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/paste - Pastes the stored copy.");
            Player.Message(p, "/paste [block] [block2].. - Pastes only the specified blocks from the copy.");
            Player.Message(p, "/paste not [block] [block2].. - Pastes all blocks from the copy, except for the specified blocks.");
            Player.Message(p, "&4BEWARE: %SThe blocks will always be pasted in a set direction");
        }
    }
}
