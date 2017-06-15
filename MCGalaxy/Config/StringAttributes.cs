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

namespace MCGalaxy.Config {
    
    public sealed class ConfigColorAttribute : ConfigAttribute {
        
        public ConfigColorAttribute(string name, string section, string desc, string defValue)
            : base(name, section, desc, defValue) {
        }
        
        public override object Parse(string value) {
            string color = Colors.Parse(value);
            if (color == "") {
                color = Colors.Name(value);
                if (color != "") return value;
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid color, using default of {1}", Name, DefaultValue);
                return DefaultValue;
            }
            return color;
        }
    }
    
    public sealed class ConfigStringAttribute : ConfigAttribute {
        
        /// <summary> Whether an empty string is an allowed for the value, or if it is treated as the default value. </summary>
        public bool AllowEmpty;
        
        /// <summary> Specifies the restricted set of characters (asides from alphanumeric characters)
        /// that the value is allowed to have. </summary>
        public string AllowedChars;
        
        /// <summary> Maximum number of characters allowed in the value. 0 means no limit. </summary>
        public int MaxLength = 0;
        
        public ConfigStringAttribute(string name, string section, string desc, string defValue,
                                     bool allowEmpty, string allowedChars, int maxLength)
            : base(name, section, desc, defValue) {
            AllowEmpty = allowEmpty;
            AllowedChars = allowedChars;
            MaxLength = maxLength;
        }
 
        // NOTE: required to define these, some compilers error when we try using optional parameters with:
        // "An attribute argument must be a constant expression, typeof expression.."
        public ConfigStringAttribute(string name, string section, string desc, string defValue,
                                     bool allowEmpty, string allowedChars)
            : base(name, section, desc, defValue) {
            AllowEmpty = allowEmpty;
            AllowedChars = allowedChars;
        }

        public ConfigStringAttribute(string name, string section, string desc, string defValue,
                                     bool allowEmpty)
            : base(name, section, desc, defValue) {
            AllowEmpty = allowEmpty;
        }

        public ConfigStringAttribute(string name, string section, string desc, string defValue)
            : base(name, section, desc, defValue) {
        }
        
        public override object Parse(string value) {
            if (value == "") {
                if (!AllowEmpty) {
                    Logger.Log(LogType.Warning, "Config key \"{0}\" has no value, using default of {1}", Name, DefaultValue);
                    return DefaultValue;
                }
                return "";
            } else if (AllowedChars == null) {
                return Truncate(value);
            }
            
            foreach (char c in value) {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    continue;
                
                if (AllowedChars.IndexOf(c) == -1) {
                    Logger.Log(LogType.Warning, "Config key \"{0}\" contains non-allowed characters, using default of {1}", Name, DefaultValue);
                    return DefaultValue;
                }
            }
            return Truncate(value);
        }
        
        string Truncate(string value) {
            if (MaxLength > 0 && value.Length > MaxLength) {
                value = value.Substring(0, MaxLength);
                Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid color, truncating to {1}", Name, value);
            }
            return value;
        }
    }
    
    public sealed class ConfigStringListAttribute : ConfigAttribute {
        
        public ConfigStringListAttribute(string name, string section, string desc)
            : base(name, section, desc, new List<string>()) {
        }
        
        public override object Parse(string value) {
            if (value == "") return new List<string>();
            
            string[] split = value.Replace(" ", "").Split(',');
            return new List<string>(split);
        }
        
        public override string Serialise(object value) {
            List<string> elements = (List<string>)value;
            return elements.Join(",");
        }
    }
}
