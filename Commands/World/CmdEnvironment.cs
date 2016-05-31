/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Commands.Building;

namespace MCGalaxy.Commands {
    public sealed class CmdEnvironment : Command {
        public override string name { get { return "environment"; } }
        public override string shortcut { get { return "env"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            message = message.ToLower();
            if (message == "preset" || message == "l preset" || message == "level preset") {
                SendPresetsMessage(p); return;
            }
            
            string[] args = message.Split(' ');
            if (args.Length <= 1) {
                if (!message.CaselessEq("normal")) { Help(p); return; }
                ResetEnv(p);
            } else {
                // Adjust for the older version of the command which had /env p and used /env l.
                if (args[0] == "player" || args[0] == "p") {
                    Player.Message(p, "Change your own env settings by pressing " +
                                   "escape and going to the appropriate options menu."); return;
                } else if ((args[0] == "level" || args[0] == "l") && args.Length >= 3) {
                    args[0] = args[1];
                    args[1] = args[2];
                }
                Handle(p, args[0], args[1]);
            }
        }
        
        void Handle(Player p, string variable, string value) {
            Level lvl = p.level;
            switch (variable) {
                case "fog":
                    SetColour(p, value, 2, "fog", ref lvl.FogColor); break;
                case "cloud":
                case "clouds":
                    SetColour(p, value, 1, "cloud", ref lvl.CloudColor); break;
                case "sky":
                    SetColour(p, value, 0, "sky", ref lvl.SkyColor); break;
                case "dark":
                case "shadow":
                    SetColour(p, value, 3, "shadow", ref lvl.ShadowColor); break;
                case "sun":
                case "light":
                case "sunlight":
                    SetColour(p, value, 4, "sunlight", ref lvl.LightColor); break;
                case "weather":
                    SetWeather(p, value); break;
                case "cloudsheight":
                case "cloudheight":
                    SetShort(p, value, EnvProp.CloudsLevel,
                             "clouds height", (short)(lvl.Height + 2), ref lvl.CloudsHeight); break;
                case "waterlevel":
                case "edgelevel":
                case "level":
                    SetShort(p, value, EnvProp.EdgeLevel,
                             "water level", (short)(lvl.Height / 2), ref lvl.EdgeLevel); break;
                case "maxfogdistance":
                case "maxfog":
                case "fogdistance":
                case "fogdist":
                    SetShort(p, value, EnvProp.MaxFog,
                             "max fog distance", 0, ref lvl.MaxFogDistance); break;
                case "cloudspeed":
                case "cloudsspeed":
                    SetFloat(p, value, EnvProp.CloudsSpeed, 256, "clouds speed",
                             256, ref lvl.CloudsSpeed, -32767, 32767); break;
                case "weatherspeed":
                    SetFloat(p, value, EnvProp.WeatherSpeed, 256, "weather speed",
                             256, ref lvl.WeatherSpeed, -32767, 32767); break;
                case "weatherfade":
                    SetFloat(p, value, EnvProp.WeatherFade, 128, "weather fade rate",
                             128, ref lvl.WeatherFade, 0, 255); break;
                case "horizon":
                case "edge":
                case "water":
                    SetBlock(p, value, EnvProp.EdgeBlock,
                             "edge block", Block.waterstill, ref lvl.HorizonBlock); break;
                case "side":
                case "border":
                case "bedrock":
                    SetBlock(p, value, EnvProp.SidesBlock,
                             "sides block", Block.blackrock, ref lvl.EdgeBlock); break;
                case "preset":
                    if (!SetPreset(p, value)) return;
                    break;
                default:
                    Help(p); return;
            }
            p.level.Save(true);
        }
        
        static void ResetEnv(Player p) {
            Level lvl = p.level;
            SetPreset(p, "normal");
            SetWeather(p, "normal");
            
            SetBlock(p, "normal", EnvProp.EdgeBlock,
                     "edge block", Block.waterstill, ref lvl.HorizonBlock);
            SetBlock(p, "normal", EnvProp.SidesBlock,
                     "sides block", Block.blackrock, ref lvl.EdgeBlock);
            
            SetShort(p, "normal", EnvProp.EdgeLevel,
                     "water level", (short)(lvl.Height / 2), ref lvl.EdgeLevel);
            SetShort(p, "normal", EnvProp.CloudsLevel,
                     "clouds height", (short)(lvl.Height + 2), ref lvl.CloudsHeight);
        }
        
        static void SetWeather(Player p, string value) {
            byte weather = 255;
            if (IsResetString(value)) {
                Player.Message(p, "Reset weather for {0}%S to 0 (sun)", p.level.name);
                weather = 0;
            } else {
                if (byte.TryParse(value, out weather)) {
                } else if (value.CaselessEq("sun")) { weather = 0;
                } else if (value.CaselessEq("rain")) { weather = 1;
                } else if (value.CaselessEq("snow")) { weather = 2;
                }
                
                if (weather > 2) {
                    Player.Message(p, "Please use a valid integer (0,1,2) or string (sun,rain,snow)"); return;
                }
                string type = weather == 0 ? "%SSun" : (weather == 1 ? "&1Rain" : "&fSnow");
                Player.Message(p, "&aSet weather for {0}&a to {1} ({2}&a)", p.level.name, weather, type);
            }
            
            p.level.Weather = weather;
            // Send the changed colour to all players affected by the command.
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == p.level && pl.HasCpeExt(CpeExt.EnvWeatherType))
                    pl.SendSetMapWeather(weather);
            }
        }
        
