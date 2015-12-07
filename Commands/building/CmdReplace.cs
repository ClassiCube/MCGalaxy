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
using System.Linq;
namespace MCGalaxy.Commands
{
    public sealed class CmdReplace : Command
    {
        public override string name { get { return "replace"; } }
        public override string shortcut { get { return "r"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdReplace() { }
        public static byte wait;

        public override void Use(Player p, string message)
        {
            wait = 0;
            string[] args = message.Split(' ');
            if (args.Length != 2)
            {
                p.SendMessage("Invail number of arguments!");
                wait = 1;
                Help(p);
                return;
            }

            CatchPos cpos = new CatchPos();
            List<string> oldType;

            oldType = new List<string>(args[0].Split(','));

            oldType = oldType.Distinct().ToList(); // Remove duplicates

            List<string> invalid = new List<string>(); //Check for invalid blocks
            foreach (string name in oldType)
                if (Block.Byte(name) == 255)
                    invalid.Add(name);
            if (Block.Byte(args[1]) == 255)
                invalid.Add(args[1]);
            if (invalid.Count > 0)
            {
                p.SendMessage(String.Format("Invalid block{0}: {1}", invalid.Count == 1 ? "" : "s", String.Join(", ", invalid.ToArray())));
                wait = 1;
                return;
            }

            if (oldType.Contains(args[1]))
                oldType.Remove(args[1]);
            if (oldType.Count < 1)
            {
                p.SendMessage("Replacing a block with the same one would be pointless!");
                return;
            }

            cpos.oldType = new List<byte>();
            foreach (string name in oldType)
                cpos.oldType.Add(Block.Byte(name));
            cpos.newType = Block.Byte(args[1]);

            foreach (byte type in cpos.oldType)
                if (!Block.canPlace(p, type) && !Block.BuildIn(type)) { p.SendMessage("Cannot replace that."); wait = 1; return; }
            if (!Block.canPlace(p, cpos.newType)) { p.SendMessage("Cannot place that."); wait = 1; return; }

            p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/replace [block,block2,...] [new] - replace block with new inside a selected cuboid");
            p.SendMessage("If more than one block is specified, they will all be replaced.");
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

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        if (cpos.oldType.Contains(p.level.GetTile(xx, yy, zz))) { BufferAdd(buffer, xx, yy, zz); }

            if (buffer.Count > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to replace " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot replace more than " + p.group.maxBlocks + ".");
                wait = 1;
                return;
            }

            Player.SendMessage(p, buffer.Count.ToString() + " blocks.");
            if (buffer.Count < 10000)
            {
                if (p.level.bufferblocks && !p.level.Instant)
                {
                    buffer.ForEach(delegate(Pos pos)
                    {
                        BlockQueue.Addblock(p, pos.x, pos.y, pos.z, cpos.newType);                  //update block for everyone
                    });
                }
                else
                {
                    buffer.ForEach(delegate(Pos pos)
                    {
                        p.level.Blockchange(p, pos.x, pos.y, pos.z, cpos.newType);                  //update block for everyone
                    });
                }
            }
            else
            {
                p.SendMessage("You tried to replace over 10000 blocks, reloading map for faster replace.");
                foreach (Pos pos in buffer)
                {
                    p.level.SetTile(pos.x, pos.y, pos.z, cpos.newType, p);
                }
                foreach (Player pl in Player.players)
                {
                    if (pl.level.name.ToLower() == p.level.name.ToLower())
                    {
                        Command.all.Find("reveal").Use(p, pl.name);
                    }
                }
            }

            wait = 2;
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z)
        {
            Pos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }

        struct Pos
        {
            public ushort x, y, z;
        }

        struct CatchPos
        {
            public List<byte> oldType;
            public byte newType;
            public ushort x, y, z;
        }

    }
}
