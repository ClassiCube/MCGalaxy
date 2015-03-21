/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
using System.IO;
using System.Threading;
namespace MCGalaxy
{
    public sealed class PlayerBot
    {
        public static List<PlayerBot> playerbots = new List<PlayerBot>(64);

        public bool hunt = false;
        public bool kill = false;

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

        public ushort[] pos = new ushort[3] { 0, 0, 0 };
        ushort[] oldpos = new ushort[3] { 0, 0, 0 };
        ushort[] basepos = new ushort[3] { 0, 0, 0 };
        public byte[] rot = new byte[2] { 0, 0 };
        byte[] oldrot = new byte[2] { 0, 0 };

        ushort[] foundPos = new ushort[3] { 0, 0, 0 };
        byte[] foundRot = new byte[2] { 0, 0 };
        bool movement = false;
        public int movementSpeed = 24;
        bool jumping = false;
        int currentjump = 0;

        public System.Timers.Timer botTimer = new System.Timers.Timer(100);
        public System.Timers.Timer moveTimer = new System.Timers.Timer(100 / 24);
        public System.Timers.Timer jumpTimer = new System.Timers.Timer(95);

        #region == constructors ==
        public PlayerBot(string n, Level l)
        {
            Server.s.Log("adding " + n + " bot");
            name = n;
            color = "&1";
            id = FreeId();

            level = l;
            ushort x = (ushort)((0.5 + level.spawnx) * 32);
            ushort y = (ushort)((1 + level.spawny) * 32);
            ushort z = (ushort)((0.5 + level.spawnz) * 32);
            pos = new ushort[3] { x, y, z }; rot = new byte[2] { level.rotx, level.roty };
            GlobalSpawn();
        }
        public PlayerBot(string n, Level l, ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            name = n;
            color = "&1";
            id = FreeId();

            level = l;
            pos = new ushort[3] { x, y, z }; rot = new byte[2] { rotx, roty };
            GlobalSpawn();

            foreach (Player p in Player.players)
            {
                if (p.level == level)
                {
                    Player.SendMessage(p, color + name + Server.DefaultColor + ", the bot, has been added.");
                }
            }

            botTimer.Elapsed += delegate
            {
                int currentNum, foundNum = (32 * 75);
                Random rand = new Random();

                x = (ushort)Math.Round((decimal)pos[0] / (decimal)32);
                y = (ushort)((pos[1] - 33) / 32);
                z = (ushort)Math.Round((decimal)pos[2] / (decimal)32);

                if (kill)
                {
                    foreach (Player p in Player.players)
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

                if (Waypoints.Count < 1)
                {
                    if (hunt)
                        Player.players.ForEach(delegate(Player p)
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
                    movement = false;

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
                            removeBot();
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

                if (!movement)
                {
                    if (rot[0] < 245) rot[0] += 8;
                    else rot[0] = 0;

                    if (rot[1] > 32 && rot[1] < 64) rot[1] = 224;
                    else if (rot[1] > 250) rot[1] = 0;
                    else rot[1] += 4;
                }
            };

            botTimer.Start();

            moveTimer.Elapsed += delegate
            {
                moveTimer.Interval = Server.updateTimer.Interval / movementSpeed;
                if (!movement) return;
                int newNum; Random rand = new Random();

                if ((pos[1] - 19) % 32 != 0 && !jumping)
                {
                    pos[1] = (ushort)((pos[1] + 19) - (pos[1] % 32));
                }

                x = (ushort)Math.Round((decimal)(pos[0] - 16) / (decimal)32);
                y = (ushort)((pos[1] - 64) / 32);
                z = (ushort)Math.Round((decimal)(pos[2] - 16) / (decimal)32);

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
            };
            moveTimer.Start();
        }
        #endregion

        #region ==Input ==
        public void SetPos(ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };
        }
        #endregion

        public void removeBot()
        {
            this.botTimer.Stop();
            GlobalDie();
            PlayerBot.playerbots.Remove(this);
        }

        public void GlobalSpawn()
        {
            Player.players.ForEach(delegate(Player p)   //bots dont need to be informed of other bots here
            {
                if (p.level != level) { return; }
                p.SendSpawn(id, color + name, pos[0], pos[1], pos[2], rot[0], rot[1]);
            });
        }

        public void GlobalDie()
        {
            Server.s.Log("removing " + name + " bot");
            Player.players.ForEach(delegate(Player p)
            {
                if (p.level != level) { return; }
                p.SendDie(id);
            });
            playerbots.Remove(this);        //dont know if this is allowed really calling itself to kind of die
        }

        public void Update()
        {
            //pos[0] += 1;
        }

        void UpdatePosition()   //Im going to avoid touching this unless nessesary
        {

            //pingDelayTimer.Stop();

            // Shameless copy from JTE's Server
            byte changed = 0;
            if (oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2]) { changed |= 1; }
            if (oldrot[0] != rot[0] || oldrot[1] != rot[1]) { changed |= 2; }
            if (Math.Abs(pos[0] - basepos[0]) > 32 || Math.Abs(pos[1] - basepos[1]) > 32 ||
                Math.Abs(pos[2] - basepos[2]) > 32) { changed |= 4; }
            if ((oldpos[0] == pos[0] && oldpos[1] == pos[1] && oldpos[2] == pos[2]) &&
                (basepos[0] != pos[0] || basepos[1] != pos[1] || basepos[2] != pos[2])) { changed |= 4; }
            byte[] buffer = new byte[0]; byte msg = 0;
            if ((changed & 4) != 0)
            {
                msg = 8; buffer = new byte[9]; buffer[0] = id;
                HTNO(pos[0]).CopyTo(buffer, 1);
                HTNO(pos[1]).CopyTo(buffer, 3);
                HTNO(pos[2]).CopyTo(buffer, 5);
                buffer[7] = rot[0]; buffer[8] = rot[1];
            }
            else if (changed == 1)
            {
                msg = 10; buffer = new byte[4]; buffer[0] = id;
                buffer[1] = (byte)(pos[0] - oldpos[0]);
                buffer[2] = (byte)(pos[1] - oldpos[1]);
                buffer[3] = (byte)(pos[2] - oldpos[2]);
            }
            else if (changed == 2)
            {
                msg = 11; buffer = new byte[3]; buffer[0] = id;
                buffer[1] = rot[0]; buffer[2] = rot[1];
            }
            else if (changed == 3)
            {
                msg = 9; buffer = new byte[6]; buffer[0] = id;
                buffer[1] = (byte)(pos[0] - oldpos[0]);
                buffer[2] = (byte)(pos[1] - oldpos[1]);
                buffer[3] = (byte)(pos[2] - oldpos[2]);
                buffer[4] = rot[0]; buffer[5] = rot[1];
            }
            try
            {
                if (changed != 0) foreach (Player p in Player.players)
                    {
                        if (p.level == level)
                        {
                            p.SendRaw(msg, buffer);
                        }
                    }
            } catch { }
            oldpos = pos;
            oldrot = rot;
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
            } unchecked { return (byte)-1; }
        }
        public static PlayerBot Find(string name)
        {
            PlayerBot tempPlayer = null; bool returnNull = false;

            foreach (PlayerBot pB in PlayerBot.playerbots)
            {
                if (pB.name.ToLower() == name.ToLower()) return pB;
                if (pB.name.ToLower().IndexOf(name.ToLower()) != -1)
                {
                    if (tempPlayer == null) tempPlayer = pB;
                    else returnNull = true;
                }
            }

            if (returnNull == true) return null;
            if (tempPlayer != null) return tempPlayer;
            return null;
        }
        public static bool ValidName(string name)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890_";
            foreach (char ch in name) { if (allowedchars.IndexOf(ch) == -1) { return false; } } return true;
        }
        #endregion

        #region == True Global ==
        public static void GlobalUpdatePosition()
        {
            playerbots.ForEach(delegate(PlayerBot b) { b.UpdatePosition(); });
        }
        public static void GlobalUpdate()
        {
            while (true)
            {
                Thread.Sleep(100);
                playerbots.ForEach(delegate(PlayerBot b) { b.Update(); });
            }
        }
        #endregion
        #region == Host <> Network ==
        byte[] HTNO(ushort x)       //Is used currently, the rest are not and may not be needed at all
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }
        ushort NTHO(byte[] x, int offset)
        {
            byte[] y = new byte[2];
            Buffer.BlockCopy(x, offset, y, 0, 2); Array.Reverse(y);
            return BitConverter.ToUInt16(y, 0);
        }
        byte[] HTNO(short x)
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }
        #endregion
    }
}