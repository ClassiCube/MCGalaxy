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
using MCGalaxy.Games;

namespace MCGalaxy.Commands {
    
    public sealed class CmdHuman : Command {
        public override string name { get { return "human"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        public CmdHuman() { }
        
        public override void Use(Player p, string message) {
        	if (p == null) { MessageInGameOnly(p); return; }
        	if (p.Game.PledgeSurvive) {
        		Player.SendMessage(p, "You cannot un-pledge that you will be infected."); return;
        	}
        	
        	p.Game.PledgeSurvive = true;
        	Server.zombie.CurLevel
        		.ChatLevel(p.ColoredName + " %Spledges that they will not succumb to the infection!");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/human %H- pledges that you will not be infected.");
            Player.SendMessage(p, "%HIf you survive, you receive an &aextra 5 %3" + Server.moneys);
            Player.SendMessage(p, "%HHowever, if you are infected, you will &close 2 %3" + Server.moneys);
        }
    }
}
