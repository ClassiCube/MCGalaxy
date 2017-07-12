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

    public delegate void OnTabListEntryAdded(Entity entity, ref string tabName, ref string tabGroup, Player dst);
    /// <summary> Called when a tab list entry is being sent to a user. </summary>
    public sealed class OnTabListEntryAddedEvent : IEvent<OnTabListEntryAdded> {
        
        public static void Call(Entity entity, ref string tabName, ref string tabGroup, Player dst) {
            IEvent<OnTabListEntryAdded>[] items = handlers.Items;
            // Can't use CallCommon because we need to pass arguments by ref
            for (int i = 0; i < items.Length; i++) {
                IEvent<OnTabListEntryAdded> handler = items[i];
                
                try {
                    handler.method(entity, ref tabName, ref tabGroup, dst);
                } catch (Exception ex) {
                    LogHandlerException(ex, handler);
                }
            }
        }
    }

    public delegate void OnTabListEntryRemoved(Entity entity, Player dst);
    /// <summary> Called when a tab list entry is being removed from a user. </summary>
    public sealed class OnTabListEntryRemovedEvent : IEvent<OnTabListEntryRemoved> {
        
        public static void Call(Entity entity, Player dst) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(entity, dst));
        }
    }
}
