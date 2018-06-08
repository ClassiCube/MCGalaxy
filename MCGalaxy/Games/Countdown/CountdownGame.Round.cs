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
using MCGalaxy.Commands.World;
using MCGalaxy.Events;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    
    public sealed partial class CountdownGame : IGame {
        List<SquarePos> squaresLeft = new List<SquarePos>();
        BufferedBlockSender bulk = new BufferedBlockSender();
        
        public void BeginRound() {
            Status = CountdownGameStatus.RoundCountdown;
            ResetMap();
            SetGlassTube(Block.Glass, Block.Glass);
            Map.ChatLevel("Countdown is about to start!");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            Position spawnPos = Position.FromFeetBlockCoords(midX, Map.Height - 2, midZ);
            
            squaresLeft.Clear();
            for (int zz = 6; zz < Map.Length - 6; zz += 3)
                for (int xx = 6; xx < Map.Width - 6; xx += 3)
                    squaresLeft.Add(new SquarePos(xx, zz));
            
            if (FreezeMode)
                Map.ChatLevel("Countdown starting with difficulty " + SpeedType + " and mode freeze in:");
            else
                Map.ChatLevel("Countdown starting with difficulty " + SpeedType + " and mode normal in:");
            
            Thread.Sleep(2000);
            SpawnPlayers(spawnPos);
            Map.ChatLevel("-----&b5%S-----");
            
            if (Status != CountdownGameStatus.RoundCountdown) return;
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, Block.Air);
            bulk.Send(true);
            Thread.Sleep(1000);
            
            if (Status != CountdownGameStatus.RoundCountdown) return;
            Map.ChatLevel("-----&b4%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b3%S-----"); Thread.Sleep(1000);
            Cuboid(midX, Map.Height - 5, midZ, midX + 1, Map.Height - 5, midZ + 1, Block.Air);
            bulk.Send(true);
            
            if (Status != CountdownGameStatus.RoundCountdown) return;
            Map.ChatLevel("-----&b2%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b1%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("GO!!!!!!!");
            
            if (Status != CountdownGameStatus.RoundCountdown) return;
            Player[] players = Players.Items;
            Remaining.Clear();
            foreach (Player pl in players) { Remaining.Add(pl); }

            DoRound();
        }
        
        void SpawnPlayers(Position pos) {
            Player[] players = Players.Items;
            
            foreach (Player pl in players) {
                if (pl.level != Map) {
                    pl.SendMessage("Sending you to the correct map.");
                    PlayerActions.ChangeMap(pl, Map.name);
                }
                
                Entities.Spawn(pl, pl, pos, pl.Rot);
                pl.SendPos(Entities.SelfID, pos, pl.Rot);
            }
        }
        
        void DoRound() {
            if (FreezeMode) {
                MessageFreezeCountdown();
                Map.ChatLevel("&bPlayers Frozen");
                
                Player[] players = Players.Items;
                foreach (Player pl in players) {
                    Position pos = pl.Pos;
                    pl.Extras.PutInt("MCG_CD_X", pos.X);
                    pl.Extras.PutInt("MCG_CD_Z", pos.Z);
                }
                RemoveAllSquareBorders();
            }
            
            CloseOffBoard();
            Status = CountdownGameStatus.RoundInProgress;
            RemoveSquares();
        }

        void MessageFreezeCountdown() {
            Thread.Sleep(500);
            Map.ChatLevel("Welcome to Freeze Mode of countdown");
            Map.ChatLevel("You have 15 seconds to stand on a square");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            Thread.Sleep(500);
            Map.ChatLevel("-----&b15%S-----"); Thread.Sleep(500);
            Map.ChatLevel("Once the countdown is up, you are stuck on your square");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            Thread.Sleep(500);
            Map.ChatLevel("-----&b14%S-----"); Thread.Sleep(500);
            Map.ChatLevel("The squares then start to disappear");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            Thread.Sleep(500);
            Map.ChatLevel("-----&b13%S-----"); Thread.Sleep(500);
            Map.ChatLevel("Whoever is last out wins!");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            Thread.Sleep(500);
            Map.ChatLevel("-----&b12%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b11%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b10%S-----");
            Map.ChatLevel("Only 10 Seconds left to pick your places!");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            Thread.Sleep(1000);
            Map.ChatLevel("-----&b9%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b8%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b7%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b6%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b5%S-----");
            Map.ChatLevel("5 Seconds left to pick your places!");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            Thread.Sleep(1000);
            Map.ChatLevel("-----&b4%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b3%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b2%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b1%S-----"); Thread.Sleep(1000);
        }

        void CloseOffBoard() {
            SetGlassTube(Block.Air, Block.Glass);
            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            
            // Cuboid the borders around game board with air
            Cuboid(4, 4, 4, maxX - 4, 4, 4, Block.Air);
            Cuboid(4, 4, maxZ - 4, maxX - 4, 4, maxZ - 4, Block.Air);
            Cuboid(4, 4, 4, 4, 4, maxZ - 4, Block.Air);
            Cuboid(maxX - 4, 4, 4, maxX - 4, 4, maxZ - 4, Block.Air);
            bulk.Send(true);
        }
        
        
        void RemoveAllSquareBorders() {
            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            for (int xx = 6 - 1; xx <= Map.Width - 6; xx += 3) {
                Cuboid(xx, 4, 4, xx, 4, maxZ - 4, Block.Air);
            }
            for (int zz = 6 - 1; zz <= Map.Length - 6; zz += 3) {
                Cuboid(4, 4, zz, maxX - 4, 4, zz, Block.Air);
            }
            bulk.Send(true);
        }
        
        void RemoveSquares() {
            Random rng = new Random();
            while (Status == CountdownGameStatus.RoundInProgress && squaresLeft.Count > 0 && Remaining.Count != 0) {
                int i = rng.Next(squaresLeft.Count);
                SquarePos nextSquare = squaresLeft[i];
                squaresLeft.RemoveAt(i);
                RemoveSquare(nextSquare);

                if (squaresLeft.Count % 10 == 0) {
                    if (Status != CountdownGameStatus.RoundInProgress) return;
                    Map.ChatLevel(squaresLeft.Count + " squares left and " + Remaining.Count + " players remaining!");
                }
            }
        }
        
        void RemoveSquare(SquarePos pos) {
            ushort x1 = pos.X, x2 = (ushort)(pos.X + 1), y = 4, z1 = pos.Z, z2 = (ushort)(pos.Z + 1);
            Cuboid(x1, y, z1, x2, y, z2, Block.Yellow);
            bulk.Send(true);
            
            Thread.Sleep(Interval);
            Cuboid(x1, y, z1, x2, y, z2, Block.Orange);
            bulk.Send(true);
            
            Thread.Sleep(Interval);
            Cuboid(x1, y, z1, x2, y, z2, Block.Red);
            bulk.Send(true);
            
            Thread.Sleep(Interval);
            Cuboid(x1, y, z1, x2, y, z2, Block.Air);
            bulk.Send(true);
            
            // Remove glass borders, if neighbouring squares were previously removed           
            bool airMaxX = false, airMinZ = false, airMaxZ = false, airMinX = false;
            if (Map.IsAirAt(x1, y, (ushort)(z2 + 2))) {
                Map.Blockchange(x1, y, (ushort)(z2 + 1), Block.Air);
                Map.Blockchange(x2, y, (ushort)(z2 + 1), Block.Air);
                airMaxZ = true;
            }
            if (Map.IsAirAt(x1, y, (ushort)(z1 - 2))) {
                Map.Blockchange(x1, y, (ushort)(z1 - 1), Block.Air);
                Map.Blockchange(x2, y, (ushort)(z1 - 1), Block.Air);
                airMinZ = true;
            }
            if (Map.IsAirAt((ushort)(x2 + 2), y, z1)) {
                Map.Blockchange((ushort)(x2 + 1), y, z1, Block.Air);
                Map.Blockchange((ushort)(x2 + 1), y, z2, Block.Air);
                airMaxX = true;
            }
            if (Map.IsAirAt((ushort)(x1 - 2), y, z1)) {
                Map.Blockchange((ushort)(x1 - 1), y, z1, Block.Air);
                Map.Blockchange((ushort)(x1 - 1), y, z2, Block.Air);
                airMinX = true;
            }
            
            // Remove glass borders, if all neighbours to this corner have been removed
            if (Map.IsAirAt((ushort)(x1 - 2), y, (ushort)(z1 - 2)) && airMinX && airMinZ) {
                Map.Blockchange((ushort)(x1 - 1), y, (ushort)(z1 - 1), Block.Air);
            }
            if (Map.IsAirAt((ushort)(x1 - 2), y, (ushort)(z2 + 2)) && airMinX && airMaxZ) {
                Map.Blockchange((ushort)(x1 - 1), y, (ushort)(z2 + 1), Block.Air);
            }
            if (Map.IsAirAt((ushort)(x2 + 2), y, (ushort)(z1 - 2)) && airMaxX && airMinZ) {
                Map.Blockchange((ushort)(x2 + 1), y, (ushort)(z1 - 1), Block.Air);
            }
            if (Map.IsAirAt((ushort)(x2 + 2), y, (ushort)(z2 + 2)) && airMaxX && airMaxZ) {
                Map.Blockchange((ushort)(x2 + 1), y, (ushort)(z2 + 1), Block.Air);
            }
        }

        void UpdatePlayersLeft() {
            if (Status != CountdownGameStatus.RoundInProgress) return;
            Player[] players = Remaining.Items;
            
            switch (players.Length) {
                case 1:
                    Map.ChatLevel(players[0].ColoredName + " %Sis the winner!");
                    EndRound(players[0]);
                    break;
                case 2:
                    Map.ChatLevel("Only 2 Players left:");
                    Map.ChatLevel(players[0].ColoredName + " %Sand " + players[1].ColoredName);
                    break;
                case 5:
                    Map.ChatLevel("Only 5 Players left:");
                    Map.ChatLevel(players.Join(pl => pl.ColoredName));
                    break;
                default:
                    Map.ChatLevel(players.Length + " players left!");
                    break;
            }
        }
        
        public override void EndRound() { EndRound(null); }
        public void EndRound(Player winner) {
            squaresLeft.Clear();
            Status = CountdownGameStatus.Enabled;
            Remaining.Clear();
            squaresLeft.Clear();
            
            if (winner != null) {
                winner.SendMessage("Congratulations, you won this round of countdown!");
                Command.Find("Spawn").Use(winner, "");
            } else {
                Player[] players = Players.Items;
                foreach (Player pl in players) {
                    Command.Find("Spawn").Use(pl, "");
                }
                Map.ChatLevel("Current round was force ended!");
            }
        }

    }
}
