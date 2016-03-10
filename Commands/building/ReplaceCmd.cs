/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Commands {
    
    public class CmdReplace : Command {
        
        public override string name { get { return "replace"; } }
        public override string shortcut { get { return "r"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        public override void Use(Player p, string message) {
            CatchPos cpos = default(CatchPos);
            cpos.message = message.ToLower();
            p.blockchangeObject = cpos;
            
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z;
            p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            BrushArgs args = new BrushArgs(p, cpos.message, type, extType);
            Brush brush = ReplaceNot ? ReplaceNotBrush.Process(args) : ReplaceBrush.Process(args);
            if (brush == null) return;
            
            DrawOp drawOp = new CuboidDrawOp();
            if (!DrawOp.DoDrawOp(drawOp, brush, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected virtual bool ReplaceNot { get { return false; } }
        
        struct CatchPos { public ushort x, y, z; public string message; }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/replace [block] [block2].. [new]");
            Player.SendMessage(p, "%HReplaces [block] with [new] between two points.");
            Player.SendMessage(p, "%H   If more than one [block] is specified, they will all be replaced.");
        }
    }
    
    public sealed class CmdReplaceNot : CmdReplace {
        
        public override string name { get { return "replacenot"; } }
        public override string shortcut { get { return "rn"; } }

        protected override bool ReplaceNot { get { return true; } }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/replacenot [block] [block2].. [new]");
            Player.SendMessage(p, "%HReplaces everything but [block] with [new] between two points.");
            Player.SendMessage(p, "%H   If more than one [block] is specified, they will all be ignored.");
        }
    }
    
    public sealed class CmdReplaceAll : Command {
        
        public override string name { get { return "replaceall"; } }
        public override string shortcut { get { return "ra"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        
        public override void Use(Player p, string message) {
            ushort x2 = (ushort)(p.level.Width - 1);
            ushort y2 = (ushort)(p.level.Height - 1);
            ushort z2 = (ushort)(p.level.Length - 1);

            BrushArgs args = new BrushArgs(p, message.ToLower(), 0, 0);
            Brush brush = ReplaceBrush.Process(args);
            if (brush == null) return;
            
            DrawOp drawOp = new CuboidDrawOp();
            if (!DrawOp.DoDrawOp(drawOp, brush, p, 0, 0, 0, x2, y2, z2))
                return;
            Player.SendMessage(p, "&4/replaceall finished!");
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "%T/replaceall [block] [block2].. [new]");
            Player.SendMessage(p, "%HReplaces [block] with [new] for the entire map.");
            Player.SendMessage(p, "%H   If more than one [block] is specified, they will all be replaced.");
        }
    }
}
