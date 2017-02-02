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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MCGalaxy {
    
    public static class Logger {
        
        public static string LogPath { get { return msgPath; } set { msgPath = value; } }
        public static string ErrorLogPath { get { return errPath; } set { errPath = value; } }

        static bool disposed;
        static bool reportBack = false; // TODO: implement report back

        static object logLock = new object();
        static Thread logThread;
        static string errPath, msgPath;        
        static Queue<string> errCache = new Queue<string>(), msgCache = new Queue<string>();

        public static void Init() {
            reportBack = Server.reportBack;
            //Should be done as part of the config
            if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
            if (!Directory.Exists("logs/errors")) Directory.CreateDirectory("logs/errors");
            msgPath = "logs/" + DateTime.Now.ToString("yyyy-MM-dd").Replace("/", "-") + ".txt";
            errPath = "logs/errors/" + DateTime.Now.ToString("yyyy-MM-dd").Replace("/", "-") + "error.log";

            logThread = new Thread(WorkerThread);
            logThread.Name = "MCG_Logger";
            logThread.IsBackground = true;
            logThread.Start();
        }

        public static void Write(string str) { LogMessage(str); }
        public static void LogMessage(string message) {

        }
        
        public static void WriteError(Exception ex) { LogError(ex); }
        public static void LogError(Exception ex) {
        }

        static void WorkerThread() {
            while (!disposed) {
                lock (logLock) {
                    if (errCache.Count > 0) FlushCache(errPath, errCache);
                    if (msgCache.Count > 0) FlushCache(msgPath, msgCache);
                }
                Thread.Sleep(500);
            }
        }

        static void FlushCache(string path, Queue<string> cache) {
            //TODO: not happy about constantly opening and closing a stream like this but I suppose its ok (Pidgeon)
            using (StreamWriter w = new StreamWriter(path, true)) {
                while (cache.Count > 0)
                    w.Write(cache.Dequeue());
            }
        }
        
        static void DescribeError(Exception e, StringBuilder sb) {
            if (e == null) return;

            // Attempt to gather this info.  Skip anything that you can't read for whatever reason
            try { sb.AppendLine("Type: " + e.GetType().Name); } catch { }
            try { sb.AppendLine("Source: " + e.Source); } catch { }
            try { sb.AppendLine("Message: " + e.Message); } catch { }
            try { sb.AppendLine("Target: " + e.TargetSite.Name); } catch { }
            try { sb.AppendLine("Trace: " + e.StackTrace); } catch { }
        }

        public static void Dispose() {
            if (disposed) return;
            disposed = true;
            lock (logLock) {
                if (errCache.Count > 0)
                    FlushCache(errPath, errCache);
                msgCache.Clear();
            }
        }
    }
}
