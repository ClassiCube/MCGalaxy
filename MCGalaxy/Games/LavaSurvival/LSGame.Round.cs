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

namespace MCGalaxy.Games {
    public sealed partial class LSGame : RoundsGame {

        int announceSecs, roundSecs, floodSecs, layerSecs;
        protected override void DoRound() {
            if (!running) return;

            ResetPlayerDeaths();
            RoundStart = DateTime.UtcNow;
            RoundInProgress = true;
            Logger.Log(LogType.GameActivity, "[Lava Survival] Round started. Map: " + Map.ColoredName);
            
            while (RoundInProgress && roundSecs < data.roundTotalSecs) {
                if (!running) return;
                if ((announceSecs % 60) == 0 && !Flooded) {
                    Map.Message(FloodTimeLeftMessage());
                }
                if (floodSecs >= data.floodDelaySecs) DoFlood();
                
                announceSecs++; roundSecs++; floodSecs++;
                Thread.Sleep(1000);
            }
            
            if (running) EndRound();
            if (running) VoteAndMoveToNextMap();
        }
        
        void DoFlood() {
            if (data.layer && (layerSecs % data.layerIntervalSecs) == 0) {
                if (data.currentLayer <= mapSettings.LayerCount) {
                    Logger.Log(LogType.GameActivity, "[Lava Survival] Layer " + data.currentLayer + " flooding.");
                    Map.Blockchange(mapSettings.LayerPos.X, (ushort)(mapSettings.LayerPos.Y + ((mapSettings.LayerHeight * data.currentLayer) - 1)), mapSettings.LayerPos.Z, data.block, true);
                    data.currentLayer++;
                }
            } else if (!data.layer && floodSecs == data.floodDelaySecs) {
                Map.Message("&4Look out, here comes the flood!");
                Logger.Log(LogType.GameActivity, "[Lava Survival] Map flooding.");
                Map.Blockchange(mapSettings.FloodPos.X, mapSettings.FloodPos.Y, mapSettings.FloodPos.Z, data.block, true);
            }
        }

        public override void EndRound() {
            if (!RoundInProgress) return;
            RoundInProgress = false;
            announceSecs = 0; roundSecs = 0; floodSecs = 0; layerSecs = 0;
            
            Flooded = false;
            Map.SetPhysics(5);
            Map.Message("The round has ended!");
        }

        internal string FloodTimeLeftMessage() {
            double mins = Math.Ceiling((RoundStart.AddMinutes(mapSettings.floodTime) - DateTime.UtcNow).TotalMinutes);
            return "&3" + mins + " minute" + (mins == 1 ? "" : "s") + " %Suntil the flood.";
        }
        
        internal string RoundTimeLeftMessage() {
            double mins = Math.Ceiling((RoundStart.AddMinutes(mapSettings.roundTime) - DateTime.UtcNow).TotalMinutes);
            return "&3" + mins + " minute" + (mins == 1 ? "" : "s") + " %Suntil the round ends.";
        }

        // TODO: common abstract method
        internal void MessageRoundStatus(Player p) {
            string block = data.water ? "water" : "lava";
            
            // TODO: send these messages if player is op
            //if (data.layer) {
            //    Map.ChatLevelOps("There will be " + mapSettings.LayerCount + " layers, each " + mapSettings.LayerHeight + " blocks high.");
            //    Map.ChatLevelOps("There will be another layer every " + mapSettings.layerInterval + " minutes.");
            //}
            
            if (data.water) Player.Message(p, "The map will be flooded with &9water %Sthis round!");
            if (data.layer) Player.Message(p, "The " + block + " will &aflood in layers %Sthis round!");
            
            if (data.fast) Player.Message(p, "The lava will be &cfast %Sthis round!");
            if (data.killer) Player.Message(p, "The " + block + " will &ckill you %Sthis round!");
            if (data.destroy) Player.Message(p, "The " + block + " will &cdestroy plants " + (data.water ? "" : "and flammable blocks ") + "%Sthis round!");
            
            
            if (!Flooded) Player.Message(p, FloodTimeLeftMessage());           
            Player.Message(p, RoundTimeLeftMessage());
        }

        protected override bool SetMap(string map) {
            bool success = base.SetMap(map);
            if (!success) return false;
            mapSettings = LoadMapSettings(map);
            data = GenerateMapData(mapSettings);
            
            Map.SetPhysics(data.destroy ? 2 : 1);
            Map.Config.PhysicsOverload = 1000000;
            Map.Config.LoadOnGoto = false;
            Level.SaveSettings(Map);
            return true;
        }

        void KillPlayer(Player p) {
            if (MaxLives <= 0) return;
            Get(p).TimesDied++;
            if (!IsPlayerDead(p)) return;
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                if (pl != p && pl.level == Map) {
                    Player.Message(pl, p.ColoredName + " &4ran out of lives, and is out of the round!");
                }
            }
            
            Player.Message(p, "&4You ran out of lives, and are out of the round!");
            Player.Message(p, "&4You can still watch, but you cannot build.");
        }
    }
}
