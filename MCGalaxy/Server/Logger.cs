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
using System.Text;

namespace MCGalaxy {
    
    public enum LogType {
        
        /// <summary> Background system activity, such as auto-saving maps, performing GC, etc. </summary>
        BackgroundActivity,
        
        /// <summary> Normal system activity, such as loading maps, etc. </summary>
        SystemActivity,
        
        /// <summary> Activity causes by games such as lava survival, TNT wars, etc. </summary>
        GameActivity,
        
        /// <summary> User activity by players or console, such as connecting, banning players, etc. </summary>
        UserActivity,
        
        /// <summary> User performs a suspicious activity, such as triggering block spam kick, noclipping in a game, etc. </summary>
        SuspiciousActivity,
        
        /// <summary> Activity on IRC. </summary>
        IRCCActivity,
        
        
        /// <summary> Warning message, such as failure to save a file. </summary>
        Warning,
        
        /// <summary> Handled or unhandled exception occurs. </summary>
        Error,
        
        /// <summary> Command used by a player. </summary>
        CommandUsage,
        
        
        /// <summary> Chat globally or only on player's level. </summary>
        PlayerChat,
        
        /// <summary> Chat from IRC. </summary>
        IRCChat,      
        
        /// <summary> Chat to all players in a particular chatroom, or across all chatrooms. </summary>
        ChatroomChat,
        
        /// <summary> Chat to all players who have permission to read certain chat group (/opchat, /adminchat). </summary>
        StaffChat,
        
        /// <summary> Chat from one player to another. </summary>
        PrivateChat,
        
        /// <summary> Chat to all players of a rank. </summary>
        RankChat,
        
        
        /// <summary> Debug messages. </summary>
        Debug,
        
        /// <summary> Message shown to console. </summary>
        ConsoleMessage,
    }
    
    public delegate void LogHandler(LogType type, string message);
    
    
    /// <summary> Logs message to file and/or console. </summary>
    public static class Logger {
        
        public static LogHandler LogHandler;
        static readonly object logLock = new object();
        
        public static void Log(LogType type, string message) {
            lock (logLock) {
                if (LogHandler != null) LogHandler(type, message);
            }
        }
        
        public static void Log(LogType type, string format, object arg0) {
            Log(type, string.Format(format, arg0));
        }
        
        public static void Log(LogType type, string format, object arg0, object arg1) {
            Log(type, string.Format(format, arg0, arg1));
        }
        
        public static void Log(LogType type, string format, object arg0, object arg1, object arg2) {
            Log(type, string.Format(format, arg0, arg1, arg2));
        }
        
        public static void Log(LogType type, string format, params object[] args) {
            Log(type, string.Format(format, args));
        }
        
        
        public static void LogError(Exception ex) {
            StringBuilder sb = new StringBuilder();
            while (ex != null) {
                DescribeError(ex, sb);
                ex = ex.InnerException;
            }
            Log(LogType.Error, sb.ToString());
        }

        static void DescribeError(Exception ex, StringBuilder sb) {
            // Attempt to gather this info.  Skip anything that you can't read for whatever reason
            try { sb.AppendLine("Type: " + ex.GetType().Name); } catch { }
            try { sb.AppendLine("Source: " + ex.Source); } catch { }
            try { sb.AppendLine("Message: " + ex.Message); } catch { }
            try { sb.AppendLine("Target: " + ex.TargetSite.Name); } catch { }
            try { sb.AppendLine("Trace: " + ex.StackTrace); } catch { }
        }
    }
}