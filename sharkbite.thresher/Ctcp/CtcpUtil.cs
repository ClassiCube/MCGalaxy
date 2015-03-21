/*
 * Thresher IRC client library
 * Copyright (C) 2002 Aaron Hunter <thresher@sharkbite.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 * 
 * See the gpl.txt file located in the top-level-directory of
 * the archive of this library for complete text of license.
*/

using System;
using System.Diagnostics;
using System.Globalization;

namespace Sharkbite.Irc
{
	/// <summary>
	/// Constants and utility methods to support CTCP.
	/// </summary>
	/// <remarks>The CTCP constants should be used to test incoming
	/// CTCP queries for their type and as the CTCP command
	/// for outgoing ones.</remarks>
	public sealed class CtcpUtil
	{
		/// <summary>CTCP Finger.</summary>
		public const string Finger = "FINGER";
		/// <summary>CTCP USERINFO.</summary>
		public const string UserInfo = "USERINFO";
		/// <summary>CTCP VERSION.</summary>
		public const string Version = "VERSION";
		/// <summary>CTCP SOURCE.</summary>
		public const string Source = "SOURCE";
		/// <summary>CTCP CLIENTINFO.</summary>
		public const string ClientInfo = "CLIENTINFO";
		/// <summary>CTCP ERRMSG.</summary>
		public const string ErrorMessage = "ERRMSG";
		/// <summary>CTCP PING.</summary>
		public const string Ping = "PING";
		/// <summary>CTCP TIME.</summary>
		public const string Time = "TIME";

		internal static TraceSwitch CtcpTrace = new TraceSwitch("CtcpTraceSwitch", "Debug level for CTCP classes.");

		//Should never be called so make it private
		private CtcpUtil(){}

		/// <summary>
		/// Generate a timestamp string suitable for the CTCP Ping command.
		/// </summary>
		/// <returns>The current time as a string.</returns>
		public static string CreateTimestamp() 
		{
			return DateTime.Now.ToFileTime().ToString( CultureInfo.InvariantCulture );
		}

	}
}
