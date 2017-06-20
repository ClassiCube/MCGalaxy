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
using MCGalaxy.SQL;

namespace MCGalaxy.Gui {

    public partial class PropertyWindow : Form { 
        
        void LoadIrcSqlProps() {
            irc_chkEnabled.Checked = Server.irc;
            irc_txtServer.Text = Server.ircServer;
            irc_txtPort.Text = Server.ircPort.ToString();
            irc_txtNick.Text = Server.ircNick;
            irc_txtChannel.Text = Server.ircChannel;
            irc_txtOpChannel.Text = Server.ircOpChannel;
            irc_chkPass.Checked = Server.ircIdentify;
            irc_txtPass.Text = Server.ircPassword;
            irc_cbTitles.Checked = Server.ircPlayerTitles;
            
            sql_chkUseSQL.Checked = Server.useMySQL;
            sql_txtUser.Text = Server.MySQLUsername;
            sql_txtPass.Text = Server.MySQLPassword;
            sql_txtDBName.Text = Server.MySQLDatabaseName;
            sql_txtHost.Text = Server.MySQLHost;
            sql_txtPort.Text = Server.MySQLPort;
        }
        
        void ApplyIrcSqlProps() {
            Server.irc = irc_chkEnabled.Checked;
            Server.ircServer = irc_txtServer.Text;
            Server.ircPort = int.Parse(irc_txtPort.Text);
            Server.ircNick = irc_txtNick.Text;            
            Server.ircChannel = irc_txtChannel.Text;
            Server.ircOpChannel = irc_txtOpChannel.Text;            
            Server.ircIdentify = irc_chkPass.Checked;
            Server.ircPassword = irc_txtPass.Text;
            Server.ircPlayerTitles = irc_cbTitles.Checked;
            
            Server.useMySQL = sql_chkUseSQL.Checked;
            Server.MySQLUsername = sql_txtUser.Text;
            Server.MySQLPassword = sql_txtPass.Text;
            Server.MySQLDatabaseName = sql_txtDBName.Text;
            Server.MySQLHost = sql_txtHost.Text;
            Server.MySQLPort = sql_txtPort.Text;
            
            Database.Backend = Server.useMySQL ? MySQLBackend.Instance : SQLiteBackend.Instance;
            //Server.MySQLPooling = ; // No setting for this?            
        }
		
		
		        
        void ToggleIrcSettings(bool enabled) {
            irc_txtServer.Enabled = enabled;
            irc_txtPort.Enabled = enabled;
            irc_txtNick.Enabled = enabled;
            irc_txtChannel.Enabled = enabled;
            irc_txtOpChannel.Enabled = enabled;
            irc_txtPass.Enabled = enabled;
            irc_chkPass.Enabled = enabled;
            irc_cbTitles.Enabled = enabled;
        }

        void ToggleMySQLSettings(bool enabled) {
            sql_txtUser.Enabled = enabled;
            sql_txtPass.Enabled = enabled;
            sql_txtPort.Enabled = enabled;
            sql_txtHost.Enabled = enabled;
            sql_txtDBName.Enabled = enabled;
        }
		
		
		
		void irc_ChkEnabled_CheckedChanged(object sender, EventArgs e) {
            ToggleIrcSettings(irc_chkEnabled.Checked);
        }

        void sql_linkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start("http://dev.mysql.com/downloads/");
            } catch {
                MessageBox.Show("Failed to open link!");
            }
        }

        void sql_chkUseSQL_CheckedChanged(object sender, EventArgs e) {
            ToggleMySQLSettings(sql_chkUseSQL.Checked);
        }
    }
}
