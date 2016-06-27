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
            
            dArgs.type = DrawCmd.GetBlock(p, args[0], out dArgs.extType);
            if (dArgs.type == Block.Zero) return;
            dArgs.newType = DrawCmd.GetBlock(p, args[1], out dArgs.newExtType);
            if (dArgs.newType == Block.Zero) return;

            Player.Message(p, "Place two blocks to determine the edges.");
            p.MakeSelection(2, dArgs, DoOutline);
        }
        
        bool DoOutline(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            DrawArgs dArgs = (DrawArgs)state;
            OutlineDrawOp op = new OutlineDrawOp();
            op.Type = dArgs.type; op.ExtType = dArgs.extType;
            op.NewType = dArgs.newType; op.NewExtType = dArgs.newExtType;
            return DrawOp.DoDrawOp(op, null, p, marks);
        }
        struct DrawArgs { public byte type, extType, newType, newExtType; }

        public override void Help(Player p) {
            Player.Message(p, "%T/outline [type] [type2]");
            Player.Message(p, "%HOutlines [type] with [type2]");
        }
    }
}
