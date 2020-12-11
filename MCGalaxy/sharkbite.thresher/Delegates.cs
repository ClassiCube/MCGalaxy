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

using System.Net;

namespace Sharkbite.Irc
{

	/// <summary>
	/// Messages that are not handled by other events and are not errors.
	/// </summary>
	/// <param name="code">The RFC 2812 numeric code.</param>
	/// <param name="message">The unparsed message text.</param>
	/// <seealso cref="Listener.OnReply"/>
	public delegate void ReplyEventHandler( ReplyCode code, string message );
	
	/// <summary>
	/// Error messages from the IRC server.
	/// </summary>
	/// <param name="code">The RFC 2812 or custom numeric code.</param>
	/// <param name="message">The error message text.</param>
	/// <seealso cref="Listener.OnError"/>
	public delegate void ErrorMessageEventHandler( ReplyCode code, string message );
	
	/// <summary>
	/// Called when a nick change fails.
	/// </summary>
	/// <remarks>
	/// <para>This method can be called under 2 conditions:
	/// It can arise when the user is already 
	/// registered with the IRC server and is trying change his nick.
	/// Or when the user is trying to register for the first time with 
	/// the IRC server and it fails.</para>
	/// <para>Note that if the later arises then you will have to manually
	/// complete the regsitration process.</para> 
	/// </remarks>
	/// <param name="badNick">The nick which caused the problem</param>
	/// <param name="reason">A message explaining the error</param>
	/// <seealso cref="Listener.OnNickError"/>
	public delegate void NickErrorEventHandler( string badNick, string reason ) ;
	
	/// <summary>
	/// Called when a server sends a keep-alive Ping.
	/// </summary>
	/// <param name="message">The message that the IRC server wants echoed back to it.</param>
	/// <seealso cref="Listener.OnPing"/>
	public delegate void PingEventHandler( string message );
	
	/// <summary>
	/// Connection with IRC server is open and registered.
	/// </summary>
	/// <seealso cref="Listener.OnRegistered"/>
	public delegate void RegisteredEventHandler();
	
	/// <summary>
	/// This connection is about to be closed 
	/// </summary>
	/// <seealso cref="Listener.OnDisconnecting"/>
	public delegate void DisconnectingEventHandler();
	
	/// <summary>
	/// This connection has been closed 
	/// </summary>
	/// <seealso cref="Listener.OnDisconnected"/>
	public delegate void DisconnectedEventHandler();
		
	/// <summary>
	/// A Notice type message was sent to a channel.
	/// </summary>
	/// <param name="user">The user who sent the message.</param>
	/// <param name="channel">The target channel.</param>
	/// <param name="notice">A message.</param>
	/// <seealso cref="Listener.OnPublicNotice"/>
	public delegate void PublicNoticeEventHandler( UserInfo user, string channel, string notice );
	
	/// <summary>
	/// A private Notice type message was sent to the user.
	/// </summary>
	/// <param name="user">The user who sent the message.</param>
	/// <param name="notice">A message.</param>
	/// <seealso cref="Listener.OnPrivateNotice"/>
	public delegate void PrivateNoticeEventHandler( UserInfo user, string notice );

	/// <summary>
	/// Someone has joined a channel.
	/// </summary>
	/// <param name="user">Who joined.</param>
	/// <param name="channel">The channel name.</param>
	/// <seealso cref="Listener.OnJoin"/>
	public delegate void JoinEventHandler( UserInfo user, string channel );
	
	/// <summary>
	/// An action message was sent to a channel.
	/// </summary>
	/// <param name="user">The user who expresses the action.</param>
	/// <param name="channel">The target channel.</param>
	/// <param name="description">An action.</param>
	/// <seealso cref="Listener.OnAction"/>
	public delegate void ActionEventHandler( UserInfo user, string channel, string description );

	/// <summary>
	/// A private action message was sent to the user.
	/// </summary>
	/// <param name="user">The user who expresses the action.</param>
	/// <param name="description">An action.</param>
	/// <seealso cref="Listener.OnPrivateAction"/>
	public delegate void PrivateActionEventHandler( UserInfo user, string description );

	/// <summary>
	/// A public message was sent to a channel.
	/// </summary>
	/// <param name="user">The user who sent the message.</param>
	/// <param name="channel">The taregt channel.</param>
	/// <param name="message">A message.</param>
	/// <seealso cref="Listener.OnPublic"/>
	public delegate void PublicMessageEventHandler( UserInfo user, string channel, string message );
	
