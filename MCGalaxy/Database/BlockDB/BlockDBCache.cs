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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.DB {
    
    /// <summary> Optimised in-memory BlockDB cache. </summary>
    public sealed class BlockDBCache {
        
        public BlockDBCacheNode Tail, Head;
        
        /// <summary> Used to synchronise adding to Cache by multiple threads. </summary>
        public readonly object Locker = new object();
        
        /// <summary> Whether changes are actually added to the BlockDB. </summary>
        public bool Enabled;
        
        /// <summary> Total number of entries in the in-memory BlockDB cache. </summary>
        public int Count;

        /// <summary> Dimensions used to pack coordinates into an index. </summary>
        /// <remarks> May be different from actual level's dimensions, such as when the level has been resized. </remarks>        
        public Vec3U16 Dims;
        
        public void Add(Player p, ushort x, ushort y, ushort z, ushort flags, BlockID old, BlockID block) {
            if (!Enabled) return;
            BlockDBCacheEntry entry;
            entry.Packed = p.DatabaseID << 8;
            int timeDelta = (int)DateTime.UtcNow.Subtract(BlockDB.Epoch).TotalSeconds;
            entry.Index = x + Dims.X * (z + Dims.Z * y);
            
            entry.OldRaw = (byte)old; entry.NewRaw = (byte)block;
            // Bit flags -> bit index
            for (int i = 0; i <= 13; i++) {
                if ((flags & (1 << i)) == 0) continue;
                entry.Packed |= i; break;
            }
            
            // Icky, match extended BlockDBFlags flags >> 4
            entry.Packed |= (old   & (1 << 9)) >> 5;
            entry.Packed |= (block & (1 << 9)) >> 4;
            entry.Packed |= (old   & (1 << 8)) >> 2;
            entry.Packed |= (block & (1 << 8)) >> 1;

            lock (Locker) {
                if (Head == null || Head.Count == Head.Entries.Length 
                    || Math.Abs(timeDelta - Head.BaseTimeDelta) > ushort.MaxValue) {
                    AddNextNode();
                }
                
                // pack the time delta
                entry.TimeDelta = (ushort)Math.Abs(timeDelta - Head.BaseTimeDelta);
                Head.Entries[Head.Count] = entry; 
                Head.Count++; Count++;
            }
        }
        
        public void Clear() {
            lock (Locker) {
                if (Tail == null) return;
                Count = 0;
                
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
    
    public sealed class BlockDBCacheNode {
        
        public BlockDBCacheNode Prev, Next;
        
        /// <summary> The number of actually used entries within this particular node. </summary>
        public int Count;
        
        /// <summary> The base offset time delta for this node, relative to BlockDB.Epoch. </summary>
        public int BaseTimeDelta;
        
        /// <summary> Buffered list of entries, pre-allocated to avoid resizing costs. </summary>
        public BlockDBCacheEntry[] Entries;
        
        public BlockDBCacheNode(int capacity) {
            Entries = new BlockDBCacheEntry[capacity];
            BaseTimeDelta = (int)DateTime.UtcNow.Subtract(BlockDB.Epoch).TotalSeconds;
        }
        
        public BlockDBEntry Unpack(BlockDBCacheEntry cEntry) {
            BlockDBEntry entry;
            entry.PlayerID = (int)((uint)cEntry.Packed >> 8);
            entry.Index = cEntry.Index;            
            entry.TimeDelta = BaseTimeDelta + cEntry.TimeDelta;
            
            entry.OldRaw = cEntry.OldRaw; entry.NewRaw = cEntry.NewRaw;
            entry.Flags = (ushort)(1 << (cEntry.Packed & 0x0F));
            entry.Flags |= (ushort)((cEntry.Packed & 0xF0) << 8);
            return entry;
        }
    }
}
