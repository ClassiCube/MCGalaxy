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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MCGalaxy.Levels.IO {
    public class JClassDesc {
        public string Name;
        public byte Flags;
        public JFieldDesc[] Fields;
        public JClassDesc SuperClass;
    }

    public class JClassData {
        public object[] Values;
    }

    public class JClass {
        public JClassDesc Desc;
    }

    public class JObject {
        public JClassDesc Desc;
        public JClassData[] ClassData;
    }

    public class JArray {
        public JClassDesc Desc;
        public object Values;
    }

    public class JFieldDesc {
        public char Type;
        public string Name, ClassName;
    }

    public class JEnum {
        public JClassDesc Desc;
        public object Name;
        public override string ToString() { return Desc.Name + "." + Name; }
    }

    // Java serialised objects are quite complicated and annoying to parse
    //  http://www.javaworld.com/article/2072752/the-java-serialization-algorithm-revealed.html
    //  https://docs.oracle.com/javase/7/docs/platform/serialization/spec/protocol.html
    // Good reference tool for comparison
    //  https://github.com/NickstaDB/SerializationDumper
    public sealed class JavaReader {
        public BinaryReader src;
        public List<object> handles = new List<object>();
        public byte[] ReadBytes(int count) { return src.ReadBytes(count); }

        public byte   ReadUInt8()  { return src.ReadByte(); }
        public short  ReadInt16()  { return IPAddress.HostToNetworkOrder(src.ReadInt16()); }
        public ushort ReadUInt16() { return (ushort)IPAddress.HostToNetworkOrder(src.ReadInt16()); }

        public int    ReadInt32()  { return IPAddress.HostToNetworkOrder(src.ReadInt32()); }
        public long   ReadInt64()  { return IPAddress.HostToNetworkOrder(src.ReadInt64()); }
        public string ReadUtf8()   { return Encoding.UTF8.GetString(src.ReadBytes(ReadUInt16())); }

        public object ReadObject() { return ReadObject(ReadUInt8()); }


        const byte TC_NULL =           0x70;
        const byte TC_REFERENCE =      0x71;
        const byte TC_CLASSDESC =      0x72;
        const byte TC_OBJECT =         0x73;
        const byte TC_STRING =         0x74;
        const byte TC_ARRAY =          0x75;
        const byte TC_CLASS =          0x76;
        const byte TC_BLOCKDATA =      0x77;
        const byte TC_ENDBLOCKDATA =   0x78;
        const byte TC_RESET =          0x79; // Unimplemented
        const byte TC_BLOCKDATALONG =  0x7A; // Unimplemented
        const byte TC_EXCEPTION =      0x7B; // Unimplemented
        const byte TC_LONGSTRING =     0x7C; // Unimplemented
        const byte TC_PROXYCLASSDESC = 0x7D; // Unimplemented
        const byte TC_ENUM =           0x7E;

        const int baseWireHandle = 0x7E0000;
        const byte SC_WRITE_METHOD = 0x01, SC_SERIALIZABLE = 0x02;


        object ReadObject(byte typeCode) {
            switch (typeCode) {
                    case TC_STRING:    return NewString();
                    case TC_NULL:      return null;
                    case TC_REFERENCE: return PrevObject();
                    case TC_OBJECT:    return NewObject();
                    case TC_ARRAY:     return NewArray();
                    case TC_ENUM:      return NewEnum();
                    case TC_CLASS:     return NewClass();
            }
            throw new InvalidDataException("Invalid typecode: " + typeCode);
        }

        string NewString() {
            string value = ReadUtf8();
            handles.Add(value);
            return value;
        }

        object PrevObject() {
            int handle = ReadInt32() - baseWireHandle;
            if (handle >= 0 && handle < handles.Count) return handles[handle];
            throw new InvalidDataException("Invalid stream handle: " + handle);
        }

        JObject NewObject() {
            JObject obj = new JObject();
            obj.Desc = ClassDesc();
            handles.Add(obj);

            List<JClassDesc> descs = new List<JClassDesc>();
            JClassDesc tmp = obj.Desc;

            // most superclass data is first
            while (tmp != null) {
                descs.Add(tmp);
                tmp = tmp.SuperClass;
            }

            obj.ClassData = new JClassData[descs.Count];
            for (int i = descs.Count - 1; i >= 0; i--) {
                obj.ClassData[i] = ClassData(descs[i]);
            }
            return obj;
        }

        JArray NewArray() {
            JArray array = new JArray();
            array.Desc = ClassDesc();
            handles.Add(array);
            char type = array.Desc.Name[1];
            int size  = ReadInt32();

            if (type == 'B') {
                array.Values = ReadBytes(size);
            } else {
                object[] values = new object[size];
                for (int i = 0; i < values.Length; i++) {
                    values[i] = Value(type);
                }
                array.Values = values;
            }
            return array;
        }

        JClassDesc NewClassDesc() {
            JClassDesc desc = new JClassDesc();
            desc.Name = ReadUtf8();
            ReadInt64(); // serial UID
            handles.Add(desc);

            // read class desc info
            desc.Flags  = ReadUInt8();
            desc.Fields = new JFieldDesc[ReadUInt16()];
            for (int i = 0; i < desc.Fields.Length; i++) {
                desc.Fields[i] = FieldDesc();
            }

            SkipAnnotation();
            desc.SuperClass = ClassDesc();
            return desc;
        }

        JEnum NewEnum() { // TC_ENUM classDesc newHandle enumConstantName
            JEnum je = new JEnum();
            je.Desc = ClassDesc();  // classDesc
            handles.Add(je);        // newHandle
            je.Name = ReadObject(); // enumConstantName
            return je;
        }

        JClass NewClass() { // TC_CLASS classDesc newHandle
            JClass jc = new JClass();
            jc.Desc = ClassDesc(); // classDesc
            handles.Add(jc);       // newHandle
            return jc;
        }

        JClassDesc ClassDesc() {
            byte typeCode = ReadUInt8();
            if (typeCode == TC_CLASSDESC) return NewClassDesc();
            if (typeCode == TC_NULL)      return null;
            if (typeCode == TC_REFERENCE) return (JClassDesc)PrevObject();

            throw new InvalidDataException("Invalid type code: " + typeCode);
        }

        JClassData ClassData(JClassDesc desc) {
            if ((desc.Flags & SC_SERIALIZABLE) == 0) {
                throw new InvalidDataException("Invalid class data flags: " + desc.Flags);
            }

            JClassData data = new JClassData();
            data.Values = new object[desc.Fields.Length];
            for (int i = 0; i < data.Values.Length; i++) {
                data.Values[i] = Value(desc.Fields[i].Type);
            }

            if ((desc.Flags & SC_WRITE_METHOD) != 0) {
                SkipAnnotation();
            }
            return data;
        }

        unsafe object Value(char type) {
            if (type == 'B') return ReadUInt8();
            if (type == 'C') return (char)ReadUInt16();
            if (type == 'D') { long tmp = ReadInt64(); return *(double*)(&tmp); }
            if (type == 'F') { int tmp  = ReadInt32(); return *(float*)(&tmp); }
            if (type == 'I') return ReadInt32();
            if (type == 'J') return ReadInt64();
            if (type == 'S') return ReadInt16();
            if (type == 'Z') return ReadUInt8() != 0;
            if (type == 'L') return ReadObject();
            if (type == '[') return ReadObject();

            throw new InvalidDataException("Invalid value code: " + type);
        }

        JFieldDesc FieldDesc() {
            JFieldDesc desc = new JFieldDesc();
            byte type = ReadUInt8();
            desc.Type = (char)type;

            if (type == 'B' || type == 'C' || type == 'D' || type == 'F' || type == 'I' || type == 'J' || type == 'S' || type == 'Z') {
                desc.Name = ReadUtf8();
            } else if (type == '[' || type == 'L') {
                desc.Name = ReadUtf8();
                desc.ClassName = (string)ReadObject();
            } else {
                throw new InvalidDataException("Invalid field type: " + type);
            }
            return desc;
        }

        void SkipAnnotation() {
            byte typeCode;
            while ((typeCode = ReadUInt8()) != TC_ENDBLOCKDATA) {
                if (typeCode == TC_BLOCKDATA) {
                    ReadBytes(ReadUInt8());
                } else {
                    ReadObject(typeCode);
                }
            }
        }
    }
}