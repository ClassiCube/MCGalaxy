/*
    Copyright 2012 MCGalaxy 
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at'
 
http://www.opensource.org/licenses/ecl2.php
http://www.gnu.org/licenses/gpl-3.0.html
 
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MCGalaxy.Gui.Utils {
    public class Utilities {

        /// <summary>
        /// Gets a color from a char.
        /// </summary>
        /// <param name="c">The char.</param>
        /// <returns>A color, that can be null</returns>
        public static Color? GetDimColorFromChar( char c ) {
            switch ( c ) {
                case '0': return Color.Black;
                case '1': return Color.FromArgb( 255, 0, 0, 161 );
                case '2': return Color.FromArgb( 255, 0, 161, 0 );
                case '3': return Color.FromArgb( 255, 0, 161, 161 );
                case '4': return Color.FromArgb( 255, 161, 0, 0 );
                case '5': return Color.FromArgb( 255, 161, 0, 161 );
                case '6': return Color.FromArgb( 255, 161, 161, 0 );
                case '7': return Color.FromArgb( 255, 161, 161, 161 );
                case '8': return Color.FromArgb( 255, 34, 34, 34 );
                case '9': return Color.FromArgb( 255, 34, 34, 225 );
                case 'a': return Color.FromArgb( 255, 34, 225, 34 );
                case 'b': return Color.FromArgb( 255, 34, 225, 225 );
                case 'c': return Color.FromArgb( 255, 225, 34, 34 );
                case 'd': return Color.FromArgb( 255, 225, 34, 225 );
                case 'e': return Color.FromArgb( 255, 225, 225, 34 );
                case 'f': return Color.Black;
                default: return null;
            }
        }

        public static int HexIntFromChar( char hexChar ) {
            hexChar = char.ToUpper( hexChar );  // may not be necessary

            return ( int ) hexChar < ( int ) 'A' ?
                ( ( int ) hexChar - ( int ) '0' ) :
                10 + ( ( int ) hexChar - ( int ) 'A' );
        }

        public static char HexCharFromInt( int hexInt ) {
            return char.Parse(hexInt.ToString());
        }

    }
}
