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
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.LevelEvents;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    public sealed partial class LSGame : RoundsGame {

        void HookEventHandlers() {
            OnLevelUnloadEvent.Register(HandleLevelUnload, Priority.High);
            OnJoinedLevelEvent.Register(HandleOnJoinedLevel, Priority.High);
            
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.High);
            OnPlayerDeathEvent.Register(HandlePlayerDeath, Priority.High);
        }
        
        void UnhookEventHandlers() {
            OnLevelUnloadEvent.Unregister(HandleLevelUnload);
            OnJoinedLevelEvent.Unregister(HandleOnJoinedLevel);
            
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDeathEvent.Unregister(HandlePlayerDeath);
        }
        
        void HandleOnJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
            if (Map != level || !RoundInProgress) return;
            MessageRoundStatus(p);
        }

        void HandlePlayerConnect(Player p) {
            Player.Message(p, "&cLava Survival %Sis running! Type %T/ls go %Sto join");
        }
        
        void HandlePlayerDeath(Player p, BlockID block) {
            if (p.level != Map) return;
         
            if (IsPlayerDead(p)) {
                p.cancelDeath = true;
            } else {
                KillPlayer(p);
            }
        }
    }
}
