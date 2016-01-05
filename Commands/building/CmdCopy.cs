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

namespace MCGalaxy.Commands
{
	public sealed class CmdCopy : Command
	{
		public override string name { get { return "copy"; } }
		public override string shortcut { get { return "c"; } }
		public override string type { get { return CommandTypes.Building; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
		public int allowoffset = 0;
		public CmdCopy() { }

		public override void Use(Player p, string message) {
			allowoffset = message.IndexOf('@');
			if (allowoffset != -1)
				message = message.Replace("@ ", "").Replace("@", "");
			
			string[] parts = message.Split(' ');
			string opt = parts.Length == 0 ? null : parts[0].ToLower();
			
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
					Player.SendMessage(p, "No such copy exists"); return; 
				}
				
				File.Delete(path);
				Player.SendMessage(p, "Deleted copy " + parts[1]);
			} else if (opt == "list") {
				string dir = "extra/savecopy/" + p.name;
				if (!Directory.Exists(dir)) {
					Player.SendMessage(p, "No such directory exists"); return;
				}
				
				FileInfo[] fin = new DirectoryInfo(dir).GetFiles();
				for (int i = 0; i < fin.Length; i++) {
					string name = fin[i].Name.Replace(".cpy", "").Replace(".cpb", "");
					Player.SendMessage(p, name);
				}
			} else {
				HandleOther(p, opt, parts);
			}
		}
		
		void HandleOther(Player p, string opt, string[] parts) {
			CatchPos cpos = default(CatchPos);
			cpos.ignoreTypes = new List<byte>();
			p.copyoffset[0] = 0; p.copyoffset[1] = 0; p.copyoffset[2] = 0;	
			
			if (opt == "cut") {
				cpos.type = 1;
			} else if (opt == "air") {
				cpos.type = 2;
			} else if (opt == "ignore") {
				for (int i = 1; i < parts.Length; i++ ) {
					string s = parts[i];
					if (Block.Byte(s) != Block.Zero) {
						cpos.ignoreTypes.Add(Block.Byte(s));
						Player.SendMessage(p, "Ignoring &b" + s);
					}
				}
			} else if (!String.IsNullOrEmpty(opt)) {
				Help(p); return;
			}

			p.blockchangeObject = cpos;
			Player.SendMessage(p, "Place two blocks to determine the edges.");
			p.ClearBlockchange();
			p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
		}

		void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
			RevertAndClearState(p, x, y, z);
			CatchPos bp = (CatchPos)p.blockchangeObject;
			p.copystart[0] = x;
			p.copystart[1] = y;
			p.copystart[2] = z;

			bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
			p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
		}

		void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
			RevertAndClearState(p, x, y, z);
			CatchPos cpos = (CatchPos)p.blockchangeObject;
			ushort minX = (ushort)Math.Min(x, cpos.x), minY = (ushort)Math.Min(y, cpos.y);
			ushort minZ = (ushort)Math.Min(z, cpos.z), maxX = (ushort)Math.Max(x, cpos.x);
			ushort maxY = (ushort)Math.Max(y, cpos.y), maxZ = (ushort)Math.Max(z, cpos.z);
			
			CopyState state = new CopyState(minX, minY, minZ, maxX - minX + 1, 
			                                maxY - minY + 1, maxZ - minZ + 1);
			state.SetOrigin(cpos.x, cpos.y, cpos.z);
			int totalAir = 0, index = 0;
			p.copyAir = cpos.type == 2;
			
			for (ushort yy = minY; yy <= maxY; ++yy)
				for (ushort zz = minZ; zz <= maxZ; ++zz)
					for (ushort xx = minX; xx <= maxX; ++xx)
			{
				byte b = p.level.GetTile(xx, yy, zz);
				if (!Block.canPlace(p, b)) { index++; continue; }
				
				if (b == Block.air && cpos.type != 2 || cpos.ignoreTypes.Contains(b))
					totalAir++;
				if (!cpos.ignoreTypes.Contains(b))
					state.Blocks[index] = b;
				index++;
			}
			p.CopyBuffer = state;

