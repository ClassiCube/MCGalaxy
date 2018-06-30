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
///////--|----------------------------------|--\\\\\\\
//////---|  TNT WARS - Coded by edh649      |---\\\\\\
/////----|                                  |----\\\\\
////-----|  Note: Double click on // to see |-----\\\\
///------|        them in the sidebar!!     |------\\\
//-------|__________________________________|-------\\
using System;
using MCGalaxy.Maths;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy.Games {
    
    public sealed partial class TntWarsGame : RoundsGame {
        
         protected override void HookEventHandlers() {
            OnPlayerChatEvent.Unregister(HandlePlayerChat);
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
            OnJoinedLevelEvent.Register(HandleJoinedLevel, Priority.High);
            
            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnPlayerChatEvent.Unregister(HandlePlayerChat);
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);
           
            base.UnhookEventHandlers();
        }
        
        void HandlePlayerChat(Player p, string message) {
            if (Picker.HandlesMessage(p, message)) { p.cancelchat = true; return; }
            if (p.level != Map || message.Length == 0 || message[0] != ':') return;
            
            TntWarsTeam team = TeamOf(p);
            if (team == null || GameMode != TntWarsGameMode.TDM) return;
            message = message.Substring(1);
            
            // "To Team &c-" + ColoredNamw + "&c- %S" + message);
            string prefix = team.Color + " - to " + team.Name;
            Chat.MessageChat(ChatScope.Level, p, prefix + " - λNICK: &f" + message,
                             Map, (pl, arg) => pl.Game.Referee || TeamOf(pl) == team);
            p.cancelchat = true;
        }
        
        void HandlePlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            if (p.level != Map) return;
            TntWarsTeam team = TeamOf(p);           
            if (team == null || GameMode != TntWarsGameMode.TDM) return;
            
            Vec3U16 coords = team.SpawnPos;
            pos = Position.FromFeetBlockCoords(coords.X, coords.Y, coords.Z);
        }
        
        void HandleJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
            
            if (level != Map) return;
            MessageMapInfo(p);
            if (TeamOf(p) != null) return;
            
            if (Blue.Members.Count > Red.Members.Count) {
                JoinTeam(p, Red);
            } else if (Red.Members.Count > Blue.Members.Count) {
                JoinTeam(p, Blue);
            } else if (Red.Score > Blue.Score) {
                JoinTeam(p, Blue);
            } else if (Blue.Score > Red.Score) {
                JoinTeam(p, Blue);
            } else {
                bool red = new Random().Next(2) == 0;
                JoinTeam(p, red ? Red : Blue);
            }
        }
    }
}
