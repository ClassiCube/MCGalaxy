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

namespace MCGalaxy.Commands.Fun {
    
    public sealed class CmdBounties : Command {
        
        public override string name { get { return "bounties"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        public CmdBounties() { }
        
        public override void Use(Player p, string message) {
            BountyData[] bounties = Server.zombie.Bounties.Items;
            if (bounties.Length == 0) {
                Player.Message(p, "There are no active bounties."); return;
            }
            
            foreach (BountyData bounty in bounties) {
                Player pl = PlayerInfo.FindExact(bounty.Target);
                if (pl == null) continue;
                Player.Message(p, "Bounty for {0} %Sis &a{1} %S{2}.", 
                               pl.ColoredName, bounty.Amount, Server.moneys);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/bounties");
            Player.Message(p, "%HOutputs a list of all active bounties on players.");
        }
    }
}
