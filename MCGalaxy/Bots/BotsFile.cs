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
        static ConfigElement[] elems;
        
        public static void Load(Level lvl) { lock (lvl.botsIOLock) { LoadCore(lvl); } }
        static void LoadCore(Level lvl) {
            string path = Paths.BotsPath(lvl.MapName);
            if (!File.Exists(path)) return;
            List<BotProperties> props = null;
            
            try {
                props = ReadAll(path);
            } catch (Exception ex) {
                Logger.LogError("Reading bots file", ex); return;
            }
            
            foreach (BotProperties data in props) {
                PlayerBot bot = new PlayerBot(data.Name, lvl);
                data.ApplyTo(bot);
                
                bot.SetModel(bot.Model);
                LoadAi(data, bot);
                PlayerBot.Add(bot, false);
            }
        }
        
        internal static List<BotProperties> ReadAll(string path) {
            List<BotProperties> props = new List<BotProperties>();
            if (elems == null) elems = ConfigElement.GetAll(typeof(BotProperties));
            string json = File.ReadAllText(path);
            
            JsonReader reader = new JsonReader(json);
            reader.OnMember   = (obj, key, value) => {
                if (obj.Meta == null) obj.Meta = new BotProperties();
                ConfigElement.Parse(elems, obj.Meta, key, (string)value);
            };
            
            JsonArray array = (JsonArray)reader.Parse();
            if (array == null) return props;
            
            foreach (object raw in array) {
                JsonObject obj = (JsonObject)raw;
                if (obj == null || obj.Meta == null) continue;
                
                BotProperties data = (BotProperties)obj.Meta;
                if (String.IsNullOrEmpty(data.DisplayName)) data.DisplayName = data.Name;
                props.Add(data);
            }
            return props;
        }
        
        public static void Save(Level lvl) { lock (lvl.botsIOLock) { SaveCore(lvl); } }
        static void SaveCore(Level lvl) {
            PlayerBot[] bots = lvl.Bots.Items;
            string path = Paths.BotsPath(lvl.MapName);
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

        internal static void WriteAll(TextWriter dst, List<BotProperties> props) {
            if (elems == null) elems = ConfigElement.GetAll(typeof(BotProperties));

            JsonConfigWriter w = new JsonConfigWriter(dst, elems);
            w.WriteArray(props);
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
        [ConfigString] public string Owner;
        
        [ConfigString] public string AI;
        [ConfigBool] public bool Kill;
        [ConfigBool] public bool Hunt;
        [ConfigInt] public int CurInstruction;
        [ConfigInt] public int CurJump;
        [ConfigInt] public int CurSpeed;
        
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
            Owner = bot.Owner;
            Name = bot.name; Level = bot.level.name;
            Skin = bot.SkinName; AI = bot.AIName;
            Model = bot.Model; Color = bot.color;
            Kill = bot.kill; Hunt = bot.hunt;
            DisplayName = bot.DisplayName;
            CurInstruction = bot.cur; CurJump = bot.curJump; CurSpeed = bot.movementSpeed;
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
            
            bot.Owner = Owner;
            bot.SkinName = Skin; bot.Model = Model; bot.color = Color;
            bot.AIName = AI; bot.hunt = Hunt; bot.kill = Kill;
            bot.DisplayName = DisplayName;
            
            bot.cur = CurInstruction; bot.curJump = CurJump;
            // NOTE: This field wasn't in old json 
            if (CurSpeed != 0) bot.movementSpeed = CurSpeed;
            bot.ClickedOnText = ClickedOnText; bot.DeathMessage = DeathMessage;
            bot.ScaleX = ScaleX; bot.ScaleY = ScaleY; bot.ScaleZ = ScaleZ;
        }
    }
}
