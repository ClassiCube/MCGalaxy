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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {    
    public class CmdWriteText : DrawCmd {
        public override string name { get { return "WriteText"; } }
        public override string shortcut { get { return "wrt"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        protected override string SelectionType { get { return "direction"; } }
        protected override string PlaceMessage { get { return "Place or break two blocks to determine direction."; } }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            Player p = dArgs.Player;
            if (!p.CanUse("Write")) {
                p.Message("You must be able to use &T/Write &Sto use &T/WriteText."); return null;
            }
            
            string[] args = dArgs.Message.SplitSpaces(3);
            if (args.Length < 3) { Help(p); return null; }
            
            byte scale = 1, spacing = 1;
            if (!CommandParser.GetByte(p, args[0], "Scale",   ref scale))   return null;
            if (!CommandParser.GetByte(p, args[1], "Spacing", ref spacing)) return null;
            
            WriteDrawOp op = new WriteDrawOp();
            op.Scale = scale; op.Spacing = spacing;
            op.Text = args[2].ToUpper();
            return op;
        }

       
        protected override void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) {
            if (m[0].X != m[1].X || m[0].Z != m[1].Z) return; 
            dArgs.Player.Message("No direction was selected");
            m = null;
        }
        
        protected override void GetBrush(DrawArgs dArgs) { dArgs.BrushArgs = ""; }

        public override void Help(Player p) {
            p.Message("&T/WriteText [scale] [spacing] [message]");
            p.Message("&HWrites the given message in blocks.");
            p.Message("&Hspacing specifies the number of blocks between each letter.");
        }
    }
    
    public sealed class CmdWrite : CmdWriteText {       
        public override string name { get { return "Write"; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            base.Use(p, "1 1 " + message, data);
        }

        public override void Help(Player p) {
            p.Message("&T/Write [message]");
            p.Message("&HWrites [message] in blocks");
            p.Message("&HNote that this has been deprecated by &T/WriteText.");
        }
    }
}
