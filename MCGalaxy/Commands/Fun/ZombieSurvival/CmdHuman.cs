/*
    Copyright 2011 MCForge
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Games;
using MCGalaxy.Eco;

namespace MCGalaxy.Commands.Fun {    
    public sealed class CmdHuman : Command2 {
        public override string name { get { return "Human"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (p.Game.PledgeSurvive) {
                p.Message("You cannot un-pledge that you will be infected."); return;
            }
            if (ZSGame.Get(p).Infected) {
                p.Message("You cannot use &T/human &Sas you are currently infected."); return;
            }
            
            if (Economy.Enabled && p.money < 5) {
                p.Message("You need to have at least 5 &3" + Server.Config.Currency + 
                                   " &Sto pledge that you will not be infected."); return;
            }
            if (!ZSGame.Instance.RoundInProgress) {
                p.Message("Can only use &T/human &Swhen a round is in progress."); return;
            }
            
            TimeSpan delta = ZSGame.Instance.RoundEnd - DateTime.UtcNow;
            if (delta < TimeSpan.FromMinutes(3)) {
                p.Message("Cannot use &T/human &Sin last three minutes of a round."); return;
            }
            
            p.Game.PledgeSurvive = true;
            ZSGame.Instance.Map
                .Message(p.ColoredName + " &Spledges that they will not succumb to the infection!");
        }
        
        public override void Help(Player p) {
            p.Message("&T/Human &H- pledges that you will not be infected.");
            p.Message("&HIf you survive, you receive an &aextra 5 &3" + Server.Config.Currency);
            p.Message("&HHowever, if you are infected, you will &close 2 &3" + Server.Config.Currency);
        }
    }
}
