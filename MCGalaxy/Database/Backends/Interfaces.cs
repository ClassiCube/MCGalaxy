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
using System.Collections.Generic;
using System.Data;

namespace MCGalaxy.SQL {
    public interface IDBConnection : IDisposable {
        IDBTransaction BeginTransaction();
        void ChangeDatabase(string databaseName);
        int ConnectionTimeout { get; }

        IDBCommand CreateCommand();
        string Database { get; }
        ConnectionState State { get; }
        
        void Open();
        void Close();
    }

    public interface IDBCommand : IDisposable {
        string CommandText { get; set; }
        int CommandTimeout { get; }
        //bool IsConnectionClosed { get; }
        void AddParam(object value);
        void ClearParams();
        
        IDBDataReader ExecuteReader();
        int ExecuteNonQuery();
        object ExecuteScalar();
        void Prepare();
    }

    public interface IDBDataRecord {
        int FieldCount { get; }
        byte GetByte(int i);
        int GetBytes(int i, int srcOffset, byte[] dst, int dstOffset, int length);        
        string GetDataTypeName(int i);
        DateTime GetDateTime(int i);
        
        double GetDouble(int i);
        Type GetFieldType(int i);
        float GetFloat(int i);
        string GetName(int i);
        int GetInt32(int i);
        long GetInt64(int i);
        
        int GetOrdinal(string name);
        string GetString(int i);
        object GetValue(int i);
        bool IsDBNull(int i);
    }
    
    public interface IDBDataReader : IDBDataRecord, IDisposable {
        void Close();
        bool IsClosed { get; }
        int RecordsAffected { get; }
        
        bool NextResult();
        bool Read();
    }

    public interface IDBDataParameter {
        DbType DbType { get; set; }
        string ParameterName { get; set; }        
        object Value { get; set; }
    }

    public interface IDBTransaction : IDisposable {
        void Commit();
        void Rollback();
    }
}
