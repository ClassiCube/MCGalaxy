/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
using MCGalaxy.Core;
using MCGalaxy.Modules.Games.Countdown;
using MCGalaxy.Modules.Games.CTF;
using MCGalaxy.Modules.Games.LS;
using MCGalaxy.Modules.Games.TW;
using MCGalaxy.Modules.Games.ZS;
using MCGalaxy.Modules.Moderation.Notes;
using MCGalaxy.Modules.Relay.Discord;
using MCGalaxy.Modules.Relay.IRC;
using MCGalaxy.Modules.Security;
using MCGalaxy.Scripting;

namespace MCGalaxy 
{
    /// <summary> This class provides for more advanced modification to MCGalaxy </summary>
    public abstract class Plugin 
    {
        /// <summary> Hooks into events and initalises states/resources etc </summary>
        /// <param name="auto"> True if plugin is being automatically loaded (e.g. on server startup), false if manually. </param>
        public abstract void Load(bool auto);
        
        /// <summary> Unhooks from events and disposes of state/resources etc </summary>
        /// <param name="auto"> True if plugin is being auto unloaded (e.g. on server shutdown), false if manually. </param>
        public abstract void Unload(bool auto);
        
        /// <summary> Called when a player does /Help on the plugin. Typically tells the player what this plugin is about. </summary>
        /// <param name="p"> Player who is doing /Help. </param>
        public virtual void Help(Player p) {
            p.Message("No help is available for this plugin.");
        }
        
        /// <summary> Name of the plugin. </summary>
        public abstract string name { get; }
        /// <summary> The oldest version of MCGalaxy this plugin is compatible with. </summary>
        public virtual string MCGalaxy_Version { get { return null; } }
        /// <summary> Version of this plugin. </summary>
        public virtual int build { get { return 0; } }
        /// <summary> Message to display once this plugin is loaded. </summary>
        public virtual string welcome { get { return ""; } }
        /// <summary> The creator/author of this plugin. (Your name) </summary>
        public virtual string creator { get { return ""; } }
        /// <summary> Whether or not to auto load this plugin on server startup. </summary>
        public virtual bool LoadAtStartup { get { return true; } }
        
        
        public static List<Plugin> core   = new List<Plugin>();
        public static List<Plugin> custom = new List<Plugin>();
        
        public static bool Load(Plugin p, bool auto) {
            try {
                string ver = p.MCGalaxy_Version;
                if (!String.IsNullOrEmpty(ver) && new Version(ver) > new Version(Server.Version)) {
                    Logger.Log(LogType.Warning, "Plugin ({0}) requires a more recent version of {1}!", p.name, Server.SoftwareName);
                    return false;
                }
                custom.Add(p);
                
                if (p.LoadAtStartup || !auto) {
                    p.Load(auto);
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

        public static bool Unload(Plugin p) {
            bool success = UnloadPlugin(p, false);
            
            // TODO only remove if successful?
            custom.Remove(p);
            core.Remove(p);
            return success;
        }
        
        static bool UnloadPlugin(Plugin p, bool auto) {
            try {
                p.Unload(auto);
                return true;
            } catch (Exception ex) {
                Logger.LogError("Error unloading plugin " + p.name, ex);
                return false;
            }
        }

        
        public static void UnloadAll() {
            for (int i = 0; i < custom.Count; i++) 
            {
                UnloadPlugin(custom[i], true);
            }
            custom.Clear();
            
            for (int i = 0; i < core.Count; i++) 
            {
                UnloadPlugin(core[i], true);
            }
        }

        public static void LoadAll() {
            LoadCorePlugin(new CorePlugin());
            LoadCorePlugin(new NotesPlugin());
            LoadCorePlugin(new DiscordPlugin());
            LoadCorePlugin(new IRCPlugin());
            LoadCorePlugin(new IPThrottler());
            
            LoadCorePlugin(new CountdownPlugin());
            LoadCorePlugin(new CTFPlugin());
            LoadCorePlugin(new LSPlugin());
            LoadCorePlugin(new TWPlugin());
            LoadCorePlugin(new ZSPlugin());
            
            IScripting.AutoloadPlugins();
        }
        
        static void LoadCorePlugin(Plugin plugin) {
            List<string> disabled = Server.Config.DisabledModules;
            if (disabled.CaselessContains(plugin.name)) return;
            
            plugin.Load(true);
            Plugin.core.Add(plugin);
        }
    }
}
