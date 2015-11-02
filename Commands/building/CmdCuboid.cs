/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Linq;

namespace MCGalaxy.Commands
{
    public sealed class CmdCuboid : Command
    {
        public override string name { get { return "cuboid"; } }

        public override string shortcut { get { return "z"; } }

        public override string type { get { return CommandTypes.Building; } }

        public override bool museumUsable { get { return false; } }

        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public CmdCuboid()
        {
        }

        public static byte wait;

        public override void Use(Player p, string message)
        {
            int number = 0;
            string msg = String.Empty;
            try
            {
                number = message.Split(' ').Length;
            } catch
            {
            }
            wait = 0;
            if (number > 2)
            {
                Help(p);
                wait = 1;
                return;
            }
            if (number == 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                byte type = Block.Byte(t);
                if (type == 255)
                {
                    Player.SendMessage(p, "There is no block \"" + t + "\".");
                    wait = 1;
                    return;
                }

                if (!Block.canPlace(p, type))
                {
                    Player.SendMessage(p, "Cannot place that.");
                    wait = 1;
                    return;
                }

                SolidType solid;
                if (s == "solid")
                {
                    solid = SolidType.solid;
                } else if (s == "hollow")
                {
                    solid = SolidType.hollow;
                } else if (s == "walls")
                {
                    solid = SolidType.walls;
                } else if (s == "holes")
                {
                    solid = SolidType.holes;
                } else if (s == "wire")
                {
                    solid = SolidType.wire;
                } else if (s == "random")
                {
                    solid = SolidType.random;
                } else
                {
                    Help(p);
                    return;
                }
                CatchPos cpos;
                cpos.solid = solid;
                cpos.type = type;
                cpos.x = 0;
                cpos.y = 0;
                cpos.z = 0;
                p.blockchangeObject = cpos;
            } else if (message != "")
            {
                SolidType solid = SolidType.solid;
                try
                {
                    msg = message.ToLower();
                } catch
                {
                }
                byte type;
                unchecked
                {
                    type = (byte)-1;
                }
                if (msg == "solid")
                {
                    solid = SolidType.solid;
                } else if (msg == "hollow")
                {
                    solid = SolidType.hollow;
                } else if (msg == "walls")
                {
                    solid = SolidType.walls;
                } else if (msg == "holes")
                {
                    solid = SolidType.holes;
                } else if (msg == "wire")
                {
                    solid = SolidType.wire;
                } else if (msg == "random")
                {
                    solid = SolidType.random;
                } else
                {
                    byte t = Block.Byte(msg);
                    if (t == 255)
                    {
                        Player.SendMessage(p, "There is no block \"" + msg + "\".");
                        wait = 1;
                        return;
                    }

                    if (!Block.canPlace(p, t))
                    {
                        Player.SendMessage(p, "Cannot place that.");
                        wait = 1;
                        return;
                    }

                    type = t;
                }
                CatchPos cpos;
                cpos.solid = solid;
                cpos.type = type;
                cpos.x = 0;
                cpos.y = 0;
                cpos.z = 0;
                p.blockchangeObject = cpos;
            } else
            {
                CatchPos cpos;
                cpos.solid = SolidType.solid;
                unchecked
                {
                    cpos.type = (byte)-1;
                }
                cpos.x = 0;
                cpos.y = 0;
                cpos.z = 0;
                p.blockchangeObject = cpos;
            }
            if (p.pyramidsilent == false)
            {
                Player.SendMessage(p, "Place two blocks to determine the edges.");
            }
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/cuboid [type] <solid/hollow/walls/holes/wire/random> - create a cuboid of blocks.");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x;
            bp.y = y;
            bp.z = z;
            p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            unchecked
            {
                if (cpos.type != (byte)-1)
                    type = cpos.type;
                else
                    type = p.bindings [type];
            }
            List<Pos> buffer = new List<Pos>();
            ushort xx;
            ushort yy;
            ushort zz;

            switch (cpos.solid)
            {
                case SolidType.solid:
                    buffer.Capacity = Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z);
                    for (xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                        for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                            for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                            {
                                if (p.level.GetTile(xx, yy, zz) != type)
                                {
                                    BufferAdd(buffer, xx, yy, zz);
                                }
                            }
                    break;
                case SolidType.hollow:
                    //todo work out if theres 800 blocks used before making the buffer
                    for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        {
                            if (p.level.GetTile(cpos.x, yy, zz) != type)
                            {
                                BufferAdd(buffer, cpos.x, yy, zz);
                            }
                            if (cpos.x != x)
                            {
                                if (p.level.GetTile(x, yy, zz) != type)
                                {
                                    BufferAdd(buffer, x, yy, zz);
                                }
                            }
                        }
                    if (Math.Abs(cpos.x - x) >= 2)
                    {
                        for (xx = (ushort)(Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                            for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                            {
                                if (p.level.GetTile(xx, cpos.y, zz) != type)
                                {
                                    BufferAdd(buffer, xx, cpos.y, zz);
                                }
                                if (cpos.y != y)
                                {
                                    if (p.level.GetTile(xx, y, zz) != type)
                                    {
                                        BufferAdd(buffer, xx, y, zz);
                                    }
                                }
                            }
                        if (Math.Abs(cpos.y - y) >= 2)
                        {
                            for (xx = (ushort)(Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                                for (yy = (ushort)(Math.Min(cpos.y, y) + 1); yy <= Math.Max(cpos.y, y) - 1; ++yy)
                                {
                                    if (p.level.GetTile(xx, yy, cpos.z) != type)
                                    {
                                        BufferAdd(buffer, xx, yy, cpos.z);
                                    }
                                    if (cpos.z != z)
                                    {
                                        if (p.level.GetTile(xx, yy, z) != type)
                                        {
                                            BufferAdd(buffer, xx, yy, z);
                                        }
                                    }
                                }
                        }
                    }
                    break;
                case SolidType.walls:
                    for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        {
                            if (p.level.GetTile(cpos.x, yy, zz) != type)
                            {
                                BufferAdd(buffer, cpos.x, yy, zz);
                            }
                            if (cpos.x != x)
                            {
                                if (p.level.GetTile(x, yy, zz) != type)
                                {
                                    BufferAdd(buffer, x, yy, zz);
                                }
                            }
                        }
                    if (Math.Abs(cpos.x - x) >= 2)
                    {
                        if (Math.Abs(cpos.z - z) >= 2)
                        {
                            for (xx = (ushort)(Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                                for (yy = (ushort)(Math.Min(cpos.y, y)); yy <= Math.Max(cpos.y, y); ++yy)
                                {
                                    if (p.level.GetTile(xx, yy, cpos.z) != type)
                                    {
                                        BufferAdd(buffer, xx, yy, cpos.z);
                                    }
                                    if (cpos.z != z)
                                    {
                                        if (p.level.GetTile(xx, yy, z) != type)
                                        {
                                            BufferAdd(buffer, xx, yy, z);
                                        }
                                    }
                                }
                        }
                    }
                    break;
                case SolidType.holes:
                    bool Checked = true, startZ, startY;

                    for (xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                    {
                        startY = Checked;
                        for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        {
                            startZ = Checked;
                            for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                            {
                                Checked = !Checked;
                                if (Checked && p.level.GetTile(xx, yy, zz) != type)
                                    BufferAdd(buffer, xx, yy, zz);
                            }
                            Checked = !startZ;
                        }
                        Checked = !startY;
                    }
                    break;
                case SolidType.wire:
                    for (xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                    {
                        BufferAdd(buffer, xx, y, z);
                        BufferAdd(buffer, xx, y, cpos.z);
                        BufferAdd(buffer, xx, cpos.y, z);
                        BufferAdd(buffer, xx, cpos.y, cpos.z);
                    }
                    for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    {
                        BufferAdd(buffer, x, yy, z);
                        BufferAdd(buffer, x, yy, cpos.z);
                        BufferAdd(buffer, cpos.x, yy, z);
                        BufferAdd(buffer, cpos.x, yy, cpos.z);
                    }
                    for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        BufferAdd(buffer, x, y, zz);
                        BufferAdd(buffer, x, cpos.y, zz);
                        BufferAdd(buffer, cpos.x, y, zz);
                        BufferAdd(buffer, cpos.x, cpos.y, zz);
                    }
                    break;
                case SolidType.random:
                    Random rand = new Random();
                    for (xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                        for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                            for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                            {
                                if (rand.Next(1, 11) <= 5 && p.level.GetTile(xx, yy, zz) != type)
                                {
                                    BufferAdd(buffer, xx, yy, zz);
                                }
                            }
                    break;
            }

            // Check to see if user is subject to anti-tunneling
            if (Server.antiTunnel && p.group.Permission == LevelPermission.Guest && !p.ignoreGrief)
            {
                int CheckForBlocksBelowY = p.level.depth / 2 - Server.maxDepth;
                if (buffer.Any(pos => pos.y < CheckForBlocksBelowY))
                {
                    p.SendMessage("You're not allowed to build this far down!");
                    return;
                }
            }

            if (Server.forceCuboid)
            {
                int counter = 1;
                buffer.ForEach(delegate(Pos pos)
                {
                    if (counter <= p.group.maxBlocks)
                    {
                        counter++;
                        p.level.Blockchange(p, pos.x, pos.y, pos.z, type);
                    }
                });
                if (counter >= p.group.maxBlocks)
                {
                    Player.SendMessage(p, "Tried to cuboid " + buffer.Count + " blocks, but your limit is " + p.group.maxBlocks + ".");
                    Player.SendMessage(p, "Executed cuboid up to limit.");
                                 
                    wait = 2;
                } else
                {
                    Player.SendMessage(p, buffer.Count.ToString() + " blocks.");
                    
                }
                wait = 2;
                if (p.staticCommands)
                    p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                return;
            }

            if (buffer.Count > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to cuboid " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot cuboid more than " + p.group.maxBlocks + ".");
                wait = 1;
                return;
            }

            if (p.pyramidsilent == false)
            {
                Player.SendMessage(p, buffer.Count.ToString() + " blocks.");
            }
            if (buffer.Count() <= 10000)
            {
                if (p.level.bufferblocks && !p.level.Instant)
                {
                    buffer.ForEach(delegate(Pos pos)
                    {
                        BlockQueue.Addblock(p, pos.x, pos.y, pos.z, type);
                    });
                } else
                {
                    buffer.ForEach(delegate(Pos pos)
                    {
                        p.level.Blockchange(p, pos.x, pos.y, pos.z, type);
                    });
                }
            } else
            {
                p.SendMessage("You tried to cuboid over 10000 blocks, reloading map for faster cuboid.");
                buffer.ForEach(delegate(Pos pos)
                   {
                        p.level.SetTile(pos.x, pos.y, pos.z, type, p);
                });
                foreach(Player pl in Player.players)
                {
                    if(pl.level.name.ToLower() == p.level.name.ToLower())
                    {
                        Command.all.Find("reveal").Use(p, pl.name);
                    }
                }
            }
                wait = 2;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z)
        {
            Pos pos;
            pos.x = x;
            pos.y = y;
            pos.z = z;
            list.Add(pos);
        }
        struct Pos
        {
            public ushort x, y, z;
        }

        struct CatchPos
        {
            public SolidType solid;
            public byte type;
            public ushort x, y, z;
        }

        enum SolidType
        {
            solid,
            hollow,
            walls,
            holes,
            wire,
            random }
        ;
    }
}
