/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.PlayerDBEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Network;

namespace MCGalaxy.Games {

    public abstract partial class RoundsGame : IGame {
        
        protected virtual void HookEventHandlers() {
            OnLevelUnloadEvent.Register(HandleLevelUnload, Priority.High);  
            OnSendingHeartbeatEvent.Register(HandleSendingHeartbeat, Priority.High);
            OnInfoSaveEvent.Register(HandleSaveStats, Priority.High);
            
            OnPlayerActionEvent.Register(HandlePlayerAction, Priority.High);
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.High);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.High);
        }
        
        protected virtual void UnhookEventHandlers() {
            OnLevelUnloadEvent.Unregister(HandleLevelUnload);
            OnSendingHeartbeatEvent.Unregister(HandleSendingHeartbeat);
            OnInfoSaveEvent.Unregister(HandleSaveStats);
            
            OnPlayerActionEvent.Unregister(HandlePlayerAction);
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
        }
        
        void HandleSaveStats(Player p, ref bool cancel) { SaveStats(p); }
        
        protected virtual void HandleSendingHeartbeat(Heartbeat service, ref string name) {
            if (Map == null || !GetConfig().MapInHeartbeat) return;
            name += " (map: " + Map.MapName + ")";
        }

        protected virtual void HandlePlayerConnect(Player p) {
            if (GetConfig().SetMainLevel) return;
            p.Message(WelcomeMessage);
        }
        
        protected virtual void HandlePlayerDisconnect(Player p, string reason) {
            if (p.level != Map) return;
            PlayerLeftGame(p);
        }
        
        protected void HandleJoinedCommon(Player p, Level prevLevel, Level level, ref bool announce) {
            if (prevLevel == Map && level != Map) {
                if (Picker.Voting) Picker.ResetVoteMessage(p);
                ResetStatus(p);
                PlayerLeftGame(p);
            } else if (level == Map) {
                if (Picker.Voting) Picker.SendVoteMessage(p);
                UpdateStatus1(p); UpdateStatus2(p); UpdateStatus3(p);
            }
            
            if (level != Map) return;
            
            if (prevLevel == Map || LastMap.Length == 0) {
                announce = false;
            } else if (prevLevel != null && prevLevel.name.CaselessEq(LastMap)) {
                // prevLevel is null when player joins main map
                announce = false;
            }
        }
        
        protected void HandleLevelUnload(Level lvl, ref bool cancel) {
            if (lvl != Map) return;
            Logger.Log(LogType.GameActivity, "Unload cancelled! A {0} game is currently going on!", GameName);
            cancel = true;
        }
        
        protected void HandlePlayerAction(Player p, PlayerAction action, string message, bool stealth) {
            if (!(action == PlayerAction.Referee || action == PlayerAction.UnReferee)) return;
            if (p.level != Map) return;
            
            if (action == PlayerAction.UnReferee) {
                PlayerActions.Respawn(p);
                PlayerJoinedGame(p);               
                p.Game.Referee = false;
            } else {
                PlayerLeftGame(p);
                p.Game.Referee = true;
                Entities.GlobalDespawn(p, false, false);
            }
            
            Entities.GlobalSpawn(p, false, "");
            TabList.Update(p, true);
        }
    }
}
