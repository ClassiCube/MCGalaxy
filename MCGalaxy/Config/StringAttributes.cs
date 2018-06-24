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
        string defCol;
        
        public ConfigColorAttribute(string name, string section, string def)
            : base(name, section) { defCol = def; }
        
        public override object Parse(string raw) {
            string col = Colors.Parse(raw);
            if (col.Length != 0) return col;
            
            col = Colors.Name(raw);
            if (col.Length > 0) return raw;
                
            Logger.Log(LogType.Warning, "Config key \"{0}\" is not a valid color, using default of {1}", Name, defCol);
            return defCol;
        }
    }
    
    public sealed class ConfigStringAttribute : ConfigAttribute {
        bool allowEmpty;
        string defValue, allowedChars;
        int maxLen = int.MaxValue;
        
        public ConfigStringAttribute(string name, string section, string def,
                                     bool empty, string allowed, int len)
            : base(name, section) { defValue = def; allowEmpty = empty; allowedChars = allowed; maxLen = len; }
 
        // NOTE: required to define these, some compilers error when we try using optional parameters with:
        // "An attribute argument must be a constant expression, typeof expression.."
        public ConfigStringAttribute(string name, string section, string def, bool empty, string allowed)
            : base(name, section) { defValue = def; allowEmpty = empty; allowedChars = allowed; }

        public ConfigStringAttribute(string name, string section, string def, bool empty)
            : base(name, section) { defValue = def; allowEmpty = empty; }

        public ConfigStringAttribute(string name, string section, string def)
            : base(name, section) { defValue = def; }
        
        public override object Parse(string value) {
            if (value.Length == 0 && !allowEmpty) {
                Logger.Log(LogType.Warning, "Config key \"{0}\" has no value, using default of {1}", Name, defValue);
                return defValue;
            }            
            if (allowedChars != null) value = Constrain(value);
            
            if (value.Length > maxLen) {
                value = value.Substring(0, maxLen);
                Logger.Log(LogType.Warning, "Config key \"{0}\" is too long, truncating to {1}", Name, value);
            }
            return value;
        }
        
        string Constrain(string value) {
            foreach (char c in value) {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) continue;                
                if (allowedChars.IndexOf(c) >= 0) continue;
                
                Logger.Log(LogType.Warning, "Config key \"{0}\" contains non-allowed characters, using default of {1}", Name, defValue);
                return defValue;
            }
            return value;
        }
    }
    
    public sealed class ConfigStringListAttribute : ConfigAttribute {
        
        public ConfigStringListAttribute(string name, string section) 
            : base(name, section) { }
        
        public override object Parse(string value) {
            if (value.Length == 0) return new List<string>();
            
            string[] split = value.Replace(" ", "").Split(',');
            return new List<string>(split);
        }
        
        public override string Serialise(object value) {
            List<string> elements = (List<string>)value;
            return elements.Join(",");
        }
    }
}
