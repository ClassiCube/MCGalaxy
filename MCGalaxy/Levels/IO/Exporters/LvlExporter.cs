/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;
using System.IO.Compression;

namespace MCGalaxy.Levels.IO {

    //WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
    //You MUST make it able to save and load as a new version other wise you will make old levels incompatible!
	public sealed class LvlExporter : IMapExporter {

		public override string Extension { get { return ".lvl"; } }
        
        const int bufferSize = 64 * 1024;
        public override void Write(Stream dst, Level lvl) {
            using (Stream gs = new GZipStream(dst, CompressionMode.Compress)) {
                // We need to copy blocks to a temp byte array due to the multithreaded nature of the server
                // Otherwise, some blocks can change between writing data and calculating its crc32, which
                // then causes the level to fail to load next time do to the crc32 not matching the data.
                byte[] buffer = new byte[bufferSize];
                
                WriteHeader(lvl, gs, buffer);
                WriteBlocksSection(lvl, gs, buffer);
                WriteBlockDefsSection(lvl, gs, buffer);
                WritePhysicsSection(lvl, gs, buffer);
            }
        }
        
        static void WriteHeader(Level lvl, Stream gs, byte[] header) {
            BitConverter.GetBytes(1874).CopyTo(header, 0);
            BitConverter.GetBytes(lvl.Width).CopyTo(header, 2);
            BitConverter.GetBytes(lvl.Length).CopyTo(header, 4);
            BitConverter.GetBytes(lvl.Height).CopyTo(header, 6);
            BitConverter.GetBytes(lvl.spawnx).CopyTo(header, 8);
            BitConverter.GetBytes(lvl.spawnz).CopyTo(header, 10);
            BitConverter.GetBytes(lvl.spawny).CopyTo(header, 12);
            header[14] = lvl.rotx;
            header[15] = lvl.roty;
            header[16] = (byte)lvl.permissionvisit;
            header[17] = (byte)lvl.permissionbuild;
            gs.Write(header, 0, 18);
        }
        
        static void WriteBlocksSection(Level lvl, Stream gs, byte[] buffer) {
            byte[] blocks = lvl.blocks;
            for (int i = 0; i < blocks.Length; i += bufferSize) {
                int len = Math.Min(bufferSize, blocks.Length - i);
                Buffer.BlockCopy(blocks, i, buffer, 0, len);
                gs.Write(buffer, 0, len);
            }
        }
        
        static void WriteBlockDefsSection(Level lvl, Stream gs, byte[] buffer) {
            gs.WriteByte(0xBD); // 'B'lock 'D'efinitions
            int index = 0;
            
            for (int y = 0; y < lvl.ChunksY; y++)
                for (int z = 0; z < lvl.ChunksZ; z++)
                    for (int x = 0; x < lvl.ChunksX; x++)
            {
                byte[] chunk = lvl.CustomBlocks[index];
                if (chunk == null) {
                    gs.WriteByte(0);
                } else {
                    gs.WriteByte(1);
                    Buffer.BlockCopy(chunk, 0, buffer, 0, chunk.Length);
                    gs.Write(buffer, 0, chunk.Length);
                }
                index++;
            }
        }
        
        unsafe static void WritePhysicsSection(Level lvl, Stream gs, byte[] buffer) {
            lock (lvl.physStepLock) {
                // Count the number of physics checks with extra info
                int used = 0, count = lvl.ListCheck.Count;
                Check[] checks = lvl.ListCheck.Items;
                for (int i = 0; i < count; i++) {
                    if (checks[i].data.Raw == 0) continue;
                    used++;
                }
                if (used == 0) return;
                
                gs.WriteByte(0xFC); // 'Ph'ysics 'C'hecks
                NetUtils.WriteI32(used, buffer, 0);
                gs.Write(buffer, 0, sizeof(int));
                
                fixed (byte* ptr = buffer) {
                    WritePhysicsEntries(gs, lvl.ListCheck, buffer, ptr);
                }
            }
        }
        
        unsafe static void WritePhysicsEntries(Stream gs, FastList<Check> items,
                                               byte[] buffer, byte* ptr) {
            Check[] checks = items.Items;
            int entries = 0, count = items.Count;
            int* ptrInt = (int*)ptr;
            const int bulkCount = bufferSize / 8;
            
            for (int i = 0; i < count; i++) {
                Check C = checks[i];
                // Does this check have extra physics data
                if (C.data.Raw == 0) continue;
                *ptrInt = C.b; ptrInt++;
                *ptrInt = (int)C.data.Raw; ptrInt++;
                entries++;
                
                // Have we filled the temp buffer?
                if (entries != bulkCount) continue;
                ptrInt = (int*)ptr;
                gs.Write(buffer, 0, entries * 8);
                entries = 0;
            }
            
            if (entries == 0) return;
            gs.Write(buffer, 0, entries * 8);
        }
    }
}