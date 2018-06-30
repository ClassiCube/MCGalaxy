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
    
    class LSLevelPicker : LevelPicker {
        public override List<string> GetCandidateMaps() { 
            return new List<string>(LSGame.Config.Maps); 
        }
    }
    
    public sealed partial class LSGame : RoundsGame {
        Random rand = new Random();
        LSMapConfig cfg = new LSMapConfig();
        public static LSConfig Config = new LSConfig();  
        
        public override string GameName { get { return "Lava survival"; } }
        public bool Flooded;
        
        bool fastMode, killerMode, destroyMode, waterMode, layerMode;
        BlockID floodBlock;
        int curLayer, roundTotalSecs, floodDelaySecs, layerIntervalSecs;
        
        public LSGame() { Picker = new LSLevelPicker(); }
        
        LSData Get(Player p) {
            object data;
            if (!p.Extras.TryGet("MCG_LS_DATA", out data)) {
                data = new LSData();
                p.Extras.Put("MCG_LS_DATA", data);
            }
            return (LSData)data;
        }
        
        public void UpdateMapConfig() {
            cfg = new LSMapConfig();
            cfg.SetDefaults(Map);
            cfg.Load(Map.name);
            
            killerMode  = rand.Next(1, 101) <= cfg.KillerChance;
            destroyMode = rand.Next(1, 101) <= cfg.DestroyChance;
            waterMode   = rand.Next(1, 101) <= cfg.WaterChance;
            layerMode   = rand.Next(1, 101) <= cfg.LayerChance;
            fastMode    = rand.Next(1, 101) <= cfg.FastChance && !waterMode;
            
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
        }
                
        protected override List<Player> GetPlayers() {
            return Map.getPlayers();
        }
        
        protected override void StartGame() {
            ResetPlayerDeaths();
        }
        
        protected override void EndGame() {
            Flooded = false;
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

        public void AddMap(string name) {
            if (!String.IsNullOrEmpty(name) && !HasMap(name)) {
                Config.Maps.Add(name);
                Config.Save();
            }
        }
        
        public void RemoveMap(string name) {
            if (Config.Maps.CaselessRemove(name)) Config.Save();
        }
        
        public bool HasMap(string name) {
            return Config.Maps.CaselessContains(name);
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
