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
    public sealed class CmdHuman : Command {
        public override string name { get { return "Human"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message) {
            if (p.Game.PledgeSurvive) {
                Player.Message(p, "You cannot un-pledge that you will be infected."); return;
            }
            if (ZSGame.Get(p).Infected) {
                Player.Message(p, "You cannot use %T/human %Sas you are currently infected."); return;
            }
            
            if (Economy.Enabled && p.money < 5) {
                Player.Message(p, "You need to have at least 5 &3" + ServerConfig.Currency + 
                                   " %Sto pledge that you will not be infected."); return;
            }
            if (!ZSGame.Instance.RoundInProgress) {
                Player.Message(p, "Can only use %T/human %Swhen a round is in progress."); return;
            }
            
            TimeSpan delta = ZSGame.Instance.RoundEnd - DateTime.UtcNow;
            if (delta < TimeSpan.FromMinutes(3)) {
                Player.Message(p, "Cannot use %T/human %Sin last three minutes of a round."); return;
            }
            
            p.Game.PledgeSurvive = true;
            ZSGame.Instance.Map
                .Message(p.ColoredName + " %Spledges that they will not succumb to the infection!");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Human %H- pledges that you will not be infected.");
            Player.Message(p, "%HIf you survive, you receive an &aextra 5 &3" + ServerConfig.Currency);
            Player.Message(p, "%HHowever, if you are infected, you will &close 2 &3" + ServerConfig.Currency);
        }
    }
}
