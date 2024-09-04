/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Commands;
using MCGalaxy.Commands.Building;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes 
{
    public class ReplaceBrushBrushFactory : BrushFactory 
    {
        public override string Name { get { return "ReplaceBrush"; } }       
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: [block] [brush name] <brush args>",
            "&HDraws by replacing existing blocks that are the given [block] with the output of the given brush"
        };
        
        public override Brush Construct(BrushArgs args) {
            BlockID include = 0;
            Brush brush = ParseArguments(args, ref include); 
            
            if (brush == null) return null;
            return new ReplaceBrushBrush(include, brush);
        }
        
        protected Brush ParseArguments(BrushArgs args, ref BlockID target) {
            string[] parts = args.Message.SplitSpaces(3);
            Player p = args.Player;
            
            if (parts.Length < 2) { p.MessageLines(Help); return null; }
            if (!CommandParser.GetBlockIfAllowed(p, parts[0], "replace", out target)) return null;
            
            BrushFactory factory = BrushFactory.FindMatch(p, parts[1]);
            if (factory == null) return null;
            
            args.Message = parts.Length > 2 ? parts[2] : "";
            return factory.Construct(args);
        }
    }
    
    public class ReplaceNotBrushBrushFactory : ReplaceBrushBrushFactory 
    {
        public override string Name { get { return "ReplaceNotBrush"; } }        
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: [block] [brush name] <brush args>",
            "&HDraws by replacing existing blocks that not the given [block] with the output of the given brush"
        };    
                
        public override Brush Construct(BrushArgs args) {
            BlockID exclude = 0;
            Brush brush = ParseArguments(args, ref exclude); 
            
            if (brush == null) return null;
            return new ReplaceNotBrushBrush(exclude, brush);
        }
    }
}
