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
using MCGalaxy.Config;

namespace MCGalaxy.Bots {

    /// <summary> Maintains persistent data for in-game bots. </summary>
    public static class BotsFile {

        public static string BotsPath(string map) { return "extra/bots/" + map + ".json"; }
        static ConfigElement[] elems;
        
        public static void Load(Level lvl) { lock (lvl.botsIOLock) { LoadCore(lvl); } }
        static void LoadCore(Level lvl) {
            string path = BotsPath(lvl.MapName);
            if (!File.Exists(path)) return;
            string json = File.ReadAllText(path);
            List<BotProperties> props = null;
            
            try {
                props = ReadAll(json);
            } catch (Exception ex) {
                Logger.LogError("Reading bots file", ex); return;
            }
            
            foreach (BotProperties data in props) {
                PlayerBot bot = new PlayerBot(data.Name, lvl);
                data.ApplyTo(bot);
                
                bot.SetModel(bot.Model, lvl);
                LoadAi(data, bot);
                PlayerBot.Add(bot, false);
            }
        }
        
        internal static List<BotProperties> ReadAll(string json) {
            List<BotProperties> props = new List<BotProperties>();
            if (elems == null) elems = ConfigElement.GetAll(typeof(BotProperties));
            
            JsonContext ctx = new JsonContext(); ctx.Val = json;
            JsonArray array = (JsonArray)Json.ParseStream(ctx);
            if (array == null) return props;
            
            foreach (object raw in array) {
                JsonObject obj = (JsonObject)raw;
                if (obj == null) continue;
                
                BotProperties data = new BotProperties();
                obj.Deserialise(elems, data);
                
                if (String.IsNullOrEmpty(data.DisplayName)) data.DisplayName = data.Name;
                props.Add(data);
            }
            return props;
        }
        
        public static void Save(Level lvl) { lock (lvl.botsIOLock) { SaveCore(lvl); } }
        static void SaveCore(Level lvl) {
            PlayerBot[] bots = lvl.Bots.Items;
            string path = BotsPath(lvl.MapName);
            if (!File.Exists(path) && bots.Length == 0) return;
            
            List<BotProperties> props = new List<BotProperties>(bots.Length);
            for (int i = 0; i < bots.Length; i++) {
                BotProperties data = new BotProperties();
                data.FromBot(bots[i]);
                props.Add(data);
            }
            
            try {
                using (StreamWriter w = new StreamWriter(path)) { WriteAll(w, props); }
            } catch (Exception ex) {
                Logger.LogError("Error saving bots to " + path, ex);
            }
        }
        
        internal static void WriteAll(StreamWriter w, List<BotProperties> props) {
            w.WriteLine("[");
            if (elems == null) elems = ConfigElement.GetAll(typeof(BotProperties));
            string separator = null;
            
            for (int i = 0; i < props.Count; i++) {
                w.Write(separator);
                Json.Serialise(w, elems, props[i]);
                separator = ",\r\n";
            }
            w.WriteLine("]");
        }
        
        internal static void LoadAi(BotProperties props, PlayerBot bot) {
            if (String.IsNullOrEmpty(props.AI)) return;
            try {
                ScriptFile.Parse(Player.Console, bot, props.AI);
            } catch (Exception ex) {
                Logger.LogError("Error loading bot AI " + props.AI, ex);
            }
            
            bot.cur = props.CurInstruction;
            if (bot.cur >= bot.Instructions.Count) bot.cur = 0;
        }
    }
    
    public sealed class BotProperties {
        [ConfigString] public string DisplayName;
        [ConfigString] public string Name;
        [ConfigString] public string Level;
        [ConfigString] public string Skin;
        [ConfigString] public string Model;
        [ConfigString] public string Color;
        [ConfigString] public string ClickedOnText;
        [ConfigString] public string DeathMessage;
        
        [ConfigString] public string AI;
        [ConfigBool] public bool Kill;
        [ConfigBool] public bool Hunt;
        [ConfigInt] public int CurInstruction;
        [ConfigInt] public int CurJump;
        
        [ConfigInt] public int X;
        [ConfigInt] public int Y;
        [ConfigInt] public int Z;
        [ConfigByte] public byte RotX;
        [ConfigByte] public byte RotY;
        
        [ConfigByte] public byte BodyX;
        [ConfigByte] public byte BodyZ;
        [ConfigFloat] public float ScaleX;
        [ConfigFloat] public float ScaleY;
        [ConfigFloat] public float ScaleZ;
        
        public void FromBot(PlayerBot bot) {
            Name = bot.name; Level = bot.level.name;
            Skin = bot.SkinName; AI = bot.AIName;
            Model = bot.Model; Color = bot.color;
            Kill = bot.kill; Hunt = bot.hunt;
            DisplayName = bot.DisplayName;
            CurInstruction = bot.cur; CurJump = bot.curJump;
            ClickedOnText = bot.ClickedOnText; DeathMessage = bot.DeathMessage;
            
            X = bot.Pos.X; Y = bot.Pos.Y; Z = bot.Pos.Z;
            RotX = bot.Rot.RotY; RotY = bot.Rot.HeadX;
            BodyX = bot.Rot.RotX; BodyZ = bot.Rot.RotZ;
            ScaleX = bot.ScaleX; ScaleY = bot.ScaleY; ScaleZ = bot.ScaleZ;
        }
        
        public void ApplyTo(PlayerBot bot) {
            bot.SetInitialPos(new Position(X, Y, Z));
            bot.SetYawPitch(RotX, RotY);
            Orientation rot = bot.Rot;
            rot.RotX = BodyX; rot.RotZ = BodyZ;
            bot.Rot = rot;
            
            bot.SkinName = Skin; bot.Model = Model; bot.color = Color;
            bot.AIName = AI; bot.hunt = Hunt; bot.kill = Kill;
            bot.DisplayName = DisplayName;
            
            bot.cur = CurInstruction; bot.curJump = CurJump;
            bot.ClickedOnText = ClickedOnText; bot.DeathMessage = DeathMessage;
            bot.ScaleX = ScaleX; bot.ScaleY = ScaleY; bot.ScaleZ = ScaleZ;
        }
    }
}
