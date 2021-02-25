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
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdOutline : DrawCmd {
        public override string name { get { return "Outline"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            if (dArgs.Message.Length == 0) {
                dArgs.Player.Message("Block name is required."); return null;
            }
            
            BlockID target;
            string[] parts = dArgs.Message.SplitSpaces(2);
            if (!CommandParser.GetBlockIfAllowed(dArgs.Player, parts[0], out target)) return null;
            
            OutlineDrawOp op = new OutlineDrawOp();
            // e.g. testing air 'above' grass - therefore op.Above needs to be false for 'up mode'
            if (dArgs.Mode == DrawMode.up)    { op.Layer = false; op.Above = false; }
            if (dArgs.Mode == DrawMode.down)  { op.Layer = false; op.Below = false; }
            if (dArgs.Mode == DrawMode.layer) { op.Above = false; op.Below = false; }
            op.Target = target;
            return op;
        }
        
                
        protected override DrawMode GetMode(string[] parts) {
            if (parts.Length == 1) return DrawMode.normal;
            
            string type = parts[1];
            if (type == "down")  return DrawMode.down;
            if (type == "up")    return DrawMode.up;
            if (type == "layer") return DrawMode.layer;
            if (type == "all")   return DrawMode.solid;
            return DrawMode.normal;
        }
        
        protected override void GetBrush(DrawArgs dArgs) {
            dArgs.BrushArgs = dArgs.Message.Splice(dArgs.ModeArgsCount + 1, 0);
        }

        public override void Help(Player p) {
            p.Message("&T/Outline [block] <brush args>");
            p.Message("&HOutlines [block] with output of your current brush.");
            p.Message("&T/Outline [block] [mode] <brush args>");
            p.Message("&HModes: &fall/up/layer/down (default all)");
            p.Message(BrushHelpLine);
        }
    }
}
