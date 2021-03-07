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

namespace MCGalaxy.Events.PlayerDBEvents {
    
    public delegate void OnInfoSave(Player p, ref bool cancel);
    /// <summary> Called whenever the server saves player's stats to the database. </summary>
    public sealed class OnInfoSaveEvent : IEvent<OnInfoSave> {
        
    	public static void Call(Player p, ref bool cancel) {
            IEvent<OnInfoSave>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) {
                try { items[i].method(p, ref cancel); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
        
    public delegate void OnInfoSwap(string src, string dst);
    /// <summary> Called when the information of two players is being swapped. </summary>
    public sealed class OnInfoSwapEvent : IEvent<OnInfoSwap> {
        
        public static void Call(string src, string dst) {
            if (handlers.Count == 0) return;
            CallCommon(pl => pl(src, dst));
        }        
    }
}
