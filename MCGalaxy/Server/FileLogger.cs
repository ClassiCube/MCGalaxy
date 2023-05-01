/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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

namespace MCGalaxy 
{
    public static class FileLogger 
    {       
        public static string LogPath      { get { return msg.Path; } }
        public static string ErrorLogPath { get { return err.Path; } }

        static bool disposed;
        static DateTime last;

        static object logLock = new object();
        static FileLogGroup err = new FileLogGroup();
        static FileLogGroup msg = new FileLogGroup();
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
            msg.Path = "logs/"        + now.ToString("yyyy-MM-dd") + ".txt";
            err.Path = "logs/errors/" + now.ToString("yyyy-MM-dd") + "error.log";

            err.Close();
            msg.Close();
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
                lock (logLock) err.Cache.Enqueue(output);
                
                message = "!!!Error! See " + ErrorLogPath + " for more information.";
            }
            
            string now = DateTime.Now.ToString("(HH:mm:ss) ");
            lock (logLock) msg.Cache.Enqueue(now + message);
        }
        

        public static void Flush(SchedulerTask task) {
            lock (logLock) {
                int errsCount = err.Cache.Count;
                int msgsCount = msg.Cache.Count;

                if (errsCount > 0 || msgsCount > 0) UpdatePaths();

                if (errsCount > 0) err.FlushCache();
                if (msgsCount > 0) msg.FlushCache();
            }
        }

        public static void Dispose() {
            if (disposed) return;
            disposed = true;
            Server.MainScheduler.Cancel(logTask);
            
            lock (logLock) {
                if (err.Cache.Count > 0) err.FlushCache();
                msg.Cache.Clear();
            }
        }
    }

    class FileLogGroup
    {
        public string Path;
        public Queue<string> Cache = new Queue<string>();
        Stream stream;
        StreamWriter writer;

        const int MAX_LOG_SIZE = 1024 * 1024 * 1024; // 1 GB

        public void FlushCache() {
            if (stream == null) {
                stream = new FileStream(Path, FileMode.Append, FileAccess.Write, 
                                        FileShare.ReadWrite, 4096, FileOptions.SequentialScan);
                writer = new StreamWriter(stream);
            }

            try {
                // Failsafe in case something has gone catastrophically wrong
                if (stream.Length > MAX_LOG_SIZE) { Cache.Clear(); return; }

                while (Cache.Count > 0)
                {
                    string item = Cache.Dequeue();
                    item = Colors.Strip(item);
                    writer.WriteLine(item);
                }
                writer.Flush();
            } catch {
                Close();
                throw;
            }
        }

        public void Close() {
            if (stream == null) return;

            stream.Dispose();
            stream = null;
            writer = null;
        }
    }
}