/*
	Copyright 2010 MCSharp Team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;

namespace Minecraft_Server
{
    public class CmdWhodid : Command
    {
        public override string name { get { return "whodid"; } }
        public CmdWhodid() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            p.SendMessage("Break/build a block to see who edited it last.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/whodid - Shows who edited the next changed block.");
        }
        public void Blockchange(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            Block b = Program.mainLevel.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b.type);
            if (b == null) { p.SendMessage("No edits found for (" + x + "," + y + "," + z + ")."); return; }
            //if (b.lastaction.Count == 0) { p.SendMessage("No edits found for (" + x + "," + y + "," + z + ")."); return; }
            p.SendMessage("Last edits of (" + x + "," + y + "," + z + "):");
            //b.lastaction.ForEach(delegate(Edit e) { p.SendMessage("> " + Player.GetColor(e.from) + e.from + ":&e " + e.before + " => " + e.after); });
        }
    }
}