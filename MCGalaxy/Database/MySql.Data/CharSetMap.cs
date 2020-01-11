// Copyright © 2004, 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;
using MySql.Data.Common;
using System.Collections.Generic;
using System.Data;
using MCGalaxy.SQL;

namespace MySql.Data.MySqlClient
{
  /// <summary>
  /// Summary description for CharSetMap.
  /// </summary>
  internal class CharSetMap
  {
    private static Dictionary<string, string> mapping;

    // we use a static constructor here since we only want to init
    // the mapping once
    static CharSetMap()
    {
      LoadCharsetMap();
    }

    public static string GetCharacterSet(string CharSetName)
    {
      string cs = null;
      if (mapping.ContainsKey(CharSetName))
        cs = mapping[CharSetName];

      if (cs == null)
        throw new MySqlException("Character set '" + CharSetName + "' is not supported by .Net Framework.");
      return cs;
    }

    /// <summary>
    /// Returns the text encoding for a given MySQL character set name
    /// </summary>
    /// <param name="version">Version of the connection requesting the encoding</param>
    /// <param name="CharSetName">Name of the character set to get the encoding for</param>
    /// <returns>Encoding object for the given character set name</returns>
    public static Encoding GetEncoding(string CharSetName)
    {
      try
      {
        string cs = GetCharacterSet(CharSetName);
        return Encoding.GetEncoding(cs);
      }
      catch (NotSupportedException)
      {
        return Encoding.UTF8;
      }
    }

    private static void LoadCharsetMap()
    {
      mapping = new Dictionary<string, string>();

      mapping.Add("latin1", "windows-1252");
      mapping.Add("big5", "big5");
      mapping.Add("dec8", mapping["latin1"]);
      mapping.Add("cp850", "ibm850");
      mapping.Add("hp8", mapping["latin1"]);
      mapping.Add("koi8r", "koi8-u");
      mapping.Add("latin2", "latin2");
      mapping.Add("swe7", mapping["latin1"]);
      mapping.Add("ujis", "EUC-JP");
      mapping.Add("eucjpms", mapping["ujis"]);
      mapping.Add("sjis", "sjis");
      mapping.Add("cp932", mapping["sjis"]);
      mapping.Add("hebrew", "hebrew");
      mapping.Add("tis620", "windows-874");
      mapping.Add("euckr", "euc-kr");
      mapping.Add("euc_kr", mapping["euckr"]);
      mapping.Add("koi8u", "koi8-u");
      mapping.Add("koi8_ru", mapping["koi8u"]);
      mapping.Add("gb2312", "gb2312");
      mapping.Add("gbk", mapping["gb2312"]);
      mapping.Add("greek", "greek");
      mapping.Add("cp1250", "windows-1250");
      mapping.Add("win1250", mapping["cp1250"]);
      mapping.Add("latin5", "latin5");
      mapping.Add("armscii8", mapping["latin1"]);
      mapping.Add("utf8", "utf-8");
      mapping.Add("ucs2", "UTF-16BE");
      mapping.Add("cp866", "cp866");
      mapping.Add("keybcs2", mapping["latin1"]);
      mapping.Add("macce", "x-mac-ce");
      mapping.Add("macroman", "x-mac-romanian");
      mapping.Add("cp852", "ibm852");
      mapping.Add("latin7", "iso-8859-7");
      mapping.Add("cp1251", "windows-1251");
      mapping.Add("win1251ukr", mapping["cp1251"]);
      mapping.Add("cp1251csas", mapping["cp1251"]);
      mapping.Add("cp1251cias", mapping["cp1251"]);
      mapping.Add("win1251", mapping["cp1251"]);
      mapping.Add("cp1256", "cp1256");
      mapping.Add("cp1257", "windows-1257");
      mapping.Add("ascii", "us-ascii");
      mapping.Add("usa7", mapping["ascii"]);
      mapping.Add("binary", mapping["ascii"]);
      mapping.Add("latin3", "latin3");
      mapping.Add("latin4", "latin4");
      mapping.Add("latin1_de", "iso-8859-1");
      mapping.Add("german1", "iso-8859-1");
      mapping.Add("danish", "iso-8859-1");
      mapping.Add("czech", "iso-8859-2");
      mapping.Add("hungarian", "iso-8859-2");
      mapping.Add("croat", "iso-8859-2");
      mapping.Add("latvian", "iso-8859-13");
      mapping.Add("latvian1", "iso-8859-13");
      mapping.Add("estonia", "iso-8859-13");
      mapping.Add("dos", "ibm437");
      mapping.Add("utf8mb4", "utf-8");
      mapping.Add("utf16", "utf-16BE");
      mapping.Add("utf16le", "utf-16");
      mapping.Add("utf32", "utf-32BE");
      mapping.Add("gb18030", "gb18030");             
    }
  }
}