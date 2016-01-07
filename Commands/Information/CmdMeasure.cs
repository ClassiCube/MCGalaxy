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
namespace MCGalaxy.Commands
{
    public sealed class CmdMeasure : Command
    {
        public override string name { get { return "measure"; } }
        public override string shortcut { get { return "ms"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMeasure() { }

        public override void Use(Player p, string message)
        {
            if (message.IndexOf(' ') != -1) { Help(p); return; }

            CatchPos cpos;
            cpos.toIgnore = Block.Byte(message);
            if (cpos.toIgnore == Block.Zero && message != "") { Player.SendMessage(p, "Could not find block specified"); return; }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/measure [ignore] - Measures all the blocks between two points");
            Player.SendMessage(p, "/measure [ignore] - Enter a block to ignore them");
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

            ushort xx, yy, zz; int foundBlocks = 0;

            for (xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        if (p.level.GetTile(xx, yy, zz) != cpos.toIgnore) foundBlocks++;
                    }

            Player.SendMessage(p, foundBlocks + " blocks are between (" + cpos.x + ", " + cpos.y + ", " + cpos.z + ") and (" + x + ", " + y + ", " + z + ")");
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        struct CatchPos
        {
            public ushort x, y, z; public byte toIgnore;
        }
    }
}
