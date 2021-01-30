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
using System;
using System.IO;
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Fun {    
    public sealed class CmdShowQueue : Command2 {      
        public override string name { get { return "ShowQueue"; } }
        public override string shortcut { get { return "sq"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }

        public override void Use(Player p, string message, CommandData data) {
            ShowQueued(p, ZSGame.Instance.Picker.QueuedMap, "level");
            ShowQueued(p, ZSGame.Instance.QueuedZombie, "zombie");
        }
        
        void ShowQueued(Player p, string queued, string type) {
            if (String.IsNullOrEmpty(queued))
                p.Message("There is no " + type + " queued.");
            else
                p.Message("\"" + queued + "\" is queued as the next " + type + ".");
        }

        public override void Help(Player p) {
            p.Message("&T/ShowQueue");
            p.Message("&HLists the currently queued level and zombie.");
        }
    }
}
