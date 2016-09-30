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
using MCGalaxy.Generator.Foilage;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdTree : Command {
        public override string name { get { return "tree"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            string[] parts = message.SplitSpaces(2);
            string brushMsg = parts.Length >= 2 ? parts[1] : "";
            
            Tree tree = Tree.Find(parts[0]);
            if (tree == null) {
                brushMsg = message;
                tree = new NormalTree();
            }
            
            DrawArgs dArgs = default(DrawArgs);
            dArgs.tree = tree;
            dArgs.brushMsg = brushMsg;
            Player.Message(p, "Select where you wish your tree to grow");
            p.MakeSelection(1, dArgs, DoTree);
        }

        bool DoTree(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            DrawArgs dArgs = (DrawArgs)state;
            TreeDrawOp op = new TreeDrawOp();
            op.Tree = dArgs.tree;
            op.random = new System.Random();
            Brush brush = null;
            
            if (dArgs.brushMsg != "") {
                if (!p.group.CanExecute("brush")) {
                    Player.Message(p, "You cannot use /brush, so therefore cannot use /tree with a brush."); return false;
                }
                brush = ParseBrush(dArgs.brushMsg, p, type, extType);
                if (brush == null) return false;
            }
            return DrawOp.DoDrawOp(op, brush, p, marks);
        }
        
        static Brush ParseBrush(string raw, Player p, byte block, byte extBlock) {
            string[] parts = raw.SplitSpaces(2);
            BrushFactory brush = BrushFactory.Find(parts[0]);
            if (brush == null) {
                Player.Message(p, "No brush found with name \"{0}\".", parts[0]);
                Player.Message(p, "Available brushes: " + BrushFactory.Available);
                return null;
            }

            string brushArgs = parts.Length >= 2 ? parts[1].ToLower() : "";
            BrushArgs args = new BrushArgs(p, brushArgs, block, extBlock);
            return brush.Construct(args);
        }
        
        struct DrawArgs { public Tree tree; public string brushMsg; }

        public override void Help(Player p) {
            Player.Message(p, "%T/tree [type] %H- Draws a tree.");
            Player.Message(p, "%T/tree [type] [brush name] <brush args>");
            Player.Message(p, "%H  Types: &f{0}", Tree.TreeTypes.Join(t => t.Key));
            Player.Message(p, "%H  For help about brushes, type %T/help brush%H.");
        }
    }
}
