/*
    Copyright 2011 MCForge
    
    Written by jordanneil23 with alot of help from TheMusiKid.
        
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
using System.IO;
using System.Threading;

namespace MCGalaxy.Commands {
    public sealed class CmdShutdown : Command {
        public override string name { get { return "shutdown"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            int secTime = 10;
            bool shutdown = message != "cancel";
            Server.abortShutdown = false;
            
            if (message == "") {
                message = "Server is going to shutdown in " + secTime + " seconds";
            } else if (message == "cancel") {
                Server.abortShutdown = true; message = "Shutdown cancelled";
            } else {
                string[] args = message.SplitSpaces(2);
                if (int.TryParse(args[0], out secTime)) {
                    message = args.Length > 1 ? args[1] : message;
                } else {
                    secTime = 10;
                }
            }
            
            if (secTime <= 0) {
                Player.Message(p, "Countdown time must be greater than zero"); return;
            }
            if (!shutdown) return;
            
            Log(message);
            for (int t = secTime; t > 0; t--) {
                if (!Server.abortShutdown) {
                    Log("Server shutdown in " + t + " seconds"); Thread.Sleep(1000);
                } else {
                    Server.abortShutdown = false; Log("Shutdown cancelled"); return;
                }
            }
            
            if (!Server.abortShutdown) {
                MCGalaxy.Gui.App.ExitProgram(false);
            } else {
                Server.abortShutdown = false; Log("Shutdown cancelled");
            }
        }
        
        static void Log(string message) {
             Player.GlobalMessage("&4" + message); 
             Server.s.Log(message);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/shutdown [time] [message]");
            Player.Message(p, "%HShuts the server down");
        }
    }
}
