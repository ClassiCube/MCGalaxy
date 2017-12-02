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
    public sealed class VolatileArray<T> where T : class {

        /// <remarks> Note this field is highly volatile, you should cache references to it. </remarks>
        public volatile T[] Items = new T[0];
        
        public int Count { get { return Items.Length; } }
        
        /// <summary> Object used to sychronise Add/Remove calls to this array. </summary>
        /// <remarks> When locking on this object from external code, you should try
        /// to minimise the amount of time the object is locked for. </remarks>
        public readonly object locker = new object();
        
        public List<T> list;
        
        readonly bool useList;
        
        public VolatileArray(bool useList = false) {
            this.useList = useList;
            if (useList) list = new List<T>();
        }
        
        public bool Add(T value) {
            lock (locker) {
                T[] newItems = new T[Items.Length + 1];
                for (int i = 0; i < Items.Length; i++) {
                    if (object.ReferenceEquals(Items[i], value)) return false;
                    newItems[i] = Items[i];
                }

                if (useList) list.Add(value);
                newItems[Items.Length] = value;
                Items = newItems;
            }
            return true;
        }
        
        public bool Contains(T value) {
            lock (locker) {
                for (int i = 0; i < Items.Length; i++) {
                    if (object.ReferenceEquals(Items[i], value)) return true;
                }
            }
            return false;
        }
        
        public bool Remove(T value) {
            lock (locker) {
                if (Items.Length == 0) return false;
                if (useList) list.Remove(value);
                
                T[] newItems = new T[Items.Length - 1];
                int j = 0;
                for (int i = 0; i < Items.Length; i++) {
                    if (object.ReferenceEquals(Items[i], value)) continue;
                    
                    // For some reason item wasn't in the list
                    if (j == newItems.Length) return false;
                    newItems[j] = Items[i]; j++;
                }
                
                // Handle very rare case when an item has been added twice
                if (newItems.Length != j) {
                    T[] temp = new T[j];
                    for (int i = 0; i < temp.Length; i++)
                        temp[i] = newItems[i];
                    Items = temp;
                } else {
                    Items = newItems;
                }
            }
            return true;
        }
        
        public bool RemoveFirst() {
            lock (locker) {
                if (Items.Length == 0) return false;
                if (useList) list.RemoveAt(0);
                
                T[] newItems = new T[Items.Length - 1];
                for (int i = 1; i < Items.Length; i++)
                    newItems[i - 1] = Items[i];
                Items = newItems;
            }
            return true;
        }
        
        public void Clear() {
            lock (locker) {
                if (useList) list.Clear();
                Items = new T[0];
            }
        }
    }
}
