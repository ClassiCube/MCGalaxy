﻿/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Core;
using MCGalaxy.Scripting;

namespace MCGalaxy {
    /// <summary> This class provides for more advanced modification to MCGalaxy </summary>
    public abstract partial class Plugin {
        internal static List<Plugin> core = new List<Plugin>();
        public static List<Plugin> all = new List<Plugin>();

        /// <summary> Loads all plugins from the given path. </summary>
        public static bool Load(string path, bool startup) {
            try {
                byte[] data  = File.ReadAllBytes(path);
                Assembly lib = Assembly.Load(data);
                List<Plugin> plugins = IScripting.LoadTypes<Plugin>(lib);
                
                foreach (Plugin plugin in plugins) {
                    if (!Load(plugin, startup)) return false;
                }
                return true;
            } catch (Exception ex) {
                Logger.LogError("Error loading plugins from " + path, ex);
                return false;
            }
        }
        
        public static bool Load(Plugin p, bool startup) {
            try {
                string ver = p.MCGalaxy_Version;
                if (!String.IsNullOrEmpty(ver) && new Version(ver) > new Version(Server.Version)) {
                    Logger.Log(LogType.Warning, "Plugin ({0}) requires a more recent version of {1}!", p.name, Server.SoftwareName);
                    return false;
                }
                all.Add(p);
                
                if (p.LoadAtStartup || !startup) {
                    p.Load(startup);
                    Logger.Log(LogType.SystemActivity, "Plugin {0} loaded...build: {1}", p.name, p.build);
                } else {
                    Logger.Log(LogType.SystemActivity, "Plugin {0} was not loaded, you can load it with /pload", p.name);
                }
                
                if (!String.IsNullOrEmpty(p.welcome)) Logger.Log(LogType.SystemActivity, p.welcome);
                return true;
            } catch (Exception ex) {
                Logger.LogError("Error loading plugin " + p.name, ex);               
                if (!String.IsNullOrEmpty(p.creator)) Logger.Log(LogType.Warning, "You can go bug {0} about it.", p.creator);
                return false;
            }
        }

        public static bool Unload(Plugin p, bool shutdown) {
            bool success = true;
            try {
                p.Unload(shutdown);
                Logger.Log(LogType.SystemActivity, "Plugin {0} was unloaded.", p.name);
            } catch (Exception ex) {
                Logger.LogError("Error unloading plugin " + p.name, ex);
                success = false;
            }
            
            all.Remove(p);
            return success;
        }

        public static void UnloadAll() {
            for (int i = 0; i < all.Count; i++) {
                Unload(all[i], true); i--;
            }
        }

        public static void LoadAll() {
            LoadCorePlugin(new CorePlugin());
            LoadCorePlugin(new NotesPlugin());
            
            if (Directory.Exists("plugins")) {
                string[] files = Directory.GetFiles("plugins", "*.dll");
                foreach (string path in files) { Load(path, true); }
            } else {
                Directory.CreateDirectory("plugins");
            }
        }
        
        static void LoadCorePlugin(Plugin plugin) {
            plugin.Load(true);
            Plugin.all.Add(plugin);
            Plugin.core.Add(plugin);
        }
    }
}