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

namespace MCGalaxy {
    
    public sealed class PlayerBot {
        
        public static List<PlayerBot> playerbots = new List<PlayerBot>(64);

        public bool hunt = false, kill = false;

        public string AIName = "";
        public string name;
        public byte id;
        public string color;
        public Level level;
        public int currentPoint = 0;
        public int countdown = 0;
        public bool nodUp = false;
        public List<Pos> Waypoints = new List<Pos>();
        public struct Pos { public string type, newscript; public int seconds, rotspeed; public ushort x, y, z; public byte rotx, roty; }

        public ushort[] pos = new ushort[3], oldpos = new ushort[3], foundPos = new ushort[3];
        public byte[] rot = new byte[2], oldrot = new byte[2], foundRot = new byte[2];
        bool movement = false;
        public int movementSpeed = 24;
        bool jumping = false;
        int currentjump = 0;

        System.Timers.Timer botTimer = new System.Timers.Timer(100);
        System.Timers.Timer moveTimer = new System.Timers.Timer(100 / 24);
        System.Timers.Timer jumpTimer = new System.Timers.Timer(95);
        static readonly object botsLock = new object();

        #region == constructors ==
        public PlayerBot(string n, Level lvl, ushort x, ushort y, ushort z, byte rotx, byte roty) {
            name = n;
            color = "&1";
            id = FreeId();
            
            level = lvl;
            SetPos(x, y, z, rotx, roty);

            botTimer.Elapsed += BotTimerFunc;
            botTimer.Start();
            moveTimer.Elapsed += MoveTimerFunc;
            moveTimer.Start();
        }
        
