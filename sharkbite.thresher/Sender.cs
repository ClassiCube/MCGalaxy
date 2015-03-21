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
using System.Text;
using System.IO;

namespace Sharkbite.Irc
{
	/// <summary>
	/// This class is used to send all the IRC commands except for CTCP and DCC
	/// messages. Instances of this class are retrieved as properties of the Connection
	/// object. All methods in this class are thread safe.
	/// </summary>
	/// <remarks>
	/// <para>Due to the asynchronous nature of IRC, none of these commands 
	/// have a return value. To get that value (or possibly an error) the client must
	/// handle the corresponding event. For example, to check if a user is online
	/// the client would send <see cref="Sender.Ison"/> then check the value of the 
	/// <see cref="Listener.OnIson"/> event to receive the answer.</para>
	/// <para>When a command can return an error, the possible error replies
	/// are listed. An error message will be sent via the <see cref="Listener.OnError"/> event
	/// with one of the listed error codes as a parameter. When checking for these 
	/// errors use the constants from <see cref="ReplyCode"/>.
	/// </para> 
	/// <para>The maximum length of any command string sent to the 
	/// server is 512 characters.</para>
	/// </remarks>
	/// <example><code>
	/// //Create a Connection object which will automatically create its own Sender
	/// Connection connection = new Connection( args, false, false );	
	/// //Send commands using the Connection object and its Sender instance.
	/// //No need to keep a separate reference to the Sender object
	/// connection.Sender.PublicMessage("#thresher", "hello");
	/// </code></example>
	public class Sender : CommandBuilder
	{
		/// <summary>
		/// Create a new Sender for a specific connection.
		/// </summary>
		internal Sender(Connection connection ) : base( connection) {}

		private bool IsEmpty( string aString ) 
		{
			return aString == null || aString.Trim().Length == 0;
		}

		/// <summary>
		/// Truncate parameters which cause a command line
		/// to be too long.
		/// </summary>
		/// <param name="parameter">The command parameter</param>
		/// <param name="commandLength">The length of the command plus whitespace</param>
		/// <returns></returns>
		private string Truncate( string parameter, int commandLength ) 
		{
			int max = MAX_COMMAND_SIZE - commandLength;
			if (parameter.Length > max ) 
			{
				return parameter.Substring(0, max);
			}
			else 
			{
				return parameter;
			}
		}

		private bool TooLong( StringBuilder buffer ) 
		{
			//2 for CR LF
			return (buffer.Length + 2) > MAX_COMMAND_SIZE;
		}

