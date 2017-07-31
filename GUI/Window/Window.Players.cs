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
using System.Windows.Forms;

namespace MCGalaxy.Gui {
    public partial class Window : Form {
        PlayerProperties playerProps;
         
        void UpdatePlayers() {
            RunOnUiThread(
                delegate {
                    pl_listBox.Items.Clear();
                    UpdateNotifyIconText();
                    
                    Player[] players = PlayerInfo.Online.Items;
                    foreach (Player p in players)
                        pl_listBox.Items.Add(p.name);
                    
                    if (curPlayer == null) return;
                    if (PlayerInfo.FindExact(curPlayer.name) != null) return;
                    
                    curPlayer = null;
                    playerProps = null;
                    pl_gbProps.Text = "Properties for (none selected)";
                    pl_pgProps.SelectedObject = null;
                });
        }
        
        void AppendPlayerStatus(string text) {
            if (InvokeRequired) {
                Action<string> d = AppendPlayerStatus;
                Invoke(d, new object[] { text, true });
            } else {
                pl_statusBox.AppendText(text + Environment.NewLine);
            }
        }
        
        void LoadPlayerTabDetails(object sender, EventArgs e) {
            Player p = PlayerInfo.FindExact(pl_listBox.Text);
            if (p == null || p == curPlayer) return;
            
            pl_statusBox.Text = "";
            AppendPlayerStatus("==" + p.name + "==");
            playerProps = new PlayerProperties(p);
            pl_gbProps.Text = "Properties for " + p.name;
            pl_pgProps.SelectedObject = playerProps;
            curPlayer = p;
            
            try {
                UpdatePlayerMapCombo();
            } catch { }
        }

        void UpdatePlayerMapCombo() {          
            if (tabs.SelectedTab != tp_Players) return;
            pl_pgProps.Refresh();
        }

        void pl_BtnUndo_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No player selected"); return; }
            if (pl_txtUndo.Text.Trim().Length == 0)  {
                AppendPlayerStatus("You didn't specify a time"); return;
            }

            try {
                Command.all.FindByName("UndoPlayer").Use(null, curPlayer.name + " " + pl_txtUndo.Text);
                AppendPlayerStatus("Undid player for " + pl_txtUndo.Text + " seconds");
            } catch {
                AppendPlayerStatus("Something went wrong!!");
            }
        }

        void pl_BtnMessage_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No player selected"); return; }
            Player.Message(curPlayer, "<CONSOLE> " + pl_txtMessage.Text);
            AppendPlayerStatus("Sent player message '<CONSOLE> " + pl_txtMessage.Text + "'");
            pl_txtMessage.Text = "";
        }

        void pl_BtnSendCommand_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No player selected"); return; }
            
            try {
                string[] args = pl_txtImpersonate.Text.Trim().SplitSpaces(2);
                args[0] = args[0].Replace("/", "");
                Command cmd = Command.all.Find(args[0]);
                if (cmd == null) {
                    AppendPlayerStatus("There is no command '" + args[0] + "'"); return;
                }
                
                cmd.Use(curPlayer, args.Length > 1 ? args[1] : "");
                if (args.Length > 1) {
                    AppendPlayerStatus("Used command '" + args[0] + "' with parameters '" + args[1] + "' as player");
                } else {
                    AppendPlayerStatus("Used command '" + args[0] + "' with no parameters as player");
                }
                pl_txtImpersonate.Text = "";
            } catch {
                AppendPlayerStatus("Something went wrong");
            }
        }

        void pl_BtnSlap_Click(object sender, EventArgs e) { DoCmd("slap", "Slapped"); }
        void pl_BtnKill_Click(object sender, EventArgs e) { DoCmd("kill", "Killed"); }
        void pl_BtnWarn_Click(object sender, EventArgs e) { DoCmd("warn", "Warned"); }
        void pl_BtnKick_Click(object sender, EventArgs e) { DoCmd("kick", "Kicked"); }
        void pl_BtnBan_Click(object sender, EventArgs e) { DoCmd("ban", "Banned"); }
        void pl_BtnIPBan_Click(object sender, EventArgs e) { DoCmd("banip", "IP-Banned"); }
        
        void DoCmd(string cmdName, string action) {
            if (curPlayer == null) { AppendPlayerStatus("No player selected"); return; }
            Command.all.Find(cmdName).Use(null, curPlayer.name);
            AppendPlayerStatus(action + " player");
        }

        void pl_BtnRules_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No Player Selected"); return; }
            Command.all.FindByName("Rules").Use(curPlayer, "");
            AppendPlayerStatus("Sent rules to player");
        }

        void pl_BtnSpawn_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No Player Selected"); return; }
            Command.all.FindByName("Spawn").Use(curPlayer, "");
            AppendPlayerStatus("Sent player to spawn");
        }

        void pl_listBox_Click(object sender, EventArgs e) {
            LoadPlayerTabDetails(sender, e);
        }

        void pl_txtImpersonate_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) pl_BtnSendCommand_Click(sender, e);
        }
        void pl_txtUndo_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) pl_BtnUndo_Click(sender, e);
        }
        void pl_txtMessage_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) pl_BtnMessage_Click(sender, e);
        }
    }
}
