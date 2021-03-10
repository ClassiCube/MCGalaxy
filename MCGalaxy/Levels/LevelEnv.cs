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
using MCGalaxy.Commands;
using BlockID = System.UInt16;

namespace MCGalaxy {    
    public sealed class EnvOption {
        public string Name, Help;
        public EnvOptions.OptionSetter SetFunc;
        
        public EnvOption(string name, EnvOptions.OptionSetter func, string help) {
            Name = name; SetFunc = func; Help = help;
        }
    }
    
    public static class EnvOptions {
        public delegate void OptionSetter(Player p, string area, EnvConfig cfg, string value);
        
        public static List<EnvOption> Options = new List<EnvOption>() {
             new EnvOption("Weather",   SetWeather,   "&HSets the weather (sun, rain, snow)"),
             new EnvOption("SmoothFog", SetSmoothFog, "&HSets whether smoother fog is used"),
             new EnvOption("Horizon",   SetHorizon,   "&HSets the \"ocean\" block outside the map"),
             new EnvOption("Border",    SetBorder,    "&HSets the \"bedrock\" block outside the map"),
             new EnvOption("CloudsHeight", SetCloudsHeight, "&HSets the clouds height of the map"),
             new EnvOption("EdgeLevel",    SetEdgeLevel,    "&HSets the water height of the map"),
             new EnvOption("SidesOffset",  SetSidesOffset,  "&HSets offset of bedrock from water (default -2)"),
             new EnvOption("MaxFog",       SetMaxFog,       "&HSets maximum fog distance in the map (e.g. 16 for a horror map)"),
             new EnvOption("Sky",    SetSky,    "&HSets color of the sky (default 99CCFF)"),
             new EnvOption("Clouds", SetClouds, "&HSets color of the clouds (default FFFFFF)"),
             new EnvOption("Fog",    SetFog,    "&HSets color of the fog (default FFFFFF)"),
             new EnvOption("Sun",    SetSun,    "&HSets color of blocks in sunlight (default FFFFFF)"),
             new EnvOption("Shadow", SetShadow, "&HSets color of blocks in darkness (default 9B9B9B)"),      
             new EnvOption("Skybox", SetSkybox, "&HSets color of the skybox (default FFFFFF)"),             
             new EnvOption("CloudsSpeed",  SetCloudsSpeed,  "&HSets how fast clouds move (negative moves in opposite direction)"),
             new EnvOption("WeatherSpeed", SetWeatherSpeed, "&HSets how fast rain/snow falls (negative falls upwards)"),
             new EnvOption("WeatherFade",  SetWeatherFade,  "&HSets how quickly rain/snow fades out over distance"),
             new EnvOption("SkyboxHorSpeed", SetSkyboxHor,  "&HSets how many times per second skybox fully spins horizontally (e.g. 0.1 is once every 10 seconds)"),
             new EnvOption("SkyboxVerSpeed", SetSkyboxVer,  "&HSets how many times per second skybox fully spins vertically (e.g. 0.1 is once every 10 seconds)"),
        };

        public static EnvOption Find(string opt) {
            if (opt.CaselessEq("ExpFog"))  opt = "SmoothFog";
            if (opt.CaselessEq("Edge"))    opt = "Horizon";
            if (opt.CaselessEq("Side"))    opt = "Border";
            if (opt.CaselessEq("Water"))   opt = "Horizon";
            if (opt.CaselessEq("Bedrock")) opt = "Border";
            if (opt.CaselessEq("CloudHeight"))  opt = "CloudsHeight";
            if (opt.CaselessEq("Level"))        opt = "EdgeLevel";
            if (opt.CaselessEq("SideOffset"))   opt = "SidesOffset";
            if (opt.CaselessEq("Bedrockffset")) opt = "SidesOffset";
            if (opt.CaselessEq("Cloud"))    opt = "Clouds";
            if (opt.CaselessEq("Dark"))     opt = "Shadow";
            if (opt.CaselessEq("Sunlight")) opt = "Sun";                        
            if (opt.CaselessEq("CloudSpeed"))  opt = "CloudsSpeed";
            if (opt.CaselessEq("SkyboxHor"))   opt = "SkyboxHorSpeed";
            if (opt.CaselessEq("SkyboxVer"))   opt = "SkyboxVerSpeed";
  
            foreach (EnvOption option in Options) {
                if (option.Name.CaselessEq(opt)) return option;
            }
            return null;
        }        
        
        
        static void SetHorizon(Player p, string area, EnvConfig cfg, string value) {
            SetBlock(p, value, area, "edge block", ref cfg.HorizonBlock);
        }        
        static void SetBorder(Player p, string area, EnvConfig cfg, string value) {
            SetBlock(p, value, area, "sides block", ref cfg.EdgeBlock);
        }
        
        static void SetCloudsHeight(Player p, string area, EnvConfig cfg, string value) {
            SetInt(p, value, area, "clouds height", ref cfg.CloudsHeight);
        }
        static void SetEdgeLevel(Player p, string area, EnvConfig cfg, string value) {
            SetInt(p, value, area, "edge level", ref cfg.EdgeLevel);
        }
        static void SetSidesOffset(Player p, string area, EnvConfig cfg, string value) {
            SetInt(p, value, area, "sides offset", ref cfg.SidesOffset);
        }
        static void SetMaxFog(Player p, string area, EnvConfig cfg, string value) {
            SetInt(p, value, area, "max fog distance", ref cfg.MaxFogDistance);
        }
        
