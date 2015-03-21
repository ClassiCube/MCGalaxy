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
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Sharkbite.Irc
{
	/// <summary>
	/// DccListener listens for incoming DCC requests on any Connection in which
	/// DCC is enabled. This class follows the singleton pattern and there so there
	/// is only a single instance which listens to all connections.
	/// </summary>
	public sealed class DccListener
	{
		/// <summary>
		/// A user from any Connection has sent a request to open a DCC
		/// chat session.
		/// </summary>
		public event DccChatRequestEventHandler OnDccChatRequest;
		/// <summary>
		/// A remote user has sent a request to send a file.
		/// </summary>
		public event DccSendRequestEventHandler OnDccSendRequest;
		/// <summary>
		/// A remote user requests that he be sent a file.
		/// </summary>
		public event DccGetRequestEventHandler OnDccGetRequest;

		private static DccListener listener;
		private static readonly object lockObject = new object();
		//Checks if a line is likely a DCC request
		private static readonly Regex dccMatchRegex;
		//Split the DCC into space separated tokens
		private readonly Regex tokenizer;
		//Extract out the DCC specific info
		private readonly Regex parser;
		//The values of the DCC string tokens
		private const int Action = 0;
		private const int FileName = 1;
		private const int Address = 2;
		private const int Port = 3;
		private const int FileSize = 4;
		//DCC action types
		private const string CHAT = "CHAT";
		private const string SEND = "SEND";
		private const string GET = "GET";
		private const string RESUME = "RESUME";
		private const string ACCEPT = "ACCEPT";

		static DccListener() 
		{
			dccMatchRegex = new Regex(":([^ ]+) PRIVMSG [^:]+:\u0001DCC (CHAT|SEND|GET|RESUME|ACCEPT)[^\u0001]*\u0001", RegexOptions.Compiled | RegexOptions.Singleline );
		}

		private DccListener()
		{
			parser = new Regex(":([^ ]+) PRIVMSG [^:]+:\u0001DCC ([^\u0001]*)\u0001", RegexOptions.Compiled | RegexOptions.Singleline );
			tokenizer = new Regex("[\\s]+", RegexOptions.Compiled | RegexOptions.Singleline);
		}

		/// <summary>
		/// Determine if the SEND or GET message included Turbo
		/// mode.
		/// </summary>
		/// <param minimumTokens="tokens"></param>
		/// <returns>True if it did.</returns>
		private bool IsTurbo( int minimumTokens, string[] tokens ) 
		{
			if( tokens.Length <= minimumTokens ) 
			{
				return false;
			}
			return tokens[ minimumTokens ] == "T";
		}

		internal void Parse( Connection connection, string message ) 
		{
			Debug.WriteLineIf( DccUtil.DccTrace.TraceInfo, "[" + Thread.CurrentThread.Name +"] DccListener::Parse()");
			Match match = parser.Match( message );
			string requestor = match.Groups[1].ToString();
			string[] tokens =tokenizer.Split( match.Groups[2].ToString().Trim() );
			switch( tokens[Action] )
			{
				case CHAT:
					if( OnDccChatRequest != null ) 
					{
						//Test for sufficient number of arguments
						if( tokens.Length < 4 ) 
						{
							connection.Listener.Error( ReplyCode.UnparseableMessage, "Incorrect CHAT arguments: " + message );
							return;
						}
						//Send event
						DccUserInfo dccUserInfo = null;
						try
						{
							dccUserInfo = new DccUserInfo(
								connection,
								Rfc2812Util.ParseUserInfoLine( requestor ),
								new IPEndPoint( DccUtil.LongToIPAddress( tokens[ Address ]), int.Parse( tokens[ Port], CultureInfo.InvariantCulture ) ) );
						}
						catch( ArgumentException ae ) 
						{
							connection.Listener.Error( ReplyCode.BadDccEndpoint, "Invalid TCP/IP connection information sent." );
							return;
						}
						try 
						{
							OnDccChatRequest( dccUserInfo );
						}
						catch( ArgumentException ae ) 
						{
							connection.Listener.Error( ReplyCode.UnknownEncryptionProtocol, ae.ToString() );
						}
					}
					break;
				case SEND:
					//Test for sufficient number of arguments
					if( tokens.Length < 5 ) 
					{
						connection.Listener.Error( ReplyCode.UnparseableMessage, "Incorrect SEND arguments: " + message );
						return;
					}
					if( OnDccSendRequest != null ) 
					{
						DccUserInfo dccUserInfo = null;
						try
						{
							dccUserInfo = new DccUserInfo(
								connection,
								Rfc2812Util.ParseUserInfoLine( requestor ),
								new IPEndPoint( DccUtil.LongToIPAddress( tokens[ Address ]), int.Parse( tokens[ Port], CultureInfo.InvariantCulture ) ) );
						}
						catch( ArgumentException ae ) 
						{
							connection.Listener.Error( ReplyCode.BadDccEndpoint, ae.ToString() );
							return;
						}
						try
						{
							OnDccSendRequest( 
								dccUserInfo,
								tokens[FileName],
								int.Parse( tokens[FileSize], CultureInfo.InvariantCulture ),
								IsTurbo( 5, tokens) );
						}
						catch( ArgumentException ae ) 
						{
							connection.Listener.Error( ReplyCode.UnknownEncryptionProtocol, ae.ToString() );
						}
					}
					break;
				case GET:
					//Test for sufficient number of arguments
					if( tokens.Length < 2 ) 
					{
						connection.Listener.Error( ReplyCode.UnparseableMessage, "Incorrect GET arguments: " + message );
						return;
					}
					if( OnDccGetRequest != null ) 
					{
						try 
						{
							OnDccGetRequest(
								new DccUserInfo( 
								connection,
								Rfc2812Util.ParseUserInfoLine( requestor ) ),
								tokens[ FileName], 
								IsTurbo( 2, tokens) );
						}
						catch( ArgumentException ae ) 
						{
							connection.Listener.Error( ReplyCode.UnknownEncryptionProtocol, ae.ToString() );
						}
					}
					break;
				case ACCEPT:
					//Test for sufficient number of arguments
					if( tokens.Length < 4 ) 
					{
						connection.Listener.Error( ReplyCode.UnparseableMessage, "Incorrect DCC ACCEPT arguments: " + message );
						return;
					}
					//DccListener will try to handle Receive at correct file position
					try 
					{
						DccFileSession session = DccFileSessionManager.DefaultInstance.LookupSession( "C" + tokens[2] );
						session.OnDccAcceptReceived( long.Parse( tokens[3] , CultureInfo.InvariantCulture) );
					}
					catch( ArgumentException e ) 
					{
						connection.Listener.Error( ReplyCode.UnableToResume, e.ToString() );
					}
					break;
				case RESUME:
					//Test for sufficient number of arguments
					if( tokens.Length < 4 ) 
					{
						connection.Listener.Error( ReplyCode.UnparseableMessage, "Incorrect DCC RESUME arguments: " + message );
						return;
					}
					//DccListener will automatically handle Resume/Accept interaction
					try 
					{
						DccFileSession session = DccFileSessionManager.DefaultInstance.LookupSession( "S" + tokens[2] );
						session.OnDccResumeRequest( long.Parse( tokens[3], CultureInfo.InvariantCulture ) );
					}
					catch( ArgumentException e ) 
					{
						connection.Listener.Error( ReplyCode.UnableToResume, e.ToString() );
					}
					break;
				default: 
					connection.Listener.Error( ReplyCode.UnparseableMessage, message );
					Debug.WriteLineIf( DccUtil.DccTrace.TraceError, "[" + Thread.CurrentThread.Name +"] DccListener::Parse() Unknown DCC command");
					break;
			}
		}

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <returns>The instance of DccListener</returns>
		public static DccListener DefaultInstance
		{
			get 
			{
				//Create the singleton instance in a lazy manner.
				//Multiple Connection threads could call this at once
				//so it is synchronized.
				lock( lockObject ) 
				{
					if( listener == null ) 
					{
						Debug.WriteLineIf( DccUtil.DccTrace.TraceVerbose, "[" + Thread.CurrentThread.Name +"] DccListener::init");
						listener = new DccListener();
					}
				}
				return listener;
			}
		}

		/// <summary>
		/// Test if the message contains a DCC request.
		/// </summary>
		/// <param name="message">The raw message from the IRC server</param>
		/// <returns>True if this is a DCC request.</returns>
		public static bool IsDccRequest( string message ) 
		{
			return dccMatchRegex.IsMatch( message );
		}
	}
}
