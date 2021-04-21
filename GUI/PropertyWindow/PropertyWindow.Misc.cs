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
using MCGalaxy.SQL;

namespace MCGalaxy.Gui {

    public partial class PropertyWindow : Form {  
        
        void LoadMiscProps() {
            bak_numTime.Value = Server.Config.BackupInterval;
            bak_txtLocation.Text = Server.Config.BackupDirectory;
            hack_lbl.Checked = Server.Config.HackrankKicks;
            hack_num.Value = Server.Config.HackrankKickDelay;
            
            afk_numTimer.Value = Server.Config.AutoAfkTime;
            chkPhysRestart.Checked = Server.Config.PhysicsRestart;
            txtRP.Text = Server.Config.PhysicsRestartLimit.ToString();
            txtNormRp.Text = Server.Config.PhysicsRestartNormLimit.ToString();
            
            chkDeath.Checked = Server.Config.AnnounceDeathCount;
            chkSmile.Checked = Server.Config.ParseEmotes;
            chk17Dollar.Checked = Server.Config.DollarNames;
            chkRepeatMessages.Checked = Server.Config.RepeatMBs;
            chkGuestLimitNotify.Checked = Server.Config.GuestLimitNotify;
            misc_numReview.Value = Server.Config.ReviewCooldown;
            chkRestart.Checked = Server.Config.restartOnError;
        }
        
        void ApplyMiscProps() {
            Server.Config.BackupInterval = bak_numTime.Value;
            Server.Config.BackupDirectory = bak_txtLocation.Text;
            Server.Config.HackrankKicks = hack_lbl.Checked;
            Server.Config.HackrankKickDelay = hack_num.Value;
            
            Server.Config.AutoAfkTime = afk_numTimer.Value;
            Server.Config.PhysicsRestart = chkPhysRestart.Checked;
            Server.Config.PhysicsRestartLimit = int.Parse(txtRP.Text);
            Server.Config.PhysicsRestartNormLimit = int.Parse(txtNormRp.Text);
            
            Server.Config.AnnounceDeathCount = chkDeath.Checked;
            Server.Config.ParseEmotes = chkSmile.Checked;
            Server.Config.DollarNames = chk17Dollar.Checked;
            Server.Config.RepeatMBs = chkRepeatMessages.Checked;
            Server.Config.GuestLimitNotify = chkGuestLimitNotify.Checked;
            Server.Config.ReviewCooldown = misc_numReview.Value;
            Server.Config.restartOnError = chkRestart.Checked; 
        }
        
        void adv_btnEditTexts_Click(object sender, EventArgs e) {
            using (Form form = new EditText()) {
                form.ShowDialog();
            }
        }
		
		
        void LoadSqlProps() {
            sql_chkUseSQL.Checked = Server.Config.UseMySQL;
            sql_txtUser.Text = Server.Config.MySQLUsername;
            sql_txtPass.Text = Server.Config.MySQLPassword;
            sql_txtDBName.Text = Server.Config.MySQLDatabaseName;
            sql_txtHost.Text = Server.Config.MySQLHost;
            sql_txtPort.Text = Server.Config.MySQLPort;
            ToggleMySQLSettings(Server.Config.UseMySQL);
        }
        
        void ApplySqlProps() {
            Server.Config.UseMySQL = sql_chkUseSQL.Checked;
            Server.Config.MySQLUsername = sql_txtUser.Text;
            Server.Config.MySQLPassword = sql_txtPass.Text;
            Server.Config.MySQLDatabaseName = sql_txtDBName.Text;
            Server.Config.MySQLHost = sql_txtHost.Text;
            Server.Config.MySQLPort = sql_txtPort.Text;
            
            Database.Backend = Server.Config.UseMySQL ? MySQLBackend.Instance : SQLiteBackend.Instance;
            //Server.Config.MySQLPooling = ; // No setting for this?            
        }


        void ToggleMySQLSettings(bool enabled) {
            sql_txtUser.Enabled = enabled; sql_lblUser.Enabled = enabled;
            sql_txtPass.Enabled = enabled; sql_lblPass.Enabled = enabled;
            sql_txtPort.Enabled = enabled; sql_lblPort.Enabled = enabled;
            sql_txtHost.Enabled = enabled; sql_lblHost.Enabled = enabled;
            sql_txtDBName.Enabled = enabled; sql_lblDBName.Enabled = enabled;
        }

        void sql_linkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Program.OpenBrowser("https://dev.mysql.com/downloads/");
        }

        void sql_chkUseSQL_CheckedChanged(object sender, EventArgs e) {
            ToggleMySQLSettings(sql_chkUseSQL.Checked);
        }
    }
}
