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

        protected override void SaveUndoData(List<Player.UndoPos> buffer, string path) {
            using (FileStream fs = File.Create(path)) {
                BinaryWriter w = new BinaryWriter(fs);
                long entriesPos = 0;
                ChunkHeader lastChunk = default(ChunkHeader);
                
                foreach (Player.UndoPos uP in buffer) {
                    int timeDiff = (int)(uP.timePlaced.ToUniversalTime() - lastChunk.BaseTime).TotalSeconds;
                    if (lastChunk.LevelName != uP.mapName || timeDiff > 65535 || lastChunk.Entries == ushort.MaxValue) {
                        WriteChunkEntries(w, lastChunk.Entries, entriesPos);
                        lastChunk = WriteEmptyChunk(w, uP, ref entriesPos);
                    }
                    
                    w.Write((ushort)timeDiff);
                    w.Write(uP.x); w.Write(uP.y); w.Write(uP.z);
                    w.Write(uP.type);
                    w.Write((byte)0); // block definitions placeholder
                    w.Write(uP.newtype);
                    w.Write((byte)0); // block definitions placeholder
                    lastChunk.Entries++;
                }
                if (lastChunk.Entries > 0)
                    WriteChunkEntries(w, lastChunk.Entries, entriesPos);
            }
        }
        
        protected override void ReadUndoData(List<Player.UndoPos> buffer, string path) {
            DateTime now = DateTime.Now;
            Player.UndoPos Pos;
            using (Stream fs = File.OpenRead(path))
                using (BinaryReader r = new BinaryReader(fs))
            {
                int approxEntries = (int)(fs.Length / 12);
                if (buffer.Capacity < approxEntries)
                    buffer.Capacity = approxEntries;
                while (fs.Position < fs.Length) {
                    ChunkHeader chunk = ReadHeader(fs, r);
                    Pos.mapName = chunk.LevelName;
                    
                    for (int j = 0; j < chunk.Entries; j++ ) {
                        Pos.timePlaced = chunk.BaseTime.AddSeconds(r.ReadUInt16());
                        Pos.x = r.ReadUInt16(); Pos.y = r.ReadUInt16(); Pos.z = r.ReadUInt16();
                        Pos.type = r.ReadByte(); r.ReadByte(); // block definitions placeholder
                        Pos.newtype = r.ReadByte(); r.ReadByte(); // block definitions placeholder
                        buffer.Add(Pos);
                    }
                }
            }
        }
        
        protected override bool UndoEntry(Player p, string path, long seconds) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            DateTime now = DateTime.Now;
            Player.UndoPos Pos;
            
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
                    Pos.mapName = chunk.LevelName;
                    fs.Seek(chunk.DataPosition, SeekOrigin.Begin);
                    
                    for (int j = 0; j < chunk.Entries; j++ ) {
                        DateTime time = chunk.BaseTime.AddSeconds(r.ReadUInt16());
                        if (time.AddSeconds(seconds) < now) return false;
                        Pos.x = r.ReadUInt16(); Pos.y = r.ReadUInt16(); Pos.z = r.ReadUInt16();
                        
                        Pos.type = lvl.GetTile(Pos.x, Pos.y, Pos.z);
                        byte oldType = r.ReadByte(); r.ReadByte(); // block definitions placeholder
                        byte newType = r.ReadByte(); r.ReadByte(); // block definitions placeholder

                        if (Pos.type == newType || Block.Convert(Pos.type) == Block.water
                            || Block.Convert(Pos.type) == Block.lava || Pos.type == Block.grass) {
                            
                            Pos.newtype = oldType;
                            Pos.timePlaced = now;
                            lvl.Blockchange(Pos.x, Pos.y, Pos.z, Pos.newtype, true);
                            if (p != null)
                                p.RedoBuffer.Add(Pos);
                        }
                    }
                }
            }
            return true;
        }

        protected override bool HighlightEntry(Player p, string path, long seconds) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            DateTime now = DateTime.Now;
            
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
                    
                    for (int j = 0; j < chunk.Entries; j++ ) {
                        DateTime time = chunk.BaseTime.AddSeconds(r.ReadUInt16());
                        if (time.AddSeconds(seconds) < now) return false;
                        ushort x = r.ReadUInt16(), y = r.ReadUInt16(), z = r.ReadUInt16();
                        
                        byte lvlTile = lvl.GetTile(x, y, z);
                        byte oldType = r.ReadByte(); r.ReadByte(); // block definitions placeholder
                        byte newType = r.ReadByte(); r.ReadByte(); // block definitions placeholder

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
        
        static bool CheckChunk(ChunkHeader chunk, DateTime now, long seconds, Player p, out Level lvl) {
            DateTime time = chunk.BaseTime;
            lvl = null;
            if (time.AddSeconds(65536).AddSeconds(seconds) < now)
                return false; // we can safely discard the entire chunk
            
            lvl = Level.FindExact(chunk.LevelName);
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
                s.Seek(header.Entries * 12, SeekOrigin.Current);
                list.Add(header);
            }
        }
        
        static ChunkHeader ReadHeader(Stream s, BinaryReader r) {
            ChunkHeader header = default(ChunkHeader);
            byte[] mapNameData = r.ReadBytes(r.ReadUInt16());
            header.LevelName = Encoding.UTF8.GetString(mapNameData);
            
            header.BaseTime = new DateTime(r.ReadInt64(), DateTimeKind.Local);
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
        
        static ChunkHeader WriteEmptyChunk(BinaryWriter w, Player.UndoPos uP, ref long entriesPos) {
            byte[] mapBytes = Encoding.UTF8.GetBytes(uP.mapName);
            w.Write((ushort)mapBytes.Length);
            w.Write(mapBytes);
            w.Write(uP.timePlaced.ToLocalTime().Ticks);
            
            entriesPos = w.BaseStream.Position;
            w.Write((ushort)0);
            ChunkHeader header = default(ChunkHeader);
            header.LevelName = uP.mapName; header.BaseTime = uP.timePlaced;
            return header;
        }
    }
}
