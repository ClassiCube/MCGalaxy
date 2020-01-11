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
using MySql.Data.MySqlClient;
using System.Globalization;

namespace MySql.Data.Types
{
  internal struct MySqlUInt32 : IMySqlValue
  {
    private uint mValue;
    private bool isNull;
    private bool is24Bit;

    private MySqlUInt32(MySqlDbType type)
    {
      is24Bit = type == MySqlDbType.Int24 ? true : false;
      isNull = true;
      mValue = 0;
    }

    public MySqlUInt32(MySqlDbType type, bool isNull)
      : this(type)
    {
      this.isNull = isNull;
    }

    public MySqlUInt32(MySqlDbType type, uint val)
      : this(type)
    {
      this.isNull = false;
      mValue = val;
    }

    #region IMySqlValue Members

    public bool IsNull
    {
      get { return isNull; }
    }

    object IMySqlValue.Value
    {
      get { return mValue; }
    }

    public uint Value
    {
      get { return mValue; }
    }

    Type IMySqlValue.SystemType
    {
      get { return typeof(UInt32); }
    }

    string IMySqlValue.MySqlTypeName
    {
      get { return is24Bit ? "MEDIUMINT" : "INT"; }
    }

    void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object v)
    {
      uint val = (v is uint) ? (uint)v : Convert.ToUInt32(v);
      if (binary)
        packet.WriteInteger((long)val, is24Bit ? 3 : 4);
      else
        packet.WriteStringNoNull(val.ToString());
    }

    IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
    {
      if (nullVal)
        return new MySqlUInt32(MySqlDbType.UInt32, true);

      if (length == -1)
        return new MySqlUInt32(MySqlDbType.UInt32, (uint)packet.ReadInteger(4));
      else
        return new MySqlUInt32(MySqlDbType.UInt32,
                     UInt32.Parse(packet.ReadString(length), NumberStyles.Any, CultureInfo.InvariantCulture));
    }

    #endregion
  }
}