        static void SetBlock(Player p, string value, EnvProp prop,
                             string variable, byte defValue, ref byte target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {0}%S to normal", variable, p.level.name);
                target = defValue;
            } else {
                if (!CheckBlock(p, value, variable, ref target)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        static void SetShort(Player p, string value, EnvProp prop,
                             string variable, short defValue, ref int target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {0}%S to normal", variable, p.level.name);
                target = defValue;
            } else {
                if (!CheckShort(p, value, variable, ref target)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        static void SetFloat(Player p, string value, EnvProp prop, int scale, string variable,
                             short defValue, ref int target, int min, int max) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {0}%S to normal", variable, p.level.name);
                target = defValue;
            } else {
                if (!CheckFloat(p, value, variable, ref target, scale, min, max)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        static bool CheckBlock(Player p, string value, string variable, ref byte modify) {
            byte extBlock = 0;
            byte block = DrawCmd.GetBlock(p, value, out extBlock, false);
            if (block == Block.Zero) return false;
            if (block >= Block.CpeCount && block != Block.custom_block) {
                Player.Message(p, "Cannot use physics block ids for /env."); return false;
            }
            
            if (block == Block.shrub || block == Block.yellowflower || block == Block.redflower ||
                block == Block.mushroom || block == Block.redmushroom || block == Block.rope || block == Block.fire) {
                Player.Message(p, "Env: Cannot use {0} for {1}.", block, variable);
            } else {
                modify = block == Block.custom_block ? extBlock : block;
                Player.Message(p, "Set {0} for {1}%S to {2}", variable, p.level.name, modify);
                return true;
            }
            return false;
        }
        
        static bool CheckShort(Player p, string raw, string variable, ref int modify) {
            short value;
            if (!short.TryParse(raw, out value)) {
                Player.Message(p, "Env: \"{0}\" is not a valid integer.", value);
                return false;
            } else {
                modify = value;
                Player.Message(p, "Set {0} for {1}%S to {2}", variable, p.level.name, value);
                return true;
            }
        }
        
        static bool CheckFloat(Player p, string raw, string variable,
                               ref int modify, int scale, float min, float max) {
            float value;
            min /= scale; max /= scale;
            
            if (!float.TryParse(raw, out value)) {
                Player.Message(p, "Env: \"{0}\" is not a valid decimal.", value);
                return false;
            } else if (value < min || value > max) {
                Player.Message(p, "Env: \"{0}\" must be between {1} and {2}.",
                               value, min.ToString("F2"), max.ToString("F2"));
                return false;
            } else {
                modify = (int)(value * scale);
                Player.Message(p, "Set {0} for {1}%S to {2}", variable, p.level.name, value.ToString("F2"));
                return true;
            }
        }
        
        static bool SetPreset(Player p,  string value) {
            EnvPreset preset = null; // fog, sky, clouds, sun, shadow
            if (value.CaselessEq("midnight")) {
                preset = new EnvPreset("8b8989", "191970", "000080", "0000cd", "918A3B");
            } else if (value.CaselessEq("cartoon")) {
                preset = new EnvPreset("00ffff", "1e90ff", "00bfff", "f5deb3", "f4a460");
            } else if (value.CaselessEq("noir")) {
                preset = new EnvPreset("000000", "1f1f1f", "000000", "696969", "1f1f1f");
            } else if (value.CaselessEq("trippy")) {
                preset = new EnvPreset("4B0082", "FFD700", "006400", "7CFC00", "B22222");
            } else if (value.CaselessEq("watery")) {
                preset = new EnvPreset("5f9ea0", "008080", "008B8B", "E0FFFF", "008B8B");
            } else if (value.CaselessEq("normal")) {
                preset = new EnvPreset("-1", "-1", "-1", "-1", "-1");
            } else if (value.CaselessEq("gloomy")) {
                preset = new EnvPreset("6A80A5", "405875", "405875", "444466", "3B3B59");
            } else if (value.CaselessEq("cloudy")) {
                preset = new EnvPreset("AFAFAF", "8E8E8E", "8E8E8E", "9b9b9b", "8C8C8C");
            } else if (value.CaselessEq("sunset")) {
                preset = new EnvPreset("FFA322", "836668", "9A6551", "7F6C60", "46444C");
            } else if (value.CaselessEq("midnight2")) {
                preset = new EnvPreset("131947", "070A23", "1E223A", "181828", "0F0F19");
            }
            
            if( preset != null ) {
                SendEnvColorPackets(p, 0, preset.Sky);
                p.level.SkyColor = preset.Sky;
                SendEnvColorPackets(p, 1, preset.Clouds);
                p.level.CloudColor = preset.Clouds;
                SendEnvColorPackets(p, 2, preset.Fog);
                p.level.FogColor = preset.Fog;
                SendEnvColorPackets(p, 3, preset.Shadow);
                p.level.ShadowColor = preset.Shadow;
                SendEnvColorPackets(p, 4, preset.Sun);
                p.level.LightColor = preset.Sun;
                return true;
            } else {
                SendPresetsMessage(p);
                return false;
            }
        }
        
        class EnvPreset {
            public string Fog, Sky, Clouds, Sun, Shadow;
            
            public EnvPreset( string fog, string sky, string clouds,
                             string sun, string shadow ) {
                Fog = fog; Sky = sky; Clouds = clouds;
                Sun = sun; Shadow = shadow;
            }
        }
        
        static bool IsValidHex(string hex) {
            for (int i = 0; i < hex.Length; i++) {
                if (!Colors.IsStandardColor(hex[i])) return false;
            }
            return true;
        }
        
        static bool IsResetString(string value) {
            return value.CaselessEq("-1") || value.CaselessEq("normal") ||
                value.CaselessEq("reset") || value.CaselessEq("default");
        }
        
        
        static void SendCurrentMapAppearance(Level lvl, EnvProp prop, int value) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl) continue;
                
                if (pl.HasCpeExt(CpeExt.EnvMapAspect))
                    pl.SendSetEnvMapProperty(prop, value);
                else
                    pl.SendCurrentMapAppearance();
            }
        }
        
        static void SetColour(Player p, string value, byte envType, string envTypeName, ref string target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} color for {1}%S to normal", envTypeName, p.level.name);
                target = null;
            } else {
                if (value.Length > 0 && value[0] == '#')
                    value = value.Substring(1);
                if (value.Length != 6 || !IsValidHex(value)) {
                    p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", value));
                    return;
                }
                
                Player.Message(p, "Set {0} color for {1}%S to #{2}", envTypeName, p.level.name, value);
                target = value;
            }
            SendEnvColorPackets(p, envType, value);
        }
        
        static void SendEnvColorPackets(Player p, byte envType, string value) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == p.level)
                    SendEnvColorPacket(pl, envType, value);
            }
        }
        
        static void SendEnvColorPacket(Player p, byte envType, string value) {
            if (p.HasCpeExt(CpeExt.EnvColors)) {
                try {
                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + value.ToUpper());
                    p.SendEnvColor(envType, col.R, col.G, col.B);
                } catch {
                    p.SendEnvColor(envType, -1, -1, -1);
                }
            }
        }
        
        static void SendPresetsMessage(Player p) {
            p.SendMessage("/env preset [type] -- Uses an env preset on your current map");
            p.SendMessage("Valid types: Cartoon/Midnight/Midnight2/Noir/Normal/Trippy/Watery/Sunset/Gloomy/Cloudy");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/env [variable] [value]");
            Player.Message(p, "%HVariables: fog, cloud, sky, sun, shadow, weather, level");
            Player.Message(p, "%H   horizon, border, preset, maxfog, cloudsheight");
            Player.Message(p, "%H   cloudspeed, weatherspeed, weatherfade");
            Player.Message(p, "%HUsing 'normal' as a value will reset the variable");
            Player.Message(p, "%T/env normal %H- resets all variables");
        }
    }
}
