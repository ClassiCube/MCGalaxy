/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.DB;

namespace MCGalaxy.Events.BlockDBEvents 
{    
    public delegate void OnBlockDBSave(BlockDB db, ref string path, ref bool cancel);
    /// <summary> Called whenever a BlockDB is being flushed from memory to disc </summary>
    public sealed class OnBlockDBSaveEvent : IEvent<OnBlockDBSave> 
    {       
        public static void Call(BlockDB db, ref string path, ref bool cancel) {
            IEvent<OnBlockDBSave>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) 
            {
                try { items[i].method(db, ref path, ref cancel); } 
                catch (Exception ex) { LogHandlerException(ex, items[i]); }
            }
        }
    }
}
