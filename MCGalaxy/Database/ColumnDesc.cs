/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy.SQL {

    /// <summary> Describes a column for an SQL create statement. </summary>
    public struct ColumnDesc {
        public readonly string Column;
        public readonly ColumnType Type;
        public readonly ushort MaxLength;
        
        public readonly bool AutoIncrement;
        public readonly bool PrimaryKey;
        public readonly bool NotNull;
        
        public ColumnDesc(string col, ColumnType type)
            : this(col, type, 0, false, false, false) { }        
        public ColumnDesc(string col, ColumnType type, ushort maxLen)
            : this(col, type, maxLen, false, false, false) { }
        
        public ColumnDesc(string col, ColumnType type, ushort maxLen = 0,
                            bool autoInc = false, bool priKey = false, bool notNull = false) {
            Column = col;
            Type = type;
            MaxLength = maxLen;
            AutoIncrement = autoInc;
            PrimaryKey = priKey;
            NotNull = notNull;
        }
        
        public string FormatType() {
            if (Type == ColumnType.Char) return "CHAR(" + MaxLength + ")";
            if (Type == ColumnType.VarChar) return "VARCHAR(" + MaxLength + ")";
            return colTypes[(int)Type];
        }
        
        static string[] colTypes = new string[] {
            "TINYINT UNSIGNED", "SMALLINT UNSIGNED", "MEDIUMINT UNSIGNED", 
            "INT UNSIGNED", "BIGINT UNSIGNED", "TINYINT", "SMALLINT", 
            "MEDIUMINT", "INT", "BIGINT", "INTEGER", "DATETIME",
        };
    }
    
    public enum ColumnType {
        UInt8, UInt16, UInt24, UInt32, UInt64,
        Int8, Int16, Int24, Int32, Int64,
        Integer, DateTime, Char, VarChar
    }
}
