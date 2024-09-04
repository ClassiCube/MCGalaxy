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
using MCGalaxy;
using MCGalaxy.Drawing.Transforms;

namespace MCGalaxy.Commands.Building {  
    public sealed class CmdTransform : Command2 {
        public override string name { get { return "Transform"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Transforms", "list"), new CommandAlias("Scale", "scale") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                p.Message("Your current transform is: " + p.Transform.Name); return;
            }
        	
            string[] args = message.SplitSpaces(2);
            if (IsListAction(args[0])) {
                TransformFactory.List(p); return;
            }
            
            TransformFactory transform = TransformFactory.FindMatch(p, args[0]);
            if (transform == null) return;

            Transform instance = transform.Construct(p, args.Length == 1 ? "" : args[1]);
            if (instance == null) return;
            
            p.Message("Set your transform to: " + transform.Name);
            p.Transform = instance;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Transform [name] <transform args>");
            p.Message("&HSets your current transform to the transform with that name.");
            p.Message("&T/Help Transform [name]");
            p.Message("&HOutputs the help for the transform with that name.");
            TransformFactory.List(p);
        }
        
        public override void Help(Player p, string message) {
            TransformFactory transform = TransformFactory.FindMatch(p, message);
            if (transform == null) return;
            
            p.MessageLines(transform.Help);
        }
    }
}
