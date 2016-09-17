/*
 * Thresher IRC client library
 * Copyright (C) 2002 Aaron Hunter <thresher@sharkbite.org>
 *
 * This program is free software, you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 * 
 * See the gpl.txt file located in the top-level-directory of
 * the archive of this library for complete text of license.
*/

namespace Sharkbite.Irc
{


	/// <summary>
	/// Numeric message codes taken from RFC 2812
	/// </summary>

	public enum ReplyCode: int
	{

		/// <summary>
		/// IRC: Welcome to the Internet Relay Networ [nick]![user]@[host]
		/// 
		/// </summary>
		RPL_WELCOME = 001,

		/// <summary>
		/// IRC: Your host is [servername], running version [ver]
		/// 
		/// </summary>
		RPL_YOURHOST = 002,

		/// <summary>
		/// IRC: This server was created [date]
		/// 
		/// </summary>
		RPL_CREATED = 003,

		/// <summary>
		/// IRC: [servername] [version] [available user modes [available channel modes]
		/// 
		/// Description: The server sends Replies 001 to 004 to a user upon
		/// successful registration.
		/// 
		/// </summary>
		RPL_MYINFO = 004,

		/// <summary>
		/// IRC: Try server [server name], port [port number]
		/// 
		/// Description: Sent by the server to a user to suggest an alternative
		/// server. This is often used when the connection is
		/// refused because the server is already full.
		/// 
		/// </summary>
		RPL_BOUNCE = 005,

		/// <summary>
		/// IRC: :*1[reply] *( " " [reply] )
		/// 
		/// Description: Reply format used by USERHOST to list replies to
		/// the query list. The reply string is composed as
		/// follows:
		/// reply = nickname [ "*" ] "=" ( "+" / "-" ) hostname
		/// The '*' indicates whether the client has registered
		/// as an Operator. The '-' or '+' characters represent
		/// whether the client has set an AWAY message or not
		/// respectively.
		/// 
		/// </summary>
		RPL_USERHOST = 302,

		/// <summary>
		/// IRC: :*1[nick] *( " " [nick] )
		/// 
		/// Description: Reply format used by ISON to list replies to the
		/// query list.
		/// 
		/// </summary>
		RPL_ISON = 303,

		/// <summary>
		/// IRC: [nick] :[away message]
		/// 
		/// </summary>
		RPL_AWAY = 301,

		/// <summary>
		/// IRC: :You are no longer marked as being away
		/// 
		/// </summary>
		RPL_UNAWAY = 305,

		/// <summary>
		/// IRC: :You have been marked as being away
		/// 
		/// Description: These replies are used with the AWAY command (if
		/// allowed). RPL_AWAY is sent to any client sending a
		/// PRIVMSG to a client which is away. RPL_AWAY is only
		/// sent by the server to which the client is connected.
		/// Replies RPL_UNAWAY and RPL_NOWAWAY are sent when the
		/// client removes and sets an AWAY message.
		/// 
		/// </summary>
		RPL_NOWAWAY = 306,

		/// <summary>
		/// IRC: [nick] [user] [host] * :[real name]
		/// 
		/// </summary>
		RPL_WHOISUSER = 311,

		/// <summary>
		/// IRC: [nick] [server] :[server info]
		/// 
		/// </summary>
		RPL_WHOISSERVER = 312,

		/// <summary>
		/// IRC: [nick] :is an IRC operator
		/// 
		/// </summary>
		RPL_WHOISOPERATOR = 313,

		/// <summary>
		/// IRC: [nick] [integer] :seconds idle
		/// 
		/// </summary>
		RPL_WHOISIDLE = 317,

		/// <summary>
		/// IRC: [nick] :End of WHOIS list
		/// 
		/// </summary>
		RPL_ENDOFWHOIS = 318,

		/// <summary>
		/// IRC: [nick] :*( ( "@" / "+" ) [channel] " " )
		/// 
		/// Description: Replies 311 - 313, 317 - 319 are all replies
		/// generated in response to a WHOIS message. Given that
		/// there are enough parameters present, the answering
		/// server MUST either formulate a reply out of the above
		/// numerics (if the query nick is found) or return an
		/// error reply. The '*' in RPL_WHOISUSER is there as
		/// the literal character and not as a wild card. For
		/// each reply set, only RPL_WHOISCHANNELS may appear
		/// more than once (for long lists of channel names).
		/// The '@' and '+' characters next to the channel name
		/// indicate whether a client is a channel operator or
		/// has been granted permission to speak on a moderated
		/// channel. The RPL_ENDOFWHOIS reply is used to mark
		/// the end of processing a WHOIS message.
		/// 
		/// </summary>
		RPL_WHOISCHANNELS = 319,

		/// <summary>
		/// IRC: [nick] [user] [host] * :[real name]
		/// 
		/// </summary>
		RPL_WHOWASUSER = 314,

