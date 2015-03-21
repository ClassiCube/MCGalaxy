/*
	Copyright 2011 MCGalaxy
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
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

namespace MCGalaxy.Gui
{
	public class AutoScrollTextBox : System.Windows.Forms.TextBox
	{
		// Constants for extern calls to various scrollbar functions
		private const int SB_HORZ = 0x0;
		private const int SB_VERT = 0x1;
		private const int WM_HSCROLL = 0x114;
		private const int WM_VSCROLL = 0x115;
		private const int SB_THUMBPOSITION = 4;
		private const int SB_BOTTOM = 7;
		private const int SB_OFFSET = 13;

		delegate void LogDelegate(string message);

		[DllImport("user32.dll")]
		static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetScrollPos(IntPtr hWnd, int nBar);
		[DllImport("user32.dll")]
		private static extern bool PostMessageA(IntPtr hWnd, int nBar, int wParam, int lParam);
		[DllImport("user32.dll")]
		static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

		public void AppendTextAndScroll(string text)
		{
			bool bottomFlag = false;
			int VSmin;
			int VSmax;
			int sbOffset;
			int savedVpos;
			// Make sure this is done in the UI thread

			if (this.InvokeRequired)
			{
				LogDelegate d = new LogDelegate(AppendTextAndScroll);
				this.Invoke(d, new object[] { text, true });
			}

			else
			{
				// Win32 magic to keep the textbox scrolling to the newest append to the textbox unless
				// the user has moved the scrollbox up
				sbOffset = (int)((this.ClientSize.Height - SystemInformation.HorizontalScrollBarHeight) / (this.Font.Height));
				savedVpos = GetScrollPos(this.Handle, SB_VERT);
				GetScrollRange(this.Handle, SB_VERT, out VSmin, out VSmax);
				if (savedVpos >= (VSmax - sbOffset - 1))
					bottomFlag = true;
				this.AppendText(text + Environment.NewLine);
				if (bottomFlag)
				{
					GetScrollRange(this.Handle, SB_VERT, out VSmin, out VSmax);
					savedVpos = VSmax - sbOffset;
					bottomFlag = false;
				}
				SetScrollPos(this.Handle, SB_VERT, savedVpos, true);
				PostMessageA(this.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * savedVpos, 0);
			}
			this.Text = ReturnLast250Lines(this.Text);


		}

		public string ReturnLast250Lines(string text)  //NOT WORKIN!
		{
			string final = "";
			string[] array = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			int i = array.Length - 1;
			if (array.Length != 0)
			{
				while (i >= 0)
				{
					if (array[i] != "[PLEASE REFER TO THE LOG TAB FOR THE REST OF THE LOGS!!!]")
					{
						if (array.Length - i >= 250)
						{
							return ("[PLEASE REFER TO THE LOG TAB FOR THE REST OF THE LOGS!!!]" + Environment.NewLine + final);
						}
						final = array[i] + "\r\n" + final;
						i--;
					}
				}
				return final;
			}
			return text;
		}
	}
}