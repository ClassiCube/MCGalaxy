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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MCGalaxy.Commands.Chatting;
using MCGalaxy.Network;
using MCGalaxy.Maths;

namespace MCGalaxy.Tasks {
    internal static class ServerTasks {

        internal static void QueueTasks() {
            Server.MainScheduler.QueueRepeat(CheckState,
                                             null, TimeSpan.FromSeconds(3));
            
            Server.Background.QueueRepeat(AutoSave,
                                          1, TimeSpan.FromSeconds(ServerConfig.BackupInterval));
            Server.Background.QueueRepeat(BlockUpdates,
                                          null, TimeSpan.FromSeconds(ServerConfig.BlockDBSaveInterval));
        }
        
        
        internal static void LocationChecks(SchedulerTask task) {
            Player[] players = PlayerInfo.Online.Items;
            players = PlayerInfo.Online.Items;
            int delay = players.Length == 0 ? 100 : 20;
            task.Delay = TimeSpan.FromMilliseconds(delay);
            
            for (int i = 0; i < players.Length; i++) {
                try {
                    TickPlayer(players[i]);
                } catch (Exception e) {
                    Logger.LogError(e);
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
            
            SchedulerTask[] tasks = p.CriticalTasks.Items;
            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < tasks.Length; i++) {
                SchedulerTask task = tasks[i];
                if (now < task.NextRun) continue;
                
                task.Callback(task);
                task.NextRun = now.Add(task.Delay);
                
                if (task.Repeating) continue;
                p.CriticalTasks.Remove(task);
            }
        }
        
        internal static void UpdateEntityPositions(SchedulerTask task) {
            Entities.GlobalUpdate();
            PlayerBot.GlobalUpdatePosition();
            task.Delay = TimeSpan.FromMilliseconds(ServerConfig.PositionUpdateInterval);
        }
        
        internal static void CheckState(SchedulerTask task) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.hasTwoWayPing) {
                    p.Send(Packet.TwoWayPing(true, p.Ping.NextTwoWayPingData()));
                } else {
                    p.Send(Packet.Ping());
                }
                
                if (ServerConfig.AutoAfkMins <= 0) return;
                if (DateTime.UtcNow < p.AFKCooldown) return;
                
                if (p.IsAfk) {
                    int time = p.group.AfkKickMinutes;
                    if (p.AutoAfk) time += ServerConfig.AutoAfkMins;
                    
                    if (p.group.AfkKicked && p.LastAction.AddMinutes(time) < DateTime.UtcNow) {
                        p.Leave("Auto-kick, AFK for " + p.group.AfkKickMinutes + " minutes");
                    }
                } else {
                    DateTime lastAction = p.LastAction;
                    if (lastAction.AddMinutes(ServerConfig.AutoAfkMins) < DateTime.UtcNow) {
                        CmdAfk.ToggleAfk(p, "auto: Not moved for " + ServerConfig.AutoAfkMins + " minutes");
                        p.AutoAfk = true;
                        p.LastAction = lastAction;
                    }
                }
            }
        }
        
        internal static void BlockUpdates(SchedulerTask task) {
            Level[] loaded = LevelInfo.Loaded.Items;
            task.Delay = TimeSpan.FromSeconds(ServerConfig.BlockDBSaveInterval);
            
            foreach (Level lvl in loaded) {
                try {
                    if (!lvl.ShouldSaveChanges()) continue;
                    lvl.SaveBlockDBChanges();
                } catch (Exception e) {
                    Logger.LogError(e);
                }
            }
        }
        
        internal static void AutoSave(SchedulerTask task) {
            int count = (int)task.State;
            count--;
            Level[] levels = LevelInfo.Loaded.Items;
            
            foreach (Level lvl in levels) {
                try {
                    if (!lvl.Changed || !lvl.ShouldSaveChanges()) continue;

                    lvl.Save();
                    if (count == 0)  {
                        int backupNumber = lvl.Backup();
                        if (backupNumber != -1) {
                            lvl.ChatLevel("Backup " + backupNumber + " saved.");
                            Logger.Log(LogType.BackgroundActivity, "Backup {0} saved for {1}", backupNumber, lvl.name);
                        }
                    }
                } catch (Exception ex) {
                    Logger.Log(LogType.Warning, "Backup for {0} has caused an error.", lvl.name);
                    Logger.LogError(ex);
                }
            }

            if (count <= 0) count = 15;
            task.State = count;
            task.Delay = TimeSpan.FromSeconds(ServerConfig.BackupInterval);

            Player[] players = PlayerInfo.Online.Items;
            try {
                foreach (Player p in players) p.save();
            } catch (Exception e) {
                Logger.LogError(e);
            }
            
            players = PlayerInfo.Online.Items;
            if (players.Length <= 0) return;
            string all = players.Join(p => p.name);
            if (all.Length > 0) {
                Logger.Log(LogType.BackgroundActivity, "!PLAYERS ONLINE: " + all);
            }

            levels = LevelInfo.Loaded.Items;
            all = levels.Join(l => l.name);
            if (all.Length > 0) {
                Logger.Log(LogType.BackgroundActivity, "!LEVELS ONLINE: " + all);
            }
        }
    }
}