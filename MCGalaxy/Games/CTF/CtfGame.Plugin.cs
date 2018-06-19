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
            
            OnBlockChangeEvent.Register(HandleBlockChange, Priority.High);
            OnPlayerDisconnectEvent.Register(HandleDisconnect, Priority.High);
            
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
            OnJoinedLevelEvent.Register(HandleOnJoinedLevel, Priority.High);
            
            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnPlayerDeathEvent.Unregister(HandlePlayerDeath);
            OnPlayerChatEvent.Unregister(HandlePlayerChat);
            OnPlayerCommandEvent.Unregister(HandlePlayerCommand);
            
            OnBlockChangeEvent.Unregister(HandleBlockChange);
            OnPlayerDisconnectEvent.Unregister(HandleDisconnect);
            
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
            OnJoinedLevelEvent.Unregister(HandleOnJoinedLevel);
            
            base.UnhookEventHandlers();
        }
        
        
        void HandlePlayerDeath(Player p, BlockID deathblock) {
            if (p.level != Map || !Get(p).HasFlag) return;
            CtfTeam team = TeamOf(p);
            if (team != null) DropFlag(p, team);
        }
        
        void HandlePlayerChat(Player p, string message) {
            if (Picker.HandlesMessage(p, message)) { p.cancelchat = true; return; }
            if (p.level != Map || !Get(p).TeamChatting) return;
            
            CtfTeam team = TeamOf(p);
            if (team == null) return;
            Player[] members = team.Members.Items;
            
            foreach (Player pl in members) {
                Player.Message(pl, "({0}) {1}: &f{2}", team.Name, p.ColoredName, message);
            }
            p.cancelchat = true;
        }
        
        void HandlePlayerCommand(Player p, string cmd, string args) {
            if (p.level != Map || cmd != "teamchat") return;
            CtfData data = Get(p);
            
            if (data.TeamChatting) {
                Player.Message(p, "You are no longer chatting with your team!");
            } else {
                Player.Message(p, "You are now chatting with your team!");
            }
            
            data.TeamChatting = !data.TeamChatting;
            p.cancelcommand = true;
        }
        
        void HandleBlockChange(Player p, ushort x, ushort y, ushort z, BlockID block, bool placing) {
            if (p.level != Map) return;
            CtfTeam team = TeamOf(p);
            if (team == null) {
                p.RevertBlock(x, y, z);
                Player.Message(p, "You are not on a team!");
                p.cancelBlock = true;
                return;
            }
            
            Vec3U16 pos = new Vec3U16(x, y, z);
            if (pos == Opposing(team).FlagPos && !Map.IsAirAt(x, y, z)) {
                TakeFlag(p, team);
            }
            if (pos == team.FlagPos && !Map.IsAirAt(x, y, z)) {
                ReturnFlag(p, team);
            }
        }
        
        void HandleDisconnect(Player p, string reason) {
            if (p.level != Map) return;
            PlayerLeftGame(p);
        }
        
        void HandlePlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            if (p.level != Map) return;
            CtfTeam team = TeamOf(p);
            
            if (team != null) pos = team.SpawnPos;
            if (team != null && respawning) DropFlag(p, team);
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
        
        void HandleOnJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
            if (prevLevel == Map && level != Map) {
                PlayerLeftGame(p);
            } else if (level != Map) { return; }
            if (TeamOf(p) != null) return;
            
            if (Blue.Members.Count > Red.Members.Count) {
                JoinTeam(p, Red);
            } else if (Red.Members.Count > Blue.Members.Count) {
                JoinTeam(p, Blue);
            } else if (new Random().Next(2) == 0) {
                JoinTeam(p, Red);
            } else {
                JoinTeam(p, Blue);
            }
        }
    }
}
