/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using MCGalaxy.SQL;

namespace MCGalaxy.Games {
    
    public sealed partial class ZombieGame {
        
        public void Start(ZombieGameStatus status, int amount) {
            Status = status;
            RoundInProgress = false;
            initialChangeLevel = false;
            MaxRounds = amount + 1;
            RoundsDone = 0;

            Thread t = new Thread(MainLoop);
            t.Name = "MCG_ZombieGame";
            t.Start();
        }

        /// <summary> If there are no infected players left, randomly selected one of the alive players to continue the infection. </summary>
        public void AssignFirstZombie() {
            if (!Running || !RoundInProgress || Infected.Count > 0) return;
            Random random = new Random();
            Player[] alive = Alive.Items;
            if (alive.Length == 0) return;
            int index = random.Next(alive.Length);
            
            while (alive[index].Game.Referee || !alive[index].level.name.CaselessEq(CurLevelName)) {
                if (index >= alive.Length - 1) {
                    index = 0;
                    alive = Alive.Items;
                    if (alive.Length == 0) return;
                } else {
                    index++;
                }
            }
            
            Player zombie = alive[index];
            CurLevel.ChatLevel(zombie.FullName + " %Scontinued the infection!");
            InfectPlayer(zombie);
        }

        public void InfectPlayer(Player p) {
            if (!RoundInProgress || p == null) return;
            Infected.Add(p);
            Alive.Remove(p);
            
            p.Game.Infected = true;
            UpdatePlayerColor(p, Colors.red);
            UpdateAllPlayerStatus();
            PlayerMoneyChanged(p);
        }

        public void DisinfectPlayer(Player p) {
            if (!RoundInProgress || p == null) return;
            Infected.Remove(p);
            Alive.Add(p);
            
            p.Game.Infected = false;
            UpdatePlayerColor(p, p.color);
            UpdateAllPlayerStatus();
            PlayerMoneyChanged(p);
        }

        void ChangeLevel(string next) {
            Player[] online = PlayerInfo.Online.Items;
            if (CurLevel != null) {
                bool saveSettings = false;
                foreach (Player pl in online)
                    saveSettings |= pl.Game.RatedMap;
                if (saveSettings) Level.SaveSettings(CurLevel);
                
                CurLevel.ChatLevel("The next map has been chosen - " + Colors.red + next.ToLower());
                CurLevel.ChatLevel("Please wait while you are transfered.");
            }
            
            CurLevelName = next;
            QueuedLevel = null;
            Command.all.Find("load").Use(null, next.ToLower() + " 0");
            CurLevel = LevelInfo.Find(next);
            if (Server.zombie.SetMainLevel)
                Server.mainLevel = CurLevel;
            
            online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                pl.Game.RatedMap = false;
                pl.Game.PledgeSurvive = false;
                if (!pl.level.name.CaselessEq(next) && pl.level.name.CaselessEq(LastLevelName)) {
                    pl.SendMessage("Going to the next map - &a" + next);
                    Command.all.Find("goto").Use(pl, next);
                }
            }
            if (LastLevelName != "")
                Command.all.Find("unload").Use(null, LastLevelName);
            LastLevelName = next;
        }

        public void ResetState() {
            Status = ZombieGameStatus.NotStarted;
            MaxRounds = 0;
            initialChangeLevel = false;
            RoundInProgress = false;
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                pl.Game.RatedMap = false;
                pl.Game.PledgeSurvive = false;
                if (pl.level == null || !pl.level.name.CaselessEq(CurLevelName))
                    continue;
                ResetCpeMessages(pl);
            }
            
            LastLevelName = "";
            CurLevelName = "";
            CurLevel = null;
        }
        
        void UpdatePlayerStatus(Player p) {
            int seconds = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
            string status = GetStatusMessage(GetTimespan(seconds));
            p.SendCpeMessage(CpeMessageType.Status1, status, true);
        }
        
        internal void UpdateAllPlayerStatus() {
            int seconds = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
            UpdateAllPlayerStatus(GetTimespan(seconds));
        }
        
        internal void UpdateAllPlayerStatus(string timespan) {
            string message = GetStatusMessage(timespan);
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != CurLevel) continue;
                p.SendCpeMessage(CpeMessageType.Status1, message, true);
            }
        }

        string GetStatusMessage(string timespan) {
            if (timespan.Length > 0) {
                const string format = "&a{0} %Salive, &c{1} %Sinfected ({2})";
                return String.Format(format, Alive.Count, Infected.Count, timespan);
            } else {
                const string format = "&a{0} %Salive, &c{1} %Sinfected";
                return String.Format(format, Alive.Count, Infected.Count);
            }
        }

        string GetTimespan(int seconds) {
            if (seconds < 0) return "";
            if (seconds <= 10) return "10 secs left";
            if (seconds <= 30) return "30 secs left";
            if (seconds <= 60) return "1 min left";
            return ((seconds + 59) / 60) + " mins left";
        }
        
        const string createSyntax =
            @"CREATE TABLE if not exists ZombieStats (
ID INTEGER {0}{2} NOT NULL,
Name CHAR(20),
TotalRounds INT,
MaxRounds INT,
TotalInfected INT,
MaxInfected INT,
Additional1 INT,
Additional2 INT,
Additional3 INT,
Additional4 INT{1};"; // reserve space for possible future additions
        
        public void CheckTableExists() {
            string primKey = Server.useMySQL ? "" : "PRIMARY KEY ";
            string autoInc = Server.useMySQL ? "AUTO_INCREMENT" : "AUTOINCREMENT";
            string primKey2 = Server.useMySQL ? ", PRIMARY KEY (ID)" : "";
            Database.executeQuery(string.Format(createSyntax, primKey, autoInc, primKey2));
        }
    }
}
