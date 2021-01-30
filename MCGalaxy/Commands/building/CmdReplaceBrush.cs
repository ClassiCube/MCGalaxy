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
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {    
    public class CmdReplaceBrush : DrawCmd {
        public override string name { get { return "ReplaceBrush"; } }
        public override string shortcut { get { return "rb"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        protected virtual bool ReplaceNot { get { return false; } }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            string[] args = dArgs.Message.SplitSpaces(3);
            Player p = dArgs.Player;
            if (args.Length < 2) { Help(p); return null; }
            
            string replaceCmd = ReplaceNot ? "ReplaceNot" : "Replace";
            if (!p.CanUse(replaceCmd) || !p.CanUse("Brush")) {
                p.Message("You cannot use /brush and/or /" + replaceCmd + 
                                   ", so therefore cannot use this command."); return null;
            }
            
            
            BlockID target;
            if (!CommandParser.GetBlockIfAllowed(p, args[0], out target)) return null;
            
            BrushFactory factory = BrushFactory.Find(args[1]);
            if (factory == null) {
                p.Message("No brush found with name \"{0}\".", args[1]);
                CmdBrush.List(p);
                return null;
            }
            
            DrawOp op = null;
            if (ReplaceNot) op = new ReplaceNotDrawOp(target);
            else op = new ReplaceDrawOp(target);
            return op;
        }
        
        protected override void GetBrush(DrawArgs dArgs) {
            string[] args = dArgs.Message.SplitSpaces(3);
            dArgs.BrushName = args[1];
            dArgs.BrushArgs = args.Length > 2 ? args[2] : "";
        }
        
        public override void Help(Player p) {
            p.Message("&T/ReplaceBrush [block] [brush name] <brush args>");
            p.Message("&HReplaces all blocks of the given type, " +
                               "in the specified area with the output of the given brush.");
            p.Message(BrushHelpLine);
        }
    }
    
    public class CmdReplaceNotBrush : CmdReplaceBrush {
        public override string name { get { return "ReplaceNotBrush"; } }
        public override string shortcut { get { return "rnb"; } }
        
        protected override bool ReplaceNot { get { return true; } }
        
        public override void Help(Player p) {
            p.Message("&T/ReplaceNotBrush [block] [brush name] <brush args>");
            p.Message("&HReplaces all blocks (except for the given block), " +
                               "in the specified area with the output of the given brush.");
            p.Message(BrushHelpLine);
        }
    }
}
