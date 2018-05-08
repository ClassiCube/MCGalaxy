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
using System.Threading;

namespace MCGalaxy.Games {
    public sealed partial class LSGame : RoundsGame {

        int announceSecs, roundSecs, floodSecs, layerSecs;
        protected override void DoRound() {
            if (!running) return;

            deaths.Clear();
            startTime = DateTime.UtcNow;
            RoundInProgress = true;
            Logger.Log(LogType.GameActivity, "[Lava Survival] Round started. Map: " + Map.ColoredName);
            
            while (RoundInProgress && roundSecs < data.roundTotalSecs) {
                if (!running) return;
                if ((announceSecs % 60) == 0 && !Flooded) {
                    AnnounceTimeLeft(true, false);
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
                Map.ChatLevel("&4Look out, here comes the flood!");
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
            Map.ChatLevel("The round has ended!");
        }

        public void AnnounceTimeLeft(bool flood, bool round, Player p = null, bool console = false) {
            if (!running || !RoundInProgress) return;
            if (flood) {
                double mins = Math.Ceiling((startTime.AddMinutes(mapSettings.floodTime) - DateTime.UtcNow).TotalMinutes);
                string msg = "&3" + mins + " minute" + (mins == 1 ? "" : "s") + " %Suntil the flood.";
                if (p == null && !console) Map.ChatLevel(msg);
                else Player.Message(p, msg);
            }
            if (round) {
                double mins = Math.Ceiling((startTime.AddMinutes(mapSettings.roundTime) - DateTime.UtcNow).TotalMinutes);
                string msg = "&3" + mins + " minute" + (mins == 1 ? "" : "s") + " %Suntil the round ends.";
                if (p == null && !console) Map.ChatLevel(msg);
                else Player.Message(p, msg);
            }
        }

        public void AnnounceRoundInfo(Player p = null, bool console = false)  {
            string blockType = data.water ? "water" : "lava";
            if (p == null && !console) {
                if (data.water) Map.ChatLevel("The map will be flooded with &9water %Sthis round!");
                if (data.layer) {
                    Map.ChatLevel("The " + blockType + " will &aflood in layers %Sthis round!");
                    Map.ChatLevelOps("There will be " + mapSettings.LayerCount + " layers, each " + mapSettings.LayerHeight + " blocks high.");
                    Map.ChatLevelOps("There will be another layer every " + mapSettings.layerInterval + " minutes.");
                }
                
                if (data.fast) Map.ChatLevel("The lava will be &cfast %Sthis round!");
                if (data.killer) Map.ChatLevel("The " + blockType + " will &ckill you %Sthis round!");
                if (data.destroy) Map.ChatLevel("The " + blockType + " will &cdestroy plants " + (data.water ? "" : "and flammable blocks ") + "%Sthis round!");
            } else {
                if (data.water) Player.Message(p, "The map will be flooded with &9water %Sthis round!");
                if (data.layer) Player.Message(p, "The " + blockType + " will &aflood in layers %Sthis round!");
                
                if (data.fast) Player.Message(p, "The lava will be &cfast %Sthis round!");
                if (data.killer) Player.Message(p, "The " + blockType + " will &ckill you %Sthis round!");
                if (data.destroy) Player.Message(p, "The " + blockType + " will &cdestroy plants " + (data.water ? "" : "and flammable blocks ") + "%Sthis round!");
            }
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
            if (MaxLives < 1) return;
            string name = p.name.ToLower();
            if (!deaths.ContainsKey(name)) deaths.Add(name, 0);
            deaths[name]++;
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
