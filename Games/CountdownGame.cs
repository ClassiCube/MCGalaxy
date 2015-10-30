/*
	Copyright 2011 MCGalaxy
		
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace MCGalaxy
{
    public sealed class CountdownGame
    {
        public static List<Player> players = new List<Player>();
        public static List<Player> playersleftlist = new List<Player>();
        public static List<string> squaresleft = new List<string>();
        public static Level mapon;

        public static int playersleft;
        public static int speed;

        public static bool freezemode = false;
        public static bool cancel = false;

        public static string speedtype;

        public static CountdownGameStatus gamestatus = CountdownGameStatus.Disabled;

        //private static ushort[] x; // this is useless?

        public static void GameStart(Player p)
        {
            switch (gamestatus)
            {
                case CountdownGameStatus.Disabled:
                    Player.SendMessage(p, "Please enable Countdown first!!");
                    return;

                case CountdownGameStatus.AboutToStart:
                    Player.SendMessage(p, "Game is about to start");
                    return;

                case CountdownGameStatus.InProgress:
                    Player.SendMessage(p, "Game is already in progress");
                    return;

                case CountdownGameStatus.Finished:
                    Player.SendMessage(p, "Game has finished");
                    return;

                case CountdownGameStatus.Enabled:
                    gamestatus = CountdownGameStatus.AboutToStart;
                    Thread.Sleep(2000);
                    break;
            }
            {
                {
                    mapon.Blockchange(15, 27, 16, Block.glass);
                    mapon.Blockchange(16, 27, 15, Block.glass);
                    mapon.Blockchange(16, 27, 16, Block.glass);
                    mapon.Blockchange(15, 27, 15, Block.glass);
                }
                {
                    {
                        mapon.Blockchange(15, 18, 14, Block.glass);
                        mapon.Blockchange(16, 18, 14, Block.glass);
                        mapon.Blockchange(15, 17, 14, Block.glass);
                        mapon.Blockchange(16, 17, 14, Block.glass);
                    }
                    {
                        mapon.Blockchange(14, 17, 15, Block.glass);
                        mapon.Blockchange(14, 18, 16, Block.glass);
                        mapon.Blockchange(14, 17, 16, Block.glass);
                        mapon.Blockchange(14, 18, 15, Block.glass);
                    }
                    {
                        mapon.Blockchange(15, 17, 17, Block.glass);
                        mapon.Blockchange(16, 18, 17, Block.glass);
                        mapon.Blockchange(15, 18, 17, Block.glass);
                        mapon.Blockchange(16, 17, 17, Block.glass);
                    }
                    {
                        mapon.Blockchange(17, 17, 16, Block.glass);
                        mapon.Blockchange(17, 18, 15, Block.glass);
                        mapon.Blockchange(17, 18, 16, Block.glass);
                        mapon.Blockchange(17, 17, 15, Block.glass);
                    }
                }
                {
                    mapon.Blockchange(16, 16, 15, Block.glass);
                    mapon.Blockchange(15, 16, 16, Block.glass);
                    mapon.Blockchange(15, 16, 15, Block.glass);
                    mapon.Blockchange(16, 16, 16, Block.glass);
                }
            }
            mapon.ChatLevel("Countdown is about to start!!");
            mapon.permissionbuild = LevelPermission.Nobody;
            ushort x1 = (ushort)((15.5) * 32);
            ushort y1 = (ushort)((30) * 32);
            ushort z1 = (ushort)((15.5) * 32);
            foreach (Player player in players)
            {
                if (player.level != mapon)
                {
                    player.SendMessage("Sending you to the correct map.");
                    Command.all.Find("goto").Use(player, mapon.name);
                    Thread.Sleep(1000);
                    // Sleep for a bit while they load
                    while (player.Loading) { Thread.Sleep(250); }
                }
                unchecked
                {
                    player.SendSpawn((byte)-1, player.name, x1, y1, z1, (byte)0, (byte)0);
                }
            }
            {
                CountdownGame.squaresleft.Clear();
                PopulateSquaresLeft();
                if (freezemode)
                {
                    mapon.ChatLevel("Countdown starting with difficulty " + speedtype + " and mode freeze in:");
                }
                else
                {
                    mapon.ChatLevel("Countdown starting with difficulty " + speedtype + " and mode normal in:");
                }
                Thread.Sleep(2000);
                mapon.ChatLevel("-----&b5" + Server.DefaultColor + "-----");
                {
                    mapon.Blockchange(16, 16, 15, Block.air);
                    mapon.Blockchange(15, 16, 16, Block.air);
                    mapon.Blockchange(15, 16, 15, Block.air);
                    mapon.Blockchange(16, 16, 16, Block.air);
                }
                Thread.Sleep(1000);
                mapon.ChatLevel("-----&b4" + Server.DefaultColor + "-----");
                Thread.Sleep(1000);
                mapon.ChatLevel("-----&b3" + Server.DefaultColor + "-----");
                Thread.Sleep(1000);
                {
                    mapon.Blockchange(15, 27, 16, Block.air);
                    mapon.Blockchange(16, 27, 15, Block.air);
                    mapon.Blockchange(16, 27, 16, Block.air);
                    mapon.Blockchange(15, 27, 15, Block.air);
                }
                mapon.ChatLevel("-----&b2" + Server.DefaultColor + "-----");
                Thread.Sleep(1000);
                mapon.ChatLevel("-----&b1" + Server.DefaultColor + "-----");
                Thread.Sleep(1000);
                mapon.ChatLevel("GO!!!!!!!");
            }
            {
                playersleft = players.Count();
                playersleftlist = players;
                foreach (Player plya in players)
                {
                    plya.incountdown = true;
                }
            }
            AfterStart();
            Play();
        }

        public static void Play()
        {
            if (freezemode == false)
            {
                while (squaresleft.Any() && playersleft != 0 && (gamestatus == CountdownGameStatus.InProgress || gamestatus == CountdownGameStatus.Finished))
                {
                    Random number = new Random();
                    int randnum = number.Next(squaresleft.Count);
                    string nextsquare = squaresleft.ElementAt(randnum);
                    squaresleft.Remove(nextsquare);
                    RemoveSquare(nextsquare);
                    if (squaresleft.Count % 10 == 0 && gamestatus != CountdownGameStatus.Finished)
                    {
                        mapon.ChatLevel(squaresleft.Count + " Squares Left and " + playersleft.ToString() + " Players left!!");
                    }
                    if (cancel)
                    {
                        End(null);
                    }
                }
                return;
            }
            else
            {
                {//Find yo places stuff (15 seconds)
                    Thread.Sleep(500);
                    MessagePlayers("Welcome to Freeze Mode of countdown");
                    MessagePlayers("You have 15 seconds to stand on a square");
                    Thread.Sleep(500);
                    MessagePlayers("-----&b15" + Server.DefaultColor + "-----");
                    Thread.Sleep(500);
                    MessagePlayers("Once the countdown is up, you are stuck on your square");
                    Thread.Sleep(500);
                    MessagePlayers("-----&b14" + Server.DefaultColor + "-----");
                    Thread.Sleep(500);
                    MessagePlayers("The squares then start to dissapear");
                    Thread.Sleep(500);
                    MessagePlayers("-----&b13" + Server.DefaultColor + "-----");
                    Thread.Sleep(500);
                    MessagePlayers("Whoever is last out wins!!");
                    Thread.Sleep(500);
                    MessagePlayers("-----&b12" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b11" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b10" + Server.DefaultColor + "-----");
                    MessagePlayers("Only 10 Seconds left to pick your places!!");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b9" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b8" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b7" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b6" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b5" + Server.DefaultColor + "-----");
                    MessagePlayers("5 Seconds left to pick your places!!");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b4" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b3" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b2" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("-----&b1" + Server.DefaultColor + "-----");
                    Thread.Sleep(1000);
                    MessagePlayers("&bPlayers Frozen");
                    {
                        mapon.countdowninprogress = true;
                        gamestatus = CountdownGameStatus.InProgress;
                        foreach (Player pl in players)
                        {
                            pl.countdownsettemps = true;
                            Thread.Sleep(100);
                        }
                    }
                    {//Get rid of glass
                        ushort x3 = 5;
                        while (x3 <= 26)
                        {
                            ushort z4 = 26;
                            while (z4 >= 4)
                            {
                                mapon.Blockchange(x3, 4, z4, Block.air);
                                z4 = (ushort)(z4 - 1);
                            }
                            x3 = (ushort)(x3 + 3);
                        }
                        ushort z3 = 5;
                        while (z3 <= 26)
                        {
                            ushort x4 = 4;
                            while (x4 <= 26)
                            {
                                mapon.Blockchange(x4, 4, z3, Block.air);
                                x4++;
                            }
                            z3 = (ushort)(z3 + 3);
                        }
                    }
                    while (squaresleft.Any() && playersleft != 0 && (gamestatus == CountdownGameStatus.InProgress || gamestatus == CountdownGameStatus.Finished))
                    {
                        Random number = new Random();
                        int randnum = number.Next(squaresleft.Count);
                        string nextsquare = squaresleft.ElementAt(randnum);
                        squaresleft.Remove(nextsquare);
                        RemoveSquare(nextsquare);
                        if (squaresleft.Count % 10 == 0 && gamestatus != CountdownGameStatus.Finished)
                        {
                            mapon.ChatLevel(squaresleft.Count + " Squares Left and " + playersleft.ToString() + " Players left!!");
                        }
                        if (cancel)
                        {
                            End(null);
                        }
                    }
                    return;
                }
            }
        }

        public static void RemoveSquare(string square)
        {
            int column = int.Parse(square.Split(':')[0]);
            int row = int.Parse(square.Split(':')[1]);
            ushort x1 = (ushort)(27 - (row * 3));
            ushort x2 = (ushort)(28 - (row * 3));
            ushort y = 4;
            ushort z1 = (ushort)(27 - (column * 3));
            ushort z2 = (ushort)(28 - (column * 3));
            {
                { //3
                    mapon.Blockchange(x1, y, z1, Block.yellow);
                    mapon.Blockchange(x2, y, z1, Block.yellow);
                    mapon.Blockchange(x2, y, z2, Block.yellow);
                    mapon.Blockchange(x1, y, z2, Block.yellow);
                }
                Thread.Sleep(speed);
                { //2
                    mapon.Blockchange(x1, y, z1, Block.orange);
                    mapon.Blockchange(x2, y, z1, Block.orange);
                    mapon.Blockchange(x2, y, z2, Block.orange);
                    mapon.Blockchange(x1, y, z2, Block.orange);
                }
                Thread.Sleep(speed);
                { //1
                    mapon.Blockchange(x1, y, z1, Block.red);
                    mapon.Blockchange(x2, y, z1, Block.red);
                    mapon.Blockchange(x2, y, z2, Block.red);
                    mapon.Blockchange(x1, y, z2, Block.red);
                }
                Thread.Sleep(speed);
                { //poof
                    mapon.Blockchange(x1, y, z1, Block.air);
                    mapon.Blockchange(x2, y, z1, Block.air);
                    mapon.Blockchange(x2, y, z2, Block.air);
                    mapon.Blockchange(x1, y, z2, Block.air);
                    { //beneath this is checking the glass next to the square
                        bool up = false;
                        bool left = false;
                        bool right = false;
                        bool down = false;
                        {//directly next to
                            if (mapon.GetTile(x1, y, (ushort)(z2 + 2)) == Block.air) //right
                            {
                                mapon.Blockchange(x1, y, (ushort)(z2 + 1), Block.air);
                                mapon.Blockchange(x2, y, (ushort)(z2 + 1), Block.air);
                                right = true;
                            }
                            if (mapon.GetTile(x1, y, (ushort)(z1 - 2)) == Block.air) //left
                            {
                                mapon.Blockchange(x1, y, (ushort)(z1 - 1), Block.air);
                                mapon.Blockchange(x2, y, (ushort)(z1 - 1), Block.air);
                                left = true;
                            }
                            if (mapon.GetTile((ushort)(x2 + 2), y, z1) == Block.air) //up
                            {
                                mapon.Blockchange((ushort)(x2 + 1), y, z1, Block.air);
                                mapon.Blockchange((ushort)(x2 + 1), y, z2, Block.air);
                                up = true;
                            }
                            if (mapon.GetTile((ushort)(x1 - 2), y, z1) == Block.air) //down
                            {
                                mapon.Blockchange((ushort)(x1 - 1), y, z1, Block.air);
                                mapon.Blockchange((ushort)(x1 - 1), y, z2, Block.air);
                                down = true;
                            }
                        }
                        {//diagonal >:(
                            if ((mapon.GetTile((ushort)(x1 - 2), y, (ushort)(z1 - 2)) == Block.air) && left && down) //bottom left
                            {
                                mapon.Blockchange((ushort)(x1 - 1), y, (ushort)(z1 - 1), Block.air);
                            }
                            if ((mapon.GetTile((ushort)(x1 - 2), y, (ushort)(z2 + 2)) == Block.air) && right && down) //bottom right
                            {
                                mapon.Blockchange((ushort)(x1 - 1), y, (ushort)(z2 + 1), Block.air);
                            }
                            if ((mapon.GetTile((ushort)(x2 + 2), y, (ushort)(z1 - 2)) == Block.air) && left && up) //top left
                            {
                                mapon.Blockchange((ushort)(x2 + 1), y, (ushort)(z1 - 1), Block.air);
                            }
                            if ((mapon.GetTile((ushort)(x2 + 2), y, (ushort)(z2 + 2)) == Block.air) && right && up) //top right
                            {
                                mapon.Blockchange((ushort)(x2 + 1), y, (ushort)(z2 + 1), Block.air);
                            }
                        }
                    }
                }
            }
        }

        public static void PopulateSquaresLeft()
        {
            int column = 1;
            int row = 1;
            while (column <= 7)
            {
                row = 1;
                while (row <= 7)
                {
                    squaresleft.Add(column.ToString() + ":" + row.ToString());
                    row = row + 1;
                }
                column = column + 1;
            }
        }

        public static void AfterStart()
        {
            {
                {
                    {
                        mapon.Blockchange(15, 18, 14, Block.air);
                        mapon.Blockchange(16, 18, 14, Block.air);
                        mapon.Blockchange(15, 17, 14, Block.air);
                        mapon.Blockchange(16, 17, 14, Block.air);
                    }
                    {
                        mapon.Blockchange(14, 17, 15, Block.air);
                        mapon.Blockchange(14, 18, 16, Block.air);
                        mapon.Blockchange(14, 17, 16, Block.air);
                        mapon.Blockchange(14, 18, 15, Block.air);
                    }
                    {
                        mapon.Blockchange(15, 17, 17, Block.air);
                        mapon.Blockchange(16, 18, 17, Block.air);
                        mapon.Blockchange(15, 18, 17, Block.air);
                        mapon.Blockchange(16, 17, 17, Block.air);
                    }
                    {
                        mapon.Blockchange(17, 17, 16, Block.air);
                        mapon.Blockchange(17, 18, 15, Block.air);
                        mapon.Blockchange(17, 18, 16, Block.air);
                        mapon.Blockchange(17, 17, 15, Block.air);
                    }
                }
                {
                    mapon.Blockchange(16, 16, 15, Block.glass);
                    mapon.Blockchange(15, 16, 16, Block.glass);
                    mapon.Blockchange(15, 16, 15, Block.glass);
                    mapon.Blockchange(16, 16, 16, Block.glass);
                }
                {
                    ushort x1 = 27;
                    while (x1 >= 4)
                    {
                        mapon.Blockchange(x1, 4, 4, Block.air);
                        x1 = (ushort)(x1 - 1);
                    }
                    ushort x2 = 4;
                    while (x2 <= 27)
                    {
                        mapon.Blockchange(x2, 4, 27, Block.air);
                        x2++;
                    }
                    ushort z1 = 27;
                    while (z1 >= 4)
                    {
                        mapon.Blockchange(4, 4, z1, Block.air);
                        z1 = (ushort)(z1 - 1);
                    }
                    ushort z2 = 4;
                    while (z2 <= 27)
                    {
                        mapon.Blockchange(27, 4, z2, Block.air);
                        z2++;
                    }
                }
            }
            
            if (freezemode == false)
            {
                mapon.countdowninprogress = true;
                gamestatus = CountdownGameStatus.InProgress;
            }
        }

        public static void Death(Player p)
        {
            playersleft = playersleft - 1;

            mapon.ChatLevel(p.color + p.name + Server.DefaultColor + " is out of countdown!!");
            p.incountdown = false;
            playersleftlist.Remove(p);
            switch (playersleft)
            {
                case 1:        
                    mapon.ChatLevel(playersleftlist.Last().color + playersleftlist.Last().name + Server.DefaultColor + " is the winner!!");
                    End(playersleftlist.Last());
                    break;
                case 2:
                    mapon.ChatLevel("Only 2 Players left:");
                    mapon.ChatLevel(playersleftlist.First().color + playersleftlist.First().name + Server.DefaultColor + " and " + playersleftlist.Last().color + playersleftlist.Last().name);
                    break;
                case 5:
                    mapon.ChatLevel("Only 5 Players left:");
                    foreach (Player pl in playersleftlist)
                    {
                        mapon.ChatLevel(pl.color + pl.name);
                        Thread.Sleep(500);
                    }
                    break;
                default:
                    mapon.ChatLevel("Now there are " + playersleft.ToString() + " players left!!");
                    break;
            }
        }

        public static void PlayerLeft(Player p)
        {
            playersleft = playersleft - 1;

            mapon.ChatLevel(p.color + p.name + Server.DefaultColor + " logged out and so is out of countdown!!");
            players.Remove(p);
            p.incountdown = false;
            playersleftlist.Remove(p);
            switch (playersleft)
            {
                case 1:
                    mapon.ChatLevel(playersleftlist.Last().color + playersleftlist.Last().name + Server.DefaultColor + " is the winner!!");
                    End(playersleftlist.Last());
                    break;
                case 2:
                    mapon.ChatLevel("Only 2 Players left:");
                    mapon.ChatLevel(playersleftlist.First().color + playersleftlist.First().name + Server.DefaultColor + " and " + playersleftlist.Last().color + playersleftlist.Last().name);
                    break;
                case 5:
                    mapon.ChatLevel("Only 5 Players left:");
                    foreach (Player pl in playersleftlist)
                    {
                        mapon.ChatLevel(pl.color + pl.name);
                        Thread.Sleep(500);
                    }
                    break;
                default:
                    mapon.ChatLevel("Now there are " + playersleft.ToString() + " players left!!");
                    break;
            }
        }

        public static void End(Player winner)
        {
            CountdownGame.squaresleft.Clear();
            if (winner != null)
            {
                winner.SendMessage("Congratulations!! You won!!!");
            }
            gamestatus = CountdownGameStatus.Finished;
            if (winner != null)
            {
                Command.all.Find("spawn").Use(winner, "");
            }
            CountdownGame.playersleftlist.Clear();
            if (winner != null)
            {
                winner.incountdown = false;
            }
            if (winner == null)
            {
                foreach (Player pl in CountdownGame.players)
                {
                    Player.SendMessage(pl, "The countdown game was canceled!");
                    Command.all.Find("spawn").Use(pl, "");
                }
                Player.GlobalMessage("The countdown game was canceled!!");
                CountdownGame.gamestatus = CountdownGameStatus.Enabled;
                CountdownGame.playersleft = 0;
                CountdownGame.playersleftlist.Clear();
                CountdownGame.players.Clear();
                CountdownGame.squaresleft.Clear();
                CountdownGame.Reset(null, true);
                CountdownGame.cancel = false;
                return;
            }
        }

        public static void Reset(Player p, bool all)
        {
            if (gamestatus == CountdownGameStatus.Enabled || gamestatus == CountdownGameStatus.Finished || gamestatus == CountdownGameStatus.Disabled)
            {
                {
                    if (all)
                    {
                        { //clean variables
                            CountdownGame.gamestatus = CountdownGameStatus.Disabled;
                            CountdownGame.playersleft = 0;
                            CountdownGame.playersleftlist.Clear();
                            CountdownGame.squaresleft.Clear();
                            CountdownGame.speed = 750;
                        }
                    }
                    { //top part of map tube thingy
                        {
                            mapon.Blockchange(15, 18, 14, Block.air);
                            mapon.Blockchange(16, 18, 14, Block.air);
                            mapon.Blockchange(15, 17, 14, Block.air);
                            mapon.Blockchange(16, 17, 14, Block.air);
                        }
                        {
                            mapon.Blockchange(14, 17, 15, Block.air);
                            mapon.Blockchange(14, 18, 16, Block.air);
                            mapon.Blockchange(14, 17, 16, Block.air);
                            mapon.Blockchange(14, 18, 15, Block.air);
                        }
                        {
                            mapon.Blockchange(15, 17, 17, Block.air);
                            mapon.Blockchange(16, 18, 17, Block.air);
                            mapon.Blockchange(15, 18, 17, Block.air);
                            mapon.Blockchange(16, 17, 17, Block.air);
                        }
                        {
                            mapon.Blockchange(17, 17, 16, Block.air);
                            mapon.Blockchange(17, 18, 15, Block.air);
                            mapon.Blockchange(17, 18, 16, Block.air);
                            mapon.Blockchange(17, 17, 15, Block.air);
                        }
                        {
                            mapon.Blockchange(16, 16, 15, Block.glass);
                            mapon.Blockchange(15, 16, 16, Block.glass);
                            mapon.Blockchange(15, 16, 15, Block.glass);
                            mapon.Blockchange(16, 16, 16, Block.glass);
                        }
                    }
                    {
                        { //sides of map
                            ushort x1 = 27;
                            while (x1 >= 4)
                            {
                                mapon.Blockchange(x1, 4, 4, Block.glass);
                                x1 = (ushort)(x1 - 1);
                            }
                            ushort x2 = 4;
                            while (x2 <= 27)
                            {
                                mapon.Blockchange(x2, 4, 27, Block.glass);
                                x2++;
                            }
                            ushort z1 = 27;
                            while (z1 >= 4)
                            {
                                mapon.Blockchange(4, 4, z1, Block.glass);
                                z1 = (ushort)(z1 - 1);
                            }
                            ushort z2 = 4;
                            while (z2 <= 27)
                            {
                                mapon.Blockchange(27, 4, z2, Block.glass);
                                z2++;
                            }
                        }
                        { //rest of glass on map
                            ushort x3 = 5;
                            while (x3 <= 26)
                            {
                                ushort z4 = 26;
                                while (z4 >= 4)
                                {
                                    mapon.Blockchange(x3, 4, z4, Block.glass);
                                    z4 = (ushort)(z4 - 1);
                                }
                                x3 = (ushort)(x3 + 3);
                            }
                            ushort z3 = 5;
                            while (z3 <= 26)
                            {
                                ushort x4 = 4;
                                while (x4 <= 26)
                                {
                                    mapon.Blockchange(x4, 4, z3, Block.glass);
                                    x4++;
                                }
                                z3 = (ushort)(z3 + 3);
                            }
                        }
                        { //green on map
                            PopulateSquaresLeft();
                            while (squaresleft.Count > 0)
                            {
                                Random number = new Random();
                                int randnum = number.Next(squaresleft.Count);
                                string nextsquare = squaresleft.ElementAt(randnum);
                                squaresleft.Remove(nextsquare);
                                {
                                    int column = int.Parse(nextsquare.Split(':')[0]);
                                    int row = int.Parse(nextsquare.Split(':')[1]);
                                    ushort x1 = (ushort)(27 - (row * 3));
                                    ushort x2 = (ushort)(28 - (row * 3));
                                    ushort y = 4;
                                    ushort z1 = (ushort)(27 - (column * 3));
                                    ushort z2 = (ushort)(28 - (column * 3));
                                    {
                                        {
                                            mapon.Blockchange(x1, y, z1, Block.green);
                                            mapon.Blockchange(x2, y, z1, Block.green);
                                            mapon.Blockchange(x2, y, z2, Block.green);
                                            mapon.Blockchange(x1, y, z2, Block.green);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (all == false)
                {
                    if (p != null)
                    {
                        p.SendMessage("The Countdown map has been reset");
                        if (gamestatus == CountdownGameStatus.Finished)
                        {
                            p.SendMessage("You do not need to re-enable it");
                        }
                    }
                    gamestatus = CountdownGameStatus.Enabled;
                    foreach (Player pl in Player.players)
                    {
                        if (pl.playerofcountdown)
                        {
                            if (pl.level == mapon)
                            {
                                Command.all.Find("countdown").Use(pl, "join");
                                Player.SendMessage(pl, "You've rejoined countdown!!");
                            }
                            else
                            {
                                Player.SendMessage(pl, "You've been removed from countdown because you aren't on the map");
                                pl.playerofcountdown = false;
                                players.Remove(pl);
                            }
                        }
                    }
                }
                else if (all)
                {
                    if (p != null)
                    {
                        p.SendMessage("Countdown has been reset");
                        if (gamestatus == CountdownGameStatus.Finished)
                        {
                            p.SendMessage("You do not need to re-enable it");
                        }
                    }
                    gamestatus = CountdownGameStatus.Enabled;
                    CountdownGame.playersleft = 0;
                    CountdownGame.playersleftlist.Clear();
                    CountdownGame.players.Clear();
                    foreach (Player pl in Player.players)
                    {
                        pl.playerofcountdown = false;
                        pl.incountdown = false;
                    }
                }
                return;
            }
            else
            {
                switch (gamestatus)
                {
                    case CountdownGameStatus.Disabled:
                        if (p != null)
                        {
                            p.SendMessage("Please enable the game first");
                        }
                        return;

                    default:
                        if (p != null)
                        {
                            p.SendMessage("Please wait till the end of the game");
                        }
                        return;
                }
            }
        }

        public static void MessagePlayers(string message)
        {
            foreach (Player pl in Player.players)
            {
                if (pl.playerofcountdown)
                {
                    Player.SendMessage(pl, message);
                }
            }
        }
    }

    public enum CountdownGameStatus : int
    {
        Disabled = 0,
        Enabled = 1,
        AboutToStart = 2,
        InProgress = 3,
        Finished = 4
    }
}
