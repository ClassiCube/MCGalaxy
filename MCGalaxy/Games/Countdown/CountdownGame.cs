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

namespace MCGalaxy.Games {
    public sealed class CountdownGame : IGame {
        
        public VolatileArray<Player> Players = new VolatileArray<Player>();
        public VolatileArray<Player> Remaining = new VolatileArray<Player>();
        public CountdownGameStatus Status = CountdownGameStatus.Disabled;        
        public override bool Running { get { return Status != CountdownGameStatus.Disabled; } }
        
        public bool FreezeMode = false;
        public int Interval;
        public string SpeedType;
        
        CountdownPlugin plugin = new CountdownPlugin();
        List<SquarePos> squaresLeft = new List<SquarePos>();
        
        
        #region Round

        public void BeginRound(Player p) {
            Status = CountdownGameStatus.RoundCountdown;
            ResetMap();
            SetGlassTube(Block.Glass, Block.Glass);
            Map.ChatLevel("Countdown is about to start!");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            int xSpawn = (midX * 32 + 16);
            int ySpawn = ((Map.Height - 2) * 32);
            int zSpawn = (midZ * 32 + 16);
            
            squaresLeft.Clear();
            for (int zz = 6; zz < Map.Length - 6; zz += 3)
                for (int xx = 6; xx < Map.Width - 6; xx += 3)
                    squaresLeft.Add(new SquarePos(xx, zz));
            
            if (FreezeMode)
                Map.ChatLevel("Countdown starting with difficulty " + SpeedType + " and mode freeze in:");
            else
                Map.ChatLevel("Countdown starting with difficulty " + SpeedType + " and mode normal in:");
            
            Thread.Sleep(2000);
            SpawnPlayers(xSpawn, ySpawn, zSpawn);
            Map.ChatLevel("-----&b5%S-----");
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, Block.Air, Map);
            Thread.Sleep(1000);
            if (Status != CountdownGameStatus.RoundCountdown) return;
            
            Map.ChatLevel("-----&b4%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b3%S-----"); Thread.Sleep(1000);
            Cuboid(midX, Map.Height - 5, midZ, midX + 1, Map.Height - 5, midZ + 1, Block.Air, Map);
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
        
