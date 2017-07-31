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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace MCGalaxy.Gui {
    public static class App {
        public static bool usingConsole = false;
        public static string parent = Path.GetFileName(Assembly.GetEntryAssembly().Location);

        public static void ExitProgram(bool restarting) { ExitProgram(restarting, ""); }        
        public static void ExitProgram(bool restarting, string msg) {
            Server.restarting = restarting;
            Server.shuttingDown = true;
            if (msg.Length == 0)
                msg = restarting ? "Server restarted. Sign in again and rejoin." : ServerConfig.DefaultShutdownMessage;
            Server.Exit(restarting, msg);
            new Thread(() => ShutdownThread(restarting, msg)).Start();
        }
        
        static void ShutdownThread(bool restarting, string msg) {
            saveAll(restarting, msg);
            if (restarting) StartProcess();
            ExitProcess();
        }
        
        static void StartProcess() {
            if (usingConsole) Process.Start(parent);
            else Process.Start(Application.ExecutablePath);
        }
        
        static void ExitProcess() {
            Environment.Exit(0);
        }

        public static void saveAll(bool restarting, string msg) {
            try {
                Player[] players = PlayerInfo.Online.Items; 
                foreach (Player p in players) {
                    p.Leave(msg);
                }
            } catch (Exception ex) { 
                Logger.LogError(ex); 
            }

            try {
                string level = null;
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    if (!lvl.ShouldSaveChanges()) continue;
                    
                    level = level + lvl.name + "=" + lvl.physics + Environment.NewLine;
                    lvl.Save(false, true);
                    lvl.SaveBlockDBChanges();
                }
                
                if (Server.ServerSetupFinished && !ServerConfig.AutoLoadMaps) {
                    File.WriteAllText("text/autoload.txt", level);
                }
            } catch (Exception ex) {
                Logger.LogError(ex); 
            }
        }
    }
}

