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
using System.Reflection;
using MCGalaxy.Config;

namespace MCGalaxy {
    
    public struct ConfigElement {
        public ConfigAttribute Attrib;
        public FieldInfo Field;
        
        public static ConfigElement[] GetAll(params Type[] types) {
            List<ConfigElement> elems = new List<ConfigElement>();
            foreach (Type type in types) {
                FieldInfo[] fields = type.GetFields();
                for (int i = 0; i < fields.Length; i++) {
                    FieldInfo field = fields[i];
                    Attribute[] attributes = Attribute.GetCustomAttributes(field, typeof(ConfigAttribute));
                    if (attributes.Length == 0) continue;
                    
                    ConfigElement elem;
                    elem.Field = field;
                    elem.Attrib = (ConfigAttribute)attributes[0];
                    elems.Add(elem);
                }
            }
            return elems.ToArray();
        }
        
        public static bool Parse(ConfigElement[] elems,
                                 string key, string value, object instance) {
            for (int i = 0; i < elems.Length; i++) {
                ConfigElement elem = elems[i];
                if (!elem.Attrib.Name.CaselessEq(key)) continue;
                
                elem.Field.SetValue(instance, elem.Attrib.Parse(value));
                return true;
            }
            return false;
        }
    }
}
