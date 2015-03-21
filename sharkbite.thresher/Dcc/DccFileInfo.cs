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

namespace Sharkbite.Irc
{
	/// <summary>
	/// Manages the information about the file being
	/// transfered. 
	/// </summary>
	public sealed class DccFileInfo
	{
		private FileInfo fileInfo;
		private FileStream fileStream;
		//Where in the file to start reading or writing
		private long fileStartingPosition;
		//Number of bytes sent or received so far in this
		//session
		private long bytesTransfered;
		//Total number of bytes to send or receive
		private long completeFileSize;
		//The last position ack value
		private long lastAckValue;

		/// <summary>
		/// Create a new instance using information sent from the remote user
		/// in his DCC Send message.
		/// </summary>
		/// <param name="fileInfo">The file being received</param>
		/// <param name="completeFileSize">The size of the file being received as specified in the DCC Send
		/// request.</param>
		public DccFileInfo( FileInfo fileInfo, long completeFileSize)
		{	
			this.fileInfo = fileInfo;
			this.completeFileSize = completeFileSize;
			fileStartingPosition = 0;
			bytesTransfered = 0;
		}
		/// <summary>
		/// Create a new instance using the file information from a local file
		/// to be sent to a remote user.
		/// </summary>
		/// <param name="fileInfo">The local file being sent</param>
		/// <exception cref="ArgumentException">If the file does not already exist.</exception>
		public DccFileInfo( FileInfo fileInfo) 
		{
			this.fileInfo = fileInfo;
			if( !fileInfo.Exists ) 
			{
				throw new ArgumentException( fileInfo.Name + " does not exist.");
			}
			this.completeFileSize = fileInfo.Length;
			fileStartingPosition = 0;
			bytesTransfered = 0;
		}
		/// <summary>
		/// Create a new instance using the file information from a local file
		/// to be sent to a remote user.
		/// </summary>
		/// <param name="fileName">The full pathname of local file being sent</param>
		/// <exception cref="ArgumentException">If the file does not already exist.</exception>
		public DccFileInfo( string fileName ) 
		{
			this.fileInfo = new FileInfo(fileName);
			if( !fileInfo.Exists ) 
			{
				throw new ArgumentException( fileName + " does not exist.");
			}
			this.completeFileSize = fileInfo.Length;
			fileStartingPosition = 0;
			bytesTransfered = 0;
		}

		/// <summary>
		/// Where to start reading or writing a file. Used during DCC Resume actions.
		/// </summary>
		/// <value>A read-only long indicating the location within the file.</value>
		public long FileStartingPosition 
		{
			get 
			{
				return fileStartingPosition;
			}
		}
		/// <summary>
		/// The number of bytes sent or received so far. This Property
		/// is thread safe.
		/// </summary>
		/// <value>A read-only long.</value>
		public long BytesTransfered
		{
			get 
			{
				lock (this ) 
				{
					return bytesTransfered;
				}
			}
		}
		/// <summary>
		/// The length of the file. This number is either the actual size
		/// of a file being sent or the number sent in the DCC SEND request.
		/// </summary>
		/// <value>A read-only long.</value>
		public long CompleteFileSize 
		{
			get 
			{
				return completeFileSize;
			}
		}
		/// <summary>
		/// The file's name with all spaces converted to underscores and
		/// without the path.
		/// </summary>
		/// <value>A read-only string.</value>
		public string DccFileName 
		{
			get 
			{
				return DccUtil.SpacesToUnderscores(fileInfo.Name);
			}
		}

		internal FileStream TransferStream 
		{
			get 
			{
				return fileStream;
			}
		}

		/// <summary>
		/// Add the most recent number of bytes received
		/// to the total count.
		/// </summary>
		/// <param name="additionalBytes"></param>
		internal void AddBytesTransfered( int additionalBytes ) 
		{
			lock( this ) 
			{
				bytesTransfered += additionalBytes;
			}
		}
		/// <summary>
		/// Does the position sent in the DCC Accept message
		/// match what we expect?
		/// </summary>
		internal bool AcceptPositionMatches( long position ) 
		{
			return position == fileStartingPosition;
		}
		/// <summary>
		/// Our Resume request was accepted so start
		/// writing at the current position + 1.
		/// </summary>
		internal void GotoWritePosition() 
		{
			fileStream.Seek( fileStartingPosition +1, SeekOrigin.Begin );
		}
		/// <summary>
		/// Advance to the correct reading start
		/// position.
		/// </summary>
		internal void GotoReadPosition() 
		{
			fileStream.Seek( fileStartingPosition, SeekOrigin.Begin );
		}
		/// <summary>
		/// Is the position where the remote user would to to resume
		/// valid?
		/// </summary>
		internal bool ResumePositionValid( long position ) 
		{
			return position > 1 && position < fileInfo.Length;
		}
		/// <summary>
		/// Can this file be resumed, i.e. does it
		/// support random access?
		/// </summary>
		internal bool CanResume() 
		{
			return fileStream.CanSeek;
		}
		/// <summary>
		/// Start a Resume where the file last left off.
		/// </summary>
		internal void SetResumeToFileSize() 
		{
			fileStartingPosition = fileInfo.Length;
		}
		/// <summary>
		/// Set the point at which the transfer will begin
		/// </summary>
		internal void SetResumePosition( long resumePosition ) 
		{
			fileStartingPosition = resumePosition;
			bytesTransfered = fileStartingPosition;
		}
		/// <summary>
		/// Where in the file is the transfer currently at?
		/// </summary>
		internal long CurrentFilePosition() 
		{
			return BytesTransfered + fileStartingPosition;
		}
		/// <summary>
		/// Have all the file's bytes been sent/received?
		/// </summary>
		internal Boolean AllBytesTransfered()
		{
			if( completeFileSize == 0 ) 
			{
				return false;
			}
			else 
			{
				return (fileStartingPosition + BytesTransfered ) == completeFileSize;
			}
		}
		/// <summary>
		/// Close the file stream.
		/// </summary>
		internal void CloseFile() 
		{
			if( fileStream != null ) 
			{
				fileStream.Close();
			}
		}
		/// <summary>
		/// Set this file stream to a read only one.
		/// </summary>
		internal void OpenForRead() 
		{
			fileStream = fileInfo.OpenRead();
		}
		/// <summary>
		/// Set this file stream to a write only one.
		/// </summary>
		internal void OpenForWrite() 
		{
			fileStream = fileInfo.OpenWrite();
		}
		/// <summary>
		/// Should we try to resume this file download?
		/// </summary>
		internal bool ShouldResume() 
		{
			return fileInfo.Length > 0 && CanResume();
		}
		/// <summary>
		/// Determine whether the acks sent during an upload
		/// signal that all bytes have been sent.
		/// 
		/// BitchX sends bad acks after a resume but we can
		/// catch that by testing for the same ack sent twice.
		/// I sure hope others behave better since I don't
		/// want to write special code for every IRC client.
		/// </summary>
		/// <param name="ack"></param>
		/// <returns>True if the acks are done</returns>
		internal bool AcksFinished( long ack ) 
		{
			bool done = (ack == BytesTransfered || ack == lastAckValue);
			lastAckValue = ack;
			return done;
		}
	
	}
}