        static void SetSky(Player p, string area, EnvConfig cfg, string value) {
            SetColor(p, value, area, "sky color", ref cfg.SkyColor);
        }
        static void SetClouds(Player p, string area, EnvConfig cfg, string value) {
            SetColor(p, value, area, "cloud color", ref cfg.CloudColor);
        }
        static void SetFog(Player p, string area, EnvConfig cfg, string value) {
            SetColor(p, value, area, "fog color", ref cfg.FogColor);
        }
        static void SetSun(Player p, string area, EnvConfig cfg, string value) {
            SetColor(p, value, area, "sun color", ref cfg.LightColor);
        }
        static void SetShadow(Player p, string area, EnvConfig cfg, string value) {
            SetColor(p, value, area, "shadow color", ref cfg.ShadowColor);
        }
        static void SetSkybox(Player p, string area, EnvConfig cfg, string value) {
            SetColor(p, value, area, "skybox color", ref cfg.SkyboxColor);
        }
        
        static void SetCloudsSpeed(Player p, string area, EnvConfig cfg, string value) {
            SetFloat(p, value, area, 256, "clouds speed", ref cfg.CloudsSpeed, -0xFFFFFF, 0xFFFFFF);
        }       
        static void SetWeatherSpeed(Player p, string area, EnvConfig cfg, string value) {
            SetFloat(p, value, area, 256, "weather speed", ref cfg.WeatherSpeed, -0xFFFFFF, 0xFFFFFF);
        }      
        static void SetWeatherFade(Player p, string area, EnvConfig cfg, string value) {
            SetFloat(p, value, area, 128, "weather fade rate", ref cfg.WeatherFade, 0, 255);
        }        
        static void SetSkyboxHor(Player p, string area, EnvConfig cfg, string value) {
            SetFloat(p, value, area, 1024, "skybox horizontal speed", ref cfg.SkyboxHorSpeed, -0xFFFFFF, 0xFFFFFF);
        }        
        static void SetSkyboxVer(Player p, string area, EnvConfig cfg, string value) {
            SetFloat(p, value, area, 1024, "skybox vertical speed", ref cfg.SkyboxVerSpeed, -0xFFFFFF, 0xFFFFFF);
        }
        
        
        static bool IsResetString(string value) {
            return value.CaselessEq("normal") || value.CaselessEq("default") 
                || value.CaselessEq("reset")  || value.Length == 0;
        }
        
        static void SetWeather(Player p, string area, EnvConfig cfg, string value) {
            int weather;
            if (IsResetString(value)) {
                p.Message("Reset weather for {0} &Sto 0 (Sun)", area);
                weather = -1;
            } else {
                if (int.TryParse(value, out weather)) {
                } else if (value.CaselessEq("sun"))  { weather = 0;
                } else if (value.CaselessEq("rain")) { weather = 1;
                } else if (value.CaselessEq("snow")) { weather = 2;
                }
                
                if (weather < 0 || weather > 2) {
                    p.Message("Weather can be either sun, rain, or snow."); return;
                }
                string type = weather == 0 ? "&SSun" : (weather == 1 ? "&1Rain" : "&fSnow");
                p.Message("Set weather for {0} &Sto {1} ({2}&S)", area, weather, type);
            }
            cfg.Weather = weather;
        }
        
        static void SetSmoothFog(Player p, string area, EnvConfig cfg, string value) {
            if (IsResetString(value)) {
                p.Message("Reset smooth fog for {0} &Sto &cOFF", area);
                cfg.ExpFog = -1;
            } else {
                bool enabled = false;
                if (!CommandParser.GetBool(p, value, ref enabled)) return;
                
                cfg.ExpFog = enabled ? 1 : 0;
                p.Message("Set smooth fog for {0} &Sto {1}", area, enabled ? "&aON" : "&cOFF");
            }
        }
        
        static void SetBlock(Player p, string input, string area, string type, ref BlockID target) {
            if (IsResetString(input)) {
                p.Message("Reset {0} for {1} &Sto normal", type, area);
                target = Block.Invalid;
            } else {
                BlockID block;
                if (!CommandParser.GetBlock(p, input, out block)) return;
                if (Block.IsPhysicsType(block)) {
                    p.Message("&WCannot use physics block ids for &T/env"); return;
                }
                
                string name = Block.GetName(p, block);
                target = block;
                p.Message("Set {0} for {1} &Sto {2}", type, area, name);
            }
        }
        
        static void SetInt(Player p, string input, string area, string type, ref int target) {
            if (IsResetString(input)) {
                p.Message("Reset {0} for {1} &Sto normal", type, area);
                target = -1;
            } else {
                int value = 0;
                if (!CommandParser.GetInt(p, input, type, ref value,
                                          short.MinValue, short.MaxValue)) return;
                
                target = (short)value;
                p.Message("Set {0} for {1} &Sto {2}", type, area, value);
            }
        }
        
        static void SetFloat(Player p, string input, string area, int scale, string type, ref int target, int min, int max) {
            if (IsResetString(input)) {
                p.Message("Reset {0} for {1} &Sto normal", type, area);
                target = -1;
            } else {
                float value = 0, minF = (float)min / scale, maxF = (float)max / scale;
                if (!CommandParser.GetReal(p, input, type, ref value, minF, maxF)) return;

                target = (int)(value * scale);
                p.Message("Set {0} for {1} &Sto {2}", type, area, value.ToString("F4"));
            }
        }
        
        static void SetColor(Player p, string input, string area, string variable, ref string target) {
            if (IsResetString(input)) {
                p.Message("Reset {0} for {1} &Sto normal", variable, area);
                target = "";
            } else {
                ColorDesc rgb = default(ColorDesc);
                if (!CommandParser.GetHex(p, input, ref rgb)) return;
                
                p.Message("Set {0} for {1} &Sto #{2}", variable, area, input);
                target = Utils.Hex(rgb.R, rgb.G, rgb.B);
            }
        }
    }
}