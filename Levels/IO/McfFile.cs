/*
Copyright (C) 2010-2013 David Mitchell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */
using System;
using System.IO;
using System.IO.Compression;

namespace MCGalaxy.Levels.IO {    
    public static class McfFile {
        
        public static Level Load(Stream stream, string name) {
            GZipStream gs = new GZipStream(stream, CompressionMode.Decompress);
            byte[] ver = new byte[2];
            gs.Read(ver, 0, ver.Length);

            if (BitConverter.ToUInt16(ver, 0) != 1874)
                throw new InvalidDataException(".mcf files must have a version of 1874");
            
            byte[] header = new byte[16];
            gs.Read(header, 0, header.Length);
            ushort width = BitConverter.ToUInt16(header, 0);
            ushort length = BitConverter.ToUInt16(header, 2);
            ushort height = BitConverter.ToUInt16(header, 4);

            Level lvl = new Level(name, width, height, length);
            lvl.permissionbuild = (LevelPermission)30;
            lvl.spawnx = BitConverter.ToUInt16(header, 6);
            lvl.spawnz = BitConverter.ToUInt16(header, 8);
            lvl.spawny = BitConverter.ToUInt16(header, 10);
            lvl.rotx = header[12];
            lvl.roty = header[13];

            byte[] blocks = new byte[2 * lvl.blocks.Length];
            gs.Read(blocks, 0, blocks.Length);
            for (int i = 0; i < blocks.Length / 2; ++i)
                lvl.blocks[i] = blocks[i * 2];
            
            gs.Close();
            lvl.Save(true);
            return lvl;
        }
    }
}