        void SpawnPlayers(int x, int y, int z) {
            Position pos = new Position(x, y, z);
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
        
        
        #region Do a round
        
        void DoRound() {
            if (FreezeMode) {
                MessageFreezeCountdown();
                Map.ChatLevel("&bPlayers Frozen");
                
                Player[] players = Players.Items;
                foreach (Player pl in players) {
                    Position pos = pl.Pos;
                    pl.CountdownFreezeX = pos.X;
                    pl.CountdownFreezeZ = pos.Z;
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
            Map.ChatLevel("The squares then start to dissapear");
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
            Cuboid(4, 4, 4, maxX - 4, 4, 4, Block.Air, Map);
            Cuboid(4, 4, maxZ - 4, maxX - 4, 4, maxZ - 4, Block.Air, Map);
            Cuboid(4, 4, 4, 4, 4, maxZ - 4, Block.Air, Map);
            Cuboid(maxX - 4, 4, 4, maxX - 4, 4, maxZ - 4, Block.Air, Map);
        }
        
        
        void RemoveAllSquareBorders() {
            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            for (int xx = 6 - 1; xx <= Map.Width - 6; xx += 3) {
                Cuboid(xx, 4, 4, xx, 4, maxZ - 4, Block.Air, Map);
            }
            for (int zz = 6 - 1; zz <= Map.Length - 6; zz += 3) {
                Cuboid(4, 4, zz, maxX - 4, 4, zz, Block.Air, Map);
            }
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
            Cuboid(x1, y, z1, x2, y, z2, Block.Yellow, Map);
            Thread.Sleep(Interval);
            Cuboid(x1, y, z1, x2, y, z2, Block.Orange, Map);
            Thread.Sleep(Interval);
            Cuboid(x1, y, z1, x2, y, z2, Block.Red, Map);
            Thread.Sleep(Interval);
            Cuboid(x1, y, z1, x2, y, z2, Block.Air, Map);
            // Remove glass borders if neighbouring squared were previously removed.
            
            bool airMaxX = false, airMinZ = false, airMaxZ = false, airMinX = false;
            if (Map.IsAirAt(x1, y, (ushort)(z2 + 2))) {
                Map.Blockchange(x1, y, (ushort)(z2 + 1), ExtBlock.Air);
                Map.Blockchange(x2, y, (ushort)(z2 + 1), ExtBlock.Air);
                airMaxZ = true;
            }
            if (Map.IsAirAt(x1, y, (ushort)(z1 - 2))) {
                Map.Blockchange(x1, y, (ushort)(z1 - 1), ExtBlock.Air);
                Map.Blockchange(x2, y, (ushort)(z1 - 1), ExtBlock.Air);
                airMinZ = true;
            }
            if (Map.IsAirAt((ushort)(x2 + 2), y, z1)) {
                Map.Blockchange((ushort)(x2 + 1), y, z1, ExtBlock.Air);
                Map.Blockchange((ushort)(x2 + 1), y, z2, ExtBlock.Air);
                airMaxX = true;
            }
            if (Map.IsAirAt((ushort)(x1 - 2), y, z1)) {
                Map.Blockchange((ushort)(x1 - 1), y, z1, ExtBlock.Air);
                Map.Blockchange((ushort)(x1 - 1), y, z2, ExtBlock.Air);
                airMinX = true;
            }
            
            // Remove glass borders for diagonals too.
            if (Map.IsAirAt((ushort)(x1 - 2), y, (ushort)(z1 - 2)) && airMinX && airMinZ) {
                Map.Blockchange((ushort)(x1 - 1), y, (ushort)(z1 - 1), ExtBlock.Air);
            }
            if (Map.IsAirAt((ushort)(x1 - 2), y, (ushort)(z2 + 2)) && airMinX && airMaxZ) {
                Map.Blockchange((ushort)(x1 - 1), y, (ushort)(z2 + 1), ExtBlock.Air);
            }
            if (Map.IsAirAt((ushort)(x2 + 2), y, (ushort)(z1 - 2)) && airMaxX && airMinZ) {
                Map.Blockchange((ushort)(x2 + 1), y, (ushort)(z1 - 1), ExtBlock.Air);
            }
            if (Map.IsAirAt((ushort)(x2 + 2), y, (ushort)(z2 + 2)) && airMaxX && airMaxZ) {
                Map.Blockchange((ushort)(x2 + 1), y, (ushort)(z2 + 1), ExtBlock.Air);
            }
        }

        #endregion
        

        public void PlayerDied(Player p) {
            Map.ChatLevel(p.ColoredName + " %Sis out of countdown!");
            Remaining.Remove(p);
            UpdatePlayersLeft();
        }

        public void UpdatePlayersLeft() {
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
        
        public void EndRound(Player winner) {
            squaresLeft.Clear();
            Status = CountdownGameStatus.Enabled;
            Remaining.Clear();
            squaresLeft.Clear();
            
            if (winner != null) {
                winner.SendMessage("Congratulations, you won this round of countdown!");
                Command.all.FindByName("Spawn").Use(winner, "");
            } else {
                Player[] players = Players.Items;
                foreach (Player pl in players) {
                    Command.all.FindByName("Spawn").Use(pl, "");
                }
                Map.ChatLevel("Current round was force ended!");
            }
        }
        
        #endregion
        
        
        public void Enable(Player p) {
            plugin.Game = this;
            plugin.Load(false);
            
            MapName = "countdown";
            CmdLoad.LoadLevel(null, "countdown");
            Map = LevelInfo.FindExact("countdown");
            
            if (Map == null) {
                Player.Message(p, "Countdown level not found, generating..");
                GenerateMap(p, 32, 32, 32);
                Map = LevelInfo.FindExact("countdown");
            }
            
            Map.Config.Deletable = false;
            Map.Config.Buildable = false;
            Map.BuildAccess.Min = LevelPermission.Nobody;
            Map.Config.MOTD = "Welcome to the Countdown map! -hax";
            
            Status = CountdownGameStatus.Enabled;
            Chat.MessageGlobal("Countdown has been enabled!");
        }

        public void Disable() {
            if (Status == CountdownGameStatus.RoundInProgress) EndRound(null);
            
            Status = CountdownGameStatus.Disabled;
            plugin.Unload(false);
            
            Map.ChatLevel("Countdown was disabled.");
            Players.Clear();
            Remaining.Clear();
            squaresLeft.Clear();
        }
        
        public void GenerateMap(Player p, int width, int height, int length) {
            Level lvl = CountdownMapGen.Generate(width, height, length);
            Level cur = LevelInfo.FindExact("countdown");
            if (cur != null) LevelActions.Replace(cur, lvl);
            else LevelInfo.Loaded.Add(lvl);
            
            lvl.Save();
            Map = lvl;
            
            const string format = "Generated map ({0}x{1}x{2}), sending you to it..";
            Player.Message(p, format, width, height, length);
            PlayerActions.ChangeMap(p, "countdown");
            
            Position pos = new Position(16 + 8 * 32, 32 + 23 * 32, 16 + 17 * 32);
            p.SendPos(Entities.SelfID, pos, p.Rot);
        }
        
        public void ResetMap() {
            SetGlassTube(Block.Air, Block.Air);

            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            Cuboid(4, 4, 4, maxX - 4, 4, maxZ - 4, Block.Glass, Map);
            for(int zz = 6; zz < maxZ - 6; zz += 3)
                for (int xx = 6; xx < maxX - 6; xx += 3)
                    Cuboid(xx, 4, zz, xx + 1, 4, zz + 1, Block.Green, Map);
            
            Map.ChatLevel("Countdown map has been reset");
        }
        
        
        void SetGlassTube(byte block, byte floorBlock) {
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            Cuboid(midX - 1, midY + 1, midZ - 2, midX, midY + 2, midZ - 2, block, Map);
            Cuboid(midX - 1, midY + 1, midZ + 1, midX, midY + 2, midZ + 1, block, Map);
            Cuboid(midX - 2, midY + 1, midZ - 1, midX - 2, midY + 2, midZ, block, Map);
            Cuboid(midX + 1, midY + 1, midZ - 1, midX + 1, midY + 2, midZ, block, Map);
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, floorBlock, Map);
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
        
        struct SquarePos {
            public ushort X, Z;
            public SquarePos(int x, int z) { X = (ushort)x; Z = (ushort)z; }
        }
        
        
        public override void PlayerJoinedGame(Player p) {
            if (!Players.Contains(p)) {
                Players.Add(p);
                Player.Message(p, "You've joined countdown!");
                Chat.MessageGlobal("{0} %Sjoined countdown!", p.ColoredName);
                if (p.level != Map) PlayerActions.ChangeMap(p, "countdown");
            } else {
                Player.Message(p, "You've already joined countdown. To leave type /countdown leave");
            }
        }
        
        public override void PlayerLeftGame(Player p) {
            Player.Message(p, "You've left countdown.");
            Players.Remove(p);
            Remaining.Remove(p);
            UpdatePlayersLeft();
        }
    }

    public enum CountdownGameStatus {
        /// <summary> Countdown is not running. </summary>
        Disabled,
        
        /// <summary> Countdown is running, but no round has begun yet. </summary>
        Enabled,
        
        /// <summary> Timer is counting down to start of round. </summary>
        RoundCountdown,
        
        /// <summary> Round is in progress. </summary>
        RoundInProgress,
    }
}
