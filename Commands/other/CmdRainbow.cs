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
namespace MCGalaxy.Commands
{
    public sealed class CmdRainbow : Command
    {
        public override string name { get { return "rainbow"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdRainbow() { }

        public override void Use(Player p, string message)
        {
            CatchPos cpos;
            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/rainbow - Taste the rainbow");
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            List<Pos> buffer = new List<Pos>();

            byte newType = Block.darkpink;

            int xdif = Math.Abs(cpos.x - x);
            int ydif = Math.Abs(cpos.y - y);
            int zdif = Math.Abs(cpos.z - z);

            if (xdif >= ydif && xdif >= zdif)
            {
                for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); xx++)
                {
                    newType += 1;
                    if (newType > Block.darkpink) newType = Block.red;
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++)
                    {
                        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); zz++)
                        {
                            if (p.level.GetTile(xx, yy, zz) != Block.air)
                                BufferAdd(buffer, xx, yy, zz, newType);
                        }
                    }
                }
            }
            else if (ydif > xdif && ydif > zdif)
            {
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++)
                {
                    newType += 1;
                    if (newType > Block.darkpink) newType = Block.red;
                    for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); xx++)
                    {
                        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); zz++)
                        {
                            if (p.level.GetTile(xx, yy, zz) != Block.air)
                                BufferAdd(buffer, xx, yy, zz, newType);
                        }
                    }
                }
            }
            else if (zdif > ydif && zdif > xdif)
            {
                for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); zz++)
                {
                    newType += 1;
                    if (newType > Block.darkpink) newType = Block.red;
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++)
                    {
                        for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); xx++)
                        {
                            if (p.level.GetTile(xx, yy, zz) != Block.air)
                                BufferAdd(buffer, xx, yy, zz, newType);
                        }
                    }
                }
            }

            if (buffer.Count > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to replace " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot replace more than " + p.group.maxBlocks + ".");
                return;
            }

            Player.SendMessage(p, buffer.Count.ToString() + " blocks.");
            buffer.ForEach(delegate(Pos pos)
            {
                p.level.Blockchange(p, pos.x, pos.y, pos.z, pos.newType);                  //update block for everyone
            });

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z, byte newType)
        {
            Pos pos;
            pos.x = x; pos.y = y; pos.z = z; pos.newType = newType;
            list.Add(pos);
        }

        struct Pos { public ushort x, y, z; public byte newType; }
        struct CatchPos { public ushort x, y, z; }
    }
}
