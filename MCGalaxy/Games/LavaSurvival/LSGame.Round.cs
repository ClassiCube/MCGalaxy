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
            if (!Running) return;

            ResetPlayerDeaths();
            RoundStart = DateTime.UtcNow;
            RoundInProgress = true;
            Logger.Log(LogType.GameActivity, "[Lava Survival] Round started. Map: " + Map.ColoredName);
            
            while (RoundInProgress && roundSecs < roundTotalSecs) {
                if (!Running) return;
                if ((announceSecs % 60) == 0 && !Flooded) {
                    Map.Message(FloodTimeLeftMessage());
                }
                if (floodSecs >= floodDelaySecs) DoFlood();
                
                announceSecs++; roundSecs++; floodSecs++;
                Thread.Sleep(1000);
            }
            
            if (Running) EndRound();
            if (Running) VoteAndMoveToNextMap();
        }
        
        void DoFlood() {
            if (layerMode && (layerSecs % layerIntervalSecs) == 0) {
                if (curLayer <= cfg.LayerCount) {
                    Logger.Log(LogType.GameActivity, "[Lava Survival] Layer " +curLayer + " flooding.");
                    ushort y = (ushort)(cfg.LayerPos.Y + ((cfg.LayerHeight * curLayer) - 1));
                    Map.Blockchange(cfg.LayerPos.X, y, cfg.LayerPos.Z, floodBlock, true);
                    curLayer++;
                }
            } else if (!layerMode && floodSecs == floodDelaySecs) {
                Map.Message("&4Look out, here comes the flood!");
                Logger.Log(LogType.GameActivity, "[Lava Survival] Map flooding.");
                Map.Blockchange(cfg.FloodPos.X, cfg.FloodPos.Y, cfg.FloodPos.Z, floodBlock, true);
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
            double mins = Math.Ceiling((RoundStart.AddMinutes(cfg.FloodTimeMins) - DateTime.UtcNow).TotalMinutes);
            return "&3" + mins + " minute" + (mins == 1 ? "" : "s") + " %Suntil the flood.";
        }
        
        internal string RoundTimeLeftMessage() {
            double mins = Math.Ceiling((RoundStart.AddMinutes(cfg.RoundTimeMins) - DateTime.UtcNow).TotalMinutes);
            return "&3" + mins + " minute" + (mins == 1 ? "" : "s") + " %Suntil the round ends.";
        }

        public override void OutputStatus(Player p) {
            string block = waterMode ? "water" : "lava";
            
            // TODO: send these messages if player is op
            //if (data.layer) {
            //    Map.ChatLevelOps("There will be " + mapSettings.LayerCount + " layers, each " + mapSettings.LayerHeight + " blocks high.");
            //    Map.ChatLevelOps("There will be another layer every " + mapSettings.layerInterval + " minutes.");
            //}
            
            if (waterMode) Player.Message(p, "The map will be flooded with &9water %Sthis round!");
            if (layerMode) Player.Message(p, "The " + block + " will &aflood in layers %Sthis round!");
            
            if (fastMode) Player.Message(p, "The lava will be &cfast %Sthis round!");
            if (killerMode) Player.Message(p, "The " + block + " will &ckill you %Sthis round!");
            if (destroyMode) Player.Message(p, "The " + block + " will &cdestroy plants " + (waterMode ? "" : "and flammable blocks ") + "%Sthis round!");          
            
            if (!Flooded) Player.Message(p, FloodTimeLeftMessage());
            Player.Message(p, RoundTimeLeftMessage());
        }

        protected override bool SetMap(string map) {
            bool success = base.SetMap(map);
            if (!success) return false;
            
            cfg = new LSMapConfig();
            cfg.SetDefaults(Map);
            cfg.Load(Map.name);
            UpdateMapConfig();
            
            Map.SetPhysics(destroyMode ? 2 : 1);
            Map.Config.PhysicsOverload = 1000000;
            Map.Config.LoadOnGoto = false;
            
            Level.SaveSettings(Map);
            return true;
        }

        void KillPlayer(Player p) {
            if (Config.MaxLives <= 0) return;
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
