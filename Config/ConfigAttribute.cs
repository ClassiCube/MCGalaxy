using System;

namespace MCGalaxy.Config {

    public abstract class ConfigAttribute : Attribute {
        
        /// <summary> Key used for writing/reading from the property file. </summary>
        public string Name;
        
        /// <summary> Section/Group in the property file this config entry is part of. </summary>
        public string Section;
        
        /// <summary> Comment shown in the property file above the key-value entry. </summary>
        public string Description;
        
        /// <summary> The default value used if validating the value read from the propery file fails. </summary>
        public object DefaultValue;
        
        /// <summary> Returns either the parsed form of the given value, or some other value if validation fails. </summary>
        public abstract object Parse(string value);
        
        public ConfigAttribute(string name, string section, string desc, object defValue) {
            Name = name; Description = desc; 
            Section = section; DefaultValue = defValue;
        }
    }
    
    public sealed class ConfigIntAttribute : ConfigAttribute {
        
        /// <summary> Minimum integer allowed for a value. </summary>
        public int MinValue;
        
        /// <summary> Maximum value integer allowed for a value. </summary>
        public int MaxValue;
        
        public ConfigIntAttribute(string name, string section, string desc, int defValue,
                                  int min = int.MinValue, int max = int.MaxValue) 
            : base(name, section, desc, defValue) {
            MinValue = min; MaxValue = max;
        }
        
        public override object Parse(string value) {
            int intValue;
            if (!int.TryParse(value, out intValue)) {
                Server.s.Log("Config key \"" + Name + "\" is not a valid integer, " +
                             "using default of " + DefaultValue);
                return DefaultValue;
            }
            
            if (intValue < MinValue) {
                Server.s.Log("Config key \"" + Name + "\" is too small an integer, using " + MinValue);
                return MinValue;
            }
            if (intValue > MaxValue) {
                Server.s.Log("Config key \"" + Name + "\" is too big an integer, using " + MaxValue);
                return MaxValue;
            }
            return intValue;
        }
    }
    
    public sealed class ConfigBoolAttribute : ConfigAttribute {
        
        public ConfigBoolAttribute(string name, string section, string desc, bool defValue) 
            : base(name, section, desc, defValue) {
        }
        
        public override object Parse(string value) {
            bool boolValue;
            if (!bool.TryParse(value, out boolValue)) {
                Server.s.Log("Config key \"" + Name + "\" is not a valid boolean, " +
                             "using default of " + DefaultValue);
                return DefaultValue;
            }
            return boolValue;
        }
    }
    
    public sealed class ConfigPermAttribute : ConfigAttribute {
        
        public ConfigPermAttribute(string name, string section, string desc, LevelPermission defValue) 
            : base(name, section, desc, defValue) {
        }
        
        public override object Parse(string value) {
            sbyte permNum;
            LevelPermission perm;
            if (!sbyte.TryParse(value, out permNum)) {
                // Try parse the permission as name.
                Group grp = Group.Find(value);
                if (grp == null) {
                    Server.s.Log("Config key \"" + Name + "\" is not a valid permission, " +
                                 "using default of " + DefaultValue);
                    return DefaultValue;
                }
                perm = grp.Permission;
            } else {
                perm = (LevelPermission)permNum;
            }
            
            if (perm < LevelPermission.Banned) {
                Server.s.Log("Config key \"" + Name + "\" cannot be below banned rank.");
                return LevelPermission.Banned;
            }
            if (perm > LevelPermission.Nobody) {
                Server.s.Log("Config key \"" + Name + "\" cannot be above nobody rank.");
                return LevelPermission.Nobody;
            }
            return perm;
        }
    }
}
