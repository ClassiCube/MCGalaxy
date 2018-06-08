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
    
    public sealed partial class CountdownGame : IGame {
        
        public VolatileArray<Player> Players = new VolatileArray<Player>();
        public VolatileArray<Player> Remaining = new VolatileArray<Player>();
        public CountdownGameStatus Status = CountdownGameStatus.Disabled;
        public override bool Running { get { return Status != CountdownGameStatus.Disabled; } }
        public override string GameName { get { return "Countdown"; } }
        
        public bool FreezeMode;
        public int Interval;
        public string SpeedType;
        
        public void Enable(Player p) {
            HookEventHandlers();
            CmdLoad.LoadLevel(null, "countdown");
            Map = LevelInfo.FindExact("countdown");
            
            if (Map == null) {
                Player.Message(p, "Countdown level not found, generating..");
                GenerateMap(p, 32, 32, 32);
                Map = LevelInfo.FindExact("countdown");
            }
            
            bulk.level = Map;
            Status = CountdownGameStatus.Enabled;
            Chat.MessageGlobal("Countdown has been enabled!");
        }

        public override void End() {
            if (Status == CountdownGameStatus.RoundInProgress) EndRound(null);            
            Status = CountdownGameStatus.Disabled;
            UnhookEventHandlers();
            
            Map.Message("Countdown was disabled.");
            Players.Clear();
            Remaining.Clear();
            squaresLeft.Clear();
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
            
            Position pos = new Position(16 + 8 * 32, 32 + 23 * 32, 16 + 17 * 32);
            p.SendPos(Entities.SelfID, pos, p.Rot);
        }
        
        public void ResetMap() {
            SetGlassTube(Block.Air, Block.Air);

            int maxX = Map.Width - 1, maxZ = Map.Length - 1;
            Cuboid(4, 4, 4, maxX - 4, 4, maxZ - 4, Block.Glass);
            for(int zz = 6; zz < maxZ - 6; zz += 3)
                for (int xx = 6; xx < maxX - 6; xx += 3)
            {
                Cuboid(xx, 4, zz, xx + 1, 4, zz + 1, Block.Green);
            }
            
            bulk.Send(true);
            Map.Message("Countdown map has been reset");
        }
        
        
        void SetGlassTube(BlockID block, BlockID floorBlock) {
            int midX = Map.Width / 2, midY = Map.Height / 2, midZ = Map.Length / 2;
            Cuboid(midX - 1, midY + 1, midZ - 2, midX, midY + 2, midZ - 2, block);
            Cuboid(midX - 1, midY + 1, midZ + 1, midX, midY + 2, midZ + 1, block);
            Cuboid(midX - 2, midY + 1, midZ - 1, midX - 2, midY + 2, midZ, block);
            Cuboid(midX + 1, midY + 1, midZ - 1, midX + 1, midY + 2, midZ, block);
            Cuboid(midX - 1, midY, midZ - 1, midX, midY, midZ, floorBlock);
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
}
