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
using System.IO;
using MCGalaxy.Maths;
using Newtonsoft.Json;

namespace MCGalaxy.Bots {

    /// <summary> Maintains persistent data for in-game bots. </summary>
    public static class BotsFile {

        public static string BotsPath(string mapName) {
            return "extra/bots/" + mapName + ".json";
        }

        public static void Load(Level lvl) { lock (lvl.botsIOLock) { LoadCore(lvl); } }
        static void LoadCore(Level lvl) {
            string path = BotsPath(lvl.MapName);
            if (!File.Exists(path)) return;
            string json = File.ReadAllText(path);
            BotProperties[] bots = JsonConvert.DeserializeObject<BotProperties[]>(json);
            
            foreach (BotProperties props in bots) {
                if (String.IsNullOrEmpty(props.DisplayName)) {
                    props.DisplayName = props.Name;
                }
                
                PlayerBot bot = new PlayerBot(props.Name, lvl);
                props.ApplyTo(bot);
                
                bot.ModelBB = AABB.ModelAABB(bot.Model, lvl);
                LoadAi(props, bot);
                PlayerBot.Add(bot, false);
            }
        }
        
        public static void Save(Level lvl) { lock (lvl.botsIOLock) { SaveCore(lvl); } }
        static void SaveCore(Level lvl) {
            PlayerBot[] bots = lvl.Bots.Items;
            string path = BotsPath(lvl.MapName);
            if (!File.Exists(path) && bots.Length == 0) return;
            
            BotProperties[] props = new BotProperties[bots.Length];
            for (int i = 0; i < props.Length; i++) {
                BotProperties savedProps = new BotProperties();
                savedProps.FromBot(bots[i]);
                props[i] = savedProps;
            }
            
            string json = JsonConvert.SerializeObject(props);
            try {
                File.WriteAllText(path, json);
            } catch (Exception ex) {
                Logger.Log(LogType.Warning, "Failed to save bots file");
                Logger.LogError(ex);
            }            
        }
        
        static void LoadAi(BotProperties props, PlayerBot bot) {
            if (String.IsNullOrEmpty(props.AI)) return;
            try {
                ScriptFile.Parse(null, bot, "bots/" + props.AI);
            } catch (Exception ex)  {
                Logger.LogError(ex);
            }
            
            bot.cur = props.CurInstruction;
            if (bot.cur >= bot.Instructions.Count) bot.cur = 0;
        }
    }
    
    public sealed class BotProperties {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Skin { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string ClickedOnText { get; set; }
        
        public string AI { get; set; }
        public bool Kill { get; set; }
        public bool Hunt { get; set; }
        public int CurInstruction { get; set; }
        
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public byte RotX { get; set; }
        public byte RotY { get; set; }
        public byte BodyX { get; set; }
        public byte BodyZ { get; set; }
        
        public void FromBot(PlayerBot bot) {
            Name = bot.name; Level = bot.level.name;
            Skin = bot.SkinName; AI = bot.AIName;
            Model = bot.Model; Color = bot.color;
            Kill = bot.kill; Hunt = bot.hunt;
            DisplayName = bot.DisplayName;
            CurInstruction = bot.cur;
            ClickedOnText = bot.ClickedOnText;
            
            X = bot.Pos.X; Y = bot.Pos.Y; Z = bot.Pos.Z;
            RotX = bot.Rot.RotY; RotY = bot.Rot.HeadX;
            BodyX = bot.Rot.RotX; BodyZ = bot.Rot.RotZ;
        }
        
        public void ApplyTo(PlayerBot bot) {
            bot.Pos = new Position(X, Y, Z);
            bot.SetYawPitch(RotX, RotY);
            Orientation rot = bot.Rot;
            rot.RotX = BodyX; rot.RotZ = BodyZ;
            bot.Rot = rot;
            
            bot.SkinName = Skin; bot.Model = Model; bot.color = Color;
            bot.AIName = AI; bot.hunt = Hunt; bot.kill = Kill;
            bot.DisplayName = DisplayName;
            
            bot.cur = CurInstruction;
            bot.ClickedOnText = ClickedOnText;
        }
        
        public BotProperties Copy() {
            BotProperties copy = new BotProperties();
            copy.DisplayName = DisplayName; copy.Name = Name;
            copy.Level = Level; copy.Skin = Skin;
            copy.Model = Model; copy.Color = Color;
            
            copy.AI = AI; copy.Kill = Kill; copy.Hunt = Hunt;
            copy.CurInstruction = CurInstruction;
            copy.ClickedOnText = ClickedOnText;
            
            copy.X = X; copy.Y = Y; copy.Z = Z;
            copy.RotX = RotX; copy.RotY = RotY;
            copy.BodyX = BodyX; copy.BodyZ = BodyZ;
            return copy;
        }
    }
}
