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

namespace MCGalaxy.Games {
    public sealed partial class LSGame : RoundsGame {

        public MapData GenerateMapData(MapSettings settings) {
            MapData data = new MapData(settings);
            data.killer  = rand.Next(1, 101) <= settings.killer;
            data.destroy = rand.Next(1, 101) <= settings.destroy;
            data.water   = rand.Next(1, 101) <= settings.water;
            data.layer   = rand.Next(1, 101) <= settings.layer;
            data.fast    = rand.Next(1, 101) <= settings.fast && !data.water;
            
            byte block = data.water ? (data.killer ? Block.Deadly_ActiveWater : Block.Water) 
                : (data.fast ? (data.killer ? Block.Deadly_FastLava : Block.FastLava) 
                   : (data.killer ? Block.Deadly_ActiveLava : Block.Lava));
            data.block = (ushort)block;
            return data;
        }

        bool needsSaveSettings;
        public void LoadSettings() {
            if (!File.Exists("properties/lavasurvival.properties")) { SaveSettings(); return; }

            try {
                PropertiesFile.Read("properties/lavasurvival.properties", ProcessSettingsLine);
                if (needsSaveSettings) SaveSettings();
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        void ProcessSettingsLine(string key, string value) {
            switch (key.ToLower()) {
                case "start-on-startup": startOnStartup = bool.Parse(value); break;
                case "send-afk-to-main": sendAfkMain = bool.Parse(value); break;
                case "vote-count": voteCount = (byte)Utils.Clamp(int.Parse(value), 2, 10); break;
                case "vote-time": voteTime = double.Parse(value); break;
                case "lives": lifeNum = int.Parse(value); break;
                    
                case "setup-rank":
                    LevelPermission setupRank = Group.ParsePermOrName(value, LevelPermission.Admin);
                    UpdateExtraPerms(setupRank, 1);
                    break;
                case "control-rank":
                    LevelPermission controlRank = Group.ParsePermOrName(value, LevelPermission.Operator);
                    UpdateExtraPerms(controlRank, 2);
                    break;
                case "maps":
                    foreach (string name in value.Split(',')) {
                        string map = name.Trim();
                        if (map.Length > 0 && !maps.CaselessContains(map))
                            maps.Add(map);
                    }
                    break;
            }
        }
        
        void UpdateExtraPerms(LevelPermission perm, int num) {
            CommandExtraPerms.Load();
            CommandExtraPerms.Set("lavasurvival", perm, "temp desc", num);
            CommandExtraPerms.Save();
            needsSaveSettings = true;
        }
        
        public void SaveSettings() {
            using (StreamWriter w = new StreamWriter("properties/lavasurvival.properties")) {
                w.WriteLine("#Lava Survival main properties");
                w.WriteLine("start-on-startup = " + startOnStartup);
                w.WriteLine("send-afk-to-main = " + sendAfkMain);
                w.WriteLine("vote-count = " + voteCount);
                w.WriteLine("vote-time = " + voteTime);
                w.WriteLine("lives = " + lifeNum);
                w.WriteLine("maps = " + maps.Join());
            }
        }

        public MapSettings LoadMapSettings(string name) {
            MapSettings settings = new MapSettings(name);
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
        public class MapSettings
        {
            public string name;
            public byte fast, killer, destroy, water, layer;
            public int LayerHeight, LayerCount;
            public double layerInterval, roundTime, floodTime;
            public Vec3U16 FloodPos, LayerPos;
            public Vec3U16[] safeZone;

            public MapSettings(string name)
            {
                this.name = name;
                fast = 0;
                killer = 100;
                destroy = 0;
                water = 0;
                layer = 0;
                LayerHeight = 3;
                LayerCount = 10;
                layerInterval = 2;
                roundTime = 15;
                floodTime = 5;
                FloodPos = new Vec3U16();
                LayerPos = new Vec3U16();
                safeZone = new Vec3U16[2];
            }
        }

        public class MapData : IDisposable
        {
            public bool fast, killer, destroy, water, layer;
            public BlockID block;
            public int currentLayer;
            public Timer roundTimer, floodTimer, layerTimer;

            public MapData(MapSettings settings)
            {
                fast = false;
                killer = false;
                destroy = false;
                water = false;
                layer = false;
                block = Block.Lava;
                currentLayer = 1;
                roundTimer = new Timer(TimeSpan.FromMinutes(settings.roundTime).TotalMilliseconds); roundTimer.AutoReset = false;
                floodTimer = new Timer(TimeSpan.FromMinutes(settings.floodTime).TotalMilliseconds); floodTimer.AutoReset = false;
                layerTimer = new Timer(TimeSpan.FromMinutes(settings.layerInterval).TotalMilliseconds); layerTimer.AutoReset = true;
            }

            public void Dispose()
            {
                roundTimer.Dispose();
                floodTimer.Dispose();
                layerTimer.Dispose();
            }
        }
    }
}
