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
    
    public sealed class OnConsoleCommandEvent : IPluginEvent<Server.OnConsoleCommand> {
        
        internal OnConsoleCommandEvent(Server.OnConsoleCommand method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(string cmd, string message) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(cmd, message);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling ConsoleCommand Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    public sealed class OnServerErrorEvent : IPluginEvent<Server.OnServerError> {
        
        internal OnServerErrorEvent(Server.OnServerError method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Exception ex) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(ex);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling ServerError Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    public sealed class OnServerLogEvent : IPluginEvent<Server.OnServerLog> {
        
        internal OnServerLogEvent(Server.OnServerLog method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(string message) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(message);
                    } catch (Exception e) { 
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling the Server LogEvent."); Server.ErrorLog(e); 
                    }
                });
        }
    }
}
