/*
    Copyright 2015 MCGalaxy
        
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
    public class CmdReplaceBrush : Command {
        public override string name { get { return "replacebrush"; } }
        public override string shortcut { get { return "rb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            
            string replaceCmd = ReplaceNot ? "replacenot" : "replace";
            if (!p.group.CanExecute(replaceCmd) || !p.group.CanExecute("brush")) {
                Player.Message(p, "You cannot use /brush and/or /" + replaceCmd + 
                                   ", so therefore cannot use this command."); return;
            }
            
            message = message.ToLower();
            string[] parts = message.SplitSpaces(3);
            if (!ValidateArgs(p, parts)) return;
 
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, message, DoReplace);
        }
        
        bool ValidateArgs(Player p, string[] args) {
            if (args.Length < 2) { Help(p); return false; }
            
            ExtBlock block;
            if (!CommandParser.GetBlockIfAllowed(p, args[0], out block)) return false;
            
            BrushFactory factory = BrushFactory.Find(args[1]);
            if (factory == null) {
                Player.Message(p, "No brush found with name \"{0}\".", args[1]);
                Player.Message(p, "Available brushes: " + BrushFactory.Available);
                return false;
            }
            
            string brushMessage = args.Length > 2 ? args[2] : "";
            BrushArgs bArgs = new BrushArgs(p, brushMessage, p.GetHeldBlock());
            return factory.Validate(bArgs);
        }
        
        bool DoReplace(Player p, Vec3S32[] marks, object state, ExtBlock held) {
            string[] args = ((string)state).SplitSpaces(3);
            ExtBlock target;
            if (!CommandParser.GetBlockIfAllowed(p, args[0], out target)) return false;
            
            BrushFactory factory = BrushFactory.Find(args[1]);
            string brushMessage = args.Length > 2 ? args[2] : "";
            BrushArgs bArgs = new BrushArgs(p, brushMessage, held);
            Brush brush = factory.Construct(bArgs);
            if (brush == null) return false;
            
            DrawOp op = null;
            if (ReplaceNot) op = new ReplaceNotDrawOp(target);
            else op = new ReplaceDrawOp(target);
            return DrawOpPerformer.Do(op, brush, p, marks);
        }
        
        protected virtual bool ReplaceNot { get { return false; } }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/rb [block] [brush name] <brush args>");
            Player.Message(p, "%HReplaces all blocks of the given type, " +
                               "in the specified area with the output of the given brush.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
    
    public class CmdReplaceNotBrush : CmdReplaceBrush {
        public override string name { get { return "replacenotbrush"; } }
        public override string shortcut { get { return "rnb"; } }
        
        protected override bool ReplaceNot { get { return true; } }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/rnb [block] [brush name] <brush args>");
            Player.Message(p, "%HReplaces all blocks (except for the given block), " +
                               "in the specified area with the output of the given brush.");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}
