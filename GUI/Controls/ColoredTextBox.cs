/*
    Copyright 2012 MCForge
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
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MCGalaxy.UI;

namespace MCGalaxy.Gui.Components {

    /// <summary> Extended rich text box that auto-colors minecraft classic text. </summary>
    public class ColoredTextBox : RichTextBox {

        bool _nightMode = false, _colorize = true;
        bool _showDateStamp = true, _autoScroll = true;
        int lines = 0;
        const int maxLines = 2000, linesToTrim = 25;

        public bool AutoScroll {
            get { return _autoScroll; }
            set {
                _autoScroll = value;
                if (value) ScrollToEnd(0);
            }
        }
        
        public bool Colorize {
            get { return _colorize; }
            set { _colorize = value; }
        }
        
        public bool DateStamp {
            get { return _showDateStamp; }
            set { _showDateStamp = value; }
        }
        
        public bool NightMode {
            get { return _nightMode; }
            set {
                _nightMode = value;
                ForeColor = value ? Color.Black : Color.White;
                BackColor = value ? Color.White : Color.Black;
                Invalidate();
            }
        }


        string CurrentDate { get { return "[" + DateTime.Now.ToString("T") + "] "; } }

        public ColoredTextBox() : base() {
            LinkClicked += HandleLinkClicked;
        }
        
        /// <summary> Clears all text from this textbox. </summary>
        public void ClearLog() {
            Clear();
            lines = 0;
        }
        
        /// <summary> Appends text to this textbox. </summary>
        public void AppendLog(string text) { AppendLog(text, ForeColor, DateStamp); }

        /// <summary> Appends text to this textbox. </summary>
        public void AppendLog(string text, Color color, bool dateStamp) {
            int line = GetLineFromCharIndex(Math.Max(0, TextLength - 1));
            int selLength = SelectionLength, selStart = 0;
            if (selLength > 0) selStart = SelectionStart;
            AppendLogCore(text, color, dateStamp);
            
            lines++;
            if (lines > maxLines) TrimLog(ref selStart);
            
            // preserve user's selection
            if (selLength > 0 && selStart > 0) {
                SelectionStart = selStart;
                SelectionLength = selLength;
            }
            if (AutoScroll) ScrollToEnd(line);
        }
        
        void AppendLogCore(string text, Color color, bool dateStamp) {
            if (dateStamp) AppendColoredText(CurrentDate, Color.Gray);
            
            if (!Colorize) {
                AppendText(text);
            } else {
                AppendFormatted(text, color);
            }
        }
        
        void TrimLog(ref int selStart) {
            int trimLength = GetFirstCharIndexFromLine(linesToTrim);
            selStart -= trimLength;
            lines -= linesToTrim;
            
            SelectionStart = 0;
            SelectionLength = trimLength;            
            string trimMsg = "----- cut off, see log files for rest of logs -----" + Environment.NewLine;
            
            SelectedText = trimMsg;
            SelectionColor = System.Drawing.Color.DarkGray;
            selStart += trimMsg.Length - 1;            
        }
        
        /// <summary> Appends text with a specific color to this textbox. </summary>
        internal void AppendColoredText(string text, Color color) {
            SelectionStart = TextLength;
            SelectionLength = 0;
            
            SelectionColor = color;
            AppendText(text);
            SelectionColor = ForeColor;
        }

        void HandleLinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e) {
            if (!Popup.OKCancel("Never open links from people that you don't trust!", "Warning!!")) return;
            try { Process.Start(e.LinkText); }
            catch { }
        }

        /// <summary> Scrolls to the end of the log </summary>
        internal void ScrollToEnd(int startIndex) {
            int lines = GetLineFromCharIndex(TextLength - 1) - startIndex + 1;
            try {
                for (int i = 0; i < lines; i++) {
                    SendMessage(Handle, 0xB5, (IntPtr)1, IntPtr.Zero);
                }
            } catch (DllNotFoundException) {
                // mono throws this if you're missing libMonoSupportW
                // TODO: Maybe we should cache this instead of catching all the time
            }
            Invalidate();
        }
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        
        void AppendFormatted(string message, Color foreColor) {
            int index = 0;
            char col = 'S';
            message = UIHelpers.Format(message);
            
            while (index < message.Length) {
                char curCol = col;
                string part = UIHelpers.OutputPart(ref col, ref index, message);
                if (part.Length > 0) AppendColoredText(part, GetCol(curCol, foreColor));
            }
        }
        
        static Color GetCol(char c, Color foreCol) {
            if (c == 'S' || c == 'f' || c == 'F' || c == '0') return foreCol;
            Colors.Map(ref c);

            if (!Colors.IsDefined(c)) return foreCol;
            ColorDesc col = Colors.Get(c);
            return Color.FromArgb(col.R, col.G, col.B);
        }
    }
}
