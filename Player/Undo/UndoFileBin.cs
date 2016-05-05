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
using System.Text;
using MCGalaxy.Drawing;

namespace MCGalaxy.Util {

    public sealed class UndoFileBin : UndoFile {
        
        protected override string Extension { get { return ".unbin"; } }
        const int entrySize = 12;

        protected override void SaveUndoData(List<Player.UndoPos> buffer, string path) {
            throw new NotSupportedException("Non-optimised binary undo files have been deprecated");
        }
        
        protected override void SaveUndoData(UndoCache buffer, string path) {
            throw new NotSupportedException("Non-optimised binary undo files have been deprecated");
        }
        
        protected override void ReadUndoData(List<Player.UndoPos> buffer, string path) {
            Player.UndoPos Pos;
            using (Stream fs = File.OpenRead(path))
                using (BinaryReader r = new BinaryReader(fs))
            {
                int approxEntries = (int)(fs.Length / entrySize);
                if (buffer.Capacity < approxEntries)
                    buffer.Capacity = approxEntries;
                while (fs.Position < fs.Length) {
                    ChunkHeader chunk = ReadHeader(fs, r);
                    Pos.mapName = chunk.LevelName;
                    
                    for (int j = 0; j < chunk.Entries; j++ ) {
                        DateTime rawTime = chunk.BaseTime.AddSeconds(r.ReadUInt16());
                        Pos.timeDelta = (int)rawTime.Subtract(Server.StartTime).TotalSeconds;
                        Pos.x = r.ReadUInt16(); Pos.y = r.ReadUInt16(); Pos.z = r.ReadUInt16();
                        Pos.type = r.ReadByte(); Pos.extType = r.ReadByte();
                        Pos.newtype = r.ReadByte(); Pos.newExtType = r.ReadByte();
                        buffer.Add(Pos);
                    }
                }
            }
        }
        
        protected override bool UndoEntry(Player p, string path, Vec3U16[] marks,
                                          ref byte[] temp, DateTime start) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            int timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
            Player.UndoPos Pos = default(Player.UndoPos);
            Vec3U16 min = marks[0], max = marks[1];
            bool undoArea = min.X != ushort.MaxValue;
            
            using (Stream fs = File.OpenRead(path))
                using (BinaryReader r = new BinaryReader(fs))
            {
                ReadHeaders(list, r);
                for (int i = list.Count - 1; i >= 0; i--) {
                    ChunkHeader chunk = list[i];
                    Level lvl;
                    if (!CheckChunk(chunk, start, p, out lvl)) return false;
                    if (lvl == null) continue;
                    if (p != null && (p.level != null && !p.level.name.CaselessEq(lvl.name))) 
                    	continue;
                    
                    BufferedBlockSender buffer = new BufferedBlockSender(lvl);
                    if (!undoArea) {
                        min = new Vec3U16(0, 0, 0);
                        max = new Vec3U16((ushort)(lvl.Width - 1), (ushort)(lvl.Height - 1), (ushort)(lvl.Length - 1));
                    }
                    
                    Pos.mapName = chunk.LevelName;
                    fs.Seek(chunk.DataPosition, SeekOrigin.Begin);
                    if (temp == null) temp = new byte[ushort.MaxValue * entrySize];
                    fs.Read(temp, 0, chunk.Entries * entrySize);
                    
                    for (int j = chunk.Entries - 1; j >= 0; j-- ) {
                        int offset = j * entrySize;
                        DateTime time = chunk.BaseTime.AddTicks(U16(temp, offset + 0) * TimeSpan.TicksPerSecond);
                        if (time < start) { buffer.CheckIfSend(true); return false; }
                        Pos.x = U16(temp, offset + 2); Pos.y = U16(temp, offset + 4); Pos.z = U16(temp, offset + 6);
                        if (Pos.x < min.X || Pos.y < min.Y || Pos.z < min.Z ||
                            Pos.x > max.X || Pos.y > max.Y || Pos.z > max.Z) continue;
                        
                        Pos.type = temp[offset + 8]; Pos.extType = temp[offset + 9];
                        Pos.newtype = temp[offset + 10]; Pos.newExtType = temp[offset + 11];
                        UndoBlock(p, lvl, Pos, timeDelta, buffer);
                    }
                    buffer.CheckIfSend(true);
                }
            }
            return true;
        }

