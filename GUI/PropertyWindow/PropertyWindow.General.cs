/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.Windows.Forms;
using MCGalaxy.Commands;
using MCGalaxy.SQL;
using MCGalaxy.Gui.Popups;

namespace MCGalaxy.Gui {

    public partial class PropertyWindow : Form {
        
        void LoadGeneralProps() {
            srv_txtName.Text = Server.name;
            srv_txtMOTD.Text = Server.motd;
            srv_txtPort.Text = Server.port.ToString();
            srv_txtOwner.Text = Server.server_owner;
            srv_chkPublic.Checked = Server.pub;
            
            numPlayers.Value = Server.players;
            numGuests.Value = Server.maxGuests;
            numGuests.Maximum = numPlayers.Value;
            chkAgreeToRules.Checked = Server.agreetorulesonentry;
            
            lvl_txtMain.Text = Server.level;
            lvl_chkAutoload.Checked = Server.AutoLoad;
            lvl_chkWorld.Checked = Server.worldChat;
            
            adv_chkVerify.Checked = Server.verify;
            adv_chkRestart.Checked = Server.restartOnError;
            adv_chkLogBeat.Checked = Server.logbeat;
            
            chkUpdates.Checked = Server.checkUpdates;
            autoUpdate.Checked = Server.autoupdate;
            notifyInGameUpdate.Checked = Server.notifyPlayers;
            updateTimeNumeric.Value = Server.restartcountdown;
        }
        
        void ApplyGeneralProps() {
            Server.name = srv_txtName.Text;
            Server.motd = srv_txtMOTD.Text;
            Server.port = int.Parse(srv_txtPort.Text);
            Server.server_owner = srv_txtOwner.Text;
            Server.pub = srv_chkPublic.Checked;
            
            Server.players = (byte)numPlayers.Value;
            Server.maxGuests = (byte)numGuests.Value;
            Server.agreetorulesonentry = chkAgreeToRules.Checked;  
            
            string main = Player.ValidName(lvl_txtMain.Text) ? lvl_txtMain.Text : "main";
            Server.SetMainLevel(main);
            Server.AutoLoad = lvl_chkAutoload.Checked;
            Server.worldChat = lvl_chkWorld.Checked;
            
            Server.verify = adv_chkVerify.Checked;
            Server.restartOnError = adv_chkRestart.Checked;
            Server.logbeat = adv_chkLogBeat.Checked;
            
            Server.checkUpdates = chkUpdates.Checked;
            Server.autoupdate = autoUpdate.Checked;           
            Server.notifyPlayers = notifyInGameUpdate.Checked;
            Server.restartcountdown = (int)updateTimeNumeric.Value;
            //Server.reportBack = ;  //No setting for this?                
        }
        
        
        
        void numPlayers_ValueChanged(object sender, EventArgs e) {
            // Ensure that number of guests is never more than number of players
            if ( numGuests.Value > numPlayers.Value ) {
                numGuests.Value = numPlayers.Value;
            }
            numGuests.Maximum = numPlayers.Value;
        }
        
        void ChkPort_Click(object sender, EventArgs e) {
            using (PortTools form = new PortTools()) {
                form.ShowDialog();
            }
        }
		
		void txtPort_TextChanged(object sender, EventArgs e) { OnlyAddDigit(srv_txtPort); }

        void forceUpdateBtn_Click(object sender, EventArgs e) {
            forceUpdateBtn.Enabled = false;
            DialogResult result = MessageBox.Show("Would you like to force update " + Server.SoftwareName + " now?", "Force Update",
                                                  MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK) {
                SaveChanges();
                Updater.PerformUpdate();
                Dispose();
            } else {
                forceUpdateBtn.Enabled = true;
            }
        }
    }
}
