// Copyright © 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections;
using System.Data;
using MySql.Data.Types;
using System.Diagnostics;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient
{
  internal class ResultSet
  {
    private Driver driver;
    private bool hasRows;
    private bool[] uaFieldsUsed;
    private MySqlField[] fields;
    private IMySqlValue[] values;
    private Dictionary<string, int> fieldHashCS;
    private Dictionary<string, int> fieldHashCI;
    private int rowIndex;
    private bool readDone;
    private int affectedRows;
    private long insertedId;
    private int statementId;
    private int totalRows;
    private int skippedRows;

    public ResultSet(int affectedRows, long insertedId)
    {
      this.affectedRows = affectedRows;
      this.insertedId = insertedId;
      readDone = true;
    }

    public ResultSet(Driver d, int statementId, int numCols)
    {
      affectedRows = -1;
      insertedId = -1;
      driver = d;
      this.statementId = statementId;
      rowIndex = -1;
      LoadColumns(numCols);
      hasRows = GetNextRow();
      readDone = !hasRows;
    }

    #region Properties

    public bool HasRows
    {
      get { return hasRows; }
    }

    public int Size
    {
      get { return fields == null ? 0 : fields.Length; }
    }

    public MySqlField[] Fields
    {
      get { return fields; }
    }

    public IMySqlValue[] Values
    {
      get { return values; }
    }

    public int AffectedRows
    {
      get { return affectedRows; }
    }

    public long InsertedId
    {
      get { return insertedId; }
    }

    public int TotalRows
    {
      get { return totalRows; }
    }

    public int SkippedRows
    {
      get { return skippedRows; }
    }

    #endregion

    /// <summary>
    /// return the ordinal for the given column name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int GetOrdinal(string name)
    {
      // first we try a quick hash lookup
      int ordinal;
      if (fieldHashCS.TryGetValue(name, out ordinal))
        return ordinal;

      // ok that failed so we use our CI hash      
      if (fieldHashCI.TryGetValue( name, out ordinal ))
        return ordinal;

      // Throw an exception if the ordinal cannot be found.
      throw new IndexOutOfRangeException(
          String.Format("Could not find specified column in results: {0}", name));
    }

    /// <summary>
    /// Retrieve the value as the given column index
    /// </summary>
    /// <param name="index">The column value to retrieve</param>
    /// <returns>The value as the given column</returns>
    public IMySqlValue this[int index]
    {
      get
      {
        if (rowIndex < 0)
          throw new MySqlException("Invalid attempt to access a field before calling Read()");

        // keep count of how many columns we have left to access
        uaFieldsUsed[index] = true;
        return values[index];
      }
    }

    private bool GetNextRow()
    {
      bool fetched = driver.FetchDataRow(statementId, Size);
      if (fetched)
        totalRows++;
      return fetched;
    }


    public bool NextRow()
    {
      if (readDone) return false;

      // if we are at row index >= 0 then we need to fetch the data row and load it
      if (rowIndex >= 0)
      {
        bool fetched = false;
        try
        {
          fetched = GetNextRow();
        }
        catch (MySqlException ex)
        {
          if (ex.IsQueryAborted)
          {
            // avoid hanging on Close()
            readDone = true;
          }
          throw;
        }

        if (!fetched)
        {
          readDone = true;
          return false;
        }
      }

      for (int i = 0; i < Size; i++)
      {
        values[i] = driver.ReadColumnValue(i, fields[i], values[i]);
      }
      rowIndex++;
      return true;
    }

    /// <summary>
    /// Closes the current resultset, dumping any data still on the wire
    /// </summary>
    public void Close()
    {
      if (!readDone)
      {

        // if we have rows but the user didn't read the first one then mark it as skipped
        if (HasRows && rowIndex == -1)
          skippedRows++;
        try
        {
          while (driver.IsOpen && driver.SkipDataRow())
          {
            totalRows++;
            skippedRows++;
          }
        }
        catch (System.IO.IOException)
        {
          // it is ok to eat IO exceptions here, we just want to 
          // close the result set
        }
        readDone = true;
      }
      else if (driver == null)
        CacheClose();

      driver = null;
    }

    private void CacheClose()
    {
      skippedRows = totalRows - rowIndex - 1;
    }

    /// <summary>
    /// Loads the column metadata for the current resultset
    /// </summary>
    private void LoadColumns(int numCols)
    {
      fields = driver.GetColumns(numCols);

      values = new IMySqlValue[numCols];
      uaFieldsUsed = new bool[numCols];
      fieldHashCS = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
      fieldHashCI = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

      for (int i = 0; i < fields.Length; i++)
      {
        string columnName = fields[i].ColumnName;
        if (!fieldHashCS.ContainsKey(columnName))
          fieldHashCS.Add(columnName, i);
        if (!fieldHashCI.ContainsKey(columnName))
          fieldHashCI.Add(columnName, i);
        values[i] = fields[i].GetValueObject();
      }
    }
  }
}
