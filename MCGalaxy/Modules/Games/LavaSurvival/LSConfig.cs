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
using MCGalaxy.Config;
using MCGalaxy.Games;
using MCGalaxy.Maths;

namespace MCGalaxy.Modules.Games.LS
{
    public sealed class LSConfig : RoundsGameConfig 
    {
        [ConfigInt("lives", "Game", 3, 0)]
        public int MaxLives = 3;
        
        [ConfigInt("below-sealevel-reward-min", "Rewards",  5, 0)]
        public int BSL_RewardMin =  5;
        [ConfigInt("below-sealevel-reward-max", "Rewards", 10, 0)]
        public int BSL_RewardMax = 10;
        [ConfigInt("above-sealevel-reward-min", "Rewards", 10, 0)]
        public int ASL_RewardMin = 10;
        [ConfigInt("above-sealevel-reward-max", "Rewards", 15, 0)]
        public int ASL_RewardMax = 15;
        
        
        [ConfigTimespan("default-layer-interval", "Defaults", 2, true)]
        public TimeSpan DefaultLayerInterval = TimeSpan.FromMinutes(2);
        [ConfigTimespan("default-round-time", "Defaults", 15, true)]
        public TimeSpan DefaultRoundTime = TimeSpan.FromMinutes(15);
        [ConfigTimespan("default-flood-time", "Defaults", 5, true)]
        public TimeSpan DefaultFloodTime = TimeSpan.FromMinutes(5);
        
        [ConfigBool("lava-spawn-protection", "Protection", true)]
        public bool SpawnProtection = true;
        [ConfigInt("lava-spawn-protection-radius", "Protection", 5)]
        public int SpawnProtectionRadius = 5;
        
        [ConfigInt("chance-calm",      "Mode chances", 35, 0, 100)]
        public int CalmChance = 35;
        [ConfigInt("chance-disturbed", "Mode chances", 45, 0, 100)]
        public int DisturbedChance = 45;
        [ConfigInt("chance-furious",   "Mode chances", 15, 0, 100)]
        public int FuriousChance = 15;
        [ConfigInt("chance-wild",      "Mode chances",  5, 0, 100)]
        public int WildChance = 5;
        [ConfigInt("chance-extreme",   "Mode chances",  0, 0, 100)]
        public int ExtremeChance = 0;
               
        [ConfigInt("sponge-life-ticks", "Sponges", 200, 0)]
        public int SpongeLife = 200;
        
        public override bool AllowAutoload { get { return false; } }
        protected override string GameName { get { return "Lava Survival"; } }
        
        
        public TimeSpan GetRoundTime(LSMapConfig mapCfg) {
            return GetTimespan(mapCfg._RoundTime, DefaultRoundTime);
        }
        
        public TimeSpan GetFloodTime(LSMapConfig mapCfg) {
            return GetTimespan(mapCfg._FloodTime, DefaultFloodTime);
        }
        
        public TimeSpan GetLayerInterval(LSMapConfig mapCfg) {
            return GetTimespan(mapCfg._LayerInterval, DefaultLayerInterval);
        }
        
        static TimeSpan GetTimespan(TimeSpan? mapValue, TimeSpan defaultValue) {         
            return mapValue.HasValue ? mapValue.Value : defaultValue;
        }
    }
    
    public sealed class LSMapConfig : RoundsGameMapConfig 
    {
        [ConfigInt("fast-chance", null, 0, 0, 100)]
        public int FastChance;
        [ConfigInt("water-chance", null, 0, 0, 100)]
        public int WaterChance;
        [ConfigInt("flood-upwards-chance", null, 0, 0, 100)]
        public int FloodUpChance;

        [ConfigInt("layer-chance", null, 0, 0, 100)]
        public int LayerChance;
        [ConfigInt("layer-height", null, 3)]
        public int LayerHeight = 3;
        [ConfigInt("layer-count", null, 10, 0)]
        public int LayerCount = 10;
        
        [ConfigOptTimespan("layer-interval", null, true)]
        public TimeSpan? _LayerInterval;
        [ConfigOptTimespan("round-time", null, true)]
        public TimeSpan? _RoundTime;
        [ConfigOptTimespan("flood-time", null, true)]
        public TimeSpan? _FloodTime;
        
        [ConfigVec3("block-flood", null)] public Vec3U16 FloodPos;
        [ConfigVec3("block-layer", null)] public Vec3U16 LayerPos;
        [ConfigVec3("safe-zone-min", null)] public Vec3U16 SafeZoneMin;
        [ConfigVec3("safe-zone-max", null)] public Vec3U16 SafeZoneMax;
        
        
        const string propsDir = "properties/lavasurvival/";
        static ConfigElement[] cfg;       
        public override void Load(string map) {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(LSMapConfig));
            LoadFrom(cfg, propsDir, map);
        }
        
        public override void Save(string map) {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(LSMapConfig));
            SaveTo(cfg, propsDir, map);
        }
        
        
        public override void SetDefaults(Level lvl) {
            ushort x = (ushort)(lvl.Width / 2), y = (ushort)(lvl.Height / 2), z = (ushort)(lvl.Length / 2);
            FloodPos = new Vec3U16(x, (ushort)(lvl.Height - 1), z);
            LayerPos = new Vec3U16(0, y                       , 0);
            
            SafeZoneMin = new Vec3U16((ushort)(x - 3), y,               (ushort)(z - 3));
            SafeZoneMax = new Vec3U16((ushort)(x + 3), (ushort)(y + 4), (ushort)(z + 3));
        }
    }
}
