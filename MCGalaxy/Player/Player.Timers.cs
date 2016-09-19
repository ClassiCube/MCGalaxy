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
using System.Data;
using System.IO;
using System.Timers;
using MCGalaxy.Commands;
using MCGalaxy.SQL;

namespace MCGalaxy {
    public sealed partial class Player : IDisposable {

        void InitTimers() {
            loginTimer.Elapsed += LoginTimerElapsed;
            loginTimer.Start();
            extraTimer.Elapsed += ExtraTimerElapsed;        
            checkTimer.Elapsed += CheckTimerElapsed;
            checkTimer.Start();
        }
        
        void LoginTimerElapsed(object sender, ElapsedEventArgs e) {
            if ( !Loading ) {
                loginTimer.Stop();
                if ( File.Exists("text/welcome.txt") ) {
                    try {
                        List<string> welcome = CP437Reader.ReadAllLines("text/welcome.txt");
                        foreach (string w in welcome)
                            SendMessage(w);
                    } catch {
                    }
                } else {
                    Server.s.Log("Could not find Welcome.txt. Using default.");
                    CP437Writer.WriteAllText("text/welcome.txt", "Welcome to my server!");
                    SendMessage("Welcome to my server!");
                }
                loginTimer.Elapsed -= LoginTimerElapsed;
                loginTimer.Dispose();
                extraTimer.Start();
            }
            LastAction = DateTime.UtcNow;
        }
        
        void ExtraTimerElapsed(object sender, ElapsedEventArgs e) {
            extraTimer.Stop();

            try {
                if (group.commands.Contains("inbox") && Database.TableExists("Inbox" + name) ) {
                    //safe against SQL injections because no user input is given here
                    using (DataTable table = Database.Backend.GetAllRows("Inbox" + name, "*")) {
                        if (table.Rows.Count > 0)
                            SendMessage("You have &a" + table.Rows.Count + " %Smessages in /inbox");
                    }
                }
            } catch {
            }
            
            if ( Server.updateTimer.Interval > 1000 )
                SendMessage("Lowlag mode is currently &aON.");
            if (Economy.Enabled)
                SendMessage("You currently have &a" + money + " %S" + Server.moneys);
            
            try {
                Group nobody = Group.findPerm(LevelPermission.Nobody);
                if (!nobody.commands.Contains("award") && !nobody.commands.Contains("awards") && !nobody.commands.Contains("awardmod") )
                    SendMessage("You have " + Awards.AwardAmount(name) + " awards.");
            } catch {
            }

            Player[] players = PlayerInfo.Online.Items;
            string prefix = players.Length == 1 ? "There is" : "There are";
            string suffix = players.Length == 1 ? " player online" : " players online";
            SendMessage(prefix + " currently &a" + players.Length + suffix);
            
            if (Server.lava.active)
                SendMessage("There is a &aLava Survival %Sgame active! Join it by typing /ls go");
            extraTimer.Elapsed -= ExtraTimerElapsed;
            extraTimer.Dispose();
        }
        
        void CheckTimerElapsed(object sender, ElapsedEventArgs e) {
            if (name == "") return;
            SendRaw(Opcode.Ping);
            if (Server.afkminutes <= 0) return;
            
            if (IsAfk) {
                int time = Server.afkkick;
                if (AutoAfk) time += Server.afkminutes;
                
                if (Server.afkkick > 0 && group.Permission < Server.afkkickperm) {
                    if (LastAction.AddMinutes(time) < DateTime.UtcNow)
                        Kick("Auto-kick, AFK for " + Server.afkkick + " minutes");
                }
                if (Moved()) CmdAfk.ToggleAfk(this, "");
            } else {
                if (Moved()) LastAction = DateTime.UtcNow;

                DateTime lastAction = LastAction;
                if (LastAction.AddMinutes(Server.afkminutes) < DateTime.UtcNow
                    && !String.IsNullOrEmpty(name)) {
                    CmdAfk.ToggleAfk(this, "auto: Not moved for " + Server.afkminutes + " minutes");
                    AutoAfk = true;
                    LastAction = lastAction;
                }
            }
        }
        
        bool Moved() { return oldrot[0] != rot[0] || oldrot[1] != rot[1]; }
        
        void DisposeTimers() {
            loginTimer.Stop();
            loginTimer.Elapsed -= LoginTimerElapsed;
            loginTimer.Dispose();

            extraTimer.Stop();
            extraTimer.Elapsed -= ExtraTimerElapsed;
            extraTimer.Dispose();
            
            checkTimer.Stop();
            checkTimer.Elapsed -= CheckTimerElapsed;
            checkTimer.Dispose();
        }
    }
}
