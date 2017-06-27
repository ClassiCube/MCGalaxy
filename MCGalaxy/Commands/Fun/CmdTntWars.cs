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

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdTntWars : Command {
        public override string name { get { return "tntwars"; } }
        public override string shortcut { get { return "tw"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can use admin commands for tntwars") }; }
        }
        bool NoTntZone = false;

        public override void Use(Player p, string message) {
            string[] text = new string[] { "", "", "", "", "" };
            string[] parts = message.ToLower().SplitSpaces();
            for (int i = 0; i < Math.Min(text.Length, parts.Length); i++)
                text[i] = parts[i];

            switch (text[0]) {
                case "list":
                case "levels":
                case "l":
                    DoLevels(p, text); break;
                case "join":
                    DoJoin(p, text); break;
                case "leave":
                case "exit":
                    DoLeave(p, text); break;
                case "rules":
                case "rule":
                case "r":
                    DoRules(p, text); break;
                case "score":
                case "scores":
                case "leaderboard":
                case "board":
                    DoScores(p, text); return;
                case "players":
                case "player":
                case "ps":
                case "pl":
                case "p":
                    DoPlayers(p, text); break;
                case "health":
                case "heal":
                case "hp":
                case "hlth":
                    DoHealth(p, text); break;
                case "setup":
                case "s":
                    DoSetup(p, text); break;
                default:
                    Help(p); break;
            }
        }
        
        void DoLevels(Player p, string[] text) {
            if (TntWarsGame.GameList.Count <= 0) {
                Player.Message(p, "There aren't any &cTNT Wars %Scurrently running!"); return;
            }
            
            Player.Message(p, "Currently running &cTNT Wars %S:");
            foreach (TntWarsGame T in TntWarsGame.GameList) {
                string msg = "";
                if (T.GameMode == TntWarsGame.TntWarsGameMode.FFA) msg += "FFA on ";
                if (T.GameMode == TntWarsGame.TntWarsGameMode.TDM) msg += "TDM on ";
                msg += T.lvl.name + " ";
                if (T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Easy) msg += "(Easy)";
                if (T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Normal) msg += "(Normal)";
                if (T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Hard) msg += "(Hard)";
                if (T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Extreme) msg += "(Extreme)";
                msg += " ";
                if (T.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers) msg += "(Waiting For Players)";
                if (T.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart) msg += "(Starting)";
                if (T.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod) msg += "(Started)";
                if (T.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress) msg += "(In Progress)";
                if (T.GameStatus == TntWarsGame.TntWarsGameStatus.Finished) msg += "(Finished)";
                Player.Message(p, msg);
            }
        }
        
        void DoJoin(Player p, string[] text) {
            TntWarsGame game = TntWarsGame.GetTntWarsGame(p);
            if (p.PlayingTntWars || (game != null && game.Players.Contains(game.FindPlayer(p)))) {
                Player.Message(p, "TNT Wars Error: You have already joined a game!"); return;
            }
            
            TntWarsGame it;
            if (text[1] == "red" || text[1] == "r" || text[1] == "1" || text[1] == "blue"
                || text[1] == "b" || text[1] == "2" || text[1] == "auto" || text[1] == "a" || text[1] == "") {
                it = TntWarsGame.Find(p.level);
                if (it == null) {
                    Player.Message(p, "TNT Wars Error: There isn't a game on your current level!"); return;
                }
            } else {
                Level lvl = Matcher.FindLevels(p, text[1]);
                if (lvl == null) return;

                it = TntWarsGame.Find(lvl);
                if (it == null) {
                    Player.Message(p, "TNT Wars Error: There isn't a game on that level!"); return;
                }
                text[1] = text[2]; //so the switch later on still works
            }
            
            TntWarsGame.player pl = new TntWarsGame.player(p);
            if (it.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart ||
                it.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod ||
                it.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress) {
                pl.spec = true;
            }
            
            if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM) {
                int red = it.RedTeam(), blue = it.BlueTeam();
                switch (text[1]) {
                    case "red":
                    case "r":
                    case "1":
                        if (it.BalanceTeams && red > blue) {
                            Player.Message(p, "TNT Wars Error: Red has too many players!"); return;
                        }
                        pl.Red = true; break;

                    case "blue":
                    case "b":
                    case "2":
                        if (it.BalanceTeams && blue > red) {
                            Player.Message(p, "TNT Wars Error: Blue has too many players!"); return;
                        }
                        pl.Blue = true; break;

                    case "auto":
                    case "a":
                    default:
                        AssignAutoTeam(pl, it, red, blue); break;
                }
            } else {
                pl.Red = false;
                pl.Blue = false;
            }
            
            it.Players.Add(pl);
            TntWarsGame.SetTitlesAndColor(pl);
            p.CurrentTntGameNumber = it.GameNumber;
            string msg = p.ColoredName + " %Sjoined TNT Wars on '" + it.lvl.name + "'";
            
            if (pl.Red)
                msg += " on the " + Colors.red + "red team";
            if (pl.Blue)
                msg += " on the " + Colors.blue + "blue team";
            if (pl.spec)
                msg += ServerConfig.DefaultColor + " (as a spectator)";
            Chat.MessageGlobal(msg);
        }
        
        void DoLeave(Player p, string[] text) {
            p.canBuild = true;
            TntWarsGame game = TntWarsGame.GetTntWarsGame(p);
            game.Players.Remove(game.FindPlayer(p));
            game.SendAllPlayersMessage("TNT Wars: " + p.ColoredName + " %Sleft the TNT Wars game!");
            TntWarsGame.SetTitlesAndColor(game.FindPlayer(p), true);
            Player.Message(p, "TNT Wars: You left the game");
        }
        
        void DoRules(Player p, string[] text) {
            if (String.IsNullOrEmpty(text[1])) {
                Player.Message(p, "TNT Wars Rules:");
                SendRules(p); return;
            }
            if (!CheckExtraPerm(p)) return;
            
            switch (text[1]) {
                case "all":
                case "a":
                case "everyone":
                    Player[] players = PlayerInfo.Online.Items;
                    foreach (Player pl in players) {
                        Player.Message(pl, "TNT Wars Rules: (sent to all players by " + p.ColoredName + " %S)");
                        SendRules(pl);
                    }
                    Player.Message(p, "TNT Wars: Sent rules to all players");
                    return;

                case "level":
                case "l":
                case "lvl":
                case "map":
                case "m":
                    foreach (Player pl in p.level.players) {
                        Player.Message(pl, "TNT Wars Rules: (sent to all players in map by " + p.ColoredName + " %S)");
                        SendRules(pl);
                    }
                    Player.Message(p, "TNT Wars: Sent rules to all current players in map");
                    return;

                case "players":
                case "pls":
                case "pl":
                case "p":
                    TntWarsGame gm = TntWarsGame.GetTntWarsGame(p);
                    if (gm == null) { Player.Message(p, "TNT Wars Error: You aren't in a TNT Wars game!"); return; }
                    
                    foreach (TntWarsGame.player pl in gm.Players) {
                        Player.Message(pl.p, "TNT Wars Rules: (sent to all current players by " + p.ColoredName + " %S)");
                        SendRules(pl.p);
                    }
                    Player.Message(p, "TNT Wars: Sent rules to all current players");
                    return;

                default:
                    Player who = PlayerInfo.FindMatches(p, text[1]);
                    if (who == null) return;
                    
                    Player.Message(who, "TNT Wars Rules: (sent to you by " + p.ColoredName + " %S)");
                    SendRules(who);
                    Player.Message(p, "TNT Wars: Sent rules to " + who.color + who.name);
                    return;
            }
        }
        
        static void SendRules(Player p) {
            Player.Message(p, "The aim of the game is to blow up people using TNT!");
            Player.Message(p, "To place tnt simply place a TNT block and after a short delay it shall explode!");
            Player.Message(p, "During the game the amount of TNT placable at one time may be limited!");
            Player.Message(p, "You are not allowed to use hacks of any sort during the game!");
        }
        
        void DoScores(Player p, string[] text) {
            TntWarsGame tntwrs = TntWarsGame.GetTntWarsGame(p);
            switch (text[1])
            {
                case "top":
                case "leaders":
                    if (tntwrs.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress) {
                        List<TntWarsGame.player> sorted = tntwrs.SortedByScore();
                        int count = Math.Min(tntwrs.PlayingPlayers(), 5);
                        
                        for (int i = 0; i < count; i++) {
                            Player.Message(p, "{0}: {1} - {2}", (i + 1), sorted[i].p.name, sorted[i].Score);
                            Thread.Sleep(500); //Maybe, not sure (250??)
                        }
                    } else {
                        Player.Message(p, "TNT Wars Error: Can't display scores - game not in progress!");
                    }
                    break;

                case "teams":
                case "team":
                case "t":
                case "red":
                case "blue":
                    if (tntwrs.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress) {
                        if (tntwrs.GameMode == TntWarsGame.TntWarsGameMode.TDM) {
                            Player.Message(p, "TNT Wars Scores:");
                            Player.Message(p, Colors.red + "RED: " + Colors.white + tntwrs.RedScore + " " + Colors.red + "(" + (tntwrs.ScoreLimit - tntwrs.RedScore).ToString() + " needed)");
                            Player.Message(p, Colors.blue + "BLUE: " + Colors.white + tntwrs.BlueScore + " " + Colors.red + "(" + (tntwrs.ScoreLimit - tntwrs.BlueScore).ToString() + " needed)");
                        } else {
                            Player.Message(p, "TNT Wars Error: Can't display team scores as this isn't team deathmatch!");
                        }
                    } else {
                        Player.Message(p, "TNT Wars Error: Can't display scores - game not in progress!");
                    }
                    break;

                case "me":
                case "mine":
                case "score":
                case "i":
                default:
                    if (tntwrs.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress) {
                        Player.Message(p, "TNT Wars: Your Score: " + Colors.white + TntWarsGame.GetTntWarsGame(p).FindPlayer(p).Score);
                    } else {
                        Player.Message(p, "TNT Wars Error: Can't display scores - game not in progress!");
                    }
                    break;
            }
        }
        
        void DoPlayers(Player p, string[] text) {
            Player.Message(p, "TNT Wars: People playing TNT Wars on '" + TntWarsGame.GetTntWarsGame(p).lvl.name + "':");
            foreach (TntWarsGame.player pl in TntWarsGame.GetTntWarsGame(p).Players) {
                if (TntWarsGame.GetTntWarsGame(p).GameMode == TntWarsGame.TntWarsGameMode.TDM) {
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
            if (TntWarsGame.GetTntWarsGame(p).GameStatus == TntWarsGame.TntWarsGameStatus.InProgress) {
                Player.Message(p, "TNT Wars: You have " + p.TntWarsHealth.ToString() + " health left");
            } else {
                Player.Message(p, "TNT Wars Error: Can't display health - game not in progress!");
            }
        }
        
        void DoSetup(Player p, string[] text) {
            if (!CheckExtraPerm(p)) {
                Player.Message(p, "Sorry, you aren't a high enough rank for that!"); return;
            }
            
            TntWarsGame it = TntWarsGame.FindFromGameNumber(p.CurrentTntGameNumber);
            if (it == null && text[1] != "new" && text[1] != "n") {
                Player.Message(p, "TNT Wars Error: You must create a new game by typing '/tntwars setup new'"); return;
            }
            
            if (it != null) {
                if (it.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress || it.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod
                    || it.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart) {
                    if (text[1] != "stop" && text[1] != "s" && text[1] != "" && text[1] != "status" &&
                        text[1] != "ready" && text[1] != "check" && text[1] != "info" && text[1] != "r" && text[1] != "c") {
                        Player.Message(p, "TNT Wars Error: Cannot edit current game because it is currently running!"); return;
                    }
                }
            }
            
            switch (text[1]) {
                case "new":
                case "n":
                    if (it != null && it.FindPlayer(p) != null) {
                        Player.Message(p, "TNT Wars Error: Please leave the current game first!"); return;
                    }
                    
                    if (it == null || it.lvl != p.level) {
                        it = new TntWarsGame(p.level);
                        it.GameNumber = TntWarsGame.GameList.Count + 1;
                        TntWarsGame.GameList.Add(it);
                        p.CurrentTntGameNumber = it.GameNumber;
                        Player.Message(p, "TNT Wars: Created New TNT Wars game on '" + p.level.name + "'");
                    } else {
                        Player.Message(p, "TNT Wars Error: Please delete the current game first!");
                    }
                    return;

                case "delete":
                case "remove":
                    if (it.GameStatus != TntWarsGame.TntWarsGameStatus.Finished && it.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
                        Player.Message(p, "Please stop the game first!"); return;
                    }
                    
                    foreach (TntWarsGame.player pl in it.Players) {
                        pl.p.CurrentTntGameNumber = -1;
                        Player.Message(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                        pl.p.PlayingTntWars = false;
                        pl.p.canBuild = true;
                        TntWarsGame.SetTitlesAndColor(pl, true);
                    }
                    Player.Message(p, "TNT Wars: Game deleted");
                    TntWarsGame.GameList.Remove(it);
                    return;
                    //break;

                case "reset":
                case "r":
                    if (it.GameStatus != TntWarsGame.TntWarsGameStatus.Finished) {
                        Player.Message(p, "TNT Wars Error: The game has to have finished to be reset!"); return;
                    }
                    
                    it.GameStatus = TntWarsGame.TntWarsGameStatus.WaitingForPlayers;
                    Command.all.Find("restore").Use(null, it.BackupNumber + it.lvl.name);
                    it.RedScore = 0;
                    it.BlueScore = 0;
                    
                    foreach (TntWarsGame.player pl in it.Players) {
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
                    if (it.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
                        if (it.CheckAllSetUp(p, true)) {
                            if (it.PlayingPlayers() >= 2) {
                                if (it.lvl.Config.overload < 2500) {
                                    it.lvl.Config.overload = 2501;
                                    Player.Message(p, "TNT Wars: Increasing physics overload to 2500");
                                    Logger.Log(LogType.GameActivity, "TNT Wars: Increasing physics overload to 2500");
                                }
                                Thread t = new Thread(it.Start);
                                t.Name = "MCG_TntGame";
                                t.Start();
                            } else {
                                Player.Message(p, "TNT Wars Error: Not Enough Players (2 or more needed)");
                            }
                        }
                    } else if (it.GameStatus == TntWarsGame.TntWarsGameStatus.Finished) {
                        Player.Message(p, "TNT Wars Error: Please use '/tntwars setup reset' to reset the game before starting!");
                    } else {
                        Player.Message(p, "TNT Wars Error: Game already in progress!!");
                    }
                    return;

                case "stop":
                    if (it.GameStatus == TntWarsGame.TntWarsGameStatus.Finished || it.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
                        Player.Message(p, "TNT Wars Error: Game already ended / not started!"); return;
                    }
                    
                    foreach (TntWarsGame.player pl in it.Players) {
                        pl.p.canBuild = true;
                        pl.p.PlayingTntWars = false;
                        pl.p.CurrentAmountOfTnt = 0;
                    }
                    it.GameStatus = TntWarsGame.TntWarsGameStatus.Finished;
                    it.SendAllPlayersMessage("TNT Wars: Game has been stopped!");
                    break;

                case "spawn":
                case "spawns":
                case "sp":
                case "teamspawns":
                case "teamspawn":
                case "ts":
                case "teams":
                case "tspawn":
                case "tspawns":
                    if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA) { Player.Message(p, "TNT Wars Error: Cannot set spawns because you are on Team Deathmatch!"); return; }
                    switch (text[2])
                    {
                        case "red":
                        case "r":
                        case "1":
                            SetSpawn(p, ref it.RedSpawn, Colors.red + "Red %Sspawn");
                            break;

                        case "blue":
                        case "b":
                        case "2":
                            SetSpawn(p, ref it.BlueSpawn, Colors.blue + "Blue %Sspawn");
                            break;
                    }
                    break;

                case "level":
                case "l":
                case "lvl":
                    if (text[2] == "") {
                        it.lvl = p.level;
                    } else {
                        it.lvl = Matcher.FindLevels(p, text[2]);
                        if (it.lvl == null) return;
                    }
                    Player.Message(p, "TNT Wars: Level is now '" + it.lvl.name + "'");
                    it.RedSpawn = null;
                    it.BlueSpawn = null;
                    it.NoTNTplacableZones.Clear();
                    it.NoBlockDeathZones.Clear();
                    it.CheckAllSetUp(p);
                    break;

                case "tntatatime":
                case "tnt":
                case "t":
                    int number = 1;
                    if (!CommandParser.GetInt(p, text[2], "TNT at a time", ref number, 0)) return;
                    
                    Player.Message(p, "TNT Wars: Number of TNTs placeable by a player at a time is now {0}",
                                   number == 0 ? "unlimited" : number.ToString());
                    it.CheckAllSetUp(p);
                    break;

                case "grace":
                case "g":
                case "graceperiod":
                    if (text[1] == "grace" || text[1] == "g") {
                        if (text[2] == "time" || text[2] == "t") {
                            text[1] = "gt";
                            text[2] = text[3];
                            break;
                        }
                    }
                    if (SetBool(p, ref it.GracePeriod, text[2], "Grace period"))
                        it.CheckAllSetUp(p);
                    break;

                case "gracetime":
                case "gt":
                case "gtime":
                case "gracet":
                case "graceperiodtime":
                    switch (text[2])
                    {
                        case "check":
                        case "current":
                        case "now":
                        case "ATM":
                        case "c":
                        case "t":
                            Player.Message(p, "TNT Wars: Current grace time is " + it.GracePeriodSecs.ToString() + " seconds long!");
                            break;

                        default:
                            if (text[2] == "set" || text[2] == "s" || text[2] == "change")
                            {
                                text[2] = text[3];
                            }
                            int numb = -1;
                            if (!int.TryParse(text[2], out numb))
                            { Player.Message(p, "TNT Wars Error: Invalid number '" + text[2] + "'"); return; }
                            if (numb <= -1) { Player.Message(p, "TNT Wars Error: Invalid number '" + text[2] + "'"); return; }
                            if (numb >= (60 * 5)) { Player.Message(p, "TNT Wars Error: Grace time cannot be above 5 minutes!!"); return; }
                            if (numb <= 9) { Player.Message(p, "TNT Wars Error: Grace time cannot be lower than 10 seconds!!"); return; }
                            else
                            {
                                it.GracePeriodSecs = numb;
                                Player.Message(p, "TNT Wars: Grace period is now " + numb.ToString() + " seconds long!");
                                return;
                            }
                            //break;
                    }
                    it.CheckAllSetUp(p);
                    break;

                case "mode":
                case "game":
                case "gamemode":
                case "m":
                    switch (text[2])
                    {
                        case "check":
                        case "current":
                        case "mode":
                        case "now":
                        case "ATM":
                        case "m":
                        case "c":
                            if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA)
                                Player.Message(p, "TNT Wars: The current game mode is Free For All");
                            if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM)
                                Player.Message(p, "TNT Wars: The current game mode is Team Deathmatch");
                            break;

                        case "tdm":
                        case "team":
                        case "teamdeathmatch":
                        case "deathmatch":
                        case "teams":
                        case "t":
                        case "td":
                        case "dm":
                        case "death":
                        case "match":
                            if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA) {
                                ModeTDM(p, it);
                            } else {
                                Player.Message(p, "TNT Wars Error: Gamemode is already Team Deathmatch!"); return;
                            }
                            break;

                        case "ffa":
                        case "all":
                        case "free":
                        case "man":
                        case "himself":
                        case "allvall":
                        case "allvsall":
                        case "allv":
                        case "allvs":
                        case "a":
                        case "f":
                        case "ff":
                        case "fa":
                            if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM) {
                                ModeFFA(p, it);
                            } else {
                                Player.Message(p, "TNT Wars Error: Gamemode is already Free For All!"); return;
                            }
                            break;

                        case "swap":
                        case "s":
                        case "change":
                        case "edit":
                        case "switch":
                        default:
                            if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA) {
                                ModeTDM(p, it);
                            } else {
                                ModeFFA(p, it);
                            }
                            break;
                    }
                    it.CheckAllSetUp(p);
                    break;

                case "difficulty":
                case "d":
                case "dif":
                case "diff":
                case "difficult":
                    switch (text[2])
                    {
                        case "easy":
                        case "e":
                        case "easiest":
                        case "1":
                        case "1st":
                            SetDifficulty(it, TntWarsGame.TntWarsDifficulty.Easy, false, "easy", p); break;

                        case "normal":
                        case "n":
                        case "medium":
                        case "m":
                        case "2":
                        case "2nd":
                        default:
                            SetDifficulty(it, TntWarsGame.TntWarsDifficulty.Normal, false, "normal", p); break;
                            
                        case "hard":
                        case "h":
                        case "difficult":
                        case "d":
                        case "3":
                        case "3rd":
                            SetDifficulty(it, TntWarsGame.TntWarsDifficulty.Hard, true, "hard", p); break;

                        case "extreme":
                        case "ex":
                        case "hardest":
                        case "impossible":
                        case "ultimate":
                        case "i":
                        case "u":
                        case "4":
                        case "4th":
                            SetDifficulty(it, TntWarsGame.TntWarsDifficulty.Extreme, true, "extreme", p); break;
                    }
                    it.CheckAllSetUp(p);
                    break;

                case "score":
                case "scores":
                case "scoring":
                    switch (text[2])
                    {
                        case "max":
                        case "m":
                        case "maximum":
                        case "limit":
                        case "top":
                        case "goal":
                        case "maxscore":
                        case "maximumscore":
                        case "scorelimit":
                            switch (text[3])
                            {
                                case "check":
                                case "current":
                                case "now":
                                case "ATM":
                                case "c":
                                case "t":
                                    Player.Message(p, "TNT Wars: Score limit is " + it.ScoreLimit.ToString() + " points!");
                                    break;

                                default:
                                    if (text[3] == "set" || text[3] == "s" || text[3] == "change")
                                    {
                                        text[3] = text[4];
                                    }
                                    int numb = -1;
                                    if (!int.TryParse(text[3], out numb))
                                    { Player.Message(p, "TNT Wars Error: Invalid number '" + text[3] + "'"); return; }
                                    if (numb <= it.ScorePerKill) { Player.Message(p, "TNT Wars Error: Minimum score limit of " + it.ScorePerKill.ToString() + " points"); return; }
                                    else
                                    {
                                        it.ScoreLimit = numb;
                                        Player.Message(p, "TNT Wars: Score limit is now " + numb.ToString() + " points!");
                                        return;
                                    }
                                    //break;
                            }
                            it.CheckAllSetUp(p);
                            break;

                        case "streaks":
                        case "streak":
                        case "s":
                            if (SetBool(p, ref it.Streaks, text[3], "Streaks"))
                                it.CheckAllSetUp(p);
                            break;

                        case "multi":
                        case "multikills":
                        case "multiples":
                        case "multiplekills":
                        case "multis":
                        case "doublekill":
                        case "double":
                        case "triplekill":
                        case "triple":
                        case "mk":
                        case "d":
                        case "t":
                            SetInt(p, it, ref it.ScorePerAssist, TntWarsGame.Properties.DefaultMultiKillBonus,
                                   text, "Mulitkill bonuses", "Mulitkill bonus per extra kill");
                            break;

                        case "scorekill":
                        case "kill":
                        case "killscore":
                        case "k":
                            switch (text[3])
                            {
                                case "check":
                                case "current":
                                case "now":
                                case "ATM":
                                case "c":
                                case "t":
                                    Player.Message(p, "TNT Wars: Score per kill is " + it.ScorePerKill + " points!");
                                    break;

                                default:
                                    if (text[3] == "set" || text[3] == "s" || text[3] == "change")
                                    {
                                        text[3] = text[4];
                                    }
                                    int numb = -1;
                                    if (!CommandParser.GetInt(p, text[3], "Score per kill", ref numb, 0)) return;
                                    
                                    it.ScorePerKill = numb;
                                    Player.Message(p, "TNT Wars: Score per kill is now " + numb + " points!");
                                    return;
                                    //break;
                            }
                            break;

                        case "assistkill":
                        case "assist":
                        case "assists":
                        case "assistscore":
                        case "a":
                            SetInt(p, it, ref it.ScorePerAssist, TntWarsGame.Properties.DefaultAssistScore,
                                   text, "Assist bonuses", "Score per assist");
                            break;

                        case "help":
                        case "h":
                        default:
                            Player.Message(p, "TNT Wars Setup Scoring Help:");
                            Player.Message(p, "/tw s score maximum {m} [check/set] <value> - set the score limit (or check it)");
                            Player.Message(p, "/tw s score streaks {s} [on/off/check] - enable/disable streaks (or check it)");
                            Player.Message(p, "/tw s score multi {mk} [on/off/switch/check/set] - enable/disable/switch multikills or set the score bonus per multikill (or check it)");
                            Player.Message(p, "/tw s score scorekill {k} [check/set] <value> - set the score per kill (or check it)");
                            Player.Message(p, "/tw s score assistkill {a} [check/set] <value> - set the score per assist (or check it)");
                            break;
                    }
                    break;

                case "balance":
                case "balanceteams":
                case "bt":
                case "b":
                    if (SetBool(p, ref it.BalanceTeams, text[2], "Team balancing"))
                        it.CheckAllSetUp(p);
                    break;

                case "teamkill":
                case "tk":
                case "tkill":
                case "teamk":
                case "friendly":
                case "friendlyfire":
                case "ff":
                case "friendlyf":
                case "ffire":
                    if (SetBool(p, ref it.TeamKills, text[2], "Team killing"))
                        it.CheckAllSetUp(p);
                    break;

                case "zone":
                case "zones":
                case "z":
                case "zn":
                case "zns":
                case "zs":
                    switch (text[2])
                    {
                        case "notnt":
                        case "tnt":
                        case "no":
                        case "none":
                        case "nothing":
                        case "blocktnt":
                        case "blockt":
                        case "bt":
                        case "nt":
                            HandleZone(p, it, true, text);
                            break;

                        case "noexplosion":
                        case "nodeleteblocks":
                        case "deleteblocks":
                        case "nd":
                        case "nb":
                        case "ne":
                        case "neb":
                        case "ndb":
                            HandleZone(p, it, false, text);
                            break;
                    }
                    break;

                case "help":
                case "h":
                    int SleepAmount = 500;
                    Player.Message(p, "TNT Wars Setup Help:");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s new {n}/delete - create/delete a game");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s start/stop/reset {r} - start/stop/reset the current game");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s level {l} - change the level for the game");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s teamsspawns {ts} [red/blue] - set the spawns for red/blue");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s tnt {t} - change the amount of tnt per player at a time");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s graceperiod {g} [on/off/check] - enable/disable the grace period (or check it)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s gracetime {gt} [set/check] <amount> - set the grace period time (in seconds) (or check it)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s gamemode {m} [check/tdm/ffa] - change the gamemode to FFA or TDM (or check it)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s difficulty {d} [1/2/3/4] - change the difficulty (easy/normal/hard/extreme)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s balanceteams {b} [on/off/check] - enable/disable balancing teams (or check it)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s teamkill {tk} [on/off/check] - enable/disable teamkills (or check it)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s zone {z} [notnt {nt}/noexplodeblocks {neb}] [add {a}/delete {d} <all>/check {c}]- create zones (No TNT zones or zones where explosions do not delete blocks");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s score - scoring setup (use '/tntwars setup scoring help' for more info)");
                    Thread.Sleep(SleepAmount);
                    Player.Message(p, "/tw s status {s} - view the status of setup");
                    break;

                default:
                case "status":
                case "s":
                case "ready":
                case "check":
                case "info":
                case "c":
                    Player.Message(p, "TNT Wars: Current Setup:");
                    //1
                    if (it.lvl == null) { Player.Message(p, "Level: " + Colors.red + "NONE"); }
                    else { Player.Message(p, "Level: " + Colors.green + it.lvl.name); }
                    //2
                    if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA) { Player.Message(p, "Gamemode: " + Colors.green + "FFA"); }
                    if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM) { Player.Message(p, "Gamemode: " + Colors.green + "TDM"); }
                    //3
                    if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Easy) { Player.Message(p, "Game difficulty: " + Colors.green + "Easy"); }
                    if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Normal) { Player.Message(p, "Game difficulty: " + Colors.green + "Normal"); }
                    if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Hard) { Player.Message(p, "Game difficulty: " + Colors.green + "Hard"); }
                    if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Extreme) { Player.Message(p, "Game difficulty: " + Colors.green + "Extreme"); }
                    //4
                    if (it.TntPerPlayerAtATime >= 1) { Player.Message(p, "TNT per player at a time: " + Colors.green + it.TntPerPlayerAtATime.ToString()); }
                    else if (it.TntPerPlayerAtATime == 0) { Player.Message(p, "TNT per player at a time: " + Colors.green + "unlimited"); }
                    //5
                    if (it.GracePeriod) { Player.Message(p, "Grace period: " + Colors.green + "enabled"); }
                    if (!it.GracePeriod) { Player.Message(p, "Grace period: " + Colors.green + "disabled"); }
                    //6
                    Player.Message(p, "Grace period time: " + Colors.green + it.GracePeriodSecs.ToString() + " seconds");
                    //7
                    if (it.BalanceTeams) { Player.Message(p, "Balance teams: " + Colors.green + "enabled"); }
                    if (!it.BalanceTeams) { Player.Message(p, "Balance teams: " + Colors.green + "disabled"); }
                    //8
                    Player.Message(p, "Score limit: " + Colors.green + it.ScoreLimit.ToString() + " points");
                    //9
                    if (it.Streaks) { Player.Message(p, "Streaks: " + Colors.green + "enabled"); }
                    if (!it.Streaks) { Player.Message(p, "Streaks: " + Colors.green + "disabled"); }
                    //10
                    if (it.MultiKillBonus == 0) { Player.Message(p, "Multikill bonus: " + Colors.green + "disabled"); }
                    if (it.MultiKillBonus != 0) { Player.Message(p, "Multikill bonus: " + Colors.green + "enabled"); }
                    //11
                    Player.Message(p, "Score per kill: " + Colors.green + it.ScorePerKill.ToString() + " points");
                    //12
                    if (it.ScorePerAssist == 0) { Player.Message(p, "Assists: " + Colors.green + "disabled"); }
                    if (it.ScorePerAssist != 0) { Player.Message(p, "Assists : " + Colors.green + "enabled (at " + it.ScorePerAssist + " points)"); }
                    //13
                    if (it.TeamKills) { Player.Message(p, "Team killing: " + Colors.green + "enabled"); }
                    if (!it.TeamKills) { Player.Message(p, "Team killing: " + Colors.green + "disabled"); }
                    //14
                    it.CheckAllSetUp(p);
                    //15
                    break;
            }
        }
        
        static void SetSpawn(Player p, ref ushort[] target, string name) {
            target = new ushort[5];
            target[0] = (ushort)p.Pos.BlockX;
            target[1] = (ushort)p.Pos.BlockY;
            target[2] = (ushort)p.Pos.BlockZ;
            target[3] = p.Rot.RotY;
            target[4] = p.Rot.HeadX;
            Player.Message(p, "TNT Wars: Set " + name);
        }
        
        void HandleZone(Player p, TntWarsGame it, bool noTntZone, string[] text) {
            NoTntZone = noTntZone;
            string msg = noTntZone ? "no TNT" : "no blocks deleted on explosions";
            
            switch (text[3]) {
                case "add":
                case "a":
                case "new":
                    Player.Message(p, "TNT Wars: Place 2 blocks to create the zone for {0}!", msg);
                    p.MakeSelection(2, null, AddZoneCallback);
                    break;

                case "delete":
                case "d":
                case "remove":
                case "r":
                    if (text[4] == "all" || text[4] == "a") {
                        if (noTntZone) it.NoTNTplacableZones.Clear();
                        else it.NoBlockDeathZones.Clear();
                        Player.Message(p, "TNT Wars: Deleted all zones for {0}!", msg);
                        return;
                    }

                    Player.Message(p, "TNT Wars: Place a block to delete the zone for {0}!", msg);
                    p.MakeSelection(1, null, DeleteZoneCallback);
                    break;

                case "check":
                case "c":
                    Player.Message(p, "TNT Wars: Place a block to check for no {0}!", msg);
                    p.MakeSelection(1, null, CheckZoneCallback);
                    break;
            }
        }
        
        static void SetDifficulty(TntWarsGame it, TntWarsGame.TntWarsDifficulty difficulty,
                                  bool teamKill, string name, Player p) {
            if (it.GameDifficulty == difficulty) {
                Player.Message(p, "TNT Wars Error: Already on {0} difficulty!", name); return;
            }
            
            it.GameDifficulty = difficulty;
            if (!it.Players.Contains(it.FindPlayer(p)))
                Player.Message(p, "TNT Wars: Changed difficulty to {0}", name);
            it.SendAllPlayersMessage("TNT Wars: Changed difficulty to " + name + "!");
            
            if (it.TeamKills == teamKill) return;
            Player.Message(p, "TNT Wars: Team killing is now {0}", teamKill ? "&aon" : "&coff");
            it.TeamKills = teamKill;
        }
        
        static void AssignAutoTeam(TntWarsGame.player pl, TntWarsGame it, int red, int blue) {
            pl.Red = false;
            pl.Blue = false;
            
            if (blue > red) {
                pl.Red = true;
            } else if (red > blue) {
                pl.Blue = true;
            } else if (it.RedScore > it.BlueScore) {
                pl.Blue = true;
            } else if (it.BlueScore > it.RedScore) {
                pl.Red = true;
            } else {
                pl.Red = true;
            }
        }
        
        static void ModeTDM(Player p, TntWarsGame it) {
            it.GameMode = TntWarsGame.TntWarsGameMode.TDM;
            if (!it.Players.Contains(it.FindPlayer(p))) {
                Player.Message(p, "TNT Wars: Changed gamemode to Team Deathmatch");
            }
            
            foreach (TntWarsGame.player pl in it.Players) {
                Player.Message(pl.p, "TNT Wars: Changed gamemode to Team Deathmatch");
                AssignAutoTeam(pl, it, it.RedTeam(), it.BlueTeam());
                
                string msg = pl.p.ColoredName + " %Sis now";
                if (pl.Red) msg += " on the " + Colors.red + "red team";
                if (pl.Blue) msg += " on the " + Colors.blue + "blue team";
                if (pl.spec) msg += " (as a spectator)";
                Chat.MessageGlobal(msg);
            }
            
            if (it.ScoreLimit == TntWarsGame.Properties.DefaultFFAmaxScore) {
                it.ScoreLimit = TntWarsGame.Properties.DefaultTDMmaxScore;
                Player.Message(p, "TNT Wars: Score limit is now " + it.ScoreLimit + " points!");
            } else {
                Player.Message(p, "TNT Wars: Score limit is still " + it.ScoreLimit + " points!");
            }
        }
        
        static void ModeFFA(Player p, TntWarsGame it) {
            it.GameMode = TntWarsGame.TntWarsGameMode.FFA;
            if (!it.Players.Contains(it.FindPlayer(p))) {
                Player.Message(p, "TNT Wars: Changed gamemode to Free For All");
            }
            
            it.SendAllPlayersMessage("TNT Wars: Changed gamemode to Free For All");
            if (it.ScoreLimit == TntWarsGame.Properties.DefaultTDMmaxScore) {
                it.ScoreLimit = TntWarsGame.Properties.DefaultFFAmaxScore;
                Player.Message(p, "TNT Wars: Score limit is now " + it.ScoreLimit + " points!");
            } else {
                Player.Message(p, "TNT Wars: Score limit is still " + it.ScoreLimit + " points!");
            }
            
            foreach (TntWarsGame.player pl in it.Players) {
                pl.p.color = pl.OldColor;
                pl.p.SetPrefix();
            }
        }
        
        static void SetInt(Player p, TntWarsGame it, ref int target, int defValue,
                           string[] text, string name, string score) {
            switch (text[3]) {
                case "on":
                case "enable":
                    target = defValue;
                    Player.Message(p, "TNT Wars: {0} are now enabled at {1} points!", name, target);
                    break;

                case "off":
                case "disable":
                    target = 0;
                    Player.Message(p, "TNT Wars: {0} are now disabled!", name);
                    break;

                case "switch":
                    if (target == 0) {
                        target = defValue;
                        Player.Message(p, "TNT Wars: {0} are now enabled at {1} points!", name, target);
                    } else {
                        target = 0;
                        Player.Message(p, "TNT Wars: {0} are now disabled!", name);
                    }
                    break;

                case "check":
                case "current":
                case "now":
                case "ATM":
                case "c":
                case "t":
                    Player.Message(p, "TNT Wars: {0} is {1} points!", score, target);
                    return;

                default:
                    if (text[3] == "set" || text[3] == "s" || text[3] == "change")
                        text[3] = text[4];
                    int numb = -1;
                    if (!CommandParser.GetInt(p, text[3], "Points", ref numb, 0)) return;
                    
                    target = numb;
                    Player.Message(p, "TNT Wars: {0} is now {1} points!", score, numb);
                    break;
            }
            it.CheckAllSetUp(p);
        }
        
        static bool SetBool(Player p, ref bool target, string opt, string name) {
            switch (opt) {
                case "yes":
                case "on":
                case "enable":
                    target = true;
                    Player.Message(p, "TNT Wars: {0} is now &aenabled", name); return true;
                case "no":
                case "off":
                case "disable":
                    target = false;
                    Player.Message(p, "TNT Wars: {0} is now &cdisabled", name); return true;
                case "check":
                case "current":
                case "now":
                case "ATM":
                case "c":
                case "t":
                    Player.Message(p, "TNT Wars: {0} is currently {1}", name, target ? "&aenabled" : "&cdisabled"); return false;
                default:
                    target = !target;
                    Player.Message(p, "TNT Wars: {0} is now {1}", name, target ? "&aenabled" : "&cdisabled"); return true;
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "TNT Wars Help:");
            Player.Message(p, "/tw list {l} - Lists all the current games");
            Player.Message(p, "/tw join <team/level> - join a game on <level> or on <team>(red/blue)");
            Player.Message(p, "/tw leave - leave the current game");
            Player.Message(p, "/tw scores <top/team/me> - view the top score/team scores/your scores");
            Player.Message(p, "/tw players {p} - view the current players in your game");
            Player.Message(p, "/tw health {hp} - view your currrent amount of health left");
            
            if (CheckExtraPerm(p)) {
                Player.Message(p, "/tw rules <all/level/players/<playername>> - send the rules to yourself, all, your map, all players in your game or to one person!");
                Player.Message(p, "/tw setup {s} - setup the game (do '/tntwars setup help' for more info!");
            } else {
                Player.Message(p, "/tw rules - read the rules");
            }
        }
        
        bool DeleteZoneCallback(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            TntWarsGame it = TntWarsGame.GetTntWarsGame(p);
            
            if (it == null) {
                Player.Message(p, "TNT Wars Error: Couldn't find your game!");
            } else if (it.InZone(x, y, z, NoTntZone)) {
                it.DeleteZone(x, y, z, NoTntZone, p);
            }
            return false;
        }
        
        bool CheckZoneCallback(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            TntWarsGame it = TntWarsGame.GetTntWarsGame(p);
            
            if (it == null) {
                Player.Message(p, "TNT Wars Error: Couldn't find your game!");
            } else if (NoTntZone) {
                if (it.InZone(x, y, z, NoTntZone)) {
                    Player.Message(p, "TNT Wars: You are currently in a no TNT zone!");
                } else {
                    Player.Message(p, "TNT Wars: You are not currently in a no TNT zone!");
                }
            } else {
                if (it.InZone(x, y, z, NoTntZone)) {
                    Player.Message(p, "TNT Wars: You are currently in a no TNT block explosion zone (explosions won't destroy blocks)!");
                } else {
                    Player.Message(p, "TNT Wars: You are currently in a TNT block explosion zone (explosions will destroy blocks)!");
                }
            }
            return false;
        }
        
        bool AddZoneCallback(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            Vec3U16 p1 = (Vec3U16)marks[0], p2 = (Vec3U16)marks[1];
            TntWarsGame.Zone Zn = new TntWarsGame.Zone();
            
            Zn.smallX = Math.Min(p1.X, p2.X);
            Zn.smallY = Math.Min(p1.Y, p2.Y);
            Zn.smallZ = Math.Min(p1.Z, p2.Z);
            Zn.bigX = Math.Max(p1.X, p2.X);
            Zn.bigY = Math.Max(p1.Y, p2.Y);
            Zn.bigZ = Math.Max(p1.Z, p2.Z);

            TntWarsGame it = TntWarsGame.GetTntWarsGame(p);
            if (it == null) {
                Player.Message(p, "TNT Wars Error: Couldn't find your game!");
            } else if (NoTntZone) {
                it.NoTNTplacableZones.Add(Zn);
            } else {
                it.NoBlockDeathZones.Add(Zn);
            }
            
            Player.Message(p, "Added zone");
            return false;
        }
    }
}
