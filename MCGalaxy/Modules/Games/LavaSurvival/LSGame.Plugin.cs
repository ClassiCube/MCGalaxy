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

            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);
            OnPlayerDeathEvent.Unregister(HandlePlayerDeath);
            OnBlockHandlersUpdatedEvent.Unregister(HandleBlockHandlersUpdated);
            OnBlockChangingEvent.Unregister(HandleBlockChanging);

            base.UnhookEventHandlers();
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
                KillPlayer(p);
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
            if (block != Block.Sponge) return;
            LSData data = Get(p);

            if (!p.Game.Referee && data.SpongesLeft <= 0) {
                p.Message("You have no sponges left");
                p.RevertBlock(x, y, z);
                cancel = true; return;
            }

            if (p.ChangeBlock(x, y, z, Block.Sponge) == ChangeResult.Unchanged) {
                cancel = true; return;
            }

            PhysInfo C = default(PhysInfo);
            C.X = x; C.Y = y; C.Z = z;
            OtherPhysics.DoSponge(Map, ref C, !waterMode);

            if (p.Game.Referee) return;
            data.SpongesLeft--;
            if ((data.SpongesLeft % 10) == 0 || data.SpongesLeft <= 10) {
                p.Message("Sponges Left: &4" + data.SpongesLeft);
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
    }
}
