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
using MCGalaxy.Commands.Chatting;
using MCGalaxy.Network;
using MCGalaxy.Maths;

namespace MCGalaxy.Tasks {
    internal static class ServerTasks {

        internal static void LocationChecks(SchedulerTask task) {
            Player[] players = PlayerInfo.Online.Items;
            players = PlayerInfo.Online.Items;
            int delay = players.Length == 0 ? 100 : 20;
            task.Delay = TimeSpan.FromMilliseconds(delay);
            
            for (int i = 0; i < players.Length; i++) {
                try {
                    TickPlayer(players[i]);
                } catch (Exception e) {
                    Server.ErrorLog(e);
                }
            }
        }
        
        static void TickPlayer(Player p) {
            if (p.following != "") {
                Player who = PlayerInfo.FindExact(p.following);
                if (who == null || who.level != p.level) {
                    p.following = "";
                    if (!p.canBuild)
                        p.canBuild = true;
                    if (who != null && who.possess == p.name)
                        who.possess = "";
                    return;
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
            
            SchedulerTask[] tasks = p.CriticalTasks.Items;
            for (int i = 0; i < tasks.Length; i++) {
                SchedulerTask task = tasks[i];
                task.Callback(task);
                
                if (task.Repeating) continue;
                p.CriticalTasks.Remove(task);
            }
        }
        
        internal static void UpdateEntityPositions(SchedulerTask task) {
            Entities.GlobalUpdate();
            PlayerBot.GlobalUpdatePosition();
            task.Delay = TimeSpan.FromMilliseconds(Server.PositionInterval);
        }
        
        internal static void CheckState(SchedulerTask task) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                p.Send(Packet.Ping());
                if (Server.afkminutes <= 0) return;
                if (DateTime.UtcNow < p.AFKCooldown) return;
                
                if (p.IsAfk) {
                    int time = Server.afkkick;
                    if (p.AutoAfk) time += Server.afkminutes;
                    
                    if (Server.afkkick > 0 && p.Rank < Server.afkkickperm) {
                        if (p.LastAction.AddMinutes(time) < DateTime.UtcNow)
                            p.Leave("Auto-kick, AFK for " + Server.afkkick + " minutes");
                    }
                } else {
                    DateTime lastAction = p.LastAction;
                    if (lastAction.AddMinutes(Server.afkminutes) < DateTime.UtcNow) {
                        CmdAfk.ToggleAfk(p, "auto: Not moved for " + Server.afkminutes + " minutes");
                        p.AutoAfk = true;
                        p.LastAction = lastAction;
                    }
                }
            }
        }
        
        internal static void BlockUpdates(SchedulerTask task) {
            Level[] loaded = LevelInfo.Loaded.Items;
            task.Delay = TimeSpan.FromSeconds(Server.blockInterval);
            
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
            
            foreach (Level lvl in levels) {
                try {
                    if (!lvl.changed || !lvl.ShouldSaveChanges()) continue;

                    lvl.Save();
                    if (count == 0)  {
                        int backupNumber = lvl.Backup();
                        if (backupNumber != -1) {
                            lvl.ChatLevel("Backup " + backupNumber + " saved.");
                            Server.s.Log("Backup " + backupNumber + " saved for " + lvl.name);
                        }
                    }
                } catch {
                    Server.s.Log("Backup for " + lvl.name + " has caused an error.");
                }
            }

            if (count <= 0) count = 15;
            task.State = count;
            task.Delay = TimeSpan.FromSeconds(Server.backupInterval);

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