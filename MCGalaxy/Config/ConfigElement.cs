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
using System.IO;
using System.Reflection;
using MCGalaxy.Config;

namespace MCGalaxy {
    
    public struct ConfigElement {
        public ConfigAttribute Attrib;
        public FieldInfo Field;
        
        public string Format(object instance) {
            object value = Field.GetValue(instance);
            return Attrib.Name + " = " + Attrib.Serialise(value);
        }
        
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static ConfigElement[] GetAll(Type type) {
            List<ConfigElement> elems = new List<ConfigElement>();
            FieldInfo[] fields = type.GetFields(flags);
            
            for (int i = 0; i < fields.Length; i++) {
                FieldInfo field = fields[i];
                Attribute[] attributes = Attribute.GetCustomAttributes(field, typeof(ConfigAttribute));
                if (attributes.Length == 0) continue;
                
                ConfigElement elem;
                elem.Field = field;
                elem.Attrib = (ConfigAttribute)attributes[0];
                
                if (elem.Attrib.Name == null) elem.Attrib.Name = field.Name;
                elems.Add(elem);
            }
            return elems.ToArray();
        }
        
        public static bool ParseFile(ConfigElement[] elements, string path, object instance) {
            return PropertiesFile.Read(path, (k, v) => Parse(elements, instance, k, v));
        }
        
        public static void Parse(ConfigElement[] elems, object instance, string k, string v) {
            foreach (ConfigElement elem in elems) {
                if (!elem.Attrib.Name.CaselessEq(k)) continue;
                
                elem.Field.SetValue(instance, elem.Attrib.Parse(v)); return;
            }
        }
        
        public static void Serialise(ConfigElement[] elements, StreamWriter dst, object instance) {
            Dictionary<string, List<ConfigElement>> sections = new Dictionary<string, List<ConfigElement>>();
            
            foreach (ConfigElement elem in elements) {
                List<ConfigElement> members;
                if (!sections.TryGetValue(elem.Attrib.Section, out members)) {
                    members = new List<ConfigElement>();
                    sections[elem.Attrib.Section] = members;
                }
                members.Add(elem);
            }
            
            // group output by sections
            foreach (var kvp in sections) {
                dst.WriteLine("# " + kvp.Key + " settings");
                foreach (ConfigElement elem in kvp.Value) {
                    dst.WriteLine(elem.Format(instance));
                }
                dst.WriteLine();
            }
        }
        
        public static void SerialiseSimple(ConfigElement[] elements, string path, object instance) {
            using (StreamWriter w = new StreamWriter(path)) {
                w.WriteLine("#Settings file");
                foreach (ConfigElement elem in elements) {
                    w.WriteLine(elem.Format(instance));
                }
            }
        }
    }
}
