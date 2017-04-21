/*
    Copyright 2011 MCForge
        
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
using System.Threading;

namespace MCGalaxy.Games {    
    public sealed partial class CountdownGame : IGame {
        
        public override void PlayerJoinedGame(Player p) {
            if (!Server.Countdown.players.Contains(p)) {
                Server.Countdown.players.Add(p);
                Player.Message(p, "You've joined the Countdown game!!");
                Chat.MessageGlobal("{0} has joined Countdown!!", p.name);
                if (p.level != Server.Countdown.mapon)
                    PlayerActions.ChangeMap(p, "countdown");
                p.playerofcountdown = true;
            } else {
                Player.Message(p, "Sorry, you have already joined!!, to leave please type /countdown leave");
            }
        }
        
        public override void PlayerLeftServer(Player p) {
            if (players.Contains(p)) {
                if (playersleftlist.Contains(p)) {
                    mapon.ChatLevel(p.ColoredName + " %Slogged out and so is out of countdown!!");
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
        
        public override bool HandlesMovement(Player p, Position next, byte yaw, byte pitch) {
            if( !p.incountdown || gamestatus != CountdownGameStatus.InProgress || !freezemode)
                return false;
            if (p.countdownsettemps) {
                p.countdowntempx = next.X;
                Thread.Sleep(100);
                p.countdowntempz = next.Z;
                Thread.Sleep(100);
                p.countdownsettemps = false;
            }
            
            if (next.X != p.countdowntempx || next.Z != p.countdowntempz) {
                next.X = p.countdowntempx; next.Z = p.countdowntempz;
                p.SendPos(Entities.SelfID, next, new Orientation(yaw, pitch));
            }
            
            p.Pos = next;
            p.SetYawPitch(yaw, pitch);
            return true;
        }
    }
}