		/// <summary>
		/// IRC: [nick] :End of WHOWAS
		/// 
		/// Description: When replying to a WHOWAS message, a server MUST use
		/// the replies RPL_WHOWASUSER, RPL_WHOISSERVER or
		/// ERR_WASNOSUCHNICK for each nickname in the presented
		/// list. At the end of all reply batches, there MUST
		/// be RPL_ENDOFWHOWAS (even if there was only one reply
		/// and it was an error).
		/// 
		/// </summary>
		RPL_ENDOFWHOWAS = 369,

		/// <summary>
		/// 
		/// Description: Obsolete. Not used.
		/// 
		/// </summary>
		RPL_LISTSTART = 321,

		/// <summary>
		/// IRC: [channel] [# visible] :[topic]
		/// 
		/// </summary>
		RPL_LIST = 322,

		/// <summary>
		/// IRC: :End of LIST
		/// 
		/// Description: Replies RPL_LIST, RPL_LISTEND mark the actual replies
		/// with data and end of the server's response to a LIST
		/// command. If there are no channels available to return,
		/// only the end reply MUST be sent.
		/// 
		/// </summary>
		RPL_LISTEND = 323,

		/// <summary>
		/// IRC: [channel] [nickname]
		/// 
		/// </summary>
		RPL_UNIQOPIS = 325,

		/// <summary>
		/// IRC: [channel] [mode] [mode params]
		/// 
		/// </summary>
		RPL_CHANNELMODEIS = 324,

		/// <summary>
		/// IRC: [channel] :No topic is set
		/// 
		/// </summary>
		RPL_NOTOPIC = 331,

		/// <summary>
		/// IRC: [channel] :[topic]
		/// 
		/// Description: When sending a TOPIC message to determine the
		/// channel topic, one of two replies is sent. If
		/// the topic is set, RPL_TOPIC is sent back else
		/// RPL_NOTOPIC.
		/// 
		/// </summary>
		RPL_TOPIC = 332,

		/// <summary>
		/// IRC: [channel] [nick]
		/// 
		/// Description: Returned by the server to indicate that the
		/// attempted INVITE message was successful and is
		/// being passed onto the end client.
		/// 
		/// </summary>
		RPL_INVITING = 341,

		/// <summary>
		/// IRC: [user] :Summoning user to IRC
		/// 
		/// Description: Returned by a server answering a SUMMON message to
		/// indicate that it is summoning that user.
		/// 
		/// </summary>
		RPL_SUMMONING = 342,

		/// <summary>
		/// IRC: [channel] [invitemask]
		/// 
		/// </summary>
		RPL_INVITELIST = 346,

		/// <summary>
		/// IRC: [channel] :End of channel invite list
		/// 
		/// Description: When listing the 'invitations masks' for a given channel,
		/// a server is required to send the list back using the
		/// RPL_INVITELIST and RPL_ENDOFINVITELIST messages. A
		/// separate RPL_INVITELIST is sent for each active mask.
		/// After the masks have been listed (or if none present) a
		/// RPL_ENDOFINVITELIST MUST be sent.
		/// 
		/// </summary>
		RPL_ENDOFINVITELIST = 347,

		/// <summary>
		/// IRC: [channel] [exceptionmask]
		/// 
		/// </summary>
		RPL_EXCEPTLIST = 348,

		/// <summary>
		/// IRC: [channel] :End of channel exception list
		/// 
		/// Description: When listing the 'exception masks' for a given channel,
		/// a server is required to send the list back using the
		/// RPL_EXCEPTLIST and RPL_ENDOFEXCEPTLIST messages. A
		/// separate RPL_EXCEPTLIST is sent for each active mask.
		/// After the masks have been listed (or if none present)
		/// a RPL_ENDOFEXCEPTLIST MUST be sent.
		/// 
		/// </summary>
		RPL_ENDOFEXCEPTLIST = 349,

		/// <summary>
		/// IRC: [version].[debuglevel] [server] :[comments]
		/// 
		/// Description: Reply by the server showing its version details.
		/// The [version] is the version of the software being
		/// used (including any patchlevel revisions) and the
		/// [debuglevel] is used to indicate if the server is
		/// running in "debug mode".
		/// The "comments" field may contain any comments about
		/// the version or further version details.
		/// 
		/// </summary>
		RPL_VERSION = 351,

		/// <summary>
		/// IRC: [channel] [user] [host] [server] [nick ( "H" / "G" ] ["*"] [ ( "@" / "+" ) :[hopcount] [real name]
		/// 
		/// </summary>
		RPL_WHOREPLY = 352,

		/// <summary>
		/// IRC: [name] :End of WHO list
		/// 
		/// Description: The RPL_WHOREPLY and RPL_ENDOFWHO pair are used
		/// to answer a WHO message. The RPL_WHOREPLY is only
		/// sent if there is an appropriate match to the WHO
		/// query. If there is a list of parameters supplied
		/// with a WHO message, a RPL_ENDOFWHO MUST be sent
		/// after processing each list item with [name] being
		/// the item.
		/// 
		/// </summary>
		RPL_ENDOFWHO = 315,

