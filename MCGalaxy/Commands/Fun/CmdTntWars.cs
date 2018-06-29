/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Collections.Generic;
using System.Threading;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdTntWars : Command {
        public override string name { get { return "TntWars"; } }
        public override string shortcut { get { return "tw"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage TNT wars") }; }
        }

        public override void Use(Player p, string message) {
            string[] text = new string[] { "", "", "", "", "" };
            string[] parts = message.ToLower().SplitSpaces();
            for (int i = 0; i < Math.Min(text.Length, parts.Length); i++)
                text[i] = parts[i];

            switch (text[0]) {
                case "list":
                    DoLevels(p); break;
                case "join":
                    DoJoin(p, text); break;
                case "leave":
                    DoLeave(p); break;
                case "rules":
                    DoRules(p); break;
                case "scores":
                    DoScores(p, text); return;
                case "players":
                    DoPlayers(p, text); break;
                case "health":
                case "hp":
                    DoHealth(p, text); break;
                case "setup":
                case "s":
                    DoSetup(p, text); break;
                default:
                    Help(p); break;
            }
        }
        
        void DoLevels(Player p) {
            if (TntWarsGame1.GameList.Count <= 0) {
                Player.Message(p, "There aren't any &cTNT Wars %Scurrently running!"); return;
            }
            
            Player.Message(p, "Currently running &cTNT Wars %S:");
            foreach (TntWarsGame1 it in TntWarsGame1.GameList) {
                string msg = it.GameMode + " at " + it.Difficulty + " difficulty on " + it.lvl.name + " ";
                
                if (it.GameStatus == TntWarsGame1.TntWarsStatus.WaitingForPlayers) msg += "(Waiting For Players)";
                if (it.GameStatus == TntWarsGame1.TntWarsStatus.AboutToStart) msg += "(Starting)";
                if (it.GameStatus == TntWarsGame1.TntWarsStatus.GracePeriod) msg += "(Started)";
                if (it.GameStatus == TntWarsGame1.TntWarsStatus.InProgress) msg += "(In Progress)";
                if (it.GameStatus == TntWarsGame1.TntWarsStatus.Finished) msg += "(Finished)";
                Player.Message(p, msg);
            }
        }
        
        void DoJoin(Player p, string[] text) {
            TntWarsGame1 it = TntWarsGame1.GameIn(p);
            if (p.PlayingTntWars || (it != null && it.FindPlayer(p) != null)) {
                Player.Message(p, "TNT Wars Error: You have already joined a game!"); return;
            }
            
            it = TntWarsGame1.Find(p.level);
            if (it == null) {
                Player.Message(p, "TNT Wars Error: There isn't a game on your current level!"); return;
            }
            
            TntWarsGame1.player pl = new TntWarsGame1.player(p);
            if (it.GameStatus == TntWarsGame1.TntWarsStatus.AboutToStart ||
                it.GameStatus == TntWarsGame1.TntWarsStatus.GracePeriod ||
                it.GameStatus == TntWarsGame1.TntWarsStatus.InProgress) {
                pl.spec = true;
            }
            
            if (it.GameMode == TntWarsGameMode.TDM) {
                int red = it.RedTeam(), blue = it.BlueTeam();
                switch (text[1]) {
                    case "red":
                        if (it.Config.BalanceTeams && red > blue) {
                            Player.Message(p, "TNT Wars Error: Red has too many players!"); return;
                        }
                        pl.Red = true; break;

                    case "blue":
                        if (it.Config.BalanceTeams && blue > red) {
                            Player.Message(p, "TNT Wars Error: Blue has too many players!"); return;
                        }
                        pl.Blue = true; break;
                    default:
                        it.AutoAssignTeam(pl); break;
                }
            } else {
                pl.Red = false;
                pl.Blue = false;
            }
            
            it.Players.Add(pl);
            TntWarsGame1.SetTitlesAndColor(pl);
            p.CurrentTntGameNumber = it.GameNumber;
            string msg = p.ColoredName + " %Sjoined TNT Wars on " + it.lvl.ColoredName;
            
            if (pl.Red)  msg += " %Son the " + Colors.red + "red team";
            if (pl.Blue) msg += " %Son the " + Colors.blue + "blue team";
            if (pl.spec) msg += " %S(as a spectator)";
            
            Chat.MessageGlobal(msg);
        }
        
        void DoLeave(Player p) {
            p.canBuild = true;
            TntWarsGame1 game = TntWarsGame1.GameIn(p);
            game.Players.Remove(game.FindPlayer(p));
            game.MessageAll("TNT Wars: " + p.ColoredName + " %Sleft the TNT Wars game!");
            TntWarsGame1.SetTitlesAndColor(game.FindPlayer(p), true);
            Player.Message(p, "TNT Wars: You left the game");
        }
        
        void DoRules(Player p) {
            Player.Message(p, "TNT Wars Rules:");
            Player.Message(p, "The aim of the game is to blow up people using TNT!");
            Player.Message(p, "To place tnt simply place a TNT block and after a short delay it shall explode!");
            Player.Message(p, "During the game the amount of TNT placable at one time may be limited!");
            Player.Message(p, "You are not allowed to use hacks of any sort during the game!");
        }
        
        void DoScores(Player p, string[] text) {
            TntWarsGame1 it = TntWarsGame1.GameIn(p);
            if (it.GameStatus != TntWarsGame1.TntWarsStatus.InProgress) {
                Player.Message(p, "TNT Wars Error: Can't display scores - game not in progress!");
                return;
            }
            
            if (text[1] == "top") {
                List<TntWarsGame1.player> sorted = it.SortedByScore();
                int count = Math.Min(it.PlayingPlayers(), 5);
                
                for (int i = 0; i < count; i++) {
                    Player.Message(p, "{0}: {1} - {2}", (i + 1), sorted[i].p.name, sorted[i].Score);
                    Thread.Sleep(500); //Maybe, not sure (250??)
                }
            } else if (text[1] == "team") {
                if (it.GameMode == TntWarsGameMode.TDM) {
                    Player.Message(p, "TNT Wars Scores:");
                    Player.Message(p, Colors.red + "RED: &f" + it.RedScore + " " + Colors.red + "(" + (it.Config.ScoreRequired - it.RedScore) + " needed)");
                    Player.Message(p, Colors.blue + "BLUE: &f" + it.BlueScore + " " + Colors.red + "(" + (it.Config.ScoreRequired - it.BlueScore) + " needed)");
                } else {
                    Player.Message(p, "TNT Wars Error: Can't display team scores as this isn't team deathmatch!");
                }
            } else {
                Player.Message(p, "TNT Wars: Your score is &a" + TntWarsGame1.GameIn(p).FindPlayer(p).Score);
            }
        }
        
        void DoPlayers(Player p, string[] text) {
            Player.Message(p, "TNT Wars: People playing TNT Wars on '" + TntWarsGame1.GameIn(p).lvl.name + "':");
            foreach (TntWarsGame1.player pl in TntWarsGame1.GameIn(p).Players) {
                if (TntWarsGame1.GameIn(p).GameMode == TntWarsGameMode.TDM) {
                    if (pl.Red && pl.spec)
                        Player.Message(p, pl.p.ColoredName + " %S- " + Colors.red + "RED %S(spectator)");
                    else if (pl.Blue && pl.spec)
                        Player.Message(p, pl.p.ColoredName + " %S- " + Colors.blue + "BLUE %S(spectator)");
                    else if (pl.Red)
                        Player.Message(p, pl.p.ColoredName + " %S- " + Colors.red + "RED");
                    else if (pl.Blue)
                        Player.Message(p, pl.p.ColoredName + " %S- " + Colors.blue + "BLUE");
                } else {
                    if (pl.spec)
                        Player.Message(p, pl.p.ColoredName + " %S(spectator)");
                    else
                        Player.Message(p, pl.p.ColoredName);
                }
            }
        }
        
        void DoHealth(Player p, string[] text) {
            if (TntWarsGame1.GameIn(p).GameStatus == TntWarsGame1.TntWarsStatus.InProgress) {
                Player.Message(p, "TNT Wars: You have " + p.TntWarsHealth + " health left");
            } else {
                Player.Message(p, "TNT Wars Error: Can't display health - game not in progress!");
            }
        }
        
        void DoSetup(Player p, string[] text) {
            if (!CheckExtraPerm(p, 1)) return;
            
            TntWarsGame1 it = TntWarsGame1.FindFromGameNumber(p.CurrentTntGameNumber);
            if (it == null && text[1] != "new") {
                Player.Message(p, "TNT Wars Error: You must create a new game by typing '/tntwars setup new'"); return;
            }
            
            if (it != null) {
                if (it.GameStatus == TntWarsGame1.TntWarsStatus.InProgress || it.GameStatus == TntWarsGame1.TntWarsStatus.GracePeriod
                    || it.GameStatus == TntWarsGame1.TntWarsStatus.AboutToStart) {
                    if (text[1] != "stop" && text[1] != "s" && text[1] != "" && text[1] != "status" &&
                        text[1] != "ready" && text[1] != "check" && text[1] != "info" && text[1] != "r" && text[1] != "c") {
                        Player.Message(p, "TNT Wars Error: Cannot edit current game because it is currently running!"); return;
                    }
                }
            }
            
            switch (text[1]) {
                case "new":
                    if (it != null && it.FindPlayer(p) != null) {
                        Player.Message(p, "TNT Wars Error: Please leave the current game first!"); return;
                    }
                    
                    if (it == null || it.lvl != p.level) {
                        it = new TntWarsGame1(p.level);
                        it.GameNumber = TntWarsGame1.GameList.Count + 1;
                        TntWarsGame1.GameList.Add(it);
                        p.CurrentTntGameNumber = it.GameNumber;
                        Player.Message(p, "TNT Wars: Created New TNT Wars game on '" + p.level.name + "'");
                    } else {
                        Player.Message(p, "TNT Wars Error: Please delete the current game first!");
                    }
                    return;

                case "delete":
                case "remove":
                    if (it.GameStatus != TntWarsGame1.TntWarsStatus.Finished && it.GameStatus != TntWarsGame1.TntWarsStatus.WaitingForPlayers) {
                        Player.Message(p, "Please stop the game first!"); return;
                    }
                    
                    foreach (TntWarsGame1.player pl in it.Players) {
                        pl.p.CurrentTntGameNumber = -1;
                        Player.Message(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                        pl.p.PlayingTntWars = false;
                        pl.p.canBuild = true;
                        TntWarsGame1.SetTitlesAndColor(pl, true);
                    }
                    Player.Message(p, "TNT Wars: Game deleted");
                    TntWarsGame1.GameList.Remove(it);
                    return;
                    //break;

                case "reset":
                    if (it.GameStatus != TntWarsGame1.TntWarsStatus.Finished) {
                        Player.Message(p, "TNT Wars Error: The game has to have finished to be reset!"); return;
                    }
                    
                    it.GameStatus = TntWarsGame1.TntWarsStatus.WaitingForPlayers;
                    Command.Find("Restore").Use(null, it.BackupNumber + it.lvl.name);
                    it.RedScore = 0;
                    it.BlueScore = 0;
                    
                    foreach (TntWarsGame1.player pl in it.Players) {
                        pl.Score = 0;
                        pl.spec = false;
                        pl.p.TntWarsKillStreak = 0;
                        pl.p.TNTWarsLastKillStreakAnnounced = 0;
                        pl.p.CurrentAmountOfTnt = 0;
                        pl.p.CurrentTntGameNumber = it.GameNumber;
                        pl.p.PlayingTntWars = false;
                        pl.p.canBuild = true;
                        pl.p.TntWarsHealth = 2;
                        pl.p.TntWarsScoreMultiplier = 1f;
                        pl.p.inTNTwarsMap = true;
                        pl.p.HarmedBy = null;
                    }
                    Player.Message(p, "TNT Wars: Reset TNT Wars");
                    break;

                case "start":
                    if (it.GameStatus == TntWarsGame1.TntWarsStatus.WaitingForPlayers) {
                        if (it.CheckAllSetUp(p, true)) {
                            if (it.PlayingPlayers() >= 2) {
                                Thread t = new Thread(it.Start);
                                t.Name = "MCG_TntGame";
                                t.Start();
                            } else {
                                Player.Message(p, "TNT Wars Error: Not Enough Players (2 or more needed)");
                            }
                        }
                    } else if (it.GameStatus == TntWarsGame1.TntWarsStatus.Finished) {
                        Player.Message(p, "TNT Wars Error: Please use '/tntwars setup reset' to reset the game before starting!");
                    } else {
                        Player.Message(p, "TNT Wars Error: Game already in progress!!");
                    }
                    return;

                case "stop":
                    if (it.GameStatus == TntWarsGame1.TntWarsStatus.Finished || it.GameStatus == TntWarsGame1.TntWarsStatus.WaitingForPlayers) {
                        Player.Message(p, "TNT Wars Error: Game already ended / not started!"); return;
                    }
                    
                    foreach (TntWarsGame1.player pl in it.Players) {
                        pl.p.canBuild = true;
                        pl.p.PlayingTntWars = false;
                        pl.p.CurrentAmountOfTnt = 0;
                    }
                    it.GameStatus = TntWarsGame1.TntWarsStatus.Finished;
                    it.MessageAll("TNT Wars: Game has been stopped!");
                    break;

                case "spawn":
                    if (it.GameMode == TntWarsGameMode.FFA) { Player.Message(p, "TNT Wars Error: Cannot set spawns because you are on Team Deathmatch!"); return; }
                    switch (text[2])
                    {
                        case "red":
                            it.Config.RedSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                            Player.Message(p, "TNT Wars: Set &cRed %Sspawn");
                            break;
                        case "blue":
                            it.Config.BlueSpawn = (Vec3U16)p.Pos.FeetBlockCoords;
                            Player.Message(p, "TNT Wars: Set &9Blue %Sspawn");
                            break;
                    }
                    break;

                case "level":
                case "lvl":
                    if (text[2].Length == 0) {
                        it.lvl = p.level;
                    } else {
                        it.lvl = Matcher.FindLevels(p, text[2]);
                        if (it.lvl == null) return;
                    }
                    
                    Player.Message(p, "TNT Wars: Level is now " + it.lvl.ColoredName);
                    it.RedSpawn = null;
                    it.BlueSpawn = null;
                    it.NoTNTplacableZones.Clear();
                    it.NoBlockDeathZones.Clear();
                    it.CheckAllSetUp(p, true);
                    break;

                case "tntatatime":
                case "tnt":
                    int amount = 1;
                    if (!CommandParser.GetInt(p, text[2], "TNT at a time", ref amount, 0)) return;
                    it.Config.MaxPlayerActiveTnt = amount;
                    
                    Player.Message(p, "TNT Wars: Number of TNTs placeable by a player at a time is now {0}",
                                   amount == 0 ? "unlimited" : amount.ToString());
                    break;
                    
                case "graceperiod":
                    SetBool(p, ref it.Config.InitialGracePeriod, text[2], "Grace period");
                    break;

                case "gracetime":
                    int time = 1;
                    if (!CommandParser.GetInt(p, text[2], "Grace time", ref time, 10, 300)) return;
                    it.Config.GracePeriodSeconds = time;
                    
                    Player.Message(p, "TNT Wars: Grace period is now {0} seconds long", time);
                    break;

                case "mode":
                case "gamemode":
                    if (text[2] == "tdm") {
                        if (it.GameMode == TntWarsGameMode.FFA) {
                            if (it.FindPlayer(p) == null) {
                                Player.Message(p, "TNT Wars: Changed gamemode to Team Deathmatch");
                            }
                            it.ModeTDM();
                        } else {
                            Player.Message(p, "TNT Wars Error: Gamemode is already Team Deathmatch!"); return;
                        }
                    } else if (text[2] == "ffa") {
                        if (it.GameMode == TntWarsGameMode.TDM) {
                            if (it.FindPlayer(p) == null) {
                                Player.Message(p, "TNT Wars: Changed gamemode to Free For All");
                            }
                            it.ModeFFA();
                        } else {
                            Player.Message(p, "TNT Wars Error: Gamemode is already Free For All!"); return;
                        }
                    }
                    break;

                case "difficulty":
                    switch (text[2])
                    {
                        case "easy":
                        case "1":
                            SetDifficulty(it, TntWarsDifficulty.Easy, false, "easy", p); break;

                        case "normal":
                        case "2":
                        default:
                            SetDifficulty(it, TntWarsDifficulty.Normal, false, "normal", p); break;
                            
                        case "hard":
                        case "3":
                            SetDifficulty(it, TntWarsDifficulty.Hard, true, "hard", p); break;

                        case "extreme":
                        case "4":
                            SetDifficulty(it, TntWarsDifficulty.Extreme, true, "extreme", p); break;
                    }
                    break;

                case "score":
                    switch (text[2])
                    {
                        case "required":
                            int score = 1;
                            if (!CommandParser.GetInt(p, text[3], "Points", ref score, 0)) return;
                            it.Config.ScoreRequired = score;
                            
                            Player.Message(p, "TNT Wars: Score required to win is now &a{0} %Spoints!", score);
                            break;

                        case "streaks":
                            SetBool(p, ref it.Config.Streaks, text[3], "Streaks");
                            break;

                        case "multi":
                            int multi = 1;
                            if (!CommandParser.GetInt(p, text[3], "Points", ref multi, 0)) return;
                            it.Config.MultiKillBonus = multi;
                            
                            if (multi == 0) {
                                Player.Message(p, "TNT Wars: Multikill bonus is now &cdisabled");
                            } else {
                                Player.Message(p, "TNT Wars: Scores per extra kill is now &a{0} %Spoints", multi);
                            }
                            break;

                        case "kill":
                            int kill = 1;
                            if (!CommandParser.GetInt(p, text[3], "Points", ref kill, 0)) return;
                            it.Config.ScorePerKill = kill;
                            
                            Player.Message(p, "TNT Wars: Score per kill is now &a{0} %Spoints", kill);
                            break;
                            
                        case "assist":
                            int assist = 1;
                            if (!CommandParser.GetInt(p, text[3], "Points", ref assist, 0)) return;
                            it.Config.AssistScore = assist;
                            
                            if (assist == 0) {
                                Player.Message(p, "TNT Wars: Scores per assist is now &cdisabled");
                            } else {
                                Player.Message(p, "TNT Wars: Score per assist is now &a{0} %Spoints", assist);
                            }
                            break;

                        default:
                            Player.Message(p, "TNT Wars Setup Scoring Help:");
                            Player.Message(p, "/tw s score required [points] - set the score required to win the round");
                            Player.Message(p, "/tw s score streaks [on/off] - enable/disable kill streaks");
                            Player.Message(p, "/tw s score multi [points] - set the score bonus per multikill");
                            Player.Message(p, "/tw s score kill [points] - set the score per kill");
                            Player.Message(p, "/tw s score assist [points] - set the score per assist");
                            break;
                    }
                    break;

                case "balanceteams":
                    SetBool(p, ref it.Config.BalanceTeams, text[2], "Team balancing");
                    break;

                case "teamkill":
                    SetBool(p, ref it.Config.TeamKills, text[2], "Team killing");
                    break;

                case "zone":
                case "zones":
                case "z":
                    switch (text[2])
                    {
                        case "notnt":
                        case "nt":
                            HandleZone(p, it, true, text);
                            break;

                        case "noexplosion":
                        case "nodeleteblocks":
                        case "neb":
                            HandleZone(p, it, false, text);
                            break;
                    }
                    break;

                case "help":
                case "h":
                    int SleepAmount = 500;
                    Player.Message(p, "TNT Wars Setup Help:");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s new/delete - create/delete a game");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s start/stop/reset - start/stop/reset the current game");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s level - change the level for the game");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s spawn [red/blue] - set the spawns for red/blue team");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s tnt [amount] - sets the amount of tnt per player at a time");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s graceperiod [on/off] - enable/disable the grace period");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s gracetime [amount] - set the grace period time (in seconds)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s gamemode [tdm/ffa] - change the gamemode to FFA or TDM");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s difficulty [1/2/3/4] - change the difficulty (easy/normal/hard/extreme)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s balanceteams [on/off] - enable/disable balancing teams");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s teamkill [on/off] - enable/disable teamkills");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s zone [notnt {nt}/noexplodeblocks {neb}] [add {a}/delete {d} <all>/check {c}]- create zones (No TNT zones or zones where explosions do not delete blocks");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s score - scoring setup (see '/tntwars setup scoring help')");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s status {s} - view the status of setup");
                    break;

                default:
                    Player.Message(p, "Gamemode: &a{0} %Sat difficulty &a{1}", it.GameMode, it.Difficulty);
                    TntWarsConfig cfg = it.Config;
                    
                    Player.Message(p, "TNT per player at a time: &a{0}",
                                   cfg.MaxPlayerActiveTnt == 0 ? "unlimited" : cfg.MaxPlayerActiveTnt.ToString());
                    Player.Message(p, "Grace period: {0} %S(for {1} seconds)",
                                   GetBool(cfg.InitialGracePeriod), cfg.GracePeriodSeconds);
                    Player.Message(p, "Team balancing: {0}%S, Team killing: {1}",
                                   GetBool(cfg.BalanceTeams), GetBool(cfg.TeamKills));
                    Player.Message(p, "Score: &a{0} %Spoints needed to win, &a{1} %Spoints per kill",
                                   cfg.ScoreRequired, cfg.ScorePerKill);
                    Player.Message(p, "Streaks: {0}%S, Multikill bonus: {1}",
                                   GetBool(cfg.Streaks), GetBool(cfg.MultiKillBonus != 0));
                    Player.Message(p, "Assists: {0} %S(at {1} points)",
                                   GetBool(cfg.AssistScore > 0), cfg.AssistScore);
                    break;
            }
        }
        
        static string GetBool(bool value) { return value ? "&aEnabled" : "&cDisabled"; }
        
        void HandleZone(Player p, TntWarsGame1 it, bool noTntZone, string[] text) {
            string msg = noTntZone ? "no TNT" : "no blocks deleted on explosions";
            List<TntWarsGame1.Zone> zones = noTntZone ? it.NoTNTplacableZones : it.NoBlockDeathZones;
            
            if (IsCreateCommand(text[3])) {
                Player.Message(p, "TNT Wars: Place 2 blocks to create the zone for {0}!", msg);
                p.MakeSelection(2, zones, AddZoneCallback);
            } else if (IsDeleteCommand(text[3])) {
                if (!text[4].CaselessEq("all")) {
                    Player.Message(p, "TNT Wars: Place a block to delete the zone for {0}!", msg);
                    p.MakeSelection(1, zones, DeleteZoneCallback);
                } else {
                    zones.Clear();
                    Player.Message(p, "TNT Wars: Deleted all zones for {0}!", msg);
                }
            } else if (text[3].CaselessEq("check")) {
                Player.Message(p, "TNT Wars: Place a block to check for {0}!", msg);
                p.MakeSelection(1, zones, CheckZoneCallback);
            }
        }
        
        static bool AddZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            Vec3U16 p1 = (Vec3U16)marks[0], p2 = (Vec3U16)marks[1];
            TntWarsGame1.Zone zn = new TntWarsGame1.Zone();
            
            zn.MinX = Math.Min(p1.X, p2.X);
            zn.MinY = Math.Min(p1.Y, p2.Y);
            zn.MinZ = Math.Min(p1.Z, p2.Z);
            zn.MaxX = Math.Max(p1.X, p2.X);
            zn.MaxY = Math.Max(p1.Y, p2.Y);
            zn.MaxZ = Math.Max(p1.Z, p2.Z);

            TntWarsGame1 it = TntWarsGame1.GameIn(p);
            if (it == null) {
                Player.Message(p, "TNT Wars Error: Couldn't find your game!"); return false;
            }
            
            List<TntWarsGame1.Zone> zones = (List<TntWarsGame1.Zone>)state;
            zones.Add(zn);
            Player.Message(p, "TNT Wars: Zonr added!");
            return false;
        }
        
        static bool DeleteZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            TntWarsGame1 it = TntWarsGame1.GameIn(p);
            if (it == null) {
                Player.Message(p, "TNT Wars Error: Couldn't find your game!"); return false;
            }
            
            List<TntWarsGame1.Zone> zones = (List<TntWarsGame1.Zone>)state;
            foreach (TntWarsGame1.Zone zn in zones) {
                if (x >= zn.MinX && x <= zn.MaxX && y >= zn.MinY && y <= zn.MaxY && z >= zn.MinZ && z <= zn.MaxZ) {
                    zones.Remove(zn);
                    Player.Message(p, "TNT Wars: Zone deleted!");
                    return false;
                }
            }
            
            Player.Message(p, "TNT Wars Error: You weren't in any zone");
            return false;
        }
        
        static bool CheckZoneCallback(Player p, Vec3S32[] marks, object state, BlockID block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            TntWarsGame1 it = TntWarsGame1.GameIn(p);
            if (it == null) {
                Player.Message(p, "TNT Wars Error: Couldn't find your game!"); return false;
            }
            
            List<TntWarsGame1.Zone> zones = (List<TntWarsGame1.Zone>)state;
            if (zones == it.NoTNTplacableZones) {
                if (it.InZone(x, y, z, zones)) {
                    Player.Message(p, "TNT Wars: You are currently in a no TNT zone!");
                } else {
                    Player.Message(p, "TNT Wars: You are not currently in a no TNT zone!");
                }
            } else if (zones == it.NoBlockDeathZones) {
                if (it.InZone(x, y, z, zones)) {
                    Player.Message(p, "TNT Wars: You are currently in a no TNT block explosion zone (explosions won't destroy blocks)!");
                } else {
                    Player.Message(p, "TNT Wars: You are currently in a TNT block explosion zone (explosions will destroy blocks)!");
                }
            }
            return false;
        }
        
        static void SetDifficulty(TntWarsGame1 it, TntWarsDifficulty difficulty,
                                  bool teamKill, string name, Player p) {
            it.Difficulty = difficulty;
            if (it.FindPlayer(p) == null)
                Player.Message(p, "TNT Wars: Changed difficulty to {0}", name);
            it.MessageAll("TNT Wars: Changed difficulty to " + name);
            
            if (it.Config.TeamKills == teamKill) return;
            Player.Message(p, "TNT Wars: Team killing is now {0}", teamKill ? "&aon" : "&coff");
            it.Config.TeamKills = teamKill;
        }
        
        static void SetBool(Player p, ref bool target, string opt, string name) {
            if (!CommandParser.GetBool(p, opt, ref target)) return;
            Player.Message(p, "TNT Wars: {0} is now {1}", name, GetBool(target));
        }
        
        
        public override void Help(Player p) {
            Player.Message(p, "/tw list - Lists all running games");
            Player.Message(p, "/tw join <team> - join a game on <team>(red/blue)");
            Player.Message(p, "/tw leave - leave the current game");
            Player.Message(p, "/tw scores <top/team/me> - view the top scores/team scores/your score");
            Player.Message(p, "/tw players - view the current players in your game");
            Player.Message(p, "/tw health {hp} - view your currrent amount of health left");
            Player.Message(p, "/tw rules - read the rules");
            
            if (HasExtraPerm(p, 1)) {
                Player.Message(p, "/tw setup {s} - setup the game (do '/tntwars setup help' for more info!");
            }
        }
    }
}
