/*
	Copyright 2011 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Linq;
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
        public TntWarsDifficulty GameDifficulty = TntWarsDifficulty.Normal;
        public int GameNumber;
        public ushort[] RedSpawn = null;
        public ushort[] BlueSpawn = null;
            //incase they don't want the default
        public int TntPerPlayerAtATime = Properties.DefaultTntPerPlayerAtATime;
        public bool GracePeriod = Properties.DefaultGracePeriodAtStart;
        public int GracePeriodSecs = Properties.DefaultGracePeriodSecs;
        public bool BalanceTeams = Properties.DefaultBalanceTeams;
            //scores/streaks
        public int ScoreLimit = Properties.DefaultTDMmaxScore;
        public bool Streaks = true;
        public int MultiKillBonus = Properties.DefaultMultiKillBonus; //This is the amount of extra points per each player that is killed per 1 tnt (if playerskilledforthistnt > 1)
        public int ScorePerKill = Properties.DefaultScorePerKill;
        public int ScorePerAssist = Properties.DefaultAssistScore;
        public bool TeamKills = false;
        public Thread Starter;

        public static TntWarsGame GuiLoaded = null;
        //======PLUGIN EVENTS======
        public delegate void Starting(TntWarsGame t);
        public delegate void Started(TntWarsGame t);
        public delegate void Death(Player killer, List<Player> deadplayers);
        public delegate void End(TntWarsGame t);
        //======PLUGIN EVENTS======
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
                    Chat.GlobalMessageOps(Colors.red + "Backing up Level for TNT Wars failed, Stopping game");
                    GameStatus = TntWarsGameStatus.Finished;
                    return;
                }
                Server.s.Log("Backed up " + lvl.name + " (" + BackupNumber.ToString() + ") for TNT Wars");
            }
            //Map stuff
            lvl.setPhysics(3);
            lvl.permissionbuild = Group.Find(Server.defaultRank).Permission;
            lvl.permissionvisit = Group.Find(Server.defaultRank).Permission;
            lvl.Killer = true;
            //Seting Up Some Player stuff
            {
                foreach (player p in Players)
                {
                    p.p.PlayingTntWars = true;
                    p.p.CurrentAmountOfTnt = 0;
                    p.p.CurrentTntGameNumber = GameNumber;
                    if (GameDifficulty == TntWarsDifficulty.Easy || GameDifficulty == TntWarsDifficulty.Normal) p.p.TntWarsHealth = 2;
                    else p.p.TntWarsHealth = 1;
                    p.p.HarmedBy = null;
                    if (GracePeriod)
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
                if (GracePeriod)
                {
                    SendAllPlayersMessage("TNT Wars: Disabled building during Grace Period!");
                }
            }
            //Spawn them (And if needed, move them to the correct level!)
            {
                foreach (player p in Players.Where(p => p.p.level != lvl))
                {
                    Command.all.Find("goto").Use(p.p, lvl.name);
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
            string Difficulty = "Normal";
            string HitsToDie = "2";
            string explosiontime = "medium";
            string explosionsize = "normal";
            switch (GameDifficulty)
            {
                case TntWarsDifficulty.Easy:
                    Difficulty = "Easy";
                    explosiontime = "long";
                    break;

                case TntWarsDifficulty.Normal:
                    Difficulty = "Normal";
                    break;

                case TntWarsDifficulty.Hard:
                    HitsToDie = "1";
                    Difficulty = "Hard";
                    break;

                case TntWarsDifficulty.Extreme:
                    HitsToDie = "1";
                    explosiontime = "short";
                    explosionsize = "big";
                    Difficulty = "Extreme";
                    break;
            }
            string teamkillling = "Disabled";
            if (TeamKills) teamkillling = "Enabled";
            Player.GlobalMessage("&cTNT Wars %Son '" + lvl.name + "' has started &3" + Gamemode + " %Swith a difficulty of &3" +
                                 Difficulty + " %S(&3" + HitsToDie + " %Shits to die, a &3" + explosiontime + 
                                 " %Sexplosion delay and with a &3" + explosionsize + " %Sexplosion size)" + 
                                 ", team killing is &3" + teamkillling + " %Sand you can place &3" + TntPerPlayerAtATime 
                                 + " %STNT at a time and there is a score limit of &3" + ScoreLimit + "%S!!");
            if (GameMode == TntWarsGameMode.TDM) SendAllPlayersMessage("TNT Wars: Start your message with ':' to send it as a team chat!");
            //GracePeriod
            if (GracePeriod) //Check This Grace Stuff
            {
                GameStatus = TntWarsGameStatus.GracePeriod;
                int GracePeriodSecsRemaining = GracePeriodSecs;
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
                if (p.spec == false)
                {
                    p.p.canBuild = true;
                }
            }
            if (GracePeriod)
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
                Player.GlobalMessage("&cTNT Wars %Shas ended because there are no longer enough players!");
            }
            else
            {
                Player.GlobalMessage("&cTNT Wars %Shas ended!!");
            }
            if (GameMode == TntWarsGameMode.TDM)
            {
                if (RedScore >= BlueScore)
                {
                    Player.GlobalMessage("TNT Wars: Team &cRed %Swon &cTNT Wars %Sby " + (RedScore - BlueScore) + " points!");
                }
                if (BlueScore >= RedScore)
                {
                    Player.GlobalMessage("TNT Wars: Team &9Blue %Swon &cTNT Wars %Sby " + (BlueScore - RedScore) + " points!");
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
                var pls = from pla in Players orderby pla.Score descending select pla; //LINQ FTW
                int count = 1;
                foreach (var pl in pls)
                {
                    if (count == 1)
                    {
                        Player.GlobalMessage("&cTNT Wars %S1st Place: " + pl.p.ColoredName + " %Swith a score of " + pl.p.color + pl.Score);
                    }
                    else if (count == 2)
                    {
                        SendAllPlayersMessage("&cTNT Wars %S2nd Place: " + pl.p.ColoredName + " %Swith a score of " + pl.p.color + pl.Score);
                    }
                    else if (count == 3)
                    {
                        SendAllPlayersMessage("&cTNT Wars %S3rd Place: " + pl.p.ColoredName + " %Swith a score of " + pl.p.color + pl.Score);
                    }
                    else if (count >= 4)
                    {
                        SendAllPlayersMessage("&cTNT Wars %S" + count + "th Place: " + pl.p.ColoredName+ " %Swith a score of " + pl.p.color + pl.Score);
                    }
                    if (count >= PlayingPlayers())
                    {
                        break;
                    }
                    count++;
                    Thread.Sleep(750); //Maybe, not sure (was 500)
                }
            }
            //Reset map
            Command.all.Find("restore").Use(null, BackupNumber.ToString() + " " + lvl.name);
            if (lvl.overload == 2501)
            {
                lvl.overload = 1500;
                Server.s.Log("TNT Wars: Set level physics overload back to 1500");
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
            Server.s.Log("[TNT Wars] [" + lvl.name + "] " + Message);
        }

        public void HandleKill(Player Killer, List<Player> Killed)
        {
            List<Player> Dead = new List<Player>();
            int HealthDamage = 1;
            int kills = 0;
            int minusfromscore = 0;
            if (GameDifficulty == TntWarsDifficulty.Hard || GameDifficulty == TntWarsDifficulty.Extreme)
            {
                HealthDamage = 2;
            }
            foreach (Player Kld in Killed.Where(Kld => !FindPlayer(Kld).spec).Where(Kld => !TeamKill(Killer, Kld) || TeamKills != false))
            {
                if (Kld.TntWarsHealth - HealthDamage <= 0)
                {
                    Kld.TntWarsHealth = 0;
                    Dead.Add(Kld);
                    if (TeamKills && TeamKill(Killer, Kld))
                    {
                        minusfromscore += ScorePerKill;
                    }
                }
                else
                {
                    Kld.TntWarsHealth -= HealthDamage;
                    Kld.HarmedBy = Killer;
                    Player.Message(Killer, "TNT Wars: You harmed " + Kld.color + Kld.name);
                    Player.Message(Kld, "TNT Wars: You were harmed by " + Killer.color + Killer.name);
                }
            }
            foreach (Player Died in Dead)
            {
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
                            Player.Message(Died.HarmedBy, "TNT Wars: - " + ScorePerAssist.ToString() + " point(s) for team kill assist!");
                            ChangeScore(Died.HarmedBy, -ScorePerAssist);
                        }
                        else
                        {
                            Player.Message(Died.HarmedBy, "TNT Wars: + " + ScorePerAssist.ToString() + " point(s) for assist!");
                            ChangeScore(Died.HarmedBy, ScorePerAssist);
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
            if (kills >= 1 && Streaks)
            {
                if (Killer.TntWarsKillStreak >= Properties.DefaultStreakOneAmount && Killer.TntWarsKillStreak < Properties.DefaultStreakTwoAmount && Killer.TNTWarsLastKillStreakAnnounced != Properties.DefaultStreakOneAmount)
                {
                    Player.Message(Killer, "TNT Wars: Kill streak of " + Killer.TntWarsKillStreak.ToString() + " (Multiplier of " + Properties.DefaultStreakOneMultiplier.ToString() + ")");
                    SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Shas a kill streak of " + Killer.TntWarsKillStreak.ToString());
                    Killer.TntWarsScoreMultiplier = Properties.DefaultStreakOneMultiplier;
                    Killer.TNTWarsLastKillStreakAnnounced = Properties.DefaultStreakOneAmount;
                }
                else if (Killer.TntWarsKillStreak >= Properties.DefaultStreakTwoAmount && Killer.TntWarsKillStreak < Properties.DefaultStreakThreeAmount && Killer.TNTWarsLastKillStreakAnnounced != Properties.DefaultStreakTwoAmount)
                {
                    Player.Message(Killer, "TNT Wars: Kill streak of " + Killer.TntWarsKillStreak.ToString() + " (Multiplier of " + Properties.DefaultStreakTwoMultiplier.ToString() + " and a bigger explosion!)");
                    SendAllPlayersMessage("TNT Wars: " + Killer.ColoredName + " %Shas a kill streak of " + Killer.TntWarsKillStreak.ToString() + " and now has a bigger explosion for their TNT!");
                    Killer.TntWarsScoreMultiplier = Properties.DefaultStreakTwoMultiplier;
                    Killer.TNTWarsLastKillStreakAnnounced = Properties.DefaultStreakTwoAmount;
                }
                else if (Killer.TntWarsKillStreak >= Properties.DefaultStreakThreeAmount && Killer.TNTWarsLastKillStreakAnnounced != Properties.DefaultStreakThreeAmount)
                {
                    Player.Message(Killer, "TNT Wars: Kill streak of " + Killer.TntWarsKillStreak.ToString() + " (Multiplier of " + Properties.DefaultStreakThreeMultiplier.ToString() + " and you now have 1 extra health!)");
                    SendAllPlayersMessage("TNT Wars: " + Killer.color + Killer.name + " %Shas a kill streak of " + Killer.TntWarsKillStreak.ToString() + " and now has 1 extra health!");
                    Killer.TntWarsScoreMultiplier = Properties.DefaultStreakThreeMultiplier;
                    Killer.TNTWarsLastKillStreakAnnounced = Properties.DefaultStreakThreeAmount;
                    if (GameDifficulty == TntWarsDifficulty.Hard || GameDifficulty == TntWarsDifficulty.Extreme)
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
                    Player.Message(Killer, "TNT Wars: Kill streak of " + Killer.TntWarsKillStreak.ToString());
                }
            }
            AddToScore += kills * ScorePerKill;
            //multikill
            if (kills > 1)
            {
                AddToScore += kills * MultiKillBonus;
            }
            //Add to score
            if (AddToScore > 0)
            {
                ChangeScore(Killer, AddToScore, Killer.TntWarsScoreMultiplier);
                Player.Message(Killer, "TNT Wars: + " + ((int)(AddToScore * Killer.TntWarsScoreMultiplier)).ToString() + " point(s) for " + kills.ToString() + " kills");
            }
            if (minusfromscore != 0)
            {
                ChangeScore(Killer, - minusfromscore);
                Player.Message(Killer, "TNT Wars: - " + minusfromscore.ToString() + " point(s) for team kill(s)!");
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

        public bool InZone(ushort x, ushort y, ushort z, bool CheckForPlacingTnt)
        {
            return CheckForPlacingTnt ? NoTNTplacableZones.Any(Zn => Zn.smallX <= x && x <= Zn.bigX && Zn.smallY <= y && y <= Zn.bigY && Zn.smallZ <= z && z <= Zn.bigZ) :
                NoBlockDeathZones.Any(Zn => Zn.smallX <= x && x <= Zn.bigX && Zn.smallY <= y && y <= Zn.bigY && Zn.smallZ <= z && z <= Zn.bigZ);
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

        public void SendAllPlayersScore(bool TotalTeamScores = false, bool TheirTotalScore = false, bool TopScores = false)
        {
            try
            {
                if (TotalTeamScores)
                {
                    SendAllPlayersMessage("TNT Wars Scores:");
                    SendAllPlayersMessage(Colors.red + "RED: " + Colors.white + RedScore + " " + Colors.red + "(" + (ScoreLimit - RedScore).ToString() + " needed)");
                    SendAllPlayersMessage(Colors.blue + "BLUE: " + Colors.white + BlueScore + " " + Colors.red + "(" + (ScoreLimit - BlueScore).ToString() + " needed)");
                    Thread.Sleep(1000);
                }
                if (TopScores)
                {
                    int max = 5;
                    if (PlayingPlayers() < 5)
                    {
                        max = PlayingPlayers();
                    }

                    var pls = from pla in Players orderby pla.Score descending select pla; //LINQ FTW

                    foreach (player p in Players)
                    {
                        int count = 1;
                        foreach (var pl in pls)
                        {
                            Player.Message(p.p, count.ToString() + ": " + pl.p.name + " - " + pl.Score.ToString());
                            if (count >= max)
                            {
                                break;
                            }
                            count++;
                            Thread.Sleep(500); //Maybe, not sure (250??)
                        }
                    }
                    Thread.Sleep(1000);
                }
                if (TheirTotalScore)
                {
                    foreach (player p in Players.Where(p => !p.spec))
                    {
                        Player.Message(p.p, "TNT Wars: Your Score: " + Colors.white + p.Score.ToString());
                    }
                    Thread.Sleep(1000);
                }
            }
            catch { }
        }

        public bool Finished()
        {
            if (GameMode == TntWarsGameMode.TDM && (RedScore >= ScoreLimit || BlueScore >= ScoreLimit))
            {
                return true;
            }
            if (GameMode == TntWarsGameMode.FFA)
            {
                try
                {
                    if (Players.Any(p => p.Score >= ScoreLimit))
                    {
                        return true;
                    }
                }
                catch { }
            }
            if (PlayingPlayers() <= 1)
            {
                return true;
            }
            if (GameStatus == TntWarsGameStatus.Finished)
            {
                return true;
            }
            return false;
        }

        //enums
        public enum TntWarsGameStatus : int
        {
            WaitingForPlayers = 0,
            AboutToStart = 1,
            GracePeriod = 2,
            InProgress = 3,
            Finished = 4
        }

        public enum TntWarsGameMode : int
        {
            FFA = 0,
            TDM = 1
        }

        public enum TntWarsDifficulty : int
        {
            Easy = 0,       //2 Hits to die, Tnt has long delay
            Normal = 1,     //2 Hits to die, Tnt has normal delay
            Hard = 2,       //1 Hit to die, Tnt has short delay
            Extreme = 3     //1 Hit to die, Tnt has short delay and BIG exlosion
        }

        //Other stuff
        public int RedTeam()
        {
            return Players.Count(p => p.Red);
        }

        public int BlueTeam()
        {
            return Players.Count(p => p.Blue);
        }

        public int PlayingPlayers()
        {
            return Players.Count(p => !p.spec);
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
            if (lvl != null
                && GameStatus == 0)
            {
                if (GameList.Any(g => g.lvl == this.lvl && g != this))
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

        public static TntWarsGame Find(Level level)
        {
            return GameList.FirstOrDefault(g => g.lvl == level);
        }

        public static TntWarsGame FindFromGameNumber(int Number)
        {
            return GameList.FirstOrDefault(g => g.GameNumber == Number);
        }

        public player FindPlayer(Player pla)
        {
            return Players.FirstOrDefault(p => p.p == pla);
        }

        public static TntWarsGame GetTntWarsGame(Player p)
        {
            TntWarsGame it = TntWarsGame.Find(p.level);
            if (it != null)
            {
                return it;
            }
            it = FindFromGameNumber(p.CurrentTntGameNumber);
            return it;
        }

        //Static Stuff
        public static class Properties
        {
            public static bool Enable = true;
            public static bool DefaultGracePeriodAtStart = true;
            public static int DefaultGracePeriodSecs = 30;
            public static int DefaultTntPerPlayerAtATime = 1;
            public static bool DefaultBalanceTeams = true;
            public static int DefaultFFAmaxScore = 75;
            public static int DefaultTDMmaxScore = 150;
            public static int DefaultScorePerKill = 10;
            public static int DefaultMultiKillBonus = 5;
            public static int DefaultAssistScore = 5;
            public static int DefaultStreakOneAmount = 3;
            public static float DefaultStreakOneMultiplier = 1.25f;
            public static int DefaultStreakTwoAmount = 5;
            public static float DefaultStreakTwoMultiplier = 1.5f;
            public static int DefaultStreakThreeAmount = 7;
            public static float DefaultStreakThreeMultiplier = 2f;
        }
    }
}