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
        
        static readonly object locker = new object();
        
        public static void Load() {
            if (!File.Exists("extra/bots.json")) return;
            lock (locker) {
                string json = File.ReadAllText("extra/bots.json");
                var bots = JsonConvert.DeserializeObject<BotProperties[]>(json);
                SavedBots = new List<BotProperties>(bots);
            }
        }
        
        static void Save() {
            var bots = SavedBots.ToArray();
            string json = JsonConvert.SerializeObject(bots);
            File.WriteAllText("extra/bots.json", json);
        }

        public static void LoadBots(Level lvl) {
            lock (locker) {
                foreach (BotProperties props in SavedBots) {
                    if (lvl.name != props.Level) continue;
                    PlayerBot bot = new PlayerBot(props.Name, lvl, props.X, props.Y, props.Z, props.RotX, props.RotY);
                    bot.skinName = props.Skin; bot.model = props.Model; bot.color = props.Color;
                    bot.AIName = props.AI; bot.hunt = props.Hunt; bot.kill = props.Kill;
                    
                    PlayerBot.Add(bot, false);
                    if (!String.IsNullOrEmpty(props.AI))
                        BotScript.Parse(null, bot, "bots/" + props.AI);
                }
            }
        }
        
        public static void UnloadBots(Level lvl) {
            lock (locker) {
                PlayerBot[] bots = PlayerBot.Bots.Items;
                foreach (PlayerBot bot in bots) {
                    if (bot.level != lvl) continue;
                    DoUpdateBot(bot, false);
                }
                Save();
            }
        }
        
        public static void UpdateBot(PlayerBot bot) {
            lock (locker) DoUpdateBot(bot, true);
        }
        
        static void DoUpdateBot(PlayerBot bot, bool save) {
            foreach (BotProperties props in SavedBots) {
                if (bot.name != props.Name || bot.level.name != props.Level) continue;
                props.FromBot(bot);
                if (save) Save();
                return;
            }
            
            BotProperties newProps = new BotProperties();
            newProps.FromBot(bot);
            SavedBots.Add(newProps);
            if (save) Save();
        }
        
        public static void RemoveBot(PlayerBot bot) {
            lock (locker) {
                for (int i = 0; i < SavedBots.Count; i++) {
                    BotProperties props = SavedBots[i];
                    if (bot.name != props.Name || bot.level.name != props.Level) continue;
                    SavedBots.RemoveAt(i);
                    Save();
                    return;
                }
            }
        }
        
        public static void RemoveLevelBots(string level) {
            lock (locker) {
                for (int i = 0; i < SavedBots.Count; i++) {
                    BotProperties props = SavedBots[i];
                    if (level != props.Level) continue;
                    SavedBots.RemoveAt(i); i--;
                }
                Save();
            }
        }
    }
    
    public sealed class BotProperties {
        public string Name { get; set; }
        public string Level { get; set; }
        public string Skin { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        
        public string AI { get; set; }
        public bool Kill { get; set; }
        public bool Hunt { get; set; }
        
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Z { get; set; }
        public byte RotX { get; set; }
        public byte RotY { get; set; }
        
        public void FromBot(PlayerBot bot) {
            Name = bot.name; Level = bot.level.name;
            Skin = bot.skinName; AI = bot.AIName;
            Model = bot.model; Color = bot.color;
            Kill = bot.kill; Hunt = bot.hunt;
            
            X = bot.pos[0]; Y = bot.pos[1]; Z = bot.pos[2];
            RotX = bot.rot[0]; RotY = bot.rot[1];
        }
    }
}
