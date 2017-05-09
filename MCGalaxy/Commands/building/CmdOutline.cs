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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdOutline : Command {
        public override string name { get { return "outline"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdOutline() { }

        public override void Use(Player p, string message)
        {
            string[] args = message.SplitSpaces(2);
            DrawArgs dArgs = default(DrawArgs);
            
            if (!CommandParser.GetBlockIfAllowed(p, args[0], out dArgs.block, out dArgs.extBlock)) return;
            string brushArgs = args.Length > 1 ? args[1] : "";
            dArgs.brushArgs = brushArgs;
            
            BrushFactory factory = BrushFactory.Find(p.BrushName);
            byte held, extHeld;
            held = p.GetActualHeldBlock(out extHeld);
            BrushArgs bArgs = new BrushArgs(p, brushArgs, held, extHeld);
            if (!factory.Validate(bArgs)) return;
            
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, dArgs, DoOutline);
        }
        
        bool DoOutline(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            DrawArgs dArgs = (DrawArgs)state;
            OutlineDrawOp op = new OutlineDrawOp();
            op.Block = dArgs.block; op.ExtBlock = dArgs.extBlock;
            
            BrushFactory factory = BrushFactory.Find(p.BrushName);
            BrushArgs bArgs = new BrushArgs(p, dArgs.brushArgs, type, extType);
            Brush brush = factory.Construct(bArgs);
            if (brush == null) return false;
            
            return DrawOpPerformer.Do(op, brush, p, marks);
        }
        struct DrawArgs { public byte block, extBlock; public string brushArgs; }

        public override void Help(Player p) {
            Player.Message(p, "%T/outline [block] <brush args>");
            Player.Message(p, "%HOutlines [block] with output of your current brush.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}
