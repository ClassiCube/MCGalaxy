/*
    Copyright 2015 MCGalaxy
    
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
using System.Data;
using System.IO;
using MCGalaxy.Bots;
using MCGalaxy.DB;
using MCGalaxy.SQL;
using Newtonsoft.Json;

namespace MCGalaxy.Tasks {
    internal static class UpgradeTasks {
        
        internal static void UpgradeOldBlacklist(SchedulerTask task) {
            if (!Directory.Exists("levels/blacklists")) return;
            string[] files = Directory.GetFiles("levels/blacklists");
            for (int i = 0; i < files.Length; i++) {
                string[] blacklist = File.ReadAllLines(files[i]);
                List<string> names = new List<string>();
                
                // Lines are in the format: day.month.year name+
                foreach (string entry in blacklist) {
                    string[] parts = entry.SplitSpaces();
                    string name = parts[parts.Length - 1];
                    name = name.Substring(0, name.Length - 1);
                    names.Add(name);
                }
                
                if (names.Count > 0) {
                    string lvlName = Path.GetFileNameWithoutExtension(files[i]);
                    string propsPath = LevelInfo.PropsPath(lvlName);
                    using (StreamWriter w = new StreamWriter(propsPath, true)) {
                        w.WriteLine("VisitBlacklist = " + names.Join());
                    }
                }
                File.Delete(files[i]);
            }
            Directory.Delete("levels/blacklists");
        }
        
        internal static void UpgradeOldAgreed() {
            // agreed.txt format used to be names separated by spaces, we need to fix that up.
            if (!File.Exists("ranks/agreed.txt")) return;
            
            string data = null;
            using (FileStream fs = File.OpenRead("ranks/agreed.txt")) {
                if (fs.ReadByte() != ' ') return;
                data = new StreamReader(fs).ReadToEnd();
                data = data.Replace(" ", Environment.NewLine);
            }
            File.WriteAllText("ranks/agreed.txt", data);
        }
        
        internal static void MovePreviousLevelFiles(SchedulerTask task) {
            if (!Directory.Exists("levels")) return;
            if (Directory.Exists("levels/prev")) return;
            
            try {
                string[] files = Directory.GetFiles("levels", "*.prev");
                if (files.Length == 0) return;
                if (!Directory.Exists("levels/prev"))
                    Directory.CreateDirectory("levels/prev");
                
                foreach (string file in files) {
                    string name = Path.GetFileName(file);
                    string newFile = "levels/prev/" + name;
                    
                    try {
                        File.Move(file, newFile);
                    } catch (Exception ex) {
                        Logger.Log(LogType.Warning, "Error while trying to move .lvl.prev file");
                        Logger.LogError(ex);
                    }
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
        
        internal static void CombineEnvFiles(SchedulerTask task) {
            if (!Directory.Exists("levels/level properties")) return;
            try {
                string[] files = Directory.GetFiles("levels/level properties", "*.env");
                if (files.Length == 0) return;
                
                Logger.Log(LogType.SystemActivity, "Combining {0} .env and .properties files..", files.Length);
                foreach (string envFile in files) {
                    try {
                        Combine(envFile);
                    } catch (Exception ex) {
                        Logger.Log(LogType.Warning, "Error while trying to combine .env and .properties file");
                        Logger.LogError(ex);
                    }
                }
                Logger.Log(LogType.SystemActivity, "Finished combining .env and .properties files.");
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
        
        static void Combine(string envFile) {
            string name = Path.GetFileNameWithoutExtension(envFile);
            string propsPath = LevelInfo.PropsPath(name);
            
            List<string> lines = new List<string>();
            if (File.Exists(propsPath)) {
                lines = Utils.ReadAllLinesList(propsPath);
            }
            
            using (StreamReader r = new StreamReader(envFile)) {
                string line = null;
                while ((line = r.ReadLine()) != null)
                    lines.Add(line);
            }
            
            File.WriteAllLines(propsPath, lines.ToArray());
            File.Delete(envFile);
        }
        
        internal static void UpgradeOldLockdown(SchedulerTask task) {
            if (!Directory.Exists("text/lockdown/map")) return;
            
            string[] files = Directory.GetFiles("text/lockdown/map");
            for (int i = 0; i < files.Length; i++) {
                File.Delete(files[i]);
                string level = Path.GetFileName(files[i]);
                Server.lockdown.AddIfNotExists(level);
            }
            
            Server.lockdown.Save();
            Directory.Delete("text/lockdown/map");
        }
        
        internal static void UpgradeOldTempranks(SchedulerTask task) {
            if (!File.Exists(Paths.TempRanksFile)) return;

            // Check if empty, or not old form
            using (StreamReader reader = new StreamReader(Paths.TempRanksFile)) {
                string line = reader.ReadLine();
                if (line == null) return;
                string[] parts = line.SplitSpaces();
                if (parts.Length < 9) return;
            }

            string[] lines = File.ReadAllLines(Paths.TempRanksFile);
            for (int i = 0; i < lines.Length; i++) {
                string[] args = lines[i].SplitSpaces();
                if (args.Length < 9) continue;

                int min = int.Parse(args[4]), hour = int.Parse(args[5]);
                int day = int.Parse(args[6]), month = int.Parse(args[7]), year = int.Parse(args[8]);
                int periodH = int.Parse(args[3]), periodM = 0;
                if (args.Length > 10) periodM = int.Parse(args[10]);
                
                DateTime assigned = new DateTime(year, month, day, hour, min, 0);
                DateTime expiry = assigned.AddHours(periodH).AddMinutes(periodM);
                
                // Line format: name assigner assigntime expiretime oldRank tempRank
                lines[i] = args[0] + " " + args[9] + " " + assigned.ToUnixTime() +
                    " " + expiry.ToUnixTime() + " " + args[2] + " " + args[1];
            }
            File.WriteAllLines(Paths.TempRanksFile, lines);
        }
        
        internal static void UpgradeBots(SchedulerTask task) {
            if (!File.Exists(Paths.BotsFile)) return;
            string json = File.ReadAllText(Paths.BotsFile);
            File.WriteAllText(Paths.BotsFile + ".bak", json);
            Logger.Log(LogType.SystemActivity, "Making bots file per-level.. " +
                       "saved backup of global bots file to extra/bots.json.bak");
            
            BotProperties[] bots = JsonConvert.DeserializeObject<BotProperties[]>(json);
            Dictionary<string, List<BotProperties>> botsByLevel = new Dictionary<string, List<BotProperties>>();
            
            foreach (BotProperties bot in bots) {
                List<BotProperties> levelBots;
                if (bot.Level == null || bot.Level.Length == 0) continue;
                
                if (!botsByLevel.TryGetValue(bot.Level, out levelBots)) {
                    levelBots = new List<BotProperties>();
                    botsByLevel[bot.Level] = levelBots;
                }
                levelBots.Add(bot);
            }
            
            foreach (var kvp in botsByLevel) {
                json = JsonConvert.SerializeObject(kvp.Value);
                File.WriteAllText(BotsFile.BotsPath(kvp.Key), json);
            }
            
            if (Server.mainLevel.Bots.Count == 0) {
                BotsFile.Load(Server.mainLevel);
            }
            File.Delete(Paths.BotsFile);
        }

        
        internal static void UpgradeDBTimeSpent(SchedulerTask task) {
            DataTable table = Database.Backend.GetRows(PlayerData.DBTable, "TimeSpent", "LIMIT 1");
            if (table.Rows.Count == 0) return; // no players
            
            string time = table.Rows[0]["TimeSpent"].ToString();
            if (time.IndexOf(' ') == -1) return; // already upgraded
            
            Logger.Log(LogType.SystemActivity, "Upgrading TimeSpent column in database to new format..");
            DumpPlayerTimeSpents();
            UpgradePlayerTimeSpents();
            Logger.Log(LogType.SystemActivity, "Upgraded {0} rows. ({1} rows failed)", playerCount, playerFailed);
        }
        
        static List<int> playerIds;
        static List<long> playerSeconds;
        static int playerCount, playerFailed = 0;
        
        
        static void DumpPlayerTimeSpents() {
            playerIds = new List<int>();
            playerSeconds = new List<long>();
            Database.ExecuteReader("SELECT ID, TimeSpent FROM Players", AddPlayerTimeSpent);
        }
        
        static void AddPlayerTimeSpent(IDataReader reader) {
            playerCount++;
            try {
                int id = reader.GetInt32(0);
                TimeSpan span = reader.GetString(1).ParseDBTime();
                
                playerIds.Add(id);
                playerSeconds.Add((long)span.TotalSeconds);
            } catch {
                playerFailed++;
            }
        }
        
        static void UpgradePlayerTimeSpents() {          
            using (BulkTransaction bulk = Database.Backend.CreateBulk()) {
                IDataParameter idParam = bulk.CreateParam("@0", DbType.Int32);
                IDataParameter secsParam = bulk.CreateParam("@1", DbType.Int64);
                
                for (int i = 0; i < playerIds.Count; i++) {
                    idParam.Value = playerIds[i];
                    secsParam.Value = playerSeconds[i];
                    
                    using (IDbCommand cmd = bulk.CreateCommand("UPDATE Players SET TimeSpent = @1 WHERE ID = @0")) {
                        cmd.Parameters.Add(idParam);
                        cmd.Parameters.Add(secsParam);
                        cmd.ExecuteNonQuery();
                    }
                }
                
                bulk.Commit();
            }
        }
    }
}