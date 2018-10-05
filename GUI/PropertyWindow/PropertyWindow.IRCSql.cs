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
            irc_chkEnabled.Checked = Server.Config.UseIRC;
            irc_txtServer.Text = Server.Config.IRCServer;
            irc_txtPort.Text = Server.Config.IRCPort.ToString();
            irc_txtNick.Text = Server.Config.IRCNick;
            irc_txtChannel.Text = Server.Config.IRCChannels;
            irc_txtOpChannel.Text = Server.Config.IRCOpChannels;
            irc_chkPass.Checked = Server.Config.IRCIdentify;
            irc_txtPass.Text = Server.Config.IRCPassword;
            
            irc_cbTitles.Checked = Server.Config.IRCShowPlayerTitles;
            irc_cbWorldChanges.Checked = Server.Config.IRCShowWorldChanges;
            irc_cbAFK.Checked = Server.Config.IRCShowAFK;
            ToggleIrcSettings(Server.Config.UseIRC);            

            irc_cbRank.Items.AddRange(GuiPerms.RankNames);            
            GuiPerms.SetDefaultIndex(irc_cbRank, Server.Config.IRCControllerRank);
            irc_cbVerify.Items.AddRange(Enum.GetNames(typeof(IRCControllerVerify)));
            irc_cbVerify.SelectedIndex = (int)Server.Config.IRCVerify;
            irc_txtPrefix.Text = Server.Config.IRCCommandPrefix;
            
            sql_chkUseSQL.Checked = Server.Config.UseMySQL;
            sql_txtUser.Text = Server.Config.MySQLUsername;
            sql_txtPass.Text = Server.Config.MySQLPassword;
            sql_txtDBName.Text = Server.Config.MySQLDatabaseName;
            sql_txtHost.Text = Server.Config.MySQLHost;
            sql_txtPort.Text = Server.Config.MySQLPort;
            ToggleMySQLSettings(Server.Config.UseMySQL);
        }
        
        void ApplyIrcSqlProps() {
            Server.Config.UseIRC = irc_chkEnabled.Checked;
            Server.Config.IRCServer = irc_txtServer.Text;
            Server.Config.IRCPort = int.Parse(irc_txtPort.Text);
            Server.Config.IRCNick = irc_txtNick.Text;            
            Server.Config.IRCChannels = irc_txtChannel.Text;
            Server.Config.IRCOpChannels = irc_txtOpChannel.Text;            
            Server.Config.IRCIdentify = irc_chkPass.Checked;
            Server.Config.IRCPassword = irc_txtPass.Text;
            
            Server.Config.IRCShowPlayerTitles = irc_cbTitles.Checked;
            Server.Config.IRCShowWorldChanges = irc_cbWorldChanges.Checked;
            Server.Config.IRCShowAFK = irc_cbAFK.Checked;
            
            Server.Config.IRCControllerRank = GuiPerms.GetPermission(irc_cbRank, LevelPermission.Nobody);
            Server.Config.IRCVerify = (IRCControllerVerify)irc_cbVerify.SelectedIndex;
            Server.Config.IRCCommandPrefix = irc_txtPrefix.Text;
            
            Server.Config.UseMySQL = sql_chkUseSQL.Checked;
            Server.Config.MySQLUsername = sql_txtUser.Text;
            Server.Config.MySQLPassword = sql_txtPass.Text;
            Server.Config.MySQLDatabaseName = sql_txtDBName.Text;
            Server.Config.MySQLHost = sql_txtHost.Text;
            Server.Config.MySQLPort = sql_txtPort.Text;
            
            Database.Backend = Server.Config.UseMySQL ? MySQLBackend.Instance : SQLiteBackend.Instance;
            //Server.Config.MySQLPooling = ; // No setting for this?            
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
