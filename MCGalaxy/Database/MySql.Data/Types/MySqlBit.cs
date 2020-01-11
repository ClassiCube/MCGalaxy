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

namespace MySql.Data.Types
{
  /// <summary>
  /// Summary description for MySqlUInt64.
  /// </summary>
  internal struct MySqlBit : IMySqlValue
  {
    private ulong mValue;
    private bool isNull;
    private bool readAsString;

    public MySqlBit(bool isnull)
    {
      mValue = 0;
      isNull = isnull;
      readAsString = false;
    }

    public bool ReadAsString
    {
      get { return readAsString; }
      set { readAsString = value; }
    }

    public bool IsNull
    {
      get { return isNull; }
    }

    object IMySqlValue.Value
    {
      get
      {
        return mValue;
      }
    }

    Type IMySqlValue.SystemType
    {
      get
      {
        return typeof(UInt64);
      }
    }

    string IMySqlValue.MySqlTypeName
    {
      get { return "BIT"; }
    }

    public void WriteValue(MySqlPacket packet, bool binary, object value)
    {
      ulong v = (value is UInt64) ? (UInt64)value : Convert.ToUInt64(value);
      if (binary)
        packet.WriteInteger((long)v, 8);
      else
        packet.WriteStringNoNull(v.ToString());
    }

    public IMySqlValue ReadValue(MySqlPacket packet, long length, bool isNull)
    {
      this.isNull = isNull;
      if (isNull)
        return this;

      if (length == -1)
        length = packet.ReadFieldLength();

      if (ReadAsString)
        mValue = UInt64.Parse(packet.ReadString(length));
      else
        mValue = (UInt64)packet.ReadBitValue((int)length);
      return this;
    }

    public void SkipValue(MySqlPacket packet)
    {
      int len = (int)packet.ReadFieldLength();
      packet.Position += len;
    }

  }
}
