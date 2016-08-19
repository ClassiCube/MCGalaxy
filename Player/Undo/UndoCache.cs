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
using System.Runtime.InteropServices;
using System.Threading;

namespace MCGalaxy.Util {

    public sealed class UndoCache {

        /// <summary> The olded/first node in the cache. </summary>
        public UndoCacheNode Head;
        
        /// <summary> The newest/last node in the cache. </summary>
        public UndoCacheNode Tail;
        
        /// <summary> Total number of items in the cache. </summary>
        public volatile int Count;
        
        /// <summary> Last time this undo buffer was cleared. </summary>
        public DateTime LastClear;
        
        /// <summary> Used to sychronise clearing an undo cache. </summary>
        public ReaderWriterLockSlim ClearLock = new ReaderWriterLockSlim();
        
        public const int TimeDeltaMax = (1 << 13) - 1;
        
        /// <summary> Appends an item to the cache. </summary>
        public void Add(Level lvl, Player.UndoPos item) {
            DateTime time = Server.StartTime.AddTicks(item.timeDelta * TimeSpan.TicksPerSecond);
            if (Tail == null) {
                Tail = UndoCacheNode.Make(lvl, time); Head = Tail;
            }

            if (lvl.name != Tail.MapName || lvl.Width != Tail.Width || lvl.Height != Tail.Height ||
                lvl.Length != Tail.Length || Math.Abs((time - Tail.BaseTime).TotalSeconds) > TimeDeltaMax) {
                UndoCacheNode node = UndoCacheNode.Make(lvl, time);
                Tail.Next = node; node.Prev = Tail;
                Tail = node;
            }
            
            short timeDiff = (short)(time - Tail.BaseTime).TotalSeconds;
            Tail.Items.Add(UndoCacheItem.Make(Tail, timeDiff, ref item));
            Count++;
        }
        
        /// <summary> Removes all items from the cache and resets the state to default. </summary>
        public void Clear() {
            Count = 0;
            if (Tail == null) return;
            LastClear = DateTime.UtcNow;
            
            UndoCacheNode node = Tail;
            while (node != null) {
                node.Items = null;
                node = node.Prev;
            }
            Tail = null; Head = null;
        }
    }
    
    public sealed class UndoCacheNode {
        
        public string MapName;
        public int Width, Height, Length;
        public DateTime BaseTime;
        
        public UndoCacheNode Prev;
        public List<UndoCacheItem> Items = new List<UndoCacheItem>();
        
        public static UndoCacheNode Make(Level lvl, DateTime time) {
            UndoCacheNode node = new UndoCacheNode();
            node.MapName = lvl.name;
            node.Width = lvl.Width; node.Height = lvl.Height; node.Length = lvl.Length;
            node.BaseTime = time;
            return node;
        }
        
        public void Unpack(int index, out ushort x, out ushort y, out ushort z) {
            x = (ushort)(index % Width);
            y = (ushort)(index / (Width * Length));
            z = (ushort)((index / Width) % Length);
        }
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UndoCacheItem {
        public int Index;
        public byte Type, NewType;
        public ushort Flags; // upper 2 bits for 'ext' or 'physics' type, lower 14 bits for time delta.
        
        public short TimeDelta {
            get {
                int delta = Flags & 0x3FFF;
                return delta >= 0x2000 ? (short)(delta - 16384) : (short)delta;
            }
        }
        
        public void GetBlock(out byte type, out byte extType) {
            if ((Flags & (1 << 14)) != 0) {
                type = Block.custom_block; extType = Type;
            } else {
                type = Type; extType = 0;
            }
        }
        
        public void GetNewBlock(out byte type, out byte extType) {
            if ((Flags & (1 << 15)) != 0) {
                type = Block.custom_block; extType = NewType;
            } else {
                type = NewType; extType = 0;
            }
        }
        
        public static UndoCacheItem Make(UndoCacheNode node, short timeDelta, ref Player.UndoPos pos) {
            UndoCacheItem item = default(UndoCacheItem);
            item.Index = pos.x + node.Width * (pos.z + node.Length * pos.y);
            item.Flags = (ushort)(timeDelta & 0x3FFF);
            
            if (pos.type == Block.custom_block) {
                item.Type = pos.extType;
                item.Flags |= (ushort)(1 << 14);
            } else {
                item.Type = pos.type;
            }
            if (pos.newtype == Block.custom_block) {
                item.NewType = pos.newExtType;
                item.Flags |= (ushort)(1 << 15);
            } else {
                item.NewType = pos.newtype;
            }
            return item;
        }
        
        public static UndoCacheItem Make(UndoCacheNode node, short timeDelta, Player.UndoPos pos) {
            return Make(node, timeDelta, ref pos);
        }
    }
    
    public sealed class UndoDrawOpEntry {
        public string DrawOpName;
        public string LevelName;
        public DateTime Start, End;
    }
}
