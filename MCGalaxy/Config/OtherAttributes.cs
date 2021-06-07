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
using MCGalaxy.Maths;

namespace MCGalaxy.Config {
    
    public sealed class ConfigBoolAttribute : ConfigAttribute {
        bool defValue;
        
        public ConfigBoolAttribute() : this(null, null, false) { }
        public ConfigBoolAttribute(string name, string section, bool def)
            : base(name, section) { defValue = def; }
        
        public override object Parse(string raw) {
            bool value;
            if (!bool.TryParse(raw, out value)) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" has invalid boolean '{2}', using default of {1}", Name, defValue, raw);
                return defValue;
            }
            return value;
        }
        
        public override string Serialise(object value) {
            bool boolValue = (bool)value;
            return boolValue ? "true" : "false";
        }
    }
    
    public sealed class ConfigPermAttribute : ConfigAttribute {
        LevelPermission defPerm;
        
        public ConfigPermAttribute(string name, string section, LevelPermission def)
            : base(name, section) { defPerm = def; }
        
        public override object Parse(string raw) {
            LevelPermission perm = Group.ParsePermOrName(raw, LevelPermission.Null);
            if (perm == LevelPermission.Null) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" has invalid permission '{2}', using default of {1}", Name, defPerm, raw);
                perm = defPerm;
            }
            
            if (perm < LevelPermission.Banned) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" cannot be below banned rank.", Name);
                perm = LevelPermission.Banned;
            }
            if (perm > LevelPermission.Nobody) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" cannot be above nobody rank.", Name);
                perm = LevelPermission.Nobody;
            }
            return perm;
        }
        
        public override string Serialise(object value) {
            LevelPermission perm = (LevelPermission)value;
            return ((sbyte)perm).ToString();
        }
    }
    
    public sealed class ConfigEnumAttribute : ConfigAttribute {
        object defValue;
        Type enumType;
        
        public ConfigEnumAttribute(string name, string section, object def, Type type)
            : base(name, section) { defValue = def; enumType = type; }
        
        public override object Parse(string raw) {
            object value;
            try {
                value = Enum.Parse(enumType, raw, true);
                if (!Enum.IsDefined(enumType, value)) throw new ArgumentException("value not member of enumeration");
            } catch {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid enum member, using default of {1}", Name, defValue);
                return defValue;
            }
            return value;
        }
    }
    
    public sealed class ConfigVec3Attribute : ConfigAttribute {
        public ConfigVec3Attribute(string name, string section) : base(name, section) { }
        
        public override object Parse(string raw) {
            Vec3U16 value;
            try {
                string[] p = raw.SplitComma();
                value = new Vec3U16(ushort.Parse(p[0]), ushort.Parse(p[1]), ushort.Parse(p[2]));
            } catch {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid vec3, using default", Name);
                value = default(Vec3U16);
            }
            return value;
        }
    }
    
    public sealed class ConfigBoolArrayAttribute : ConfigAttribute {
        bool defValue;
        int minCount;
        
        public ConfigBoolArrayAttribute() : this(null, null, false, 0) { }
        public ConfigBoolArrayAttribute(string name, string section, bool def, int min)
            : base(name, section) { defValue = def; minCount = min; }
        
        public override object Parse(string value) {
            string[] parts = value.SplitComma();
            bool[] values  = new bool[minCount];
            int i;
            
            for (i = 0; i < parts.Length; i++) {
                if (bool.TryParse(parts[i], out values[i])) continue;
                
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid boolean, using default of {1}", Name, defValue);
                values[i] = defValue;
            }
            
            // shouldn't usually happen, but handle anyways
            for (; i < values.Length; i++) values[i] = defValue;
            return values;
        }
        
        public override string Serialise(object value) {
            bool[] values = (bool[])value;
            return values.Join(b => b.ToString(), ", ");
        }
    }
}
