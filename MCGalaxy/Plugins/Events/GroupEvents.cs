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

namespace MCGalaxy.Events {
    
    public sealed class OnGroupLoadedEvent : IPluginEvent<Group.GroupLoaded> {
        public static void Call(Group g) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(g));
        }
    }
    
    public sealed class OnGroupLoadEvent : IPluginEvent<Group.GroupLoad> {
        public static void Call() {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl());
        }
    }
    
    public sealed class OnGroupSaveEvent : IPluginEvent<Group.GroupSave> {
        public static void Call() {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl());
        }
    }
    
    public sealed class OnPlayerRankSetEvent : IPluginEvent<Group.RankSet> {
        public static void Call(Player p, Group newRank) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p, newRank));
        }
    }
}
