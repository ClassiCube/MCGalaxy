// Copyright © 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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
using MySql.Data.Types;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Collections;
#if !RT
using System.Data;
using System.Data.Common;
#endif
using MCGalaxy.SQL;

namespace MySql.Data.MySqlClient
{
  /// <summary>
  /// Represents a parameter to a <see cref="MySqlCommand"/>, and optionally, its mapping to <see cref="DataSet"/> columns. This class cannot be inherited.
  /// </summary>
  public sealed class MySqlParameter : IDBDataParameter
  {
    private const int UNSIGNED_MASK = 0x8000;
    private object paramValue;
    private string paramName;
    private MySqlDbType mySqlDbType;
    private bool inferType = true;

    public MySqlParameter() { }
    public MySqlParameter(string parameterName, object value)
    {
      ParameterName = parameterName;
      Value = value;
    }

    public MySqlParameter(string parameterName, MySqlDbType dbType) : this(parameterName, null)
    {
      MySqlDbType = dbType;
    }

    public string ParameterName
    {
      get { return paramName; }
      set { paramName = value; }
    }

    internal Encoding Encoding { get; set; }

    internal bool TypeHasBeenSet
    {
      get { return inferType == false; }
    }


    internal string BaseName
    {
      get
      {
        if (ParameterName.StartsWith("@", StringComparison.Ordinal) || ParameterName.StartsWith("?", StringComparison.Ordinal))
          return ParameterName.Substring(1);
        return ParameterName;
      }
    }

    public MySqlDbType MySqlDbType
    {
      get { return mySqlDbType; }
      set
      {
        SetMySqlDbType(value);
        inferType = false;
      }
    }

    public object Value
    {
      get { return paramValue; }
      set
      {
        paramValue = value;
        if (inferType) SetTypeFromValue();
      }
    }

    IMySqlValue _valueObject;
    internal IMySqlValue ValueObject
    {
      get { return _valueObject; }
      private set { _valueObject = value; }
    }

    public override string ToString()
    {
      return paramName;
    }

    internal int GetPSType()
    {
      switch (mySqlDbType)
      {
        case MySqlDbType.Bit:
          return (int)MySqlDbType.Int64 | UNSIGNED_MASK;
        case MySqlDbType.UByte:
          return (int)MySqlDbType.Byte | UNSIGNED_MASK;
        case MySqlDbType.UInt64:
          return (int)MySqlDbType.Int64 | UNSIGNED_MASK;
        case MySqlDbType.UInt32:
          return (int)MySqlDbType.Int32 | UNSIGNED_MASK;
        case MySqlDbType.UInt24:
          return (int)MySqlDbType.Int32 | UNSIGNED_MASK;
        case MySqlDbType.UInt16:
          return (int)MySqlDbType.Int16 | UNSIGNED_MASK;
        default:
          return (int)mySqlDbType;
      }
    }

    internal void Serialize(MySqlPacket packet, bool binary, MySqlConnectionStringBuilder settings)
    {
      if (!binary && (paramValue == null || paramValue == DBNull.Value))
        packet.WriteStringNoNull("NULL");
      else
      {
        if (ValueObject.MySqlDbType == MySqlDbType.Guid)
        {
          MySqlGuid g = (MySqlGuid)ValueObject;
          g.OldGuids = settings.OldGuids;
          ValueObject = g;
        }
        ValueObject.WriteValue(packet, binary, paramValue);
      }
    }

    private void SetMySqlDbType(MySqlDbType mysql_dbtype)
    {
      mySqlDbType = mysql_dbtype;
      ValueObject = MySqlField.GetIMySqlValue(mySqlDbType);
      SetDbTypeFromMySqlDbType();
    }

    private void SetTypeFromValue()
    {
      if (paramValue == null || paramValue == DBNull.Value) return;

      if (paramValue is Guid)
        MySqlDbType = MySqlDbType.Guid;
      else if (paramValue is TimeSpan)
        MySqlDbType = MySqlDbType.Time;
      else if (paramValue is bool)
        MySqlDbType = MySqlDbType.Byte;
      else
      {
        Type t = paramValue.GetType();
        switch (t.Name)
        {
          case "SByte": MySqlDbType = MySqlDbType.Byte; break;
          case "Byte": MySqlDbType = MySqlDbType.UByte; break;
          case "Int16": MySqlDbType = MySqlDbType.Int16; break;
          case "UInt16": MySqlDbType = MySqlDbType.UInt16; break;
          case "Int32": MySqlDbType = MySqlDbType.Int32; break;
          case "UInt32": MySqlDbType = MySqlDbType.UInt32; break;
          case "Int64": MySqlDbType = MySqlDbType.Int64; break;
          case "UInt64": MySqlDbType = MySqlDbType.UInt64; break;
          case "DateTime": MySqlDbType = MySqlDbType.DateTime; break;
          case "String": MySqlDbType = MySqlDbType.VarChar; break;
          case "Single": MySqlDbType = MySqlDbType.Float; break;
          case "Double": MySqlDbType = MySqlDbType.Double; break;

          case "Decimal": MySqlDbType = MySqlDbType.Decimal; break;
          case "Object": 
          default:
            if( t.BaseType == typeof( Enum ) )
              MySqlDbType = MySqlDbType.Int32;
            else 
              MySqlDbType = MySqlDbType.Blob; 
            break;
        }
      }
    }
    
    
    private DbType dbType;
    public DbType DbType
    {
      get { return dbType; }
      set
      {
        SetDbType(value);
        inferType = false;
      }
    }

