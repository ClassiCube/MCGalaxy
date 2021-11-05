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
using System.IO;
using MCGalaxy.Config;
using MCGalaxy.Maths;

namespace MCGalaxy.Games {
    
    public sealed class LSConfig : RoundsGameConfig {
        [ConfigInt("lives", null, 3, 0)]
        public int MaxLives = 3;
        [ConfigInt("max-award", null, 25, 0)]
        public int MaxAward = 25;
        [ConfigInt("award-reducer", null, 5, 0)]
        public int AwardReducer = 2;

        static ConfigElement[] cfg;
        public override bool AllowAutoload { get { return false; } }
        protected override string GameName { get { return "Lava Survival"; } }
        protected override string PropsPath { get { return "properties/lavasurvival.properties"; } }

        public override void Save()
        {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(LSConfig));

            using (StreamWriter w = new StreamWriter(PropsPath))
            {
                w.WriteLine("#" + GameName + " configuration");
                ConfigElement.SerialiseElements(cfg, w, this);
            }
        }

        public override void Load()
        {
            if (!File.Exists(PropsPath))
            {
                // Enter the default configuration
                string configuration  = string.Format(
                              $"# Lava Survival\n" +
                              $"lives = {MaxLives}\n" +
                              $"max-award = {MaxAward}\n" +
                              $"award-reducer = {AwardReducer}\n\n" +
                              $"# General\n" +
                              "set-main-level = false\n" +
                              "map-in-heartbeat = false\n" +
                              "maps = bouda_metro,cube,desert_well,island,lava_geyser,passage,pluto,volcano"
                              );

                File.WriteAllText(PropsPath, configuration);
            }
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(ZSConfig));
            PropertiesFile.Read(PropsPath, ProcessConfigLine);
        }

        void ProcessConfigLine(string key, string value)
        {
            // backwards compatibility
            if (key.CaselessEq("maps"))
            {
                Maps = new List<string>(value.SplitComma());
            }
            else
            {
                ConfigElement.Parse(cfg, this, key, value);
            }
        }
    }
    
    public sealed class LSMapConfig : RoundsGameMapConfig {
        [ConfigInt("fast-chance", null, 0, 0, 100)]
        public int FastChance;
        [ConfigInt("killer-chance", null, 100, 0, 100)]
        public int KillerChance = 100;
        [ConfigInt("destroy-chance", null, 0, 0, 100)]
        public int DestroyChance;
        [ConfigInt("water-chance", null, 0, 0, 100)]
        public int WaterChance;
        [ConfigInt("layer-chance", null, 0, 0, 100)]
        public int LayerChance;
        
        [ConfigInt("layer-height", null, 3, 0)]
        public int LayerHeight = 3;
        [ConfigInt("layer-count", null, 10, 0)]
        public int LayerCount = 10;
        
        [ConfigTimespan("layer-interval", null, 2, true)]
        public TimeSpan LayerInterval = TimeSpan.FromMinutes(2);
        [ConfigTimespan("round-time", null, 15, true)]
        public TimeSpan RoundTime = TimeSpan.FromMinutes(15);
        [ConfigTimespan("flood-time", null, 5, true)]
        public TimeSpan FloodTime = TimeSpan.FromMinutes(5);
        
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
