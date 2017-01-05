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
                DisposeTimer(loginTimer, LoginTimerElapsed);
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
                extraTimer.Start();
            }
            LastAction = DateTime.UtcNow;
        }
        
        void ExtraTimerElapsed(object sender, ElapsedEventArgs e) {
            DisposeTimer(extraTimer, ExtraTimerElapsed);

            try {
                if (group.commands.Contains("inbox") && Database.TableExists("Inbox" + name) ) {
                    using (DataTable table = Database.Backend.GetRows("Inbox" + name, "*")) {
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
                Group nobody = Group.NobodyRank;
                if (!nobody.commands.Contains("award") && !nobody.commands.Contains("awards") && !nobody.commands.Contains("awardmod") )
                    SendMessage("You have " + Awards.AwardAmount(name) + " awards.");
            } catch {
            }

            Player[] players = PlayerInfo.Online.Items;
            int visible = 0;
            foreach (Player pl in players) {
                if (pl == this || Entities.CanSee(this, pl)) visible++;
            }
            
            string prefix = visible == 1 ? "There is" : "There are";
            string suffix = visible == 1 ? " player online" : " players online";
            SendMessage(prefix + " currently &a" + visible + suffix);
            
            if (Server.lava.active)
                SendMessage("There is a &aLava Survival %Sgame active! Join it by typing /ls go");
        }
        
        void CheckTimerElapsed(object sender, ElapsedEventArgs e) {
            if (name == "") return;
            SendRaw(Opcode.Ping);
            if (Server.afkminutes <= 0) return;
            if (DateTime.UtcNow < AFKCooldown) return;
            
            if (IsAfk) {
                int time = Server.afkkick;
                if (AutoAfk) time += Server.afkminutes;
                
                if (Server.afkkick > 0 && group.Permission < Server.afkkickperm) {
                    if (LastAction.AddMinutes(time) < DateTime.UtcNow)
                        Leave("Auto-kick, AFK for " + Server.afkkick + " minutes");
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
            DisposeTimer(loginTimer, LoginTimerElapsed);
            DisposeTimer(extraTimer, ExtraTimerElapsed);
            DisposeTimer(checkTimer, CheckTimerElapsed);
        }
        
        void DisposeTimer(Timer timer, ElapsedEventHandler handler) {
            // Note: Some frameworks throw an ObjectDisposedException, 
            //       if a timer has already been disposed and we try to stop it
            try {
                timer.Stop();
                timer.Elapsed -= handler;
                timer.Dispose();
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        }
    }
}
