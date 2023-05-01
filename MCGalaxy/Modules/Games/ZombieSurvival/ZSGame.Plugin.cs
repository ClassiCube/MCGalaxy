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
using MCGalaxy.Games;
using BlockID = System.UInt16;

namespace MCGalaxy.Modules.Games.ZS
{
    public sealed partial class ZSGame : RoundsGame 
    {
        protected override void HookEventHandlers() {
            OnEntitySpawnedEvent.Register(HandleEntitySpawned, Priority.High);
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
            OnMoneyChangedEvent.Register(HandleMoneyChanged, Priority.High);
            OnBlockChangingEvent.Register(HandleBlockChanging, Priority.High);
            OnSendingModelEvent.Register(HandleSendingModel, Priority.High);
            
            OnPlayerMoveEvent.Register(HandlePlayerMove, Priority.High);
            OnPlayerDiedEvent.Register(HandlePlayerDied, Priority.High);
            OnJoinedLevelEvent.Register(HandleJoinedLevel, Priority.High);           
            OnPlayerChatEvent.Register(HandlePlayerChat, Priority.High);
            OnGettingCanSeeEntityEvent.Register(HandleCanSeeEntity, Priority.High);
            
            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnEntitySpawnedEvent.Unregister(HandleEntitySpawned);
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
            OnMoneyChangedEvent.Unregister(HandleMoneyChanged);
            OnBlockChangingEvent.Unregister(HandleBlockChanging);
            OnSendingModelEvent.Unregister(HandleSendingModel);
            
            OnPlayerMoveEvent.Unregister(HandlePlayerMove);
            OnPlayerDiedEvent.Unregister(HandlePlayerDied);
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);            
            OnPlayerChatEvent.Unregister(HandlePlayerChat);
            OnGettingCanSeeEntityEvent.Unregister(HandleCanSeeEntity);
            
            base.UnhookEventHandlers();
        }
        
        
        void HandleCanSeeEntity(Player p, ref bool canSee, Entity other) {
            Player target = other as Player;
            if (!canSee || p.Game.Referee || target == null) return;
            
            ZSData data = TryGet(target);
            if (data == null || target.level != Map) return;
            canSee = !(target.Game.Referee || data.Invisible);
        }
        
        void HandleSendingModel(Entity e, ref string model, Player dst) {
            Player p = e as Player;
            if (p == null || !IsInfected(p)) return;
            model = p == dst ? p.Model : Config.ZombieModel;
        }
        
        void HandleTabListEntryAdded(Entity e, ref string tabName, ref string tabGroup, Player dst) {
            Player p = e as Player;
            if (p == null || p.level != Map) return;
            
            if (p.Game.Referee) {
                tabGroup = "&2Referees";
            } else if (IsInfected(p)) {
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
            if (p == null || !IsInfected(p)) return;

            name = p.truename;
            if (Config.ZombieName.Length > 0 && !Get(dst).AkaMode) {
                name = Config.ZombieName; skin = name;
            }
            name = Colors.red + name;
        }
        
        void HandlePlayerMove(Player p, Position next, byte rotX, byte rotY, ref bool cancel) {
            if (!RoundInProgress || p.level != Map) return;
            
            // TODO: Maybe tidy this up?
            if (p.Game.Noclip == null) p.Game.Noclip = new NoclipDetector(p);
            if (p.Game.Speed  == null) p.Game.Speed  = new SpeedhackDetector(p);
            
            bool reverted = p.Game.Noclip.Detect(next) || p.Game.Speed.Detect(next, Config.MaxMoveDist);
            if (reverted) cancel = true;
        }

        void HandlePlayerDied(Player p, BlockID cause, ref TimeSpan cooldown) {
            if (p.level != Map || !Config.InfectUponDeath) return;

            if (!p.Game.Referee && RoundInProgress && !IsInfected(p)) {
                InfectPlayer(p, null);
            }
        }
        
        void HandleJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
            p.SetPrefix(); // TODO: Kinda hacky, not sure if needed 
            if (level != Map) return;

            ZSData data = Get(p);
            data.PledgeSurvive = false;
            p.SetPrefix();
            
             
            if (prevLevel == null && Alive.Contains(p)) {
                // Fixes players who login at very end of 'round countdown' being added to
                //  Alive list, but then becoming auto infected after level is finished sending
                // TODO redo Player.Login.cs to not add to PlayerInfo.Online so early ??
            } else if (RoundInProgress) {
                p.Message("You joined in the middle of a round. &cYou are now infected!");
                data.BlocksLeft = 25;
                InfectPlayer(p, null);
            }

            double startLeft = (RoundStart - DateTime.UtcNow).TotalSeconds;
            if (startLeft >= 0) {
                p.Message("&a{0} &Sseconds left until the round starts. &aRun!", (int)startLeft);
            }
            
            OutputMapSummary(p, Map.Config);
            p.Message("This map's win chance is &a{0}&S%", Map.WinChance);
        }
        
