/*
 * Thresher IRC client library
 * Copyright (C) 2002 Aaron Hunter <thresher@sharkbite.org>
 *
 * This program is free software, you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 * 
 * See the gpl.txt file located in the top-level-directory of
 * the archive of this library for complete text of license.
*/

namespace Sharkbite.Irc
{


	/// <summary>
	/// Numeric message codes taken from RFC 2812
	/// </summary>

	public enum ReplyCode: int
	{
		/// <summary>
		/// IRC: Welcome to the Internet Relay Networ [nick]![user]@[host]
		/// 
		/// </summary>
		RPL_WELCOME = 001,

		/// <summary>
		/// IRC: ( "=" / "*" / "@" ) [channel :[ "@" / "+" ] [nick] *( " " [ "@" / "+" ] [nick] 
		/// 
		/// Description: "@" is used for secret channels, "*" for private
		/// channels, and "=" for others (public channels).
		/// 
		/// </summary>
		RPL_NAMREPLY = 353,

		/// <summary>
		/// IRC: [channel] :End of NAMES list
		/// 
		/// Description: To reply to a NAMES message, a reply pair consisting
		/// of RPL_NAMREPLY and RPL_ENDOFNAMES is sent by the
		/// server back to the client. If there is no channel
		/// found as in the query, then only RPL_ENDOFNAMES is
		/// returned. The exception to this is when a NAMES
		/// message is sent with no parameters and all visible
		/// channels and contents are sent back in a series of
		/// RPL_NAMEREPLY messages with a RPL_ENDOFNAMES to mark
		/// the end.
		/// 
		/// </summary>
		RPL_ENDOFNAMES = 366,

		/// <summary>
		/// IRC: You are service [servicename]
		/// 
		/// Description: Sent by the server to a service upon successful
		/// registration.
		/// 
		/// </summary>
		RPL_YOURESERVICE = 383,

		/// <summary>
		/// IRC: [command] :Please wait a while and try again.
		/// 
		/// Description: When a server drops a command without processing it,
		/// it MUST use the reply RPL_TRYAGAIN to inform the
		/// originating client.
		/// 
		/// </summary>
		RPL_TRYAGAIN = 263,

		/// <summary>
		/// IRC: [nickname] :No such nick/channel
		/// 
		/// Description: Used to indicate the nickname parameter supplied to a
		/// command is currently unused.
		/// 
		/// </summary>
		ERR_NOSUCHNICK = 401,

		/// <summary>
		/// IRC: [nick] :Nickname is already in use
		/// 
		/// Description: Returned when a NICK message is processed that results
		/// in an attempt to change to a currently existing
		/// nickname.
		/// 
		/// </summary>
		ERR_NICKNAMEINUSE = 433,

		/// <summary>
		/// IRC: [nick] :Nickname collision KILL from [user]@[host]
		/// 
		/// Description: Returned by a server to a client when it detects a
		/// nickname collision (registered of a NICK that
		/// already exists by another server).
		/// 
		/// </summary>
		ERR_NICKCOLLISION = 436,

		/// <summary>
		/// IRC: :Cannot change mode for other users
		/// 
		/// Description: Error sent to any user trying to view or change the
		/// user mode for a user other than themselves.
		/// 
		/// </summary>
		ERR_USERSDONTMATCH = 502,

		/// <summary>
		/// The IRC server sent an 'ERROR' message for some
		/// reason.
		/// </summary>
		IrcServerError = 1001,

		/// <summary>
		/// A message from the IRC server that cannot be parsed. This may be because
		/// the message is intended to cause problems, it may be an unsupported protocol
		/// such as DCC Voice, or it may be that the Thresher parser simply cannot understand
		/// it.
		/// </summary>
		UnparseableMessage = 1003,

	}

}
