using System;

namespace MCGalaxy.SQL
{
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

	public interface ISqlReader : IDisposable, ISqlRecord
	{
		int RecordsAffected { get; }
		void Close();
		bool NextResult();
		bool Read();
	}

	public interface ISqlCommand : IDisposable
	{
		void Associate(ISqlTransaction transaction);
		void ClearParameters();
		void AddParameter(string name, object value);
		
		void Prepare();
		int ExecuteNonQuery();
		ISqlReader ExecuteReader();
	}

	public interface ISqlTransaction : IDisposable
	{
		void Commit();
		void Rollback();
	}

	public interface ISqlConnection : IDisposable
	{
		ISqlTransaction BeginTransaction();
		ISqlCommand CreateCommand(string sql);
		
		void Open();
		void ChangeDatabase(string name);
		void Close();
	}
}
