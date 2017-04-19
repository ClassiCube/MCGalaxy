/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.IO;
using System.Threading;

namespace MCGalaxy.Tasks {    
    internal static class ServerTasks {

        internal static void LocationChecks() {
            while (true) {
                Player[] players = PlayerInfo.Online.Items;
                Thread.Sleep(players.Length == 0 ? 20 : 10);
                players = PlayerInfo.Online.Items;
                
                for (int i = 0; i < players.Length; i++) {
                    try {
                        Player p = players[i];

                        if (p.following != "") {
                            Player who = PlayerInfo.FindExact(p.following);
                            if (who == null || who.level != p.level) {
                                p.following = "";
                                if (!p.canBuild)
                                    p.canBuild = true;
                                if (who != null && who.possess == p.name)
                                    who.possess = "";
                                continue;
                            }
                            
                            p.SendPos(Entities.SelfID, who.Pos, who.Rot);
                        } else if (p.possess != "") {
                            Player who = PlayerInfo.FindExact(p.possess);
                            if (who == null || who.level != p.level)
                                p.possess = "";
                        }
                        
                        Vec3U16 P = (Vec3U16)p.Pos.BlockCoords;
                        if (p.level.Death)
                            p.CheckSurvival(P.X, P.Y, P.Z);
                        p.CheckBlock();
                        p.oldIndex = p.level.PosToInt(P.X, P.Y, P.Z);
                    } catch (Exception e) {
                        Server.ErrorLog(e);
                    }
                }
            }
        }

        internal static void BlockUpdates(SchedulerTask task) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                try {
                    lvl.saveChanges();
                } catch (Exception e) {
                    Server.ErrorLog(e);
                }
            }
        }
        
        internal static void AutoSave(SchedulerTask task) {
            int count = (int)task.State;
            count--;
            Level[] levels = LevelInfo.Loaded.Items;
            
            foreach (Level l in levels) {
                try {
                    if (!l.changed || !l.ShouldSaveChanges()) continue;

                    l.Save();
                    if (count == 0)  {
                        int backupNumber = l.Backup();
                        if (backupNumber != -1) {
                            l.ChatLevel("Backup " + backupNumber + " saved.");
                            Server.s.Log("Backup " + backupNumber + " saved for " + l.name);
                        }
                    }
                } catch {
                    Server.s.Log("Backup for " + l.name + " has caused an error.");
                }
            }

            if (count <= 0) count = 15;
            task.State = count;

            Player[] players = PlayerInfo.Online.Items;
            try {
                foreach (Player p in players) p.save();
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
            
            players = PlayerInfo.Online.Items;
            if (players.Length <= 0) return;
            string all = players.Join(p => p.name);
            if (all.Length > 0) Server.s.Log("!PLAYERS ONLINE: " + all, true);

            levels = LevelInfo.Loaded.Items;
            all = levels.Join(l => l.name);
            if (all.Length > 0) Server.s.Log("!LEVELS ONLINE: " + all, true);
        }
        
        internal static void TemprankExpiry(SchedulerTask task) {
            Player[] players = PlayerInfo.Online.Items;         
            
            foreach (string line in File.ReadAllLines(Paths.TempRanksFile))
                foreach (Player p in players)
            {
                if (!line.CaselessStarts(p.name)) continue;
                string[] args = line.SplitSpaces();

                int min = int.Parse(args[4]), hour = int.Parse(args[5]);
                int day = int.Parse(args[6]), month = int.Parse(args[7]), year = int.Parse(args[8]);
                int periodH = int.Parse(args[3]), periodM = 0;
                if (args.Length > 10) periodM = int.Parse(args[10]);
                
                DateTime expire = new DateTime(year, month, day, hour, min, 0)
                    .AddHours(periodH).AddMinutes(periodM);
                if (DateTime.Now >= expire)
                    Command.all.Find("temprank").Use(null, p.name + " delete");
            }
            
            DateTime now = DateTime.UtcNow;
            task.Delay = TimeSpan.FromSeconds(60 - now.Second); // TODO: down to seconds
        }
    }
}