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

namespace MCGalaxy.Commands {
	
	public sealed class CmdRedo : Command {
		
		public override string name { get { return "redo"; } }
		public override string shortcut { get { return ""; } }
		public override string type { get { return CommandTypes.Building; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
		public CmdRedo() { }

		public override void Use(Player p, string message) {
			if (message != "") { Help(p); return; }

			for (int i = p.RedoBuffer.Count - 1; i >= 0; i--) {
				Player.UndoPos Pos = p.RedoBuffer[i];
				Level lvl = Level.FindExact(Pos.mapName);
				if (lvl == null)
					continue;
				
				byte type = lvl.GetTile(Pos.x, Pos.y, Pos.z), extType = 0;
				if (type == Block.custom_block)
					extType = lvl.GetExtTile(Pos.x, Pos.y, Pos.z);
				lvl.Blockchange(Pos.x, Pos.y, Pos.z, Pos.type, Pos.extType);
				Pos.newtype = Pos.type; Pos.newExtType = Pos.extType;
				Pos.type = type; Pos.extType = extType;
				Pos.timePlaced = DateTime.Now;
				p.UndoBuffer.Add(Pos);
			}

			Player.SendMessage(p, "Redo performed.");
		}

		public override void Help(Player p) {
			Player.SendMessage(p, "/redo - Redoes the Undo you just performed.");
		}
	}
}
