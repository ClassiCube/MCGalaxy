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
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    internal sealed class LSData {
        public int TimesDied;
    }
    
    public sealed partial class LSGame : RoundsGame {
        LSMapConfig cfg = new LSMapConfig();
        public static LSConfig Config = new LSConfig();
        public override string GameName { get { return "Lava survival"; } }
        public override RoundsGameConfig GetConfig() { return Config; }
        
        bool flooded, fastMode, killerMode, destroyMode, waterMode, layerMode;
        BlockID floodBlock;
        int curLayer, roundTotalSecs, floodDelaySecs, layerIntervalSecs;
        
        public static LSGame Instance = new LSGame();
        public LSGame() { Picker = new LevelPicker(); }
        
        LSData Get(Player p) {
            object data;
            if (!p.Extras.TryGet("MCG_LS_DATA", out data)) {
                data = new LSData();
                p.Extras.Put("MCG_LS_DATA", data);
            }
            return (LSData)data;
        }
        
        public override void UpdateMapConfig() {
            LSMapConfig cfg = new LSMapConfig();
            cfg.SetDefaults(Map);
            cfg.Load(Map.name);
            this.cfg = cfg;            
            Random rnd = new Random();
            
            killerMode  = rnd.Next(1, 101) <= cfg.KillerChance;
            destroyMode = rnd.Next(1, 101) <= cfg.DestroyChance;
            waterMode   = rnd.Next(1, 101) <= cfg.WaterChance;
            layerMode   = rnd.Next(1, 101) <= cfg.LayerChance;
            fastMode    = rnd.Next(1, 101) <= cfg.FastChance && !waterMode;
            
            if (waterMode) {
                floodBlock = killerMode ? Block.Deadly_ActiveWater : Block.Water;
            } else if (fastMode) {
                floodBlock = killerMode ? Block.Deadly_FastLava : Block.FastLava;
            } else {
                floodBlock = killerMode ? Block.Deadly_ActiveLava : Block.Lava;
            }

            curLayer = 1;
            roundTotalSecs = (int)cfg.RoundTime.TotalSeconds;
            floodDelaySecs = (int)cfg.FloodTime.TotalSeconds;
            layerIntervalSecs = (int)cfg.LayerInterval.TotalSeconds;
            
            if (RoundInProgress) Map.SetPhysics(destroyMode ? 2 : 1);
        }
                
        protected override List<Player> GetPlayers() {
            return Map.getPlayers();
        }
        
        protected override void StartGame() {
            ResetPlayerDeaths();
        }
        
        protected override void EndGame() {
            flooded = false;
            ResetPlayerDeaths();
        }
        
        public bool IsPlayerDead(Player p) {
            return Config.MaxLives > 0 && Get(p).TimesDied >= Config.MaxLives;
        }
        
        void ResetPlayerDeaths() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == Map) Get(p).TimesDied = 0;
            }
        }

        public bool InSafeZone(ushort x, ushort y, ushort z) {
            return x >= cfg.SafeZoneMin.X && x <= cfg.SafeZoneMax.X && y >= cfg.SafeZoneMin.Y
                && y <= cfg.SafeZoneMax.Y && z >= cfg.SafeZoneMin.Z && z <= cfg.SafeZoneMax.Z;
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleJoinedLevel(p, Map, Map, ref announce);
        }
        
        public override bool HandlesChatMessage(Player p, string message) {
            if (!Running || p.level != Map) return false;
            return Picker.HandlesMessage(p, message);
        }
    }
}
