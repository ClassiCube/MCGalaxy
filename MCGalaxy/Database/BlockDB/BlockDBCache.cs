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

namespace MCGalaxy.DB {
    
    /// <summary> Optimised in-memory BlockDB cache. </summary>
    public sealed class BlockDBCache {
        
        public BlockDBCacheNode Tail, Head;
        
        /// <summary> Used to synchronise adding to Cache by multiple threads. </summary>
        public readonly object Locker = new object();
        
        public void Add(ref BlockDBEntry entry) {
            lock (Locker) {
                if (Head == null || Head.Count == Head.Entries.Length)
                    AddNextNode();
                
                Head.Entries[Head.Count] = entry; Head.Count++;
            }
        }
        
        public void Clear() {
            lock (Locker) {
                if (Tail == null) return;
                
                BlockDBCacheNode cur = Tail;
                while (cur != null) {
                    // Unlink the nodes
                    cur.Prev = null;                   
                    BlockDBCacheNode next = cur.Next;
                    cur.Next = null;
                    cur = next;
                }
                Head = null; Tail = null;
            }
        }
        
        void AddNextNode() {
            BlockDBCacheNode newHead = new BlockDBCacheNode(nextSize);
            newHead.Prev = Head;
            if (Head != null) Head.Next = newHead;
            Head = newHead;
            if (Tail == null) Tail = Head;
            
            // use smaller increases at first to minimise memory usage
            if (nextSize == 50 * 1000) nextSize = 100 * 1000;
            if (nextSize == 20 * 1000) nextSize = 50 * 1000;
            if (nextSize == 10 * 1000) nextSize = 20 * 1000;
        }
        
        int nextSize = 10 * 1000;
    }
    
    // TODO: track start time so we can use int16 instead of int32 time delta
    public sealed class BlockDBCacheNode {
        
        public BlockDBCacheNode Prev, Next;
        
        public int Count;
        
        public BlockDBEntry[] Entries;
        
        public BlockDBCacheNode(int capacity) {
            Entries = new BlockDBEntry[capacity];
        }
    }
}
