/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {	
	public class CmdReplaceBrush : Command {
		public override string name { get { return "replacebrush"; } }
		public override string shortcut { get { return "rb"; } }
		public override string type { get { return CommandTypes.Building; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

		public override void Use(Player p, string message) {
			if (message == "") { Help(p); return; }
			if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
			string replaceCmd = ReplaceNot ? "replacenot" : "replace";
			if (!p.group.CanExecute(replaceCmd) || !p.group.CanExecute("brush")) {
				Player.Message(p, "You cannot use /brush and/or /" + replaceCmd + 
				                   ", so therefore cannot use this command."); return;
			}
			
			CatchPos cpos = default(CatchPos);
			cpos.message = message.ToLower();
			p.blockchangeObject = cpos;
			
			Player.Message(p, "Place two blocks to determine the edges.");
			p.ClearBlockchange();
			p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
		}
		
		void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
			RevertAndClearState(p, x, y, z);
			CatchPos bp = (CatchPos)p.blockchangeObject;
			bp.x = x; bp.y = y; bp.z = z;
			p.blockchangeObject = bp;
			p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
		}
		
		void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
			RevertAndClearState(p, x, y, z);
			CatchPos cpos = (CatchPos)p.blockchangeObject;
			type = type < 128 ? p.bindings[type] : type;
			
			string[] parts = cpos.message.SplitSpaces(3);
			if (parts.Length < 2) { Help(p); return; }
			
			byte extTile = 0;
			byte tile = DrawCmd.GetBlock(p, parts[0], out extTile);
			if (tile == Block.Zero) return;
			string brushName = CmdBrush.FindBrush(parts[1]);
			if (brushName == null) {
				Player.Message(p, "No brush found with name \"" + parts[1] + "\".");
				Player.Message(p, "Available brushes: " + CmdBrush.AvailableBrushes);
				return;
			}

			string brushMessage = parts.Length > 2 ? parts[2].ToLower() : "";
			BrushArgs args = new BrushArgs(p, brushMessage, type, extType);
			Brush brush = Brush.Brushes[brushName](args);
			if (brush == null) return;
			
			DrawOp drawOp = null;
			if (ReplaceNot) drawOp = new ReplaceNotDrawOp(tile, extTile);
			else drawOp = new ReplaceDrawOp(tile, extTile);
			
			if (!DrawOp.DoDrawOp(drawOp, brush, p, cpos.x, cpos.y, cpos.z, x, y, z))
				return;
			if (p.staticCommands)
				p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
		}
		
		protected virtual bool ReplaceNot { get { return false; } }
		
		struct CatchPos { public ushort x, y, z; public string message; }
		
		public override void Help(Player p) {
			Player.Message(p, "%T/rb [block] [brush name] <brush args>");
			Player.Message(p, "%HReplaces all blocks of the given type, " +
			                   "in the specified area with the output of the given brush.");
			Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
		}
	}
	
	public class CmdReplaceNotBrush : CmdReplaceBrush {
		public override string name { get { return "replacenotbrush"; } }
		public override string shortcut { get { return "rnb"; } }
		
		protected override bool ReplaceNot { get { return true; } }
		
		public override void Help(Player p) {
			Player.Message(p, "%T/rnb [block] [brush name] <brush args>");
			Player.Message(p, "%HReplaces all blocks (except for the given block), " +
			                   "in the specified area with the output of the given brush.");
			Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
		}
	}
}
