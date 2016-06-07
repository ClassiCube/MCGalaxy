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
using System.Threading;

namespace MCGalaxy.Gui {
    public static class Handlers {
        
        public static void HandleChat(string text, Action<string> output) {
            if (text != null) text = text.Trim();
            if (String.IsNullOrEmpty(text)) return;
            if (Chat.HandleModes(null, text)) return;
            
            Player.GlobalMessage("Console [&a" + Server.ZallState + Server.DefaultColor + "]:&f " + text);
            Server.IRC.Say("Console [&a" + Server.ZallState + "%S]: " + text);
            Server.s.Log("(Console): " + text, true);
            output("<CONSOLE>: " + text);
        }
        
        public static Thread HandleCommand(string text, Action<string> output) {
            if (text != null) text = text.Trim();
            if (String.IsNullOrEmpty(text)) {
                output("CONSOLE: Whitespace commands are not allowed."); return null;
            }
            if (text[0] == '/' && text.Length > 1)
                text = text.Substring(1);
            
            int sep = text.IndexOf(' ');
            string name = "", args = "";
            if (sep >= 0) {
                name = text.Substring(0, sep);
                args = text.Substring(sep + 1);
            } else {
                name = text;
            }
            if (Server.Check(name, args)) { Server.cancelcommand = false; return null; }
            
            Command cmd = Command.all.Find(name);
            if (cmd == null) { output("CONSOLE: No such command."); return null; }
            Thread thread = new Thread(
                () =>
                {
                    try {
                        cmd.Use(null, args);
                        output("CONSOLE: USED /" + text);
                        Server.s.Log("(Console) used /" + text, true);
                    } catch (Exception ex) {
                        Server.ErrorLog(ex);
                        output("CONSOLE: Failed command");
                    }
                });
            thread.Name = "MCG_ConsoleCommand";
            thread.IsBackground = true;
            thread.Start();
            return thread;
        }
    }
}

