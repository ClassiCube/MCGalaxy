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
//StormCom Object Generator
//
//Full use to all StormCom Server System codes (in regards to minecraft classic) have been granted to MCGalaxy without restriction.
//
// ~Merlin33069
using System;
using System.Collections.Generic;

namespace MCGalaxy.Util {
	
	public sealed class SCOGenerator {
		
		public static void Cone(Player p, ushort x, ushort y, ushort z, int height, int radius, byte type, byte extType, bool invert) {
			List<Pos> buffer = new List<Pos>();
			Pos temp = new Pos();
			for (short yy = 0; yy <= height; yy++)
				for (short zz = (short)-radius; zz <= radius; zz++)
					for (short xx = (short)-radius; xx <= radius; xx++)
			{
				int cx = (x + xx), cy = (y + yy), cz = (z + zz);
				int curHeight = invert ? yy : height - yy;
				if (curHeight == 0)
					continue;
				double curRadius = radius * ((double)curHeight / (double)height);
				int dist = xx * xx + zz * zz;
				if (dist > curRadius * curRadius)
					continue;
				
				byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
				if (ctile == 0) {
					temp.x = (ushort)cx; temp.y = (ushort)cy; temp.z = (ushort)cz;
					buffer.Add(temp);
				}
			}
			Draw(ref buffer, p, x, y, z, height, type, extType, invert);
		}
		
		public static void HCone(Player p, ushort x, ushort y, ushort z, int height, int radius, byte type, byte extType, bool invert) {
			List<Pos> buffer = new List<Pos>();
			Pos temp = new Pos();
			for (short yy = 0; yy <= height; yy++)
				for (short zz = (short)-radius; zz <= radius; zz++)
					for (short xx = (short)-radius; xx <= radius; xx++)
			{
				int cx = (x + xx), cy = (y + yy), cz = (z + zz);
				int curHeight = invert ? yy : height - yy;
				if (curHeight == 0)
					continue;
				double curRadius = radius * ((double)curHeight / (double)height);
				int dist = xx * xx + zz * zz;
				if (dist > curRadius * curRadius || dist < (curRadius - 1) * (curRadius - 1))
					continue;
				
				byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
				if (ctile == 0) {
					temp.x = (ushort)cx; temp.y = (ushort)cy; temp.z = (ushort)cz;
					buffer.Add(temp);
				}
			}
			Draw(ref buffer, p, x, y, z, height, type, extType, invert);
		}

		//For the pyramid commands, Radius still refers to the distance from the center point, but is axis independant, rather than a referance to both axes
		public static void Pyramid(Player p, ushort x, ushort y, ushort z, int height, int radius, byte type, byte extType, bool invert) {
			List<Pos> buffer = new List<Pos>();
			Pos temp = new Pos();
			for (short yy = 0; yy <= height; yy++)
				for (short zz = (short)-radius; zz <= radius; zz++)
					for (short xx = (short)-radius; xx <= radius; xx++)
			{
				int cx = (x + xx), cy = (y + yy), cz = (z + zz);
				int curHeight = invert ? yy : height - yy;
				if (curHeight == 0)
					continue;
				double curRadius = radius * ((double)curHeight / (double)height);
				if (Math.Abs(xx) > curRadius || Math.Abs(zz) > curRadius)
					continue;

				byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
				if (ctile == 0){
					temp.x = (ushort)cx; temp.y = (ushort)cy; temp.z = (ushort)cz;
					buffer.Add(temp);
				}
			}
			Draw(ref buffer, p, x, y, z, height, type, extType, invert);
		}
		
		public static void HPyramid(Player p, ushort x, ushort y, ushort z, int height, int radius, byte type, byte extType, bool invert) {
			List<Pos> buffer = new List<Pos>();
			Pos temp = new Pos();
			for (short yy = 0; yy <= height; yy++)
				for (short zz = (short)-radius; zz <= radius; zz++)
					for (short xx = (short)-radius; xx <= radius; xx++)
			{
				int cx = (x + xx), cy = (y + yy), cz = (z + zz);
				int curHeight = invert ? yy : height - yy;
				if (curHeight == 0)
					continue;
				double curRadius = radius * ((double)curHeight / (double)height);
				int absx = Math.Abs(xx), absz = Math.Abs(zz);
				if (absx > curRadius || absz > curRadius) continue;
				if (absx < (curRadius - 1) && absz < (curRadius - 1)) continue;

				byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
				if (ctile == 0) {
					temp.x = (ushort)cx; temp.y = (ushort)cy; temp.z = (ushort)cz;
					buffer.Add(temp);
				}
			}
			Draw(ref buffer, p, x, y, z, height, type, extType, invert);
		}
		
