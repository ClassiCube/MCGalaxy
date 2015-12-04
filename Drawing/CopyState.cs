/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
		
		public ushort[] MinCoords;
		public ushort[] MaxCoords;
		public byte[] Blocks;
		
		int width, height, length;
		const int identifier = 0x434F5059; // 'COPY'
		
		public int Volume {
			get { return width * height * length; }
		}
		
		public CopyState() { }
		
		public CopyState(ushort minX, ushort minY, ushort minZ,
		                 ushort maxX, ushort maxY, ushort maxZ) {
			SetBounds(minX, minY, minZ, maxX, maxY, maxZ);
			width = maxX - minX + 1;
			height = maxY - minY + 1;
			length = maxZ - minZ + 1;
			Blocks = new byte[width * height * length];
		}
		
		public void SetBounds(ushort minX, ushort minY, ushort minZ,
		                      ushort maxX, ushort maxY, ushort maxZ) {
			MinCoords = new [] { minX, minY, minZ };
			MaxCoords = new [] { maxX, maxY, maxZ };
		}
		
		public void Clear() {
			Blocks = null;
			MinCoords = null;
			MaxCoords = null;
		}
		
		public void GetCoords(int index, out ushort x, out ushort y, out ushort z) {
			y = (ushort)(index / width / length);
			index -= y * width * length;
			z = (ushort)(index / width);
			index -= z * width;
			x = (ushort)index;
		}
		
		public void SaveTo(Stream stream) {
			BinaryWriter w = new BinaryWriter(stream);
			w.Write(identifier);
			w.Write(MinCoords[0]); w.Write(MinCoords[1]); w.Write(MinCoords[2]);
			w.Write(MaxCoords[0]); w.Write(MaxCoords[1]); w.Write(MaxCoords[2]);
			
			w.Write(width);
			w.Write(height);
			w.Write(length);
			w.Write(Blocks);
		}
		
		public void LoadFrom(Stream stream) {
			BinaryReader r = new BinaryReader(stream);
			if (r.ReadInt32() != identifier)
				throw new InvalidDataException("invalid identifier");
			ushort minX = r.ReadUInt16(), minY = r.ReadUInt16(), minZ = r.ReadUInt16();
			ushort maxX = r.ReadUInt16(), maxY = r.ReadUInt16(), maxZ = r.ReadUInt16();
			SetBounds( minX, minY, minZ, maxX, maxY, maxZ );
			
			width = r.ReadInt32();
			height = r.ReadInt32();
			length = r.ReadInt32();
			Blocks = r.ReadBytes(width * height * length);
		}
	}
}
