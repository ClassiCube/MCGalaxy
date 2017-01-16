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
        
        /// <summary> Whether changes are actually added to the BlockDB. </summary>
        public bool Enabled;
        
        /// <summary> Total number of entries in the in-memory BlockDB cache. </summary>
        public int Count;

        /// <summary> Dimensions used to pack coordinates into an index. </summary>
        /// <remarks> May be different from actual level's dimensions, such as when the level has been resized. </remarks>        
        public Vec3U16 Dims;
        
        public void Add(Player p, ushort x, ushort y, ushort z, ushort flags,
                        byte oldBlock, byte oldExt, byte block, byte ext) {
            if (!Enabled) return;
            BlockDBCacheEntry entry;
            entry.Packed1 = (uint)p.UserID;
            int timeDelta = (int)DateTime.UtcNow.Subtract(BlockDB.Epoch).TotalSeconds;
            entry.Index = x + Dims.X * (z + Dims.Z * y);
            
            entry.OldRaw = oldBlock; entry.NewRaw = block;
            entry.Flags = flags;
            
            if (block == Block.custom_block) {
                entry.Flags |= BlockDBFlags.NewCustom;
                entry.NewRaw = ext;
            }
            if (oldBlock == Block.custom_block) {
                entry.Flags |= BlockDBFlags.OldCustom;
                entry.OldRaw = oldExt;
            }

            lock (Locker) {
                const int maxTime = (1 << 11); // 11 bits for time
                if (Head == null || Head.Count == Head.Entries.Length 
                    || Math.Abs(timeDelta - Head.BaseTimeDelta) >= maxTime) {
                    AddNextNode();
                }
                
                // pack the time delta
                int delta = Math.Abs(timeDelta - Head.BaseTimeDelta);
                entry.Packed1 |= (uint)(delta & 0xFF) << 24;
                entry.Flags |= (ushort)((delta & 0x700) << 3);
                
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
        
        const int idMask = (1 << 24) - 1, idShift = 24;
        public BlockDBEntry Unpack(BlockDBCacheEntry cEntry) {
            BlockDBEntry entry;
            entry.PlayerID = (int)(cEntry.Packed1 & idMask);
            entry.Index = cEntry.Index;
            entry.NewRaw = cEntry.NewRaw;
            entry.OldRaw = cEntry.OldRaw;
            
            const int hiTimeBits = 7 << 11;
            entry.Flags = (ushort)(cEntry.Flags & ~hiTimeBits); // hi 3 bits for time
            entry.TimeDelta = BaseTimeDelta;
            
            // offset from base delta
            entry.TimeDelta += (int)(cEntry.Packed1 >> idShift); // lo 8 bits for time
            entry.TimeDelta += (cEntry.Flags & hiTimeBits) >> 3; // hi 3 bits for time
            return entry;
        }
    }
}