		/// <summary>
		/// IRC: ( "=" / "*" / "@" ) [channel :[ "@" / "+" ] [nick] *( " " [ "@" / "+" ] [nick] 
		/// 
		/// Description: "@" is used for secret channels, "*" for private
		/// channels, and "=" for others (public channels).
		/// 
		/// </summary>
		RPL_NAMREPLY = 353,

		/// <summary>
		/// IRC: [channel] :End of NAMES list
		/// 
		/// Description: To reply to a NAMES message, a reply pair consisting
		/// of RPL_NAMREPLY and RPL_ENDOFNAMES is sent by the
		/// server back to the client. If there is no channel
		/// found as in the query, then only RPL_ENDOFNAMES is
		/// returned. The exception to this is when a NAMES
		/// message is sent with no parameters and all visible
		/// channels and contents are sent back in a series of
		/// RPL_NAMEREPLY messages with a RPL_ENDOFNAMES to mark
		/// the end.
		/// 
		/// </summary>
		RPL_ENDOFNAMES = 366,

		/// <summary>
		/// IRC: [mask] [server] :[hopcount] [server info]
		/// 
		/// </summary>
		RPL_LINKS = 364,

		/// <summary>
		/// IRC: [mask] :End of LINKS list
		/// 
		/// Description: In replying to the LINKS message, a server MUST send
		/// replies back using the RPL_LINKS numeric and mark the
		/// end of the list using an RPL_ENDOFLINKS reply.
		/// 
		/// </summary>
		RPL_ENDOFLINKS = 365,

		/// <summary>
		/// IRC: [channel] [banmask]
		/// 
		/// </summary>
		RPL_BANLIST = 367,

		/// <summary>
		/// IRC: [channel] :End of channel ban list
		/// 
		/// Description: When listing the active 'bans' for a given channel,
		/// a server is required to send the list back using the
		/// RPL_BANLIST and RPL_ENDOFBANLIST messages. A separate
		/// RPL_BANLIST is sent for each active banmask. After the
		/// banmasks have been listed (or if none present) a
		/// RPL_ENDOFBANLIST MUST be sent.
		/// 
		/// </summary>
		RPL_ENDOFBANLIST = 368,

		/// <summary>
		/// IRC: :[string]
		/// 
		/// </summary>
		RPL_INFO = 371,

		/// <summary>
		/// IRC: :End of INFO list
		/// 
		/// Description: A server responding to an INFO message is required to
		/// send all its 'info' in a series of RPL_INFO messages
		/// with a RPL_ENDOFINFO reply to indicate the end of the
		/// replies.
		/// 
		/// </summary>
		RPL_ENDOFINFO = 374,

		/// <summary>
		/// IRC: :- [server] Message of the day - 
		/// 
		/// </summary>
		RPL_MOTDSTART = 375,

		/// <summary>
		/// IRC: :- [text]
		/// 
		/// </summary>
		RPL_MOTD = 372,

		/// <summary>
		/// IRC: :End of MOTD command
		/// 
		/// Description: When responding to the MOTD message and the MOTD file
		/// is found, the file is displayed line by line, with
		/// each line no longer than 80 characters, using
		/// RPL_MOTD format replies. These MUST be surrounded
		/// by a RPL_MOTDSTART (before the RPL_MOTDs) and an
		/// RPL_ENDOFMOTD (after).
		/// 
		/// </summary>
		RPL_ENDOFMOTD = 376,

		/// <summary>
		/// IRC: :You are now an IRC operator
		/// 
		/// Description: RPL_YOUREOPER is sent back to a client which has
		/// just successfully issued an OPER message and gained
		/// operator status.
		/// 
		/// </summary>
		RPL_YOUREOPER = 381,

		/// <summary>
		/// IRC: [config file] :Rehashing
		/// 
		/// Description: If the REHASH option is used and an operator sends
		/// a REHASH message, an RPL_REHASHING is sent back to
		/// the operator.
		/// 
		/// </summary>
		RPL_REHASHING = 382,

		/// <summary>
		/// IRC: You are service [servicename]
		/// 
		/// Description: Sent by the server to a service upon successful
		/// registration.
		/// 
		/// </summary>
		RPL_YOURESERVICE = 383,

		/// <summary>
		/// IRC: [server] :[string showing server's local time]
		/// 
		/// Description: When replying to the TIME message, a server MUST send
		/// the reply using the RPL_TIME format above. The string
		/// showing the time need only contain the correct day and
		/// time there. There is no further requirement for the
		/// time string.
		/// 
		/// </summary>
		RPL_TIME = 391,

		/// <summary>
		/// IRC: :UserID Terminal Host
		/// 
		/// </summary>
		RPL_USERSSTART = 392,

		/// <summary>
		/// IRC: :[username] [ttyline] [hostname]
		/// 
		/// </summary>
		RPL_USERS = 393,

