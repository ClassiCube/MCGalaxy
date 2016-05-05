/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Games;

namespace MCGalaxy.Commands {
    
    public sealed class CmdTime : Command {
        public override string name { get { return "time"; } }
        public override string shortcut { get { return "ti"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdTime() { }

        public override void Use(Player p, string message) {
            string time = DateTime.Now.ToString("HH:mm:ss"); //DateTime.Now.ToString();
            Player.Message(p, "Server time is " + time);
            if (!Server.zombie.Running) return;
            
            int delta = (int)(Server.zombie.RoundEnd - DateTime.UtcNow).TotalSeconds;
            if (delta > 0) {
                Player.Message(p, "&a" + delta + " %Sseconds until the round ends.");
            } else {
                delta = (int)(Server.zombie.RoundStart - DateTime.UtcNow).TotalSeconds;
                if (delta > 0)
                    Player.Message(p, "&a" + delta + " %Sseconds until the round starts.");
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/time - Shows the server time.");
            Player.Message(p, "If zombie survival is running, shows time left until round end or start.");
        }
    }
}
