/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

        public static void Start() {
            blocktimer.Elapsed += delegate {
                if (started) return;
                started = true;
                try {
                    Server.levels.ForEach(l => ProcessLevelBlocks(l));
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
        
        static void ProcessLevelBlocks(Level l) {
            try {
                if (l.blockqueue.Count < 1) return;
                int count = blockupdates;
                if (l.blockqueue.Count < blockupdates || l.players.Count == 0)
                    count = l.blockqueue.Count;
                Level.BlockPos bP;

                for (int c = 0; c < count; c++) {
                    block item = l.blockqueue[c];
                    bP.name = item.p.name;
                    bP.timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
                    ushort x, y, z;
                    l.IntToPos(item.index, out x, out y, out z);
                    
                    bP.index = item.index;
                    bP.type = item.type;
                    bP.extType = item.extType;
                    bP.deleted = bP.type == 0;
                    l.Blockchange(item.p, x, y, z, bP.type, bP.extType);
                    l.blockCache.Add(bP);
                }
                l.blockqueue.RemoveRange(0, count);
            } catch (Exception e)  {
                Server.s.ErrorCase("error:" + e);
                Server.s.Log(String.Format("Block cache failed for map: {0}. {1} lost.", l.name, l.blockqueue.Count));
                l.blockqueue.Clear();
            }
        }

        public struct block { public Player p; public int index; public byte type, extType; }
    }
}
