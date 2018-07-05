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
using MCGalaxy.Games;

namespace MCGalaxy.Events.GameEvents {
    
    public delegate void OnStateChanged(IGame game);
    /// <summary> Raised when state of a game changed (started, stopped, round). </summary>
    public sealed class OnStateChangedEvent : IEvent<OnStateChanged> {
        
        public static void Call(IGame game) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(game));
        }
    }
    
    public delegate void OnMapsChanged(RoundsGame game);
    /// <summary> Raised when maps list in a game changes. </summary>
    public sealed class OnMapsChangedEvent : IEvent<OnMapsChanged> {
        
        public static void Call(RoundsGame game) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(game));
        }
    }
}
