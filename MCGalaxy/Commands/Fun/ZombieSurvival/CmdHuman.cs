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

namespace MCGalaxy.Commands {    
    public sealed class CmdHuman : Command {
        public override string name { get { return "human"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        public CmdHuman() { }
        
        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (p.Game.PledgeSurvive) {
                Player.Message(p, "You cannot un-pledge that you will be infected."); return;
            }
            if (p.Game.Infected) {
                Player.Message(p, "You cannot use /human as you are currently infected."); return;
            }
            
            if (Economy.Enabled && p.money < 5) {
                Player.Message(p, "You need to have at least 5 &3" + Server.moneys + 
                                   " %Sto pledge that you will not be infected."); return;
            }
            if (!Server.zombie.RoundInProgress) {
                Player.Message(p, "Can only use /human when a round is in progress."); return;
            }
            
            TimeSpan delta = Server.zombie.RoundEnd - DateTime.UtcNow;
            if (delta < TimeSpan.FromMinutes(3)) {
                Player.Message(p, "Cannot use /human in last three minutes of a round."); return;
            }
            
            p.Game.PledgeSurvive = true;
            Server.zombie.CurLevel
                .ChatLevel(p.ColoredName + " %Spledges that they will not succumb to the infection!");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/human %H- pledges that you will not be infected.");
            Player.Message(p, "%HIf you survive, you receive an &aextra 5 %3" + Server.moneys);
            Player.Message(p, "%HHowever, if you are infected, you will &close 2 %3" + Server.moneys);
        }
    }
}
