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
                Server.s.Log("Config key \"" + Name + "\" is not a valid color, " +
                             "using default of " + DefaultValue);
                return DefaultValue;
            }
            return color;
        }
    }
    
    public sealed class ConfigStringAttribute : ConfigAttribute {
        
        /// <summary> Whether the empty string is an allowed, or if is treated as the default value. </summary>
        public bool AllowEmpty;
        
        /// <summary> Specifies the restricted set of characters (asides from alphanumeric characters) 
        /// that this field is allowed to have. </summary>
        public string AllowedChars;
        
        public ConfigStringAttribute(string name, string section, string desc, string defValue,
                                    bool allowEmpty = false, string allowedChars = null)
            : base(name, section, desc, defValue) {
            AllowEmpty = allowEmpty;
            AllowedChars = allowedChars;
        }
        
        public override object Parse(string value) {
            if (value == "") {
                if (!AllowEmpty) {
                      Server.s.Log("Config key \"" + Name + "\" has no value, using default of " + DefaultValue);
                      return DefaultValue;
                }
                return "";
            } else if (AllowedChars == null) {
                return value;
            }
            
            foreach (char c in value) {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    continue;
                
                if (AllowedChars.IndexOf(c) == -1) {
                      Server.s.Log("Config key \"" + Name + "\" contains " +
                                 "a non-allowed character, using default of " + DefaultValue);
                      return DefaultValue;
                }
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
            return elements.Concatenate(",");
        }
    }
}
