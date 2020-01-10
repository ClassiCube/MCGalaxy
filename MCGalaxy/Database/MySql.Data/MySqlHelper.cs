// Copyright © 2004, 2014, Oracle and/or its affiliates. All rights reserved.
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

using System.Data;
using MySql.Data.MySqlClient;
using System.Text;

namespace MySql.Data.MySqlClient
{
  /// <summary>
  /// Helper class that makes it easier to work with the provider.
  /// </summary>
  public sealed class MySqlHelper
  {
    enum CharClass : byte
    {
      None,
      Quote,
      Backslash
    }

    private static string stringOfBackslashChars = "\u005c\u00a5\u0160\u20a9\u2216\ufe68\uff3c";
    private static string stringOfQuoteChars =
        "\u0022\u0027\u0060\u00b4\u02b9\u02ba\u02bb\u02bc\u02c8\u02ca\u02cb\u02d9\u0300\u0301\u2018\u2019\u201a\u2032\u2035\u275b\u275c\uff07";

    private static CharClass[] charClassArray = makeCharClassArray();

    // this class provides only static methods
    private MySqlHelper()
    {
    }

    #region Utility methods
    private static CharClass[] makeCharClassArray()
    {

      CharClass[] a = new CharClass[65536];
      foreach (char c in stringOfBackslashChars)
      {
        a[c] = CharClass.Backslash;
      }
      foreach (char c in stringOfQuoteChars)
      {
        a[c] = CharClass.Quote;
      }
      return a;
    }

    private static bool needsQuoting(string s)
    {
      foreach (char c in s)
      {
        if (charClassArray[c] != CharClass.None)
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Escapes the string.
    /// </summary>
    /// <param name="value">The string to escape</param>
    /// <returns>The string with all quotes escaped.</returns>
    public static string EscapeString(string value)
    {
      if (!needsQuoting(value))
        return value;

      StringBuilder sb = new StringBuilder();

      foreach (char c in value)
      {
        CharClass charClass = charClassArray[c];
        if (charClass != CharClass.None)
        {
          sb.Append("\\");
        }
        sb.Append(c);
      }
      return sb.ToString();
    }

    #endregion
  }
}