        void BotTimerFunc(object sender, ElapsedEventArgs e) {
            int currentNum, foundNum = (32 * 75);
            Random rand = new Random();
            ushort x = (ushort)Math.Round((decimal)pos[0] / 32);
            ushort y = (ushort)((pos[1] - 33) / 32);
            ushort z = (ushort)Math.Round((decimal)pos[2] / 32);

            if (kill)
            {
                foreach (Player p in PlayerInfo.players)
                {
                    if ((ushort)(p.pos[0] / 32) == x)
                    {
                        if (Math.Abs((ushort)(p.pos[1] / 32) - y) < 2)
                        {
                            if ((ushort)(p.pos[2] / 32) == z)
                            {
                                p.HandleDeath(Block.Zero);
                            }
                        }
                    }
                }
            }
            movement = false;

            if (Waypoints.Count == 0)
            {
                if (hunt)
                    PlayerInfo.players.ForEach(delegate(Player p)
                                           {
                                               if (p.level == level && !p.invincible)
                                               {
                                                   currentNum = Math.Abs(p.pos[0] - pos[0]) + Math.Abs(p.pos[1] - pos[1]) + Math.Abs(p.pos[2] - pos[2]);
                                                   if (currentNum < foundNum)
                                                   {
                                                       foundNum = currentNum;
                                                       foundPos = p.pos;
                                                       foundRot = p.rot;
                                                       movement = true;
                                                       rot[1] = (byte)(255 - foundRot[1]);
                                                       if (foundRot[0] < 128) rot[0] = (byte)(foundRot[0] + 128);
                                                       else rot[0] = (byte)(foundRot[0] - 128);
                                                   }
                                               }
                                           });
            }
            else
            {
                bool skip = false;
                

                retry: switch (Waypoints[currentPoint].type)
                {
                    case "walk":
                        foundPos[0] = Waypoints[currentPoint].x;
                        foundPos[1] = Waypoints[currentPoint].y;
                        foundPos[2] = Waypoints[currentPoint].z;
                        movement = true;

                        if ((ushort)(pos[0] / 32) == (ushort)(Waypoints[currentPoint].x / 32))
                        {
                            if ((ushort)(pos[2] / 32) == (ushort)(Waypoints[currentPoint].z / 32))
                            {
                                rot[0] = Waypoints[currentPoint].rotx;
                                rot[1] = Waypoints[currentPoint].roty;
                                currentPoint++;
                                movement = false;

                                if (currentPoint == Waypoints.Count) currentPoint = 0;
                                if (!skip) { skip = true; goto retry; }
                            }
                        }
                        break;
                    case "teleport":
                        pos[0] = Waypoints[currentPoint].x;
                        pos[1] = Waypoints[currentPoint].y;
                        pos[2] = Waypoints[currentPoint].z;
                        rot[0] = Waypoints[currentPoint].rotx;
                        rot[1] = Waypoints[currentPoint].roty;
                        currentPoint++;
                        if (currentPoint == Waypoints.Count) currentPoint = 0;
                        return;
                    case "wait":
                        if (countdown != 0)
                        {
                            countdown--;
                            if (countdown == 0)
                            {
                                currentPoint++;
                                if (currentPoint == Waypoints.Count) currentPoint = 0;
                                if (!skip) { skip = true; goto retry; }
                            }
                        }
                        else
                        {
                            countdown = Waypoints[currentPoint].seconds;
                        }
                        return;
                    case "nod":
                        if (countdown != 0)
                        {
                            countdown--;

                            if (nodUp)
                            {
                                if (rot[1] > 32 && rot[1] < 128) nodUp = !nodUp;
                                else
                                {
                                    if (rot[1] + (byte)Waypoints[currentPoint].rotspeed > 255) rot[1] = 0;
                                    else rot[1] += (byte)Waypoints[currentPoint].rotspeed;
                                }
                            }
                            else
                            {
                                if (rot[1] > 128 && rot[1] < 224) nodUp = !nodUp;
                                else
                                {
                                    if (rot[1] - (byte)Waypoints[currentPoint].rotspeed < 0) rot[1] = 255;
                                    else rot[1] -= (byte)Waypoints[currentPoint].rotspeed;
                                }
                            }

                            if (countdown == 0)
                            {
                                currentPoint++;
                                if (currentPoint == Waypoints.Count) currentPoint = 0;
                                if (!skip) { skip = true; goto retry; }
                            }
                        }
                        else
                        {
                            countdown = Waypoints[currentPoint].seconds;
                        }
                        return;
                    case "spin":
                        if (countdown != 0)
                        {
                            countdown--;

                            if (rot[0] + (byte)Waypoints[currentPoint].rotspeed > 255) rot[0] = 0;
                            else if (rot[0] + (byte)Waypoints[currentPoint].rotspeed < 0) rot[0] = 255;
                            else rot[0] += (byte)Waypoints[currentPoint].rotspeed;

                            if (countdown == 0)
                            {
                                currentPoint++;
                                if (currentPoint == Waypoints.Count) currentPoint = 0;
                                if (!skip) { skip = true; goto retry; }
                            }
                        }
                        else
                        {
                            countdown = Waypoints[currentPoint].seconds;
                        }
                        return;
                    case "speed":
                        movementSpeed = (int)Math.Round((decimal)((decimal)24 / (decimal)100 * (decimal)Waypoints[currentPoint].seconds));
                        if (movementSpeed == 0) movementSpeed = 1;

                        currentPoint++;
                        if (currentPoint == Waypoints.Count) currentPoint = 0;
                        if (!skip) { skip = true; goto retry; }
                        return;
                    case "reset":
                        currentPoint = 0;
                        return;
                    case "remove":
                        PlayerBot.Remove(this);
                        return;
                    case "linkscript":
                        if (File.Exists("bots/" + Waypoints[currentPoint].newscript))
                        {
                            Command.all.Find("botset").Use(null, this.name + " " + Waypoints[currentPoint].newscript);
                            return;
                        }

                        currentPoint++;
                        if (currentPoint == Waypoints.Count) currentPoint = 0;
                        if (!skip) { skip = true; goto retry; }
                        return;
                    case "jump":
                        jumpTimer.Elapsed += delegate
                        {
                            currentjump++;
                            switch (currentjump)
                            {
                                case 1:
                                    case 2: pos[1] += 24; break;
                                    case 3: break;
                                    case 4: pos[1] -= 24; break;
                                    case 5: pos[1] -= 24; jumping = false; currentjump = 0; jumpTimer.Stop(); break;
                            }
                        };
                        jumpTimer.Start();

                        currentPoint++;
                        if (currentPoint == Waypoints.Count) currentPoint = 0;
                        if (!skip) { skip = true; goto retry; }
                        break;
                }

                if (currentPoint == Waypoints.Count) currentPoint = 0;
            }

            if (!movement && Waypoints.Count > 0)
            {
                if (rot[0] < 245) rot[0] += 8;
                else rot[0] = 0;

                if (rot[1] > 32 && rot[1] < 64) rot[1] = 224;
                else if (rot[1] > 250) rot[1] = 0;
                else rot[1] += 4;
            }
        }
        
