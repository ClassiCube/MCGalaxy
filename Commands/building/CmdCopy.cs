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
using System.IO;
using System.IO.Compression;
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
	public sealed class CmdCopy : Command {
		public override string name { get { return "copy"; } }
		public override string shortcut { get { return "c"; } }
		public override string type { get { return CommandTypes.Building; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
		public override CommandAlias[] Aliases {
			get { return new[] { new CommandAlias("cut", "cut") }; }
		}
		public CmdCopy() { }

		public override void Use(Player p, string message) {
			int allowoffset = message.IndexOf('@');
			if (allowoffset != -1)
				message = message.Replace("@ ", "").Replace("@", "");
			
			string[] parts = message.Split(' ');
			string opt = parts[0].ToLower();
			
			if (opt == "save") {
				if (parts.Length != 2) { Help(p); return; }
				SaveCopy(p, parts[1]);
			} else if (opt == "load") {
				if (parts.Length != 2) { Help(p); return; }
				LoadCopy(p, parts[1]);
			} else if (opt == "delete") {
				if (parts.Length != 2) { Help(p); return; }
				string path = "extra/savecopy/" + parts[1] + "/" + message + ".cpy";
				if (!File.Exists(path)) {
					Player.Message(p, "No such copy exists"); return;
				}
				
				File.Delete(path);
				Player.Message(p, "Deleted copy " + parts[1]);
			} else if (opt == "list") {
				string dir = "extra/savecopy/" + p.name;
				if (!Directory.Exists(dir)) {
					Player.Message(p, "No such directory exists"); return;
				}
				
				string[] files = Directory.GetFiles(dir);
				for (int i = 0; i < files.Length; i++) {
					Player.Message(p, Path.GetFileNameWithoutExtension(files[i]));
				}
			} else {
				HandleOther(p, opt, parts, allowoffset);
			}
		}
		
		void HandleOther(Player p, string opt, string[] parts, int allowoffset) {
			CatchPos cpos = default(CatchPos);
			p.copyoffset[0] = 0; p.copyoffset[1] = 0; p.copyoffset[2] = 0;
			cpos.allowoffset = allowoffset;
			
			if (opt == "cut") {
				cpos.type = 1;
			} else if (opt == "air") {
				cpos.type = 2;
			} else if (!String.IsNullOrEmpty(opt)) {
				Help(p); return;
			}

			p.blockchangeObject = cpos;
			Player.Message(p, "Place two blocks to determine the edges.");
			p.ClearBlockchange();
			p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
		}

		void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
			RevertAndClearState(p, x, y, z);
			CatchPos bp = (CatchPos)p.blockchangeObject;
			p.copystart[0] = x; p.copystart[1] = y; p.copystart[2] = z;

			bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
			p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
		}

		void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
			RevertAndClearState(p, x, y, z);
			CatchPos cpos = (CatchPos)p.blockchangeObject;
			ushort minX = (ushort)Math.Min(x, cpos.x), maxX = (ushort)Math.Max(x, cpos.x);
			ushort minY = (ushort)Math.Min(y, cpos.y), maxY = (ushort)Math.Max(y, cpos.y);
			ushort minZ = (ushort)Math.Min(z, cpos.z), maxZ = (ushort)Math.Max(z, cpos.z);
			
			CopyState state = new CopyState(minX, minY, minZ, maxX - minX + 1,
			                                maxY - minY + 1, maxZ - minZ + 1);
			state.SetOrigin(cpos.x, cpos.y, cpos.z);
			int index = 0; state.UsedBlocks = 0;
			state.PasteAir = cpos.type == 2;
			
			for (ushort yy = minY; yy <= maxY; ++yy)
				for (ushort zz = minZ; zz <= maxZ; ++zz)
					for (ushort xx = minX; xx <= maxX; ++xx)
			{
				byte b = p.level.GetTile(xx, yy, zz), extB = 0;
				if (!Block.canPlace(p, b)) { index++; continue; }
				if (b == Block.custom_block)
					extB = p.level.GetExtTile(xx, yy, zz);
				
				if (b != Block.air || state.PasteAir)
					state.UsedBlocks++;
				state.Blocks[index] = b;
				state.ExtBlocks[index] = extB;
				index++;
			}
			
			if (state.UsedBlocks > p.group.maxBlocks) {
				Player.Message(p, "You tried to copy {0} blocks. You cannot copy more than {1} blocks.", 
				               state.UsedBlocks, p.group.maxBlocks);
				state.Blocks = null; state.ExtBlocks = null; state = null;
				return;
			}
			
			p.CopyBuffer = state;
			if (cpos.type == 1) {
				DrawOp op = new CuboidDrawOp();
				Brush brush = new SolidBrush(Block.air, 0);
				Vec3S32[] marks = { new Vec3S32(minX, minY, minZ), new Vec3S32(maxX, maxY, maxZ) };
				DrawOp.DoDrawOp(op, brush, p, marks, false);
			}

			string format = "Copied &a{0} %Sblocks." +
				(state.PasteAir ? "" : " To also copy air blocks, use %T/copy air");
			Player.Message(p, format, state.UsedBlocks);
			if (cpos.allowoffset != -1) {
				Player.Message(p, "Place a block to determine where to paste from");
				p.Blockchange += new Player.BlockchangeEventHandler(Blockchange3);
			}
		}

		void Blockchange3(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
			RevertAndClearState(p, x, y, z);
			CatchPos cpos = (CatchPos)p.blockchangeObject;

			p.copyoffset[0] = (p.copystart[0] - x);
			p.copyoffset[1] = (p.copystart[1] - y);
			p.copyoffset[2] = (p.copystart[2] - z);
		}

		void SaveCopy(Player p, string file) {
			if (!ValidName(p, file, "saved copy")) return;
			
			if (!Directory.Exists("extra/savecopy"))
				Directory.CreateDirectory("extra/savecopy");
			if (!Directory.Exists("extra/savecopy/" + p.name))
				Directory.CreateDirectory("extra/savecopy/" + p.name);
			if (Directory.GetFiles("extra/savecopy/" + p.name).Length > 15) {
				Player.Message(p, "You can only save a maxmium of 15 copies. /copy delete some.");
				return;
			}
			
			string path = "extra/savecopy/" + p.name + "/" + file + ".cpb";
			using (FileStream fs = new FileStream(path, FileMode.Create))
				using(GZipStream gs = new GZipStream(fs, CompressionMode.Compress))
			{
				p.CopyBuffer.SaveTo(gs);
			}
			Player.Message(p, "Saved copy as " + file);
		}

		void LoadCopy(Player p, string file) {
			string path = "extra/savecopy/" + p.name + "/" + file;
			bool existsNew = File.Exists(path + ".cpb");
			bool existsOld = File.Exists(path + ".cpy");
			if (!existsNew && !existsOld) {
				Player.Message(p, "No such copy exists"); return;
			}
			path = existsNew ? path + ".cpb" : path + ".cpy";

			using (FileStream fs = new FileStream(path, FileMode.Open))
				using(GZipStream gs = new GZipStream(fs, CompressionMode.Decompress))
			{
				CopyState state = new CopyState(0, 0, 0, 0, 0, 0, null, null);
				if (existsNew)
					state.LoadFrom(gs);
				else
					state.LoadFromOld(gs, fs);
				p.CopyBuffer = state;
			}
			Player.Message(p, "Loaded copy as " + file);
		}
		
		struct CatchPos { public ushort x, y, z; public int type; public int allowoffset; }
		
		public override void Help(Player p) {
			Player.Message(p, "/copy - Copies the blocks in an area.");
			Player.Message(p, "/copy save <save_name> - Saves what you have copied.");
			Player.Message(p, "/copy load <load_name> - Loads what you have saved.");
			Player.Message(p, "/copy delete <delete_name> - Deletes the specified copy.");
			Player.Message(p, "/copy list - Lists all saved copies you have");
			Player.Message(p, "/copy cut - Copies the blocks in an area, then removes them.");
			Player.Message(p, "/copy air - Copies the blocks in an area, including air.");
			Player.Message(p, "/copy @ - @ toggle for all the above, gives you a third click after copying that determines where to paste from");
		}
	}
}
