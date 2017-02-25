using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Updater {
    
    // !! NOTE !!! You must not use any MCGalaxy code here, as you cannot reference the dlls because updating replaces thems
    class Program {
        static int tries = 0;
        static bool usingConsole = false;
        const string check = "securitycheck10934579068013978427893755755270374";
        
        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException += UnhandledError;
            if (args.Length < 1 || !args[0].Contains(check)) {
                ShowMessage("Updater was started incorrectly.", true); return;
            }
            usingConsole = IsConsole();
            
            try {
                args[0] = args[0].Replace(check, "");
                if (args[0] == ".exe") args[0] = "MCGalaxy.exe";
                
                // Wait for other processes to finish
                Console.WriteLine("Waiting for " + args[0] + " to exit...");
                while (Process.GetProcessesByName(args[0]).Length > 0) {
                    Thread.Sleep(1);
                }
            } catch (Exception e) { 
                UpdateFailed(e); 
            }
            Update(args);
        }
        
        static bool IsConsole() {
            try {
                string[] lines = File.ReadAllLines("Viewmode.cfg");
                foreach (string line in lines) {
                    // Find the cli = true/false line
                    if (!line.StartsWith("cli")) continue;
                    int sep = line.IndexOf('=');
                    if (sep == -1) continue;
                    
                    string value = line.Substring(sep + 1).Trim();
                    return value.Equals("true", StringComparison.OrdinalIgnoreCase);
                }
            } catch {
            }
            return false;
        }
        
        static void Update(string[] args) {
            Console.WriteLine("Updating MCGalaxy...");
            // No files to update
            if (!File.Exists("MCGalaxy.update") && !File.Exists("MCGalaxy_.update")) {
                ShowMessage("Updater has no files to update", true); return;
            }
            
            for (tries = 1; tries <= 3; tries++) {
                if (!UpdateFile("MCGalaxy", ".exe")) continue;
                if (!UpdateFile("MCGalaxy_", ".dll")) continue;

                Console.WriteLine("Successfully updated MCGalaxy. Starting...");
                StartMCGalaxy(args[0]);
                return;
            }
        }
        
        static bool UpdateFile(string name, string ext) {
            if (!File.Exists(name + ".update")) return true;
            
            try {
                if (File.Exists(name + ext)) {
                    if (File.Exists(name + ".backup"))
                        File.Delete(name + ".backup");
                    File.Move(name + ext, name + ".backup");
                }
                
                File.Move(name + ".update", name + ext);
                return true;
            } catch (Exception ex) {
                Retry(ex);
                return false;
            }
        }
        
        static void Retry(Exception ex) {
            if (tries == 3) {
                UpdateFailed(ex);
            } else {
                Console.WriteLine("\n\nAn error occured while updating.  Retrying...\n\n");
                Thread.Sleep(100);
            }
        }
        
        
        static void StartMCGalaxy(string file) {
            Console.WriteLine("Successfully updated MCGalaxy. Starting...");
            try {
                bool mono = Type.GetType("Mono.Runtime") != null;
                if (!mono) {
                    Process.Start(file);
                } else {
                    string absolutePath = AppDomain.CurrentDomain.BaseDirectory;
                    Process.Start("mono", Path.Combine(absolutePath, file));
                }
            } catch (Exception) {
                ShowMessage("Updater has updated MCGalaxy, but was unable to start it. You will need to start it manually.", false);
            }
        }
        
        
        static void UpdateFailed(Exception e) {
            ShowMessage("Updater failed to update MCGalaxy:\n\n" + e, true);
        }
        
        static void UnhandledError(object sender, UnhandledExceptionEventArgs args) {
            Exception e = (Exception)args.ExceptionObject;
            ShowMessage("UnhandledException:\n\n" + e, true);
        }
        
        static void ShowMessage(string message, bool error) {
            if (usingConsole) {
                message = error ? message + " \n\nPress any key to exit." : message;
                Console.WriteLine(message);
            } else {
                string title = error ? "Updater Error" : "Updater";
                MessageBoxIcon icon = error ? MessageBoxIcon.Error : MessageBoxIcon.Information;
                MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
            }
            
            if (error) Environment.Exit(0);
        }
    }
}
