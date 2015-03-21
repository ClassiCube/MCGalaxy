/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy
{
    public static class BlockQueue
    {
        public static int time { get { return (int)blocktimer.Interval; } set { blocktimer.Interval = value; } }
        public static int blockupdates = 200;
        static block b = new block();
        static System.Timers.Timer blocktimer = new System.Timers.Timer(100);
        static byte started = 0;

        public static void Start()
        {
            blocktimer.Elapsed += delegate
            {
                if (started == 1) return;
                started = 1;
                Server.levels.ForEach((l) =>
                {
                    try
                    {
                        if (l.blockqueue.Count < 1) return;
                        int count;
                        if (l.blockqueue.Count < blockupdates || l.players.Count == 0) count = l.blockqueue.Count;
                        else count = blockupdates;

                        for (int c = 0; c < count; c++)
                        {
                            l.Blockchange(l.blockqueue[c].p, l.blockqueue[c].x, l.blockqueue[c].y, l.blockqueue[c].z, l.blockqueue[c].type);
                        }
                        l.blockqueue.RemoveRange(0, count);
                    }
                    catch (Exception e)
                    {
                        Server.s.ErrorCase("error:" + e);
                        Server.s.Log(String.Format("Block cache failed for map: {0}. {1} lost.", l.name, l.blockqueue.Count));
                        l.blockqueue.Clear();
                    }
                });
                started = 0;
            };
            blocktimer.Start();
        }
        public static void pause() { blocktimer.Enabled = false; }
        public static void resume() { blocktimer.Enabled = true; }

        public static void Addblock(Player P, ushort X, ushort Y, ushort Z, byte Type)
        {
            b.x = X; b.y = Y; b.z = Z; b.type = Type; b.p = P;
            P.level.blockqueue.Add(b);
        }

        public struct block { public Player p; public ushort x; public ushort y; public ushort z; public byte type; }
    }
}
