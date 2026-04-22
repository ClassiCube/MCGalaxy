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

namespace Sharkbite.Irc
{
	public delegate void ErrorMessageEventHandler( int code, string message );
	public delegate void NickErrorEventHandler( string badNick ) ;
	public delegate void RegisteredEventHandler();
	public delegate void PublicNoticeEventHandler( string user, string channel, string notice );
	public delegate void PrivateNoticeEventHandler( string user, string notice );
	public delegate void JoinEventHandler( string user, string channel );
	public delegate void PublicActionEventHandler( string user, string channel, string description );
	public delegate void PrivateActionEventHandler( string user, string description );
	public delegate void PublicMessageEventHandler( string user, string channel, string message );
	public delegate void PrivateMessageEventHandler( string user, string message );
	public delegate void NickEventHandler( string user, string newNick );
	public delegate void PartEventHandler( string user, string channel, string reason);
	public delegate void QuitEventHandler( string user, string reason);
	public delegate void KickEventHandler( string user, string channel, string kickee, string reason );
	public delegate void NamesEventHandler( string channel, string[] nicks, bool last );
	public delegate void ChannelModeChangeEventHandler( string who, string channel );
	public delegate void KillEventHandler( string user, string nick, string reason );
	
	public static class IRCUtils
	{
	    public static char[] SPACE = { ' ' };
	    
		public static string ExtractNick(string fullUserName) 
		{
		    // from RFC - nickname [ [ "!" user ] "@" host ]
		    // i.e. 'user' and 'host' are both optional parameters
		    if (String.IsNullOrEmpty(fullUserName)) return "";
			
			int userBeg = TryFindPrefix( fullUserName, '!' );
			int hostBeg = TryFindPrefix( fullUserName, '@' );
			int nickEnd = Math.Min( userBeg, hostBeg );

			return fullUserName.Substring(0, nickEnd);
		}
		
		static int TryFindPrefix(string str, char c) {
		    int index = str.IndexOf(c);
		    return index == -1 ? str.Length : index;
		}
		
	    
	    public static string ExtractPrefix(string str, ref int index) {
		    // See RFC 2812, 2.3.1 Message format in Augmented BNF
		    // message =  [ ":" prefix SPACE ] command [ params ] crlf
		    EatWhitespace(str, ref index);
		    if (index >= str.Length || str[index] != ':') return "";
		    
		    index++; // skip :
		    return EatWord(str, ref index);
		}
	    
	    public static string NextParam(string str, ref int index) {
	        EatWhitespace(str, ref index);
	        if (index >= str.Length) return "";
        
	        if (str[index] != ':')
	            return EatWord(str, ref index);
	        
	        index++; // skip :
	        return EatToEnd(str, ref index);
	    }
	    
	    public static string NextAll(string str, ref int index) {
	        EatWhitespace(str, ref index);
	        if (index >= str.Length) return "";
	        
	        if (str[index] == ':') index++; // skip :
	        return EatToEnd(str, ref index);
	    }
	    
	    
	    static void EatWhitespace(string str, ref int index) {
	        while (index < str.Length && str[index] == ' ') index++;
	    }
	    
	    static string EatWord(string str, ref int index) {
	        int spaceIdx = str.IndexOf(' ', index);
		    if (spaceIdx == -1) spaceIdx = str.Length;
		    
		    string part = str.Substring(index, spaceIdx - index);
		    index = spaceIdx;
		    return part;
	    }
	    
	    static string EatToEnd(string str, ref int index) {	        
	        string rest = str.Substring(index);
	        index = str.Length;
	        return rest.TrimEnd(SPACE);	        
	    }
	}
}
