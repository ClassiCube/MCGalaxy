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
using MCGalaxy.Maths;

namespace MCGalaxy.Levels.IO {
    
    /// <summary> Reads/Loads block data (and potentially metadata) encoded in a particular format. </summary>
    public abstract class IMapImporter {
        
        /// <summary> The file extension of this format. </summary>
        public abstract string Extension { get; }
        
        /// <summary> Reads the data for a level from a file containing data encoded in this format. </summary>
        /// <param name="metadata"> Whether metadata should be loaded. </param>
        public Level Read(string path, string name, bool metadata) {
            using (FileStream fs = File.OpenRead(path))
                return Read(fs, name, metadata);
        }
        
        /// <summary> Reads the data for a level from a file containing data encoded in this format. </summary>
        /// <param name="metadata"> Whether metadata should be loaded. </param>
        public abstract Level Read(Stream src, string name, bool metadata);

        /// <summary> Reads the dimensions for a level from a file containing data encoded in this format. </summary>
        public Vec3U16 ReadDimensions(string path) {
            using (FileStream fs = File.OpenRead(path))
                return ReadDimensions(fs);
        }
        
        /// <summary> Reads the dimensions for a level from a file containing data encoded in this format. </summary>
        public abstract Vec3U16 ReadDimensions(Stream src);
        
        public static List<IMapImporter> Formats = new List<IMapImporter>() {
            new LvlImporter(), new CwImporter(), new FcmImporter(), new McfImporter(), 
            new DatImporter(), new BinVoxImporter(), new McLevelImporter(),
        };
        
        protected void ConvertCustom(Level lvl) {
            ushort x, y, z;
            byte[] blocks = lvl.blocks;
            for (int i = 0; i < blocks.Length; i++) {
                byte raw = blocks[i];
                if (raw <= Block.CpeMaxBlock) continue;
                
                blocks[i] = Block.custom_block;
                lvl.IntToPos(i, out x, out y, out z);
                lvl.SetExtTileNoCheck(x, y, z, raw);
            }
        }
    }

    /// <summary> Writes/Saves block data (and potentially metadata) encoded in a particular format. </summary>
    public abstract class IMapExporter {

        /// <summary> The file extension of this format. </summary>
        public abstract string Extension { get; }
        
        /// <summary> Saves the data encoded in this format for a level to a file. </summary>
        public void Write(string path, Level lvl) {
            using (FileStream fs = File.Create(path)) {
                Write(fs, lvl);
            }
        }
        
        /// <summary> Saves the data encoded in this format for a level to a file. </summary>
        public abstract void Write(Stream dst, Level lvl);
        
        public static List<IMapExporter> Formats = new List<IMapExporter>() {
            new LvlExporter()
        };
    }
}
