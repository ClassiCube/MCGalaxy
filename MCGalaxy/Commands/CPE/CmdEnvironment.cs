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
using MCGalaxy.Network;

namespace MCGalaxy.Commands.CPE {
    public sealed class CmdEnvironment : Command {
        public override string name { get { return "Environment"; } }
        public override string shortcut { get { return "Env"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            message = message.ToLower();
            if (message == "preset") {
                SendPresetsMessage(p); return;
            }
            
            string[] args = message.SplitSpaces();
            if (args.Length <= 1) {
                if (!message.CaselessEq("normal")) { Help(p); return; }
                ResetEnv(p);
            } else if (args[0] == "preset") {
                SetPreset(p, args[1]);
            } else if (!Handle(p, args[0], args[1])) {
                Help(p);
            }
        }
        
        internal static bool Handle(Player p, string opt, string value) {
            Level lvl = p.level;
            // using if else instead of switch here reduces IL by about 200 bytes
            
            if (opt == "fog") {
                LevelEnv.SetColor(p, value, 2, opt, ref lvl.Config.FogColor);
            } else if (opt == "cloud" || opt == "clouds") {
                LevelEnv.SetColor(p, value, 1, opt, ref lvl.Config.CloudColor);
            } else if (opt == "sky") {
                LevelEnv.SetColor(p, value, 0, opt, ref lvl.Config.SkyColor);
            } else if (opt == "dark" || opt == "shadow") {
                LevelEnv.SetColor(p, value, 3, opt, ref lvl.Config.ShadowColor);
            } else if (opt == "sun" || opt == "light" || opt == "sunlight") {
                LevelEnv.SetColor(p, value, 4, opt, ref lvl.Config.LightColor);
            } else if (opt == "weather") {
                LevelEnv.SetWeather(p, p.level, value);
            } else if (opt == "cloudheight" || opt == "cloudsheight") {
                LevelEnv.SetShort(p, value, EnvProp.CloudsLevel,
                                  "clouds height", (short)(lvl.Height + 2), ref lvl.Config.CloudsHeight);
            } else if (opt == "waterlevel" || opt == "edgelevel" || opt == "level") {
                LevelEnv.SetShort(p, value, EnvProp.EdgeLevel,
                                  "water level", (short)(lvl.Height / 2), ref lvl.Config.EdgeLevel);
            } else if (opt == "bedrockoffset" || opt == "sidesoffset" || opt == "sideoffset") {
                LevelEnv.SetShort(p, value, EnvProp.SidesOffset,
                                  "bedrock offset", -2, ref lvl.Config.SidesOffset);
            } else if (opt == "maxfogdistance" || opt == "maxfog" || opt == "fogdistance") {
                LevelEnv.SetShort(p, value, EnvProp.MaxFog,
                                  "max fog distance", 0, ref lvl.Config.MaxFogDistance);
            } else if (opt == "cloudspeed" || opt == "cloudsspeed") {
                LevelEnv.SetFloat(p, value, EnvProp.CloudsSpeed, 256, "clouds speed",
                                  256, ref lvl.Config.CloudsSpeed, -0xFFFFFF, 0xFFFFFF);
            } else if (opt == "weatherspeed") {
                LevelEnv.SetFloat(p, value, EnvProp.WeatherSpeed, 256, "weather speed",
                                  256, ref lvl.Config.WeatherSpeed, -0xFFFFFF, 0xFFFFFF);
            } else if (opt == "weatherfade") {
                LevelEnv.SetFloat(p, value, EnvProp.WeatherFade, 128, "weather fade rate",
                                  128, ref lvl.Config.WeatherFade, 0, 255);
            } else if (opt == "horizon" || opt == "edge" || opt == "water") {
                LevelEnv.SetBlock(p, value, EnvProp.EdgeBlock,
                                  "edge block", Block.Water, ref lvl.Config.HorizonBlock);
            } else if (opt == "side" || opt == "border" || opt == "bedrock") {
                LevelEnv.SetBlock(p, value, EnvProp.SidesBlock,
                                  "sides block", Block.Bedrock, ref lvl.Config.EdgeBlock);
            } else if (opt == "expfog") {
                LevelEnv.SetBool(p, value, EnvProp.ExpFog,
                                 "exp fog", false, ref lvl.Config.ExpFog);
            } else if (opt == "skyboxhorspeed" || opt == "skyboxhor") {
                LevelEnv.SetFloat(p, value, EnvProp.SkyboxHorSpeed, 1024, "skybox horizontal speed",
                                  0, ref lvl.Config.SkyboxHorSpeed, -0xFFFFFF, 0xFFFFFF);
            }  else if (opt == "skyboxverspeed" || opt == "skyboxver") {
                LevelEnv.SetFloat(p, value, EnvProp.SkyboxVerSpeed, 1024, "skybox vertical speed",
                                  0, ref lvl.Config.SkyboxVerSpeed, -0xFFFFFF, 0xFFFFFF);
            } else {
                return false;
            }
            Level.SaveSettings(lvl);
            return true;
        }
        
        static void ResetEnv(Player p) {
            Level lvl = p.level;
            LevelConfig cfg = lvl.Config;
            SetPreset(p, "normal");
            cfg.Weather = 0;
            cfg.CloudsHeight = (short)(lvl.Height + 2);
            cfg.EdgeLevel = (short)(lvl.Height / 2);
            cfg.SidesOffset = -2;
            cfg.MaxFogDistance = 0;
            cfg.CloudsSpeed = 256;
            cfg.WeatherSpeed = 256;
            cfg.WeatherFade = 128;
            cfg.EdgeBlock = Block.Bedrock;
            cfg.HorizonBlock = Block.Water;
            cfg.ExpFog = false;
            cfg.SkyboxHorSpeed = 0;
            cfg.SkyboxVerSpeed = 0;
                   
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl) continue;
                pl.SendCurrentMapAppearance();
                
                if (pl.Supports(CpeExt.EnvWeatherType)) {
                    pl.Send(Packet.EnvWeatherType(0));
                }
            }
            Level.SaveSettings(lvl);
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
            Player.Message(p, "%T/Env preset [type] %H- Uses an env preset on your current map");
            Player.Message(p, "Valid types: Cartoon/Midnight/Midnight2/Noir/Normal/Trippy/Watery/Sunset/Gloomy/Cloudy");
            if (!Directory.Exists("presets")) return;
            
            string[] files = Directory.GetFiles("presets", "*.env");
            string all = files.Join(f => Path.GetFileNameWithoutExtension(f));
            if (all.Length > 0) Player.Message(p, "Custom preset types: " + all);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Environment [variable] [value]");
            Player.Message(p, "%HVariables: fog, cloud, sky, sun, shadow, weather, level,");
            Player.Message(p, "%H   horizon, border, preset, maxfog, cloudsheight, cloudspeed,");
            Player.Message(p, "%H   weatherspeed, weatherfade, expfog, sidesoffset,");
            Player.Message(p, "%H   skyboxhorspeed, skyboxverspeed");
            Player.Message(p, "%HUsing 'normal' as a value will reset the variable");
            Player.Message(p, "%T/Environment normal %H- resets all variables");
        }
    }
}
