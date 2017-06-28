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
using System.Threading;
using MCGalaxy.Events;

namespace MCGalaxy.Games {
    public sealed class CountdownPlugin : Plugin_Simple {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.VersionString; } }
        public override string name { get { return "Core_CountdownPlugin"; } }
        public CountdownGame Game;

        public override void Load(bool startup) {
            OnPlayerMoveEvent.Register(HandlePlayerMove, Priority.High, this);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.High, this);
        }
        
        public override void Unload(bool shutdown) {
            OnPlayerMoveEvent.UnRegister(this);
            OnPlayerDisconnectEvent.UnRegister(this);
        }
        
        
        void HandlePlayerMove(Player p, Position next, byte yaw, byte pitch) {
            if (!p.InCountdown || Game.Status != CountdownGameStatus.InProgress || !Game.freezemode)
                return;
            
            if (p.CountdownSetFreezePos) {
                p.CountdownFreezeX = next.X;
                Thread.Sleep(100);
                p.CountdownFreezeZ = next.Z;
                Thread.Sleep(100);
                p.CountdownSetFreezePos = false;
            }
            
            if (next.X != p.CountdownFreezeX || next.Z != p.CountdownFreezeZ) {
                next.X = p.CountdownFreezeX; next.Z = p.CountdownFreezeZ;
                p.SendPos(Entities.SelfID, next, new Orientation(yaw, pitch));
            }
            
            p.Pos = next;
            p.SetYawPitch(yaw, pitch);
            Plugin.CancelPlayerEvent(PlayerEvents.PlayerMove, p);
        }
        
        void HandlePlayerDisconnect(Player p, string reason) {
            if (!Game.players.Contains(p)) return;
            
            if (Game.playersleftlist.Contains(p)) {
                Game.mapon.ChatLevel(p.ColoredName + " %Slogged out and so is out of countdown");
                Game.PlayerLeftGame(p);
            }
            Game.players.Remove(p);
        }
    }
}