    void SetDbTypeFromMySqlDbType()
    {
      switch (mySqlDbType)
      {
        case MySqlDbType.NewDecimal:
        case MySqlDbType.Decimal:
          dbType = DbType.Decimal;
          break;
        case MySqlDbType.Byte:
          dbType = DbType.SByte;
          break;
        case MySqlDbType.UByte:
          dbType = DbType.Byte;
          break;
        case MySqlDbType.Int16:
          dbType = DbType.Int16;
          break;
        case MySqlDbType.UInt16:
          dbType = DbType.UInt16;
          break;
        case MySqlDbType.Int24:
        case MySqlDbType.Int32:
          dbType = DbType.Int32;
          break;
        case MySqlDbType.UInt24:
        case MySqlDbType.UInt32:
          dbType = DbType.UInt32;
          break;
        case MySqlDbType.Int64:
          dbType = DbType.Int64;
          break;
        case MySqlDbType.UInt64:
          dbType = DbType.UInt64;
          break;
        case MySqlDbType.Bit:
          dbType = DbType.UInt64;
          break;
        case MySqlDbType.Float:
          dbType = DbType.Single;
          break;
        case MySqlDbType.Double:
          dbType = DbType.Double;
          break;
        case MySqlDbType.Timestamp:
        case MySqlDbType.DateTime:
          dbType = DbType.DateTime;
          break;
        case MySqlDbType.Date:
        case MySqlDbType.Newdate:
        case MySqlDbType.Year:
          dbType = DbType.Date;
          break;
        case MySqlDbType.Time:
          dbType = DbType.Time;
          break;
        case MySqlDbType.Enum:
        case MySqlDbType.Set:
        case MySqlDbType.VarChar:
          dbType = DbType.String;
          break;
        case MySqlDbType.TinyBlob:
        case MySqlDbType.MediumBlob:
        case MySqlDbType.LongBlob:
        case MySqlDbType.Blob:
          dbType = DbType.Object;
          break;
        case MySqlDbType.String:
          dbType = DbType.StringFixedLength;
          break;
        case MySqlDbType.Guid:
          dbType = DbType.Guid;
          break;
      }
    }


    private void SetDbType(DbType db_type)
    {
      dbType = db_type;
      switch (dbType)
      {
        case DbType.Guid:
          mySqlDbType = MySqlDbType.Guid;
          break;

        case DbType.AnsiString:
        case DbType.String:
          mySqlDbType = MySqlDbType.VarChar;
          break;

        case DbType.AnsiStringFixedLength:
        case DbType.StringFixedLength:
          mySqlDbType = MySqlDbType.String;
          break;

        case DbType.Boolean:
        case DbType.Byte:
          mySqlDbType = MySqlDbType.UByte;
          break;

        case DbType.SByte:
          mySqlDbType = MySqlDbType.Byte;
          break;

        case DbType.Date:
          mySqlDbType = MySqlDbType.Date;
          break;
        case DbType.DateTime:
          mySqlDbType = MySqlDbType.DateTime;
          break;

        case DbType.Time:
          mySqlDbType = MySqlDbType.Time;
          break;
        case DbType.Single:
          mySqlDbType = MySqlDbType.Float;
          break;
        case DbType.Double:
          mySqlDbType = MySqlDbType.Double;
          break;

        case DbType.Int16:
          mySqlDbType = MySqlDbType.Int16;
          break;
        case DbType.UInt16:
          mySqlDbType = MySqlDbType.UInt16;
          break;

        case DbType.Int32:
          mySqlDbType = MySqlDbType.Int32;
          break;
        case DbType.UInt32:
          mySqlDbType = MySqlDbType.UInt32;
          break;

        case DbType.Int64:
          mySqlDbType = MySqlDbType.Int64;
          break;
        case DbType.UInt64:
          mySqlDbType = MySqlDbType.UInt64;
          break;

        case DbType.Decimal:
        case DbType.Currency:
          mySqlDbType = MySqlDbType.Decimal;
          break;

        case DbType.Object:
        case DbType.VarNumeric:
        case DbType.Binary:
        default:
          mySqlDbType = MySqlDbType.Blob;
          break;
      }

      ValueObject = MySqlField.GetIMySqlValue(mySqlDbType);
    }
  }

}
