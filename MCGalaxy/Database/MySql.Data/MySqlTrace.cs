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
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace MySql.Data.MySqlClient
{
    public sealed partial class MySqlTrace  
    {
        private static TraceSource source = new TraceSource("mysql");

        internal static TraceSource Source
        {
            get { return source; }
        }

        internal static void LogInformation(int id, string msg)
        {
            Source.TraceEvent(TraceEventType.Information, id, msg, MySqlTraceEventType.NonQuery, -1);
            Trace.TraceInformation(msg);
        }

        internal static void LogWarning(int id, string msg)
        {
            Source.TraceEvent(TraceEventType.Warning, id, msg, MySqlTraceEventType.NonQuery, -1);
            Trace.TraceWarning(msg);
        }

        internal static void LogError(int id, string msg)
        {
            Source.TraceEvent(TraceEventType.Error, id, msg, MySqlTraceEventType.NonQuery, -1);
            Trace.TraceError(msg);
        }

        internal static void TraceEvent(TraceEventType eventType,
            MySqlTraceEventType mysqlEventType, string msgFormat, params object[] args)
        {
            Source.TraceEvent(eventType, (int)mysqlEventType, msgFormat, args);
        }
    }

    public enum UsageAdvisorWarningFlags
    {
        NoIndex = 1,
        BadIndex,
        SkippedRows,
        SkippedColumns,
        FieldConversion
    }

    public enum MySqlTraceEventType : int
    {
        ConnectionOpened = 1,
        ConnectionClosed,
        QueryOpened,
        ResultOpened,
        ResultClosed,
        QueryClosed,
        StatementPrepared,
        StatementExecuted,
        StatementClosed,
        NonQuery,
        UsageAdvisorWarning,
        Warning,
        Error,
        QueryNormalized
    }
}
