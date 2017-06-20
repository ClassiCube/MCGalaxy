/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy.Games {    
    public sealed partial class CountdownGame {
        
        public List<Player> players = new List<Player>();
        public List<Player> playersleftlist = new List<Player>();
        public List<SquarePos> squaresLeft = new List<SquarePos>();
        public Level mapon;

        public int speed;
        public bool freezemode = false;
        public bool cancel = false;

        public string speedtype;

        public CountdownGameStatus gamestatus = CountdownGameStatus.Disabled;

        public void GameStart(Player p) {
            switch (gamestatus) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Please enable Countdown first!!"); return;
                case CountdownGameStatus.AboutToStart:
                    Player.Message(p, "Game is about to start"); return;
                case CountdownGameStatus.InProgress:
                    Player.Message(p, "Game is already in progress"); return;
                case CountdownGameStatus.Finished:
                    Player.Message(p, "Game has finished"); return;
                case CountdownGameStatus.Enabled:
                    gamestatus = CountdownGameStatus.AboutToStart;
                    Thread.Sleep(2000); break;
            }
            
            SetGlassTube(Block.glass, Block.glass);
            mapon.ChatLevel("Countdown is about to start!!");
            mapon.BuildAccess.Min = LevelPermission.Nobody;
            int midX = mapon.Width / 2, midY = mapon.Height / 2, midZ = mapon.Length / 2;
            int xSpawn = (midX * 32 + 16);
            int ySpawn = ((mapon.Height - 2) * 32);
            int zSpawn = (midZ * 32 + 16);
            
            squaresLeft.Clear();
            for(int zz = 6; zz < mapon.Length - 6; zz += 3)
                for (int xx = 6; xx < mapon.Width - 6; xx += 3)
                    squaresLeft.Add(new SquarePos(xx, zz));            
            
            if (freezemode)
                mapon.ChatLevel("Countdown starting with difficulty " + speedtype + " and mode freeze in:");
            else
                mapon.ChatLevel("Countdown starting with difficulty " + speedtype + " and mode normal in:");
            
            Thread.Sleep(2000);
            SpawnPlayers(xSpawn, ySpawn, zSpawn);
            mapon.ChatLevel("-----&b5%S-----");
            
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, Block.air, mapon);
            Thread.Sleep(1000);
            mapon.ChatLevel("-----&b4%S-----"); Thread.Sleep(1000);
            mapon.ChatLevel("-----&b3%S-----"); Thread.Sleep(1000);
            Cuboid(midX, mapon.Height - 5, midZ, midX + 1, mapon.Height - 5, midZ + 1, Block.air, mapon);
            mapon.ChatLevel("-----&b2%S-----"); Thread.Sleep(1000);
            mapon.ChatLevel("-----&b1%S-----"); Thread.Sleep(1000);
            mapon.ChatLevel("GO!!!!!!!");
            
            playersleftlist = players;
            foreach (Player pl in players)
                pl.incountdown = true;
            AfterStart();
            Play();
        }

        public void Play() {
            if (!freezemode) {
                RemoveRandomSquares();
            } else {
                SendFreezeMessages();
                MessageAll("&bPlayers Frozen");
                gamestatus = CountdownGameStatus.InProgress;
                foreach (Player pl in players)
                    pl.countdownsettemps = true;
                Thread.Sleep(500);
                
                RemoveGlassBlocks();
                RemoveRandomSquares();
            }
        }
        
        void SpawnPlayers(int x, int y, int z) {
            Position pos = new Position(x, y, z);
            foreach (Player pl in players) {
                if (pl.level != mapon) {
                    pl.SendMessage("Sending you to the correct map.");
                    PlayerActions.ChangeMap(pl, mapon.name);
                }
                Entities.Spawn(pl, pl, pos, pl.Rot);
            }
        }
        
        void SendFreezeMessages() {
            Thread.Sleep(500);
            MessageAll("Welcome to Freeze Mode of countdown");
            MessageAll("You have 15 seconds to stand on a square");
            Thread.Sleep(500);
            MessageAll("-----&b15%S-----"); Thread.Sleep(500);
            MessageAll("Once the countdown is up, you are stuck on your square");
            Thread.Sleep(500);
            MessageAll("-----&b14%S-----"); Thread.Sleep(500);
            MessageAll("The squares then start to dissapear");
            Thread.Sleep(500);
            MessageAll("-----&b13%S-----"); Thread.Sleep(500);
            MessageAll("Whoever is last out wins!!");
            Thread.Sleep(500);
            MessageAll("-----&b12%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b11%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b10%S-----");
            MessageAll("Only 10 Seconds left to pick your places!!");
            Thread.Sleep(1000);
            MessageAll("-----&b9%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b8%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b7%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b6%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b5%S-----");
            MessageAll("5 Seconds left to pick your places!!");
            Thread.Sleep(1000);
            MessageAll("-----&b4%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b3%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b2%S-----"); Thread.Sleep(1000);
            MessageAll("-----&b1%S-----"); Thread.Sleep(1000);
        }
        
        void RemoveGlassBlocks() {
            int maxX = mapon.Width - 1, maxZ = mapon.Length - 1;
            for (int xx = 6; xx < maxX - 6; xx += 3)
                Cuboid(xx - 1, 4, 4, xx - 1, 4, maxZ - 4, Block.air, mapon);
            for(int zz = 6; zz < maxZ - 6; zz += 3)
                Cuboid(4, 4, zz - 1, maxX - 4, 4, zz - 2, Block.air, mapon);
        }
        
        void RemoveRandomSquares() {
            while (squaresLeft.Count > 0 && playersleftlist.Count != 0
                   && (gamestatus == CountdownGameStatus.InProgress || gamestatus == CountdownGameStatus.Finished))
            {
                Random number = new Random();
                int index = number.Next(squaresLeft.Count);
                SquarePos nextsquare = squaresLeft[index];
                squaresLeft.RemoveAt(index);
                RemoveSquare(nextsquare);
                
                if (squaresLeft.Count % 10 == 0 && gamestatus != CountdownGameStatus.Finished)
                    mapon.ChatLevel(squaresLeft.Count + " Squares Left and " + playersleftlist.Count + " Players left!!");
                if (cancel)
                    End(null);
            }
        }
        
        void RemoveSquare(SquarePos pos) {
            ushort x1 = pos.X, x2 = (ushort)(pos.X + 1), y = 4, z1 = pos.Z, z2 = (ushort)(pos.Z + 1);
            Cuboid(x1, y, z1, x2, y, z2, Block.yellow, mapon);
            Thread.Sleep(speed);
            Cuboid(x1, y, z1, x2, y, z2, Block.orange, mapon);
            Thread.Sleep(speed);
            Cuboid(x1, y, z1, x2, y, z2, Block.red, mapon);
            Thread.Sleep(speed);
            Cuboid(x1, y, z1, x2, y, z2, Block.air, mapon);
            
            //beneath this is checking the glass next to the square
            bool up = false, left = false, right = false, down = false;
            //directly next to
            if (mapon.IsAirAt(x1, y, z2 + 2)) //right
            {
                mapon.Blockchange(x1, y, (ushort)(z2 + 1), ExtBlock.Air);
                mapon.Blockchange(x2, y, (ushort)(z2 + 1), ExtBlock.Air);
                right = true;
            }
            if (mapon.IsAirAt(x1, y, z1 - 2)) //left
            {
                mapon.Blockchange(x1, y, (ushort)(z1 - 1), ExtBlock.Air);
                mapon.Blockchange(x2, y, (ushort)(z1 - 1), ExtBlock.Air);
                left = true;
            }
            if (mapon.IsAirAt(x2 + 2, y, z1)) //up
            {
                mapon.Blockchange((ushort)(x2 + 1), y, z1, ExtBlock.Air);
                mapon.Blockchange((ushort)(x2 + 1), y, z2, ExtBlock.Air);
                up = true;
            }
            if (mapon.IsAirAt(x1 - 2, y, z1)) //down
            {
                mapon.Blockchange((ushort)(x1 - 1), y, z1, ExtBlock.Air);
                mapon.Blockchange((ushort)(x1 - 1), y, z2, ExtBlock.Air);
                down = true;
            }
            
            //diagonal >:(
            if (mapon.IsAirAt(x1 - 2, y, z1 - 2) && left && down) //bottom left
            {
                mapon.Blockchange((ushort)(x1 - 1), y, (ushort)(z1 - 1), ExtBlock.Air);
            }
            if (mapon.IsAirAt(x1 - 2, y, z2 + 2) && right && down) //bottom right
            {
                mapon.Blockchange((ushort)(x1 - 1), y, (ushort)(z2 + 1), ExtBlock.Air);
            }
            if (mapon.IsAirAt(x2 + 2, y, z1 - 2) && left && up) //top left
            {
                mapon.Blockchange((ushort)(x2 + 1), y, (ushort)(z1 - 1), ExtBlock.Air);
            }
            if (mapon.IsAirAt(x2 + 2, y, z2 + 2) && right && up) //top right
            {
                mapon.Blockchange((ushort)(x2 + 1), y, (ushort)(z2 + 1), ExtBlock.Air);
            }
        }

        void AfterStart() {
            SetGlassTube(Block.air, Block.glass);
            
            int maxX = mapon.Width - 1, maxZ = mapon.Length - 1;
            Cuboid(4, 4, 4, maxX - 4, 4, 4, Block.air, mapon);
            Cuboid(4, 4, maxZ - 4, maxX - 4, 4, maxZ - 4, Block.air, mapon);
            Cuboid(4, 4, 4, 4, 4, maxZ - 4, Block.air, mapon);
            Cuboid(maxX - 4, 4, 4, maxX - 4, 4, maxZ - 4, Block.air, mapon);

            if (!freezemode) {
                gamestatus = CountdownGameStatus.InProgress;
            }
        }

        public void Death(Player p) {
            mapon.ChatLevel(p.ColoredName + " %Sis out of countdown!!");
            p.incountdown = false;
            playersleftlist.Remove(p);
            MessagePlayersLeft();
        }

        void MessagePlayersLeft() {
            switch (playersleftlist.Count) {
                case 1:
                    mapon.ChatLevel(playersleftlist[0].ColoredName + " %Sis the winner!!");
                    End(playersleftlist[0]);
                    break;
                case 2:
                    mapon.ChatLevel("Only 2 Players left:");
                    mapon.ChatLevel(playersleftlist[0].ColoredName + " %Sand " + playersleftlist[1].ColoredName);
                    break;
                case 5:
                    mapon.ChatLevel("Only 5 Players left:");
                    foreach (Player pl in playersleftlist) {
                        mapon.ChatLevel(pl.ColoredName);
                        Thread.Sleep(500);
                    }
                    break;
                default:
                    mapon.ChatLevel("Now there are " + playersleftlist.Count + " players left!!");
                    break;
            }
        }
        
        void End(Player winner) {
            squaresLeft.Clear();
            gamestatus = CountdownGameStatus.Finished;
            playersleftlist.Clear();
            
            if (winner != null) {
                winner.SendMessage("Congratulations!! You won!!!");
                Command.all.Find("spawn").Use(winner, "");
                winner.incountdown = false;
            } else {
                foreach (Player pl in players) {
                    Player.Message(pl, "The countdown game was canceled!");
                    Command.all.Find("spawn").Use(pl, "");
                }
                Chat.MessageGlobal("The countdown game was canceled!!");
                gamestatus = CountdownGameStatus.Enabled;
                playersleftlist.Clear();
                players.Clear();
                squaresLeft.Clear();
                Reset(null, true);
                cancel = false;
            }
        }

        public void Reset(Player p, bool all) {
            if (!(gamestatus == CountdownGameStatus.Enabled || gamestatus == CountdownGameStatus.Finished || gamestatus == CountdownGameStatus.Disabled)) {
                switch (gamestatus) {
                    case CountdownGameStatus.Disabled:
                        Player.Message(p, "Please enable the game first"); return;
                    default:
                        Player.Message(p, "Please wait till the end of the game"); return;
                }
            }
            SetGlassTube(Block.air, Block.air);

            int maxX = mapon.Width - 1, maxZ = mapon.Length - 1;
            Cuboid(4, 4, 4, maxX - 4, 4, maxZ - 4, Block.glass, mapon);
            for(int zz = 6; zz < maxZ - 6; zz += 3)
                for (int xx = 6; xx < maxX - 6; xx += 3)
                    Cuboid(xx, 4, zz, xx + 1, 4, zz + 1, Block.green, mapon);
            
            if (!all) {
                Player.Message(p, "The Countdown map has been reset");
                if (gamestatus == CountdownGameStatus.Finished)
                    Player.Message(p, "You do not need to re-enable it");
                gamestatus = CountdownGameStatus.Enabled;
                
                Player[] online = PlayerInfo.Online.Items; 
                foreach (Player pl in online) {
                    if (!pl.playerofcountdown) continue;
                    if (pl.level == mapon) {
                        Command.all.Find("countdown").Use(pl, "join");
                        Player.Message(pl, "You've rejoined countdown!!");
                    } else {
                        Player.Message(pl, "You've been removed from countdown because you aren't on the map");
                        pl.playerofcountdown = false;
                        players.Remove(pl);
                    }
                }
            } else {
                Player.Message(p, "Countdown has been reset");
                if (gamestatus == CountdownGameStatus.Finished)
                    Player.Message(p, "You do not need to re-enable it");
                gamestatus = CountdownGameStatus.Enabled;
                playersleftlist.Clear();
                players.Clear();
                squaresLeft.Clear();
                
                speed = 750;
                Player[] online = PlayerInfo.Online.Items;
                foreach (Player pl in online) {
                    pl.playerofcountdown = false;
                    pl.incountdown = false;
                }
            }
        }
        
        void SetGlassTube(byte block, byte floorBlock) {        
            int midX = mapon.Width / 2, midY = mapon.Height / 2, midZ = mapon.Length / 2;
            Cuboid(midX - 1, midY + 1, midZ - 2, midX, midY + 2, midZ - 2, block, mapon);
            Cuboid(midX - 1, midY + 1, midZ + 1, midX, midY + 2, midZ + 1, block, mapon);
            Cuboid(midX - 2, midY + 1, midZ - 1, midX - 2, midY + 2, midZ, block, mapon);
            Cuboid(midX + 1, midY + 1, midZ - 1, midX + 1, midY + 2, midZ, block, mapon);
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, floorBlock, mapon);
        }

        public void MessageAll(string message) {
            Player[] online = PlayerInfo.Online.Items; 
            foreach (Player pl in online) {
                if (pl.playerofcountdown)
                    Player.Message(pl, message);
            }
        }
        
        static void Cuboid(int x1, int y1, int z1, int x2, int y2, int z2, byte raw, Level lvl) {
            ExtBlock block = (ExtBlock)raw;
            for (int y = y1; y <= y2; y++)
                for (int z = z1; z <= z2; z++)
                    for (int x = x1; x <= x2; x++)
            {
                lvl.Blockchange((ushort)x, (ushort)y, (ushort)z, block);
            }
        }
        
        public struct SquarePos {
            public ushort X, Z;
            
            public SquarePos(int x, int z) {
                X = (ushort)x; Z = (ushort)z;
            }
        }
    }

    public enum CountdownGameStatus {
        Disabled, Enabled, AboutToStart, InProgress, Finished,
    }
}
