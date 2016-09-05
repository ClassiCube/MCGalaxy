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
using System.Data;
using System.IO;
using System.Threading;
using MCGalaxy.Commands.World;
using MCGalaxy.SQL;

namespace MCGalaxy.Games {
    
    public struct ZombieStats {
        public int TotalRounds, MaxRounds, TotalInfected, MaxInfected;
    }
    
    public sealed partial class ZombieGame {
        
        public void Start(ZombieGameStatus status, Level level, int rounds) {
            Status = status;
            RoundInProgress = false;
            MaxRounds = rounds + 1;
            RoundsDone = 0;
            if (!SetStartLevel(level)) return;
            
            Thread t = new Thread(MainLoop);
            t.Name = "MCG_ZombieGame";
            t.Start();
        }
        
        bool SetStartLevel(Level level) {
            if (level == null) {
                List<string> levels = GetCandidateLevels();
                if (levels == null) return false;
                
                CurLevelName = GetRandomLevel(new Random(), levels);
                CurLevel = LevelInfo.FindExact(CurLevelName) 
                    ?? CmdLoad.LoadLevel(null, CurLevelName);
                if (CurLevel == null) return false;
            } else {
                CurLevelName = level.name;
                CurLevel = level;
            }
            
            Chat.MessageAll("A game of zombie survival is starting on: {0}", CurLevelName);
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != CurLevel) continue;
                PlayerJoinedLevel(p, p.level, p.level);
            }
            
