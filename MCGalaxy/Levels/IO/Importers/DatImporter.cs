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
using System.Net;
using System.Text;
using MCGalaxy.Maths;

namespace MCGalaxy.Levels.IO {
    sealed class DatReader {
        public BinaryReader src;
        public List<object> handles = new List<object>();
        public byte[] ReadBytes(int count) { return src.ReadBytes(count); }
        
        public byte ReadUInt8() { return src.ReadByte(); }
        public short ReadInt16() { return IPAddress.HostToNetworkOrder(src.ReadInt16()); }
        public ushort ReadUInt16() { return (ushort)IPAddress.HostToNetworkOrder(src.ReadInt16()); }
        public int ReadInt32() { return IPAddress.HostToNetworkOrder(src.ReadInt32()); }
        public long ReadInt64() { return IPAddress.HostToNetworkOrder(src.ReadInt64()); }
        public string ReadUtf8() { return Encoding.UTF8.GetString(src.ReadBytes(ReadUInt16())); }
    }
    
    public sealed class DatImporter : IMapImporter {
        public override string Extension { get { return ".dat"; } }
        public override string Description { get { return "Minecraft Classic map"; } }

        public override Vec3U16 ReadDimensions(Stream src) {
            throw new NotSupportedException();
        }
        
        const byte TC_NULL = 0x70;
        const byte TC_REFERENCE = 0x71;
        const byte TC_CLASSDESC = 0x72;
        const byte TC_OBJECT = 0x73;
        const byte TC_STRING = 0x74;
        const byte TC_ARRAY = 0x75;
        const byte TC_CLASS = 0x76;
        const byte TC_BLOCKDATA = 0x77;
        const byte TC_ENDBLOCKDATA = 0x78;
        const byte TC_RESET = 0x79;
        const byte TC_BLOCKDATALONG = 0x7A;
        
        const int baseWireHandle = 0x7E0000;
        const byte SC_WRITE_METHOD = 0x01, SC_SERIALIZABLE = 0x02;

        class JClassDesc {
            public string Name;
            public byte Flags;
            public JFieldDesc[] Fields;
            public JClassDesc SuperClass;
        }
        
        class JClassData {
            public object[] Values;
        }
        
        class JObject {
            public JClassDesc Desc;
            public JClassData[] ClassData;
        }
        class JArray {
            public JClassDesc Desc;
            public object Values;
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
        
        public override Level Read(Stream src, string name, bool metadata) {
            using (GZipStream s = new GZipStream(src, CompressionMode.Decompress)) {
                Level lvl   = new Level(name, 0, 0, 0);
                DatReader r = new DatReader();
                r.src = new BinaryReader(s);
                
                if (r.ReadInt32() != 0x271BB788 || r.ReadUInt8() != 0x02) {
                    throw new InvalidDataException("Unexpected constant in .dat file");
                }
                
                if (r.ReadUInt16() != 0xACED) throw new InvalidDataException("Invalid stream magic");
                if (r.ReadUInt16() != 0x0005) throw new InvalidDataException("Invalid stream version");
                
                JObject obj = (JObject)ReadObject(r);
                ParseRootObject(lvl, obj);
                return lvl;
            }
        }
        
        static object ReadContent(DatReader r, byte typeCode) {
            if (typeCode == TC_BLOCKDATA) {
                return r.ReadBytes(r.ReadUInt8());
            } else if (typeCode == TC_BLOCKDATALONG) {
                return r.ReadBytes(r.ReadInt32());
            } else {
                return ReadObject(r, typeCode);
            }
        }
        
        static object ReadObject(DatReader r) { return ReadObject(r, r.ReadUInt8()); }
        static object ReadObject(DatReader r, byte typeCode) {
            switch (typeCode) {
                case TC_STRING:    return NewString(r);
                case TC_RESET: r.handles.Clear(); return null;
                case TC_NULL:      return null;
                case TC_REFERENCE: return PrevObject(r);
                case TC_CLASS:     return NewClass(r);
                case TC_OBJECT:    return NewObject(r);
                case TC_ARRAY:     return NewArray(r);
                case TC_CLASSDESC: return NewClassDesc(r);
            }
            throw new InvalidDataException("Invalid typecode: " + typeCode);
        }
        
        static string NewString(DatReader r) {
            string value = r.ReadUtf8();
            r.handles.Add(value);
            return value;
        }
        
        static object PrevObject(DatReader r) {
            int handle = r.ReadInt32() - baseWireHandle;
            if (handle >= 0 && handle < r.handles.Count) return r.handles[handle];
            throw new InvalidDataException("Invalid stream handle: " + handle);
        }
        