		/// <summary>
		/// IRC: :End of users
		/// 
		/// </summary>
		RPL_ENDOFUSERS = 394,

		/// <summary>
		/// IRC: :Nobody logged in
		/// 
		/// Description: If the USERS message is handled by a server, the
		/// replies RPL_USERSTART, RPL_USERS, RPL_ENDOFUSERS and
		/// RPL_NOUSERS are used. RPL_USERSSTART MUST be sent
		/// first, following by either a sequence of RPL_USERS
		/// or a single RPL_NOUSER. Following this is
		/// RPL_ENDOFUSERS.
		/// 
		/// </summary>
		RPL_NOUSERS = 395,

		/// <summary>
		/// IRC: Link [version and debug level] [destination [next server] V[protocol version [link uptime in seconds] [backstream sendq [upstream sendq]
		/// 
		/// </summary>
		RPL_TRACELINK = 200,

		/// <summary>
		/// IRC: Try. [class] [server]
		/// 
		/// </summary>
		RPL_TRACECONNECTING = 201,

		/// <summary>
		/// IRC: H.S. [class] [server]
		/// 
		/// </summary>
		RPL_TRACEHANDSHAKE = 202,

		/// <summary>
		/// IRC: ???? [class] [[client IP address in dot form]]
		/// 
		/// </summary>
		RPL_TRACEUNKNOWN = 203,

		/// <summary>
		/// IRC: Oper [class] [nick]
		/// 
		/// </summary>
		RPL_TRACEOPERATOR = 204,

		/// <summary>
		/// IRC: User [class] [nick]
		/// 
		/// </summary>
		RPL_TRACEUSER = 205,

		/// <summary>
		/// IRC: Serv [class] [int]S [int]C [server [nick!user|*!*]@[host|server] V[protocol version]
		/// 
		/// </summary>
		RPL_TRACESERVER = 206,

		/// <summary>
		/// IRC: Service [class] [name] [type] [active type]
		/// 
		/// </summary>
		RPL_TRACESERVICE = 207,

		/// <summary>
		/// IRC: [newtype] 0 [client name]
		/// 
		/// </summary>
		RPL_TRACENEWTYPE = 208,

		/// <summary>
		/// IRC: Class [class] [count]
		/// 
		/// </summary>
		RPL_TRACECLASS = 209,

		/// <summary>
		/// IRC: Class [class] [count] Unused
		/// 
		/// </summary>
		RPL_TRACERECONNECT = 210,

		/// <summary>
		/// IRC: File [logfile] [debug level]
		/// 
		/// </summary>
		RPL_TRACELOG = 261,

		/// <summary>
		/// IRC: [server name] [version and debug level] :End of TRACE
		/// 
		/// Description: The RPL_TRACE are all returned by the server in
		/// response to the TRACE message. How many are
		/// returned is dependent on the TRACE message and
		/// whether it was sent by an operator or not. There
		/// is no predefined order for which occurs first.
		/// Replies RPL_TRACEUNKNOWN, RPL_TRACECONNECTING and
		/// RPL_TRACEHANDSHAKE are all used for connections
		/// which have not been fully established and are either
		/// unknown, still attempting to connect or in the
		/// process of completing the 'server handshake'.
		/// RPL_TRACELINK is sent by any server which handles
		/// a TRACE message and has to pass it on to another
		/// server. The list of RPL_TRACELINKs sent in
		/// response to a TRACE command traversing the IRC
		/// network should reflect the actual connectivity of
		/// the servers themselves along that path.
		/// RPL_TRACENEWTYPE is to be used for any connection
		/// which does not fit in the other categories but is
		/// being displayed anyway.
		/// RPL_TRACEEND is sent to indicate the end of the list.
		/// </summary>
		RPL_TRACEEND = 262,

		/// <summary>
		/// IRC: [linkname] [sendq] [sent messages [sent Kbytes] [received messages [received Kbytes] [time open]
		/// 
		/// Description: reports statistics on a connection. [linkname]
		/// identifies the particular connection, [sendq] is
		/// the amount of data that is queued and waiting to be
		/// sent [sent messages] the number of messages sent,
		/// and [sent Kbytes] the amount of data sent, in
		/// Kbytes. [received messages] and [received Kbytes]
		/// are the equivalent of [sent messages] and [sent
		/// Kbytes] for received data, respectively. [time
		/// open] indicates how long ago the connection was
		/// opened, in seconds.
		/// 
		/// </summary>
		RPL_STATSLINKINFO = 211,

		/// <summary>
		/// IRC: [command] [count] [byte count] [remote count]
		/// 
		/// Description: reports statistics on commands usage.
		/// 
		/// </summary>
		RPL_STATSCOMMANDS = 212,

		/// <summary>
		/// IRC: [stats letter] :End of STATS report
		/// 
		/// </summary>
		RPL_ENDOFSTATS = 219,

