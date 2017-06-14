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

namespace MCGalaxy.Gui {
    public partial class EditText : Form {
        
		TextFile currentFile;
		string currentText;
		
        public EditText() {
            InitializeComponent();
            foreach (var kvp in TextFile.Files) {
                cmbList.Items.Add(kvp.Key);
            }
        }

        bool loaded = false;
        string oldtxt;
        string loadedfile;

        private void LoadTxt_Click(object sender, EventArgs e) {
            SaveCurrentFile(sender, e);
            
            
            loaded = true;
            try
            {
                if (File.Exists("text/" + loadedfile + ".txt")) { oldtxt = File.ReadAllText("text/" + loadedfile + ".txt"); }
                else { MessageBox.Show("File doesn't exist!!"); loaded = false; loadedfile = null; return; }
            }
            catch { MessageBox.Show("Something went wrong!!"); loaded = false; loadedfile = null; return; }
            txtEdit.Text = oldtxt;
        }
        
        
        void EditText_SelectedIndexChanged(object sender, EventArgs e) {
        	if (cmbList.SelectedIndex == -1) return;
        	SaveCurrentFile();
        	
        	string selectedName = cmbList.SelectedItem.ToString();
        	currentFile = TextFile.Files[selectedName];
        	currentText = 
        }
        
        void SaveCurrentFile() {
        	if (currentFile == null) return;
        	
        	string msg = "Save changes to " + currentFile.Filename + "?";
            if (MessageBox.Show(msg, MessageBoxButtons.YesNo) == DialogResult.Yes) {
        		currentFile.SetText(currentText);
        		MessageBox.Show("Saved " + currentFile.Filename);
            }
        }
        
        void EditTxt_Unload(object sender, EventArgs e) {
            SaveCurrentFile(sender, e);
        }
    }
}
