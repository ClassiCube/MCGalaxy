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
        bool warnDisabledVerification = true;
        
        void LoadGeneralProps() {
            srv_txtName.Text = Server.Config.Name;
            srv_txtMOTD.Text = Server.Config.MOTD;
            srv_numPort.Value = Server.Config.Port;
            srv_txtOwner.Text = Server.Config.OwnerName;
            srv_chkPublic.Checked = Server.Config.Public;
            
            srv_numPlayers.Value = Server.Config.MaxPlayers;
            srv_numGuests.Value = Server.Config.MaxGuests;
            srv_numGuests.Maximum = srv_numPlayers.Value;
            srv_cbMustAgree.Checked = Server.Config.AgreeToRulesOnEntry;
            
            lvl_txtMain.Text = Server.Config.MainLevel;
            lvl_chkAutoload.Checked = Server.Config.AutoLoadMaps;
            lvl_chkWorld.Checked = Server.Config.ServerWideChat;
            
            warnDisabledVerification = false;
            adv_chkVerify.Checked    = Server.Config.VerifyNames;
            warnDisabledVerification = true;
            adv_chkCPE.Checked = Server.Config.EnableCPE;       
            chkUpdates.Checked = Server.Config.CheckForUpdates;
        }
        
        void ApplyGeneralProps() {
            Server.Config.Name = srv_txtName.Text;
            Server.Config.MOTD = srv_txtMOTD.Text;
            Server.Config.Port = (int)srv_numPort.Value;
            Server.Config.OwnerName = srv_txtOwner.Text;
            Server.Config.Public = srv_chkPublic.Checked;
            
            Server.Config.MaxPlayers = (byte)srv_numPlayers.Value;
            Server.Config.MaxGuests = (byte)srv_numGuests.Value;
            Server.Config.AgreeToRulesOnEntry = srv_cbMustAgree.Checked;  
            
            Server.Config.MainLevel = lvl_txtMain.Text;
            Server.Config.AutoLoadMaps = lvl_chkAutoload.Checked;
            Server.Config.ServerWideChat = lvl_chkWorld.Checked;
            
            Server.Config.VerifyNames = adv_chkVerify.Checked;
            Server.Config.EnableCPE = adv_chkCPE.Checked;            
            Server.Config.CheckForUpdates = chkUpdates.Checked;
            //Server.Config.reportBack = ;  //No setting for this?                
        }        
        
        
        const string warnMsg = "Disabling name verification means players\ncan login as anyone, including YOU\n\n" +
            "Are you sure you want to disable name verification?";
        void chkVerify_CheckedChanged(object sender, EventArgs e) {
            if (!warnDisabledVerification || adv_chkVerify.Checked) return;            
            if (Popup.OKCancel(warnMsg, "Security warning")) return;
            adv_chkVerify.Checked = true;
        }
        
        void numPlayers_ValueChanged(object sender, EventArgs e) {
            // Ensure that number of guests is never more than number of players
            if (srv_numGuests.Value > srv_numPlayers.Value) {
                srv_numGuests.Value = srv_numPlayers.Value;
            }
            srv_numGuests.Maximum = srv_numPlayers.Value;
        }
        
        void ChkPort_Click(object sender, EventArgs e) {
            int port = (int)srv_numPort.Value;
            using (PortTools form = new PortTools(port)) {
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