		/// <summary>
		/// IRC: :Server Up %d days %d:%02d:%02d
		/// 
		/// Description: reports the server uptime.
		/// 
		/// </summary>
		RPL_STATSUPTIME = 242,

		/// <summary>
		/// IRC: O [hostmask] * [name]
		/// 
		/// Description: reports the allowed hosts from where user may become IRC
		/// operators.
		/// 
		/// </summary>
		RPL_STATSOLINE = 243,

		/// <summary>
		/// IRC: [user mode string]
		/// 
		/// Description: To answer a query about a client's own mode,
		/// RPL_UMODEIS is sent back.
		/// 
		/// </summary>
		RPL_UMODEIS = 221,

		/// <summary>
		/// IRC: [name] [server] [mask] [type] [hopcount] [info]
		/// 
		/// </summary>
		RPL_SERVLIST = 234,

		/// <summary>
		/// IRC: [mask] [type] :End of service listing
		/// 
		/// Description: When listing services in reply to a SERVLIST message,
		/// a server is required to send the list back using the
		/// RPL_SERVLIST and RPL_SERVLISTEND messages. A separate
		/// RPL_SERVLIST is sent for each service. After the
		/// services have been listed (or if none present) a
		/// RPL_SERVLISTEND MUST be sent.
		/// 
		/// </summary>
		RPL_SERVLISTEND = 235,

		/// <summary>
		/// IRC: :There are [integer] users and [integer services on [integer] servers
		/// 
		/// </summary>
		RPL_LUSERCLIENT = 251,

		/// <summary>
		/// IRC: [integer] :operator(s) online
		/// 
		/// </summary>
		RPL_LUSEROP = 252,

		/// <summary>
		/// IRC: [integer] :unknown connection(s)
		/// 
		/// </summary>
		RPL_LUSERUNKNOWN = 253,

		/// <summary>
		/// IRC: [integer] :channels formed
		/// 
		/// </summary>
		RPL_LUSERCHANNELS = 254,

		/// <summary>
		/// IRC: :I have [integer] clients and [integer servers
		/// 
		/// Description: In processing an LUSERS message, the server
		/// sends a set of replies from RPL_LUSERCLIENT,
		/// RPL_LUSEROP, RPL_USERUNKNOWN,
		/// RPL_LUSERCHANNELS and RPL_LUSERME. When
		/// replying, a server MUST send back
		/// and RPL_LUSERME. The other
		/// replies are only sent back if a non-zero count
		/// is found for them.
		/// 
		/// </summary>
		RPL_LUSERME = 255,

		/// <summary>
		/// IRC: [server] :Administrative info
		/// 
		/// </summary>
		RPL_ADMINME = 256,

		/// <summary>
		/// IRC: :[admin info]
		/// 
		/// </summary>
		RPL_ADMINLOC1 = 257,

		/// <summary>
		/// IRC: :[admin info]
		/// 
		/// </summary>
		RPL_ADMINLOC2 = 258,

		/// <summary>
		/// IRC: :[admin info]
		/// 
		/// Description: When replying to an ADMIN message, a server
		/// is expected to use replies RPL_ADMINME
		/// through to RPL_ADMINEMAIL and provide a text
		/// message with each. For RPL_ADMINLOC1 a
		/// description of what city, state and country
		/// the server is in is expected, followed by
		/// details of the institution (RPL_ADMINLOC2)
		/// and constly the administrative contact for the
		/// server (an email address here is REQUIRED)
		/// in RPL_ADMINEMAIL.
		/// 
		/// </summary>
		RPL_ADMINEMAIL = 259,

		/// <summary>
		/// IRC: [command] :Please wait a while and try again.
		/// 
		/// Description: When a server drops a command without processing it,
		/// it MUST use the reply RPL_TRYAGAIN to inform the
		/// originating client.
		/// 
		/// </summary>
		RPL_TRYAGAIN = 263,

		/// <summary>
		/// IRC: [nickname] :No such nick/channel
		/// 
		/// Description: Used to indicate the nickname parameter supplied to a
		/// command is currently unused.
		/// 
		/// </summary>
		ERR_NOSUCHNICK = 401,

		/// <summary>
		/// IRC: [server name] :No such server
		/// 
		/// Description: Used to indicate the server name given currently
		/// does not exist.
		/// 
		/// </summary>
		ERR_NOSUCHSERVER = 402,

		/// <summary>
		/// IRC: [channel name] :No such channel
		/// 
		/// Description: Used to indicate the given channel name is invalid.
		/// 
		/// </summary>
		ERR_NOSUCHCHANNEL = 403,

		/// <summary>
		/// IRC: [channel name] :Cannot send to channel
		/// 
		/// Description: Sent to a user who is either (a) not on a channel
		/// which is mode +n or (b) not a chanop (or mode +v) on
		/// a channel which has mode +m set or where the user is
		/// banned and is trying to send a PRIVMSG message to
		/// that channel.
		/// 
		/// </summary>
		ERR_CANNOTSENDTOCHAN = 404,

