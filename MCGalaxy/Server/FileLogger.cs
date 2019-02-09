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
using MCGalaxy.Tasks;

namespace MCGalaxy {
    
    public static class FileLogger {
        
        public static string LogPath { get { return msgPath; } }
        public static string ErrorLogPath { get { return errPath; } }

        static bool disposed;
        static DateTime last;

        static object logLock = new object();
        static string errPath, msgPath;
        static Queue<string> errCache = new Queue<string>(), msgCache = new Queue<string>();
        static SchedulerTask logTask;

        public static void Init() {
            if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
            if (!Directory.Exists("logs/errors")) Directory.CreateDirectory("logs/errors");
            UpdatePaths();
            Logger.LogHandler += LogMessage;
            
            logTask = Server.MainScheduler.QueueRepeat(Flush, null,
                                                       TimeSpan.FromMilliseconds(500));
        }
        
        // Update paths only if a new date
        static void UpdatePaths() {
            DateTime now = DateTime.Now;
            if (now.Year == last.Year && now.Month == last.Month && now.Day == last.Day) return;
            
            last = now;
            msgPath = "logs/" + now.ToString("yyyy-MM-dd").Replace("/", "-") + ".txt";
            errPath = "logs/errors/" + now.ToString("yyyy-MM-dd").Replace("/", "-") + "error.log";
        }
        
        static void LogMessage(LogType type, string message) {
            if (String.IsNullOrEmpty(message)) return;
            if (!Server.Config.FileLogging[(int)type]) return;
            
            if (type == LogType.Error) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("----" + DateTime.Now + " ----");
                sb.AppendLine(message);
                sb.Append('-', 25);
                
                string output = sb.ToString();
                lock (logLock) errCache.Enqueue(output);
                
                message = "!!!Error! See " + ErrorLogPath + " for more information.";
            }
            
            string now = DateTime.Now.ToString("(HH:mm:ss) ");
            lock (logLock) msgCache.Enqueue(now + message);
        }
        

        public static void Flush(SchedulerTask task) {
            lock (logLock) {
                if (errCache.Count > 0 || msgCache.Count > 0) UpdatePaths();
                
                if (errCache.Count > 0) FlushCache(errPath, errCache);
                if (msgCache.Count > 0) FlushCache(msgPath, msgCache);
            }
        }

        static void FlushCache(string path, Queue<string> cache) {
            //TODO: not happy about constantly opening and closing a stream like this but I suppose its ok (Pidgeon)
            using (StreamWriter w = new StreamWriter(path, true)) {
                while (cache.Count > 0) {
                    string item = cache.Dequeue();
                    item = Colors.Strip(item);
                    w.WriteLine(item);
                }
            }
        }

        public static void Dispose() {
            if (disposed) return;
            disposed = true;
            Server.MainScheduler.Cancel(logTask);
            
            lock (logLock) {
                if (errCache.Count > 0)
                    FlushCache(errPath, errCache);
                msgCache.Clear();
            }
        }
    }
}