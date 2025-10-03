﻿/*
    Copyright 2015-2024 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
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

namespace MCGalaxy.Levels.IO 
{    
    /// <summary> Reads/Loads block data (and potentially metadata) encoded in a particular format. </summary>
    public abstract class IMapImporter 
    {
        public abstract string Extension   { get; }
        public abstract string Description { get; }
        
        public virtual Level Read(string path, string name, bool metadata) {
            using (FileStream fs = File.OpenRead(path))
                return Read(fs, name, metadata);
        }
        
        public abstract Level Read(Stream src, string name, bool metadata);

        public virtual Vec3U16 ReadDimensions(string path) {
            using (FileStream fs = File.OpenRead(path))
                return ReadDimensions(fs);
        }

        public abstract Vec3U16 ReadDimensions(Stream src);
        
        
        protected static void ConvertCustom(Level lvl) {
            ushort x, y, z;
            byte[] blocks = lvl.blocks; // local var to avoid JIT bounds check
            for (int i = 0; i < blocks.Length; i++) {
                byte raw = blocks[i];
                if (raw <= Block.CPE_MAX_BLOCK) continue;
                
                blocks[i] = Block.custom_block;
                lvl.IntToPos(i, out x, out y, out z);
                lvl.FastSetExtTile(x, y, z, raw);
            }
        }
         
        
        /// <summary> List of all level format importers </summary>
        public static List<IMapImporter> Formats = new List<IMapImporter>() {
            new LvlImporter(), new CwImporter(), new FcmImporter(), new McfImporter(), 
            new DatImporter(), new McLevelImporter(),
        };
        
        /// <summary> Returns an IMapImporter capable of decoding the given level file </summary>
        /// <remarks> Determines importer suitability by comparing file extensions </remarks>
        /// <remarks> A suitable IMapImporter, or null if no suitable importer is found </remarks>
        public static IMapImporter GetFor(string path) {
            foreach (IMapImporter imp in Formats) 
            {
                if (path.CaselessEnds(imp.Extension)) return imp;
            }
            return null;
        }
        
        /// <summary> Decodes the given level file into a Level instance </summary>
        public static Level Decode(string path, string name, bool metadata) {
            IMapImporter imp = GetFor(path) ?? Formats[0];
            return imp.Read(path, name, metadata);
        }
    }

    /// <summary> Writes/Saves block data (and potentially metadata) encoded in a particular format. </summary>
    public abstract class IMapExporter 
    {
        public abstract string Extension { get; }
        
        public void Write(string path, Level lvl) {
            using (FileStream fs = File.Create(path)) {
                Write(fs, lvl);
            }
        }
        
        public abstract void Write(Stream dst, Level lvl);
        
        public static List<IMapExporter> Formats = new List<IMapExporter>() {
            new LvlExporter()
        };
    }
}
