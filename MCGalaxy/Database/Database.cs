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
    
    public static class Database {
        public static IDatabaseBackend Backend;
        public static bool TableExists(string table) { return Backend.TableExists(table); }
        public const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        
        static object ReadInt(IDataRecord record, object arg) { return record.GetInt32(0); }
        public static int CountRows(string table, string modifier = "", params object[] args) {
            return (int)Backend.ReadRows(table, "COUNT(*)", 0, ReadInt, modifier, args);
        }
        
        static object ReadString(IDataRecord record, object arg) { return record.GetText(0); }
        public static string ReadString(string table, string column,
                                        string modifier = "", params object[] args) {
            return (string)Backend.ReadRows(table, column, null, ReadString, modifier, args);
        }
        
        internal static object ReadList(IDataRecord record, object arg) {
            ((List<string>)arg).Add(record.GetText(0)); return arg;
        }
        internal static List<string> GetStrings(string sql, params object[] args) {
            List<string> values = new List<string>();
            Iterate(sql, values, ReadList, args);
            return values;
        }
        
        internal static object ReadFields(IDataRecord record, object arg) {
            string[] field = new string[record.FieldCount];
            for (int i = 0; i < field.Length; i++) { field[i] = record.GetStringValue(i); }
            ((List<string[]>)arg).Add(field);
            return arg;
        }
        
        public static List<string[]> GetRows(string table, string columns,
                                             string modifier = "", params object[] args) {
            List<string[]> fields = new List<string[]>();
            Backend.ReadRows(table, columns, fields, ReadFields, modifier, args);
            return fields;
        }
        
        
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
            
            try {
                File.AppendAllText("MySQL_error.log", DateTime.Now + " " + sql + "\r\n");
            } catch { }
            Logger.LogError(e);
            return arg;
        }

        
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