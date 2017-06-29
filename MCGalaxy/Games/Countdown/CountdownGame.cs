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
    public sealed class CountdownGame : IGame {
        
        /// <summary> All players who are playing this countdown game. </summary>
        public List<Player> Players = new List<Player>();
        
        /// <summary> Players who are still alive in the current round. </summary>
        public List<Player> PlayersRemaining = new List<Player>();
       
        /// <summary> Map countdown is running on. </summary>
        public Level Map;
        
        /// <summary> Current status of the countdown game. </summary>
        public CountdownGameStatus Status = CountdownGameStatus.Disabled;        

        public int Speed;
        public bool FreezeMode = false;
        public bool cancel = false;
        public string SpeedType;
        
        CountdownPlugin plugin;
        List<SquarePos> squaresLeft = new List<SquarePos>();

        public void BeginRound(Player p) {
            if (plugin == null) {
                plugin = new CountdownPlugin();
                plugin.Game = this;
                plugin.Load(false);
            }
            
            SetGlassTube(Block.glass, Block.glass);
            Map.ChatLevel("Countdown is about to start!");
            Map.BuildAccess.Min = LevelPermission.Nobody;
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            int xSpawn = (midX * 32 + 16);
            int ySpawn = ((Map.Height - 2) * 32);
            int zSpawn = (midZ * 32 + 16);
            
            squaresLeft.Clear();
            for(int zz = 6; zz < Map.Length - 6; zz += 3)
                for (int xx = 6; xx < Map.Width - 6; xx += 3)
                    squaresLeft.Add(new SquarePos(xx, zz));            
            
            if (FreezeMode)
                Map.ChatLevel("Countdown starting with difficulty " + SpeedType + " and mode freeze in:");
            else
                Map.ChatLevel("Countdown starting with difficulty " + SpeedType + " and mode normal in:");
            
            Thread.Sleep(2000);
            SpawnPlayers(xSpawn, ySpawn, zSpawn);
            Map.ChatLevel("-----&b5%S-----");
            
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, Block.air, Map);
            Thread.Sleep(1000);
            Map.ChatLevel("-----&b4%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b3%S-----"); Thread.Sleep(1000);
            Cuboid(midX, Map.Height - 5, midZ, midX + 1, Map.Height - 5, midZ + 1, Block.air, Map);
            Map.ChatLevel("-----&b2%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("-----&b1%S-----"); Thread.Sleep(1000);
            Map.ChatLevel("GO!!!!!!!");
            
            PlayersRemaining = new List<Player>(Players);
            foreach (Player pl in Players) {
                pl.InCountdown = true;
            }
            
            DoRound();
        }
        
        void SpawnPlayers(int x, int y, int z) {
            Position pos = new Position(x, y, z);
            foreach (Player pl in Players) {
                if (pl.level != Map) {
                    pl.SendMessage("Sending you to the correct map.");
                    PlayerActions.ChangeMap(pl, Map.name);
                }
                Entities.Spawn(pl, pl, pos, pl.Rot);
            }
        }
        
        
        #region Do a round
        
        void DoRound() {
            if (FreezeMode) {
                MessageFreezeCountdown();
                MessageAll("&bPlayers Frozen");
                
                foreach (Player pl in Players) {
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

        void CloseOffBoard() {
            SetGlassTube(Block.air, Block.glass);            
            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            
            // Cuboid the borders around game board with air
            Cuboid(4, 4, 4, maxX - 4, 4, 4, Block.air, Map);
            Cuboid(4, 4, maxZ - 4, maxX - 4, 4, maxZ - 4, Block.air, Map);
            Cuboid(4, 4, 4, 4, 4, maxZ - 4, Block.air, Map);
            Cuboid(maxX - 4, 4, 4, maxX - 4, 4, maxZ - 4, Block.air, Map);
        }        
        
        
        void RemoveAllSquareBorders() {
            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            for (int xx = 6; xx < maxX - 6; xx += 3)
                Cuboid(xx - 1, 4, 4, xx - 1, 4, maxZ - 4, Block.air, Map);
            for(int zz = 6; zz < maxZ - 6; zz += 3)
                Cuboid(4, 4, zz - 1, maxX - 4, 4, zz - 2, Block.air, Map);
        }
        
        void RemoveSquares() {
            Random rng = new Random();
            while (squaresLeft.Count > 0 && PlayersRemaining.Count != 0
                   && (Status == CountdownGameStatus.RoundInProgress || Status == CountdownGameStatus.RoundFinished))
            {                
                int index = rng.Next(squaresLeft.Count);
                SquarePos nextSquare = squaresLeft[index];
                squaresLeft.RemoveAt(index);
                RemoveSquare(nextSquare);
                
                if (squaresLeft.Count % 10 == 0 && Status != CountdownGameStatus.RoundFinished)
                    Map.ChatLevel(squaresLeft.Count + " squares left and " + PlayersRemaining.Count + " players remaining!");
                if (cancel)
                    End(null);
            }
        }
        
        void RemoveSquare(SquarePos pos) {
            ushort minX = pos.X, maxX = (ushort)(pos.X + 1), y = 4, minZ = pos.Z, maxZ = (ushort)(pos.Z + 1);
            Cuboid(minX, y, minZ, maxX, y, maxZ, Block.yellow, Map);
            Thread.Sleep(Speed);
            Cuboid(minX, y, minZ, maxX, y, maxZ, Block.orange, Map);
            Thread.Sleep(Speed);
            Cuboid(minX, y, minZ, maxX, y, maxZ, Block.red, Map);
            Thread.Sleep(Speed);
            Cuboid(minX, y, minZ, maxX, y, maxZ, Block.air, Map);           
            // Remove glass borders if neighbouring squared were previously removed.
            
            bool airMaxX = false, airMinZ = false, airMaxZ = false, airMinX = false;
            if (Map.IsAirAt(minX, y, maxZ + 2)) {
                Map.Blockchange(minX, y, (ushort)(maxZ + 1), ExtBlock.Air);
                Map.Blockchange(maxX, y, (ushort)(maxZ + 1), ExtBlock.Air);
                airMaxZ = true;
            }
            if (Map.IsAirAt(minX, y, minZ - 2)) {
                Map.Blockchange(minX, y, (ushort)(minZ - 1), ExtBlock.Air);
                Map.Blockchange(maxX, y, (ushort)(minZ - 1), ExtBlock.Air);
                airMinZ = true;
            }
            if (Map.IsAirAt(maxX + 2, y, minZ)) {
                Map.Blockchange((ushort)(maxX + 1), y, minZ, ExtBlock.Air);
                Map.Blockchange((ushort)(maxX + 1), y, maxZ, ExtBlock.Air);
                airMaxX = true;
            }
            if (Map.IsAirAt(minX - 2, y, minZ)) {
                Map.Blockchange((ushort)(minX - 1), y, minZ, ExtBlock.Air);
                Map.Blockchange((ushort)(minX - 1), y, maxZ, ExtBlock.Air);
                airMinX = true;
            }
            
            // Remove glass borders for diagonals too.
            if (Map.IsAirAt(minX - 2, y, minZ - 2) && airMinZ && airMinX) {
                Map.Blockchange((ushort)(minX - 1), y, (ushort)(minZ - 1), ExtBlock.Air);
            }
            if (Map.IsAirAt(minX - 2, y, maxZ + 2) && airMaxZ && airMinX) {
                Map.Blockchange((ushort)(minX - 1), y, (ushort)(maxZ + 1), ExtBlock.Air);
            }
            if (Map.IsAirAt(maxX + 2, y, minZ - 2) && airMinZ && airMaxX) {
                Map.Blockchange((ushort)(maxX + 1), y, (ushort)(minZ - 1), ExtBlock.Air);
            }
            if (Map.IsAirAt(maxX + 2, y, maxZ + 2) && airMaxZ && airMaxX) {
                Map.Blockchange((ushort)(maxX + 1), y, (ushort)(maxZ + 1), ExtBlock.Air);
            }
        }

        #endregion
        

        public void Death(Player p) {
            Map.ChatLevel(p.ColoredName + " %Sis out of countdown!!");
            p.InCountdown = false;
            PlayersRemaining.Remove(p);
            UpdatePlayersLeft();
        }

        public void UpdatePlayersLeft() {
            if (Status != CountdownGameStatus.RoundInProgress) return;
        	
            switch (PlayersRemaining.Count) {
                case 1:
                    Map.ChatLevel(PlayersRemaining[0].ColoredName + " %Sis the winner!!");
                    End(PlayersRemaining[0]);
                    break;
                case 2:
                    Map.ChatLevel("Only 2 Players left:");
                    Map.ChatLevel(PlayersRemaining[0].ColoredName + " %Sand " + PlayersRemaining[1].ColoredName);
                    break;
                case 5:
                    Map.ChatLevel("Only 5 Players left:");
                    foreach (Player pl in PlayersRemaining) {
                        Map.ChatLevel(pl.ColoredName);
                        Thread.Sleep(500);
                    }
                    break;
                default:
                    Map.ChatLevel("Now there are " + PlayersRemaining.Count + " players left!!");
                    break;
            }
        }
        
        void End(Player winner) {
            squaresLeft.Clear();
            Status = CountdownGameStatus.RoundFinished;
            PlayersRemaining.Clear();
            
            if (winner != null) {
                winner.SendMessage("Congratulations!! You won!!!");
                Command.all.Find("spawn").Use(winner, "");
                winner.InCountdown = false;
            } else {
                foreach (Player pl in Players) {
                    Player.Message(pl, "The countdown game was canceled!");
                    Command.all.Find("spawn").Use(pl, "");
                }
                Chat.MessageGlobal("The countdown game was canceled!!");
                Status = CountdownGameStatus.Enabled;
                PlayersRemaining.Clear();
                Players.Clear();
                squaresLeft.Clear();
                Reset(null, true);
                cancel = false;
            }
        }

        public void Reset(Player p, bool all) {
            if (!(Status == CountdownGameStatus.Enabled || Status == CountdownGameStatus.RoundFinished || Status == CountdownGameStatus.Disabled)) {
                switch (Status) {
                    case CountdownGameStatus.Disabled:
                        Player.Message(p, "Please enable the game first"); return;
                    default:
                        Player.Message(p, "Please wait till the end of the game"); return;
                }
            }
            SetGlassTube(Block.air, Block.air);

            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            Cuboid(4, 4, 4, maxX - 4, 4, maxZ - 4, Block.glass, Map);
            for(int zz = 6; zz < maxZ - 6; zz += 3)
                for (int xx = 6; xx < maxX - 6; xx += 3)
                    Cuboid(xx, 4, zz, xx + 1, 4, zz + 1, Block.green, Map);
            
            if (!all) {
                Player.Message(p, "The Countdown map has been reset");
                if (Status == CountdownGameStatus.RoundFinished)
                    Player.Message(p, "You do not need to re-enable it");
                Status = CountdownGameStatus.Enabled;
                
                Player[] online = PlayerInfo.Online.Items; 
                foreach (Player pl in online) {
                    if (!pl.playerofcountdown) continue;
                    if (pl.level == Map) {
                        Command.all.Find("countdown").Use(pl, "join");
                        Player.Message(pl, "You've rejoined countdown!!");
                    } else {
                        Player.Message(pl, "You've been removed from countdown because you aren't on the map");
                        pl.playerofcountdown = false;
                        Players.Remove(pl);
                    }
                }
            } else {
                Player.Message(p, "Countdown has been reset");
                if (Status == CountdownGameStatus.RoundFinished)
                    Player.Message(p, "You do not need to re-enable it");
                Status = CountdownGameStatus.Enabled;
                PlayersRemaining.Clear();
                Players.Clear();
                squaresLeft.Clear();
                
                Speed = 750;
                Player[] online = PlayerInfo.Online.Items;
                foreach (Player pl in online) {
                    pl.playerofcountdown = false;
                    pl.InCountdown = false;
                }
            }
        }
        
        void SetGlassTube(byte block, byte floorBlock) {        
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            Cuboid(midX - 1, midY + 1, midZ - 2, midX, midY + 2, midZ - 2, block, Map);
            Cuboid(midX - 1, midY + 1, midZ + 1, midX, midY + 2, midZ + 1, block, Map);
            Cuboid(midX - 2, midY + 1, midZ - 1, midX - 2, midY + 2, midZ, block, Map);
            Cuboid(midX + 1, midY + 1, midZ - 1, midX + 1, midY + 2, midZ, block, Map);
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, floorBlock, Map);
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
        
        struct SquarePos {
            public ushort X, Z;
            
            public SquarePos(int x, int z) {
                X = (ushort)x; Z = (ushort)z;
            }
        }        
        
        
        public override void PlayerJoinedGame(Player p) {
            if (!Server.Countdown.Players.Contains(p)) {
                Server.Countdown.Players.Add(p);
                Player.Message(p, "You've joined the Countdown game!!");
                Chat.MessageGlobal("{0} has joined Countdown!!", p.name);
                if (p.level != Server.Countdown.Map)
                    PlayerActions.ChangeMap(p, "countdown");
                p.playerofcountdown = true;
            } else {
                Player.Message(p, "Sorry, you have already joined!!, to leave please type /countdown leave");
            }
        }
        
        public override void PlayerLeftGame(Player p) {
            p.InCountdown = false;
            p.playerofcountdown = false;
            Players.Remove(p);
            PlayersRemaining.Remove(p);
            UpdatePlayersLeft();
        }
    }

    public enum CountdownGameStatus {
        /// <summary> Countdown is not running. </summary>
        Disabled,
        
        /// <summary> Countdown is running, but no round has been started at all yet. </summary>
        Enabled, 
        
        /// <summary> Timer is counting down to start of round. </summary>
        RoundCountdown,
        
        /// <summary> Round is in progress. </summary>
        RoundInProgress, 
        
        /// <summary> Round has ended. </summary>
        RoundFinished,
    }
}
