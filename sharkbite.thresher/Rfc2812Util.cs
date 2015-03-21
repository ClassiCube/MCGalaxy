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
using System.Collections;
using System.Diagnostics;
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
		private const string Space = " ";

		internal static TraceSwitch IrcTrace = new TraceSwitch("IrcTraceSwitch", "Debug level for RFC2812 classes.");
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

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// an array of nicknames names is valid.
		/// </summary>
		/// <returns>True if the channel names are all valid.</returns>
		public static bool IsValidNicklList( string[] nicks ) 
		{
			if( nicks == null || nicks.Length == 0 ) 
			{
				return false;
			}
			foreach( string nick in nicks ) 
			{
				if( !IsValidNick( nick ) )
				{
					return false;
				}
			}
			return true;
		}	

		/// <summary>
		/// Convert a ModeAction into its RFC2812 character.
		/// </summary>
		/// <param name="action">The action enum.</param>
		/// <returns>Either '+' or '-'.</returns>
		public static char ModeActionToChar( ModeAction action ) 
		{
			return Convert.ToChar( (byte) action, CultureInfo.InvariantCulture ) ;
		}

		/// <summary>
		/// Converts the char received from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="action">Either '+' or '-'.</param>
		/// <returns>An action enum.</returns>
		public static ModeAction CharToModeAction( char action ) 
		{
			byte b = Convert.ToByte( action, CultureInfo.InvariantCulture );
			return (ModeAction) Enum.Parse( typeof( ModeAction), b.ToString( CultureInfo.InvariantCulture), false );
		}

		/// <summary>
		/// Converts a UserMode into its RFC2812 character.
		/// </summary>
		/// <param name="mode">The mode enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char UserModeToChar( UserMode mode ) 
		{
			return Convert.ToChar( (byte) mode, CultureInfo.InvariantCulture ) ;
		}

		/// <summary>
		/// Convert a string of UserModes characters to
		/// an array of UserMode enums.
		/// </summary>
		/// <param name="modes">A string of UserMode chars from the IRC server.</param>
		/// <returns>An array of UserMode enums. Charactres that are not from RFC2812 will be droppped.</returns>
		public static UserMode[] UserModesToArray( string modes ) 
		{
			ArrayList list = new ArrayList();
			for( int i = 0; i < modes.Length; i++ ) 
			{
				if( IsValidModeChar( modes[i], UserModes ) ) 
				{
					list.Add( CharToUserMode( modes[i] ));
				}
			}
			return (UserMode[]) list.ToArray( typeof(UserMode) );
		}

		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="mode">One of the IRC mode characters, e.g. 'a','i', etc...</param>
		/// <returns>An mode enum.</returns>
		public static UserMode CharToUserMode( char mode ) 
		{
			byte b = Convert.ToByte( mode, CultureInfo.InvariantCulture );
			return (UserMode) Enum.Parse( typeof( UserMode), b.ToString(CultureInfo.InvariantCulture), false );
		}

		/// <summary>
		/// Convert a string of ChannelModes characters to
		/// an array of ChannelMode enums.
		/// </summary>
		/// <param name="modes">A string of ChannelMode chars from the IRC server.</param>
		/// <returns>An array of ChannelMode enums. Charactres that are not from RFC2812 will be droppped.</returns>
		public static ChannelMode[] ChannelModesToArray( string modes ) 
		{
			ArrayList list = new ArrayList();
			for( int i = 0; i < modes.Length; i++ ) 
			{
				if( IsValidModeChar( modes[i], ChannelModes ) ) 
				{
					list.Add( CharToChannelMode( modes[i] ));
				}
			}
			return (ChannelMode[]) list.ToArray( typeof(ChannelMode) );
		}

		/// <summary>
		/// Converts a ChannelMode into its RFC2812 character.
		/// </summary>
		/// <param name="mode">The mode enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char ChannelModeToChar( ChannelMode mode ) 
		{
			return Convert.ToChar( (byte) mode, CultureInfo.InvariantCulture ) ;
		}
		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="mode">One of the IRC mode characters, e.g. 'O','i', etc...</param>
		/// <returns>An mode enum.</returns>
		public static ChannelMode CharToChannelMode( char mode ) 
		{
			byte b = Convert.ToByte( mode, CultureInfo.InvariantCulture );
			return (ChannelMode) Enum.Parse( typeof( ChannelMode), b.ToString( CultureInfo.InvariantCulture), false );
		}

		/// <summary>
		/// Converts a StatQuery enum value to its RFC2812 character.
		/// </summary>
		/// <param name="query">The query enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char StatsQueryToChar( StatsQuery query ) 
		{
			return Convert.ToChar( (byte) query, CultureInfo.InvariantCulture ) ;
		}

		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="queryType">One of the IRC stats query characters, e.g. 'u','l', etc...</param>
		/// <returns>An StatsQuery enum.</returns>
		public static StatsQuery CharToStatsQuery( char queryType ) 
		{
			byte b = Convert.ToByte( queryType, CultureInfo.InvariantCulture );
			return (StatsQuery) Enum.Parse( typeof( StatsQuery), b.ToString(CultureInfo.InvariantCulture), false );
		}



		
		private static bool IsValidModeChar( char c, string validList ) 
		{
			return validList.IndexOf( c ) != -1;
		}

		private static bool ContainsSpace( string text ) 
		{
			return text.IndexOf( Space, 0, text.Length ) != -1;
		}
	}
}
