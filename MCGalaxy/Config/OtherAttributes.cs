﻿/*
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

namespace MCGalaxy.Config {
    
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
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid integer, using default of {1}", Name, DefaultValue);
                return DefaultValue;
            }
            
            if (intValue < MinValue) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too small an integer, using {1}", Name, MinValue);
                return MinValue;
            }
            if (intValue > MaxValue) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too big an integer, using {1}", Name, MaxValue);
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
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid boolean, using default of {1}", Name, DefaultValue);
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
                // Try parse the permission as name for backwards compatibility
                Group grp = Group.Find(value);
                if (grp == null) {
                    Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid permission, using default of {1}", Name, DefaultValue);
                    return DefaultValue;
                }
                perm = grp.Permission;
            } else {
                perm = (LevelPermission)permNum;
            }
            
            if (perm < LevelPermission.Banned) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" cannot be below banned rank.", Name);
                return LevelPermission.Banned;
            }
            if (perm > LevelPermission.Nobody) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" cannot be above nobody rank.", Name);
                return LevelPermission.Nobody;
            }
            return perm;
        }
        
        public override string Serialise(object value) {
            LevelPermission perm = (LevelPermission)value;
            return ((sbyte)perm).ToString();
        }
    }
    
    public sealed class ConfigDateTimeAttribute : ConfigAttribute {
        
        public ConfigDateTimeAttribute(string name, string section, string desc)
            : base(name, section, desc, DateTime.MinValue) {
        }
        
        public override object Parse(string value) {
            DateTime time;
            if (!DateTime.TryParse(value, out time)) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid datetime, using default of {1}", Name, DefaultValue);
                return DefaultValue;
            }
            return time;
        }
        
        public override string Serialise(object value) {
            DateTime time = (DateTime)value;
            return time.ToShortDateString();
        }
    }
    
    public sealed class ConfigEnumAttribute : ConfigAttribute {
        
        /// <summary> The type of members of this enumeration. </summary>
        public Type EnumType;
        
        public ConfigEnumAttribute(string name, string section, string desc, 
                                   object defValue, Type enumType)
            : base(name, section, desc, defValue) {
            EnumType = enumType;
        }
        
        public override object Parse(string value) {
            object result;
            try {
                result = Enum.Parse(EnumType, value, true);
            } catch {
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid enum member, using default of {1}", Name, DefaultValue);
                return DefaultValue;
            }
            return result;
        }
    }
}
