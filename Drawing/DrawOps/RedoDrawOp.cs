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
using MCGalaxy.BlockPhysics;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Util;

namespace MCGalaxy.Drawing.Ops {
    
    public class RedoSelfDrawOp : DrawOp {
        public override string Name { get { return "RedoSelf"; } }
        
        /// <summary> Point in time that the /undo should go backwards up to. </summary>
        public DateTime Start = DateTime.MinValue;
        
        /// <summary> Point in time that the /undo should start updating blocks. </summary>
        public DateTime End = DateTime.MaxValue;
        
        public override long GetBlocksAffected(Level lvl, Vec3S32[] marks) { return -1; }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
            UndoCache cache = p.UndoBuffer;
            using (IDisposable locker = cache.ClearLock.AccquireReadLock()) {
                RedoBlocks(p);
            }
            yield break;
        }
        
        void RedoBlocks(Player p) {
            UndoCache cache = p.UndoBuffer;
            UndoCacheNode node = p.UndoBuffer.Tail;
            if (node == null) return;
            
            while (node != null) {
                Level lvl = LevelInfo.FindExact(node.MapName);
                if (lvl == null || (p.level != null && !p.level.name.CaselessEq(lvl.name))) {
                    node = node.Prev; continue;
                }
                List<UndoCacheItem> items = node.Items;
                BufferedBlockSender buffer = new BufferedBlockSender(lvl);
                
                for (int i = items.Count - 1; i >= 0; i--) {
                    UndoCacheItem item = items[i];
                    ushort x, y, z;
                    node.Unpack(item.Index, out x, out y, out z);
                    
                    DateTime time = node.BaseTime.AddTicks(item.TimeDelta * TimeSpan.TicksPerSecond);
                    if (time > End) continue;
                    if (time < Start) { buffer.CheckIfSend(true); return; }
                    
                    byte tile, extTile;
                    item.GetBlock(out tile, out extTile);
                    if (lvl.DoBlockchange(p, x, y, z, tile, extTile, true)) {
                        buffer.Add(lvl.PosToInt(x, y, z), tile, extTile);
                        buffer.CheckIfSend(false);
                    }
                }
                buffer.CheckIfSend(true);
                node = node.Prev;
            }
            return;
        }
    }
}
