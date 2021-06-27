/*
    Copyright 2011 MCForge
    
    Written by fenderrock87
        
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
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    public sealed partial class CTFGame : RoundsGame {

        protected override void HookEventHandlers() {
            OnPlayerDeathEvent.Register(HandlePlayerDeath, Priority.High);
            OnPlayerChatEvent.Register(HandlePlayerChat, Priority.High);
            OnPlayerCommandEvent.Register(HandlePlayerCommand, Priority.High);            
            OnBlockChangingEvent.Register(HandleBlockChanging, Priority.High);
            
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
            OnSentMapEvent.Register(HandleSentMap, Priority.High);
            OnJoinedLevelEvent.Register(HandleJoinedLevel, Priority.High);
            
            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnPlayerDeathEvent.Unregister(HandlePlayerDeath);
            OnPlayerChatEvent.Unregister(HandlePlayerChat);
            OnPlayerCommandEvent.Unregister(HandlePlayerCommand);           
            OnBlockChangingEvent.Unregister(HandleBlockChanging);
            
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
            OnSentMapEvent.Unregister(HandleSentMap);
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);
            
            base.UnhookEventHandlers();
        }
        
        
        void HandlePlayerDeath(Player p, BlockID deathblock) {
            if (p.level != Map || !Get(p).HasFlag) return;
            CtfTeam team = TeamOf(p);
            if (team != null) DropFlag(p, team);
        }
        
        void HandlePlayerChat(Player p, string message) {
            if (p.level != Map || !Get(p).TeamChatting) return;
            
            CtfTeam team = TeamOf(p);
            if (team == null) return;

            string prefix = team.Color + " - to " + team.Name;
            Chat.MessageChat(ChatScope.Level, p, prefix + " - λNICK: &f" + message,
                             Map, (pl, arg) => pl.Game.Referee || TeamOf(pl) == team);
            p.cancelchat = true;
        }
        
        void HandlePlayerCommand(Player p, string cmd, string args, CommandData data) {
            if (p.level != Map || cmd != "teamchat") return;
            CtfData data_ = Get(p);
            
            if (data_.TeamChatting) {
                p.Message("You are no longer chatting with your team!");
            } else {
                p.Message("You are now chatting with your team!");
            }
            
            data_.TeamChatting = !data_.TeamChatting;
            p.cancelcommand = true;
        }
        
        void HandleBlockChanging(Player p, ushort x, ushort y, ushort z, BlockID block, bool placing, ref bool cancel) {
            if (p.level != Map) return;
            CtfTeam team = TeamOf(p);
            if (team == null) {
                p.RevertBlock(x, y, z);
                cancel = true;
                p.Message("You are not on a team!");
                return;
            }
            
            Vec3U16 pos = new Vec3U16(x, y, z);
            if (pos == Opposing(team).FlagPos && !Map.IsAirAt(x, y, z)) {
                TakeFlag(p, team);
            }
            if (pos == team.FlagPos && !Map.IsAirAt(x, y, z)) {
                ReturnFlag(p, team);
                cancel = true;
            }
        }
        
        void HandlePlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            if (p.level != Map) return;
            CtfTeam team = TeamOf(p);
            
            if (team == null) return;
            if (respawning) DropFlag(p, team);
            
            Vec3U16 coords = team.SpawnPos;
            pos = Position.FromFeetBlockCoords(coords.X, coords.Y, coords.Z);
        }
        
        void HandleTabListEntryAdded(Entity entity, ref string tabName, ref string tabGroup, Player dst) {
            Player p = entity as Player;
            if (p == null || p.level != Map) return;
            CtfTeam team = TeamOf(p);
            
            if (p.Game.Referee) {
                tabGroup = "&2Referees";
            } else if (team != null) {
                tabGroup = team.ColoredName + " team";
            } else {
                tabGroup = "&7Spectators";
            }
        }
        
        void HandleSentMap(Player p, Level prevLevel, Level level) {
            if (level != Map) return;
            MessageMapInfo(p);
            if (TeamOf(p) == null) AutoAssignTeam(p);
        }
		
        void HandleJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
        }
    }
}
