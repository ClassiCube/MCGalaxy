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
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Network;

namespace MCGalaxy.Games.ZS {
    public sealed class ZSPlugin : Plugin_Simple {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.VersionString; } }
        public override string name { get { return "Core_ZSPlugin"; } }
        public ZombieGame Game;

        public override void Load(bool startup) {
            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
            OnMoneyChangedEvent.Register(HandleMoneyChanged, Priority.High);
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.High);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.High);
            OnPlayerMoveEvent.Register(HandlePlayerMove, Priority.High);
            OnPlayerActionEvent.Register(HandlePlayerAction, Priority.High);
        }
        
        public override void Unload(bool shutdown) {
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
            OnMoneyChangedEvent.Unregister(HandleMoneyChanged);
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
            OnPlayerMoveEvent.Unregister(HandlePlayerMove);
            OnPlayerActionEvent.Unregister(HandlePlayerAction);
        }
        
        void HandleTabListEntryAdded(Entity entity, ref string tabName, ref string tabGroup, Player dst) {
            Player p = entity as Player;
            if (p == null || p.level != Game.CurLevel) return;
            
            if (p.Game.Referee) {
                tabGroup = "&2Referees";
            } else if (p.Game.Infected) {
                tabGroup = "&cZombies";
                if (ZSConfig.ZombieName != "" && !dst.Game.Aka) {
                    tabName = "&c" + ZSConfig.ZombieName;
                } else {
                    tabName = "&c" + p.truename;
                }
            } else {
                tabGroup = "&fHumans";
            }
        }
        
        void HandleMoneyChanged(Player p) {
            if (p.level != Game.CurLevel) return;
            HUD.UpdateTertiary(p);
        }
        
        void HandlePlayerConnect(Player p) {
            if (!ZSConfig.SetMainLevel) return;
            Player.Message(p, "Zombie Survival is running! Type %T/zs go %Sto join.");
        }
        
        void HandlePlayerDisconnect(Player p, string reason) {
            Game.Alive.Remove(p);
            Game.Infected.Remove(p);
            p.Game.Infected = false;
            Game.RemoveBounties(p);
            
            Game.AssignFirstZombie();
            HUD.UpdateAllPrimary(Game);
        }
        
        void HandlePlayerMove(Player p, Position next, byte rotX, byte rotY) {
            if (!Game.RoundInProgress || p.level != Game.CurLevel) return;
            
            bool reverted = MovementCheck.DetectNoclip(p, next)
                || MovementCheck.DetectSpeedhack(p, next, ZSConfig.MaxMoveDistance);
            if (reverted) p.cancelmove = true;
        }
        
        void HandlePlayerAction(Player p, PlayerAction action, string message, bool stealth) {
            if (!(action == PlayerAction.Referee || action == PlayerAction.UnReferee)) return;
            if (p.level != Game.CurLevel) return;
            
            if (action == PlayerAction.UnReferee) {
                Game.PlayerJoinedLevel(p, Game.CurLevel, Game.CurLevel);
                Command.all.Find("spawn").Use(p, "");
                
                if (p.HasCpeExt(CpeExt.HackControl))
                    p.Send(Hacks.MakeHackControl(p));
            } else {
                HandlePlayerDisconnect(p, null);
                Entities.GlobalDespawn(p, false, true);
                
                if (p.HasCpeExt(CpeExt.HackControl))
                    p.Send(Packet.HackControl(true, true, true, true, true, -1));
            }
            
            Entities.GlobalSpawn(p, false, "");
            TabList.Add(p, p, Entities.SelfID);
            p.SetPrefix();
        }
    }
}
