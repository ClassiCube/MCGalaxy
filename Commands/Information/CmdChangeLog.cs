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
using System;
using System.IO;

namespace MCGalaxy.Commands {
    public sealed class CmdChangeLog : Command {
        public override string name { get { return "changelog"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        
        public override void Use(Player p, string message) {
            if (!File.Exists("changelog.txt")) { Player.Message(p, "Unable to find changelog"); return; }
            
            using (StreamReader r = new StreamReader("changelog.txt")) {
                string line = null;
                while ((line = r.ReadLine()) != null) {
                    // a blank line is used to separate the changelog for different versions
                    if (line == "") return;
                    Player.Message(p, "&f" + line);
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/changelog");
            Player.Message(p, "%HView the most recent changelog for MCGalaxy.");
        }
    }
}
