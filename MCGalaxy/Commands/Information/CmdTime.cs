/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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

namespace MCGalaxy.Commands.Info 
{
    public sealed class CmdTime : Command2 
    {
        public override string name { get { return "Time"; } }
        public override string shortcut { get { return "ti"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            p.Message("Server time: {0:HH:mm:ss} on {0:yyyy-MM-dd}", DateTime.Now);
            IGame game = IGame.GameOn(p.level);
            if (game != null) game.OutputTimeInfo(p);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Time");
            p.Message("&HShows the server time.");
            p.Message("&HIf a time limit round-based game is running on the level you are currently on, " +
                      "shows time left until round end or start.");
        }
    }
}
