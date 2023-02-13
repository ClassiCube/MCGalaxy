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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Modules.Games.LS
{
    public sealed class LSData 
    {
        public int TimesDied, SpongesLeft, WaterLeft;
    }
    
    public enum LSFloodMode
    {
        Calm, Disturbed, Furious, Wild, Extreme
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
        
        bool flooded, fastMode, waterMode, layerMode, floodUp;
        BlockID floodBlock;
        LSFloodMode floodMode;
        int curLayer, spreadDelay;
        int roundTotalSecs, floodDelaySecs, layerIntervalSecs;
        byte destroyDelay, dissipateChance, burnChance;
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
            
            waterMode = rnd.Next(1, 101) <= cfg.WaterChance;
            layerMode = rnd.Next(1, 101) <= cfg.LayerChance;
            fastMode  = rnd.Next(1, 101) <= cfg.FastChance && !waterMode;
            floodUp   = rnd.Next(1, 101) <= cfg.FloodUpChance;
            
            if (waterMode) {
                floodBlock = Block.Deadly_ActiveWater;
            } else {
                floodBlock = Block.Deadly_ActiveLava;
            }
            spreadDelay = fastMode ? 0 : 4;

            roundTotalSecs    = (int)Config.GetRoundTime(cfg).TotalSeconds;
            floodDelaySecs    = (int)Config.GetFloodTime(cfg).TotalSeconds;
            layerIntervalSecs = (int)Config.GetLayerInterval(cfg).TotalSeconds;
            SetFloodMode(RandomFloodMode(rnd));
        }
        
        LSFloodMode RandomFloodMode(Random rnd) {
            int likelihood = rnd.Next(1, 101);
            int threshold = 0;
            
            int[] chances = { Config.CalmChance, Config.DisturbedChance,
                Config.WildChance, Config.FuriousChance, Config.ExtremeChance 
            };
            
            for (int i = 0; i < chances.Length; i++)
            {
                threshold += chances[i];
                if (likelihood <= threshold) return (LSFloodMode)i;
            }
            return LSFloodMode.Calm;
        }        
        
        public void SetFloodMode(LSFloodMode mode) {
            floodMode       = mode;
            destroyDelay    = GetDestroyDelay();
            dissipateChance = GetDissipateChance();
            burnChance      = GetBurnChance();
            
            if (!RoundInProgress) return;
            Map.SetPhysics(floodMode > LSFloodMode.Calm ? 2 : 1);
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
                if (p.level != Map) continue;
                AddLives(p, Get(p).TimesDied, true);
            }
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleJoinedLevel(p, Map, Map, ref announce);
        }

        static void ResetRoundState(Player p, LSData data) {
            data.SpongesLeft = 10;
            data.WaterLeft   = 30;
        }
        
        
        bool InSafeZone(ushort x, ushort y, ushort z) {
            return x >= cfg.SafeZoneMin.X && x <= cfg.SafeZoneMax.X && y >= cfg.SafeZoneMin.Y
                && y <= cfg.SafeZoneMax.Y && z >= cfg.SafeZoneMin.Z && z <= cfg.SafeZoneMax.Z;
        }
        
        Vec3U16 CurrentLayerPos() {
            Vec3U16 pos = cfg.LayerPos;
            pos.Y = (ushort)(pos.Y + ((cfg.LayerHeight * curLayer) - 1));
            return pos;
        }
        
        string FloodBlockName() {
            return waterMode ? "water" : "lava";
        }
        
        
        public bool IsPlayerDead(Player p) {
            return Config.MaxLives > 0 && Get(p).TimesDied >= Config.MaxLives;
        }
        
        public string DescribeLives(Player p) {
            if (Config.MaxLives <= 0) return "have &ainfinite &Slives";

            int lives = Config.MaxLives - Get(p).TimesDied;
            return lives <= 0 ? "are &4dead" : "have &a" + lives + " &Slives left";
        }
        
        public void AddLives(Player p, int amount, bool silent) {
            LSData data = Get(p);
            if (Config.MaxLives <= 0) { data.TimesDied = 0; return; }
            
            data.TimesDied -= amount;
            UpdateStatus1(p);
            if (silent) return;
            
            p.Message("You " + DescribeLives(p));
            if (!IsPlayerDead(p)) return;
            
            Chat.MessageFromLevel(p, "λNICK &4ran out of lives, and is out of the round!");
            p.Message("&4You can still watch, but you cannot build.");
            // TODO: Buy life message
        }
        
        protected override string FormatStatus1(Player p) {
            string money = "&a" + p.money + " &S" + Server.Config.Currency;
            return money + ", you " + DescribeLives(p);
        }
    }
}
