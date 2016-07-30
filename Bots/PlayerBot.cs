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
using System.IO;
using System.Threading;
using System.Timers;
using MCGalaxy.Bots;

namespace MCGalaxy {
    
    public sealed class PlayerBot {
        
        [Obsolete("Use PlayerBot.Bots.Items instead")]
        public static List<PlayerBot> playerbots;
        public static VolatileArray<PlayerBot> Bots = new VolatileArray<PlayerBot>(true);

        public bool hunt = false, kill = false;

        public string AIName = "", color, model = "humanoid";
        public string name, SkinName, DisplayName;
        public string ColoredName { get { return color + DisplayName; } }
        
        public byte id;
        public Level level;
        public int cur = 0;
        public int countdown = 0;
        public bool nodUp = false;
        public List<Pos> Waypoints = new List<Pos>();
        public struct Pos { public string type, newscript; public int seconds, rotspeed; public ushort x, y, z; public byte rotx, roty; }

        public ushort[] pos = new ushort[3], oldpos = new ushort[3], foundPos = new ushort[3];
        public byte[] rot = new byte[2], oldrot = new byte[2], foundRot = new byte[2];
        public bool movement = false;
        public int movementSpeed = 3;
        internal bool jumping = false;
        internal int currentjump = 0;

        System.Timers.Timer botTimer = new System.Timers.Timer(100);
        internal System.Timers.Timer jumpTimer = new System.Timers.Timer(95);

        public PlayerBot(string n, Level lvl, ushort x, ushort y, ushort z, byte rotx, byte roty) {
            name = n;
            DisplayName = n;
            SkinName = n;
            color = "&1";
            id = FreeId();
            
            level = lvl;
            SetPos(x, y, z, rotx, roty);

            botTimer.Elapsed += BotTimerFunc;
            botTimer.Start();
        }
        
        #region Script handling
        
        void BotTimerFunc(object sender, ElapsedEventArgs e) {
            if (kill) Instructions.DoKill(this);
            movement = false;

            if (Waypoints.Count == 0) {
                if (hunt) Instructions.DoHunt(this);
            } else {
                bool doNextInstruction = !DoInstruction();
                if (cur == Waypoints.Count) cur = 0;
                
                if (!doNextInstruction) return;
                DoInstruction();
                if (cur == Waypoints.Count) cur = 0;
            }
        }
        
        bool DoInstruction() {
            Func<PlayerBot, bool> instruction;
            if (!Instructions.Defined.TryGetValue(Waypoints[cur].type, out instruction))
                return false;
            return instruction(this);
        }
        
        public void AdvanceRotation() {
            if (!movement && Waypoints.Count > 0) {
                if (rot[0] < 245) rot[0] += 8;
                else rot[0] = 0;

                if (rot[1] > 32 && rot[1] < 64) rot[1] = 224;
                else if (rot[1] > 250) rot[1] = 0;
                else rot[1] += 4;
            }
        }
        
        public void NextInstruction() {
            cur++;
            if (cur == Waypoints.Count) cur = 0;
        }
        