		/// <summary>
		/// IRC: [channel name] :You have joined too many channels
		/// 
		/// Description: Sent to a user when they have joined the maximum
		/// number of allowed channels and they try to join
		/// another channel.
		/// 
		/// </summary>
		ERR_TOOMANYCHANNELS = 405,

		/// <summary>
		/// IRC: [nickname] :There was no such nickname
		/// 
		/// Description: Returned by WHOWAS to indicate there is no history
		/// information for that nickname.
		/// 
		/// </summary>
		ERR_WASNOSUCHNICK = 406,

		/// <summary>
		/// IRC: [target] :[error code] recipients. [abort message]
		/// 
		/// Description: Returned to a client which is attempting to send a
		/// PRIVMSG/NOTICE using the user@host destination format
		/// and for a user@host which has several occurrences.
		/// Returned to a client which trying to send a
		/// PRIVMSG/NOTICE to too many recipients.
		/// Returned to a client which is attempting to JOIN a safe
		/// channel using the shortname when there are more than one
		/// such channel.
		/// 
		/// </summary>
		ERR_TOOMANYTARGETS = 407,

		/// <summary>
		/// IRC: [service name] :No such service
		/// 
		/// Description: Returned to a client which is attempting to send a SQUERY
		/// to a service which does not exist.
		/// 
		/// </summary>
		ERR_NOSUCHSERVICE = 408,

		/// <summary>
		/// IRC: :No origin specified
		/// 
		/// Description: PING or PONG message missing the originator parameter.
		/// 
		/// </summary>
		ERR_NOORIGIN = 409,

		/// <summary>
		/// IRC: :No recipient given ([command])
		/// 
		/// </summary>
		ERR_NORECIPIENT = 411,

		/// <summary>
		/// IRC: :No text to send
		/// 
		/// </summary>
		ERR_NOTEXTTOSEND = 412,

		/// <summary>
		/// IRC: [mask] :No toplevel domain specified
		/// 
		/// </summary>
		ERR_NOTOPLEVEL = 413,

		/// <summary>
		/// IRC: [mask] :Wildcard in toplevel domain
		/// 
		/// </summary>
		ERR_WILDTOPLEVEL = 414,

		/// <summary>
		/// IRC: PRIVMSG $[server]" or "PRIVMSG #[host]" is attempted
		/// 
		/// Description: 412 - 415 are returned by PRIVMSG to indicate that
		/// the message wasn't delivered for some reason.
		/// ERR_NOTOPLEVEL and ERR_WILDTOPLEVEL are errors that
		/// are returned when an invalid use of
		/// 
		/// </summary>
		ERR_BADMASK = 415,

		/// <summary>
		/// A query returned too many results.
		/// This is not an offical part of the RFC but added since it seems to be in use.
		/// </summary>
		ERR_TOOMANYLINES = 416,

		/// <summary>
		/// IRC: [command] :Unknown command
		/// 
		/// Description: Returned to a registered client to indicate that the
		/// command sent is unknown by the server.
		/// 
		/// </summary>
		ERR_UNKNOWNCOMMAND = 421,

		/// <summary>
		/// IRC: :MOTD File is missing
		/// 
		/// Description: Server's MOTD file could not be opened by the server.
		/// 
		/// </summary>
		ERR_NOMOTD = 422,

		/// <summary>
		/// IRC: [server] :No administrative info available
		/// 
		/// Description: Returned by a server in response to an ADMIN message
		/// when there is an error in finding the appropriate
		/// information.
		/// 
		/// </summary>
		ERR_NOADMININFO = 423,

		/// <summary>
		/// IRC: :File error doing [file op] on [file]
		/// 
		/// Description: Generic error message used to report a failed file
		/// operation during the processing of a message.
		/// 
		/// </summary>
		ERR_FILEERROR = 424,

		/// <summary>
		/// IRC: :No nickname given
		/// 
		/// Description: Returned when a nickname parameter expected for a
		/// command and isn't found.
		/// 
		/// </summary>
		ERR_NONICKNAMEGIVEN = 431,

		/// <summary>
		/// IRC: [nick] :Erroneous nickname
		/// 
		/// Description: Returned after receiving a NICK message which contains
		/// characters which do not fall in the defined set. See
		/// section 2.3.1 for details on valid nicknames.
		/// 
		/// </summary>
		ERR_ERRONEUSNICKNAME = 432,

		/// <summary>
		/// IRC: [nick] :Nickname is already in use
		/// 
		/// Description: Returned when a NICK message is processed that results
		/// in an attempt to change to a currently existing
		/// nickname.
		/// 
		/// </summary>
		ERR_NICKNAMEINUSE = 433,

		/// <summary>
		/// IRC: [nick] :Nickname collision KILL from [user]@[host]
		/// 
		/// Description: Returned by a server to a client when it detects a
		/// nickname collision (registered of a NICK that
		/// already exists by another server).
		/// 
		/// </summary>
		ERR_NICKCOLLISION = 436,

