/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Windows.Forms;

namespace MCGalaxy.Gui {
    [DefaultBindingProperty("Seconds"), DefaultEvent("ValueChanged"), DefaultProperty("Seconds")]
    public class TimespanUpDown : UpDownBase, ISupportInitialize {
        long totalSecs;
        bool initialising;

        public event EventHandler ValueChanged;
        
        [Bindable(true)]
        public long Seconds {
            get {
                if (UserEdit) ValidateEditText();
                return totalSecs;
            }
            set {
                if (value == totalSecs) return;
                if (value < 0) value = 0;
                
                totalSecs = value;
                if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
                UpdateEditText();
            }
        }
        
        public TimeSpan Value {
            get { return TimeSpan.FromSeconds(Seconds); }
            set { Seconds = value.SecondsLong(); }
        }
        
        public TimespanUpDown() { Text = "0s"; }
        public void BeginInit() { initialising = true; }
        
        public void EndInit() {
            initialising = false;
            Seconds = totalSecs;
            UpdateEditText();
        }

        protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e) {
            base.OnTextBoxKeyPress(source, e);
            // don't intercept ctrl+A, ctrl+C etc
            if ((Control.ModifierKeys & (Keys.Control | Keys.Alt)) != Keys.None) return;
            // always allowed to input numbers
            if (e.KeyChar == '\b' || char.IsDigit(e.KeyChar)) return;
            
            try {
                (Text + e.KeyChar.ToString()).ParseShort("s");
            } catch {
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
        }
        
        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            if (UserEdit) UpdateEditText();
        }
        
        public override void DownButton() {
            if (UserEdit) ParseEditText();
            if (totalSecs <= 0) totalSecs = 1;
            Seconds = totalSecs - 1;
        }
        
        public override void UpButton() {
            if (UserEdit) ParseEditText();
            if (totalSecs == long.MaxValue) return;
            Seconds = totalSecs + 1;
        }

        void ParseEditText() {
            try {
                Value = Text.ParseShort("s");
            } catch {
            } finally {
                UserEdit = false;
            }
        }
        
        protected override void UpdateEditText() {
            if (initialising) return;
            if (UserEdit) ParseEditText();
            Text = TimeSpan.FromSeconds(totalSecs).Shorten(true, true);
        }
        
        protected override void ValidateEditText() {
            ParseEditText();
            UpdateEditText();
        }
    }
}