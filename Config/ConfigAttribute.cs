using System;

namespace MCGalaxy.Config {

    public abstract class ConfigAttribute : Attribute {
        
        /// <summary> Key used for writing/reading from the property file. </summary>
        public string Name;
        
        /// <summary> Comment shown in the property file above the key-value entry. </summary>
        public string Description;
        
        /// <summary> The default value used if validating the value read from the propery file fails. </summary>
        public object DefaultValue;
        
        /// <summary> Returns either the parsed form of the given value, or some other value if validation fails. </summary>
        public abstract object Validate(string value);
        
        public ConfigAttribute(string name, string desc, object defValue) {
            Name = name; Description = desc; DefaultValue = defValue;
        }
    }
    
    public sealed class ConfigIntAttribute : ConfigAttribute {
        
        /// <summary> Minimum integer allowed for a value. </summary>
        public int MinValue;
        
        /// <summary> Maximum value integer allowed for a value. </summary>
        public int MaxValue;
        
        public ConfigIntAttribute(string name, string desc, int defValue,
                                  int min = int.MinValue, int max = int.MaxValue) 
            : base(name, desc, defValue) {
            MinValue = min; MaxValue = max;
        }
        
        public override object Validate(string value) {
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
        
        public ConfigBoolAttribute(string name, string desc, bool defValue) 
            : base(name, desc, defValue) {
        }
        
        public override object Validate(string value) {
            bool boolValue;
            if (!bool.TryParse(value, out boolValue)) {
                Server.s.Log("Config key \"" + Name + "\" is not a valid boolean, " +
                             "using default of " + DefaultValue);
                return DefaultValue;
            }
            return boolValue;
        }
    }
}
