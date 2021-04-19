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
	public class Sender
	{
		// Buffer to hold commands 
		StringBuilder buffer;
		//Containing conenction instance
		Connection connection;

		const char SPACE = ' ';
		const string SPACE_COLON = " :";
		const int MAX_COMMAND_SIZE = 512;
		const int MAX_HOSTNAME_LEN = 63;
		const int MAX_NICKNAME_LEN = 30;
		
		/// <summary>
		/// Create a new Sender for a specific connection.
		/// </summary>
		internal Sender(Connection connection ) {
			this.connection = connection;
			buffer = new StringBuilder(MAX_COMMAND_SIZE);
		}

		/// <summary>
		/// This methods actually sends the notice and privmsg commands.
		/// It assumes that the message has already been broken up
		/// and has a valid target.
		/// </summary>
		void SendMessage(string type, string target, string message) {
			buffer.Append(type);
			buffer.Append(SPACE);
			buffer.Append(target);
			buffer.Append(SPACE_COLON);
			buffer.Append(message);
			connection.SendCommand( buffer );
		}
		
		/// <summary>
		/// Break up a large message into smaller peices that will fit within the IRC
		/// max message size.
		/// </summary>
		/// <param name="message">The text to be broken up</param>
		/// <param name="maxSize">The largest size a piece can be</param>
		/// <returns>A string array holding the correctly sized messages.</returns>
		string[] BreakUpMessage(string message, int maxSize) 
		{
			int pieces = (int) Math.Ceiling( (float)message.Length / (float)maxSize );
			string[] parts = new string[ pieces ];
			for( int i = 0; i < pieces; i++ ) 
			{
				int start = i * maxSize;
				if( i == pieces - 1 ) 
				{
					parts[i] = message.Substring( start );	
				}
				else 
				{
					parts[i] = message.Substring( start , maxSize );	
				}
			}
			return parts;
		}

		bool IsEmpty( string aString ) 
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
		string Truncate( string parameter, int commandLength ) 
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

		/// <summary>
		/// The USER command is only used at the beginning of Connection to specify
		/// the username, hostname and realname of a new user.
		/// </summary>
		/// <param name="args">The user Connection data</param>
		internal void User( ConnectionArgs args ) 
		{
			lock( this )
			{
				buffer.Append("USER");
				buffer.Append(SPACE);
				buffer.Append( args.UserName );
				buffer.Append(SPACE);
				buffer.Append( args.ModeMask );
				buffer.Append(SPACE);
				buffer.Append('*');
				buffer.Append(SPACE);
				buffer.Append(':');
				buffer.Append( args.RealName );
				connection.SendCommand( buffer );
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
			if ( IsEmpty( reason ) ) 
				throw new ArgumentException("Quite reason cannot be null or empty.");
			
			lock( this ) 
			{
				buffer.Append("QUIT");
				
				buffer.Append(SPACE_COLON);
				if (reason.Length > 502) 
				{
					reason = reason.Substring(0, 504);
				}
				buffer.Append(reason);
				connection.SendCommand( buffer );
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
			buffer.Append("PONG");
			buffer.Append(SPACE);
			buffer.Append(message);
			connection.SendAutomaticReply( buffer );
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
				buffer.Append("PASS");
				buffer.Append(SPACE);
				buffer.Append(password);
				connection.SendCommand( buffer );
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
			if ( !Rfc2812Util.IsValidChannelName( channel ) ) 
				throw new ArgumentException(channel + " is not a valid channel name.");
			
			lock( this ) 
			{
				buffer.Append("JOIN");
				buffer.Append(SPACE);
				buffer.Append(channel);
				connection.SendCommand( buffer );
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
			if ( IsEmpty( password ) ) 
				throw new ArgumentException("Password cannot be empty or null.");
			if ( !Rfc2812Util.IsValidChannelName( channel ) )
				throw new ArgumentException(channel + " is not a valid channel name.");
			
			lock( this ) 
			{
				buffer.Append("JOIN");
				buffer.Append(SPACE);
				buffer.Append(channel);
				buffer.Append(SPACE);
				//8 is the JOIN + 2 spaces + CR + LF
				password = Truncate( password, 8 );
				buffer.Append(password);
				connection.SendCommand( buffer );
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
			if ( !Rfc2812Util.IsValidNick( newNick ) )
				throw new ArgumentException(newNick + " is not a valid nickname.");
				
			lock( this ) 
			{
				buffer.Append("NICK");
				buffer.Append(SPACE);
				buffer.Append(newNick);
				connection.SendCommand( buffer );
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
		/// </code></example>
		/// <exception cref="ArgumentException">If the channel name is not valid.</exception> 
		/// <seealso cref="Listener.OnNames"/>
		public void Names( string channel ) 
		{
			if ( !Rfc2812Util.IsValidChannelName( channel ) ) 
				throw new ArgumentException(channel + " is not a valid channel name.");
			
			lock( this ) 
			{
				buffer.Append("NAMES");
				buffer.Append(SPACE);
				buffer.Append(channel);
				connection.SendCommand( buffer );
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
			if ( IsEmpty( message ) ) 
				throw new ArgumentException("Public message cannot be null or empty.");
			if ( !Rfc2812Util.IsValidChannelName( channel ) ) 
				throw new ArgumentException(channel + " is not a valid channel name.");

			lock( this )
			{
				// 11 is PRIVMSG + 2 x Spaces + : + CR + LF
				int max = MAX_COMMAND_SIZE - 11 - channel.Length - MAX_HOSTNAME_LEN - MAX_NICKNAME_LEN;
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
			if ( IsEmpty( message ) ) 
				throw new ArgumentException("Private message cannot be null or empty.");
			if ( !Rfc2812Util.IsValidNick( nick ) ) 
				throw new ArgumentException(nick + " is not a valid nickname.");
				
			lock( this )
			{
				// 11 is PRIVMSG + 2 x Spaces + : + CR + LF
				int max = MAX_COMMAND_SIZE - 11 - nick.Length - MAX_HOSTNAME_LEN - MAX_NICKNAME_LEN;
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
			connection.connectionArgs.Nick = newNick;
			Nick( connection.connectionArgs.Nick );
			User( connection.connectionArgs );
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
			if ( IsEmpty( message ) ) 
				throw new ArgumentException("Message cannot be null or empty.");
			
			lock( this )
			{
				if (message.Length > MAX_COMMAND_SIZE ) 
				{
					message = message.Substring( 0, MAX_COMMAND_SIZE );
				}
				buffer.Append( message );
				connection.SendCommand( buffer );
			}
		}
	}
}
