/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

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

namespace MCGalaxy.Games {
    
    public sealed partial class ZombieGame : IGame {
        
        public override bool HandlesManualChange(Player p, ushort x, ushort y, ushort z,
                                                 byte action, byte tile, byte b) {
            if (action == 1 && noPillaring && !p.referee) {
                if (p.lastYblock == y - 1 && p.lastXblock == x && p.lastZblock == z ) {
                    p.blocksStacked++;
                } else {
                    p.blocksStacked = 0;
                }
                
                if (p.blocksStacked == 2 ) {
                    p.SendMessage("You are pillaring! Stop before you get kicked!");
                }
                if (p.blocksStacked == 4 ) {
                    p.Kick("No pillaring allowed!"); return true;
                }
            }
            p.lastXblock = x; p.lastYblock = y; p.lastZblock = z;
            
            if (action == 1 || (action == 0 && p.painting)) {
                if (p.level.name != Server.zombie.currentLevelName || p.referee) return false;
                
                if (p.blockCount == 0 ) {
                    p.SendMessage("You have no blocks left.");
                    p.RevertBlock(x, y, z); return true;
                }

                p.blockCount--;
                if ((p.blockCount % 10) == 0 || (p.blockCount >= 0 && p.blockCount <= 10))
                    p.SendMessage("Blocks Left: " + Colors.maroon + p.blockCount);
            }
            return false;
        }
        
        public override bool HandlesMovement(Player p, ushort x, ushort y, ushort z,
                                             byte rotX, byte rotY) {
            if (!p.referee && noRespawn) {
                if (p.pos[0] >= x + 70 || p.pos[0] <= x - 70 ) {
                    p.SendPos(0xFF, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]);
                    return true;
                }
                if (p.pos[2] >= z + 70 || p.pos[2] <= z - 70) {
                    p.SendPos(0xFF, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]);
                    return true;
                }
            }
            return false;
        }
        
        public override bool HandlesChatMessage(Player p, string message) {
            message = message.ToLower();
            if (Player.CheckVote(message, p, "1", "one", ref Level1Vote) ||
                Player.CheckVote(message, p, "2", "two", ref Level2Vote) ||
                Player.CheckVote(message, p, "3", "three", ref Level3Vote))
                return true;
            
            if (!p.voice) {
                p.SendMessage("Chat moderation is on while voting is on!"); return true;
            }
            return false;
        }
        
        public override void PlayerLeftServer(Player p) {
            InfectedPlayerDC();
        }
        
        public override void PlayerJoinedServer(Player p) {
            if (Status == ZombieGameStatus.NotStarted) return;
            Player.SendMessage(p, "There is a Zombie Survival game currently in-progress! " +
                               "Join it by typing /g " + Server.zombie.currentLevelName);
        }
        
        public override void PlayerJoinedLevel(Player p, Level oldLevl) {
            if (Server.zombie.RoundInProgress && p.level.name == currentLevelName)
                Server.zombie.InfectedPlayerLogin(p);
            if (p.level.name == currentLevelName) {
            	double startLeft = (RoundStart - DateTime.UtcNow).TotalSeconds;
            	if (startLeft >= 0)
            		p.SendMessage("%a" + (int)startLeft + " %Sseconds left until the round starts. %aRun!");
                //p.SendMessage(CpeMessageType.BottomRight1, "%SYou have &a" + p.money + " %S" + Server.moneys);
                return;
            }
            
            p.SendMessage(CpeMessageType.BottomRight1, "");
            if(ZombieGame.alive.Contains(p))
                ZombieGame.alive.Remove(p);
            if (ZombieGame.infectd.Contains(p))
                ZombieGame.infectd.Remove(p);
        }
    }
}
