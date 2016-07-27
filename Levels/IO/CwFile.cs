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
using fNbt;

namespace MCGalaxy.Levels.IO {   
    public static class CwFile {

        public static Level Load(Stream stream, string name) {
            NbtFile file = new NbtFile();
            file.LoadFromStream(stream, NbtCompression.GZip);
            
            NbtCompound root = file.RootTag;
            if (root["FormatVersion"].ByteValue > 1)
                throw new NotSupportedException("Only version 1 of ClassicWorld format is supported.");
            
            short x = root["X"].ShortValue, y = root["Y"].ShortValue, z = root["Z"].ShortValue;
            if (x <= 0 || y <= 0 || z <= 0)
                throw new InvalidDataException("Level dimensions must be > 0.");
            
            Level lvl = new Level(name, (ushort)x, (ushort)y, (ushort)z);
            lvl.blocks = root["BlockArray"].ByteArrayValue;
            FcmFile.ConvertExtended(lvl);
            return lvl;
        }
    }
}