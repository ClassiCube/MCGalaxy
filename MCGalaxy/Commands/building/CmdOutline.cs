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
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdOutline : Command {
        public override string name { get { return "outline"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdOutline() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length != 2) { Help(p); return; }
            DrawArgs dArgs = default(DrawArgs);           
            
            int block = DrawCmd.GetBlock(p, args[0], out dArgs.extBlock);
            if (block == -1) return;
            dArgs.block = (byte)block;
            
            int newBlock = DrawCmd.GetBlock(p, args[1], out dArgs.newExtBlock);
            if (newBlock == -1) return;
            dArgs.newBlock = (byte)newBlock;

            Player.Message(p, "Place two blocks to determine the edges.");
            p.MakeSelection(2, dArgs, DoOutline);
        }
        
        bool DoOutline(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            DrawArgs dArgs = (DrawArgs)state;
            OutlineDrawOp op = new OutlineDrawOp();
            op.Block = dArgs.block; op.ExtBlock = dArgs.extBlock;
            op.NewBlock = dArgs.newBlock; op.NewExtBlock = dArgs.newExtBlock;
            return DrawOp.DoDrawOp(op, null, p, marks);
        }
        struct DrawArgs { public byte block, extBlock, newBlock, newExtBlock; }

        public override void Help(Player p) {
            Player.Message(p, "%T/outline [block] [block2]");
            Player.Message(p, "%HOutlines [block] with [block2]");
        }
    }
}
