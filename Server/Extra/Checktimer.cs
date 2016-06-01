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
permiusing MCGalaxy;ssions and limitations under the Licenses.
 */
﻿using System;
using System.IO;

namespace MCGalaxy {
    
    public static class Checktimer {
        
        static System.Timers.Timer t;
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        public static void StartTimer() {
            t = new System.Timers.Timer();
            t.AutoReset = false;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Interval = GetInterval();
            t.Start();
        }
        
        static double GetInterval() {
            DateTime now = DateTime.Now;
            return ((60 - now.Second) * 1000 - now.Millisecond);
        }
        
        static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            t.Interval = GetInterval();
            t.Start();
            TRExpiryCheck(); // every 60 seconds
        }
        
        public static void TRExpiryCheck() {
            Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) {
                foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                    if (!line.StartsWith(p.name, comp)) continue;
                    string[] args = line.Split(' ');

                    int period = Convert.ToInt32(args[3]);
                    int minutes = Convert.ToInt32(args[4]);
                    int hours = Convert.ToInt32(args[5]);
                    int days = Convert.ToInt32(args[6]);
                    int months = Convert.ToInt32(args[7]);
                    int years = Convert.ToInt32(args[8]);
                    
                    DateTime expire = new DateTime(years, months, days, hours, minutes, 0)
                        .AddHours(Convert.ToDouble(period));
                    if (DateTime.Now >= expire)
                        Command.all.Find("temprank").Use(null, p.name + " delete");
                }
            }
        }
    }
}