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
using MCGalaxy.Events.EconomyEvents;
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Network;

namespace MCGalaxy.Games.ZS {
    public sealed class ZSPlugin : Plugin_Simple {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.VersionString; } }
        public override string name { get { return "Core_ZSPlugin"; } }
        public ZSGame Game;

        public override void Load(bool startup) {
            OnEntitySpawnedEvent.Register(HandleEntitySpawned, Priority.High);
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
            OnMoneyChangedEvent.Register(HandleMoneyChanged, Priority.High);
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.High);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.High);
            OnPlayerMoveEvent.Register(HandlePlayerMove, Priority.High);
            OnPlayerActionEvent.Register(HandlePlayerAction, Priority.High);
            OnPlayerSpawningEvent.Register(HandlePlayerSpawning, Priority.High);
            OnBlockChangeEvent.Register(HandleBlockChange, Priority.High);
            OnLevelUnloadEvent.Register(HandleLevelUnload, Priority.High);
        }
        
        public override void Unload(bool shutdown) {
            OnEntitySpawnedEvent.Unregister(HandleEntitySpawned);
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
            OnMoneyChangedEvent.Unregister(HandleMoneyChanged);
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
            OnPlayerMoveEvent.Unregister(HandlePlayerMove);
            OnPlayerActionEvent.Unregister(HandlePlayerAction);
            OnPlayerSpawningEvent.Unregister(HandlePlayerSpawning);
            OnBlockChangeEvent.Unregister(HandleBlockChange);
            OnLevelUnloadEvent.Unregister(HandleLevelUnload);
        }
        
        void HandleTabListEntryAdded(Entity entity, ref string tabName, ref string tabGroup, Player dst) {
            Player p = entity as Player;
            if (p == null || p.level != Game.Map) return;
            
            if (p.Game.Referee) {
                tabGroup = "&2Referees";
            } else if (p.Game.Infected) {
                tabGroup = "&cZombies";
                if (ZSConfig.ZombieName.Length > 0 && !dst.Game.Aka) {
                    tabName = "&c" + ZSConfig.ZombieName;
                } else {
                    tabName = "&c" + p.truename;
                }
            } else {
                tabGroup = "&fHumans";
            }
        }
        
        void HandleMoneyChanged(Player p) {
            if (p.level != Game.Map) return;
            HUD.UpdateTertiary(p);
        }
        
        void HandleEntitySpawned(Entity entity, ref string name, ref string skin, ref string model, Player dst) {
            Player p = entity as Player;
            if (p == null || !p.Game.Infected) return;

            name = p.truename;
            if (ZSConfig.ZombieName.Length > 0 && !dst.Game.Aka) {
                name = ZSConfig.ZombieName; skin = name;
            }
            name = Colors.red + name;
            model = p == dst ? p.Model : ZSConfig.ZombieModel;
        }
        
        void HandlePlayerConnect(Player p) {
            if (!ZSConfig.SetMainLevel) return;
            Player.Message(p, "Zombie Survival is running! Type %T/ZS go %Sto join.");
        }
        
        void HandlePlayerDisconnect(Player p, string reason) {
            Game.PlayerLeftGame(p);
        }
        
        void HandlePlayerMove(Player p, Position next, byte rotX, byte rotY) {
            if (!Game.RoundInProgress || p.level != Game.Map) return;
            
            bool reverted = MovementCheck.DetectNoclip(p, next)
                || MovementCheck.DetectSpeedhack(p, next, ZSConfig.MaxMoveDistance);
            if (reverted) p.cancelmove = true;
        }
        
        void HandlePlayerAction(Player p, PlayerAction action, string message, bool stealth) {
            if (!(action == PlayerAction.Referee || action == PlayerAction.UnReferee)) return;
            if (p.level != Game.Map) return;
            
            if (action == PlayerAction.UnReferee) {
                Game.PlayerJoinedLevel(p, Game.Map, Game.Map);
                Command.all.FindByName("Spawn").Use(p, "");
                p.Game.Referee = false;
                
                if (p.Supports(CpeExt.HackControl))
                    p.Send(Hacks.MakeHackControl(p, p.level.GetMotd(p)));
            } else {
                HandlePlayerDisconnect(p, null);                
                p.Game.Referee = true;
                Entities.GlobalDespawn(p, false, false);
                
                if (p.Supports(CpeExt.HackControl))
                    p.Send(Packet.HackControl(true, true, true, true, true, -1));
            }
            
            Entities.GlobalSpawn(p, false, "");
            TabList.Update(p, true);
            p.SetPrefix();
        }
        
        void HandlePlayerSpawning(Player p, ref Position pos, ref byte yaw, ref byte pitch, bool respawning) {
            if (p.level != Game.Map) return;
            
            if (!p.Game.Referee && !p.Game.Infected && Game.RoundInProgress) {
                Game.InfectPlayer(p, null);
            }
        }
        
        void HandleBlockChange(Player p, ushort x, ushort y, ushort z, ExtBlock block, bool placing) {
            if (p.level != Game.Map) return;
            ExtBlock old = Game.Map.GetBlock(x, y, z);
            
            if (Game.Map.Config.BuildType == BuildType.NoModify) {
                p.RevertBlock(x, y, z); p.cancelBlock = true; return;
            }
            if (Game.Map.Config.BuildType == BuildType.ModifyOnly && Game.Map.Props[old.Index].OPBlock) {
                p.RevertBlock(x, y, z); p.cancelBlock = true; return;
            }
            
            if (Pillaring.Handles(p, x, y, z, placing, block, old, Game)) {
                 p.cancelBlock = true; return;
            }
            
            if (placing || (!placing && p.painting)) {
                if (p.Game.Referee) return;                
                if (p.Game.BlocksLeft == 0) {
                    Player.Message(p, "You have no blocks left.");
                    p.RevertBlock(x, y, z); p.cancelBlock = true; return;
                }

                p.Game.BlocksLeft--;
                if ((p.Game.BlocksLeft % 10) == 0 || (p.Game.BlocksLeft >= 0 && p.Game.BlocksLeft <= 10))
                    Player.Message(p, "Blocks Left: &4" + p.Game.BlocksLeft);
            }
        }
        
        void HandleLevelUnload(Level lvl) {
            if (lvl == Game.Map) lvl.cancelunload = true;
        }
    }
}
