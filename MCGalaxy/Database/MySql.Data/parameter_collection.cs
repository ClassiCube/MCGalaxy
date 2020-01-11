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
using System.Collections.Generic;
using MCGalaxy.SQL;

namespace MySql.Data.MySqlClient
{
  /// <summary>
  /// Represents a collection of parameters relevant to a <see cref="MySqlCommand"/> as well as their respective mappings to columns in a <see cref="System.Data.DataSet"/>. This class cannot be inherited.
  /// </summary>
  public sealed class MySqlParameterCollection : IDBDataParameterCollection
  {
    List<MySqlParameter> items = new List<MySqlParameter>();

    public void Clear()
    {
      items.Clear();
    }

    internal MySqlParameter FindParam(string parameterName)
    {
      foreach (MySqlParameter p in items)
      {
        if (parameterName.Equals(p.ParameterName, StringComparison.OrdinalIgnoreCase)) return p;
      }
      return null;
    }

    public void Add(object value)
    {
      items.Add((MySqlParameter)value);
    }
  }
}
