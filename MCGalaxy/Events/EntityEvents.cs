/*
    Copyright 2015 MCGalaxy
        
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

namespace MCGalaxy.Events.EntityEvents {

    public delegate void OnTabListEntryAdded(Entity e, ref string tabName, ref string tabGroup, Player dst);
    /// <summary> Called when a tab list entry is being sent to a user. </summary>
    public sealed class OnTabListEntryAddedEvent : IEvent<OnTabListEntryAdded> {
        
        public static void Call(Entity e, ref string tabName, ref string tabGroup, Player dst) {
            IEvent<OnTabListEntryAdded>[] items = handlers.Items;
            // Can't use CallCommon because we need to pass arguments by ref
            for (int i = 0; i < items.Length; i++) {
                try {
                    items[i].method(e, ref tabName, ref tabGroup, dst);
                } catch (Exception ex) {
                    LogHandlerException(ex, items[i]);
                }
            }
        }
    }

    public delegate void OnTabListEntryRemoved(Entity e, Player dst);
    /// <summary> Called when a tab list entry is being removed from a user. </summary>
    public sealed class OnTabListEntryRemovedEvent : IEvent<OnTabListEntryRemoved> {
        
        public static void Call(Entity e, Player dst) {
            IEvent<OnTabListEntryRemoved>[] items = handlers.Items;
            // Don't use CallCommon, because this event is called very frequently
            // and want to avoid lots of pointless temp mem allocations
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(e, dst); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
    
    public delegate void OnEntitySpawned(Entity e, ref string name, ref string skin, ref string model, Player dst);
    /// <summary> Called when an entity is being spawned to someone. </summary>
    public sealed class OnEntitySpawnedEvent : IEvent<OnEntitySpawned> {
        
        public static void Call(Entity e, ref string name, ref string skin, ref string model, Player dst) {
            IEvent<OnEntitySpawned>[] items = handlers.Items;
            // Can't use CallCommon because we need to pass arguments by ref
            for (int i = 0; i < items.Length; i++) {
                try {
                    items[i].method(e, ref name, ref skin, ref model, dst);
                } catch (Exception ex) {
                    LogHandlerException(ex, items[i]);
                }
            }
        }
    }
    
    public delegate void OnEntityDespawned(Entity e, Player dst);
    /// <summary> Called when an entity is being despawned from someone. </summary>
    public sealed class OnEntityDespawnedEvent : IEvent<OnEntityDespawned> {
        
        public static void Call(Entity e, Player dst) {
            IEvent<OnEntityDespawned>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(e, dst); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }

    public delegate void OnSendingModel(Entity e, ref string model, Player dst);
    /// <summary> Called when model is being sent to a player. </summary>
    public sealed class OnSendingModelEvent : IEvent<OnSendingModel> {
        
        public static void Call(Entity e, ref string model, Player dst) {
            IEvent<OnSendingModel>[] items = handlers.Items;
            // Can't use CallCommon because we need to pass arguments by ref
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(e, ref model, dst); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
    
    public delegate void OnGettingCanSeeEntity(Player p, ref bool canSee, Entity target);
    /// <summary> Called when checking if this player can see the given entity as an entity in the level. </summary>
    /// <remarks> e.g. You can use this event to make a player invisible during a game. </remarks>
    public sealed class OnGettingCanSeeEntityEvent : IEvent<OnGettingCanSeeEntity> {
        
        public static void Call(Player p, ref bool canSee, Entity target) {
            IEvent<OnGettingCanSeeEntity>[] items = handlers.Items;
            // Can't use CallCommon because we need to pass arguments by ref
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(p, ref canSee, target); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
}
