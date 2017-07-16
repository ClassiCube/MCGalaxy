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
using MCGalaxy.Commands.Building;

namespace MCGalaxy.Commands.CPE {
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
            
            string[] args = message.SplitSpaces();
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
                
                if (args[0] == "preset") {
                    SetPreset(p, args[1]);
                } else if (!Handle(p, args[0], args[1])) {
                    Help(p);
                }
            }
        }
        
        internal static bool Handle(Player p, string variable, string value) {
            Level lvl = p.level;
            switch (variable) {
                case "fog":
                    LevelEnv.SetColor(p, value, 2, "fog", ref lvl.Config.FogColor); break;
                case "cloud":
                case "clouds":
                    LevelEnv.SetColor(p, value, 1, "cloud", ref lvl.Config.CloudColor); break;
                case "sky":
                    LevelEnv.SetColor(p, value, 0, "sky", ref lvl.Config.SkyColor); break;
                case "dark":
                case "shadow":
                    LevelEnv.SetColor(p, value, 3, "shadow", ref lvl.Config.ShadowColor); break;
                case "sun":
                case "light":
                case "sunlight":
                    LevelEnv.SetColor(p, value, 4, "sunlight", ref lvl.Config.LightColor); break;
                case "weather":
                    LevelEnv.SetWeather(p, p.level, value); break;
                case "cloudsheight":
                case "cloudheight":
                    LevelEnv.SetShort(p, value, EnvProp.CloudsLevel,
                             "clouds height", (short)(lvl.Height + 2), ref lvl.Config.CloudsHeight); break;
                case "waterlevel":
                case "edgelevel":
                case "level":
                    LevelEnv.SetShort(p, value, EnvProp.EdgeLevel,
                             "water level", (short)(lvl.Height / 2), ref lvl.Config.EdgeLevel); break;
                case "bedrockoffset":
                case "sidesoffset":
                case "sideoffset":
                    LevelEnv.SetShort(p, value, EnvProp.SidesOffset,
                             "bedrock offset", -2, ref lvl.Config.SidesOffset); break;
                case "maxfogdistance":
                case "maxfog":
                case "fogdistance":
                case "fogdist":
                    LevelEnv.SetShort(p, value, EnvProp.MaxFog,
                             "max fog distance", 0, ref lvl.Config.MaxFogDistance); break;
                case "cloudspeed":
                case "cloudsspeed":
                    LevelEnv.SetFloat(p, value, EnvProp.CloudsSpeed, 256, "clouds speed",
                             256, ref lvl.Config.CloudsSpeed, -32767, 32767); break;
                case "weatherspeed":
                    LevelEnv.SetFloat(p, value, EnvProp.WeatherSpeed, 256, "weather speed",
                             256, ref lvl.Config.WeatherSpeed, -32767, 32767); break;
                case "weatherfade":
                    LevelEnv.SetFloat(p, value, EnvProp.WeatherFade, 128, "weather fade rate",
                             128, ref lvl.Config.WeatherFade, 0, 255); break;
                case "horizon":
                case "edge":
                case "water":
                    LevelEnv.SetBlock(p, value, EnvProp.EdgeBlock,
                             "edge block", Block.Water, ref lvl.Config.HorizonBlock); break;
                case "side":
                case "border":
                case "bedrock":
                    LevelEnv.SetBlock(p, value, EnvProp.SidesBlock,
                             "sides block", Block.Bedrock, ref lvl.Config.EdgeBlock); break;
                case "expfog":
                    LevelEnv.SetBool(p, value, EnvProp.ExpFog,
                                     "exp fog", false, ref lvl.Config.ExpFog); break;
                default:
                    return false;
            }
            Level.SaveSettings(lvl);
            return true;
        }
        
        static void ResetEnv(Player p) {
            Level lvl = p.level;
            SetPreset(p, "normal");
            LevelEnv.SetWeather(p, lvl, "normal");
            
            LevelEnv.SetBlock(p, "normal", EnvProp.EdgeBlock,
                     "edge block", Block.Water, ref lvl.Config.HorizonBlock);
            LevelEnv.SetBlock(p, "normal", EnvProp.SidesBlock,
                     "sides block", Block.Bedrock, ref lvl.Config.EdgeBlock);
            
            LevelEnv.SetShort(p, "normal", EnvProp.EdgeLevel,
                     "water level", (short)(lvl.Height / 2), ref lvl.Config.EdgeLevel);
            LevelEnv.SetShort(p, "normal", EnvProp.SidesOffset,
                     "bedrock offset", -2, ref lvl.Config.SidesOffset);
            LevelEnv.SetShort(p, "normal", EnvProp.CloudsLevel,
                     "clouds height", (short)(lvl.Height + 2), ref lvl.Config.CloudsHeight);
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
            } else if (File.Exists("presets/" + value.ToLower() + ".env")) {
                string text = File.ReadAllText("presets/" + value.ToLower() + ".env");
                string[] parts = text.SplitSpaces();
                preset = new EnvPreset(parts[0], parts[1], parts[2], parts[3], parts[4]);
            }            
            if (preset == null) { SendPresetsMessage(p); return false; }
            
            LevelEnv.UpdateEnvColor(p, 0, preset.Sky);
            p.level.Config.SkyColor = preset.Sky;
            LevelEnv.UpdateEnvColor(p, 1, preset.Clouds);
            p.level.Config.CloudColor = preset.Clouds;
            LevelEnv.UpdateEnvColor(p, 2, preset.Fog);
            p.level.Config.FogColor = preset.Fog;
            LevelEnv.UpdateEnvColor(p, 3, preset.Shadow);
            p.level.Config.ShadowColor = preset.Shadow;
            LevelEnv.UpdateEnvColor(p, 4, preset.Sun);
            p.level.Config.LightColor = preset.Sun;
            
            Level.SaveSettings(p.level);
            return true;
        }
        
        class EnvPreset {
            public string Fog, Sky, Clouds, Sun, Shadow;
            
            public EnvPreset( string fog, string sky, string clouds,
                             string sun, string shadow ) {
                Fog = fog; Sky = sky; Clouds = clouds;
                Sun = sun; Shadow = shadow;
            }
        }

        static void SendPresetsMessage(Player p) {
            Player.Message(p, "/env preset [type] -- Uses an env preset on your current map");
            Player.Message(p, "Valid types: Cartoon/Midnight/Midnight2/Noir/Normal/Trippy/Watery/Sunset/Gloomy/Cloudy");           
            if (!Directory.Exists("presets")) return;
            
            string[] files = Directory.GetFiles("presets", "*.env");
            string all = files.Join(f => Path.GetFileNameWithoutExtension(f));
            if (all != "")
                Player.Message(p, "Custom preset types: " + all);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/env [variable] [value]");
            Player.Message(p, "%HVariables: fog, cloud, sky, sun, shadow, weather, level");
            Player.Message(p, "%H   horizon, border, preset, maxfog, cloudsheight");
            Player.Message(p, "%H   cloudspeed, weatherspeed, weatherfade, expfog, sidesoffset");
            Player.Message(p, "%HUsing 'normal' as a value will reset the variable");
            Player.Message(p, "%T/env normal %H- resets all variables");
        }
    }
}
