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
using System.IO;
using System.Collections;
using System.Text;
#if !RT
using System.Data;
using System.Data.Common;
#endif
using MySql.Data.Common;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
  /// <include file='docs/mysqlcommand.xml' path='docs/ClassSummary/*'/> 
  public sealed partial class MySqlCommand : ICloneable, IDisposable
  {
    MySqlConnection connection;
    MySqlTransaction curTransaction;
    string cmdText;
    CommandType cmdType;
    MySqlParameterCollection parameters;
    internal Int64 lastInsertedId;
    private PreparableStatement statement;
    private int commandTimeout;
    private bool canceled;
    private bool resetSqlSelect;
    CommandTimer commandTimer;
    private bool useDefaultTimeout;
    private bool internallyCreated;
    private static List<string> keywords = null;
    private bool disposed = false;

    /// <include file='docs/mysqlcommand.xml' path='docs/ctor1/*'/>
    public MySqlCommand()
    {
      cmdType = CommandType.Text;
      parameters = new MySqlParameterCollection(this);
      cmdText = String.Empty;
      useDefaultTimeout = true;
      Constructor();
    }

    partial void Constructor();

    /// <include file='docs/mysqlcommand.xml' path='docs/ctor2/*'/>
    public MySqlCommand(string cmdText)
      : this()
    {
      CommandText = cmdText;
    }

    /// <include file='docs/mysqlcommand.xml' path='docs/ctor3/*'/>
    public MySqlCommand(string cmdText, MySqlConnection connection)
      : this(cmdText)
    {
      Connection = connection;
    }

    /// <include file='docs/mysqlcommand.xml' path='docs/ctor4/*'/>
    public MySqlCommand(string cmdText, MySqlConnection connection,
            MySqlTransaction transaction)
      :
      this(cmdText, connection)
    {
      curTransaction = transaction;
    }

    #region Destructor
#if !RT
    ~MySqlCommand()
    {
      Dispose(false);
    }
#else
    ~MySqlCommand()
    {
      this.Dispose();
    }
#endif
    #endregion

    #region Properties


    public Int64 LastInsertedId
    {
      get { return lastInsertedId; }
    }

    public override string CommandText
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

    public override int CommandTimeout
    {
      get { return useDefaultTimeout ? 30 : commandTimeout; }
      set
      {
        if (commandTimeout < 0)
          Throw(new ArgumentException("Command timeout must not be negative"));

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

    public override CommandType CommandType
    {
      get { return cmdType; }
      set { cmdType = value; }
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
        /*
        * The connection is associated with the transaction
        * so set the transaction object to return a null reference if the connection 
        * is reset.
        */
        if (connection != value)
          Transaction = null;

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

    public new MySqlParameterCollection Parameters
    {
      get { return parameters; }
    }


    public new MySqlTransaction Transaction
    {
      get { return curTransaction; }
      set { curTransaction = value; }
    }

    internal bool Canceled
    {
      get { return canceled; }
    }

    internal bool InternallyCreated
    {
      get { return internallyCreated; }
      set { internallyCreated = value; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Attempts to cancel the execution of a currently active command
    /// </summary>
    /// <remarks>
    /// Cancelling a currently active query only works with MySQL versions 5.0.0 and higher.
    /// </remarks>
    public override void Cancel()
    {
      connection.CancelQuery(connection.ConnectionTimeout);
      canceled = true;
    }

    /// <summary>
    /// Creates a new instance of a <see cref="MySqlParameter"/> object.
    /// </summary>
    /// <remarks>
    /// This method is a strongly-typed version of <see cref="IDbCommand.CreateParameter"/>.
    /// </remarks>
    /// <returns>A <see cref="MySqlParameter"/> object.</returns>
    /// 
    public new MySqlParameter CreateParameter()
    {
      return (MySqlParameter)CreateDbParameter();
    }

    /// <summary>
    /// Check the connection to make sure
    ///		- it is open
    ///		- it is not currently being used by a reader
    ///		- and we have the right version of MySQL for the requested command type
    /// </summary>
    private void CheckState()
    {
      // There must be a valid and open connection.
      if (connection == null)
        Throw(new InvalidOperationException("Connection must be valid and open."));

      if (connection.State != ConnectionState.Open && !connection.SoftClosed)
        Throw(new InvalidOperationException("Connection must be valid and open."));

      // Data readers have to be closed first
      if (connection.IsInUse && !this.internallyCreated)
        Throw(new MySqlException("There is already an open DataReader associated with this Connection which must be closed first."));
    }

    /// <include file='docs/mysqlcommand.xml' path='docs/ExecuteNonQuery/*'/>
    public override int ExecuteNonQuery()
    {
      // ok, none of our interceptors handled this so we default
      using (MySqlDataReader reader = ExecuteReader())
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
      ResetSqlSelectLimit();
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

    /// <summary>
    /// Reset SQL_SELECT_LIMIT that could have been modified by CommandBehavior.
    /// </summary>
    internal void ResetSqlSelectLimit()
    {
      // if we are supposed to reset the sql select limit, do that here
      if (resetSqlSelect)
      {
        resetSqlSelect = false;
        MySqlCommand command = new MySqlCommand("SET SQL_SELECT_LIMIT=DEFAULT", connection);
        command.internallyCreated = true;
        command.ExecuteNonQuery();
      }
    }

    /// <include file='docs/mysqlcommand.xml' path='docs/ExecuteReader/*'/>
    public new MySqlDataReader ExecuteReader()
    {
      return ExecuteReader(CommandBehavior.Default);
    }


    /// <include file='docs/mysqlcommand.xml' path='docs/ExecuteReader1/*'/>
    public new MySqlDataReader ExecuteReader(CommandBehavior behavior)
    {
      // interceptors didn't handle this so we fall through
      bool success = false;
      CheckState();
      Driver driver = connection.driver;

      cmdText = cmdText.Trim();
      if (String.IsNullOrEmpty(cmdText))
        Throw(new InvalidOperationException(Resources.CommandTextNotInitialized));

      string sql = cmdText.Trim(';');

      lock (driver)
      {

        // We have to recheck that there is no reader, after we got the lock
        if (connection.Reader != null)
        {
          Throw(new MySqlException(Resources.DataReaderOpen));
        }

#if !RT
        System.Transactions.Transaction curTrans = System.Transactions.Transaction.Current;

        if (curTrans != null)
        {
          bool inRollback = false;
          if (driver.CurrentTransaction != null)
            inRollback = driver.CurrentTransaction.InRollback;
          if (!inRollback)
          {
            System.Transactions.TransactionStatus status = System.Transactions.TransactionStatus.InDoubt;
            try
            {
              // in some cases (during state transitions) this throws
              // an exception. Ignore exceptions, we're only interested 
              // whether transaction was aborted or not.
              status = curTrans.TransactionInformation.Status;
            }
            catch (System.Transactions.TransactionException)
            {
            }
            if (status == System.Transactions.TransactionStatus.Aborted)
              Throw(new System.Transactions.TransactionAbortedException());
          }
        }
#endif
        commandTimer = new CommandTimer(connection, CommandTimeout);

        lastInsertedId = -1;

        if (CommandType == CommandType.TableDirect)
          sql = "SELECT * FROM " + sql;
        else if (CommandType == CommandType.Text)
        {
          // validates single word statetment (maybe is a stored procedure call)
          if (sql.IndexOf(" ") == -1)
          {
            if (AddCallStatement(sql))
              sql = "call " + sql;
          }
        }

        if (statement == null || !statement.IsPrepared)
        {
          statement = new PreparableStatement(this, sql);
        }

        // stored procs are the only statement type that need do anything during resolve
        statement.Resolve(false);

        // Now that we have completed our resolve step, we can handle our
        // command behaviors
        HandleCommandBehaviors(behavior);

        try
        {
          MySqlDataReader reader = new MySqlDataReader(this, statement, behavior);
          connection.Reader = reader;
          canceled = false;
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
          throw new MySqlException(Resources.FatalErrorDuringExecute, ioex);
        }
        catch (MySqlException ex)
        {

          if (ex.InnerException is TimeoutException)
            throw; // already handled

          try
          {
            ResetReader();
            ResetSqlSelectLimit();
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
            throw new MySqlException(Resources.FatalErrorDuringExecute, ex);
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


    /// <include file='docs/mysqlcommand.xml' path='docs/ExecuteScalar/*'/>
    public override object ExecuteScalar()
    {
      lastInsertedId = -1;
      object val = null;

      using (MySqlDataReader reader = ExecuteReader())
      {
        if (reader.Read())
          val = reader.GetValue(0);
      }

      return val;
    }

    private void HandleCommandBehaviors(CommandBehavior behavior)
    {
      if ((behavior & CommandBehavior.SchemaOnly) != 0)
      {
        new MySqlCommand("SET SQL_SELECT_LIMIT=0", connection).ExecuteNonQuery();
        resetSqlSelect = true;
      }
      else if ((behavior & CommandBehavior.SingleRow) != 0)
      {
        new MySqlCommand("SET SQL_SELECT_LIMIT=1", connection).ExecuteNonQuery();
        resetSqlSelect = true;
      }
    }

    /// <include file='docs/mysqlcommand.xml' path='docs/Prepare2/*'/>
    private void Prepare(int cursorPageSize)
    {
      using (new CommandTimer(Connection, CommandTimeout))
      {
        // if the length of the command text is zero, then just return
        string psSQL = CommandText;
        if (psSQL == null ||
             psSQL.Trim().Length == 0)
          return;

        statement = new PreparableStatement(this, CommandText);
        statement.Resolve(true);
        statement.Prepare();
      }
    }

    /// <include file='docs/mysqlcommand.xml' path='docs/Prepare/*'/>
    public override void Prepare()
    {
      if (connection == null)
        Throw(new InvalidOperationException("The connection property has not been set."));
      if (connection.State != ConnectionState.Open)
        Throw(new InvalidOperationException("The connection is not open."));
      if (connection.Settings.IgnorePrepare)
        return;

      Prepare(0);
    }
    #endregion

    #region Private Methods

    /*		private ArrayList PrepareSqlBuffers(string sql)
                {
                    ArrayList buffers = new ArrayList();
                    MySqlStreamWriter writer = new MySqlStreamWriter(new MemoryStream(), connection.Encoding);
                    writer.Version = connection.driver.Version;

                    // if we are executing as a stored procedure, then we need to add the call
                    // keyword.
                    if (CommandType == CommandType.StoredProcedure)
                    {
                        if (storedProcedure == null)
                            storedProcedure = new StoredProcedure(this);
                        sql = storedProcedure.Prepare( CommandText );
                    }

                    // tokenize the SQL
                    sql = sql.TrimStart(';').TrimEnd(';');
                    ArrayList tokens = TokenizeSql( sql );

                    foreach (string token in tokens)
                    {
                        if (token.Trim().Length == 0) continue;
                        if (token == ";" && ! connection.driver.SupportsBatch)
                        {
                            MemoryStream ms = (MemoryStream)writer.Stream;
                            if (ms.Length > 0)
                                buffers.Add( ms );

                            writer = new MySqlStreamWriter(new MemoryStream(), connection.Encoding);
                            writer.Version = connection.driver.Version;
                            continue;
                        }
                        else if (token[0] == parameters.ParameterMarker) 
                        {
                            if (SerializeParameter(writer, token)) continue;
                        }

                        // our fall through case is to write the token to the byte stream
                        writer.WriteStringNoNull(token);
                    }

                    // capture any buffer that is left over
                    MemoryStream mStream = (MemoryStream)writer.Stream;
                    if (mStream.Length > 0)
                        buffers.Add( mStream );

                    return buffers;
                }*/


    /// <summary>
    /// Verifies if a query is valid even if it has not spaces or is a stored procedure call
    /// </summary>
    /// <param name="query">Query to validate</param>
    /// <returns>If it is necessary to add call statement</returns>
    private bool AddCallStatement(string query)
    {
      if (string.IsNullOrEmpty(query)) return false;

      string keyword = query.ToUpper();
      int indexChar = keyword.IndexOfAny(new char[] { '(', '"', '@', '\'', '`' });
      if (indexChar > 0)
        keyword = keyword.Substring(0, indexChar);

      if (keywords == null)
        keywords = new List<string>(Resources.keywords.Replace("\r", "").Split('\n'));

      return !keywords.Contains(keyword);
    }

    #endregion

    #region ICloneable

    /// <summary>
    /// Creates a clone of this MySqlCommand object.  CommandText, Connection, and Transaction properties
    /// are included as well as the entire parameter list.
    /// </summary>
    /// <returns>The cloned MySqlCommand object</returns>
    public MySqlCommand Clone()
    {
      MySqlCommand clone = new MySqlCommand(cmdText, connection, curTransaction);
      clone.CommandType = CommandType;
      clone.commandTimeout = commandTimeout;
      clone.useDefaultTimeout = useDefaultTimeout;
      PartialClone(clone);

      foreach (MySqlParameter p in parameters)
      {
        clone.Parameters.Add(p.Clone());
      }
      return clone;
    }

    partial void PartialClone(MySqlCommand clone);

    object ICloneable.Clone()
    {
      return this.Clone();
    }

    #endregion

    // This method is used to throw all exceptions from this class.  
    private void Throw(Exception ex)
    {
      if (connection != null)
        connection.Throw(ex);
      throw ex;
    }

#if !RT
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposed)
        return;

      if (disposing)
        if (statement != null && statement.IsPrepared)
          statement.CloseStatement();

      base.Dispose(disposing);
      disposed = true;

    }
#else
    public void Dispose()
    {
      GC.SuppressFinalize(this);
    }
#endif
  }
}

