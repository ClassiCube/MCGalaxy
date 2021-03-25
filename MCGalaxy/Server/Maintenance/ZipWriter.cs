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
        
        public const uint SigLocal    = 0x04034b50;
        public const uint SigCentral  = 0x02014b50;
        public const uint SigEnd      = 0x06054b50;
        public const uint SigZip64End = 0x06064b50;
        public const uint SigZip64Loc = 0x07064b50;
    }
    
    sealed class ZipWriterStream : Stream {
        public uint Crc32 = uint.MaxValue;
        public long CompressedLen;
        public Stream stream;
        
        public ZipWriterStream(Stream stream) { this.stream = stream; }        
        public override bool CanRead  { get { return false; } }
        public override bool CanSeek  { get { return false; } }
        public override bool CanWrite { get { return true;  } }
        
        static Exception ex = new NotSupportedException();
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
        static ZipWriterStream() {
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

    /// <summary> Writes entries into a ZIP archive. </summary>
    public sealed class ZipWriter {
        BinaryWriter writer;
        Stream stream;
        byte[] buffer = new byte[81920];
        
        DateTime now = DateTime.Now;
        bool zip64;
        List<ZipEntry> entries = new List<ZipEntry>();
        
        int numEntries;
        long centralDirOffset, centralDirSize, zip64EndOffset;
        const ushort ver_norm = 20, ver_zip64 = 45;
        const ushort zip64CentralExtra = 28, zip64LocalExtra = 20;
        
        public ZipWriter(Stream stream) {
            this.stream = stream;
            writer = new BinaryWriter(stream);
        }
        
        public void WriteEntry(Stream src, string file, bool compress) {
            ZipEntry entry = default(ZipEntry);
            entry.Filename = Encoding.UTF8.GetBytes(file);
            entry.LocalHeaderOffset = stream.Position;
            
            // leave some room to fill in header later
            int headerSize = 30 + entry.Filename.Length + zip64LocalExtra;
            stream.Write(buffer, 0, headerSize);
            
            // set bit flag for non-ascii filename
            foreach (char c in file) {
                if (c < ' ' || c > '~') entry.BitFlags |= (1 << 11);
            }
            
            ZipWriterStream dst = new ZipWriterStream(stream);
            entry.UncompressedSize = dst.WriteStream(src, buffer, compress);
            dst.stream = null;
            
            if (compress && entry.UncompressedSize > 0) 
                entry.CompressionMethod = 8;
            
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
                if (!zip64) entry.LocalHeaderOffset += zip64LocalExtra;
                
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
            ushort extraLen = (ushort)(zip64 ? zip64LocalExtra : 0);
            ushort version = zip64 ? ver_zip64 : ver_norm;
            BinaryWriter w = writer;
            ZipEntry copy = entry;
            if (zip64) entry.Reset();
            
            w.Write(ZipEntry.SigLocal);
            w.Write(version);
            w.Write(entry.BitFlags);
            w.Write(entry.CompressionMethod);
            WriteLastModified();
            w.Write(entry.Crc32);
            w.Write((uint)entry.CompressedSize);
            w.Write((uint)entry.UncompressedSize);
            w.Write((ushort)entry.Filename.Length);
            w.Write(extraLen);
            
            w.Write(entry.Filename);
            if (!zip64) return;
            w.Write((ushort)1);
            
            w.Write((ushort)(zip64LocalExtra - 4));
            w.Write(copy.UncompressedSize);
            w.Write(copy.CompressedSize);
        }
        
        void WriteCentralDirectoryRecord(ZipEntry entry) {
            ushort extraLen = (ushort)(zip64 ? zip64CentralExtra : 0);
            ushort version = zip64 ? ver_zip64 : ver_norm;
            BinaryWriter w = writer;
            ZipEntry copy = entry;
            if (zip64) entry.Reset();
            
            w.Write(ZipEntry.SigCentral);
            w.Write(version);
            w.Write(version);
            w.Write(entry.BitFlags);
            w.Write(entry.CompressionMethod);
            WriteLastModified();
            w.Write(entry.Crc32);
            w.Write((uint)entry.CompressedSize);
            w.Write((uint)entry.UncompressedSize);
            w.Write((ushort)entry.Filename.Length);
            w.Write(extraLen);
            w.Write((ushort)0);  // file comment length
            w.Write((ushort)0);  // disc number
            w.Write((ushort)0);  // internal attributes
            w.Write(0);          // external attributes
            w.Write((uint)entry.LocalHeaderOffset);
            
            w.Write(entry.Filename);
            if (!zip64) return;
            w.Write((ushort)1);
            
            w.Write((ushort)(zip64CentralExtra - 4));
            w.Write(copy.UncompressedSize);
            w.Write(copy.CompressedSize);
            w.Write(copy.LocalHeaderOffset);
        }
        
        void WriteLastModified() {
            int modTime = (now.Second / 2) | (now.Minute << 5) | (now.Hour << 11);
            int modDate = (now.Day) | (now.Month << 5) | ((now.Year - 1980) << 9);
            writer.Write((ushort)modTime);
            writer.Write((ushort)modDate);
        }
        
        void WriteZip64EndOfCentralDirectoryRecord() {
            BinaryWriter w = writer;
            const long zip64EndDataSize = (2 * 2) + (2 * 4) + (4 * 8);
            
            w.Write(ZipEntry.SigZip64End);
            w.Write(zip64EndDataSize);
            w.Write(ver_zip64);
            w.Write(ver_zip64);
            w.Write(0); // disc number
            w.Write(0); // disc number of central directory
            w.Write((long)numEntries);
            w.Write((long)numEntries);
            w.Write(centralDirSize);
            w.Write(centralDirOffset);
        }
        
        void WriteZip64EndOfCentralDirectoryLocator() {
            BinaryWriter w = writer;
            w.Write(ZipEntry.SigZip64Loc);
            w.Write(0); // disc number of zip64 end of central directory
            w.Write(zip64EndOffset);
            w.Write(1); // total number of discs
        }
        
        void WriteEndOfCentralDirectoryRecord() {
            BinaryWriter w = writer;
            w.Write(ZipEntry.SigEnd);
            w.Write((ushort)0); // disc number
            w.Write((ushort)0); // disc number of start
            w.Write((ushort)numEntries);
            w.Write((ushort)numEntries);
            w.Write((uint)centralDirSize);
            w.Write((uint)centralDirOffset);
            w.Write((ushort)0);  // comment length
        }
    }
}
