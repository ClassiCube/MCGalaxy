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
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Util {

    public sealed class UndoFileCBin : UndoFile {
        
        protected override string Ext { get { return ".uncbin"; } }
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
                                Server.s.Log("Missing map file \"" + lastLoggedName+ "\", skipping undo entries");
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
                UndoCacheNode node = buffer.Head;
                
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
                    node = node.Next;
                }
            }
        }

        protected override IEnumerable<Player.UndoPos> GetEntries(Stream s, UndoEntriesArgs args) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            Player.UndoPos pos;
            UndoCacheItem item = default(UndoCacheItem);
            bool super = args.Player == null || args.Player.ircNick != null;
            DateTime start = args.StartRange;            

            ReadHeaders(list, s);
            for (int i = list.Count - 1; i >= 0; i--) {
                ChunkHeader chunk = list[i];
                // Can we safely discard the entire chunk?
                bool inRange = chunk.BaseTime.AddTicks((65536 >> 2) * TimeSpan.TicksPerSecond) >= start;
                if (!inRange) { args.Stop = true; yield break; }
                if (!super && !args.Player.level.name.CaselessEq(chunk.LevelName)) continue;
                pos.mapName = chunk.LevelName;
                
                s.Seek(chunk.DataPosition, SeekOrigin.Begin);
                if (args.Temp == null)
                    args.Temp = new byte[ushort.MaxValue * entrySize];
                s.Read(args.Temp, 0, chunk.Entries * entrySize);
                byte[] temp = args.Temp;
                
                for (int j = chunk.Entries - 1; j >= 0; j-- ) {
                    int offset = j * entrySize;
                    item.Flags = U16(temp, offset + 0);
                    DateTime time = chunk.BaseTime.AddTicks((item.Flags & 0x3FFF) * TimeSpan.TicksPerSecond);
                    if (time < start) { args.Stop = true; yield break; }
                    pos.timeDelta = (int)time.Subtract(Server.StartTime).TotalSeconds;
                    
                    int index = I32(temp, offset + 2);
                    pos.x = (ushort)(index % chunk.Width);
                    pos.y = (ushort)((index / chunk.Width) / chunk.Length);
                    pos.z = (ushort)((index / chunk.Width) % chunk.Length);
                    
                    item.Type = temp[offset + 6];
                    item.NewType = temp[offset + 7];
                    item.GetBlock(out pos.type, out pos.extType);
                    item.GetNewBlock(out pos.newtype, out pos.newExtType);
                    yield return pos;
                }
            }
        }
        
        static ushort U16(byte[] buffer, int offset) {
            return (ushort)(buffer[offset + 0] | buffer[offset + 1] << 8);
        }
        
        static int I32(byte[] buffer, int offset) {
            return buffer[offset + 0] | buffer[offset + 1] << 8 |
                buffer[offset + 2] << 16 | buffer[offset + 3] << 24;
        }
        
        struct ChunkHeader {
            public string LevelName;
            public DateTime BaseTime;
            public ushort Entries;
            public ushort Width, Height, Length;
            public long DataPosition;
        }
        
        static void ReadHeaders(List<ChunkHeader> list, Stream s) {
            BinaryReader r = new BinaryReader(s);
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