            if (ZombieGame.SetMainLevel)
                Server.mainLevel = CurLevel;
            return true;
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
            CurLevel.ChatLevel(zombie.ColoredName + " %Scontinued the infection!");
            InfectPlayer(zombie);
        }

        public void InfectPlayer(Player p) {
            if (!RoundInProgress || p == null) return;
            Infected.Add(p);
            Alive.Remove(p);
            p.Game.CurrentRoundsSurvived = 0;
            p.SetPrefix();
            ResetInvisibility(p);
            
            p.Game.Infected = true;
            p.Game.TimeInfected = DateTime.UtcNow;
            UpdatePlayerColor(p, Colors.red);
            UpdateAllPlayerStatus();
            PlayerMoneyChanged(p);
        }

        public void DisinfectPlayer(Player p) {
            if (!RoundInProgress || p == null) return;
            Infected.Remove(p);
            Alive.Add(p);
            ResetInvisibility(p);
            
            p.Game.Infected = false;
            UpdatePlayerColor(p, p.color);
            UpdateAllPlayerStatus();
            PlayerMoneyChanged(p);
        }
        
        void ResetInvisibility(Player p) {
            if (!p.Game.Invisible) return;
            p.SendCpeMessage(CpeMessageType.BottomRight2, "", false);
            p.Game.ResetInvisibility();
            Entities.GlobalSpawn(p, false); 
        }

        void ChangeLevel(string next) {
            Player[] online = PlayerInfo.Online.Items;
            if (CurLevel != null) {
                Level.SaveSettings(CurLevel);
                CurLevel.ChatLevel("The next map has been chosen - " + Colors.red + next.ToLower());
                CurLevel.ChatLevel("Please wait while you are transfered.");
            }
            string lastLevel = CurLevelName;
            
            CurLevelName = next;
            QueuedLevel = null;
            CmdLoad.LoadLevel(null, next);
            CurLevel = LevelInfo.Find(next);
            if (ZombieGame.SetMainLevel)
                Server.mainLevel = CurLevel;
            
            online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                pl.Game.RatedMap = false;
                pl.Game.PledgeSurvive = false;
                if (!pl.level.name.CaselessEq(next) && pl.level.name.CaselessEq(lastLevel)) {
                    pl.SendMessage("Going to the next map - &a" + next);
                    PlayerActions.ChangeMap(pl, next);
                }
            }
            if (LastLevelName != "")
                Command.all.Find("unload").Use(null, LastLevelName);
            LastLevelName = next;
        }

        public void ResetState() {
            Status = ZombieGameStatus.NotStarted;
            MaxRounds = 0;
            RoundInProgress = false;
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            Player[] online = PlayerInfo.Online.Items;
            Lottery.Clear();
            Bounties.Clear();
            
            foreach (Player pl in online) {
                pl.Game.Referee = false;
                pl.Game.ResetZombieState();              
                ResetInvisibility(pl);
                pl.SetPrefix();
                
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
            p.SendCpeMessage(CpeMessageType.Status1, status);
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
                p.SendCpeMessage(CpeMessageType.Status1, message);
            }
        }

        string GetStatusMessage(string timespan) {
            if (timespan.Length > 0) {
                const string format = "&a{0} %Salive %S({2}, map: {1})";
                return String.Format(format, Alive.Count, CurLevelName, timespan);
            } else {
                const string format = "&a{0} %Salive %S(map: {1})";
                return String.Format(format, Alive.Count, CurLevelName);
            }
        }

        string GetTimespan(int seconds) {
            if (seconds < 0) return "";
            if (seconds <= 10) return "10s left";
            if (seconds <= 30) return "30s left";
            if (seconds <= 60) return "1m left";
            return ((seconds + 59) / 60) + "m left";
        }
        
        static string[] defMessages = new string[] { "{0} WIKIWOO'D {1}", "{0} stuck their teeth into {1}",
            "{0} licked {1}'s brain ", "{0} danubed {1}", "{0} made {1} meet their maker", "{0} tripped {1}",
            "{0} made some zombie babies with {1}", "{0} made {1} see the dark side", "{0} tweeted {1}",
            "{0} made {1} open source", "{0} infected {1}", "{0} iDotted {1}", "{1} got nommed on",
            "{0} transplanted {1}'s living brain" };
        
        public void LoadInfectMessages() {
            messages.Clear();
            try {
                if (!File.Exists("text/infectmessages.txt"))
                    File.WriteAllLines("text/infectmessages.txt", defMessages);
                messages = CP437Reader.ReadAllLines("text/infectmessages.txt");
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
            if (messages.Count == 0)
                messages = new List<string>(defMessages);
        }
        
        public bool IsZombieMap(string name) {
            if (!Running) return false;
            if (IgnorePersonalWorlds && name.IndexOf('+') >= 0) return false;
            if (IgnoredLevelList.CaselessContains(name)) return false;
            return LevelList.Count == 0 ? true : LevelList.CaselessContains(name);
        }
        
        #region Database
        
        const string createSyntax =
            @"CREATE TABLE if not exists ZombieStats (
ID INTEGER {0}{1} NOT NULL,
Name CHAR(20),
TotalRounds INT,
MaxRounds INT,
TotalInfected INT,
MaxInfected INT,
Additional1 INT,
Additional2 INT,
Additional3 INT,
Additional4 INT{2});"; // reserve space for possible future additions
        
        public void CheckTableExists() {
            string primKey = Server.useMySQL ? "" : "PRIMARY KEY ";
            string autoInc = Server.useMySQL ? "AUTO_INCREMENT" : "AUTOINCREMENT";
            string primKey2 = Server.useMySQL ? ", PRIMARY KEY (ID)" : "";
            Database.Execute(string.Format(createSyntax, primKey, autoInc, primKey2));
        }
        
        public ZombieStats LoadZombieStats(string name) {
            DataTable table = Database.Fill("SELECT * FROM ZombieStats WHERE Name=@0", name);
            ZombieStats stats = default(ZombieStats);
            
            if (table.Rows.Count > 0) {
                DataRow row = table.Rows[0];
                stats.TotalRounds = int.Parse(row["TotalRounds"].ToString());
                stats.MaxRounds = int.Parse(row["MaxRounds"].ToString());
                stats.TotalInfected = int.Parse(row["TotalInfected"].ToString());
                stats.MaxInfected = int.Parse(row["MaxInfected"].ToString());
            }
            table.Dispose();
            return stats;
        }
        
        public void SaveZombieStats(Player p) {
            if (p.Game.TotalRoundsSurvived == 0 && p.Game.TotalInfected == 0) return;
            DataTable table = Database.Fill("SELECT * FROM ZombieStats WHERE Name=@0", p.name);

            string syntax = table.Rows.Count == 0 ?
                "INSERT INTO ZombieStats (TotalRounds, MaxRounds, TotalInfected, MaxInfected, Name) VALUES (@0, @1, @2, @3, @4)"
                : "UPDATE ZombieStats SET TotalRounds=@0, MaxRounds=@1, TotalInfected=@2, MaxInfected=@3 WHERE Name=@4";
            Database.Execute(syntax, p.Game.TotalRoundsSurvived, p.Game.MaxRoundsSurvived,
                             p.Game.TotalInfected, p.Game.MaxInfected, p.name);
            table.Dispose();
        }
        #endregion
    }
}
