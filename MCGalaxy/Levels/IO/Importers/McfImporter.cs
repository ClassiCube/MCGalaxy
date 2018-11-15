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
using MCGalaxy.Maths;

namespace MCGalaxy.Levels.IO {
    public sealed class McfImporter : IMapImporter {

        public override string Extension { get { return ".mcf"; } }
        public override string Description { get { return "MCForge redux map"; } }

        public override Vec3U16 ReadDimensions(Stream src) {
            using (Stream gs = new GZipStream(src, CompressionMode.Decompress, true)) {
                byte[] header = new byte[16];
                return ReadHeader(header, gs);
            }
        }
        
        public override Level Read(Stream src, string name, bool metadata) {
            using (Stream gs = new GZipStream(src, CompressionMode.Decompress)) {
                byte[] header = new byte[16];
                Vec3U16 dims = ReadHeader(header, gs);

                Level lvl = new Level(name, dims.X, dims.Y, dims.Z);
                lvl.spawnx = BitConverter.ToUInt16(header, 6);
                lvl.spawnz = BitConverter.ToUInt16(header, 8);
                lvl.spawny = BitConverter.ToUInt16(header, 10);
                lvl.rotx = header[12]; lvl.roty = header[13];
                // 2 bytes for perbuild and pervisit

                byte[] blocks = new byte[2 * lvl.blocks.Length];
                gs.Read(blocks, 0, blocks.Length);
                for (int i = 0; i < blocks.Length / 2; ++i)
                    lvl.blocks[i] = blocks[i * 2];
                return lvl;
            }
        }
        
        static Vec3U16 ReadHeader(byte[] header, Stream gs) {
            ReadFully(gs, header, 2);
            if (BitConverter.ToUInt16(header, 0) != 1874)
                throw new InvalidDataException(".mcf files must have a version of 1874");
            
            ReadFully(gs, header, 16);
            Vec3U16 dims;
            dims.X = BitConverter.ToUInt16(header, 0);
            dims.Z = BitConverter.ToUInt16(header, 2);
            dims.Y = BitConverter.ToUInt16(header, 4);
            return dims;
        }
    }
}