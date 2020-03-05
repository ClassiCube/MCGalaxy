/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using MCGalaxy.Network;
using MCGalaxy.Tasks;

namespace MCGalaxy {
    public static class Updater {
        
        static string exeName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
        public const string BaseURL = "https://raw.githubusercontent.com/UnknownShadow200/MCGalaxy/master/";
        public const string UploadsURL = "https://github.com/UnknownShadow200/MCGalaxy/tree/master/Uploads";
        
        const string CurrentVersionFile = BaseURL + "Uploads/current_version.txt";
        #if TEN_BIT_BLOCKS
        const string dllURL = BaseURL + "Uploads/MCGalaxy_infid.dll?raw=true";
        #else
        const string dllURL = BaseURL + "Uploads/MCGalaxy_.dll?raw=true";
        #endif
        const string changelogURL = BaseURL + "Changelog.txt";
        const string guiURL = BaseURL + "Uploads/MCGalaxy.exe?raw=true";
        const string cliURL = BaseURL + "Uploads/MCGalaxyCLI.exe?raw=true";

        public static event EventHandler NewerVersionDetected;
        
        public static void UpdaterTask(SchedulerTask task) {
            UpdateCheck();
            task.Delay = TimeSpan.FromHours(2);
        }

        static void UpdateCheck() {
            if (!Server.Config.CheckForUpdates) return;
            WebClient client = HttpUtil.CreateWebClient();

            try {
                string raw = client.DownloadString(CurrentVersionFile);
                Version latestVersion = new Version(raw);
                
                if (latestVersion <= Server.Version) {
                    Logger.Log(LogType.SystemActivity, "No update found!");
                } else if (NewerVersionDetected != null) {
                    NewerVersionDetected(null, EventArgs.Empty);
                }
            } catch (Exception ex) {
                Logger.LogError("Error checking for updates", ex);
            }
            
            client.Dispose();
        }

        public static void PerformUpdate() {
            try {
                try {
                    DeleteFiles("Changelog.txt", "MCGalaxy_.update", "MCGalaxy.update", "MCGalaxyCLI.update");
                } catch {
                }
                
                WebClient client = HttpUtil.CreateWebClient();
                client.DownloadFile(dllURL, "MCGalaxy_.update");
                client.DownloadFile(guiURL, "MCGalaxy.update");
                client.DownloadFile(cliURL, "MCGalaxyCLI.update");
                client.DownloadFile(changelogURL, "Changelog.txt");

                Level[] levels = LevelInfo.Loaded.Items;
                foreach (Level lvl in levels) {
                    if (!lvl.SaveChanges) continue;
                    lvl.Save();
                    lvl.SaveBlockDBChanges();
                }

                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) pl.save();
                
                string path = Path.Combine(Utils.FolderPath, "Updater.exe");
                if (!File.Exists(path)) throw new FileNotFoundException("Unable to find " + path);
                
                bool mono = Type.GetType("Mono.Runtime") != null;
                if (!mono) {
                    Process.Start(path, "securitycheck10934579068013978427893755755270374" + exeName);
                } else {
                    Process.Start("mono", path + " securitycheck10934579068013978427893755755270374" + exeName);
                }
                Server.Stop(false, "Updating server.");
            } catch (Exception ex) {
                Logger.LogError("Error performing update", ex);
            }
        }
        
        static void DeleteFiles(params string[] files) {
            foreach (string f in files) {
                if (File.Exists(f)) File.Delete(f);
            }
        }
    }
}
