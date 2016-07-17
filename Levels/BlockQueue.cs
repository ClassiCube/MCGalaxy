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
namespace MCGalaxy {
    
    public static class BlockQueue {
        
        public static int time { get { return (int)blocktimer.Interval; } set { blocktimer.Interval = value; } }
        public static int blockupdates = 250;
        static System.Timers.Timer blocktimer = new System.Timers.Timer(100);
        static bool started = false;
        static BufferedBlockSender bulkSender = new BufferedBlockSender(null);

        public static void Start() {
            blocktimer.Elapsed += delegate {
                if (started) return;
                started = true;
                try {
                    Level[] loaded = LevelInfo.Loaded.Items;
                    foreach (Level lvl in loaded) {
                        lock (lvl.queueLock)
                            ProcessLevelBlocks(lvl);
                    }
                    bulkSender.level = null;
                } catch (Exception ex) {
                    Server.ErrorLog(ex);
                    throw;
                } finally {
                    started = false;
                }
            };
            blocktimer.Start();
        }
        
        public static void Pause() { blocktimer.Enabled = false; }
        public static void Resume() { blocktimer.Enabled = true; }

        public static void Addblock(Player p, int index, byte type, byte extType = 0) {
            if (index == -1) return;
            // Bit packing format
            // 32-63: index
            // 9-31: session ID
            // 8: is ext block or not
            // 0-7: raw type
            ulong flags = (ulong)index << 32;
            flags |= (ulong)p.SessionID << 9;
            flags |= (type == Block.custom_block ? 0x100UL : 0x000UL);
            flags |= (type == Block.custom_block ? extType : type);
            
            lock (p.level.queueLock)
                p.level.blockqueue.Add(flags);
        }
        
        static void ProcessLevelBlocks(Level lvl) {
            try {
                if (lvl.blockqueue.Count < 1) return;
                bulkSender.level = lvl;
                int count = blockupdates;
                if (lvl.blockqueue.Count < blockupdates || !lvl.HasPlayers())
                    count = lvl.blockqueue.Count;

                for (int i = 0; i < count; i++) {
                    ulong flags = lvl.blockqueue[i];
                    int index = (int)(flags >> 32);                    
                    byte type = (flags & 0x100) != 0 ? Block.custom_block : (byte)flags;
                    byte extType = (flags & 0x100) != 0 ? (byte)flags : Block.air;
                    
                    bulkSender.Add(index, type, extType);
                    bulkSender.CheckIfSend(false);
                }
                bulkSender.CheckIfSend(true);
                lvl.blockqueue.RemoveRange(0, count);
            } catch (Exception e)  {
                Server.s.ErrorCase("error:" + e);
                Server.s.Log(String.Format("Block cache failed for map: {0}. {1} lost.", lvl.name, lvl.blockqueue.Count));
                lvl.blockqueue.Clear();
            }
        }
    }
}
