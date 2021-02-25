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
namespace MCGalaxy.Commands.Building {
    public sealed class CmdDelete : Command2 {        
        public override string name { get { return "Delete"; } }
        public override string shortcut { get { return "d"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data)  {
            if (message.Length > 0) { Help(p); return; }

            p.deleteMode = !p.deleteMode;
            p.Message("Delete mode: &a" + p.deleteMode);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Delete");
            p.Message("&HDeletes any block you click");
            p.Message("&H\"any block\" meaning Door_Air, Portals, MBs, etc");
        }
    }
}