		/// <summary>
		/// The USER command is only used at the beginning of Connection to specify
		/// the username, hostname and realname of a new user.
		/// </summary>
		/// <param name="args">The user Connection data</param>
		internal void User( ConnectionArgs args ) 
		{
			lock( this )
			{
				Buffer.Append("USER");
				Buffer.Append(SPACE);
				Buffer.Append( args.UserName );
				Buffer.Append(SPACE);
				Buffer.Append( args.ModeMask );
				Buffer.Append(SPACE);
				Buffer.Append('*');
				Buffer.Append(SPACE);
				Buffer.Append( args.RealName );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// A client session is terminated with a quit message.
		/// </summary>
		/// <remarks> 
		/// <para>The server
		/// acknowledges this by sending an ERROR message to the client. 
		/// </para>
		/// <para>Before closing the Connection with the IRC server this method
		/// will call <c>Listener.beforeDisconnect()</c> and after
		/// the Connection is closed it will call <c> Listener.OnDisconnect()</c>
		/// </para>
		/// </remarks>
		/// <param name="reason">Reason for quitting.</param>
		internal void Quit(string reason) 
		{
			lock( this ) 
			{
				Buffer.Append("QUIT");
				if( IsEmpty( reason ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Quite reason cannot be null or empty.");
				}
				Buffer.Append(SPACE_COLON);
				if (reason.Length > 502) 
				{
					reason = reason.Substring(0, 504);
				}
				Buffer.Append(reason);
				Connection.SendCommand( Buffer );
			}
		}
			/// <summary>
		/// A PONG message is a reply to server PING message. Only called by
		/// the Connection object to keep the Connection alive.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// <list type="bullet">
		/// 			<item><description>ERR_NOORIGIN</description></item>
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// </list>
		/// </remarks>
		/// <param name="message">The text sent by the IRC server in the PING message.</param>
		internal void Pong(string message) 
		{
			//Not synchronized because it will only be called during on OnPing event by
			//the dispatch thread
			Buffer.Append("PONG");
			Buffer.Append(SPACE);
			Buffer.Append(message);
			Connection.SendAutomaticReply( Buffer );
		}
		/// <summary>
		/// The PASS command is used to set a 'Connection password'. 
		/// </summary>
		/// <remarks>
		/// The optional password can and MUST be set before any attempt to register
		/// the Connection is made. Currently this requires that user send a
		/// PASS command before sending the NICK/USER combination.
		/// </remarks>
		internal void Pass( string password ) 
		{
			lock( this )
			{
				Buffer.Append("PASS");
				Buffer.Append(SPACE);
				Buffer.Append(password);
				Connection.SendCommand( Buffer );
			}
		}	
		/// <summary>
		/// User registration consists of 3 commands:
		/// 1. PASS
		/// 2. NICK
		/// 3. USER
		/// Pass will rarely fail but the proposed Nick might already be taken in
		/// which case the client will have to register by manually calling Nick
		/// and User.
		/// </summary>
		internal void RegisterConnection( ConnectionArgs args ) 
		{
			Pass( args.ServerPassword );
			Nick( args.Nick );
			User( args );
		}

		/// <summary>
		/// Join the specified channel. 
		/// </summary>
		/// <remarks>
		/// <para>Once a user has joined a channel, he receives information about
		/// all commands his server receives affecting the channel. This
		/// includes JOIN, MODE, KICK, PART, QUIT and of course PRIVMSG/NOTICE.
		/// This allows channel members to keep track of the other channel
		/// members, as well as channel modes.</para>
		/// <para>If a JOIN is successful, the user receives a JOIN message as
		/// confirmation and is then sent the channel's topic ( <see cref="Listener.OnTopicRequest"/> and
		/// the list of users who are on the channel ( <see cref="Listener.OnNames"/> ), which
		/// MUST include the user joining.</para>
		/// 
		/// Possible Errors
		/// <list type="bullet">
		/// 	<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 	<item><description>ERR_BANNEDFROMCHAN</description></item>
		/// 	<item><description>ERR_INVITEONLYCHAN</description></item>
		/// 	<item><description>ERR_BADCHANNELKEY</description></item>
		/// 	<item><description>ERR_CHANNELISFULL</description></item>
		/// 	<item><description>ERR_BADCHANMASK</description></item>
		/// 	<item><description>ERR_NOSUCHCHANNEL</description></item>
		/// 	<item><description>ERR_TOOMANYCHANNELS</description></item>
		/// 	<item><description>ERR_TOOMANYTARGETS</description></item>
		/// 	<item><description>ERR_UNAVAILRESOURCE</description></item>
		/// </list>
		/// </remarks>
		/// <param name="channel">The channel to join. Channel names must begin with '&amp;', '#', '+' or '!'.</param>
		/// <example><code>
		/// //Most channels you will see begin with the '#'. The others are reserved
		/// //for special channels and may not even be available on a particular server.
		/// connection.Sender.Join("#thresher");
		/// </code></example>
		/// <exception cref="ArgumentException">If the channel name is not valid.</exception>
		/// <seealso cref="Listener.OnJoin"/>
		public void Join( string channel ) 
		{
			lock( this ) 
			{
				if ( Rfc2812Util.IsValidChannelName( channel ) ) 
				{
					Buffer.Append("JOIN");
					Buffer.Append(SPACE);
					Buffer.Append(channel);
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>
		/// Join a passworded channel.
		/// </summary>
		/// <param name="channel">Channel to join</param>
		/// <param name="password">The channel's pasword. Cannot be null or empty.</param>
		/// <exception cref="ArgumentException">If the channel name is not valid or the password is null.</exception> 
		/// <seealso cref="Listener.OnJoin"/>
		public void Join(string channel, string password) 
		{
			lock( this ) 
			{
				if ( IsEmpty( password) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Password cannot be empty or null.");
				}
				if (Rfc2812Util.IsValidChannelName(channel)) 
				{
					Buffer.Append("JOIN");
					Buffer.Append(SPACE);
					Buffer.Append(channel);
					Buffer.Append(SPACE);
					//8 is the JOIN + 2 spaces + CR + LF
					password = Truncate( password, 8 );
					Buffer.Append(password);
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>
		/// Change the user's nickname.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 	<list type="bullet">
		/// 		<item><description>ERR_NONICKNAMEGIVEN</description></item>
		/// 		<item><description>ERR_ERRONEUSNICKNAME</description></item>
		/// 		<item><description>ERR_NICKNAMEINUSE</description></item>
		/// 		<item><description>ERR_NICKCOLLISION</description></item>
		/// 		<item><description>ERR_UNAVAILRESOURCE</description></item>
		/// 		<item><description>ERR_RESTRICTED</description></item>
		/// 	</list>
		/// </remarks>
		/// <param name="newNick"> The new nickname</param>
		/// <example><code>
		/// //Make sure and verify that the nick is valid and of the right length
		/// string nick = GetUserInput();
		/// if( Rfc2812Util.IsValidNick( connection, nick) ) { 
		/// connection.Sender.Nick( nick );
		/// }
		/// </code></example>
		/// <exception cref="ArgumentException">If the nickname is not valid.</exception> 
		/// <seealso cref="Listener.OnNick"/>
		public void Nick( string newNick ) 
		{
			lock( this ) 
			{
				if ( Rfc2812Util.IsValidNick(newNick) ) 
				{
					Buffer.Append("NICK");
					Buffer.Append(SPACE);
					Buffer.Append(newNick);
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(newNick + " is not a valid nickname.");
				}
			}
		}
		/// <summary> 
		/// Request a list of all nicknames on a given channel.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_TOOMANYMATCHES</description></item>
		/// </list>
		/// </remarks>
		/// <param name="channels">One or more channel names.</param>
		/// <example><code>
		/// //Make the request for a single channel
		/// connection.Sender.Names( "#test" );
		/// //Make the request for several channels at once
		/// connection.Sender.Names( "#test","#alpha","#bravo" );
		/// </code></example>
		/// <exception cref="ArgumentException">If any of the channels are not valid.</exception> 
		/// <seealso cref="Listener.OnNames"/>
		public void Names( params string[] channels ) 
		{
			lock( this ) 
			{
				if ( Rfc2812Util.IsValidChannelList( channels ) ) 
				{
					Buffer.Append("NAMES");
					Buffer.Append(SPACE);
					Buffer.Append( String.Join(",", channels) );
					if( TooLong( Buffer ) ) 
					{
						ClearBuffer();
						throw new ArgumentException("Channels are too long.");
					}
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException("One of the channel names is not valid.");
				}
			}
		}
		/// <summary>
		/// Request a list of all visible channels along with their users. If the server allows this
		/// kind of request then expect a rather large reply. 
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 	<list type="bullet">
		/// 		<item><description>ERR_TOOMANYMATCHES</description></item>
		/// 	</list>
		/// </remarks> 
		/// <seealso cref="Listener.OnNames"/> 
		public void AllNames() 
		{
			lock( this ) 
			{
				Buffer.Append("NAMES");
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>Request basic information about a channel, i.e. number
		/// of visible users and topic.</summary>
		/// <remarks>
		/// Possible Errors
		/// 	<list type="bullet">
		/// 		<item><description>ERR_TOOMANYMATCHES</description></item>
		/// </list>
		/// </remarks> 
		/// <param name="channels">One or more channel names.</param>
		/// <example><code>
		/// //Make the request for a single channel
		/// connection.Sender.List( "#test" );
		/// //Make the request for several channels at once
		/// connection.Sender.List( "#test","#alpha",#"bravo" );
		/// </code></example>
		/// <exception cref="ArgumentException">If any of the channels are not valid.</exception> 
		/// <seealso cref="Listener.OnList"/> 
		public void List(params string[] channels ) 
		{
			lock( this ) 
			{
				if (Rfc2812Util.IsValidChannelList(channels)) 
				{
					Buffer.Append("LIST");
					Buffer.Append(SPACE);
					Buffer.Append( String.Join(",", channels) );
					if( TooLong( Buffer ) ) 
					{
						ClearBuffer();
						throw new ArgumentException("Channels are too long.");
					}
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException("One of the channel names is not valid.");
				}
			}
		}
		/// <summary>
		/// Request basic information for all the channels on the current
		/// network.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 	<list type="bullet">
		/// 		<item><description>ERR_TOOMANYMATCHES</description></item>
		/// </list>
		/// </remarks> 
		/// <seealso cref="Listener.OnList"/>
		public void AllList() 
		{
			lock( this ) 
			{
				Buffer.Append("LIST");
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>Change the topic of the given channel.</summary>
		/// <remarks>
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 		<item><description>ERR_NOTONCHANNEL</description></item>
		/// 		<item><description>ERR_CHANOPRIVSNEEDED</description></item>
		/// 		<item><description>ERR_NOCHANMODES</description></item>
		/// </list>
		/// </remarks>
		/// <param name="channel">The target channel.</param>
		/// <param name="newTopic">The new topic.</param>
		/// <example><code>
		/// connection.Sender.ChangeTopic( "#thresher","Beta 27 Released" );
		/// </code></example>	
		/// <exception cref="ArgumentException">If the channel name is not valid or the topic is null.</exception> 
		/// <seealso cref="Listener.OnTopicChanged"/> 
		public void ChangeTopic(string channel, string newTopic) 
		{
			lock( this ) 
			{
				if (IsEmpty( newTopic ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Topic cannot be empty or null.");
				}
				if (Rfc2812Util.IsValidChannelName(channel)) 
				{
					Buffer.Append("TOPIC");
					Buffer.Append(SPACE);
					Buffer.Append(channel);
					Buffer.Append(SPACE_COLON);
					// 9 is TOPIC + 2 x Spaces + : + CR = LF
					newTopic = Truncate( newTopic, 9 + channel.Length );
					Buffer.Append(newTopic);
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>Clear the channel's topic.</summary>
		/// <remarks>
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 		<item><description>ERR_NOTONCHANNEL</description></item>
		/// 		<item><description>ERR_CHANOPRIVSNEEDED</description></item>
		/// 		<item><description>ERR_NOCHANMODES</description></item>
		/// </list>
		/// </remarks>
		/// <param name="channel">The target channel.</param>
		/// <exception cref="ArgumentException">If the channel name is not valid.</exception> 
		/// <seealso cref="Listener.OnTopicChanged"/> 
		public void ClearTopic(string channel) 
		{
			lock( this ) 
			{
				if (Rfc2812Util.IsValidChannelName(channel)) 
				{
					Buffer.Append("TOPIC");
					Buffer.Append(SPACE);
					Buffer.Append(channel);
					Buffer.Append(SPACE_COLON);
					Buffer.Append(SPACE);
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>Request the topic for the given channel.</summary>
		/// <remarks>
		/// <para>
		/// The reply will be sent via the <see cref="Listener.OnTopicRequest"/> event. If there is no topic
		/// then <see cref="Listener.OnError"/> will be called with a code of <see cref="ReplyCode.RPL_NOTOPIC"/>.
		/// </para>
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 		<item><description>ERR_NOTONCHANNEL</description></item>
		/// 		<item><description>ERR_CHANOPRIVSNEEDED</description></item>
		/// 		<item><description>ERR_NOCHANMODES</description></item>
		/// </list>
		/// </remarks>
		/// <param name="channel">The target channel.</param>
		/// <exception cref="ArgumentException">If the channel name is not valid.</exception> 
		/// <seealso cref="Listener.OnTopicRequest"/> 
		public void RequestTopic(string channel) 
		{
			lock( this ) 
			{
				if (Rfc2812Util.IsValidChannelName(channel)) 
				{
					Buffer.Append("TOPIC");
					Buffer.Append(SPACE);
					Buffer.Append(channel);
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>
		/// Leave the given channel.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 		<item><description>ERR_NOSUCHCHANNEL</description></item>
		/// 		<item><description>ERR_NOTONCHANNEL</description></item>
		/// </list>
		/// </remarks>
		/// <param name="reason">A goodbye message.</param>
		/// <param name="channels">One or more channels to leave.</param>
		/// <example><code>
		/// //Leave a single channel
		/// connection.Sender.Part("Goodbye", "#test" );
		/// //Leave several at once
		/// connection.Sender.Part( "Goodbye", "#test","#alpha",#"bravo" );
		/// </code></example>
		/// <exception cref="ArgumentException">If the channel name is not valid or the reason is null.</exception> 
		/// <seealso cref="Listener.OnPart"/> 
		public void Part( string reason, params string[] channels ) 
		{
			lock( this ) 
			{
				if ( IsEmpty( reason ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Part reason cannot be empty or null.");
				}
				if (Rfc2812Util.IsValidChannelList(channels)) 
				{
					Buffer.Append("PART");
					Buffer.Append(SPACE);
					string channelList = String.Join(",", channels);
					Buffer.Append( channelList );
					Buffer.Append(SPACE_COLON);
					// 9 is PART + 2 x Spaces + : + CR + LF
					reason = Truncate( reason, 9 );
					Buffer.Append(reason);
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException("One of the channels names is not valid.");
				}
			}
		}
		/// <summary>
		/// Leave a channel without giving a reason.
		/// </summary>
		/// <param name="channel">The channel to leave.</param>
		/// <exception cref="ArgumentException">If the channel name is not valid.</exception> 
		/// <seealso cref="Listener.OnPart"/> 
		public void Part( string channel ) 
		{
			lock( this ) 
			{
				if (Rfc2812Util.IsValidChannelName( channel ) ) 
				{
					Buffer.Append("PART");
					Buffer.Append(SPACE);
					Buffer.Append( channel );
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException( channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>Send a notice to a channel.</summary>
		/// <remarks>
		/// <para>The difference between a notice and a normal message is that 
		/// automatic replies must never be sent in response to a notice. This rule 
		/// applies to servers too - they must not send any error reply back to the 
		/// client on receipt of a notice. The object of this rule is to avoid loops
		/// between clients automatically sending something in response to
		/// something it received. See <see cref="Sender.PublicMessage"/> for possible errors.</para>
		/// </remarks>
		/// <param name="channel">The target channel.</param>
		/// <param name="message">Text message. If the text is too large to be sent in one
		/// piece it will be broken up into smaller strings which will then
		/// be sent individually.</param>
		/// <exception cref="ArgumentException">If the channel name is not valid or the message is empty or null.</exception> 
		/// <seealso cref="Listener.OnPublicNotice"/> 
		public void PublicNotice(string channel, string message) 
		{
			lock( this ) 
			{
				if ( IsEmpty( message) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Notice message cannot be null or empty.");
				}
				if (Rfc2812Util.IsValidChannelName(channel) ) 
				{
					// 11 is NOTICE + 2 x Spaces + : + CR + LF
					int max = MAX_COMMAND_SIZE - 11 - channel.Length;
					if (message.Length > max) 
					{
						string[] parts = BreakUpMessage( message, max );
						foreach( string part in parts )
						{
							SendMessage("NOTICE", channel, part);
						}
					}
					else 
					{
						SendMessage("NOTICE", channel, message);
					}
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>Send a notice to a user.</summary>
		/// <remarks>
		/// <para>The difference between a notice and a normal message is that 
		/// automatic replies must never be sent in response to a notice. This rule 
		/// applies to servers too - they must not send any error reply back to the 
		/// client on receipt of a notice. The object of this rule is to avoid loops
		/// between clients automatically sending something in response to
		/// something it received. See <see cref="Sender.PrivateMessage"/> for possible errors.</para>
		/// </remarks>
		/// <param name="nick">The target nickname.</param>
		/// <param name="message">Text message. If the text is too large to be sent in one
		/// piece it will be broken up into smaller strings which will then
		/// be sent individually.</param>
		/// <exception cref="ArgumentException">If the nick is not valid or the message is empty or null.</exception> 
		/// <seealso cref="Listener.OnPrivateNotice"/> 
		public void PrivateNotice(string nick, string message) 
		{
			lock( this ) 
			{
				if ( IsEmpty(message ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Notice message cannot be empty or null.");
				}
				if ( Rfc2812Util.IsValidNick(nick)) 
				{
					// 11 is NOTICE + 2 x Spaces + : + CR + LF
					int max = MAX_COMMAND_SIZE - 11 - nick.Length;
					if (message.Length > max) 
					{
						string[] parts = BreakUpMessage( message, max );
						foreach( string part in parts )
						{
							SendMessage("NOTICE", nick, part);
						}
					}
					else 
					{
						SendMessage("NOTICE", nick, message);
					}
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nickname.");
				}
			}
		}
		/// <summary>
		/// Send a message to all the users in a channel.</summary>
		/// <remarks>
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_CANNOTSENDTOCHAN</description></item>
		/// 		<item><description>ERR_NOTEXTTOSEND</description></item>
		/// </list>
		/// </remarks>
		/// <param name="channel">The target channel.</param>
		/// <param name="message">A message. If the message is too long it will be broken
		/// up into smaller piecese which will be sent sequentially.</param>
		/// <exception cref="ArgumentException">If the channel name is not valid or if the message is null.</exception> 
		/// <seealso cref="Listener.OnPublic"/> 
		public void PublicMessage(string channel, string message) 
		{
			lock( this )
			{
				if ( IsEmpty( message ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Public message cannot be null or empty.");
				}
				if (Rfc2812Util.IsValidChannelName(channel)) 
				{
					// 11 is PRIVMSG + 2 x Spaces + : + CR + LF
					int max = MAX_COMMAND_SIZE - 11 - channel.Length;
					if (message.Length > max) 
					{
						string[] parts = BreakUpMessage( message, max );
						foreach( string part in parts )
						{
							SendMessage("PRIVMSG", channel, part );
						}
					}
					else 
					{
						SendMessage("PRIVMSG", channel, message);
					}
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>
		/// Send a message to a user.</summary>
		/// <remarks>
		/// <para>If the target user status is away, the <see cref="Listener.OnAway"/> event will be
		/// called along with the away message if any.
		/// </para>
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_NORECIPIENT</description></item>
		/// 		<item><description>ERR_NOTEXTTOSEND</description></item>
		/// 		<item><description>ERR_NOSUCHNICK</description></item>
		/// </list>
		/// </remarks>
		/// <param name="nick">The target user.</param>
		/// <param name="message">A message. If the message is too long it will be broken
		/// up into smaller piecese which will be sent sequentially.</param>
		/// <exception cref="ArgumentException">If the nickname is not valid or if the message is null or empty.</exception> 
		/// <seealso cref="Listener.OnPrivate"/> 
		public void PrivateMessage(string nick, string message) 
		{
			lock( this )
			{
				if ( IsEmpty( message ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Private message cannot be null or empty.");
				}
				if (Rfc2812Util.IsValidNick(nick)) 
				{
					// 11 is PRIVMSG + 2 x Spaces + : + CR + LF
					int max = MAX_COMMAND_SIZE - 11 - nick.Length;
					if (message.Length > max) 
					{
						string[] parts = BreakUpMessage( message, max );
						foreach( string part in parts )
						{
							SendMessage("PRIVMSG", nick, part );
						}
					}
					else 
					{
						SendMessage("PRIVMSG", nick, message);
					}
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nickname.");
				}
			}
		}
		/// <summary>
		/// Invite a user to a channel.</summary>
		/// <remarks>
		/// <para>The parameter nickname is the nickname of the person to be invited to
		/// the target channel channel. There is no requirement that the
		/// channel the target user is being invited to must exist or be a valid
		/// channel. However, if the channel exists, only members of the channel
		/// are allowed to invite other users. When the channel has invite-only
		/// flag set, only channel operators may an invite.</para>
		/// 
		/// <para>Only the user inviting and the user being invited will receive
		/// notification of the invitation. Other channel members are not
		/// notified. (This is unlike the mode changes, and is occasionally the
		/// source of trouble for users.)</para>
		/// 
		/// <para>After the invite is sent the IRC server will signal that it 
		/// was correctly received by calling <see cref="Listener.OnInviteSent"/>.
		/// </para>
		/// 
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 		<item><description>ERR_NOSUCHNICK</description></item>
		/// 		<item><description>ERR_NOTONCHANNEL</description></item>
		/// 		<item><description>ERR_USERONCHANNEL</description></item>
		/// 		<item><description>ERR_CHANOPRIVSNEEDED</description></item>
		/// </list>
		/// </remarks>
		/// <param name="who">The nick of the person to invite</param>
		/// <param name="channel">The channel they are invited to join.</param>
		/// <exception cref="ArgumentException">If the nickname or channel is not valid.</exception> 
		/// <seealso cref="Listener.OnInviteSent"/> 
		/// <seealso cref="Listener.OnInvite"/> 
		public void Invite(string who, string channel) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidNick(who)) 
				{
					ClearBuffer();
					throw new ArgumentException(who + " is not a valid nickname.");
				}
				if (!Rfc2812Util.IsValidChannelName(channel)) 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel.");
				}
				Buffer.Append("INVITE");
				Buffer.Append(SPACE);
				Buffer.Append(who);
				Buffer.Append(SPACE);
				Buffer.Append(channel);
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>Kick a user off a channel.</summary>
		/// <remarks>
		/// Possible Replies
		/// <list type="bullet">
		/// 		<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 		<item><description>ERR_NOSUCHCHANNEL</description></item>
		/// 		<item><description>ERR_BADCHANMASK</description></item>
		/// 		<item><description>ERR_CHANOPRIVSNEEDED</description></item>
		/// 		<item><description>ERR_USERNOTINCHANNEL</description></item>
		/// 		<item><description>ERR_NOTONCHANNEL</description></item>
		/// 	</list>
		/// </remarks>
		/// <param name="nicks">One or more users to kick.</param>
		/// <param name="channel">Which channel to kick the user from.</param>
		/// <param name="reason">Why.</param>
		/// <example><code>
		/// //Kick a single user
		/// connection.Sender.Kick("#thresher","No Perl please","lwall" );
		/// //Kicks several users at once
		/// connection.Sender.Kick( "#thresher", "Bye", "John","Dick","Harry" );
		/// </code></example>
		/// <exception cref="ArgumentException">If the nickname or channel is not valid or the reason is null.</exception> 
		/// <seealso cref="Listener.OnKick"/> 
		public void Kick(string channel, string reason, params string[] nicks) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidNicklList( nicks)) 
				{
					ClearBuffer();
					throw new ArgumentException("One of the nicknames is invalid.");
				}
				if (!Rfc2812Util.IsValidChannelName(channel)) 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel.");
				}
				if ( IsEmpty( reason ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("The reason for kicking cannot be null.");
				}
				string nickList = String.Join(",", nicks);
				// 10 is KICK + 3 x Spaces + : + CR + LF
				reason = Truncate( reason, 10 + channel.Length + nickList.Length );
				Buffer.Append("KICK");
				Buffer.Append(SPACE);
				Buffer.Append(channel);
				Buffer.Append(SPACE);
				Buffer.Append(nickList);
				Buffer.Append(SPACE_COLON);
				Buffer.Append(reason);
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request to know if a user is currenlty on IRC.</summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="nick">Target user.</param>
		/// <example><code>
		/// //Query for a nick
		/// connection.Sender.Ison( "joe" );
		/// </code></example>
		/// <exception cref="ArgumentException">If the nickname is not valid.</exception> 
		/// <seealso cref="Listener.OnIson"/> 
		public void Ison( string nick ) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidNick(nick)) 
				{
					ClearBuffer();
					throw new ArgumentException( nick + " is not a valid nick.");
				}
				Buffer.Append("ISON");
				Buffer.Append(SPACE);
				Buffer.Append(nick );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request a list of users who match a given query.</summary>
		/// <remarks>
		/// Possible Errors
		/// <list type="bullet">
		/// 		<item><description>ERR_TOOMANYLINES</description></item>
		/// </list>
		/// </remarks>
		/// <param name="mask">The mask passed to Who is matched against a users' host, 
		/// real name or nickname. It uses the wildcard system of matching
		/// where the '*' can stand for any number of characters and '?' stands for any single
		/// character. The query will only match against one component so it is not possible
		/// to match against both nick and host for example.
		/// </param>
		/// <param name="operatorsOnly"> True if the query should only try to match the mask
		/// to users designated operators.</param>
		/// <example><code>
		/// //Find all users from Russia, i.e. who have .ru in their hostnames
		/// connection.Sender.Who("*.ru", false );
		/// //Find all users from clan [DX], i.e. have '[DX]' in their nick
		/// connection.Sender.Who("[DX]*", false );
		/// </code></example>
		/// <exception cref="ArgumentException">If the mask is null,empty, or too long.</exception> 
		/// <seealso cref="Listener.OnWho"/> 
		public void Who( string mask, bool operatorsOnly ) 
		{
			lock( this )
			{
				//7 is WHO + Space +O + CR + LF
				int max = MAX_COMMAND_SIZE - 7;
				if( IsEmpty( mask) ||
					mask.Length > max ) 
				{
					ClearBuffer();
					throw new ArgumentException("Who mask is invalid.");
				}
				Buffer.Append("WHO");
				Buffer.Append(SPACE);
				Buffer.Append(mask);
				if( operatorsOnly ) 
				{
					Buffer.Append( SPACE ) ;
					Buffer.Append("o");
				}
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request a list all visible users (whose mode is not +i) and those 
		/// who don't have a common channel with the requesting 
		/// client are listed. 
		/// </summary>
		/// <seealso cref="Listener.OnWho"/> 
		public void AllWho() 
		{
			lock( this )
			{
				Buffer.Append("WHO");
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request detailed information about a given user.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHNICK</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="nick">The nick of the query subject.</param>
		/// <exception cref="ArgumentException">If the nick is invalid.</exception> 
		/// <seealso cref="Listener.OnWhois"/> 
		/// <seealso cref="WhoisInfo"/> 
		public void Whois( string nick )
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidNick(nick)) 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nickname.");
				}
				Buffer.Append("WHOIS");
				Buffer.Append(SPACE);
				Buffer.Append(nick);
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Set the user status to away and set an automatic reply 
		/// to any private message.
		/// </summary>
		/// <remarks>
		/// This functions as a kind of IRC answering machine. It is normally used
		/// to indicate that the user is away from IRC.
		/// </remarks>
		/// <param name="message">The message that will be sent back to others when you
		/// are away. Overly long message will be truncated.</param>
		/// <exception cref="ArgumentException">If the message is null or empty.</exception> 
		/// <seealso cref="Listener.OnAway"/> 
		public void Away( string message) 
		{
			lock( this ) 
			{
				if ( IsEmpty( message ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Away message cannot be empty or null.");
				}
				Buffer.Append("AWAY");
				Buffer.Append(SPACE_COLON);
				// 8 is AWAY + Space + : + CR + LF
				message = Truncate( message, 8);
				Buffer.Append(message);
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Turns off the away status and the accompanying message.
		/// </summary>
		public void UnAway() 
		{
			lock( this ) 
			{
				Buffer.Append("AWAY");
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request information about a user who is no longer on IRC.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NONICKNAMEGIVEN</description></item>
		/// 			<item><description>ERR_WASNOSUCHNICK</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="nick">Target nick</param>
		/// <exception cref="ArgumentException">If the nick is invalid.</exception> 
		/// <seealso cref="Listener.OnWhowas"/> 
		public void Whowas( string nick )
		{
			lock( this ) 
			{
				if (!Rfc2812Util.IsValidNick(nick)) 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nickname.");
				}
				Buffer.Append("WHOWAS");
				Buffer.Append( SPACE );
				Buffer.Append( nick );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request information about a user who is no longer on IRC
		/// but with a maximum number of responses.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NONICKNAMEGIVEN</description></item>
		/// 			<item><description>ERR_WASNOSUCHNICK</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="count">The maximum number of replies the IRC server
		/// should send back.</param>
		/// <param name="nick">Target nick</param>
		/// <exception cref="ArgumentException">If the nick is invalid or if count is less 
		/// than or equal to zero.</exception> 
		/// <seealso cref="Listener.OnWhowas"/> 
		public void Whowas( string nick, int count )
		{
			lock( this ) 
			{
				if (!Rfc2812Util.IsValidNick(nick)) 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nickname.");
				}
				if( count < 1 ) 
				{
					ClearBuffer();
					throw new ArgumentException("Count must be more than zero.");
				}
				Buffer.Append("WHOWAS");
				Buffer.Append( SPACE );
				Buffer.Append( nick );
				Buffer.Append( SPACE );
				Buffer.Append( count );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request the modes set for this user.
		/// </summary>
		/// <seealso cref="Listener.OnUserModeRequest"/>
		/// <seealso cref="UserMode"/>
		public void RequestUserModes() 
		{
			lock( this ) 
			{
				Buffer.Append("MODE");
				Buffer.Append( SPACE );
				Buffer.Append( Connection.ConnectionData.Nick );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>Changes this client's mode. To change another nick's mode
		/// use <see cref="ChangeChannelMode"/>.</summary>
		/// <remarks>
		/// Away cannot be set here but should be set using <see cref="Sender.Away"/> 
		/// or removed using <see cref="Sender.UnAway"/>.
		/// </remarks>
		/// <param name="action">Add or remove a mode.</param>
		/// <param name="mode">The mode to be changed.</param>
		/// <example><code>
		/// //Turn off invisibility
		/// connection.Sender.ChangeUserMode( ModeAction.Remove, UserMode.Invisible );
		/// //Turn on wallops (and get a lot of IRC garbage)
		/// connection.Sender.ChangeUserMode( ModeAction.Add, UserMode.Wallops );
		/// </code></example>
		/// <exception cref="ArgumentException">If the UserMode parameter is Away.</exception> 
		/// <seealso cref="Listener.OnUserModeChange"/>
		public void ChangeUserMode( ModeAction action, UserMode mode ) 
		{
			lock( this ) 
			{
				if ( mode == UserMode.Away ) 
				{
					ClearBuffer();
					throw new ArgumentException("Away mode can only be changed with the Away and Unaway commands.");
				}
				Buffer.Append("MODE");
				Buffer.Append( SPACE );
				Buffer.Append( Connection.ConnectionData.Nick );
				Buffer.Append( SPACE );
				Buffer.Append( Rfc2812Util.ModeActionToChar( action ) );
				Buffer.Append( Rfc2812Util.UserModeToChar( mode ) );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Change a channel's mode.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 			<item><description>ERR_KEYSET</description></item>
		/// 			<item><description>ERR_NOCHANMODES</description></item>
		/// 			<item><description>ERR_CHANOPRIVSNEEDED</description></item>
		/// 			<item><description>ERR_USERNOTINCHANNEL</description></item>
		/// 			<item><description>ERR_UNKNOWNMODE</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="channel">The target channel.</param>
		/// <param name="action">Add or remove.</param>
		/// <param name="mode">The target mode.</param>
		/// <param name="param">An optional parameter for certain modes. If the mode 
		/// does not require one this should be null.</param>
		/// <example><code>
		/// //Give 'nick' the ability to talk on a moderated channel, i.e. add Voice
		/// connection.Sender.ChangeChannelMode("#thresher", ModeAction.Add, ChannelMode.Voice,"nick" );
		/// //Make a channel private
		/// connection.Sender.ChangeChannelMode( "#thresher", ModeAction.Add, ChannelMode.Private, null );
		/// </code></example>
		/// <exception cref="ArgumentException">If the channel name is invalid.</exception> 
		/// <seealso cref="Listener.OnChannelModeChange"/>
		public void ChangeChannelMode( string channel, ModeAction action, ChannelMode mode, string param ) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidChannelName(channel)) 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel.");
				}
				Buffer.Append("MODE");
				Buffer.Append(SPACE);
				Buffer.Append( channel );
				Buffer.Append(SPACE);
				Buffer.Append( Rfc2812Util.ModeActionToChar( action ) );
				Buffer.Append( Rfc2812Util.ChannelModeToChar( mode ) );
				if( !IsEmpty( param ) ) 
				{
					Buffer.Append(SPACE);
					Buffer.Append( param );
				}
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request the list of users that a channel keeps for the given mode.. 
		/// </summary>
		/// <remarks>
		/// Each channel maintains a list of those banned, those excepted from a ban,
		/// those on automatic invite, and the channel creator. Use this method to retreieve one
		/// of those lists.
		/// </remarks>
		/// <param name="channel">The taregt channel.</param>
		/// <param name="mode">Must be one of:
		/// Ban, Exception, Invitation, or ChannelCreator.
		/// </param>
		/// <example><code>
		/// //Request the channel's banned list
		/// connection.Sender.RequestChannelList("#thresher", ChannelMode.Ban );
		/// </code></example>
		/// <exception cref="ArgumentException">If the channel is invalid or the ChannelMode is
		/// not one of the 4 allowed types.</exception> 
		/// <seealso cref="Listener.OnChannelList"/>
		public void RequestChannelList( string channel, ChannelMode mode ) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidChannelName(channel)) 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel.");
				}
				if( mode != ChannelMode.Ban &&
					mode != ChannelMode.Exception &&
					mode != ChannelMode.Invitation &&
					mode != ChannelMode.ChannelCreator ) 
				{
					ClearBuffer();
					throw new ArgumentException( Enum.GetName( typeof(ChannelMode), mode ) + " is not a valid channel mode for this request.");
				}
				Buffer.Append("MODE");
				Buffer.Append(SPACE);
				Buffer.Append( channel );
				Buffer.Append(SPACE);
				Buffer.Append( Rfc2812Util.ChannelModeToChar( mode ) );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request the modes of a channel.
		/// </summary>
		/// <param name="channel">The target channel.</param>
		/// <exception cref="ArgumentException">If the channel is invalid, null, or empty.</exception> 
		/// <seealso cref="Listener.OnChannelModeRequest"/>
		public void RequestChannelModes( string channel ) 
		{
			lock( this )
			{
				if (!Rfc2812Util.IsValidChannelName(channel)) 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel.");
				}
				Buffer.Append("MODE");
				Buffer.Append(SPACE);
				Buffer.Append( channel );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Send an action message to a channel.
		/// </summary>
		/// <remarks>
		/// This is actually a CTCP command but it is so widely used
		/// that it is included here. These are the '\me Laughs' type messages. 
		/// </remarks>
		/// <param name="channel">The target channel.</param>
		/// <param name="description">A description of the action. If this is too long it will
		/// be truncated.</param>
		/// <example><code>
		/// //Express an emotion...
		/// connection.Sender.Action("#thresher", "Kicks down the door" );
		/// </code></example>
		/// <exception cref="ArgumentException">If the channel name is not valid. Will
		/// also be thrown if the description is null or empty.</exception> 
		/// <seealso cref="Listener.OnAction"/>
		public void Action(string channel, string description ) 
		{
			lock( this )
			{
				if (IsEmpty( description ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Action description cannot be null or empty.");
				}
				if (Rfc2812Util.IsValidChannelName(channel)) 
				{
					// 19 is PRIVMSG + 2 x Spaces + : + CR + LF + 2xCtcpQuote + ACTION
					description = Truncate( description, 19 + channel.Length) ;
					SendMessage("PRIVMSG", channel, CtcpQuote + "ACTION " + description + CtcpQuote );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(channel + " is not a valid channel name.");
				}
			}
		}
		/// <summary>
		/// Send an action message to a user instead of a channel.
		/// </summary>
		/// <param name="nick">The target user.</param>
		/// <param name="description">A description of the action. If this is too long it will
		/// be truncated.</param>
		/// <exception cref="ArgumentException">If the nickname is not valid. Will
		/// also be thrown if the description is null or empty.</exception>
		/// <seealso cref="Listener.OnPrivateAction"/>
		public void PrivateAction(string nick, string description ) 
		{
			lock( this )
			{
				if ( IsEmpty( description) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Action description cannot be null or empty.");
				}
				if (Rfc2812Util.IsValidNick(nick)) 
				{
					// 19 is PRIVMSG + 2 x Spaces + : + CR + LF + 2xCtcpQuote + ACTION
					description = Truncate( description, 19 + nick.Length );
					SendMessage("PRIVMSG", nick, CtcpQuote + "ACTION " + description + CtcpQuote );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nickname.");
				}
			}
		}
		/// <summary>Register this connection with the IRC server.</summary>
		/// <remarks>
		/// This method should be called when the initial attempt
		/// to register with the IRC server fails because the nick is already
		/// taken. To be informed when this fails you must be subscribed
		/// to <see cref="Listener.OnNickError"/>. If <see cref="Connection.HandleNickTaken"/>
		/// is set to true (which is its default value) then Thresher will automatically
		/// create an alternate nick and use that. The new nick can be retrieved
		/// by calling <see cref="Connection.ConnectionData.Nick"/>.
		/// </remarks>
		/// <param name="newNick">The changed nick name.</param>
		/// <seealso cref="NameGenerator"/>
		public void Register( string newNick ) 
		{
			Connection.connectionArgs.Nick = newNick;
			Nick( Connection.connectionArgs.Nick );
			User( Connection.connectionArgs );
		}
		/// <summary>
		/// Send an arbitrary text message to the IRC server.
		/// </summary>
		/// <remarks>
		/// Messages that are too long will be truncated. There is no corresponding 
		/// events so it will be necessary to check for standard reply codes and possibly
		/// errors.
		/// </remarks>
		/// <param name="message">A text message.</param>
		/// <exception cref="ArgumentException">If the message is null or empty.</exception> 
		public void Raw( string message ) 
		{
			lock( this )
			{
				if ( IsEmpty( message ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Message cannot be null or empty.");
				}
				if (message.Length > MAX_COMMAND_SIZE ) 
				{
					message = message.Substring( 0, MAX_COMMAND_SIZE );
				}
				Buffer.Append( message );
				Connection.SendCommand( Buffer );
			}
		}
		/// <summary>
		/// Request the version of the IRC server program.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <seealso cref="Listener.OnVersion"/>
		public void Version() 
		{
			Version( null );
		}
		/// <summary>
		/// Used to query the version of the IRC server program.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="targetServer">The FQDN of the IRC server to query. Wildcards are allowed.
		/// Must be a server part of the same IRC network this connection is connected to.</param>
		/// <seealso cref="Listener.OnVersion"/>
		public void Version( string targetServer ) 
		{
			lock( this )
			{
				Buffer.Append("VERSION");
				if ( !IsEmpty(targetServer) ) 
				{
					//10 is VERSION + 1 x Spaces + CR + LF 
					targetServer = Truncate( targetServer, 10);
					Buffer.Append( SPACE );
					Buffer.Append( targetServer );
				}
				Connection.SendCommand( Buffer );				
			}
		}
		/// <summary>
		/// Request the "Message Of The Day" from the current server.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOMOTD</description></item>
		/// 		</list>
		/// </remarks>
		/// <seealso cref="Listener.OnMotd"/>
		public void Motd() 
		{
			Motd( null );
		}
		/// <summary>
		/// Request the "Message Of The Day" from the given server.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOMOTD</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="targetServer">The FQDN of the IRC server to query. Wildcards are allowed.
		/// Must be a server part of the same IRC network this connection is connected to.</param>
		/// <seealso cref="Listener.OnMotd"/>
		public void Motd( string targetServer ) 
		{
			lock( this )
			{
				Buffer.Append("MOTD");
				if ( !IsEmpty(targetServer) ) 
				{
					//7 is MOTD + 1 x Spaces + CR + LF 
					targetServer = Truncate( targetServer, 7);
					Buffer.Append( SPACE );
					Buffer.Append( targetServer );
				}
				Connection.SendCommand( Buffer );				
			}
		}
		/// <summary>
		/// Request the local time from the current server.
		/// </summary>
		/// <seealso cref="Listener.OnTime"/>
		public void Time() 
		{
			Time( null );
		}
		/// <summary>
		/// Request the local time from the given server.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="targetServer">The FQDN of the IRC server to query. Wildcards are allowed.
		/// Must be a server part of the same IRC network this connection is connected to.</param>
		/// <seealso cref="Listener.OnTime"/>
		public void Time( string targetServer ) 
		{
			lock( this )
			{
				Buffer.Append("TIME");
				if ( !IsEmpty(targetServer) ) 
				{
					//8 is TIME + 1 x Spaces + CR + LF 
					targetServer = Truncate( targetServer, 8);
					Buffer.Append( SPACE );
					Buffer.Append( targetServer );
				}
				Connection.SendCommand( Buffer );				
			}
		}
		/// <summary>
		/// Send a message to all users who have the 'w' user mode set.</summary>
		/// <remarks>
		/// This will likely be forbidden to all but IRC
		/// OPS.
		/// </remarks>
		/// <param name="message">Any text message.</param>
		/// <exception cref="ArgumentException">If the message is empty or null.</exception>
		public void Wallops( string message ) 
		{
			lock( this )
			{
				if ( IsEmpty( message ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Wallops message cannot be null or empty.");
				}
				Buffer.Append("WALLOPS");
				// 11 is WALLOPS + 1 x Spaces + CR + LF
				message = Truncate( message, 10 );
				Buffer.Append( SPACE );
				Buffer.Append( message );
				Connection.SendCommand( Buffer );				
			}
		}
		/// <summary>
		/// Request information about the software
		/// of the current IRC server.
		/// </summary>
		/// <remarks>
		/// This returns information describing the
		/// server: its version, when it was compiled, the patchlevel, when it
		/// was started, and any other miscellaneous information which may be
		/// 	considered relevant.
		/// </remarks>
		/// <seealso cref="Listener.OnInfo"/>
		public void Info() 
		{
			Info( null );
		}
		/// <summary>
		/// Request information about the software
		/// of the target IRC server.
		/// </summary>
		/// <remarks>
		/// <para>This returns information describing the
		/// server: its version, when it was compiled, the patchlevel, when it
		/// was started, and any other miscellaneous information which may be
		/// 	considered relevant.</para>
		/// 	
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="target">Either a user nickname or a specific IRC server connected
		/// to the current network. If it is a nickname then return the information about
		/// the server to which 'nick' is connected. Can include wildcards.</param>
		/// <example><code>
		/// //Query a specific server
		/// connection.Sender.Info( "sunray.sharkbite.org" );
		/// //Query the server Bob is connected to
		/// connection.Sender.Info("Bob");
		/// </code></example>
		/// <seealso cref="Listener.OnInfo"/>
		public void Info( string target ) 
		{
			lock( this )
			{
				Buffer.Append("INFO");
				if ( !IsEmpty(target) ) 
				{
					//7 is INFO + 1 x Spaces + CR + LF 
					target = Truncate( target, 7);
					Buffer.Append( SPACE );
					Buffer.Append( target );
				}
				Connection.SendCommand( Buffer );				
			}
		}
		/// <summary>
		/// Request information about the administrator
		/// of the current IRC server.
		/// </summary>
		/// <remarks>
		/// This returns information such as the administrator's
		/// email address, geographical location and whatever else
		/// the IRC is configured to send as a response.
		/// </remarks>
		/// <seealso cref="Listener.OnAdmin"/>
		public void Admin() 
		{
			Admin( null );
		}
		/// <summary>
		/// Request information about the administrator
		/// of the target IRC server.
		/// </summary>
		/// <remarks>
		/// <para> This returns information such as the administrator's
		/// email address, geographical location and whatever else
		/// the IRC is configured to send as a response.
		/// </para>
		/// 	
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="target">Either a user nickname or a specific IRC server connected
		/// to the current network. If it is a nickname then return the information about
		/// the server to which 'nick' is connected. Can include wildcards.</param>
		/// <example><code>
		/// //Request info about the administrator of the specified server
		/// connection.Sender.Admin( "sunray.sharkbite.org" );
		/// //Request info about the administrator of the server Bob is connected to
		/// connection.Sender.Admin("Bob");
		/// </code></example>
		/// <seealso cref="Listener.OnAdmin"/>
		public void Admin( string target ) 
		{
			lock( this )
			{
				Buffer.Append("ADMIN");
				if ( !IsEmpty(target) ) 
				{
					//8 is INFO + 1 x Spaces + CR + LF 
					target = Truncate( target, 8);
					Buffer.Append( SPACE );
					Buffer.Append( target );
				}
				Connection.SendCommand( Buffer );				
			}
		}
		
		/// <summary>
		/// Request statistics about the size of the IRC network.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <seealso cref="Listener.OnLusers"/>
		public void Lusers() 
		{
			Lusers( null, null );
		}

		/// <summary>
		/// Request statistics about the size of the IRC network.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="hostMask">Limits the kinds of servers included in the response by
		/// specifiying a hostname string. Can include wildcards.</param>
		/// <param name="targetServer">Specifies the server that should process the request. Can be null
		/// to indicate that the current server should handle the request. Can include wildcards.</param>
		/// <example><code>
		/// //Request stats from the current server
		/// connection.Sender.Lusers();
		/// //Request stats about all servers ending in '.net' from the current server
		/// connection.Sender.Lusers("*.net", null );
		/// //Request stats about all servers ending in '.net' from 'west.gamesnet.net'
		/// connection.Sender.Lusers("*.net", "west.gamesnet.net");
		/// </code></example>
		/// <exception cref="ArgumentException">If the host mask and server names are too long.</exception>
		/// <seealso cref="Listener.OnLusers"/>
		public void Lusers( string hostMask, string targetServer ) 
		{
			lock( this )
			{
				Buffer.Append("LUSERS");
				if ( !IsEmpty(hostMask) ) 
				{
					Buffer.Append( SPACE );
					Buffer.Append( hostMask );
				}
				if( !IsEmpty(targetServer) ) 
				{
					Buffer.Append( SPACE );
					Buffer.Append( targetServer );
				}
				
				if( TooLong( Buffer ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Hostmask and TargetServer are too long.");
				}
				
				Connection.SendCommand( Buffer );				
			}
		}

		/// <summary>
		/// Request all server names which are known by the current server.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <seealso cref="Listener.OnLinks"/>
		public void Links() 
		{
			Links( null );
		}

		/// <summary>
		/// Request all server names which are known by the target server
		/// and which match a given host mask.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="masks">Either a single string which acts as a host
		/// mask filter for the query. Or two strings with the first as host mask
		/// and the second a target server. Any other arguments will be ignored.</param>
		/// <example><code>
		/// //Request names from the current server
		/// connection.Sender.Links();
		/// //Request names of all servers ending in '.net' from the current server
		/// connection.Sender.Links("*.edu" );
		/// //Request names of all servers ending in '.edu' from '*.gnome.org' servers
		/// connection.Sender.Links("*.edu", "*.gnome.org");
		/// </code></example>
		/// <exception cref="ArgumentException">If the masks are too long.</exception>
		/// <seealso cref="Listener.OnLinks"/>
		public void Links( params string[] masks ) 
		{
			lock( this )
			{
				Buffer.Append("LINKS");
				if( masks != null ) 
				{
					Buffer.Append( SPACE );
					Buffer.Append(  masks[0] );
	
					if( masks.Length >= 2 ) 
					{
						Buffer.Append( SPACE );
						Buffer.Append(  masks[1] );
					}
				}
				
				if( TooLong( Buffer ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Masks are too long.");
				}
				Connection.SendCommand( Buffer );				
			}
		}


		/// <summary>
		/// Request certain kinds of statistics about the current server.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="query">The type of query to send. See <see cref="StatsQuery"/> for choice.</param>
		/// <example><code>
		/// //Request server link stats
		/// connection.Sender.Stats( StatsQuery.Connections );
		/// </code></example>
		/// <seealso cref="Listener.OnStats"/>
		public void Stats( StatsQuery query ) 
		{
			Stats( query, null );
		}

		/// <summary>
		/// Request certain kinds of statistics about the current server.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOSUCHSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="query">The type of query to send. See <see cref="StatsQuery"/> for choice.</param>
		/// <param name="targetServer">Specifies the server that should process the request. Can include wildcards.</param>
		/// <example><code>
		/// //Request list of Operators from the server 'irc.gnome.org'
		/// connection.Sender.Stats( StatsQuery.Operators, "irc.gnome.org" );
		/// </code></example>
		/// <exception cref="ArgumentException">If the target server name is too long.</exception>
		/// <seealso cref="Listener.OnStats"/>
		public void Stats( StatsQuery query, string targetServer ) 
		{
			lock( this )
			{
				Buffer.Append("STATS");
				Buffer.Append( SPACE );
				Buffer.Append( Rfc2812Util.StatsQueryToChar( query ) );
				if( targetServer != null ) 
				{
					Buffer.Append( SPACE );
					Buffer.Append( targetServer );	
				}
				if( TooLong( Buffer ) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Target server name is too long.");
				}
				Connection.SendCommand( Buffer );				
			}
		}

		/// <summary>
		/// Forcefully disconnect a user form the IRC server. This can only be used
		/// by Operators.
		/// </summary>
		/// <remarks>
		/// Possible Errors
		/// 		<list type="bullet">
		/// 			<item><description>ERR_NOPRIVILEGES</description></item>
		/// 			<item><description>ERR_NEEDMOREPARAMS</description></item>
		/// 			<item><description>ERR_NOSUCHNICK</description></item>
		/// 			<item><description>ERR_CANTKILLSERVER</description></item>
		/// 		</list>
		/// </remarks>
		/// <param name="nick">User to kill</param>
		/// <param name="reason">The reason for disconnecting the user.</param>
		/// <exception cref="ArgumentException">If the nick is not valid or the reason is null.</exception> 
		/// <seealso cref="Listener.OnKill"/>
		public void Kill(string nick, string reason) 
		{
			lock( this ) 
			{
				if ( IsEmpty( nick) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Nick cannot be empty or null.");
				}
				if ( IsEmpty( reason) ) 
				{
					ClearBuffer();
					throw new ArgumentException("Reason cannot be empty or null.");
				}
				if (Rfc2812Util.IsValidNick(nick)) 
				{
					Buffer.Append("KILL");
					Buffer.Append(SPACE);
					Buffer.Append(nick);
					Buffer.Append(SPACE);
					Buffer.Append( reason );
					Connection.SendCommand( Buffer );
				}
				else 
				{
					ClearBuffer();
					throw new ArgumentException(nick + " is not a valid nick name.");
				}
			}
		}

	}
}
