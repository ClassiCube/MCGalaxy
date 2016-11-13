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
using MCGalaxy.SQL;

namespace MCGalaxy.DB {
    
    /// <summary> Exports BlockDB tables to the new binary format. </summary>
    public sealed class DBExporter {
        
        string mapName;
        Dictionary<string, int> nameCache = new Dictionary<string, int>();
        Stream stream;
        
        public void ExportTable(string table) {
            mapName = table.Substring("Block".Length);
            
            try {
                Database.ExecuteReader("SELECT * FROM `" + table + "`", DumpRow);
            } finally {
                if (stream != null) stream.Close();
                stream = null;
            }
        }
        
        void DumpRow(IDataReader reader) {
            if (stream == null) {
                stream = File.Create("blockdefs/" + mapName + ".dump");
            }
            
            string user = reader.GetString(0);
            string date = TableDumper.GetDate(reader, 1);
            int x = reader.GetInt32(2);
            int y = reader.GetInt32(3);
            int z = reader.GetInt32(4);
            byte type = reader.GetByte(5);
            byte deleted = reader.GetByte(6);
        }
    }
}