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
	/// Whether a mode has been added or removed.
	/// </summary>
	public enum ModeAction: int
	{
		/// <summary>
		/// Mode added
		/// </summary>
		Add = 43 , //+

		/// <summary>
		/// Mode removed
		/// </summary>
		Remove = 45 //-
	};

	/// <summary>
	/// The possible user modes.
	/// </summary>
	public enum UserMode: int
	{
		/// <summary>
		/// User is away
		/// </summary>
		Away = 97, //a

		/// <summary>
		/// User will receive server status messages
		/// </summary>
		Wallops =119, //w

		/// <summary>
		/// User cannot be seen by certain IRC queries
		/// </summary>
		Invisible =105, //i

		/// <summary>
		/// The user is an IRC operator (IRCOP)
		/// </summary>
		Operator =111, //o

		/// <summary>
		/// Not used
		/// </summary>
		Restricted =114, //r

		/// <summary>
		/// User is a channel operator/owner
		/// </summary>
		LocalOperator = 79, //O

		/// <summary>
		/// Marks a user for receipt of server notices
		/// </summary>
		ServerNotices = 115 //s
		
	};

	/// <summary>
	/// Possible channel modes.
	/// </summary>
	public enum ChannelMode: int
	{
		/// <summary>
		/// The mode 'O' is only used in conjunction with "safe channels" and
		/// SHALL NOT be manipulated by users. Servers use it to give the user
		/// creating the channel the status of "channel creator".
		/// </summary>
		ChannelCreator = 79, //O
		
		/// <summary>
		/// The mode 'o' is used to toggle the operator status of a channel
		/// member. 
		/// 	
		/// Should include a nick or user mask parameter.
		/// </summary>
		ChannelOperator = 111, //o

		/// <summary>
		/// The mode 'h' is used to toggle the non-standard but common half-operator status of a channel
		/// member. 
		/// 	
		/// Should include a nick or user mask parameter.
		/// </summary>
		HalfChannelOperator = 104, //h
		
		/// <summary>
		/// The mode 'v' is used to give and take voice privilege to/from a
		/// channel member. Users with this privilege can talk on moderated
		/// channels. 
		/// 	
		/// Should include a nick or user mask parameter.
		/// </summary>
		Voice = 118, //v

		/// <summary>
		/// The channel flag 'a' defines an anonymous channel. This means that
		/// when a message sent to the channel is sent by the server to users,
		/// and the origin is a user, then it MUST be masked. 
		/// 	
		/// No param required.
		/// </summary>
		Anonymous = 97, //a

		/// <summary>
		/// When the channel flag 'i' is set, new members are only accepted if
		/// their mask matches Invite-list or they have been
		/// invited by a channel operator. This flag also restricts the usage of
		/// the INVITE command to channel operators.
		/// 
		/// No param required.
		/// </summary>
		InviteOnly = 105, //i

		/// <summary>
		/// The channel flag 'm' is used to control who may speak on a channel.
		/// When it is set, only channel operators, and members who have been
		/// given the voice privilege may send messages to the channel.
		/// 
		/// No param required.
		/// </summary>
		Moderated = 109, //m

		/// <summary>
		/// When the channel flag 'n' is set, only channel members MAY send
		/// messages to the channel.
		/// 	
		/// No param required
		/// </summary>
		NoOutside = 110, //n

		/// <summary>
		/// The channel flag 'q' is for use by servers only. When set, it
		/// restricts the type of data sent to users about the channel
		/// operations: other user joins, parts and nick changes are not sent.
		/// From a user's point of view, the channel contains only one user.
		/// </summary>
		Quiet = 113, //q

		/// <summary>
		/// The channel flag 'p' is used to mark a channel "private" and the
		/// channel flag 's' to mark a channel "secret". Both properties are
		/// similar and conceal the existence of the channel from other users.
		/// 	
		/// No param required.
		/// </summary>
		Private = 112, //p

		/// <summary>
		/// When a channel is "secret", in addition to the restrictions of Private, the
		/// server will act as if the channel does not exist for queries like the
		/// TOPIC, LIST, NAMES commands. Note that there is one exception to
		/// this rule: servers will correctly reply to the MODE command.
		/// Finally, secret channels are not accounted for in the reply to the
		/// LUSERS command when the mask parameter is specified.
		/// 	
		/// No param required.
		/// </summary>
		Secret = 115, //s

		/// <summary>
		/// The channel flag 'r' is only available on channels which name begins
		/// with the character '!' and MAY only be toggled by the "channel
		/// creator".
		/// 	
		/// No param required.
		/// </summary>
		ServerReop = 114, //r

		/// <summary>
		/// The channel flag 't' is used to restrict the usage of the TOPIC
		/// command to channel operators.
		/// 
		/// No param required.
		/// </summary>
		TopicSettable = 116, //t

		/// <summary>
		/// When a channel key is set (by using the mode 'k'), servers MUST
		/// reject their local users request to join the channel unless this key
		/// is given.
		/// 	
		/// Param is the channel password.
		/// </summary>
		Password = 107, //k

		/// <summary>
		/// A user limit may be set on channels by using the channel flag 'l'.
		/// When the limit is reached, servers MUST forbid their local users to
		/// join the channel.
		/// 	
		/// Param is a whole number indicating the max number of users.
		/// </summary>
		UserLimit = 108, //l

		/// <summary>
		/// When a user requests to join a channel, his local server checks if
		/// the user's address matches any of the ban masks set for the channel.
		/// If a match is found, the user request is denied unless the address
		/// also matches an exception mask set for the channel.
		/// 
		/// Param is a nick or user mask.
		/// </summary>
		Ban = 98, //b


		/// <summary>
		/// An error ocurred.
		/// </summary>
		Exception = 101, //e

		/// <summary>
		/// For channels which have the invite-only flag set, users whose 
		/// address matches an invitation mask set for the channel are 
		/// allowed to join the channel without any invitation.
		/// 
		/// Param is a nick or user mask.
		/// </summary>
		Invitation = 73//I
	};


	/// <summary>
	/// The possible stats message query parameters.
	/// </summary>
	public enum StatsQuery: int
	{
		/// <summary>
		/// A list of server connections.
		/// </summary>
		Connections = 108, //l

		/// <summary>
		/// The usage count for each of command supported
		/// by the server.
		/// </summary>
		CommandUsage = 109, //m

		/// <summary>
		/// The list of IRC operators.
		/// </summary>
		Operators = 111, //o

		/// <summary>
		/// The server uptime.
		/// </summary>
		Uptime = 117, //u
	};

	/// <summary>
	/// All recognized mIRC colors.
	/// </summary>
	public enum MircColor
	{
		White = 0,
		Black = 1,
		Blue = 2,
		Green = 3,
		LightRed = 4,
		Brown = 5,
		Purple = 6,
		Orange = 7,
		Yellow = 8,
		LightGreen = 9,
		Cyan = 10,
		LightCyan = 11,
		LightBlue = 12,
		Pink = 13,
		Grey = 14,
		LightGrey = 15,
		Transparent = 99
	};
}
