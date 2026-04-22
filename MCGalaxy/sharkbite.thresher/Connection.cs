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
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Sharkbite.Irc
{
	/// <summary>
	/// This class manages the connection to the IRC server and provides
	/// access to all the objects needed to send and receive messages.
	/// </summary>
	public sealed class Connection
	{
		public StreamReader reader;
		public StreamWriter writer;
		Random rnd = new Random();

		public Connection()
		{
			RegisterDelegates();
		}
		
		/// <summary> The user's current nick name. </summary>
		public string Nick;	
		/// <summary> The user's 'real' name. </summary>
		public string RealName;		
		/// <summary> The user's machine logon name. </summary>
		public string UserName;
				
		/// <summary> Whether this client is connected and has successfully registered. </summary>
		public bool Registered;

		void MyNickChanged(string user, string newNick)
		{
			string nick = IRCUtils.ExtractNick(user);
			if ( Nick == nick )
			{
				Nick = newNick;
			}
		}
		
		string GetNewNick()
        {
            // prefer just adding _ to end of real nick
            if (Nick.Length < MAX_NICKNAME_LEN) return Nick + "_";

            // .. and then just randomly mutate a leading character
            int idx  = rnd.Next(MAX_NICKNAME_LEN / 3);
			char val = (char)('A' + rnd.Next(26));
			return Nick.Substring(0, idx) + val + Nick.Substring(idx + 1);
		}
		
		
		void HandleNickError( string badNick )
		{
			if( Registered ) return;
			
			// If this is our initial connection attempt
			Nick = GetNewNick();
			SendNick(Nick);
		}
		
		void RegisterDelegates()
		{
			OnNick += MyNickChanged;
			OnNickError += HandleNickError;
		}
		
		public void Init( Stream s )
		{
			Encoding encoding = new UTF8Encoding(false);
			Registered = false;
			
			writer = new StreamWriter( s, encoding );
			writer.AutoFlush = true;
			reader = new StreamReader( s, encoding );			
		}
		
		public void UpdateUser()
		{
			// NOTE: The following two commands may fail if
			//   nick is already in use by another IRC user
			SendNick(Nick);
			SendUser();		    
		}
		

		#region Sending
		const int MAX_COMMAND_SIZE = 512;
		const int MAX_HOSTNAME_LEN = 63;
		const int MAX_NICKNAME_LEN = 30;
		readonly object sendLock = new object();
				
		/// <summary> Send a message to the IRC server. </summary>
		void SendCommand(string command)
		{
			try
			{
				lock (sendLock) { writer.WriteLine( command ); }
			}
			catch
			{
			}
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

		void SendUser() 
		{
			// 4 = IRC mode mask (invisible and not receive wallops)
			SendCommand("USER " + UserName + " 4 * :" + RealName );
		}

		void SendPong(string message) 
		{
			SendCommand("PONG " + message);
		}

		public void SendNick(string nick) 
		{
			if ( !IsValidNick( nick ) )
				throw new ArgumentException(nick + " is not a valid nickname.");
				
			SendCommand("NICK " + nick);
		}

		public void SendMessage(string target, string message) 
		{
			// target is either a channel name or user nickname
			if ( IsEmpty( message ) ) 
				throw new ArgumentException("Public message cannot be null or empty.");
			if ( IsEmpty( target ) )
				throw new ArgumentException("Channel/Nick name cannot be null or empty.");

			lock (sendLock)
			{
				// 11 is PRIVMSG + 2 x Spaces + : + CR + LF
				int max = MAX_COMMAND_SIZE - 11 - target.Length - MAX_HOSTNAME_LEN - MAX_NICKNAME_LEN;
				if (message.Length > max) 
				{
					string[] parts = BreakUpMessage( message, max );
					foreach( string part in parts )
					{
						SendCommand("PRIVMSG " + target + " :" + part);
					}
				}
				else 
				{
					SendCommand("PRIVMSG " + target + " :" + message);
				}
			}
		}

		public void SendRaw(string message) 
		{
			if ( IsEmpty( message ) )
				throw new ArgumentException("Message cannot be null or empty.");			
			if ( message.Length > MAX_COMMAND_SIZE ) 
				message = message.Substring( 0, MAX_COMMAND_SIZE );

			SendCommand( message );
		}
		#endregion


		#region Events
		/// <summary>
		/// Error messages from the IRC server.
		/// </summary>
		public event ErrorMessageEventHandler OnError;
		/// <summary>
		/// The user tried to change his nick but it failed.
		/// </summary>
		public event NickErrorEventHandler OnNickError;
		/// <summary>
		/// Connection with the IRC server is open and registered.
		/// </summary>
		public event RegisteredEventHandler OnRegistered;
		/// <summary>
		/// A Notice type message was sent to a channel.
		/// </summary>
		public event PublicNoticeEventHandler OnPublicNotice;
		/// <summary>
		/// A private Notice type message was sent to the user.
		/// </summary>
		public event PrivateNoticeEventHandler OnPrivateNotice;
		/// <summary>
		/// Someone has joined a channel.
		/// </summary>
		public event JoinEventHandler OnJoin;
		/// <summary>
		/// A public message was sent to a channel.
		/// </summary>
		public event PublicMessageEventHandler OnPublic;
		/// <summary>
		/// An action message was sent to a channel.
		/// </summary>
		public event PublicActionEventHandler OnAction;
		/// <summary>
		/// A private action message was sent to the user.
		/// </summary>
		public event PrivateActionEventHandler OnPrivateAction;
		/// <summary>
		/// A user changed his nickname.
		/// </summary>
		public event NickEventHandler OnNick; 
		/// <summary>
		/// A private message was sent to the user.
		/// </summary>
		public event PrivateMessageEventHandler OnPrivate;
		/// <summary>
		/// Someone has left a channel. 
		/// </summary>
		public event PartEventHandler OnPart;
		/// <summary>
		/// Someone has quit IRC.
		/// </summary>
		public event QuitEventHandler OnQuit;
		/// <summary>
		/// Someone has been kicked from a channel. 
		/// </summary>
		public event KickEventHandler OnKick;
		/// <summary>
		/// The response to a <see cref="Sender.Names"/> request.
		/// </summary>
		public event NamesEventHandler OnNames;
		/// <summary>
		/// A channel's mode has changed.
		/// </summary>
		public event ChannelModeChangeEventHandler OnChannelModeChange;
		/// <summary>
		/// A User has been disconnected via a Kill message.
		/// </summary>
		public event KillEventHandler OnKill;
		#endregion


		#region Parsing
		const string CTCP_ACTION = "\u0001ACTION";
		const string CTCP_PREFIX = "\u0001";
		readonly char[] Separator = new char[] { ' ' };

		public void Parse(string line ) 
		{
		    int index = 0;
		    string prefix = IRCUtils.ExtractPrefix(line, ref index);
		    string cmd    = IRCUtils.NextParam(line, ref index);
			int code;
			
			if ( int.TryParse( cmd, out code ) ) {
				ParseReply( prefix, code, line, index );
			} else {
				ParseCommand( prefix, cmd, line, index );
			}
		}
		
		void ParseCommand(string user, string cmd, string line, int index ) 
		{	
		    string msg, channel, target, nick;
		    
			switch( cmd ) 
			{
			    case "PING":
			        // 3.7.2 Ping
                    msg = IRCUtils.NextAll(line, ref index);
				    SendPong( msg );
				    break;
				    
				case "ERROR":
				    // 3.7.4 Error - <error message>
                    msg = IRCUtils.NextAll(line, ref index);
				    OnError( -1, msg );
				    break;
				    
				case "NOTICE":
				    // 3.3.2 Notice - <msgtarget> <text>
				    // "The difference between NOTICE and PRIVMSG is that automatic replies 
				    //  MUST NEVER be sent in response to a NOTICE message"
					target = IRCUtils.NextParam(line, ref index);
					msg    = IRCUtils.NextAll(  line, ref index);
					
					if( IsValidChannelName( target ) )
					{			
						OnPublicNotice( user, target, msg );
					}
					else 
					{
						OnPrivateNotice( user, msg );
					}
					break;
					
				case "JOIN":
					// 3.2.1 Join - ( <channel> *( "," <channel> ) [ <key> *( "," <key> ) ] ) / "0"
					channel = IRCUtils.NextParam(line, ref index);
					
					OnJoin( user, channel );
					break;
					
				case "PRIVMSG":
					// 3.3.1 Private messages - <msgtarget> <text to be sent>
					target = IRCUtils.NextParam(line, ref index);
					msg    = IRCUtils.NextAll(  line, ref index);
					
					if( msg.StartsWith(CTCP_ACTION) )
					{
					    msg = msg.Replace("\x01", "");
						if( IsValidChannelName( target ) )
						{
							OnAction( user, target, msg );
						}
						else 
						{
						    OnPrivateAction( user, msg );
						}
					}
					else if( msg.StartsWith( CTCP_PREFIX ) ) 
					{
					    // Other CTCP/DCC etc messages aren't supported
					}
					else if( IsValidChannelName( target ) )
					{
						OnPublic( user, target, msg );
					}
					else 
					{
						OnPrivate( user, msg );
					}
					break;
					
				case "NICK":
					// 3.1.2 Nick message - <nickname>
					nick    = IRCUtils.NextParam(line, ref index);
					
					OnNick(	user, nick );
					break;
					
				case "PART":
					// 3.2.2 Part - Parameters: <channel> *( "," <channel> ) [ <Part Message> ]
					channel = IRCUtils.NextParam(line, ref index);
					msg     = IRCUtils.NextAll(  line, ref index);
					
					OnPart( user, channel, msg );
					break;
					
				case "QUIT":
					// 3.1.7 Quit - [ <Quit Message> ]
					msg = IRCUtils.NextAll(line, ref index);
					
					OnQuit( user, msg );
					break;
					
				case "KICK":
					// 3.2.8 Kick - <channel> *( "," <channel> ) <user> *( "," <user> ) [<comment>]
					channel = IRCUtils.NextParam(line, ref index);
					nick    = IRCUtils.NextParam(line, ref index);
					msg     = IRCUtils.NextAll(  line, ref index);
					
					OnKick( user, channel, nick, msg );
					break;
					
				case "MODE":
					// 3.1.5 User mode - <nickname> *( ( "+" / "-" ) *( "i" / "w" / "o" / "O" / "r" ) )
					// 3.2.3 Channel mode - <channel> *( ( "-" / "+" ) *<modes> *<modeparams> )
					target = IRCUtils.NextParam(line, ref index);
					
					if ( IsValidChannelName( target ) )
					{
						OnChannelModeChange( user, target );
					}
					break;
					
				case "KILL":
					// 3.7.1 Kill - <nickname> <comment>
					nick = IRCUtils.NextParam(line, ref index);
					msg  = IRCUtils.NextAll(line, ref index);
					
					OnKill( user, nick, msg );
					break;
			}
		}
		
		const int RPL_WELCOME    = 001;
		const int RPL_NAMREPLY   = 353;
		const int RPL_ENDOFNAMES = 366;
		const int RPL_TRYAGAIN   = 263;
		
		const int ERR_NOSUCHNICK     = 401;
		const int ERR_NICKNAMEINUSE  = 433;
		const int ERR_NICKCOLLISION  = 436;
		const int ERR_USERSDONTMATCH = 502;
		
		void ParseReply( string prefix, int code, string line, int index ) 
		{
		    string nick, channel, chanType;
		    string target = IRCUtils.NextParam(line, ref index);
		    string[] names;
		    
		    if (code == RPL_WELCOME) {
				OnRegistered();
		    } else if (code == RPL_NAMREPLY) {
		        // RPL_NAMREPLY - ( "=" / "*" / "@" ) <channel> :[ "@" / "+" ] <nick> *( " " [ "@" / "+" ] <nick> )
				chanType = IRCUtils.NextParam(line, ref index);
				channel  = IRCUtils.NextParam(line, ref index);
				names    = IRCUtils.NextAll(  line, ref index).Split(IRCUtils.SPACE);
					
				OnNames( channel, names, false );
		    } else if (code == RPL_ENDOFNAMES) {
		        // RPL_ENDOFNAMES - <channel> :End of NAMES list
				channel  = IRCUtils.NextParam(line, ref index);
				OnNames( channel, new string[0], true );
		    } else if (code == ERR_NICKNAMEINUSE || code == ERR_NICKCOLLISION) {
		        // ERR_NICKNAMEINUSE - <nick> :Nickname is already in use
		        // ERR_NICKCOLLISION - <nick> :Nickname collision KILL from <user>@<host>
				nick = IRCUtils.NextParam(line, ref index);
				OnNickError( nick );
		    } else if (code >= ERR_NOSUCHNICK && code <= ERR_USERSDONTMATCH) {
		        OnError(code, IRCUtils.NextAll(line, ref index) );
			}
		}
		#endregion


		#region Utilities
		// Odd chars that IRC allows in nicknames 
		const string Special = "\\[\\]\\`_\\^\\{\\|\\}";
		const string NickChars = "[" + Special + "a-zA-Z][\\w\\-" + Special + "]{0,8}";

		// Regex that matches a legal IRC nick 
		static readonly Regex nickRegex = new Regex( NickChars );
		const string CHAN_PREFIXES = "#!+&";
		
		static bool IsValidChannelName( string channel ) 
		{
			if( IsEmpty(  channel ) ) return false;
			if( HasSpace( channel ) ) return false;

			// valid channels start with #, !, + or &
			return channel.Length > 1 && CHAN_PREFIXES.IndexOf(channel[0]) >= 0;
		}

		static bool IsValidNick( string nick ) 
		{
			if( IsEmpty(  nick ) ) return false;
			if( HasSpace( nick ) ) return false;

			return nickRegex.IsMatch( nick );
		}

		static bool IsEmpty( string str ) 
		{
			return str == null || str.Trim().Length == 0;
		}

		static bool HasSpace( string str ) 
		{
			return str.IndexOf( ' ' ) != -1;
		}
		#endregion
	}
}
