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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {  
    public class CmdReplace : Command {        
        public override string name { get { return "replace"; } }
        public override string shortcut { get { return "r"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        protected virtual bool ReplaceNot { get { return false; } }
        
        public override void Use(Player p, string message) {
            string brushMsg = message.ToLower();
            byte block, extBlock;
            block = p.GetActualHeldBlock(out extBlock);
            
            BrushArgs args = new BrushArgs(p, brushMsg, block, extBlock);
            string name = ReplaceNot ? "replacenot" : "replace";
            if (!BrushFactory.Find(name).Validate(args)) return;
            
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, message.ToLower(), DoReplace);
        }
        
        bool DoReplace(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            BrushArgs args = new BrushArgs(p, (string)state, type, extType);
            string name = ReplaceNot ? "replacenot" : "replace";
            Brush brush = BrushFactory.Find(name).Construct(args);
            if (brush == null) return false;
            
            DrawOp op = new CuboidDrawOp();
            return DrawOpPerformer.Do(op, brush, p, marks);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/replace [block] [block2].. [new]");
            Player.Message(p, "%HReplaces [block] with [new] between two points.");
            Player.Message(p, "%H  If more than one [block] is given, they are all replaced.");
            Player.Message(p, "%H  If only [block] is given, replaces with your held block.");
        }
    }
    
    public sealed class CmdReplaceNot : CmdReplace {        
        public override string name { get { return "replacenot"; } }
        public override string shortcut { get { return "rn"; } }
        protected override bool ReplaceNot { get { return true; } }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/replacenot [block] [block2].. [new]");
            Player.Message(p, "%HReplaces everything but [block] with [new] between two points.");
            Player.Message(p, "%H  If more than one [block] is given, they are all skipped.");
            Player.Message(p, "%H  If only [block] is given, replaces with your held block.");
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
            Brush brush = BrushFactory.Find("replace").Construct(args);
            if (brush == null) return;
            
            DrawOp op = new CuboidDrawOp();
            if (!DrawOpPerformer.Do(op, brush, p, 0, 0, 0, x2, y2, z2))
                return;
            Player.Message(p, "&4/replaceall finished!");
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/replaceall [block] [block2].. [new]");
            Player.Message(p, "%HReplaces [block] with [new] for the entire map.");
            Player.Message(p, "%H  If more than one [block] is given, they are all replaced.");
            Player.Message(p, "%H  If only [block] is given, replaces with your held block.");
        }
    }
}
