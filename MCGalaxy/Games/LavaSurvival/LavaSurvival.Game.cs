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

namespace MCGalaxy.Games {
    
    public sealed partial class LavaSurvival : IGame {
        
        public override bool HandlesChatMessage(Player p, string message) {
            message = message.ToLower();
            if (HasPlayer(p) && HasVote(message)) {
                if (AddVote(p, message)) {
                    Player.Message(p, "Your vote for &5" + message.Capitalize() + " %Shas been placed. Thanks!");
                    return true;
                } else {
                    Player.Message(p, "&cYou already voted!");
                    return true;
                }
            }
            return false;
        }


        public override void PlayerJoinedServer(Player p) {
            if (!active) return;
            Player.Message(p, "There is a &aLava Survival %Sgame active! Join it by typing /ls go");
        }
        
        public override void PlayerJoinedLevel(Player p, Level lvl, Level oldLevl) {
            if (Server.lava.active && !Server.lava.sendingPlayers && Server.lava.map == lvl) {
                if (Server.lava.roundActive) {
                    Server.lava.AnnounceRoundInfo(p);
                    Server.lava.AnnounceTimeLeft(!Server.lava.flooded, true, p);
                } else {
                    Player.Message(p, "Vote for the next map!");
                    Player.Message(p, "Choices: " + Server.lava.VoteString);
                }
            }
        }
    }
}
