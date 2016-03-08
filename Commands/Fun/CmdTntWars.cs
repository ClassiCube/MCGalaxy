/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
using System;
using System.Linq;
using System.Threading;
namespace MCGalaxy.Commands
{
	public sealed class CmdTntWars : Command
	{
		public override string name { get { return "tntwars"; } }
		public override string shortcut { get { return "tw"; } }
		public override string type { get { return CommandTypes.Games; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "The lowest rank that can use admin commands for tntwars", 1) }; }
        }

        public bool DeleteZone = false;
        public bool CheckZone = false;
        public bool NoTntZone = false;

		public override void Use(Player p, string message)
		{
			string[] text = new string[5];
			text[0] = "";
			text[1] = "";
			text[2] = "";
			text[3] = "";
			text[4] = "";
			try
			{
				text[0] = message.ToLower().Split(' ')[0];
				text[1] = message.ToLower().Split(' ')[1];
				text[2] = message.ToLower().Split(' ')[2];
				text[3] = message.ToLower().Split(' ')[3];
				text[4] = message.ToLower().Split(' ')[4];
			}
			catch { }

			switch (text[0])
			{
				case "list":
				case "levels":
				case "l":
					if (TntWarsGame.GameList.Count <= 0)
					{
						Player.SendMessage(p, "There aren't any " + Colors.red + "TNT Wars " + Server.DefaultColor + "currently running!");
						return;
					}
					else
					{
						Player.SendMessage(p, "Currently running " + Colors.red + "TNT Wars" + Server.DefaultColor + ":");
						foreach (TntWarsGame T in TntWarsGame.GameList)
						{
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
							Player.SendMessage(p, msg);
						}
					}
					break;

				case "join":
					if (p.PlayingTntWars || (TntWarsGame.GetTntWarsGame(p) != null && TntWarsGame.GetTntWarsGame(p).Players.Contains(TntWarsGame.GetTntWarsGame(p).FindPlayer(p))))
					{
						Player.SendMessage(p, "TNT Wars Error: You have already joined a game!");
						return;
					}
					else
					{
						TntWarsGame it;
						bool add = true;
						if (text[1] == "red" || text[1] == "r" || text[1] == "1" || text[1] == "blue" || text[1] == "b" || text[1] == "2" || text[1] == "auto" || text[1] == "a" || text[1] == "")
						{
							it = TntWarsGame.Find(p.level);
							if (it == null)
							{
								Player.SendMessage(p, "TNT Wars Error: There isn't a game on your current level!");
								return;
							}
						}
						else
						{
							Level lvl = LevelInfo.Find(text[1]);
							if (lvl == null)
							{
								Player.SendMessage(p, "TNT Wars Error: Couldn't find level '" + text[1] + "'");
								return;
							}
							else
							{
								it = TntWarsGame.Find(lvl);
								if (it == null)
								{
									Player.SendMessage(p, "TNT Wars Error: There isn't a game on that level!");
									return;
								}
								else
								{
									text[1] = text[2]; //so the switch later on still works 
								}
							}
						}
						TntWarsGame.player pl = new TntWarsGame.player(p);
						if (it.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart || it.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod || it.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress)
						{
							pl.spec = true;
						}
						if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM)
						{
							int Red = it.RedTeam();
							int Blue = it.BlueTeam();
							switch (text[1])
							{
								case "red":
								case "r":
								case "1":
									if (it.BalanceTeams)
									{
										if (Red > Blue)
										{
											add = false;
											Player.SendMessage(p, "TNT Wars Error: Red has too many players!");
											return;
										}
									}
									pl.Red = true;
									break;

								case "blue":
								case "b":
								case "2":
									if (it.BalanceTeams)
									{
										if (Blue > Red)
										{
											add = false;
											Player.SendMessage(p, "TNT Wars Error: Blue has too many players!");
											return;
										}
									}
									pl.Blue = true;
									break;

								case "auto":
								case "a":
								default:
									if (Blue > Red)
									{
										pl.Red = true;
										break;
									}
									else if (Red > Blue)
									{
										pl.Blue = true;
										break;
									}
									else if (it.RedScore > it.BlueScore)
									{
										pl.Blue = true;
										break;
									}
									else if (it.BlueScore > it.RedScore)
									{
										pl.Red = true;
										break;
									}
									else
									{
										pl.Red = true;
										break;
									}
							}
						}
						else
						{
							pl.Red = false;
							pl.Blue = false;
						}
						if (add)
						{
							it.Players.Add(pl);
							TntWarsGame.SetTitlesAndColor(pl);
							p.CurrentTntGameNumber = it.GameNumber;
							string msg = p.color + p.name + Server.DefaultColor + " " + "joined TNT Wars on '" + it.lvl.name + "'";
							if (pl.Red)
							{
								msg += " on the " + Colors.red + "red team";
							}
							if (pl.Blue)
							{
								msg += " on the " + Colors.blue + "blue team";
							}
							if (pl.spec)
							{
								msg += Server.DefaultColor + " (as a spectator)";
							}
							Player.GlobalMessage(msg);
						}

					}
					break;

				case "leave":
				case "exit":
					p.canBuild = true;
					TntWarsGame game = TntWarsGame.GetTntWarsGame(p);
					game.Players.Remove(game.FindPlayer(p));
					game.SendAllPlayersMessage("TNT Wars: " + p.color + p.name + Server.DefaultColor + " left the TNT Wars game!");
					TntWarsGame.SetTitlesAndColor(game.FindPlayer(p), true);
					Player.SendMessage(p, "TNT Wars: You left the game");
					break;

				case "rules":
				case "rule":
				case "r":
					switch (text[1])
					{
						case "all":
						case "a":
                        case "everyone":
							if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
                            {
								Player[] players = PlayerInfo.Online;
                                foreach (Player who in players)
                                {
                                    Player.SendMessage(who, "TNT Wars Rules: (sent to all players by " + p.color + p.name + Server.DefaultColor + " )");
                                    Player.SendMessage(who, "The aim of the game is to blow up people using TNT!");
                                    Player.SendMessage(who, "To place tnt simply place a TNT block and after a short delay it shall explode!");
                                    Player.SendMessage(who, "During the game the amount of TNT placable at one time may be limited!");
                                    Player.SendMessage(who, "You are not allowed to use hacks of any sort during the game!");
                                }
                                Player.SendMessage(p, "TNT Wars: Sent rules to all players");
                                return;
                            }
							break;

						case "level":
						case "l":
                        case "lvl":
                        case "map":
                        case "m":
                            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
                            {
                                foreach (Player who in p.level.players)
                                {
                                    Player.SendMessage(who, "TNT Wars Rules: (sent to all players in map by " + p.color + p.name + Server.DefaultColor + " )");
                                    Player.SendMessage(who, "The aim of the game is to blow up people using TNT!");
                                    Player.SendMessage(who, "To place tnt simply place a TNT block and after a short delay it shall explode!");
                                    Player.SendMessage(who, "During the game the amount of TNT placable at one time may be limited!");
                                    Player.SendMessage(who, "You are not allowed to use hacks of any sort during the game!");
                                        
                                }
                                Player.SendMessage(p, "TNT Wars: Sent rules to all current players in map");
                                return;
                            }
							break;

						case "players":
                        case "pls":
                        case "pl":
						case "p":
                            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
                            {
                                TntWarsGame gm = TntWarsGame.GetTntWarsGame(p);
                                if (gm == null)
                                {
                                    Player.SendMessage(p, "TNT Wars Error: You aren't in a TNT Wars game!");
                                    return;
                                }
                                else
                                {
                                    foreach (TntWarsGame.player who in gm.Players)
                                    {
                                        Player.SendMessage(who.p, "TNT Wars Rules: (sent to all current players by " + p.color + p.name + Server.DefaultColor + " )");
                                        Player.SendMessage(who.p, "The aim of the game is to blow up people using TNT!");
                                        Player.SendMessage(who.p, "To place tnt simply place a TNT block and after a short delay it shall explode!");
                                        Player.SendMessage(who.p, "During the game the amount of TNT placable at one time may be limited!");
                                        Player.SendMessage(who.p, "You are not allowed to use hacks of any sort during the game!");
                                        
                                    }
                                    Player.SendMessage(p, "TNT Wars: Sent rules to all current players");
                                    return;
                                }
                            }
							break;

						default:
                            if (text[1] != null && (int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
                            {
                                Player who = PlayerInfo.Find(text[1]);
                                if (who != null)
                                {
                                    Player.SendMessage(who, "TNT Wars Rules: (sent to you by " + p.color + p.name + Server.DefaultColor + " )");
                                    Player.SendMessage(who, "The aim of the game is to blow up people using TNT!");
                                    Player.SendMessage(who, "To place tnt simply place a TNT block and after a short delay it shall explode!");
                                    Player.SendMessage(who, "During the game the amount of TNT placable at one time may be limited!");
                                    Player.SendMessage(who, "You are not allowed to use hacks of any sort during the game!");
                                    Player.SendMessage(p, "TNT Wars: Sent rules to " + who.color + who.name);
                                    return;
                                }
                                else
                                {
                                    Player.SendMessage(p, "TNT Wars Error: Couldn't find player '" + text[1] + "' to send rules to!");
                                    return;
                                }
                            }
                            else
                            {
                                Player.SendMessage(p, "TNT Wars Rules:");
                                Player.SendMessage(p, "The aim of the game is to blow up people using TNT!");
                                Player.SendMessage(p, "To place tnt simply place a TNT block and after a short delay it shall explode!");
                                Player.SendMessage(p, "During the game the amount of TNT placable at one time may be limited!");
                                Player.SendMessage(p, "You are not allowed to use hacks of any sort during the game!");
                                return;
                            }
							//break;
					}
					break;

				case "score":
				case "scores":
				case "leaderboard":
				case "board":
					TntWarsGame tntwrs = TntWarsGame.GetTntWarsGame(p);
					switch (text[1])
					{
						case "top":
						case "leaders":
							if (tntwrs.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress)
							{
								int max = 5;
								if (tntwrs.PlayingPlayers() < 5)
								{
									max = tntwrs.PlayingPlayers();
								}

								var pls = from pla in tntwrs.Players orderby pla.Score descending select pla; //LINQ FTW
								int count = 1;
								foreach (var pl in pls)
								{
									Player.SendMessage(p, count.ToString() + ": " + pl.p.name + " - " + pl.Score.ToString());
									if (count >= max)
									{
										break;
									}
									count++;
									Thread.Sleep(500); //Maybe, not sure (250??)
								}
							}
							else
							{
								Player.SendMessage(p, "TNT Wars Error: Can't display scores - game not in progress!");
							}
							break;

						case "teams":
						case "team":
						case "t":
						case "red":
						case "blue":
							if (tntwrs.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress)
							{
								if (tntwrs.GameMode == TntWarsGame.TntWarsGameMode.TDM)
								{
									Player.SendMessage(p, "TNT Wars Scores:");
									Player.SendMessage(p, Colors.red + "RED: " + Colors.white + tntwrs.RedScore + " " + Colors.red + "(" + (tntwrs.ScoreLimit - tntwrs.RedScore).ToString() + " needed)");
									Player.SendMessage(p, Colors.blue + "BLUE: " + Colors.white + tntwrs.BlueScore + " " + Colors.red + "(" + (tntwrs.ScoreLimit - tntwrs.BlueScore).ToString() + " needed)");
								}
								else
								{
									Player.SendMessage(p, "TNT Wars Error: Can't display team scores as this isn't team deathmatch!");
								}
							}
							else
							{
								Player.SendMessage(p, "TNT Wars Error: Can't display scores - game not in progress!");
							}
							break;

						case "me":
						case "mine":
						case "score":
						case "i":
						default:
							if (tntwrs.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress)
							{
								Player.SendMessage(p, "TNT Wars: Your Score: " + Colors.white + TntWarsGame.GetTntWarsGame(p).FindPlayer(p).Score);
							}
							else
							{
								Player.SendMessage(p, "TNT Wars Error: Can't display scores - game not in progress!");
							}
							break;
					}
					return;

				case "players":
				case "player":
				case "ps":
				case "pl":
				case "p":
					Player.SendMessage(p, "TNT Wars: People playing TNT Wars on '" + TntWarsGame.GetTntWarsGame(p).lvl.name + "':");
					foreach (TntWarsGame.player pl in TntWarsGame.GetTntWarsGame(p).Players)
					{
						if (TntWarsGame.GetTntWarsGame(p).GameMode == TntWarsGame.TntWarsGameMode.TDM)
						{
							if (pl.Red && pl.spec)
								Player.SendMessage(p, pl.p.color + pl.p.name + Server.DefaultColor + " - " + Colors.red + "RED" + Server.DefaultColor + " (spectator)");
							else if (pl.Blue && pl.spec)
								Player.SendMessage(p, pl.p.color + pl.p.name + Server.DefaultColor + " - " + Colors.blue + "BLUE" + Server.DefaultColor + " (spectator)");
							else if (pl.Red)
								Player.SendMessage(p, pl.p.color + pl.p.name + Server.DefaultColor + " - " + Colors.red + "RED" + Server.DefaultColor);
							else if (pl.Blue)
								Player.SendMessage(p, pl.p.color + pl.p.name + Server.DefaultColor + " - " + Colors.blue + "BLUE" + Server.DefaultColor);
						}
						else
						{
							if (pl.spec)
								Player.SendMessage(p, pl.p.color + pl.p.name + Server.DefaultColor + " (spectator)");
							else
								Player.SendMessage(p, pl.p.color + pl.p.name);
						}
					}
					break;

				case "health":
				case "heal":
				case "hp":
				case "hlth":
					if (TntWarsGame.GetTntWarsGame(p).GameStatus == TntWarsGame.TntWarsGameStatus.InProgress)
					{
						Player.SendMessage(p, "TNT Wars: You have " + p.TntWarsHealth.ToString() + " health left");
					}
					else
					{
						Player.SendMessage(p, "TNT Wars Error: Can't display health - game not in progress!");
						return;
					}
					break;

				case "setup":
				case "s":
					if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
					{
						bool justcreated = false;
						TntWarsGame it = TntWarsGame.FindFromGameNumber(p.CurrentTntGameNumber);
						if (it == null)
						{
							if (text[1] != "new" && text[1] != "n")
							{
								Player.SendMessage(p, "TNT Wars Error: You must create a new game by typing '/tntwars setup new'");
								return;
							}
						}
						if (it != null)
						{
							if (it.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress || it.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod || it.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart)
							{
								if (text[1] != "stop" && text[1] != "s" && text[1] != "" && text[1] != "status" && text[1] != "ready" && text[1] != "check" && text[1] != "info" && text[1] != "r" && text[1] != "c")
								{
									Player.SendMessage(p, "TNT Wars Error: Cannot edit current game because it is currently running!");
									return;
								}
							}
						}
                        switch (text[1])
                        {
                            case "new":
                            case "n":
                                if (it != null)
                                {
                                    if (it.FindPlayer(p) != null)
                                    {
                                        Player.SendMessage(p, "TNT Wars Error: Please leave the current game first!");
                                        return;
                                    }
                                }
                                if (it == null || it.lvl != p.level)
                                {
                                    it = new TntWarsGame(p.level);
                                    it.GameNumber = TntWarsGame.GameList.Count + 1;
                                    TntWarsGame.GameList.Add(it);
                                    p.CurrentTntGameNumber = it.GameNumber;
                                    Player.SendMessage(p, "TNT Wars: Created New TNT Wars game on '" + p.level.name + "'");
                                    justcreated = true;
                                    return;
                                }
                                else if (justcreated == false)
                                {
                                    Player.SendMessage(p, "TNT Wars Error: Please delete the current game first!");
                                    return;
                                }
                                break;

                            case "delete":
                            case "remove":
                                if (it.GameStatus != TntWarsGame.TntWarsGameStatus.Finished && it.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers)
                                {
                                    Player.SendMessage(p, "Please stop the game first!");
                                    return;
                                }
                                else
                                {
                                    foreach (TntWarsGame.player pl in it.Players)
                                    {
                                        pl.p.CurrentTntGameNumber = -1;
                                        Player.SendMessage(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                                        pl.p.PlayingTntWars = false;
                                        pl.p.canBuild = true;
                                        TntWarsGame.SetTitlesAndColor(pl, true);
                                    }
                                    Player.SendMessage(p, "TNT Wars: Game deleted");
                                    TntWarsGame.GameList.Remove(it);
                                    return;
                                }
                                //break;

                            case "reset":
                            case "r":
                                if (it.GameStatus != TntWarsGame.TntWarsGameStatus.Finished)
                                {
                                    Player.SendMessage(p, "TNT Wars Error: The game has to have finished to be reset!");
                                    return;
                                }
                                else
                                {
                                    it.GameStatus = TntWarsGame.TntWarsGameStatus.WaitingForPlayers;
                                    Command.all.Find("restore").Use(null, it.BackupNumber + it.lvl.name);
                                    it.RedScore = 0;
                                    it.BlueScore = 0;
                                    foreach (TntWarsGame.player pl in it.Players)
                                    {
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
                                    Player.SendMessage(p, "TNT Wars: Reset TNT Wars");
                                }
                                break;

                            case "start":
                                if (it.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers)
                                {
                                    if (it.CheckAllSetUp(p, true))
                                    {
                                        if (it.PlayingPlayers() >= 2)
                                        {
                                            if (it.lvl.overload < 2500)
                                            {
                                                it.lvl.overload = 2501;
                                                Player.SendMessage(p, "TNT Wars: Increasing physics overload to 2500");
                                                Server.s.Log("TNT Wars: Increasing physics overload to 2500");
                                            }
                                            Thread t = new Thread(it.Start);
                                            t.Name = "MCG_TntGame";
                                            t.Start();
                                        }
                                        else
                                        {
                                            Player.SendMessage(p, "TNT Wars Error: Not Enough Players (2 or more needed)");
                                            return;
                                        }
                                    }
                                }
                                else if (it.GameStatus == TntWarsGame.TntWarsGameStatus.Finished)
                                {
                                    Player.SendMessage(p, "TNT Wars Error: Please use '/tntwars setup reset' to reset the game before starting!");
                                    return;
                                }
                                else
                                {
                                    Player.SendMessage(p, "TNT Wars Error: Game already in progress!!");
                                }
                                return;

                            case "stop":
                                if (it.GameStatus == TntWarsGame.TntWarsGameStatus.Finished || it.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers)
                                {
                                    Player.SendMessage(p, "TNT Wars Error: Game already ended / not started!");
                                    return;
                                }
                                else
                                {
                                    foreach (TntWarsGame.player pl in it.Players)
                                    {
                                        pl.p.canBuild = true;
                                        pl.p.PlayingTntWars = false;
                                        pl.p.CurrentAmountOfTnt = 0;
                                    }
                                    it.GameStatus = TntWarsGame.TntWarsGameStatus.Finished;
                                    it.SendAllPlayersMessage("TNT Wars: Game has been stopped!");
                                }
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
                                if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA) { Player.SendMessage(p, "TNT Wars Error: Cannot set spawns because you are on Team Deathmatch!"); return; }
                                switch (text[2])
                                {
                                    case "red":
                                    case "r":
                                    case "1":
                                        it.RedSpawn = new ushort[5];
                                        it.RedSpawn[0] = (ushort)(p.pos[0] / 32);
                                        it.RedSpawn[1] = (ushort)(p.pos[1] / 32);
                                        it.RedSpawn[2] = (ushort)(p.pos[2] / 32);
                                        it.RedSpawn[3] = p.rot[0];
                                        it.RedSpawn[4] = p.rot[1];
                                        Player.SendMessage(p, "TNT Wars: Set " + Colors.red + "Red" + Server.DefaultColor + " spawn");
                                        break;

                                    case "blue":
                                    case "b":
                                    case "2":
                                        it.BlueSpawn = new ushort[5];
                                        it.BlueSpawn[0] = (ushort)(p.pos[0] / 32);
                                        it.BlueSpawn[1] = (ushort)(p.pos[1] / 32);
                                        it.BlueSpawn[2] = (ushort)(p.pos[2] / 32);
                                        it.BlueSpawn[3] = p.rot[0];
                                        it.BlueSpawn[4] = p.rot[1];
                                        Player.SendMessage(p, "TNT Wars: Set " + Colors.blue + "Blue" + Server.DefaultColor + " spawn");
                                        break;
                                }
                                break;

                            case "level":
                            case "l":
                            case "lvl":
                                if (text[2] == "")
                                {
                                    it.lvl = p.level;
                                }
                                else
                                {
                                    it.lvl = LevelInfo.Find(text[2]);
                                    if (it.lvl == null)
                                    {
                                        Player.SendMessage(p, "TNT Wars Error: '" + text[2] + "' is not a level!");
                                        return;
                                    }
                                }
                                Player.SendMessage(p, "TNT Wars: Level is now '" + it.lvl.name + "'");
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
                                try
                                {
                                    number = int.Parse(text[2]);
                                }
                                catch
                                {
                                    Player.SendMessage(p, "TNT Wars Error: '" + text[2] + "' is not a number!");
                                    return;
                                }
                                if (number >= 0)
                                {
                                    it.TntPerPlayerAtATime = number;
                                    if (number == 0)
                                    {
                                        Player.SendMessage(p, "TNT Wars: Number of TNTs placeable by a player at a time is now unlimited");
                                    }
                                    else
                                    {
                                        Player.SendMessage(p, "TNT Wars: Number of TNTs placeable by a player at a time is now " + number.ToString());
                                    }
                                    it.CheckAllSetUp(p);
                                }
                                break;

                            case "grace":
                            case "g":
                            case "graceperiod":
                                if (text[1] == "grace" || text[1] == "g")
                                {
                                    if (text[2] == "time" || text[2] == "t")
                                    {
                                        text[1] = "gt";
                                        text[2] = text[3];
                                        break;
                                    }
                                }
                                string msg;
                                if (text[2] != "")
                                {
                                    switch (text[2])
                                    {
                                        case "true":
                                        case "yes":
                                        case "t":
                                        case "y":
                                        case "enable":
                                        case "e":
                                        case "on":
                                            it.GracePeriod = true;
                                            Player.SendMessage(p, "TNT Wars: Grace period is now enabled");
                                            return;

                                        case "false":
                                        case "no":
                                        case "f":
                                        case "n":
                                        case "disable":
                                        case "d":
                                        case "off":
                                            it.GracePeriod = false;
                                            Player.SendMessage(p, "TNT Wars: Grace period is now disabled");
                                            return;

                                        case "check":
                                        case "current":
                                        case "c":
                                        case "now":
                                        case "ATM":
                                            if (it.GracePeriod) msg = "enabled";
                                            else msg = "disabled";
                                            Player.SendMessage(p, "TNT Wars: Grace Period is currently " + msg);
                                            return;

                                        default:
                                            it.GracePeriod = !it.GracePeriod;
                                            break;
                                    }
                                }
                                else
                                {
                                    it.GracePeriod = !it.GracePeriod;
                                }
                                {
                                    if (it.GracePeriod) msg = "enabled";
                                    else msg = "disabled";
                                    Player.SendMessage(p, "TNT Wars: Grace Period is now " + msg);
                                }
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
                                        Player.SendMessage(p, "TNT Wars: Current grace time is " + it.GracePeriodSecs.ToString() + " seconds long!");
                                        break;

                                    default:
                                        if (text[2] == "set" || text[2] == "s" || text[2] == "change")
                                        {
                                            text[2] = text[3];
                                        }
                                        int numb = -1;
                                        if (int.TryParse(text[2], out numb) == false)
                                        { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[2] + "'"); return; }
                                        if (numb <= -1) { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[2] + "'"); return; }
                                        if (numb >= (60 * 5)) { Player.SendMessage(p, "TNT Wars Error: Grace time cannot be above 5 minutes!!"); return; }
                                        if (numb <= 9) { Player.SendMessage(p, "TNT Wars Error: Grace time cannot be lower than 10 seconds!!"); return; }
                                        else
                                        {
                                            it.GracePeriodSecs = numb;
                                            Player.SendMessage(p, "TNT Wars: Grace period is now " + numb.ToString() + " seconds long!");
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
                                        if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA) Player.SendMessage(p, "TNT Wars: The current game mode is Free For All");
                                        if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM) Player.SendMessage(p, "TNT Wars: The current game mode is Team Deathmatch");
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
                                        if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA)
                                        {
                                            it.GameMode = TntWarsGame.TntWarsGameMode.TDM;
                                            if (!it.Players.Contains(it.FindPlayer(p)))
                                            {
                                                Player.SendMessage(p, "TNT Wars: Changed gamemode to Team Deathmatch");
                                            }
                                            foreach (TntWarsGame.player pl in it.Players)
                                            {
                                                {
                                                    Player.SendMessage(pl.p, "TNT Wars: Changed gamemode to Team Deathmatch");
                                                    pl.Red = false;
                                                    pl.Blue = false;
                                                    if (it.BlueTeam() > it.RedTeam())
                                                    {
                                                        pl.Red = true;
                                                    }
                                                    else if (it.RedTeam() > it.BlueTeam())
                                                    {
                                                        pl.Blue = true;
                                                    }
                                                    else if (it.RedScore > it.BlueScore)
                                                    {
                                                        pl.Blue = true;
                                                    }
                                                    else if (it.BlueScore > it.RedScore)
                                                    {
                                                        pl.Red = true;
                                                    }
                                                    else
                                                    {
                                                        pl.Red = true;
                                                    }
                                                }
                                                {
                                                    string mesg = pl.p.color + pl.p.name + Server.DefaultColor + " " + "is now";
                                                    if (pl.Red)
                                                    {
                                                        mesg += " on the " + Colors.red + "red team";
                                                    }
                                                    if (pl.Blue)
                                                    {
                                                        mesg += " on the " + Colors.blue + "blue team";
                                                    }
                                                    if (pl.spec)
                                                    {
                                                        mesg += Server.DefaultColor + " (as a spectator)";
                                                    }
                                                    Player.GlobalMessage(mesg);
                                                }
                                            }
                                            if (it.ScoreLimit == TntWarsGame.Properties.DefaultFFAmaxScore)
                                            {
                                                it.ScoreLimit = TntWarsGame.Properties.DefaultTDMmaxScore;
                                                Player.SendMessage(p, "TNT Wars: Score limit is now " + it.ScoreLimit.ToString() + " points!");
                                            }
                                            else
                                            {
                                                Player.SendMessage(p, "TNT Wars: Score limit is still " + it.ScoreLimit.ToString() + " points!");
                                            }
                                        }
                                        else
                                        {
                                            Player.SendMessage(p, "TNT Wars Error: Gamemode is already Team Deathmatch!");
                                            return;
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
                                        if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM)
                                        {
                                            it.GameMode = TntWarsGame.TntWarsGameMode.FFA;
                                            if (!it.Players.Contains(it.FindPlayer(p)))
                                            {
                                                Player.SendMessage(p, "TNT Wars: Changed gamemode to Free For All");
                                            }
                                            it.SendAllPlayersMessage("TNT Wars: Changed gamemode to Free For All");
                                            if (it.ScoreLimit == TntWarsGame.Properties.DefaultTDMmaxScore)
                                            {
                                                it.ScoreLimit = TntWarsGame.Properties.DefaultFFAmaxScore;
                                                Player.SendMessage(p, "TNT Wars: Score limit is now " + it.ScoreLimit.ToString() + " points!");
                                            }
                                            else
                                            {
                                                Player.SendMessage(p, "TNT Wars: Score limit is still " + it.ScoreLimit.ToString() + " points!");
                                            }
                                            foreach (TntWarsGame.player pl in it.Players)
                                            {
                                                pl.p.color = pl.OldColor;
                                                pl.p.SetPrefix();
                                            }
                                        }
                                        else
                                        {
                                            Player.SendMessage(p, "TNT Wars Error: Gamemode is already Free For All!");
                                            return;
                                        }
                                        break;

                                    case "swap":
                                    case "s":
                                    case "change":
                                    case "edit":
                                    case "switch":
                                    default:
                                        {
                                            if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA)
                                            {
                                                it.GameMode = TntWarsGame.TntWarsGameMode.TDM;
                                                if (!it.Players.Contains(it.FindPlayer(p)))
                                                {
                                                    Player.SendMessage(p, "TNT Wars: Changed gamemode to Team Deathmatch");
                                                }
                                                int Red = it.RedTeam();
                                                int Blue = it.BlueTeam();
                                                foreach (TntWarsGame.player pl in it.Players)
                                                {
                                                    {
                                                        Player.SendMessage(pl.p, "TNT Wars: Changed gamemode to Team Deathmatch");
                                                        pl.Red = false;
                                                        pl.Blue = false;
                                                        if (Blue > Red)
                                                        {
                                                            pl.Red = true;
                                                        }
                                                        else if (Red > Blue)
                                                        {
                                                            pl.Blue = true;
                                                        }
                                                        else if (it.RedScore > it.BlueScore)
                                                        {
                                                            pl.Blue = true;
                                                        }
                                                        else if (it.BlueScore > it.RedScore)
                                                        {
                                                            pl.Red = true;
                                                        }
                                                        else
                                                        {
                                                            pl.Red = true;
                                                        }
                                                    }
                                                    string mesg = p.color + p.name + Server.DefaultColor + " " + "is now";
                                                    if (pl.Red)
                                                    {
                                                        mesg += " on the " + Colors.red + "red team";
                                                    }
                                                    if (pl.Blue)
                                                    {
                                                        mesg += " on the " + Colors.blue + "blue team";
                                                    }
                                                    if (pl.spec)
                                                    {
                                                        mesg += Server.DefaultColor + " (as a spectator)";
                                                    }
                                                    Player.GlobalMessage(mesg);
                                                }
                                                if (it.ScoreLimit == TntWarsGame.Properties.DefaultFFAmaxScore)
                                                {
                                                    it.ScoreLimit = TntWarsGame.Properties.DefaultTDMmaxScore;
                                                    Player.SendMessage(p, "TNT Wars: Score limit is now " + it.ScoreLimit.ToString() + " points!");
                                                }
                                                else
                                                {
                                                    Player.SendMessage(p, "TNT Wars: Score limit is still " + it.ScoreLimit.ToString() + " points!");
                                                }
                                            }
                                            else
                                            {
                                                it.GameMode = TntWarsGame.TntWarsGameMode.FFA;
                                                if (!it.Players.Contains(it.FindPlayer(p)))
                                                {
                                                    Player.SendMessage(p, "TNT Wars: Changed gamemode to Free For All");
                                                }
                                                it.SendAllPlayersMessage("TNT Wars: Changed gamemode to Free For All");
                                                if (it.ScoreLimit == TntWarsGame.Properties.DefaultTDMmaxScore)
                                                {
                                                    it.ScoreLimit = TntWarsGame.Properties.DefaultFFAmaxScore;
                                                    Player.SendMessage(p, "TNT Wars: Score limit is now " + it.ScoreLimit.ToString() + " points!");
                                                }
                                                else
                                                {
                                                    Player.SendMessage(p, "TNT Wars: Score limit is still " + it.ScoreLimit.ToString() + " points!");
                                                }
                                            }

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
                                        if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Easy) Player.SendMessage(p, "TNT Wars Error: Already on easy difficulty!");
                                        else
                                        {
                                            it.GameDifficulty = TntWarsGame.TntWarsDifficulty.Easy;
                                            if (!it.Players.Contains(it.FindPlayer(p))) Player.SendMessage(p, "TNT Wars: Changed difficulty to easy");
                                            it.SendAllPlayersMessage("TNT Wars: Changed difficulty to easy!");
                                            if (it.TeamKills)
                                            {
                                                Player.SendMessage(p, "TNT Wars: Team killing is now off");
                                                it.TeamKills = false;
                                            }
                                        }
                                        break;

                                    case "normal":
                                    case "n":
                                    case "medium":
                                    case "m":
                                    case "2":
                                    case "2nd":
                                    default:
                                        if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Normal) Player.SendMessage(p, "TNT Wars Error: Already on normal difficulty!");
                                        else
                                        {
                                            it.GameDifficulty = TntWarsGame.TntWarsDifficulty.Normal;
                                            if (!it.Players.Contains(it.FindPlayer(p))) Player.SendMessage(p, "TNT Wars: Changed difficulty to normal");
                                            it.SendAllPlayersMessage("TNT Wars: Changed difficulty to normal!");
                                            if (it.TeamKills)
                                            {
                                                Player.SendMessage(p, "TNT Wars: Team killing is now off");
                                                it.TeamKills = false;
                                            }
                                        }
                                        break;

                                    case "hard":
                                    case "h":
                                    case "difficult":
                                    case "d":
                                    case "3":
                                    case "3rd":
                                        if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Hard) Player.SendMessage(p, "TNT Wars Error: Already on hard difficulty!");
                                        else
                                        {
                                            it.GameDifficulty = TntWarsGame.TntWarsDifficulty.Hard;
                                            if (!it.Players.Contains(it.FindPlayer(p))) Player.SendMessage(p, "TNT Wars: Changed difficulty to hard");
                                            it.SendAllPlayersMessage("TNT Wars: Changed difficulty to hard!");
                                            if (it.TeamKills == false)
                                            {
                                                Player.SendMessage(p, "TNT Wars: Team killing is now on");
                                                it.TeamKills = true;
                                            }
                                        }
                                        break;

                                    case "extreme":
                                    case "ex":
                                    case "hardest":
                                    case "impossible":
                                    case "ultimate":
                                    case "i":
                                    case "u":
                                    case "4":
                                    case "4th":
                                        if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Extreme) Player.SendMessage(p, "TNT Wars Error: Already on extreme difficulty!");
                                        else
                                        {
                                            it.GameDifficulty = TntWarsGame.TntWarsDifficulty.Extreme;
                                            if (!it.Players.Contains(it.FindPlayer(p))) Player.SendMessage(p, "TNT Wars: Changed difficulty to extreme");
                                            it.SendAllPlayersMessage("TNT Wars: Changed difficulty to extreme!");
                                            if (it.TeamKills == false)
                                            {
                                                Player.SendMessage(p, "TNT Wars: Team killing is now on");
                                                it.TeamKills = true;
                                            }
                                        }
                                        break;


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
                                                Player.SendMessage(p, "TNT Wars: Score limit is " + it.ScoreLimit.ToString() + " points!");
                                                break;

                                            default:
                                                if (text[3] == "set" || text[3] == "s" || text[3] == "change")
                                                {
                                                    text[3] = text[4];
                                                }
                                                int numb = -1;
                                                if (int.TryParse(text[3], out numb) == false)
                                                { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[3] + "'"); return; }
                                                if (numb <= it.ScorePerKill) { Player.SendMessage(p, "TNT Wars Error: Minimum score limit of " + it.ScorePerKill.ToString() + " points"); return; }
                                                else
                                                {
                                                    it.ScoreLimit = numb;
                                                    Player.SendMessage(p, "TNT Wars: Score limit is now " + numb.ToString() + " points!");
                                                    return;
                                                }
                                                //break;
                                        }
                                        it.CheckAllSetUp(p);
                                        break;

                                    case "streaks":
                                    case "streak":
                                    case "s":
                                        switch (text[3])
                                        {
                                            case "on":
                                            case "enable":
                                                if (it.Streaks)
                                                {
                                                    Player.SendMessage(p, "TNT Wars Error: Streaks are already enabled");
                                                    return;
                                                }
                                                else
                                                {
                                                    it.Streaks = true;
                                                    Player.SendMessage(p, "TNT Wars: Streaks are now enabled");
                                                }
                                                break;

                                            case "off":
                                            case "disable":
                                                if (it.Streaks == false)
                                                {
                                                    Player.SendMessage(p, "TNT Wars Error:  Streaks are already disabled");
                                                    return;
                                                }
                                                else
                                                {
                                                    it.Streaks = false;
                                                    Player.SendMessage(p, "TNT Wars: Streaks are now disabled");
                                                }
                                                break;

                                            case "check":
                                            case "current":
                                            case "now":
                                            case "ATM":
                                            case "c":
                                            case "t":
                                                if (it.Streaks) { Player.SendMessage(p, "TNT Wars: Streaks are currently enabled"); return; }
                                                else if (it.Streaks == false) { Player.SendMessage(p, "TNT Wars: Streaks are currently disabled"); return; }
                                                break;

                                            default:
                                                if (it.Streaks == false)
                                                {
                                                    it.Streaks = true;
                                                    Player.SendMessage(p, "TNT Wars: Streaks are now enabled");
                                                    return;
                                                }
                                                else if (it.Streaks)
                                                {
                                                    it.Streaks = false;
                                                    Player.SendMessage(p, "TNT Wars: Streaks are now disabled");
                                                    return;
                                                }
                                                break;
                                        }
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
                                        switch (text[3])
                                        {
                                            case "on":
                                            case "enable":
                                                if (it.MultiKillBonus > 0)
                                                {
                                                    Player.SendMessage(p, "TNT Wars Error: Multikill bonuses are already enabled (at " + it.MultiKillBonus.ToString() + " points!");
                                                    return;
                                                }
                                                else
                                                {
                                                    it.MultiKillBonus = TntWarsGame.Properties.DefaultMultiKillBonus;
                                                    Player.SendMessage(p, "TNT Wars: Multikill bonuses are now enabled at " + it.MultiKillBonus.ToString() + " points!");
                                                }
                                                break;

                                            case "off":
                                            case "disable":
                                                if (it.MultiKillBonus == 0)
                                                {
                                                    Player.SendMessage(p, "TNT Wars Error: Multikill bonuses are already disabled!");
                                                    return;
                                                }
                                                else
                                                {
                                                    it.MultiKillBonus = 0;
                                                    Player.SendMessage(p, "TNT Wars: Multikill bonuses are now disabled!");
                                                }
                                                break;

                                            case "switch":
                                                if (it.MultiKillBonus == 0)
                                                {
                                                    it.MultiKillBonus = TntWarsGame.Properties.DefaultMultiKillBonus;
                                                    Player.SendMessage(p, "TNT Wars: Multikill bonuses are now enabled at " + it.MultiKillBonus.ToString() + " points!");
                                                    return;
                                                }
                                                else
                                                {
                                                    it.MultiKillBonus = 0;
                                                    Player.SendMessage(p, "TNT Wars: Multikill bonuses are now disabled!");
                                                }
                                                break;

                                            case "check":
                                            case "current":
                                            case "now":
                                            case "ATM":
                                            case "c":
                                            case "t":
                                                Player.SendMessage(p, "TNT Wars: Mulitkill bonus per extra kill is " + it.MultiKillBonus.ToString() + " points!");
                                                break;

                                            default:
                                                if (text[3] == "set" || text[3] == "s" || text[3] == "change")
                                                {
                                                    text[3] = text[4];
                                                }
                                                int numb = -1;
                                                if (int.TryParse(text[3], out numb) == false)
                                                { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[3] + "'"); return; }
                                                if (numb <= -1) { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[3] + "'"); return; }
                                                else
                                                {
                                                    it.MultiKillBonus = numb;
                                                    Player.SendMessage(p, "TNT Wars: Mulitkill bonus per extra kill is now " + numb.ToString() + " points!");
                                                    return;
                                                }
                                                //break;
                                        }
                                        it.CheckAllSetUp(p);
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
                                                Player.SendMessage(p, "TNT Wars: Score per kill is " + it.ScorePerKill.ToString() + " points!");
                                                break;

                                            default:
                                                if (text[3] == "set" || text[3] == "s" || text[3] == "change")
                                                {
                                                    text[3] = text[4];
                                                }
                                                int numb = -1;
                                                if (int.TryParse(text[3], out numb) == false)
                                                { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[3] + "'"); return; }
                                                if (numb <= -1) { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[3] + "'"); return; }
                                                else
                                                {
                                                    it.ScorePerKill = numb;
                                                    Player.SendMessage(p, "TNT Wars: Score per kill is now " + numb.ToString() + " points!");
                                                    return;
                                                }
                                                //break;
                                        }
                                        it.CheckAllSetUp(p);
                                        break;

                                    case "assistkill":
                                    case "assist":
                                    case "assists":
                                    case "assistscore":
                                    case "a":
                                        switch (text[3])
                                        {
                                            case "on":
                                            case "enable":
                                                if (it.ScorePerAssist > 0)
                                                {
                                                    Player.SendMessage(p, "TNT Wars Error: Assist bonuses are already enabled (at " + it.ScorePerAssist.ToString() + " points!");
                                                    return;
                                                }
                                                else
                                                {
                                                    it.ScorePerAssist = TntWarsGame.Properties.DefaultAssistScore;
                                                    Player.SendMessage(p, "TNT Wars: Assist bonuses are now enabled at " + it.ScorePerAssist.ToString() + " points!");
                                                }
                                                break;

                                            case "off":
                                            case "disable":
                                                if (it.ScorePerAssist == 0)
                                                {
                                                    Player.SendMessage(p, "TNT Wars Error: Assist bonuses are already disabled!");
                                                    return;
                                                }
                                                else
                                                {
                                                    it.ScorePerAssist = 0;
                                                    Player.SendMessage(p, "TNT Wars: Assist bonuses are now disabled!");
                                                }
                                                break;

                                            case "switch":
                                                if (it.ScorePerAssist == 0)
                                                {
                                                    it.ScorePerAssist = TntWarsGame.Properties.DefaultAssistScore;
                                                    Player.SendMessage(p, "TNT Wars: Assist bonuses are now enabled at " + it.ScorePerAssist.ToString() + " points!");
                                                    return;
                                                }
                                                else
                                                {
                                                    it.ScorePerAssist = 0;
                                                    Player.SendMessage(p, "TNT Wars: Assist bonuses are now disabled!");
                                                }
                                                break;

                                            case "check":
                                            case "current":
                                            case "now":
                                            case "ATM":
                                            case "c":
                                            case "t":
                                                Player.SendMessage(p, "TNT Wars: Score per assist is " + it.ScorePerAssist.ToString() + " points!");
                                                break;

                                            default:
                                                if (text[3] == "set" || text[3] == "s" || text[3] == "change")
                                                {
                                                    text[3] = text[4];
                                                }
                                                int numb = -1;
                                                if (int.TryParse(text[3], out numb) == false)
                                                { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[3] + "'"); return; }
                                                if (numb <= -1) { Player.SendMessage(p, "TNT Wars Error: Invalid number '" + text[3] + "'"); return; }
                                                else
                                                {
                                                    it.ScorePerAssist = numb;
                                                    Player.SendMessage(p, "TNT Wars: Score per assist is now " + numb.ToString() + " points!");
                                                    return;
                                                }
                                                //break;
                                        }
                                        it.CheckAllSetUp(p);
                                        break;

                                    case "help":
                                    case "h":
                                    default:
                                        Player.SendMessage(p, "TNT Wars Setup Scoring Help:");
                                        Player.SendMessage(p, "/tw s score maximum {m} [check/set] <value> - set the score limit (or check it)");
                                        Player.SendMessage(p, "/tw s score streaks {s} [on/off/check] - enable/disable streaks (or check it)");
                                        Player.SendMessage(p, "/tw s score multi {mk} [on/off/switch/check/set] - enable/disable/switch multikills or set the score bonus per multikill (or check it)");
                                        Player.SendMessage(p, "/tw s score scorekill {k} [check/set] <value> - set the score per kill (or check it)");
                                        Player.SendMessage(p, "/tw s score assistkill {a} [check/set] <value> - set the score per assist (or check it)");
                                        break;
                                }
                                break;

                            case "balance":
                            case "balanceteams":
                            case "bt":
                            case "b":
                                switch (text[2])
                                {
                                    case "on":
                                    case "enable":
                                        if (it.BalanceTeams)
                                        {
                                            Player.SendMessage(p, "TNT Wars Error: Team balancing is already enabled");
                                            return;
                                        }
                                        else
                                        {
                                            it.BalanceTeams = true;
                                            Player.SendMessage(p, "TNT Wars: Team balancing is now enabled");
                                        }
                                        break;

                                    case "off":
                                    case "disable":
                                        if (it.BalanceTeams == false)
                                        {
                                            Player.SendMessage(p, "TNT Wars Error: Team balancing is already disabled");
                                            return;
                                        }
                                        else
                                        {
                                            it.BalanceTeams = false;
                                            Player.SendMessage(p, "TNT Wars: Team balancing is now disabled");
                                        }
                                        break;

                                    case "check":
                                    case "current":
                                    case "now":
                                    case "ATM":
                                    case "c":
                                    case "t":
                                        if (it.BalanceTeams) { Player.SendMessage(p, "TNT Wars: Team balancing is currently enabled"); return; }
                                        else if (it.BalanceTeams == false) { Player.SendMessage(p, "TNT Wars: Team balancing is currently disabled"); return; }
                                        break;

                                    default:
                                        if (it.BalanceTeams == false)
                                        {
                                            it.BalanceTeams = true;
                                            Player.SendMessage(p, "TNT Wars: Team balancing is now enabled");
                                            return;
                                        }
                                        else if (it.BalanceTeams)
                                        {
                                            it.BalanceTeams = false;
                                            Player.SendMessage(p, "TNT Wars: Team balancing is now disabled");
                                            return;
                                        }
                                        break;
                                }
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
                                switch (text[2])
                                {
                                    case "on":
                                    case "enable":
                                        if (it.TeamKills)
                                        {
                                            Player.SendMessage(p, "TNT Wars Error: Team killing is already enabled");
                                            return;
                                        }
                                        else
                                        {
                                            it.TeamKills = true;
                                            Player.SendMessage(p, "TNT Wars: Team killing is now enabled");
                                        }
                                        break;

                                    case "off":
                                    case "disable":
                                        if (it.TeamKills == false)
                                        {
                                            Player.SendMessage(p, "TNT Wars Error: Team killing is already disabled");
                                            return;
                                        }
                                        else
                                        {
                                            it.TeamKills = false;
                                            Player.SendMessage(p, "TNT Wars: Team killing is now disabled");
                                        }
                                        break;

                                    case "check":
                                    case "current":
                                    case "now":
                                    case "ATM":
                                    case "c":
                                    case "t":
                                        if (it.TeamKills) { Player.SendMessage(p, "TNT Wars: Team killing is currently enabled"); return; }
                                        else if (it.TeamKills == false) { Player.SendMessage(p, "TNT Wars: Team killing is currently disabled"); return; }
                                        break;

                                    default:
                                        if (it.TeamKills == false)
                                        {
                                            it.TeamKills = true;
                                            Player.SendMessage(p, "TNT Wars: Team killing is now enabled");
                                            return;
                                        }
                                        else if (it.TeamKills)
                                        {
                                            it.TeamKills = false;
                                            Player.SendMessage(p, "TNT Wars: Team killing is now disabled");
                                            return;
                                        }
                                        break;
                                }
                                it.CheckAllSetUp(p);
                                break;

                            case "zone":
                            case "zones":
                            case "z":
                            case "zn":
                            case "zns":
                            case "zs":
                                CatchPos cpos;
                                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
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
                                        NoTntZone = true;
                                        DeleteZone = false;
                                        CheckZone = false;
                                        switch (text[3])
                                        {
                                            case "add":
                                            case "a":
                                            case "new":
                                                DeleteZone = false;
                                                CheckZone = false;
                                                Player.SendMessage(p, "TNT Wars: Place 2 blocks to create the zone for no TNT!");
                                                //p.ClearBlockchange();
                                                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                                                break;

                                            case "delete":
                                            case "d":
                                            case "remove":
                                            case "r":
                                                if (text[4] == "all" || text[4] == "a")
                                                {
                                                    it.NoTNTplacableZones.Clear();
                                                    Player.SendMessage(p, "TNT Wars: Deleted all zones for no blocks deleted on explosions!");
                                                    return;
                                                }
                                                DeleteZone = true;
                                                CheckZone = false;
                                                Player.SendMessage(p, "TNT Wars: Place a block to delete the zone for no TNT!");
                                                p.ClearBlockchange();
                                                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                                                break;

                                            case "check":
                                            case "c":
                                                DeleteZone = false;
                                                CheckZone = true;
                                                Player.SendMessage(p, "TNT Wars: Place a block to check for no TNT zones!");
                                                p.ClearBlockchange();
                                                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                                                break;
                                        }
                                        break;

                                    case "noexplosion":
                                    case "nodeleteblocks":
                                    case "deleteblocks":
                                    case "nd":
                                    case "nb":
                                    case "ne":
                                    case "neb":
                                    case "ndb":
                                        NoTntZone = false;
                                        DeleteZone = false;
                                        CheckZone = false;
                                        switch (text[3])
                                        {
                                            case "add":
                                            case "a":
                                            case "new":
                                                DeleteZone = false;
                                                CheckZone = false;
                                                Player.SendMessage(p, "TNT Wars: Place 2 blocks to create the zone for no blocks deleted on explosions!");
                                                p.ClearBlockchange();
                                                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                                                break;

                                            case "delete":
                                            case "d":
                                            case "remove":
                                            case "r":
                                                if (text[4] == "all" || text[4] == "a")
                                                {
                                                    it.NoBlockDeathZones.Clear();
                                                    Player.SendMessage(p, "TNT Wars: Deleted all zones for no blocks deleted on explosions!");
                                                    return;
                                                }
                                                DeleteZone = true;
                                                CheckZone = false;
                                                Player.SendMessage(p, "TNT Wars: Place a block to delete the zone for no blocks deleted on explosions!");
                                                p.ClearBlockchange();
                                                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                                                break;

                                            case "check":
                                            case "c":
                                                DeleteZone = false;
                                                CheckZone = true;
                                                Player.SendMessage(p, "TNT Wars: Place a block to check for no blocks deleted on explosions!");
                                                p.ClearBlockchange();
                                                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                                                break;
                                        }
                                        break;
                                }
                                break;

                            case "help":
                            case "h":
                                int SleepAmount = 500;
                                Player.SendMessage(p, "TNT Wars Setup Help:");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s new {n}/delete - create/delete a game");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s start/stop/reset {r} - start/stop/reset the current game");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s level {l} - change the level for the game");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s teamsspawns {ts} [red/blue] - set the spawns for red/blue");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s tnt {t} - change the amount of tnt per player at a time");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s graceperiod {g} [on/off/check] - enable/disable the grace period (or check it)");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s gracetime {gt} [set/check] <amount> - set the grace period time (in seconds) (or check it)");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s gamemode {m} [check/tdm/ffa] - change the gamemode to FFA or TDM (or check it)");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s difficulty {d} [1/2/3/4] - change the difficulty (easy/normal/hard/extreme)");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s balanceteams {b} [on/off/check] - enable/disable balancing teams (or check it)");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s teamkill {tk} [on/off/check] - enable/disable teamkills (or check it)");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s zone {z} [notnt {nt}/noexplodeblocks {neb}] [add {a}/delete {d} <all>/check {c}]- create zones (No TNT zones or zones where explosions do not delete blocks");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s score - scoring setup (use '/tntwars setup scoring help' for more info)");
                                Thread.Sleep(SleepAmount);
                                Player.SendMessage(p, "/tw s status {s} - view the status of setup");
                                break;

                            default:
                            case "status":
                            case "s":
                            case "ready":
                            case "check":
                            case "info":
                            case "c":
                                Player.SendMessage(p, "TNT Wars: Current Setup:");
                                //1
                                if (it.lvl == null) { Player.SendMessage(p, "Level: " + Colors.red + "NONE"); }
                                else { Player.SendMessage(p, "Level: " + Colors.green + it.lvl.name); }
                                //2
                                if (it.GameMode == TntWarsGame.TntWarsGameMode.FFA) { Player.SendMessage(p, "Gamemode: " + Colors.green + "FFA"); }
                                if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM) { Player.SendMessage(p, "Gamemode: " + Colors.green + "TDM"); }
                                //3
                                if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Easy) { Player.SendMessage(p, "Game difficulty: " + Colors.green + "Easy"); }
                                if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Normal) { Player.SendMessage(p, "Game difficulty: " + Colors.green + "Normal"); }
                                if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Hard) { Player.SendMessage(p, "Game difficulty: " + Colors.green + "Hard"); }
                                if (it.GameDifficulty == TntWarsGame.TntWarsDifficulty.Extreme) { Player.SendMessage(p, "Game difficulty: " + Colors.green + "Extreme"); }
                                //4
                                if (it.TntPerPlayerAtATime >= 1) { Player.SendMessage(p, "TNT per player at a time: " + Colors.green + it.TntPerPlayerAtATime.ToString()); }
                                else if (it.TntPerPlayerAtATime == 0) { Player.SendMessage(p, "TNT per player at a time: " + Colors.green + "unlimited"); }
                                //5
                                if (it.GracePeriod) { Player.SendMessage(p, "Grace period: " + Colors.green + "enabled"); }
                                if (!it.GracePeriod) { Player.SendMessage(p, "Grace period: " + Colors.green + "disabled"); }
                                //6
                                Player.SendMessage(p, "Grace period time: " + Colors.green + it.GracePeriodSecs.ToString() + " seconds");
                                //7
                                if (it.BalanceTeams) { Player.SendMessage(p, "Balance teams: " + Colors.green + "enabled"); }
                                if (!it.BalanceTeams) { Player.SendMessage(p, "Balance teams: " + Colors.green + "disabled"); }
                                //8
                                Player.SendMessage(p, "Score limit: " + Colors.green + it.ScoreLimit.ToString() + " points");
                                //9
                                if (it.Streaks) { Player.SendMessage(p, "Streaks: " + Colors.green + "enabled"); }
                                if (!it.Streaks) { Player.SendMessage(p, "Streaks: " + Colors.green + "disabled"); }
                                //10
                                if (it.MultiKillBonus == 0) { Player.SendMessage(p, "Multikill bonus: " + Colors.green + "disabled"); }
                                if (it.MultiKillBonus != 0) { Player.SendMessage(p, "Multikill bonus: " + Colors.green + "enabled"); }
                                //11
                                Player.SendMessage(p, "Score per kill: " + Colors.green + it.ScorePerKill.ToString() + " points");
                                //12
                                if (it.ScorePerAssist == 0) { Player.SendMessage(p, "Assists: " + Colors.green + "disabled"); }
                                if (it.ScorePerAssist != 0) { Player.SendMessage(p, "Assists : " + Colors.green + "enabled (at " + it.ScorePerAssist.ToString() + " points)"); }
                                //13
                                if (it.TeamKills) { Player.SendMessage(p, "Team killing: " + Colors.green + "enabled"); }
                                if (!it.TeamKills) { Player.SendMessage(p, "Team killing: " + Colors.green + "disabled"); }
                                //14
                                it.CheckAllSetUp(p);
                                //15
                                break;
                        }
					}
					else
					{
						Player.SendMessage(p, "Sorry, you aren't a high enough rank for that!");
					}
					break;

				default:
					Help(p);
					return;
			}
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "TNT Wars Help:");
			Player.SendMessage(p, "/tw list {l} - Lists all the current games");
			Player.SendMessage(p, "/tw join <team/level> - join a game on <level> or on <team>(red/blue)");
			Player.SendMessage(p, "/tw leave - leave the current game");
			Player.SendMessage(p, "/tw scores <top/team/me> - view the top score/team scores/your scores");
			Player.SendMessage(p, "/tw players {p} - view the current players in your game");
			Player.SendMessage(p, "/tw health {hp} - view your currrent amount of health left");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
            {
                Player.SendMessage(p, "/tw rules <all/level/players/<playername>> - send the rules to yourself, all, your map, all players in your game or to one person!");
                Player.SendMessage(p, "/tw setup {s} - setup the game (do '/tntwars setup help' for more info!");
            }
            else
            {
                Player.SendMessage(p, "/tw rules - read the rules");
            }
		}
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            if (DeleteZone == false && CheckZone == false)
            {
                Player.SendMessage(p, "TNT Wars: Place another block to mark the other corner of the zone!");
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
                return;
            }
            TntWarsGame it = TntWarsGame.GetTntWarsGame(p);
            if (DeleteZone)
            {
                if (it == null)
                {
                    Player.SendMessage(p, "TNT Wars Error: Couldn't find your game!");
                }
                else
                {
                    if (it.InZone(x, y, z, NoTntZone))
                    {
                        it.DeleteZone(x, y, z, NoTntZone, p);
                    }
                }
            }
            else if (CheckZone && NoTntZone)
            {
                if (it == null)
                {
                    Player.SendMessage(p, "TNT Wars Error: Couldn't find your game!");
                    return;
                }
                else
                {
                    if (it.InZone(x, y, z, true))
                    {
                        Player.SendMessage(p, "TNT Wars: You are currently in a no TNT zone!");
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "TNT Wars: You are not currently in a no TNT zone!");
                        return;
                    }
                }
            }
            else if (CheckZone && NoTntZone == false)
            {
                if (it == null)
                {
                    Player.SendMessage(p, "TNT Wars Error: Couldn't find your game!");
                    return;
                }
                else
                {
                    if (it.InZone(x, y, z, true))
                    {
                        Player.SendMessage(p, "TNT Wars: You are currently in a no TNT block explosion zone (explosions won't destory blocks)!");
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "TNT Wars: You are currently in a TNT block explosion zone (explosions will destory blocks)!");
                        return;
                    }
                }
            }
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            TntWarsGame.Zone Zn = new TntWarsGame.Zone();

            Zn.smallX = Math.Min(cpos.x, x);
            Zn.smallY = Math.Min(cpos.y, y);
            Zn.smallZ = Math.Min(cpos.z, z);
            Zn.bigX = Math.Max(cpos.x, x);
            Zn.bigY = Math.Max(cpos.y, y);
            Zn.bigZ = Math.Max(cpos.z, z);

            TntWarsGame it = TntWarsGame.GetTntWarsGame(p);
            if (it == null)
            {
                Player.SendMessage(p, "TNT Wars Error: Couldn't find your game!");
                return;
            }
            if (NoTntZone) it.NoTNTplacableZones.Add(Zn);
            else it.NoBlockDeathZones.Add(Zn);
            Player.SendMessage(p, "Added zone");
            return;
        }

        struct CatchPos { public ushort x, y, z; }
    }
}
