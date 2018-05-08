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
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy.Games {
    
    public sealed partial class CountdownGame : IGame {
        
        void HookEventHandlers() {
            OnPlayerMoveEvent.Register(HandlePlayerMove, Priority.High);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.High);
            OnLevelUnloadEvent.Register(HandleLevelUnload, Priority.High);
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
        }
        
        void UnhookEventHandlers() {
            OnPlayerMoveEvent.Unregister(HandlePlayerMove);
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
            OnLevelUnloadEvent.Unregister(HandleLevelUnload);
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
        }
        
        
        void HandlePlayerMove(Player p, Position next, byte yaw, byte pitch) {
            if (Status != CountdownGameStatus.RoundInProgress || !FreezeMode) return;
            if (!Remaining.Contains(p)) return;
            
            if (next.X != p.CountdownFreezeX || next.Z != p.CountdownFreezeZ) {
                next.X = p.CountdownFreezeX; next.Z = p.CountdownFreezeZ;
                p.SendPos(Entities.SelfID, next, new Orientation(yaw, pitch));
            }
            
            p.Pos = next;
            p.SetYawPitch(yaw, pitch);
            p.cancelmove = true;
        }
        
        void HandlePlayerDisconnect(Player p, string reason) {
            if (!Players.Contains(p)) return;
            
            if (Remaining.Contains(p)) {
                Map.ChatLevel(p.ColoredName + " %Slogged out, and so is out of countdown");
                PlayerLeftGame(p);
            }
            Players.Remove(p);
        }
        
        void HandleLevelUnload(Level lvl) {
            if (Status == CountdownGameStatus.Disabled || lvl != Map) return;
            Disable();
        }
        
        void HandlePlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            if (!respawning || !Remaining.Contains(p)) return;
            PlayerDied(p);
        }
    }
}
