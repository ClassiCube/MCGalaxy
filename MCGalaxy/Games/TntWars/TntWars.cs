/*
    Copyright 2011 MCForge
        
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
///////--|----------------------------------|--\\\\\\\
//////---|  TNT WARS - Coded by edh649      |---\\\\\\
/////----|                                  |----\\\\\
////-----|  Note: Double click on // to see |-----\\\\
///------|        them in the sidebar!!     |------\\\
//-------|__________________________________|-------\\
using System.Collections.Generic;
using System.Threading;

namespace MCGalaxy.Games
{
    public sealed class TntWarsGame
    {
        //Vars
        public static List<TntWarsGame> GameList = new List<TntWarsGame>();
        public Level lvl;
        public TntWarsGameStatus GameStatus = TntWarsGameStatus.WaitingForPlayers;
        public int BackupNumber;
        public bool AllSetUp = false;
        public TntWarsGameMode GameMode = TntWarsGameMode.TDM;
        public TntWarsDifficulty Difficulty = TntWarsDifficulty.Normal;
        public int GameNumber;
        public ushort[] RedSpawn = null;
        public ushort[] BlueSpawn = null;
        
        public TntWarsConfig Config = new TntWarsConfig();
        //scores/streaks
        public int ScoreLimit = TntWarsConfig.Default.ScoreMaxTDM;
        public Thread Starter;

        public TntWarsGame(Level level)
        {
            Starter = new Thread(Start);
            lvl = level;
        }

        //Player/Team stuff
        public List<player> Players = new List<player>();
        public class player
        {
            public Player p;
            public bool Red = false;
            public bool Blue = false;
            public bool spec = false;
            public int Score = 0;
            public string OldColor;
            public string OldTitle;
            public string OldTitleColor;
            public player(Player pl)
            {
                p = pl;
                OldColor = pl.color;
                OldTitle = pl.title;
                OldTitleColor = pl.titlecolor;
            }
        }
        public int RedScore = 0;
        public int BlueScore = 0;

        //Zones
        public List<Zone> NoTNTplacableZones = new List<Zone>();
        public List<Zone> NoBlockDeathZones = new List<Zone>();
        public class Zone
        {
            public ushort bigX;
            public ushort bigY;
            public ushort bigZ;
            public ushort smallX;
            public ushort smallY;
            public ushort smallZ;
        }

        //During Game Main Methods
        public void Start()
        {
            GameStatus = TntWarsGameStatus.AboutToStart;
            //Checking Backups & physics etc.
            {
                BackupNumber = lvl.Backup(true);
                if (BackupNumber <= 0)
                {
                    SendAllPlayersMessage(Colors.red + "Backing up Level for TNT Wars failed, Stopping game");
                    Chat.MessageOps(Colors.red + "Backing up Level for TNT Wars failed, Stopping game");
                    GameStatus = TntWarsGameStatus.Finished;
                    return;
                }
                Logger.Log(LogType.SystemActivity, "Backed up {0} ({1}) for TNT Wars", lvl.name, BackupNumber);
            }
            //Map stuff
            lvl.setPhysics(3);
            lvl.BuildAccess.Min = Group.standard.Permission;
            lvl.VisitAccess.Min = Group.standard.Permission;
            lvl.Config.KillerBlocks = true;
            //Seting Up Some Player stuff
            {
                foreach (player p in Players)
                {
                    p.p.PlayingTntWars = true;
                    p.p.CurrentAmountOfTnt = 0;
                    p.p.CurrentTntGameNumber = GameNumber;
                    if (Difficulty == TntWarsDifficulty.Easy || Difficulty == TntWarsDifficulty.Normal) p.p.TntWarsHealth = 2;
                    else p.p.TntWarsHealth = 1;
                    p.p.HarmedBy = null;
                    if (Config.InitialGracePeriod)
                    {
                        p.p.canBuild = false;
                    }
                    if (p.spec)
                    {
                        p.p.canBuild = false;
                        Player.Message(p.p, "TNT Wars: Disabled building because you are a spectator!");
                    }
                    p.p.TntWarsKillStreak = 0;
                    p.p.TntWarsScoreMultiplier = 1f;
                    p.p.TNTWarsLastKillStreakAnnounced = 0;
                    SetTitlesAndColor(p);
                }
                if (Config.InitialGracePeriod)
                {
                    SendAllPlayersMessage("TNT Wars: Disabled building during Grace Period!");
                }
            }
            //Spawn them (And if needed, move them to the correct level!)
            {
                foreach (player p in Players)
                {
                    if (p.p.level == lvl) continue;
                    PlayerActions.ChangeMap(p.p, lvl);
                    p.p.inTNTwarsMap = true;
                }
                if (GameMode == TntWarsGameMode.TDM) { Command.all.Find("reveal").Use(null, "all " + lvl.name); }//So peoples names apear above their heads in the right color!
                foreach (player p in Players)
                {
                    Command.all.Find("spawn").Use(p.p, ""); //This has to be after reveal so that they spawn in the correct place!!
                    Thread.Sleep(250);
                }
            }
            //Announcing Etc.
            string Gamemode = "Free For All";
            if (GameMode == TntWarsGameMode.TDM) Gamemode = "Team Deathmatch";
            string difficulty = "Normal";
            string HitsToDie = "2";
            string explosiontime = "medium";
            string explosionsize = "normal";
            switch (Difficulty)
            {
                case TntWarsDifficulty.Easy:
                    difficulty = "Easy";
                    explosiontime = "long";
                    break;

                case TntWarsDifficulty.Normal:
                    difficulty = "Normal";
                    break;

                case TntWarsDifficulty.Hard:
                    HitsToDie = "1";
                    difficulty = "Hard";
                    break;

                case TntWarsDifficulty.Extreme:
                    HitsToDie = "1";
                    explosiontime = "short";
                    explosionsize = "big";
                    difficulty = "Extreme";
                    break;
            }
            string teamkillling = "Disabled";
            if (Config.TeamKills) teamkillling = "Enabled";
            Chat.MessageGlobal("&cTNT Wars %Son " + lvl.ColoredName + " %Shas started &3" + Gamemode + " %Swith a difficulty of &3" +
                            difficulty + " %S(&3" + HitsToDie + " %Shits to die, a &3" + explosiontime + 
                            " %Sexplosion delay and with a &3" + explosionsize + " %Sexplosion size)" + 
                            ", team killing is &3" + teamkillling + " %Sand you can place &3" + Config.MaxPlayerActiveTnt 
                            + " %STNT at a time and there is a score limit of &3" + ScoreLimit + "%S!!");
            if (GameMode == TntWarsGameMode.TDM) SendAllPlayersMessage("TNT Wars: Start your message with ':' to send it as a team chat!");
            //GracePeriod
            if (Config.InitialGracePeriod) //Check This Grace Stuff
            {
                GameStatus = TntWarsGameStatus.GracePeriod;
                int GracePeriodSecsRemaining = Config.GracePeriodSeconds;
                SendAllPlayersMessage("TNT Wars: Grace Period of &a" + GracePeriodSecsRemaining + " %Sseconds");
                while (GracePeriodSecsRemaining > 0)
                {
                    switch (GracePeriodSecsRemaining)
                    {
                        case 300:
                            SendAllPlayersMessage("TNT Wars: &35 %Sminutes remaining!"); break;
                        case 240:
                            SendAllPlayersMessage("TNT Wars: &34 %Sminutes remaining!"); break;
                        case 180:
                            SendAllPlayersMessage("TNT Wars: &33 %Sminutes remaining!"); break;
                        case 120:
                            SendAllPlayersMessage("TNT Wars: &32 %Sminutes remaining!"); break;
                        case 90:
                            SendAllPlayersMessage("TNT Wars: &31 %Sminute and &330 %Sseconds remaining!"); break;
                        case 60:
                            SendAllPlayersMessage("TNT Wars: &31 %Sminute remaining!"); break;
                        case 45:
                            SendAllPlayersMessage("TNT Wars: &345 %Sseconds remaining!"); break;
                        case 30:
                            SendAllPlayersMessage("TNT Wars: &330 %Sseconds remaining!"); break;
                        case 15:
                            SendAllPlayersMessage("TNT Wars: &315 %Sseconds remaining!"); break;
                        case 10:
                            SendAllPlayersMessage("TNT Wars: &310 %Sseconds remaining!"); break;
                        case 9:
                            SendAllPlayersMessage("TNT Wars: &39 %Sseconds remaining!"); break;
                        case 8:
                            SendAllPlayersMessage("TNT Wars: &38 %Sseconds remaining!"); break;
                        case 7:
                            SendAllPlayersMessage("TNT Wars: &37 %Sseconds remaining!"); break;
                        case 6:
                            SendAllPlayersMessage("TNT Wars: &36 %Sseconds remaining!"); break;
                        case 5:
                            SendAllPlayersMessage("TNT Wars: &35 %Sseconds remaining!"); break;
                        case 4:
                            SendAllPlayersMessage("TNT Wars: &34 %Sseconds remaining!"); break;
                        case 3:
                            SendAllPlayersMessage("TNT Wars: &33 %Sseconds remaining!"); break;
                        case 2:
                            SendAllPlayersMessage("TNT Wars: &32 %Sseconds remaining!"); break;
                        case 1:
                            SendAllPlayersMessage("TNT Wars: &31 %Ssecond remaining!"); break;
                    }
                                
                    Thread.Sleep(1000);
                    GracePeriodSecsRemaining -= 1;
                }
                SendAllPlayersMessage("TNT Wars: Grace Period is over!!!!!");
                SendAllPlayersMessage("TNT Wars: You may now place " + Colors.red + "TNT");
            }
            SendAllPlayersMessage("TNT Wars: " + Colors.white + "The Game Has Started!!!!!");
            GameStatus = TntWarsGameStatus.InProgress;
            foreach (player p in Players)
            {
                if (!p.spec)
                {
                    p.p.canBuild = true;
                }
            }
            if (Config.InitialGracePeriod)
            {
                SendAllPlayersMessage("TNT Wars: You can now build!!");
            }
            //MainLoop
            while (!Finished())
            {
                int i = 1; //For making a top 5 (or whatever) players announcement every 3 loops (if TDM)
                Thread.Sleep(3 * 1000); if (Finished()) break;  //--\\
                Thread.Sleep(3 * 1000); if (Finished()) break;  //----\
                Thread.Sleep(3 * 1000); if (Finished()) break;  //-----> So that if it finsihes, we don't have to wait like 10 secs for the announcement!!
                Thread.Sleep(3 * 1000); if (Finished()) break;  //----/
                Thread.Sleep(3 * 1000); if (Finished()) break;  //--//
                if (GameMode == TntWarsGameMode.TDM)
                {
                    if (i < 3)
                    {
                        SendAllPlayersScore(true, true);
                    }
                    if (i >= 3)
                    {
                        SendAllPlayersScore(true, true, true);
                        i = 0;
                    }
                    i++;
                }
                else if (GameMode == TntWarsGameMode.FFA)
                {
                    SendAllPlayersScore(false, true, true);
                }
            }
            END();
        }
        public void END()
        {
            GameStatus = TntWarsGameStatus.Finished;
            //let them build and spawn them and change playingtntwars to false
            foreach (player p in Players)
            {
                p.p.canBuild = true;
                Command.all.Find("spawn").Use(p.p, "");
                p.p.PlayingTntWars = false;
            }
            //Message about winners etc.
            if (Players.Count <= 1)
            {
                Chat.MessageGlobal("&cTNT Wars %Shas ended because there are no longer enough players!");
            }
            else
            {
                Chat.MessageGlobal("&cTNT Wars %Shas ended!!");
            }
            if (GameMode == TntWarsGameMode.TDM)
            {
                if (RedScore >= BlueScore)
                {
                    Chat.MessageGlobal("TNT Wars: Team &cRed %Swon &cTNT Wars %Sby {0} points!", RedScore - BlueScore);
                }
                if (BlueScore >= RedScore)
                {
                    Chat.MessageGlobal("TNT Wars: Team &9Blue %Swon &cTNT Wars %Sby {1} points!", BlueScore - RedScore);
                }
                try
                {
                    foreach (player p in Players)
                    {
                        if (!p.spec)
                        {
                            Player.Message(p.p, "TNT Wars: You Scored " + p.Score + " points");
                        }
                    }
                }
                catch { }
                SendAllPlayersMessage("TNT Wars: Top Scores:");
                SendAllPlayersScore(false, false, true);
            }
            if (GameMode == TntWarsGameMode.FFA)
            {
                int count = PlayingPlayers();
                List<player> pls = SortedByScore();
                for (int i = 0; i < count; i++) {
                    player pl = pls[i];
                    if (i == 0) 
                    {
                        Chat.MessageGlobal("&cTNT Wars %S1st Place: " + pl.p.ColoredName + " %Swith a score of " + pl.p.color + pl.Score);
                    }
                    else if (i == 1)
                    {
                        SendAllPlayersMessage("&cTNT Wars %S2nd Place: " + pl.p.ColoredName + " %Swith a score of " + pl.p.color + pl.Score);
                    }
                    else if (i == 2)
                    {
                        SendAllPlayersMessage("&cTNT Wars %S3rd Place: " + pl.p.ColoredName + " %Swith a score of " + pl.p.color + pl.Score);
                    }
                    else
                    {
                        SendAllPlayersMessage("&cTNT Wars %S" + count + "th Place: " + pl.p.ColoredName+ " %Swith a score of " + pl.p.color + pl.Score);
                    }
                    Thread.Sleep(750); //Maybe, not sure (was 500)
                }
            }
            //Reset map
            Command.all.FindByName("restore").Use(null, BackupNumber + " " + lvl.name);
            if (lvl.Config.PhysicsOverload == 2501)
            {
                lvl.Config.PhysicsOverload = 1500;
                Logger.Log(LogType.GameActivity, "TNT Wars: Set level physics overload back to 1500");
            }
        }

        public void SendAllPlayersMessage(string Message)
        {
            try
            {
                foreach (player p in Players)
                {
                    Player.Message(p.p, Message);
                }
            }
            catch { };
            Logger.Log(LogType.GameActivity, "[TNT Wars] [" + lvl.name + "] " + Message);
        }

        public void HandleKill(Player Killer, List<Player> Killed)
        {
            List<Player> Dead = new List<Player>();
            int HealthDamage = 1;
            int kills = 0;
            int minusfromscore = 0;
            if (Difficulty == TntWarsDifficulty.Hard || Difficulty == TntWarsDifficulty.Extreme) {
                HealthDamage = 2;
            }
            
            foreach (Player Kld in Killed) {
                if (FindPlayer(Kld).spec) continue;
                if (!Config.TeamKills && TeamKill(Killer, Kld)) continue;
                
                if (Kld.TntWarsHealth - HealthDamage <= 0)
                {
                    Kld.TntWarsHealth = 0;
                    Dead.Add(Kld);
                    if (Config.TeamKills && TeamKill(Killer, Kld))
                    {
                        minusfromscore += Config.ScorePerKill;
                    }
                }
                else
                {
                    Kld.TntWarsHealth -= HealthDamage;
                    Kld.HarmedBy = Killer;
                    Player.Message(Killer, "TNT Wars: You harmed " + Kld.ColoredName);
                    Player.Message(Kld, "TNT Wars: You were harmed by " + Killer.ColoredName);
                }
            }
            
            foreach (Player Died in Dead) {
                Died.TntWarsKillStreak = 0;
                Died.TntWarsScoreMultiplier = 1f;
                Died.TNTWarsLastKillStreakAnnounced = 0;
                if (Died.HarmedBy == null || Died.HarmedBy == Killer)
                {
                    if (TeamKill(Killer, Died))
                    {
                        SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Steam killed " + Died.ColoredName);
                    }
                    else
                    {
                        SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Skilled " + Died.ColoredName);
                        kills += 1;
                    }
                }
                else
                {
                    {
                        if (TeamKill(Killer, Died))
                        {
                            SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Steam killed " + Died.ColoredName + " %S(with help from " + Died.HarmedBy.ColoredName + ")");
                        }
                        else
                        {
                            SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Skilled " + Died.ColoredName + " %S(with help from " + Died.HarmedBy.ColoredName + ")");
                            kills += 1;
                        }
                    }
                    {
                        if (TeamKill(Died.HarmedBy, Died))
                        {
                            Player.Message(Died.HarmedBy, "TNT Wars: - " + Config.AssistScore + " point(s) for team kill assist!");
                            ChangeScore(Died.HarmedBy, -Config.AssistScore);
                        }
                        else
                        {
                            Player.Message(Died.HarmedBy, "TNT Wars: + " + Config.AssistScore + " point(s) for assist!");
                            ChangeScore(Died.HarmedBy, Config.AssistScore);
                        }
                    }
                    Died.HarmedBy = null;
                }
                Command.all.Find("spawn").Use(Died, "");
                Died.TntWarsHealth = 2;
            }
            //Scoring
            int AddToScore = 0;
            //streaks
            Killer.TntWarsKillStreak += kills;
            if (kills >= 1 && Config.Streaks)
            {
                if (Killer.TntWarsKillStreak >= Config.StreakOneAmount && Killer.TntWarsKillStreak < Config.StreakTwoAmount && Killer.TNTWarsLastKillStreakAnnounced != Config.StreakOneAmount)
                {
                    Player.Message(Killer, "TNT Wars: Kill streak of " + Killer.TntWarsKillStreak + " (Multiplier of " + Config.StreakOneMultiplier + ")");
                    SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Shas a kill streak of " + Killer.TntWarsKillStreak);
                    Killer.TntWarsScoreMultiplier = Config.StreakOneMultiplier;
                    Killer.TNTWarsLastKillStreakAnnounced = Config.StreakOneAmount;
                }
                else if (Killer.TntWarsKillStreak >= Config.StreakTwoAmount && Killer.TntWarsKillStreak < Config.StreakThreeAmount && Killer.TNTWarsLastKillStreakAnnounced != Config.StreakTwoAmount)
                {
                    Player.Message(Killer, "TNT Wars: Kill streak of " + Killer.TntWarsKillStreak + " (Multiplier of " + Config.StreakTwoMultiplier + " and a bigger explosion!)");
                    SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Shas a kill streak of " + Killer.TntWarsKillStreak + " and now has a bigger explosion for their TNT!");
                    Killer.TntWarsScoreMultiplier = Config.StreakTwoMultiplier;
                    Killer.TNTWarsLastKillStreakAnnounced = Config.StreakTwoAmount;
                }
                else if (Killer.TntWarsKillStreak >= Config.StreakThreeAmount && Killer.TNTWarsLastKillStreakAnnounced != Config.StreakThreeAmount)
                {
                    Player.Message(Killer, "TNT Wars: Kill streak of " + Killer.TntWarsKillStreak + " (Multiplier of " + Config.StreakThreeMultiplier + " and you now have 1 extra health!)");
                    SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Shas a kill streak of " + Killer.TntWarsKillStreak + " and now has 1 extra health!");
                    Killer.TntWarsScoreMultiplier = Config.StreakThreeMultiplier;
                    Killer.TNTWarsLastKillStreakAnnounced = Config.StreakThreeAmount;
                    if (Difficulty == TntWarsDifficulty.Hard || Difficulty == TntWarsDifficulty.Extreme)
                    {
                        Killer.TntWarsHealth += 2;
                    }
                    else
                    {
                        Killer.TntWarsHealth += 1;
                    }
                }
                else
                {
                    Player.Message(Killer, "TNT Wars: Kill streak of " + Killer.TntWarsKillStreak);
                }
            }
            AddToScore += kills * Config.ScorePerKill;
            //multikill
            if (kills > 1)
            {
                AddToScore += kills * Config.MultiKillBonus;
            }
            //Add to score
            if (AddToScore > 0)
            {
                ChangeScore(Killer, AddToScore, Killer.TntWarsScoreMultiplier);
                Player.Message(Killer, "TNT Wars: + " + ((int)(AddToScore * Killer.TntWarsScoreMultiplier)) + " point(s) for " + kills + " kills");
            }
            if (minusfromscore != 0)
            {
                ChangeScore(Killer, - minusfromscore);
                Player.Message(Killer, "TNT Wars: - " + minusfromscore + " point(s) for team kill(s)!");
            }
        }

        public void ChangeScore(Player p, int Amount, float multiplier = 1f)
        {
            ChangeScore(FindPlayer(p), Amount, multiplier);
        }

        public void ChangeScore(player p, int Amount, float multiplier = 1f)
        {
            p.Score += (int)(Amount * multiplier);
            if (GameMode != TntWarsGameMode.TDM) return;
            if (p.Red)
            {
                RedScore += (int)(Amount * multiplier);
            }
            if (p.Blue)
            {
                BlueScore += (int)(Amount * multiplier);
            }
        }

        public bool InZone(ushort x, ushort y, ushort z, bool CheckForPlacingTnt) {
            List<Zone> zones = CheckForPlacingTnt ? NoTNTplacableZones : NoBlockDeathZones; 
            return InZone(x, y, z, zones);
        }
        
        bool InZone(ushort x, ushort y, ushort z, List<Zone> zones) {
            foreach (Zone Zn in zones) {
                if (x >= Zn.smallX && y >= Zn.smallY && z >= Zn.smallZ 
                    && x <= Zn.bigX && y <= Zn.bigY && z <= Zn.bigZ) return true;
            }
            return false;
        }

        public void DeleteZone(ushort x, ushort y, ushort z, bool NoTntZone, Player p = null)
        {
            try
            {
                Zone Z = new Zone();
                if (NoTntZone)
                {
                    foreach (Zone Zn in NoTNTplacableZones)
                    {
                        if (Zn.smallX <= x && x <= Zn.bigX && Zn.smallY <= y && y <= Zn.bigY && Zn.smallZ <= z && z <= Zn.bigZ)
                        {
                            Z = Zn;
                        }
                    }
                    NoTNTplacableZones.Remove(Z);
                    if (p != null)
                    {
                        Player.Message(p, "TNT Wars: Zone Deleted!");
                    }
                    return;
                }
                else
                {
                    foreach (Zone Zn in NoBlockDeathZones)
                    {
                        if (Zn.smallX <= x && x <= Zn.bigX && Zn.smallY <= y && y <= Zn.bigY && Zn.smallZ <= z && z <= Zn.bigZ)
                        {
                            Z = Zn;
                        }
                    }
                    NoBlockDeathZones.Remove(Z);
                    if (p != null)
                    {
                        Player.Message(p, "TNT Wars: Zone Deleted!");
                    }
                    return;
                }
            }
            catch
            {
                Player.Message(p, "TNT Wars Error: Zone not deleted!");
            }

        }
        
        public bool TeamKill (Player p1, Player p2)
        {
            return TeamKill(FindPlayer(p1), FindPlayer(p2));
        }

        public bool TeamKill(player p1, player p2)
        {
            if (GameMode == TntWarsGameMode.TDM)
            {
                if (p1.Red && p2.Red) return true;
                if (p1.Blue && p2.Blue) return true;
            }
            return false;
        }
        
        public List<player> SortedByScore() {
            List<TntWarsGame.player> sorted = new List<TntWarsGame.player>(Players);
            sorted.Sort((a, b) => b.Score.CompareTo(a.Score));
            return sorted;
        }

        public void SendAllPlayersScore(bool TotalTeamScores = false, bool TheirTotalScore = false, bool TopScores = false)
        {
            try
            {
                if (TotalTeamScores)
                {
                    SendAllPlayersMessage("TNT Wars Scores:");
                    SendAllPlayersMessage(Colors.red + "RED: " + Colors.white + RedScore + " " + Colors.red + "(" + (ScoreLimit - RedScore) + " needed)");
                    SendAllPlayersMessage(Colors.blue + "BLUE: " + Colors.white + BlueScore + " " + Colors.red + "(" + (ScoreLimit - BlueScore) + " needed)");
                    Thread.Sleep(1000);
                }
                if (TopScores)
                {
                    int count = System.Math.Min(PlayingPlayers(), 5);
                    List<player> pls = SortedByScore();
                   
                    for (int i = 0; i < count; i++) {
                        foreach (player p in Players) {
                            Player.Message(p.p, "{0}: {1} - {2}", (i + 1), pls[i].p.name, pls[i].Score);
                        }
                        Thread.Sleep(500); //Maybe, not sure (250??)
                    }
                    Thread.Sleep(1000);
                }
                if (TheirTotalScore)
                {
                    foreach (player p in Players)
                    {
                        if (p.spec) continue;
                        Player.Message(p.p, "TNT Wars: Your Score: " + Colors.white + p.Score);
                    }
                    Thread.Sleep(1000);
                }
            }
            catch { }
        }

        public bool Finished() {
            if (GameMode == TntWarsGameMode.TDM && (RedScore >= ScoreLimit || BlueScore >= ScoreLimit))
                return true;
            
            if (GameMode == TntWarsGameMode.FFA) {
                try
                {
                    foreach (player p in Players) {
                        if (p.Score >= ScoreLimit) return true;
                    }
                }
                catch { }
            }
            if (PlayingPlayers() <= 1) return true;
            return GameStatus == TntWarsGameStatus.Finished;
        }

        //enums
        public enum TntWarsGameStatus {
            WaitingForPlayers = 0,
            AboutToStart = 1,
            GracePeriod = 2,
            InProgress = 3,
            Finished = 4
        }

        public enum TntWarsGameMode {
            FFA = 0,
            TDM = 1
        }

        public enum TntWarsDifficulty {
            Easy = 0,       //2 Hits to die, Tnt has long delay
            Normal = 1,     //2 Hits to die, Tnt has normal delay
            Hard = 2,       //1 Hit to die, Tnt has short delay
            Extreme = 3     //1 Hit to die, Tnt has short delay and BIG exlosion
        }

        //Other stuff
        public int RedTeam() {
            int count = 0;
            foreach (player p in Players) {
                if (p.Red) count++;
            }
            return count;
        }

        public int BlueTeam() {
            int count = 0;
            foreach (player p in Players) {
                if (p.Blue) count++;
            }
            return count;
        }

        public int PlayingPlayers() {
            int count = 0;
            foreach (player p in Players) {
                if (!p.spec) count++;
            }
            return count;
        }

        public static void SetTitlesAndColor(player p, bool reset = false)
        {
            try
            {
                if (reset)
                {
                    p.p.title = p.OldTitle;
                    p.p.color = p.OldTitleColor;
                    p.p.color = p.OldColor;
                    p.p.SetPrefix();
                }
                else
                {
                    p.p.title = "TNT Wars";
                    p.p.titlecolor = Colors.white;
                    if (p.Red) p.p.color = Colors.red;
                    if (p.Blue) p.p.color = Colors.blue;
                    p.p.SetPrefix();
                }
            }
            catch { }
        }

        public bool CheckAllSetUp(Player p, bool ReturnErrors = false, bool TellPlayerOnSuccess = true)
        {
            if (lvl != null && GameStatus == 0)
            {
                TntWarsGame existing = Find(lvl);
                if (existing != null && existing != this)
                {
                    if (ReturnErrors) Player.Message(p, "There is already a TNT Wars game on that map");
                    AllSetUp = false;
                    return false;
                }
                if (TellPlayerOnSuccess) Player.Message(p, "TNT Wars setup is done!");
                AllSetUp = true;
                return true;
            }
            if (ReturnErrors) SendPlayerCheckSetupErrors(p);
            AllSetUp = false;
            return false;

        }

        public void SendPlayerCheckSetupErrors(Player p)
        {
            if (lvl == null)
            {
                Player.Message(p, "TNT Wars Error: No Level Selected");
            }
            else if (GameStatus != 0)
            {
                Player.Message(p, "Game is already in progress");
            }
        }

        public static TntWarsGame Find(Level level) {
            foreach (TntWarsGame g in GameList) {
                if (g.lvl == level) return g;
            }
            return null;
        }

        public static TntWarsGame FindFromGameNumber(int num) {
            foreach (TntWarsGame g in GameList) {
                if (g.GameNumber == num) return g;
            }
            return null;
        }

        public player FindPlayer(Player pla) {
            foreach (player p in Players) {
                if (p.p == pla) return p;
            }
            return null;
        }

        public static TntWarsGame GameIn(Player p)
        {
            TntWarsGame it = TntWarsGame.Find(p.level);
            if (it != null) return it;
            it = FindFromGameNumber(p.CurrentTntGameNumber);
            return it;
        }
    }
}