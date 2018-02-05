/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Network;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy {
    
    public static class BlockQueue {
        
        public static int Interval = 100;
        public static int UpdatesPerTick = 750;
        static BufferedBlockSender bulkSender = new BufferedBlockSender();
        public const int BlockMask = 0x1FF;

        public static void Loop(SchedulerTask task) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                lock (lvl.queueLock)
                    ProcessLevelBlocks(lvl);
            }
            
            bulkSender.level = null;
            task.Delay = TimeSpan.FromMilliseconds(Interval);
        }

        public static void Add(Player p, int index, BlockID block) {
            if (index == -1) return;
            // Bit packing format
            // 32-63: index
            // 9-31: session ID
            // 8: is ext block or not
            // 0-7: raw type
            ulong flags = (ulong)index << 32;
            flags |= (ulong)p.SessionID << 9;
            flags |= (ulong)block & BlockMask;
            
            lock (p.level.queueLock)
                p.level.blockqueue.Add(flags);
        }
        
        static void ProcessLevelBlocks(Level lvl) {
            try {
                if (lvl.blockqueue.Count == 0) return;
                if (!lvl.HasPlayers()) { lvl.blockqueue.Clear(); return; }
                    
                bulkSender.level = lvl;
                int count = UpdatesPerTick;
                if (lvl.blockqueue.Count < UpdatesPerTick)
                    count = lvl.blockqueue.Count;

                for (int i = 0; i < count; i++) {
                    ulong flags = lvl.blockqueue[i];
                    int index = (int)(flags >> 32);
                    BlockID block = (BlockID)(flags & BlockMask);
                    bulkSender.Add(index, block);
                }
                bulkSender.Send(true);
                lvl.blockqueue.RemoveRange(0, count);
            } catch (Exception e)  {
                Logger.LogError(e);
                Logger.Log(LogType.Warning, "Block cache failed for map: {0}. {1} lost.", lvl.name, lvl.blockqueue.Count);
                lvl.blockqueue.Clear();
            }
        }
    }
}
