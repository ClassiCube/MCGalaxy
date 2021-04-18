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
using BlockID = System.UInt16;
   
namespace MCGalaxy.Events.LevelEvents {
    
    public delegate void OnLevelLoaded(Level lvl);
    public sealed class OnLevelLoadedEvent : IEvent<OnLevelLoaded> {
        
        public static void Call(Level lvl) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(lvl));
        }
    }
    
    public delegate void OnLevelLoad(string name, string path, ref bool cancel);
    public sealed class OnLevelLoadEvent : IEvent<OnLevelLoad> {
        
        public static void Call(string name, string path, ref bool cancel) {
            IEvent<OnLevelLoad>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(name, path, ref cancel); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
    
    public delegate void OnLevelSave(Level lvl, ref bool cancel);
    public sealed class OnLevelSaveEvent : IEvent<OnLevelSave> {
        
        public static void Call(Level lvl, ref bool cancel) {
            IEvent<OnLevelSave>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(lvl, ref cancel); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
    
    public delegate void OnLevelUnload(Level lvl, ref bool cancel);
    public sealed class OnLevelUnloadEvent : IEvent<OnLevelUnload> {
        
    	public static void Call(Level lvl, ref bool cancel) {
            IEvent<OnLevelUnload>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(lvl, ref cancel); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
    
    public delegate void OnLevelAdded(Level lvl);
    public sealed class OnLevelAddedEvent : IEvent<OnLevelAdded> {
        
        public static void Call(Level lvl) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(lvl));
        }
    }
    
    public delegate void OnLevelRemoved(Level lvl);
    public sealed class OnLevelRemovedEvent : IEvent<OnLevelRemoved> {
        
        public static void Call(Level lvl) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(lvl));
        }
    }
    
    public delegate void OnPhysicsStateChanged(Level lvl, PhysicsState state);
    public sealed class OnPhysicsStateChangedEvent : IEvent<OnPhysicsStateChanged> {
        
        public static void Call(Level lvl, PhysicsState state) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(lvl, state));
        }
    }
    
    public delegate void OnPhysicsLevelChanged(Level lvl, int level);
    public sealed class OnPhysicsLevelChangedEvent : IEvent<OnPhysicsLevelChanged> {
        
        public static void Call(Level lvl, int level) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(lvl, level));
        }
    }
    
    public delegate void OnPhysicsUpdate(ushort x, ushort y, ushort z, PhysicsArgs args, Level lvl);
    public sealed class OnPhysicsUpdateEvent : IEvent<OnPhysicsUpdate> {
        public static void Call(ushort x, ushort y, ushort z, PhysicsArgs extraInfo, Level l) {
            
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(x, y, z, extraInfo, l));
        }
    }    
    
    public delegate void OnLevelRenamed(string srcMap, string dstMap);
    public sealed class OnLevelRenamedEvent : IEvent<OnLevelRenamed> {
        
        public static void Call(string srcMap, string dstMap) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(srcMap, dstMap));
        }
    }    
    
    public delegate void OnLevelCopied(string srcMap, string dstMap);
    public sealed class OnLevelCopiedEvent : IEvent<OnLevelCopied> {
        
        public static void Call(string srcMap, string dstMap) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(srcMap, dstMap));
        }
    }  
        
    public delegate void OnLevelDeleted(string map);
    public sealed class OnLevelDeletedEvent : IEvent<OnLevelDeleted> {
        
        public static void Call(string map) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(map));
        }
    }
        
    public delegate void OnBlockHandlersUpdated(Level lvl, BlockID block);
    public sealed class OnBlockHandlersUpdatedEvent : IEvent<OnBlockHandlersUpdated> {
        
        public static void Call(Level lvl, BlockID block) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(lvl, block));
        }
    }
}
