// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.IO;
using MySql.Data.MySqlClient;

namespace MySql.Data.Types
{
  internal struct MySqlString : IMySqlValue
  {
    private string mValue;
    private bool isNull;
    private MySqlDbType type;

    public MySqlString(MySqlDbType type, bool isNull)
    {
      this.type = type;
      this.isNull = isNull;
      mValue = String.Empty;
    }

    public MySqlString(MySqlDbType type, string val)
    {
      this.type = type;
      this.isNull = false;
      mValue = val;
    }

    #region IMySqlValue Members

    public bool IsNull
    {
      get { return isNull; }
    }

    MySqlDbType IMySqlValue.MySqlDbType
    {
      get { return type; }
    }

    object IMySqlValue.Value
    {
      get { return mValue; }
    }

    public string Value
    {
      get { return mValue; }
    }

    Type IMySqlValue.SystemType
    {
      get { return typeof(string); }
    }

    string IMySqlValue.MySqlTypeName
    {
      get { return type == MySqlDbType.Set ? "SET" : type == MySqlDbType.Enum ? "ENUM" : "VARCHAR"; }
    }


    void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
    {
      string v = val.ToString();
      if (length > 0)
      {
        length = Math.Min(length, v.Length);
        v = v.Substring(0, length);
      }

      if (binary)
        packet.WriteLenString(v);
      else
        packet.WriteStringNoNull("'" + MySqlHelper.EscapeString(v) + "'");
    }

    IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
    {
      if (nullVal)
        return new MySqlString(type, true);

      string s = String.Empty;
      if (length == -1)
        s = packet.ReadLenString();
      else
        s = packet.ReadString(length);
      MySqlString str = new MySqlString(type, s);
      return str;
    }

    void IMySqlValue.SkipValue(MySqlPacket packet)
    {
      int len = (int)packet.ReadFieldLength();
      packet.Position += len;
    }

    #endregion

  }
}