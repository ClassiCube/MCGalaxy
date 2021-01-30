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
using MCGalaxy.Drawing.Transforms;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdDoNotMark : Command2 {
        public override string name { get { return "DoNotMark"; } }
        public override string shortcut { get { return "dnm"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases { get { return new[] { new CommandAlias("dm") }; } }

        public override void Use(Player p, string message, CommandData data) {
           p.ClickToMark = !p.ClickToMark;
           p.Message("Click blocks to &T/mark&S: {0}", p.ClickToMark ? "&2ON" : "&4OFF");
        }
        
        public override void Help(Player p) {
            p.Message("&T/DoNotMark");
            p.Message("&HToggles whether clicking blocks adds a marker to a selection. (e.g. &T/cuboid&H)");
        }
    }
}
