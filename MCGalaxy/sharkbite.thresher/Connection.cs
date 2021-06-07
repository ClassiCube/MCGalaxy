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
		Listener listener;
		Sender sender;
		StreamReader reader;
		StreamWriter writer;
		//Connected and registered with IRC server
		bool registered;
		//TCP/IP connection established with IRC server
		bool connected;
		bool handleNickFailure = true;
		Encoding encoding;

		/// <summary>
		/// Prepare a connection to an IRC server but do not open it.
		/// </summary>
		/// <param name="textEncoding">The text encoding for the incoming stream.</param>
		public Connection( Encoding textEncoding )
		{
			sender = new Sender( this );
			listener = new Listener( );
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
		/// <summary>
		/// Set's the user's initial IRC mode mask. Set to 0 to recieve wallops
		/// and be invisible. Set to 4 to be invisible and not receive wallops.
		/// </summary>
		/// <value>A number mask such as 0 or 4.</value>
		public string ModeMask = "4";		
		/// <summary> The user's nick name. </summary>
		/// <value>A string which conforms to the IRC nick standard.</value>
		public string Nick;	
		/// <summary> The user's 'real' name. </summary>
		/// <value>A short string with any legal characters.</value>
		public string RealName;		
		/// <summary> The user's machine logon name. </summary>
		/// <value>A short string with any legal characters.</value>
		public string UserName;		
		/// <summary> The password for this server. These are seldomly used. Set to '*'  </summary>
		/// <value>A short string with any legal characters.</value>
		public string ServerPassword = "*";
		
		
		/// <summary>
		/// A read-only property indicating whether the connection
		/// has been opened with the IRC server and the
		/// client has been successfully registered.
		/// </summary>
		/// <value>True if the client is connected and registered.</value>
		public bool Registered { get { return registered; } }
		/// <summary>
		/// A read-only property indicating whether a connection
		/// has been opened with the IRC server (but not whether
		/// registration has succeeded).
		/// </summary>
		/// <value>True if the client is connected.</value>
		public bool Connected { get { return connected; } }

		/// <summary>
		/// The object used to send commands to the IRC server.
		/// </summary>
		/// <value>Read-only Sender.</value>
		public Sender Sender { get { return sender; } }
		/// <summary>
		/// The object that parses messages and notifies the appropriate delegate.
		/// </summary>
		/// <value>Read only Listener.</value>
		public Listener Listener { get { return listener; } }

		/// <summary>
		/// Respond to IRC keep-alives.
		/// </summary>
		/// <param name="message">The message that should be echoed back</param>
		void KeepAlive(string message)
		{
			sender.Pong( message );
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
			registered = true;
			listener.OnRegistered -= OnRegistered;
		}
		
		void OnNickError( string badNick, string reason )
		{
			//If this is our initial connection attempt
			if( !registered && handleNickFailure )
			{
				NameGenerator generator = new NameGenerator();
				string nick;
				do
				{
					nick = generator.MakeName();
				}
				while(!Rfc2812Util.IsValidNick(nick) || nick.Length == 1);
				//Try to reconnect
				Sender.Register(nick);
			}
		}
		
		void RegisterDelegates()
		{
			listener.OnPing += KeepAlive;
			listener.OnNick += MyNickChanged;
			listener.OnNickError += OnNickError;
			listener.OnRegistered += OnRegistered;
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
					try
					{
						if( IsDccRequest( line ) ) continue;
						if( IsCtcpMessage( line) ) continue;
						
						listener.Parse( line );
					}
					catch( ThreadAbortException )
					{
						Thread.ResetAbort();
						//This exception is raised when the Thread
						//is stopped at user request, i.e. via Disconnect()
						//This will stop the read loop and close the connection.
						break;
					}
				}
			}
			catch (IOException)
			{
				//Trap a connection failure
				listener.Error( ReplyCode.ConnectionFailed, "Connection to server unexpectedly failed.");
			}
			catch (Exception ex)
			{
				listener.Error( ReplyCode.ConnectionFailed, "Unhandled error: " + ex);
			}
			finally
			{
				//The connection to the IRC server has been closed either
				//by client request or the server itself closed the connection.
				client.Close();
				registered = false;
				connected = false;
				listener.Disconnected();
			}
		}
		/// <summary>
		/// Send a message to the IRC server and clear the command buffer.
		/// </summary>
		internal void SendCommand( StringBuilder command)
		{
			try
			{
				writer.WriteLine( command.ToString() );
			}
			catch( Exception )
			{
			}
			command.Remove(0, command.Length );
		}

		Stream MakeDataStream() 
		{
			Stream raw = client.GetStream();
			if (!UseSSL) return raw;
			return MCGalaxy.Network.HttpUtil.WrapSSLStream( raw, Hostname );
		}
		
		/// <summary>
		/// Connect to the IRC server and start listening for messages on a new thread.
		/// </summary>
		/// <exception cref="SocketException">If a connection cannot be established with the IRC server</exception>
		public void Connect()
		{
			lock ( this )
			{
				if( connected ) throw new InvalidOperationException("Connection with IRC server already opened.");
				client = new TcpClient();
				client.Connect( Hostname, Port );
				Stream s  = MakeDataStream();
				connected = true;
				
				writer = new StreamWriter( s, encoding );
				writer.AutoFlush = true;
				reader = new StreamReader( s, encoding );
				sender.RegisterConnection();
			}
		}

		public void Disconnect( string reason )
		{
			lock ( this )
			{
				sender.Quit( reason );
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
	}
}
