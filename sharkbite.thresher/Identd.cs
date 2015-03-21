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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;


namespace Sharkbite.Irc
{
	/// <summary>
	/// An Ident daemon is still used by some IRC networks for 
	/// authentication. It is a simple service which when queried
	/// by a remote system returns a username. The server is controlled via static
	/// methods all of which are Thread safe.
	/// </summary>
	public sealed class Identd
	{
		private static TcpListener listener;
		private static bool running; 
		private static object lockObject;
		private static string username;
		private const string Reply = " : USERID : UNIX : ";
		private const int IdentdPort = 113;

		static Identd() 
		{
			running = false;
			lockObject = new object();
		}

		//Declare constructor private so it cannot be instatiated.
		private Identd() {}

		/// <summary>
		/// The Identd server will start listening for queries
		/// in its own thread. It can be stopped by calling
		/// <see cref="Identd.Stop"/>.
		/// </summary>
		/// <param name="userName">Should be the same username as the one used
		/// in the ConnectionArgs object when establishing a connection.</param>
		/// <exception cref="Exception">If the server has already been started.</exception>
		public static void Start( string userName ) 
		{
			lock( lockObject ) 
			{
				if( running == true ) 
				{
					throw new Exception("Identd already started.");
				}
				running = true;
				username = userName;
				Thread socketThread = new Thread( new ThreadStart( Identd.Run ) );
				socketThread.Name = "Identd";
				socketThread.Start();	
			}
		}
		/// <summary>
		/// Check if the Identd server is running
		/// </summary>
		/// <returns>True if it is running</returns>
		public static bool IsRunning() 
		{
			lock( lockObject ) 
			{
				return running;
			}
		}
		/// <summary>
		/// Stop the Identd server and close the thread.
		/// </summary>
		public static void Stop() 
		{
			lock( lockObject ) 
			{
				if( running == true ) 
				{
					listener.Stop();
					Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"[" + Thread.CurrentThread.Name +"] Identd::Stop()");
					listener = null;
					running = false;
				}
			}
		}

		private static void Run() 
		{
			Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"[" + Thread.CurrentThread.Name +"] Identd::Run()");
			try 
			{
				listener = new TcpListener( IdentdPort );
				listener.Start();

				loop:
				{
					try 
					{
						TcpClient client = listener.AcceptTcpClient();
						//Read query
						StreamReader reader =  new StreamReader(client.GetStream() );
						string line = reader.ReadLine();
						Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceVerbose,"[" + Thread.CurrentThread.Name +"] Identd::Run() received=" + line);

						//Send back reply
						StreamWriter writer = new StreamWriter( client.GetStream() );
						writer.WriteLine( line.Trim() + Reply + username );
						writer.Flush();

						//Close connection with client
						client.Close();
					}
					catch( IOException ioe ) 
					{
						Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceWarning,"[" + Thread.CurrentThread.Name +"] Identd::Run() exception=" + ioe);
					}
					goto loop;
				}
			}
			catch( Exception e ) 
			{
				Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"[" + Thread.CurrentThread.Name +"] Identd::Run() Identd stopped");
			}
			finally 
			{
				running = false;
			}
		}

	}
}
