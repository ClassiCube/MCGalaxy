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
using System.Text.RegularExpressions;

namespace Sharkbite.Irc
{
	public delegate void ReplyEventHandler( ReplyCode code, string message );
	public delegate void ErrorMessageEventHandler( ReplyCode code, string message );
	public delegate void NickErrorEventHandler( string badNick, string reason ) ;
	public delegate void PingEventHandler( string message );
	public delegate void RegisteredEventHandler();
	public delegate void DisconnectedEventHandler();
	public delegate void PublicNoticeEventHandler( UserInfo user, string channel, string notice );
	public delegate void PrivateNoticeEventHandler( UserInfo user, string notice );
	public delegate void JoinEventHandler( UserInfo user, string channel );
	public delegate void PublicActionEventHandler( UserInfo user, string channel, string description );
	public delegate void PrivateActionEventHandler( UserInfo user, string description );
	public delegate void PublicMessageEventHandler( UserInfo user, string channel, string message );
	public delegate void PrivateMessageEventHandler( UserInfo user, string message );
	public delegate void NickEventHandler( UserInfo user, string newNick );
	public delegate void PartEventHandler( UserInfo user, string channel, string reason);
	public delegate void QuitEventHandler( UserInfo user, string reason);
	public delegate void InviteEventHandler( UserInfo user, string channel );
	public delegate void KickEventHandler( UserInfo user, string channel, string kickee, string reason );
	public delegate void NamesEventHandler( string channel, string[] nicks, bool last );
	public delegate void ChannelModeChangeEventHandler( UserInfo who, string channel );
	public delegate void KillEventHandler( UserInfo user, string nick, string reason );

	/// <summary>
	/// RFC 2812 Utility methods.
	/// </summary>
	public static class Rfc2812Util 
	{
		// Regex that matches a legal IRC nick 
		private static readonly Regex nickRegex;
		//Regex to create a UserInfo from a string
		private static readonly Regex nameSplitterRegex;
		private const string ChannelPrefix = "#!+&";

		// Odd chars that IRC allows in nicknames 
		internal const string Special = "\\[\\]\\`_\\^\\{\\|\\}";
		internal const string Nick = "[" + Special + "a-zA-Z][\\w\\-" + Special + "]{0,8}";

		/// <summary>
		/// Static initializer 
		/// </summary>
		static Rfc2812Util() 
		{
			nickRegex = new Regex( Nick ); 
			nameSplitterRegex = new Regex("[!@]",RegexOptions.Compiled | RegexOptions.Singleline );
		}

		/// <summary>
		/// Converts the user string sent by the IRC server
		/// into a UserInfo object.
		/// </summary>
		/// <param name="fullUserName">The user in nick!user@host form.</param>
		/// <returns>A UserInfo object.</returns>
		public static UserInfo UserInfoFromString( string fullUserName ) 
		{
			if( fullUserName == null || fullUserName.Trim().Length == 0 ) 
			{
				return UserInfo.Empty;
			}

			Match match = nameSplitterRegex.Match( fullUserName );
			if( match.Success ) 
			{
				string[] parts = nameSplitterRegex.Split( fullUserName );
				return new UserInfo( parts[0] );
			}
			else 
			{
				return new UserInfo( fullUserName );
			}
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

			if( ContainsSpace(channel) ) 
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
			if( ContainsSpace( nick ) ) 
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

	public class UserInfo 
	{
		/// <summary> Create a new UserInfo and set all its values. </summary>
		public UserInfo(string nick) { Nick = nick; }

		/// <summary> The IRC user's nick name. </summary>
		public string Nick;
		
		/// <summary> A singleton blank instance of UserInfo used when an instance is required
		/// by a method signature but no infomation is available, e.g. the last reply
		/// from a Who request. </summary>
		public static UserInfo Empty = new UserInfo("");

		/// <summary> A string representation of this object which shows all its values. </summary>
		public override string ToString() {
			return string.Format("Nick={0}", Nick ); 
		}
	}
}
