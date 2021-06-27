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
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

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

		/// <summary>
		/// Prepare a connection to an IRC server but do not open it.
		/// </summary>
		/// <param name="textEncoding">The text encoding for the incoming stream.</param>
		public Connection( Encoding textEncoding )
		{
			Listener = new Listener( );
			RegisterDelegates();
			encoding = textEncoding;
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

		/// <summary> The object that parses messages and notifies the appropriate delegate.
		public Listener Listener;

		/// <summary> Respond to IRC keep-alives. </summary>
		/// <param name="message">The message that should be echoed back</param>
		void KeepAlive(string message)
		{
			SendPong( message );
		}

		void MyNickChanged(UserInfo user, string newNick)
		{
			if ( Nick == user.Nick )
			{
				Nick = newNick;
			}
		}
		
		void OnRegistered()
		{
			Registered = true;
			Listener.OnRegistered -= OnRegistered;
		}
		
		string GetNewNick()
		{
			// prefer just adding _ to end of real nick
			if (Nick.Length < MAX_NICKNAME_LEN) return Nick + "_";
			
			NameGenerator generator = new NameGenerator();
			string name;
			do
			{
				name = generator.MakeName();
			}
			while (!Rfc2812Util.IsValidNick(name) || name.Length == 1);
			
			return name;
		}
		
		
		void OnNickError( string badNick, string reason )
		{
			if( Registered ) return;
			
			// If this is our initial connection attempt
			Nick = GetNewNick();
			SendNick(Nick);
			SendUser();
		}
		
		void RegisterDelegates()
		{
			Listener.OnPing += KeepAlive;
			Listener.OnNick += MyNickChanged;
			Listener.OnNickError += OnNickError;
			Listener.OnRegistered += OnRegistered;
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
						
					Listener.Parse( line );
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
		public static bool IsCtcpMessage( string message ) {
			return ctcpRegex.IsMatch( message );
		}
		
		static Regex dccMatchRegex = new Regex(":([^ ]+) PRIVMSG [^:]+:\u0001DCC (CHAT|SEND|GET|RESUME|ACCEPT)[^\u0001]*\u0001", RegexOptions.Compiled | RegexOptions.Singleline );
		/// <summary> Test if the message contains a DCC request. </summary>
		/// <param name="message">The raw message from the IRC server</param>
		/// <returns>True if this is a DCC request.</returns>
		public static bool IsDccRequest( string message ) {
			return dccMatchRegex.IsMatch( message );
		}
		
		
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

		bool IsEmpty( string aString ) 
		{
			return aString == null || aString.Trim().Length == 0;
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
			if ( !Rfc2812Util.IsValidNick( nick ) )
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
	}
}
