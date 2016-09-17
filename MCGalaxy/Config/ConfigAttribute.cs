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
        
        /// <summary> Converts the given value into its serialised string form. </summary>
        public virtual string Serialise(object value) { return value == null ? "" : value.ToString(); }
        
        public ConfigAttribute(string name, string section, string desc, object defValue) {
            Name = name; Description = desc; 
            Section = section; DefaultValue = defValue;
        }
    }
}
