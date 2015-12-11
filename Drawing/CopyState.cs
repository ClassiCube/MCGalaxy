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
using System.IO;

namespace MCGalaxy.Drawing {

	public sealed class CopyState {
		
		public byte[] Blocks;
		public int X, Y, Z;
		public int Width, Height, Length;
		
		const int identifier = 0x434F5059; // 'COPY'
		
		public int Volume {
			get { return Width * Height * Length; }
		}
		
		public CopyState(int x, int y, int z, int width, int height, int length, byte[] blocks) {
			X = x; Y = y; Z = z;
			Width = width; Height = height; Length = length;
			Blocks = blocks;
		}
		
		public CopyState(int x, int y, int z, int width, int height, int length)
			: this(x, y, z, width, height, length, null) {
			Blocks = new byte[width * height * length];
		}
		
		public void Clear() {
			Blocks = null;
		}
		
		public void GetCoords(int index, out ushort x, out ushort y, out ushort z) {
			y = (ushort)(index / Width / Length);
			index -= y * Width * Length;
			z = (ushort)(index / Width);
			index -= z * Width;
			x = (ushort)index;
		}
		
		public int GetIndex(int x, int y, int z) {
			return (y * Length + z) * Width + x;
		}
		
		public void Set(int x, int y, int z, byte block) {
			Blocks[(y * Length + z) * Width + x] = block;
		}
		
		public void SaveTo(Stream stream) {
			BinaryWriter w = new BinaryWriter(stream);
			w.Write(identifier);
			
			w.Write(X); w.Write(Y); w.Write(Z);
			w.Write(Width); w.Write(Height); w.Write(Length);
			w.Write(Blocks);
		}
		
		public void LoadFrom(Stream stream) {
			BinaryReader r = new BinaryReader(stream);
			if (r.ReadInt32() != identifier)
				throw new InvalidDataException("invalid identifier");
			
			X = r.ReadInt32(); Y = r.ReadInt32(); Z = r.ReadInt32();
			Width = r.ReadInt32(); Height = r.ReadInt32(); Length = r.ReadInt32();
			Blocks = r.ReadBytes(Width * Height * Length);
		}
		
		public void LoadFromOld(Stream stream, Stream underlying) {
			byte[] raw = new byte[underlying.Length];
			underlying.Read(raw, 0, (int)underlying.Length);
			raw = raw.Decompress();
			if (raw.Length == 0) return;
			
			CalculateBoundsOld(raw);
			for (int i = 0; i < raw.Length; i += 7) {
				ushort x = BitConverter.ToUInt16(raw, i + 0);
				ushort y = BitConverter.ToUInt16(raw, i + 2);
				ushort z = BitConverter.ToUInt16(raw, i + 4);
				byte type = raw[i + 6];
				Set(x - X, y - Y, z - Z, type);
			}
		}
		
		void CalculateBoundsOld(byte[] raw) {
			int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
			int maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;
			for (int i = 0; i < raw.Length; i += 7) {
				ushort x = BitConverter.ToUInt16(raw, i + 0);
				ushort y = BitConverter.ToUInt16(raw, i + 2);
				ushort z = BitConverter.ToUInt16(raw, i + 4);
				
				minX = Math.Min(x, minX); maxX = Math.Max(x, maxX);
				minY = Math.Min(y, minY); maxY = Math.Max(y, maxY);
				minZ = Math.Min(z, minZ); maxZ = Math.Max(z, maxZ);
				
			}
			X = minX; Y = minY; Z = minZ;
			Width = maxX - minX + 1;
			Height = maxY - minY + 1;
			Length = maxZ - minZ + 1;
			Blocks = new byte[Width * Height * Length];
		}
	}
}
