/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands.Misc {
    public sealed class CmdBack : Command2 {
        public override string name { get { return "Back"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            if (p.PreTeleportMap == null) {
                p.Message("You have not teleported anywhere yet"); return;
            }
            
            if (!p.level.name.CaselessEq(p.PreTeleportMap))
                PlayerActions.ChangeMap(p, p.PreTeleportMap);
            p.SendPos(Entities.SelfID, p.PreTeleportPos, p.PreTeleportRot);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Back");
            p.Message("&HTakes you back to the position you were in before teleportation");
        }
    }
}
