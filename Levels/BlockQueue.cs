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
                    foreach (Level l in loaded)
                        ProcessLevelBlocks(l);
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

        public static void Addblock(Player p, ushort x, ushort y, ushort z, byte type, byte extType = 0) {
            int index = p.level.PosToInt(x, y, z);
            if (index < 0) return;
            block item;
            
            item.p = p; item.index = index;
            item.type = type; item.extType = extType;
            p.level.blockqueue.Add(item);
        }
        
        static void ProcessLevelBlocks(Level lvl) {
            try {
                if (lvl.blockqueue.Count < 1) return;
                bulkSender.level = lvl;
                int count = blockupdates;
                if (lvl.blockqueue.Count < blockupdates || !lvl.HasPlayers())
                    count = lvl.blockqueue.Count;
                Level.BlockPos bP = default(Level.BlockPos);

                for (int c = 0; c < count; c++) {
                    block item = lvl.blockqueue[c];
                    bP.name = item.p.name;
                    ushort x, y, z;
                    lvl.IntToPos(item.index, out x, out y, out z);
                    
                    bP.index = item.index;
                    bP.SetData(item.type, item.extType, item.type == 0);
                    if (lvl.DoBlockchange(item.p, x, y, z, item.type, item.extType)) {
                        bulkSender.Add(item.index, item.type, item.extType);
                        bulkSender.CheckIfSend(false);
                        lvl.blockCache.Add(bP);
                    }                    
                }
                bulkSender.CheckIfSend(true);
                lvl.blockqueue.RemoveRange(0, count);
            } catch (Exception e)  {
                Server.s.ErrorCase("error:" + e);
                Server.s.Log(String.Format("Block cache failed for map: {0}. {1} lost.", lvl.name, lvl.blockqueue.Count));
                lvl.blockqueue.Clear();
            }
        }

        public struct block { public Player p; public int index; public byte type, extType; }
    }
}
