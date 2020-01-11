// Copyright © 2004, 2018 Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using MCGalaxy.SQL;

namespace MySql.Data.MySqlClient
{
  public sealed class MySqlCommand : IDBCommand
  {
    MySqlConnection connection;
    string cmdText;
    List<MySqlParameter> parameters;
    internal Int64 lastInsertedId;
    private PreparableStatement statement;
    private int commandTimeout;
    CommandTimer commandTimer;
    private bool useDefaultTimeout;
    private bool internallyCreated;
    private bool disposed = false;

    public MySqlCommand()
    {
      parameters = new List<MySqlParameter>();
      cmdText = String.Empty;
      useDefaultTimeout = true;
    }

    public MySqlCommand(string cmdText)
      : this()
    {
      CommandText = cmdText;
    }

    public MySqlCommand(string cmdText, MySqlConnection connection)
      : this(cmdText)
    {
      Connection = connection;
    }

    ~MySqlCommand() { Dispose(false); }

    #region Properties


    public Int64 LastInsertedId
    {
      get { return lastInsertedId; }
    }

    public string CommandText
    {
      get { return cmdText; }
      set
      {
        cmdText = value ?? string.Empty;
        statement = null;
        if (cmdText != null && cmdText.EndsWith("DEFAULT VALUES", StringComparison.OrdinalIgnoreCase))
        {
          cmdText = cmdText.Substring(0, cmdText.Length - 14);
          cmdText = cmdText + "() VALUES ()";
        }
      }
    }

    public int CommandTimeout
    {
      get { return useDefaultTimeout ? 30 : commandTimeout; }
      set
      {
        if (commandTimeout < 0)
          throw new ArgumentException("Command timeout must not be negative");

        // Timeout in milliseconds should not exceed maximum for 32 bit
        // signed integer (~24 days), because underlying driver (and streams)
        // use milliseconds expressed ints for timeout values.
        // Hence, truncate the value.
        int timeout = Math.Min(value, Int32.MaxValue / 1000);
        if (timeout != value)
        {
          MySqlTrace.LogWarning(connection.ServerThread,
          "Command timeout value too large ("
          + value + " seconds). Changed to max. possible value ("
          + timeout + " seconds)");
        }
        commandTimeout = timeout;
        useDefaultTimeout = false;
      }
    }

    public bool IsPrepared
    {
      get { return statement != null && statement.IsPrepared; }
    }

    public new MySqlConnection Connection
    {
      get { return connection; }
      set
      {
        connection = value;

        // if the user has not already set the command timeout, then
        // take the default from the connection
        if (connection != null)
        {
          if (useDefaultTimeout)
          {
            commandTimeout = (int)connection.Settings.DefaultCommandTimeout;
            useDefaultTimeout = false;
          }
        }
      }
    }
    
    public void AddParam(object value) 
    {
      parameters.Add((MySqlParameter)value); 
    }
    
    public void ClearParams() 
    {
      parameters.Clear(); 
    }
    
    internal MySqlParameter FindParam(string parameterName)
    {
      foreach (MySqlParameter p in parameters)
      {
        if (parameterName.Equals(p.ParameterName, StringComparison.OrdinalIgnoreCase)) return p;
      }
      return null;
    }