			if ((state.Volume - totalAir) > p.group.maxBlocks) {
				Player.SendMessage(p, "You tried to copy " + state.Volume + " blocks.");
				Player.SendMessage(p, "You cannot copy more than " + p.group.maxBlocks + ".");
				p.CopyBuffer = null;
				return;
			}

			if (cpos.type == 1)
				for (ushort yy = minY; yy <= maxY; ++yy)
					for (ushort zz = minZ; zz <= maxZ; ++zz)
						for (ushort xx = minX; xx <= maxX; ++xx)
			{
				byte b = p.level.GetTile(xx, yy, zz);
				if (b != Block.air && Block.canPlace(p, b))
					p.level.Blockchange(p, xx, yy, zz, Block.air);
			}

			Player.SendMessage(p, (state.Volume - totalAir) + " blocks copied.");
			if (allowoffset != -1) {
				Player.SendMessage(p, "Place a block to determine where to paste from");
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
			if (!Player.ValidName(file)) {
				Player.SendMessage(p, "Bad file name");
				return;
			}
			
			if (!Directory.Exists("extra/savecopy"))
				Directory.CreateDirectory("extra/savecopy");
			if (!Directory.Exists("extra/savecopy/" + p.name))
				Directory.CreateDirectory("extra/savecopy/" + p.name);
			if (Directory.GetFiles("extra/savecopy/" + p.name).Length > 15) {
				Player.SendMessage(p, "You can only save a maxmium of 15 copies. /copy delete some.");
				return;
			}
			
			string path = "extra/savecopy/" + p.name + "/" + file + ".cpb";
			using (FileStream fs = new FileStream(path, FileMode.Create))
				using(GZipStream gs = new GZipStream(fs, CompressionMode.Compress))
			{
				p.CopyBuffer.SaveTo(gs);
			}
			Player.SendMessage(p, "Saved copy as " + file);
		}

		void LoadCopy(Player p, string file) {		
			string path = "extra/savecopy/" + p.name + "/" + file;
			bool existsNew = File.Exists(path + ".cpb");
			bool existsOld = File.Exists(path + ".cpy");
			if (!existsNew && !existsOld) {
				Player.SendMessage(p, "No such copy exists");
				return;
			}
			path = existsNew ? path + ".cpb" : path + ".cpy";

			using (FileStream fs = new FileStream(path, FileMode.Open))
				using(GZipStream gs = new GZipStream(fs, CompressionMode.Decompress))
			{
				CopyState state = new CopyState(0, 0, 0, 0, 0, 0, null);
				if (existsNew)
					state.LoadFrom(gs);
				else
					state.LoadFromOld(gs, fs);
				p.CopyBuffer = state;
			}
			Player.SendMessage(p, "Loaded copy as " + file);
		}
		
		struct CatchPos { public ushort x, y, z; public int type; public List<byte> ignoreTypes; }
		
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/copy - Copies the blocks in an area.");
			Player.SendMessage(p, "/copy save <save_name> - Saves what you have copied.");
			Player.SendMessage(p, "/copy load <load_name> - Loads what you have saved.");
			Player.SendMessage(p, "/copy delete <delete_name> - Deletes the specified copy.");
			Player.SendMessage(p, "/copy list - Lists all saved copies you have");
			Player.SendMessage(p, "/copy cut - Copies the blocks in an area, then removes them.");
			Player.SendMessage(p, "/copy air - Copies the blocks in an area, including air.");
			Player.SendMessage(p, "/copy ignore <block1> <block2>.. - Ignores <blocks> when copying");
			Player.SendMessage(p, "/copy @ - @ toggle for all the above, gives you a third click after copying that determines where to paste from");
		}
	}
}
