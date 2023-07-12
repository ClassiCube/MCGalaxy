/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Levels.IO;
using MCGalaxy.Maths;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy.DB 
{
    /// <summary> Exports a BlockDB table to the new binary format. </summary>
    public sealed class BlockDBTableDumper 
    {       
        string mapName;
        Dictionary<string, int> nameCache = new Dictionary<string, int>();
        Stream stream;
        bool errorOccurred;
        Vec3U16 dims;
        BlockDBEntry entry;
        BlockDBEntry[] buffer = new BlockDBEntry[BlockDBFile.BULK_ENTRIES];
        int bufferCount;
        uint entriesWritten;
        
        public void DumpTable(string table) {
            bufferCount    = 0;
            entriesWritten = 0;
            errorOccurred  = false;
            mapName = table.Substring("Block".Length);
            
            try {
                Database.ReadRows(table, "*", DumpRow);
                FlushBuffer();
                AppendCbdbFile();
                SaveCbdbFile();
            } finally {
                if (stream != null) stream.Close();
                stream = null;
            }
            
            if (errorOccurred) return;
            Database.DeleteTable(table);
        }
        
        void DumpRow(ISqlRecord record) {
            if (errorOccurred) return;
            
            try {
                if (stream == null) {
                    stream = File.Create(BlockDBFile.DumpPath(mapName));
                    string lvlPath = LevelInfo.MapPath(mapName);
                    dims = IMapImporter.Formats[0].ReadDimensions(lvlPath);
                    BlockDBFile.V1.WriteHeader(stream, dims); // TODO: BlockDBFile.CurrentVersion ??
                }
                
                // Only log maps which have a used BlockDB to avoid spam
                entriesWritten++;
                if (entriesWritten == 10) {
                    string progress = " (" + DBUpgrader.Progress + ")";
                    Logger.Log(LogType.SystemActivity, "Dumping BlockDB for " + mapName + progress);
                }
                
                ExtractBlock(record);
                ExtractCoords(record);
                ExtractPlayerID(record);
                ExtractTimestamp(record);
                
                buffer[bufferCount++] = entry;
                if (bufferCount == BlockDBFile.BULK_ENTRIES)
                    FlushBuffer();
            } catch (Exception ex) {
                Logger.LogError(ex);
                errorOccurred = true;
            }
        }
        
        void FlushBuffer() {
            if (bufferCount == 0) return;
            
            BlockDBFile.V1.WriteEntries(stream, buffer, bufferCount);
            bufferCount = 0;
        }
        
        unsafe void AppendCbdbFile() {
            string path = BlockDBFile.FilePath(mapName);
            if (!File.Exists(path) || stream == null) return;
            
            BlockDBFile fmt;
            byte[] bulk = new byte[BlockDBFile.BULK_BUFFER_SIZE];
            Vec3U16 dims;
            
            using (Stream cbdb = File.OpenRead(path)) {
                fmt = BlockDBFile.ReadHeader(cbdb, out dims);               
                fixed (byte* ptr = bulk) {
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    
                    while (true) 
                    {
                        int count = fmt.ReadForward(cbdb, bulk, entryPtr);
                        if (count == 0) break;
                        
                        BlockDBFile.V1.WriteRaw(stream, bulk, entryPtr, count); // TODO: BlockDBFile.CurrentVersion ??
                    }
                }
            }
        }
        
        void SaveCbdbFile() {
            if (stream == null) return;
            stream.Close();
            stream = null;
            
            string dumpPath = BlockDBFile.DumpPath(mapName);
            string filePath = BlockDBFile.FilePath(mapName);
            if (File.Exists(filePath)) File.Delete(filePath);
            File.Move(dumpPath, filePath);
        }
        
        
        void ExtractBlock(ISqlRecord record) {
            entry.OldRaw    = Block.Invalid;
            entry.NewRaw    = (byte)record.GetInt32(5);
            byte blockFlags = (byte)record.GetInt32(6);
            entry.Flags = BlockDBFlags.ManualPlace;
            
            if ((blockFlags & 1) != 0) { // deleted block
                entry.NewRaw = Block.Air;
            }
            if ((blockFlags & 2) != 0) { // new block is custom
                entry.Flags |= BlockDBFlags.NewExtended;
            }
        }
        
        void ExtractCoords(ISqlRecord record) {
            int x = record.GetInt32(2);
            int y = record.GetInt32(3);
            int z = record.GetInt32(4);
            entry.Index = x + dims.X * (z + dims.Z * y);
        }
        
        void ExtractPlayerID(ISqlRecord record) {
            int id;
            string user = record.GetString(0);
            if (!nameCache.TryGetValue(user, out id)) {
                int[] ids = NameConverter.FindIds(user);
                if (ids.Length > 0) {
                    nameCache[user] = ids[0];
                } else {
                    nameCache[user] = NameConverter.InvalidNameID(user);
                }
            }
            entry.PlayerID = id;
        }
        
        void ExtractTimestamp(ISqlRecord record) {
            DateTime time   = record.GetDateTime(1).ToUniversalTime();
            entry.TimeDelta = (int)time.Subtract(BlockDB.Epoch).TotalSeconds;
        }
    }
}