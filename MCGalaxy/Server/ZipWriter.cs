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

namespace MCGalaxy {
    
    struct ZipEntry {
        public byte[] Filename;
        public long CompressedSize, UncompressedSize, LocalHeaderOffset;
        public uint Crc32;
        public ushort BitFlags, CompressionMethod;
        
        public void Reset() {
            // signify to use zip64 version of these fields instead
            CompressedSize = uint.MaxValue;
            UncompressedSize = uint.MaxValue;
            LocalHeaderOffset = uint.MaxValue;
        }
    }
    
    sealed class ZipEntryStream : Stream {
        public uint Crc32 = uint.MaxValue;
        public long CompressedLen;
        public Stream stream;
        
        public ZipEntryStream(Stream stream) { this.stream = stream; }        
        public override bool CanRead  { get { return false; } }
        public override bool CanSeek  { get { return false; } }
        public override bool CanWrite { get { return true; } }
        
        static Exception ex = new NotSupportedException("Stream does not support length/seeking.");
        public override void Flush() { stream.Flush(); }
        public override long Length { get { throw ex; } }
        public override long Position { get { throw ex; } set { throw ex; } }
        public override int Read(byte[] buffer, int offset, int count) { throw ex; }
        public override long Seek(long offset, SeekOrigin origin) { throw ex; }
        public override void SetLength(long length) { throw ex; }

        public override void Write(byte[] buffer, int offset, int count) {
            stream.Write(buffer, offset, count);
            CompressedLen += count;
        }
        
        public override void WriteByte(byte value) {
            stream.WriteByte(value);
            CompressedLen++;
        }
        
        public override void Close() { stream = null; }  
        public long WriteStream(Stream src, byte[] buffer, bool compress) {
            if (compress) {
                using (DeflateStream ds = new DeflateStream(this, CompressionMode.Compress))
                    return WriteData(ds, src, buffer);
            }
            return WriteData(this, src, buffer);
        }
        
        long WriteData(Stream dst, Stream src, byte[] buffer) {
            int count = 0;
            long totalLen = 0;
            
            while ((count = src.Read(buffer, 0, buffer.Length)) > 0) {
                dst.Write(buffer, 0, count);
                totalLen += count;
                
                for (int i = 0; i < count; i++) {
                    Crc32 = crc32Table[(Crc32 ^ buffer[i]) & 0xFF] ^ (Crc32 >> 8);
                }
            }
            return totalLen;
        }
        
        static uint[] crc32Table;
        static ZipEntryStream() {
            crc32Table = new uint[256];
            for (int i = 0; i < crc32Table.Length; i++) {
                uint c = (uint)i;
                
                for (int j = 0; j < 8; j++ ) {
                    if ((c & 1) != 0) {
                        c = 0xEDB88320 ^ (c >> 1);
                    } else { c >>= 1; }
                }
                crc32Table[i] = c;
            }
        }
    }

    public sealed class ZipWriter {
        BinaryWriter w;
        Stream stream;
        byte[] buffer = new byte[81920];
        
        DateTime now = DateTime.Now;
        bool zip64;
        List<ZipEntry> entries = new List<ZipEntry>();
        
        int numEntries;
        long centralDirOffset, centralDirSize, zip64EndOffset;
        const ushort ver_norm = 20, ver_zip64 = 45, zip64Extra = 28;
        
        public ZipWriter(Stream stream) {
            this.stream = stream;
            w = new BinaryWriter(stream);
        }
        
        public void WriteEntry(Stream src, string file, bool compress) {
            ZipEntry entry = default(ZipEntry);
            entry.Filename = Encoding.UTF8.GetBytes(file);
            entry.LocalHeaderOffset = stream.Position;
            
            // leave some room to fill in header later
            int headerSize = 30 + entry.Filename.Length + zip64Extra;
            stream.Write(buffer, 0, headerSize);
            
            // set bit flag for non-ascii filename
            foreach (char c in file) {
                if (c < ' ' || c > '~') entry.BitFlags |= (1 << 11);
            }
            
            if (compress) entry.CompressionMethod = 8;           
            ZipEntryStream dst = new ZipEntryStream(stream);
            entry.UncompressedSize = dst.WriteStream(src, buffer, compress);
            dst.stream = null;
            
            entry.CompressedSize = dst.CompressedLen;
            entry.Crc32 = dst.Crc32 ^ uint.MaxValue;
            entries.Add(entry); numEntries++;
        }
        
        public void FinishEntries() {
            // account for central directory too
            const int maxLen = int.MaxValue - 4 * 1000 * 1000;
            zip64 = numEntries >= ushort.MaxValue || stream.Length >= maxLen;
            long pos = stream.Position;

            for (int i = 0; i < numEntries; i++) {
                // turns out we didn't actually need zip64 extra field
                ZipEntry entry = entries[i];
                if (!zip64) entry.LocalHeaderOffset += zip64Extra;
                
                stream.Seek(entry.LocalHeaderOffset, SeekOrigin.Begin);
                WriteLocalFileRecord(entry);
                entries[i] = entry;
            }
            
            stream.Seek(pos, SeekOrigin.Begin);
        }
        
