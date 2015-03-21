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
using System.Globalization;


namespace Sharkbite.Irc
{
	/// <summary>
	/// The collection of information about a user 
	/// returned by a Whois query. Instances of this class
	/// are created internally.
	/// </summary>
	public sealed class WhoisInfo
	{
		internal UserInfo userInfo;
		internal string realName;
		internal string[] channels;
		internal string ircServer;
		internal string serverDescription;
		internal long idleTime;
		internal bool isOperator;

		/// <summary>
		/// Create an empty instance where the operator
		/// property defaults to false.
		/// </summary>
		internal WhoisInfo() 
		{
			isOperator = false;
		}

		/// <summary>
		/// A user's nick, logon, and hostname.
		/// </summary>
		/// <value>A UserInfo instance.</value>
		public UserInfo User
		{
			get
			{
				return userInfo;
			}
		}
		/// <summary>
		/// A user's real name.
		/// </summary>
		/// <value>A string</value>
		public string RealName
		{
			get
			{
				return realName;
			}
		}
		/// <summary>
		/// The name of IRC server.
		/// </summary>
		/// <value>The IRC server FQDN hostname string.</value>
		public string Server
		{
			get
			{
				return ircServer;
			}
		}
		/// <summary>
		/// Text describing the IRC server.
		/// </summary>
		/// <value>A string describing the IRC network this server is a member of.</value>
		public string ServerDescription
		{
			get
			{
				return serverDescription;
			}
		}
		/// <summary>
		/// User's idle time in seconds.
		/// </summary>
		/// <value>Seconds as a long.</value>
		public long IdleTime
		{
			get
			{
				return idleTime;
			}
		}
		/// <summary>
		/// Whether the user is an operator or not.
		/// </summary>
		/// <value>True if the user is an IRC operator.</value>
		public bool Operator
		{
			get
			{
				return isOperator;
			}
		}

		internal void SetChannels( string[] channels ) 
		{
			this.channels = channels;
		}

		/// <summary>
		/// An array of channel names. Names may have =,@, or + prefixed to them.
		/// </summary>
		/// <returns>A string array.</returns>
		public string[] GetChannels()
		{
			return channels;
		}

	}
}
