/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
    public class CmdWriteText : Command {        
        public override string name { get { return "writetext"; } }
        public override string shortcut { get { return "wrt"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (!p.group.CanExecute("write")) {
                Player.Message(p, "You must be able to use /write to use /writetext."); return;
            }
            
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            if (args.Length < 3) { Help(p); return; }
            
            byte scale = 1, spacing = 1;
            if (!byte.TryParse(args[0], out scale)) scale = 1;
            if (!byte.TryParse(args[1], out spacing)) spacing = 1;

            WriteArgs wArgs = default(WriteArgs);
            wArgs.scale = scale; wArgs.spacing = spacing;
            wArgs.message = args[2].ToUpper();
            Player.Message(p, "Place two blocks to determine direction.");
            p.MakeSelection(2, wArgs, DoWrite);
        }

        bool DoWrite(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            WriteArgs wArgs = (WriteArgs)state;
            if (marks[0].X == marks[1].X && marks[0].Z == marks[1].Z) { 
                Player.Message(p, "No direction was selected"); return false; 
            }
            
            WriteDrawOp op = new WriteDrawOp();
            op.Text = wArgs.message;
            op.Scale = wArgs.scale; op.Spacing = wArgs.spacing;
            Brush brush = new SolidBrush(type, extType);
            return DrawOp.DoDrawOp(op, brush, p, marks);
        }
        
        struct WriteArgs { public byte scale, spacing; public ushort x, y, z; public string message; }

        public override void Help(Player p) {
            Player.Message(p, "%T/wrt [scale] [spacing] [message]");
            Player.Message(p, "%HWrites the given message in blocks.");
            Player.Message(p, "%Hspacing specifies the number of blocks between each letter.");
        }
    }
    
    public sealed class CmdWrite : CmdWriteText {       
        public override string name { get { return "write"; } }
        public override string shortcut { get { return ""; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            base.Use(p, "1 1 " + message);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/write [message]");
            Player.Message(p, "%HWrites [message] in blocks");
            Player.Message(p, "%HNote that this command has been deprecated by /writetext.");
        }
    }
}
