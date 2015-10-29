/*
	Copyright 2011 MCGalaxy
	
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
using System.Threading;
namespace MCGalaxy.Commands
{
    public sealed class CmdFly : Command
    {
        public override string name { get { return "fly"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdFly() { }

        public override void Use(Player p, string message)
        {
            Command CmdFly = Command.all.Find("fly");
            if (p.level.ctfmode)
            {
                Player.SendMessage(p, "You can not fly while playing CTF, that is cheating!");
                if (p.isFlying) CmdFly.Use(p, String.Empty);
                return;
            }
            p.isFlying = !p.isFlying;
            if (!p.isFlying) return;

            Player.SendMessage(p, "You are now flying. &cJump!");

            Thread flyThread = new Thread(new ThreadStart(delegate
            {
                Pos pos;
                ushort[] oldpos = new ushort[3];
                List<Pos> buffer = new List<Pos>();
                while (p.isFlying)
                {
                    Thread.Sleep(20);
                    if (p.pos == oldpos) continue;
                    try
                    {
                        List<Pos> tempBuffer = new List<Pos>();
                        List<Pos> toRemove = new List<Pos>();
                        ushort x = (ushort)((p.pos[0]) / 32);
                        ushort y = (ushort)((p.pos[1] - 60) / 32);
                        ushort z = (ushort)((p.pos[2]) / 32);

                        try
                        {
                            for (ushort xx = (ushort)(x - 1); xx <= x + 1; xx++)
                            {
                                for (ushort yy = (ushort)(y - 1); yy <= y; yy++)
                                {
                                    for (ushort zz = (ushort)(z - 1); zz <= z + 1; zz++)
                                    {
                                        if (p.level.GetTile(xx, yy, zz) == Block.air)
                                        {
                                            pos.x = xx; pos.y = yy; pos.z = zz;
                                            tempBuffer.Add(pos);
                                        }
                                    }
                                }
                            }
                            foreach (Pos cP in tempBuffer)
                            {
                                if (!buffer.Contains(cP))
                                {
                                    buffer.Add(cP);
                                    p.SendBlockchange(cP.x, cP.y, cP.z, Block.glass);
                                }
                            }
                            foreach (Pos cP in buffer)
                            {
                                if (!tempBuffer.Contains(cP))
                                {
                                    p.SendBlockchange(cP.x, cP.y, cP.z, Block.air);
                                    toRemove.Add(cP);
                                }
                            }
                            foreach (Pos cP in toRemove)
                            {
                                buffer.Remove(cP);
                            }
                            tempBuffer.Clear();
                            toRemove.Clear();
                        }
                        catch { }
                    }
                    catch { }
                    p.pos.CopyTo(oldpos, 0);
                }

                foreach (Pos cP in buffer)
                {
                    p.SendBlockchange(cP.x, cP.y, cP.z, Block.air);
                }

                Player.SendMessage(p, "Stopped flying");
            }));
            flyThread.Start();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/fly - The old method of flight before custom clients.");
            Player.SendMessage(p, "May not work at all depending on your connection.");
        }

        struct Pos { public ushort x, y, z; }
    }
}