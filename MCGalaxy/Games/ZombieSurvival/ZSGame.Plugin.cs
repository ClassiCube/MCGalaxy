/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Data;
using MCGalaxy.Blocks;
using MCGalaxy.Events;
using MCGalaxy.Events.EconomyEvents;
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Games.ZS;
using MCGalaxy.Network;
using MCGalaxy.SQL;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    public sealed partial class ZSGame : RoundsGame {

        void HookEventHandlers() {
            OnEntitySpawnedEvent.Register(HandleEntitySpawned, Priority.High);
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
            OnMoneyChangedEvent.Register(HandleMoneyChanged, Priority.High);
            OnBlockChangeEvent.Register(HandleBlockChange, Priority.High);
            OnLevelUnloadEvent.Register(HandleLevelUnload, Priority.High);
            OnSendingHeartbeatEvent.Register(HandleSendingHeartbeat, Priority.High);
            
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.High);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.High);
            OnPlayerMoveEvent.Register(HandlePlayerMove, Priority.High);
            OnPlayerActionEvent.Register(HandlePlayerAction, Priority.High);
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
            OnJoinedLevelEvent.Register(HandleJoinedLevel, Priority.High);
            
            OnSQLSaveEvent.Register(SavePlayerStats, Priority.High);
        }
        
        void UnhookEventHandlers() {
            OnEntitySpawnedEvent.Unregister(HandleEntitySpawned);
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
            OnMoneyChangedEvent.Unregister(HandleMoneyChanged);
            OnBlockChangeEvent.Unregister(HandleBlockChange);
            OnLevelUnloadEvent.Unregister(HandleLevelUnload);
            OnSendingHeartbeatEvent.Unregister(HandleSendingHeartbeat);
            
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
            OnPlayerMoveEvent.Unregister(HandlePlayerMove);
            OnPlayerActionEvent.Unregister(HandlePlayerAction);
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);
            
            OnSQLSaveEvent.Unregister(SavePlayerStats);
        }

        
        void HandleTabListEntryAdded(Entity entity, ref string tabName, ref string tabGroup, Player dst) {
            Player p = entity as Player;
            if (p == null || p.level != Map) return;
            
            if (p.Game.Referee) {
                tabGroup = "&2Referees";
            } else if (Get(p).Infected) {
                tabGroup = "&cZombies";
                if (ZSConfig.ZombieName.Length > 0 && !Get(dst).AkaMode) {
                    tabName = "&c" + ZSConfig.ZombieName;
                } else {
                    tabName = "&c" + p.truename;
                }
            } else {
                tabGroup = "&fHumans";
            }
        }
        
        void HandleMoneyChanged(Player p) {
            if (p.level != Map) return;
            HUD.UpdateTertiary(p, Get(p).Infected);
        }
        
        void HandleEntitySpawned(Entity entity, ref string name, ref string skin, ref string model, Player dst) {
            Player p = entity as Player;
            if (p == null || !Get(p).Infected) return;

            name = p.truename;
            if (ZSConfig.ZombieName.Length > 0 && !Get(dst).AkaMode) {
                name = ZSConfig.ZombieName; skin = name;
            }
            name = Colors.red + name;
            model = p == dst ? p.Model : ZSConfig.ZombieModel;
        }
        
        void HandlePlayerConnect(Player p) {
            if (ZSConfig.SetMainLevel) return;
            Player.Message(p, "&3Zombie Survival %Sis running! Type %T/ZS go %Sto join");
        }
        
        void HandlePlayerDisconnect(Player p, string reason) {
            PlayerLeftGame(p);
        }
        
        void HandlePlayerMove(Player p, Position next, byte rotX, byte rotY) {
            if (!RoundInProgress || p.level != Map) return;
            
            bool reverted = MovementCheck.DetectNoclip(p, next)
                || MovementCheck.DetectSpeedhack(p, next, ZSConfig.MaxMoveDistance);
            if (reverted) p.cancelmove = true;
        }
        
        void HandlePlayerAction(Player p, PlayerAction action, string message, bool stealth) {
            if (!(action == PlayerAction.Referee || action == PlayerAction.UnReferee)) return;
            if (p.level != Map) return;
            
            if (action == PlayerAction.UnReferee) {
                PlayerJoinedGame(p);
                Command.Find("Spawn").Use(p, "");
                p.Game.Referee = false;
            } else {
                PlayerLeftGame(p);
                p.Game.Referee = true;
                Entities.GlobalDespawn(p, false, false);
            }
            
            Entities.GlobalSpawn(p, false, "");
            TabList.Update(p, true);
            p.SetPrefix();
        }
        
        void HandlePlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            if (p.level != Map) return;
            if (!p.Game.Referee && RoundInProgress && !Get(p).Infected) {
                InfectPlayer(p, null);
            }
        }
        
        void HandleJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
            p.SetPrefix(); // TODO: Kinda hacky, not sure if needed 
            
            if (prevLevel == Map && level != Map) {
                HUD.Reset(p);
                if (RoundInProgress) PlayerLeftGame(p);
            } else if (level != Map) { return; }
            PlayerJoinedGame(p);
        }
        
        void HandleSendingHeartbeat(Heartbeat service, ref string name) {
            if (!ZSConfig.IncludeMapInHeartbeat || Map == null) return;
            name += " (map: " + Map.MapName + ")";
        }
        
        void HandleBlockChange(Player p, ushort x, ushort y, ushort z, BlockID block, bool placing) {
            if (p.level != Map) return;
            BlockID old = Map.GetBlock(x, y, z);
            
            if (Map.Config.BuildType == BuildType.NoModify) {
                p.RevertBlock(x, y, z); p.cancelBlock = true; return;
            }
            if (Map.Config.BuildType == BuildType.ModifyOnly && Map.Props[old].OPBlock) {
                p.RevertBlock(x, y, z); p.cancelBlock = true; return;
            }
            
            ZSData data = Get(p);
            // Check pillaring
            if (placing && !Map.Config.Pillaring && !p.Game.Referee) {
                if (NotPillaring(block, old)) {
                    data.BlocksStacked = 0;
                } else if (CheckCoords(p, data, x, y, z)) {
                    data.BlocksStacked++;
                } else {
                    data.BlocksStacked = 0;
                }             
                if (WarnPillaring(p, data, x, y, z)) { p.cancelBlock = true; return; }
            }
            data.LastX = x; data.LastY = y; data.LastZ = z;
            
            if (placing || (!placing && p.painting)) {
                if (p.Game.Referee) return;
                if (data.BlocksLeft <= 0) {
                    Player.Message(p, "You have no blocks left.");
                    p.RevertBlock(x, y, z); p.cancelBlock = true; return;
                }

                data.BlocksLeft--;
                if ((data.BlocksLeft % 10) == 0 || data.BlocksLeft <= 10) {
                    Player.Message(p, "Blocks Left: &4" + data.BlocksLeft);
                }
            }
        }
        

        bool NotPillaring(BlockID b, BlockID old) {
            byte collide = Map.CollideType(b);
            if (collide == CollideType.WalkThrough) return true;
            
            collide = Map.CollideType(old);
            return collide == CollideType.SwimThrough || collide == CollideType.LiquidWater
                || collide == CollideType.LiquidLava;
        }
        
        static bool CheckCoords(Player p, ZSData data, ushort x, ushort y, ushort z) {
            if (data.LastY != y - 1 || data.LastX != x || data.LastZ != z) return false;
            int minX = (p.Pos.X - 8) / 32, minZ = (p.Pos.Z - 8) / 32;
            int maxX = (p.Pos.X + 8) / 32, maxZ = (p.Pos.Z + 8) / 32;
            
            // Check the four possible coords/blocks the player could be pillaring up on
            return (minX == x && minZ == z) || (minX == x && maxZ == z)
                || (maxX == x && minZ == z) || (maxX == x && maxZ == z);
        }
        
        static bool WarnPillaring(Player p, ZSData data, ushort x, ushort y, ushort z) {
            if (data.BlocksStacked == 2) {
                TimeSpan delta = DateTime.UtcNow - data.LastPillarWarn;
                if (delta.TotalSeconds >= 5) {
                    Chat.MessageFromOps(p, "  &cWarning: λNICK %Sis pillaring!");
                    data.LastPillarWarn = DateTime.UtcNow;
                }
                
                string action = data.PillarFined ? "kicked" : "fined 10 " + ServerConfig.Currency;
                Player.Message(p, "You are pillaring! &cStop before you are " + action + "!");
            } else if (data.BlocksStacked == 4) {
                if (!data.PillarFined) {
                    Chat.MessageFromOps(p, "  &cWarning: λNICK %Sis pillaring!");
                    Command.Find("Take").Use(null, p.name + " 10 Auto fine for pillaring");
                    Player.Message(p, "  &cThe next time you pillar, you will be &4kicked&c.");
                } else {                   
                    ModAction action = new ModAction(p.name, null, ModActionType.Kicked, "Auto kick for pillaring");
                    OnModActionEvent.Call(action);
                    p.Kick("No pillaring allowed!");
                }
                
                p.RevertBlock(x, y, z);
                data.PillarFined = true;
                data.BlocksStacked = 0;
                return true;
            }
            return false;
        }
        
        void SavePlayerStats(Player p) {
            ZSData data = TryGet(p);
            if (data == null || data.TotalRoundsSurvived == 0 && data.TotalInfected == 0) return;
            
            int count = 0;
            using (DataTable table = Database.Backend.GetRows("ZombieStats", "*", "WHERE Name=@0", p.name)) {
                count = table.Rows.Count;
            }

            if (count == 0) {
                Database.Backend.AddRow("ZombieStats", "TotalRounds, MaxRounds, TotalInfected, MaxInfected, Name",
                                        data.TotalRoundsSurvived, data.MaxRoundsSurvived,
                                        data.TotalInfected,       data.MaxInfected, p.name);
            } else {
                Database.Backend.UpdateRows("ZombieStats", "TotalRounds=@0, MaxRounds=@1, TotalInfected=@2, MaxInfected=@3",
                                            "WHERE Name=@4", data.TotalRoundsSurvived, data.MaxRoundsSurvived,
                                                             data.TotalInfected,       data.MaxInfected, p.name);
            }
        }
    }
}
