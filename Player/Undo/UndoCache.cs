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
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace MCGalaxy.Util {

    public sealed class UndoCache {
        
        /// <summary> The oldest/first node in the cache. </summary>
        public UndoCacheNode Head;
        
        /// <summary> The newest/last node in the cache. </summary>
        public UndoCacheNode Tail;
        
        /// <summary> Total number of items in the cache. </summary>
        public volatile int Count;
        
        /// <summary> Appends an item to the cache. </summary>
        public void Add(Level lvl, Player.UndoPos item) {
            DateTime time = Server.StartTime.AddSeconds(item.timeDelta);
            if (Head == null) {
                Head = UndoCacheNode.Make(lvl, time);
                Tail = Head;
            }

            if (lvl.name != Tail.MapName || lvl.Width != Tail.Width || lvl.Height != Tail.Height ||
                lvl.Length != Tail.Length || Math.Abs((time - Tail.BaseTime).TotalSeconds) > 32767) {
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
            if( Head == null ) return;
            
            UndoCacheNode node = Head;
            while( node != null ) {
                node.Items.Clear();
                node = node.Next;
            }
            Head = null; Tail = null;
        }
    }
    
    public sealed class UndoCacheNode {
        
        public string MapName;
        public int Width, Height, Length;
        public DateTime BaseTime;
        
        public UndoCacheNode Prev, Next;
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
        public byte Type, ExtType;
        public byte NewType, NewExtType;
        public short TimeDelta;
        
        public static UndoCacheItem Make(UndoCacheNode node, short timeDelta, ref Player.UndoPos pos) {
            UndoCacheItem item = default(UndoCacheItem);
            item.Index = pos.x + node.Width * (pos.z + node.Length * pos.y);
            item.Type = pos.type; item.ExtType = pos.extType;
            item.NewType = pos.newtype; item.NewExtType = pos.newExtType;
            item.TimeDelta = timeDelta;
            return item;
        }
    }
}
