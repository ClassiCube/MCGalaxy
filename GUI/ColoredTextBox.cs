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

namespace MCGalaxy.Gui.Components {

    /// <summary> Extended rich text box that auto-colors minecraft classic text. </summary>
    public partial class ColoredTextBox : RichTextBox {

        bool _nightMode = false, _colorize = true;
        bool _showDateStamp = true, _autoScroll = true;

        public bool AutoScroll {
            get { return _autoScroll; }
            set {
                _autoScroll = value;
                if ( value ) ScrollToEnd(0);
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

        /// <summary> Initializes a new instance of the <see cref="ColoredTextBox"/> class. </summary>
        public ColoredTextBox() : base() {
            LinkClicked += HandleLinkClicked;
        }

        /// <summary> Appends text to this textbox. </summary>
        public void AppendLog(string text, Color foreColor, bool dateStamp) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)(() => AppendLog(text, foreColor, dateStamp)));
                return;
            }
            if (dateStamp) AppendLog(CurrentDate, Color.Gray);
            int line = GetLineFromCharIndex(Math.Max(0, TextLength - 1));
            
            if (!Colorize) {
                AppendText(text);
            } else {
                 LineFormatter.Format(text, (c, s) => LineFormatter.FormatGui(c, s, this, foreColor));
            }         
            if (AutoScroll) ScrollToEnd(line);
        }

        /// <summary> Appends text to this textbox. </summary>
        public void AppendLog(string text) { AppendLog(text, ForeColor, DateStamp); }

        /// <summary> Appends text to this textbox. </summary>
        internal void AppendLog(string text, Color color) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)(() => AppendLog(text, color))); return;
            }
            
            int selLength = SelectionLength, selStart = 0;
            if (selLength > 0) selStart = SelectionStart;
            AppendColoredText(text, color);
            
            // preserve user's selection when appending text
            if (selLength == 0) return;
            SelectionStart = selStart;
            SelectionLength = selLength;
        }
        
        void AppendColoredText(string text, Color color) {
            SelectionStart = TextLength;
            SelectionLength = 0;
            
            SelectionColor = color;
            AppendText(text);
            SelectionColor = ForeColor;            
        }

        void HandleLinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e) {
            if ( !e.LinkText.StartsWith("http://www.minecraft.net/classic/play/") ) {
                if ( MessageBox.Show("Never open links from people that you don't trust!", "Warning!!", MessageBoxButtons.OKCancel) == DialogResult.Cancel )
                    return;
            }

            try { Process.Start(e.LinkText); }
            catch { }
        }

        /// <summary> Scrolls to the end of the log </summary>
        public void ScrollToEnd(int startIndex) {
            if ( InvokeRequired ) {
                Invoke((MethodInvoker)(() => ScrollToEnd(startIndex)));
                return;
            }
            
            int lines = GetLineFromCharIndex(TextLength - 1) - startIndex + 1;
            for (int i = 0; i < lines; i++) {
                SendMessage(Handle, 0xB5, (IntPtr)1, IntPtr.Zero);
            }
            Invalidate();
        }
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
