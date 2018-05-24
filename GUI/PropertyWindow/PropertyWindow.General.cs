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
            srv_txtName.Text = ServerConfig.Name;
            srv_txtMOTD.Text = ServerConfig.MOTD;
            srv_numPort.Value = ServerConfig.Port;
            srv_txtOwner.Text = ServerConfig.OwnerName;
            srv_chkPublic.Checked = ServerConfig.Public;
            
            srv_numPlayers.Value = ServerConfig.MaxPlayers;
            srv_numGuests.Value = ServerConfig.MaxGuests;
            srv_numGuests.Maximum = srv_numPlayers.Value;
            srv_cbMustAgree.Checked = ServerConfig.AgreeToRulesOnEntry;
            
            lvl_txtMain.Text = ServerConfig.MainLevel;
            lvl_chkAutoload.Checked = ServerConfig.AutoLoadMaps;
            lvl_chkWorld.Checked = ServerConfig.ServerWideChat;
            
            adv_chkVerify.Checked = ServerConfig.VerifyNames;
            adv_chkRestart.Checked = ServerConfig.restartOnError;
            
            chkUpdates.Checked = ServerConfig.CheckForUpdates;
        }
        
        void ApplyGeneralProps() {
            ServerConfig.Name = srv_txtName.Text;
            ServerConfig.MOTD = srv_txtMOTD.Text;
            ServerConfig.Port = (int)srv_numPort.Value;
            ServerConfig.OwnerName = srv_txtOwner.Text;
            ServerConfig.Public = srv_chkPublic.Checked;
            
            ServerConfig.MaxPlayers = (byte)srv_numPlayers.Value;
            ServerConfig.MaxGuests = (byte)srv_numGuests.Value;
            ServerConfig.AgreeToRulesOnEntry = srv_cbMustAgree.Checked;  
            
            string main = Player.ValidName(lvl_txtMain.Text) ? lvl_txtMain.Text : "main";
            Server.SetMainLevel(main);
            ServerConfig.AutoLoadMaps = lvl_chkAutoload.Checked;
            ServerConfig.ServerWideChat = lvl_chkWorld.Checked;
            
            ServerConfig.VerifyNames = adv_chkVerify.Checked;
            ServerConfig.restartOnError = adv_chkRestart.Checked;
            
            ServerConfig.CheckForUpdates = chkUpdates.Checked;
            //ServerConfig.reportBack = ;  //No setting for this?                
        }
        
        
        
        void numPlayers_ValueChanged(object sender, EventArgs e) {
            // Ensure that number of guests is never more than number of players
            if (srv_numGuests.Value > srv_numPlayers.Value) {
                srv_numGuests.Value = srv_numPlayers.Value;
            }
            srv_numGuests.Maximum = srv_numPlayers.Value;
        }
        
        void ChkPort_Click(object sender, EventArgs e) {
            using (PortTools form = new PortTools()) {
                form.ShowDialog();
            }
        }

        void forceUpdateBtn_Click(object sender, EventArgs e) {
            srv_btnForceUpdate.Enabled = false;
            string msg = "Would you like to force update " + Server.SoftwareName + " now?";
            
            if (Popup.YesNo(msg, "Force update")) {
                SaveChanges();
                Updater.PerformUpdate();
                Dispose();
            } else {
                srv_btnForceUpdate.Enabled = true;
            }
        }
    }
}
