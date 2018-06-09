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

    class CountdownLevelPicker : LevelPicker {
        public override List<string> GetCandidateMaps() { return new List<string>() { "countdown" }; }
    }
    
    public sealed partial class CountdownGame : RoundsGame {        
        public VolatileArray<Player> Players = new VolatileArray<Player>();
        public VolatileArray<Player> Remaining = new VolatileArray<Player>();
        public override string GameName { get { return "Countdown"; } }
        
        public bool FreezeMode;
        public int Interval;
        public string SpeedType;
        
        public CountdownGame() {
            Picker = new CountdownLevelPicker();
        }
        
        protected override List<Player> GetPlayers() {
            List<Player> playing = new List<Player>();
            playing.AddRange(Players.Items);
            return playing;
        }
        
        public override void OutputStatus(Player p) {
            Player[] players = Players.Items;            
            Player.Message(p, "Players in countdown:");
            
            if (RoundInProgress) {               
                Player.Message(p, players.Join(pl => FormatPlayer(pl)));
            } else {
                Player.Message(p, players.Join(pl => pl.ColoredName));
            }
            
            Player.Message(p, squaresLeft.Count + " squares left");
        }
        
        string FormatPlayer(Player pl) {
            string suffix = Remaining.Contains(pl) ? " &a[IN]" : " &c[OUT]";
            return pl.ColoredName + suffix;
        }
        
        public override void Start(Player p, string map, int rounds) {
            map = "countdown";
            if (!LevelInfo.MapExists(map)) {
                Player.Message(p, "Countdown level not found, generating..");
                GenerateMap(p, 32, 32, 32);
            }
            
            if (!SetMap(map)) {
                Player.Message(p, "Failed to load initial map!"); return;
            }
            
            bulk.level = Map;
            Chat.MessageGlobal("Countdown has been enabled!");
            
            RoundsLeft = rounds;
            Running = true;
            HookEventHandlers();
            
            Thread t = new Thread(RunGame);
            t.Name = "MCG_CountdownGame";
            t.Start();
        }

        public override void End() {
            if (!Running) return;
            Running = false;
            
            if (RoundInProgress) EndRound(null);
            UnhookEventHandlers();
            
            Players.Clear();
            Remaining.Clear();
            squaresLeft.Clear();
            EndCommon();
        }
        
        public void GenerateMap(Player p, int width, int height, int length) {
            Level lvl = CountdownMapGen.Generate(width, height, length);
            Level cur = LevelInfo.FindExact("countdown");
            if (cur != null) LevelActions.Replace(cur, lvl);
            else LevelInfo.Add(lvl);
            
            lvl.Save();
            Map = lvl;
            
            const string format = "Generated map ({0}x{1}x{2}), sending you to it..";
            Player.Message(p, format, width, height, length);
            PlayerActions.ChangeMap(p, "countdown");
            
            Position pos = Position.FromFeetBlockCoords(8, 23, 17);
            p.SendPos(Entities.SelfID, pos, p.Rot);
        }
        
        void ResetBoard() {
            SetBoardOpening(Block.Glass);
            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            Cuboid(4, 4, 4, maxX - 4, 4, maxZ - 4, Block.Glass);          
            squaresLeft.Clear();
            
            for (int zz = 6; zz < Map.Length - 6; zz += 3)
                for (int xx = 6; xx < Map.Width - 6; xx += 3)
            {
                Cuboid(xx, 4, zz, xx + 1, 4, zz + 1, Block.Green);
                squaresLeft.Add(new SquarePos(xx, zz));
            }
            
            bulk.Send(true);
        }        
        
        void SetBoardOpening(BlockID block) {
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, block);
            bulk.Send(true);
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
        
        struct SquarePos {
            public ushort X, Z;
            public SquarePos(int x, int z) { X = (ushort)x; Z = (ushort)z; }
        }
        
        
        public override void PlayerJoinedGame(Player p) {
            if (!Players.Contains(p)) {
                Players.Add(p);
                Player.Message(p, "You've joined countdown!");
                Chat.MessageFrom(p, "λNICK %Sjoined countdown!");
                if (p.level != Map) PlayerActions.ChangeMap(p, "countdown");
            } else {
                Player.Message(p, "You've already joined countdown. To leave, go to another map.");
            }
        }
        
        public override void PlayerLeftGame(Player p) {
            Player.Message(p, "You've left countdown.");
            Players.Remove(p);
            Remaining.Remove(p);
            UpdatePlayersLeft();
        }
    }
}
