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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdTree : Command {
        public override string name { get { return "Tree"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message) {
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
            p.MakeSelection(1, "Selecting location for %STree", dArgs, DoTree);
        }
        
        static bool CheckBrush(Player p, string brushMsg) {
            if (brushMsg.Length == 0) return true;
            
            if (!p.group.CanExecute("brush")) {
                Player.Message(p, "You cannot use %T/Brush%S, so therefore cannot use %T/Tree%S with a brush."); return false;
            }        
            return ParseBrush(brushMsg, p, ExtBlock.Air) != null;
        }

        bool DoTree(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            DrawArgs dArgs = (DrawArgs)state;
            TreeDrawOp op = new TreeDrawOp();
            op.Tree = dArgs.tree;
            op.Size = dArgs.size;
            
            Brush brush = null;
            if (dArgs.brushMsg != "") brush = ParseBrush(dArgs.brushMsg, p, block);
            DrawOpPerformer.Do(op, brush, p, marks);
            return true;
        }
        
        
        static Brush ParseBrush(string raw, Player p, ExtBlock block) {
            string[] parts = raw.SplitSpaces(2);
            BrushFactory brush = BrushFactory.Find(parts[0]);
            if (brush == null) {
                Player.Message(p, "No brush found with name \"{0}\".", parts[0]);
                Player.Message(p, "Available brushes: " + BrushFactory.Available);
                return null;
            }

            string brushArgs = parts.Length >= 2 ? parts[1].ToLower() : "";
            BrushArgs args = new BrushArgs(p, brushArgs, block);
            return brush.Construct(args);
        }
        
        struct DrawArgs { public Tree tree; public string brushMsg; public int size; }

        public override void Help(Player p) {
            Player.Message(p, "%T/Tree [type] %H- Draws a tree.");
            Player.Message(p, "%T/Tree [type] [size] %H- Draws a tree of given size.");
            Player.Message(p, "%T/Tree [type] [brush name] <brush args>");
            Player.Message(p, "%T/Tree [type] [size] [brush name] <brush args>");
            Player.Message(p, "%H  Types: &f{0}", Tree.TreeTypes.Join(t => t.Key));
            Player.Message(p, "%H  For help about brushes, type %T/Help Brush%H.");
        }
    }
}
