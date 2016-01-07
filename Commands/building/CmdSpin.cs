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
using System.Collections.Generic;
using MCGalaxy.Drawing;

namespace MCGalaxy.Commands
{
	public sealed class CmdSpin : Command
	{
		public override string name { get { return "spin"; } }
		public override string shortcut { get { return ""; } }
		public override string type { get { return CommandTypes.Building; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
		public CmdSpin() { }

		public override void Use(Player p, string message)
		{
			if (message.Split(' ').Length > 1) { Help(p); return; }
			if (message == "") message = "y";
			
			if (p.CopyBuffer == null) {
				Player.SendMessage(p, "You haven't copied anything yet"); return;
			}

			switch (message)
			{
				case "90":
				case "y":
					p.CopyBuffer = RotateY(p.CopyBuffer);
					goto case "m";
				case "180":
					FlipX(p.CopyBuffer); FlipZ(p.CopyBuffer); break;
				case "upsidedown":
				case "u":
					FlipY(p.CopyBuffer); break;
				case "mirror":
				case "m":
					FlipX(p.CopyBuffer); break;
				case "z":
					p.CopyBuffer = RotateZ(p.CopyBuffer); break;
				case "x":
					p.CopyBuffer = RotateX(p.CopyBuffer); break;

				default:
					Player.SendMessage(p, "Incorrect syntax");
					Help(p); return;
			}
			Player.SendMessage(p, "Spun: &b" + message);
		}
		
		CopyState RotateX(CopyState state) {
			CopyState newState = new CopyState(state.X, state.Y, state.Z,
			                                   state.Width, state.Length, state.Height);
			byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
			int newMaxY = newState.Height - 1;
			
			for (int i = 0; i < blocks.Length; i++) {
				ushort x, y, z;
				state.GetCoords(i, out x, out y, out z);
				newState.Set(x, newMaxY - z, y, blocks[i], extBlocks[i]);
			}
			newState.SetOrigin(state.OriginX, state.OriginY, state.OriginZ);
			return newState;
		}
		
		CopyState RotateY(CopyState state) {
			CopyState newState = new CopyState(state.X, state.Y, state.Z,
			                                   state.Length, state.Height, state.Width);
			byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
			
			for (int i = 0; i < blocks.Length; i++) {
				ushort x, y, z;
				state.GetCoords(i, out x, out y, out z);
				newState.Set(z, y, x, blocks[i], extBlocks[i]);
			}
			newState.SetOrigin(state.OriginX, state.OriginY, state.OriginZ);
			return newState;
		}
		
		CopyState RotateZ(CopyState state) {
			CopyState newState = new CopyState(state.X, state.Y, state.Z,
			                                   state.Height, state.Width, state.Length);
			byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
			int newMaxY = newState.Height - 1;
			
			for (int i = 0; i < blocks.Length; i++) {
				ushort x, y, z;
				state.GetCoords(i, out x, out y, out z);
				newState.Set(y, newMaxY - x, z, blocks[i], extBlocks[i]);
			}
			newState.SetOrigin(state.OriginX, state.OriginY, state.OriginZ);
			return newState;
		}
		
		void FlipX(CopyState state) {
			int midX = state.Width / 2, maxX = state.Width - 1;
			byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
			
			for (int y = 0; y < state.Height; y++) {			
				for (int z = 0; z < state.Length; z++) {
					for (int x = 0; x < midX; x++) {
						int endX = maxX - x;
						int start = state.GetIndex(x, y, z);
						int end = state.GetIndex(endX, y, z);
						Swap(blocks, extBlocks, start, end);
					}
				}
			}
		}
		
		void FlipY(CopyState state) {
			int midY = state.Height / 2, maxY = state.Height - 1;
			byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
			
			for (int y = 0; y < midY; y++) {
				int endY = maxY - y;
				int start = state.GetIndex(0, y, 0);
				int end = state.GetIndex(0, endY, 0);			
				for (int z = 0; z < state.Length; z++) {
					for (int x = 0; x < state.Width; x++) {
						Swap(blocks, extBlocks, start, end);
						start++; end++;
					}
				}
			}
		}
		
		void FlipZ(CopyState state) {
			int midZ = state.Width / 2, maxZ = state.Length - 1;
			byte[] blocks = state.Blocks, extBlocks = state.ExtBlocks;
			
			for (int y = 0; y < state.Height; y++) {			
				for (int z = 0; z < midZ; z++) {
					int endZ = maxZ - z;
					int start = state.GetIndex(0, y, z);
					int end = state.GetIndex(0, y, endZ);
					for (int x = 0; x < state.Width; x++) {						
						Swap(blocks, extBlocks, start, end);
						start++; end++;
					}
				}
			}
		}

		static void Swap(byte[] blocks, byte[] extBlocks, int a, int b) {
			byte tmp = blocks[a]; blocks[a] = blocks[b]; blocks[b] = tmp;
			tmp = extBlocks[a]; extBlocks[a] = extBlocks[b]; extBlocks[b] = tmp;
		}
		
		public override void Help(Player p) {
			Player.SendMessage(p, "/spin <y/180/mirror/upsidedown> - Spins the copied object.");
			Player.SendMessage(p, "Shotcuts: m for mirror, u for upside down, x for spin 90 on x, y for spin 90 on y, z for spin 90 on z.");
		}
	}
}
