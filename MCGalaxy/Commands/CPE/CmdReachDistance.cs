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
using MCGalaxy.Network;

namespace MCGalaxy.Commands.CPE {
    public sealed class CmdReachDistance : Command2 {
        public override string name { get { return "ReachDistance"; } }
        public override string shortcut { get { return "Reach"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {          
            if (!p.Supports(CpeExt.ClickDistance)) {
                p.Message("Your client doesn't support changing your reach distance."); return;
            }
            if (message.Length == 0) {
                p.Message("Your reach distance is {0} blocks", p.ReachDistance); return;
            }
            
            float dist = 0;
            if (!CommandParser.GetReal(p, message, "Distance", ref dist, 0, 1024)) return;
            
            int packedDist = (int)(dist * 32);
            if (packedDist > short.MaxValue) {
                p.Message("\"{0}\", is too long a reach distance. Max is 1023 blocks.", message);
            } else {
                p.Send(Packet.ClickDistance((short)packedDist));
                p.ReachDistance = dist;
                p.Message("Set your reach distance to {0} blocks.", dist);
                Server.reach.Update(p.name, packedDist.ToString());
                Server.reach.Save();
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/ReachDistance [distance]");
            p.Message("&HSets the reach distance for how far away you can modify blocks.");
            p.Message("&H  The default reach distance is 5.");
        }
    }
}
