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
namespace MCGalaxy.Commands
{
    public sealed class CmdRainbow : Command
    {
        public override string name { get { return "rainbow"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdRainbow() { }

        public override void Use(Player p, string message)
        {
            CatchPos cpos;
            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            Player.Message(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.Message(p, "/rainbow - Taste the rainbow");
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
            int dx = Math.Abs(cpos.x - x), dy = Math.Abs(cpos.y - y), dz = Math.Abs(cpos.z - z);        
            byte stepX = 0, stepY = 0, stepZ = 0;
            
            if (dx >= dy && dx >= dz) {
                stepX = 1;
            } else if (dy > dx && dy > dz) {
                stepY = 1;            
            } else if (dz > dy && dz > dx) {
                stepZ = 1;
            }
            
            int i = 12;
            for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++) {
                i = (i + stepY) % 13;
                int startZ = i;
                for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); zz++) {
                    i = (i + stepZ) % 13;
                    int startX = i;
                    for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); xx++) {
                        i = (i + stepX) % 13;
                        if (p.level.GetTile(xx, yy, zz) != Block.air)
                            BufferAdd(buffer, xx, yy, zz, (byte)(Block.red + i));
                    }
                    i = startX;
                }
                i = startZ;
            }

            if (buffer.Count > p.group.maxBlocks) {
                Player.Message(p, "You tried to replace " + buffer.Count + " blocks.");
                Player.Message(p, "You cannot replace more than " + p.group.maxBlocks + ".");
                return;
            }

            Player.Message(p, "Rainbowing " + buffer.Count + " blocks.");
            buffer.ForEach(P => p.level.UpdateBlock(p, P.x, P.y, P.z, P.newType, 0));

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