		/// <summary>
		/// IRC: [nick/channel] :Nick/channel is temporarily unavailable
		/// 
		/// Description: Returned by a server to a user trying to join a channel
		/// currently blocked by the channel delay mechanism.
		/// Returned by a server to a user trying to change nickname
		/// when the desired nickname is blocked by the nick delay
		/// mechanism.
		/// 
		/// </summary>
		ERR_UNAVAILRESOURCE = 437,

		/// <summary>
		/// IRC: [nick] [channel] :They aren't on that channel
		/// 
		/// Description: Returned by the server to indicate that the target
		/// user of the command is not on the given channel.
		/// 
		/// </summary>
		ERR_USERNOTINCHANNEL = 441,

		/// <summary>
		/// IRC: [channel] :You're not on that channel
		/// 
		/// Description: Returned by the server whenever a client tries to
		/// perform a channel affecting command for which the
		/// client isn't a member.
		/// 
		/// </summary>
		ERR_NOTONCHANNEL = 442,

		/// <summary>
		/// IRC: [user] [channel] :is already on channel
		/// 
		/// Description: Returned when a client tries to invite a user to a
		/// channel they are already on.
		/// 
		/// </summary>
		ERR_USERONCHANNEL = 443,

		/// <summary>
		/// IRC: [user] :User not logged in
		/// 
		/// Description: Returned by the summon after a SUMMON command for a
		/// user was unable to be performed since they were not
		/// logged in.
		/// 
		/// </summary>
		ERR_NOLOGIN = 444,

		/// <summary>
		/// IRC: :SUMMON has been disabled
		/// 
		/// Description: Returned as a response to the SUMMON command. MUST be
		/// returned by any server which doesn't implement it.
		/// 
		/// </summary>
		ERR_SUMMONDISABLED = 445,

		/// <summary>
		/// IRC: :USERS has been disabled
		/// 
		/// Description: Returned as a response to the USERS command. MUST be
		/// returned by any server which does not implement it.
		/// 
		/// </summary>
		ERR_USERSDISABLED = 446,

		/// <summary>
		/// IRC: :You have not registered
		/// 
		/// Description: Returned by the server to indicate that the client
		/// MUST be registered before the server will allow it
		/// to be parsed in detail.
		/// 
		/// </summary>
		ERR_NOTREGISTERED = 451,

		/// <summary>
		/// IRC: [command] :Not enough parameters
		/// 
		/// Description: Returned by the server by numerous commands to
		/// indicate to the client that it didn't supply enough
		/// parameters.
		/// 
		/// </summary>
		ERR_NEEDMOREPARAMS = 461,

		/// <summary>
		/// IRC: :Unauthorized command (already registered)
		/// 
		/// Description: Returned by the server to any link which tries to
		/// change part of the registered details (such as
		/// password or user details from second USER message).
		/// 
		/// </summary>
		ERR_ALREADYREGISTRED = 462,

		/// <summary>
		/// IRC: :Your host isn't among the privileged
		/// 
		/// Description: Returned to a client which attempts to register with
		/// a server which does not been setup to allow
		/// connections from the host the attempted connection
		/// is tried.
		/// 
		/// </summary>
		ERR_NOPERMFORHOST = 463,

		/// <summary>
		/// IRC: :Password incorrect
		/// 
		/// Description: Returned to indicate a failed attempt at registering
		/// a connection for which a password was required and
		/// was either not given or incorrect.
		/// 
		/// </summary>
		ERR_PASSWDMISMATCH = 464,

		/// <summary>
		/// IRC: :You are banned from this server
		/// 
		/// Description: Returned after an attempt to connect and register
		/// yourself with a server which has been setup to
		/// explicitly deny connections to you.
		/// 
		/// </summary>
		ERR_YOUREBANNEDCREEP = 465,

		/// <summary>
		/// IRC: :You are banned from this server
		/// 
		/// Description: Sent by a server to a user to inform that access to the
		/// server will soon be denied.
		/// 
		/// </summary>
		ERR_YOUWILLBEBANNED = 466,

		/// <summary>
		/// IRC: [channel] :Channel key already set
		/// 
		/// </summary>
		ERR_KEYSET = 467,

		/// <summary>
		/// IRC: [channel] :Cannot join channel (+l)
		/// 
		/// </summary>
		ERR_CHANNELISFULL = 471,

		/// <summary>
		/// IRC: [char] :is unknown mode char to me for [channel]
		/// 
		/// </summary>
		ERR_UNKNOWNMODE = 472,

		/// <summary>
		/// IRC: [channel] :Cannot join channel (+i)
		/// 
		/// </summary>
		ERR_INVITEONLYCHAN = 473,

		/// <summary>
		/// IRC: [channel] :Cannot join channel (+b)
		/// 
		/// </summary>
		ERR_BANNEDFROMCHAN = 474,

