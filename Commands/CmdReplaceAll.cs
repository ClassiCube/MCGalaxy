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
    public sealed class CmdReplaceAll : Command
    {
        public override string name { get { return "replaceall"; } }
        public override string shortcut { get { return "ra"; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdReplaceAll() { }
        public static byte wait;

        public override void Use(Player p, string message)
        {
            wait = 0;
            string[] args = message.Split(' ');
            if (args.Length != 2) { p.SendMessage("Invalid number of arguments!"); Help(p); wait = 1; return; }

            List<string> temp;

            if (args[0].Contains(","))
                temp = new List<string>(args[0].Split(','));
            else
                temp = new List<string>() { args[0] };

            temp = temp.Distinct().ToList(); // Remove duplicates

            List<string> invalid = new List<string>(); //Check for invalid blocks
            foreach (string name in temp)
                if (Block.Byte(name) == 255)
                    invalid.Add(name);
            if (Block.Byte(args[1]) == 255)
                invalid.Add(args[1]);
            if (invalid.Count > 0)
            {
                p.SendMessage(String.Format("Invalid block{0}: {1}", invalid.Count == 1 ? "" : "s", String.Join(", ", invalid.ToArray())));
                return;
            }
            if (temp.Contains(args[1]))
                temp.Remove(args[1]);
            if (temp.Count < 1)
            {
                p.SendMessage("Replacing a block with the same one would be pointless!");
                return;
            }

            List<byte> oldType = new List<byte>();
            foreach (string name in temp)
                oldType.Add(Block.Byte(name));
            byte newType = Block.Byte(args[1]);

            foreach (Byte type in oldType)
                if (!Block.canPlace(p, type) && !Block.BuildIn(type)) { p.SendMessage("Cannot replace that."); wait = 1; return; }
            if (!Block.canPlace(p, newType)) { Player.SendMessage(p, "Cannot place that."); wait = 1; return; }
            
            ushort x, y, z; int currentBlock = 0;
            List<Pos> stored = new List<Pos>(); Pos pos;

            foreach (byte b in p.level.blocks)
            {
                if (oldType.Contains(b))
                {
                    p.level.IntToPos(currentBlock, out x, out y, out z);
                    pos.x = x; pos.y = y; pos.z = z;
                    stored.Add(pos);
                }
                currentBlock++;
            }

            if (stored.Count > (p.group.maxBlocks * 2)) { Player.SendMessage(p, "Cannot replace more than " + (p.group.maxBlocks * 2) + " blocks."); wait = 1; return; }

            p.SendMessage(stored.Count + " blocks out of " + currentBlock + " will be replaced.");
            if (stored.Count < 10000)
            {
                if (p.level.bufferblocks && !p.level.Instant)
                {
                    foreach (Pos Pos in stored)
                    {
                        BlockQueue.Addblock(p, Pos.x, Pos.y, Pos.z, newType);
                    }
                }
                else
                {
                    foreach (Pos Pos in stored)
                    {
                        p.level.Blockchange(p, Pos.x, Pos.y, Pos.z, newType);
                    }
                }
            }
            else
            {
                p.SendMessage("You tried to replace over 10000 blocks, reloading map for faster replace.");
                foreach (Pos Pos in stored)
                {
                    p.level.SetTile(Pos.x, Pos.y, Pos.z, newType, p);
                }
                foreach (Player pl in Player.players)
                {
                    if (pl.level.name.ToLower() == p.level.name.ToLower())
                    {
                        Command.all.Find("reveal").Use(p, pl.name);
                    }
                }
            }

            Player.SendMessage(p, "&4/replaceall finished!");
            wait = 2;
        }
        public struct Pos { public ushort x, y, z; }

        public override void Help(Player p)
        {
            p.SendMessage("/replaceall [block,block2,...] [new] - Replaces all of [block] with [new] in a map");
            p.SendMessage("If more than one block is specified, they will all be replaced.");
        }
    }
}