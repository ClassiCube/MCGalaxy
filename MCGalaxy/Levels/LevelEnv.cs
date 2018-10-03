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
using MCGalaxy.Commands;
using MCGalaxy.Commands.Building;
using MCGalaxy.Network;
using MCGalaxy.Blocks;
using BlockID = System.UInt16;

namespace MCGalaxy {
    public static class LevelEnv {
        
        static bool IsResetString(string value) {
            return value.CaselessEq("normal") || value.CaselessEq("reset") || value.CaselessEq("default");
        }
        
        public static void SetWeather(Player p, string input, string area, ref int target) {
            int value;
            if (IsResetString(input)) {
                p.Message("Reset weather for {0} %Sto 0 (Sun)", area);
                value = -1;
            } else {
                if (int.TryParse(input, out value)) {
                } else if (input.CaselessEq("sun")) { value = 0;
                } else if (input.CaselessEq("rain")) { value = 1;
                } else if (input.CaselessEq("snow")) { value = 2;
                }
                
                if (value < 0 || value > 2) {
                    p.Message("Weather can be either sun, rain, or snow."); return;
                }
                string type = value == 0 ? "%SSun" : (value == 1 ? "&1Rain" : "&fSnow");
                p.Message("Set weather for {0} %Sto {1} ({2}%S)", area, value, type);
            }
            target = value;
        }
        
        public static void SetBlock(Player p, string input, string area, string type, ref BlockID target) {
            if (IsResetString(input)) {
                p.Message("Reset {0} for {1} %Sto normal", type, area);
                target = Block.Invalid;
            } else {
                BlockID block;
                if (!CommandParser.GetBlock(p, input, out block)) return;
                if (Block.IsPhysicsType(block)) {
                    p.Message("Cannot use physics block ids for /env."); return;
                }
                
                string name = Block.GetName(p, block);
                target = block;
                p.Message("Set {0} for {1} %Sto {2}", type, area, name);
            }
        }
        
        public static void SetShort(Player p, string input, string area, string type, ref int target) {
            if (IsResetString(input)) {
                p.Message("Reset {0} for {1} %Sto normal", type, area);
                target = -1;
            } else {
                int value = 0;
                if (!CommandParser.GetInt(p, input, type, ref value,
                                          short.MinValue, short.MaxValue)) return;
                
                target = (short)value;
                p.Message("Set {0} for {1} %Sto {2}", type, area, value);
            }
        }
        
        public static void SetFloat(Player p, string input, string area, int scale, string type,
                                    ref int target, int min, int max) {
            if (IsResetString(input)) {
                p.Message("Reset {0} for {1} %Sto normal", type, area);
                target = -1;
            } else {
                float value = 0, minF = (float)min / scale, maxF = (float)max / scale;
                if (!CommandParser.GetReal(p, input, type, ref value, minF, maxF)) return;

                target = (int)(value * scale);
                p.Message("Set {0} for {1} %Sto {2}", type, area, value.ToString("F4"));
            }
        }
        
        public static void SetColor(Player p, string input, string area,
                                    string variable, ref string target) {
            if (IsResetString(input)) {
                p.Message("Reset {0} for {1} %Sto normal", variable, area);
                target = "";
            } else {
                ColorDesc rgb = default(ColorDesc);
                if (!CommandParser.GetHex(p, input, ref rgb)) return;
                
                p.Message("Set {0} for {1} %Sto #{2}", variable, area, input);
                target = Utils.Hex(rgb.R, rgb.G, rgb.B);
            }
        }
        
        public static void SetBool(Player p, string value, string area,
                                   string variable, int defValue, ref int target) {
            if (IsResetString(value)) {
                p.Message("Reset {0} for {1} %Sto normal", variable, area);
                target = defValue;
            } else {
                bool enabled = false;
                if (!CommandParser.GetBool(p, value, ref enabled)) return;
                
                target = enabled ? 1 : 0;
                p.Message("Set {0} for {1} %Sto {2}", variable,
                               area, enabled ? "&aON" : "&cOFF");
            }
        }
    }
}