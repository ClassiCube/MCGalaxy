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

namespace MCGalaxy {
    
    public static class BlockQueue {
        
        public static int time = 100;
        public static int blockupdates = 250;
        static BufferedBlockSender bulkSender = new BufferedBlockSender();

        public static void Loop(SchedulerTask task) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                lock (lvl.queueLock)
                    ProcessLevelBlocks(lvl);
            }
            
            bulkSender.level = null;
            task.Delay = TimeSpan.FromMilliseconds(time);
        }

        public static void Addblock(Player p, int index, ExtBlock block) {
            if (index == -1) return;
            // Bit packing format
            // 32-63: index
            // 9-31: session ID
            // 8: is ext block or not
            // 0-7: raw type
            ulong flags = (ulong)index << 32;
            flags |= (ulong)p.SessionID << 9;
            flags |= (block.BlockID == Block.custom_block ? 0x100UL : 0x000UL);
            flags |= (block.BlockID == Block.custom_block ? block.ExtID : block.BlockID);
            
            lock (p.level.queueLock)
                p.level.blockqueue.Add(flags);
        }
        
        static void ProcessLevelBlocks(Level lvl) {
            try {
                if (lvl.blockqueue.Count == 0) return;
                if (!lvl.HasPlayers()) { lvl.blockqueue.Clear(); return; }
                    
                bulkSender.level = lvl;
                int count = blockupdates;
                if (lvl.blockqueue.Count < blockupdates)
                    count = lvl.blockqueue.Count;

                for (int i = 0; i < count; i++) {
                    ulong flags = lvl.blockqueue[i];
                    int index = (int)(flags >> 32);
                    byte block = (flags & 0x100) != 0 ? Block.custom_block : (byte)flags;
                    byte extBlock = (flags & 0x100) != 0 ? (byte)flags : Block.air;
                    bulkSender.Add(index, block, extBlock);
                }
                bulkSender.Send(true);
                lvl.blockqueue.RemoveRange(0, count);
            } catch (Exception e)  {
                Server.s.ErrorCase("error:" + e);
                Server.s.Log(String.Format("Block cache failed for map: {0}. {1} lost.", lvl.name, lvl.blockqueue.Count));
                lvl.blockqueue.Clear();
            }
        }
    }
}
