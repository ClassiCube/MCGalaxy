/*    
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
using System.Windows.Forms;
using MCGalaxy.UI;

namespace MCGalaxy.Gui {
    public partial class Window : Form {
        
        Player GetSelectedPlayer() {
            if (main_Players.SelectedRows.Count <= 0) return null;
            return (Player)(main_Players.SelectedRows[0].DataBoundItem);
        }
        
        void PlayerCmd(string command) {
            Player player = GetSelectedPlayer();
            if (player == null) return;
            UIHelpers.HandleCommand(command + " " + player.name);
        }
        
        void PlayerCmd(string command, string prefix, string suffix) {
            Player player = GetSelectedPlayer();
            if (player == null) return;
            UIHelpers.HandleCommand(command + " " + prefix + player.name + suffix);
        }
        
        void tsPlayer_Clones_Click(object sender, EventArgs e) { PlayerCmd("Clones"); }
        void tsPlayer_Voice_Click(object sender, EventArgs e) { PlayerCmd("Voice"); }
        void tsPlayer_Whois_Click(object sender, EventArgs e) { PlayerCmd("WhoIs"); }       
        void tsPlayer_Ban_Click(object sender, EventArgs e) { PlayerCmd("Ban"); }
        void tsPlayer_Kick_Click(object sender, EventArgs e) { PlayerCmd("Kick", "", " You have been kicked by the console."); }
        void tsPlayer_Promote_Click(object sender, EventArgs e) { PlayerCmd("SetRank", "+up ", ""); }
        void tsPlayer_Demote_Click(object sender, EventArgs e) { PlayerCmd("SetRank", "-down ", ""); }


        
        Level GetSelectedLevel() {
            if (main_Maps.SelectedRows.Count <= 0) return null;
            return (Level)(main_Maps.SelectedRows[0].DataBoundItem);
        }
        
        void LevelCmd(string command) {
            Level level = GetSelectedLevel();
            if (level == null) return;
            UIHelpers.HandleCommand(command + " " + level.name);
        }

        void LevelCmd(string command, string prefix, string suffix) {
            Level level = GetSelectedLevel();
            if (level == null) return;
            UIHelpers.HandleCommand(command + " " + prefix + level.name + suffix);
        }  
        
        void tsMap_Info_Click(object sender, EventArgs e) { LevelCmd("Map"); LevelCmd("mapinfo"); }
        void tsMap_MoveAll_Click(object sender, EventArgs e) { LevelCmd("MoveAll"); }
        void tsMap_Physics0_Click(object sender, EventArgs e) { LevelCmd("Physics", "", " 0"); }
        void tsMap_Physics1_Click(object sender, EventArgs e) { LevelCmd("Physics", "", " 1"); }
        void tsMap_Physics2_Click(object sender, EventArgs e) { LevelCmd("Physics", "", " 2"); }
        void tsMap_Physics3_Click(object sender, EventArgs e) { LevelCmd("Physics", "", " 3"); }
        void tsMap_Physics4_Click(object sender, EventArgs e) { LevelCmd("Physics", "", " 4"); }
        void tsMap_Physics5_Click(object sender, EventArgs e) { LevelCmd("Physics", "", " 5"); }
        void tsMap_Save_Click(object sender, EventArgs e) { LevelCmd("Save"); }
        void tsMap_Unload_Click(object sender, EventArgs e) { LevelCmd("Unload"); }
        void tsMap_Reload_Click(object sender, EventArgs e) { LevelCmd("Reload", "all ", ""); }
        
        
        
        List<string> inputLog = new List<string>(21);
        int inputIndex = -1;
        
        void main_TxtInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Up) {
                inputIndex = Math.Min(inputIndex + 1, inputLog.Count - 1);
                if (inputIndex > -1) SetInputText();
            } else if (e.KeyCode == Keys.Down) {
                inputIndex = Math.Max(inputIndex - 1, -1);
                if (inputIndex > -1) SetInputText();
            } else if (e.KeyCode == Keys.Enter) {
                InputText();
            } else {
                inputIndex = -1; return;
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        
        void SetInputText() {
            if (inputIndex == -1) return;
            main_txtInput.Text = inputLog[inputIndex];
            main_txtInput.SelectionLength = 0;
            main_txtInput.SelectionStart = main_txtInput.Text.Length;
        }
        
        void InputText() {
            string text = main_txtInput.Text;
            if (text.Length == 0) return;
            
            inputLog.Insert(0, text);
            if (inputLog.Count > 20) 
                inputLog.RemoveAt(20);
            
            if (text == "/") {
                UIHelpers.RepeatCommand();
            } else if (text[0] == '/' && text.Length > 1 && text[1] == '/') {
                UIHelpers.HandleChat(text.Substring(1));
            } else if (text[0] == '/') {
                UIHelpers.HandleCommand(text.Substring(1));
            } else {
                UIHelpers.HandleChat(text);
            }
            main_txtInput.Clear();
        }
        
        void main_BtnRestart_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to restart?", "Restart", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                Server.Stop(true);
            }
        }
        
        void main_TxtUrl_DoubleClick(object sender, EventArgs e) {
            main_txtUrl.SelectAll();
        }
        
        void main_BtnSaveAll_Click(object sender, EventArgs e) {
            UIHelpers.HandleCommand("Save all");
        }

        void main_BtnKillPhysics_Click(object sender, EventArgs e) {
            UIHelpers.HandleCommand("Physics kill");
        }

        void main_BtnUnloadEmpty_Click(object sender, EventArgs e) {
            UIHelpers.HandleCommand("Unload empty");
        }
        

        
        void tsLog_Night_Click(object sender, EventArgs e) {
            main_txtLog.NightMode = tsLog_night.Checked;
            tsLog_night.Checked = !tsLog_night.Checked;
        }

        void tsLog_Colored_Click(object sender, EventArgs e) {
            main_txtLog.Colorize = !tsLog_Colored.Checked;
            tsLog_Colored.Checked = !tsLog_Colored.Checked;
        }

        void tsLog_DateStamp_Click(object sender, EventArgs e) {
            main_txtLog.DateStamp = !tsLog_dateStamp.Checked;
            tsLog_dateStamp.Checked = !tsLog_dateStamp.Checked;
        }

        void tsLog_AutoScroll_Click(object sender, EventArgs e) {
            main_txtLog.AutoScroll = !tsLog_autoScroll.Checked;
            tsLog_autoScroll.Checked = !tsLog_autoScroll.Checked;
        }

        void tsLog_CopySelected_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(main_txtLog.SelectedText)) return;
            Clipboard.SetText(main_txtLog.SelectedText, TextDataFormat.Text);
        }
        
        void tsLog_CopyAll_Click(object sender, EventArgs e) {
            Clipboard.SetText(main_txtLog.Text, TextDataFormat.Text);
        }
        
        void tsLog_Clear_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to clear logs?", "You sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                main_txtLog.ClearLog();
            }
        }
    }
}
