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

namespace MCGalaxy.Commands {
    
    public sealed class CmdShowQueue : Command {
        
        public override string name { get { return "showqueue"; } }
        public override string shortcut { get { return "sq"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }      
        public CmdShowQueue() { }

        public override void Use(Player p, string message) {
            ShowQueued(p, Server.zombie.QueuedLevel, "level");
            ShowQueued(p, Server.zombie.QueuedZombie, "zombie");
        }
        
        void ShowQueued(Player p, string queued, string type) {
        	if (String.IsNullOrEmpty(queued))
                Player.Message(p, "There is no " + type + " queued.");
            else
                Player.Message(p, "\"" + queued + "\" is queued as the next " + type + ".");
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/showqueue");
            Player.Message(p, "%HLists the currently queued level and zombie.");
        }
    }
}
