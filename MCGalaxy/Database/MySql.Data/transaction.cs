// Copyright (c) 2004, 2016 Oracle and/or its affiliates. All rights reserved.
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
using System.Data;
using System.Data.Common;
using MCGalaxy.SQL;

namespace MySql.Data.MySqlClient
{
  public sealed class MySqlTransaction : IDBTransaction, IDisposable
  {
    private MySqlConnection conn;
    private bool open;
    private bool disposed = false;

    internal MySqlTransaction(MySqlConnection c)
    {
      conn = c;
      open = true;
    }

    ~MySqlTransaction()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing)
    {
      if (disposed) return;
      if (disposing)
      {
        if ((conn != null && conn.State == ConnectionState.Open) && open)
          Rollback();
      }
      disposed = true;
    }

    public void Commit()
    {
      if (conn == null || conn.State != ConnectionState.Open)
        throw new InvalidOperationException("Connection must be valid and open to commit transaction");
      if (!open)
        throw new InvalidOperationException("Transaction has already been committed or is not pending");
      MySqlCommand cmd = new MySqlCommand("COMMIT", conn);
      cmd.ExecuteNonQuery();
      open = false;
    }

    public void Rollback()
    {
      if (conn == null || conn.State != ConnectionState.Open)
        throw new InvalidOperationException("Connection must be valid and open to rollback transaction");
      if (!open)
        throw new InvalidOperationException("Transaction has already been rolled back or is not pending");
      MySqlCommand cmd = new MySqlCommand("ROLLBACK", conn);
      cmd.ExecuteNonQuery();
      open = false;
    }
  }
}
