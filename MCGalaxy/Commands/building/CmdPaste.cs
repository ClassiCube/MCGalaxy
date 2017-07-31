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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPaste : Command {
        public override string name { get { return "Paste"; } }
        public override string shortcut { get { return "v"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("PasteNot", "not"), new CommandAlias("pn", "not") }; }
        }
        
        public override void Use(Player p, string message) {
            if (p.CopyBuffer == null) { Player.Message(p, "You haven't copied anything yet"); return; }
            
            BrushArgs args = new BrushArgs(p, message, ExtBlock.Air);
            Brush brush = BrushFactory.Find("paste").Construct(args);
            if (brush == null) return;
            
            Player.Message(p, "Place a block in the corner of where you want to paste.");
            p.MakeSelection(1, "Selecting location for %SPaste", brush, DoPaste);
        }

        bool DoPaste(Player p, Vec3S32[] m, object state, ExtBlock block) {
            CopyState cState = p.CopyBuffer;
            m[0] += cState.Offset;

            PasteDrawOp op = new PasteDrawOp();
            op.CopyState = p.CopyBuffer;
            DrawOpPerformer.Do(op, (Brush)state, p, m);
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Paste %H- Pastes the stored copy.");
            Player.Message(p, "%T/Paste [block] [block2].. %H- Pastes only the specified blocks from the copy.");
            Player.Message(p, "%T/Paste not [block] [block2].. %H- Pastes all blocks from the copy, except for the specified blocks.");
            Player.Message(p, "&4BEWARE: %SThe blocks will always be pasted in a set direction");
        }
    }
}
