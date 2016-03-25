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
using System.Linq;
using System.Text;
using MCGalaxy.Drawing;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Util {

    public sealed class UndoFileCBin : UndoFile {
        
        protected override string Extension { get { return ".uncbin"; } }
        const int entrySize = 8;

        protected override void SaveUndoData(List<Player.UndoPos> buffer, string path) {
            UndoCacheNode node = new UndoCacheNode();
            string lastLoggedName = "";
            
            using (FileStream fs = File.Create(path)) {
                BinaryWriter w = new BinaryWriter(fs);
                long entriesPos = 0;
                ChunkHeader last = default(ChunkHeader);
                
                foreach (Player.UndoPos uP in buffer) {
                    DateTime time = Server.StartTime.AddSeconds(uP.timeDelta);
                    int timeDiff = (int)(time - last.BaseTime).TotalSeconds;
                    if (last.LevelName != uP.mapName || timeDiff > (65535 >> 2) || last.Entries == ushort.MaxValue) {
                        if (!LevelInfo.ExistsOffline(uP.mapName)) {
                            if (uP.mapName != lastLoggedName) {
                                lastLoggedName = uP.mapName;
                                Server.s.Log("Missing map file\"" + lastLoggedName+ "\", skipping undo entries");
                            }
                            continue;
                        }
                        
                        ushort width, height, length;
                        LvlFile.LoadDimensions(LevelInfo.LevelPath(uP.mapName), out width, out height, out length);
                        node.Width = width; node.Height = height; node.Length = length;
                        WriteChunkEntries(w, last.Entries, entriesPos);
                        node.MapName = uP.mapName;
                        last = WriteEmptyChunk(w, node, time, ref entriesPos);
                    }
                    
                    UndoCacheItem item = UndoCacheItem.Make(node, 0, uP);
                    int flags = (item.Flags & 0xC000) | timeDiff;
                    w.Write((ushort)flags); w.Write(item.Index);
                    w.Write(item.Type); w.Write(item.NewType);
                    last.Entries++;
                }
                if (last.Entries > 0)
                    WriteChunkEntries(w, last.Entries, entriesPos);
            }
        }
        
        protected override void SaveUndoData(UndoCache buffer, string path) {
            using (FileStream fs = File.Create(path)) {
                BinaryWriter w = new BinaryWriter(fs);
                long entriesPos = 0;
                ChunkHeader last = default(ChunkHeader);
                UndoCacheNode node = buffer.Tail;
                
                while (node != null) {
                    List<UndoCacheItem> items = node.Items;
                    for (int i = 0; i < items.Count; i++) {
                        UndoCacheItem uP = items[i];
                        DateTime time = node.BaseTime.AddSeconds(uP.TimeDelta);
                        int timeDiff = (int)(time - last.BaseTime).TotalSeconds;
                        if (last.LevelName != node.MapName || timeDiff > (65535 >> 2) || last.Entries == ushort.MaxValue) {
                            WriteChunkEntries(w, last.Entries, entriesPos);
                            last = WriteEmptyChunk(w, node, time, ref entriesPos);
                        }
                        
                        int flags = (uP.Flags & 0xC000) | timeDiff;
                        w.Write((ushort)flags); w.Write(uP.Index);
                        w.Write(uP.Type); w.Write(uP.NewType);
                        last.Entries++;
                    }
                    if (last.Entries > 0)
                        WriteChunkEntries(w, last.Entries, entriesPos);
                    node = node.Prev;
                }
            }
        }
        
        protected override void ReadUndoData(List<Player.UndoPos> buffer, string path) {
            Player.UndoPos Pos;
            UndoCacheItem item = default(UndoCacheItem);
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
                        item.Flags = r.ReadUInt16();
                        DateTime time = chunk.BaseTime.AddTicks((item.Flags & 0x3FFF) * TimeSpan.TicksPerSecond);
                        Pos.timeDelta = (int)time.Subtract(Server.StartTime).TotalSeconds;
                        int index = r.ReadInt32();
                        Pos.x = (ushort)(index % chunk.Width);
                        Pos.y = (ushort)((index / chunk.Width) / chunk.Length);
                        Pos.z = (ushort)((index / chunk.Width) % chunk.Length);
                        
                        item.Type = r.ReadByte();
                        item.NewType = r.ReadByte();
                        item.GetExtBlock(out Pos.type, out Pos.extType);
                        item.GetNewExtBlock(out Pos.newtype, out Pos.newExtType);
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
            UndoCacheItem item = default(UndoCacheItem);
            
            using (Stream fs = File.OpenRead(path))
                using (BinaryReader r = new BinaryReader(fs))
            {
                ReadHeaders(list, r);
                for (int i = list.Count - 1; i >= 0; i--) {
                    ChunkHeader chunk = list[i];
                    Level lvl;
                    if (!CheckChunk(chunk, start, p, out lvl))
                        return false;
                    if (lvl == null || (p.level != null && !p.level.name.CaselessEq(lvl.name)))
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
                        item.Flags = U16(temp, offset + 0);
                        DateTime time = chunk.BaseTime.AddTicks((item.Flags & 0x3FFF) * TimeSpan.TicksPerSecond);
                        if (time < start) { buffer.CheckIfSend(true); return false; }
                        
                        int index = NetUtils.ReadI32(temp, offset + 2);
                        Pos.x = (ushort)(index % chunk.Width);
                        Pos.y = (ushort)((index / chunk.Width) / chunk.Length);
                        Pos.z = (ushort)((index / chunk.Width) % chunk.Length);
                        if (Pos.x < min.X || Pos.y < min.Y || Pos.z < min.Z ||
                            Pos.x > max.X || Pos.y > max.Y || Pos.z > max.Z) continue;
                        
                        item.Type = temp[offset + 6];
                        item.NewType = temp[offset + 7];
                        item.GetExtBlock(out Pos.type, out Pos.extType);
                        item.GetNewExtBlock(out Pos.newtype, out Pos.newExtType);
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
                        ushort flags = U16(temp, offset + 0);
                        DateTime time = chunk.BaseTime.AddTicks((flags & 0x3FFF) * TimeSpan.TicksPerSecond);
                        if (time < start) return false;
                        
                        int index = NetUtils.ReadI32(temp, offset + 2);
                        ushort x = (ushort)(index % chunk.Width);
                        ushort y = (ushort)((index / chunk.Width) / chunk.Length);
                        ushort z = (ushort)((index / chunk.Width) % chunk.Length);
                        HighlightBlock(p, lvl, temp[offset + 7], x, y, z);
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
            if (time.AddTicks((65536 >> 2) * TimeSpan.TicksPerSecond) < start)
                return false; // we can safely discard the entire chunk
            lvl = LevelInfo.FindExact(chunk.LevelName);
            return true;
        }
        
        struct ChunkHeader {
            public string LevelName;
            public DateTime BaseTime;            
            public ushort Entries;
            public ushort Width, Height, Length;
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
            header.BaseTime = new DateTime(r.ReadInt64(), DateTimeKind.Utc);
            header.Entries = r.ReadUInt16();
            
            header.Width = r.ReadUInt16();
            header.Height = r.ReadUInt16(); 
            header.Length = r.ReadUInt16();
            header.DataPosition = s.Position;
            return header;
        }
        
        static void WriteChunkEntries(BinaryWriter w, ushort entries, long entriesPos) {
            long curPos = w.BaseStream.Position;
            w.BaseStream.Seek(entriesPos, SeekOrigin.Begin);
            
            w.Write(entries);
            w.BaseStream.Seek(curPos, SeekOrigin.Begin);
        }
        
        static ChunkHeader WriteEmptyChunk(BinaryWriter w, UndoCacheNode node, DateTime time, ref long entriesPos) {
            ChunkHeader header = default(ChunkHeader);
            time = time.ToUniversalTime();
            byte[] mapBytes = Encoding.UTF8.GetBytes(node.MapName);
            w.Write((ushort)mapBytes.Length);
            w.Write(mapBytes); header.LevelName = node.MapName;
            w.Write(time.Ticks); header.BaseTime = time;
            
            entriesPos = w.BaseStream.Position;
            w.Write((ushort)0);
            w.Write((ushort)node.Width); header.Width = (ushort)node.Width;
            w.Write((ushort)node.Height); header.Height = (ushort)node.Height;
            w.Write((ushort)node.Length); header.Length = (ushort)node.Length;
            return header;
        }
    }
}
