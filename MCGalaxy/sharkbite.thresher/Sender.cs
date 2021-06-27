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
		//Containing conenction instance
		Connection connection;
		const int MAX_COMMAND_SIZE = 512;
		const int MAX_HOSTNAME_LEN = 63;
		const int MAX_NICKNAME_LEN = 30;
		
		/// <summary>
		/// Create a new Sender for a specific connection.
		/// </summary>
		internal Sender(Connection connection ) {
			this.connection = connection;
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
		/// Truncate parameters which cause a command line to be too long.
		/// </summary>
		/// <param name="parameter">The command parameter</param>
		/// <param name="commandLength">The length of the command plus whitespace</param>
		string Truncate( string parameter, int commandLength ) 
		{
			int max = MAX_COMMAND_SIZE - commandLength;
			if (parameter.Length < max) return parameter;
			
			return parameter.Substring(0, max);
		}

		internal void User() 
		{
			lock( this )
			{
				// 4 = IRC mode mask (invisible and not receive wallops)
				connection.SendCommand("USER " + connection.UserName + " 4 * :" + connection.RealName );
			}
		}

		internal void Quit(string reason) 
		{
			if ( IsEmpty( reason ) ) 
				throw new ArgumentException("Quit reason cannot be null or empty.");						
			// 10 = "QUIT :" + 2 spaces + CR + LF
			reason = Truncate(reason, 10);
			
			lock( this ) 
			{
				connection.SendCommand("QUIT :" + reason);
			}
		}

		internal void Pong(string message) 
		{
			//Not synchronized because it will only be called during on OnPing event by
			//the dispatch thread
			connection.SendCommand("PONG " + message);
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
				connection.SendCommand("PASS " + password);
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
		internal void RegisterConnection() 
		{
			Pass( connection.ServerPassword );
			Nick( connection.Nick );
			User();
		}

		public void Join(string channel) 
		{
			if ( IsEmpty( channel ) )
				throw new ArgumentException(channel + " is not a valid channel name.");
			
			lock( this ) 
			{
				connection.SendCommand("JOIN " + channel);
			}
		}

		public void Join(string channel, string password) 
		{
			if ( IsEmpty( password ) ) 
				throw new ArgumentException("Password cannot be empty or null.");
			if ( IsEmpty( channel ) )
				throw new ArgumentException("Channel name cannot be empty or null.");			
			// 8 = "JOIN" + 2 spaces + CR + LF
			password = Truncate(password, 8);
				
			lock( this ) 
			{
				connection.SendCommand("JOIN " + channel + " " + password);
			}
		}

		public void Nick(string nick) 
		{
			if ( !Rfc2812Util.IsValidNick( nick ) )
				throw new ArgumentException(nick + " is not a valid nickname.");
				
			lock( this ) 
			{
				connection.SendCommand("NICK " + nick);
			}
		}

		public void Names(string channel) 
		{
			if ( IsEmpty( channel ) ) 
				throw new ArgumentException("Channel name cannot be null or empty.");
			
			lock( this ) 
			{
				connection.SendCommand("NAMES " + channel);
			}
		}

		public void Message(string target, string message) 
		{
			// target is either a channel name or user nickname
			if ( IsEmpty( message ) ) 
				throw new ArgumentException("Public message cannot be null or empty.");
			if ( IsEmpty( target ) )
				throw new ArgumentException("Channel/Nick name cannot be null or empty.");

			lock( this )
			{
				// 11 is PRIVMSG + 2 x Spaces + : + CR + LF
				int max = MAX_COMMAND_SIZE - 11 - target.Length - MAX_HOSTNAME_LEN - MAX_NICKNAME_LEN;
				if (message.Length > max) 
				{
					string[] parts = BreakUpMessage( message, max );
					foreach( string part in parts )
					{
						connection.SendCommand("PRIVMSG " + target + " :" + part);
					}
				}
				else 
				{
					connection.SendCommand("PRIVMSG " + target + " :" + message);
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
			connection.Nick = newNick;
			Nick( connection.Nick );
			User();
		}

		public void Raw(string message) 
		{
			if ( IsEmpty( message ) ) 
				throw new ArgumentException("Message cannot be null or empty.");
			
			if ( message.Length > MAX_COMMAND_SIZE ) 
				message = message.Substring( 0, MAX_COMMAND_SIZE );
			
			lock( this )
			{
				connection.SendCommand( message );
			}
		}
	}
}
