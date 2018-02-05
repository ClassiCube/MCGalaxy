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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing {

    public sealed class CopyState {
        
        byte[] raw, isExt;
        public int X, Y, Z;
        public int OriginX, OriginY, OriginZ;
        public int Width, Height, Length;
        public bool PasteAir;
        public int UsedBlocks;
        public Vec3S32 Offset;
        public DateTime CopyTime;
        public string CopySource;
        
        internal int OppositeOriginX { get { return OriginX == X ? X + Width - 1 : X; } }
        internal int OppositeOriginY { get { return OriginY == Y ? Y + Height - 1 : Y; } }
        internal int OppositeOriginZ { get { return OriginZ == Z ? Z + Length - 1 : Z; } }        
        public int Volume { get { return Width * Height * Length; } }
        public string Summary { 
            get { return Volume + " blocks from " + CopySource + ", " + (DateTime.UtcNow - CopyTime).Shorten(true) + " ago"; } 
        }
        
        public CopyState(int x, int y, int z, int width, int height, int length) {
            Init(x, y, z, width, height, length);
            CopyTime = DateTime.UtcNow;
        }
        
        void Init(int x, int y, int z, int width, int height, int length) {
            X = x; Y = y; Z = z;
            Width = width; Height = height; Length = length;
            raw = new byte[Volume];
            isExt = new byte[(Volume + 7) / 8]; // ceiling divide by 8, as 8 bits fit into 1 byte
            UsedBlocks = Volume;
        }

        public void Clear() {
            raw = null;
            isExt = null;
        }
        
        
        public void GetCoords(int index, out ushort x, out ushort y, out ushort z) {
            y = (ushort)(index / Width / Length);
            index -= y * Width * Length;
            z = (ushort)(index / Width);
            index -= z * Width;
            x = (ushort)index;
        }
        
        public int GetIndex(int x, int y, int z) {
            return (y * Length + z) * Width + x;
        }
        
        public ushort Get(int index) {
            return Block.FromRaw(raw[index],
                                    (isExt[index >> 3] & (1 << (index & 0x07))) != 0);
        }
        
        public ushort Get(int x, int y, int z) {
            int index = (y * Length + z) * Width + x;
            return Block.FromRaw(raw[index],
                                    (isExt[index >> 3] & (1 << (index & 0x07))) != 0);
        }
        
        public void Set(BlockID block, int index) {
            raw[index] = (byte)block;
            if (block >= Block.Extended) {
                isExt[index >> 3] |= (byte)(1 << (index & 0x07));
            } else {
                isExt[index >> 3] &= (byte)~(1 << (index & 0x07));
            }
        }
        
        public void Set(BlockID block, int x, int y, int z) {
            int index = (y * Length + z) * Width + x;
            raw[index] = (byte)block;
            if (block >= Block.Extended) {
                isExt[index >> 3] |= (byte)(1 << (index & 0x07));
            } else {
                isExt[index >> 3] &= (byte)~(1 << (index & 0x07));
            }
        }
        
        
        const int identifier1 = 0x434F5059; // version 1, 'COPY' (copy)
        const int identifier2 = 0x434F5043; // 'COPC' (copy compressed)
        const int identifier3 = 0x434F504F; // 'COPO' (copy optimised)
        
        /// <summary> Saves this copy state to the given stream. </summary>
        public void SaveTo(Stream stream) {
            BinaryWriter w = new BinaryWriter(stream);
            w.Write(identifier3);
            w.Write(X); w.Write(Y); w.Write(Z);
            w.Write(Width); w.Write(Height); w.Write(Length);
            
            byte[] data = raw.GZip();
            w.Write(data.Length);
            w.Write(data);
            data = isExt.GZip();
            w.Write(data.Length);
            w.Write(data);
            
            w.Write(OriginX); w.Write(OriginY); w.Write(OriginZ);
            w.Write((byte)0x0f); // 0ffset
            w.Write(Offset.X); w.Write(Offset.Y); w.Write(Offset.Z);
            w.Write((byte)(PasteAir ? 1 : 0));
        }
        
        /// <summary> Loads this copy state from the given stream. </summary>
        public void LoadFrom(Stream stream) {
            BinaryReader r = new BinaryReader(stream);
            int identifier = r.ReadInt32();
            if (!(identifier == identifier1 || identifier == identifier2 || identifier == identifier3))
                throw new InvalidDataException("invalid identifier");
            
            X = r.ReadInt32(); Y = r.ReadInt32(); Z = r.ReadInt32();
            Width = r.ReadInt32(); Height = r.ReadInt32(); Length = r.ReadInt32();
            LoadBlocks(r, identifier);
            
            UsedBlocks = Volume;
            OriginX = r.ReadInt32(); OriginY = r.ReadInt32(); OriginZ = r.ReadInt32();
            if (stream.ReadByte() != 0x0f) return;
            Offset.X = r.ReadInt32(); Offset.Y = r.ReadInt32(); Offset.Z = r.ReadInt32();
            PasteAir = stream.ReadByte() == 1;
        }
        
        void LoadBlocks(BinaryReader r, int identifier) {
            byte[] extBlocks;
            int dataLen;
            switch (identifier) {
                case identifier1:
                    raw = r.ReadBytes(Volume);
                    extBlocks = r.ReadBytes(Volume);
                    UnpackExtBlocks(extBlocks);
                    break;
                    
                case identifier2:
                    dataLen = r.ReadInt32();
                    raw = r.ReadBytes(dataLen).Decompress(Volume);
                    dataLen = r.ReadInt32();
                    extBlocks = r.ReadBytes(dataLen).Decompress(Volume);
                    UnpackExtBlocks(extBlocks);
                    break;
                    
                case identifier3:
                    dataLen = r.ReadInt32();
                    raw = r.ReadBytes(dataLen).Decompress(Volume);
                    dataLen = r.ReadInt32();
                    isExt = r.ReadBytes(dataLen).Decompress((Volume + 7) / 8);
                    break;                    
            }
        }

        /// <summary> Loads this copy state from the given stream, using the very old format. </summary>
        public void LoadFromOld(Stream stream, Stream underlying) {
            byte[] raw = new byte[underlying.Length];
            underlying.Read(raw, 0, (int)underlying.Length);
            raw = raw.Decompress();
            if (raw.Length == 0) return;
            
            CalculateBounds(raw);
            for (int i = 0; i < raw.Length; i += 7) {
                ushort x = BitConverter.ToUInt16(raw, i + 0);
                ushort y = BitConverter.ToUInt16(raw, i + 2);
                ushort z = BitConverter.ToUInt16(raw, i + 4);
                
                ushort block = raw[i + 6];
                Set(block, x - X, y - Y, z - Z);
            }
            UsedBlocks = Volume;
            OriginX = X; OriginY = Y; OriginZ = Z;
        }
        
        void UnpackExtBlocks(byte[] extBlocks) {
            isExt = new byte[(Volume + 7) / 8];
            for (int i = 0; i < raw.Length; i++) {
                if (raw[i] != Block.custom_block) continue;
                raw[i] = extBlocks[i];
                isExt[i >> 3] |= (byte)(1 << (i & 0x07));
            }
        }
        
        void CalculateBounds(byte[] raw) {
            int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;
            for (int i = 0; i < raw.Length; i += 7) {
                ushort x = BitConverter.ToUInt16(raw, i + 0);
                ushort y = BitConverter.ToUInt16(raw, i + 2);
                ushort z = BitConverter.ToUInt16(raw, i + 4);
                
                minX = Math.Min(x, minX); maxX = Math.Max(x, maxX);
                minY = Math.Min(y, minY); maxY = Math.Max(y, maxY);
                minZ = Math.Min(z, minZ); maxZ = Math.Max(z, maxZ);                
            }
            
            Init(minX, minY, minZ,
                 (maxX - minX) + 1,
                 (maxY - minY) + 1,
                 (maxZ - minZ) + 1);
        }
    }
}
