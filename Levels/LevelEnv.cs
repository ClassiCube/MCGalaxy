/*
    Copyright 2015 MCGalaxy team
    
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
using MCGalaxy.Commands.Building;

namespace MCGalaxy {
    public static class LevelEnv {
        
        public static void SetWeather(Player p, Level lvl, string value) {
            byte weather = 255;
            if (IsResetString(value)) {
                Player.Message(p, "Reset weather for {0} %Sto 0 (%SSun)", lvl.name);
                weather = 0;
            } else {
                if (byte.TryParse(value, out weather)) {
                } else if (value.CaselessEq("sun")) { weather = 0;
                } else if (value.CaselessEq("rain")) { weather = 1;
                } else if (value.CaselessEq("snow")) { weather = 2;
                }
                
                if (weather > 2) {
                    Player.Message(p, "Weather can be either sun, rain, or snow."); return;
                }
                string type = weather == 0 ? "%SSun" : (weather == 1 ? "&1Rain" : "&fSnow");
                Player.Message(p, "&aSet weather for {0}&a to {1} ({2}&a)", lvl.name, weather, type);
            }
            
            lvl.Weather = weather;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == lvl && pl.HasCpeExt(CpeExt.EnvWeatherType))
                    pl.SendSetMapWeather(weather);
            }
        }
        
        public static void SetBlock(Player p, string value, EnvProp prop,
                                    string variable, byte defValue, ref int target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {1} %Sto normal", variable, p.level.name);
                target = defValue;
            } else {
                if (!CheckBlock(p, value, variable, ref target)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        public static void SetShort(Player p, string value, EnvProp prop,
                                    string variable, short defValue, ref int target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {1} %Sto normal", variable, p.level.name);
                target = defValue;
            } else {
                if (!CheckShort(p, value, variable, ref target)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        public static void SetFloat(Player p, string value, EnvProp prop, int scale, string variable,
                                    short defValue, ref int target, int min, int max) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {0} %Sto normal", variable, p.level.name);
                target = defValue;
            } else {
                if (!CheckFloat(p, value, variable, ref target, scale, min, max)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        public static void SetColor(Player p, string value, byte envType, string envTypeName, ref string target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} color for {1} %Sto normal", envTypeName, p.level.name);
                target = "";
            } else {
			    if (!Utils.CheckHex(p, ref value)) return;
                Player.Message(p, "Set {0} color for {1} %Sto #{2}", envTypeName, p.level.name, value);
                target = value;
            }
            SendEnvColorPackets(p, envType, value);
        }
        
        static bool CheckBlock(Player p, string value, string variable, ref int modify) {
            byte extBlock = 0;
            int block = DrawCmd.GetBlock(p, value, out extBlock, false);
            if (block == -1 || block == Block.Zero) return false;
            if (block >= Block.CpeCount && block != Block.custom_block) {
                Player.Message(p, "Cannot use physics block ids for /env."); return false;
            }
            
            if (block == Block.shrub || block == Block.yellowflower || block == Block.redflower ||
                block == Block.mushroom || block == Block.redmushroom || block == Block.rope || block == Block.fire) {
                Player.Message(p, "Env: Cannot use {0} for {1}.", block, variable);
            } else {
            	modify = block == Block.custom_block ? extBlock : (byte)block;
                Player.Message(p, "Set {0} for {1} %Sto {2}", variable, p.level.name, modify);
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
                Player.Message(p, "Set {0} for {1} %Sto {2}", variable, p.level.name, value);
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
                Player.Message(p, "Set {0} for {1} %Sto {2}", variable, p.level.name, value.ToString("F2"));
                return true;
            }
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
        
        internal static void SendEnvColorPackets(Player p, byte envType, string value) {
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
    }
}