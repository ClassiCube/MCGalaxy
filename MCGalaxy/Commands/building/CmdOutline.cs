/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdOutline : DrawCmd {
        public override string name { get { return "Outline"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            Player p = dArgs.Player;
            if (dArgs.Message.Length == 0) {
                p.Message("Block name is required."); return null;
            }
            
            BlockID target;
            string[] parts = dArgs.Message.SplitSpaces(2);
            // NOTE: Don't need to check if allowed to use block here
            // (OutlineDrawOp skips all blocks that are equal to target)
            if (!CommandParser.GetBlock(p, parts[0], out target)) return null;
            
            OutlineDrawOp op = new OutlineDrawOp();
            op.side = GetSides(dArgs.Message.SplitSpaces());
            if (op.side == OutlineDrawOp.Side.Unspecified) op.side = OutlineDrawOp.Side.All;
            op.Target = target;
            return op;
        }

        OutlineDrawOp.Side GetSides(string[] parts) {
            if (parts.Length == 1) return OutlineDrawOp.Side.Unspecified;

            string type = parts[1];
            if (type == "left")  return OutlineDrawOp.Side.Left;
            if (type == "right") return OutlineDrawOp.Side.Right;
            if (type == "front") return OutlineDrawOp.Side.Front;
            if (type == "back")  return OutlineDrawOp.Side.Back;
            if (type == "down")  return OutlineDrawOp.Side.Down;
            if (type == "up")    return OutlineDrawOp.Side.Up;

            if (type == "layer") return OutlineDrawOp.Side.Layer;
            if (type == "all")   return OutlineDrawOp.Side.All;

            return OutlineDrawOp.Side.Unspecified;
        }
        
        // Parts is just Command.Use's message.SplitSpaces()
        protected override DrawMode GetMode(string[] parts) {
            // Need to return "normal" if a unique side was typed, otherwise "not normal". This tells the ModeArgsCount how to work correctly
            return GetSides(parts) == OutlineDrawOp.Side.Unspecified ? DrawMode.normal : DrawMode.solid;
        }
        
        protected override void GetBrush(DrawArgs dArgs) {
            dArgs.BrushArgs = dArgs.Message.Splice(dArgs.ModeArgsCount + 1, 0);
        }

        public override void Help(Player p) {
            p.Message("&T/Outline [block] <brush args>");
            p.Message("&HOutlines [block] with output of your current brush.");
            p.Message("&T/Outline [block] [mode] <brush args>");
            p.Message("&HModes: &fall/up/layer/down/left/right/front/back (default all)");
            p.Message(BrushHelpLine);
        }
    }
}
