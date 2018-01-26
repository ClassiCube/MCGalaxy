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
            bak_numTime.Value = ServerConfig.BackupInterval;
            bak_txtLocation.Text = ServerConfig.BackupDirectory;
            hackrank_kick.Checked = ServerConfig.HackrankKicks;
            hackrank_kick_time.Text = ServerConfig.HackrankKickDelay.ToString();
            
            afk_txtTimer.Text = ServerConfig.AutoAfkMins.ToString();
            chkPhysicsRest.Checked = ServerConfig.PhysicsRestart;
            txtRP.Text = ServerConfig.PhysicsRestartLimit.ToString();
            txtNormRp.Text = ServerConfig.PhysicsRestartNormLimit.ToString();
            
            chkDeath.Checked = ServerConfig.AnnounceDeathCount;
            chkSmile.Checked = ServerConfig.ParseEmotes;
            chk17Dollar.Checked = ServerConfig.DollarNames;
            chkRepeatMessages.Checked = ServerConfig.RepeatMBs;
            chkGuestLimitNotify.Checked = ServerConfig.GuestLimitNotify;
            txtMoneys.Text = ServerConfig.Currency;
            nudCooldownTime.Value = ServerConfig.ReviewCooldown;
            chkProfanityFilter.Checked = ServerConfig.ProfanityFiltering; // TODO: not visible?
        }
        
        void ApplyMiscProps() {
            ServerConfig.BackupInterval = (int)bak_numTime.Value;
            ServerConfig.BackupDirectory = bak_txtLocation.Text;
            ServerConfig.HackrankKicks = hackrank_kick.Checked;
            ServerConfig.HackrankKickDelay = int.Parse(hackrank_kick_time.Text);
            
            ServerConfig.AutoAfkMins = int.Parse(afk_txtTimer.Text);            
            ServerConfig.PhysicsRestart = chkPhysicsRest.Checked;
            ServerConfig.PhysicsRestartLimit = int.Parse(txtRP.Text);
            ServerConfig.PhysicsRestartNormLimit = int.Parse(txtNormRp.Text);
            
            ServerConfig.AnnounceDeathCount = chkDeath.Checked;
            ServerConfig.ParseEmotes = chkSmile.Checked;
            ServerConfig.DollarNames = chk17Dollar.Checked;
            ServerConfig.RepeatMBs = chkRepeatMessages.Checked;
            ServerConfig.GuestLimitNotify = chkGuestLimitNotify.Checked;
            ServerConfig.Currency = txtMoneys.Text;
            ServerConfig.ReviewCooldown = (int)nudCooldownTime.Value;
            ServerConfig.ProfanityFiltering = chkProfanityFilter.Checked;
        }
        
                
        
        void buttonEco_Click(object sender, EventArgs e) {
            new Gui.Eco.EconomyWindow().ShowDialog();
        }
        
        void adv_btnEditTexts_Click(object sender, EventArgs e) {
            using (Form form = new EditText()) {
                form.ShowDialog();
            }
        }
    }
}
