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
using System.IO.Compression;
using System.Net;
using System.Text;

namespace MCGalaxy.Levels.IO {
    
    public static class FcmFile {

		public static Level Load(Stream stream, string name) {
			BinaryReader reader = new BinaryReader(stream);
			if (reader.ReadInt32() != 0x0FC2AF40 || reader.ReadByte() != 13) {
				throw new InvalidDataException( "Unexpected constant in .fcm file" );
			}
			
			ushort width = reader.ReadUInt16();
			ushort height = reader.ReadUInt16();
			ushort length = reader.ReadUInt16();
			Level lvl = new Level(name, width, height, length);

			lvl.spawnx = (ushort)(reader.ReadInt32() / 32);
			lvl.spawny = (ushort)(reader.ReadInt32() / 32);
			lvl.spawnz = (ushort)(reader.ReadInt32() / 32);
			lvl.rotx = reader.ReadByte();
			lvl.roty = reader.ReadByte();

			reader.ReadUInt32(); // date modified
			reader.ReadUInt32(); // date created
			reader.ReadBytes(16); // uuid
			reader.ReadBytes(26); // layer index
			int metaSize = reader.ReadInt32();			

			using (DeflateStream ds = new DeflateStream(stream, CompressionMode.Decompress)) {
				reader = new BinaryReader(ds);
				for (int i = 0; i < metaSize; i++) {
					string group = ReadString(reader);
					string key = ReadString(reader);
					string value = ReadString(reader);
				}
				int read = ds.Read(lvl.blocks, 0, lvl.blocks.Length);
			}
			Upgrade(lvl);
			lvl.Save(true);
            return lvl;
		}
		
		static void Upgrade(Level lvl) {
			ushort x, y, z;
			for (int i = 0; i < lvl.blocks.Length; i++) {
				byte block = lvl.blocks[i];
				if (block <= Block.CpeMaxBlock) continue;
				
				lvl.blocks[i] = Block.custom_block;
				lvl.IntToPos(i, out x, out y, out z);
				lvl.SetExtTile(x, y, z, block);
			}
		}
		
		static string ReadString(BinaryReader reader) {
			int length = reader.ReadUInt16();
			byte[] data = reader.ReadBytes(length);
			return Encoding.ASCII.GetString(data);
		}
	}
}