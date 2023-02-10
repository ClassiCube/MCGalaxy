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
using System;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Events;
using MCGalaxy.Events.EconomyEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Modules.Games.LS
{
    public sealed partial class LSGame : RoundsGame 
    {
        protected override void HookEventHandlers() {
            OnJoinedLevelEvent.Register(HandleJoinedLevel, Priority.High);
            OnPlayerDeathEvent.Register(HandlePlayerDeath, Priority.High);
            OnBlockHandlersUpdatedEvent.Register(HandleBlockHandlersUpdated, Priority.High);
            OnBlockChangingEvent.Register(HandleBlockChanging, Priority.High);
            OnMoneyChangedEvent.Register(HandleMoneyChanged, Priority.High);

            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);
            OnPlayerDeathEvent.Unregister(HandlePlayerDeath);
            OnBlockHandlersUpdatedEvent.Unregister(HandleBlockHandlersUpdated);
            OnBlockChangingEvent.Unregister(HandleBlockChanging);
            OnMoneyChangedEvent.Unregister(HandleMoneyChanged);

            base.UnhookEventHandlers();
        }
        
        void HandleMoneyChanged(Player p) {
            if (p.level != Map) return;
            UpdateStatus1(p);
        }
        
        void HandleJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
            
            if (Map != level) return;
            ResetRoundState(p, Get(p)); // TODO: Check for /reload case?
            OutputMapSummary(p, Map.Config);
            if (RoundInProgress) OutputStatus(p);
        }
        
        void HandlePlayerDeath(Player p, BlockID block) {
            if (p.level != Map) return;
         
            if (IsPlayerDead(p)) {
                p.cancelDeath = true;
            } else {
                AddLives(p, -1, false);
            }
        }
        
        void HandleBlockChanging(Player p, ushort x, ushort y, ushort z, BlockID block, bool placing, ref bool cancel) {
            if (p.level != Map || !(placing || p.painting)) return;
            
            if (Config.SpawnProtection && NearLavaSpawn(x, y, z)) {
                p.Message("You can't place blocks so close to the {0} spawn", FloodBlockName());
                p.RevertBlock(x, y, z);
                cancel = true; return;
            }

            block = p.GetHeldBlock();
            LSData data = Get(p);
            
            if (block == Block.Sponge) {
                bool placed = TryPlaceBlock(p, ref data.SpongesLeft, "Sponges", Block.Sponge, x, y, z);
                if (!placed) { cancel = true; return; }

                PhysInfo C = default(PhysInfo);
                C.X = x; C.Y = y; C.Z = z;
                OtherPhysics.DoSponge(Map, ref C, !waterMode);
            } else if (block == Block.StillWater) {
                bool placed = TryPlaceBlock(p, ref data.WaterLeft, "Water blocks", Block.StillWater, x, y, z);
                if (!placed) { cancel = true; return; }
            }
        }
        
        bool NearLavaSpawn(ushort x, ushort y, ushort z) {
            Vec3U16 pos = layerMode ? CurrentLayerPos() : cfg.FloodPos;
            int dist    = Config.SpawnProtectionRadius;
            
            int dx = Math.Abs(x - pos.X);
            int dy = Math.Abs(y - pos.Y);
            int dz = Math.Abs(z - pos.Z);
            return dx <= dist && dy <= dist && dz <= dist;
        }
        
        bool TryPlaceBlock(Player p, ref int blocksLeft, string type, 
                           BlockID block, ushort x, ushort y, ushort z) {
            if (!p.Game.Referee && blocksLeft <= 0) {
                p.Message("You have no {0} left", type);
                p.RevertBlock(x, y, z);
                return false;
            }

            if (p.ChangeBlock(x, y, z, block) == ChangeResult.Unchanged)
                return false;           
            if (p.Game.Referee) return true;
            
            blocksLeft--;
            if ((blocksLeft % 10) == 0 || blocksLeft <= 10) {
                p.Message("{0} left: &4{1}", type, blocksLeft);
            }
            return true;
        }
    }
}