		static void Draw(ref List<Pos> buffer, Player p, ushort x, ushort y, ushort z, int height, byte type, byte extType, bool invert) {
			if (buffer.Count > p.group.maxBlocks) {
				Player.SendMessage(p, "You tried drawing " + buffer.Count + " blocks, your limit is " + p.group.maxBlocks); buffer = null; return;
			}
			buffer.ForEach(delegate(Pos pos) { p.level.UpdateBlock(p, pos.x, pos.y, pos.z, type, extType); });
			buffer = null;
			if (invert)
				p.level.UpdateBlock(p, x, y, z, type, extType);
			else
				p.level.UpdateBlock(p, x, (ushort)(y + height), z, type, extType);
		}

		public static void Sphere(Player p, ushort x, ushort y, ushort z, int radius, byte type, byte extType) {
			int upper = (radius + 1) * (radius + 1);
			List<Pos> buffer = new List<Pos>();
			Pos temp = new Pos();
			for (short yy = (short)-radius; yy <= radius; yy++)
				for (short zz = (short)-radius; zz <= radius; zz++)
					for (short xx = (short)-radius; xx <= radius; xx++)
			{
				int curDist = xx * xx + yy * yy + zz * zz;
				if (curDist < upper) {
					temp.x = (ushort)(x + xx); temp.y = (ushort)(y + yy); temp.z = (ushort)(z + zz);
					buffer.Add(temp);
				}
			}
			if (buffer.Count > p.group.maxBlocks) { Player.SendMessage(p, "You tried Sphering " + buffer.Count + " blocks, your limit is " + p.group.maxBlocks); buffer = null; return; }
			buffer.ForEach(delegate(Pos pos) { p.level.UpdateBlock(p, pos.x, pos.y, pos.z, type, extType); });
			buffer = null;
		}
		
		public static void HSphere(Player p, ushort x, ushort y, ushort z, int radius, byte type, byte extType) {
			int upper = (radius + 1) * (radius + 1), inner = (radius - 1) * (radius - 1);
			List<Pos> buffer = new List<Pos>();
			Pos temp = new Pos();
			for (short yy = (short)-radius; yy <= radius; yy++)
				for (short zz = (short)-radius; zz <= radius; zz++)
					for (short xx = (short)-radius; xx <= radius; xx++)
			{
				int curDist = xx * xx + yy * yy + zz * zz;
				if (curDist < upper && curDist >= inner) {
					temp.x = (ushort)(x + xx); temp.y = (ushort)(y + yy); temp.z = (ushort)(z + zz);
					buffer.Add(temp);
				}
			}
			if (buffer.Count > p.group.maxBlocks) { Player.SendMessage(p, "You tried HSphering " + buffer.Count + " blocks, your limit is " + p.group.maxBlocks); buffer = null; return; }
			buffer.ForEach(delegate(Pos pos) { p.level.UpdateBlock(p, pos.x, pos.y, pos.z, type, extType); });
			buffer = null;
		}

		public static void Volcano(Player p, ushort x, ushort y, ushort z, int height, int radius) {
			List<CatchPos> buffer = new List<CatchPos>();
			CatchPos temp = new CatchPos();
			for (short yy = 0; yy <= height; yy++)
				for (short zz = (short)-radius; zz <= radius; zz++)
					for (short xx = (short)-radius; xx <= radius; xx++)
			{
				int cx = (x + xx), cy = (y + yy), cz = (z + zz);
				int curHeight = height - yy;
				if (curHeight == 0)
					continue;
				double curRadius = radius * ((double)curHeight / (double)height);
				int dist = xx * xx + zz * zz;
				if (dist > curRadius * curRadius)
					continue;
				
				byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
				if (ctile == 0) {
					temp.x = (ushort)cx; temp.y = (ushort)cy; temp.z = (ushort)cz;
					bool layer = dist >= (curRadius - 1) * (curRadius - 1);
					temp.type = layer ? Block.grass : Block.lavastill;
					buffer.Add(temp);
				}
			}
			if (buffer.Count > p.group.maxBlocks) { Player.SendMessage(p, "You tried Valcanoing " + buffer.Count + " blocks, your limit is " + p.group.maxBlocks); buffer = null; return; }
			buffer.ForEach(delegate(CatchPos pos) { p.level.Blockchange(p, pos.x, pos.y, pos.z, pos.type); });
			buffer = null;
		}
		struct CatchPos { public ushort x, y, z; public byte type; }
		struct Pos { public ushort x, y, z; }
	}
}