/*
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
using System.IO.Compression;
using System.Net;
using System.Text;
using MCGalaxy.Maths;

namespace MCGalaxy.Levels.IO 
{
    public sealed class DatImporter : IMapImporter 
    {
        public override string Extension { get { return ".dat"; } }
        public override string Description { get { return "Minecraft Classic map"; } }

        public override Vec3U16 ReadDimensions(Stream src) {
            throw new NotSupportedException();
        }
        
        public override Level Read(Stream src, string name, bool metadata) {
            using (GZipStream s = new GZipStream(src, CompressionMode.Decompress)) {
                Level lvl    = new Level(name, 0, 0, 0);
                JavaReader r = new JavaReader();
                r.src = new BinaryReader(s);
                
                int signature = r.ReadInt32();
                // Format version 0 - preclassic to classic 0.12
                //  (technically this format doesn't have a signature, 
                //   but 99% of such maps will start with these 4 bytes)
                if (signature == 0x01010101)
                    return ReadFormat0(lvl, s);
                
                // All valid .dat maps must start with these 4 bytes
                if (signature != 0x271BB788) 
                    throw new InvalidDataException("Invalid .dat map signature");
                
                switch (r.ReadUInt8())
                {
                    // Format version 1 - classic 0.13
                    case 0x01: return ReadFormat1(lvl, r);
                    // Format version 2 - classic 0.15 to 0.30
                    case 0x02: return ReadFormat2(lvl, r);
                }
                throw new InvalidDataException("Invalid .dat map version");
            }
        }
        
        
        // Map 'format' is just the 256x64x256 blocks of the level
        const int PC_WIDTH = 256, PC_HEIGHT = 64, PC_LENGTH = 256;
        static Level ReadFormat0(Level lvl, Stream s) {
            lvl.Width  = PC_WIDTH;
            lvl.Height = PC_HEIGHT;
            lvl.Length = PC_LENGTH;
            
            // First 4 bytes were already read earlier as signature
            byte[] blocks = new byte[PC_WIDTH * PC_HEIGHT * PC_LENGTH];
            blocks[0] = 1; blocks[1] = 1; blocks[2] = 1; blocks[3] = 1;
            s.Read(blocks, 4, blocks.Length - 4);
            lvl.blocks = blocks;
            
            SetupClassic013(lvl);
            // Similiar env to how it appears in preclassic client
            lvl.Config.EdgeBlock    = Block.Air;
            lvl.Config.HorizonBlock = Block.Air;
            return lvl;
        }
        
        
        static Level ReadFormat1(Level lvl, JavaReader r) {
            r.ReadUtf8();  // level name
            r.ReadUtf8();  // level author
            r.ReadInt64(); // created timestamp (currentTimeMillis)
            
            lvl.Width  = r.ReadUInt16();
            lvl.Length = r.ReadUInt16();
            lvl.Height = r.ReadUInt16();            
            lvl.blocks = r.ReadBytes(lvl.Width * lvl.Height * lvl.Length); // TODO readfully
            SetupClassic013(lvl);
            return lvl;
        }
        
        static void SetupClassic013(Level lvl) {
            // You always spawn in a random place in 0.13,
            //  so just use middle of map for spawn instead
            lvl.spawnx = (ushort)(lvl.Width  / 2);
            lvl.spawny = lvl.Height;
            lvl.spawnz = (ushort)(lvl.Length / 2);
            
            // Similiar env to how it appears in 0.13 client
            lvl.Config.CloudsHeight = -30000;
            lvl.Config.SkyColor     = "#7FCCFF";
            lvl.Config.FogColor     = "#7FCCFF";
        }

        
        // Really annoying map format to parse, because it's just a Java serialised object
        // See JavaDeserialiser.cs for the ugly details of parsing Java serialised objects
        static Level ReadFormat2(Level lvl, JavaReader r) {
            if (r.ReadUInt16() != 0xACED) throw new InvalidDataException("Invalid stream magic");
            if (r.ReadUInt16() != 0x0005) throw new InvalidDataException("Invalid stream version");
            
            JObject obj = (JObject)r.ReadObject();
            ParseRootObject(lvl, obj);
            return lvl;
        }
        
        // object is actually an int, so a simple cast to ushort will fail
        static ushort U16(object o) { return (ushort)((int)o); }
        
        static void ParseRootObject(Level lvl, JObject obj) {
            JFieldDesc[] fields = obj.Desc.Fields;
            object[] values     = obj.ClassData[0].Values;
            
            for (int i = 0; i < fields.Length; i++) {
                JFieldDesc f = fields[i];
                object value = values[i];
                
                // yes height/depth are swapped intentionally
                if (f.Name == "width")  lvl.Width  = U16(value);
                if (f.Name == "height") lvl.Length = U16(value);
                if (f.Name == "depth")  lvl.Height = U16(value);
                if (f.Name == "blocks") lvl.blocks = (byte[])((JArray)value).Values;
                if (f.Name == "xSpawn") lvl.spawnx = U16(value);
                if (f.Name == "ySpawn") lvl.spawny = U16(value);
                if (f.Name == "zSpawn") lvl.spawnz = U16(value);
            }
        }
    }
}