        static JClassDesc NewClass(DatReader r) {
            JClassDesc classDesc = ClassDesc(r);
            r.handles.Add(classDesc);
            return classDesc;
        }
        
        static JObject NewObject(DatReader r) {
            JObject obj = new JObject();
            obj.Desc = ClassDesc(r);
            r.handles.Add(obj);
            
            List<JClassDesc> descs = new List<JClassDesc>();
            JClassDesc tmp = obj.Desc;
            
            // most superclass data is first
            while (tmp != null) {
                descs.Add(tmp);
                tmp = tmp.SuperClass;
            }
            
            obj.ClassData = new JClassData[descs.Count];
            for (int i = descs.Count - 1; i >= 0; i--) {
                obj.ClassData[i] = ClassData(r, descs[i]);
            }
            return obj;
        }
        
        static JArray NewArray(DatReader r) {
            JArray array = new JArray();
            array.Desc = ClassDesc(r);
            r.handles.Add(array);
            char type = array.Desc.Name[1];
            int size  = r.ReadInt32();
            
            if (type == 'B') {
                array.Values = r.ReadBytes(size);
            } else {
                object[] values = new object[size];
                for (int i = 0; i < values.Length; i++) {
                    values[i] = Value(r, type);
                }
                array.Values = values;
            }
            return array;
        }
        
        static JClassDesc NewClassDesc(DatReader r) {
            JClassDesc desc = new JClassDesc();
            desc.Name = r.ReadUtf8();
            r.ReadInt64(); // serial UID
            r.handles.Add(desc);
            
            // read class desc info
            desc.Flags  = r.ReadUInt8();
            desc.Fields = new JFieldDesc[r.ReadUInt16()];
            for (int i = 0; i < desc.Fields.Length; i++) {
                desc.Fields[i] = FieldDesc(r);
            }
            
            SkipAnnotation(r);
            desc.SuperClass = ClassDesc(r);
            return desc;
        }
        
        static JClassDesc ClassDesc(DatReader r) {
            byte typeCode = r.ReadUInt8();
            if (typeCode == TC_CLASSDESC) return NewClassDesc(r);
            if (typeCode == TC_NULL)      return null;
            if (typeCode == TC_REFERENCE) return (JClassDesc)PrevObject(r);
            
            throw new InvalidDataException("Invalid type code: " + typeCode);
        }
        
        static JClassData ClassData(DatReader r, JClassDesc desc) {
            if ((desc.Flags & SC_SERIALIZABLE) == 0) {
                throw new InvalidDataException("Invalid class data flags: " + desc.Flags);
            }
            
            JClassData data = new JClassData();
            data.Values = new object[desc.Fields.Length];
            for (int i = 0; i < data.Values.Length; i++) {
                data.Values[i] = Value(r, desc.Fields[i].Type);
            }
            
            if ((desc.Flags & SC_WRITE_METHOD) != 0) {
                SkipAnnotation(r);
            }
            return data;
        }
        
        static unsafe object Value(DatReader r, char type) {
            if (type == 'B') return r.ReadUInt8();
            if (type == 'C') return (char)r.ReadUInt16();
            if (type == 'D') { long tmp = r.ReadInt64(); return *(double*)(&tmp); }
            if (type == 'F') { int tmp  = r.ReadInt32(); return *(float*)(&tmp); }
            if (type == 'I') return r.ReadInt32();
            if (type == 'J') return r.ReadInt64();
            if (type == 'S') return r.ReadInt16();
            if (type == 'Z') return r.ReadUInt8() != 0;
            if (type == 'L') return ReadObject(r);
            if (type == '[') return ReadObject(r);
            
            throw new InvalidDataException("Invalid value code: " + type);
        }
        
        class JFieldDesc {
            public char Type;
            public string Name, ClassName;
        }
        static JFieldDesc FieldDesc(DatReader r) {
            JFieldDesc desc = new JFieldDesc();
            byte type = r.ReadUInt8();
            desc.Type = (char)type;
            
            if (type == 'B' || type == 'C' || type == 'D' || type == 'F' || type == 'I' || type == 'J' || type == 'S' || type == 'Z') {
                desc.Name = r.ReadUtf8();
            } else if (type == '[' || type == 'L') {
                desc.Name = r.ReadUtf8();
                desc.ClassName = (string)ReadObject(r);
            } else {
                throw new InvalidDataException("Invalid field type: " + type);
            }
            return desc;
        }
        
        static void SkipAnnotation(DatReader r) {
            byte typeCode;
            while ((typeCode = r.ReadUInt8()) != TC_ENDBLOCKDATA) {
                ReadContent(r, typeCode);
            }
        }
    }
}