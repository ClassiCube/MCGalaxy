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
    
    public sealed class OnGroupLoadedEvent : IPluginEvent<Group.GroupLoaded> {
        
        internal OnGroupLoadedEvent(Group.GroupLoaded method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Group g) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(g);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling GroupLoaded Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    public sealed class OnGroupLoadEvent : IPluginEvent<Group.GroupLoad> {
        
        internal OnGroupLoadEvent(Group.GroupLoad method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call() {
            events.ForEach(
                pl => {
                    try {
                        pl.method();
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling GroupLoad Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    public sealed class OnGroupSaveEvent : IPluginEvent<Group.GroupSave> {
        
        internal OnGroupSaveEvent(Group.GroupSave method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call() {
            events.ForEach(
                pl => {
                    try {
                        pl.method();
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling GroupSave Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
    
    public sealed class OnPlayerRankSetEvent : IPluginEvent<Group.RankSet> {
        
        internal OnPlayerRankSetEvent(Group.RankSet method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p, Group newRank) {
            events.ForEach(
                pl => {
                    try {
                        pl.method(p, newRank);
                    } catch (Exception e) {
                        Server.s.Log("Plugin " + pl.plugin.name + " errored when calling PlayerRankSet Event."); Server.ErrorLog(e);
                    }
                });
        }
    }
}
