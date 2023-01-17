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
using MCGalaxy.Games;
using BlockID = System.UInt16;

namespace MCGalaxy.Modules.Games.LS
{
    public sealed class LSData 
    {
        public int TimesDied, SpongesLeft;
    }
    
    public sealed partial class LSGame : RoundsGame 
    {
        LSMapConfig cfg = new LSMapConfig();
        public LSConfig Config = new LSConfig();
        public override string GameName { get { return "Lava survival"; } }
        public override RoundsGameConfig GetConfig() { return Config; }
        
        protected override string WelcomeMessage {
            get { return "&cLava Survival &Sis running! Type &T/LS go &Sto join"; }
        }
        
        bool flooded, fastMode, destroyMode, waterMode, layerMode, floodUp;
        BlockID floodBlock;
        int curLayer, spreadDelay;
        int roundTotalSecs, floodDelaySecs, layerIntervalSecs;
        static bool hooked;
        
        public static LSGame Instance = new LSGame();
        public LSGame() { Picker = new LevelPicker(); }
        
        public static LSData Get(Player p) {
            object data;
            if (!p.Extras.TryGet("MCG_LS_DATA", out data)) {
                data = new LSData();
                p.Extras["MCG_LS_DATA"] = data;
            }
            return (LSData)data;
        }
        
        public override void UpdateMapConfig() {
            LSMapConfig cfg = new LSMapConfig();
            cfg.SetDefaults(Map);
            cfg.Load(Map.name);
            this.cfg = cfg;            
            Random rnd = new Random();
            
            destroyMode = rnd.Next(1, 101) <= cfg.DestroyChance;
            waterMode   = rnd.Next(1, 101) <= cfg.WaterChance;
            layerMode   = rnd.Next(1, 101) <= cfg.LayerChance;
            fastMode    = rnd.Next(1, 101) <= cfg.FastChance && !waterMode;
            floodUp     = rnd.Next(1, 101) <= cfg.FloodUpChance;
            
            if (waterMode) {
                floodBlock = Block.Deadly_ActiveWater;
            } else {
                floodBlock = Block.Deadly_ActiveLava;
            }
            spreadDelay = fastMode ? 0 : 4;

            curLayer = 1;
            roundTotalSecs    = (int)cfg.RoundTime.TotalSeconds;
            floodDelaySecs    = (int)cfg.FloodTime.TotalSeconds;
            layerIntervalSecs = (int)cfg.LayerInterval.TotalSeconds;
            
            if (RoundInProgress) Map.SetPhysics(destroyMode ? 2 : 1);
        }
                
        protected override List<Player> GetPlayers() {
            return Map.getPlayers();
        }
        
        protected override void StartGame() {
            ResetPlayerDeaths();
            if (hooked) return;
            
            hooked = true;
            //HookStats();
            HookCommands();
            HookItems();
        }
        
        protected override void EndGame() {
            flooded = false;
            ResetPlayerDeaths();
            UpdateBlockHandlers();
            
            hooked = false;
            //UnhookStats();
            UnhookCommands();
            UnhookItems();
        }
        
        public bool IsPlayerDead(Player p) {
            return Config.MaxLives > 0 && Get(p).TimesDied >= Config.MaxLives;
        }
        
        public string DescribeLives(Player p) {
            if (Config.MaxLives <= 0) return "(infinite)";

            int lives = Config.MaxLives - Get(p).TimesDied;
            return lives <= 0 ? "&40" : lives.ToString();
        }
        
        public override bool HandlesBlockchange(Player p, ushort x, ushort y, ushort z) {
            if (!IsPlayerDead(p)) return false;
            
            p.Message("You are out of the round, and cannot build.");
            p.RevertBlock(x, y, z); 
            return true;
        }
        
        void ResetPlayerDeaths() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) 
            {
                if (p.level == Map) Get(p).TimesDied = 0;
            }
        }

        bool InSafeZone(ushort x, ushort y, ushort z) {
            return x >= cfg.SafeZoneMin.X && x <= cfg.SafeZoneMax.X && y >= cfg.SafeZoneMin.Y
                && y <= cfg.SafeZoneMax.Y && z >= cfg.SafeZoneMin.Z && z <= cfg.SafeZoneMax.Z;
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleJoinedLevel(p, Map, Map, ref announce);
        }

        static void ResetRoundState(Player p, LSData data) {
            data.SpongesLeft = 10;
        }
    }
}
