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
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.Cli;

namespace MCGalaxy.Gui {
    
    public static class Popup {
        public static void Message(string message, string title = "") {
            MessageBox.Show(message, title);
        }
        
        public static void Error(string message) {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        public static void Warning(string message) {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        public static bool OKCancel(string message, string title) {
            return MessageBox.Show(message, title, MessageBoxButtons.OKCancel,
                                  MessageBoxIcon.Warning) == DialogResult.OK;
        }
        
        public static bool YesNo(string message, string title) {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, 
                                   MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
    
    public static class Program {
        
        [STAThread]
        public static void Main(string[] args) {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            bool useConsole = DetectConsole();
            
            if (useConsole) {
                CLI.RunCLI();
            } else {
                RunGUI();
            }
        }
        
        // For compatibility with older MCGalaxy versions, which use Viewmode.cfg
        static bool DetectConsole() {
            try {
                if (!File.Exists("Viewmode.cfg")) return false;
                string[] lines = File.ReadAllLines("Viewmode.cfg");
                
                for (int i = 0; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line.Length == 0 || line[0] == '#') continue;
                    
                    line = line.Trim().Replace(" ", "");
                    if (!line.StartsWith("cli=")) continue;                  
                    return bool.Parse(line.Substring("cli=".Length));
                }
            } catch { }
            return false;
        }
        
        static void RunGUI() {
            if (!File.Exists("MCGalaxy_.dll")) {
                Popup.Error("Cannot start server as MCGalaxy_.dll is missing\r\n" +
                            "Download it from https://github.com/Hetal728/MCGalaxy/tree/master/Uploads");
                return;
            }
            // separate method, in case MCGalaxy_.dll is missing
            StartGUI();
        }
        
        static void StartGUI() {
            FileLogger.Init();
            Server.RestartPath = Application.ExecutablePath;
            AppDomain.CurrentDomain.UnhandledException += GlobalExHandler;
            Application.ThreadException += ThreadExHandler;

            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault( false );
                Application.Run(new Window());
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        static void GlobalExHandler(object sender, UnhandledExceptionEventArgs e) {
            CLI.LogAndRestart((Exception)e.ExceptionObject);
        }

        static void ThreadExHandler(object sender, ThreadExceptionEventArgs e) {
            CLI.LogAndRestart(e.Exception);
        }
    }
}

