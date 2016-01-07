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
namespace MCGalaxy.Commands
{
    public sealed class CmdWrite : Command
    {
        public override string name { get { return "write"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdWrite() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            CatchPos cpos;

            cpos.givenMessage = message.ToUpper();
            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine direction.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/write [message] - Writes [message] in blocks");
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
            type = p.bindings[type];
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            ushort cur;
            if (x == cpos.x && z == cpos.z) { Player.SendMessage(p, "No direction was selected"); return; }

            if (Math.Abs(cpos.x - x) > Math.Abs(cpos.z - z)) {
                cur = cpos.x;
                int dir = x > cpos.x ? 0 : 1;
                foreach (char c in cpos.givenMessage)
                	cur = FindReference.writeLetter(p, c, cur, cpos.y, cpos.z, type, 0);
            } else {
                cur = cpos.z;
                int dir = z > cpos.z ? 2 : 3;
                foreach (char c in cpos.givenMessage)
                	cur = FindReference.writeLetter(p, c, cpos.x, cpos.y, cur, type, dir);
            }

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos
        {
            public ushort x, y, z; public string givenMessage;
        }

    }
}
