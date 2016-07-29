/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy {
    // NOTE: You must use a different delegate type for each subclass
    // This is because the static events are unique to each generic instantiation, not each subclass.
    public class IPluginEvent<IMethod> {
        protected internal static List<IPluginEvent<IMethod>> handlers = new List<IPluginEvent<IMethod>>();
        protected internal Plugin plugin;
        protected internal IMethod method;
        protected internal Priority priority;
        
        internal IPluginEvent(IMethod method, Priority priority, Plugin plugin) { 
            this.plugin = plugin; 
            this.priority = priority; 
            this.method = method; 
        }
        
        /// <summary> Register this event </summary>
        /// <param name="method">This is the delegate that will get called when this event occurs</param>
        /// <param name="priority">The priority (imporantce) of this call</param>
        /// <param name="plugin">The plugin object that is registering the event</param>
        /// <param name="bypass">Register more than one of the same event</param>
        public static void Register(IMethod method, Priority priority, Plugin plugin, bool bypass = false) {
            if (Find(plugin) != null && !bypass)
                throw new ArgumentException("The user tried to register 2 of the same event!");
            handlers.Add(new IPluginEvent<IMethod>(method, priority, plugin));
            Organize();
        }
        
        /// <summary> UnRegister this event </summary>
        /// <param name="plugin">The plugin object that has this event registered</param>
        public static void UnRegister(Plugin plugin) {
            if (Find(plugin) == null)
                throw new ArgumentException("This plugin doesnt have this event registered!");
            else
                handlers.Remove(Find(plugin));
        }
        
        public static IPluginEvent<IMethod> Find(Plugin plugin) {
            foreach (var p in handlers) {
                if (p.plugin == plugin) return p;
            }
            return null;
        }
        
        
        protected static void Organize() {
            handlers.Sort((a, b) => b.priority.CompareTo(a.priority));
        }
        
        protected static void CallImpl(Action<IMethod> action) {
            try {
                foreach (var pl in handlers) {
                    try {
                        action(pl.method);
                    } catch (Exception ex) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling " + typeof(IMethod).Name + " event"); 
                        Server.ErrorLog(ex);
                    }
                }
            } catch (Exception ex) {
                Server.s.Log("Error when calling " + typeof(IMethod).Name + " event"); 
                Server.ErrorLog(ex);
            }
        }
    }
}
