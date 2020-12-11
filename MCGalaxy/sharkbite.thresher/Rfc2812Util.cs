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
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sharkbite.Irc
{

	/// <summary>
	/// RFC 2812 Utility methods.
	/// </summary>
	public sealed class Rfc2812Util 
	{
		// Regex that matches the standard IRC 'nick!user@host' 
		private static readonly Regex userRegex;
		// Regex that matches a legal IRC nick 
		private static readonly Regex nickRegex;
		//Regex to create a UserInfo from a string
		private static readonly Regex nameSplitterRegex;
		private const string ChannelPrefix = "#!+&";
		private const string ActionModes = "+-";
		private const string UserModes = "awiorOs";
		private const string ChannelModes = "OohvaimnqpsrtklbeI";

		// Odd chars that IRC allows in nicknames 
		internal const string Special = "\\[\\]\\`_\\^\\{\\|\\}";
		internal const string Nick = "[" + Special + "a-zA-Z][\\w\\-" + Special + "]{0,8}";
		internal const string User = "(" + Nick+ ")!([\\~\\w]+)@([\\w\\.\\-]+)";

		/// <summary>
		/// Static initializer 
		/// </summary>
		static Rfc2812Util() 
		{
			userRegex = new Regex( User );
			nickRegex = new Regex( Nick ); 
			nameSplitterRegex = new Regex("[!@]",RegexOptions.Compiled | RegexOptions.Singleline );
		}

		//Should never be instantiated
		private Rfc2812Util() {}

		/// <summary>
		/// Converts the user string sent by the IRC server
		/// into a UserInfo object.
		/// </summary>
		/// <param name="fullUserName">The user in nick!user@host form.</param>
		/// <returns>A UserInfo object.</returns>
		public static UserInfo UserInfoFromString( string fullUserName ) 
		{
			string[] parts = ParseUserInfoLine( fullUserName );
			if( parts == null ) 
			{
				return UserInfo.Empty;
			}
			else 
			{
				return new UserInfo( parts[0], parts[1], parts[2] );
			}
		}
		/// <summary>
		/// Break up an IRC user string into its component
		/// parts. 
		/// </summary>
		/// <param name="fullUserName">The user in nick!user@host form</param>
		/// <returns>A string array with the first item being nick, then user, and then host.</returns>
		public static string[] ParseUserInfoLine( string fullUserName ) 
		{
			if( fullUserName == null || fullUserName.Trim().Length == 0 ) 
			{
				return null;
			}
			Match match = nameSplitterRegex.Match( fullUserName );
			if( match.Success ) 
			{
				string[] parts = nameSplitterRegex.Split( fullUserName );
				return parts;
			}
			else 
			{
				return new string[] { fullUserName, "","" };
			}
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// an array of channel names is valid.
		/// </summary>
		/// <returns>True if the channel names are all valid.</returns>
		public static bool IsValidChannelList( string[] channels ) 
		{
			if( channels == null || channels.Length == 0 ) 
			{
				return false;
			}
			foreach( string channel in channels ) 
			{
				if( !IsValidChannelName( channel ) )
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// the channel name is valid.
		/// </summary>
		/// <returns>True if the channel name is valid.</returns>
		public static bool IsValidChannelName(string channel) 
		{
			if (channel == null || channel.Trim().Length == 0 ) 
			{
				return false;
			}

			if( Rfc2812Util.ContainsSpace(channel) ) 
			{
				return false;
			}
			if (ChannelPrefix.IndexOf( channel[0] ) != -1) 
			{
				if (channel.Length <= 50) 
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// the nickname is valid.
		/// </summary>
		/// <returns>True is the nickname is valid</returns>
		public static bool IsValidNick( string nick) 
		{
			if( nick == null || nick.Trim().Length == 0 ) 
			{
				return false;
			}
			if( Rfc2812Util.ContainsSpace( nick ) ) 
			{
				return false;
			}
			if ( nickRegex.IsMatch( nick ) ) 
			{
				return true;
			}
			return false;
		}

		private static bool ContainsSpace( string text ) {
			return text.IndexOf( ' ' ) != -1;
		}
	}
}
