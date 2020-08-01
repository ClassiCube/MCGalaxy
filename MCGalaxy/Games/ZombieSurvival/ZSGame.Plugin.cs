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
using MCGalaxy.Blocks;
using MCGalaxy.Events;
using MCGalaxy.Events.EconomyEvents;
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    public sealed partial class ZSGame : RoundsGame {

        protected override void HookEventHandlers() {
            OnEntitySpawnedEvent.Register(HandleEntitySpawned, Priority.High);
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
            OnMoneyChangedEvent.Register(HandleMoneyChanged, Priority.High);
            OnBlockChangeEvent.Register(HandleBlockChange, Priority.High);
            OnSendingModelEvent.Register(HandleSendingModel, Priority.High);
            
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.High);
            OnPlayerMoveEvent.Register(HandlePlayerMove, Priority.High);
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
            OnJoinedLevelEvent.Register(HandleJoinedLevel, Priority.High);           
            OnPlayerChatEvent.Register(HandlePlayerChat, Priority.High);
            OnGettingCanSeeEntityEvent.Register(HandleCanSeeEntity, Priority.High);
            
            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnEntitySpawnedEvent.Unregister(HandleEntitySpawned);
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
            OnMoneyChangedEvent.Unregister(HandleMoneyChanged);
            OnBlockChangeEvent.Unregister(HandleBlockChange);
            OnSendingModelEvent.Unregister(HandleSendingModel);
            
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerMoveEvent.Unregister(HandlePlayerMove);
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);            
            OnPlayerChatEvent.Unregister(HandlePlayerChat);
            OnGettingCanSeeEntityEvent.Unregister(HandleCanSeeEntity);
            
            base.UnhookEventHandlers();
        }
        
        
        void HandleCanSeeEntity(Player p, ref bool canSee, Entity other) {
            Player target = other as Player;
            if (!canSee || p.Game.Referee) return;
            
            ZSData data = TryGet(target);
            if (data == null) return;
            canSee = !(target.Game.Referee || data.Invisible);
        }
        
        void HandleSendingModel(Entity e, ref string model, Player dst) {
            Player p = e as Player;
            if (p == null || !Get(p).Infected) return;
            model = p == dst ? p.Model : Config.ZombieModel;
        }
        
        void HandleTabListEntryAdded(Entity e, ref string tabName, ref string tabGroup, Player dst) {
            Player p = e as Player;
            if (p == null || p.level != Map) return;
            
            if (p.Game.Referee) {
                tabGroup = "&2Referees";
            } else if (Get(p).Infected) {
                tabGroup = Config.ZombieTabListGroup;
                if (Config.ZombieName.Length > 0 && !Get(dst).AkaMode) {
                    tabName = "&c" + Config.ZombieName;
                } else {
                    tabName = "&c" + p.truename;
                }
            } else {
                tabGroup = Config.HumanTabListGroup;
            }
        }
        
        void HandleMoneyChanged(Player p) {
            if (p.level != Map) return;
            UpdateStatus3(p);
        }
        
        void HandleEntitySpawned(Entity e, ref string name, ref string skin, ref string model, Player dst) {
            Player p = e as Player;
            if (p == null || !Get(p).Infected) return;

            name = p.truename;
            if (Config.ZombieName.Length > 0 && !Get(dst).AkaMode) {
                name = Config.ZombieName; skin = name;
            }
            name = Colors.red + name;
        }
        
        void HandlePlayerConnect(Player p) {
            if (GetConfig().SetMainLevel) return;
            p.Message("&3Zombie Survival %Sis running! Type %T/ZS go %Sto join");
        }
        
        void HandlePlayerMove(Player p, Position next, byte rotX, byte rotY) {
            if (!RoundInProgress || p.level != Map) return;
            
            bool reverted = MovementCheck.DetectNoclip(p, next)
                || MovementCheck.DetectSpeedhack(p, next, Config.MaxMoveDist);
            if (reverted) p.cancelmove = true;
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
            if (level != Map) return;
            
            ZSData data = Get(p);
            p.SetPrefix();
            
            if (RoundInProgress) {
                p.Message("You joined in the middle of a round. &cYou are now infected!");
                data.BlocksLeft = 25;
                InfectPlayer(p, null);
            }

            double startLeft = (RoundStart - DateTime.UtcNow).TotalSeconds;
            if (startLeft >= 0) {
                p.Message("&a{0} %Sseconds left until the round starts. &aRun!", (int)startLeft);
            }
            
            MessageMapInfo(p);
            p.Message("This map's win chance is &a{0}%S%", Map.WinChance);
        }
        
        void HandlePlayerChat(Player p, string message) {
            if (p.level != Map || message.Length <= 1) return;
            
            if (message[0] == '~') {
                message = message.Substring(1);
                
                if (Get(p).Infected) {
                    Chat.MessageChat(ChatScope.Level, p, "&c- to zombies - λNICK: &f" + message,
                                    Map, (pl, arg) => pl.Game.Referee || Get(pl).Infected);
                } else {
                    Chat.MessageChat(ChatScope.Level, p, "&a- to humans - λNICK: &f" + message,
                                    Map, (pl, arg) => pl.Game.Referee || !Get(pl).Infected);
                }
                p.cancelchat = true;
            } else if (message[0] == '`') {
                if (p.Game.Team == null) {
                    p.Message("You are not on a team, so cannot send a team message.");
                } else {
                    p.Game.Team.Message(p, message.Substring(1));
                }
                p.cancelchat = true;
            }
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
            
            if (p.Game.Referee) return;
            ZSData data = Get(p);
            
            // Check pillaring
            if (placing && !Map.Config.Pillaring) {
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
                if (data.BlocksLeft <= 0) {
                    p.Message("You have no blocks left.");
                    p.RevertBlock(x, y, z); p.cancelBlock = true; return;
                }

                data.BlocksLeft--;
                if ((data.BlocksLeft % 10) == 0 || data.BlocksLeft <= 10) {
                    p.Message("Blocks Left: &4" + data.BlocksLeft);
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
                
                string action = data.PillarFined ? "kicked" : "fined 10 " + Server.Config.Currency;
                p.Message("You are pillaring! %WStop before you are " + action + "!");
            } else if (data.BlocksStacked == 4) {
                if (!data.PillarFined) {
                    Chat.MessageFromOps(p, "  &cWarning: λNICK %Sis pillaring!");
                    Command.Find("Take").Use(Player.Console, p.name + " 10 Auto fine for pillaring");
                    p.Message("  %WThe next time you pillar, you will be &4kicked!");
                } else {
                    ModAction action = new ModAction(p.name, Player.Console, ModActionType.Kicked, "Auto kick for pillaring");
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
    }
}
