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
                Player.Message(p, "Reset weather for {0} %Sto 0 (%SSun)", lvl.ColoredName);
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
                Player.Message(p, "Set weather for {0} %Sto {1} ({2}%S)", lvl.ColoredName, weather, type);
            }
            
            lvl.Weather = weather;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == lvl && pl.HasCpeExt(CpeExt.EnvWeatherType))
                    pl.Send(Packet.EnvWeatherType(weather));
            }
        }
        
        public static void SetBlock(Player p, string value, EnvProp prop,
                                    string variable, byte defValue, ref int target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {1} %Sto normal", variable, p.level.ColoredName);
                target = defValue;
            } else {
                if (!CheckBlock(p, value, variable, ref target)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        public static void SetShort(Player p, string value, EnvProp prop,
                                    string variable, short defValue, ref int target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {1} %Sto normal", variable, p.level.ColoredName);
                target = defValue;
            } else {
                if (!CheckShort(p, value, variable, ref target)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        public static void SetFloat(Player p, string value, EnvProp prop, int scale, string variable,
                                    short defValue, ref int target, int min, int max) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} for {0} %Sto normal", variable, p.level.ColoredName);
                target = defValue;
            } else {
                if (!CheckFloat(p, value, variable, ref target, scale, min, max)) return;
            }
            SendCurrentMapAppearance(p.level, prop, target);
        }
        
        public static void SetColor(Player p, string value, byte envType, string envTypeName, ref string target) {
            if (IsResetString(value)) {
                Player.Message(p, "Reset {0} color for {1} %Sto normal", envTypeName, p.level.ColoredName);
                target = "";
            } else {
                if (!Utils.CheckHex(p, ref value)) return;
                Player.Message(p, "Set {0} color for {1} %Sto #{2}", envTypeName, p.level.ColoredName, value);
                target = value;
            }
            UpdateEnvColor(p, envType, value);
        }
        
        static bool CheckBlock(Player p, string value, string variable, ref int modify) {
            byte extBlock = 0;
            byte block = (byte)DrawCmd.GetBlock(p, value, out extBlock);
            if (block == Block.Invalid) return false;
            if (block >= Block.CpeCount && block != Block.custom_block) {
                Player.Message(p, "Cannot use physics block ids for /env."); return false;
            }
            
            string name = p.level.BlockName(block, extBlock);
            if (block == Block.shrub || block == Block.yellowflower || block == Block.redflower ||
                block == Block.mushroom || block == Block.redmushroom || block == Block.rope || block == Block.fire) {
                Player.Message(p, "Env: Cannot use {0} for {1}.", name, variable);
            } else {
                modify = block == Block.custom_block ? extBlock : block;
                Player.Message(p, "Set {0} for {1} %Sto {2}", variable, p.level.ColoredName, name);
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
                Player.Message(p, "Set {0} for {1} %Sto {2}", variable, p.level.ColoredName, value);
                return true;
            }
        }
        
        static bool CheckFloat(Player p, string raw, string variable,
                               ref int modify, int scale, float min, float max) {
            float value;
            min /= scale; max /= scale;
            
            if (!Utils.TryParseDecimal(raw, out value)) {
                Player.Message(p, "Env: \"{0}\" is not a valid decimal.", value);
                return false;
            } else if (value < min || value > max) {
                Player.Message(p, "Env: \"{0}\" must be between {1} and {2}.",
                               value, min.ToString("F2"), max.ToString("F2"));
                return false;
            } else {
                modify = (int)(value * scale);
                Player.Message(p, "Set {0} for {1} %Sto {2}", variable, p.level.ColoredName, value.ToString("F2"));
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
                
                if (pl.HasCpeExt(CpeExt.EnvMapAspect)) {
                    pl.Send(Packet.EnvMapProperty(prop, value));
                } else {
                    pl.SendCurrentMapAppearance();
                }
            }
        }
        
        internal static void UpdateEnvColor(Player p, byte type, string hex) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != p.level || !pl.HasCpeExt(CpeExt.EnvColors)) continue;
                pl.SendEnvColor(type, hex);
            }
        }
    }
}