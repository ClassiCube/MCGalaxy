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

namespace MCGalaxy.Gui {
    
    // This part of the file handles loading/saving config variables to the gui
    public partial class PropertyWindow : Form {
        
        void LoadGeneralProps() {
            srv_txtName.Text = Server.name;
            srv_txtMOTD.Text = Server.motd;
            srv_txtPort.Text = Server.port.ToString();
            srv_txtOwner.Text = Server.server_owner;
            srv_chkPublic.Checked = Server.pub;
            
            numPlayers.Value = Server.players;
            numGuests.Value = Server.maxGuests;
            numGuests.Maximum = numPlayers.Value;
            chkAgreeToRules.Checked = Server.agreetorulesonentry;
            
            lvl_txtMain.Text = Server.level;
            lvl_chkAutoload.Checked = Server.AutoLoad;
            lvl_chkWorld.Checked = Server.worldChat;
            
            adv_chkVerify.Checked = Server.verify;
            adv_chkRestart.Checked = Server.restartOnError;
            adv_chkLogBeat.Checked = Server.logbeat;
            
            chkUpdates.Checked = Server.checkUpdates;
            autoUpdate.Checked = Server.autoupdate;
            notifyInGameUpdate.Checked = Server.notifyPlayers;
            updateTimeNumeric.Value = Server.restartcountdown;
        }
    	
    	void ApplyGeneralProps() {
            Server.name = srv_txtName.Text;
            Server.motd = srv_txtMOTD.Text;
            Server.port = int.Parse(srv_txtPort.Text);
            Server.server_owner = srv_txtOwner.Text;
            Server.pub = srv_chkPublic.Checked;
            
            Server.players = (byte)numPlayers.Value;
            Server.maxGuests = (byte)numGuests.Value;
            Server.agreetorulesonentry = chkAgreeToRules.Checked;  
            
            string main = Player.ValidName(lvl_txtMain.Text) ? lvl_txtMain.Text : "main";
            Server.SetMainLevel(main);
            Server.AutoLoad = lvl_chkAutoload.Checked;
            Server.worldChat = lvl_chkWorld.Checked;
            
            Server.verify = adv_chkVerify.Checked;
            Server.restartOnError = adv_chkRestart.Checked;
            Server.logbeat = adv_chkLogBeat.Checked;
            
            Server.checkUpdates = chkUpdates.Checked;
            Server.autoupdate = autoUpdate.Checked;           
            Server.notifyPlayers = notifyInGameUpdate.Checked;
            Server.restartcountdown = (int)updateTimeNumeric.Value;
            //Server.reportBack = ;  //No setting for this?                
        }
        
        void LoadChatProps() {
            ParseColor(Server.DefaultColor, chat_cmbDefault);
            ParseColor(Server.IRCColour, chat_cmbIRC);
            ParseColor(Server.HelpSyntaxColor, chat_cmbSyntax);
            ParseColor(Server.HelpDescriptionColor, chat_cmbDesc);
            
            chat_txtConsole.Text = Server.ZallState;
            chat_cbTabRank.Checked = Server.TablistRankSorted;
            chat_cbTabLevel.Checked = !Server.TablistGlobal;
            chat_cbTabBots.Checked = Server.TablistBots;
            
            chat_txtShutdown.Text = Server.shutdownMessage;
            chat_chkCheap.Checked = Server.cheapMessage;
            chat_txtCheap.Text = Server.cheapMessageGiven;
            chat_txtBan.Text = Server.defaultBanMessage;
            chat_txtPromote.Text = Server.defaultPromoteMessage;
            chat_txtDemote.Text = Server.defaultDemoteMessage;
        }
        