	/// <summary>
	/// A user changed his nickname.
	/// </summary>
	/// <param name="user">The user who is changing his nick.</param>
	/// <param name="newNick">The new nickname.</param>
	/// <seealso cref="Listener.OnNick"/>
	public delegate void NickEventHandler( UserInfo user, string newNick );
	
	/// <summary>
	/// A private message was sent to the user.
	/// </summary>
	/// <param name="user">Who sent the message.</param>
	/// <param name="message">The message.</param>
	/// <seealso cref="Listener.OnPrivate"/>
	public delegate void PrivateMessageEventHandler( UserInfo user, string message );

	/// <summary>
	/// Someone has left a channel. 
	/// </summary>
	/// <param name="user">The user who left.</param>
	/// <param name="channel">The channel he left.</param>
	/// <param name="reason">The reason for leaving.</param>
	/// <seealso cref="Listener.OnPart"/>
	public delegate void PartEventHandler( UserInfo user, string channel, string reason);

	/// <summary>
	/// Someone has quit IRC.
	/// </summary>
	/// <param name="user">The user who quit.</param>
	/// <param name="reason">A goodbye message.</param>
	/// <seealso cref="Listener.OnQuit"/>
	public delegate void QuitEventHandler( UserInfo user, string reason);
	
	/// <summary>
	/// The user has been invited to a channel.
	/// </summary>
	/// <param name="user">Who sent the invite.</param>
	/// <param name="channel">The target channel.</param>
	/// <seealso cref="Listener.OnInvite"/>
	public delegate void InviteEventHandler( UserInfo user, string channel );
	
	/// <summary>
	/// Someone has been kicked from a channel. 
	/// </summary>
	/// <param name="user">Who did the kicking.</param>
	/// <param name="channel">The channel that the person was kicked from.</param>
	/// <param name="kickee">Who was kicked.</param>
	/// <param name="reason">Why the person was kicked.</param>
	/// <seealso cref="Listener.OnKick"/>
	public delegate void KickEventHandler( UserInfo user, string channel, string kickee, string reason );
	
	/// <summary>
	/// The response to a <see cref="Sender.Names"/> request.
	/// </summary>
	/// <param name="channel">The channel the user is on. "@" is used for secret channels, "*" for private
	/// channels, and "=" for public channels.</param>
	/// <param name="nicks">A list of nicks on the channel. If this is the last reply
	/// then it will be empty. Nicks prefixed with a '@' are channel
	/// operators. Nicks prefixed with a '+' have voice privileges on
	/// a moderated channel, i.e. they are allowed to send public messages.</param>
	/// <param name="last">True if this is the last names reply.</param>
	/// <seealso cref="Listener.OnNames"/>
	public delegate void NamesEventHandler( string channel, string[] nicks, bool last );

	/// <summary>
	/// A channel's mode has changed.
	/// </summary>
	/// <param name="who">Who changed the mode.</param>
	/// <param name="channel">The name of the channel.</param>
	/// <seealso cref="Listener.OnChannelModeChange"/>
	public delegate void ChannelModeChangeEventHandler( UserInfo who, string channel );

	
	/// <summary>
	/// The full unparsed text messages received from the IRC server. It
	/// includes all messages received except for those exchanged during a DCC chat.
	/// </summary>
	/// <param name="message">The text received.</param>
	/// <see cref="Connection.OnRawMessageReceived"/>
	public delegate void RawMessageReceivedEventHandler( string message );

	/// <summary>
	/// The full unparsed text messages sent to the IRC server. It
	/// includes all messages sent except for those exchanged during a DCC chat.
	/// </summary>
	/// <param name="message">The text sent.</param>
	/// <see cref="Connection.OnRawMessageSent"/>
	public delegate void RawMessageSentEventHandler( string message );

	/// <summary>
	/// Someone was disconnected from the server via a Kill.
	/// </summary>
	/// <param name="user">Which Operator send teh Kill command</param>
	/// <param name="nick">Who was Killed.</param>
	/// <param name="reason">Why the nick was disconnected.</param>
	/// <seealso cref="Listener.OnKill"/>
	public delegate void KillEventHandler( UserInfo user, string nick, string reason );
}
