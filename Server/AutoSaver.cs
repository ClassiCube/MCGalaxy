/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MCGalaxy
{
    public sealed class AutoSaver
    {
        static int _interval;
        string backupPath = @Server.backupLocation;

        static int count = 1;
        public AutoSaver(int interval) {
            _interval = interval * 1000;
            Thread t = new Thread(AutoSaveLoop);
            t.Name = "MCG_AutoSaver";
            t.Start();
        }
        
        static void AutoSaveLoop() {
            while (true) {
                Thread.Sleep(_interval);
                Server.ml.Queue(Run);

                Player[] players = PlayerInfo.Online.Items;
                if (players.Length <= 0) continue;
                string allCount = players.Aggregate("", (current, pl) => current + (", " + pl.name));
                try { Server.s.Log("!PLAYERS ONLINE: " + allCount.Remove(0, 2), true); }
                catch { }

                Level[] levels = LevelInfo.Loaded.Items;
                allCount = levels.Aggregate("", (current, l) => current + (", " + l.name));
                try { Server.s.Log("!LEVELS ONLINE: " + allCount.Remove(0, 2), true); }
                catch { }
            }
        }

        public static void Run()
        {
            try
            {
                count--;
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level l in loaded) {
                    try
                    {
                    	if (!l.changed || !l.ShouldSaveChanges()) return;

                        l.Save();
                        if (count == 0)
                        {
                            int backupNumber = l.Backup();
                            if (backupNumber != -1)
                            {
                                l.ChatLevel("Backup " + backupNumber + " saved.");
                                Server.s.Log("Backup " + backupNumber + " saved for " + l.name);
                            }
                        }
                    }
                    catch
                    {
                        Server.s.Log("Backup for " + l.name + " has caused an error.");
                    }
                }

                if (count <= 0)
                {
                    count = 15;
                }
            } catch (Exception e) {
                Server.ErrorLog(e);
            }

            try {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player p in players) { p.save(); }
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
        }
    }
}
