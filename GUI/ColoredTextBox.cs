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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

//MCGalaxy 6 Preview :D

namespace MCGalaxy.Gui.Components {

    /// <summary> A rich text box, that can parse Minecraft/MCGalaxy color codes. </summary>
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
            InitializeComponent();
        }

        /// <summary> Appends the log. </summary>
        /// <param name="text">The text to log.</param>
        public void AppendLog(string text, Color foreColor, bool dateStamp) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)(() => AppendLog(text, foreColor, dateStamp)));
                return;
            }
            if (dateStamp) Append(CurrentDate, Color.Gray);
            int line = GetLineFromCharIndex(Math.Max(0, TextLength - 1));
            
            if (!Colorize) {                
                AppendText(text);                
                if (AutoScroll) ScrollToEnd(line);
                return;
            }
            LineFormatter.Format(text, (c, s) => LineFormatter.FormatGui(c, s, this, foreColor));
            if (AutoScroll) ScrollToEnd(line);
        }

        /// <summary> Appends the log. </summary>
        /// <param name="text">The text to log.</param>
        public void AppendLog(string text) {
            AppendLog(text, ForeColor, DateStamp);
        }

        /// <summary> Appends the log. </summary>
        /// <param name="text">The text to log.</param>
        /// <param name="foreColor">Color of the foreground.</param>
        internal void Append(string text, Color foreColor) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)(() => Append(text, foreColor))); return;
            }

            SelectionStart = TextLength;
            SelectionLength = 0;
            SelectionColor = foreColor;
            AppendText(text);
            SelectionColor = ForeColor;
        }

        void ColoredReader_LinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e) {
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
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
