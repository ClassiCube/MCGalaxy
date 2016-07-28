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
using System.Collections.Generic;

namespace MCGalaxy {
    public sealed class ThreadSafeCache {
        static readonly List<ThreadSafeCache> caches = new List<ThreadSafeCache>();
        static readonly object cachesLock = new object();
        
        public static ThreadSafeCache DBCache = new ThreadSafeCache(() => new object());
        
        readonly object locker = new object();
        readonly Dictionary<string, object> items = new Dictionary<string, object>();
        readonly Dictionary<string, DateTime> access = new Dictionary<string, DateTime>();
        readonly Func<object> constructor;
        
        public ThreadSafeCache(Func<object> constructor) {
            this.constructor = constructor;
            lock (cachesLock)
                caches.Add(this);
        }
        
        public object Get(string key) {
            lock (locker) {
                object value;
                if (!items.TryGetValue(key, out value)) {
                    value = constructor();
                    items[key] = value;
                }
                
                access[key] = DateTime.UtcNow;
                return value;
            }
        }
        
        
        internal static void CleanupTask(SchedulerTask task) {
            lock (cachesLock) {
                foreach (ThreadSafeCache cache in caches)
                    cache.CleanupOld();
            }
        }
        
        void CleanupOld() {
            List<string> free = null;
            DateTime now = DateTime.UtcNow;
            
            lock (locker) {
                foreach (var kvp in access) {
                    // Has the cached item last been accessed in 5 minutes?
                    if ((now - kvp.Value).TotalMinutes <= 5) continue;
                    
                    if (free == null) free = new List<string>();
                    free.Add(kvp.Key);
                }
                
                if (free == null) return;
                foreach (string key in free) {
                    items.Remove(key);
                    access.Remove(key);
                }
            }
        }
    }
}
