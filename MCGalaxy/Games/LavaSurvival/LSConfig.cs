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
using System.IO;
using System.Timers;
using MCGalaxy.Commands;
using MCGalaxy.Maths;
using BlockID = System.UInt16;
using MCGalaxy.Config;
using System.Collections.Generic;

namespace MCGalaxy.Games {
    
    public sealed class LSConfig {
        [ConfigBool("start-on-startup", null, false)]
        public bool StartImmediately;
        [ConfigInt("lives", null, 3, 0)]
        public int MaxLives = 3;
        [ConfigStringList("maps", null)]
        public List<string> Maps = new List<string>();
        
        static ConfigElement[] cfg;
        const string propsFile = "properties/lavasurvival.properties";
        
        public void Save() {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(LSConfig));
            ConfigElement.SerialiseSimple(cfg, propsFile, this);
        }
        
        public void Load() {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(LSConfig));
            ConfigElement.ParseFile(cfg, "Lava survival", propsFile, this);
        }
    }
    
    public sealed class LSMapConfig {
        [ConfigInt("fast-chance", "Lava", 0, 0, 100)]
        public int FastChance;       
        [ConfigInt("killer-chance", "Lava", 100, 0, 100)]
        public int KillerChance = 100;
        [ConfigInt("destroy-chance", "Lava", 0, 0, 100)]
        public int DestroyChance;
        [ConfigInt("water-chance", "Lava", 0, 0, 100)]
        public int WaterChance;
        [ConfigInt("layer-chance", "Lava", 0, 0, 100)]
        public int LayerChance;
        
        [ConfigInt("layer-height", "Lava", 0)]
        public int LayerHeight;
        [ConfigInt("layer-count", "Lava", 0)]
        public int LayerCount;
        
        [ConfigReal("layer-interval", "Lava", 2, 0)]
        public float LayerInterval = 2;
        [ConfigReal("round-time", "Lava", 15, 0)]
        public float RoundTimeMins = 15;
        [ConfigReal("flood-time", "Lava", 5, 0)]
        public float FloodTimeMins = 5;
        
        /*
            public Vec3U16 FloodPos, LayerPos;
            public Vec3U16[] safeZone = new Vec3U16[2];
         */
    }
    
    public sealed partial class LSGame : RoundsGame {

        public static LSConfig Config = new LSConfig();
        
        public MapData GenerateMapData(MapSettings settings) {
            MapData data = new MapData(settings);
            data.killer  = rand.Next(1, 101) <= settings.killer;
            data.destroy = rand.Next(1, 101) <= settings.destroy;
            data.water   = rand.Next(1, 101) <= settings.water;
            data.layer   = rand.Next(1, 101) <= settings.layer;
            data.fast    = rand.Next(1, 101) <= settings.fast && !data.water;
            
            BlockID block =
                data.water   ? (data.killer ? Block.Deadly_ActiveWater : Block.Water)
                : (data.fast ? (data.killer ? Block.Deadly_FastLava    : Block.FastLava)
                   : (data.killer ? Block.Deadly_ActiveLava  : Block.Lava));
            data.block = block;
            return data;
        }

        public MapSettings LoadMapSettings(string name) {
            MapSettings settings = new MapSettings();
            settings.name = name;
            
            if (!Directory.Exists(propsDir)) Directory.CreateDirectory(propsDir);
            string path = propsDir + name + ".properties";
            if (!File.Exists(path)) { SaveMapSettings(settings); return settings; }

            try {
                PropertiesFile.Read(path, ref settings, ProcessMapLine);
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return settings;
        }
        
        void ProcessMapLine(string key, string value, ref MapSettings map) {
            switch (key.ToLower()) {
                    case "fast-chance": map.fast = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                    case "killer-chance": map.killer = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                    case "destroy-chance": map.destroy = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                    case "water-chance": map.water = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                    case "layer-chance": map.layer = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                    case "layer-height": map.LayerHeight = int.Parse(value); break;
                    case "layer-count": map.LayerCount = int.Parse(value); break;
                    case "layer-interval": map.layerInterval = double.Parse(value); break;
                    case "round-time": map.roundTime = double.Parse(value); break;
                    case "flood-time": map.floodTime = double.Parse(value); break;
                    
                case "block-flood":
                    map.FloodPos = Vec3U16.Parse(value); break;
                case "block-layer":
                    map.LayerPos = Vec3U16.Parse(value); break;
                case "safe-zone":
                    string[] p = value.Split('-');
                    map.safeZone = new Vec3U16[] { Vec3U16.Parse(p[0]), Vec3U16.Parse(p[1]) };
                    break;
            }
        }
        
        public void SaveMapSettings(MapSettings settings) {
            if (!Directory.Exists(propsDir)) Directory.CreateDirectory(propsDir);

            using (StreamWriter w = new StreamWriter(propsDir + settings.name + ".properties")) {
                w.WriteLine("#Lava Survival properties for " + settings.name);
                w.WriteLine("fast-chance = " + settings.fast);
                w.WriteLine("killer-chance = " + settings.killer);
                w.WriteLine("destroy-chance = " + settings.destroy);
                w.WriteLine("water-chance = " + settings.water);
                w.WriteLine("layer-chance = " + settings.layer);
                w.WriteLine("layer-height = " + settings.LayerHeight);
                w.WriteLine("layer-count = " + settings.LayerCount);
                w.WriteLine("layer-interval = " + settings.layerInterval);
                w.WriteLine("round-time = " + settings.roundTime);
                w.WriteLine("flood-time = " + settings.floodTime);
                w.WriteLine("block-flood = " + settings.FloodPos);
                w.WriteLine("block-layer = " + settings.LayerPos);
                w.WriteLine("safe-zone = " + settings.safeZone[0] + " - " + settings.safeZone[1]);
            }
        }

        // Internal classes
        public class MapSettings {
            public string name;
            public byte fast, killer = 100, destroy, water, layer;
            public int LayerHeight = 3, LayerCount = 10;
            public double layerInterval = 2, roundTime = 15, floodTime = 5;
            public Vec3U16 FloodPos, LayerPos;
            public Vec3U16[] safeZone = new Vec3U16[2];
            
            public void ApplyDefaults(Level lvl) {
                FloodPos = new Vec3U16((ushort)(lvl.Width / 2), (ushort)(lvl.Height - 1), (ushort)(lvl.Length / 2));
                LayerPos = new Vec3U16(0, (ushort)(lvl.Height / 2), 0);
                ushort x = (ushort)(lvl.Width / 2), y = (ushort)(lvl.Height / 2), z = (ushort)(lvl.Length / 2);
                safeZone = new Vec3U16[] { new Vec3U16((ushort)(x - 3), y, (ushort)(z - 3)), 
                    new Vec3U16((ushort)(x + 3), (ushort)(y + 4), (ushort)(z + 3)) };
            }
        }

        public class MapData {
            public bool fast, killer, destroy, water, layer;
            public BlockID block;
            public int currentLayer;
            public int roundTotalSecs, floodDelaySecs, layerIntervalSecs;

            public MapData(MapSettings settings) {
                block = Block.Lava;
                currentLayer = 1;
                roundTotalSecs = (int)(settings.roundTime * 60);
                floodDelaySecs = (int)(settings.floodTime * 60);
                layerIntervalSecs = (int)(settings.layerInterval * 60);
            }
        }
    }
}
