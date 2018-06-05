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
namespace MCGalaxy.Commands.Fun {    
    public sealed class CmdInfect : Command {        
        public override string name { get { return "Infect"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }      
        
        public override void Use(Player p, string message) {
            Player who = message.Length == 0 ? p : PlayerInfo.FindMatches(p, message);
            if (who == null) return;
            
            if (!Server.zombie.RoundInProgress || Server.zombie.Get(who).Infected) {
                Player.Message(p, "Cannot infect player");
            } else if (!who.Game.Referee) {
                Server.zombie.InfectPlayer(who, p);
                Chat.MessageFrom(who, "λNICK %Swas infected.");
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Infect [name]");
            Player.Message(p, "%HTurns [name] into a zombie");
        }
    }
}
