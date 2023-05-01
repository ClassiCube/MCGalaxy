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
using System.Collections.Generic;
using System.Threading;
using MCGalaxy.Games;
using MCGalaxy.Maths;

namespace MCGalaxy.Modules.Games.LS
{
    public sealed partial class LSGame : RoundsGame 
    {
        int roundSecs, layerSecs;
        
        protected override void DoRound() {
            if (!Running) return;
            roundSecs = 0;
            layerSecs = 0;
            curLayer  = 1;

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) 
            {
                if (p.level != Map) continue;
                
                ResetRoundState(p, Get(p));
                OutputRoundInfo(p);
            }

            ResetPlayerDeaths();
            RoundStart = DateTime.UtcNow;
            RoundInProgress = true;
            UpdateBlockHandlers();

            while (RoundInProgress && roundSecs < roundTotalSecs) {
                if (!Running) return;
                if (!flooded) AnnounceFloodTime();
                
                if (roundSecs >= floodDelaySecs) {
                    if (!layerMode && roundSecs == floodDelaySecs) {
                        FloodFrom(cfg.FloodPos);
                    } else if (layerMode && (layerSecs % layerIntervalSecs) == 0 && curLayer <= cfg.LayerCount) {
                        FloodFrom(CurrentLayerPos());
                        curLayer++;
                    }
                    
                    layerSecs++;
                }
                
                roundSecs++; 
                Thread.Sleep(1000);
            }
        }
        
        void FloodFrom(Vec3U16 pos) {
            Map.Blockchange(pos.X, pos.Y, pos.Z, floodBlock, true);
            if (flooded) return;
            
            Map.Message("&4Look out, here comes the flood!");
            Logger.Log(LogType.GameActivity, "[Lava Survival] Starting map flood.");
            flooded = true;
        }
        
        void RewardPlayer(Player p, Random rnd) {
            if (IsPlayerDead(p)) return;
            
            if (p.Pos.FeetBlockCoords.Y >= Map.GetEdgeLevel()) {
                AwardMoney(p, Config.ASL_RewardMin, Config.ASL_RewardMax, 
                           rnd, 0);
            } else {
                AwardMoney(p, Config.BSL_RewardMin, Config.BSL_RewardMax, 
                           rnd, 0);                
            }
        }

        void AnnounceFloodTime() {
            int left = floodDelaySecs - roundSecs;

            if (left == 0) {
                MessageMap(CpeMessageType.Announcement, "");
            } else if (left <= 10) {
                MessageCountdown("&3{0} &Sseconds until the flood", left, 10);
            } else if ((roundSecs % 60) == 0) { 
                Map.Message(FloodTimeLeftMessage()); 
            }
        }

        public override void EndRound() {
            if (!RoundInProgress) return;
            RoundInProgress = false;
            flooded = false;
            
            Map.SetPhysics(5);
            Map.Message("The round has ended!");
            
            Random rnd = new Random();
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) 
            {
                if (p.level == Map) RewardPlayer(p, rnd);
            }
        }
        

        string FloodTimeLeftMessage() {
            TimeSpan left = TimeSpan.FromSeconds(floodDelaySecs - roundSecs);
            return "&3" + left.Shorten(true) + " &Suntil the flood starts";
        }
        
        string RoundTimeLeftMessage() {
            TimeSpan left = TimeSpan.FromSeconds(roundTotalSecs - roundSecs);
            return "&3" + left.Shorten(true) + " &Suntil the round ends";
        }
        
        string ModeMessage(string block) {
            LSFloodMode mode = floodMode;
            
            return block.Capitalize() + " will be &c" + mode + " &Sthis round";
        }

        public override void OutputStatus(Player p) {
            // TODO: send these messages if player is op
            //if (data.layer) {
            //    Map.ChatLevelOps("There will be " + mapSettings.LayerCount + " layers, each " + mapSettings.LayerHeight + " blocks high.");
            //    Map.ChatLevelOps("There will be another layer every " + mapSettings.layerInterval + " minutes.");
            //}
            
            OutputRoundInfo(p);
            OutputTimeInfo(p);
        }
        
        void OutputRoundInfo(Player p) {
            string block = FloodBlockName();
            if (waterMode) p.Message("The map will be flooded with &9water &Sthis round!");
            if (layerMode) p.Message("The {0} will &aflood in layers &Sthis round!", block);
            
            if (fastMode) p.Message("The {0} will be &cfast &Sthis round!", block);
            if (floodUp) p.Message("The {0} will &cflood upwards &Sthis round!", block);
            p.Message(ModeMessage(block));
        }

        public override void OutputTimeInfo(Player p) {
            if (!flooded) p.Message(FloodTimeLeftMessage());
            p.Message(RoundTimeLeftMessage());
        }
        

        protected override bool SetMap(string map) {
            if (!base.SetMap(map)) return false;
            
            Map.Config.PhysicsOverload = 1000000;
            return true;
        }
    }
}
