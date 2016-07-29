/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Threading;

namespace MCGalaxy {
    /// <summary> This class provides for more advanced modification to MCGalaxy </summary>
    public abstract partial class Plugin {

        /// <summary> List of all plugins. </summary>
        public static List<Plugin> all = new List<Plugin>();

        /// <summary> Look to see if a plugin is loaded </summary>
        /// <param name="name">The name of the plugin</param>
        /// <returns>Returns the plugin (returns null if non is found)</returns>
        public static Plugin Find(string name) {
            List<Plugin> tempList = new List<Plugin>();
            tempList.AddRange(all);
            Plugin match = null; int matches = 0;
            name = name.ToLower();

            foreach (Plugin p in tempList) {
                if (p.name.ToLower() == name) return p;
                if (p.name.ToLower().Contains(name)) {
                	match = p; matches++;
                }
            }
            return matches == 1 ? match : null;
        }

        
        /// <summary> Load a plugin </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="startup">Is this startup?</param>
        public static void Load(string name, bool startup){
            string creator = "";
            string path = "plugins/" + name + ".dll";
            try {
                Plugin instance = null;
                Assembly lib = null;
                using (FileStream fs = File.Open(path, FileMode.Open)) {
                    using (MemoryStream ms = new MemoryStream()) {
                        byte[] buffer = new byte[1024];
                        int read = 0;
                        while ((read = fs.Read(buffer, 0, 1024)) > 0)
                            ms.Write(buffer, 0, read);
                        lib = Assembly.Load(ms.ToArray());
                    }
                }

                try {
                	foreach (Type t in lib.GetTypes()) {
                		if (!t.IsSubclassOf(typeof(Plugin))) continue;
                		instance = (Plugin)Activator.CreateInstance(t);
                		break;
                    }
                } catch { }
                if (instance == null) {
                    Server.s.Log("The plugin " + name + " couldn't be loaded!");
                    return;
                }
                creator = instance.creator;
                
                string ver = instance.MCGalaxy_Version;
                if (!String.IsNullOrEmpty(ver) && new Version(ver) > Server.Version) {
                    Server.s.Log("This plugin (" + instance.name + ") isn't compatible with this version of MCGalaxy!");
                    Thread.Sleep(1000);
                    if (!Server.unsafe_plugin) return;
                    
                    Server.s.Log("Will attempt to load!");
                }

                Plugin.all.Add(instance);
                
                if (instance.LoadAtStartup) {
                    instance.Load(startup);
                    Server.s.Log("Plugin: " + instance.name + " loaded...build: " + instance.build);
                } else {
                    Server.s.Log("Plugin: " + instance.name + " was not loaded, you can load it with /pload");
                }
                Server.s.Log(instance.welcome);
            } catch (Exception e) {
                Server.ErrorLog(e);
                Server.s.Log("The plugin " + name + " failed to load!");
                if (creator != "") Server.s.Log("You can go bug " + creator + " about it.");
                Thread.Sleep(1000);
            }
        }
        
        /// <summary> Unload a plugin </summary>
        /// <param name="p">The plugin to unload</param>
        /// <param name="shutdown">Is this shutdown?</param>
        public static void Unload(Plugin p, bool shutdown) {
            try {
                p.Unload(shutdown);
                all.Remove(p);
                Server.s.Log(p.name + " was unloaded.");
            } catch { Server.s.Log("An error occurred while unloading a plugin."); 
        	}
        }

        /// <summary> Unload all plugins </summary>
        public static void Unload() {
        	for (int i = 0; i < all.Count; i++) {
        	    Unload(all[i], true); i--;
        	}
        }
        
        /// <summary> Load all plugins </summary>
        public static void Load() {
            if (Directory.Exists("plugins")) {
                foreach (string path in Directory.GetFiles("plugins", "*.dll")) {
                    string name = Path.GetFileNameWithoutExtension(path);
                    Load(name, true);
                }
        	} else {
                Directory.CreateDirectory("plugins");
        	}
        	
            // Load Internal Plugins
            CTF.Setup temp = new CTF.Setup();
            temp.Load(true);
            Plugin.all.Add(temp);
        }
    }
}

