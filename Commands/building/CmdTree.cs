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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdTree : Command {
        public override string name { get { return "tree"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            int mode = TreeDrawOp.T_Tree;
            string[] parts = message.SplitSpaces(2);
            string brushMsg = parts.Length >= 2 ? parts[1] : "";
            
            switch (parts[0].ToLower()) {
                case "1":
                    case "fern": mode = TreeDrawOp.T_Tree; break;
                case "2":
                    case "cactus": mode = TreeDrawOp.T_Cactus; break;
                case "3":
                    case "notch": mode = TreeDrawOp.T_NotchTree; break;
                case "4":
                    case "swamp": mode = TreeDrawOp.T_NotchSwamp; break;
                    default: brushMsg = message; break;
            }
            
            CatchPos cpos = default(CatchPos);
            cpos.mode = mode;
            cpos.brushMsg = brushMsg;
            p.ClearBlockchange();
            p.blockchangeObject = cpos;
            p.Blockchange += PlacedBase;
            Player.Message(p, "Select where you wish your tree to grow");
        }

        void PlacedBase(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            type = type < 128 ? p.bindings[type] : type;
            
            TreeDrawOp op = new TreeDrawOp();
            op.Type = cpos.mode;
            op.random = p.random;
            Brush brush = null;
            
            if (cpos.brushMsg != "") {
                if (!p.group.CanExecute("brush")) {
                    Player.Message(p, "You cannot use /brush, so therefore cannot use /tree with a brush."); return;
                }
                brush = ParseBrush(cpos.brushMsg, p, type, extType);
                if (brush == null) return;
            }
            
            Vec3S32[] marks = { new Vec3S32(x, y, z) };
            if (!DrawOp.DoDrawOp(op, brush, p, marks))
                return;
            if (p.staticCommands)
                p.Blockchange += PlacedBase;
        }
        
        static Brush ParseBrush(string brushMsg, Player p, byte type, byte extType) {
            string[] parts = brushMsg.SplitSpaces(2);
            string brushName = CmdBrush.FindBrush(parts[0]);
            if (brushName == null) {
                Player.Message(p, "No brush found with name \"" + parts[0] + "\".");
                Player.Message(p, "Available brushes: " + CmdBrush.AvailableBrushes);
                return null;
            }

            string brushMessage = parts.Length >= 2 ? parts[1].ToLower() : "";
            BrushArgs args = new BrushArgs(p, brushMessage, type, extType);
            return Brush.Brushes[brushName](args);
        }
        
        struct CatchPos { public int mode; public string brushMsg; }

        public override void Help(Player p) {
            Player.Message(p, "%T/tree [type] %H- Draws a tree.");
            Player.Message(p, "%HTypes - &fFern/1, Cactus/2, Notch/3, Swamp/4");
            Player.Message(p, "%T/tree [type] [brush name] <brush args>");
			Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}
