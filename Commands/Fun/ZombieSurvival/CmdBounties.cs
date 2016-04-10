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
using System.Collections.Generic;
using MCGalaxy.Games;

namespace MCGalaxy.Commands {
    
    public sealed class CmdBounties : Command {
        
        public override string name { get { return "bounties"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }      
        public CmdBounties() { }
        
        public override void Use(Player p, string message) {
            Dictionary<string, BountyData> bounties = Server.zombie.Bounties;
            if (bounties.Count == 0) {
                Player.SendMessage(p, "There are no active bounties.");
            } else {
                foreach (var pair in bounties) {
                    Player pl = PlayerInfo.FindExact(pair.Key);
                    if (pl == null) continue;
                    Player.SendMessage(p, "Bounty for " + pl.FullName + " %Sis &a" 
                                       + pair.Value.Amount + "%S" + Server.moneys + ".");
                }
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/bounties");
            Player.SendMessage(p, "%HOutputs a list of all active bounties on players.");
        }
    }
}
