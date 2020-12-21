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

namespace MCGalaxy.Util {
	
    /// <summary> A faster alternative to List&lt;T&gt; that does no error checking </summary>
    public class FastList<T> {
        
        public T[] Items;
        public int Count;
        
        public FastList() { Items = new T[4]; }

        public FastList(int capacity) { Items = new T[capacity]; }

        public void Add(T item) {
            if (Count == Items.Length) EnsureCapacity(Count + 1);
            Items[Count++] = item;
        }
        
        public void Clear() {
            if (Count == 0) return;
            Array.Clear(Items, 0, Count);
            Count = 0;
        }
        
        public void RemoveAt(int index) {
            Count--;
            if (index < Count)
                Array.Copy(Items, index + 1, Items, index, Count - index);
            Items[Count] = default(T);
        }
        
        void EnsureCapacity(int threshold) {
            if (Items.Length >= threshold) return;
            int newSize = Items.Length * 2;
            if (newSize < threshold) newSize = threshold;
            
            T[] array = new T[newSize];
            if (Count > 0)
                Array.Copy(Items, 0, array, 0, Count);
            Items = array;
        }
    }
}
