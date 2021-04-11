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
    
    public sealed partial class CountdownGame : RoundsGame {
        List<SquarePos> squaresLeft = new List<SquarePos>();
        BufferedBlockSender bulk = new BufferedBlockSender();
        
        protected override void DoRound() {
            bulk.level = Map;
            SetBoardOpening(Block.Glass);
            ResetBoard();
            if (!Running) return;
            DoRoundCountdown(10);
            if (!Running) return;
            
            SetBoardOpening(Block.Air);
            bulk.Flush();
            if (!Running) return;
            
            SpawnPlayers();
            if (!Running) return;
            
            BeginRound();
            SetBoardOpening(Block.Glass);
            RemveBoardBorders();
            if (!Running) return;
            
            RoundInProgress = true;
            if (FreezeMode) UpdateAllMotd();
            UpdateAllStatus();
            RemoveSquares();
        }
        
        protected override void ContinueOnSameMap() {
            // countdown only modifies board in the map, so it's fine to continue on the same map
            // without needing to reload the entire map
        }
        
        void BeginRound() {
            string modeSuffix = FreezeMode ? " in freeze mode" : "";
            Map.Message("Starting " + SpeedType + " speed Countdown" + modeSuffix);
            
            if (FreezeMode) {
                Map.Message("You have 20 seconds to stand on a square");
                Map.Message("You won't be able to move from that square once the game starts!");
                DoCountdown("&b{0} &Sseconds left", 20, 20);
            } else {
                Map.Message("You have 5 seconds before squares start disappearing");
                DoCountdown("&b{0} &Sseconds left", 5, 5);
            }
            
            if (!Running) return;
            Map.Message("GO!!!!!!!");
            
            Player[] players = Players.Items;
            Remaining.Clear();
            foreach (Player pl in players) { Remaining.Add(pl); }
            
            if (!Running || !FreezeMode) return;
            Map.Message("&bPlayers Frozen");
            
            foreach (Player pl in players) {
                Position pos = pl.Pos;
                pl.Extras["MCG_CD_X"] = pos.X;
                pl.Extras["MCG_CD_Z"] = pos.Z;
            }
            RemoveAllSquareBorders();
        } 
                
        void ResetBoard() {
            SetBoardOpening(Block.Glass);
            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            Cuboid(4, 4, 4, maxX - 4, 4, maxZ - 4, Block.Glass);          
            squaresLeft.Clear();
            
            int begX, endX, begZ, endZ;
            CountdownMap.CalcBoardExtents(Map.Width,  out begX, out endX);
            CountdownMap.CalcBoardExtents(Map.Length, out begZ, out endZ);
            
            for (int z = begZ; z <= endZ; z += 3)
                for (int x = begX; x <= endX; x += 3)
            {
                Cuboid(x, 4, z, x + 1, 4, z + 1, Block.Green);
                squaresLeft.Add(new SquarePos(x, z));
            }
            
            bulk.Flush();
        }        
        
        void SetBoardOpening(BlockID block) {
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, block);
            bulk.Flush();
        }
        
        void Cuboid(int x1, int y1, int z1, int x2, int y2, int z2, BlockID block) {
            for (int y = y1; y <= y2; y++)
                for (int z = z1; z <= z2; z++)
                    for (int x = x1; x <= x2; x++)
            {
                int index = Map.PosToInt((ushort)x, (ushort)y, (ushort)z);
                if (Map.DoPhysicsBlockchange(index, block)) {
                    bulk.Add(index, block);
                }
            }
        }
        
        void SpawnPlayers() {
            Player[] players = Players.Items;
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            Position pos = Position.FromFeetBlockCoords(midX, midY, midZ);
            pos.X -= 16; pos.Z -= 16;
            
            foreach (Player pl in players) {
                if (pl.level != Map) {
                    pl.Message("Sending you to the correct map.");
                    PlayerActions.ChangeMap(pl, Map.name);
                }
                
                Entities.Spawn(pl, pl, pos, pl.Rot);
                pl.SendPos(Entities.SelfID, pos, pl.Rot);
            }
        }

        void RemveBoardBorders() {
            int minX1 = 4, maxX2 = (Map.Width  - 1) - 4;
            int minZ1 = 4, maxZ2 = (Map.Length - 1) - 4;
            
            int maxX1, minX2, maxZ1, minZ2;
            CountdownMap.CalcBoardExtents(Map.Width,  out maxX1, out minX2);
            CountdownMap.CalcBoardExtents(Map.Length, out maxZ1, out minZ2);
            // Adjust coordinates to the borders around the board
            maxX1 -= 2; maxZ1 -= 2; minX2 += 3; minZ2 += 3;
            
            // Cuboid the borders around game board with air
            Cuboid(minX1, 4, minZ1, maxX2, 4, maxZ1, Block.Air);
            Cuboid(minX1, 4, minZ2, maxX2, 4, maxZ2, Block.Air);           
            Cuboid(minX1, 4, minZ1, maxX1, 4, maxZ2, Block.Air);
            Cuboid(minX2, 4, minZ1, maxX2, 4, maxZ2, Block.Air);
            bulk.Flush();
        }
        
        void RemoveAllSquareBorders() {
            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            for (int xx = 6 - 1; xx <= Map.Width - 6; xx += 3) {
                Cuboid(xx, 4, 4, xx, 4, maxZ - 4, Block.Air);
            }
            for (int zz = 6 - 1; zz <= Map.Length - 6; zz += 3) {
                Cuboid(4, 4, zz, maxX - 4, 4, zz, Block.Air);
            }
            bulk.Flush();
        }
        
        void RemoveSquares() {
            Random rng = new Random();
            while (RoundInProgress && Running && squaresLeft.Count > 0 && Remaining.Count > 0) {
                int i = rng.Next(squaresLeft.Count);
                SquarePos nextSquare = squaresLeft[i];
                squaresLeft.RemoveAt(i);
                
                RemoveSquare(nextSquare);
                if (!Running || !RoundInProgress) return;
                UpdateAllStatus1();
            }
        }
        
        void RemoveSquare(SquarePos pos) {
            ushort x1 = pos.X, x2 = (ushort)(pos.X + 1), z1 = pos.Z, z2 = (ushort)(pos.Z + 1);
            Cuboid(x1, 4, z1, x2, 4, z2, Block.Yellow);
            bulk.Flush();
            
            Thread.Sleep(Interval);
            Cuboid(x1, 4, z1, x2, 4, z2, Block.Orange);
            bulk.Flush();
            
            Thread.Sleep(Interval);
            Cuboid(x1, 4, z1, x2, 4, z2, Block.Red);
            bulk.Flush();
            
            Thread.Sleep(Interval);
            Cuboid(x1, 4, z1, x2, 4, z2, Block.Air);
            bulk.Flush();
            
            // Remove glass borders, if neighbouring squares were previously removed
            bool airMaxX = false, airMinZ = false, airMaxZ = false, airMinX = false;
            if (Map.IsAirAt(x1, 4, (ushort)(z2 + 2))) {
                Map.Blockchange(x1, 4, (ushort)(z2 + 1), Block.Air);
                Map.Blockchange(x2, 4, (ushort)(z2 + 1), Block.Air);
                airMaxZ = true;
            }
            if (Map.IsAirAt(x1, 4, (ushort)(z1 - 2))) {
                Map.Blockchange(x1, 4, (ushort)(z1 - 1), Block.Air);
                Map.Blockchange(x2, 4, (ushort)(z1 - 1), Block.Air);
                airMinZ = true;
            }
            if (Map.IsAirAt((ushort)(x2 + 2), 4, z1)) {
                Map.Blockchange((ushort)(x2 + 1), 4, z1, Block.Air);
                Map.Blockchange((ushort)(x2 + 1), 4, z2, Block.Air);
                airMaxX = true;
            }
            if (Map.IsAirAt((ushort)(x1 - 2), 4, z1)) {
                Map.Blockchange((ushort)(x1 - 1), 4, z1, Block.Air);
                Map.Blockchange((ushort)(x1 - 1), 4, z2, Block.Air);
                airMinX = true;
            }
            
            // Remove glass borders, if all neighbours to this corner have been removed
            if (Map.IsAirAt((ushort)(x1 - 2), 4, (ushort)(z1 - 2)) && airMinX && airMinZ) {
                Map.Blockchange((ushort)(x1 - 1), 4, (ushort)(z1 - 1), Block.Air);
            }
            if (Map.IsAirAt((ushort)(x1 - 2), 4, (ushort)(z2 + 2)) && airMinX && airMaxZ) {
                Map.Blockchange((ushort)(x1 - 1), 4, (ushort)(z2 + 1), Block.Air);
            }
            if (Map.IsAirAt((ushort)(x2 + 2), 4, (ushort)(z1 - 2)) && airMaxX && airMinZ) {
                Map.Blockchange((ushort)(x2 + 1), 4, (ushort)(z1 - 1), Block.Air);
            }
            if (Map.IsAirAt((ushort)(x2 + 2), 4, (ushort)(z2 + 2)) && airMaxX && airMaxZ) {
                Map.Blockchange((ushort)(x2 + 1), 4, (ushort)(z2 + 1), Block.Air);
            }
        }

        void OnPlayerDied(Player p) {
            if (!Remaining.Remove(p) || !RoundInProgress) return;
            Player[] players = Remaining.Items;
            
            switch (players.Length) {
                case 1:
                    Map.Message(players[0].ColoredName + " &Sis the winner!");
                    EndRound(players[0]);
                    break;
                case 2:
                    Map.Message("Only 2 Players left:");
                    Map.Message(players[0].ColoredName + " &Sand " + players[1].ColoredName);
                    break;
                default:
                    Map.Message(players.Length + " players left!");
                    break;
            }
            UpdateAllStatus2();
        }
        
        public override void EndRound() { EndRound(null); }
        public void EndRound(Player winner) {
            squaresLeft.Clear();
            RoundInProgress = false;
            Remaining.Clear();
            squaresLeft.Clear();
            UpdateAllStatus();
            if (FreezeMode) UpdateAllMotd();
            
            if (winner != null) {
                winner.Message("Congratulations, you won this round of countdown!");
                PlayerActions.Respawn(winner);
            } else {
                Player[] players = Players.Items;
                foreach (Player pl in players) {
                    PlayerActions.Respawn(pl);
                }
            }
        }
    }
}
