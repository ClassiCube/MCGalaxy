/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Levels.IO {

    //WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
    //You MUST make it able to save and load as a new version other wise you will make old levels incompatible!
    public static class LvlFile {
        
        public static void Save(Level level, string file) {
            using (Stream fs = File.Create(file), gs = new GZipStream(fs, CompressionMode.Compress, true))
            {
                byte[] header = new byte[16];
                BitConverter.GetBytes(1874).CopyTo(header, 0);
                gs.Write(header, 0, 2);

                BitConverter.GetBytes(level.Width).CopyTo(header, 0);
                BitConverter.GetBytes(level.Length).CopyTo(header, 2);
                BitConverter.GetBytes(level.Height).CopyTo(header, 4);
                BitConverter.GetBytes(level.spawnx).CopyTo(header, 6);
                BitConverter.GetBytes(level.spawnz).CopyTo(header, 8);
                BitConverter.GetBytes(level.spawny).CopyTo(header, 10);
                header[12] = level.rotx;
                header[13] = level.roty;
                header[14] = (byte)level.permissionvisit;
                header[15] = (byte)level.permissionbuild;
                gs.Write(header, 0, header.Length);
                byte[] blocks = level.blocks;
                int start = 0, len = 0;

                for (int i = 0; i < blocks.Length; ++i) {
                    byte block = blocks[i], convBlock = 0;
                    if (block < Block.CpeCount || (convBlock = Block.SaveConvert(block)) == block) {
                    	if (len == 0) start = i;
                        len++;
                    } else {
                    	if (len > 0) gs.Write(blocks, start, len);
                    	len = 0;
                    	gs.WriteByte(convBlock);
                    }
                }
                if (len > 0) gs.Write(blocks, start, len);
                
                // write out new blockdefinitions data
                gs.WriteByte(0xBD);
                int index = 0;
                for (int y = 0; y < level.ChunksY; y++)
                	for (int z = 0; z < level.ChunksZ; z++)
                		for (int x = 0; x < level.ChunksX; x++)
                {
                    byte[] chunk = level.CustomBlocks[index];
                    if (chunk == null) {
                        gs.WriteByte(0);
                    } else {
                        gs.WriteByte(1);
                        gs.Write(chunk, 0, chunk.Length);
                    }
                    index++;
                }
            }
        }
        
        public static Level Load(string name, string file) {
            using (Stream fs = File.OpenRead(file), gs = new GZipStream(fs, CompressionMode.Decompress, true))
            {
                byte[] header = new byte[16];
                gs.Read(header, 0, 2);
                ushort[] vars = new ushort[6];
                vars[0] = BitConverter.ToUInt16(header, 0);

                int offset = 0;
                if (vars[0] == 1874) { // version field, width is next ushort
                    gs.Read(header, 0, 16);
                    vars[0] = BitConverter.ToUInt16(header, 0);
                    offset = 2;
                } else {
                    gs.Read(header, 0, 12);
                }
                vars[1] = BitConverter.ToUInt16(header, offset);
                vars[2] = BitConverter.ToUInt16(header, offset + 2);

                Level level = new Level(name, vars[0], vars[2], vars[1], "full_empty");
                level.spawnx = BitConverter.ToUInt16(header, offset + 4);
                level.spawnz = BitConverter.ToUInt16(header, offset + 6);
                level.spawny = BitConverter.ToUInt16(header, offset + 8);
                level.rotx = header[offset + 10];
                level.roty = header[offset + 11];
                
                gs.Read(level.blocks, 0, level.blocks.Length);
                if (gs.ReadByte() != 0xBD) 
                    return level;
                
                int index = 0;
                for (int y = 0; y < level.ChunksY; y++)
                	for (int z = 0; z < level.ChunksZ; z++)
                		for (int x = 0; x < level.ChunksX; x++)
                {
                    if (gs.ReadByte() == 1) {
                        byte[] chunk = new byte[16 * 16 * 16];
                        gs.Read(chunk, 0, chunk.Length);
                        level.CustomBlocks[index] = chunk;
                    }
                    index++;
                }
                return level;
            }
        }
		
        public static void LoadDimensions(string file, out ushort width, out ushort height, out ushort length) {
            using (Stream fs = File.OpenRead(file), gs = new GZipStream(fs, CompressionMode.Decompress, true))
            {
                byte[] header = new byte[16];
                gs.Read(header, 0, 2);
                ushort[] vars = new ushort[6];
                vars[0] = BitConverter.ToUInt16(header, 0);

                int offset = 0;
                if (vars[0] == 1874) { // version field, width is next ushort
                    gs.Read(header, 0, 16);
                    vars[0] = BitConverter.ToUInt16(header, 0);
                    offset = 2;
                } else {
                    gs.Read(header, 0, 12);
                }
                vars[1] = BitConverter.ToUInt16(header, offset);
                vars[2] = BitConverter.ToUInt16(header, offset + 2);
                width = vars[0]; height = vars[2]; length = vars[1];
            }
        }
    }
}