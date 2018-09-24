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

    /// <summary> Reads undo entries in the old MCGalaxy optimised undo binary format. </summary>
    public sealed class UndoFormatCBin : UndoFormat {
        
        protected override string Ext { get { return ".uncbin"; } }
        const int entrySize = 8;

        public override void EnumerateEntries(Stream s, UndoFormatArgs args) {
            List<ChunkHeader> list = new List<ChunkHeader>();
            UndoFormatEntry pos;
            DateTime time;

            ReadHeaders(list, s);
            for (int i = list.Count - 1; i >= 0; i--) {
                ChunkHeader chunk = list[i];
                // Can we safely discard the entire chunk?
                bool inRange = chunk.BaseTime.AddTicks((65536 >> 2) * TimeSpan.TicksPerSecond) >= args.Start;
                if (!inRange) { args.Finished = true; return; }
                if (!args.Map.CaselessEq(chunk.LevelName)) continue;
                
                s.Seek(chunk.DataPosition, SeekOrigin.Begin);
                if (args.Temp == null)
                    args.Temp = new byte[ushort.MaxValue * entrySize];
                s.Read(args.Temp, 0, chunk.Entries * entrySize);
                byte[] temp = args.Temp;
                
                for (int j = chunk.Entries - 1; j >= 0; j-- ) {
                    int offset = j * entrySize;
                    ushort flags = U16(temp, offset + 0);
                    // upper 2 bits for 'ext' or 'physics' type, lower 14 bits for time delta.
                    
                    // TODO: should this be instead:
                    // int delta = Flags & 0x3FFF;
                    // timeDeltaSeconds = delta >= 0x2000 ? (short)(delta - 16384) : (short)delta;
                    time = chunk.BaseTime.AddTicks((flags & 0x3FFF) * TimeSpan.TicksPerSecond);
                    if (time < args.Start) { args.Finished = true; return; }
                    if (time > args.End) continue;
                    
                    int index = I32(temp, offset + 2);
                    pos.X = (ushort)(index % chunk.Width);
                    pos.Y = (ushort)((index / chunk.Width) / chunk.Length);
                    pos.Z = (ushort)((index / chunk.Width) % chunk.Length);
                    
                    pos.Block = Block.FromRaw(temp[offset + 6],    (flags & (1 << 14)) != 0);
                    pos.NewBlock = Block.FromRaw(temp[offset + 7], (flags & (1 << 15)) != 0);
                    args.Output(pos);
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
    }
}
