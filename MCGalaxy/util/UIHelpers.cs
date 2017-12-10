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
using System.Threading;
using MCGalaxy;

namespace MCGalaxy.UI {
    public static class UIHelpers {
        
        static string lastCMD = "";
        public static void HandleChat(string text) {
            if (text != null) text = text.Trim();
            if (String.IsNullOrEmpty(text)) return;
            if (ChatModes.Handle(null, text)) return;
            
            Chat.MessageGlobal("Console [&a{0}%S]:&f {1}", ServerConfig.ConsoleName, text);
            Server.IRC.Say("Console [&a" + ServerConfig.ConsoleName + "%S]: " + text);
            Logger.Log(LogType.PlayerChat, "(console): " + text);
        }
        
        public static Thread RepeatCommand() {
            if (lastCMD.Length == 0) {
                Logger.Log(LogType.CommandUsage, "(console): Cannot repeat command - no commands used yet.");
                return null;
            }
            Logger.Log(LogType.CommandUsage, "Repeating %T/" + lastCMD);
            return HandleCommand(lastCMD);
        }
        
        public static Thread HandleCommand(string text) {
            if (text != null) text = text.Trim();
            if (String.IsNullOrEmpty(text)) {
                Logger.Log(LogType.CommandUsage, "(console): Whitespace commands are not allowed."); 
                return null;
            }
            if (text[0] == '/' && text.Length > 1)
                text = text.Substring(1);
            
            lastCMD = text;
            int sep = text.IndexOf(' ');
            string name = "", args = "";
            
            if (sep >= 0) {
                name = text.Substring(0, sep);
                args = text.Substring(sep + 1);
            } else {
                name = text;
            }
            
            Command.Search(ref name, ref args);
            if (Server.Check(name, args)) { Server.cancelcommand = false; return null; }            
            Command cmd = Command.all.Find(name);
            
            if (cmd == null) { 
                Logger.Log(LogType.CommandUsage, "(console): Unknown command \"{0}\"", name); return null; 
            }
            if (!cmd.SuperUseable) { 
                Logger.Log(LogType.CommandUsage, "(console): /{0} can only be used in-game.", cmd.name); return null; 
            }
            
            Thread thread = new Thread(
                () =>
                {
                    try {
                        cmd.Use(null, args);
                        Logger.Log(LogType.CommandUsage, "(console) used /" + text);
                    } catch (Exception ex) {
                        Logger.LogError(ex);
                        Logger.Log(LogType.CommandUsage, "(console): FAILED COMMAND");
                    }
                });
            thread.Name = "MCG_ConsoleCommand";
            thread.IsBackground = true;
            thread.Start();
            return thread;
        }
        
        public static string Format(string message) {
            message = message.Replace("%S", "&f"); // We want %S to be treated specially when displayed in UI
            message = Colors.Escape(message);      // Need to Replace first, otherwise it's mapped by Colors.Escape
            return message;
        }
        
        public static string OutputPart(ref char nextCol, ref int start, string message) {
            int next = NextPart(start, message);
            string part;
            if (next == -1) {
                part = message.Substring(start);
                start = message.Length;
            } else {
                part = message.Substring(start, next - start);
                start = next + 2;
                nextCol = message[next + 1];
            }
            return part;
        }
        
        static int NextPart(int start, string message) {
            for (int i = start; i < message.Length; i++) {
                if (message[i] != '&') continue;
                // No colour code follows this
                if (i == message.Length - 1) return -1;
                
                // Check following character is an actual colour code
                char col = message[i + 1];
                if (Colors.Map(ref col)) return i;
            }
            return -1;
        }
    }
}

