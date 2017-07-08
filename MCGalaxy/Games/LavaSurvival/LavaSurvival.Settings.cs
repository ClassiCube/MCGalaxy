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
using MCGalaxy.Maths;

namespace MCGalaxy.Games {
    public sealed partial class LavaSurvival {

        public MapData GenerateMapData(MapSettings settings) {
            MapData data = new MapData(settings);
            data.killer = rand.Next(1, 101) <= settings.killer;
            data.destroy = rand.Next(1, 101) <= settings.destroy;
            data.water = rand.Next(1, 101) <= settings.water;
            data.layer = rand.Next(1, 101) <= settings.layer;
            data.fast = rand.Next(1, 101) <= settings.fast && !data.water;
            
            byte block = data.water ? (data.killer ? Block.activedeathwater : Block.water) 
                : (data.fast ? (data.killer ? Block.fastdeathlava : Block.lava_fast) 
                   : (data.killer ? Block.activedeathlava : Block.lava));
            data.block = (ExtBlock)block;
            return data;
        }

        public void LoadSettings() {
            if (!File.Exists("properties/lavasurvival.properties")) { SaveSettings(); return; }

            try {
                PropertiesFile.Read("properties/lavasurvival.properties", ProcessSettingsLine);
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        void ProcessSettingsLine(string key, string value) {
            LevelPermission perm;
            switch (key.ToLower()) {
                case "start-on-startup": startOnStartup = bool.Parse(value); break;
                case "send-afk-to-main": sendAfkMain = bool.Parse(value); break;
                case "vote-count": voteCount = (byte)Utils.Clamp(decimal.Parse(value), 2, 10); break;
                case "vote-time": voteTime = double.Parse(value); break;
                case "lives": lifeNum = int.Parse(value); break;
                    
                case "setup-rank":
                    setupRank = Group.ParsePermOrName(value, LevelPermission.Admin);
                    break;
                case "control-rank":
                    controlRank = Group.ParsePermOrName(value, LevelPermission.Operator);
                    break;
                case "maps":
                    foreach (string name in value.Split(',')) {
                        string map = name.Trim();
                        if (map != "" && !maps.Contains(map))
                            maps.Add(map);
                    }
                    break;
            }
        }
        
        public void SaveSettings() {
            using (StreamWriter w = new StreamWriter("properties/lavasurvival.properties")) {
                w.WriteLine("#Lava Survival main properties");
                w.WriteLine("start-on-startup = " + startOnStartup);
                w.WriteLine("send-afk-to-main = " + sendAfkMain);
                w.WriteLine("vote-count = " + voteCount);
                w.WriteLine("vote-time = " + voteTime);
                w.WriteLine("lives = " + lifeNum);
                w.WriteLine("setup-rank = " + (int)setupRank);
                w.WriteLine("control-rank = " + (int)controlRank);
                w.WriteLine("maps = " + maps.Join());
            }
        }

        public MapSettings LoadMapSettings(string name) {
            MapSettings settings = new MapSettings(name);
            if (!Directory.Exists(propsPath)) Directory.CreateDirectory(propsPath);
            string path = propsPath + name + ".properties";
            if (!File.Exists(path)) { SaveMapSettings(settings); return settings; }

            try {
                PropertiesFile.Read(path, ref settings, ProcessMapLine);
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return settings;
        }
        
        void ProcessMapLine(string key, string value, ref MapSettings map) {
            string[] sp;
            switch (key.ToLower()) {
                case "fast-chance": map.fast = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                case "killer-chance": map.killer = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                case "destroy-chance": map.destroy = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                case "water-chance": map.water = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                case "layer-chance": map.layer = (byte)Utils.Clamp(int.Parse(value), 0, 100); break;
                case "layer-height": map.layerHeight = int.Parse(value); break;
                case "layer-count": map.layerCount = int.Parse(value); break;
                case "layer-interval": map.layerInterval = double.Parse(value); break;
                case "round-time": map.roundTime = double.Parse(value); break;
                case "flood-time": map.floodTime = double.Parse(value); break;
                
                case "block-flood":
                    sp = value.Split(',');
                    map.blockFlood = new Vec3U16(ushort.Parse(sp[0]), ushort.Parse(sp[1]), ushort.Parse(sp[2]));
                    break;
                case "block-layer":
                    sp = value.Split(',');
                    map.blockLayer = new Vec3U16(ushort.Parse(sp[0]), ushort.Parse(sp[1]), ushort.Parse(sp[2]));
                    break;
                case "safe-zone":
                    sp = value.Split('-');
                    string[] p1 = sp[0].Split(','), p2 = sp[1].Split(',');
                    map.safeZone = new Vec3U16[] {
                        new Vec3U16(ushort.Parse(p1[0]), ushort.Parse(p1[1]), ushort.Parse(p1[2])),
                        new Vec3U16(ushort.Parse(p2[0]), ushort.Parse(p2[1]), ushort.Parse(p2[2])) };
                    break;
            }
        }
        
        public void SaveMapSettings(MapSettings settings) {
            if (!Directory.Exists(propsPath)) Directory.CreateDirectory(propsPath);

            using (StreamWriter w = new StreamWriter(propsPath + settings.name + ".properties")) {
                w.WriteLine("#Lava Survival properties for " + settings.name);
                w.WriteLine("fast-chance = " + settings.fast);
                w.WriteLine("killer-chance = " + settings.killer);
                w.WriteLine("destroy-chance = " + settings.destroy);
                w.WriteLine("water-chance = " + settings.water);
                w.WriteLine("layer-chance = " + settings.layer);
                w.WriteLine("layer-height = " + settings.layerHeight);
                w.WriteLine("layer-count = " + settings.layerCount);
                w.WriteLine("layer-interval = " + settings.layerInterval);
                w.WriteLine("round-time = " + settings.roundTime);
                w.WriteLine("flood-time = " + settings.floodTime);
                w.WriteLine("block-flood = " + settings.blockFlood);
                w.WriteLine("block-layer = " + settings.blockLayer);
                w.WriteLine(String.Format("safe-zone = {0}-{1}", settings.safeZone[0].ToString(), settings.safeZone[1].ToString()));
            }
        }

        // Internal classes
        public class MapSettings
        {
            public string name;
            public byte fast, killer, destroy, water, layer;
            public int layerHeight, layerCount;
            public double layerInterval, roundTime, floodTime;
            public Vec3U16 blockFlood, blockLayer;
            public Vec3U16[] safeZone;

            public MapSettings(string name)
            {
                this.name = name;
                fast = 0;
                killer = 100;
                destroy = 0;
                water = 0;
                layer = 0;
                layerHeight = 3;
                layerCount = 10;
                layerInterval = 2;
                roundTime = 15;
                floodTime = 5;
                blockFlood = new Vec3U16();
                blockLayer = new Vec3U16();
                safeZone = new Vec3U16[2];
            }
        }

        public class MapData : IDisposable
        {
            public bool fast, killer, destroy, water, layer;
            public ExtBlock block;
            public int currentLayer;
            public Timer roundTimer, floodTimer, layerTimer;

            public MapData(MapSettings settings)
            {
                fast = false;
                killer = false;
                destroy = false;
                water = false;
                layer = false;
                block = (ExtBlock)Block.lava;
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
