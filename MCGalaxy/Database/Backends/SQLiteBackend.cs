/********************************************************
* ADO.NET 2.0 Data Provider for SQLite Version 3.X
* Written by Robert Simpson (robert@blackcastlesoft.com)
*
* Released to the public domain, use at your own risk!
********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using SQLiteErrorCode = System.Int32;

namespace MCGalaxy.SQL {

    [SuppressUnmanagedCodeSecurity]
    internal static class Interop {
        const string lib = "sqlite3";
        
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_open_v2(byte[] utf8Filename, ref IntPtr db, int flags, IntPtr vfs);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_close_v2(IntPtr db); /* 3.7.14+ */
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_exec(IntPtr db, byte[] strSql, IntPtr pvCallback, IntPtr pvParam, ref IntPtr errMsg);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_prepare_v2(IntPtr db, byte[] strSql, int nBytes, ref IntPtr stmt, ref IntPtr ptrRemain);
        
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_busy_timeout(IntPtr db, int ms);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long sqlite3_last_insert_rowid(IntPtr db);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_changes(IntPtr db);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_get_autocommit(IntPtr db);

        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_bind_double(IntPtr stmt, int index, double value);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_bind_int(IntPtr stmt, int index, int value);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_bind_int64(IntPtr stmt, int index, long value);
        
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_bind_null(IntPtr stmt, int index);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_bind_blob(IntPtr stmt, int index, byte[] value, int nSize, IntPtr nTransient);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_bind_text(IntPtr stmt, int index, byte[] value, int nlen, IntPtr pvReserved);
        
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_bind_parameter_count(IntPtr stmt);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_bind_parameter_name(IntPtr stmt, int index);

        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double sqlite3_column_double(IntPtr stmt, int index);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_column_int(IntPtr stmt, int index);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long sqlite3_column_int64(IntPtr stmt, int index);
        
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_column_bytes(IntPtr stmt, int index);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_column_blob(IntPtr stmt, int index);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_column_text(IntPtr stmt, int index);
        
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_column_count(IntPtr stmt);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern TypeAffinity sqlite3_column_type(IntPtr stmt, int index);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_column_decltype(IntPtr stmt, int index);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_column_name(IntPtr stmt, int index);
        
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_step(IntPtr stmt);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_reset(IntPtr stmt);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_finalize(IntPtr stmt);

        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_errmsg(IntPtr db);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_errcode(IntPtr db);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SQLiteErrorCode sqlite3_extended_errcode(IntPtr db);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_errstr(SQLiteErrorCode rc); /* 3.7.15+ */
    }

    public abstract class SQLiteConnection : ISqlConnection 
    {
        public bool Closed = true;
        internal int _transactionLevel;
        IntPtr handle;
        
        protected abstract bool ConnectionPooling { get; }
        protected abstract string DBPath { get; }

        public ISqlTransaction BeginTransaction() {
            return new SQLiteTransaction(this);
        }

        public void ChangeDatabase(string databaseName) { }
        public ISqlCommand CreateCommand(string sql) { return new SQLiteCommand(sql, this); }

        public long LastInsertRowId {
            get {
                if (handle == IntPtr.Zero) throw new InvalidOperationException("Database connection closed");
                return Interop.sqlite3_last_insert_rowid(handle);
            }
        }

        public int Changes {
            get {
                if (handle == IntPtr.Zero) throw new InvalidOperationException("Database connection closed");
                return Interop.sqlite3_changes(handle);
            }
        }

        public bool AutoCommit {
            get {
                if (handle == IntPtr.Zero) throw new InvalidOperationException("Database connection closed");
                return Interop.sqlite3_get_autocommit(handle) == 1;
            }
        }
        
        public SQLiteErrorCode ResultCode() {
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Database connection closed");
            return Interop.sqlite3_errcode(handle);
        }
        
        public SQLiteErrorCode ExtendedResultCode() {
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Database connection closed");
            return Interop.sqlite3_extended_errcode(handle);
        }
        
        internal string GetLastError() {
            if (handle == IntPtr.Zero) return "database connection closed";
            return SQLiteConvert.FromUTF8(Interop.sqlite3_errmsg(handle), -1);
        }
        
        internal SQLiteStatement Prepare(string strSql, ref string strRemain) {
            byte[] b = SQLiteConvert.ToUTF8(strSql);
            uint start = (uint)Environment.TickCount;
            while (true) {
                IntPtr stmt = IntPtr.Zero, ptr = IntPtr.Zero;
                SQLiteErrorCode n = Interop.sqlite3_prepare_v2(handle, b, b.Length - 1, ref stmt, ref ptr);

                if (n == SQLiteErrorCodes.Ok) {
                    strRemain = SQLiteConvert.FromUTF8(ptr, -1);
                    if (stmt != IntPtr.Zero) return new SQLiteStatement(this, stmt);
                    return null;
                } else if (n == SQLiteErrorCodes.Locked || n == SQLiteErrorCodes.Busy) {
                    // Locked -- delay a small amount before retrying
                    SQLiteConvert.TrySleep(this, n, start);
                    if (stmt != IntPtr.Zero) Interop.sqlite3_finalize(stmt);
                } else {
                    throw new SQLiteException(n, GetLastError());
                }
            }
        }
        
        public void Open() {
            if (!Closed) throw new InvalidOperationException();
            Close();

            try {
                if (ConnectionPooling) handle = RemoveFromPool();
                
                if (handle == IntPtr.Zero) {
                    IntPtr db = IntPtr.Zero;

                    const int flags = 4 | 2; // CREATE(4) | READ_WRITE(2)
                    SQLiteErrorCode n = Interop.sqlite3_open_v2(SQLiteConvert.ToUTF8(DBPath), ref db, flags, IntPtr.Zero);
                    
                    if (n != SQLiteErrorCodes.Ok) throw new SQLiteException(n, null);
                    handle = db;
                }
                
                SetTimeout(0);
                Closed = false;
            } catch (SQLiteException) {
                Close();
                throw;
            }
        }
        
        void SetTimeout(int timeoutMS) {
            if (handle == IntPtr.Zero) throw new SQLiteException("no connection handle available");
            SQLiteErrorCode n = Interop.sqlite3_busy_timeout(handle, timeoutMS);
            if (n != SQLiteErrorCodes.Ok) throw new SQLiteException(n, GetLastError());
        }
        
        internal static void Check(SQLiteConnection connection) {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (connection.Closed)
                throw new InvalidOperationException("The connection is not open.");
            if (connection.handle == IntPtr.Zero)
                throw new InvalidOperationException("The connection handle is invalid.");
        }
        
        internal bool Reset(bool canThrow) {
            if (handle == IntPtr.Zero) return false;
            IntPtr stmt = IntPtr.Zero;

            do {
                stmt = Interop.sqlite3_next_stmt(handle, stmt);
                if (stmt != IntPtr.Zero) Interop.sqlite3_finalize(stmt);
            } while (stmt != IntPtr.Zero);

            // NOTE: Is a transaction NOT pending on the connection?
            if (AutoCommit) return true;
            
            SQLiteErrorCode n = Interop.sqlite3_exec(handle, SQLiteConvert.ToUTF8("ROLLBACK"),
                                                     IntPtr.Zero, IntPtr.Zero, ref stmt);
            if (n == SQLiteErrorCodes.Ok) return true;
            
            if (canThrow) throw new SQLiteException(n, GetLastError());
            return false;
        }
        
        public void Dispose() { Close(false); }
        public void Close() { Close(true); }
        
        void Close(bool canThrow) {
            if (handle == IntPtr.Zero) return;

            // TODO: handle leak here??
            if (ConnectionPooling) {
                if (Reset(canThrow)) AddToPool(handle);
            } else {
                LimitPool(0);
                Reset(canThrow);
                Interop.sqlite3_close_v2(handle);
            }

            handle = IntPtr.Zero;
            _transactionLevel = 0;
        }
        
        static readonly Queue<IntPtr> pool = new Queue<IntPtr>();
        const int MaxPoolSize = 300;
        static readonly object poolLocker = new object();
        
        static void LimitPool(int max) {
            lock (poolLocker) {
                while (pool.Count > max) {
                    IntPtr handle = pool.Dequeue();
                    Interop.sqlite3_close_v2(handle);
                }                
            }
        }
        
        static void AddToPool(IntPtr handle) {
            lock (poolLocker) {
                LimitPool(MaxPoolSize - 1);
                pool.Enqueue(handle);
            }
        }
        
        static IntPtr RemoveFromPool() {
            lock (poolLocker) {
                if (pool.Count > 0) return pool.Dequeue();
                return IntPtr.Zero;
            }
        }
    }

    public sealed class SQLiteCommand : ISqlCommand {
        string strCmdText, strRemaining;
        internal SQLiteConnection conn;
        SQLiteStatement stmt;
        List<string> param_names  = new List<string>();
        List<object> param_values = new List<object>();
        
        public SQLiteCommand(string sql, SQLiteConnection connection) {
            strCmdText   = sql;
            strRemaining = sql;
            conn = connection;
        }
        
        void DisposeStatement() {
            if (stmt != null) stmt.Dispose();
            stmt = null;
        }
        
        public void Dispose() {
            conn = null;
            param_names.Clear();
            param_values.Clear();
            strCmdText = null;
            strRemaining = null;
            DisposeStatement();
        }

        internal SQLiteStatement NextStatement() {
            if (stmt != null) DisposeStatement();
            if (String.IsNullOrEmpty(strRemaining)) return null;
            
            try {
                stmt = conn.Prepare(strRemaining, ref strRemaining);
            } catch (Exception) {
                DisposeStatement();
                // Cannot continue on, so set the remaining text to null.
                strRemaining = null;
                throw;
            }
            
            if (stmt != null) stmt.BindAll(param_names, param_values);
            return stmt;
        }

        public void Associate(ISqlTransaction transaction) { }
        public void Prepare() { }

        public void ClearParameters() {
            param_names.Clear();
            param_values.Clear();
        }
        
        public void AddParameter(string name, object value) {
            param_names.Add(name);
            param_values.Add(value);
        }

        public ISqlReader ExecuteReader() {
            SQLiteConnection.Check(conn);
            return new SQLiteDataReader(this);
        }

        public int ExecuteNonQuery() {
            using (ISqlReader reader = ExecuteReader()) {
                while (reader.NextResult()) { }
                return reader.RecordsAffected;
            }
        }
    }

    public sealed class SQLiteDataReader : ISqlReader {
        SQLiteCommand _command;
        SQLiteStatement stmt;
        int readState, rowsAffected, columns;
        string[] fieldNames;
        SQLiteType[] fieldTypes;

        internal SQLiteDataReader(SQLiteCommand cmd) {
            _command = cmd;
            NextResult();
        }
        
        
        void CheckClosed() {
            if (_command == null)
                throw new InvalidOperationException("DataReader has been closed");
            if (_command.conn.Closed)
                throw new InvalidOperationException("Connection was closed, statement was terminated");
        }

        public int FieldCount { get { return columns; } }

        void VerifyForGet() {
            CheckClosed();
            if (readState != 0) throw new InvalidOperationException("No current row");
        }

        public bool GetBoolean(int i) { return GetInt32(i) != 0; }

        public byte[] GetBytes(int i) {
            if (CheckAffinity(i) == TypeAffinity.Blob)
                return stmt.GetBytes(i);
            throw new InvalidCastException();
        }

        public DateTime GetDateTime(int i) {
            TypeAffinity aff = CheckAffinity(i);
            if (aff == TypeAffinity.Int64 || aff == TypeAffinity.Double || aff == TypeAffinity.Text)
                return stmt.GetDateTime(i);
            throw new NotSupportedException();
        }
        
        public double GetDouble(int i) {
            TypeAffinity aff = CheckAffinity(i);
            if (aff == TypeAffinity.Int64 || aff == TypeAffinity.Double)
                return stmt.GetDouble(i);
            throw new NotSupportedException();
        }

        public Type GetFieldType(int i) {
            SQLiteType t = GetSQLiteType(i);
            if (t.Type == DbType.Object)
                return SQLiteConvert.affinity_to_type[(int)t.Affinity];
            else
                return SQLiteConvert.dbtype_to_type[(int)t.Type];
        }
        public string GetName(int i) { return stmt.ColumnName(i); }

        public int GetInt32(int i) {
            if (CheckAffinity(i) == TypeAffinity.Int64)
                return stmt.GetInt32(i);
            throw new InvalidCastException();
        }

        public long GetInt64(int i) {
            if (CheckAffinity(i) == TypeAffinity.Int64)
                return stmt.GetInt64(i);
            throw new InvalidCastException();
        }

        public int GetOrdinal(string name) {
            VerifyForGet();
            if (fieldNames == null) fieldNames = new string[columns];

            for (int i = 0; i < columns; i++) {
                string field = fieldNames[i];
                if (field == null) {
                    field = stmt.ColumnName(i);
                    fieldNames[i] = field;
                }
                if (name.Equals(field, StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        public string GetString(int i) { return stmt.GetText(i); }
        
        public bool IsDBNull(int i) {
            VerifyForGet();
            return stmt.ColumnAffinity(i) == TypeAffinity.Null;
        }

        public object GetValue(int i) {
            VerifyForGet();
            SQLiteType t = GetSQLiteType(i);
            return stmt.GetValue(i, t);
        }
        
        public object this[string name] { get { return GetValue(GetOrdinal(name)); } }
        public object this[int i] { get { return GetValue(i); } }
        
        
        public int RecordsAffected { get { return rowsAffected; } }
        
        public void Dispose() { Close(); }
        public void Close() {
            _command = null;
            stmt = null;
            fieldNames = null;
            fieldTypes = null;
        }
        
        public bool NextResult() {
            CheckClosed();

            while (true) {
                stmt = _command.NextStatement(); // next statement to execute
                readState = 1; // set the state to "done reading"
                
                // reached the end of the statements, no more resultsets
                if (stmt == null) return false;
                
                columns = stmt.ColumnCount();
                if (stmt.Step()) {
                    readState = -1;
                } else if (columns == 0) {
                    // No rows or columns returned, move to the next statement
                    rowsAffected += stmt.conn.Changes;
                    continue;
                } else {
                    // This statement returned columns but no rows
                }

                // Found a row-returning resultset eligible to be returned!
                fieldTypes = new SQLiteType[columns];
                fieldNames = null;
                return true;
            }
        }

        TypeAffinity CheckAffinity(int i) {
            VerifyForGet();
            return GetSQLiteType(i).Affinity;
        }
        
        SQLiteType GetSQLiteType(int i) {
            SQLiteType typ = fieldTypes[i];
            TypeAffinity affinity = stmt.ColumnAffinity(i);
            // NOTE: affinity of a column can change (e.g. NULL when null string, STRING for when has value)
            if (affinity == typ.Affinity) return typ;

            // Fetch the declared column datatype and attempt to convert it to a known DbType.
            typ.Affinity = affinity;
            string typeName = stmt.ColumnType(i);
            typ.Type = SQLiteConvert.TypeNameToDbType(typeName);
            
            fieldTypes[i] = typ;
            return typ;
        }

        public bool Read() {
            CheckClosed();

            // First Row was already read at NextResult() level, so don't step again here
            if (readState == -1) {
                readState = 0; return true;
            } else if (readState == 0) { // Actively reading rows
                if (stmt.Step()) return true;
                readState = 1; // Finished reading rows
            }
            return false;
        }
    }

    public sealed class SQLiteTransaction : ISqlTransaction {
        SQLiteConnection conn;
        
        internal SQLiteTransaction(SQLiteConnection connection) {
            conn = connection;
            if (conn._transactionLevel++ == 0) {
                try {
                    using (ISqlCommand cmd = conn.CreateCommand("BEGIN IMMEDIATE")) {
                        cmd.ExecuteNonQuery();
                    }
                } catch (SQLiteException) {
                    conn._transactionLevel--;
                    conn = null;
                    throw;
                }
            }
        }
        
        bool disposed;
        public void Dispose() {
            if (disposed) return;
            if (IsValid(false)) IssueRollback(false);
            disposed = true;
        }
        
        public void Commit() {
            SQLiteConnection.Check(conn);
            IsValid(true);

            if (--conn._transactionLevel == 0) {
                using (ISqlCommand cmd = conn.CreateCommand("COMMIT")) {
                    cmd.ExecuteNonQuery();
                }
            }
            conn = null;
        }

        public void Rollback() {
            SQLiteConnection.Check(conn);
            IsValid(true);
            IssueRollback(true);
        }

        void IssueRollback(bool throwError) {
            if (conn == null) return;
            
            try {
                using (ISqlCommand cmd = conn.CreateCommand("ROLLBACK")) {
                    cmd.ExecuteNonQuery();
                }
            } catch {
                if (throwError) throw;
            }
            conn._transactionLevel = 0;
        }

        bool IsValid(bool throwError) {
            if (conn == null) {
                if (throwError) throw new ArgumentNullException("No connection associated with this transaction");
                return false;
            }

            if (conn.Closed) {
                if (throwError) throw new SQLiteException("Connection was closed");
                return false;
            }

            if (conn._transactionLevel == 0 || conn.AutoCommit) {
                conn._transactionLevel = 0; // Make sure the transaction level is reset before returning
                if (throwError) throw new SQLiteException("No transaction is active on this connection");
                return false;
            }
            return true;
        }
    }
    
    static class SQLiteConvert {
        static string[] _datetimeFormats = new string[] {
            "yyyy-MM-dd HH:mm:ss.FFFFFFFK", /* NOTE: UTC default (0). */
            "yyyy-MM-dd HH:mm:ssK",
            "yyyy-MM-dd HH:mmK",
            
            "yyyy-MM-dd HH:mm:ss.FFFFFFF", /* NOTE: Non-UTC default (3). */
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm",
        };

        static readonly string _datetimeFormatUtc = _datetimeFormats[0];
        static readonly string _datetimeFormatLocal = _datetimeFormats[3];
        static Encoding utf8 = new UTF8Encoding();

        public static byte[] ToUTF8(string text) {
            int count = utf8.GetByteCount(text) + 1;
            byte[] data = new byte[count];
            
            count = utf8.GetBytes(text, 0, text.Length, data, 0);
            data[count] = 0;

            return data;
        }

        public static string FromUTF8(IntPtr ptr, int len) {
            if (ptr == IntPtr.Zero) return "";
            
            if (len < 0) {
                len = 0;
                while (Marshal.ReadByte(ptr, len) != 0) { len++; }
            }
            if (len == 0) return "";
            
            byte[] data = new byte[len];
            Marshal.Copy(ptr, data, 0, len);
            return utf8.GetString(data, 0, len);
        }
        
        public static DateTime ToDateTime(string text) {
            return DateTime.SpecifyKind(DateTime.ParseExact(
                text, _datetimeFormats, DateTimeFormatInfo.InvariantInfo,
                DateTimeStyles.None), DateTimeKind.Unspecified);
        }

        public static string ToString(DateTime value) {
            string format = (value.Kind == DateTimeKind.Utc) ? _datetimeFormatUtc : _datetimeFormatLocal;
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
        
        
        public const int Timeout = 30;
        static uint seed = 123456789;
        
        internal static void TrySleep(SQLiteConnection conn, SQLiteErrorCode n, uint start) {
            if ((uint)Environment.TickCount > start + (Timeout * 1000)) {
                throw new SQLiteException(n, conn.GetLastError());
            } else {
                seed = seed * 1103515245 + 12345;
                Thread.Sleep((int)(seed % 150) + 1);
            }
        }
        

        internal static Type[] affinity_to_type = {
            typeof(object),   // Uninitialized (0)
            typeof(Int64),    // Int64 (1)
            typeof(Double),   // Double (2)
            typeof(string),   // Text (3)
            typeof(byte[]),   // Blob (4)
            typeof(object),   // Null (5)
        };

        internal static DbType TypeToDbType(Type typ) {
            TypeCode tc = Type.GetTypeCode(typ);
            if (tc == TypeCode.Object) {
                if (typ == typeof(byte[])) return DbType.Binary;
                return DbType.String;
            }
            return type_to_dbtype[(int)tc];
        }

        static DbType[] type_to_dbtype = {
            DbType.Object,   // Empty (0)
            DbType.Binary,   // Object (1)
            DbType.Object,   // DBNull (2)
            DbType.Boolean,  // Boolean (3)
            DbType.SByte,    // Char (4)
            DbType.SByte,    // SByte (5)
            DbType.Byte,     // Byte (6)
            DbType.Int16,    // Int16 (7)
            DbType.UInt16,   // UInt16 (8)
            DbType.Int32,    // Int32 (9)
            DbType.UInt32,   // UInt32 (10)
            DbType.Int64,    // Int64 (11)
            DbType.UInt64,   // UInt64 (12)
            DbType.Single,   // Single (13)
            DbType.Double,   // Double (14)
            DbType.Decimal,  // Decimal (15)
            DbType.DateTime, // DateTime (16)
            DbType.Object,   // ?? (17)
            DbType.String    // String (18)
        };

        internal static Type[] dbtype_to_type = {
            typeof(string),   // AnsiString (0)
            typeof(byte[]),   // Binary (1)
            typeof(byte),     // Byte (2)
            typeof(bool),     // Boolean (3)
            typeof(decimal),  // Currency (4)
            typeof(DateTime), // Date (5)
            typeof(DateTime), // DateTime (6)
            typeof(decimal),  // Decimal (7)
            typeof(double),   // Double (8)
            typeof(Guid),     // Guid (9)
            typeof(Int16),    // Int16 (10)
            typeof(Int32),    // Int32 (11)
            typeof(Int64),    // Int64 (12)
            typeof(object),   // Object (13)
            typeof(sbyte),    // SByte (14)
            typeof(float),    // Single (15)
            typeof(string),   // String (16)
            typeof(DateTime), // Time (17)
            typeof(UInt16),   // UInt16 (18)
            typeof(UInt32),   // UInt32 (19)
            typeof(UInt64),   // UInt64 (20)
        };
        
        static bool TryParseDbType(string typeName, out DbType type) {
            string[] names = all_names;
            for (int i = 0; i < names.Length; i++) {
                if (!typeName.Equals(names[i], StringComparison.OrdinalIgnoreCase)) continue;
                type = all_types[i]; return true;
            }
            type = 0; return false;
        }
        
        internal static DbType TypeNameToDbType(string typeName) {
            if (typeName == null) return DbType.Object;

            DbType value;
            if (TryParseDbType(typeName, out value)) return value;
            
            int i = typeName.IndexOf('(');
            if (i > 0 && TryParseDbType(typeName.Substring(0, i).TrimEnd(), out value)) return value;
            
            return DbType.Object;
        }
        
        static string[] all_names = new string[] {
            "BIGINT", "BIGUINT", "BINARY", "BLOB",
            "BOOL", "BOOLEAN", "CHAR", "DATE",
            "DATETIME", "DOUBLE", "FLOAT", "IDENTITY",
            "INT", "INT8", "INT16", "INT32",
            "INT64", "INTEGER", "INTEGER8", "INTEGER16",
            "INTEGER32", "INTEGER64", "LONG", "MEDIUMINT",
            "REAL", "SINGLE", "SMALLINT", "SMALLUINT",
            "STRING", "TEXT", "TIME", "TINYINT",
            "TINYSINT", "UINT", "UINT8", "UINT16",
            "UINT32", "UINT64", "ULONG", "UNSIGNEDINTEGER",
            "UNSIGNEDINTEGER8", "UNSIGNEDINTEGER16", "UNSIGNEDINTEGER32", "UNSIGNEDINTEGER64",
            "VARCHAR",
        };
        
        static DbType[] all_types = new DbType[] {
            DbType.Int64, DbType.UInt64, DbType.Binary, DbType.Binary,
            DbType.Boolean, DbType.Boolean, DbType.String, DbType.DateTime,
            DbType.DateTime, DbType.Double, DbType.Double, DbType.Int64,
            DbType.Int32, DbType.SByte, DbType.Int16, DbType.Int32,
            DbType.Int64, DbType.Int64, DbType.SByte, DbType.Int16,
            DbType.Int32, DbType.Int64, DbType.Int64, DbType.Int32,
            DbType.Double, DbType.Single, DbType.Int16, DbType.UInt16,
            DbType.String, DbType.String, DbType.DateTime, DbType.Byte,
            DbType.SByte, DbType.UInt32, DbType.Byte, DbType.UInt16,
            DbType.UInt32, DbType.UInt64, DbType.UInt64, DbType.UInt64,
            DbType.Byte, DbType.UInt16, DbType.UInt32, DbType.UInt64,
            DbType.String,
        };
    }

    enum TypeAffinity {
        Uninitialized = 0,
        Int64 = 1,
        Double = 2,
        Text = 3,
        Blob = 4,
        Null = 5,
        DateTime = 10,
    }
    
    enum DbType {
        DateTime, Boolean,
        String, Object, Binary,
        Single, Double, Decimal,
        Byte, Int16, Int32, Int64,
        SByte, UInt16, UInt32, UInt64,
    }

    struct SQLiteType {
        public DbType Type;
        public TypeAffinity Affinity;
    }

    sealed class SQLiteException : ExternalException {
        SQLiteErrorCode _code;

        public SQLiteException(SQLiteErrorCode code, string message)
            : base(FormatError(code, message)) { _code = code; }

        public SQLiteException(string message) : this(SQLiteErrorCodes.Unknown, message) { }
        public override int ErrorCode { get { return (int)_code; } }
        
        static string FormatError(SQLiteErrorCode code, string message) {
            string msg = GetErrorString(code) + Environment.NewLine + message;
            return msg.Trim();
        }
        
        static string[] errors = new string[] {
            /* SQLITE_OK          */ "not an error",
            /* SQLITE_ERROR       */ "SQL logic error or missing database",
            /* SQLITE_INTERNAL    */ "internal logic error",
            /* SQLITE_PERM        */ "access permission denied",
            /* SQLITE_ABORT       */ "callback requested query abort",
            /* SQLITE_BUSY        */ "database is locked",
            /* SQLITE_LOCKED      */ "database table is locked",
            /* SQLITE_NOMEM       */ "out of memory",
            /* SQLITE_READONLY    */ "attempt to write a readonly database",
            /* SQLITE_INTERRUPT   */ "interrupted",
            /* SQLITE_IOERR       */ "disk I/O error",
            /* SQLITE_CORRUPT     */ "database disk image is malformed",
            /* SQLITE_NOTFOUND    */ "unknown operation",
            /* SQLITE_FULL        */ "database or disk is full",
            /* SQLITE_CANTOPEN    */ "unable to open database file",
            /* SQLITE_PROTOCOL    */ "locking protocol",
            /* SQLITE_EMPTY       */ "table contains no data",
            /* SQLITE_SCHEMA      */ "database schema has changed",
            /* SQLITE_TOOBIG      */ "string or blob too big",
            /* SQLITE_CONSTRAINT  */ "constraint failed",
            /* SQLITE_MISMATCH    */ "datatype mismatch",
            /* SQLITE_MISUSE      */ "library routine called out of sequence",
            /* SQLITE_NOLFS       */ "large file support is disabled",
            /* SQLITE_AUTH        */ "authorization denied",
            /* SQLITE_FORMAT      */ "auxiliary database format error",
            /* SQLITE_RANGE       */ "bind or column index out of range",
            /* SQLITE_NOTADB      */ "file is encrypted or is not a database",
            /* SQLITE_NOTICE      */ "notification message",
            /* SQLITE_WARNING     */ "warning message"
        };

        internal static string GetErrorString(SQLiteErrorCode rc) {
            try {
                IntPtr ptr = Interop.sqlite3_errstr(rc);
                if (ptr != IntPtr.Zero) {
                    return Marshal.PtrToStringAnsi(ptr);
                }
            } catch (EntryPointNotFoundException) {
                // do nothing.
            }
            
            if (rc < 0 || rc >= errors.Length)
                rc = SQLiteErrorCodes.Error;
            return errors[rc];
        }
    }

    static class SQLiteErrorCodes {
        public const int Unknown = -1;
        public const int Ok = 0;
        public const int Error = 1;
        public const int Busy = 5;
        public const int Locked = 6;
        public const int Row = 100;
        public const int Done = 101;
    }

    sealed class SQLiteStatement : IDisposable {
        IntPtr handle;
        internal SQLiteConnection conn;
        string[] paramNames;

        internal SQLiteStatement(SQLiteConnection connection, IntPtr handle) {
            conn = connection;
            this.handle = handle;

            // Determine parameters for this statement (if any) and prepare space for them.
            int count = Interop.sqlite3_bind_parameter_count(handle);
            if (count <= 0) return;

            paramNames = new string[count];
            for (int i = 0; i < count; i++) {
                IntPtr p = Interop.sqlite3_bind_parameter_name(handle, i + 1);
                paramNames[i] = SQLiteConvert.FromUTF8(p, -1);
            }
        }
        
        public void Dispose() {
            if (handle != IntPtr.Zero) {
                Interop.sqlite3_finalize(handle);
                handle = IntPtr.Zero;
            }
            
            paramNames = null;
            conn = null;
        }
        
        internal bool Step() {
            uint start = (uint)Environment.TickCount;
            while (true) {
                SQLiteErrorCode n = Interop.sqlite3_step(handle);
                if (n == SQLiteErrorCodes.Row)  return true;
                if (n == SQLiteErrorCodes.Done) return false;
                if (n == SQLiteErrorCodes.Ok)   continue;
                
                // An error occurred, attempt to reset the statement. If it errored because
                // the database is locked, then keep retrying until the command timeout occurs.
                n = Interop.sqlite3_reset(handle);
                if (n == SQLiteErrorCodes.Locked || n == SQLiteErrorCodes.Busy) {
                    SQLiteConvert.TrySleep(conn, n, start);
                } else if (n != SQLiteErrorCodes.Ok) {
                    throw new SQLiteException(n, conn.GetLastError());
                }
            }
        }
        
        internal int ColumnCount() { return Interop.sqlite3_column_count(handle); }

        internal string ColumnName(int index) {
            IntPtr p = Interop.sqlite3_column_name(handle, index);
            return SQLiteConvert.FromUTF8(p, -1);
        }

        internal TypeAffinity ColumnAffinity(int index) {
            return Interop.sqlite3_column_type(handle, index);
        }

        internal string ColumnType(int index) {
            IntPtr p = Interop.sqlite3_column_decltype(handle, index);
            return SQLiteConvert.FromUTF8(p, -1);
        }

        internal void BindAll(List<string> names, List<object> values) {
            if (paramNames == null || names.Count == 0) return;
            
            for (int idx = 0; idx < names.Count; idx++) {
                int i = FindParameter(names[idx]);
                if (i == -1) continue;
                
                SQLiteErrorCode n = BindParameter(i + 1, values[idx]);
                if (n != SQLiteErrorCodes.Ok) throw new SQLiteException(n, conn.GetLastError());
            }
        }
        
        int FindParameter(string name) {
            int count = paramNames.Length;
            for (int i = 0; i < count; i++) {
                if (name.Equals(paramNames[i], StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        SQLiteErrorCode BindParameter(int i, object obj) {
            DbType type = DbType.String;
            if (obj != null)
                type = SQLiteConvert.TypeToDbType(obj.GetType());
            
            if (obj == null || obj == DBNull.Value) {
                return Interop.sqlite3_bind_null(handle, i);
            }

            switch (type) {
                case DbType.DateTime:
                    //
                    // NOTE: The old method (commented below) does not honor the selected date format
                    //       for the connection.
                    // _sql.Bind_DateTime(this, index, Convert.ToDateTime(obj, cultureInfo));
                    return Bind_DateTime(i, (obj is string) ?
                                         SQLiteConvert.ToDateTime((string)obj) : 
                                         Convert.ToDateTime(obj, CultureInfo.InvariantCulture));
                case DbType.Boolean:
                    return Bind_Int32(i, Convert.ToBoolean(obj) ? 1 : 0);
                case DbType.SByte:
                    return Bind_Int32(i, Convert.ToSByte(obj));
                case DbType.Int16:
                    return Bind_Int32(i, Convert.ToInt16(obj));
                case DbType.Int32:
                    return Bind_Int32(i, Convert.ToInt32(obj));
                case DbType.Int64:
                    return Bind_Int64(i, Convert.ToInt64(obj));
                case DbType.Byte:
                    return Bind_Int32(i, Convert.ToByte(obj));
                case DbType.UInt16:
                    return Bind_Int32(i, Convert.ToUInt16(obj));
                case DbType.UInt32:
                    return Bind_Int32(i, (int)Convert.ToUInt32(obj));
                case DbType.UInt64:
                    return Bind_Int64(i, (long)Convert.ToUInt64(obj));
                case DbType.Single:
                case DbType.Double:
                case DbType.Decimal:
                    return Interop.sqlite3_bind_double(handle, i, Convert.ToDouble(obj));
                case DbType.Binary:
                    byte[] b = (byte[])obj;
                    return Interop.sqlite3_bind_blob(handle, i, b, b.Length, (IntPtr)(-1));
                default:
                    return Bind_Text(i, obj.ToString());
            }
        }

        SQLiteErrorCode Bind_Int32(int index, int value) {
            return Interop.sqlite3_bind_int(handle, index, value);
        }

        SQLiteErrorCode Bind_Int64(int index, long value) {
            return Interop.sqlite3_bind_int64(handle, index, value);
        }

        SQLiteErrorCode Bind_Text(int index, string value) {
            byte[] b = SQLiteConvert.ToUTF8(value);
            return Interop.sqlite3_bind_text(handle, index, b, b.Length - 1, (IntPtr)(-1));
        }

        SQLiteErrorCode Bind_DateTime(int index, DateTime dt) {
            return Bind_Text(index, SQLiteConvert.ToString(dt));
        }
        
        internal object GetValue(int index, SQLiteType typ) {
            if (typ.Type == DbType.DateTime) return GetDateTime(index);

            switch (typ.Affinity) {
                case TypeAffinity.Blob:
                    return GetBytes(index);
                case TypeAffinity.Double:
                    return GetDouble(index);
                case TypeAffinity.Int64:
                    return GetInt64(index);
                case TypeAffinity.Null:
                    return DBNull.Value;
            }
            return GetText(index);
        }
        
        internal double GetDouble(int index) {
            return Interop.sqlite3_column_double(handle, index);
        }
        
        internal int GetInt32(int index) {
            return Interop.sqlite3_column_int(handle, index);
        }

        internal long GetInt64(int index) {
            return Interop.sqlite3_column_int64(handle, index);
        }

        internal string GetText(int index) {
            return SQLiteConvert.FromUTF8(Interop.sqlite3_column_text(handle, index),
                                          Interop.sqlite3_column_bytes(handle, index));
        }

        internal DateTime GetDateTime(int index) {
            return SQLiteConvert.ToDateTime(GetText(index));
        }

        internal byte[] GetBytes(int index) {
            int len = Interop.sqlite3_column_bytes(handle, index);
            if (len <= 0) return new byte[0];
            
            byte[] dst = new byte[len];
            IntPtr src = Interop.sqlite3_column_blob(handle, index);
            Marshal.Copy(src, dst, 0, len);
            return dst;
        }
    }
}
