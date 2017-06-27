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
            srv_txtName.Text = ServerConfig.name;
            srv_txtMOTD.Text = ServerConfig.motd;
            srv_txtPort.Text = ServerConfig.port.ToString();
            srv_txtOwner.Text = ServerConfig.server_owner;
            srv_chkPublic.Checked = ServerConfig.pub;
            
            srv_numPlayers.Value = ServerConfig.players;
            srv_numGuests.Value = ServerConfig.maxGuests;
            srv_numGuests.Maximum = srv_numPlayers.Value;
            srv_cbMustAgree.Checked = ServerConfig.agreetorulesonentry;
            
            lvl_txtMain.Text = ServerConfig.level;
            lvl_chkAutoload.Checked = ServerConfig.AutoLoad;
            lvl_chkWorld.Checked = ServerConfig.worldChat;
            
            adv_chkVerify.Checked = ServerConfig.verify;
            adv_chkRestart.Checked = ServerConfig.restartOnError;
            adv_chkLogBeat.Checked = ServerConfig.logbeat;
            
            chkUpdates.Checked = ServerConfig.checkUpdates;
            autoUpdate.Checked = ServerConfig.autoupdate;
            notifyInGameUpdate.Checked = ServerConfig.notifyPlayers;
            updateTimeNumeric.Value = ServerConfig.restartcountdown;
        }
        
        void ApplyGeneralProps() {
            ServerConfig.name = srv_txtName.Text;
            ServerConfig.motd = srv_txtMOTD.Text;
            ServerConfig.port = int.Parse(srv_txtPort.Text);
            ServerConfig.server_owner = srv_txtOwner.Text;
            ServerConfig.pub = srv_chkPublic.Checked;
            
            ServerConfig.players = (byte)srv_numPlayers.Value;
            ServerConfig.maxGuests = (byte)srv_numGuests.Value;
            ServerConfig.agreetorulesonentry = srv_cbMustAgree.Checked;  
            
            string main = Player.ValidName(lvl_txtMain.Text) ? lvl_txtMain.Text : "main";
            Server.SetMainLevel(main);
            ServerConfig.AutoLoad = lvl_chkAutoload.Checked;
            ServerConfig.worldChat = lvl_chkWorld.Checked;
            
            ServerConfig.verify = adv_chkVerify.Checked;
            ServerConfig.restartOnError = adv_chkRestart.Checked;
            ServerConfig.logbeat = adv_chkLogBeat.Checked;
            
            ServerConfig.checkUpdates = chkUpdates.Checked;
            ServerConfig.autoupdate = autoUpdate.Checked;           
            ServerConfig.notifyPlayers = notifyInGameUpdate.Checked;
            ServerConfig.restartcountdown = (int)updateTimeNumeric.Value;
            //ServerConfig.reportBack = ;  //No setting for this?                
        }
        
        
        
        void numPlayers_ValueChanged(object sender, EventArgs e) {
            // Ensure that number of guests is never more than number of players
            if ( srv_numGuests.Value > srv_numPlayers.Value ) {
                srv_numGuests.Value = srv_numPlayers.Value;
            }
            srv_numGuests.Maximum = srv_numPlayers.Value;
        }
        
        void ChkPort_Click(object sender, EventArgs e) {
            using (PortTools form = new PortTools()) {
                form.ShowDialog();
            }
        }
		
		void txtPort_TextChanged(object sender, EventArgs e) { OnlyAddDigit(srv_txtPort); }

        void forceUpdateBtn_Click(object sender, EventArgs e) {
            srv_btnForceUpdate.Enabled = false;
            DialogResult result = MessageBox.Show("Would you like to force update " + Server.SoftwareName + " now?", "Force Update",
                                                  MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK) {
                SaveChanges();
                Updater.PerformUpdate();
                Dispose();
            } else {
                srv_btnForceUpdate.Enabled = true;
            }
        }
    }
}
