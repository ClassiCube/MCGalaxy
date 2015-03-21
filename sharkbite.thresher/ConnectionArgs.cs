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


namespace Sharkbite.Irc
{
	/// <summary>
	/// A collection of parameters necessary to establish
	/// an IRC connection.
	/// </summary>
	public struct ConnectionArgs
	{
		private string realName;
		private string nickName;
		private string userName;
		private string modeMask;
		private string hostname;
		private int port;
		private string serverPassword;

		/// <summary>
		/// Create a new instance initialized with the default values:
		/// TCP/IP port 6667, no server password, and user mode
		/// invisible.
		/// </summary>
		/// <param name="name">The nick, user name, and real name are 
		/// all set to this value.</param>
		/// <param name="hostname">The hostname of the IRC server.</param>
		public ConnectionArgs( string name, string hostname ) 
		{
			realName = name;
			nickName = name;
			userName = name;
			modeMask = "4";
			this.hostname = hostname;
			port = 6667;
			serverPassword = "*";
		}

		/// <summary>
		/// The IRC server hostname
		/// </summary>
		/// <value>The full hostname such as irc.gamesnet.net</value>
		public string Hostname
		{
			get
			{
				return hostname;
			}
			set
			{
				hostname = value;
			}
		}
		/// <summary>
		/// Set's the user's initial IRC mode mask. Set to 0 to recieve wallops
		/// and be invisible. Set to 4 to be invisible and not receive wallops.
		/// </summary>
		/// <value>A number mask such as 0 or 4.</value>
		public string ModeMask
		{
			get
			{
				return modeMask;
			}
			set
			{
				modeMask = value;
			}
		}
		/// <summary>
		/// The user's nick name.
		/// </summary>
		/// <value>A string which conforms to the IRC nick standard.</value>
		public string Nick
		{
			get
			{
				return nickName;
			}
			set
			{
				nickName = value;
			}
		}
		/// <summary>
		/// The TCP/IP port the IRC listens server listens on.
		/// </summary>
		/// <value> Normally should be set to 6667. </value>
		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}
		/// <summary>
		/// The user's 'real' name.
		/// </summary>
		/// <value>A short string with any legal characters.</value>
		public string RealName
		{
			get
			{
				return realName;
			}
			set
			{
				realName = value;
			}
		}
		/// <summary>
		/// The user's machine logon name.
		/// </summary>
		/// <value>A short string with any legal characters.</value>
		public string UserName
		{
			get
			{
				return userName;
			}
			set
			{
				userName = value;
			}
		}
		/// <summary>
		/// The password for this server. These are seldomly used. Set to '*' 
		/// </summary>
		/// <value>A short string with any legal characters.</value>
		public string ServerPassword
		{
			get
			{
				return serverPassword;
			}
			set
			{
				serverPassword = value;
			}
		}
	}
}
