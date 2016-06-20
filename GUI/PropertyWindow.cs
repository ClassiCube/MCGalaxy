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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.Games;
using MCGalaxy.Gui.Popups;
using MCGalaxy.Util;
using Microsoft.Win32;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {
        string lsLoadedMap = "";
        ZombieSettings zSettings = new ZombieSettings();

        public PropertyWindow() {
        	InitializeComponent();
        	this.propsZG.SelectedObject = zSettings;
        	this.zSettings.LoadFromServer();
        	this.propsZG.Invalidate();
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            this.Font = SystemFonts.IconTitleFont;
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) {
            if (e.Category == UserPreferenceCategory.Window) {
                this.Font = SystemFonts.IconTitleFont;
            }
        }

        private void PropertyWindow_FormClosing(object sender, FormClosingEventArgs e) {
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
        }

        private void PropertyWindow_Load(object sender, EventArgs e) {
        	List<string> colors = new List<string>() { "black", "navy", "green", "teal", "maroon", 
        		"purple", "gold", "silver", "gray", "blue", "lime", "aqua", "red", "pink", "yellow", "white" };
        	for (int i = 0; i < 256; i++) {
        		if (Colors.ExtColors[i].Undefined) continue;
        		colors.Add(Colors.ExtColors[i].Name);
        	}
        	string[] colorsArray = colors.ToArray();
            chat_cmbDefault.Items.AddRange(colorsArray);
            chat_cmbIRC.Items.AddRange(colorsArray);
            chat_cmbSyntax.Items.AddRange(colorsArray);
            chat_cmbDesc.Items.AddRange(colorsArray);
            cmbColor.Items.AddRange(colorsArray);

            toggleIrcSettings(Server.irc);
            toggleMySQLSettings(Server.useMySQL);

            string opchatperm = String.Empty;
            string adminchatperm = String.Empty;
            string verifyadminsperm = String.Empty;
            string grieferstonerank = String.Empty;
            string afkkickrank = String.Empty;
            string osmaprank = String.Empty;

            foreach ( Group grp in Group.GroupList ) {
                cmbDefaultRank.Items.Add(grp.name);
                cmbOpChat.Items.Add(grp.name);
                cmbAdminChat.Items.Add(grp.name);
                cmbVerificationRank.Items.Add(grp.name);
                lsCmbSetupRank.Items.Add(grp.name);
                lsCmbControlRank.Items.Add(grp.name);
                cmbAFKKickPerm.Items.Add(grp.name);
                cmbOsMap.Items.Add(grp.name);

                if ( grp.Permission == Server.opchatperm )
                    opchatperm = grp.name;
                if ( grp.Permission == Server.adminchatperm )
                    adminchatperm = grp.name;
                if ( grp.Permission == Server.verifyadminsrank )
                    verifyadminsperm = grp.name;
                if ( grp.Permission == Server.afkkickperm )
                    afkkickrank = grp.name;
                if( grp.Permission == Server.osPerbuildDefault )
                    osmaprank = grp.name;
            }
            
            listPasswords.Items.Clear();
            if (Directory.Exists("extra/passwords")) {
                string[] files = Directory.GetFiles("extra/passwords", "*.dat");
                listPasswords.BeginUpdate();
                foreach (string file in files) {
                    string name = Path.GetFileNameWithoutExtension(file);
                    listPasswords.Items.Add(name);
                }
                listPasswords.EndUpdate();
            }
            
            cmbDefaultRank.SelectedIndex = 1;
            cmbOpChat.SelectedIndex = ( opchatperm != String.Empty ? cmbOpChat.Items.IndexOf(opchatperm) : 1 );
            cmbAdminChat.SelectedIndex = ( adminchatperm != String.Empty ? cmbAdminChat.Items.IndexOf(adminchatperm) : 1 );
            cmbVerificationRank.SelectedIndex = ( verifyadminsperm != String.Empty ? cmbVerificationRank.Items.IndexOf(verifyadminsperm) : 1 );
            cmbAFKKickPerm.SelectedIndex = ( afkkickrank != String.Empty ? cmbAFKKickPerm.Items.IndexOf(afkkickrank) : 1 );
            cmbOsMap.SelectedIndex = ( osmaprank != String.Empty ? cmbOsMap.Items.IndexOf(osmaprank) : 1 );

            //Load server stuff
            LoadProp("properties/server.properties");
            LoadRanks();
            try {
                LoadCommands();
                LoadBlocks();
                LoadExtraCmdCmds();
            } catch (Exception ex) {
            	Server.ErrorLog(ex);
                Server.s.Log("Failed to load commands and blocks!");
            }

            try {
                LoadLavaSettings();
                UpdateLavaMapList();
                UpdateLavaControls();
            } catch (Exception ex) {
            	Server.ErrorLog(ex);
                Server.s.Log("Failed to load Lava Survival settings!");
            }

            try {
                lavaUpdateTimer = new System.Timers.Timer(10000) { AutoReset = true };
                lavaUpdateTimer.Elapsed += delegate {
                    UpdateLavaControls();
                    UpdateLavaMapList(false);
                };
                lavaUpdateTimer.Start();
            }
            catch {
                Server.s.Log("Failed to start lava control update timer!");
            }
            try { nudCooldownTime.Value = Server.reviewcooldown; }
            catch { }

            //Sigh. I wish there were SOME event to help me.
            foreach(var command in Command.all.commands) {
                if ( Command.core.commands.Contains( command ) )
                    continue;

                lstCommands.Items.Add ( command.name );
            }
        }

        public static bool EditTextOpen = false;

        private void PropertyWindow_Unload(object sender, EventArgs e) {
            lavaUpdateTimer.Dispose();
            Window.prevLoaded = false;
            TntWarsGame.GuiLoaded = null;
        }

        List<Group> storedRanks = new List<Group>();
        List<GrpCommands.rankAllowance> storedCommands = new List<GrpCommands.rankAllowance>();
        List<Block.Blocks> storedBlocks = new List<Block.Blocks>();

        public void LoadRanks() {
            txtCmdRanks.Text = "The following ranks are available: \r\n\r\n";
            txtcmdranks2.Text = "The following ranks are available: \r\n\r\n";
            listRanks.Items.Clear();
            storedRanks.Clear();
            storedRanks.AddRange(Group.GroupList);
            foreach ( Group grp in storedRanks ) {
                txtCmdRanks.Text += "    " + grp.name + " (" + (int)grp.Permission + ")\r\n";
                txtcmdranks2.Text += "    " + grp.name + " (" + (int)grp.Permission + ")\r\n";
                listRanks.Items.Add(grp.trueName + " = " + (int)grp.Permission);
            }
            txtBlRanks.Text = txtCmdRanks.Text;
            listRanks.SelectedIndex = 0;
        }
        public void SaveRanks() {
            Group.saveGroups(storedRanks);
            Group.InitAll();
            LoadRanks();
        }

        public void LoadCommands() {
            listCommands.Items.Clear();
            storedCommands.Clear();
            foreach ( GrpCommands.rankAllowance aV in GrpCommands.allowedCommands ) {
                storedCommands.Add(aV);
                listCommands.Items.Add(aV.commandName);
            }
            if ( listCommands.SelectedIndex == -1 )
                listCommands.SelectedIndex = 0;
            // Sort the commands list
            listCommands.Sorted = true;
            listCommands.Sorted = false;
        }
        public void SaveCommands() {
            GrpCommands.Save(storedCommands);
            GrpCommands.fillRanks();
            LoadCommands();
        }

        public void LoadBlocks() {
            listBlocks.Items.Clear();
            storedBlocks.Clear();
            storedBlocks.AddRange(Block.BlockList);
            foreach ( Block.Blocks bs in storedBlocks ) {
                if ( Block.Name(bs.type) != "unknown" )
                    listBlocks.Items.Add(Block.Name(bs.type));
            }
            if ( listBlocks.SelectedIndex == -1 )
                listBlocks.SelectedIndex = 0;
        }
        public static bool prevLoaded = false;
        Form PropertyForm;
        //Form UpdateForm; // doesnt seem to be used, uncomment as needed.
        //Form EditTxtForm;

        public void SaveBlocks() {
            Block.SaveBlocks(storedBlocks);
            Block.SetBlocks();
            LoadBlocks();
        }

        public void LoadProp(string givenPath) {
        	PropertiesFile.Read(givenPath, LineProcessor);
        }
        
        void LineProcessor(string key, string value) {
            switch (key.ToLower()) {
                case "server-name":
                    if ( ValidString(value, "![]&:.,{}~-+()?_/\\' ") ) txtName.Text = value;
                    else txtName.Text = "[MCGalaxy] Minecraft server";
                    break;
                case "motd":
                    if ( ValidString(value, "=![]&:.,{}~-+()?_/\\' ") ) txtMOTD.Text = value; // allow = in the motd
                    else txtMOTD.Text = "Welcome to my server!";
                    break;
                case "port":
                    try { txtPort.Text = Convert.ToInt32(value).ToString(); }
                    catch { txtPort.Text = "25565"; }
                    break;
                case "verify-names":
                    chkVerify.Checked = ( value.ToLower() == "true" );
                    break;
                case "public":
                    chkPublic.Checked = ( value.ToLower() == "true" );
                    break;
                case "world-chat":
                    chkWorld.Checked = ( value.ToLower() == "true" );
                    break;
                case "max-players":
                    try {
                        if ( Convert.ToByte(value) > 128 ) {
                            value = "128";
                        }
                        else if ( Convert.ToByte(value) < 1 ) {
                            value = "1";
                        }
                        numPlayers.Value = Convert.ToInt16(value);
                    }
                    catch {
                        Server.s.Log("max-players invalid! setting to default.");
                        numPlayers.Value = 12;
                    }
                    numGuests.Maximum = numPlayers.Value;
                    break;
                case "max-guests":
                    try {
                        if ( Convert.ToByte(value) > numPlayers.Value ) {
                            value = numPlayers.Value.ToString();
                        }
                        else if ( Convert.ToByte(value) < 0 ) {
                            value = "0";
                        }
                        numGuests.Minimum = 0;
                        numGuests.Maximum = numPlayers.Value;
                        numGuests.Value = Convert.ToInt16(value);
                    }
                    catch {
                        Server.s.Log("max-guests invalid! setting to default.");
                        numGuests.Value = 10;
                    }
                    break;
                case "irc":
                    chkIRC.Checked = ( value.ToLower() == "true" );
                    break;
                case "irc-server":
                    txtIRCServer.Text = value;
                    break;
                case "irc-port":
                    txtIRCPort.Text = value;
                    break;
                case "irc-nick":
                    txtNick.Text = value;
                    break;
                case "irc-channel":
                    txtChannel.Text = value;
                    break;
                case "irc-opchannel":
                    txtOpChannel.Text = value;
                    break;
                case "irc-identify":
                    chkIrcId.Checked = value.ToLower() == "true";
                    break;
                case "irc-password":
                    txtIrcId.Text = value;
                    break;
                case "irc-player-titles":
                    irc_cbTitles.Checked = value.ToLower() == "true";
                    break;
                    
                case "rplimit":
                    try { txtRP.Text = value; }
                    catch { txtRP.Text = "500"; }
                    break;
                case "rplimit-norm":
                    try { txtNormRp.Text = value; }
                    catch { txtNormRp.Text = "10000"; }
                    break;

                case "log-heartbeat":
                    chkLogBeat.Checked = ( value.ToLower() == "true" );
                    break;

                case "force-cuboid":
                    chkForceCuboid.Checked = ( value.ToLower() == "true" );
                    break;

                case "profanity-filter":
                    chkProfanityFilter.Checked = ( value.ToLower() == "true" );
                    break;

                case "backup-time":
                    txtBackup.Text = Convert.ToInt32(value) > 1 ? value : "300";
                    break;

                case "backup-location":
                    if ( !value.Contains("System.Windows.Forms.TextBox, Text:") )
                        txtBackupLocation.Text = value;
                    break;

                case "physicsrestart":
                    chkPhysicsRest.Checked = value.ToLower() == "true"; break;
                case "deathcount":
                    chkDeath.Checked = value.ToLower() == "true"; break;

                case "defaultcolor":
                    ParseColor(value, chat_cmbDefault); break;
                case "irc-color":
                    ParseColor(value, chat_cmbIRC); break;
                case "help-syntax-color":
                    ParseColor(value, chat_cmbSyntax); break;
                case "help-desc-color":
                    ParseColor(value, chat_cmbDesc); break;                       
                case "default-rank":
                    try {
                        if ( cmbDefaultRank.Items.IndexOf(value.ToLower()) != -1 )
                            cmbDefaultRank.SelectedIndex = cmbDefaultRank.Items.IndexOf(value.ToLower());
                    }
                    catch { cmbDefaultRank.SelectedIndex = 1; }
                    break;

                case "cheapmessage":
                    chat_chkCheap.Checked = value.ToLower() == "true"; break;
                case "cheap-message-given":
                    chat_txtCheap.Text = value; break;
                case "custom-ban-message":
                    chat_txtBan.Text = value; break;
                case "custom-shutdown-message":
                    chat_txtShutdown.Text = value; break;
                case "custom-promote-message":
                    chat_txtPromote.Text = value; break;
                case "custom-demote-message":
                    chat_txtDemote.Text = value; break;
                    
                case "auto-restart":
                    chkRestartTime.Checked = value.ToLower() == "true"; break;
                case "restarttime":
                    txtRestartTime.Text = value; break;
                case "afk-minutes":
                    try { txtafk.Text = Convert.ToInt16(value).ToString(); }
                    catch { txtafk.Text = "10"; }
                    break;
                case "afk-kick":
                    try { txtAFKKick.Text = Convert.ToInt16(value).ToString(); }
                    catch { txtAFKKick.Text = "45"; }
                    break;
                case "check-updates":
                    chkUpdates.Checked = value.ToLower() == "true"; break;
                case "auto-update":
                    autoUpdate.Checked = value.ToLower() == "true"; break;
                case "in-game-update-notify":
                    notifyInGameUpdate.Checked = value.ToLower() == "true"; break;
                case "update-countdown":
                    try { updateTimeNumeric.Value = Convert.ToDecimal(value); }
                    catch { updateTimeNumeric.Value = 10; }
                    break;
                case "tablist-rank-sorted":
                    chat_cbTabRank.Checked = value.ToLower() == "true"; break;
                case "tablist-global":
                    chat_cbTabLevel.Checked = value.ToLower() != "true"; break;
                case "tablist-bots":
                    chat_cbTabBots.Checked = value.ToLower() == "true"; break;
                case "autoload":
                    chkAutoload.Checked = value.ToLower() == "true"; break;
                case "parse-emotes":
                    chkSmile.Checked = value.ToLower() == "true"; break;
                case "allow-tp-to-higher-ranks":
                    chkTpToHigherRanks.Checked = value.ToLower() == "true"; break;
                case "agree-to-rules-on-entry":
                    chkAgreeToRules.Checked = value.ToLower() == "true"; break;
                case "admins-join-silent":
                    chkAdminsJoinSilent.Checked = value.ToLower() == "true"; break;
                case "main-name":
                    txtMain.Text = value; break;
                case "dollar-before-dollar":
                    chk17Dollar.Checked = value.ToLower() == "true"; break;
                case "money-name":
                    txtMoneys.Text = value; break;
                case "restart-on-error":
                    chkRestart.Checked = value.ToLower() == "true"; break;
                case "repeat-messages":
                    chkRepeatMessages.Checked = value.ToLower() == "true"; break;
                case "host-state":
                    if ( value != "" ) chat_txtConsole.Text = value;
                    break;
                case "kick-on-hackrank":
                    hackrank_kick.Checked = value.ToLower() == "true"; break;
                case "hackrank-kick-time":
                    hackrank_kick_time.Text = value; break;
                case "server-owner":
                    txtServerOwner.Text = value; break;
                case "zombie-on-server-start":
                    zSettings.StartImmediately = value.ToLower() == "true"; break;
                case "no-respawning-during-zombie":
                    zSettings.Respawning = value.ToLower() != "true"; break;
                case "no-level-saving-during-zombie":
                    zSettings.SaveZombieLevelChanges = value.ToLower() != "true"; break;
                case "no-pillaring-during-zombie":
                    zSettings.Pillaring = value.ToLower() != "true"; break;
                case "zombie-name-while-infected":
                    zSettings.Name = value; break;
                case "zombie-model-while-infected":
                    zSettings.Model = value; break;                    
                case "enable-changing-levels":
                    zSettings.ChangeLevels = value.ToLower() == "true"; break;
                case "zombie-survival-only-server":
                    zSettings.SetMainLevel = value.ToLower() == "true"; break;
                case "zombie-levels-list":
                    zSettings.LevelsList = value.Replace(" ", ""); break;
                case "zombie-save-blockchanges":
                    zSettings.SaveZombieLevelChanges = value.ToLower() == "true"; break;
                case "zombie-hitbox-precision":
                    zSettings.HitboxPrecision = int.Parse(value); break;
                case "zombie-maxmove-distance":
                    zSettings.MaxMoveDistance = int.Parse(value); break;
                case "zombie-ignore-personalworlds":
                    zSettings.IgnorePersonalWorlds = value.ToLower() == "true"; break;
                case "zombie-map-inheartbeat":
                    zSettings.IncludeMapInHeartbeat = value.ToLower() == "true"; break;                    
                    
                case "guest-limit-notify":
                    chkGuestLimitNotify.Checked = value.ToLower() == "true"; break;
                case "admin-verification":
                    chkEnableVerification.Checked = value.ToLower() == "true"; break;
                case "usemysql":
                    chkUseSQL.Checked = value.ToLower() == "true"; break;
                case "username":
                    if ( value != "" ) txtSQLUsername.Text = value;
                    break;
                case "password":
                    if ( value != "" ) txtSQLPassword.Text = value;
                    break;
                case "databasename":
                    if ( value != "" ) txtSQLDatabase.Text = value;
                    break;
                case "host":
                    try {
                        IPAddress.Parse(value);
                        txtSQLHost.Text = value;
                    }
                    catch {
                        txtSQLHost.Text = "127.0.0.1";
                    }
                    break;
                case "sqlport":
                    try {
                        int.Parse(value);
                        txtSQLPort.Text = value;
                    }
                    catch {
                        txtSQLPort.Text = "3306";
                    }
                    break;
                case "mute-on-spam":
                    chkSpamControl.Checked = value.ToLower() == "true";
                    break;
                case "spam-messages":
                    try {
                        numSpamMessages.Value = Convert.ToInt16(value);
                    }
                    catch {
                        numSpamMessages.Value = 8;
                    }
                    break;
                case "spam-mute-time":
                    try {
                        numSpamMute.Value = Convert.ToInt16(value);
                    }
                    catch {
                        numSpamMute.Value = 60;
                    }
                    break;
                case "log-notes":
                    cbLogNotes.Checked = value.ToLower() == "true";
                    break;
                case "show-empty-ranks":
                    chkShowEmptyRanks.Checked = value.ToLower() == "true";
                    break;

                case "cooldown":
                    try {
                        Server.reviewcooldown = Convert.ToInt32(value.ToLower()) < 600 ? Convert.ToInt32(value.ToLower()) : 600;
                    }
                    catch {
                        Server.reviewcooldown = 600;
                        Server.s.Log("An error occurred reading the review cooldown value");
                    }
                    break;
            }
        }
        
        void ParseColor(string value, ComboBox target) {
            string color = Colors.Parse(value);
            if (color == "") {
                color = Colors.Name(value);
                if (color != "") color = value;
                else { Server.s.Log("Could not find " + value); return; }
            }
            target.SelectedIndex = target.Items.IndexOf(Colors.Name(color));
        }
        
        public bool ValidString(string str, string allowed) {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890" + allowed;
            foreach ( char ch in str ) {
                if ( allowedchars.IndexOf(ch) == -1 ) {
                    return false;
                }
            } return true;
        }

        void SaveProperties() {
            try {
                ApplyAll();
                SrvProperties.Save();
            } catch( Exception ex ) {
                Server.ErrorLog(ex);
                Server.s.Log("SAVE FAILED! properties/server.properties");
            }
        }

        void ApplyAll() {
            Server.name = txtName.Text;
            Server.motd = txtMOTD.Text;
            Server.port = int.Parse(txtPort.Text);
            Server.verify = chkVerify.Checked;
            Server.pub = chkPublic.Checked;
            Server.players = (byte)numPlayers.Value;
            Server.maxGuests = (byte)numGuests.Value;
            Server.worldChat = chkWorld.Checked;
            Server.notifyPlayers = notifyInGameUpdate.Checked;
            Server.AutoLoad = chkAutoload.Checked;
            Server.autorestart = chkRestartTime.Checked;
            try { Server.restarttime = DateTime.Parse(txtRestartTime.Text); }
            catch { } // ignore bad values
            Server.restartOnError = chkRestart.Checked;
            Server.level = ( Player.ValidName(txtMain.Text) ? txtMain.Text : "main" );
            Server.irc = chkIRC.Checked;
            Server.ircNick = txtNick.Text;
            Server.ircServer = txtIRCServer.Text;
            Server.ircChannel = txtChannel.Text;
            Server.ircOpChannel = txtOpChannel.Text;
            Server.ircPort = int.Parse(txtIRCPort.Text);
            Server.ircIdentify = chkIrcId.Checked;
            Server.ircPassword = txtIrcId.Text;
            Server.ircPlayerTitles = irc_cbTitles.Checked;

            Server.rpLimit = int.Parse(txtRP.Text);
            Server.rpNormLimit = int.Parse(txtRP.Text);
            Server.physicsRestart = chkPhysicsRest.Checked;
            Server.deathcount = chkDeath.Checked;
            Server.afkminutes = int.Parse(txtafk.Text);
            Server.afkkick = int.Parse(txtAFKKick.Text);
            Server.parseSmiley = chkSmile.Checked;
            Server.dollarNames = chk17Dollar.Checked;
            //Server.useWhitelist = ; //We don't have a setting for this?
            Server.moneys = txtMoneys.Text;
            Server.osPerbuildDefault = Group.GroupList.Find(grp => grp.name == cmbOsMap.SelectedItem.ToString()).Permission;
            	
            Server.opchatperm = Group.GroupList.Find(grp => grp.name == cmbOpChat.SelectedItem.ToString()).Permission;
            Server.adminchatperm = Group.GroupList.Find(grp => grp.name == cmbAdminChat.SelectedItem.ToString()).Permission;
            Server.logbeat = chkLogBeat.Checked;
            Server.forceCuboid = chkForceCuboid.Checked;
            Server.profanityFilter = chkProfanityFilter.Checked;
            Server.repeatMessage = chkRepeatMessages.Checked;
            Server.ZallState = chat_txtConsole.Text;
            Server.agreetorulesonentry = chkAgreeToRules.Checked;
            Server.adminsjoinsilent = chkAdminsJoinSilent.Checked;
            Server.server_owner = txtServerOwner.Text;
            zSettings.ApplyToServer();
            Server.guestLimitNotify = chkGuestLimitNotify.Checked;

            Server.backupInterval = int.Parse(txtBackup.Text);
            Server.backupLocation = txtBackupLocation.Text;
            //Server.reportBack = ;  //No setting for this?

            Server.useMySQL = chkUseSQL.Checked;
            Server.MySQLHost = txtSQLHost.Text;
            Server.MySQLPort = txtSQLPort.Text;
            Server.MySQLUsername = txtSQLUsername.Text;
            Server.MySQLPassword = txtSQLPassword.Text;
            Server.MySQLDatabaseName = txtSQLDatabase.Text;
            //Server.MySQLPooling = ; // No setting for this?

            Server.DefaultColor = Colors.Parse(chat_cmbDefault.SelectedItem.ToString());
            Server.IRCColour = Colors.Parse(chat_cmbIRC.SelectedItem.ToString());
            Server.HelpSyntaxColor = Colors.Parse(chat_cmbSyntax.SelectedItem.ToString());
            Server.HelpDescriptionColor = Colors.Parse(chat_cmbDesc.SelectedItem.ToString());
            Server.TablistRankSorted = chat_cbTabRank.Checked;
            Server.TablistGlobal = !chat_cbTabLevel.Checked;
            Server.TablistBots = chat_cbTabBots.Checked;
           
            Server.higherranktp = chkTpToHigherRanks.Checked;
            Server.checkUpdates = chkUpdates.Checked;

            Server.cheapMessage = chat_chkCheap.Checked;
            Server.cheapMessageGiven = chat_txtCheap.Text;
            Server.defaultBanMessage = chat_txtBan.Text;
            Server.shutdownMessage = chat_txtShutdown.Text;
            Server.defaultDemoteMessage = chat_txtDemote.Text;
            Server.defaultPromoteMessage = chat_txtPromote.Text;
            
            Server.defaultRank = cmbDefaultRank.SelectedItem.ToString();

            Server.hackrank_kick = hackrank_kick.Checked;
            Server.hackrank_kick_time = int.Parse(hackrank_kick_time.Text);
            Server.verifyadmins = chkEnableVerification.Checked;
            Server.verifyadminsrank = Group.GroupList.Find(grp => grp.name == cmbVerificationRank.SelectedItem.ToString()).Permission;

            Server.checkspam = chkSpamControl.Checked;
            Server.spamcounter = (int)numSpamMessages.Value;
            Server.mutespamtime = (int)numSpamMute.Value;
            Server.spamcountreset = (int)numCountReset.Value;
            Server.LogNotes = cbLogNotes.Checked;
            Server.showEmptyRanks = chkShowEmptyRanks.Checked;
            Server.reviewcooldown = (int)nudCooldownTime.Value;
        }

        private void chat_cmbDefault_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colDefault.BackColor = GetColor(chat_cmbDefault.Items[chat_cmbDefault.SelectedIndex].ToString());
        }

        private void chat_cmbIRC_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colIRC.BackColor = GetColor(chat_cmbIRC.Items[chat_cmbIRC.SelectedIndex].ToString());
        }
        
        private void chat_cmbSyntax_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colSyntax.BackColor = GetColor(chat_cmbSyntax.Items[chat_cmbSyntax.SelectedIndex].ToString());
        }

        private void chat_cmbDesc_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colDesc.BackColor = GetColor(chat_cmbDesc.Items[chat_cmbDesc.SelectedIndex].ToString());
        }        
        
        Color GetColor(string name) {
        	string code = Colors.Parse(name);
        	if (code == "") return SystemColors.Control;
        	if (Colors.IsStandardColor(code[1])) return Color.FromName(name);
        	
        	CustomColor col = Colors.ExtColors[code[1]];
        	return Color.FromArgb(col.R, col.G, col.B);
        }

        void removeDigit(TextBox foundTxt) {
            try {
                int lastChar = int.Parse(foundTxt.Text[foundTxt.Text.Length - 1].ToString());
            }
            catch {
                foundTxt.Text = "";
            }
        }

        private void txtPort_TextChanged(object sender, EventArgs e) { removeDigit(txtPort); }
        private void txtBackup_TextChanged(object sender, EventArgs e) { removeDigit(txtBackup); }

        private void btnSave_Click(object sender, EventArgs e) { saveStuff(); Dispose(); }
        private void btnApply_Click(object sender, EventArgs e) { saveStuff(); }

        void saveStuff() {
            foreach ( Control tP in tabControl.Controls )
                if ( tP is TabPage && tP != pageCommands && tP != pageBlocks )
                    foreach ( Control ctrl in tP.Controls )
                        if ( ctrl is TextBox && ctrl.Name.ToLower() != "txtgrpmotd" )
                            if ( ctrl.Text == "" ) {
                                MessageBox.Show("A textbox has been left empty. It must be filled.\n" + ctrl.Name);
                                return;
                            }

            SaveProperties();
            SaveRanks();
            SaveCommands();
            SaveOldExtraCustomCmdChanges();
            SaveBlocks();
            try { SaveLavaSettings(); }
            catch { Server.s.Log("Error saving Lava Survival settings!"); }

            SrvProperties.Load("properties/server.properties", true); // loads when saving?
            GrpCommands.fillRanks();

            // Trigger profanity filter reload
            // Not the best way of doing things, but it kinda works
            ProfanityFilter.Init();
        }

        private void btnDiscard_Click(object sender, EventArgs e) {
            this.Dispose();
        }

        private void toolTip_Popup(object sender, PopupEventArgs e) {

        }

        private void tabPage2_Click(object sender, EventArgs e) {

        }

        private void tabPage1_Click(object sender, EventArgs e) {

        }

        private void chkPhysicsRest_CheckedChanged(object sender, EventArgs e) {
        }

        private void chkGC_CheckedChanged(object sender, EventArgs e) {
        }

        private void chkIRC_CheckedChanged(object sender, EventArgs e) {

            toggleIrcSettings(chkIRC.Checked);
        }

        private void btnBackup_Click(object sender, EventArgs e) {
            /*FolderBrowserDialog folderDialog = new FolderBrowserDialog();
folderDialog.Description = "Select Folder";
if (folderDialog.ShowDialog() == DialogResult.OK) {
txtBackupLocation.Text = folderDialog.SelectedPath;
}*/
            MessageBox.Show("Currently glitchy! Just type in the location by hand.");
        }

        #region rankTab
        private void cmbColor_SelectedIndexChanged(object sender, EventArgs e) {
            lblColor.BackColor = GetColor(cmbColor.Items[cmbColor.SelectedIndex].ToString());
            storedRanks[listRanks.SelectedIndex].color = Colors.Parse(cmbColor.Items[cmbColor.SelectedIndex].ToString());
        }

        bool skip = false;
        private void listRanks_SelectedIndexChanged(object sender, EventArgs e) {
            if ( skip ) return;
            Group grp = storedRanks.Find(G => G.trueName == listRanks.Items[listRanks.SelectedIndex].ToString().Split('=')[0].Trim());
            if ( grp.Permission == LevelPermission.Nobody ) { listRanks.SelectedIndex = 0; return; }

            txtRankName.Text = grp.trueName;
            txtPermission.Text = ( (int)grp.Permission ).ToString();
            txtLimit.Text = grp.maxBlocks.ToString();
            txtMaxUndo.Text = grp.maxUndo.ToString();
            cmbColor.SelectedIndex = cmbColor.Items.IndexOf(Colors.Name(grp.color));
            txtGrpMOTD.Text = String.IsNullOrEmpty(grp.MOTD) ? String.Empty : grp.MOTD;
            txtFileName.Text = grp.fileName;
            txtOSMaps.Text = grp.OverseerMaps.ToString();
            txtPrefix.Text = grp.prefix;
        }

        private void txtRankName_TextChanged(object sender, EventArgs e) {
            if ( txtRankName.Text != "" && txtRankName.Text.ToLower() != "nobody" ) {
                storedRanks[listRanks.SelectedIndex].trueName = txtRankName.Text;
                skip = true;
                listRanks.Items[listRanks.SelectedIndex] = txtRankName.Text + " = " + (int)storedRanks[listRanks.SelectedIndex].Permission;
                skip = false;
            }
        }

        private void txtPermission_TextChanged(object sender, EventArgs e) {
            if ( txtPermission.Text != "" ) {
                int foundPerm;
                if (!int.TryParse(txtPermission.Text, out foundPerm)) {
                    if ( txtPermission.Text != "-" )
                        txtPermission.Text = txtPermission.Text.Remove(txtPermission.Text.Length - 1);
                    return;
                }

                if ( foundPerm < -50 ) { txtPermission.Text = "-50"; return; }
                else if ( foundPerm > 119 ) { txtPermission.Text = "119"; return; }

                storedRanks[listRanks.SelectedIndex].Permission = (LevelPermission)foundPerm;
                skip = true;
                listRanks.Items[listRanks.SelectedIndex] = storedRanks[listRanks.SelectedIndex].trueName + " = " + foundPerm;
                skip = false;
            }
        }

        private void txtLimit_TextChanged(object sender, EventArgs e) {
            if ( txtLimit.Text != "" ) {
                int drawLimit;
                if (!int.TryParse(txtLimit.Text, out drawLimit)) {
                    txtLimit.Text = txtLimit.Text.Remove(txtLimit.Text.Length - 1);
                    return;
                }

                if ( drawLimit < 1 ) { txtLimit.Text = "1"; return; }

                storedRanks[listRanks.SelectedIndex].maxBlocks = drawLimit;
            }
        }

        private void txtMaxUndo_TextChanged(object sender, EventArgs e) {
            if ( txtMaxUndo.Text != "" ) {
                long maxUndo;
                if (!long.TryParse(txtMaxUndo.Text, out maxUndo)) {
                    txtMaxUndo.Text = txtMaxUndo.Text.Remove(txtMaxUndo.Text.Length - 1);
                    return;
                }

                if ( maxUndo < -1 ) { txtMaxUndo.Text = "0"; return; }

                storedRanks[listRanks.SelectedIndex].maxUndo = maxUndo;
            }
        }
        
        private void txtOSMaps_TextChanged(object sender, EventArgs e) {
            if ( txtOSMaps.Text != "" ) {
                byte maxMaps;
                if (!byte.TryParse(txtOSMaps.Text, out maxMaps)) {
                    txtOSMaps.Text = txtOSMaps.Text.Remove(txtOSMaps.Text.Length - 1);
                    return;
                }
                storedRanks[listRanks.SelectedIndex].OverseerMaps = maxMaps;
            }
        }        

        private void txtFileName_TextChanged(object sender, EventArgs e) {
            if ( txtFileName.Text != "" ) {
                storedRanks[listRanks.SelectedIndex].fileName = txtFileName.Text;
            }
        }
        
        private void txtPrefix_TextChanged(object sender, EventArgs e) {
            storedRanks[listRanks.SelectedIndex].prefix = txtPrefix.Text;
        }

        private void btnAddRank_Click(object sender, EventArgs e) {
            Group newGroup = new Group((LevelPermission)5, 600, 30, "CHANGEME", '1', String.Empty, "CHANGEME.txt");
            storedRanks.Add(newGroup);
            listRanks.Items.Add(newGroup.trueName + " = " + (int)newGroup.Permission);
        }

        private void button1_Click(object sender, EventArgs e) {
            if ( listRanks.Items.Count > 1 ) {
                storedRanks.RemoveAt(listRanks.SelectedIndex);
                skip = true;
                listRanks.Items.RemoveAt(listRanks.SelectedIndex);
                skip = false;

                listRanks.SelectedIndex = 0;
            }
        }
        #endregion

        #region commandTab
        private void listCommands_SelectedIndexChanged(object sender, EventArgs e) {
            Command cmd = Command.all.Find(listCommands.SelectedItem.ToString());
            GrpCommands.rankAllowance allowVar = storedCommands.Find(aV => aV.commandName == cmd.name);

            if ( Group.findPerm(allowVar.lowestRank) == null ) allowVar.lowestRank = cmd.defaultRank;
            txtCmdLowest.Text = (int)allowVar.lowestRank + "";

            bool foundOne = false;
            txtCmdDisallow.Text = "";
            foreach ( LevelPermission perm in allowVar.disallow ) {
                foundOne = true;
                txtCmdDisallow.Text += "," + (int)perm;
            }
            if ( foundOne ) txtCmdDisallow.Text = txtCmdDisallow.Text.Remove(0, 1);

            foundOne = false;
            txtCmdAllow.Text = "";
            foreach ( LevelPermission perm in allowVar.allow ) {
                foundOne = true;
                txtCmdAllow.Text += "," + (int)perm;
            }
            if ( foundOne ) txtCmdAllow.Text = txtCmdAllow.Text.Remove(0, 1);
        }
        private void txtCmdLowest_TextChanged(object sender, EventArgs e) {
            fillLowest(ref txtCmdLowest, ref storedCommands[listCommands.SelectedIndex].lowestRank);
        }
        private void txtCmdDisallow_TextChanged(object sender, EventArgs e) {
            fillAllowance(ref txtCmdDisallow, ref storedCommands[listCommands.SelectedIndex].disallow);
        }
        private void txtCmdAllow_TextChanged(object sender, EventArgs e) {
            fillAllowance(ref txtCmdAllow, ref storedCommands[listCommands.SelectedIndex].allow);
        }
        #endregion

        #region BlockTab
        private void listBlocks_SelectedIndexChanged(object sender, EventArgs e) {
            byte b = Block.Byte(listBlocks.SelectedItem.ToString());
            Block.Blocks bs = storedBlocks.Find(bS => bS.type == b);

            txtBlLowest.Text = (int)bs.lowestRank + "";

            bool foundOne = false;
            txtBlDisallow.Text = "";
            if (bs.disallow != null) {
                foreach ( LevelPermission perm in bs.disallow ) {
                    foundOne = true;
                    txtBlDisallow.Text += "," + (int)perm;
                }
            }
            if ( foundOne ) txtBlDisallow.Text = txtBlDisallow.Text.Remove(0, 1);

            foundOne = false;
            txtBlAllow.Text = "";
            if (bs.allow != null) {
                foreach ( LevelPermission perm in bs.allow ) {
                    foundOne = true;
                    txtBlAllow.Text += "," + (int)perm;
                }
            }
            if ( foundOne ) txtBlAllow.Text = txtBlAllow.Text.Remove(0, 1);
        }
        private void txtBlLowest_TextChanged(object sender, EventArgs e) {
            fillLowest(ref txtBlLowest, ref storedBlocks[Block.Byte(listBlocks.SelectedItem.ToString())].lowestRank);
        }
        private void txtBlDisallow_TextChanged(object sender, EventArgs e) {
        	if (storedBlocks[listBlocks.SelectedIndex].disallow == null)
        	    storedBlocks[listBlocks.SelectedIndex].disallow = new List<LevelPermission>();        	
            fillAllowance(ref txtBlDisallow, ref storedBlocks[listBlocks.SelectedIndex].disallow);
        }
        private void txtBlAllow_TextChanged(object sender, EventArgs e) {
        	if (storedBlocks[listBlocks.SelectedIndex].allow == null)
        	    storedBlocks[listBlocks.SelectedIndex].allow = new List<LevelPermission>();
            fillAllowance(ref txtBlAllow, ref storedBlocks[listBlocks.SelectedIndex].allow);
        }
        #endregion
        private void fillAllowance(ref TextBox txtBox, ref List<LevelPermission> addTo) {
            addTo.Clear();
            if ( txtBox.Text != "" ) {
                string[] perms = txtBox.Text.Split(',');
                for ( int i = 0; i < perms.Length; i++ ) {
                    perms[i] = perms[i].Trim().ToLower();
                    int foundPerm;
                    try {
                        foundPerm = int.Parse(perms[i]);
                    }
                    catch {
                        Group foundGroup = Group.Find(perms[i]);
                        if ( foundGroup != null ) foundPerm = (int)foundGroup.Permission;
                        else { Server.s.Log("Could not find " + perms[i]); continue; }
                    }
                    addTo.Add((LevelPermission)foundPerm);
                }

                txtBox.Text = "";
                foreach ( LevelPermission p in addTo ) {
                    txtBox.Text += "," + (int)p;
                }
                if ( txtBox.Text != "" ) txtBox.Text = txtBox.Text.Remove(0, 1);
            }
        }
        private void fillLowest(ref TextBox txtBox, ref LevelPermission toChange) {
            if ( txtBox.Text != "" ) {
                txtBox.Text = txtBox.Text.Trim().ToLower();
                int foundPerm = -100;
                try {
                    foundPerm = int.Parse(txtBox.Text);
                }
                catch {
                    Group foundGroup = Group.Find(txtBox.Text);
                    if ( foundGroup != null ) foundPerm = (int)foundGroup.Permission;
                    else { Server.s.Log("Could not find " + txtBox.Text); }
                }

                txtBox.Text = "";
                if ( foundPerm < -99 ) txtBox.Text = (int)toChange + "";
                else txtBox.Text = foundPerm + "";

                toChange = (LevelPermission)Convert.ToInt16(txtBox.Text);
            }
        }

        private void btnBlHelp_Click(object sender, EventArgs e) {
            getHelp(listBlocks.SelectedItem.ToString());
        }
        private void btnCmdHelp_Click(object sender, EventArgs e) {
            getHelp(listCommands.SelectedItem.ToString());
        }
        private void getHelp(string toHelp) {
            Player.storedHelp = "";
            Player.storeHelp = true;
            Command.all.Find("help").Use(null, toHelp);
            Player.storeHelp = false;
            string messageInfo = "Help information for " + toHelp + ":\r\n\r\n";
            messageInfo += Player.storedHelp;
            MessageBox.Show(messageInfo);
        }


        private void ChkPort_Click(object sender, EventArgs e) {
            using ( var form = new PortTools() ) {
                form.ShowDialog();
            }
        }



        private void CrtCustCmd_Click(object sender, EventArgs e) {
            if ( txtCommandName.Text != null ) {
                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                    MessageBox.Show("Sorry, That command already exists!!");
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text);
                    MessageBox.Show("Command Created!!");
                }
            }
            else {
                MessageBox.Show("You didnt specify a name for the command!!");
            }
        }

        private void CompileCustCmd_Click(object sender, EventArgs e) {
            if ( txtCommandName.Text != null ) {
                if ( File.Exists("extra/commands/dll/Cmd" + txtCommandName.Text + ".dll") ) {
                    MessageBox.Show("Sorry, That command already exists!!");
                }
                else {
                    Command.all.Find("compile").Use(null, txtCommandName.Text);
                    MessageBox.Show("Command Compiled!!");
                }
            }
            else {
                MessageBox.Show("You didnt specify a name for the command!!");
            }
        }



        private void numPlayers_ValueChanged(object sender, EventArgs e) {
            // Ensure that number of guests is never more than number of players
            if ( numGuests.Value > numPlayers.Value ) {
                numGuests.Value = numPlayers.Value;
            }
            numGuests.Maximum = numPlayers.Value;
        }

        private void editTxtsBt_Click_1(object sender, EventArgs e) {
            if ( EditTextOpen ) {
                return;
            }
            PropertyForm = new EditText();
            PropertyForm.Show();
        }

        private void btnCreate_Click(object sender, EventArgs e) {
            if(String.IsNullOrEmpty(txtCommandName.Text.Trim())) {
                MessageBox.Show ( "Command must have a name" );
                return;
            }

            if ( radioButton1.Checked ) {
                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") || File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                    MessageBox.Show("Command already exists", "", MessageBoxButtons.OK);
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text.ToLower() + " vb");
                    MessageBox.Show("New Command Created: " + txtCommandName.Text.ToLower() + " Created.");
                }
            }
            else {



                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") || File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") ) {
                    MessageBox.Show("Command already exists", "", MessageBoxButtons.OK);
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text.ToLower());
                    MessageBox.Show("New Command Created: " + txtCommandName.Text.ToLower() + " Created.");
                }
            }
        }

       

        private void btnLoad_Click(object sender, EventArgs e) {
            Command[] commands = null;

            using(FileDialog fileDialog = new OpenFileDialog()) {
                fileDialog.RestoreDirectory = true;
                fileDialog.Filter = "Accepted File Types (*.cs, *.vb, *.dll)|*.cs;*.vb;*.dll|C# Source (*.cs)|*.cs|Visual Basic Source (*.vb)|*.vb|.NET Assemblies (*.dll)|*.dll";

                if ( fileDialog.ShowDialog() == DialogResult.OK ) {

                    string fileName = fileDialog.FileName;

                    if ( fileName.EndsWith ( ".dll" ) ) {
                        commands = MCGalaxyScripter.FromAssemblyFile ( fileName );
                    }
                    else {

                        ScriptLanguage language = fileName.EndsWith ( ".cs" ) ? ScriptLanguage.CSharp : ScriptLanguage.VB;

                        if ( File.Exists ( fileName ) ) {
                            var result = MCGalaxyScripter.Compile ( File.ReadAllText ( fileName ), language );

                            if ( result == null ) {
                                MessageBox.Show ( "Error compiling files" );
                                return;
                            }

                            if ( result.CompilerErrors != null ) {
                                foreach ( CompilerError err in result.CompilerErrors ) {
                                    Server.s.ErrorCase( "Error #" + err.ErrorNumber );
                                    Server.s.ErrorCase( "Message: " + err.ErrorText );
                                    Server.s.ErrorCase( "Line: " + err.Line );

                                    Server.s.ErrorCase( "=================================" );
                                }
                                MessageBox.Show ( "Error compiling from source. Check logs for error" );
                                return;
                            }

                            commands = result.Commands;
                        }
                    }
                }
            }

            if ( commands == null ) {
                MessageBox.Show( "Error compiling files" );
                return;
            }

            for ( int i = 0; i < commands.Length; i++ ) {
                Command command = commands[ i ];

                if(lstCommands.Items.Contains(command.name)) {
                    MessageBox.Show ( "Command " + command.name + " already exists. As a result, it was not loaded" );
                    continue;
                }

                lstCommands.Items.Add ( command.name );
                Command.all.Add(command);
                Server.s.Log("Added " + command.name + " to commands");
            }
            
            GrpCommands.fillRanks();
        }

        private void btnUnload_Click(object sender, EventArgs e) {
            Command foundCmd = Command.all.Find(lstCommands.SelectedItem.ToString());
            if ( foundCmd == null ) {
                MessageBox.Show(txtCommandName.Text + " is not a valid or loaded command.", "");
                return;
            }

            lstCommands.Items.Remove( foundCmd.name );
            Command.all.Remove(foundCmd);
            GrpCommands.fillRanks();
            MessageBox.Show("Command was successfully unloaded.", "");
        }


        private void btnDiscardcmd_Click(object sender, EventArgs e) {
            switch ( MessageBox.Show("Are you sure you want to discard this whole file?", "Discard?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ) {
                case DialogResult.Yes:
                    if ( radioButton1.Checked ) {

                        if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") ) {
                            File.Delete("extra/commands/source/Cmd" + txtCommandName.Text + ".vb");
                        }
                        else { MessageBox.Show("File: " + txtCommandName.Text + ".vb Doesnt Exist."); }
                    }
                    else {
                        if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                            File.Delete("extra/commands/source/Cmd" + txtCommandName.Text + ".cs");
                        }
                        else { MessageBox.Show("File: " + txtCommandName.Text + ".cs Doesnt Exist."); }
                    }
                    break;

            }
        }

        private void btnReset_Click(object sender, EventArgs e) {
            if ( listPasswords.Text == "" ) {
                MessageBox.Show("You have not selected a user's password to reset!");
                return;
            }
            try {
                File.Delete("extra/passwords/" + listPasswords.Text + ".dat");
                listPasswords.Items.Clear();                
                string[] files = Directory.GetFiles("extra/passwords", "*.dat");
                
                listPasswords.BeginUpdate();
                foreach (string file in files) {
                    string name = Path.GetFileNameWithoutExtension(file);
                    listPasswords.Items.Add(name);
                }
                listPasswords.EndUpdate();
            }
            catch {
                MessageBox.Show("Failed to reset password!");
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start("http://dev.mysql.com/downloads/");
            }
            catch {
                MessageBox.Show("Failed to open link!");
            }
        }

        private void chkUseSQL_CheckedChanged(object sender, EventArgs e) {
            toggleMySQLSettings(chkUseSQL.Checked);
        }

        private void forceUpdateBtn_Click(object sender, EventArgs e) {
            forceUpdateBtn.Enabled = false;
            if ( MessageBox.Show("Would you like to force update MCGalaxy now?", "Force Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK ) {
                saveStuff();
                MCGalaxy.Gui.App.PerformUpdate();
                Dispose();
            }
            else {
                forceUpdateBtn.Enabled = true;
            }
        }

        private bool skipExtraPermChanges;
        private int oldnumber;
        private Command oldcmd;
        private void listCommandsExtraCmdPerms_SelectedIndexChanged(object sender, EventArgs e) {
            SaveOldExtraCustomCmdChanges();
            Command cmd = Command.all.Find(listCommandsExtraCmdPerms.SelectedItem.ToString());
            oldcmd = cmd;
            skipExtraPermChanges = true;
            extracmdpermnumber.Maximum = CommandOtherPerms.GetMaxNumber(cmd);
            extracmdpermnumber.ReadOnly = extracmdpermnumber.Maximum == 1;
            extracmdpermnumber.Value = 1;
            skipExtraPermChanges = false;
            extracmdpermdesc.Text = CommandOtherPerms.Find(cmd, 1).Description;
            extracmdpermperm.Text = CommandOtherPerms.Find(cmd, 1).Permission.ToString();
            oldnumber = (int)extracmdpermnumber.Value;
        }

        private void SaveOldExtraCustomCmdChanges() {
            if (oldcmd == null || skipExtraPermChanges) return;
            CommandOtherPerms.Edit(CommandOtherPerms.Find(oldcmd, oldnumber), int.Parse(extracmdpermperm.Text));
            CommandOtherPerms.Save();
        }

        private void extracmdpermnumber_ValueChanged(object sender, EventArgs e) {
            SaveOldExtraCustomCmdChanges();
            oldnumber = (int)extracmdpermnumber.Value;
            extracmdpermdesc.Text = CommandOtherPerms.Find(oldcmd, (int)extracmdpermnumber.Value).Description;
            extracmdpermperm.Text = CommandOtherPerms.Find(oldcmd, (int)extracmdpermnumber.Value).Permission.ToString();
        }
        private void LoadExtraCmdCmds() {
            listCommandsExtraCmdPerms.Items.Clear();
            foreach ( Command cmd in Command.all.commands ) {
                if ( CommandOtherPerms.Find(cmd) != null ) {
                    listCommandsExtraCmdPerms.Items.Add(cmd.name);
                }
            }
            listCommandsExtraCmdPerms.Sorted = true;
            listCommandsExtraCmdPerms.Sorted = false;
        }

        private void txtGrpMOTD_TextChanged(object sender, EventArgs e) {
            if ( txtGrpMOTD.Text != null ) storedRanks[listRanks.SelectedIndex].MOTD = txtGrpMOTD.Text;
        }

        private void buttonEco_Click(object sender, EventArgs e) {
            new GUI.Eco.EconomyWindow().ShowDialog();
        }

        
        private void lstCommands_SelectedIndexChanged ( object sender, EventArgs e ) {
            btnUnload.Enabled = lstCommands.SelectedIndex != -1;
        }


        /// <summary> Toggles enabled state for IRC options. </summary>
        /// <param name="enabled"></param>
        protected void toggleIrcSettings(bool enabled) {
            txtIRCServer.Enabled = enabled;
            txtIRCPort.Enabled = enabled;
            txtNick.Enabled = enabled;
            txtChannel.Enabled = enabled;
            txtOpChannel.Enabled = enabled;
            txtIrcId.Enabled = enabled;
            chkIrcId.Enabled = enabled;
            irc_cbTitles.Enabled = enabled;
        }


        /// <summary> Toggles enabeld state for MySQL options. </summary>
        /// <param name="enabled"></param>
        protected void toggleMySQLSettings(bool enabled)
        {
            txtSQLUsername.Enabled = enabled;
            txtSQLPassword.Enabled = enabled;
            txtSQLPort.Enabled = enabled;
            txtSQLHost.Enabled = enabled;
            txtSQLDatabase.Enabled = enabled;
        }
    }
}
