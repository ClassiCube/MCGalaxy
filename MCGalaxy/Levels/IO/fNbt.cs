/*Copyright (c) 2012-2015, Matvei "fragmer" Stefarov <me@matvei.org>
  All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
    list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
    this list of conditions and the following disclaimer in the documentation
    and/or other materials provided with the distribution.
3. Neither the name of fNbt nor the names of its contributors may be used to
    endorse or promote products derived from this software without specific
    prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace fNbt {
    /// <summary> A tag containing a single byte. </summary>
    public sealed class NbtByte : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.Byte; } }
        public byte Value;

        internal override void ReadTag(NbtBinaryReader reader) {
            Value = reader.ReadByte();
        }
    }

    /// <summary> A tag containing an array of bytes. </summary>
    public sealed class NbtByteArray : NbtTag {
        static readonly byte[] empty = new byte[0];
        public override NbtTagType TagType { get { return NbtTagType.ByteArray; } }
        public byte[] Value = empty;

        internal override void ReadTag(NbtBinaryReader reader) {
            int length = reader.ReadInt32();
            if (length < 0)
                throw new InvalidDataException("Negative length given in TAG_Byte_Array");

            Value = reader.ReadBytes(length);
            if (Value.Length < length) throw new EndOfStreamException();
        }
    }

    /// <summary> A tag containing a set of other named tags. Order is not guaranteed. </summary>
    public sealed class NbtCompound : NbtTag, IEnumerable<NbtTag> {
        public override NbtTagType TagType { get { return NbtTagType.Compound; } }
        readonly Dictionary<string, NbtTag> tags = new Dictionary<string, NbtTag>();

        public NbtCompound() {}
        public NbtCompound(string tagName) { Name = tagName; }
        
        public override NbtTag this[string tagName] {
            get {
                NbtTag result;
                if (tags.TryGetValue(tagName, out result)) return result;
                return null;
            }
        }

        public bool Contains(string tagName) { return tags.ContainsKey(tagName); }

        internal override void ReadTag(NbtBinaryReader reader) {
            while (true) {
                NbtTagType nextTag = reader.ReadTagType();
                if (nextTag == NbtTagType.End) return;
                NbtTag newTag = Construct(nextTag);
                
                newTag.Name = reader.ReadString();
                newTag.ReadTag(reader);
                tags.Add(newTag.Name, newTag);
            }
        }

        public IEnumerator<NbtTag> GetEnumerator() { return tags.Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return tags.Values.GetEnumerator(); }
    }

    public sealed class NbtDouble : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.Double; } }
        public double Value;

        internal override void ReadTag(NbtBinaryReader reader) {
            Value = reader.ReadDouble();
        }
    }

    public sealed class NbtFloat : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.Float; } }
        public float Value;
        
        internal override void ReadTag(NbtBinaryReader reader) {
            Value = reader.ReadSingle();
        }
    }

    /// <summary> A tag containing a single signed 32-bit integer. </summary>
    public sealed class NbtInt : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.Int; } }
        public int Value;

        internal override void ReadTag(NbtBinaryReader reader) {
            Value = reader.ReadInt32();
        }
    }

    /// <summary> A tag containing an array of signed 32-bit integers. </summary>
    public sealed class NbtIntArray : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.IntArray; } }
        public int[] Value;

        internal override void ReadTag(NbtBinaryReader reader) {
            int length = reader.ReadInt32();
            if (length < 0)
                throw new InvalidDataException("Negative length given in TAG_Int_Array");

            Value = new int[length];
            for (int i = 0; i < length; i++)
                Value[i] = reader.ReadInt32();
        }
    }

    /// <summary> A tag containing a list of unnamed tags, all of the same kind. </summary>
    public sealed class NbtList : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.List; } }
        public readonly List<NbtTag> Tags = new List<NbtTag>();
        public NbtTagType ListType;

        internal override void ReadTag(NbtBinaryReader reader) {
            ListType = reader.ReadTagType();

            int length = reader.ReadInt32();
            if (length < 0) throw new InvalidDataException("Negative list size given");

            for (int i = 0; i < length; i++) {
                NbtTag newTag = NbtTag.Construct(ListType);
                newTag.ReadTag(reader);
                Tags.Add(newTag);
            }
        }
    }

    /// <summary> A tag containing a single signed 64-bit integer. </summary>
    public sealed class NbtLong : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.Long; } }
        public long Value;

        internal override void ReadTag(NbtBinaryReader reader) {
            Value = reader.ReadInt64();
        }
    }

    /// <summary> A tag containing a single signed 16-bit integer. </summary>
    public sealed class NbtShort : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.Short; } }
        public short Value;

        internal override void ReadTag(NbtBinaryReader reader) {
            Value = reader.ReadInt16();
        }
    }

    /// <summary> A tag containing a single string. String is stored in UTF-8 encoding. </summary>
    public sealed class NbtString : NbtTag {
        public override NbtTagType TagType { get { return NbtTagType.String; } }
        public string Value;

        internal override void ReadTag(NbtBinaryReader reader) {
            Value = reader.ReadString();
        }
    }

    /// <summary> Base class for different kinds of named binary tags. </summary>
    public abstract class NbtTag {
        public abstract NbtTagType TagType { get; }
        public string Name;

        internal abstract void ReadTag(NbtBinaryReader reader);

        public virtual NbtTag this[string tagName] {
            get { throw new InvalidOperationException("String indexers only work on NbtCompound tags."); }
            set { throw new InvalidOperationException("String indexers only work on NbtCompound tags."); }
        }

        public byte ByteValue {
            get {
                if (TagType == NbtTagType.Byte) return ((NbtByte)this).Value;
                throw new InvalidCastException("Cannot get ByteValue from " + TagType);
            }
        }
        
        public float FloatValue {
            get {
                if (TagType == NbtTagType.Float) return ((NbtFloat)this).Value;
                throw new InvalidCastException("Cannot get FloatValue from " + TagType);
            }
        }

        public short ShortValue {
            get {
                switch (TagType) {
                    case NbtTagType.Byte: return ((NbtByte)this).Value;
                    case NbtTagType.Short: return ((NbtShort)this).Value;
                    default:
                        throw new InvalidCastException("Cannot get ShortValue from " + TagType);
                }
            }
        }

        public int IntValue {
            get {
                switch (TagType) {
                    case NbtTagType.Byte: return ((NbtByte)this).Value;
                    case NbtTagType.Short: return ((NbtShort)this).Value;
                    case NbtTagType.Int:  return ((NbtInt)this).Value;
                    default:
                        throw new InvalidCastException("Cannot get IntValue from " + TagType);
                }
            }
        }

        public byte[] ByteArrayValue {
            get {
                if (TagType == NbtTagType.ByteArray) return ((NbtByteArray)this).Value;
                throw new InvalidCastException("Cannot get ByteArrayValue from " + TagType);
            }
        }
        
        public string StringValue {
            get {
                if (TagType == NbtTagType.String) return ((NbtString)this).Value;
                throw new InvalidCastException("Cannot get StringValue from " + TagType);
            }
        }
        
        internal static NbtTag Construct(NbtTagType type) {
            switch (type) {
                case NbtTagType.Byte: return new NbtByte();
                case NbtTagType.Short: return new NbtShort();
                case NbtTagType.Int: return new NbtInt();
                case NbtTagType.Long: return new NbtLong();
                case NbtTagType.Float: return new NbtFloat();
                case NbtTagType.Double: return new NbtDouble();
                case NbtTagType.ByteArray: return new NbtByteArray();
                case NbtTagType.String: return new NbtString();
                case NbtTagType.List: return new NbtList();
                case NbtTagType.Compound: return new NbtCompound();
                case NbtTagType.IntArray: return new NbtIntArray();
            }
            return null;
        }
    }

    /// <summary> BinaryReader wrapper that takes care of reading primitives from an NBT stream,
    /// while taking care of endianness, string encoding, and skipping. </summary>
    internal sealed class NbtBinaryReader : BinaryReader {
        readonly byte[] buffer = new byte[sizeof(double)];
        readonly bool swapNeeded;
        // avoid allocation for small strings (which is majority of them)
        readonly byte[] strBuffer = new byte[64];

        public NbtBinaryReader(Stream input, bool bigEndian) : base(input) {
            swapNeeded = (BitConverter.IsLittleEndian == bigEndian);
        }

        public NbtTagType ReadTagType() {
            int type = ReadByte();
            if (type < 0) throw new EndOfStreamException();
            if (type > (int)NbtTagType.IntArray)
                throw new NbtFormatException("NBT tag type out of range: " + type);
            return (NbtTagType)type;
        }

        public override short ReadInt16() {
            if (swapNeeded) {
                return Swap(base.ReadInt16());
            }
            return base.ReadInt16();
        }

        public override int ReadInt32() {
            if (swapNeeded) {
                return Swap(base.ReadInt32());
            }
            return base.ReadInt32();
        }

        public override long ReadInt64() {
            if (swapNeeded) {
                return Swap(base.ReadInt64());
            }
            return base.ReadInt64();
        }

        public override float ReadSingle() {
            if (swapNeeded) {
                FillBuffer(sizeof(float));
                Array.Reverse(buffer, 0, sizeof(float));
                return BitConverter.ToSingle(buffer, 0);
            }
            return base.ReadSingle();
        }

        public override double ReadDouble() {
            if (swapNeeded) {
                FillBuffer(sizeof(double));
                Array.Reverse(buffer);
                return BitConverter.ToDouble(buffer, 0);
            }
            return base.ReadDouble();
        }

        public override string ReadString() {
            short length = ReadInt16();
            if (length < 0) {
                throw new InvalidDataException("Negative string length given");
            }
            
            if (length < strBuffer.Length) {
                int offset = 0;
                while (offset < length) {
                    int read = BaseStream.Read(strBuffer, offset, length - offset);
                    if (read == 0) throw new EndOfStreamException();
                    offset += read;
                }
                return Encoding.UTF8.GetString(strBuffer, 0, length);
            } else {
                byte[] data = ReadBytes(length);
                if (data.Length < length) throw new EndOfStreamException();
                return Encoding.UTF8.GetString(data);
            }
        }

        new void FillBuffer(int numBytes) {
            int offset = 0;
            do {
                int num = BaseStream.Read(buffer, offset, numBytes - offset);
                if (num == 0) throw new EndOfStreamException();
                offset += num;
            } while (offset < numBytes);
        }

        static short Swap(short v) {
            return (short)((v >> 8) & 0x00FF | (v << 8) & 0xFF00);
        }

        static int Swap(int v) {
            uint v2 = (uint)v;
            return
                (int)
                ((v2 >> 24) & 0x000000FF | (v2 >> 8) & 0x0000FF00 | (v2 << 8) & 0x00FF0000 |
                 (v2 << 24) & 0xFF000000);
        }

        static long Swap(long v) {
            return (Swap((int)v) & uint.MaxValue) << 32 | Swap((int)(v >> 32)) & uint.MaxValue;
        }
    }

    /// <summary> Represents a complete NBT file. </summary>
    public sealed class NbtFile {
        public NbtCompound RootTag;
        public NbtFile() { RootTag = new NbtCompound(""); }

        public void LoadFromStream(Stream stream) {
            if (stream == null) throw new ArgumentNullException("stream");

            using (GZipStream decStream = new GZipStream(stream, CompressionMode.Decompress, true)) {
                // Size of buffers that are used to avoid frequent reads from / writes to compressed streams
                BufferedStream buffered = new BufferedStream(decStream, 8*1024);
                
                // Make sure the first byte in this file is the tag for a TAG_Compound
                int header = buffered.ReadByte();
                if (header < 0) throw new EndOfStreamException();
                if (header != (int)NbtTagType.Compound)
                    throw new NbtFormatException("Given NBT stream does not start with a TAG_Compound");
                
                NbtBinaryReader reader = new NbtBinaryReader(buffered, true);
                NbtCompound rootCompound = new NbtCompound(reader.ReadString());
                rootCompound.ReadTag(reader);
                RootTag = rootCompound;
            }
        }
    }

    public sealed class NbtFormatException : Exception {
        internal NbtFormatException(string message) : base(message) {}
    }

    /// <summary> Enumeration of named binary tag types, and their corresponding codes. </summary>
    public enum NbtTagType : byte {
        /// <summary> TAG_End: This unnamed tag serves no purpose but to signify the end of an open TAG_Compound. </summary>
        End = 0x00,
        /// <summary> TAG_Byte: A single byte. </summary>
        Byte = 0x01,
        /// <summary> TAG_Short: A single signed 16-bit integer. </summary>
        Short = 0x02,
        /// <summary> TAG_Int: A single signed 32-bit integer. </summary>
        Int = 0x03,
        /// <summary> TAG_Long: A single signed 64-bit integer. </summary>
        Long = 0x04,
        /// <summary> TAG_Float: A single IEEE-754 single-precision floating point number. </summary>
        Float = 0x05,
        /// <summary> TAG_Double: A single IEEE-754 double-precision floating point number. </summary>
        Double = 0x06,
        /// <summary> TAG_Byte_Array: A length-prefixed array of bytes. </summary>
        ByteArray = 0x07,
        /// <summary> TAG_String: A length-prefixed UTF-8 string. </summary>
        String = 0x08,
        /// <summary> TAG_List: A list of nameless tags, all of the same type. </summary>
        List = 0x09,
        /// <summary> TAG_Compound: A set of named tags. </summary>
        Compound = 0x0a,
        /// <summary> TAG_Byte_Array: A length-prefixed array of signed 32-bit integers. </summary>
        IntArray = 0x0b
    }
}