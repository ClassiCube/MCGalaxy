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
namespace MCGalaxy.Commands
{
    public sealed class CmdHollow : Command
    {
        public override string name { get { return "hollow"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdHollow() { }

        public override void Use(Player p, string message)
        {
            CatchPos cpos;
            if (message != "")
                if (Block.Byte(message.ToLower()) == Block.Zero) { Player.SendMessage(p, "Cannot find block entered."); return; }

            if (message != "")
            {
                cpos.countOther = Block.Byte(message.ToLower());
            }
            else
            {
                cpos.countOther = Block.Zero;
            }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/hollow - Hollows out an area without flooding it");
            Player.SendMessage(p, "/hollow [block] - Hollows around [block]");
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            List<Pos> buffer = new List<Pos>();
            Pos pos;

            bool AddMe = false;

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        AddMe = true;

                        if (!Block.RightClick(Block.Convert(p.level.GetTile(xx, yy, zz)), true) && p.level.GetTile(xx, yy, zz) != cpos.countOther)
                        {
                            if (Block.RightClick(Block.Convert(p.level.GetTile((ushort)(xx - 1), yy, zz))) || p.level.GetTile((ushort)(xx - 1), yy, zz) == cpos.countOther) AddMe = false;
                            else if (Block.RightClick(Block.Convert(p.level.GetTile((ushort)(xx + 1), yy, zz))) || p.level.GetTile((ushort)(xx + 1), yy, zz) == cpos.countOther) AddMe = false;
                            else if (Block.RightClick(Block.Convert(p.level.GetTile(xx, (ushort)(yy - 1), zz))) || p.level.GetTile(xx, (ushort)(yy - 1), zz) == cpos.countOther) AddMe = false;
                            else if (Block.RightClick(Block.Convert(p.level.GetTile(xx, (ushort)(yy + 1), zz))) || p.level.GetTile(xx, (ushort)(yy + 1), zz) == cpos.countOther) AddMe = false;
                            else if (Block.RightClick(Block.Convert(p.level.GetTile(xx, yy, (ushort)(zz - 1)))) || p.level.GetTile(xx, yy, (ushort)(zz - 1)) == cpos.countOther) AddMe = false;
                            else if (Block.RightClick(Block.Convert(p.level.GetTile(xx, yy, (ushort)(zz + 1)))) || p.level.GetTile(xx, yy, (ushort)(zz + 1)) == cpos.countOther) AddMe = false;
                        }
                        else AddMe = false;

                        if (AddMe) { pos.x = xx; pos.y = yy; pos.z = zz; buffer.Add(pos); }
                    }

            if (buffer.Count > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to hollow more than " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot hollow more than " + p.group.maxBlocks + ".");
                return;
            }

            buffer.ForEach(P => p.level.UpdateBlock(p, P.x, P.y, P.z, Block.air, 0));

            Player.SendMessage(p, "You hollowed " + buffer.Count + " blocks.");

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct Pos
        {
            public ushort x, y, z;
        }
        struct CatchPos
        {
            public ushort x, y, z; public byte countOther;
        }

    }
}
