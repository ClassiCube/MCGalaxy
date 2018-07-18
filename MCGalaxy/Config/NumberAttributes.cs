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
using BlockID = System.UInt16;

namespace MCGalaxy.Config {
    
    public class ConfigIntAttribute : ConfigAttribute {
        int defValue, minValue, maxValue;
        
        public ConfigIntAttribute(string name, string section, int def,
                                  int min = int.MinValue, int max = int.MaxValue)
            : base(name, section) { defValue = def; minValue = min; maxValue = max; }
        
        public override object Parse(string raw) {
            int value;
            if (!int.TryParse(raw, out value)) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid integer, using default of {1}", Name, defValue);
                value = defValue;
            }
            
            if (value < minValue) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too small an integer, using {1}", Name, minValue);
                value = minValue;
            }
            if (value > maxValue) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too big an integer, using {1}", Name, maxValue);
                value = maxValue;
            }
            return value;
        }
    }
    
    // Hacky workaround for old ExponentialFog attribute
    public sealed class ConfigBoolIntAttribute : ConfigIntAttribute {
        
        public ConfigBoolIntAttribute(string name, string section, int defValue)
            : base(name, section, defValue, -1, 1) {
        }
        
        public override object Parse(string raw) {
            bool value;
            if (bool.TryParse(raw, out value)) { return value ? 1 : 0; }
            return base.Parse(raw);
        }
    }
    
    public class ConfigBlockAttribute : ConfigIntAttribute {
        
        public ConfigBlockAttribute(string name, string section, int def)
            : base(name, section, def, 0, Block.ExtendedCount - 1) {
        }
        
        public override object Parse(string raw) {
            int value = (int)base.Parse(raw);
            
            // Can't directly unbox object to block ID - must unbox to int, then cast to block ID
            if (value == Block.Invalid) return Block.Invalid;
            return Block.MapOldRaw((BlockID)value);
        }
    }
    
    public class ConfigRealAttribute : ConfigAttribute {
        float defValue, minValue, maxValue;
        
        public ConfigRealAttribute(string name, string section, float def,
                                   float min = float.NegativeInfinity, float max = float.PositiveInfinity)
            : base(name, section) { defValue = def; minValue = min; maxValue = max; }
        
        public override object Parse(string raw) {
            float value;
            if (!Utils.TryParseDecimal(raw, out value)) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid number, using default of {1}", Name, defValue);
                value = defValue;
            }
            
            if (value < minValue) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too small a number, using {1}", Name, minValue);
                value = minValue;
            }
            if (value > maxValue) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too big a number, using {1}", Name, maxValue);
                value = maxValue;
            }
            return value;
        }
    }
    
    public class ConfigTimespanAttribute : ConfigRealAttribute {
        bool mins;
        public ConfigTimespanAttribute(string name, string section, float def, bool mins)
            : base(name, section, def, 0) { this.mins = mins; }
        
        public override object Parse(string raw) {
            float value = (float)base.Parse(raw);
            if (mins) {
                return TimeSpan.FromMinutes(value);
            } else {
                return TimeSpan.FromSeconds(value);
            }
        }
        
        public override string Serialise(object value) {
            TimeSpan span = (TimeSpan)value;
            double time = mins ? span.TotalMinutes : span.TotalSeconds;
            return time.ToString();
        }
    }
}