		/// <summary>
		/// IRC: [channel] :Cannot join channel (+k)
		/// 
		/// </summary>
		ERR_BADCHANNELKEY = 475,

		/// <summary>
		/// IRC: [channel] :Bad Channel Mask
		/// 
		/// </summary>
		ERR_BADCHANMASK = 476,

		/// <summary>
		/// IRC: [channel] :Channel doesn't support modes
		/// 
		/// </summary>
		ERR_NOCHANMODES = 477,

		/// <summary>
		/// IRC: [channel] [char] :Channel list is full
		/// 
		/// </summary>
		ERR_BANLISTFULL = 478,

		/// <summary>
		/// IRC: :Permission Denied- You're not an IRC operator
		/// 
		/// Description: Any command requiring operator privileges to operate
		/// MUST return this error to indicate the attempt was
		/// unsuccessful.
		/// 
		/// </summary>
		ERR_NOPRIVILEGES = 481,

		/// <summary>
		/// IRC: [channel] :You're not channel operator
		/// 
		/// Description: Any command requiring 'chanop' privileges (such as
		/// MODE messages) MUST return this error if the client
		/// making the attempt is not a chanop on the specified
		/// channel.
		/// 
		/// </summary>
		ERR_CHANOPRIVSNEEDED = 482,

		/// <summary>
		/// IRC: :You can't kill a server!
		/// 
		/// Description: Any attempts to use the KILL command on a server
		/// are to be refused and this error returned directly
		/// to the client.
		/// 
		/// </summary>
		ERR_CANTKILLSERVER = 483,

		/// <summary>
		/// IRC: :Your connection is restricted!
		/// 
		/// Description: Sent by the server to a user upon connection to indicate
		/// the restricted nature of the connection (user mode "+r").
		/// 
		/// </summary>
		ERR_RESTRICTED = 484,

		/// <summary>
		/// IRC: :You're not the original channel operator
		/// 
		/// Description: Any MODE requiring "channel creator" privileges MUST
		/// return this error if the client making the attempt is not
		/// a chanop on the specified channel.
		/// 
		/// </summary>
		ERR_UNIQOPPRIVSNEEDED = 485,

		/// <summary>
		/// IRC: :No O-lines for your host
		/// 
		/// Description: If a client sends an OPER message and the server has
		/// not been configured to allow connections from the
		/// client's host as an operator, this error MUST be
		/// returned.
		/// 
		/// </summary>
		ERR_NOOPERHOST = 491,

		/// <summary>
		/// IRC: :Unknown MODE flag
		/// 
		/// Description: Returned by the server to indicate that a MODE
		/// message was sent with a nickname parameter and that
		/// the a mode flag sent was not recognized.
		/// 
		/// </summary>
		ERR_UMODEUNKNOWNFLAG = 501,

		/// <summary>
		/// IRC: :Cannot change mode for other users
		/// 
		/// Description: Error sent to any user trying to view or change the
		/// user mode for a user other than themselves.
		/// 
		/// </summary>
		ERR_USERSDONTMATCH = 502,

		/// <summary>
		/// When the TCP/IP connection unexpectedly fails.
		/// </summary>
		ConnectionFailed = 1000,

		/// <summary>
		/// The IRC server sent an 'ERROR' message for some
		/// reason.
		/// </summary>
		IrcServerError = 1001,
		
		/// <summary>
		/// When the socket connection information sent
		/// by the remote user in a DCC request is bad.
		/// </summary>
		BadDccEndpoint = 1002,

		/// <summary>
		/// A message from the IRC server that cannot be parsed. This may be because
		/// the message is intended to cause problems, it may be an unsupported protocol
		/// such as DCC Voice, or it may be that the Thresher parser simply cannot understand
		/// it.
		/// </summary>
		UnparseableMessage = 1003,

		/// <summary>
		/// Normally a DCC Resume message is sent in response to
		/// a DCC Send. This error is signaled when a DCC Resume message is
		/// received without a previous Send or the Send session has timed out.
		/// </summary>
		UnableToResume = 1004,

		/// <summary>
		/// Signaled when a DCC Get or SEND contains an encryption
		/// protocol that Thresher does not support.
		/// </summary>
		UnknownEncryptionProtocol = 1005,

		/// <summary>
		/// When trying to resume a DCC transfer the remote user is
		/// supposed to send an Accept message with the same starting position
		/// in the file. If these numbers differ this error is raised.
		/// </summary>
		BadDccAcceptValue = 1006,

		/// <summary>
		/// If the remote user sends a DCC resume request which
		/// asks for a start position greater than or equals to the file
		/// size then this error is raised.
		/// </summary>
		BadResumePosition = 1007,

		/// <summary>
		/// When attempting to connect to another
		/// machine using DCC and the target machine
		/// refuses the connection this error is raised.
		/// </summary>
		DccConnectionRefused = 1008

	}

}
