/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy.SQL {
    /// <summary> Callback function invoked on a row returned from an SQL query. </summary>
    public delegate object ReaderCallback(IDataRecord record, object arg);
    
    /// <summary> Abstracts a SQL database management engine. </summary>
    public static class Database {
        public static IDatabaseBackend Backend;
        public const string DateFormat = "yyyy-MM-dd HH:mm:ss";        

        static object ReadInt(IDataRecord record, object arg) { return record.GetInt32(0); }
        /// <summary> Counts rows in the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are counted. </param>
        public static int CountRows(string table, string modifier = "", params object[] args) {
            object raw = ReadRows(table, "COUNT(*)", null, ReadInt, modifier, args);
            return raw == null ? 0 : (int)raw;
        }
        
        static object ReadString(IDataRecord record, object arg) { return record.GetText(0); }
        /// <summary> Returns value of first column in last row read from the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are read. </param>
        public static string ReadString(string table, string column,
                                        string modifier = "", params object[] args) {
            return (string)ReadRows(table, column, null, ReadString, modifier, args);
        }
        
        internal static object ReadList(IDataRecord record, object arg) {
            ((List<string>)arg).Add(record.GetText(0)); return arg;
        }
        
        internal static object ReadFields(IDataRecord record, object arg) {
            string[] field = new string[record.FieldCount];
            for (int i = 0; i < field.Length; i++) { field[i] = record.GetStringValue(i); }
            ((List<string[]>)arg).Add(field);
            return arg;
        }
        
        /// <summary> Returns all columns of all rows read from the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are read. </param>
        public static List<string[]> GetRows(string table, string columns,
                                             string modifier = "", params object[] args) {
            List<string[]> fields = new List<string[]>();
            ReadRows(table, columns, fields, ReadFields, modifier, args);
            return fields;
        }
        
        
        #region High level table management
        
        /// <summary> Returns whether a table (case sensitive) exists by that name. </summary>
        public static bool TableExists(string table) {
            ValidateName(table);
            return Backend.TableExists(table); 
        }
        
        /// <summary> Creates a new table in the database (unless it already exists). </summary>
        public static void CreateTable(string table, ColumnDesc[] columns) {
            ValidateName(table);
            string sql = Backend.CreateTableSql(table, columns);
            Execute(sql, null);
        } 
        
        /// <summary> Renames the source table to the given name. </summary>
        public static void RenameTable(string srcTable, string dstTable) {
            ValidateName(srcTable);
            ValidateName(dstTable);
            string sql = Backend.RenameTableSql(srcTable, dstTable);
            Execute(sql, null);
        }
        
        /// <summary> Completely removes the given table. </summary>
        public static void DeleteTable(string table) {
            ValidateName(table);
            string sql = Backend.DeleteTableSql(table);
            Execute(sql, null);
        }        
        
        /// <summary> Adds a new coloumn to the given table. </summary>
        /// <remarks> Note colAfter is only a hint - some database backends ignore this. </remarks>
        public static void AddColumn(string table, ColumnDesc col, string colAfter) {
            ValidateName(table);
            string sql = Backend.AddColumnSql(table, col, colAfter);
            Execute(sql, null);
        }
        #endregion
        
        
        #region High level functions
        
        /// <summary> Inserts/Copies all the rows from the source table into the destination table. </summary>
        /// <remarks> May NOT work correctly if the tables have different schema. </remarks>
        public static void CopyAllRows(string srcTable, string dstTable) {
            ValidateName(srcTable);
            ValidateName(dstTable);
            string sql = Backend.CopyAllRowsSql(srcTable, dstTable);
            Execute(sql, null);
        }
        
        /// <summary> Iterates over read rows for the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are read,
        /// return rows in a certain order, etc.</param>
        public static object ReadRows(string table, string columns, object arg,
                                      ReaderCallback callback, string modifier = "", params object[] args) {
            ValidateName(table);
            string sql = Backend.ReadRowsSql(table, columns, modifier);
            return Iterate(sql, arg, callback, args);
        }
        
        /// <summary> Updates rows for the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are updated. </param>
        public static void UpdateRows(string table, string columns,
                                       string modifier = "", params object[] args) {
            ValidateName(table);
            string sql = Backend.UpdateRowsSql(table, columns, modifier);
            Execute(sql, args);
        }
        
        /// <summary> Deletes rows for the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are deleted. </param>
        public static void DeleteRows(string table, string modifier = "", params object[] args) {
            ValidateName(table);
            string sql = Backend.DeleteRowsSql(table, modifier);
            Execute(sql, args);
        }

        /// <summary> Adds a row to the given table. </summary>
        public static void AddRow(string table, string columns, params object[] args) {
            ValidateName(table);
            string sql = Backend.AddRowSql(table, columns, args);
            Execute(sql, args);
        }
        
        /// <summary> Adds or replaces a row (same primary key) in the given table. </summary>
        public static void AddOrReplaceRow(string table, string columns, params object[] args) {
            ValidateName(table);
            string sql = Backend.AddOrReplaceRowSql(table, columns, args);
            Execute(sql, args);
        }
        
        #endregion
        
        
        #region Low level functions
        
        /// <summary> Executes an SQL command that does not return any results. </summary>
        public static void Execute(string sql, params object[] args) {
            Do(sql, false, null, null, args);
        }

        /// <summary> Executes an SQL query, invoking callback function on each returned row. </summary>
        public static object Iterate(string sql, object arg, ReaderCallback callback, params object[] args) {
            return Do(sql, false, arg, callback, args);
        }

        internal static object Do(string sql, bool createDB, object arg,
                                  ReaderCallback callback, params object[] args) {
            Exception e = null;
            for (int i = 0; i < 10; i++) {
                try {
                    if (callback != null) {
                        arg = SqlQuery.Iterate(sql, args, arg, callback);
                    } else {
                        SqlQuery.Execute(sql, args, createDB);
                    }
                    
                    return arg;
                } catch (Exception ex) {
                    e = ex; // try yet again
                }
            }

            Logger.LogError("Error executing SQL statement: " + sql, e);
            return arg;
        }
        #endregion
        
        internal static bool ValidNameChar(char c) {
            return 
                c > ' '   && c != '"' && c != '%' && c != '&'  &&
                c != '\'' && c != '*' && c != '/' && c != ':'  &&
                c != '<'  && c != '>' && c != '?' && c != '\\' &&
                c != '`'  && c != '|' && c <= '~';
        }
        
        internal static void ValidateName(string table) {
            foreach (char c in table) {
                if (ValidNameChar(c)) continue;
                throw new ArgumentException("Invalid character in table name: " + c);
            }
        }
    }
    
    internal static class DatabaseExts {
        internal static string GetText(this IDataRecord record, int col) {
            return record.IsDBNull(col) ? "" : record.GetString(col);
        }
        
        internal static string GetText(this IDataRecord record, string name) {
            int col = record.GetOrdinal(name);
            return record.IsDBNull(col) ? "" : record.GetString(col);
        }
        
        internal static int GetInt(this IDataRecord record, string name) {
            int col = record.GetOrdinal(name);
            return record.IsDBNull(col) ? 0 : record.GetInt32(col);
        }
        
        internal static long GetLong(this IDataRecord record, string name) {
            int col = record.GetOrdinal(name);
            return record.IsDBNull(col) ? 0 : record.GetInt64(col);
        }
        
        internal static string GetStringValue(this IDataRecord record, int col) {
            if (record.IsDBNull(col)) return "";
            Type type = record.GetFieldType(col);
            
            if (type == typeof(string)) return record.GetString(col);
            if (type == typeof(DateTime)) {
                return Database.Backend.RawGetDateTime(record, col);
            }
            return record.GetValue(col).ToString();
        }        
    }
}