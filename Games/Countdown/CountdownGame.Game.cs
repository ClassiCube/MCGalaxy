/*
    Copyright 2011 MCGalaxy
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace MCGalaxy {
    
    public sealed partial class CountdownGame : IGame {
        
        public override void PlayerJoinedGame(Player p) {
            if (!Server.Countdown.players.Contains(p)) {
                Server.Countdown.players.Add(p);
                Player.SendMessage(p, "You've joined the Countdown game!!");
                Player.GlobalMessage(p.name + " has joined Countdown!!");
                if (p.level != Server.Countdown.mapon)
                	Command.all.Find("goto").Use(p, "countdown");
                p.playerofcountdown = true;
            } else {
                Player.SendMessage(p, "Sorry, you have already joined!!, to leave please type /countdown leave");
            }
        }
        
        public override void PlayerLeftServer(Player p) {
            if (players.Contains(p)) {
                if (playersleftlist.Contains(p)) {
                    mapon.ChatLevel(p.color + p.name + Server.DefaultColor + " logged out and so is out of countdown!!");
                    PlayerLeftGame(p);
                }
                players.Remove(p);
            }
        }
        
        public override void PlayerLeftGame(Player p) {
            p.incountdown = false;
            p.playerofcountdown = false;
            players.Remove(p);
            playersleftlist.Remove(p);
            MessagePlayersLeft();
        }
        
        public override bool HandlesMovement(Player p, ushort x, ushort y, ushort z,
                                             byte rotX, byte rotY) {
            if( !p.incountdown || gamestatus != CountdownGameStatus.InProgress || !freezemode)
                return false;          
            if (p.countdownsettemps) {
                p.countdowntempx = x;
                Thread.Sleep(100);
                p.countdowntempz = z;
                Thread.Sleep(100);
                p.countdownsettemps = false;
            }
            
            if (x != p.countdowntempx || z != p.countdowntempz) {
                x = p.countdowntempx; z = p.countdowntempz;
                p.SendPos(0xFF, x, (ushort)(y - 22), z, rotX, rotY);
            }
            p.pos[0] = x; p.pos[1] = y; p.pos[2] = z;
            p.rot[0] = rotX; p.rot[1] = rotY;
            return true;
        }
    }
}
