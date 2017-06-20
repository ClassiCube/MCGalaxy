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
using System.IO;
using System.Windows.Forms;
using MCGalaxy.Util;

namespace MCGalaxy.Gui.Popups {
    public partial class EditText : Form {
        TextFile curFile;
        
        public EditText() {
            InitializeComponent();
            foreach (var kvp in TextFile.Files) {
                cmbList.Items.Add(kvp.Key);
            }
            cmbList.Text = "Select file..";
        }
        
        void cmbList_SelectedIndexChanged(object sender, EventArgs e) {
            if (cmbList.SelectedIndex == -1) return;
            SaveCurrentFile();
            
            string selectedName = cmbList.SelectedItem.ToString();
            curFile = TextFile.Files[selectedName];
            
            try {
                curFile.EnsureExists();
                txtEdit.Lines = curFile.GetText();
                Text = "Editing " + curFile.Filename;
            } catch (Exception ex) {
                Logger.LogError(ex);
                MessageBox.Show("Failed to read text from " + curFile.Filename);
                
                curFile = null;
                cmbList.Text = "";
                Text = "Editing (none)";
            }
        }
        
        void SaveCurrentFile() {
            if (curFile == null) return;
            string[] userLines = txtEdit.Lines;
            if (!HasTextChanged(userLines)) return;
            
            string msg = "Save changes to " + curFile.Filename + "?";
            if (MessageBox.Show(msg, "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                curFile.SetText(userLines);
                MessageBox.Show("Saved " + curFile.Filename);
            }
        }
        
        bool HasTextChanged(string[] userLines) {
            string[] lines = curFile.GetText();
            if (lines.Length != userLines.Length) return true;
            
            for (int i = 0; i < lines.Length; i++) {
                if (userLines[i] != lines[i]) return true;
            }
            return false;
        }
        

        void btnColor_Click(object sender, EventArgs e) {
            using (ColorSelector sel = new ColorSelector("Insert color", '\0')) {
                DialogResult result = sel.ShowDialog();
                if (result == DialogResult.Cancel) return;
                
                string color = "&" + sel.ColorCode;
                int selStart = txtEdit.SelectionStart, selLength = txtEdit.SelectionLength;
                
                txtEdit.Paste(color);
                // re highlight now replaced text
                if (selLength > 0) txtEdit.Select(selStart, 2);
                txtEdit.Focus();
            }
        }
        
        void EditTxt_Unload(object sender, EventArgs e) {
            SaveCurrentFile();
        }
    }
}
