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
using MCGalaxy.Blocks.Physics;

namespace MCGalaxy.Events {
    
    public sealed class OnLevelLoadedEvent : IPluginEvent<Level.OnLevelLoaded> {        
        internal OnLevelLoadedEvent(Level.OnLevelLoaded method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Level l) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(l));
        }
    }
    
    public sealed class OnLevelLoadEvent : IPluginEvent<Level.OnLevelLoad> {        
        internal OnLevelLoadEvent(Level.OnLevelLoad method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(string name) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(name));
        }
    }
    
    public sealed class OnLevelSaveEvent : IPluginEvent<Level.OnLevelSave> {       
        internal OnLevelSaveEvent(Level.OnLevelSave method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Level l) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(l));
        }
    }
    
    public sealed class OnLevelUnloadEvent : IPluginEvent<Level.OnLevelUnload> {        
        internal OnLevelUnloadEvent(Level.OnLevelUnload method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Level l) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(l));
        }
    }
    
    public sealed class OnPhysicsUpdateEvent : IPluginEvent<Level.OnPhysicsUpdate> {
        
        internal OnPhysicsUpdateEvent(Level.OnPhysicsUpdate method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(ushort x, ushort y, ushort z, PhysicsArgs extraInfo, Level l) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(x, y, z, extraInfo, l));
        }
    }
}
