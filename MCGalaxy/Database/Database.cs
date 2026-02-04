/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy.SQL 
{
    /// <summary> Callback function invoked on a row returned from an SQL query. </summary>
    public delegate void ReaderCallback(ISqlRecord record);
    
    /// <summary> Abstracts a SQL database management engine. </summary>
    public static class Database 
    {
        public static IDatabaseBackend Backend;
        public const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary> Counts rows in the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are counted. </param>
        public static int CountRows(string table, string modifier = "", params object[] args) {
            int value = 0;
            ReadRows(table, "COUNT(*)", 
                    record => value = record.GetInt32(0), 
                    modifier, args);
            return value;
        }
        
        /// <summary> Returns value of first column in last row read from the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are read. </param>
        public static string ReadString(string table, string column,
                                        string modifier = "", params object[] args) {
            string value = null;
            ReadRows(table, column, 
                    record => value = record.GetText(0), 
                    modifier, args);
            return value;
        }
        
        internal static string[] ParseFields(ISqlRecord record) {
            string[] field = new string[record.FieldCount];
            for (int i = 0; i < field.Length; i++) { field[i] = record.GetStringValue(i); }
            return field;
        }
        
        /// <summary> Returns all columns of all rows read from the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are read. </param>
        public static List<string[]> GetRows(string table, string columns,
                                             string modifier = "", params object[] args) {
            List<string[]> fields = new List<string[]>();
            ReadRows(table, columns, 
                    record => fields.Add(ParseFields(record)),
                    modifier, args);
            return fields;
        }
        
        
        #region High level table management
        
        /// <summary> Returns whether a table (case sensitive) exists by that name </summary>
        public static bool TableExists(string table) {
            SqlUtils.ValidateName(table);
            return Backend.TableExists(table); 
        }
        
        /// <summary> Creates a new table in the database. </summary>
        /// <remarks> Does nothing if a table with the same name already exists. </remarks>
        public static void CreateTable(string table, ColumnDesc[] columns) {
            SqlUtils.ValidateName(table);
            string sql = Backend.CreateTableSql(table, columns);
            Execute(sql, null);
        } 
        
        /// <summary> Renames the source table to the given name. </summary>
        public static void RenameTable(string srcTable, string dstTable) {
            SqlUtils.ValidateName(srcTable);
            SqlUtils.ValidateName(dstTable);
            string sql = Backend.RenameTableSql(srcTable, dstTable);
            Execute(sql, null);
        }
        
        /// <summary> Completely removes the given table. </summary>
        /// <remarks> Does nothing if no table with the given name exists. </remarks>
        public static void DeleteTable(string table) {
            SqlUtils.ValidateName(table);
            string sql = Backend.DeleteTableSql(table);
            Execute(sql, null);
        }        
        
        /// <summary> Adds a new coloumn to the given table. </summary>
        /// <remarks> Note colAfter is only a hint - some database backends ignore this. </remarks>
        public static void AddColumn(string table, ColumnDesc col, string colAfter) {
            SqlUtils.ValidateName(table);
            string sql = Backend.AddColumnSql(table, col, colAfter);
            Execute(sql, null);
        }
        #endregion
        
        
        #region High level functions
        
        /// <summary> Inserts/Copies all the rows from the source table into the destination table. </summary>
        /// <remarks> May NOT work correctly if the tables have different schema. </remarks>
        /// <returns> The number of rows copied </returns>
        public static int CopyAllRows(string srcTable, string dstTable) {
            SqlUtils.ValidateName(srcTable);
            SqlUtils.ValidateName(dstTable);
            
            string sql = Backend.CopyAllRowsSql(srcTable, dstTable);
            return Execute(sql, null);
        }
        
        /// <summary> Iterates over read rows for the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are read,
        /// return rows in a certain order, etc.</param>
        public static void ReadRows(string table, string columns,
                                    ReaderCallback callback, string modifier = "", params object[] args) {
            SqlUtils.ValidateName(table);
            
            string sql = Backend.ReadRowsSql(table, columns, modifier);
            Iterate(sql, callback, args);
        }
        
        /// <summary> Updates rows for the given table </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are updated. Can be just "" </param>
        /// <returns> The number of rows updated </returns>
        public static int UpdateRows(string table, string columns,
                                     string modifier, params object[] args) {
            SqlUtils.ValidateName(table);
            string sql = Backend.UpdateRowsSql(table, columns, modifier);
            return Execute(sql, args);
        }
        
        /// <summary> Deletes rows for the given table. </summary>
        /// <param name="modifier"> Optional SQL to filter which rows are deleted. Can be just "" </param>
        /// <returns> The number of rows deleted </returns>
        public static int DeleteRows(string table, string modifier, params object[] args) {
            SqlUtils.ValidateName(table);
            
            string sql = Backend.DeleteRowsSql(table, modifier);
            return Execute(sql, args);
        }

        /// <summary> Adds a row to the given table. </summary>
        public static void AddRow(string table, string columns, params object[] args) {
            SqlUtils.ValidateName(table);
            
            string sql = Backend.AddRowSql(table, columns, args.Length);
            Execute(sql, args);
        }
        
        /// <summary> Adds or replaces a row (same primary key) in the given table. </summary>
        public static void AddOrReplaceRow(string table, string columns, params object[] args) {
            SqlUtils.ValidateName(table);
            
            string sql = Backend.AddOrReplaceRowSql(table, columns, args.Length);
            Execute(sql, args);
        }
        
        #endregion
        
        
        #region Low level functions
        
        /// <summary> Executes an SQL command that does not return any results. </summary>
        public static int Execute(string sql, params object[] args) {
            return Do(sql, false, null, args);
        }

        /// <summary> Executes an SQL query, invoking callback function on each returned row. </summary>
        public static int Iterate(string sql, ReaderCallback callback, params object[] args) {
            return Do(sql, false, callback, args);
        }

        internal static int Do(string sql, bool createDB, ReaderCallback callback, object[] args) {
            IDatabaseBackend db = Backend;
            Exception e = null;
            
            for (int i = 0; i < 5; i++) {
                try {
                    if (callback != null) {
                        return db.Iterate(sql, args, callback);
                    } else {
                        return db.Execute(sql, args, createDB);
                    }
                } catch (Exception ex) {
                    e = ex; // try yet again
                }
            }

            Logger.LogError("Error executing SQL statement: " + sql, e);
            return 0;
        }
        #endregion


        public static void UpdateActiveBackend() {
#if MCG_STANDALONE
            Backend = SQLiteBackend.Instance;
#else
            Backend = Server.Config.UseMySQL ? MySQLBackend.Instance : SQLiteBackend.Instance;
#endif
        }
        
        
        internal static TimeSpan ParseOldDBTimeSpent(string value) {
            string[] parts = value.SplitSpaces();
            
            int days  = NumberUtils.ParseInt32(parts[0]);
            int hours = NumberUtils.ParseInt32(parts[1]);
            int mins  = NumberUtils.ParseInt32(parts[2]);
            int secs  = NumberUtils.ParseInt32(parts[3]);
            
            return new TimeSpan(days, hours, mins, secs);
        }
        
        public static DateTime ParseDBDate(string value) {
            DateTime dt;
            // prefer the exact format
            if (value.TryParseInvariantDateString(out dt)) return dt;
            
            return DateTime.Parse(value);
        }
    }
}