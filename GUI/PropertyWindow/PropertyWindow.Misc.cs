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

namespace MCGalaxy.Gui {

    public partial class PropertyWindow : Form {  
        
        void LoadMiscProps() {
            bak_txtTime.Text = Server.backupInterval.ToString();
            bak_txtLocation.Text = Server.backupLocation;
            hackrank_kick.Checked = Server.hackrank_kick;
            hackrank_kick_time.Text = Server.hackrank_kick_time.ToString();
            
            afk_txtTimer.Text = Server.afkminutes.ToString();
            afk_txtKickTime.Text = Server.afkkick.ToString();
            GuiPerms.SetDefaultIndex(afk_cmbKickPerm, Server.afkkickperm);
            
            chkPhysicsRest.Checked = Server.physicsRestart;
            txtRP.Text = Server.rpLimit.ToString();
            txtNormRp.Text = Server.rpNormLimit.ToString();
            
            chkDeath.Checked = Server.deathcount;
            chkSmile.Checked = Server.parseSmiley;
            chkShowEmptyRanks.Checked = Server.showEmptyRanks;
            chk17Dollar.Checked = Server.dollarNames;
            chkRepeatMessages.Checked = Server.repeatMessage;
            chkRestartTime.Checked = Server.autorestart;
            txtRestartTime.Text = Server.restarttime.ToString();
            chkGuestLimitNotify.Checked = Server.guestLimitNotify;
            txtMoneys.Text = Server.moneys;
            nudCooldownTime.Value = Server.reviewcooldown;
            chkProfanityFilter.Checked = Server.profanityFilter; // TODO: not visible?
        }
        
        void ApplyMiscProps() {
            Server.backupInterval = int.Parse(bak_txtTime.Text);
            Server.backupLocation = bak_txtLocation.Text;
            Server.hackrank_kick = hackrank_kick.Checked;
            Server.hackrank_kick_time = int.Parse(hackrank_kick_time.Text);
            
            Server.afkminutes = int.Parse(afk_txtTimer.Text);
            Server.afkkick = int.Parse(afk_txtKickTime.Text);
            Server.afkkickperm = GuiPerms.GetPermission(afk_cmbKickPerm, LevelPermission.AdvBuilder);
            
            Server.physicsRestart = chkPhysicsRest.Checked;
            Server.rpLimit = int.Parse(txtRP.Text);
            Server.rpNormLimit = int.Parse(txtNormRp.Text);
            
            Server.deathcount = chkDeath.Checked;
            Server.parseSmiley = chkSmile.Checked;
            Server.showEmptyRanks = chkShowEmptyRanks.Checked;
            Server.dollarNames = chk17Dollar.Checked;
            Server.repeatMessage = chkRepeatMessages.Checked;
            Server.autorestart = chkRestartTime.Checked;
            try { Server.restarttime = DateTime.Parse(txtRestartTime.Text); }
            catch { } // ignore bad values
            Server.guestLimitNotify = chkGuestLimitNotify.Checked;
            Server.moneys = txtMoneys.Text;
            Server.reviewcooldown = (int)nudCooldownTime.Value;
            Server.profanityFilter = chkProfanityFilter.Checked;
        }
        
                
        
        void buttonEco_Click(object sender, EventArgs e) {
            new Gui.Eco.EconomyWindow().ShowDialog();
        }
        
        void adv_btnEditTexts_Click(object sender, EventArgs e) {
            new EditText().Show();
        }
        
        void txtBackup_TextChanged(object sender, EventArgs e) { OnlyAddDigit(bak_txtTime); }
    }
}