        void DoMove() {
            if ((pos[1] - 19) % 32 != 0 && !jumping)
                pos[1] = (ushort)((pos[1] + 19) - (pos[1] % 32));

            ushort x = (ushort)Math.Round((decimal)(pos[0] - 16) / 32);
            ushort y = (ushort)((pos[1] - 64) / 32);
            ushort z = (ushort)Math.Round((decimal)(pos[2] - 16) / 32);
            int dx = Math.Sign(foundPos[0] - pos[0]), dz = Math.Sign(foundPos[2] - pos[2]);

            byte b = Block.Convert(level.GetTile(x, y, z));
            byte b1, b2, b3;

            if (Block.Walkthrough(b) && !jumping)
                pos[1] = (ushort)(pos[1] - 32);
            y = (ushort)((pos[1] - 64) / 32);   //Block below feet

            int index = level.PosToInt((ushort)(x + dx), y, (ushort)(z + dz));
            b = Block.Convert(level.GetTile(index));
            b1 = Block.Convert(level.GetTile(level.IntOffset(index, 0, 1, 0)));
            b2 = Block.Convert(level.GetTile(level.IntOffset(index, 0, 2, 0)));
            b3 = Block.Convert(level.GetTile(level.IntOffset(index, 0, 3, 0)));

            if (Block.Walkthrough(b2) && Block.Walkthrough(b3) && !Block.Walkthrough(b1)) {
                pos[0] += (ushort)dx; // Get ready to go up step
                pos[1] += (ushort)32;
                pos[2] += (ushort)dz;
            } else if (Block.Walkthrough(b1) && Block.Walkthrough(b2)) {
                pos[0] += (ushort)dx; // Stay on current level
                pos[2] += (ushort)dz;
            } else if (Block.Walkthrough(b) && Block.Walkthrough(b1)) {
                pos[0] += (ushort)dx; // Drop a level
                pos[1] -= (ushort)32;
                pos[2] += (ushort)dz;
            }

            /*x = (ushort)Math.Round((decimal)(pos[0] - 16) / (decimal)32);
            y = (ushort)((pos[1] - 64) / 32);
            z = (ushort)Math.Round((decimal)(pos[2] - 16) / (decimal)32);

            b1 = Block.Convert(level.GetTile(x, (ushort)(y + 1), z));
            b2 = Block.Convert(level.GetTile(x, (ushort)(y + 2), z));
            b3 = Block.Convert(level.GetTile(x, y, z));

            
                if ((ushort)(foundPos[1] / 32) > y) {
                    if (b1 == Block.water || b1 == Block.waterstill || b1 == Block.lava || b1 == Block.lavastill) {
                        if (Block.Walkthrough(b2)) {
                            pos[1] = (ushort)(pos[1] + (Math.Sign(foundPos[1] - pos[1])));
                        }
                    } else if (b2 == Block.water || b2 == Block.waterstill || b2 == Block.lava || b2 == Block.lavastill) {
                        pos[1] = (ushort)(pos[1] + (Math.Sign(foundPos[1] - pos[1])));
                    }
                } else if ((ushort)(foundPos[1] / 32) < y) {
                    if (b3 == Block.water || b3 == Block.waterstill || b3 == Block.lava || b3 == Block.lavastill) {
                        pos[1] = (ushort)(pos[1] + (Math.Sign(foundPos[1] - pos[1])));
                    }
                }*/
        }
        #endregion

        public void SetPos(ushort x, ushort y, ushort z, byte rotx, byte roty) {
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };
        }
        
        public static void Add(PlayerBot bot, bool save = true) {
            Bots.Add(bot);
            bot.GlobalSpawn();
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == bot.level)
                    Player.Message(p, bot.color + bot.name + "%S, the bot, has been added.");
            }
            if (save)
                BotsFile.UpdateBot(bot);
        }

        public static void Remove(PlayerBot bot, bool save = true) {
            Bots.Remove(bot);
            bot.GlobalDespawn();
            
            bot.botTimer.Stop();
            bot.jumpTimer.Stop();
            if (save)
                BotsFile.RemoveBot(bot);
        }
        
        public static void UnloadFromLevel(Level lvl) {
            BotsFile.UnloadBots(lvl);
            RemoveAll(lvl, false);
        }
        
        public static void RemoveAllFromLevel(Level lvl) {
            RemoveAll(lvl, true);
            BotsFile.RemoveLevelBots(lvl.name);
        }
        
        static void RemoveAll(Level lvl, bool save) {
            PlayerBot[] bots = Bots.Items;
            for (int i = 0; i < bots.Length; i++) {
                PlayerBot bot = bots[i];
                if (bots[i].level == lvl) Remove(bot, save);
            }
        }

        public void GlobalSpawn() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == level) Entities.Spawn(p, this);
            }
        }

        public void GlobalDespawn() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == level) Entities.Despawn(p, id);
            }
        }

        void UpdatePosition() {
            if (movement) {
                for (int i = 0; i < movementSpeed; i++)
                    DoMove();
            }
            
            byte[] packet = Entities.GetPositionPacket(this);
            oldpos = pos; oldrot = rot;
            if (packet == null) return;

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
                if (p.level == level) p.SendRaw(packet);
        }

        
        static byte FreeId() {
            for (byte i = 127; i >= 64; i--) {
                foreach (PlayerBot b in playerbots) {
                    if (b.id == i) { goto Next; }
                } return i;
                Next: continue;
            }
            return 0xFF;
        }
        
        public static PlayerBot Find(string name) {
            PlayerBot match = null; int matches = 0;
            name = name.ToLower();
            PlayerBot[] bots = Bots.Items;

            foreach (PlayerBot bot in bots) {
                if (bot.name.ToLower() == name) return bot;
                if (bot.name.ToLower().Contains(name)) {
                    match = bot; matches++;
                }
            }
            return matches == 1 ? match : null;
        }
        
        public static PlayerBot FindMatches(Player pl, string name) {
            int matches = 0;
            return Extensions.FindMatches<PlayerBot>(pl, name, out matches, Bots.Items,
                                                     b => true, b => b.name, "bots");
        }

        public static void GlobalUpdatePosition() {
            PlayerBot[] bots = Bots.Items;
            foreach (PlayerBot b in bots) b.UpdatePosition();
        }
    }
}