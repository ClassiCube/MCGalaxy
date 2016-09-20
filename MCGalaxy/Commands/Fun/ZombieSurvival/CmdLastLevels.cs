/*
    Copyright 2015 MCGalaxy
 
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

namespace MCGalaxy.Commands {    
    public sealed class CmdLastLevels : Command {
        public override string name { get { return "lastlevels"; } }
        public override string shortcut { get { return "ll"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        
         public override void Use(Player p, string message) {
            List<string> recent = Server.zombie.RecentMaps;
            if (recent.Count == 0) {
                Player.Message(p, "No maps have been used yet.");
            } else {
                Player.Message(p, "Recent maps played (last map is most recent):");
                Player.Message(p, "&f" + recent.Join());
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/lastlevels");
            Player.Message(p, "%HShows the maps most recently used for zombie survival rounds.");
        }
    }
}
