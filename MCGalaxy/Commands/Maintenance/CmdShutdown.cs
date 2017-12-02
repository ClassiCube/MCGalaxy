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
using MCGalaxy.Tasks;

namespace MCGalaxy.Commands.Maintenance {
    public sealed class CmdShutdown : Command {
        public override string name { get { return "Shutdown"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        static SchedulerTask shutdownTask;
        public override void Use(Player p, string message) {
            if (message.CaselessEq("abort") || message.CaselessEq("cancel")) {
                if (shutdownTask == null) {
                    Player.Message(p, "No server shutdown is in progress."); return;
                }
                
                Log("Shutdown aborted.");
                Server.MainScheduler.Cancel(shutdownTask);
                shutdownTask = null;
            } else {
                if (shutdownTask != null) {
                    Player.Message(p, "Server is already shutting down, use " +
                                   "%T/Shutdown abort %Sto abort the shutdown."); return;
                }
                
                string reason = "";
                int secTime = 0;
                string[] args = message.SplitSpaces(2);
                
                if (int.TryParse(args[0], out secTime)) {
                    reason = args.Length > 1 ? args[1] : "";
                } else {
                    reason = args[0]; secTime = 10;
                }
                if (secTime <= 0) { Player.Message(p, "Countdown time must be greater than zero"); return; } 
                DoShutdown(secTime, reason);
            }
        }
        
        static void DoShutdown(int secTime, string reason) {
            ShutdownArgs args = new ShutdownArgs();
            args.Delay = secTime - 1;
            args.Reason = reason;
            
            if (reason.Length > 0) reason = ": " + reason;
            Log("Server shutdown started" + reason);
            Log("Server shutdown in " + secTime + " seconds");
            
            shutdownTask = Server.MainScheduler.QueueRepeat(
                ShutdownCallback, args, TimeSpan.FromSeconds(1));
        }
        
        
        static void Log(string message) {
            Chat.MessageAll("&4" + message);
            Logger.Log(LogType.SystemActivity, message);
        }
        
        static void ShutdownCallback(SchedulerTask task) {
            ShutdownArgs args = (ShutdownArgs)task.State;
            if (args.Delay == 0) {
                Server.Stop(false, args.Reason);
            } else {
                Log("Server shutdown in " + args.Delay + " seconds");
                args.Delay--;
            }
        }
        
        class ShutdownArgs {
            public int Delay;
            public string Reason;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Shutdown [delay] <reason>");
            Player.Message(p, "%HShuts the server down after [delay] seconds.");
            Player.Message(p, "%T/Shutdown abort");
            Player.Message(p, "%HAborts the current server shutdown.");
        }
    }
}
