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
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace Sharkbite.Irc
{
	public delegate void ReplyEventHandler( ReplyCode code, string message );
	public delegate void ErrorMessageEventHandler( ReplyCode code, string message );
	public delegate void NickErrorEventHandler( string badNick, string reason ) ;
	public delegate void PingEventHandler( string message );
	public delegate void RegisteredEventHandler();
	public delegate void DisconnectedEventHandler();
	public delegate void PublicNoticeEventHandler( UserInfo user, string channel, string notice );
	public delegate void PrivateNoticeEventHandler( UserInfo user, string notice );
	public delegate void JoinEventHandler( UserInfo user, string channel );
	public delegate void PublicActionEventHandler( UserInfo user, string channel, string description );
	public delegate void PrivateActionEventHandler( UserInfo user, string description );
	public delegate void PublicMessageEventHandler( UserInfo user, string channel, string message );
	public delegate void PrivateMessageEventHandler( UserInfo user, string message );
	public delegate void NickEventHandler( UserInfo user, string newNick );
	public delegate void PartEventHandler( UserInfo user, string channel, string reason);
	public delegate void QuitEventHandler( UserInfo user, string reason);
	public delegate void InviteEventHandler( UserInfo user, string channel );
	public delegate void KickEventHandler( UserInfo user, string channel, string kickee, string reason );
	public delegate void NamesEventHandler( string channel, string[] nicks, bool last );
	public delegate void ChannelModeChangeEventHandler( UserInfo who, string channel );
	public delegate void KillEventHandler( UserInfo user, string nick, string reason );
	
	/// <summary>
	/// This class parses messages received from the IRC server and
	/// raises the appropriate event. 
	/// </summary>
	public sealed class Listener
	{
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

		private const string PING = "PING";
		private const string ERROR = "ERROR";
		private const string NOTICE = "NOTICE";
		private const string JOIN = "JOIN";
		private const string PRIVMSG = "PRIVMSG";
		private const string NICK = "NICK";
		private const string TOPIC = "TOPIC";
		private const string PART = "PART";
		private const string QUIT = "QUIT";
		private const string INVITE = "INVITE";
		private const string KICK = "KICK";
		private const string MODE = "MODE";
		private const string KILL = "KILL";
		private const string ACTION = "\u0001ACTION";
		private readonly char[] Separator = new char[] { ' ' };
		private readonly Regex channelPattern;
		private readonly Regex replyRegex;

		/// <summary> Create an instance ready to parse incoming messages. </summary>
		public Listener() 
		{
			channelPattern = new Regex( "([#!+&]\\w+)", RegexOptions.Compiled | RegexOptions.Singleline);
			replyRegex = new Regex("^:([^\\s]*) ([\\d]{3}) ([^\\s]*) (.*)", RegexOptions.Compiled | RegexOptions.Singleline);
		}

		internal void Parse(string message ) 
		{
			string[] tokens = message.Split( Separator );
			if( tokens[0] == PING ) 
			{
				if( OnPing != null ) 
				{
					tokens[1] = RemoveLeadingColon( tokens[1] );
					OnPing( CondenseStrings(tokens, 1) );
				}
			}
			else if( tokens[0] == NOTICE ) 
			{
				if( OnPrivateNotice != null )
				{
					OnPrivateNotice(
						UserInfo.Empty,
						CondenseStrings( tokens, 2) );
				}
			}
			else if ( tokens[0] == ERROR ) 
			{
				tokens[1] = RemoveLeadingColon( tokens[1] );
				Error( ReplyCode.IrcServerError, CondenseStrings(tokens, 1) );
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
		/// Tell listeners that an error has been encountered
		/// </summary>
		internal void Error( ReplyCode code, string message ) 
		{
			if( OnError != null )
			{
				OnError( code, message );
			}
		}

		/// <summary>
		/// Parse the message and call the callback methods on the listeners.
		/// </summary>
		/// <param name="tokens">The text received from the IRC server</param>
		void ParseCommand(string[] tokens ) 
		{	
			//Remove colon user info string
			tokens[0] = RemoveLeadingColon( tokens[0] );
			switch( tokens[1] ) 
			{
				case NOTICE:		
					tokens[3] = RemoveLeadingColon( tokens[3] );
					if( Rfc2812Util.IsValidChannelName( tokens[2] ) )
					{			
						if( OnPublicNotice != null )
						{
							OnPublicNotice(
								Rfc2812Util.UserInfoFromString( tokens[0] ),
								tokens[2],
								CondenseStrings( tokens, 3) );
						}
					}
					else 
					{
						if( OnPrivateNotice != null )
						{
							OnPrivateNotice(
								Rfc2812Util.UserInfoFromString( tokens[0] ),
								CondenseStrings( tokens, 3) );
						}
					}
					break;
				case JOIN:
					if( OnJoin != null )
					{
						OnJoin( Rfc2812Util.UserInfoFromString( tokens[0] ), RemoveLeadingColon( tokens[2] ) );
					}
					break;
				case PRIVMSG:
					tokens[3] = RemoveLeadingColon( tokens[3] );
					if( tokens[3] == ACTION ) 
					{
						if( Rfc2812Util.IsValidChannelName( tokens[2] ) )
						{
							if( OnAction != null ) 
							{
								int last = tokens.Length - 1;
								tokens[ last ] = RemoveTrailingQuote( tokens[last] );
								OnAction( Rfc2812Util.UserInfoFromString( tokens[0] ),tokens[2],CondenseStrings( tokens, 4) );
							}
						}
						else 
						{
							if( OnPrivateAction != null ) 
							{
								int last = tokens.Length - 1;
								tokens[ last ] = RemoveTrailingQuote( tokens[last] );
								OnPrivateAction( Rfc2812Util.UserInfoFromString( tokens[0] ),CondenseStrings( tokens, 4) );
							}
						}
					}
					else if( channelPattern.IsMatch( tokens[2] ) )
					{
						if( OnPublic != null )
						{
							OnPublic(Rfc2812Util.UserInfoFromString( tokens[0] ),tokens[2],CondenseStrings( tokens, 3) );
						}
					}
					else 
					{
						if( OnPrivate != null )
						{
							OnPrivate(Rfc2812Util.UserInfoFromString( tokens[0] ), CondenseStrings( tokens, 3) );
						}
					}
					break;
				case NICK:
					if( OnNick != null )
					{
						OnNick(	Rfc2812Util.UserInfoFromString( tokens[0] ), RemoveLeadingColon( tokens[2] ) );
					}
					break;
				case PART:
					if( OnPart != null )					
					{
						OnPart(
							Rfc2812Util.UserInfoFromString( tokens[0] ), 
							tokens[2],
							tokens.Length >= 4 ? RemoveLeadingColon(CondenseStrings( tokens, 3)) : "" );
					}
					break;
				case QUIT:
					if( OnQuit != null ) 
					{
						tokens[2] = RemoveLeadingColon( tokens[2] );
						OnQuit( Rfc2812Util.UserInfoFromString( tokens[0] ), CondenseStrings( tokens, 2) );
					}
					break;
				case INVITE:
					if( OnInvite != null ) 
					{
						OnInvite(
							Rfc2812Util.UserInfoFromString( tokens[0] ), RemoveLeadingColon( tokens[3] ) );
					}
					break;
				case KICK:
					if( OnKick != null )
					{	
						tokens[4] = RemoveLeadingColon( tokens[4] );
						OnKick(Rfc2812Util.UserInfoFromString( tokens[0] ),tokens[2],tokens[3], CondenseStrings( tokens, 4) );
					}
					break;
				case MODE:
					if( channelPattern.IsMatch( tokens[2] ) )
					{
						if( OnChannelModeChange != null ) 
						{
							UserInfo who = Rfc2812Util.UserInfoFromString( tokens[0] );
							OnChannelModeChange( who, tokens[2] );
						}
					}
					break;
				case KILL:
					if( OnKill != null )
					{
						string reason = "";
						if( tokens.Length >= 4 ) 
						{
							tokens[3] = RemoveLeadingColon( tokens[3] );
							reason = CondenseStrings( tokens, 3 );
						}
						OnKill( Rfc2812Util.UserInfoFromString( tokens[0] ), tokens[2], reason );
					}
					break;
				default: 
					if( OnError != null ) 
					{
						OnError( ReplyCode.UnparseableMessage, CondenseStrings( tokens, 0 ) );
					}
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
					if( OnRegistered != null ) 
					{
						OnRegistered();
					}
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
					if( OnNames != null ) 
					{
						OnNames( tokens[3], new string[0], true );
					}
					break;
				case ReplyCode.ERR_NICKNAMEINUSE:
				case ReplyCode.ERR_NICKCOLLISION:
					if ( OnNickError != null ) 
					{
						tokens[4] = RemoveLeadingColon( tokens[4] );
						OnNickError( tokens[3], CondenseStrings( tokens, 4) );
					}
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
				if( OnError != null )
				{
					OnError(code, CondenseStrings( tokens, 3) );
				}
			}
			else if( OnReply != null )
			{
				OnReply(code, CondenseStrings( tokens, 3) );
			}
		}
		
		/// <summary>
		/// Turn an array of strings back into a single string.
		/// </summary>
		string CondenseStrings( string[] strings, int start ) 
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
		
		string RemoveLeadingColon( string text ) 
		{
			if( text[0] == ':' )
			{
				return text.Substring(1);
			}
			return text;
		}
		
		/// <summary>
		/// Strip off the trailing CTCP quote.
		/// </summary>
		string RemoveTrailingQuote( string text ) 
		{
			return text.Substring(0, text.Length -1 );		
		}
	}
}