        void HandlePlayerChat(Player p, string message) {
            if (p.level != Map || message.Length <= 1) return;
            
            if (message[0] == '~') {
                message = message.Substring(1);
                
                if (IsInfected(p)) {
                    Chat.MessageChat(ChatScope.Level, p, "&c- to zombies - λNICK: &f" + message,
                                    Map, (pl, arg) => pl.Game.Referee ||  IsInfected(pl));
                } else {
                    Chat.MessageChat(ChatScope.Level, p, "&a- to humans - λNICK: &f" + message,
                                    Map, (pl, arg) => pl.Game.Referee || !IsInfected(pl));
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
        
        
        void HandleBlockChanging(Player p, ushort x, ushort y, ushort z, BlockID block, bool placing, ref bool cancel) {
            if (p.level != Map) return;
            BlockID old = Map.GetBlock(x, y, z);
            ZSData data = Get(p);
            bool nonReplacable = Map.Config.BuildType == BuildType.NoModify || 
                                 Map.Config.BuildType == BuildType.ModifyOnly && Map.Props[old].OPBlock;
            
            // Check pillaring
            if (placing && !Map.Config.Pillaring && !p.Game.Referee) {
                if (NotPillaring(block, old)) {
                    data.BlocksStacked = 0;
                } else if (CheckCoords(p, data, x, y, z)) {
                    data.BlocksStacked++;
                } else {
                    data.BlocksStacked = 0;
                }
                if (WarnPillaring(p, data, x, y, z, nonReplacable)) { cancel = true; return; }
            }
            data.LastX = x; data.LastY = y; data.LastZ = z;
            
            if (nonReplacable) {
                p.RevertBlock(x, y, z); cancel = true; return;
            }
            
            if (p.Game.Referee) return;
            
            if (placing || (!placing && p.painting)) {
                if (data.BlocksLeft <= 0) {
                    p.Message("You have no blocks left.");
                    p.RevertBlock(x, y, z); cancel = true; return;
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
        
        static bool WarnPillaring(Player p, ZSData data, ushort x, ushort y, ushort z, bool nonReplacable) {
            if ((!nonReplacable && data.BlocksStacked == 2) || (nonReplacable && data.BlocksStacked == 1)) {
                TimeSpan delta = DateTime.UtcNow - data.LastPillarWarn;
                if (delta.TotalSeconds >= 5) {
                    Chat.MessageFromOps(p, "  &cWarning: λNICK &Sis pillaring!");
                    data.LastPillarWarn = DateTime.UtcNow;
                }
                
                string action = data.PillarFined ? "kicked" : "fined 10 " + Server.Config.Currency;
                p.Message("You are pillaring! &WStop before you are " + action + "!");
            } else if ((!nonReplacable && data.BlocksStacked == 4) || (nonReplacable && data.BlocksStacked == 2)) {
                if (!data.PillarFined) {
                    Chat.MessageFromOps(p, "  &cWarning: λNICK &Sis pillaring!");
                    Command.Find("Take").Use(Player.Console, p.name + " 10 Auto fine for pillaring");
                    p.Message("  &WThe next time you pillar, you will be &4kicked!");
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
