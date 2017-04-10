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
using MCGalaxy.Drawing.Transforms;

namespace MCGalaxy.Commands.Building {  
    public sealed class CmdTransform : Command {
        public override string name { get { return "transform"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("transforms", "list"), new CommandAlias("scale", "scale") }; }
        }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "") {
                Player.Message(p, "Your current transform is: " + p.Transform.Name); return;
            }
            string[] args = message.SplitSpaces(2);
            TransformFactory transform = TransformFactory.Find(args[0]);
            
            if (args[0].CaselessEq("list")) {
                Player.Message(p, "%HAvailable transforms: %S" + TransformFactory.Available);
            } else if (transform == null) {
                Player.Message(p, "No transform found with name \"{0}\".", args[0]);
                Player.Message(p, "Available transforms: " + TransformFactory.Available);
            } else {
                Player.Message(p, "Set your transform to: " + transform.Name);
                Transform instance = transform.Construct(p, args.Length == 1 ? "" : args[1]);
                if (instance == null) return;
                p.Transform = instance;
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/transform [name] <transform args>");
            Player.Message(p, "%HSets your current transform to the transform with that name.");
            Player.Message(p, "%T/help transform [name]");
            Player.Message(p, "%HOutputs the help for the transform with that name.");
            Player.Message(p, "%HAvailable transform: %S" + TransformFactory.Available);
        }
        
        public override void Help(Player p, string message) {
            TransformFactory transform = TransformFactory.Find(message);
            if (transform == null) {
                Player.Message(p, "No transform found with name \"{0}\".", message);
                Player.Message(p, "%HAvailable transforms: %S" + TransformFactory.Available);
            } else {
                Player.MessageLines(p, transform.Help);
            }
        }
    }
}
