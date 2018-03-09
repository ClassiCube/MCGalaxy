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
            if (message.CaselessEq("preset")) {
                SendPresetsMessage(p); return;
            }
            
            if (!LevelInfo.ValidateAction(p, p.level.name, "set env settings of this level")) return;
            string[] args = message.SplitSpaces();
            string opt = args[0];
            
            if (args.Length <= 1) {
                if (!opt.CaselessEq("normal")) { Help(p); return; }
                ResetEnv(p); return;
            }
            if (opt.CaselessEq("preset")) {
                SetPreset(p, args[1]); return;
            }
            
            Level lvl = p.level;
            Predicate<Player> selector = pl => pl.level == lvl;
            if (!Handle(p, selector, opt, args[1], p.level.Config, p.level.ColoredName)) {
                Help(p);
            }
        }
        
        internal static bool Handle(Player p, Predicate<Player> selector, string opt, string value, 
                                    AreaConfig cfg, string area) {
            Level lvl = p.level;
            // using if else instead of switch here reduces IL by about 200 bytes
            if (opt == "sky") {
                LevelEnv.SetColor(p, value, area, "sky color", ref cfg.SkyColor);
                LevelEnv.UpdateColor(selector, 0, cfg.SkyColor);
            } else if (opt == "cloud" || opt == "clouds") {
                LevelEnv.SetColor(p, value, area, "cloud color", ref cfg.CloudColor);
                LevelEnv.UpdateColor(selector, 1, cfg.CloudColor);
            } else if (opt == "fog") {
                LevelEnv.SetColor(p, value, area, "fog color", ref cfg.FogColor);
                LevelEnv.UpdateColor(selector, 2, cfg.FogColor);
            } else if (opt == "dark" || opt == "shadow") {
                LevelEnv.SetColor(p, value, area, "shadow color", ref cfg.ShadowColor);
                LevelEnv.UpdateColor(selector, 3, cfg.ShadowColor);
            } else if (opt == "sun" || opt == "light" || opt == "sunlight") {
                LevelEnv.SetColor(p, value, area, "sun color", ref cfg.LightColor);
                LevelEnv.UpdateColor(selector, 4, cfg.LightColor);
            } else if (opt == "weather") {
                LevelEnv.SetWeather(p, value, area, ref cfg.Weather);
                LevelEnv.UpdateWeather(selector, (byte)cfg.Weather);
            } else if (opt == "cloudheight" || opt == "cloudsheight") {
                LevelEnv.SetShort(p, value, area, "clouds height",
                                  lvl.Height + 2, ref cfg.CloudsHeight);
                LevelEnv.UpdateAppearance(selector, EnvProp.CloudsLevel, cfg.CloudsHeight);
            } else if (opt == "waterlevel" || opt == "edgelevel" || opt == "level") {
                LevelEnv.SetShort(p, value, area, "water level",
                                  lvl.Height / 2, ref cfg.EdgeLevel);
                LevelEnv.UpdateAppearance(selector, EnvProp.EdgeLevel, cfg.EdgeLevel);
            } else if (opt == "bedrockoffset" || opt == "sidesoffset" || opt == "sideoffset") {
                LevelEnv.SetShort(p, value, area, "bedrock offset",
                                  -2, ref cfg.SidesOffset);
                LevelEnv.UpdateAppearance(selector, EnvProp.SidesOffset, cfg.SidesOffset);
            } else if (opt == "maxfogdistance" || opt == "maxfog" || opt == "fogdistance") {
                LevelEnv.SetShort(p, value, area, "max fog distance",
                                  0, ref cfg.MaxFogDistance);
                LevelEnv.UpdateAppearance(selector, EnvProp.MaxFog, cfg.MaxFogDistance);
            } else if (opt == "cloudspeed" || opt == "cloudsspeed") {
                LevelEnv.SetFloat(p, value,area, 256, "clouds speed",
                                  256, ref cfg.CloudsSpeed, -0xFFFFFF, 0xFFFFFF);
                LevelEnv.UpdateAppearance(selector, EnvProp.CloudsSpeed, cfg.CloudsSpeed);
            } else if (opt == "weatherspeed") {
                LevelEnv.SetFloat(p, value, area, 256, "weather speed",
                                  256, ref cfg.WeatherSpeed, -0xFFFFFF, 0xFFFFFF);
                LevelEnv.UpdateAppearance(selector, EnvProp.WeatherSpeed, cfg.WeatherSpeed);
            } else if (opt == "weatherfade") {
                LevelEnv.SetFloat(p, value, area, 128, "weather fade rate",
                                  128, ref cfg.WeatherFade, 0, 255);
                LevelEnv.UpdateAppearance(selector, EnvProp.WeatherFade, cfg.WeatherFade);
            } else if (opt == "horizon" || opt == "edge" || opt == "water") {
                LevelEnv.SetBlock(p, value, area, "edge block", Block.Water, ref cfg.HorizonBlock);
                LevelEnv.UpdateAppearance(selector, EnvProp.EdgeBlock, cfg.HorizonBlock);
            } else if (opt == "side" || opt == "border" || opt == "bedrock") {
                LevelEnv.SetBlock(p, value, area, "sides block", Block.Bedrock, ref cfg.EdgeBlock);
                LevelEnv.UpdateAppearance(selector, EnvProp.SidesBlock, cfg.EdgeBlock);
            } else if (opt == "expfog") {
                LevelEnv.SetBool(p, value, area, "exp fog", 0, ref cfg.ExpFog);
                LevelEnv.UpdateAppearance(selector, EnvProp.ExpFog, cfg.ExpFog);
            } else if (opt == "skyboxhorspeed" || opt == "skyboxhor") {
                LevelEnv.SetFloat(p, value, area, 1024, "skybox horizontal speed",
                                  0, ref cfg.SkyboxHorSpeed, -0xFFFFFF, 0xFFFFFF);
                LevelEnv.UpdateAppearance(selector, EnvProp.SkyboxHorSpeed, cfg.SkyboxHorSpeed);
            }  else if (opt == "skyboxverspeed" || opt == "skyboxver") {
                LevelEnv.SetFloat(p, value, area, 1024, "skybox vertical speed",
                                  0, ref cfg.SkyboxVerSpeed, -0xFFFFFF, 0xFFFFFF);
                LevelEnv.UpdateAppearance(selector, EnvProp.SkyboxVerSpeed, cfg.SkyboxVerSpeed);
            } else {
                return false;
            }
            Level.SaveSettings(lvl);
            return true;
        }
        
        static void ResetEnv(Player p) {
            Level lvl = p.level;
            LevelConfig cfg = lvl.Config;
            cfg.Reset(lvl.Height);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl) continue;
                pl.SendCurrentTextures();
                pl.OnChangedZone();
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
            
            Level lvl = p.level;
            Predicate<Player> selector = pl => pl.level == lvl;
            LevelEnv.UpdateColor(selector, 0, preset.Sky);
            lvl.Config.SkyColor = preset.Sky;
            LevelEnv.UpdateColor(selector, 1, preset.Clouds);
            lvl.Config.CloudColor = preset.Clouds;
            LevelEnv.UpdateColor(selector, 2, preset.Fog);
            lvl.Config.FogColor = preset.Fog;
            LevelEnv.UpdateColor(selector, 3, preset.Shadow);
            lvl.Config.ShadowColor = preset.Shadow;
            LevelEnv.UpdateColor(selector, 4, preset.Sun);
            lvl.Config.LightColor = preset.Sun;
            
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
