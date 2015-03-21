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
using System.Diagnostics;
using System.Globalization;


namespace Sharkbite.Irc
{
	/// <summary>
	/// Utility methods needed to handle DCC requests.
	/// </summary>
	public sealed class DccUtil
	{
		internal static TraceSwitch DccTrace = new TraceSwitch("DccTraceSwitch", "Debug level for DCC classes.");

		//Should never be called so make it private
		private DccUtil(){}

		/// <summary>
		/// Get the IPAddress object for the local machine.
		/// </summary>
		/// <returns>An instance of IPAddress.</returns>
		public static IPAddress LocalHost() 
		{
			IPHostEntry localhost = Dns.Resolve( Dns.GetHostName() );
			return localhost.AddressList[0];
		}
		/// <summary>
		/// Convert a signed long into an unsigned int in
		/// network byte order.
		/// </summary>
		/// <param name="bytesReceived">The number of bytes received as a long.</param>
		/// <returns>An unsigned int as a 4 byte array.</returns>
		public static byte[] DccBytesReceivedFormat( long bytesReceived ) 
		{
			byte[] size = new byte[4];
			byte[] longBytes = BitConverter.GetBytes( NetworkUnsignedLong( bytesReceived ) );
			Array.Copy( longBytes,0,size,0,4);
			return size;
		}
		/// <summary>
		/// Convert the 4 byte current DCC position
		/// into a host order long.
		/// </summary>
		/// <param name="received">The 4 byte unsigned integer.</param>
		/// <returns>A long</returns>
		public static long DccBytesToLong( byte[] received ) 
		{
			return IPAddress.NetworkToHostOrder( BitConverter.ToInt32( received, 0 ) );
		}
		/// <summary>
		/// Convert an IP address into the network order
		/// long required by the DCC protocol.
		/// </summary>
		/// <param name="ipAddress">A valid IP address</param>
		/// <returns>The long in string form</returns>
		public static string IPAddressToLong( IPAddress ipAddress ) 
		{
			if( ipAddress == null ) 
			{
				throw new ArgumentException("Address cannot be null");
			}
			return NetworkUnsignedLong( ipAddress.Address ).ToString( CultureInfo.InvariantCulture );
		}
		/// <summary>
		/// Convert the network order address received from a DCC
		/// request into an IP address.
		/// </summary>
		/// <param name="networkOrder">The address long in string form.</param>
		/// <returns>An IpAddress object</returns>
		public static IPAddress LongToIPAddress( string networkOrder ) 
		{
			if( networkOrder == null || networkOrder.Trim() == "" )
			{
				throw new ArgumentException("Network order address cannot be null or empty.");
			}
			try 
			{
				//Johan's routine
				byte[] quads = BitConverter.GetBytes( long.Parse( networkOrder, CultureInfo.InvariantCulture ) );
				return IPAddress.Parse( quads[3] + "." +quads[2] + "." + quads[1] + "." + quads[0] );
			}
			catch( FormatException fe ) 
			{
				throw new ArgumentException( networkOrder + " is not a valid network address.");
			}

		}

		/// <summary>
		/// Convert the spaces in a file name to underscores.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		/// <returns>Underscored string.</returns>
		public static string SpacesToUnderscores( string fileName ) 
		{
			return fileName.Replace(' ','_');
		}

		/// <summary>
		/// Convert a long into an unsigned 4 byte in in network order
		/// </summary>
		/// <param name="hostOrderLong">A long in host order</param>
		/// <returns>The long as unsigned int in network order</returns>
		private static long NetworkUnsignedLong( long hostOrderLong ) 
		{
			long networkLong = IPAddress.HostToNetworkOrder( hostOrderLong );
			//Network order has the octets in reverse order starting with byte 7
			//To get the correct string simply shift them down 4 bytes
			//and zero out the first 4 bytes.
			return (networkLong >> 32 ) & 0x00000000ffffffff;
		}

	}
}
