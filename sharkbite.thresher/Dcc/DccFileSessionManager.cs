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
using System.Collections;
using System.Threading;


namespace Sharkbite.Irc
{
	/// <summary>
	/// This class checks each file session to see if it has not 
	/// had any activity within the timeout period so that
	/// inactive sessions can be closed.
	/// </summary>
	public sealed class DccFileSessionManager
	{
		//How long to wait
		private TimeSpan timeout;
		//A clone of the session hashtable to iterate over
		private Hashtable sessionClone;
		//A place to store the active sessions
		private Hashtable sessions;
		//Check for timeouts every 10 seconds
		private const int TimeoutCheckPeriod = 10000;
		//Default to tming out after 30 seconds of no activity.
		private const int DefaultTimeout = 30000;
		private static DccFileSessionManager defaultInstance;
		private static object lockObject = new object();
		private Timer timerThread;
		private bool timerStopped;

		private DccFileSessionManager( )
		{
			timeout = new TimeSpan( DefaultTimeout * TimeSpan.TicksPerMillisecond );
			//Create Timer but don't start it yet
			timerThread = new Timer( new TimerCallback( CheckSessions ), null, Timeout.Infinite, TimeoutCheckPeriod );
			timerStopped = true;
			sessions = Hashtable.Synchronized( new Hashtable() );
		}
	
		private Boolean TimedOut( DccFileSession session ) 
		{
			if( ( DateTime.Now - session.LastActivity ) >= timeout )
			{
				return true;
			}
			return false;
		}

		internal void AddSession( DccFileSession session ) 
		{
			sessions.Add( session.ID, session );
			if( timerStopped ) 
			{
				timerStopped = false;
				timerThread.Change(TimeoutCheckPeriod, TimeoutCheckPeriod);
			}
			Debug.WriteLineIf( DccUtil.DccTrace.TraceInfo, "[" + Thread.CurrentThread.Name +"] DccSessionManager::AddSession() ID=" + session.ID );
		}
		internal void RemoveSession( DccFileSession session ) 
		{
			sessions.Remove( session.ID );
			if( sessions.Count == 0 ) 
			{
				timerStopped = true;
				timerThread.Change( Timeout.Infinite, TimeoutCheckPeriod );
			}
			Debug.WriteLineIf( DccUtil.DccTrace.TraceInfo, "[" + Thread.CurrentThread.Name +"] DccSessionManager::RemoveSession() ID=" + session.ID );
		}
		internal void CheckSessions( object state ) 
		{
			Debug.WriteLineIf( DccUtil.DccTrace.TraceVerbose, "[" + Thread.CurrentThread.Name +"] DccSessionManager::CheckSessions()");
			sessionClone = (Hashtable) sessions.Clone();
			foreach( object session in sessionClone.Values ) 
			{
				DccFileSession fileSession = (DccFileSession) session;
				lock( fileSession ) 
				{
					if( TimedOut( fileSession ) ) 
					{
						fileSession.TimedOut();
					}
				}
			}
		}
		internal bool ContainsSession( string sessionID )
		{
			return sessions.Contains( sessionID );
		}
		internal DccFileSession LookupSession( string sessionID ) 
		{
			//Make sure this session is till active
			if( !ContainsSession( sessionID ) ) 
			{
				throw new ArgumentException( sessionID + " is not active.");
			}
			//Lookup corresponding session
			return (DccFileSession) sessions[ sessionID ];
		}

		/// <summary>
		/// Returns the singleton instance.
		/// </summary>
		public static DccFileSessionManager DefaultInstance
		{
			get 
			{
				lock( lockObject ) 
				{
					if( defaultInstance == null ) 
					{
						defaultInstance = new DccFileSessionManager();
						Debug.WriteLineIf( DccUtil.DccTrace.TraceVerbose, "[" + Thread.CurrentThread.Name +"] DccFileSessionManager::init");
					}
				}
				return defaultInstance;
			}
		}
		/// <summary>
		/// Timeout period in milliseconds
		/// </summary>
		public long TimeoutPeriod 
		{
			get
			{
				lock( defaultInstance ) 
				{
					return timeout.Ticks * TimeSpan.TicksPerMillisecond;
				}
			}
			set 
			{
				lock( defaultInstance ) 
				{
					timeout = new TimeSpan( value * TimeSpan.TicksPerMillisecond );
				}
			}
		}

	}
}
