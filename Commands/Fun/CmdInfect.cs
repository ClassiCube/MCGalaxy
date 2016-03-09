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
    
    public sealed class CmdInfect : Command {
        
        public override string name { get { return "infect"; } }
        public override string shortcut { get { return "i"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdInfect() { }
        
        public override void Use(Player p, string message) {
            Player who = message == "" ? p : PlayerInfo.Find(message);
            if (who == null) {
                Player.SendMessage(p, "There is no player \"" + message + "\"!"); return;
            }
            
            if (who.infected || !Server.zombie.GameInProgess()) {
                Player.SendMessage(p, "Cannot infect player");
            } else if (!who.referee) {
                Server.zombie.InfectPlayer(who);
                Player.GlobalMessage(who.color + who.DisplayName + Server.DefaultColor + " was infected!");
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/infect [name] - infects [name]");
        }
    }
}
