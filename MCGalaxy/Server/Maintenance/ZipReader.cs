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
    
    sealed class ZipReaderStream : Stream {
        public long CompressedLen;
        public Stream stream;
        
        public ZipReaderStream(Stream stream) { this.stream = stream; }
        public override bool CanRead  { get { return true;  } }
        public override bool CanSeek  { get { return false; } }
        public override bool CanWrite { get { return false; } }
        
        static Exception ex = new NotSupportedException();
        public override void Flush() { stream.Flush(); }
        public override long Length { get { throw ex; } }
        public override long Position { get { throw ex; } set { throw ex; } }
        public override long Seek(long offset, SeekOrigin origin) { throw ex; }
        public override void SetLength(long length) { throw ex; }
        public override void Write(byte[] buffer, int offset, int count) { throw ex; }
        public override void WriteByte(byte value) { throw ex; }
        
        public override int Read(byte[] buffer, int offset, int count) {
            if (CompressedLen <= 0) return 0;
            if (count >= CompressedLen) count = (int)CompressedLen;
            
            count = stream.Read(buffer, offset, count);
            CompressedLen -= count;
            return count;
        }
        
        public override int ReadByte() {
            if (CompressedLen <= 0) return -1;
            CompressedLen--;
            return stream.ReadByte();
        }
        
        public override void Close() { stream = null; }
    }

    /// <summary> Reads entries from a ZIP archive. </summary>
    public sealed class ZipReader {
        BinaryReader reader;
        Stream stream;
        
        List<ZipEntry> entries = new List<ZipEntry>();
        int numEntries;
        long centralDirOffset, zip64EndOffset;
        const ushort ver_norm = 20, ver_zip64 = 45;
        const ushort zip64CentralExtra = 28, zip64LocalExtra = 20;
        
        public ZipReader(Stream stream) {
            this.stream = stream;
            reader = new BinaryReader(stream);
        }
        
        public Stream GetEntry(int i, out string file) {
            ZipEntry entry = entries[i];
            stream.Seek(entry.LocalHeaderOffset, SeekOrigin.Begin);
            file = null;
            
            uint sig = reader.ReadUInt32();
            if (sig != ZipEntry.SigLocal) {
                Logger.Log(LogType.Warning, "&WFailed to find local file entry {0}", i); return null;
            }
            
            entry = ReadLocalFileRecord();
            file = Encoding.UTF8.GetString(entry.Filename);
            
            ZipReaderStream part = new ZipReaderStream(stream);
            part.CompressedLen = entry.CompressedSize;
            
            if (entry.CompressionMethod == 0) return part;
            return new DeflateStream(part, CompressionMode.Decompress);
        }
        
        public int FindEntries() {
            stream.Seek(centralDirOffset, SeekOrigin.Begin);
            for (int i = 0; i < numEntries; i++) {
                uint sig = reader.ReadUInt32();
                if (sig != ZipEntry.SigCentral) {
                    Logger.Log(LogType.Warning, "&WFailed to find central dir entry {0}", i); return i;
                }
                
                ZipEntry entry = ReadCentralDirectoryRecord();
                entries.Add(entry);
            }
            return numEntries;
        }
        
        public void FindFooter() {
            BinaryReader r = reader;
            uint sig = 0;
            
            // At -22 for nearly all zips, but try a bit further back in case of comment
            int i, len = Math.Min(257, (int)stream.Length);
            for (i = 22; i < len; i++) {
                stream.Seek(-i, SeekOrigin.End);
                sig = r.ReadUInt32();
                if (sig == ZipEntry.SigEnd) break;
            }
            
            if (sig != ZipEntry.SigEnd) {
                Logger.Log(LogType.Warning, "&WFailed to find end of central directory"); return;
            }
            ReadEndOfCentralDirectoryRecord();
            
            if (centralDirOffset != uint.MaxValue) return;
            Logger.Log(LogType.SystemActivity, "Backup .zip is using ZIP64 format");
            
            stream.Seek(-i - 20, SeekOrigin.End);
            sig = r.ReadUInt32();
            if (sig != ZipEntry.SigZip64Loc) {
                Logger.Log(LogType.Warning, "&WFailed to find ZIP64 locator"); return;
            }
            ReadZip64EndOfCentralDirectoryLocator();
            
            stream.Seek(zip64EndOffset, SeekOrigin.Begin);
            sig = r.ReadUInt32();
            if (sig != ZipEntry.SigZip64End) {
                Logger.Log(LogType.Warning, "&WFailed to find ZIP64 end"); return;
            }
            ReadZip64EndOfCentralDirectoryRecord();
        }
        
        
        ZipEntry ReadLocalFileRecord() {
            BinaryReader r = reader;
            ZipEntry entry = default(ZipEntry);
            
            r.ReadUInt16(); // version
            r.ReadUInt16(); // bitflags
            entry.CompressionMethod = r.ReadUInt16();
            r.ReadUInt32(); // last modified
            r.ReadUInt32(); // crc32
            entry.CompressedSize = r.ReadUInt32();
            entry.UncompressedSize = r.ReadUInt32();
            int filenameLen = r.ReadUInt16();
            int extraLen = r.ReadUInt16();
            
            entry.Filename = r.ReadBytes(filenameLen);
            if (extraLen == 0) return entry;
            long extraEnd = stream.Position + extraLen;
            
            // zip 64 mapping ID
            if (r.ReadUInt16() == 1) {
                r.ReadUInt16(); // data len
                if (entry.UncompressedSize == uint.MaxValue)
                    entry.UncompressedSize = r.ReadInt64();
                if (entry.CompressedSize   == uint.MaxValue)
                    entry.CompressedSize   = r.ReadInt64();
            }

            stream.Seek(extraEnd, SeekOrigin.Begin);
            return entry;
        }
        
        ZipEntry ReadCentralDirectoryRecord() {
            BinaryReader r = reader;
            ZipEntry entry = default(ZipEntry);
            
            r.ReadUInt16(); // version
            r.ReadUInt16(); // version
            r.ReadUInt16(); // bit flags
            entry.CompressionMethod = r.ReadUInt16();
            r.ReadUInt32(); // last modified
            r.ReadUInt32(); // crc32
            entry.CompressedSize = r.ReadUInt32();
            entry.UncompressedSize = r.ReadUInt32();
            int filenameLen = r.ReadUInt16();
            int extraLen = r.ReadUInt16();
            int commentLen = r.ReadUInt16();
            r.ReadUInt16(); // disk number
            r.ReadUInt16(); // internal attributes
            r.ReadUInt32(); // external attributes
            entry.LocalHeaderOffset = r.ReadUInt32();
            
            entry.Filename = r.ReadBytes(filenameLen);
            if (extraLen == 0) return entry;
            long extraEnd = stream.Position + extraLen;
            
            // zip 64 mapping ID
            if (r.ReadUInt16() == 1) {
                r.ReadUInt16(); // data len
                if (entry.UncompressedSize  == uint.MaxValue)
                    entry.UncompressedSize  = r.ReadInt64();
                if (entry.CompressedSize    == uint.MaxValue)
                    entry.CompressedSize    = r.ReadInt64();
                if (entry.LocalHeaderOffset == uint.MaxValue)
                    entry.LocalHeaderOffset = r.ReadInt64();
            }

            stream.Seek(extraEnd, SeekOrigin.Begin);
            return entry;
        }
        
        void ReadZip64EndOfCentralDirectoryRecord() {
            BinaryReader r = reader;
            r.ReadInt64(); // zip64 end of central dir size
            r.ReadUInt16(); // version
            r.ReadUInt16(); // version
            r.ReadUInt32(); // disk number
            r.ReadUInt32(); // disk number of central directory
            numEntries = (int)r.ReadInt64();
            r.ReadInt64(); // num entries on disk
            r.ReadInt64(); // central dir size
            centralDirOffset = r.ReadInt64();
        }
        
        void ReadZip64EndOfCentralDirectoryLocator() {
            BinaryReader r = reader;
            r.ReadUInt32(); // disk number of zip64 end of central directory
            zip64EndOffset = reader.ReadInt64();
            r.ReadUInt32(); // total number of disks
        }
        
        void ReadEndOfCentralDirectoryRecord() {
            BinaryReader r = reader;
            r.ReadUInt16(); // disk number
            r.ReadUInt16(); // disk number of start
            numEntries = r.ReadUInt16();
            r.ReadUInt16(); // num entries on disk
            r.ReadUInt32(); // cental dir size
            centralDirOffset = r.ReadUInt32();
            r.ReadUInt16(); // comment length
        }
    }
}
