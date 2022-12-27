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

namespace MCGalaxy.Commands.Fun {    
    public sealed class CmdLastLevels : Command2 {
        public override string name { get { return "LastLevels"; } }
        public override string shortcut { get { return "ll"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        
        public override void Use(Player p, string message, CommandData data) {
            List<string> recent = ZSGame.Instance.Picker.RecentMaps;
            if (recent.Count == 0) {
                p.Message("No maps have been used yet.");
            } else {
                p.Message("Recent maps played (last map is most recent):");
                p.Message("&f" + recent.Join());
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/LastLevels");
            p.Message("&HShows the maps most recently used for zombie survival rounds.");
        }
    }
}