    internal bool InternallyCreated
    {
      get { return internallyCreated; }
      set { internallyCreated = value; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Check the connection to make sure
    ///		- it is open
    ///		- it is not currently being used by a reader
    ///		- and we have the right version of MySQL for the requested command type
    /// </summary>
    private void CheckState()
    {
      // There must be a valid and open connection.
      if (connection == null || connection.State != ConnectionState.Open)
        throw new InvalidOperationException("Connection must be valid and open.");

      // Data readers have to be closed first
      if (connection.IsInUse && !this.internallyCreated)
        throw new MySqlException("There is already an open DataReader associated with this Connection which must be closed first.");
    }

    public int ExecuteNonQuery()
    {
      // ok, none of our interceptors handled this so we default
      using (IDBDataReader reader = ExecuteReader())
      {
        reader.Close();
        return reader.RecordsAffected;
      }
    }

    internal void ClearCommandTimer()
    {
      if (commandTimer != null)
      {
        commandTimer.Dispose();
        commandTimer = null;
      }
    }

    internal void Close(MySqlDataReader reader)
    {
      if (statement != null)
        statement.Close(reader);
      
      if (statement != null && connection != null && connection.driver != null)
        connection.driver.CloseQuery(connection, statement.StatementId);
      ClearCommandTimer();
    }

    /// <summary>
    /// Reset reader to null, to avoid "There is already an open data reader"
    /// on the next ExecuteReader(). Used in error handling scenarios.
    /// </summary>
    private void ResetReader()
    {
      if (connection != null && connection.Reader != null)
      {
        connection.Reader.Close();
        connection.Reader = null;
      }
    }

    public IDBDataReader ExecuteReader()
    {
      // interceptors didn't handle this so we fall through
      bool success = false;
      CheckState();
      Driver driver = connection.driver;

      cmdText = cmdText.Trim();
      if (String.IsNullOrEmpty(cmdText))
        throw new InvalidOperationException("The CommandText property has not been properly initialized");

      string sql = cmdText.Trim(';');

      lock (driver)
      {

        // We have to recheck that there is no reader, after we got the lock
        if (connection.Reader != null)
        {
          throw new MySqlException("There is already an open DataReader associated with this Connection which must be closed first");
        }
        
        commandTimer = new CommandTimer(connection, CommandTimeout);
        lastInsertedId = -1;

        if (statement == null || !statement.IsPrepared)
        {
          statement = new PreparableStatement(this, sql);
        }

        try
        {
          MySqlDataReader reader = new MySqlDataReader(this, statement);
          connection.Reader = reader;
          // execute the statement
          statement.Execute();
          // wait for data to return
          reader.NextResult();
          success = true;
          return reader;
        }
        catch (TimeoutException tex)
        {
          connection.HandleTimeoutOrThreadAbort(tex);
          throw; //unreached
        }
        catch (ThreadAbortException taex)
        {
          connection.HandleTimeoutOrThreadAbort(taex);
          throw;
        }
        catch (IOException ioex)
        {
          connection.Abort(); // Closes connection without returning it to the pool
          throw new MySqlException("Fatal error encountered during command execution", ioex);
        }
        catch (MySqlException ex)
        {

          if (ex.InnerException is TimeoutException)
            throw; // already handled

          try
          {
            ResetReader();
          }
          catch (Exception)
          {
            // Reset SqlLimit did not work, connection is hosed.
            Connection.Abort();
            throw new MySqlException(ex.Message, true, ex);
          }

          // if we caught an exception because of a cancel, then just return null
          if (ex.IsQueryAborted)
            return null;
          if (ex.IsFatal)
            Connection.Close();
          if (ex.Number == 0)
            throw new MySqlException("Fatal error encountered during command execution", ex);
          throw;
        }
        finally
        {
          if (connection != null)
          {
            if (connection.Reader == null)
            {
              // Something went seriously wrong,  and reader would not
              // be able to clear timeout on closing.
              // So we clear timeout here.
              ClearCommandTimer();
            }
            if (!success)
            {
              // ExecuteReader failed.Close Reader and set to null to 
              // prevent subsequent errors with DataReaderOpen
              ResetReader();
            }
          }
        }
      }
    }


    public object ExecuteScalar()
    {
      lastInsertedId = -1;
      object val = null;

      using (IDBDataReader reader = ExecuteReader())
      {
        if (reader.Read())
          val = reader.GetValue(0);
      }

      return val;
    }

    private void Prepare(int cursorPageSize)
    {
      using (new CommandTimer(Connection, CommandTimeout))
      {
        // if the length of the command text is zero, then just return
        string psSQL = CommandText;
        if (psSQL == null || psSQL.Trim().Length == 0)
          return;

        statement = new PreparableStatement(this, CommandText);
        statement.Prepare();
      }
    }

    public void Prepare()
    {
      if (connection == null)
        throw new InvalidOperationException("The connection property has not been set.");
      if (connection.State != ConnectionState.Open)
        throw new InvalidOperationException("The connection is not open.");
      if (connection.Settings.IgnorePrepare)
        return;

      Prepare(0);
    }
    #endregion

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing)
    {
      if (disposed) return;

      if (disposing && statement != null && statement.IsPrepared)
        statement.CloseStatement();
      disposed = true;
    }
  }
}

