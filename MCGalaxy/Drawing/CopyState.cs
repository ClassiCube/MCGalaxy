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
using BlockRaw = System.Byte;

namespace MCGalaxy.Drawing {

    public sealed class CopyState {
        
        byte[] blocks;
        byte[][] extBlocks;
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
        
        const int chunkSize = 0x1000, chunkShift = 12, chunkMask = 0xFFF;
        public int Volume { get { return Width * Height * Length; } }
        public int ExtChunks { get { return (Volume + (chunkSize - 1)) / chunkSize; } }
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
            blocks = new byte[Volume];
            extBlocks = new byte[ExtChunks][];
            UsedBlocks = Volume;
        }

        public void Clear() {
            blocks = null;
            extBlocks = null;
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
        
        public BlockID Get(int index) {
            byte raw = blocks[index];            
            #if TEN_BIT_BLOCKS
            BlockID extended = Block.ExtendedBase[raw];
            if (extended == 0) return raw;
            byte[] chunk = extBlocks[index >> chunkShift];
            return chunk == null ? Block.Air : (BlockID)(extended | chunk[index & chunkMask]);
            #else
            if (raw != Block.custom_block) return raw;
            byte[] chunk = extBlocks[index >> chunkShift];
            return chunk == null ? Block.Air : (BlockID)(Block.Extended | chunk[index & chunkMask]);
            #endif
        }
        
        public void Set(BlockID block, int index) {
            if (block >= Block.Extended) {
                #if TEN_BIT_BLOCKS
                blocks[index] = Block.ExtendedClass[block >> Block.ExtendedShift];
                #else
                blocks[index] = Block.custom_block;
                #endif
                
                byte[] chunk = extBlocks[index >> chunkShift];
                if (chunk == null) {
                    chunk = new byte[chunkSize];
                    extBlocks[index >> chunkShift] = chunk;
                }
                chunk[index & chunkMask] = (BlockRaw)block;
            } else {
                blocks[index] = (BlockRaw)block;
            }
        }
        
        public void Set(BlockID block, int x, int y, int z) {
            Set(block, (y * Length + z) * Width + x);
        }
        
        
        const int identifier1 = 0x434F5059; // version 1, 'COPY' (copy)
        const int identifier2 = 0x434F5043; // 'COPC' (copy compressed)
        const int identifier3 = 0x434F504F; // 'COPO' (copy optimised)
        const int identifier4 = 0x434F5053; // 'COPS' (copy sparse)
        
        /// <summary> Saves this copy state to the given stream. </summary>
        public void SaveTo(Stream stream) {
            BinaryWriter w = new BinaryWriter(stream);
            w.Write(identifier4);
            w.Write(X); w.Write(Y); w.Write(Z);
            w.Write(Width); w.Write(Height); w.Write(Length);
            
            byte[] data = blocks.GZip();
            w.Write(data.Length);
            w.Write(data);
            
            for (int i = 0; i < extBlocks.Length; i++) {
                if (extBlocks[i] == null) {
                    w.Write((byte)0); continue;
                }
                
                w.Write((byte)1);
                data = extBlocks[i].GZip();
                w.Write((ushort)data.Length);
                w.Write(data);
            }
            
            w.Write(OriginX); w.Write(OriginY); w.Write(OriginZ);
            w.Write((byte)0x0f); // 0ffset
            w.Write(Offset.X); w.Write(Offset.Y); w.Write(Offset.Z);
            w.Write((byte)(PasteAir ? 1 : 0));
        }
        
        /// <summary> Loads this copy state from the given stream. </summary>
        public void LoadFrom(Stream stream) {
            BinaryReader r = new BinaryReader(stream);
            int id = r.ReadInt32();
            if (!(id == identifier1 || id == identifier2 || id == identifier3 || id == identifier4))
                throw new InvalidDataException("invalid identifier");
            
            X = r.ReadInt32(); Y = r.ReadInt32(); Z = r.ReadInt32();
            Width = r.ReadInt32(); Height = r.ReadInt32(); Length = r.ReadInt32();
            LoadBlocks(r, id);       
            UsedBlocks = Volume;
            
            // origin is not present in version 1
            if (id == identifier1) return;
            OriginX = r.ReadInt32(); OriginY = r.ReadInt32(); OriginZ = r.ReadInt32();
            
            // was added in later (ReadByte also catches end of stream)
            if (stream.ReadByte() != 0x0f) return;
            Offset.X = r.ReadInt32(); Offset.Y = r.ReadInt32(); Offset.Z = r.ReadInt32();
            PasteAir = stream.ReadByte() == 1;
        }
        
        void LoadBlocks(BinaryReader r, int id) {
            byte[] allExtBlocks;
            int dataLen;
            extBlocks = new byte[(Volume + (chunkSize - 1)) / chunkSize][];
            
            if (id == identifier1) {
                blocks = r.ReadBytes(Volume);
                allExtBlocks = r.ReadBytes(Volume);
                UnpackExtBlocks(allExtBlocks);
            } else {
                dataLen = r.ReadInt32();
                blocks = r.ReadBytes(dataLen).Decompress(Volume);
                
                if (id == identifier2) {
                    dataLen = r.ReadInt32();
                    allExtBlocks = r.ReadBytes(dataLen).Decompress(Volume);
                    UnpackExtBlocks(allExtBlocks);
                } else if (id == identifier3) {
                    dataLen = r.ReadInt32();
                    allExtBlocks = r.ReadBytes(dataLen).Decompress((Volume + 7) / 8);
                    UnpackPackedExtBlocks(allExtBlocks);
                } else {
                    for (int i = 0; i < extBlocks.Length; i++) {
                        if (r.ReadByte() == 0) continue;
                        dataLen = r.ReadUInt16();
                        extBlocks[i] = r.ReadBytes(dataLen).Decompress(chunkSize);
                    }
                }
            }
        }
        
        void UnpackExtBlocks(byte[] allExtBlocks) {
            for (int i = 0; i < blocks.Length; i++) {
                if (blocks[i] != Block.custom_block) continue;
                Set((BlockID)(Block.Extended | allExtBlocks[i]), i);
            }
        }
        
        void UnpackPackedExtBlocks(byte[] allExtBlocks) {
            for (int i = 0; i < blocks.Length; i++) {
                bool isExt = (allExtBlocks[i >> 3] & (1 << (i & 0x7))) != 0;
                if (isExt) { Set((BlockID)(Block.Extended | blocks[i]), i); }
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
                
                byte rawBlock = raw[i + 6];
                Set(rawBlock, x - X, y - Y, z - Z);
            }
            UsedBlocks = Volume;
            OriginX = X; OriginY = Y; OriginZ = Z;
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
