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
    public sealed class VolatileArray<T> {

        /// <remarks> Note this field is highly volatile, you should cache references to it. </remarks>
        public volatile T[] Items = new T[0];
        
        /// <summary> Returns the number of items in this array. </summary>
        public int Count { get { return Items.Length; } }
        
        /// <summary> Object used to sychronise Add/Remove calls to this array. </summary>
        /// <remarks> When locking on this object from external code, you should try
        /// to minimise the amount of time the object is locked for. </remarks>
        public readonly object locker = new object();
        
        public List<T> list;
        
        readonly bool useList;
        
        public VolatileArray(bool useList) {
            this.useList = useList;
            if (useList) list = new List<T>();
        }
        
        public void Add(T value) {
            lock (locker) {
                if (useList) list.Add(value);
                T[] newItems = new T[Items.Length + 1];
                for (int i = 0; i < Items.Length; i++)
                    newItems[i] = Items[i];
                
                newItems[Items.Length] = value;
                Items = newItems;
            }
        }
        
        public void Remove(T value) {
            lock (locker) {
                if (useList) list.Remove(value);
                if (Items.Length == 0) return;
                
                T[] newItems = new T[Items.Length - 1];
                int j = 0;
                for (int i = 0; i < Items.Length; i++) {
                    if (object.ReferenceEquals(Items[i], value)) continue;
                    
                    // For some reason item wasn't in the list
                    if (j == newItems.Length) return;
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
        }
        
        public void RemoveFirst() {
            lock (locker) {
                if (useList) list.RemoveAt(0);
                if (Items.Length == 0) return;
                
                T[] newItems = new T[Items.Length - 1];
                for (int i = 1; i < Items.Length; i++)
                    newItems[i - 1] = Items[i];
                Items = newItems;
            }
        }
        
        public void Clear() {
            lock (locker) {
                if (useList) list.Clear();
                Items = new T[0];
            }
        }
    }
}
