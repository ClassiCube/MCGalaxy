// Copyright © 2013, 2018, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;
using System.Globalization;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace MySql.Data.MySqlClient
{
  public sealed partial class MySqlConnectionStringBuilder 
  {
    internal Dictionary<string, object> values = new Dictionary<string, object>();
    //internal Dictionary<string, object> values
    //{
    //  get { lock (this) { return _values; } }
    //}

    private static MySqlConnectionStringOptionCollection options = new MySqlConnectionStringOptionCollection();

    static MySqlConnectionStringBuilder()
    {
      // Server options
      options.Add(new MySqlConnectionStringOption("server", "host,data source,datasource,address,addr,network address", typeof(string), "" /*"localhost"*/, false));
      options.Add(new MySqlConnectionStringOption("database", "initial catalog", typeof(string), string.Empty, false));
      options.Add(new MySqlConnectionStringOption("port", null, typeof(uint), ( uint )3306, false));
      options.Add(new MySqlConnectionStringOption("pipe", "pipe name,pipename", typeof(string), "MYSQL", false));
      options.Add(new MySqlConnectionStringOption("allowbatch", "allow batch", typeof(bool), true, false));
      options.Add(new MySqlConnectionStringOption("logging", null, typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("sharedmemoryname", "shared memory name", typeof(string), "MYSQL", false));
      options.Add(new MySqlConnectionStringOption("useoldsyntax", "old syntax,oldsyntax,use old syntax", typeof(bool), false, true,
        delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
        {
          MySqlTrace.LogWarning(-1, "Use Old Syntax is now obsolete.  Please see documentation");
          msb.SetValue("useoldsyntax", value);
        },
        delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender)
        {
          return (bool)msb.values["useoldsyntax"];
        }
        ));
      options.Add(new MySqlConnectionStringOption("connectiontimeout", "connection timeout,connect timeout", typeof(uint), (uint)15, false,
        delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object Value)
        {
          uint value = (uint)Convert.ChangeType(Value, sender.BaseType);          
          // Timeout in milliseconds should not exceed maximum for 32 bit
          // signed integer (~24 days). We truncate the value if it exceeds 
          // maximum (MySqlCommand.CommandTimeout uses the same technique
          uint timeout = Math.Min(value, Int32.MaxValue / 1000);
          if (timeout != value)
          {
            MySqlTrace.LogWarning(-1, "Connection timeout value too large ("
                + value + " seconds). Changed to max. possible value" +
                +timeout + " seconds)");
          }
          msb.SetValue("connectiontimeout", timeout);
        },
        delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender)
        {
          return (uint)msb.values["connectiontimeout"];
        }
        ));
      options.Add(new MySqlConnectionStringOption("defaultcommandtimeout", "command timeout,default command timeout", typeof(uint), ( uint )30, false));
      options.Add(new MySqlConnectionStringOption("usedefaultcommandtimeoutforef", "use default command timeout for ef", typeof(bool), false, false));

      // authentication options
      options.Add(new MySqlConnectionStringOption("user id", "uid,username,user name,user,userid", typeof(string), "", false));
      options.Add(new MySqlConnectionStringOption("password", "pwd", typeof(string), "", false));
      options.Add(new MySqlConnectionStringOption("persistsecurityinfo", "persist security info", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("encrypt", null, typeof(bool), false, true,
        delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
        {
          // just for this case, reuse the logic to translate string to bool
          sender.ValidateValue(ref value);
          MySqlTrace.LogWarning(-1, "Encrypt is now obsolete. Use Ssl Mode instead");
          msb.SetValue("Ssl Mode", ( bool)value ? MySqlSslMode.Prefered : MySqlSslMode.None);
        },
        delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender)
        {
          return msb.SslMode != MySqlSslMode.None;
        }
        ));
      options.Add(new MySqlConnectionStringOption("certificatefile", "certificate file", typeof(string), null, false));
      options.Add(new MySqlConnectionStringOption("certificatepassword", "certificate password", typeof(string), null, false));
      options.Add(new MySqlConnectionStringOption("certificatestorelocation", "certificate store location", typeof(MySqlCertificateStoreLocation), MySqlCertificateStoreLocation.None, false));
      options.Add(new MySqlConnectionStringOption("certificatethumbprint", "certificate thumb print", typeof(string), null, false));
      options.Add(new MySqlConnectionStringOption("allowpublickeyretrieval", null, typeof(bool), false, false));

      // Other properties
      options.Add(new MySqlConnectionStringOption("allowzerodatetime", "allow zero datetime", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("convertzerodatetime", "convert zero datetime", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("useusageadvisor", "use usage advisor,usage advisor", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("procedurecachesize", "procedure cache size,procedure cache,procedurecache", typeof(uint), ( uint )25, false));
      options.Add(new MySqlConnectionStringOption("useperformancemonitor", "use performance monitor,useperfmon,perfmon", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("ignoreprepare", "ignore prepare", typeof(bool), true, false));
      options.Add(new MySqlConnectionStringOption("useprocedurebodies", "use procedure bodies,procedure bodies", typeof(bool), true, true,
        delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
        {
          sender.ValidateValue(ref value);
          MySqlTrace.LogWarning(-1, "Use Procedure Bodies is now obsolete.  Use Check Parameters instead");
          msb.SetValue("checkparameters", value);
          msb.SetValue("useprocedurebodies", value);
        },
        delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender)
        {
          return (bool)msb.values["useprocedurebodies"];
        }
        ));
      options.Add(new MySqlConnectionStringOption("autoenlist", "auto enlist", typeof(bool), true, false));
      options.Add(new MySqlConnectionStringOption("respectbinaryflags", "respect binary flags", typeof(bool), true, false));
      options.Add(new MySqlConnectionStringOption("treattinyasboolean", "treat tiny as boolean", typeof(bool), true, false));
      options.Add(new MySqlConnectionStringOption("allowuservariables", "allow user variables", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("interactivesession", "interactive session,interactive", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("functionsreturnstring", "functions return string", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("useaffectedrows", "use affected rows", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("oldguids", "old guids", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("keepalive", "keep alive", typeof(uint), (uint)0, false));
      options.Add(new MySqlConnectionStringOption("sqlservermode", "sql server mode", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("tablecaching", "table cache,tablecache", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("defaulttablecacheage", "default table cache age", typeof(int), ( int ) 60, false));
      options.Add(new MySqlConnectionStringOption("checkparameters", "check parameters", typeof(bool), true, false));
      options.Add(new MySqlConnectionStringOption("replication", null, typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("exceptioninterceptors", "exception interceptors", typeof(string), null, false));
      options.Add(new MySqlConnectionStringOption("commandinterceptors", "command interceptors", typeof(string), null, false));
      options.Add(new MySqlConnectionStringOption("includesecurityasserts", "include security asserts", typeof(bool), false, false));

      // pooling options
      options.Add(new MySqlConnectionStringOption("connectionlifetime", "connection lifetime", typeof(uint), ( uint )0, false));
      options.Add(new MySqlConnectionStringOption("pooling", null, typeof(bool), true, false));
      options.Add(new MySqlConnectionStringOption("minpoolsize", "minimumpoolsize,min pool size,minimum pool size", typeof(uint), (uint)0, false));
      options.Add(new MySqlConnectionStringOption("maxpoolsize", "maximumpoolsize,max pool size,maximum pool size", typeof(uint), (uint)100, false));
      options.Add(new MySqlConnectionStringOption("connectionreset", "connection reset", typeof(bool), false, false));
      options.Add(new MySqlConnectionStringOption("cacheserverproperties", "cache server properties", typeof(bool), false, false));

      // language and charset options
      options.Add(new MySqlConnectionStringOption("characterset", "character set,charset", typeof(string), "", false));]
      options.Add(new MySqlConnectionStringOption("sslmode", "ssl mode", typeof(MySqlSslMode), MySqlSslMode.Preferred, false));
    }

    public MySqlConnectionStringBuilder()
    {
      // Populate initial values
      lock (this)
      {
        for (int i = 0; i < options.Options.Count; i++)
        {
          values[options.Options[i].Keyword] = options.Options[i].DefaultValue;
        }
      }
    }

    public MySqlConnectionStringBuilder(string connStr)
      : this()
    {
      lock (this)
      {
        ConnectionString = connStr;
      }
    }

    #region Server Properties

    /// <summary>
    /// Gets or sets the name of the server.
    /// </summary>
    /// <value>The server.</value>
    public string Server
    {
      get { return this["server"] as string; }
      set { this[ "server" ] = value; }
    }

    /// <summary>
    /// Gets or sets the name of the database the connection should 
    /// initially connect to.
    /// </summary>
    public string Database
    {
      get { return values["database"] as string; }
      set { SetValue("database", value); }
    }

    /// <summary>
    /// Gets or sets the name of the named pipe that should be used
    /// for communicating with MySQL.
    /// </summary>
    public string PipeName
    {
      get { return (string)values["pipe"]; }
      set { SetValue("pipe", value); }
    }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether this connection will allow
    /// commands to send multiple SQL statements in one execution.
    /// </summary>
    public bool AllowBatch
    {
      get { return (bool)values["allowbatch"]; }
      set { SetValue("allowbatch", value); }
    }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether logging is enabled.
    /// </summary>
    public bool Logging
    {
      get { return (bool)values["logging"]; }
      set { SetValue("logging", value); }
    }

    /// <summary>
    /// Gets or sets the base name of the shared memory objects used to 
    /// communicate with MySQL when the shared memory protocol is being used.
    /// </summary>
    public string SharedMemoryName
    {
      get { return (string)values["sharedmemoryname"]; }
      set { SetValue("sharedmemoryname", value); }
    }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether this connection uses
    /// the old style (@) parameter markers or the new (?) style.
    /// </summary>
    public bool UseOldSyntax
    {
      get { return (bool)values["useoldsyntax"]; }
      set { SetValue("useoldsyntax", value); }
    }

    /// <summary>
    /// Gets or sets the port number that is used when the socket
    /// protocol is being used.
    /// </summary>
    public uint Port
    {
      get { return (uint)values["port"]; }
      set { SetValue("port", value); }
    }

    /// <summary>
    /// Gets or sets the connection timeout.
    /// </summary>
    public uint ConnectionTimeout
    {
      get { return (uint)values["connectiontimeout"]; }

      set
      {
        // Timeout in milliseconds should not exceed maximum for 32 bit
        // signed integer (~24 days). We truncate the value if it exceeds 
        // maximum (MySqlCommand.CommandTimeout uses the same technique
        uint timeout = Math.Min(value, Int32.MaxValue / 1000);
        if (timeout != value)
        {
          MySqlTrace.LogWarning(-1, "Connection timeout value too large ("
              + value + " seconds). Changed to max. possible value" +
              +timeout + " seconds)");
        }
        SetValue("connectiontimeout", timeout);
      }
    }

    /// <summary>
    /// Gets or sets the default command timeout.
    /// </summary>
    public uint DefaultCommandTimeout
    {
      get { return (uint)values["defaultcommandtimeout"]; }
      set { SetValue("defaultcommandtimeout", value); }
    }

    #endregion

    #region Authentication Properties

    /// <summary>
    /// Gets or sets the user id that should be used to connect with.
    /// </summary>
    public string UserID
    {
      get { return (string)values["user id"]; }
      set { SetValue("user id", value); }
    }

    /// <summary>
    /// Gets or sets the password that should be used to connect with.
    /// </summary>
    public string Password
    {
      get { return (string)values["password"]; }
      set { SetValue("password", value); }
    }

    /// <summary>
    /// Gets or sets a boolean value that indicates if the password should be persisted
    /// in the connection string.
    /// </summary>
    public bool PersistSecurityInfo
    {
      get { return (bool)values["persistsecurityinfo"]; }
      set { SetValue("persistsecurityinfo", value); }
    }

    internal bool Encrypt
    {
      get { return SslMode != MySqlSslMode.None; }
      set
      {
        SetValue("Ssl Mode", value ? MySqlSslMode.Prefered : MySqlSslMode.None);
      }
    }

    public string CertificateFile
    {
      get { return (string)values["certificatefile"]; }
      set { SetValue("certificatefile", value); }
    }

    public string CertificatePassword
    {
      get { return (string)values["certificatepassword"]; }
      set { SetValue("certificatepassword", value); }
    }

    public MySqlCertificateStoreLocation CertificateStoreLocation
    {
      get { return (MySqlCertificateStoreLocation)values["certificatestorelocation"]; }
      set { SetValue("certificatestorelocation", value); }
    }

    public string CertificateThumbprint
    {
      get { return (string)values["certificatethumbprint"]; }
      set { SetValue("certificatethumbprint", value); }
    }
    
    public bool AllowPublicKeyRetrieval
    {
      get { return (bool) values["allowpublickeyretrieval"]; }
      set { SetValue("allowpublickeyretrieval", value); }
    }

    #endregion

    #region Other Properties

    /// <summary>
    /// Gets or sets a boolean value that indicates if zero date time values are supported.
    /// </summary>
    public bool AllowZeroDateTime
    {
      get { return (bool)values["allowzerodatetime"]; }
      set { SetValue("allowzerodatetime", value); }
    }

    /// <summary>
    /// Gets or sets a boolean value indicating if zero datetime values should be 
    /// converted to DateTime.MinValue.
    /// </summary>
    public bool ConvertZeroDateTime
    {
      get { return (bool)values["convertzerodatetime"]; }
      set { SetValue("convertzerodatetime", value); }
    }

    /// <summary>
    /// Gets or sets a boolean value indicating if calls to Prepare() should be ignored.
    /// </summary>
    public bool IgnorePrepare
    {
      get { return (bool)values["ignoreprepare"]; }
      set { SetValue("ignoreprepare", value); }
    }

    public bool AutoEnlist
    {
      get { return (bool)values["autoenlist"]; }
      set { SetValue("autoenlist", value); }
    }

    public bool RespectBinaryFlags
    {
      get { return (bool)values["respectbinaryflags"]; }
      set { SetValue("respectbinaryflags", value); }
    }

    public bool AllowUserVariables
    {
      get { return (bool)values["allowuservariables"]; }
      set { SetValue("allowuservariables", value); }
    }

    public bool InteractiveSession
    {
      get { return (bool)values["interactivesession"]; }
      set { SetValue("interactivesession", value); }
    }

    public bool FunctionsReturnString
    {
      get { return (bool)values["functionsreturnstring"]; }
      set { SetValue("functionsreturnstring", value); }
    }

    public bool UseAffectedRows
    {
      get { return (bool)values["useaffectedrows"]; }
      set { SetValue("useaffectedrows", value); }
    }


    public bool OldGuids
    {
      get { return (bool)values["oldguids"]; }
      set { SetValue("oldguids", value); }
    }

    public uint Keepalive
    {
      get { return (uint)values["keepalive"]; }
      set { SetValue("keepalive", value); }
    }

    public bool SqlServerMode
    {
      get { return (bool)values["sqlservermode"]; }
      set { SetValue("sqlservermode", value); }
    }

    #endregion

    #region Pooling Properties

    /// <summary>
    /// Gets or sets the lifetime of a pooled connection.
    /// </summary>
    public uint ConnectionLifeTime
    {
      get { return (uint)values["connectionlifetime"]; }
      set { SetValue("connectionlifetime", value); }
    }

    /// <summary>
    /// Gets or sets a boolean value indicating if connection pooling is enabled.
    /// </summary>
    public bool Pooling
    {
      get { return (bool)values["pooling"]; }
      set { SetValue("pooling", value); }
    }

    /// <summary>
    /// Gets the minimum connection pool size.
    /// </summary>
    public uint MinimumPoolSize
    {
      get { return (uint)values["minpoolsize"]; }
      set { SetValue("minpoolsize", value); }
    }

    /// <summary>
    /// Gets or sets the maximum connection pool setting.
    /// </summary>
    public uint MaximumPoolSize
    {
      get { return (uint)values["maxpoolsize"]; }
      set { SetValue("maxpoolsize", value); }
    }

    /// <summary>
    /// Gets or sets a boolean value indicating if the connection should be reset when retrieved
    /// from the pool.
    /// </summary>
    public bool ConnectionReset
    {
      get { return (bool)values["connectionreset"]; }
      set { SetValue("connectionreset", value); }
    }

    public bool CacheServerProperties
    {
      get { return (bool)values["cacheserverproperties"]; }
      set { SetValue("cacheserverproperties", value); }
    }

    #endregion

    #region Language and Character Set Properties

    /// <summary>
    /// Gets or sets the character set that should be used for sending queries to the server.
    /// </summary>
    public string CharacterSet
    {
      get { return (string)values["characterset"]; }
      set { SetValue("characterset", value); }
    }

    /// <summary>
    /// Indicates whether to use SSL connections and how to handle server certificate errors.
    /// </summary>
    public MySqlSslMode SslMode
    {
      get { return (MySqlSslMode)values["sslmode"]; }
      set { SetValue("sslmode", value); }
    }

    #endregion

    public override object this[string keyword]
    {
      get { MySqlConnectionStringOption opt = GetOption(keyword); return opt.Getter( this, opt ); }
      set { MySqlConnectionStringOption opt = GetOption(keyword); opt.Setter( this, opt, value); }
    }

    public override void Clear()
    {
      base.Clear();
      lock (this)
      {
        foreach (var option in options.Options)
          if (option.DefaultValue != null)
            values[option.Keyword] = option.DefaultValue;
          else
            values[option.Keyword] = null;
      }
    }

    internal void SetValue(string keyword, object value)
    {
      MySqlConnectionStringOption option = GetOption(keyword);
      option.ValidateValue(ref value);

      // remove all related keywords
      option.Clean(this);

      if (value != null)
      {
        lock (this)
        {
          // set value for the given keyword
          values[option.Keyword] = value;
          base[keyword] = value;
        }
      }
    }

    private MySqlConnectionStringOption GetOption(string key)
    {
      MySqlConnectionStringOption option = options.Get(key);
      if (option == null)
        throw new ArgumentException("Keyword not supported", key);
      else
        return option;
    }

    public override bool ContainsKey(string keyword)
    {
      MySqlConnectionStringOption option = options.Get(keyword);
      return option != null;
    }

    public override bool Remove(string keyword)
    {
      bool removed = false;
      lock (this) { removed = base.Remove(keyword); }
      if (!removed) return false;
      MySqlConnectionStringOption option = GetOption(keyword);
      lock (this)
      {
        values[option.Keyword] = option.DefaultValue;
      }
      return true;
    }

    public string GetConnectionString(bool includePass)
    {
      if (includePass) return ConnectionString;

      StringBuilder conn = new StringBuilder();
      string delimiter = "";
      foreach (string key in this.Keys)
      {
        if (String.Compare(key, "password", StringComparison.OrdinalIgnoreCase) == 0 ||
            String.Compare(key, "pwd", StringComparison.OrdinalIgnoreCase) == 0) continue;
        conn.AppendFormat(CultureInfo.CurrentCulture, "{0}{1}={2}",
            delimiter, key, this[key]);
        delimiter = ";";
      }
      return conn.ToString();
    }

    public override bool Equals(object obj)
    {
      MySqlConnectionStringBuilder other = obj as MySqlConnectionStringBuilder;
      if( obj == null )
        return false;
        
      if( this.values.Count != other.values.Count ) return false;

      foreach (KeyValuePair<string, object> kvp in this.values)
      {
        if (other.values.ContainsKey(kvp.Key))
        {
          object v = other.values[kvp.Key];
          if (v == null && kvp.Value != null) return false;
          if (kvp.Value == null && v != null) return false;
          if (kvp.Value == null && v == null) return true;
          if (!v.Equals(kvp.Value)) return false;
        }
        else
        {
          return false;
        }
      }

      return true;
    }
  }

  class MySqlConnectionStringOption
  {
    public MySqlConnectionStringOption(string keyword, string synonyms, Type baseType, object defaultValue, bool obsolete, 
      SetterDelegate setter, GetterDelegate getter )
    {
      Keyword = StringUtility.ToLowerInvariant(keyword);
      if (synonyms != null)
        Synonyms = StringUtility.ToLowerInvariant( synonyms ).Split(',');
      BaseType = baseType;
      Obsolete = obsolete;
      DefaultValue = defaultValue;
      Setter = setter;
      Getter = getter;
    }

    public MySqlConnectionStringOption(string keyword, string synonyms, Type baseType, object defaultValue, bool obsolete) :
      this(keyword, synonyms, baseType, defaultValue, obsolete,
       delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
       {
           sender.ValidateValue(ref value);
           //if ( sender.BaseType.IsEnum )
           //  msb.SetValue( sender.Keyword, Enum.Parse( sender.BaseType, ( string )value, true ));
           //else
           msb.SetValue(sender.Keyword, Convert.ChangeType(value, sender.BaseType));
       },
       delegate (MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender)
       {
         return msb.values[ sender.Keyword ];
       }
     )
    {
    }

    public string[] Synonyms { get; private set; }
    public bool Obsolete { get; private set; }
    public Type BaseType { get; private set; }
    public string Keyword { get; private set; }
    public object DefaultValue { get; private set; }
    public SetterDelegate Setter { get; private set; }
    public GetterDelegate Getter { get; private set; }

    public delegate void SetterDelegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value);
    public delegate object GetterDelegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender);

    public bool HasKeyword(string key)
    {
      if (Keyword == key) return true;
      if (Synonyms == null) return false;
      foreach (var syn in Synonyms)
        if (syn == key) return true;
      return false;
    }

    public void Clean(MySqlConnectionStringBuilder builder)
    {
      builder.Remove(Keyword);
      if (Synonyms == null) return;
      foreach (var syn in Synonyms)
        builder.Remove(syn);
    }

    public void ValidateValue(ref object value)
    {
      bool b;
      if (value == null) return;
      string typeName = BaseType.Name;
      Type valueType = value.GetType();
      if (valueType.Name == "String" ) {
        if( BaseType == valueType) return;
        else if (BaseType == typeof(bool))
        {
          if (string.Compare("yes", ( string )value, StringComparison.OrdinalIgnoreCase) == 0) value = true;
          else if (string.Compare("no", (string)value, StringComparison.OrdinalIgnoreCase) == 0) value = false;
          else if (Boolean.TryParse(value.ToString(), out b)) value = b;
          else throw new ArgumentException(String.Format("Value '{0}' is not of the correct type", value));
          return;
        }
      }
      
      if (typeName == "Boolean" && Boolean.TryParse(value.ToString(), out b)) { value = b; return; }

      UInt64 uintVal;
      if (typeName.StartsWith("UInt64") && UInt64.TryParse(value.ToString(), out uintVal)) { value = uintVal; return; }

      UInt32 uintVal32;
      if (typeName.StartsWith("UInt32") && UInt32.TryParse(value.ToString(), out uintVal32)) { value = uintVal32; return; }

      Int64 intVal;
      if (typeName.StartsWith("Int64") && Int64.TryParse(value.ToString(), out intVal)) { value = intVal;  return; }

      Int32 intVal32;
      if (typeName.StartsWith("Int32") && Int32.TryParse(value.ToString(), out intVal32)) { value = intVal32; return; }

      object objValue;
      Type baseType = BaseType.BaseType;
      if (baseType != null && baseType.Name == "Enum" && ParseEnum(value.ToString(), out objValue)) 
      {
        value = objValue;  return;
      }

      throw new ArgumentException(String.Format("Value '{0}' is not of the correct type", value));
    }

    private bool ParseEnum(string requestedValue, out object value)
    {
      value = null;
      try
      {
        value = Enum.Parse(BaseType, requestedValue, true);
        return true;
      }
      catch (ArgumentException)
      {
        return false;
      }
    }

  }

  internal class MySqlConnectionStringOptionCollection : Dictionary<string, MySqlConnectionStringOption>
  {
    List<MySqlConnectionStringOption> options;

    internal List<MySqlConnectionStringOption> Options { get { return options; } }

    internal MySqlConnectionStringOptionCollection() : base( StringComparer.OrdinalIgnoreCase )
    {
      options = new List<MySqlConnectionStringOption>();
    }

    internal void Add(MySqlConnectionStringOption option)
    {
      options.Add(option);
      // Register the option with all the keywords.
      base.Add(option.Keyword, option);
      if (option.Synonyms != null)
      {
        for (int i = 0; i < option.Synonyms.Length; i++)
          base.Add(option.Synonyms[i], option);
      }
    }

    internal MySqlConnectionStringOption Get(string keyword)
    {
      MySqlConnectionStringOption option = null;
      base.TryGetValue(keyword, out option);
      return option;
    }
  }
}
