﻿/*
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
        public override string name { get { return "Brush"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("Brushes", "list") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) {
                Player.Message(p, "Your current brush is: " + p.BrushName); return;
            }
            string[] args = message.SplitSpaces(2);
            BrushFactory brush = BrushFactory.Find(args[0]);
            
            if (args[0].CaselessEq("list")) {
                List(p);
            } else if (brush == null) {
                Player.Message(p, "No brush found with name \"{0}\".", args[0]);
                List(p);
            } else {
                Player.Message(p, "Set your brush to: " + brush.Name);
                p.BrushName = brush.Name;
                p.DefaultBrushArgs = args.Length > 1 ? args[1] : "";
            }
        }
        
        internal static void List(Player p) {
            Player.Message(p, "%HAvailable brushes: &f" + BrushFactory.Brushes.Join(b => b.Name));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Brush [name] <default brush args>");
            Player.Message(p, "%HSets your current brush to the brush with that name.");
            Player.Message(p, "%T/Help Brush [name]");
            Player.Message(p, "%HOutputs the help for the brush with that name.");
            List(p);
            Player.Message(p, "%H- If \"skip\" is used for a block name, " +
                           "existing blocks in the map will not be replaced by this block.");
        }

        public override void Help(Player p, string message) {
            BrushFactory brush = BrushFactory.Find(message);
            if (brush == null) {
                Player.Message(p, "No brush found with name \"{0}\".", message);
                List(p);
            } else {
                Player.MessageLines(p, brush.Help);
            }
        }
    }
}
