/*
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

        public static bool Load(string name, bool startup) {
            string creator = "";
            string path = IScripting.PluginPath(name);
            
            try {
                byte[] data = File.ReadAllBytes(path);
                Assembly lib = Assembly.Load(data);
                List<Plugin> plugins = IScripting.LoadTypes<Plugin>(lib);
                
                foreach (Plugin plugin in plugins) {
                    creator = plugin.creator;
                    string ver = plugin.MCGalaxy_Version;
                    if (!String.IsNullOrEmpty(ver) && new Version(ver) > Server.Version) {
                        Logger.Log(LogType.Warning, "Plugin ({0}) requires a more recent version of {1}!", plugin.name, Server.SoftwareName);
                        return false;
                    }
                    Plugin.all.Add(plugin);
                    
                    if (plugin.LoadAtStartup || !startup) {
                        plugin.Load(startup);
                        Logger.Log(LogType.SystemActivity, "Plugin: {0} loaded...build: {1}", plugin.name, plugin.build);
                    } else {
                        Logger.Log(LogType.SystemActivity, "Plugin: {0} was not loaded, you can load it with /pload", plugin.name);
                    }
                    Logger.Log(LogType.SystemActivity, plugin.welcome);
                }
                return true;
            } catch (Exception ex) {
                Logger.LogError("Error loading plugin " + name, ex);
                if (!String.IsNullOrEmpty(creator)) Logger.Log(LogType.Warning, "You can go bug {0} about it.", creator);
                return false;
            }
        }

        public static bool Unload(Plugin p, bool shutdown) {
            bool success = true;
            try {
                p.Unload(shutdown);
                Logger.Log(LogType.SystemActivity, p.name + " was unloaded.");
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
                foreach (string path in Directory.GetFiles("plugins", "*.dll")) {
                    string name = Path.GetFileNameWithoutExtension(path);
                    Load(name, true);
                }
            } else {
                Directory.CreateDirectory("plugins");
            }
        }
        
        internal static void LoadCorePlugin(Plugin plugin) {
            plugin.Load(true);
            Plugin.all.Add(plugin);
            Plugin.core.Add(plugin);
        }
    }
}