/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using MCGalaxy.Eco;

namespace MCGalaxy.Events {
    
    /// <summary> Raised whenever a player's online money changes. </summary>
    public sealed class OnMoneyChangedEvent : IPluginEvent<Economy.OnMoneyChanged> {
        internal OnMoneyChangedEvent(Economy.OnMoneyChanged method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(Player p) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(p));
        }
    }
    
    /// <summary> Raised whenever an economic transaction occurs. </summary>
    public sealed class OnEcoTransactionEvent : IPluginEvent<Economy.OnEcoTransaction> {        
        internal OnEcoTransactionEvent(Economy.OnEcoTransaction method, Priority priority, Plugin plugin)
            : base(method, priority, plugin) { }
        
        public static void Call(EcoTransaction transaction) {
            if (handlers.Count == 0) return;
            CallImpl(pl => pl(transaction));
        }
    }
}
