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

namespace MCGalaxy.Events.GroupEvents {
    
    public delegate void GroupLoaded(Group group);
    /// <summary> Raised when a particular group is loaded.</summary>
    public sealed class OnGroupLoadedEvent : IEvent<GroupLoaded> {
        
        public static void Call(Group g) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(g));
        }
    }
    
    public delegate void GroupLoad();
    /// <summary> Raised when list of groups has been loaded or reloaded. </summary>
    public sealed class OnGroupLoadEvent : IEvent<GroupLoad> {
        
        public static void Call() {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl());
        }
    }
    
    public delegate void GroupSave();
    /// <summary> Raised when list of groups has been saved. </summary>
    public sealed class OnGroupSaveEvent : IEvent<GroupSave> {
        
        public static void Call() {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl());
        }
    }
    
    public delegate void OnChangingGroup(string player, Group curRank, Group newRank, ref bool cancel);
    /// <summary> Raised when a player is about to have their rank changed. Cancelable. </summary>
    public sealed class OnChangingGroupEvent : IEvent<OnChangingGroup> {
        
        public static void Call(string player, Group curRank, Group newRank, ref bool cancel) {
            IEvent<OnChangingGroup>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(player, curRank, newRank, ref cancel); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
}
