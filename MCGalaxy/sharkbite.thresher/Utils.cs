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
