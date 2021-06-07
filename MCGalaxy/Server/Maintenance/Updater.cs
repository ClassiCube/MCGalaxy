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
    /// <summary> Checks for and applies software updates. </summary>
    public static class Updater {
        
        public static string SourceURL = "https://github.com/UnknownShadow200/MCGalaxy";
        public const string BaseURL    = "https://raw.githubusercontent.com/UnknownShadow200/MCGalaxy/master/";
        public const string UploadsURL = "https://github.com/UnknownShadow200/MCGalaxy/tree/master/Uploads";
        
        const string CurrentVersionURL = BaseURL + "Uploads/current_version.txt";
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
                string latest = client.DownloadString(CurrentVersionURL);
                
                if (new Version(Server.Version) >= new Version(latest)) {
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
                    DeleteFiles("Changelog.txt", "MCGalaxy_.update", "MCGalaxy.update", "MCGalaxyCLI.update",
                                "prev_MCGalaxy_.dll", "prev_MCGalaxy.exe", "prev_MCGalaxyCLI.exe");
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
                foreach (Player pl in players) pl.SaveStats();
                
                // Move current files to previous files (by moving instead of copying, 
                //  can overwrite original the files without breaking the server)
                AtomicIO.TryMove("MCGalaxy_.dll",   "prev_MCGalaxy_.dll");
                AtomicIO.TryMove("MCGalaxy.exe",    "prev_MCGalaxy.exe");
                AtomicIO.TryMove("MCGalaxyCLI.exe", "prev_MCGalaxyCLI.exe");
                
                // Move update files to current files
                File.Move("MCGalaxy_.update",   "MCGalaxy_.dll");
                File.Move("MCGalaxy.update",    "MCGalaxy.exe");
                File.Move("MCGalaxyCLI.update", "MCGalaxyCLI.exe");                             

                Server.Stop(true, "Updating server.");
            } catch (Exception ex) {
                Logger.LogError("Error performing update", ex);
            }
        }
        
        static void DeleteFiles(params string[] paths) {
            foreach (string path in paths) { AtomicIO.TryDelete(path); }
        }
    }
}
