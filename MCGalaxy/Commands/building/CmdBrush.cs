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
    public sealed class CmdBrush : Command2 {
        public override string name { get { return "Brush"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("Brushes", "list") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                p.Message("Your current brush is: " + p.BrushName); return;
            }
            string[] args = message.SplitSpaces(2);
            BrushFactory brush = BrushFactory.Find(args[0]);
            
            if (IsListCommand(args[0])) {
                List(p);
            } else if (brush == null) {
                p.Message("No brush found with name \"{0}\".", args[0]);
                List(p);
            } else {
                p.Message("Set your brush to: " + brush.Name);
                p.BrushName = brush.Name;
                p.DefaultBrushArgs = args.Length > 1 ? args[1] : "";
            }
        }
        
        internal static void List(Player p) {
            p.Message("%HAvailable brushes: &f" + BrushFactory.Brushes.Join(b => b.Name));
        }
        
        public override void Help(Player p) {
            p.Message("%T/Brush [name] <default brush args>");
            p.Message("%HSets your current brush to the brush with that name.");
            p.Message("%T/Help Brush [name]");
            p.Message("%HOutputs the help for the brush with that name.");
            List(p);
            p.Message("%H- If \"skip\" is used for a block name, " +
                           "existing blocks in the map will not be replaced by this block.");
        }

        public override void Help(Player p, string message) {
            BrushFactory brush = BrushFactory.Find(message);
            if (brush == null) {
                p.Message("No brush found with name \"{0}\".", message);
                List(p);
            } else {
                p.MessageLines(brush.Help);
            }
        }
    }
}