        protected override bool HighlightEntry(Player p, string path,
                                               ref byte[] temp, DateTime start) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            
            using (Stream fs = File.OpenRead(path))
                using (BinaryReader r = new BinaryReader(fs))
            {
                ReadHeaders(list, r);
                for (int i = list.Count - 1; i >= 0; i--) {
                    ChunkHeader chunk = list[i];
                    Level lvl;
                    if (!CheckChunk(chunk, start, p, out lvl))
                        return false;
                    if (lvl == null || lvl != p.level) continue;
                    
                    fs.Seek(chunk.DataPosition, SeekOrigin.Begin);
                    if (temp == null) temp = new byte[ushort.MaxValue * entrySize];
                    fs.Read(temp, 0, chunk.Entries * entrySize);
                    
                    for (int j = chunk.Entries - 1; j >= 0; j-- ) {
                        int offset = j * entrySize;
                        DateTime time = chunk.BaseTime.AddTicks(U16(temp, offset + 0) * TimeSpan.TicksPerSecond);
                        if (time < start) return false;
                        ushort x = U16(temp, offset + 2), y = U16(temp, offset + 4), z = U16(temp, offset + 6);
                        HighlightBlock(p, lvl, temp[offset + 10], x, y, z);
                    }
                }
            }
            return true;
        }
        
        static ushort U16(byte[] buffer, int offset) {
            return (ushort)(buffer[offset + 0] | buffer[offset + 1] << 8);
        }
        
        static bool CheckChunk(ChunkHeader chunk, DateTime start, Player p, out Level lvl) {
            DateTime time = chunk.BaseTime;
            lvl = null;
            if (time.AddTicks(65536 * TimeSpan.TicksPerSecond) < start)
                return false; // we can safely discard the entire chunk
            lvl = LevelInfo.FindExact(chunk.LevelName);
            return true;
        }
        
        struct ChunkHeader {
            public string LevelName;
            public DateTime BaseTime;
            public ushort Entries;
            public long DataPosition;
        }
        
        static void ReadHeaders(List<ChunkHeader> list, BinaryReader r) {
            Stream s = r.BaseStream;
            long len = s.Length;
            while (s.Position < len) {
                ChunkHeader header = ReadHeader(s, r);
                s.Seek(header.Entries * entrySize, SeekOrigin.Current);
                list.Add(header);
            }
        }
        
        static ChunkHeader ReadHeader(Stream s, BinaryReader r) {
            ChunkHeader header = default(ChunkHeader);
            byte[] mapNameData = r.ReadBytes(r.ReadUInt16());
            header.LevelName = Encoding.UTF8.GetString(mapNameData);
            
            header.BaseTime = new DateTime(r.ReadInt64(), DateTimeKind.Local).ToUniversalTime();
            header.Entries = r.ReadUInt16();
            header.DataPosition = s.Position;
            return header;
        }
        
        static void WriteChunkEntries(BinaryWriter w, ushort entries, long entriesPos) {
            long curPos = w.BaseStream.Position;
            w.BaseStream.Seek(entriesPos, SeekOrigin.Begin);
            
            w.Write(entries);
            w.BaseStream.Seek(curPos, SeekOrigin.Begin);
        }
        
        static ChunkHeader WriteEmptyChunk(BinaryWriter w, string mapName, DateTime time, ref long entriesPos) {
            byte[] mapBytes = Encoding.UTF8.GetBytes(mapName);
            w.Write((ushort)mapBytes.Length);
            w.Write(mapBytes);
            w.Write(time.ToLocalTime().Ticks);
            
            entriesPos = w.BaseStream.Position;
            w.Write((ushort)0);
            ChunkHeader header = default(ChunkHeader);
            header.LevelName = mapName; header.BaseTime = time;
            return header;
        }
    }
}
