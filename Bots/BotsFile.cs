/*
    Copyright 2015 MCGalaxy
        
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
using Newtonsoft.Json;

namespace MCGalaxy.Bots {

    /// <summary> Maintains persistent data for in-game bots. </summary>
    public static class BotsFile {
        
        public static List<BotProperties> SavedBots = new List<BotProperties>();
        
        public static void Load() {
            if (!File.Exists("extra/bots.json")) return;
            string json = File.ReadAllText("extra/bots.json");
            var bots = JsonConvert.DeserializeObject<BotProperties[]>(json);
            SavedBots = new List<BotProperties>(bots);
        }
        
        public static void Save() {
            var bots = SavedBots.ToArray();
            string json = JsonConvert.SerializeObject(bots);
            File.WriteAllText("extra/bots.json", json);
        }
    }
    
    public sealed class BotProperties {
        public string Name { get; set; }
        public string LevelName { get; set; }
        public string SkinName { get; set; }
        public string AIName { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        
        public byte ID { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Z { get; set; }
        public byte RotX { get; set; }
        public byte RotY { get; set; }
    }
}
