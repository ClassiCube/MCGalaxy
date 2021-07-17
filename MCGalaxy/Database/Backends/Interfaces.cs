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

namespace MCGalaxy.SQL
{
    /// <summary> Abstracts a connection to a SQL database </summary>
    public interface ISqlConnection : IDisposable
    {
        ISqlTransaction BeginTransaction();
        ISqlCommand CreateCommand(string sql);
        
        void Open();
        void ChangeDatabase(string name);
        void Close();
    }

    /// <summary> Abstracts a SQL command/statement </summary>
    public interface ISqlCommand : IDisposable
    {
        void ClearParameters();
        void AddParameter(string name, object value);
        
        void Prepare();
        /// <summary> Executes this command and returns the number of rows affected </summary>
        int ExecuteNonQuery();
        /// <summary> Executes this command and returns an ISqlReader for reading the results </summary>
        ISqlReader ExecuteReader();
    }

    public interface ISqlTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
    
    /// <summary> Abstracts iterating over the results from executing a SQL command </summary>
    public interface ISqlReader : IDisposable, ISqlRecord
    {
        int RowsAffected { get; }
        void Close();
        bool Read();
    }

    public interface ISqlRecord
    {
        int FieldCount { get; }
        string GetName(int i);
        Type GetFieldType(int i);
        int GetOrdinal(string name);
        
        object this[int i] { get; }
        object this[string name] { get; }
        object GetValue(int i);
        
        bool GetBoolean(int i);
        byte[] GetBytes(int i);
        int GetInt32(int i);
        long GetInt64(int i);
        double GetDouble(int i);
        string GetString(int i);
        DateTime GetDateTime(int i);
        bool IsDBNull(int i);
    }
}
