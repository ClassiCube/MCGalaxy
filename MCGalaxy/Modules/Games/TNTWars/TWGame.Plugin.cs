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
using System.Collections.Generic;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy.Modules.Games.TW
{    
    public sealed partial class TWGame : RoundsGame 
    {    
        protected override void HookEventHandlers() {
            OnPlayerChatEvent.Register(HandlePlayerChat, Priority.High);
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
            OnSentMapEvent.Register(HandleSentMap, Priority.High);
            OnJoinedLevelEvent.Register(HandleJoinedLevel, Priority.High);
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
            OnSettingColorEvent.Register(HandleSettingColor, Priority.High);
            OnBlockHandlersUpdatedEvent.Register(HandleBlockHandlersUpdated, Priority.High);
            
            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnPlayerChatEvent.Unregister(HandlePlayerChat);
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
            OnSentMapEvent.Unregister(HandleSentMap);
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
            OnSettingColorEvent.Unregister(HandleSettingColor);
            OnBlockHandlersUpdatedEvent.Unregister(HandleBlockHandlersUpdated);
            
            base.UnhookEventHandlers();
        }
        
        void HandlePlayerChat(Player p, string message) {
            if (p.level != Map || message.Length == 0 || message[0] != ':') return;
            
            TWTeam team = TeamOf(p);
            if (team == null || Config.Mode != TWGameMode.TDM) return;
            message = message.Substring(1);
            
            // "To Team &c-" + ColoredName + "&c- &S" + message);
            string prefix = team.Color + " - to " + team.Name;
            Chat.MessageChat(ChatScope.Level, p, prefix + " - λNICK: &f" + message,
                             Map, (pl, arg) => pl.Game.Referee || TeamOf(pl) == team);
            p.cancelchat = true;
        }
        
        void HandlePlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            if (p.level != Map) return;
            
            TWData data = Get(p);
            if (respawning) {
                data.Health = 2;
                data.KillStreak = 0;
                data.ScoreMultiplier = 1f;              
                data.LastKillStreakAnnounced = 0;
            }
            
            TWTeam team = TeamOf(p);
            if (team == null || Config.Mode != TWGameMode.TDM) return;
            
            Vec3U16 coords = team.SpawnPos;
            pos = Position.FromFeetBlockCoords(coords.X, coords.Y, coords.Z);
        }
        
        void HandleTabListEntryAdded(Entity entity, ref string tabName, ref string tabGroup, Player dst) {
            Player p = entity as Player;
            if (p == null || p.level != Map) return;
            TWTeam team = TeamOf(p);
            
            if (p.Game.Referee) {
                tabGroup = "&2Referees";
            } else if (team != null) {
                tabGroup = team.ColoredName + " team";
            } else {
                tabGroup = "&7Spectators";
            }
        }
        
        void HandleSettingColor(Player p, ref string color) {
            if (p.level != Map) return;
            TWTeam team = TeamOf(p);
            if (team != null) color = team.Color;
        }
        
        void HandleSentMap(Player p, Level prevLevel, Level level) {
            if (level != Map) return;
            OutputMapSummary(p, Map.Config);
            if (TeamOf(p) == null) AutoAssignTeam(p);
        }
        
        void HandleJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
            if (level == Map) allPlayers.Add(p);
        }
    }
}
