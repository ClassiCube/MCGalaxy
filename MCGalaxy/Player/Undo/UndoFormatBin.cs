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

namespace MCGalaxy.Undo {

    public sealed class UndoFormatBin : UndoFormat {
        
        protected override string Ext { get { return ".unbin"; } }
        const int entrySize = 12;
        
        protected override IEnumerable<UndoFormatEntry> GetEntries(Stream s, UndoFormatArgs args) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            UndoFormatEntry pos;
            bool super = Player.IsSuper(args.Player);
            DateTime start = args.Start;
            
            ReadHeaders(list, s);
            for (int i = list.Count - 1; i >= 0; i--) {
                ChunkHeader chunk = list[i];
                // Can we safely discard the entire chunk?
                bool inRange = chunk.BaseTime.AddTicks(65536 * TimeSpan.TicksPerSecond) >= start;
                if (!inRange) { args.Stop = true; yield break; }
                if (!super && !args.Player.level.name.CaselessEq(chunk.LevelName)) continue;
                pos.LevelName = chunk.LevelName;
                
                s.Seek(chunk.DataPosition, SeekOrigin.Begin);
                if (args.Temp == null)
                    args.Temp = new byte[ushort.MaxValue * entrySize];
                s.Read(args.Temp, 0, chunk.Entries * entrySize);
                byte[] temp = args.Temp;
                
                for (int j = chunk.Entries - 1; j >= 0; j-- ) {
                    int offset = j * entrySize;
                    pos.Time = chunk.BaseTime.AddTicks(U16(temp, offset + 0) * TimeSpan.TicksPerSecond);
                    if (pos.Time < start) { args.Stop = true; yield break; }
                    
                    pos.X = U16(temp, offset + 2);
                    pos.Y = U16(temp, offset + 4);
                    pos.Z = U16(temp, offset + 6);
                    
                    pos.Block = temp[offset + 8]; pos.ExtBlock = temp[offset + 9];
                    pos.NewBlock = temp[offset + 10]; pos.NewExtBlock = temp[offset + 11];
                    yield return pos;
                }
            }
        }
        
        static ushort U16(byte[] buffer, int offset) {
            return (ushort)(buffer[offset + 0] | buffer[offset + 1] << 8);
        }
        
        struct ChunkHeader {
            public string LevelName;
            public DateTime BaseTime;
            public ushort Entries;
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
            
            header.BaseTime = new DateTime(r.ReadInt64(), DateTimeKind.Local).ToUniversalTime();
            header.Entries = r.ReadUInt16();
            header.DataPosition = s.Position;
            return header;
        }
    }
}
