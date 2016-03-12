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

namespace MCGalaxy.Util {

    public sealed class UndoFileBin : UndoFile {
        
        protected override string Extension { get { return ".unbin"; } }
        const int entrySize = 12;

        protected override void SaveUndoData(List<Player.UndoPos> buffer, string path) {
            using (FileStream fs = File.Create(path)) {
                BinaryWriter w = new BinaryWriter(fs);
                long entriesPos = 0;
                ChunkHeader last = default(ChunkHeader);
                
                foreach (Player.UndoPos uP in buffer) {
                    DateTime time = Server.StartTime.AddSeconds(uP.timeDelta);
                    int timeDiff = (int)(time - last.BaseTime).TotalSeconds;
                    if (last.LevelName != uP.mapName || timeDiff > 65535 || last.Entries == ushort.MaxValue) {
                        WriteChunkEntries(w, last.Entries, entriesPos);
                        last = WriteEmptyChunk(w, uP.mapName, time, ref entriesPos);
                    }
                    
                    w.Write((ushort)timeDiff);
                    w.Write(uP.x); w.Write(uP.y); w.Write(uP.z);
                    w.Write(uP.type); w.Write(uP.extType);
                    w.Write(uP.newtype); w.Write(uP.newExtType);
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
                        if (last.LevelName != node.MapName || timeDiff > 65535 || last.Entries == ushort.MaxValue) {
                            WriteChunkEntries(w, last.Entries, entriesPos);
                            last = WriteEmptyChunk(w, node.MapName, time, ref entriesPos);
                        }
                        
                        ushort x, y, z;
                        node.Unpack(uP.Index, out x, out y, out z);
                        byte tile = 0, extTile = 0;
                        uP.GetExtBlock(out tile, out extTile);
                        byte newTile = 0, newExtTile = 0;
                        uP.GetNewExtBlock(out newTile, out newExtTile);
                        
                        w.Write((ushort)timeDiff);
                        w.Write(x); w.Write(y); w.Write(z);
                        w.Write(tile); w.Write(extTile);
                        w.Write(newTile); w.Write(newExtTile);
                        last.Entries++;
                    }
                    if (last.Entries > 0)
                        WriteChunkEntries(w, last.Entries, entriesPos);
                    node = node.Prev;
                }
            }
        }
        
        protected override void ReadUndoData(List<Player.UndoPos> buffer, string path) {
            DateTime now = DateTime.Now;
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
        
        protected override bool UndoEntry(Player p, string path, ref byte[] temp, long seconds) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            DateTime now = DateTime.UtcNow;
            int timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
            Player.UndoPos Pos;
            bool isPlayer = p != null && p.group.Permission < LevelPermission.Nobody;
            seconds *= TimeSpan.TicksPerSecond;
            
            using (Stream fs = File.OpenRead(path))
                using (BinaryReader r = new BinaryReader(fs))
            {
                ReadHeaders(list, r);
                for (int i = list.Count - 1; i >= 0; i--) {
                    ChunkHeader chunk = list[i];
                    Level lvl;
                    if (!CheckChunk(chunk, now, seconds, p, out lvl))
                        return false;
                    if (lvl == null || (isPlayer && lvl != p.level)) continue;
                    BufferedBlockSender buffer = new BufferedBlockSender(lvl);
                    
                    Pos.mapName = chunk.LevelName;
                    fs.Seek(chunk.DataPosition, SeekOrigin.Begin);
                    if (temp == null) temp = new byte[ushort.MaxValue * entrySize];
                    fs.Read(temp, 0, chunk.Entries * entrySize);
                    
                    for (int j = chunk.Entries - 1; j >= 0; j-- ) {
                        int offset = j * entrySize;
                        DateTime time = chunk.BaseTime.AddTicks(U16(temp, offset + 0) * TimeSpan.TicksPerSecond);
                        if (time.AddTicks(seconds) < now) { buffer.CheckIfSend(true); return false; }
                        Pos.x = U16(temp, offset + 2); Pos.y = U16(temp, offset + 4); Pos.z = U16(temp, offset + 6);
                        
                        Pos.type = lvl.GetTile(Pos.x, Pos.y, Pos.z);
                        byte oldType = temp[offset + 8], oldExtType = temp[offset + 9];
                        byte newType = temp[offset + 10], newExtType = temp[offset + 11];

                        if (Pos.type == newType || Block.Convert(Pos.type) == Block.water
                            || Block.Convert(Pos.type) == Block.lava || Pos.type == Block.grass) {
                            
                            Pos.newtype = oldType; Pos.newExtType = oldExtType;
                            Pos.extType = newExtType; Pos.timeDelta = timeDelta;
                            if (lvl.DoBlockchange(p, Pos.x, Pos.y, Pos.z, Pos.newtype, Pos.newExtType)) {
                                buffer.Add(lvl.PosToInt(Pos.x, Pos.y, Pos.z), Pos.newtype, Pos.newExtType);
                                buffer.CheckIfSend(false);
                            }
                            if (p != null) p.RedoBuffer.Add(lvl, Pos);
                        }
                    }
                    buffer.CheckIfSend(true);
                }
            }
            return true;
        }

        protected override bool HighlightEntry(Player p, string path, ref byte[] temp, long seconds) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            DateTime now = DateTime.UtcNow;
            seconds *= TimeSpan.TicksPerSecond;
            
            using (Stream fs = File.OpenRead(path))
                using (BinaryReader r = new BinaryReader(fs))
            {
                ReadHeaders(list, r);
                for (int i = list.Count - 1; i >= 0; i--) {
                    ChunkHeader chunk = list[i];
                    Level lvl;
                    if (!CheckChunk(chunk, now, seconds, p, out lvl))
                        return false;
                    if (lvl == null || lvl != p.level) continue;
                    
                    fs.Seek(chunk.DataPosition, SeekOrigin.Begin);
                    if (temp == null) temp = new byte[ushort.MaxValue * entrySize];
                    fs.Read(temp, 0, chunk.Entries * entrySize);
                    
                    for (int j = chunk.Entries - 1; j >= 0; j-- ) {
                        int offset = j * entrySize;
                        DateTime time = chunk.BaseTime.AddTicks(U16(temp, offset + 0) * TimeSpan.TicksPerSecond);
                        if (time.AddTicks(seconds) < now) return false;
                        ushort x = U16(temp, offset + 2), y = U16(temp, offset + 4), z = U16(temp, offset + 6);
                        
                        byte lvlTile = lvl.GetTile(x, y, z);
                        byte oldType = temp[offset + 8], oldExtType = temp[offset + 9];
                        byte newType = temp[offset + 10], newExtType = temp[offset + 11];

                        if (lvlTile == newType || Block.Convert(lvlTile) == Block.water || Block.Convert(lvlTile) == Block.lava) {
                            
                            byte block = (lvlTile == Block.air || Block.Convert(lvlTile) == Block.water
                                          || Block.Convert(lvlTile) == Block.lava) ? Block.red : Block.green;
                            p.SendBlockchange(x, y, z, block);
                        }
                    }
                }
            }
            return true;
        }
        
        static ushort U16(byte[] buffer, int offset) {
            return (ushort)(buffer[offset + 0] | buffer[offset + 1] << 8);
        }
        
        static bool CheckChunk(ChunkHeader chunk, DateTime now, long seconds, Player p, out Level lvl) {
            DateTime time = chunk.BaseTime;
            lvl = null;
            if (time.AddSeconds(65536 + seconds) < now)
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
