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
using System.Text;
using System.Text.RegularExpressions;

namespace Sharkbite.Irc
{

	/// <summary>
	/// This class conatins a set of methods for adding and removing mIRC color
	/// and other control codes.
	/// </summary>
	public sealed class TextColor
	{

		private const char ColorControl = '\x0003';
		private const char UnderlineControl = '\x001F';
		private const char BoldControl = '\x0002';
		private const char PlainControl = '\x000F';
		private const char ReverseControl = '\x0016';

		private const string TextColorFormat = "\x0003{0}{1}\x0003" ;
		private const string FullColorFormat = "\x0003{0},{1}{2}\x0003";

		private static readonly Regex colorPattern;

		static TextColor() 
		{
			colorPattern = new Regex("\\u0003[\\d]{1,2}(,[\\d]{1,2})?([^\\u0003]+)\\u0003", RegexOptions.Compiled | RegexOptions.Singleline);
		}

		private TextColor()
		{
		}

		/// <summary>
		/// Removes all mIRC control codes from the string.
		/// </summary>
		/// <param name="text">Dirty text.</param>
		/// <returns>Cleaned text.</returns>
		public static string StripControlChars( string text ) 
		{
			StringBuilder buffer = new StringBuilder();
			text = StripColor(text);

			foreach( char c in text ) 
			{
				if( !IsControlCode( c ) ) 
				{
					buffer.Append( c );
				}
			}
			return buffer.ToString();
		}

		private static string StripColor( string text ) 
		{
			Match match = colorPattern.Match( text );
			if( match.Success ) 
			{
				return text.Substring(0, match.Index) +
					match.Groups[2].ToString() +
					text.Substring( (match.Index + match.Length)  );
			}
			return text;
		}

		/// <summary>
		/// Add Bold control codes.
		/// </summary>
		/// <param name="text">A piece of text.</param>
		/// <returns>The text with the added control codes.</returns>
		public static string MakeBold( string text )
		{
			return BoldControl + text + BoldControl;
		}

		/// <summary>
		/// Add Plain control codes.
		/// </summary>
		/// <param name="text">A piece of text.</param>
		/// <returns>The text with the added control codes.</returns>
		public static string MakePlain( string text )
		{
			return PlainControl + text + PlainControl;
		}

		/// <summary>
		/// Add Underline control codes.
		/// </summary>
		/// <param name="text">A piece of text.</param>
		/// <returns>The text with the added control codes.</returns>
		public static string MakeUnderline( string text )
		{
			return UnderlineControl + text + UnderlineControl;
		}

		/// <summary>
		/// Add Rverse Video control codes.
		/// </summary>
		/// <param name="text">A piece of text.</param>
		/// <returns>The text with the added control codes.</returns>
		public static string MakeReverseVideo( string text )
		{
			return ReverseControl + text + ReverseControl;
		}

		/// <summary>
		/// Add Color control codes.
		/// </summary>
		/// <param name="text">A piece of text.</param>
		/// <param name="textColor">The color of the text taken from one of the mIRC color enums.</param>
		/// <returns>The text with the added control codes.</returns>
		public static string MakeColor( string text, MircColor textColor )
		{
			return string.Format( TextColorFormat, (int) textColor, text ); 
		}

		/// <summary>
		/// Add Color control codes.
		/// </summary>
		/// <param name="text">A piece of text.</param>
		/// <param name="textColor">The color of the text taken from one of the mIRC color enums.</param>
		/// <param name="backgroundColor">The background of the designated text.</param>
		/// <returns>The text with the added control codes.</returns>
		public static string MakeColor( string text, MircColor textColor, MircColor backgroundColor )
		{
			return string.Format( FullColorFormat, (int) textColor, (int)backgroundColor, text ); 
		}


		private static bool IsControlCode( char c ) 
		{
			return
				c == '\x0003' ||
				c == '\x001F' ||
				c == '\x0002' ||
				c == '\x000F' ||
				c == '\x0016';
		}

		
		
	}
}
