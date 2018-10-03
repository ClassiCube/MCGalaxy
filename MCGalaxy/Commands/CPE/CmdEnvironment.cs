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
    public sealed class CmdEnvironment : Command2 {
        public override string name { get { return "Environment"; } }
        public override string shortcut { get { return "Env"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.CaselessEq("preset")) {
                SendPresetsMessage(p); return;
            }
            message = message.ToLower();
            Level lvl = p.level;
            
            if (!LevelInfo.Check(p, data.Rank, lvl, "set env settings of this level")) return;
            string[] args = message.SplitSpaces();
            string opt = args[0];
            
            if (args.Length <= 1) {
                if (!opt.CaselessEq("normal")) { Help(p); return; }
                ResetEnv(p, lvl); return;
            }
            
            if (opt.CaselessEq("preset")) {
                SetPreset(p, lvl, args[1]); return;
            }
            
            if (!Handle(p, lvl, opt, args[1], lvl.Config, lvl.ColoredName)) { Help(p); }
        }
        
        internal static bool Handle(Player p, Level lvl, string opt, string value, AreaConfig cfg, string area) {
            // using if else instead of switch here reduces IL by about 200 bytes
            if (opt == "sky") {
                LevelEnv.SetColor(p, value, area, "sky color", ref cfg.SkyColor);
            } else if (opt == "cloud" || opt == "clouds") {
                LevelEnv.SetColor(p, value, area, "cloud color", ref cfg.CloudColor);
            } else if (opt == "fog") {
                LevelEnv.SetColor(p, value, area, "fog color", ref cfg.FogColor);
            } else if (opt == "dark" || opt == "shadow") {
                LevelEnv.SetColor(p, value, area, "shadow color", ref cfg.ShadowColor);
            } else if (opt == "sun" || opt == "light" || opt == "sunlight") {
                LevelEnv.SetColor(p, value, area, "sun color", ref cfg.LightColor);
            } else if (opt == "weather") {
                LevelEnv.SetWeather(p, value, area, ref cfg.Weather);
            } else if (opt == "cloudheight" || opt == "cloudsheight") {
                LevelEnv.SetShort(p, value, area, "clouds height", ref cfg.CloudsHeight);
            } else if (opt == "waterlevel" || opt == "edgelevel" || opt == "level") {
                LevelEnv.SetShort(p, value, area, "water level", ref cfg.EdgeLevel);
            } else if (opt == "bedrockoffset" || opt == "sidesoffset" || opt == "sideoffset") {
                LevelEnv.SetShort(p, value, area, "bedrock offset", ref cfg.SidesOffset);
            } else if (opt == "maxfogdistance" || opt == "maxfog" || opt == "fogdistance") {
                LevelEnv.SetShort(p, value, area, "max fog distance", ref cfg.MaxFogDistance);
            } else if (opt == "cloudspeed" || opt == "cloudsspeed") {
                LevelEnv.SetFloat(p, value, area, 256, "clouds speed",
                                  ref cfg.CloudsSpeed, -0xFFFFFF, 0xFFFFFF);
            } else if (opt == "weatherspeed") {
                LevelEnv.SetFloat(p, value, area, 256, "weather speed",
                                  ref cfg.WeatherSpeed, -0xFFFFFF, 0xFFFFFF);
            } else if (opt == "weatherfade") {
                LevelEnv.SetFloat(p, value, area, 128, "weather fade rate",
                                  ref cfg.WeatherFade, 0, 255);
            } else if (opt == "horizon" || opt == "edge" || opt == "water") {
                LevelEnv.SetBlock(p, value, area, "edge block", ref cfg.HorizonBlock);
            } else if (opt == "side" || opt == "border" || opt == "bedrock") {
                LevelEnv.SetBlock(p, value, area, "sides block", ref cfg.EdgeBlock);
            } else if (opt == "expfog") {
                LevelEnv.SetBool(p, value, area, "exp fog", 0, ref cfg.ExpFog);
            } else if (opt == "skyboxhorspeed" || opt == "skyboxhor") {
                LevelEnv.SetFloat(p, value, area, 1024, "skybox horizontal speed",
                                  ref cfg.SkyboxHorSpeed, -0xFFFFFF, 0xFFFFFF);
            }  else if (opt == "skyboxverspeed" || opt == "skyboxver") {
                LevelEnv.SetFloat(p, value, area, 1024, "skybox vertical speed",
                                  ref cfg.SkyboxVerSpeed, -0xFFFFFF, 0xFFFFFF);
            } else {
                return false;
            }
            
            SendEnv(lvl);
            Level.SaveSettings(lvl);
            return true;
        }
        
        static void SendEnv(Level lvl) {
        	Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl) continue;
                pl.SendCurrentEnv();
            }
        }
        
        static void ResetEnv(Player p, Level lvl) {
            LevelConfig cfg = lvl.Config;
            cfg.ResetAllEnv();
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl) continue;
                pl.SendCurrentTextures();
                pl.SendCurrentEnv();
            }
            Level.SaveSettings(lvl);
        }
        
        static bool SetPreset(Player p, Level lvl, string value) {
            EnvPreset preset = null; // fog, sky, clouds, sun, shadow
            if (value.CaselessEq("cartoon")) {
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
            } else if (value.CaselessEq("midnight")) {
                preset = new EnvPreset("131947", "070A23", "1E223A", "181828", "0F0F19");
            } else if (File.Exists("presets/" + value.ToLower() + ".env")) {
                string text = File.ReadAllText("presets/" + value.ToLower() + ".env");
                string[] parts = text.SplitSpaces();
                preset = new EnvPreset(parts[0], parts[1], parts[2], parts[3], parts[4]);
            }
            
            if (preset == null) { SendPresetsMessage(p); return false; }
            LevelConfig cfg = lvl.Config;
            
            cfg.SkyColor    = preset.Sky;   
            cfg.CloudColor  = preset.Clouds;
            cfg.FogColor    = preset.Fog;   
            cfg.ShadowColor = preset.Shadow;
            cfg.LightColor  = preset.Sun;   
            
            SendEnv(lvl);
            Level.SaveSettings(lvl);
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
            p.Message("%T/Env preset [type] %H- Uses an env preset on your current map");
            p.Message("Valid types: Cartoon/Midnight/Noir/Normal/Trippy/Watery/Sunset/Gloomy/Cloudy");
            if (!Directory.Exists("presets")) return;
            
            string[] files = Directory.GetFiles("presets", "*.env");
            string all = files.Join(f => Path.GetFileNameWithoutExtension(f));
            if (all.Length > 0) p.Message("Custom preset types: " + all);
        }
        
        public override void Help(Player p) {
            p.Message("%T/Environment [variable] [value]");
            p.Message("%HSee %T/Help env variables %Hfor list of variables");
            p.Message("%HUsing 'normal' for [value] will reset the variable");
            p.Message("%T/Environment normal %H- resets all variables");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("variable") || message.CaselessEq("variables")) {
                p.Message("%HVariables: fog, cloud, sky, sun, shadow, weather, level,");
                p.Message("%H   horizon, border, preset, maxfog, cloudsheight, cloudspeed,");
                p.Message("%H   weatherspeed, weatherfade, expfog, sidesoffset,");
                p.Message("%H   skyboxhorspeed, skyboxverspeed");
            } else {
                base.Help(p, message);
            }
        }
    }
}
