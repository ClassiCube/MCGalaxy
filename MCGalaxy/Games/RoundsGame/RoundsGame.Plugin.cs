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
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Network;

namespace MCGalaxy.Games {

    public abstract partial class RoundsGame : IGame {
        
        protected virtual void HookEventHandlers() {
            OnLevelUnloadEvent.Register(HandleLevelUnload, Priority.High);  
            OnSendingHeartbeatEvent.Register(HandleSendingHeartbeat, Priority.High);
            OnSQLSaveEvent.Register(SaveStats, Priority.High);
            
            OnPlayerActionEvent.Register(HandlePlayerAction, Priority.High);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.High);
        }
        
        protected virtual void UnhookEventHandlers() {
            OnLevelUnloadEvent.Unregister(HandleLevelUnload);
            OnSendingHeartbeatEvent.Unregister(HandleSendingHeartbeat);
            OnSQLSaveEvent.Unregister(SaveStats);
            
            OnPlayerActionEvent.Unregister(HandlePlayerAction);            
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
        }
        
        protected virtual void HandleSendingHeartbeat(Heartbeat service, ref string name) {
            if (Map == null || !GetConfig().MapInHeartbeat) return;
            name += " (map: " + Map.MapName + ")";
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
            if (prevLevel == Map || LastMap.Length == 0 || prevLevel.name.CaselessEq(LastMap))
                announce = false;
        }
        
        protected void MessageMapInfo(Player p) {
            p.Message("This map has &a{0} likes %Sand &c{1} dislikes",
                           Map.Config.Likes, Map.Config.Dislikes);
            string[] authors = Map.Config.Authors.SplitComma();
            if (authors.Length == 0) return;
            
            p.Message("It was created by {0}",
                           authors.Join(n => PlayerInfo.GetColoredName(p, n)));
        }
        
        protected void HandleLevelUnload(Level lvl) {
            if (lvl != Map) return;
            Logger.Log(LogType.GameActivity, "Unload cancelled! A {0} game is currently going on!", GameName);
            lvl.cancelunload = true;
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