        void MoveTimerFunc(object sender, ElapsedEventArgs e) {
            moveTimer.Interval = Server.updateTimer.Interval / movementSpeed;
            if (!movement) return;
            int newNum; Random rand = new Random();

            if ((pos[1] - 19) % 32 != 0 && !jumping)
            {
                pos[1] = (ushort)((pos[1] + 19) - (pos[1] % 32));
            }

            ushort x = (ushort)Math.Round((decimal)(pos[0] - 16) / 32);
            ushort y = (ushort)((pos[1] - 64) / 32);
            ushort z = (ushort)Math.Round((decimal)(pos[2] - 16) / 32);

            byte b = Block.Convert(level.GetTile(x, y, z));
            byte b1, b2, b3;//, b4;

            if (Block.Walkthrough(b) && !jumping)
            {
                pos[1] = (ushort)(pos[1] - 32);
            }

            y = (ushort)((pos[1] - 64) / 32);   //Block below feet

            newNum = level.PosToInt((ushort)(x + Math.Sign(foundPos[0] - pos[0])), y, (ushort)(z + Math.Sign(foundPos[2] - pos[2])));
            b = Block.Convert(level.GetTile(newNum));
            b1 = Block.Convert(level.GetTile(level.IntOffset(newNum, 0, 1, 0)));
            b2 = Block.Convert(level.GetTile(level.IntOffset(newNum, 0, 2, 0)));
            b3 = Block.Convert(level.GetTile(level.IntOffset(newNum, 0, 3, 0)));

            if (Block.Walkthrough(b2) && Block.Walkthrough(b3) && !Block.Walkthrough(b1))
            {     //Get ready to go up step
                pos[0] += (ushort)Math.Sign(foundPos[0] - pos[0]);
                pos[1] += (ushort)32;
                pos[2] += (ushort)Math.Sign(foundPos[2] - pos[2]);
            }
            else if (Block.Walkthrough(b1) && Block.Walkthrough(b2))
            {                        //Stay on current level
                pos[0] += (ushort)Math.Sign(foundPos[0] - pos[0]);
                pos[2] += (ushort)Math.Sign(foundPos[2] - pos[2]);
            }
            else if (Block.Walkthrough(b) && Block.Walkthrough(b1))
            {                         //Drop a level
                pos[0] += (ushort)Math.Sign(foundPos[0] - pos[0]);
                pos[1] -= (ushort)32;
                pos[2] += (ushort)Math.Sign(foundPos[2] - pos[2]);
            }

            x = (ushort)Math.Round((decimal)(pos[0] - 16) / (decimal)32);
            y = (ushort)((pos[1] - 64) / 32);
            z = (ushort)Math.Round((decimal)(pos[2] - 16) / (decimal)32);

            b1 = Block.Convert(level.GetTile(x, (ushort)(y + 1), z));
            b2 = Block.Convert(level.GetTile(x, (ushort)(y + 2), z));
            b3 = Block.Convert(level.GetTile(x, y, z));

            /*
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
        
        public static void Add(PlayerBot bot) {
            lock (botsLock)
                playerbots.Add(bot);        
            bot.GlobalSpawn();
            
            foreach (Player p in PlayerInfo.players) {
                if (p.level == bot.level)
                    Player.SendMessage(p, bot.color + bot.name + "%S, the bot, has been added.");
            }
        }

        public static void Remove(PlayerBot bot) {
            lock (botsLock)
                playerbots.Remove(bot);
            bot.GlobalDespawn();    
            
            bot.botTimer.Stop();
            bot.moveTimer.Stop();
            bot.jumpTimer.Stop();
        }
        
        public static void RemoveAllFromLevel(Level lvl) {
            lock (botsLock)
                RemoveAll(lvl);
        }
        
        static void RemoveAll(Level lvl) {
            for (int i = 0; i < PlayerBot.playerbots.Count; i++) {
                PlayerBot bot = PlayerBot.playerbots[i];
                if (PlayerBot.playerbots[i].level == lvl) {
                    Remove(bot);
                    i--;
                }
            }
        }

        public void GlobalSpawn() {
            PlayerInfo.players.ForEach(p => {
                                       if (p.level == level)
                                           p.SendSpawn(id, color + name, pos[0], pos[1], pos[2], rot[0], rot[1]);
                                   });
        }

        public void GlobalDespawn() {
            PlayerInfo.players.ForEach(p => {
                                       if (p.level == level)
                                           p.SendDespawn(id);
                                   });
        }

        public void Update() { }

        void UpdatePosition() {
            byte[] packet = NetUtils.GetPositionPacket(id, pos, oldpos, rot, oldrot, rot[1], true);
            oldpos = pos; oldrot = rot;
            if (packet == null) return;
            
            try {
                foreach (Player p in PlayerInfo.players) {
                    if (p.level == level)
                        p.SendRaw(packet);
                }
            } catch { }
        }

        #region == Misc ==
        static byte FreeId()
        {
            for (byte i = 64; i < 128; ++i)
            {
                foreach (PlayerBot b in playerbots)
                {
                    if (b.id == i) { goto Next; }
                } return i;
                Next: continue;
            }
            return 0xFF;
        }
        
        public static PlayerBot Find(string name) {
            PlayerBot match = null; int matches = 0;
            name = name.ToLower();

            foreach (PlayerBot pB in PlayerBot.playerbots) {
                if (pB.name.ToLower() == name) return pB;
                if (pB.name.ToLower().Contains(name)) {
                    match = pB; matches++;
                }
            }
            return matches == 1 ? match : null;
        }
        #endregion

        public static void GlobalUpdatePosition() {
            playerbots.ForEach(
                delegate(PlayerBot b) { b.UpdatePosition(); }
            );
        }
        
        public static void GlobalUpdate() {
            while (true) {
                Thread.Sleep(100);
                playerbots.ForEach(delegate(PlayerBot b) { b.Update(); });
            }
        }
    }
}