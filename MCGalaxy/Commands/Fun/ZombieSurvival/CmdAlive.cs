/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
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
    public sealed class CmdAlive : Command2 {
        public override string name { get { return "Alive"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        
        public override void Use(Player p, string message, CommandData data) {
            List<Player> alive = PlayerInfo.OnlyCanSee(p, data.Rank, 
                                                       ZSGame.Instance.Alive.Items);
            if (alive.Count == 0) { p.Message("No one is alive."); return; }
            
            p.Message("Players who are &2alive &Sare:");
            p.Message(alive.Join(pl => pl.ColoredName));
        }
        
        public override void Help(Player p) {
            p.Message("&T/Alive");
            p.Message("&HShows who is alive/a human");
        }
    }
}