        void ApplyChatProps() {
            Server.DefaultColor = Colors.Parse(chat_cmbDefault.SelectedItem.ToString());
            Server.IRCColour = Colors.Parse(chat_cmbIRC.SelectedItem.ToString());
            Server.HelpSyntaxColor = Colors.Parse(chat_cmbSyntax.SelectedItem.ToString());
            Server.HelpDescriptionColor = Colors.Parse(chat_cmbDesc.SelectedItem.ToString());
            
            Server.ZallState = chat_txtConsole.Text;
            Server.TablistRankSorted = chat_cbTabRank.Checked;
            Server.TablistGlobal = !chat_cbTabLevel.Checked;
            Server.TablistBots = chat_cbTabBots.Checked;
            
            Server.shutdownMessage = chat_txtShutdown.Text;
            Server.cheapMessage = chat_chkCheap.Checked;
            Server.cheapMessageGiven = chat_txtCheap.Text;
            Server.defaultBanMessage = chat_txtBan.Text;
            Server.defaultPromoteMessage = chat_txtPromote.Text;
            Server.defaultDemoteMessage = chat_txtDemote.Text;
        }
        
        
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
        
        
        void LoadMiscProps() {
            bak_txtTime.Text = Server.backupInterval.ToString();
            bak_txtLocation.Text = Server.backupLocation;
            hackrank_kick.Checked = Server.hackrank_kick;
            hackrank_kick_time.Text = Server.hackrank_kick_time.ToString();
            
            afk_txtTimer.Text = Server.afkminutes.ToString();
            afk_txtKickTime.Text = Server.afkkick.ToString();
            
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
            Server.afkkickperm = Program.GetPermission(afk_cmbKickPerm, LevelPermission.AdvBuilder);
            
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
        
        void LoadRankProps() {
            string defRank = Server.defaultRank.ToLower();
            if (rank_cmbDefault.Items.IndexOf(defRank) != -1 )
                rank_cmbDefault.SelectedIndex = rank_cmbDefault.Items.IndexOf(defRank);
            rank_chkTpToHigherRanks.Checked = Server.higherranktp;
            chkAdminsJoinSilent.Checked = Server.adminsjoinsilent;
        }
        
        void ApplyRankProps() {
            Server.defaultRank = rank_cmbDefault.SelectedItem.ToString();
            Server.higherranktp = rank_chkTpToHigherRanks.Checked;
            Server.adminsjoinsilent = chkAdminsJoinSilent.Checked;
            
            Server.osPerbuildDefault = Program.GetPermission(rank_cmbOsMap, LevelPermission.Nobody);
            var perms = CommandExtraPerms.Find("opchat");
            perms.MinRank = Program.GetPermission(rank_cmbOpChat, LevelPermission.Operator);
            perms = CommandExtraPerms.Find("adminchat");
            perms.MinRank = Program.GetPermission(rank_cmbAdminChat, LevelPermission.Admin);
        }
        
        
        void LoadSecurityProps() {
            sec_cbLogNotes.Checked = Server.LogNotes;            
            sec_cbVerifyAdmins.Checked = Server.verifyadmins;
            sec_cbWhitelist.Checked = Server.useWhitelist;
            
            sec_cbChatAuto.Checked = Server.checkspam;
            sec_numChatMsgs.Value = Server.spamcounter;
            sec_numChatSecs.Value = Server.spamcountreset;
            sec_numChatMute.Value = Server.mutespamtime;
            
            sec_cbCmdAuto.Checked = Server.CmdSpamCheck;
            sec_numCmdMsgs.Value = Server.CmdSpamCount;
            sec_numCmdSecs.Value = Server.CmdSpamInterval;
            sec_numCmdMute.Value = Server.CmdSpamBlockTime;
            
            sec_cbBlocksAuto.Checked = Server.BlockSpamCheck;
            sec_numBlocksMsgs.Value = Server.BlockSpamCount;
            sec_numBlocksSecs.Value = Server.BlockSpamInterval;
            
            sec_cbIPAuto.Checked = Server.IPSpamCheck;
            sec_numIPMsgs.Value = Server.IPSpamCount;
            sec_numIPSecs.Value = Server.IPSpamInterval;
            sec_numIPMute.Value = Server.IPSpamBlockTime;
        }

        void ApplySecurityProps() {
            Server.LogNotes = sec_cbLogNotes.Checked;
            Server.verifyadmins = sec_cbVerifyAdmins.Checked;
            Server.verifyadminsrank = Program.GetPermission(sec_cmbVerifyRank, LevelPermission.Operator);
            Server.useWhitelist = sec_cbWhitelist.Checked;
            if (Server.useWhitelist && Server.whiteList == null)
                Server.whiteList = PlayerList.Load("whitelist.txt");
            
            Server.checkspam = sec_cbChatAuto.Checked;
            Server.spamcounter = (int)sec_numChatMsgs.Value;
            Server.spamcountreset = (int)sec_numChatSecs.Value;
            Server.mutespamtime = (int)sec_numChatMute.Value;
            
            Server.CmdSpamCheck = sec_cbCmdAuto.Checked;
            Server.CmdSpamCount = (int)sec_numCmdMsgs.Value;
            Server.CmdSpamInterval = (int)sec_numCmdSecs.Value;
            Server.CmdSpamBlockTime = (int)sec_numCmdMute.Value;
            
            Server.BlockSpamCheck = sec_cbBlocksAuto.Checked;
            Server.BlockSpamCount = (int)sec_numBlocksMsgs.Value;
            Server.BlockSpamInterval = (int)sec_numBlocksSecs.Value;
            
            Server.IPSpamCheck = sec_cbIPAuto.Checked;
            Server.IPSpamCount = (int)sec_numIPMsgs.Value;
            Server.IPSpamInterval = (int)sec_numIPSecs.Value;
            Server.IPSpamBlockTime = (int)sec_numIPMute.Value;
        }
    }
}
