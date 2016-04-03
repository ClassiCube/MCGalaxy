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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MCGalaxy.Gui.Utils {

    internal class Natives {

        #region Consts

        public const int S_OK = 0x0;

        public const int EP_EDITTEXT = 1;
        public const int ETS_DISABLED = 4;
        public const int ETS_NORMAL = 1;
        public const int ETS_READONLY = 6;

        public const int WM_THEMECHANGED = 0x031A;
        public const int WM_NCPAINT = 0x85;
        public const int WM_NCCALCSIZE = 0x83;

        public const int WS_EX_CLIENTEDGE = 0x200;
        public const int WVR_HREDRAW = 0x100;
        public const int WVR_VREDRAW = 0x200;
        public const int WVR_REDRAW = ( WVR_HREDRAW | WVR_VREDRAW );

        #endregion

        #region Enums/Structs

        [StructLayout( LayoutKind.Sequential )]
        public struct DLLVersionInfo {
            public int cbSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformID;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct NCCALCSIZE_PARAMS {
            public RECT rgrc0, rgrc1, rgrc2;
            public IntPtr lppos;
        }

        #endregion

        #region P/Invoke API Calls

        [DllImport( "UxTheme.dll", CharSet = CharSet.Auto )]
        public static extern bool IsAppThemed();

        [DllImport( "UxTheme.dll", CharSet = CharSet.Auto )]
        public static extern bool IsThemeActive();

        [DllImport( "comctl32.dll", CharSet = CharSet.Auto )]
        public static extern int DllGetVersion( ref DLLVersionInfo version );

        [DllImport( "uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode )]
        public static extern IntPtr OpenThemeData( IntPtr hWnd, String classList );

        [DllImport( "uxtheme.dll", ExactSpelling = true )]
        public extern static Int32 CloseThemeData( IntPtr hTheme );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static Int32 DrawThemeBackground( IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pRect, IntPtr pClipRect );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static int IsThemeBackgroundPartiallyTransparent( IntPtr hTheme, int iPartId, int iStateId );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static Int32 GetThemeBackgroundContentRect( IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pBoundingRect, out RECT pContentRect );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static Int32 DrawThemeParentBackground( IntPtr hWnd, IntPtr hdc, ref RECT pRect );

        [DllImport( "user32.dll" )]
        public static extern IntPtr GetWindowDC( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        public static extern int ReleaseDC( IntPtr hWnd, IntPtr hDC );

        [DllImport( "user32.dll" )]
        public static extern bool GetWindowRect( IntPtr hWnd, out RECT lpRect );

        [DllImport( "gdi32.dll" )]
        public static extern int ExcludeClipRect( IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect );

        #endregion

        #region Drawing / Utils

        //Most methods have been snipped

        public static bool CanRender() {
            Type t = typeof(Application);
            System.Reflection.PropertyInfo pi = t.GetProperty("RenderWithVisualStyles");

            if (pi == null) {
                OperatingSystem os = System.Environment.OSVersion;
                if (os.Platform == PlatformID.Win32NT && (((os.Version.Major == 5) && (os.Version.Minor >= 1)) || (os.Version.Major > 5))) {
                    DLLVersionInfo version = new DLLVersionInfo();
                    version.cbSize = Marshal.SizeOf(typeof(DLLVersionInfo));
                    if (DllGetVersion(ref version) == 0) {
                        return (version.dwMajorVersion > 5) && IsThemeActive() && IsAppThemed();
                    }
                }

                return false;
            } else {
            	return (bool)pi.GetValue(null, null);
            }
        }

        #endregion
    }
}
