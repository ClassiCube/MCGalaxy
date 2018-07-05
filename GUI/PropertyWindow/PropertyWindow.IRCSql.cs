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
            irc_chkEnabled.Checked = ServerConfig.UseIRC;
            irc_txtServer.Text = ServerConfig.IRCServer;
            irc_txtPort.Text = ServerConfig.IRCPort.ToString();
            irc_txtNick.Text = ServerConfig.IRCNick;
            irc_txtChannel.Text = ServerConfig.IRCChannels;
            irc_txtOpChannel.Text = ServerConfig.IRCOpChannels;
            irc_chkPass.Checked = ServerConfig.IRCIdentify;
            irc_txtPass.Text = ServerConfig.IRCPassword;
            
            irc_cbTitles.Checked = ServerConfig.IRCShowPlayerTitles;
            irc_cbWorldChanges.Checked = ServerConfig.IRCShowWorldChanges;
            irc_cbAFK.Checked = ServerConfig.IRCShowAFK;
            ToggleIrcSettings(ServerConfig.UseIRC);            

            irc_cbRank.Items.AddRange(GuiPerms.RankNames);            
            GuiPerms.SetDefaultIndex(irc_cbRank, ServerConfig.IRCControllerRank);
            irc_cbVerify.Items.AddRange(Enum.GetNames(typeof(IRCControllerVerify)));
            irc_cbVerify.SelectedIndex = (int)ServerConfig.IRCVerify;
            irc_txtPrefix.Text = ServerConfig.IRCCommandPrefix;
            
            sql_chkUseSQL.Checked = ServerConfig.UseMySQL;
            sql_txtUser.Text = ServerConfig.MySQLUsername;
            sql_txtPass.Text = ServerConfig.MySQLPassword;
            sql_txtDBName.Text = ServerConfig.MySQLDatabaseName;
            sql_txtHost.Text = ServerConfig.MySQLHost;
            sql_txtPort.Text = ServerConfig.MySQLPort;
            ToggleMySQLSettings(ServerConfig.UseMySQL);
        }
        
        void ApplyIrcSqlProps() {
            ServerConfig.UseIRC = irc_chkEnabled.Checked;
            ServerConfig.IRCServer = irc_txtServer.Text;
            ServerConfig.IRCPort = int.Parse(irc_txtPort.Text);
            ServerConfig.IRCNick = irc_txtNick.Text;            
            ServerConfig.IRCChannels = irc_txtChannel.Text;
            ServerConfig.IRCOpChannels = irc_txtOpChannel.Text;            
            ServerConfig.IRCIdentify = irc_chkPass.Checked;
            ServerConfig.IRCPassword = irc_txtPass.Text;
            
            ServerConfig.IRCShowPlayerTitles = irc_cbTitles.Checked;
            ServerConfig.IRCShowWorldChanges = irc_cbWorldChanges.Checked;
            ServerConfig.IRCShowAFK = irc_cbAFK.Checked;
            
            ServerConfig.IRCControllerRank = GuiPerms.GetPermission(irc_cbRank, LevelPermission.Nobody);
            ServerConfig.IRCVerify = (IRCControllerVerify)irc_cbVerify.SelectedIndex;
            ServerConfig.IRCCommandPrefix = irc_txtPrefix.Text;
            
            ServerConfig.UseMySQL = sql_chkUseSQL.Checked;
            ServerConfig.MySQLUsername = sql_txtUser.Text;
            ServerConfig.MySQLPassword = sql_txtPass.Text;
            ServerConfig.MySQLDatabaseName = sql_txtDBName.Text;
            ServerConfig.MySQLHost = sql_txtHost.Text;
            ServerConfig.MySQLPort = sql_txtPort.Text;
            
            Database.Backend = ServerConfig.UseMySQL ? MySQLBackend.Instance : SQLiteBackend.Instance;
            //ServerConfig.MySQLPooling = ; // No setting for this?            
        }
        
        
                
        void ToggleIrcSettings(bool enabled) {
            irc_txtServer.Enabled = enabled; irc_lblServer.Enabled = enabled;
            irc_txtPort.Enabled = enabled; irc_lblPort.Enabled = enabled;
            irc_txtNick.Enabled = enabled; irc_lblNick.Enabled = enabled;
            irc_txtChannel.Enabled = enabled; irc_lblChannel.Enabled = enabled;
            irc_txtOpChannel.Enabled = enabled; irc_lblOpChannel.Enabled = enabled;    
            irc_chkPass.Enabled = enabled; irc_txtPass.Enabled = enabled && irc_chkPass.Checked;
            
            irc_cbTitles.Enabled = enabled;
            irc_cbWorldChanges.Enabled = enabled;
            irc_cbAFK.Enabled = enabled;           
            irc_lblRank.Enabled = enabled; irc_cbRank.Enabled = enabled;
            irc_lblVerify.Enabled = enabled; irc_cbVerify.Enabled = enabled;
            irc_lblPrefix.Enabled = enabled; irc_txtPrefix.Enabled = enabled;
        }

        void ToggleMySQLSettings(bool enabled) {
            sql_txtUser.Enabled = enabled; sql_lblUser.Enabled = enabled;
            sql_txtPass.Enabled = enabled; sql_lblPass.Enabled = enabled;
            sql_txtPort.Enabled = enabled; sql_lblPort.Enabled = enabled;
            sql_txtHost.Enabled = enabled; sql_lblHost.Enabled = enabled;
            sql_txtDBName.Enabled = enabled; sql_lblDBName.Enabled = enabled;
        }
        
        
        
        void irc_chkEnabled_CheckedChanged(object sender, EventArgs e) {
            ToggleIrcSettings(irc_chkEnabled.Checked);
        }        
        
        void irc_chkPass_CheckedChanged(object sender, EventArgs e) {
            irc_txtPass.Enabled = irc_chkEnabled.Checked && irc_chkPass.Checked;
        }

        void sql_linkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start("http://dev.mysql.com/downloads/");
            } catch {
                Popup.Error("Failed to open link!");
            }
        }

        void sql_chkUseSQL_CheckedChanged(object sender, EventArgs e) {
            ToggleMySQLSettings(sql_chkUseSQL.Checked);
        }
    }
}
