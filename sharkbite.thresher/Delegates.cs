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
	/// A Notice or Private message was sent to someone
	/// whose status is away.
	/// </summary>
	/// <param name="nick">The nick of the user who is away.</param>
	/// <param name="awayMessage">An away message, if any, set by the user. </param>
	/// <seealso cref="Listener.OnAway"/>
	public delegate void AwayEventHandler( string nick, string awayMessage );
	
	/// <summary>
	/// An Invite message was successfully sent to another user. 
	/// </summary>
	/// <param name="nick">The nick of the user who was invited</param>
	/// <param name="channel">The name of the channel the user was invited to join</param>
	/// <seealso cref="Listener.OnInviteSent"/>
	public delegate void InviteSentEventHandler( string nick, string channel );
	
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
	/// A channel's topic has changed.
	/// </summary>
	/// <param name="user">Who changed the topic.</param>
	/// <param name="channel">Which channel had its topic changed.</param>
	/// <param name="newTopic">The new topic.</param>
	/// <seealso cref="Listener.OnTopicChanged"/>
	public delegate void TopicEventHandler( UserInfo user, string channel, string newTopic);
	
	/// <summary>
	/// The response to a <see cref="Sender.RequestTopic"/> command.
	/// </summary>
	/// <param name="channel">The channel who topic was requested.</param>
	/// <param name="topic">The topic.</param>
	/// <seealso cref="Listener.OnTopicRequest"/>
	public delegate void TopicRequestEventHandler( string channel, string topic);

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
	/// The response to a <see cref="Sender.List"/> request.
	/// </summary>
	/// <param name="channel">The channel name.</param>
	/// <param name="visibleNickCount">The number of visible users on that channel.</param>
	/// <param name="topic">The channel's topic.</param>
	/// <param name="last">True if this is the last reply.</param>
	/// <seealso cref="Listener.OnList"/>
	public delegate void ListEventHandler( string channel, int visibleNickCount, string topic, bool last );

	/// <summary>
	/// The response to a <see cref="Sender.Ison"/> request.
	/// </summary>
	/// <param name="nicks">If someone with this nick is on the same IRC network their nick
	/// will be returned here. Otherwise nick will be an empty string.</param>
	/// <seealso cref="Listener.OnIson"/>
	public delegate void IsonEventHandler( string nicks );

	/// <summary>
	/// The response to a <see cref="Sender.Who"/> request.
	/// </summary>
	/// <param name="user">The subject of the query</param>
	/// <param name="channel">The channel the user is on</param>
	/// <param name="ircServer">The name of the user's IRC server</param>
	/// <param name="mask">The user's mode mask</param>
	/// <param name="hopCount">Number of network hops to the user</param>
	/// <param name="realName">The user's real name</param>
	/// <param name="last">True if this is the last response</param>
	/// <seealso cref="Listener.OnWho"/>
	public delegate void WhoEventHandler( UserInfo user, string channel, string ircServer, string mask, 
	int hopCount, string realName, bool last );

	/// <summary>
	/// The response to a <see cref="Sender.Whois"/> request.
	/// </summary>
	/// <param name="whoisInfo">The data associated with the nick queried.</param>
	/// <seealso cref="Listener.OnWho"/>
	public delegate void WhoisEventHandler( WhoisInfo whoisInfo );

	/// <summary>
	/// The response to a <see cref="Sender.Whowas"/> request.
	/// </summary>
	/// <param name="user">Information on the user.</param>
	/// <param name="realName">The user's real name.</param>
	/// <param name="last">True if this is the final reply.</param>
	/// <seealso cref="Listener.OnWhowas"/>
	public delegate void WhowasEventHandler( UserInfo user, string realName, bool last );

	/// <summary>
	/// This user's mode has changed.
	/// </summary>
	/// <param name="action">Whether a mode was added or removed.</param>
	/// <param name="mode">The mode that was changed.</param>
	/// <seealso cref="Listener.OnUserModeChange"/>
	public delegate void UserModeChangeEventHandler( ModeAction action, UserMode mode );

	/// <summary>
	/// The response to a <see cref="Sender.RequestUserModes"/> command for this user.
	/// </summary>
	/// <param name="modes">The complete list of user modes as an array.</param>
	/// <seealso cref="Listener.OnUserModeRequest"/>
	public delegate void UserModeRequestEventHandler( UserMode[] modes );

	/// <summary>
	/// The response to a <see cref="Sender.RequestChannelModes"/> command.
	/// </summary>
	/// <param name="channel">The name of the channel.</param>
	/// <param name="modes">Objects which hold all the information about a channel's modes.</param>
	/// <seealso cref="Listener.OnChannelModeRequest"/>
	public delegate void ChannelModeRequestEventHandler( string channel, ChannelModeInfo[] modes);

	/// <summary>
	/// A channel's mode has changed.
	/// </summary>
	/// <param name="who">Who changed the mode.</param>
	/// <param name="channel">The name of the channel.</param>
	/// <param name="modes">Objects which hold all the information about 1 or more mode changes.</param>
	/// <seealso cref="Listener.OnChannelModeChange"/>
	public delegate void ChannelModeChangeEventHandler( UserInfo who, string channel, ChannelModeInfo[] modes );


	/// <summary>
	/// Response to a <see cref="Sender.RequestChannelList"/> command.
	/// </summary>
	/// <param name="channel">The channel name.</param>
	/// <param name="mode">What type is this a list? For example bans, invitation masks, etc..</param>
	/// <param name="item">A mask or nick (in the case of ChannelCreator).</param>
	/// <param name="last">Is this the last item. If its the last then the item paramter
	/// will be empty unless the mode is ChannelCreator.</param>
	/// <param name="who">Who set the mask (not for ChannelCreator).</param>
	/// <param name="whenSet">When was it set (not for ChannelCreator).</param>
	/// <seealso cref="Listener.OnChannelList"/>
	public delegate void ChannelListEventHandler( string channel, ChannelMode mode, string item, UserInfo who, long whenSet, bool last );


	/// <summary>
	/// Someone has replied to a Ctcp request sent by this client.
	/// </summary>
	/// <param name="who">Who sent the reply.</param>
	/// <param name="command">The Ctcp command this replies to."</param>
	/// <param name="reply">The text of the reply.</param>
	/// <see cref="CtcpListener.OnCtcpReply"/>
	public delegate void CtcpReplyEventHandler( string command, UserInfo who, string reply );

	/// <summary>
	/// Someone has sent a Ctcp request.
	/// </summary>
	/// <param name="who">Who sent the request.</param>
	/// <param name="command">The Ctcp command to send to IRC."</param>
	/// <see cref="CtcpListener.OnCtcpRequest"/>
	public delegate void CtcpRequestEventHandler( string command, UserInfo who );

	/// <summary>
	/// Someone has replied to a Ctcp ping request sent by this client.
	/// </summary>
	/// <param name="who">Who sent the reply.</param>
	/// <param name="timestamp">The timestamp originally sent in the request."</param>
	/// <see cref="CtcpListener.OnCtcpPingReply"/>
	public delegate void CtcpPingReplyEventHandler( UserInfo who, string timestamp );

	/// <summary>
	/// Someone has sent a Ctcp Ping request.
	/// </summary>
	/// <param name="who">Who sent the request.</param>
	/// <param name="timestamp">The timestamp which should be sent 
	/// back."</param>
	/// <see cref="CtcpListener.OnCtcpPingRequest"/>
	public delegate void CtcpPingRequestEventHandler( UserInfo who, string timestamp );
	
	/// <summary>
	/// Someone has requested a DCC chat session.
	/// </summary>
	/// <param name="dccUserInfo">The collection of information about the remote user.</param>
	/// <see cref="DccListener.OnDccChatRequest"/>
	public delegate void DccChatRequestEventHandler( DccUserInfo dccUserInfo );
	
	/// <summary>
	/// A DCC chat session has been opened with a remote user.
	/// </summary>
	/// <param name="session">Which session is the source of the event.</param>
	/// <see cref="DccChatSession.OnChatSessionOpened"/>
	public delegate void ChatSessionOpenedEventHandler( DccChatSession session );

	/// <summary>
	/// A DCC chat session has been closed.
	/// </summary>
	/// <param name="session">Which session is the source of the event.</param>
	/// <see cref="DccChatSession.OnChatSessionClosed"/>
	public delegate void ChatSessionClosedEventHandler( DccChatSession session );

	/// <summary>
	/// A DCC chat message was received from the remote user.
	/// </summary>
	/// <param name="session">Which session is the source of the event.</param>
	/// <param name="message">A string message.</param>
	/// <see cref="DccChatSession.OnChatMessageReceived"/>
	public delegate void ChatMessageReceivedEventHandler( DccChatSession session , string message );

	/// <summary>
	/// When trying to initiate a DCC chat request the remote user
	/// did not respond within the alotted time.
	/// </summary>
	/// <param name="session">Which session is the source of the event.</param>
	/// <see cref="DccChatSession.OnChatRequestTimeout"/>
	public delegate void ChatRequestTimeoutEventHandler( DccChatSession session );

	/// <summary>
	/// Another user has offered to send a file.
	/// </summary>
	/// <param name="dccUserInfo">The collection of information about the remote user.</param>
	/// <param name="fileName">The name of the file to be sent.</param>
	/// <param name="size">The size in bytes of the offered file.</param>
	/// <param name="turbo">True if the sender will use send-ahead mode.</param>
	/// <see cref="DccListener.OnDccSendRequest"/>
	public delegate void DccSendRequestEventHandler( DccUserInfo dccUserInfo, string fileName, int size, bool turbo );

	/// <summary>
	/// There has been no activity in this session for the timeout period. The 
	/// session is automatically closed and this event is raised. 
	/// </summary>
	/// <param name="session">The session in which the timeout occurred.</param> 
	/// <see cref="DccFileSession.OnFileTransferTimeout"/>
	public delegate void FileTransferTimeoutEventHandler( DccFileSession session ); 		
	
	/// <summary>
	/// The file transfer connection has been successfully opened and the data
	/// transfer has begun.
	/// </summary>
	/// <param name="session">The session in which the transfer has started.</param> 
	/// <see cref="DccFileSession.OnFileTransferStarted"/>
	public delegate void FileTransferStartedEventHandler( DccFileSession session ); 

	/// <summary>
	/// Something happened to stop the transfer before it was completed. Normally
	/// this will be due to one of the sides canceling the transfer.
	/// </summary>
	/// <param name="session">The session in which the transfer was interrupted.</param> 
	/// <see cref="DccFileSession.OnFileTransferInterrupted"/>
	public delegate void FileTransferInterruptedEventHandler( DccFileSession session ); 

	/// <summary>
	/// A file was succefully transfered.
	/// </summary>
	/// <param name="session">The session in which the transfer was successfully completed.</param> 
	/// <see cref="DccFileSession.OnFileTransferCompleted"/>
	public delegate void FileTransferCompletedEventHandler( DccFileSession session ); 

	/// <summary>
	/// Called for each successful data block transfer. This allows the developer
	/// to show a transfer progress display of some kind.
	/// </summary>
	/// <param name="session">The session in which data was transfered.</param> 
	/// <param name="bytesSent">The number of bytes sent in this block. The DccFileSession
	/// contains the cumulative number of bytes sent/received and the total number
	/// the will be processed.</param> 
	/// <see cref="DccFileSession.OnFileTransferProgress"/>
	public delegate void FileTransferProgressEventHandler( DccFileSession session , int bytesSent );

	/// <summary>
	/// A remote user has requested a file. To respond
	/// use <see cref="DccFileSession.Send"/> with the relevant information.
	/// </summary>
	/// <param name="dccUserInfo">The requestor's information.</param>
	/// <param name="fileName">The name of the requested file.</param>
	/// <param name="turbo">True to use send-ahead mode for transfers.</param>
	/// <see cref="DccListener.OnDccGetRequest"/>
	public delegate void DccGetRequestEventHandler( DccUserInfo dccUserInfo, string fileName, bool turbo);

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
	/// The response to a <see cref="Sender.Version"/> request.
	/// </summary>
	/// <param name="versionInfo">The information string in the form 
	/// IRC: [version].[debuglevel] [server] :[comments]</param>
	/// <seealso cref="Listener.OnVersion"/>
	public delegate void VersionEventHandler( string versionInfo );

	/// <summary>
	/// The server's "Message of the Day" if any.
	/// </summary>
	/// <param name="message">An information string.</param>
	/// <param name="last">True if this is the last in the set of messages.</param>
	/// <seealso cref="Listener.OnMotd"/>
	public delegate void MotdEventHandler( string message, bool last );

	/// <summary>
	/// The response to a <see cref="Sender.Time"/> request.
	/// </summary>
	/// <param name="time">The name of the server and
	/// its local time</param>
	/// <seealso cref="Listener.OnTime"/>
	public delegate void TimeEventHandler( string time );

	/// <summary>
	/// The response to an <see cref="Sender.Info"/> request.
	/// </summary>
	/// <param name="message">An information string.</param>
	/// <param name="last">True if this is the last in the set of messages.</param>
	/// <seealso cref="Listener.OnInfo"/>
	public delegate void InfoEventHandler( string message, bool last );

	/// <summary>
	/// The response to an <see cref="Sender.Admin"/> request.
	/// </summary>
	/// <param name="message">An information string.</param>
	/// <seealso cref="Listener.OnAdmin"/>
	public delegate void AdminEventHandler( string message );

	/// <summary>
	/// The response to a <see cref="Sender.Lusers"/> request.
	/// </summary>
	/// <param name="message">An information string.</param>
	/// <seealso cref="Listener.OnLusers"/>
	public delegate void LusersEventHandler( string message );

	/// <summary>
	/// The response to a <see cref="Sender.Links"/> request.
	/// </summary>
	/// <param name="mask">The hostname as it appears in IRC queries.</param>
	/// <param name="hostname">The actual hostname.</param>
	/// <param name="hopCount">The number of hops from this server to the target server.</param>
	/// <param name="serverInfo">Information about the server, usually the network name.</param>
	/// <param name="done">True if this is the last message in the series. If it is the
	/// last it will not contain any server information.</param>
	/// <seealso cref="Listener.OnLinks"/>
	public delegate void LinksEventHandler( string mask, string hostname, int hopCount, string serverInfo, bool done );

	/// <summary>
	/// The response to a <see cref="Sender.Stats"/> request.
	/// </summary>
	/// <param name="queryType">What kind of query this is in response to.</param>
	/// <param name="message">The actual response.</param>
	/// <param name="done">True if this is the last message in the series.</param>
	/// <seealso cref="Listener.OnStats"/>
	public delegate void StatsEventHandler( StatsQuery queryType, string message, bool done );

	/// <summary>
	/// Someone was disconnected from the server via a Kill.
	/// </summary>
	/// <param name="user">Which Operator send teh Kill command</param>
	/// <param name="nick">Who was Killed.</param>
	/// <param name="reason">Why the nick was disconnected.</param>
	/// <seealso cref="Listener.OnKill"/>
	public delegate void KillEventHandler( UserInfo user, string nick, string reason );
}
