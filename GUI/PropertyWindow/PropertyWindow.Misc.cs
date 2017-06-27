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
using MCGalaxy.Gui.Popups;

namespace MCGalaxy.Gui {

    public partial class PropertyWindow : Form {  
        
        void LoadMiscProps() {
            bak_txtTime.Text = ServerConfig.backupInterval.ToString();
            bak_txtLocation.Text = ServerConfig.backupLocation;
            hackrank_kick.Checked = ServerConfig.hackrank_kick;
            hackrank_kick_time.Text = ServerConfig.hackrank_kick_time.ToString();
            
            afk_txtTimer.Text = ServerConfig.afkminutes.ToString();
            afk_txtKickTime.Text = ServerConfig.afkkick.ToString();
            GuiPerms.SetDefaultIndex(afk_cmbKickPerm, ServerConfig.afkkickperm);
            
            chkPhysicsRest.Checked = ServerConfig.physicsRestart;
            txtRP.Text = ServerConfig.rpLimit.ToString();
            txtNormRp.Text = ServerConfig.rpNormLimit.ToString();
            
            chkDeath.Checked = ServerConfig.deathcount;
            chkSmile.Checked = ServerConfig.parseSmiley;
            chk17Dollar.Checked = ServerConfig.dollarNames;
            chkRepeatMessages.Checked = ServerConfig.repeatMessage;
            chkRestartTime.Checked = ServerConfig.autorestart;
            txtRestartTime.Text = ServerConfig.restarttime.ToString();
            chkGuestLimitNotify.Checked = ServerConfig.guestLimitNotify;
            txtMoneys.Text = ServerConfig.moneys;
            nudCooldownTime.Value = ServerConfig.reviewcooldown;
            chkProfanityFilter.Checked = ServerConfig.profanityFilter; // TODO: not visible?
        }
        
        void ApplyMiscProps() {
            ServerConfig.backupInterval = int.Parse(bak_txtTime.Text);
            ServerConfig.backupLocation = bak_txtLocation.Text;
            ServerConfig.hackrank_kick = hackrank_kick.Checked;
            ServerConfig.hackrank_kick_time = int.Parse(hackrank_kick_time.Text);
            
            ServerConfig.afkminutes = int.Parse(afk_txtTimer.Text);
            ServerConfig.afkkick = int.Parse(afk_txtKickTime.Text);
            ServerConfig.afkkickperm = GuiPerms.GetPermission(afk_cmbKickPerm, LevelPermission.AdvBuilder);
            
            ServerConfig.physicsRestart = chkPhysicsRest.Checked;
            ServerConfig.rpLimit = int.Parse(txtRP.Text);
            ServerConfig.rpNormLimit = int.Parse(txtNormRp.Text);
            
            ServerConfig.deathcount = chkDeath.Checked;
            ServerConfig.parseSmiley = chkSmile.Checked;
            ServerConfig.dollarNames = chk17Dollar.Checked;
            ServerConfig.repeatMessage = chkRepeatMessages.Checked;
            ServerConfig.autorestart = chkRestartTime.Checked;
            try { ServerConfig.restarttime = DateTime.Parse(txtRestartTime.Text); }
            catch { } // ignore bad values
            ServerConfig.guestLimitNotify = chkGuestLimitNotify.Checked;
            ServerConfig.moneys = txtMoneys.Text;
            ServerConfig.reviewcooldown = (int)nudCooldownTime.Value;
            ServerConfig.profanityFilter = chkProfanityFilter.Checked;
        }
        
                
        
        void buttonEco_Click(object sender, EventArgs e) {
            new Gui.Eco.EconomyWindow().ShowDialog();
        }
        
        void adv_btnEditTexts_Click(object sender, EventArgs e) {
            using (Form form = new EditText()) {
                form.ShowDialog();
            }
        }
        
        void txtBackup_TextChanged(object sender, EventArgs e) { OnlyAddDigit(bak_txtTime); }
    }
}
