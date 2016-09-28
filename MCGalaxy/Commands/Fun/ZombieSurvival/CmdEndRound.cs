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
namespace MCGalaxy.Commands {
    public sealed class CmdEndRound : Command {
        public override string name { get { return "endround"; } }
        public override string shortcut { get { return "er"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        public CmdEndRound() { }
        
        public override void Use(Player p, string message) {
            if (Server.zombie.RoundInProgress)
            	Server.zombie.EndRound();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/endround");
            Player.Message(p, "%HEnds the current round of zombie survival");
        }
    }
}
