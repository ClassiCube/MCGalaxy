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
using System.Data;
using System.IO;
using MCGalaxy.Levels.IO;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy.DB {
    
    /// <summary> Exports a BlockDB table to the new binary format. </summary>
    public sealed class BlockDBTableDumper {
        
        string mapName;
        Dictionary<string, int> nameCache = new Dictionary<string, int>();
        Stream stream;
        bool errorOccurred;
        Vec3U16 dims;
        BlockDBEntry entry;
        FastList<BlockDBEntry> buffer = new FastList<BlockDBEntry>(4096);
        
        public void DumpTable(string table) {
            buffer.Count = 0;
            errorOccurred = false;
            mapName = table.Substring("Block".Length);            
            
            Database.ExecuteReader("SELECT * FROM `" + table + "`", DumpRow);
            WriteBuffer(false);
            if (stream != null) stream.Close();
            stream = null;
            
            if (errorOccurred) return;
            //Database.Backend.DeleteTable(table); TODO: delete once tested
        }
        
        void DumpRow(IDataReader reader) {
            if (errorOccurred) return;
            
            try {
                if (stream == null) {
                    stream = File.Create("blockdb/" + mapName + ".dump");
                    string lvlPath = LevelInfo.LevelPath(mapName);
                    dims = IMapImporter.Formats[0].ReadDimensions(lvlPath);
                    BlockDBFile.WriteHeader(stream, dims);
                }
                
                UpdateBlock(reader);
                UpdateCoords(reader);
                UpdatePlayerID(reader);
                UpdateTimestamp(reader);
                
                buffer.Add(entry);
                WriteBuffer(false);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                errorOccurred = true;
            }
        }
        
        void WriteBuffer(bool force) {
            if (buffer.Count == 0) return;
            if (!force && buffer.Count < 4096) return;
            
            BlockDBFile.WriteEntries(stream, buffer);
            buffer.Count = 0;
        }
        
        
        void UpdateBlock(IDataReader reader) {
            entry.OldRaw = Block.Invalid;
            entry.NewRaw = reader.GetByte(5);
            byte blockFlags = reader.GetByte(6);
            entry.Flags = BlockDBFlags.ManualPlace;
            
            if ((blockFlags & 1) != 0) { // deleted block
                entry.NewRaw = Block.air;
            }
            if ((blockFlags & 2) != 0) { // new block is custom
                entry.Flags |= BlockDBFlags.NewCustom;
            }
        }
        
        void UpdateCoords(IDataReader reader) {
            int x = reader.GetInt32(2);
            int y = reader.GetInt32(3);
            int z = reader.GetInt32(4);
            entry.Index = x + dims.X * (z + dims.Z * y);
        }
        
        void UpdatePlayerID(IDataReader reader) {
            int id;
            string user = reader.GetString(0);
            if (!nameCache.TryGetValue(user, out id)) {
                id = NameConverter.FindIds(user)[0];
                nameCache[user] = id;
            }
            entry.PlayerID = id;
        }
        
        void UpdateTimestamp(IDataReader reader) {
            // date is in format yyyy-MM-dd hh:mm:ss
            string date = TableDumper.GetDate(reader, 1);
            int year =  (date[0] - '0') * 1000 + (date[1] - '0') * 100 + (date[2] - '0') * 10 + (date[3] - '0');
            int month = (date[5] - '0') * 10   + (date[6] - '0');
            int day =   (date[8] - '0') * 10   + (date[9] - '0');
            int hour =  (date[11] - '0') * 10  + (date[12] - '0');
            int min =   (date[14] - '0') * 10  + (date[15] - '0');
            int sec =   (date[17] - '0') * 10  + (date[18] - '0');
            
            DateTime time = new DateTime(year, month, day, hour, min, sec);
            entry.TimeDelta = (int)time.Subtract(BlockDB.Epoch).TotalSeconds;
        }
    }
}