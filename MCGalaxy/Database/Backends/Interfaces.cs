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
    public abstract class ISqlConnection : IDisposable
    {
        public abstract ISqlTransaction BeginTransaction();
        public abstract ISqlCommand CreateCommand(string sql);

        public abstract void Open();
        public abstract void ChangeDatabase(string name);
        public abstract void Close();
        public abstract void Dispose();
    }

    /// <summary> Abstracts a SQL command/statement </summary>
    public abstract class ISqlCommand : IDisposable
    {
        public abstract void ClearParameters();
        public abstract void AddParameter(string name, object value);

        public abstract void Prepare();
        /// <summary> Executes this command and returns the number of rows affected </summary>
        public abstract int ExecuteNonQuery();
        /// <summary> Executes this command and returns an ISqlReader for reading the results </summary>
        public abstract ISqlReader ExecuteReader();
        public abstract void Dispose();
    }

    public abstract class ISqlTransaction : IDisposable
    {
        public abstract void Commit();
        public abstract void Rollback();
        public abstract void Dispose();
    }

    /// <summary> Abstracts iterating over the results from executing a SQL command </summary>
    public abstract class ISqlReader : ISqlRecord, IDisposable
    {
        public abstract int RowsAffected { get; }
        public abstract void Close();
        public abstract bool Read();
        public abstract void Dispose();
    }

    public abstract class ISqlRecord
    {
        public abstract int FieldCount { get; }
        public abstract string GetName(int i);
        public abstract int GetOrdinal(string name);

        public abstract byte[] GetBytes(int i);
        public abstract bool GetBoolean(int i);
        public abstract int GetInt32(int i);
        public abstract long GetInt64(int i);
        public abstract double GetDouble(int i);
        public abstract string GetString(int i);
        public abstract DateTime GetDateTime(int i);
        public abstract bool IsDBNull(int i);

        public abstract object GetValue(int i);
        public abstract string GetStringValue(int col);
        public abstract string DumpValue(int col);


        public string GetText(int col) {
            return IsDBNull(col) ? "" : GetString(col);
        }

        public string GetText(string name) {
            int col = GetOrdinal(name);
            return IsDBNull(col) ? "" : GetString(col);
        }

        public int GetInt(string name) {
            int col = GetOrdinal(name);
            return IsDBNull(col) ? 0 : GetInt32(col);
        }

        public long GetLong(string name) {
            int col = GetOrdinal(name);
            return IsDBNull(col) ? 0 : GetInt64(col);
        }

        protected static string Quote(string value) {
            if (value.IndexOf('\'') >= 0) // escape '
                value = value.Replace("'", "''");
            return "'" + value + "'";
        }
    }
}