        public void WriteFooter() {
            centralDirOffset = stream.Position;
            for (int i = 0; i < numEntries; i++) {
                WriteCentralDirectoryRecord(entries[i]);
            }
            centralDirSize = stream.Position - centralDirOffset;
            
            if (zip64) WriteZip64EndOfCentralDirectory();
            WriteEndOfCentralDirectoryRecord();
        }
        
        void WriteZip64EndOfCentralDirectory() {
            zip64EndOffset = stream.Position;
            WriteZip64EndOfCentralDirectoryRecord();
            WriteZip64EndOfCentralDirectoryLocator();
            
            // signify to use zip64 record to find data
            numEntries = ushort.MaxValue;
            centralDirOffset = uint.MaxValue;
            centralDirSize = uint.MaxValue;
        }
        
        
        void WriteLocalFileRecord(ZipEntry entry) {
            ushort extraLen = (ushort)(zip64 ? zip64Extra : 0);
            ushort version = zip64 ? ver_zip64 : ver_norm;
            ZipEntry copy = entry;
            if (zip64) entry.Reset();
            
            w.Write(0x04034b50);
            w.Write(version);
            w.Write(entry.BitFlags);
            w.Write(entry.CompressionMethod);
            WriteLastModified();
            w.Write(entry.Crc32);
            w.Write((int)entry.CompressedSize);
            w.Write((int)entry.UncompressedSize);
            w.Write((ushort)entry.Filename.Length);
            w.Write(extraLen);
            
            w.Write(entry.Filename);
            if (zip64) WriteZip64ExtraField(copy, false);
        }
        
        void WriteCentralDirectoryRecord(ZipEntry entry) {
            ushort extraLen = (ushort)(zip64 ? zip64Extra : 0);
            ushort version = zip64 ? ver_zip64 : ver_norm;
            ZipEntry copy = entry;
            if (zip64) entry.Reset();
            
            w.Write(0x02014b50); // signature
            w.Write(version);
            w.Write(version);
            w.Write(entry.BitFlags);
            w.Write(entry.CompressionMethod);
            WriteLastModified();
            w.Write(entry.Crc32);
            w.Write((int)entry.CompressedSize);
            w.Write((int)entry.UncompressedSize);
            w.Write((ushort)entry.Filename.Length);
            w.Write(extraLen);
            w.Write((ushort)0);  // file comment length
            w.Write((ushort)0);  // disk number
            w.Write((ushort)0);  // internal attributes
            w.Write(0);          // external attributes
            w.Write((int)entry.LocalHeaderOffset);
            
            w.Write(entry.Filename);
            if (zip64) WriteZip64ExtraField(copy, true);
        }
        
        void WriteZip64ExtraField(ZipEntry entry, bool offset) {
            int len = zip64Extra - 4; // ignore header size
            if (!offset) len -= 8;
            
            w.Write((ushort)1); // mapping id
            w.Write((ushort)len);
            w.Write(entry.UncompressedSize);
            w.Write(entry.CompressedSize);
            if (offset) w.Write(entry.LocalHeaderOffset);
        }
        
        void WriteLastModified() {
            int modTime = (now.Second / 2) | (now.Minute << 5) | (now.Hour << 11);
            int modDate = (now.Day) | (now.Month << 5) | ((now.Year - 1980) << 9);
            w.Write((ushort)modTime);
            w.Write((ushort)modDate);
        }
        
        void WriteZip64EndOfCentralDirectoryRecord() {
            w.Write((uint)0x06064b50);
            const long zip64EndDataSize = (2 * 2) + (2 * 4) + (4 * 8);
            w.Write(zip64EndDataSize);
            w.Write(ver_zip64);
            w.Write(ver_zip64);
            w.Write(0); // disk number
            w.Write(0); // disk number of central directory
            w.Write((long)numEntries);
            w.Write((long)numEntries);
            w.Write(centralDirSize);
            w.Write(centralDirOffset);
        }
        
        void WriteZip64EndOfCentralDirectoryLocator() {
            w.Write((uint)0x07064b50);
            w.Write(0); // disk number of zip64 end of central directory
            w.Write(zip64EndOffset);
            w.Write(1); // total number of disks
        }
        
        void WriteEndOfCentralDirectoryRecord() {
            w.Write(0x06054b50);
            w.Write((ushort)0); // disk number
            w.Write((ushort)0); // disk number of start
            w.Write((ushort)numEntries);
            w.Write((ushort)numEntries);
            w.Write((uint)centralDirSize);
            w.Write((uint)centralDirOffset);
            w.Write((ushort)0);  // comment length
        }
    }
}
