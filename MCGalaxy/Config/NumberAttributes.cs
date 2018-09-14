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
    
    public abstract class ConfigIntegerAttribute : ConfigAttribute {        
        public ConfigIntegerAttribute(string name, string section) 
            : base(name, section) { }
         
        // separate function to avoid boxing in derived classes
        protected int ParseInteger(string raw, int def, int min, int max) {
            int value;
            if (!int.TryParse(raw, out value)) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid integer, using default of {1}", Name, def);
                value = def;
            }
            
            if (value < min) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too small an integer, using {1}", Name, min);
                value = min;
            }
            if (value > max) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too big an integer, using {1}", Name, max);
                value = max;
            }
            return value;
        }
    }
    
    public sealed class ConfigIntAttribute : ConfigIntegerAttribute {
        int defValue, minValue, maxValue;
        
        public ConfigIntAttribute()
            : this(null, null, 0, int.MinValue, int.MaxValue) { }
        public ConfigIntAttribute(string name, string section, int def,
                                  int min = int.MinValue, int max = int.MaxValue)
            : base(name, section) { defValue = def; minValue = min; maxValue = max; }
        
        public override object Parse(string value) {
            return ParseInteger(value, defValue, minValue, maxValue);
        }
    }
    
    // Hacky workaround for old ExponentialFog attribute
    sealed class ConfigBoolIntAttribute : ConfigIntegerAttribute {
        public ConfigBoolIntAttribute(string name, string section)
            : base(name, section) { }
        
        public override object Parse(string raw) {
            bool value;
            if (bool.TryParse(raw, out value)) return value ? 1 : 0;
            return ParseInteger(raw, 0, -1, 1);
        }
    }
    
    public sealed class ConfigBlockAttribute : ConfigIntegerAttribute {
        BlockID defBlock;
        public ConfigBlockAttribute() : this(null, null, Block.Air) { }
        public ConfigBlockAttribute(string name, string section, BlockID def)
            : base(name, section) { defBlock = def; }
        
        public override object Parse(string raw) {
            BlockID block = (BlockID)ParseInteger(raw, defBlock, 0, Block.ExtendedCount - 1);
            if (block == Block.Invalid) return Block.Invalid;
            return Block.MapOldRaw(block);
        }
    }
    
    public class ConfigByteAttribute : ConfigIntegerAttribute {        
        public ConfigByteAttribute() : this(null, null) { }
        public ConfigByteAttribute(string name, string section) : base(name, section) { }
        
        public override object Parse(string raw) { 
            return (byte)ParseInteger(raw, 0, 0, byte.MaxValue); 
        }
    }
    
    public class ConfigUShortAttribute : ConfigIntegerAttribute {
        public ConfigUShortAttribute() : this(null, null) { }
        public ConfigUShortAttribute(string name, string section) : base(name, section) { }
        
        public override object Parse(string raw) { 
            return (ushort)ParseInteger(raw, 0, 0, ushort.MaxValue);
        }
    }
    
    public abstract class ConfigRealAttribute : ConfigAttribute {
        public ConfigRealAttribute(string name, string section) 
            : base(name, section) { }
        
        protected double ParseReal(string raw, double def, double min, double max) {
            double value;
            if (!Utils.TryParseDouble(raw, out value)) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid number, using default of {1}", Name, def);
                value = def;
            }
            
            if (value < min) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too small a number, using {1}", Name, min);
                value = min;
            }
            if (value > max) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too big a number, using {1}", Name, max);
                value = max;
            }
            return value;
        }
    }
    
    public class ConfigFloatAttribute : ConfigRealAttribute {
        float defValue, minValue, maxValue;
        
        public ConfigFloatAttribute()
            : this(null, null, 0, float.NegativeInfinity, float.PositiveInfinity) { }
        public ConfigFloatAttribute(string name, string section, float def,
                                    float min = float.NegativeInfinity, float max = float.PositiveInfinity)
            : base(name, section) { defValue = def; minValue = min; maxValue = max; }
        
        public override object Parse(string raw) {
            return (float)ParseReal(raw, defValue, minValue, maxValue);
        }
    }
    
    public class ConfigTimespanAttribute : ConfigRealAttribute {
        bool mins; int def;
        public ConfigTimespanAttribute(string name, string section, int def, bool mins)
            : base(name, section) { this.def = def; this.mins = mins; }
        
        public override object Parse(string raw) {
            double value = ParseReal(raw, def, 0, int.MaxValue);
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
