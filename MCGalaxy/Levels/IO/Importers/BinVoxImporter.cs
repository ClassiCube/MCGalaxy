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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace MCGalaxy.Levels.IO {
    public sealed class BinVoxImporter : IMapImporter {

        public override string Extension { get { return ".binvox"; } }
        
        public override Vec3U16 ReadDimensions(Stream src) {
            BinaryReader reader = new BinaryReader(src);
            return ReadHeader(reader);
        }
        
        public override Level Read(Stream src, string name, bool metadata) {
            BinaryReader reader = new BinaryReader(src);
            Vec3U16 dims = ReadHeader(reader);
            Level lvl = new Level(name, dims.X, dims.Y, dims.Z);
            lvl.EdgeLevel = 0;
            lvl.SidesLevel = lvl.EdgeLevel - 2;

            byte[] blocks = lvl.blocks;
            int size = dims.X * dims.Y * dims.Z, i = 0;
            while (i < size) {
                byte value = reader.ReadByte(), count = reader.ReadByte();
                if (value == Block.air) { i += count; continue; } // skip redundantly changing air
                
                for (int j = 0; j < count; j++) {
                    int index = i + j;
                    int x = (index / dims.Y) / dims.Z; // need to reorder from X Z Y to Y Z X
                    int y = (index / dims.Y) % dims.Z;
                    int z = index % dims.Z;
                    lvl.blocks[(y * dims.Z + z) * dims.X + x] = value;
                }
                i += count;
            }
            return lvl;
        }
        
        // http://www.patrickmin.com/binvox/binvox.html
        // http://www.patrickmin.com/binvox/ReadBinvox.java
        static Vec3U16 ReadHeader(BinaryReader reader) {
            Vec3U16 dims = default(Vec3U16);
            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                string line = ReadString(reader);
                if (line.CaselessEq("data")) break;
                if (!line.CaselessStarts("dim ")) continue;
                
                string[] parts = line.SplitSpaces(4);
                dims.Z = ushort.Parse(parts[1]);
                dims.Y = ushort.Parse(parts[2]);
                dims.X = ushort.Parse(parts[3]);
            }
            return dims;
        }

        static string ReadString(BinaryReader reader) {
            List<byte> buffer = new List<byte>();
            
            byte value = 0;
            do {
                value = reader.ReadByte();
                buffer.Add(value);
            } while (value != '\n');
            
            return Encoding.ASCII.GetString(buffer.ToArray()).Trim('\r', '\n');
        }
    }
}