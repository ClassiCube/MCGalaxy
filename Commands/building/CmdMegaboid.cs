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
    public sealed class CmdMegaboid : Command
    {
        public override string name { get { return "megaboid"; } }
        public override string shortcut { get { return "zm"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdMegaboid() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "The console may not use megaboid!"); return; }
            if (p.megaBoid) { Player.SendMessage(p, "You may only have one Megaboid going at a time. Use /abort to cancel the current megaboid."); return; }
            if (p.level.permissionbuild > p.group.Permission) { Player.SendMessage(p, "You may not megaboid on this level!"); return; }
            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            if (number == 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                byte type = Block.Byte(t);
                if (type == 255) { Player.SendMessage(p, "There is no block \"" + t + "\"."); return; }

                if (!Block.canPlace(p, type)) { Player.SendMessage(p, "Cannot place that."); return; }

                SolidType solid;
                if (s == "solid") { solid = SolidType.solid; }
                else if (s == "hollow") { solid = SolidType.hollow; }
                else if (s == "walls") { solid = SolidType.walls; }
                else { Help(p); return; }
                CatchPos cpos; cpos.solid = solid; cpos.type = type;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            else if (message != "")
            {
                SolidType solid = SolidType.solid;
                message = message.ToLower();
                byte type; unchecked { type = (byte)-1; }
                if (message == "solid") { solid = SolidType.solid; }
                else if (message == "hollow") { solid = SolidType.hollow; }
                else if (message == "walls") { solid = SolidType.walls; }
                else
                {
                    byte t = Block.Byte(message);
                    if (t == 255) { Player.SendMessage(p, "There is no block \"" + message + "\"."); return; }
                    if (!Block.canPlace(p, t)) { Player.SendMessage(p, "Cannot place that."); return; }

                    type = t;
                } CatchPos cpos; cpos.solid = solid; cpos.type = type;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            else
            {
                CatchPos cpos; cpos.solid = SolidType.solid; unchecked { cpos.type = (byte)-1; }
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/megaboid [type] <solid/hollow/walls/holes> - create a cuboid of blocks.");
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            System.Timers.Timer megaTimer = new System.Timers.Timer(1);

            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            unchecked { if (cpos.type != (byte)-1) type = cpos.type; else type = p.bindings[type]; }
            List<Pos> buffer = new List<Pos>();

            ushort xx; ushort yy; ushort zz;

            switch (cpos.solid)
            {
                case SolidType.solid:
                    buffer.Capacity = Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z);
                    for (xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                        for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                            for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                            {
                                if (p.level.GetTile(xx, yy, zz) != type) { BufferAdd(buffer, xx, yy, zz); }
                            }
                    break;
                case SolidType.hollow:
                    //todo work out if theres 800 blocks used before making the buffer
                    for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        {
                            if (p.level.GetTile(cpos.x, yy, zz) != type) { BufferAdd(buffer, cpos.x, yy, zz); }
                            if (cpos.x != x) { if (p.level.GetTile(x, yy, zz) != type) { BufferAdd(buffer, x, yy, zz); } }
                        }
                    if (Math.Abs(cpos.x - x) >= 2)
                    {
                        for (xx = (ushort)(Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                            for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                            {
                                if (p.level.GetTile(xx, cpos.y, zz) != type) { BufferAdd(buffer, xx, cpos.y, zz); }
                                if (cpos.y != y) { if (p.level.GetTile(xx, y, zz) != type) { BufferAdd(buffer, xx, y, zz); } }
                            }
                        if (Math.Abs(cpos.y - y) >= 2)
                        {
                            for (xx = (ushort)(Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                                for (yy = (ushort)(Math.Min(cpos.y, y) + 1); yy <= Math.Max(cpos.y, y) - 1; ++yy)
                                {
                                    if (p.level.GetTile(xx, yy, cpos.z) != type) { BufferAdd(buffer, xx, yy, cpos.z); }
                                    if (cpos.z != z) { if (p.level.GetTile(xx, yy, z) != type) { BufferAdd(buffer, xx, yy, z); } }
                                }
                        }
                    }
                    break;
                case SolidType.walls:
                    for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        {
                            if (p.level.GetTile(cpos.x, yy, zz) != type) { BufferAdd(buffer, cpos.x, yy, zz); }
                            if (cpos.x != x) { if (p.level.GetTile(x, yy, zz) != type) { BufferAdd(buffer, x, yy, zz); } }
                        }
                    if (Math.Abs(cpos.x - x) >= 2)
                    {
                        if (Math.Abs(cpos.z - z) >= 2)
                        {
                            for (xx = (ushort)(Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                                for (yy = (ushort)(Math.Min(cpos.y, y)); yy <= Math.Max(cpos.y, y); ++yy)
                                {
                                    if (p.level.GetTile(xx, yy, cpos.z) != type) { BufferAdd(buffer, xx, yy, cpos.z); }
                                    if (cpos.z != z) { if (p.level.GetTile(xx, yy, z) != type) { BufferAdd(buffer, xx, yy, z); } }
                                }
                        }
                    }
                    break;
            }

            if (buffer.Count > 450000)
            {
                Player.SendMessage(p, "You cannot megaboid more than 450000 blocks.");
                Player.SendMessage(p, "You tried to megaboid " + buffer.Count + " blocks.");
                return;
            }

            Player.SendMessage(p, buffer.Count.ToString() + " blocks.");
            Player.SendMessage(p, "Use /abort to cancel the megaboid at any time.");
            p.megaBoid = true;
            Pos pos; int CurrentLoop = 0;
            Level currentLevel = p.level;
            megaTimer.Start();
            megaTimer.Elapsed += delegate
            {
                if (p.megaBoid)
                {
                    pos = buffer[CurrentLoop];
                    try { currentLevel.Blockchange(pos.x, pos.y, pos.z, type); }
                    catch { }
                    CurrentLoop++;
                    if (CurrentLoop % 1000 == 0) Player.SendMessage(p, CurrentLoop + " blocks down, " + (buffer.Count - CurrentLoop) + " to go.");
                    if (CurrentLoop >= buffer.Count) { Player.SendMessage(p, "Completed megaboid"); buffer.Clear(); p.megaBoid = false; megaTimer.Stop(); }
                }
                else
                {
                    megaTimer.Stop();
                }
            };

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
            public SolidType solid;
            public byte type;
            public ushort x, y, z;
        }
        enum SolidType { solid, hollow, walls };
    }
}
