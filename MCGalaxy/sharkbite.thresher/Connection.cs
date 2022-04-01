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
		TcpClient client;
		StreamReader reader;
		StreamWriter writer;
		Encoding encoding;
		Random rnd = new Random();

		/// <summary>
		/// Prepare a connection to an IRC server but do not open it.
		/// </summary>
		/// <param name="textEncoding">The text encoding for the incoming stream.</param>
		public Connection(Encoding textEncoding)
		{
			encoding = textEncoding;
			RegisterDelegates();
		}
		
		
		/// <summary> The IRC server hostname </summary>
		/// <value>The full hostname such as irc.gamesnet.net</value>
		public string Hostname;
		/// <summary> The TCP/IP port the IRC listens server listens on. </summary>
		/// <value> Normally should be set to 6667. </value>
		public int Port = 6667;	
		/// <summary> Whether to connect using SSL or not </summary>		
		public bool UseSSL;
		/// <summary> The user's current nick name. </summary>
		public string Nick;	
		/// <summary> The user's 'real' name. </summary>
		public string RealName;		
		/// <summary> The user's machine logon name. </summary>
		public string UserName;		
		/// <summary> The password for this server. These are seldomly used. Set to '*'  </summary>
		public string ServerPassword = "*";
				
		/// <summary> Whether this client is connected and has successfully registered. </summary>
		public bool Registered;
		/// <summary> Whether a TCP/IP connection has been established with the IRC server. </summary>
		public bool Connected;

		/// <summary> Respond to IRC keep-alives. </summary>
		/// <param name="message">The message that should be echoed back</param>
		void KeepAlive(string message)
		{
			SendPong( message );
		}

		void MyNickChanged(string user, string newNick)
		{
			string nick = ExtractNick(user);
			if ( Nick == nick )
			{
				Nick = newNick;
			}
		}
		
		void HandleRegistered()
		{
			Registered = true;
			OnRegistered -= HandleRegistered;
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
		
		
		void HandleNickError( string badNick, string reason )
		{
			if( Registered ) return;
			
			// If this is our initial connection attempt
			Nick = GetNewNick();
			SendNick(Nick);
			SendUser();
		}
		
		void RegisterDelegates()
		{
			OnPing += KeepAlive;
			OnNick += MyNickChanged;
			OnNickError += HandleNickError;
			OnRegistered += HandleRegistered;
		}

		/// <summary>
		/// Read in message lines from the IRC server and send them to a parser for processing.
		/// Discards CTCP and DCC messages if these protocols are not enabled.
		/// </summary>
		public void ReceiveIRCMessages()
		{
			string line;
			try
			{
				while ( (line = reader.ReadLine() ) != null )
				{
					if( IsDccRequest( line ) ) continue;
					if( IsCtcpMessage( line) ) continue;

					Parse( line );
				}
			}
			finally
			{
				//The connection to the IRC server has been closed either
				//by client request or the server itself closed the connection.
				client.Close();
				Registered = false;
				Connected  = false;
			}
		}
		
		Stream MakeDataStream() 
		{
			Stream raw = client.GetStream();
			if (!UseSSL) return raw;
			return MCGalaxy.Network.HttpUtil.WrapSSLStream( raw, Hostname );
		}
		
		/// <summary> Connect to the IRC server and start listening for messages on a new thread. </summary>
		/// <exception cref="SocketException">If a connection cannot be established with the IRC server</exception>
		public void Connect()
		{
			lock ( this )
			{
				if( Connected ) throw new InvalidOperationException("Connection with IRC server already opened.");
				client = new TcpClient();
				client.Connect( Hostname, Port );
				Stream s  = MakeDataStream();
				Connected = true;
				
				writer = new StreamWriter( s, encoding );
				writer.AutoFlush = true;
				reader = new StreamReader( s, encoding );
				
				SendPass(ServerPassword);
				// NOTE: The following two commands may fail if
				//   nick is already in use by another IRC user
				SendNick(Nick);
				SendUser();
			}
		}

		public void Disconnect( string reason )
		{
			lock ( this )
			{
				SendQuit( reason );
				client.Close();
			}
		}
		

		const string ctcpTypes = "(FINGER|USERINFO|VERSION|SOURCE|CLIENTINFO|ERRMSG|PING|TIME)";
		static Regex ctcpRegex = new Regex(":([^ ]+) [A-Z]+ [^:]+:\u0001" + ctcpTypes + "([^\u0001]*)\u0001", RegexOptions.Compiled | RegexOptions.Singleline );
		/// <summary> Test if the message contains CTCP commands. </summary>
		/// <param name="message">The raw message from the IRC server</param>
		/// <returns>True if this is a Ctcp request or reply.</returns>
		static bool IsCtcpMessage( string message ) {
			return ctcpRegex.IsMatch( message );
		}
		
		static Regex dccMatchRegex = new Regex(":([^ ]+) PRIVMSG [^:]+:\u0001DCC (CHAT|SEND|GET|RESUME|ACCEPT)[^\u0001]*\u0001", RegexOptions.Compiled | RegexOptions.Singleline );
		/// <summary> Test if the message contains a DCC request. </summary>
		/// <param name="message">The raw message from the IRC server</param>
		/// <returns>True if this is a DCC request.</returns>
		static bool IsDccRequest( string message ) {
			return dccMatchRegex.IsMatch( message );
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

		void SendQuit(string reason) 
		{
			if ( IsEmpty( reason ) ) 
				throw new ArgumentException("Quit reason cannot be null or empty.");
			
			SendCommand("QUIT :" + reason);
		}

		void SendPong(string message) 
		{
			SendCommand("PONG " + message);
		}

		void SendPass(string password) 
		{
			SendCommand("PASS " + password);
		}

		public void SendJoin(string channel) 
		{
			if ( IsEmpty( channel ) )
				throw new ArgumentException(channel + " is not a valid channel name.");
			
			SendCommand("JOIN " + channel);
		}

		public void SendJoin(string channel, string password) 
		{
			if ( IsEmpty( password ) ) 
				throw new ArgumentException("Password cannot be empty or null.");
			if ( IsEmpty( channel ) )
				throw new ArgumentException("Channel name cannot be empty or null.");
				
			SendCommand("JOIN " + channel + " " + password);
		}

		public void SendNick(string nick) 
		{
			if ( !IsValidNick( nick ) )
				throw new ArgumentException(nick + " is not a valid nickname.");
				
			SendCommand("NICK " + nick);
		}

		public void SendNames(string channel) 
		{
			if ( IsEmpty( channel ) ) 
				throw new ArgumentException("Channel name cannot be null or empty.");
			
			SendCommand("NAMES " + channel);
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
		/// Messages that are not handled by other events and are not errors.
		/// </summary>
		public event ReplyEventHandler OnReply;
		/// <summary>
		/// Error messages from the IRC server.
		/// </summary>
		public event ErrorMessageEventHandler OnError;
		/// <summary>
		/// The user tried to change his nick but it failed.
		/// </summary>
		public event NickErrorEventHandler OnNickError;
		/// <summary>
		/// A server keep-alive message.
		/// </summary>
		public event PingEventHandler OnPing;
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
		/// The user has been invited to a channel.
		/// </summary>
		public event InviteEventHandler OnInvite;
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
		private const string ACTION = "\u0001ACTION";
		private readonly char[] Separator = new char[] { ' ' };
		private readonly Regex replyRegex = new Regex("^:([^\\s]*) ([\\d]{3}) ([^\\s]*) (.*)", RegexOptions.Compiled | RegexOptions.Singleline);

		void Parse(string message ) 
		{
			string[] tokens = message.Split( Separator );
			if( tokens[0] == "PING" ) 
			{
				OnPing( GetSuffix( tokens, 1 ) );
			}
			else if( tokens[0] == "NOTICE" ) 
			{
				OnPrivateNotice( "", GetSuffix( tokens, 2 ) );
			}
			else if ( tokens[0] == "ERROR" ) 
			{
				OnError( ReplyCode.IrcServerError, GetSuffix( tokens, 1 ) );
			}
			else if( replyRegex.IsMatch( message ) )
			{
				ParseReply( tokens );
			}
			else 
			{
				ParseCommand( tokens );
			}
		}

		/// <summary>
		/// Parse the message and call the callback methods on the listeners.
		/// </summary>
		/// <param name="tokens">The text received from the IRC server</param>
		void ParseCommand(string[] tokens ) 
		{	
			// Remove leading colon from user info
			string user = RemoveLeadingColon( tokens[0] );

			switch( tokens[1] ) 
			{
				case "NOTICE":
					if( IsValidChannelName( tokens[2] ) )
					{			
						OnPublicNotice( user, tokens[2], GetSuffix( tokens, 3 ) );
					}
					else 
					{
						OnPrivateNotice( user, GetSuffix( tokens, 3 ) );
					}
					break;
				case "JOIN":
					OnJoin( user, RemoveLeadingColon( tokens[2] ) );
					break;
				case "PRIVMSG":
					tokens[3] = RemoveLeadingColon( tokens[3] );
					if( tokens[3] == ACTION ) 
					{
						if( IsValidChannelName( tokens[2] ) )
						{
							int last = tokens.Length - 1;
							tokens[ last ] = RemoveTrailingQuote( tokens[last] );
							OnAction( user, tokens[2], CondenseStrings( tokens, 4) );
						}
						else 
						{
							int last = tokens.Length - 1;
							tokens[ last ] = RemoveTrailingQuote( tokens[last] );
							OnPrivateAction( user, CondenseStrings( tokens, 4) );
						}
					}
					else if( IsValidChannelName( tokens[2] ) )
					{
						OnPublic( user, tokens[2], CondenseStrings( tokens, 3) );
					}
					else 
					{
						OnPrivate( user, CondenseStrings( tokens, 3) );
					}
					break;
				case "NICK":
					OnNick(	user, RemoveLeadingColon( tokens[2] ) );
					break;
				case "PART":
					OnPart( user, tokens[2], GetSuffix( tokens, 3 ) );
					break;
				case "QUIT":
					OnQuit( user, GetSuffix( tokens, 2 ) );
					break;
				case "INVITE":
					if( OnInvite != null ) 
					{
						OnInvite( user, RemoveLeadingColon( tokens[3] ) );
					}
					break;
				case "KICK":
					OnKick( user, tokens[2], tokens[3], GetSuffix( tokens, 4 ) );
					break;
				case "MODE":
					if ( IsValidChannelName( tokens[2] ) )
					{
						OnChannelModeChange( tokens[0], tokens[2] );
					}
					break;
				case "KILL":
					OnKill( user, tokens[2], GetSuffix( tokens, 3 ) );
					break;
				default: 
					OnError( ReplyCode.UnparseableMessage, GetSuffix( tokens, 0 ) );
					break;
			}
		}
		
		void ParseReply( string[] tokens ) 
		{
			ReplyCode code = (ReplyCode) int.Parse( tokens[1], CultureInfo.InvariantCulture );
			tokens[3] = RemoveLeadingColon( tokens[3] );
			switch( code )
			{
				//Messages sent upon successful registration 
				case ReplyCode.RPL_WELCOME:
				case ReplyCode.RPL_YOURESERVICE:
					OnRegistered();
					break;	
				case ReplyCode.RPL_NAMREPLY:
					if ( OnNames != null ) 
					{
						tokens[5] = RemoveLeadingColon( tokens[5] );
						int numberOfUsers = tokens.Length - 5;
						string[] users = new string[ numberOfUsers ];
						Array.Copy( tokens, 5 , users, 0 , numberOfUsers);
						OnNames( tokens[4], 
							users,
							false );
					}
					break;
				case ReplyCode.RPL_ENDOFNAMES:
					OnNames( tokens[3], new string[0], true );
					break;
				case ReplyCode.ERR_NICKNAMEINUSE:
				case ReplyCode.ERR_NICKCOLLISION:
					OnNickError( tokens[3], GetSuffix( tokens, 4 ) );
					break;
				default:
					HandleDefaultReply( code, tokens );
					break;
			}
		}

		void HandleDefaultReply( ReplyCode code, string[] tokens ) 
		{
			if (code >= ReplyCode.ERR_NOSUCHNICK && code <= ReplyCode.ERR_USERSDONTMATCH) 
			{
				OnError(code, GetSuffix( tokens, 3 ) );
			}
			else if( OnReply != null )
			{
				OnReply(code, GetSuffix( tokens, 3 ) );
			}
		}
		
		/// <summary>
		/// Turn an array of strings back into a single string.
		/// </summary>
		static string CondenseStrings( string[] strings, int start ) 
		{
			if( strings.Length == start + 1 ) 
			{
				return strings[start];
			}
			else 
			{
				return String.Join(" ", strings, start, (strings.Length - start) );
			}
		}
		
		static string RemoveLeadingColon( string text ) 
		{
			if( text[0] == ':' )
			{
				return text.Substring(1);
			}
			return text;
		}

		static string GetSuffix( string[] strings, int start ) 
		{
			if( start >= strings.Length ) return "";

			return RemoveLeadingColon( CondenseStrings( strings, start ) );
		}
		
		/// <summary>
		/// Strip off the trailing CTCP quote.
		/// </summary>
		static string RemoveTrailingQuote( string text ) 
		{
			return text.Substring(0, text.Length -1 );		
		}
		#endregion


		#region Utilities
		// Odd chars that IRC allows in nicknames 
		const string Special = "\\[\\]\\`_\\^\\{\\|\\}";
		const string NickChars = "[" + Special + "a-zA-Z][\\w\\-" + Special + "]{0,8}";

		// Regex that matches a legal IRC nick 
		static readonly Regex nickRegex = new Regex( NickChars ); 
		//Regex to create a UserInfo from a string
		static readonly Regex nameSplitterRegex = new Regex("[!@]",RegexOptions.Compiled | RegexOptions.Singleline );
		const string ChannelPrefix = "#!+&";

		public static string ExtractNick( string fullUserName ) 
		{
			if( IsEmpty( fullUserName ) ) return "";

			Match match = nameSplitterRegex.Match( fullUserName );
			if( match.Success ) 
			{
				string[] parts = nameSplitterRegex.Split( fullUserName );
				return parts[0];
			}
			return fullUserName;
		}

		static bool IsValidChannelName( string channel ) 
		{
			if( IsEmpty(  channel ) ) return false;
			if( HasSpace( channel ) ) return false;

			// valid channels start with #, !, + or &
			return channel.Length > 1 && ChannelPrefix.IndexOf(channel[0]) >= 0;
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
