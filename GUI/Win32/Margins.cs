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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MCGalaxy.Gui.Utils {

    /// <summary>
    /// Native margins
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct Margins {

        private int _Left;
        private int _Right;
        private int _Top;
        private int _Bottom;

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public int Left {
            get {
                return _Left;
            }
            set {
                _Left = value;
            }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        public int Right {
            get {
                return _Right;
            }
            set {
                _Right = value;
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public int Top {
            get {
                return _Top;
            }
            set {
                _Top = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>
        /// The bottom.
        /// </value>
        public int Bottom {
            get {
                return _Bottom;
            }
            set {
                _Bottom = value;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Margins"/> struct.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="top">The top.</param>
        /// <param name="bottom">The bottom.</param>
        public Margins( int left, int right, int top, int bottom ) {
            _Top = top;
            _Bottom = bottom;
            _Right = right;
            _Left = left;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Margins"/> struct.
        /// </summary>
        /// <param name="allMargs">All of the values.</param>
        public Margins( int allMargs )
            : this( allMargs, allMargs, allMargs, allMargs ) {
        }


        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty {
            get {
                return Left <= 0 && Right <= 0 && Top <= 0 && Bottom <= 0;
            }
        }

        /// <summary>
        /// Determines whether the specified point is touching glass
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the specified point is touching glass otherwise, <c>false</c>.
        /// </returns>
        public bool IsTouchingGlass( Point point ) {
            if ( IsEmpty )
                return true;

            return ( point.X < _Left ||
                           point.X > _Right ||
                           point.Y < _Top ||
                           point.Y > _Bottom );
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MCGalaxy.Gui.Utils.Margins"/> to <see cref="MCGalaxy.Gui.Utils.RECT"/>.
        /// </summary>
        /// <param name="margs">The margs.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator RECT( Margins margs ) {
            return new RECT( margs.Left, margs.Top, margs.Right, margs.Bottom );
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MCGalaxy.Gui.Utils.Margins"/> to <see cref="System.Windows.Forms.Padding"/>.
        /// </summary>
        /// <param name="margs">The margs.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Padding( Margins margs ) {
            return new Padding( margs.Left, margs.Top, margs.Right, margs.Bottom );
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MCGalaxy.Gui.Utils.Margins"/> to <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="margs">The margs.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Rectangle( Margins margs ) {
            return new Rectangle( margs.Left, margs.Top, margs.Right, margs.Bottom );
        }



        /// <summary>
        /// Performs an implicit conversion from <see cref="MCGalaxy.Gui.Utils.RECT"/> to <see cref="MCGalaxy.Gui.Utils.Margins"/>.
        /// </summary>
        /// <param name="margs">The margs.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Margins( RECT margs ) {
            return new Margins( margs.Left, margs.Right, margs.Top, margs.Bottom );
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Drawing.Rectangle"/> to <see cref="MCGalaxy.Gui.Utils.Margins"/>.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Margins( Rectangle rect ) {
            return new Margins( rect.X, rect.Width, rect.Y, rect.Height );
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Windows.Forms.Padding"/> to <see cref="MCGalaxy.Gui.Utils.Margins"/>.
        /// </summary>
        /// <param name="margs">The margs.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Margins( Padding margs ) {
            return new Margins( margs.Left, margs.Right, margs.Top, margs.Bottom );
        }

    }
}
