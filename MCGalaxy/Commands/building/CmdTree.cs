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
using MCGalaxy.Generator.Foliage;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdTree : Command {
        public override string name { get { return "tree"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message)
        {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            string[] parts = message.SplitSpaces(3);
            
            DrawArgs dArgs = default(DrawArgs);
            dArgs.size = -1;
            
            Tree tree = Tree.Find(parts[0]);
            if (tree == null) {
                dArgs.brushMsg = message;
                tree = new NormalTree();
            }
            dArgs.tree = tree;
            
            int size;
            if (parts.Length > 1 && int.TryParse(parts[1], out size)) {
                if (size < tree.MinSize) {
                    Player.Message(p, "Value must be {0} or above for {1} trees.", tree.MinSize, parts[0]); return;
                }
                if (size > tree.MaxSize) {
                    Player.Message(p, "Value must be {0} or below for {1} trees.", tree.MaxSize, parts[0]); return;
                }
                
                dArgs.size = size;
                dArgs.brushMsg = parts.Length >= 3 ? parts[2] : ""; // type value brush
            } else {
                dArgs.brushMsg = parts.Length >= 2 ? parts[1] : ""; // type brush
            }            

            if (!CheckBrush(p, dArgs.brushMsg)) return;           
            Player.Message(p, "Select where you wish your tree to grow");
            p.MakeSelection(1, dArgs, DoTree);
        }
        
        static bool CheckBrush(Player p, string brushMsg) {
            if (brushMsg == "") return true;
            
            if (!p.group.CanExecute("brush")) {
                Player.Message(p, "You cannot use %T/brush%S, so therefore cannot use %T/tree%S with a brush."); return false;
            }        
            return ParseBrush(brushMsg, p, 0, 0) != null;
        }

        bool DoTree(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            DrawArgs dArgs = (DrawArgs)state;
            TreeDrawOp op = new TreeDrawOp();
            op.Tree = dArgs.tree;
            op.Size = dArgs.size;
            
            Brush brush = null;
            if (dArgs.brushMsg != "") brush = ParseBrush(dArgs.brushMsg, p, type, extType);
            return DrawOpPerformer.Do(op, brush, p, marks);
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
        
        struct DrawArgs { public Tree tree; public string brushMsg; public int size; }

        public override void Help(Player p) {
            Player.Message(p, "%T/tree [type] %H- Draws a tree.");
            Player.Message(p, "%T/tree [type] [size] %H- Draws a tree of given size.");
            Player.Message(p, "%T/tree [type] [brush name] <brush args>");
            Player.Message(p, "%T/tree [type] [size] [brush name] <brush args>");
            Player.Message(p, "%H  Types: &f{0}", Tree.TreeTypes.Join(t => t.Key));
            Player.Message(p, "%H  For help about brushes, type %T/help brush%H.");
        }
    }
}
