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
using MCGalaxy;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Commands.Building {  
    public sealed class CmdBrush : Command {
        public override string name { get { return "brush"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("brushes", "list") }; }
        }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "") {
                Player.Message(p, "Your current brush is: " + p.BrushName); return;
            }
            string[] args = message.SplitSpaces(2);
            BrushFactory brush = BrushFactory.Find(args[0]);
            
            if (args[0].CaselessEq("list")) {
                Player.Message(p, "%HAvailable brushes: %S" + BrushFactory.Available);
            } else if (brush == null) {
            	Player.Message(p, "No brush found with name \"{0}\".", args[0]);
                Player.Message(p, "Available brushes: " + BrushFactory.Available);
            } else {
                Player.Message(p, "Set your brush to: " + brush.Name);
                p.BrushName = brush.Name;
                p.DefaultBrushArgs = args.Length > 1 ? args[1] : "";
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/brush [name] <default brush args>");
            Player.Message(p, "%HSets your current brush to the brush with that name.");
            Player.Message(p, "%T/help brush [name]");
            Player.Message(p, "%HOutputs the help for the brush with that name.");
            Player.Message(p, "%HAvailable brushes: %S" + BrushFactory.Available);
            Player.Message(p, "%H- The default brush takes one argument specifying the block to draw with. " +
                           "If no arguments are given, draws with your currently held block.");
            Player.Message(p, "%H- If \"skip\" is used for a block name, " +
                           "existing blocks in the map will not be replaced by this block.");
        }
        
        public override void Help(Player p, string message) {
            BrushFactory brush = BrushFactory.Find(message);
            if (brush == null) {
                Player.Message(p, "No brush found with name \"{0}\".", message);
                Player.Message(p, "%HAvailable brushes: %S" + BrushFactory.Available);
            } else {
                Player.MessageLines(p, brush.Help);
            }
        }
    }
}
