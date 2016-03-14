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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands {
    
    public sealed class CmdWriteText : Command {
        
        public override string name { get { return "writetext"; } }
        public override string shortcut { get { return "wrt"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        static char[] trimChars = { ' ' };
        
        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (!p.group.CanExecute("write")) {
                Player.SendMessage(p, "You must be able to use /write to use /writetext."); return;
            }
            
            if (message == "") { Help(p); return; }
            string[] args = message.Split(trimChars, 3);
            if (args.Length < 3) { Help(p); return; }
            
            byte scale = 1, spacing = 1;
            if (!byte.TryParse(args[0], out scale)) scale = 1;
            if (!byte.TryParse(args[1], out spacing)) spacing = 1;

            CatchPos cpos = default(CatchPos);
            cpos.scale = scale; cpos.spacing = spacing;
            cpos.givenMessage = args[2].ToUpper();
            p.blockchangeObject = cpos;
            
            Player.SendMessage(p, "Place two blocks to determine direction.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            type = type < 128 ? p.bindings[type] : type;
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            Level lvl = p.level;
            if (x == cpos.x && z == cpos.z) { Player.SendMessage(p, "No direction was selected"); return; }
            
            WriteDrawOp op = new WriteDrawOp();
            op.Text = cpos.givenMessage;
            op.Scale = cpos.scale; op.Spacing = cpos.spacing;
            Brush brush = new SolidBrush(type, extType);
            if (!DrawOp.DoDrawOp(op, brush, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;

            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        struct CatchPos { public byte scale, spacing; public ushort x, y, z; public string givenMessage; }

        public override void Help(Player p) {
            Player.SendMessage(p, "%T/wrt [scale] [spacing] [message]");
            Player.SendMessage(p, "%TWrites the given message in blocks.");
            Player.SendMessage(p, "%Tspacing specifies the number of blocks between each letter.");
        }
    }
    
    public sealed class CmdWrite : Command {
        
        public override string name { get { return "write"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            Command.all.Find("writetext").Use(p, "1 1 " + message);
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/write [message] - Writes [message] in blocks");
            Player.SendMessage(p, "Note that this command has been deprecated by /writetext.");
        }
    }
}
