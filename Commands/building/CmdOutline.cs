/*
	Copyright 2011 MCForge
		
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
    public sealed class CmdOutline : Command
    {
        public override string name { get { return "outline"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdOutline() { }

        public override void Use(Player p, string message)
        {
            int number = message.Split(' ').Length;
            if (number != 2) { Help(p); return; }

            int pos = message.IndexOf(' ');
            string t = message.Substring(0, pos).ToLower();
            string t2 = message.Substring(pos + 1).ToLower();
            byte type = Block.Byte(t);
            if (type == 255) { Player.SendMessage(p, "There is no block \"" + t + "\"."); return; }
            byte type2 = Block.Byte(t2);
            if (type2 == 255) { Player.SendMessage(p, "There is no block \"" + t2 + "\"."); return; }
            if (!Block.canPlace(p, type2)) { Player.SendMessage(p, "Cannot place that block type."); return; }

            CatchPos cpos; cpos.newType = type2; cpos.type = type;

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/outline [type] [type2] - Outlines [type] with [type2]");
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
            if (cpos.type != Block.Zero) 
                type = cpos.type;
            List<Vec3U16> buffer = new List<Vec3U16>();
            Vec3U16 pos;

            bool AddMe = false;

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        AddMe = false;

                        if (p.level.GetTile((ushort)(xx - 1), yy, zz) == cpos.type) AddMe = true;
                        else if (p.level.GetTile((ushort)(xx + 1), yy, zz) == cpos.type) AddMe = true;
                        else if (p.level.GetTile(xx, (ushort)(yy - 1), zz) == cpos.type) AddMe = true;
                        else if (p.level.GetTile(xx, (ushort)(yy + 1), zz) == cpos.type) AddMe = true;
                        else if (p.level.GetTile(xx, yy, (ushort)(zz - 1)) == cpos.type) AddMe = true;
                        else if (p.level.GetTile(xx, yy, (ushort)(zz + 1)) == cpos.type) AddMe = true;

                        if (AddMe && p.level.GetTile(xx, yy, zz) != cpos.type) { pos.X = xx; pos.Y = yy; pos.Z = zz; buffer.Add(pos); }
                    }

            if (buffer.Count > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to outline more than " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot outline more than " + p.group.maxBlocks + ".");
                return;
            }

            buffer.ForEach(P => p.level.UpdateBlock(p, P.X, P.Y, P.Z, cpos.newType, 0));
            Player.SendMessage(p, "You outlined " + buffer.Count + " blocks.");

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos
        {
            public byte type;
            public byte newType;
            public ushort x, y, z;
        }

    }
}
