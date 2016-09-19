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
namespace MCGalaxy.Gui
{
    partial class PropertyWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabChat = new System.Windows.Forms.TabPage();
            this.chat_gbTab = new System.Windows.Forms.GroupBox();
            this.chat_cbTabRank = new System.Windows.Forms.CheckBox();
            this.chat_cbTabLevel = new System.Windows.Forms.CheckBox();
            this.chat_cbTabBots = new System.Windows.Forms.CheckBox();
            this.chat_gbMessages = new System.Windows.Forms.GroupBox();
            this.chat_lblDemote = new System.Windows.Forms.Label();
            this.chat_lblPromote = new System.Windows.Forms.Label();
            this.chat_lblBan = new System.Windows.Forms.Label();
            this.chat_txtDemote = new System.Windows.Forms.TextBox();
            this.chat_txtPromote = new System.Windows.Forms.TextBox();
            this.chat_lblShutdown = new System.Windows.Forms.Label();
            this.chat_txtBan = new System.Windows.Forms.TextBox();
            this.chat_txtCheap = new System.Windows.Forms.TextBox();
            this.chat_chkCheap = new System.Windows.Forms.CheckBox();
            this.chat_txtShutdown = new System.Windows.Forms.TextBox();
            this.chat_gbOther = new System.Windows.Forms.GroupBox();
            this.chat_lblConsole = new System.Windows.Forms.Label();
            this.chat_txtConsole = new System.Windows.Forms.TextBox();
            this.chat_gbColors = new System.Windows.Forms.GroupBox();
            this.chat_colDesc = new System.Windows.Forms.Label();
            this.chat_cmbDesc = new System.Windows.Forms.ComboBox();
            this.chat_lblDesc = new System.Windows.Forms.Label();
            this.chat_colSyntax = new System.Windows.Forms.Label();
            this.chat_cmbSyntax = new System.Windows.Forms.ComboBox();
            this.chat_lblSyntax = new System.Windows.Forms.Label();
            this.chat_colIRC = new System.Windows.Forms.Label();
            this.chat_cmbIRC = new System.Windows.Forms.ComboBox();
            this.chat_lblIRC = new System.Windows.Forms.Label();
            this.chat_colDefault = new System.Windows.Forms.Label();
            this.chat_cmbDefault = new System.Windows.Forms.ComboBox();
            this.chat_lblDefault = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDiscard = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pageCommands = new System.Windows.Forms.TabPage();
            this.pageCommandsList = new System.Windows.Forms.TabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.txtCmdRanks = new System.Windows.Forms.TextBox();
            this.btnCmdHelp = new System.Windows.Forms.Button();
            this.txtCmdAllow = new System.Windows.Forms.TextBox();
            this.txtCmdLowest = new System.Windows.Forms.TextBox();
            this.txtCmdDisallow = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.listCommands = new System.Windows.Forms.ListBox();
            this.pageCommandsCustom = new System.Windows.Forms.TabPage();
            this.lblLoadedCommands = new System.Windows.Forms.Label();
            this.lstCommands = new System.Windows.Forms.ListBox();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.btnCreate = new System.Windows.Forms.Button();
            this.txtCommandName = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.btnUnload = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.pageCommandPerms = new System.Windows.Forms.TabPage();
            this.txtcmdranks2 = new System.Windows.Forms.TextBox();
            this.label74 = new System.Windows.Forms.Label();
            this.label73 = new System.Windows.Forms.Label();
            this.extracmdpermnumber = new System.Windows.Forms.NumericUpDown();
            this.label72 = new System.Windows.Forms.Label();
            this.extracmdpermdesc = new System.Windows.Forms.TextBox();
            this.extracmdpermperm = new System.Windows.Forms.TextBox();
            this.listCommandsExtraCmdPerms = new System.Windows.Forms.ListBox();
            this.label24 = new System.Windows.Forms.Label();
            this.chkPhysicsRest = new System.Windows.Forms.CheckBox();
            this.chkDeath = new System.Windows.Forms.CheckBox();
            this.txtBackup = new System.Windows.Forms.TextBox();
            this.txtafk = new System.Windows.Forms.TextBox();
            this.txtAFKKick = new System.Windows.Forms.TextBox();
            this.hackrank_kick = new System.Windows.Forms.CheckBox();
            this.chkIRC = new System.Windows.Forms.CheckBox();
            this.txtNick = new System.Windows.Forms.TextBox();
            this.txtIRCServer = new System.Windows.Forms.TextBox();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.txtOpChannel = new System.Windows.Forms.TextBox();
            this.chkVerify = new System.Windows.Forms.CheckBox();
            this.chkWorld = new System.Windows.Forms.CheckBox();
            this.chkAutoload = new System.Windows.Forms.CheckBox();
            this.chkPublic = new System.Windows.Forms.CheckBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtMOTD = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.chkLogBeat = new System.Windows.Forms.CheckBox();
            this.chkUseSQL = new System.Windows.Forms.CheckBox();
            this.sec_cmbVerifyRank = new System.Windows.Forms.ComboBox();
            this.sec_cbVerifyAdmins = new System.Windows.Forms.CheckBox();
            this.cmbAFKKickPerm = new System.Windows.Forms.ComboBox();
            this.chkGuestLimitNotify = new System.Windows.Forms.CheckBox();
            this.cmbAdminChat = new System.Windows.Forms.ComboBox();
            this.cmbOpChat = new System.Windows.Forms.ComboBox();
            this.chkTpToHigherRanks = new System.Windows.Forms.CheckBox();
            this.cmbDefaultRank = new System.Windows.Forms.ComboBox();
            this.cmbOsMap = new System.Windows.Forms.ComboBox();
            this.sec_cbWhitelist = new System.Windows.Forms.CheckBox();
            this.sec_cbLogNotes = new System.Windows.Forms.CheckBox();
            this.sec_cbChatAuto = new System.Windows.Forms.CheckBox();
            this.pageBlocks = new System.Windows.Forms.TabPage();
            this.btnBlHelp = new System.Windows.Forms.Button();
            this.txtBlRanks = new System.Windows.Forms.TextBox();
            this.txtBlAllow = new System.Windows.Forms.TextBox();
            this.txtBlLowest = new System.Windows.Forms.TextBox();
            this.txtBlDisallow = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.listBlocks = new System.Windows.Forms.ListBox();
            this.pageRanks = new System.Windows.Forms.TabPage();
            this.gbRankGeneral = new System.Windows.Forms.GroupBox();
            this.label29 = new System.Windows.Forms.Label();
            this.chkAdminsJoinSilent = new System.Windows.Forms.CheckBox();
            this.lblOpChat = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.gbRankSettings = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtOSMaps = new System.Windows.Forms.TextBox();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.txtLimit = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lblColor = new System.Windows.Forms.Label();
            this.txtGrpMOTD = new System.Windows.Forms.TextBox();
            this.txtPermission = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtRankName = new System.Windows.Forms.TextBox();
            this.cmbColor = new System.Windows.Forms.ComboBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.lblMOTD = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtMaxUndo = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnAddRank = new System.Windows.Forms.Button();
            this.listRanks = new System.Windows.Forms.ListBox();
            this.label85 = new System.Windows.Forms.Label();
            this.pageMisc = new System.Windows.Forms.TabPage();
            this.economyGroupBox = new System.Windows.Forms.GroupBox();
            this.buttonEco = new System.Windows.Forms.Button();
            this.grpExtra = new System.Windows.Forms.GroupBox();
            this.nudCooldownTime = new System.Windows.Forms.NumericUpDown();
            this.label84 = new System.Windows.Forms.Label();
            this.lblOsMap = new System.Windows.Forms.Label();
            this.chkShowEmptyRanks = new System.Windows.Forms.CheckBox();
            this.chkRepeatMessages = new System.Windows.Forms.CheckBox();
            this.txtRestartTime = new System.Windows.Forms.TextBox();
            this.txtMoneys = new System.Windows.Forms.TextBox();
            this.chkRestartTime = new System.Windows.Forms.CheckBox();
            this.chk17Dollar = new System.Windows.Forms.CheckBox();
            this.chkSmile = new System.Windows.Forms.CheckBox();
            this.label34 = new System.Windows.Forms.Label();
            this.grpMessages = new System.Windows.Forms.GroupBox();
            this.hackrank_kick_time = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.grpPhysics = new System.Windows.Forms.GroupBox();
            this.txtRP = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.txtNormRp = new System.Windows.Forms.TextBox();
            this.grpAFK = new System.Windows.Forms.GroupBox();
            this.label76 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.grpBackups = new System.Windows.Forms.GroupBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txtBackupLocation = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.chkProfanityFilter = new System.Windows.Forms.CheckBox();
            this.pageIRC = new System.Windows.Forms.TabPage();
            this.grpSQL = new System.Windows.Forms.GroupBox();
            this.txtSQLPort = new System.Windows.Forms.TextBox();
            this.label70 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.txtSQLHost = new System.Windows.Forms.TextBox();
            this.label43 = new System.Windows.Forms.Label();
            this.txtSQLDatabase = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.txtSQLPassword = new System.Windows.Forms.TextBox();
            this.txtSQLUsername = new System.Windows.Forms.TextBox();
            this.grpIRC = new System.Windows.Forms.GroupBox();
            this.txtIRCPort = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.txtIrcId = new System.Windows.Forms.TextBox();
            this.chkIrcId = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.irc_cbTitles = new System.Windows.Forms.CheckBox();
            this.pageServer = new System.Windows.Forms.TabPage();
            this.grpLevels = new System.Windows.Forms.GroupBox();
            this.label27 = new System.Windows.Forms.Label();
            this.txtMain = new System.Windows.Forms.TextBox();
            this.grpAdvanced = new System.Windows.Forms.GroupBox();
            this.editTxtsBt = new System.Windows.Forms.Button();
            this.chkRestart = new System.Windows.Forms.CheckBox();
            this.grpGeneral = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtServerOwner = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ChkPort = new System.Windows.Forms.Button();
            this.grpUpdate = new System.Windows.Forms.GroupBox();
            this.forceUpdateBtn = new System.Windows.Forms.Button();
            this.updateTimeNumeric = new System.Windows.Forms.NumericUpDown();
            this.lblUpdateSeconds = new System.Windows.Forms.Label();
            this.notifyInGameUpdate = new System.Windows.Forms.CheckBox();
            this.autoUpdate = new System.Windows.Forms.CheckBox();
            this.chkUpdates = new System.Windows.Forms.CheckBox();
            this.grpPlayers = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.numPlayers = new System.Windows.Forms.NumericUpDown();
            this.chkAgreeToRules = new System.Windows.Forms.CheckBox();
            this.label35 = new System.Windows.Forms.Label();
            this.numGuests = new System.Windows.Forms.NumericUpDown();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.pageGames = new System.Windows.Forms.TabPage();
            this.tabGames = new System.Windows.Forms.TabControl();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.lsBtnEndVote = new System.Windows.Forms.Button();
            this.lsBtnEndRound = new System.Windows.Forms.Button();
            this.lsBtnStopGame = new System.Windows.Forms.Button();
            this.lsBtnStartGame = new System.Windows.Forms.Button();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.pg_lavaMap = new System.Windows.Forms.PropertyGrid();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.pg_lava = new System.Windows.Forms.PropertyGrid();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.label54 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.lsAddMap = new System.Windows.Forms.Button();
            this.lsRemoveMap = new System.Windows.Forms.Button();
            this.lsMapNoUse = new System.Windows.Forms.ListBox();
            this.lsMapUse = new System.Windows.Forms.ListBox();
            this.tabPage14 = new System.Windows.Forms.TabPage();
            this.SlctdTntWrsPlyrs = new System.Windows.Forms.TextBox();
            this.label87 = new System.Windows.Forms.Label();
            this.SlctdTntWrdStatus = new System.Windows.Forms.TextBox();
            this.label86 = new System.Windows.Forms.Label();
            this.SlctdTntWrsLvl = new System.Windows.Forms.TextBox();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.groupBox36 = new System.Windows.Forms.GroupBox();
            this.TntWrsStreaksChck = new System.Windows.Forms.CheckBox();
            this.groupBox35 = new System.Windows.Forms.GroupBox();
            this.TntWrsMpsList = new System.Windows.Forms.ListBox();
            this.TntWrsCrtNwTntWrsBt = new System.Windows.Forms.Button();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.TntWrsDiffSlctBt = new System.Windows.Forms.Button();
            this.TntWrsDiffAboutBt = new System.Windows.Forms.Button();
            this.TntWrsDiffCombo = new System.Windows.Forms.ComboBox();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.TntWrsTKchck = new System.Windows.Forms.CheckBox();
            this.TntWrsBlnceTeamsChck = new System.Windows.Forms.CheckBox();
            this.TntWrsTmsChck = new System.Windows.Forms.CheckBox();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.label90 = new System.Windows.Forms.Label();
            this.TntWrsGraceTimeChck = new System.Windows.Forms.NumericUpDown();
            this.TntWrsGracePrdChck = new System.Windows.Forms.CheckBox();
            this.groupBox31 = new System.Windows.Forms.GroupBox();
            this.TntWrsMltiKlChck = new System.Windows.Forms.CheckBox();
            this.TntWrsMltiKlScPrUpDown = new System.Windows.Forms.NumericUpDown();
            this.TntWrsAsstChck = new System.Windows.Forms.CheckBox();
            this.TntWrsAstsScrUpDwn = new System.Windows.Forms.NumericUpDown();
            this.label89 = new System.Windows.Forms.Label();
            this.TntWrsScrPrKlUpDwn = new System.Windows.Forms.NumericUpDown();
            this.label88 = new System.Windows.Forms.Label();
            this.TntWrsScrLmtUpDwn = new System.Windows.Forms.NumericUpDown();
            this.groupBox30 = new System.Windows.Forms.GroupBox();
            this.TntWrsStrtGame = new System.Windows.Forms.Button();
            this.TntWrsDltGame = new System.Windows.Forms.Button();
            this.TntWrsEndGame = new System.Windows.Forms.Button();
            this.TntWrsRstGame = new System.Windows.Forms.Button();
            this.EditTntWarsGameBT = new System.Windows.Forms.Button();
            this.TntWarsGamesList = new System.Windows.Forms.ListBox();
            this.tabPage11 = new System.Windows.Forms.TabPage();
            this.propsZG = new System.Windows.Forms.PropertyGrid();
            this.pageSecurity = new System.Windows.Forms.TabPage();
            this.sec_gbBlocks = new System.Windows.Forms.GroupBox();
            this.sec_cbBlocksAuto = new System.Windows.Forms.CheckBox();
            this.sec_lblBlocksOnMute = new System.Windows.Forms.Label();
            this.sec_numBlocksMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblBlocksOnMsgs = new System.Windows.Forms.Label();
            this.sec_numBlocksSecs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblBlocksOnSecs = new System.Windows.Forms.Label();
            this.sec_gbCmd = new System.Windows.Forms.GroupBox();
            this.sec_cbCmdAuto = new System.Windows.Forms.CheckBox();
            this.sec_lblCmdOnMute = new System.Windows.Forms.Label();
            this.sec_numCmdMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblCmdOnMsgs = new System.Windows.Forms.Label();
            this.sec_numCmdSecs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblCmdOnSecs = new System.Windows.Forms.Label();
            this.sec_lblCmdForMute = new System.Windows.Forms.Label();
            this.sec_numCmdMute = new System.Windows.Forms.NumericUpDown();
            this.sec_lblCmdForSecs = new System.Windows.Forms.Label();
            this.sec_gbOther = new System.Windows.Forms.GroupBox();
            this.sec_lblRank = new System.Windows.Forms.Label();
            this.sec_gbChat = new System.Windows.Forms.GroupBox();
            this.sec_lblChatOnMute = new System.Windows.Forms.Label();
            this.sec_numChatMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblChatOnMsgs = new System.Windows.Forms.Label();
            this.sec_numChatSecs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblChatOnSecs = new System.Windows.Forms.Label();
            this.sec_lblChatForMute = new System.Windows.Forms.Label();
            this.sec_numChatMute = new System.Windows.Forms.NumericUpDown();
            this.sec_lblChatForSecs = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabChat.SuspendLayout();
            this.chat_gbTab.SuspendLayout();
            this.chat_gbMessages.SuspendLayout();
            this.chat_gbOther.SuspendLayout();
            this.chat_gbColors.SuspendLayout();
            this.pageCommands.SuspendLayout();
            this.pageCommandsList.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.pageCommandsCustom.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pageCommandPerms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.extracmdpermnumber)).BeginInit();
            this.pageBlocks.SuspendLayout();
            this.pageRanks.SuspendLayout();
            this.gbRankGeneral.SuspendLayout();
            this.gbRankSettings.SuspendLayout();
            this.pageMisc.SuspendLayout();
            this.economyGroupBox.SuspendLayout();
            this.grpExtra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldownTime)).BeginInit();
            this.grpMessages.SuspendLayout();
            this.grpPhysics.SuspendLayout();
            this.grpAFK.SuspendLayout();
            this.grpBackups.SuspendLayout();
            this.pageIRC.SuspendLayout();
            this.grpSQL.SuspendLayout();
            this.grpIRC.SuspendLayout();
            this.pageServer.SuspendLayout();
            this.grpLevels.SuspendLayout();
            this.grpAdvanced.SuspendLayout();
            this.grpGeneral.SuspendLayout();
            this.grpUpdate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updateTimeNumeric)).BeginInit();
            this.grpPlayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGuests)).BeginInit();
            this.tabControl.SuspendLayout();
            this.pageGames.SuspendLayout();
            this.tabGames.SuspendLayout();
            this.tabPage10.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.tabPage14.SuspendLayout();
            this.groupBox29.SuspendLayout();
            this.groupBox36.SuspendLayout();
            this.groupBox35.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.groupBox33.SuspendLayout();
            this.groupBox32.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsGraceTimeChck)).BeginInit();
            this.groupBox31.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsMltiKlScPrUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsAstsScrUpDwn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsScrPrKlUpDwn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsScrLmtUpDwn)).BeginInit();
            this.groupBox30.SuspendLayout();
            this.tabPage11.SuspendLayout();
            this.pageSecurity.SuspendLayout();
            this.sec_gbBlocks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numBlocksMsgs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numBlocksSecs)).BeginInit();
            this.sec_gbCmd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdMsgs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdSecs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdMute)).BeginInit();
            this.sec_gbOther.SuspendLayout();
            this.sec_gbChat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatMsgs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatSecs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatMute)).BeginInit();
            this.SuspendLayout();
            // 
            // tabChat
            // 
            this.tabChat.BackColor = System.Drawing.SystemColors.Control;
            this.tabChat.Controls.Add(this.chat_gbTab);
            this.tabChat.Controls.Add(this.chat_gbMessages);
            this.tabChat.Controls.Add(this.chat_gbOther);
            this.tabChat.Controls.Add(this.chat_gbColors);
            this.tabChat.Location = new System.Drawing.Point(4, 22);
            this.tabChat.Name = "tabChat";
            this.tabChat.Padding = new System.Windows.Forms.Padding(3);
            this.tabChat.Size = new System.Drawing.Size(498, 521);
            this.tabChat.TabIndex = 10;
            this.tabChat.Text = "Chat";
            // 
            // chat_gbTab
            // 
            this.chat_gbTab.Controls.Add(this.chat_cbTabRank);
            this.chat_gbTab.Controls.Add(this.chat_cbTabLevel);
            this.chat_gbTab.Controls.Add(this.chat_cbTabBots);
            this.chat_gbTab.Location = new System.Drawing.Point(235, 59);
            this.chat_gbTab.Name = "chat_gbTab";
            this.chat_gbTab.Size = new System.Drawing.Size(256, 92);
            this.chat_gbTab.TabIndex = 3;
            this.chat_gbTab.TabStop = false;
            this.chat_gbTab.Text = "Tab list";
            // 
            // chat_cbTabRank
            // 
            this.chat_cbTabRank.AutoSize = true;
            this.chat_cbTabRank.Location = new System.Drawing.Point(6, 19);
            this.chat_cbTabRank.Name = "chat_cbTabRank";
            this.chat_cbTabRank.Size = new System.Drawing.Size(116, 17);
            this.chat_cbTabRank.TabIndex = 31;
            this.chat_cbTabRank.Text = "Sort tab list by rank";
            this.chat_cbTabRank.UseVisualStyleBackColor = true;
            // 
            // chat_cbTabLevel
            // 
            this.chat_cbTabLevel.AutoSize = true;
            this.chat_cbTabLevel.Location = new System.Drawing.Point(6, 44);
            this.chat_cbTabLevel.Name = "chat_cbTabLevel";
            this.chat_cbTabLevel.Size = new System.Drawing.Size(108, 17);
            this.chat_cbTabLevel.TabIndex = 30;
            this.chat_cbTabLevel.Text = "Level only tab list";
            this.chat_cbTabLevel.UseVisualStyleBackColor = true;
            // 
            // chat_cbTabBots
            // 
            this.chat_cbTabBots.AutoSize = true;
            this.chat_cbTabBots.Location = new System.Drawing.Point(6, 69);
            this.chat_cbTabBots.Name = "chat_cbTabBots";
            this.chat_cbTabBots.Size = new System.Drawing.Size(120, 17);
            this.chat_cbTabBots.TabIndex = 32;
            this.chat_cbTabBots.Text = "Show bots in tab list";
            this.chat_cbTabBots.UseVisualStyleBackColor = true;
            // 
            // chat_gbMessages
            // 
            this.chat_gbMessages.Controls.Add(this.chat_lblDemote);
            this.chat_gbMessages.Controls.Add(this.chat_lblPromote);
            this.chat_gbMessages.Controls.Add(this.chat_lblBan);
            this.chat_gbMessages.Controls.Add(this.chat_txtDemote);
            this.chat_gbMessages.Controls.Add(this.chat_txtPromote);
            this.chat_gbMessages.Controls.Add(this.chat_lblShutdown);
            this.chat_gbMessages.Controls.Add(this.chat_txtBan);
            this.chat_gbMessages.Controls.Add(this.chat_txtCheap);
            this.chat_gbMessages.Controls.Add(this.chat_chkCheap);
            this.chat_gbMessages.Controls.Add(this.chat_txtShutdown);
            this.chat_gbMessages.Location = new System.Drawing.Point(8, 157);
            this.chat_gbMessages.Name = "chat_gbMessages";
            this.chat_gbMessages.Size = new System.Drawing.Size(483, 180);
            this.chat_gbMessages.TabIndex = 2;
            this.chat_gbMessages.TabStop = false;
            this.chat_gbMessages.Text = "Messages";
            // 
            // chat_lblDemote
            // 
            this.chat_lblDemote.AutoSize = true;
            this.chat_lblDemote.Location = new System.Drawing.Point(6, 147);
            this.chat_lblDemote.Name = "chat_lblDemote";
            this.chat_lblDemote.Size = new System.Drawing.Size(119, 13);
            this.chat_lblDemote.TabIndex = 41;
            this.chat_lblDemote.Text = "Default demote reason:";
            // 
            // chat_lblPromote
            // 
            this.chat_lblPromote.AutoSize = true;
            this.chat_lblPromote.Location = new System.Drawing.Point(6, 113);
            this.chat_lblPromote.Name = "chat_lblPromote";
            this.chat_lblPromote.Size = new System.Drawing.Size(123, 13);
            this.chat_lblPromote.TabIndex = 40;
            this.chat_lblPromote.Text = "Default promote reason:";
            // 
            // chat_lblBan
            // 
            this.chat_lblBan.AutoSize = true;
            this.chat_lblBan.Location = new System.Drawing.Point(6, 83);
            this.chat_lblBan.Name = "chat_lblBan";
            this.chat_lblBan.Size = new System.Drawing.Size(100, 13);
            this.chat_lblBan.TabIndex = 39;
            this.chat_lblBan.Text = "Default ban reason:";
            // 
            // chat_txtDemote
            // 
            this.chat_txtDemote.Location = new System.Drawing.Point(134, 144);
            this.chat_txtDemote.MaxLength = 64;
            this.chat_txtDemote.Name = "chat_txtDemote";
            this.chat_txtDemote.Size = new System.Drawing.Size(343, 21);
            this.chat_txtDemote.TabIndex = 38;
            // 
            // chat_txtPromote
            // 
            this.chat_txtPromote.Location = new System.Drawing.Point(134, 110);
            this.chat_txtPromote.MaxLength = 64;
            this.chat_txtPromote.Name = "chat_txtPromote";
            this.chat_txtPromote.Size = new System.Drawing.Size(343, 21);
            this.chat_txtPromote.TabIndex = 36;
            // 
            // chat_lblShutdown
            // 
            this.chat_lblShutdown.AutoSize = true;
            this.chat_lblShutdown.Location = new System.Drawing.Point(6, 23);
            this.chat_lblShutdown.Name = "chat_lblShutdown";
            this.chat_lblShutdown.Size = new System.Drawing.Size(101, 13);
            this.chat_lblShutdown.TabIndex = 34;
            this.chat_lblShutdown.Text = "Shutdown message:";
            // 
            // chat_txtBan
            // 
            this.chat_txtBan.Location = new System.Drawing.Point(134, 80);
            this.chat_txtBan.MaxLength = 64;
            this.chat_txtBan.Name = "chat_txtBan";
            this.chat_txtBan.Size = new System.Drawing.Size(343, 21);
            this.chat_txtBan.TabIndex = 33;
            // 
            // chat_txtCheap
            // 
            this.chat_txtCheap.Location = new System.Drawing.Point(134, 50);
            this.chat_txtCheap.Name = "chat_txtCheap";
            this.chat_txtCheap.Size = new System.Drawing.Size(343, 21);
            this.chat_txtCheap.TabIndex = 31;
            // 
            // chat_chkCheap
            // 
            this.chat_chkCheap.AutoSize = true;
            this.chat_chkCheap.Location = new System.Drawing.Point(9, 52);
            this.chat_chkCheap.Name = "chat_chkCheap";
            this.chat_chkCheap.Size = new System.Drawing.Size(123, 17);
            this.chat_chkCheap.TabIndex = 30;
            this.chat_chkCheap.Text = "/invincible message:";
            this.toolTip.SetToolTip(this.chat_chkCheap, "Is immortality cheap and unfair?");
            this.chat_chkCheap.UseVisualStyleBackColor = true;
            // 
            // chat_txtShutdown
            // 
            this.chat_txtShutdown.Location = new System.Drawing.Point(134, 20);
            this.chat_txtShutdown.MaxLength = 128;
            this.chat_txtShutdown.Name = "chat_txtShutdown";
            this.chat_txtShutdown.Size = new System.Drawing.Size(343, 21);
            this.chat_txtShutdown.TabIndex = 29;
            // 
            // chat_gbOther
            // 
            this.chat_gbOther.Controls.Add(this.chat_lblConsole);
            this.chat_gbOther.Controls.Add(this.chat_txtConsole);
            this.chat_gbOther.Location = new System.Drawing.Point(235, 6);
            this.chat_gbOther.Name = "chat_gbOther";
            this.chat_gbOther.Size = new System.Drawing.Size(256, 47);
            this.chat_gbOther.TabIndex = 1;
            this.chat_gbOther.TabStop = false;
            this.chat_gbOther.Text = "Other";
            // 
            // chat_lblConsole
            // 
            this.chat_lblConsole.AutoSize = true;
            this.chat_lblConsole.Location = new System.Drawing.Point(6, 20);
            this.chat_lblConsole.Name = "chat_lblConsole";
            this.chat_lblConsole.Size = new System.Drawing.Size(77, 13);
            this.chat_lblConsole.TabIndex = 4;
            this.chat_lblConsole.Text = "Console name:";
            // 
            // chat_txtConsole
            // 
            this.chat_txtConsole.Location = new System.Drawing.Point(105, 17);
            this.chat_txtConsole.Name = "chat_txtConsole";
            this.chat_txtConsole.Size = new System.Drawing.Size(145, 21);
            this.chat_txtConsole.TabIndex = 3;
            this.chat_txtConsole.TextChanged += new System.EventHandler(this.txtPort_TextChanged);
            // 
            // chat_gbColors
            // 
            this.chat_gbColors.Controls.Add(this.chat_colDesc);
            this.chat_gbColors.Controls.Add(this.chat_cmbDesc);
            this.chat_gbColors.Controls.Add(this.chat_lblDesc);
            this.chat_gbColors.Controls.Add(this.chat_colSyntax);
            this.chat_gbColors.Controls.Add(this.chat_cmbSyntax);
            this.chat_gbColors.Controls.Add(this.chat_lblSyntax);
            this.chat_gbColors.Controls.Add(this.chat_colIRC);
            this.chat_gbColors.Controls.Add(this.chat_cmbIRC);
            this.chat_gbColors.Controls.Add(this.chat_lblIRC);
            this.chat_gbColors.Controls.Add(this.chat_colDefault);
            this.chat_gbColors.Controls.Add(this.chat_cmbDefault);
            this.chat_gbColors.Controls.Add(this.chat_lblDefault);
            this.chat_gbColors.Location = new System.Drawing.Point(8, 6);
            this.chat_gbColors.Name = "chat_gbColors";
            this.chat_gbColors.Size = new System.Drawing.Size(221, 145);
            this.chat_gbColors.TabIndex = 0;
            this.chat_gbColors.TabStop = false;
            this.chat_gbColors.Text = "Colors";
            // 
            // chat_colDesc
            // 
            this.chat_colDesc.Location = new System.Drawing.Point(99, 98);
            this.chat_colDesc.Name = "chat_colDesc";
            this.chat_colDesc.Size = new System.Drawing.Size(18, 18);
            this.chat_colDesc.TabIndex = 34;
            // 
            // chat_cmbDesc
            // 
            this.chat_cmbDesc.FormattingEnabled = true;
            this.chat_cmbDesc.Location = new System.Drawing.Point(120, 98);
            this.chat_cmbDesc.Name = "chat_cmbDesc";
            this.chat_cmbDesc.Size = new System.Drawing.Size(95, 21);
            this.chat_cmbDesc.TabIndex = 33;
            this.toolTip.SetToolTip(this.chat_cmbDesc, "The colour of the /cmdname [args] in /help.");
            this.chat_cmbDesc.SelectedIndexChanged += new System.EventHandler(this.chat_cmbDesc_SelectedIndexChanged);
            // 
            // chat_lblDesc
            // 
            this.chat_lblDesc.AutoSize = true;
            this.chat_lblDesc.Location = new System.Drawing.Point(6, 101);
            this.chat_lblDesc.Name = "chat_lblDesc";
            this.chat_lblDesc.Size = new System.Drawing.Size(90, 13);
            this.chat_lblDesc.TabIndex = 32;
            this.chat_lblDesc.Text = "/help description:";
            // 
            // chat_colSyntax
            // 
            this.chat_colSyntax.Location = new System.Drawing.Point(99, 71);
            this.chat_colSyntax.Name = "chat_colSyntax";
            this.chat_colSyntax.Size = new System.Drawing.Size(18, 18);
            this.chat_colSyntax.TabIndex = 35;
            // 
            // chat_cmbSyntax
            // 
            this.chat_cmbSyntax.FormattingEnabled = true;
            this.chat_cmbSyntax.Location = new System.Drawing.Point(120, 71);
            this.chat_cmbSyntax.Name = "chat_cmbSyntax";
            this.chat_cmbSyntax.Size = new System.Drawing.Size(95, 21);
            this.chat_cmbSyntax.TabIndex = 30;
            this.toolTip.SetToolTip(this.chat_cmbSyntax, "The colour for the description of a command in /help.");
            this.chat_cmbSyntax.SelectedIndexChanged += new System.EventHandler(this.chat_cmbSyntax_SelectedIndexChanged);
            // 
            // chat_lblSyntax
            // 
            this.chat_lblSyntax.AutoSize = true;
            this.chat_lblSyntax.Location = new System.Drawing.Point(6, 74);
            this.chat_lblSyntax.Name = "chat_lblSyntax";
            this.chat_lblSyntax.Size = new System.Drawing.Size(68, 13);
            this.chat_lblSyntax.TabIndex = 31;
            this.chat_lblSyntax.Text = "/help syntax:";
            // 
            // chat_colIRC
            // 
            this.chat_colIRC.BackColor = System.Drawing.Color.Black;
            this.chat_colIRC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chat_colIRC.Location = new System.Drawing.Point(99, 44);
            this.chat_colIRC.Name = "chat_colIRC";
            this.chat_colIRC.Size = new System.Drawing.Size(18, 18);
            this.chat_colIRC.TabIndex = 25;
            // 
            // chat_cmbIRC
            // 
            this.chat_cmbIRC.FormattingEnabled = true;
            this.chat_cmbIRC.Location = new System.Drawing.Point(120, 44);
            this.chat_cmbIRC.Name = "chat_cmbIRC";
            this.chat_cmbIRC.Size = new System.Drawing.Size(95, 21);
            this.chat_cmbIRC.TabIndex = 24;
            this.toolTip.SetToolTip(this.chat_cmbIRC, "The colour of the IRC nicks used in the IRC.");
            this.chat_cmbIRC.SelectedIndexChanged += new System.EventHandler(this.chat_cmbIRC_SelectedIndexChanged);
            // 
            // chat_lblIRC
            // 
            this.chat_lblIRC.AutoSize = true;
            this.chat_lblIRC.Location = new System.Drawing.Point(6, 47);
            this.chat_lblIRC.Name = "chat_lblIRC";
            this.chat_lblIRC.Size = new System.Drawing.Size(74, 13);
            this.chat_lblIRC.TabIndex = 22;
            this.chat_lblIRC.Text = "IRC messages:";
            // 
            // chat_colDefault
            // 
            this.chat_colDefault.BackColor = System.Drawing.Color.Black;
            this.chat_colDefault.Location = new System.Drawing.Point(99, 15);
            this.chat_colDefault.Name = "chat_colDefault";
            this.chat_colDefault.Size = new System.Drawing.Size(18, 18);
            this.chat_colDefault.TabIndex = 26;
            // 
            // chat_cmbDefault
            // 
            this.chat_cmbDefault.FormattingEnabled = true;
            this.chat_cmbDefault.Location = new System.Drawing.Point(120, 15);
            this.chat_cmbDefault.Name = "chat_cmbDefault";
            this.chat_cmbDefault.Size = new System.Drawing.Size(95, 21);
            this.chat_cmbDefault.TabIndex = 10;
            this.toolTip.SetToolTip(this.chat_cmbDefault, "The colour of the default chat used in the server.\nFor example, when you are aske" +
                        "d to select two corners in a cuboid.");
            this.chat_cmbDefault.SelectedIndexChanged += new System.EventHandler(this.chat_cmbDefault_SelectedIndexChanged);
            // 
            // chat_lblDefault
            // 
            this.chat_lblDefault.AutoSize = true;
            this.chat_lblDefault.Location = new System.Drawing.Point(6, 18);
            this.chat_lblDefault.Name = "chat_lblDefault";
            this.chat_lblDefault.Size = new System.Drawing.Size(71, 13);
            this.chat_lblDefault.TabIndex = 11;
            this.chat_lblDefault.Text = "Default color:";
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(346, 553);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDiscard
            // 
            this.btnDiscard.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDiscard.Location = new System.Drawing.Point(4, 553);
            this.btnDiscard.Name = "btnDiscard";
            this.btnDiscard.Size = new System.Drawing.Size(75, 23);
            this.btnDiscard.TabIndex = 1;
            this.btnDiscard.Text = "Discard";
            this.btnDiscard.UseVisualStyleBackColor = true;
            this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
            // 
            // btnApply
            // 
            this.btnApply.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(427, 553);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 8000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Information";
            this.toolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip_Popup);
            // 
            // pageCommands
            // 
            this.pageCommands.AutoScroll = true;
            this.pageCommands.Controls.Add(this.pageCommandsList);
            this.pageCommands.Location = new System.Drawing.Point(4, 22);
            this.pageCommands.Name = "pageCommands";
            this.pageCommands.Size = new System.Drawing.Size(498, 521);
            this.pageCommands.TabIndex = 2;
            this.pageCommands.Text = "Commands";
            this.toolTip.SetToolTip(this.pageCommands, "Which ranks can use which commands.");
            // 
            // pageCommandsList
            // 
            this.pageCommandsList.Controls.Add(this.tabPage6);
            this.pageCommandsList.Controls.Add(this.pageCommandsCustom);
            this.pageCommandsList.Controls.Add(this.pageCommandPerms);
            this.pageCommandsList.Location = new System.Drawing.Point(9, 4);
            this.pageCommandsList.Name = "pageCommandsList";
            this.pageCommandsList.SelectedIndex = 0;
            this.pageCommandsList.Size = new System.Drawing.Size(476, 502);
            this.pageCommandsList.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage6.Controls.Add(this.txtCmdRanks);
            this.tabPage6.Controls.Add(this.btnCmdHelp);
            this.tabPage6.Controls.Add(this.txtCmdAllow);
            this.tabPage6.Controls.Add(this.txtCmdLowest);
            this.tabPage6.Controls.Add(this.txtCmdDisallow);
            this.tabPage6.Controls.Add(this.label17);
            this.tabPage6.Controls.Add(this.label15);
            this.tabPage6.Controls.Add(this.label8);
            this.tabPage6.Controls.Add(this.listCommands);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(468, 476);
            this.tabPage6.TabIndex = 0;
            this.tabPage6.Text = "Commands";
            // 
            // txtCmdRanks
            // 
            this.txtCmdRanks.Location = new System.Drawing.Point(9, 109);
            this.txtCmdRanks.Multiline = true;
            this.txtCmdRanks.Name = "txtCmdRanks";
            this.txtCmdRanks.ReadOnly = true;
            this.txtCmdRanks.Size = new System.Drawing.Size(225, 339);
            this.txtCmdRanks.TabIndex = 33;
            // 
            // btnCmdHelp
            // 
            this.btnCmdHelp.Location = new System.Drawing.Point(253, 419);
            this.btnCmdHelp.Name = "btnCmdHelp";
            this.btnCmdHelp.Size = new System.Drawing.Size(141, 29);
            this.btnCmdHelp.TabIndex = 34;
            this.btnCmdHelp.Text = "Help information";
            this.btnCmdHelp.UseVisualStyleBackColor = true;
            this.btnCmdHelp.Click += new System.EventHandler(this.btnCmdHelp_Click);
            // 
            // txtCmdAllow
            // 
            this.txtCmdAllow.Location = new System.Drawing.Point(108, 82);
            this.txtCmdAllow.Name = "txtCmdAllow";
            this.txtCmdAllow.Size = new System.Drawing.Size(86, 21);
            this.txtCmdAllow.TabIndex = 31;
            this.txtCmdAllow.LostFocus += new System.EventHandler(this.txtCmdAllow_TextChanged);
            // 
            // txtCmdLowest
            // 
            this.txtCmdLowest.Location = new System.Drawing.Point(108, 28);
            this.txtCmdLowest.Name = "txtCmdLowest";
            this.txtCmdLowest.Size = new System.Drawing.Size(86, 21);
            this.txtCmdLowest.TabIndex = 32;
            this.txtCmdLowest.LostFocus += new System.EventHandler(this.txtCmdLowest_TextChanged);
            // 
            // txtCmdDisallow
            // 
            this.txtCmdDisallow.Location = new System.Drawing.Point(108, 55);
            this.txtCmdDisallow.Name = "txtCmdDisallow";
            this.txtCmdDisallow.Size = new System.Drawing.Size(86, 21);
            this.txtCmdDisallow.TabIndex = 30;
            this.txtCmdDisallow.LostFocus += new System.EventHandler(this.txtCmdDisallow_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(52, 85);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(56, 13);
            this.label17.TabIndex = 29;
            this.label17.Text = "And allow:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(28, 58);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 13);
            this.label15.TabIndex = 28;
            this.label15.Text = "But don\'t allow:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Lowest rank needed:";
            // 
            // listCommands
            // 
            this.listCommands.FormattingEnabled = true;
            this.listCommands.Location = new System.Drawing.Point(253, 19);
            this.listCommands.Name = "listCommands";
            this.listCommands.Size = new System.Drawing.Size(141, 368);
            this.listCommands.TabIndex = 26;
            this.listCommands.SelectedIndexChanged += new System.EventHandler(this.listCommands_SelectedIndexChanged);
            // 
            // pageCommandsCustom
            // 
            this.pageCommandsCustom.BackColor = System.Drawing.SystemColors.Control;
            this.pageCommandsCustom.Controls.Add(this.lblLoadedCommands);
            this.pageCommandsCustom.Controls.Add(this.lstCommands);
            this.pageCommandsCustom.Controls.Add(this.groupBox24);
            this.pageCommandsCustom.Controls.Add(this.btnUnload);
            this.pageCommandsCustom.Controls.Add(this.btnLoad);
            this.pageCommandsCustom.Location = new System.Drawing.Point(4, 22);
            this.pageCommandsCustom.Name = "pageCommandsCustom";
            this.pageCommandsCustom.Padding = new System.Windows.Forms.Padding(3);
            this.pageCommandsCustom.Size = new System.Drawing.Size(468, 476);
            this.pageCommandsCustom.TabIndex = 1;
            this.pageCommandsCustom.Text = "Custom commands";
            // 
            // lblLoadedCommands
            // 
            this.lblLoadedCommands.AutoSize = true;
            this.lblLoadedCommands.Location = new System.Drawing.Point(7, 120);
            this.lblLoadedCommands.Name = "lblLoadedCommands";
            this.lblLoadedCommands.Size = new System.Drawing.Size(96, 13);
            this.lblLoadedCommands.TabIndex = 40;
            this.lblLoadedCommands.Text = "Loaded commands";
            // 
            // lstCommands
            // 
            this.lstCommands.FormattingEnabled = true;
            this.lstCommands.Location = new System.Drawing.Point(7, 139);
            this.lstCommands.Name = "lstCommands";
            this.lstCommands.Size = new System.Drawing.Size(450, 303);
            this.lstCommands.TabIndex = 39;
            this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.panel1);
            this.groupBox24.Controls.Add(this.btnCreate);
            this.groupBox24.Controls.Add(this.txtCommandName);
            this.groupBox24.Controls.Add(this.label33);
            this.groupBox24.Location = new System.Drawing.Point(6, 6);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(459, 100);
            this.groupBox24.TabIndex = 38;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Quick command";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(13, 58);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(84, 29);
            this.panel1.TabIndex = 37;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.Location = new System.Drawing.Point(41, 6);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(36, 16);
            this.radioButton1.TabIndex = 27;
            this.radioButton1.Text = "VB";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.Location = new System.Drawing.Point(2, 6);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(35, 16);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "C#";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreate.Location = new System.Drawing.Point(299, 64);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(149, 23);
            this.btnCreate.TabIndex = 29;
            this.btnCreate.Text = "Create command";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // txtCommandName
            // 
            this.txtCommandName.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommandName.Location = new System.Drawing.Point(93, 20);
            this.txtCommandName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtCommandName.Name = "txtCommandName";
            this.txtCommandName.Size = new System.Drawing.Size(355, 18);
            this.txtCommandName.TabIndex = 27;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(11, 23);
            this.label33.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(78, 12);
            this.label33.TabIndex = 28;
            this.label33.Text = "Command Name:";
            // 
            // btnUnload
            // 
            this.btnUnload.Enabled = false;
            this.btnUnload.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnload.Location = new System.Drawing.Point(374, 447);
            this.btnUnload.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(83, 23);
            this.btnUnload.TabIndex = 32;
            this.btnUnload.Text = "Unload";
            this.btnUnload.UseVisualStyleBackColor = true;
            this.btnUnload.Click += new System.EventHandler(this.btnUnload_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.Location = new System.Drawing.Point(254, 447);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(116, 23);
            this.btnLoad.TabIndex = 31;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // pageCommandPerms
            // 
            this.pageCommandPerms.BackColor = System.Drawing.SystemColors.Control;
            this.pageCommandPerms.Controls.Add(this.txtcmdranks2);
            this.pageCommandPerms.Controls.Add(this.label74);
            this.pageCommandPerms.Controls.Add(this.label73);
            this.pageCommandPerms.Controls.Add(this.extracmdpermnumber);
            this.pageCommandPerms.Controls.Add(this.label72);
            this.pageCommandPerms.Controls.Add(this.extracmdpermdesc);
            this.pageCommandPerms.Controls.Add(this.extracmdpermperm);
            this.pageCommandPerms.Controls.Add(this.listCommandsExtraCmdPerms);
            this.pageCommandPerms.Location = new System.Drawing.Point(4, 22);
            this.pageCommandPerms.Name = "pageCommandPerms";
            this.pageCommandPerms.Size = new System.Drawing.Size(468, 476);
            this.pageCommandPerms.TabIndex = 2;
            this.pageCommandPerms.Text = "Extra command permissions";
            // 
            // txtcmdranks2
            // 
            this.txtcmdranks2.Location = new System.Drawing.Point(72, 189);
            this.txtcmdranks2.Multiline = true;
            this.txtcmdranks2.Name = "txtcmdranks2";
            this.txtcmdranks2.ReadOnly = true;
            this.txtcmdranks2.Size = new System.Drawing.Size(241, 273);
            this.txtcmdranks2.TabIndex = 46;
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(6, 70);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(64, 13);
            this.label74.TabIndex = 45;
            this.label74.Text = "Description:";
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(3, 18);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(128, 13);
            this.label73.TabIndex = 44;
            this.label73.Text = "Extra permission number:";
            // 
            // extracmdpermnumber
            // 
            this.extracmdpermnumber.Location = new System.Drawing.Point(188, 16);
            this.extracmdpermnumber.Maximum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.extracmdpermnumber.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.extracmdpermnumber.Name = "extracmdpermnumber";
            this.extracmdpermnumber.Size = new System.Drawing.Size(120, 21);
            this.extracmdpermnumber.TabIndex = 43;
            this.extracmdpermnumber.Value = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.extracmdpermnumber.ValueChanged += new System.EventHandler(this.extracmdpermnumber_ValueChanged);
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Location = new System.Drawing.Point(7, 46);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(63, 13);
            this.label72.TabIndex = 42;
            this.label72.Text = "Permission:";
            // 
            // extracmdpermdesc
            // 
            this.extracmdpermdesc.Location = new System.Drawing.Point(72, 70);
            this.extracmdpermdesc.Multiline = true;
            this.extracmdpermdesc.Name = "extracmdpermdesc";
            this.extracmdpermdesc.ReadOnly = true;
            this.extracmdpermdesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.extracmdpermdesc.Size = new System.Drawing.Size(241, 113);
            this.extracmdpermdesc.TabIndex = 41;
            // 
            // extracmdpermperm
            // 
            this.extracmdpermperm.Location = new System.Drawing.Point(72, 43);
            this.extracmdpermperm.Name = "extracmdpermperm";
            this.extracmdpermperm.Size = new System.Drawing.Size(116, 21);
            this.extracmdpermperm.TabIndex = 40;
            // 
            // listCommandsExtraCmdPerms
            // 
            this.listCommandsExtraCmdPerms.FormattingEnabled = true;
            this.listCommandsExtraCmdPerms.Location = new System.Drawing.Point(319, 16);
            this.listCommandsExtraCmdPerms.Name = "listCommandsExtraCmdPerms";
            this.listCommandsExtraCmdPerms.Size = new System.Drawing.Size(141, 446);
            this.listCommandsExtraCmdPerms.TabIndex = 27;
            this.listCommandsExtraCmdPerms.SelectedIndexChanged += new System.EventHandler(this.listCommandsExtraCmdPerms_SelectedIndexChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(5, 52);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(48, 13);
            this.label24.TabIndex = 15;
            this.label24.Text = "/rp limit:";
            this.toolTip.SetToolTip(this.label24, "Limit for custom physics set by /rp");
            // 
            // chkPhysicsRest
            // 
            this.chkPhysicsRest.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkPhysicsRest.Location = new System.Drawing.Point(8, 20);
            this.chkPhysicsRest.Name = "chkPhysicsRest";
            this.chkPhysicsRest.Size = new System.Drawing.Size(119, 23);
            this.chkPhysicsRest.TabIndex = 22;
            this.chkPhysicsRest.Text = "Restart physics";
            this.chkPhysicsRest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.chkPhysicsRest, "Restart physics on shutdown, clearing all physics blocks.");
            this.chkPhysicsRest.UseVisualStyleBackColor = true;
            // 
            // chkDeath
            // 
            this.chkDeath.AutoSize = true;
            this.chkDeath.Location = new System.Drawing.Point(6, 20);
            this.chkDeath.Name = "chkDeath";
            this.chkDeath.Size = new System.Drawing.Size(84, 17);
            this.chkDeath.TabIndex = 21;
            this.chkDeath.Text = "Death count";
            this.toolTip.SetToolTip(this.chkDeath, "\"Bob has died 10 times.\"");
            this.chkDeath.UseVisualStyleBackColor = true;
            // 
            // txtBackup
            // 
            this.txtBackup.Location = new System.Drawing.Point(81, 43);
            this.txtBackup.Name = "txtBackup";
            this.txtBackup.Size = new System.Drawing.Size(41, 21);
            this.txtBackup.TabIndex = 5;
            this.toolTip.SetToolTip(this.txtBackup, "How often should backups be taken, in seconds.\nDefault = 300");
            // 
            // txtafk
            // 
            this.txtafk.Location = new System.Drawing.Point(61, 16);
            this.txtafk.Name = "txtafk";
            this.txtafk.Size = new System.Drawing.Size(66, 21);
            this.txtafk.TabIndex = 10;
            this.toolTip.SetToolTip(this.txtafk, "How long the server should wait before declaring someone ask afk. (0 = No timer a" +
                        "t all)");
            // 
            // txtAFKKick
            // 
            this.txtAFKKick.Location = new System.Drawing.Point(61, 43);
            this.txtAFKKick.Name = "txtAFKKick";
            this.txtAFKKick.Size = new System.Drawing.Size(66, 21);
            this.txtAFKKick.TabIndex = 9;
            this.toolTip.SetToolTip(this.txtAFKKick, "Kick the user after they have been afk for this many minutes (0 = No kick)");
            // 
            // hackrank_kick
            // 
            this.hackrank_kick.AutoSize = true;
            this.hackrank_kick.Location = new System.Drawing.Point(7, 20);
            this.hackrank_kick.Name = "hackrank_kick";
            this.hackrank_kick.Size = new System.Drawing.Size(193, 17);
            this.hackrank_kick.TabIndex = 32;
            this.hackrank_kick.Text = "Kick people who use hackrank after ";
            this.toolTip.SetToolTip(this.hackrank_kick, "Hackrank kicks people? Or not?");
            this.hackrank_kick.UseVisualStyleBackColor = true;
            // 
            // chkIRC
            // 
            this.chkIRC.AutoSize = true;
            this.chkIRC.Location = new System.Drawing.Point(9, 20);
            this.chkIRC.Name = "chkIRC";
            this.chkIRC.Size = new System.Drawing.Size(61, 17);
            this.chkIRC.TabIndex = 22;
            this.chkIRC.Text = "Use IRC";
            this.toolTip.SetToolTip(this.chkIRC, "Whether to use the IRC bot or not.\nIRC stands for Internet Relay Chat and allows " +
                        "for communication with the server while outside Minecraft.");
            this.chkIRC.UseVisualStyleBackColor = true;
            this.chkIRC.CheckedChanged += new System.EventHandler(this.chkIRC_CheckedChanged);
            // 
            // txtNick
            // 
            this.txtNick.Location = new System.Drawing.Point(82, 101);
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(106, 21);
            this.txtNick.TabIndex = 16;
            this.toolTip.SetToolTip(this.txtNick, "The Nick that the IRC bot will try and use.");
            // 
            // txtIRCServer
            // 
            this.txtIRCServer.Location = new System.Drawing.Point(82, 47);
            this.txtIRCServer.Name = "txtIRCServer";
            this.txtIRCServer.Size = new System.Drawing.Size(106, 21);
            this.txtIRCServer.TabIndex = 15;
            this.toolTip.SetToolTip(this.txtIRCServer, "The IRC server to be used.\nDefault = irc.geekshed.net\nAnother choice = irc.esper." +
                        "netd.net");
            // 
            // txtChannel
            // 
            this.txtChannel.Location = new System.Drawing.Point(82, 128);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(106, 21);
            this.txtChannel.TabIndex = 17;
            this.toolTip.SetToolTip(this.txtChannel, "The IRC channel to be used.");
            // 
            // txtOpChannel
            // 
            this.txtOpChannel.Location = new System.Drawing.Point(82, 155);
            this.txtOpChannel.Name = "txtOpChannel";
            this.txtOpChannel.Size = new System.Drawing.Size(106, 21);
            this.txtOpChannel.TabIndex = 26;
            this.toolTip.SetToolTip(this.txtOpChannel, "The IRC channel to be used.");
            // 
            // chkVerify
            // 
            this.chkVerify.AutoSize = true;
            this.chkVerify.Location = new System.Drawing.Point(9, 20);
            this.chkVerify.Name = "chkVerify";
            this.chkVerify.Size = new System.Drawing.Size(87, 17);
            this.chkVerify.TabIndex = 4;
            this.chkVerify.Text = "Verify Names";
            this.toolTip.SetToolTip(this.chkVerify, "Make sure the user is who they claim to be.");
            this.chkVerify.UseVisualStyleBackColor = true;
            // 
            // chkWorld
            // 
            this.chkWorld.AutoSize = true;
            this.chkWorld.Location = new System.Drawing.Point(9, 72);
            this.chkWorld.Name = "chkWorld";
            this.chkWorld.Size = new System.Drawing.Size(105, 17);
            this.chkWorld.TabIndex = 4;
            this.chkWorld.Text = "Server-wide chat";
            this.toolTip.SetToolTip(this.chkWorld, "If disabled, every map has isolated chat.\nIf enabled, every map is able to commun" +
                        "icate without special letters.");
            this.chkWorld.UseVisualStyleBackColor = true;
            // 
            // chkAutoload
            // 
            this.chkAutoload.AutoSize = true;
            this.chkAutoload.Location = new System.Drawing.Point(9, 49);
            this.chkAutoload.Name = "chkAutoload";
            this.chkAutoload.Size = new System.Drawing.Size(90, 17);
            this.chkAutoload.TabIndex = 4;
            this.chkAutoload.Text = "Load on /goto";
            this.toolTip.SetToolTip(this.chkAutoload, "Load a map when a user wishes to go to it, and unload empty maps");
            this.chkAutoload.UseVisualStyleBackColor = true;
            // 
            // chkPublic
            // 
            this.chkPublic.AutoSize = true;
            this.chkPublic.Location = new System.Drawing.Point(9, 124);
            this.chkPublic.Name = "chkPublic";
            this.chkPublic.Size = new System.Drawing.Size(55, 17);
            this.chkPublic.TabIndex = 4;
            this.chkPublic.Text = "Public";
            this.toolTip.SetToolTip(this.chkPublic, "Whether or not the server will appear on the server list.");
            this.chkPublic.UseVisualStyleBackColor = true;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(83, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(387, 21);
            this.txtName.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtName, "The name of the server.\nPick something good!");
            // 
            // txtMOTD
            // 
            this.txtMOTD.Location = new System.Drawing.Point(83, 46);
            this.txtMOTD.Name = "txtMOTD";
            this.txtMOTD.Size = new System.Drawing.Size(387, 21);
            this.txtMOTD.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtMOTD, "The MOTD of the server.\nUse \"+hax\" to allow any WoM hack, \"-hax\" to disallow any " +
                        "hacks at all and use \"-fly\" and whatnot to disallow other things.");
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(83, 73);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(63, 21);
            this.txtPort.TabIndex = 2;
            this.toolTip.SetToolTip(this.txtPort, "The port that the server will output on.\nDefault = 25565\n\nChanging will reset you" +
                        "r ExternalURL.");
            this.txtPort.TextChanged += new System.EventHandler(this.txtPort_TextChanged);
            // 
            // chkLogBeat
            // 
            this.chkLogBeat.AutoSize = true;
            this.chkLogBeat.Location = new System.Drawing.Point(9, 68);
            this.chkLogBeat.Name = "chkLogBeat";
            this.chkLogBeat.Size = new System.Drawing.Size(98, 17);
            this.chkLogBeat.TabIndex = 24;
            this.chkLogBeat.Text = "Log Heartbeat?";
            this.toolTip.SetToolTip(this.chkLogBeat, "Debugging feature -- Toggles whether to log heartbeat activity.\r\nUseful when your" +
                        " server gets a URL slowly or not at all.");
            this.chkLogBeat.UseVisualStyleBackColor = true;
            // 
            // chkUseSQL
            // 
            this.chkUseSQL.AutoSize = true;
            this.chkUseSQL.Location = new System.Drawing.Point(12, 20);
            this.chkUseSQL.Name = "chkUseSQL";
            this.chkUseSQL.Size = new System.Drawing.Size(77, 17);
            this.chkUseSQL.TabIndex = 28;
            this.chkUseSQL.Tag = "Whether or not the use of MySQL is enabled. You will need to have installed it fo" +
            "r this to work. MySQL includes features such as block tracking, colors, titles a" +
            "nd player info.";
            this.chkUseSQL.Text = "Use MySQL";
            this.toolTip.SetToolTip(this.chkUseSQL, "Whether to use the IRC bot or not.\nIRC stands for Internet Relay Chat and allows " +
                        "for communication with the server while outside Minecraft.");
            this.chkUseSQL.UseVisualStyleBackColor = true;
            this.chkUseSQL.CheckedChanged += new System.EventHandler(this.chkUseSQL_CheckedChanged);
            // 
            // sec_cmbVerifyRank
            // 
            this.sec_cmbVerifyRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sec_cmbVerifyRank.FormattingEnabled = true;
            this.sec_cmbVerifyRank.Location = new System.Drawing.Point(72, 95);
            this.sec_cmbVerifyRank.Name = "sec_cmbVerifyRank";
            this.sec_cmbVerifyRank.Size = new System.Drawing.Size(103, 21);
            this.sec_cmbVerifyRank.TabIndex = 22;
            this.toolTip.SetToolTip(this.sec_cmbVerifyRank, "Minimum rank that is required to use /pass before they can use commands.");
            // 
            // sec_cbVerifyAdmins
            // 
            this.sec_cbVerifyAdmins.AutoSize = true;
            this.sec_cbVerifyAdmins.Location = new System.Drawing.Point(15, 74);
            this.sec_cbVerifyAdmins.Name = "sec_cbVerifyAdmins";
            this.sec_cbVerifyAdmins.Size = new System.Drawing.Size(111, 17);
            this.sec_cbVerifyAdmins.TabIndex = 23;
            this.sec_cbVerifyAdmins.Text = "Admin verification";
            this.toolTip.SetToolTip(this.sec_cbVerifyAdmins, "If enabled, admins must verify with /pass before they can use commands.");
            this.sec_cbVerifyAdmins.UseVisualStyleBackColor = true;
            this.sec_cbVerifyAdmins.CheckedChanged += new System.EventHandler(this.VerifyAdminsChecked);
            // 
            // cmbAFKKickPerm
            // 
            this.cmbAFKKickPerm.FormattingEnabled = true;
            this.cmbAFKKickPerm.Location = new System.Drawing.Point(61, 71);
            this.cmbAFKKickPerm.Name = "cmbAFKKickPerm";
            this.cmbAFKKickPerm.Size = new System.Drawing.Size(66, 21);
            this.cmbAFKKickPerm.TabIndex = 46;
            this.toolTip.SetToolTip(this.cmbAFKKickPerm, "Maximum rank that will be kicked by AFK kick.");
            // 
            // chkGuestLimitNotify
            // 
            this.chkGuestLimitNotify.AutoSize = true;
            this.chkGuestLimitNotify.Location = new System.Drawing.Point(6, 134);
            this.chkGuestLimitNotify.Name = "chkGuestLimitNotify";
            this.chkGuestLimitNotify.Size = new System.Drawing.Size(109, 17);
            this.chkGuestLimitNotify.TabIndex = 46;
            this.chkGuestLimitNotify.Text = "Guest Limit Notify";
            this.toolTip.SetToolTip(this.chkGuestLimitNotify, "Notify in-game if a guest can\'t join due to the guest limit being reached.");
            this.chkGuestLimitNotify.UseVisualStyleBackColor = true;
            // 
            // cmbAdminChat
            // 
            this.cmbAdminChat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdminChat.FormattingEnabled = true;
            this.cmbAdminChat.Location = new System.Drawing.Point(104, 74);
            this.cmbAdminChat.Name = "cmbAdminChat";
            this.cmbAdminChat.Size = new System.Drawing.Size(81, 21);
            this.cmbAdminChat.TabIndex = 38;
            this.toolTip.SetToolTip(this.cmbAdminChat, "Default rank required to read op chat.");
            // 
            // cmbOpChat
            // 
            this.cmbOpChat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOpChat.FormattingEnabled = true;
            this.cmbOpChat.Location = new System.Drawing.Point(104, 47);
            this.cmbOpChat.Name = "cmbOpChat";
            this.cmbOpChat.Size = new System.Drawing.Size(81, 21);
            this.cmbOpChat.TabIndex = 36;
            this.toolTip.SetToolTip(this.cmbOpChat, "Default rank required to read op chat.");
            // 
            // chkTpToHigherRanks
            // 
            this.chkTpToHigherRanks.AutoSize = true;
            this.chkTpToHigherRanks.Location = new System.Drawing.Point(11, 142);
            this.chkTpToHigherRanks.Name = "chkTpToHigherRanks";
            this.chkTpToHigherRanks.Size = new System.Drawing.Size(136, 17);
            this.chkTpToHigherRanks.TabIndex = 42;
            this.chkTpToHigherRanks.Text = "Allow tp to higher ranks";
            this.toolTip.SetToolTip(this.chkTpToHigherRanks, "Allows the use of /tp to players of higher rank");
            this.chkTpToHigherRanks.UseVisualStyleBackColor = true;
            // 
            // cmbDefaultRank
            // 
            this.cmbDefaultRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultRank.FormattingEnabled = true;
            this.cmbDefaultRank.Location = new System.Drawing.Point(104, 20);
            this.cmbDefaultRank.Name = "cmbDefaultRank";
            this.cmbDefaultRank.Size = new System.Drawing.Size(81, 21);
            this.cmbDefaultRank.TabIndex = 44;
            this.toolTip.SetToolTip(this.cmbDefaultRank, "Default rank assigned to new visitors to the server.");
            // 
            // cmbOsMap
            // 
            this.cmbOsMap.FormattingEnabled = true;
            this.cmbOsMap.Location = new System.Drawing.Point(143, 207);
            this.cmbOsMap.Name = "cmbOsMap";
            this.cmbOsMap.Size = new System.Drawing.Size(172, 21);
            this.cmbOsMap.TabIndex = 47;
            this.toolTip.SetToolTip(this.cmbOsMap, "Default min rank that can build on maps made with /os map add.\nIf \'nobody\' is sel" +
                        "ected, the default min rank used is the min rank that can use /os.");
            // 
            // sec_cbWhitelist
            // 
            this.sec_cbWhitelist.Location = new System.Drawing.Point(15, 44);
            this.sec_cbWhitelist.Name = "sec_cbWhitelist";
            this.sec_cbWhitelist.Size = new System.Drawing.Size(104, 24);
            this.sec_cbWhitelist.TabIndex = 23;
            this.sec_cbWhitelist.Text = "Use whitelist";
            this.toolTip.SetToolTip(this.sec_cbWhitelist, "If enabled, only players who have been whitelisted with /whitelist are allowed to" +
                        " join");
            this.sec_cbWhitelist.UseVisualStyleBackColor = true;
            // 
            // sec_cbLogNotes
            // 
            this.sec_cbLogNotes.AutoSize = true;
            this.sec_cbLogNotes.Location = new System.Drawing.Point(15, 20);
            this.sec_cbLogNotes.Name = "sec_cbLogNotes";
            this.sec_cbLogNotes.Size = new System.Drawing.Size(178, 17);
            this.sec_cbLogNotes.TabIndex = 22;
            this.sec_cbLogNotes.Text = "Log notes (/ban, /warn, /kick etc)";
            this.sec_cbLogNotes.UseVisualStyleBackColor = true;
            // 
            // sec_cbChatAuto
            // 
            this.sec_cbChatAuto.AutoSize = true;
            this.sec_cbChatAuto.Location = new System.Drawing.Point(10, 20);
            this.sec_cbChatAuto.Name = "sec_cbChatAuto";
            this.sec_cbChatAuto.Size = new System.Drawing.Size(142, 17);
            this.sec_cbChatAuto.TabIndex = 24;
            this.sec_cbChatAuto.Text = "Enable automatic muting";
            this.sec_cbChatAuto.UseVisualStyleBackColor = true;
            this.sec_cbChatAuto.CheckedChanged += new System.EventHandler(this.sec_cbChatAuto_Checked);
            // 
            // pageBlocks
            // 
            this.pageBlocks.BackColor = System.Drawing.SystemColors.Control;
            this.pageBlocks.Controls.Add(this.btnBlHelp);
            this.pageBlocks.Controls.Add(this.txtBlRanks);
            this.pageBlocks.Controls.Add(this.txtBlAllow);
            this.pageBlocks.Controls.Add(this.txtBlLowest);
            this.pageBlocks.Controls.Add(this.txtBlDisallow);
            this.pageBlocks.Controls.Add(this.label18);
            this.pageBlocks.Controls.Add(this.label19);
            this.pageBlocks.Controls.Add(this.label20);
            this.pageBlocks.Controls.Add(this.listBlocks);
            this.pageBlocks.Location = new System.Drawing.Point(4, 22);
            this.pageBlocks.Name = "pageBlocks";
            this.pageBlocks.Padding = new System.Windows.Forms.Padding(3);
            this.pageBlocks.Size = new System.Drawing.Size(498, 521);
            this.pageBlocks.TabIndex = 5;
            this.pageBlocks.Text = "Blocks";
            // 
            // btnBlHelp
            // 
            this.btnBlHelp.Location = new System.Drawing.Point(253, 419);
            this.btnBlHelp.Name = "btnBlHelp";
            this.btnBlHelp.Size = new System.Drawing.Size(141, 29);
            this.btnBlHelp.TabIndex = 23;
            this.btnBlHelp.Text = "Help information";
            this.btnBlHelp.UseVisualStyleBackColor = true;
            this.btnBlHelp.Click += new System.EventHandler(this.btnBlHelp_Click);
            // 
            // txtBlRanks
            // 
            this.txtBlRanks.Location = new System.Drawing.Point(11, 122);
            this.txtBlRanks.Multiline = true;
            this.txtBlRanks.Name = "txtBlRanks";
            this.txtBlRanks.ReadOnly = true;
            this.txtBlRanks.Size = new System.Drawing.Size(225, 321);
            this.txtBlRanks.TabIndex = 22;
            // 
            // txtBlAllow
            // 
            this.txtBlAllow.Location = new System.Drawing.Point(113, 95);
            this.txtBlAllow.Name = "txtBlAllow";
            this.txtBlAllow.Size = new System.Drawing.Size(92, 21);
            this.txtBlAllow.TabIndex = 20;
            this.txtBlAllow.LostFocus += new System.EventHandler(this.txtBlAllow_TextChanged);
            // 
            // txtBlLowest
            // 
            this.txtBlLowest.Location = new System.Drawing.Point(113, 41);
            this.txtBlLowest.Name = "txtBlLowest";
            this.txtBlLowest.Size = new System.Drawing.Size(92, 21);
            this.txtBlLowest.TabIndex = 21;
            this.txtBlLowest.LostFocus += new System.EventHandler(this.txtBlLowest_TextChanged);
            // 
            // txtBlDisallow
            // 
            this.txtBlDisallow.Location = new System.Drawing.Point(113, 68);
            this.txtBlDisallow.Name = "txtBlDisallow";
            this.txtBlDisallow.Size = new System.Drawing.Size(92, 21);
            this.txtBlDisallow.TabIndex = 21;
            this.txtBlDisallow.LostFocus += new System.EventHandler(this.txtBlDisallow_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(57, 99);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(56, 13);
            this.label18.TabIndex = 18;
            this.label18.Text = "And allow:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(33, 72);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(80, 13);
            this.label19.TabIndex = 17;
            this.label19.Text = "But don\'t allow:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(8, 44);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(105, 13);
            this.label20.TabIndex = 16;
            this.label20.Text = "Lowest rank needed:";
            // 
            // listBlocks
            // 
            this.listBlocks.FormattingEnabled = true;
            this.listBlocks.Location = new System.Drawing.Point(253, 19);
            this.listBlocks.Name = "listBlocks";
            this.listBlocks.Size = new System.Drawing.Size(141, 368);
            this.listBlocks.TabIndex = 15;
            this.listBlocks.SelectedIndexChanged += new System.EventHandler(this.listBlocks_SelectedIndexChanged);
            // 
            // pageRanks
            // 
            this.pageRanks.BackColor = System.Drawing.SystemColors.Control;
            this.pageRanks.Controls.Add(this.gbRankGeneral);
            this.pageRanks.Controls.Add(this.gbRankSettings);
            this.pageRanks.Controls.Add(this.button1);
            this.pageRanks.Controls.Add(this.btnAddRank);
            this.pageRanks.Controls.Add(this.listRanks);
            this.pageRanks.Location = new System.Drawing.Point(4, 22);
            this.pageRanks.Name = "pageRanks";
            this.pageRanks.Padding = new System.Windows.Forms.Padding(3);
            this.pageRanks.Size = new System.Drawing.Size(498, 521);
            this.pageRanks.TabIndex = 4;
            this.pageRanks.Text = "Ranks";
            // 
            // gbRankGeneral
            // 
            this.gbRankGeneral.AutoSize = true;
            this.gbRankGeneral.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbRankGeneral.Controls.Add(this.label29);
            this.gbRankGeneral.Controls.Add(this.cmbDefaultRank);
            this.gbRankGeneral.Controls.Add(this.chkAdminsJoinSilent);
            this.gbRankGeneral.Controls.Add(this.chkTpToHigherRanks);
            this.gbRankGeneral.Controls.Add(this.lblOpChat);
            this.gbRankGeneral.Controls.Add(this.cmbAdminChat);
            this.gbRankGeneral.Controls.Add(this.cmbOpChat);
            this.gbRankGeneral.Controls.Add(this.label37);
            this.gbRankGeneral.Location = new System.Drawing.Point(11, 317);
            this.gbRankGeneral.Name = "gbRankGeneral";
            this.gbRankGeneral.Size = new System.Drawing.Size(191, 179);
            this.gbRankGeneral.TabIndex = 19;
            this.gbRankGeneral.TabStop = false;
            this.gbRankGeneral.Text = "General settings";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(30, 23);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(68, 13);
            this.label29.TabIndex = 43;
            this.label29.Text = "Default rank:";
            // 
            // chkAdminsJoinSilent
            // 
            this.chkAdminsJoinSilent.AutoSize = true;
            this.chkAdminsJoinSilent.Location = new System.Drawing.Point(11, 119);
            this.chkAdminsJoinSilent.Name = "chkAdminsJoinSilent";
            this.chkAdminsJoinSilent.Size = new System.Drawing.Size(118, 17);
            this.chkAdminsJoinSilent.TabIndex = 41;
            this.chkAdminsJoinSilent.Tag = "Players who have the adminchat rank join the game silently.";
            this.chkAdminsJoinSilent.Text = "Admins join silently";
            this.chkAdminsJoinSilent.UseVisualStyleBackColor = true;
            // 
            // lblOpChat
            // 
            this.lblOpChat.AutoSize = true;
            this.lblOpChat.Location = new System.Drawing.Point(28, 50);
            this.lblOpChat.Name = "lblOpChat";
            this.lblOpChat.Size = new System.Drawing.Size(70, 13);
            this.lblOpChat.TabIndex = 35;
            this.lblOpChat.Text = "Op Chat rank:";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(11, 77);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(87, 13);
            this.label37.TabIndex = 37;
            this.label37.Text = "Admin Chat rank:";
            // 
            // gbRankSettings
            // 
            this.gbRankSettings.AutoSize = true;
            this.gbRankSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbRankSettings.Controls.Add(this.label22);
            this.gbRankSettings.Controls.Add(this.label10);
            this.gbRankSettings.Controls.Add(this.txtOSMaps);
            this.gbRankSettings.Controls.Add(this.txtPrefix);
            this.gbRankSettings.Controls.Add(this.txtLimit);
            this.gbRankSettings.Controls.Add(this.label12);
            this.gbRankSettings.Controls.Add(this.lblColor);
            this.gbRankSettings.Controls.Add(this.txtGrpMOTD);
            this.gbRankSettings.Controls.Add(this.txtPermission);
            this.gbRankSettings.Controls.Add(this.label14);
            this.gbRankSettings.Controls.Add(this.txtRankName);
            this.gbRankSettings.Controls.Add(this.cmbColor);
            this.gbRankSettings.Controls.Add(this.txtFileName);
            this.gbRankSettings.Controls.Add(this.lblMOTD);
            this.gbRankSettings.Controls.Add(this.label11);
            this.gbRankSettings.Controls.Add(this.label13);
            this.gbRankSettings.Controls.Add(this.label16);
            this.gbRankSettings.Controls.Add(this.txtMaxUndo);
            this.gbRankSettings.Controls.Add(this.label52);
            this.gbRankSettings.Location = new System.Drawing.Point(11, 16);
            this.gbRankSettings.Name = "gbRankSettings";
            this.gbRankSettings.Size = new System.Drawing.Size(194, 291);
            this.gbRankSettings.TabIndex = 18;
            this.gbRankSettings.TabStop = false;
            this.gbRankSettings.Text = "Rank settings";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(40, 253);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(37, 13);
            this.label22.TabIndex = 20;
            this.label22.Text = "Prefix:";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(26, 199);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "/os maps:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOSMaps
            // 
            this.txtOSMaps.Location = new System.Drawing.Point(85, 196);
            this.txtOSMaps.Name = "txtOSMaps";
            this.txtOSMaps.Size = new System.Drawing.Size(100, 21);
            this.txtOSMaps.TabIndex = 19;
            this.txtOSMaps.TextChanged += new System.EventHandler(this.txtOSMaps_TextChanged);
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(85, 250);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(100, 21);
            this.txtPrefix.TabIndex = 21;
            this.txtPrefix.TextChanged += new System.EventHandler(this.txtPrefix_TextChanged);
            // 
            // txtLimit
            // 
            this.txtLimit.Location = new System.Drawing.Point(85, 74);
            this.txtLimit.Name = "txtLimit";
            this.txtLimit.Size = new System.Drawing.Size(100, 21);
            this.txtLimit.TabIndex = 4;
            this.txtLimit.TextChanged += new System.EventHandler(this.txtLimit_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 50);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Permission:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblColor
            // 
            this.lblColor.Location = new System.Drawing.Point(170, 128);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(18, 18);
            this.lblColor.TabIndex = 13;
            // 
            // txtGrpMOTD
            // 
            this.txtGrpMOTD.Location = new System.Drawing.Point(85, 155);
            this.txtGrpMOTD.Name = "txtGrpMOTD";
            this.txtGrpMOTD.Size = new System.Drawing.Size(100, 21);
            this.txtGrpMOTD.TabIndex = 17;
            this.txtGrpMOTD.TextChanged += new System.EventHandler(this.txtGrpMOTD_TextChanged);
            // 
            // txtPermission
            // 
            this.txtPermission.Location = new System.Drawing.Point(85, 47);
            this.txtPermission.Name = "txtPermission";
            this.txtPermission.Size = new System.Drawing.Size(100, 21);
            this.txtPermission.TabIndex = 6;
            this.txtPermission.TextChanged += new System.EventHandler(this.txtPermission_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(23, 225);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "Filename:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRankName
            // 
            this.txtRankName.Location = new System.Drawing.Point(85, 20);
            this.txtRankName.Name = "txtRankName";
            this.txtRankName.Size = new System.Drawing.Size(100, 21);
            this.txtRankName.TabIndex = 5;
            this.txtRankName.TextChanged += new System.EventHandler(this.txtRankName_TextChanged);
            // 
            // cmbColor
            // 
            this.cmbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColor.FormattingEnabled = true;
            this.cmbColor.Location = new System.Drawing.Point(85, 128);
            this.cmbColor.Name = "cmbColor";
            this.cmbColor.Size = new System.Drawing.Size(79, 21);
            this.cmbColor.TabIndex = 12;
            this.cmbColor.SelectedIndexChanged += new System.EventHandler(this.cmbColor_SelectedIndexChanged);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(85, 223);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(100, 21);
            this.txtFileName.TabIndex = 4;
            this.txtFileName.TextChanged += new System.EventHandler(this.txtFileName_TextChanged);
            // 
            // lblMOTD
            // 
            this.lblMOTD.AutoSize = true;
            this.lblMOTD.Location = new System.Drawing.Point(39, 159);
            this.lblMOTD.Name = "lblMOTD";
            this.lblMOTD.Size = new System.Drawing.Size(38, 13);
            this.lblMOTD.TabIndex = 16;
            this.lblMOTD.Text = "MOTD:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(39, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(38, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Name:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 77);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "Draw limit:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(42, 131);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(35, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "Color:";
            // 
            // txtMaxUndo
            // 
            this.txtMaxUndo.Location = new System.Drawing.Point(85, 101);
            this.txtMaxUndo.Name = "txtMaxUndo";
            this.txtMaxUndo.Size = new System.Drawing.Size(100, 21);
            this.txtMaxUndo.TabIndex = 15;
            this.txtMaxUndo.TextChanged += new System.EventHandler(this.txtMaxUndo_TextChanged);
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(20, 104);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(57, 13);
            this.label52.TabIndex = 14;
            this.label52.Text = "Max Undo:";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(315, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(57, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Del";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAddRank
            // 
            this.btnAddRank.Location = new System.Drawing.Point(243, 6);
            this.btnAddRank.Name = "btnAddRank";
            this.btnAddRank.Size = new System.Drawing.Size(57, 23);
            this.btnAddRank.TabIndex = 1;
            this.btnAddRank.Text = "Add";
            this.btnAddRank.UseVisualStyleBackColor = true;
            this.btnAddRank.Click += new System.EventHandler(this.btnAddRank_Click);
            // 
            // listRanks
            // 
            this.listRanks.FormattingEnabled = true;
            this.listRanks.Location = new System.Drawing.Point(230, 35);
            this.listRanks.Name = "listRanks";
            this.listRanks.Size = new System.Drawing.Size(161, 368);
            this.listRanks.TabIndex = 0;
            this.listRanks.SelectedIndexChanged += new System.EventHandler(this.listRanks_SelectedIndexChanged);
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(6, 9);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(35, 13);
            this.label85.TabIndex = 5;
            this.label85.Text = "Level:";
            // 
            // pageMisc
            // 
            this.pageMisc.BackColor = System.Drawing.SystemColors.Control;
            this.pageMisc.Controls.Add(this.economyGroupBox);
            this.pageMisc.Controls.Add(this.grpExtra);
            this.pageMisc.Controls.Add(this.grpMessages);
            this.pageMisc.Controls.Add(this.grpPhysics);
            this.pageMisc.Controls.Add(this.grpAFK);
            this.pageMisc.Controls.Add(this.grpBackups);
            this.pageMisc.Controls.Add(this.chkProfanityFilter);
            this.pageMisc.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageMisc.Location = new System.Drawing.Point(4, 22);
            this.pageMisc.Name = "pageMisc";
            this.pageMisc.Size = new System.Drawing.Size(498, 521);
            this.pageMisc.TabIndex = 3;
            this.pageMisc.Text = "Misc";
            // 
            // economyGroupBox
            // 
            this.economyGroupBox.Controls.Add(this.buttonEco);
            this.economyGroupBox.Location = new System.Drawing.Point(352, 248);
            this.economyGroupBox.Name = "economyGroupBox";
            this.economyGroupBox.Size = new System.Drawing.Size(133, 144);
            this.economyGroupBox.TabIndex = 44;
            this.economyGroupBox.TabStop = false;
            this.economyGroupBox.Text = "Economy";
            // 
            // buttonEco
            // 
            this.buttonEco.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonEco.Location = new System.Drawing.Point(8, 17);
            this.buttonEco.Name = "buttonEco";
            this.buttonEco.Size = new System.Drawing.Size(119, 23);
            this.buttonEco.TabIndex = 43;
            this.buttonEco.Text = "Economy Settings";
            this.buttonEco.UseVisualStyleBackColor = true;
            this.buttonEco.Click += new System.EventHandler(this.buttonEco_Click);
            // 
            // grpExtra
            // 
            this.grpExtra.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpExtra.Controls.Add(this.nudCooldownTime);
            this.grpExtra.Controls.Add(this.label84);
            this.grpExtra.Controls.Add(this.lblOsMap);
            this.grpExtra.Controls.Add(this.cmbOsMap);
            this.grpExtra.Controls.Add(this.chkGuestLimitNotify);
            this.grpExtra.Controls.Add(this.chkShowEmptyRanks);
            this.grpExtra.Controls.Add(this.chkRepeatMessages);
            this.grpExtra.Controls.Add(this.chkDeath);
            this.grpExtra.Controls.Add(this.txtRestartTime);
            this.grpExtra.Controls.Add(this.txtMoneys);
            this.grpExtra.Controls.Add(this.chkRestartTime);
            this.grpExtra.Controls.Add(this.chk17Dollar);
            this.grpExtra.Controls.Add(this.chkSmile);
            this.grpExtra.Controls.Add(this.label34);
            this.grpExtra.Location = new System.Drawing.Point(10, 158);
            this.grpExtra.Name = "grpExtra";
            this.grpExtra.Size = new System.Drawing.Size(332, 270);
            this.grpExtra.TabIndex = 40;
            this.grpExtra.TabStop = false;
            this.grpExtra.Text = "Extra";
            // 
            // nudCooldownTime
            // 
            this.nudCooldownTime.Location = new System.Drawing.Point(143, 234);
            this.nudCooldownTime.Maximum = new decimal(new int[] {
                                    86400,
                                    0,
                                    0,
                                    0});
            this.nudCooldownTime.Name = "nudCooldownTime";
            this.nudCooldownTime.Size = new System.Drawing.Size(57, 21);
            this.nudCooldownTime.TabIndex = 50;
            this.nudCooldownTime.Value = new decimal(new int[] {
                                    600,
                                    0,
                                    0,
                                    0});
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(23, 238);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(115, 13);
            this.label84.TabIndex = 49;
            this.label84.Text = "Review cooldown time:";
            // 
            // lblOsMap
            // 
            this.lblOsMap.AutoSize = true;
            this.lblOsMap.Location = new System.Drawing.Point(31, 210);
            this.lblOsMap.Name = "lblOsMap";
            this.lblOsMap.Size = new System.Drawing.Size(103, 13);
            this.lblOsMap.TabIndex = 48;
            this.lblOsMap.Text = "/os default perbuild:";
            // 
            // chkShowEmptyRanks
            // 
            this.chkShowEmptyRanks.AutoSize = true;
            this.chkShowEmptyRanks.Location = new System.Drawing.Point(6, 66);
            this.chkShowEmptyRanks.Name = "chkShowEmptyRanks";
            this.chkShowEmptyRanks.Size = new System.Drawing.Size(135, 17);
            this.chkShowEmptyRanks.TabIndex = 41;
            this.chkShowEmptyRanks.Text = "Empty ranks in /players";
            this.chkShowEmptyRanks.UseVisualStyleBackColor = true;
            // 
            // chkRepeatMessages
            // 
            this.chkRepeatMessages.AutoSize = true;
            this.chkRepeatMessages.Location = new System.Drawing.Point(6, 111);
            this.chkRepeatMessages.Name = "chkRepeatMessages";
            this.chkRepeatMessages.Size = new System.Drawing.Size(136, 17);
            this.chkRepeatMessages.TabIndex = 29;
            this.chkRepeatMessages.Text = "Repeat message blocks";
            this.chkRepeatMessages.UseVisualStyleBackColor = true;
            // 
            // txtRestartTime
            // 
            this.txtRestartTime.Location = new System.Drawing.Point(143, 155);
            this.txtRestartTime.Name = "txtRestartTime";
            this.txtRestartTime.Size = new System.Drawing.Size(172, 21);
            this.txtRestartTime.TabIndex = 1;
            this.txtRestartTime.Text = "HH: mm: ss";
            // 
            // txtMoneys
            // 
            this.txtMoneys.Location = new System.Drawing.Point(143, 180);
            this.txtMoneys.Name = "txtMoneys";
            this.txtMoneys.Size = new System.Drawing.Size(172, 21);
            this.txtMoneys.TabIndex = 1;
            // 
            // chkRestartTime
            // 
            this.chkRestartTime.AutoSize = true;
            this.chkRestartTime.Location = new System.Drawing.Point(6, 157);
            this.chkRestartTime.Name = "chkRestartTime";
            this.chkRestartTime.Size = new System.Drawing.Size(131, 17);
            this.chkRestartTime.TabIndex = 0;
            this.chkRestartTime.Text = "Restart server at time:";
            this.chkRestartTime.UseVisualStyleBackColor = true;
            // 
            // chk17Dollar
            // 
            this.chk17Dollar.AutoSize = true;
            this.chk17Dollar.Location = new System.Drawing.Point(6, 90);
            this.chk17Dollar.Name = "chk17Dollar";
            this.chk17Dollar.Size = new System.Drawing.Size(100, 17);
            this.chk17Dollar.TabIndex = 22;
            this.chk17Dollar.Text = "$ before $name";
            this.chk17Dollar.UseVisualStyleBackColor = true;
            // 
            // chkSmile
            // 
            this.chkSmile.AutoSize = true;
            this.chkSmile.Location = new System.Drawing.Point(6, 43);
            this.chkSmile.Name = "chkSmile";
            this.chkSmile.Size = new System.Drawing.Size(91, 17);
            this.chkSmile.TabIndex = 19;
            this.chkSmile.Text = "Parse emotes";
            this.chkSmile.UseVisualStyleBackColor = true;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(63, 183);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(71, 13);
            this.label34.TabIndex = 11;
            this.label34.Text = "Money name:";
            // 
            // grpMessages
            // 
            this.grpMessages.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpMessages.Controls.Add(this.hackrank_kick);
            this.grpMessages.Controls.Add(this.hackrank_kick_time);
            this.grpMessages.Controls.Add(this.label36);
            this.grpMessages.Location = new System.Drawing.Point(10, 103);
            this.grpMessages.Name = "grpMessages";
            this.grpMessages.Size = new System.Drawing.Size(332, 49);
            this.grpMessages.TabIndex = 39;
            this.grpMessages.TabStop = false;
            this.grpMessages.Text = "Messages";
            // 
            // hackrank_kick_time
            // 
            this.hackrank_kick_time.Location = new System.Drawing.Point(201, 18);
            this.hackrank_kick_time.Name = "hackrank_kick_time";
            this.hackrank_kick_time.Size = new System.Drawing.Size(60, 21);
            this.hackrank_kick_time.TabIndex = 33;
            this.hackrank_kick_time.Text = "5";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(268, 21);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(46, 13);
            this.label36.TabIndex = 34;
            this.label36.Text = "Seconds";
            // 
            // grpPhysics
            // 
            this.grpPhysics.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpPhysics.Controls.Add(this.label24);
            this.grpPhysics.Controls.Add(this.txtRP);
            this.grpPhysics.Controls.Add(this.label28);
            this.grpPhysics.Controls.Add(this.txtNormRp);
            this.grpPhysics.Controls.Add(this.chkPhysicsRest);
            this.grpPhysics.Location = new System.Drawing.Point(352, 124);
            this.grpPhysics.Name = "grpPhysics";
            this.grpPhysics.Size = new System.Drawing.Size(133, 117);
            this.grpPhysics.TabIndex = 38;
            this.grpPhysics.TabStop = false;
            this.grpPhysics.Text = "Physics Restart";
            // 
            // txtRP
            // 
            this.txtRP.Location = new System.Drawing.Point(72, 49);
            this.txtRP.Name = "txtRP";
            this.txtRP.Size = new System.Drawing.Size(55, 21);
            this.txtRP.TabIndex = 14;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(5, 79);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(61, 13);
            this.label28.TabIndex = 16;
            this.label28.Text = "Normal /rp:";
            // 
            // txtNormRp
            // 
            this.txtNormRp.Location = new System.Drawing.Point(72, 76);
            this.txtNormRp.Name = "txtNormRp";
            this.txtNormRp.Size = new System.Drawing.Size(55, 21);
            this.txtNormRp.TabIndex = 13;
            // 
            // grpAFK
            // 
            this.grpAFK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpAFK.Controls.Add(this.cmbAFKKickPerm);
            this.grpAFK.Controls.Add(this.label76);
            this.grpAFK.Controls.Add(this.label25);
            this.grpAFK.Controls.Add(this.txtafk);
            this.grpAFK.Controls.Add(this.label26);
            this.grpAFK.Controls.Add(this.txtAFKKick);
            this.grpAFK.Location = new System.Drawing.Point(352, 13);
            this.grpAFK.Name = "grpAFK";
            this.grpAFK.Size = new System.Drawing.Size(133, 100);
            this.grpAFK.TabIndex = 37;
            this.grpAFK.TabStop = false;
            this.grpAFK.Text = "AFK";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(5, 74);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(54, 13);
            this.label76.TabIndex = 13;
            this.label76.Text = "Kick Rank:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(5, 21);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(54, 13);
            this.label25.TabIndex = 12;
            this.label25.Text = "AFK timer:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(5, 47);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(48, 13);
            this.label26.TabIndex = 11;
            this.label26.Text = "AFK Kick:";
            // 
            // grpBackups
            // 
            this.grpBackups.AutoSize = true;
            this.grpBackups.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpBackups.Controls.Add(this.label32);
            this.grpBackups.Controls.Add(this.txtBackupLocation);
            this.grpBackups.Controls.Add(this.label9);
            this.grpBackups.Controls.Add(this.txtBackup);
            this.grpBackups.Location = new System.Drawing.Point(10, 13);
            this.grpBackups.Name = "grpBackups";
            this.grpBackups.Size = new System.Drawing.Size(332, 84);
            this.grpBackups.TabIndex = 36;
            this.grpBackups.TabStop = false;
            this.grpBackups.Text = "Backups";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(5, 21);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(44, 13);
            this.label32.TabIndex = 3;
            this.label32.Text = "Backup:";
            // 
            // txtBackupLocation
            // 
            this.txtBackupLocation.Location = new System.Drawing.Point(81, 17);
            this.txtBackupLocation.Name = "txtBackupLocation";
            this.txtBackupLocation.Size = new System.Drawing.Size(245, 21);
            this.txtBackupLocation.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Backup time:";
            // 
            // chkProfanityFilter
            // 
            this.chkProfanityFilter.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkProfanityFilter.AutoSize = true;
            this.chkProfanityFilter.Location = new System.Drawing.Point(535, 325);
            this.chkProfanityFilter.Name = "chkProfanityFilter";
            this.chkProfanityFilter.Size = new System.Drawing.Size(87, 23);
            this.chkProfanityFilter.TabIndex = 30;
            this.chkProfanityFilter.Text = "Profanity Filter";
            this.chkProfanityFilter.UseVisualStyleBackColor = true;
            // 
            // pageIRC
            // 
            this.pageIRC.BackColor = System.Drawing.SystemColors.Control;
            this.pageIRC.Controls.Add(this.grpSQL);
            this.pageIRC.Controls.Add(this.grpIRC);
            this.pageIRC.Location = new System.Drawing.Point(4, 22);
            this.pageIRC.Name = "pageIRC";
            this.pageIRC.Size = new System.Drawing.Size(498, 521);
            this.pageIRC.TabIndex = 6;
            this.pageIRC.Text = "IRC/SQL";
            // 
            // grpSQL
            // 
            this.grpSQL.Controls.Add(this.txtSQLPort);
            this.grpSQL.Controls.Add(this.chkUseSQL);
            this.grpSQL.Controls.Add(this.label70);
            this.grpSQL.Controls.Add(this.linkLabel1);
            this.grpSQL.Controls.Add(this.txtSQLHost);
            this.grpSQL.Controls.Add(this.label43);
            this.grpSQL.Controls.Add(this.txtSQLDatabase);
            this.grpSQL.Controls.Add(this.label42);
            this.grpSQL.Controls.Add(this.label40);
            this.grpSQL.Controls.Add(this.label41);
            this.grpSQL.Controls.Add(this.txtSQLPassword);
            this.grpSQL.Controls.Add(this.txtSQLUsername);
            this.grpSQL.Location = new System.Drawing.Point(264, 3);
            this.grpSQL.Name = "grpSQL";
            this.grpSQL.Size = new System.Drawing.Size(227, 272);
            this.grpSQL.TabIndex = 29;
            this.grpSQL.TabStop = false;
            this.grpSQL.Text = "MySQL";
            // 
            // txtSQLPort
            // 
            this.txtSQLPort.Location = new System.Drawing.Point(111, 155);
            this.txtSQLPort.Name = "txtSQLPort";
            this.txtSQLPort.Size = new System.Drawing.Size(100, 21);
            this.txtSQLPort.TabIndex = 32;
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(9, 158);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(30, 13);
            this.label70.TabIndex = 31;
            this.label70.Text = "Port:";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(108, 21);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(113, 13);
            this.linkLabel1.TabIndex = 30;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Tag = "Click here to go to the download page for MySQL.";
            this.linkLabel1.Text = "MySQL Download Page";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // txtSQLHost
            // 
            this.txtSQLHost.Location = new System.Drawing.Point(111, 128);
            this.txtSQLHost.Name = "txtSQLHost";
            this.txtSQLHost.Size = new System.Drawing.Size(100, 21);
            this.txtSQLHost.TabIndex = 8;
            this.txtSQLHost.Tag = "The host name for the database. Leave this unless problems occur.";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(9, 131);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(32, 13);
            this.label43.TabIndex = 7;
            this.label43.Text = "Host:";
            // 
            // txtSQLDatabase
            // 
            this.txtSQLDatabase.Location = new System.Drawing.Point(111, 101);
            this.txtSQLDatabase.Name = "txtSQLDatabase";
            this.txtSQLDatabase.Size = new System.Drawing.Size(100, 21);
            this.txtSQLDatabase.TabIndex = 6;
            this.txtSQLDatabase.Tag = "The name of the database stored (Default = MCZall)";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(9, 104);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(86, 13);
            this.label42.TabIndex = 5;
            this.label42.Text = "Database Name:";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(9, 77);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(56, 13);
            this.label40.TabIndex = 4;
            this.label40.Text = "Password:";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(9, 50);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(59, 13);
            this.label41.TabIndex = 3;
            this.label41.Text = "Username:";
            // 
            // txtSQLPassword
            // 
            this.txtSQLPassword.Location = new System.Drawing.Point(111, 74);
            this.txtSQLPassword.Name = "txtSQLPassword";
            this.txtSQLPassword.PasswordChar = '*';
            this.txtSQLPassword.Size = new System.Drawing.Size(100, 21);
            this.txtSQLPassword.TabIndex = 2;
            this.txtSQLPassword.Tag = "The password set while installing MySQL";
            // 
            // txtSQLUsername
            // 
            this.txtSQLUsername.Location = new System.Drawing.Point(111, 47);
            this.txtSQLUsername.Name = "txtSQLUsername";
            this.txtSQLUsername.Size = new System.Drawing.Size(100, 21);
            this.txtSQLUsername.TabIndex = 1;
            this.txtSQLUsername.Tag = "The username set while installing MySQL";
            // 
            // grpIRC
            // 
            this.grpIRC.Controls.Add(this.txtIRCPort);
            this.grpIRC.Controls.Add(this.label50);
            this.grpIRC.Controls.Add(this.label49);
            this.grpIRC.Controls.Add(this.chkIRC);
            this.grpIRC.Controls.Add(this.txtIrcId);
            this.grpIRC.Controls.Add(this.chkIrcId);
            this.grpIRC.Controls.Add(this.label6);
            this.grpIRC.Controls.Add(this.txtOpChannel);
            this.grpIRC.Controls.Add(this.txtIRCServer);
            this.grpIRC.Controls.Add(this.label31);
            this.grpIRC.Controls.Add(this.txtChannel);
            this.grpIRC.Controls.Add(this.label4);
            this.grpIRC.Controls.Add(this.txtNick);
            this.grpIRC.Controls.Add(this.label5);
            this.grpIRC.Controls.Add(this.irc_cbTitles);
            this.grpIRC.Location = new System.Drawing.Point(8, 3);
            this.grpIRC.Name = "grpIRC";
            this.grpIRC.Size = new System.Drawing.Size(250, 272);
            this.grpIRC.TabIndex = 27;
            this.grpIRC.TabStop = false;
            this.grpIRC.Text = "IRC";
            // 
            // txtIRCPort
            // 
            this.txtIRCPort.Location = new System.Drawing.Point(82, 74);
            this.txtIRCPort.Name = "txtIRCPort";
            this.txtIRCPort.Size = new System.Drawing.Size(63, 21);
            this.txtIRCPort.TabIndex = 31;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(6, 77);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(30, 13);
            this.label50.TabIndex = 30;
            this.label50.Text = "Port:";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(25, 213);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(56, 13);
            this.label49.TabIndex = 29;
            this.label49.Text = "Password:";
            // 
            // txtIrcId
            // 
            this.txtIrcId.Location = new System.Drawing.Point(82, 210);
            this.txtIrcId.Name = "txtIrcId";
            this.txtIrcId.PasswordChar = '*';
            this.txtIrcId.Size = new System.Drawing.Size(106, 21);
            this.txtIrcId.TabIndex = 28;
            this.txtIrcId.Tag = "The password used for NickServ";
            // 
            // chkIrcId
            // 
            this.chkIrcId.AutoSize = true;
            this.chkIrcId.Location = new System.Drawing.Point(9, 187);
            this.chkIrcId.Name = "chkIrcId";
            this.chkIrcId.Size = new System.Drawing.Size(152, 17);
            this.chkIrcId.TabIndex = 27;
            this.chkIrcId.Text = "I have a NickServ password";
            this.chkIrcId.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Server:";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(6, 158);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(64, 13);
            this.label31.TabIndex = 25;
            this.label31.Text = "Op Channel:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Nick:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Channel:";
            // 
            // irc_cbTitles
            // 
            this.irc_cbTitles.AutoSize = true;
            this.irc_cbTitles.Location = new System.Drawing.Point(9, 251);
            this.irc_cbTitles.Name = "irc_cbTitles";
            this.irc_cbTitles.Size = new System.Drawing.Size(140, 17);
            this.irc_cbTitles.TabIndex = 32;
            this.irc_cbTitles.Text = "Show player titles on IRC";
            this.irc_cbTitles.UseVisualStyleBackColor = true;
            // 
            // pageServer
            // 
            this.pageServer.BackColor = System.Drawing.SystemColors.Control;
            this.pageServer.Controls.Add(this.grpLevels);
            this.pageServer.Controls.Add(this.grpAdvanced);
            this.pageServer.Controls.Add(this.grpGeneral);
            this.pageServer.Controls.Add(this.grpUpdate);
            this.pageServer.Controls.Add(this.grpPlayers);
            this.pageServer.Location = new System.Drawing.Point(4, 22);
            this.pageServer.Name = "pageServer";
            this.pageServer.Padding = new System.Windows.Forms.Padding(3);
            this.pageServer.Size = new System.Drawing.Size(498, 521);
            this.pageServer.TabIndex = 0;
            this.pageServer.Text = "Server";
            this.pageServer.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // grpLevels
            // 
            this.grpLevels.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpLevels.Controls.Add(this.label27);
            this.grpLevels.Controls.Add(this.txtMain);
            this.grpLevels.Controls.Add(this.chkAutoload);
            this.grpLevels.Controls.Add(this.chkWorld);
            this.grpLevels.Location = new System.Drawing.Point(314, 160);
            this.grpLevels.Name = "grpLevels";
            this.grpLevels.Size = new System.Drawing.Size(177, 105);
            this.grpLevels.TabIndex = 44;
            this.grpLevels.TabStop = false;
            this.grpLevels.Text = "Level Settings";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(6, 22);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(63, 13);
            this.label27.TabIndex = 3;
            this.label27.Text = "Main name:";
            // 
            // txtMain
            // 
            this.txtMain.Location = new System.Drawing.Point(75, 19);
            this.txtMain.Name = "txtMain";
            this.txtMain.Size = new System.Drawing.Size(87, 21);
            this.txtMain.TabIndex = 2;
            // 
            // grpAdvanced
            // 
            this.grpAdvanced.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpAdvanced.Controls.Add(this.editTxtsBt);
            this.grpAdvanced.Controls.Add(this.chkRestart);
            this.grpAdvanced.Controls.Add(this.chkVerify);
            this.grpAdvanced.Controls.Add(this.chkLogBeat);
            this.grpAdvanced.Location = new System.Drawing.Point(8, 271);
            this.grpAdvanced.Name = "grpAdvanced";
            this.grpAdvanced.Size = new System.Drawing.Size(206, 120);
            this.grpAdvanced.TabIndex = 42;
            this.grpAdvanced.TabStop = false;
            this.grpAdvanced.Text = "Advanced Configuration";
            // 
            // editTxtsBt
            // 
            this.editTxtsBt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.editTxtsBt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.editTxtsBt.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editTxtsBt.Location = new System.Drawing.Point(6, 91);
            this.editTxtsBt.Name = "editTxtsBt";
            this.editTxtsBt.Size = new System.Drawing.Size(80, 23);
            this.editTxtsBt.TabIndex = 35;
            this.editTxtsBt.Text = "Edit Text Files";
            this.editTxtsBt.UseVisualStyleBackColor = true;
            this.editTxtsBt.Click += new System.EventHandler(this.editTxtsBt_Click_1);
            // 
            // chkRestart
            // 
            this.chkRestart.AutoSize = true;
            this.chkRestart.Location = new System.Drawing.Point(9, 43);
            this.chkRestart.Name = "chkRestart";
            this.chkRestart.Size = new System.Drawing.Size(101, 17);
            this.chkRestart.TabIndex = 4;
            this.chkRestart.Text = "Restart on error";
            this.chkRestart.UseVisualStyleBackColor = true;
            // 
            // grpGeneral
            // 
            this.grpGeneral.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpGeneral.Controls.Add(this.label3);
            this.grpGeneral.Controls.Add(this.label7);
            this.grpGeneral.Controls.Add(this.chkPublic);
            this.grpGeneral.Controls.Add(this.txtServerOwner);
            this.grpGeneral.Controls.Add(this.label1);
            this.grpGeneral.Controls.Add(this.txtName);
            this.grpGeneral.Controls.Add(this.label2);
            this.grpGeneral.Controls.Add(this.txtMOTD);
            this.grpGeneral.Controls.Add(this.txtPort);
            this.grpGeneral.Controls.Add(this.ChkPort);
            this.grpGeneral.Location = new System.Drawing.Point(8, 6);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(483, 148);
            this.grpGeneral.TabIndex = 41;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "General Configuration";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Port:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Server owner:";
            // 
            // txtServerOwner
            // 
            this.txtServerOwner.Location = new System.Drawing.Point(83, 100);
            this.txtServerOwner.Name = "txtServerOwner";
            this.txtServerOwner.Size = new System.Drawing.Size(119, 21);
            this.txtServerOwner.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "MOTD:";
            // 
            // ChkPort
            // 
            this.ChkPort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ChkPort.Location = new System.Drawing.Point(152, 71);
            this.ChkPort.Name = "ChkPort";
            this.ChkPort.Size = new System.Drawing.Size(110, 23);
            this.ChkPort.TabIndex = 25;
            this.ChkPort.Text = "Server Port Utilities";
            this.ChkPort.UseVisualStyleBackColor = true;
            this.ChkPort.Click += new System.EventHandler(this.ChkPort_Click);
            // 
            // grpUpdate
            // 
            this.grpUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpUpdate.Controls.Add(this.forceUpdateBtn);
            this.grpUpdate.Controls.Add(this.updateTimeNumeric);
            this.grpUpdate.Controls.Add(this.lblUpdateSeconds);
            this.grpUpdate.Controls.Add(this.notifyInGameUpdate);
            this.grpUpdate.Controls.Add(this.autoUpdate);
            this.grpUpdate.Controls.Add(this.chkUpdates);
            this.grpUpdate.Location = new System.Drawing.Point(220, 271);
            this.grpUpdate.Name = "grpUpdate";
            this.grpUpdate.Size = new System.Drawing.Size(271, 120);
            this.grpUpdate.TabIndex = 44;
            this.grpUpdate.TabStop = false;
            this.grpUpdate.Text = "Update Settings";
            // 
            // forceUpdateBtn
            // 
            this.forceUpdateBtn.AutoSize = true;
            this.forceUpdateBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.forceUpdateBtn.Location = new System.Drawing.Point(186, 16);
            this.forceUpdateBtn.Name = "forceUpdateBtn";
            this.forceUpdateBtn.Size = new System.Drawing.Size(79, 23);
            this.forceUpdateBtn.TabIndex = 6;
            this.forceUpdateBtn.Text = "Force update";
            this.forceUpdateBtn.UseVisualStyleBackColor = true;
            this.forceUpdateBtn.Click += new System.EventHandler(this.forceUpdateBtn_Click);
            // 
            // updateTimeNumeric
            // 
            this.updateTimeNumeric.Location = new System.Drawing.Point(129, 89);
            this.updateTimeNumeric.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.updateTimeNumeric.Name = "updateTimeNumeric";
            this.updateTimeNumeric.Size = new System.Drawing.Size(39, 21);
            this.updateTimeNumeric.TabIndex = 29;
            this.updateTimeNumeric.Value = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            // 
            // lblUpdateSeconds
            // 
            this.lblUpdateSeconds.AutoSize = true;
            this.lblUpdateSeconds.Location = new System.Drawing.Point(3, 91);
            this.lblUpdateSeconds.Name = "lblUpdateSeconds";
            this.lblUpdateSeconds.Size = new System.Drawing.Size(120, 13);
            this.lblUpdateSeconds.TabIndex = 5;
            this.lblUpdateSeconds.Text = "Restart time in seconds:";
            // 
            // notifyInGameUpdate
            // 
            this.notifyInGameUpdate.AutoSize = true;
            this.notifyInGameUpdate.Location = new System.Drawing.Point(6, 43);
            this.notifyInGameUpdate.Name = "notifyInGameUpdate";
            this.notifyInGameUpdate.Size = new System.Drawing.Size(95, 17);
            this.notifyInGameUpdate.TabIndex = 7;
            this.notifyInGameUpdate.Text = "Notify In-Game";
            this.notifyInGameUpdate.UseVisualStyleBackColor = true;
            // 
            // autoUpdate
            // 
            this.autoUpdate.AutoSize = true;
            this.autoUpdate.Location = new System.Drawing.Point(6, 68);
            this.autoUpdate.Name = "autoUpdate";
            this.autoUpdate.Size = new System.Drawing.Size(85, 17);
            this.autoUpdate.TabIndex = 6;
            this.autoUpdate.Text = "Auto Update";
            this.autoUpdate.UseVisualStyleBackColor = true;
            // 
            // chkUpdates
            // 
            this.chkUpdates.AutoSize = true;
            this.chkUpdates.Location = new System.Drawing.Point(6, 20);
            this.chkUpdates.Name = "chkUpdates";
            this.chkUpdates.Size = new System.Drawing.Size(110, 17);
            this.chkUpdates.TabIndex = 4;
            this.chkUpdates.Text = "Check for updates";
            this.chkUpdates.UseVisualStyleBackColor = true;
            // 
            // grpPlayers
            // 
            this.grpPlayers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpPlayers.Controls.Add(this.label21);
            this.grpPlayers.Controls.Add(this.numPlayers);
            this.grpPlayers.Controls.Add(this.chkAgreeToRules);
            this.grpPlayers.Controls.Add(this.label35);
            this.grpPlayers.Controls.Add(this.numGuests);
            this.grpPlayers.Location = new System.Drawing.Point(8, 160);
            this.grpPlayers.Name = "grpPlayers";
            this.grpPlayers.Size = new System.Drawing.Size(300, 105);
            this.grpPlayers.TabIndex = 46;
            this.grpPlayers.TabStop = false;
            this.grpPlayers.Text = "Players";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 22);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(67, 13);
            this.label21.TabIndex = 3;
            this.label21.Text = "Max Players:";
            // 
            // numPlayers
            // 
            this.numPlayers.Location = new System.Drawing.Point(83, 20);
            this.numPlayers.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.numPlayers.Name = "numPlayers";
            this.numPlayers.Size = new System.Drawing.Size(60, 21);
            this.numPlayers.TabIndex = 29;
            this.numPlayers.Value = new decimal(new int[] {
                                    12,
                                    0,
                                    0,
                                    0});
            this.numPlayers.ValueChanged += new System.EventHandler(this.numPlayers_ValueChanged);
            // 
            // chkAgreeToRules
            // 
            this.chkAgreeToRules.AutoSize = true;
            this.chkAgreeToRules.Location = new System.Drawing.Point(9, 74);
            this.chkAgreeToRules.Name = "chkAgreeToRules";
            this.chkAgreeToRules.Size = new System.Drawing.Size(169, 17);
            this.chkAgreeToRules.TabIndex = 32;
            this.chkAgreeToRules.Tag = "Forces guests to use /agree on entry to the server";
            this.chkAgreeToRules.Text = "Force new guests to read rules";
            this.chkAgreeToRules.UseVisualStyleBackColor = true;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(6, 49);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(65, 13);
            this.label35.TabIndex = 27;
            this.label35.Text = "Max Guests:";
            // 
            // numGuests
            // 
            this.numGuests.Location = new System.Drawing.Point(83, 47);
            this.numGuests.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.numGuests.Name = "numGuests";
            this.numGuests.Size = new System.Drawing.Size(60, 21);
            this.numGuests.TabIndex = 28;
            this.numGuests.Value = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                                    | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.pageServer);
            this.tabControl.Controls.Add(this.tabChat);
            this.tabControl.Controls.Add(this.pageIRC);
            this.tabControl.Controls.Add(this.pageMisc);
            this.tabControl.Controls.Add(this.pageGames);
            this.tabControl.Controls.Add(this.pageRanks);
            this.tabControl.Controls.Add(this.pageCommands);
            this.tabControl.Controls.Add(this.pageBlocks);
            this.tabControl.Controls.Add(this.pageSecurity);
            this.tabControl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(506, 547);
            this.tabControl.TabIndex = 1;
            // 
            // pageGames
            // 
            this.pageGames.BackColor = System.Drawing.SystemColors.Control;
            this.pageGames.Controls.Add(this.tabGames);
            this.pageGames.Location = new System.Drawing.Point(4, 22);
            this.pageGames.Name = "pageGames";
            this.pageGames.Padding = new System.Windows.Forms.Padding(3);
            this.pageGames.Size = new System.Drawing.Size(498, 521);
            this.pageGames.TabIndex = 8;
            this.pageGames.Text = "Games";
            // 
            // tabGames
            // 
            this.tabGames.Controls.Add(this.tabPage10);
            this.tabGames.Controls.Add(this.tabPage14);
            this.tabGames.Controls.Add(this.tabPage11);
            this.tabGames.Location = new System.Drawing.Point(3, 3);
            this.tabGames.Name = "tabGames";
            this.tabGames.SelectedIndex = 0;
            this.tabGames.Size = new System.Drawing.Size(492, 515);
            this.tabGames.TabIndex = 0;
            this.tabGames.Click += new System.EventHandler(this.tabControl2_Click);
            // 
            // tabPage10
            // 
            this.tabPage10.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage10.Controls.Add(this.groupBox23);
            this.tabPage10.Controls.Add(this.groupBox22);
            this.tabPage10.Controls.Add(this.groupBox21);
            this.tabPage10.Controls.Add(this.groupBox20);
            this.tabPage10.Location = new System.Drawing.Point(4, 22);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage10.Size = new System.Drawing.Size(484, 489);
            this.tabPage10.TabIndex = 0;
            this.tabPage10.Text = "Lava Survival";
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.lsBtnEndVote);
            this.groupBox23.Controls.Add(this.lsBtnEndRound);
            this.groupBox23.Controls.Add(this.lsBtnStopGame);
            this.groupBox23.Controls.Add(this.lsBtnStartGame);
            this.groupBox23.Location = new System.Drawing.Point(53, 397);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(370, 51);
            this.groupBox23.TabIndex = 4;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Controls";
            // 
            // lsBtnEndVote
            // 
            this.lsBtnEndVote.Location = new System.Drawing.Point(280, 20);
            this.lsBtnEndVote.Name = "lsBtnEndVote";
            this.lsBtnEndVote.Size = new System.Drawing.Size(80, 23);
            this.lsBtnEndVote.TabIndex = 3;
            this.lsBtnEndVote.Text = "End Vote";
            this.lsBtnEndVote.UseVisualStyleBackColor = true;
            this.lsBtnEndVote.Click += new System.EventHandler(this.lsBtnEndVote_Click);
            // 
            // lsBtnEndRound
            // 
            this.lsBtnEndRound.Location = new System.Drawing.Point(190, 20);
            this.lsBtnEndRound.Name = "lsBtnEndRound";
            this.lsBtnEndRound.Size = new System.Drawing.Size(80, 23);
            this.lsBtnEndRound.TabIndex = 2;
            this.lsBtnEndRound.Text = "End Round";
            this.lsBtnEndRound.UseVisualStyleBackColor = true;
            this.lsBtnEndRound.Click += new System.EventHandler(this.lsBtnEndRound_Click);
            // 
            // lsBtnStopGame
            // 
            this.lsBtnStopGame.Location = new System.Drawing.Point(100, 20);
            this.lsBtnStopGame.Name = "lsBtnStopGame";
            this.lsBtnStopGame.Size = new System.Drawing.Size(80, 23);
            this.lsBtnStopGame.TabIndex = 1;
            this.lsBtnStopGame.Text = "Stop Game";
            this.lsBtnStopGame.UseVisualStyleBackColor = true;
            this.lsBtnStopGame.Click += new System.EventHandler(this.lsBtnStopGame_Click);
            // 
            // lsBtnStartGame
            // 
            this.lsBtnStartGame.Location = new System.Drawing.Point(10, 20);
            this.lsBtnStartGame.Name = "lsBtnStartGame";
            this.lsBtnStartGame.Size = new System.Drawing.Size(80, 23);
            this.lsBtnStartGame.TabIndex = 0;
            this.lsBtnStartGame.Text = "Start Game";
            this.lsBtnStartGame.UseVisualStyleBackColor = true;
            this.lsBtnStartGame.Click += new System.EventHandler(this.lsBtnStartGame_Click);
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.pg_lavaMap);
            this.groupBox22.Location = new System.Drawing.Point(193, 174);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(285, 219);
            this.groupBox22.TabIndex = 3;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Map Settings";
            // 
            // pg_lavaMap
            // 
            this.pg_lavaMap.HelpVisible = false;
            this.pg_lavaMap.Location = new System.Drawing.Point(6, 20);
            this.pg_lavaMap.Name = "pg_lavaMap";
            this.pg_lavaMap.Size = new System.Drawing.Size(273, 191);
            this.pg_lavaMap.TabIndex = 7;
            this.pg_lavaMap.ToolbarVisible = false;
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.pg_lava);
            this.groupBox21.Location = new System.Drawing.Point(193, 6);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(285, 162);
            this.groupBox21.TabIndex = 2;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Settings";
            // 
            // pg_lava
            // 
            this.pg_lava.HelpVisible = false;
            this.pg_lava.Location = new System.Drawing.Point(6, 17);
            this.pg_lava.Name = "pg_lava";
            this.pg_lava.Size = new System.Drawing.Size(273, 139);
            this.pg_lava.TabIndex = 0;
            this.pg_lava.ToolbarVisible = false;
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.label54);
            this.groupBox20.Controls.Add(this.label53);
            this.groupBox20.Controls.Add(this.lsAddMap);
            this.groupBox20.Controls.Add(this.lsRemoveMap);
            this.groupBox20.Controls.Add(this.lsMapNoUse);
            this.groupBox20.Controls.Add(this.lsMapUse);
            this.groupBox20.Location = new System.Drawing.Point(6, 6);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(181, 387);
            this.groupBox20.TabIndex = 1;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "Maps";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(187, 17);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(83, 13);
            this.label54.TabIndex = 6;
            this.label54.Text = "Maps Not In Use";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(6, 17);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(38, 13);
            this.label53.TabIndex = 5;
            this.label53.Text = "In use:";
            // 
            // lsAddMap
            // 
            this.lsAddMap.Location = new System.Drawing.Point(6, 188);
            this.lsAddMap.Name = "lsAddMap";
            this.lsAddMap.Size = new System.Drawing.Size(77, 23);
            this.lsAddMap.TabIndex = 4;
            this.lsAddMap.Text = "<< Add";
            this.lsAddMap.UseVisualStyleBackColor = true;
            this.lsAddMap.Click += new System.EventHandler(this.lsAddMap_Click);
            // 
            // lsRemoveMap
            // 
            this.lsRemoveMap.Location = new System.Drawing.Point(100, 188);
            this.lsRemoveMap.Name = "lsRemoveMap";
            this.lsRemoveMap.Size = new System.Drawing.Size(75, 23);
            this.lsRemoveMap.TabIndex = 3;
            this.lsRemoveMap.Text = "Remove >>";
            this.lsRemoveMap.UseVisualStyleBackColor = true;
            this.lsRemoveMap.Click += new System.EventHandler(this.lsRemoveMap_Click);
            // 
            // lsMapNoUse
            // 
            this.lsMapNoUse.FormattingEnabled = true;
            this.lsMapNoUse.Location = new System.Drawing.Point(6, 219);
            this.lsMapNoUse.Name = "lsMapNoUse";
            this.lsMapNoUse.Size = new System.Drawing.Size(169, 160);
            this.lsMapNoUse.TabIndex = 2;
            // 
            // lsMapUse
            // 
            this.lsMapUse.FormattingEnabled = true;
            this.lsMapUse.Location = new System.Drawing.Point(6, 33);
            this.lsMapUse.Name = "lsMapUse";
            this.lsMapUse.Size = new System.Drawing.Size(169, 147);
            this.lsMapUse.TabIndex = 0;
            this.lsMapUse.SelectedIndexChanged += new System.EventHandler(this.lsMapUse_SelectedIndexChanged);
            // 
            // tabPage14
            // 
            this.tabPage14.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage14.Controls.Add(this.SlctdTntWrsPlyrs);
            this.tabPage14.Controls.Add(this.label87);
            this.tabPage14.Controls.Add(this.SlctdTntWrdStatus);
            this.tabPage14.Controls.Add(this.label86);
            this.tabPage14.Controls.Add(this.label85);
            this.tabPage14.Controls.Add(this.SlctdTntWrsLvl);
            this.tabPage14.Controls.Add(this.groupBox29);
            this.tabPage14.Controls.Add(this.EditTntWarsGameBT);
            this.tabPage14.Controls.Add(this.TntWarsGamesList);
            this.tabPage14.Location = new System.Drawing.Point(4, 22);
            this.tabPage14.Name = "tabPage14";
            this.tabPage14.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage14.Size = new System.Drawing.Size(484, 489);
            this.tabPage14.TabIndex = 2;
            this.tabPage14.Text = "TNT Wars";
            // 
            // SlctdTntWrsPlyrs
            // 
            this.SlctdTntWrsPlyrs.Location = new System.Drawing.Point(426, 6);
            this.SlctdTntWrsPlyrs.Name = "SlctdTntWrsPlyrs";
            this.SlctdTntWrsPlyrs.ReadOnly = true;
            this.SlctdTntWrsPlyrs.Size = new System.Drawing.Size(36, 21);
            this.SlctdTntWrsPlyrs.TabIndex = 9;
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Location = new System.Drawing.Point(375, 9);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(45, 13);
            this.label87.TabIndex = 8;
            this.label87.Text = "Players:";
            // 
            // SlctdTntWrdStatus
            // 
            this.SlctdTntWrdStatus.Location = new System.Drawing.Point(234, 6);
            this.SlctdTntWrdStatus.Name = "SlctdTntWrdStatus";
            this.SlctdTntWrdStatus.ReadOnly = true;
            this.SlctdTntWrdStatus.Size = new System.Drawing.Size(135, 21);
            this.SlctdTntWrdStatus.TabIndex = 7;
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(188, 9);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(40, 13);
            this.label86.TabIndex = 6;
            this.label86.Text = "Status:";
            // 
            // SlctdTntWrsLvl
            // 
            this.SlctdTntWrsLvl.Location = new System.Drawing.Point(47, 6);
            this.SlctdTntWrsLvl.Name = "SlctdTntWrsLvl";
            this.SlctdTntWrsLvl.ReadOnly = true;
            this.SlctdTntWrsLvl.Size = new System.Drawing.Size(135, 21);
            this.SlctdTntWrsLvl.TabIndex = 4;
            // 
            // groupBox29
            // 
            this.groupBox29.Controls.Add(this.groupBox36);
            this.groupBox29.Controls.Add(this.groupBox35);
            this.groupBox29.Controls.Add(this.groupBox34);
            this.groupBox29.Controls.Add(this.groupBox33);
            this.groupBox29.Controls.Add(this.groupBox32);
            this.groupBox29.Controls.Add(this.groupBox31);
            this.groupBox29.Controls.Add(this.groupBox30);
            this.groupBox29.Location = new System.Drawing.Point(9, 33);
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.Size = new System.Drawing.Size(453, 268);
            this.groupBox29.TabIndex = 3;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Edit Selected Game";
            // 
            // groupBox36
            // 
            this.groupBox36.Controls.Add(this.TntWrsStreaksChck);
            this.groupBox36.Location = new System.Drawing.Point(355, 20);
            this.groupBox36.Name = "groupBox36";
            this.groupBox36.Size = new System.Drawing.Size(92, 103);
            this.groupBox36.TabIndex = 10;
            this.groupBox36.TabStop = false;
            this.groupBox36.Text = "Other:";
            // 
            // TntWrsStreaksChck
            // 
            this.TntWrsStreaksChck.AutoSize = true;
            this.TntWrsStreaksChck.Checked = true;
            this.TntWrsStreaksChck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TntWrsStreaksChck.Location = new System.Drawing.Point(7, 21);
            this.TntWrsStreaksChck.Name = "TntWrsStreaksChck";
            this.TntWrsStreaksChck.Size = new System.Drawing.Size(61, 17);
            this.TntWrsStreaksChck.TabIndex = 0;
            this.TntWrsStreaksChck.Text = "Streaks";
            this.TntWrsStreaksChck.UseVisualStyleBackColor = true;
            this.TntWrsStreaksChck.CheckedChanged += new System.EventHandler(this.TntWrsStreaksChck_CheckedChanged);
            // 
            // groupBox35
            // 
            this.groupBox35.Controls.Add(this.TntWrsMpsList);
            this.groupBox35.Controls.Add(this.TntWrsCrtNwTntWrsBt);
            this.groupBox35.Location = new System.Drawing.Point(196, 20);
            this.groupBox35.Name = "groupBox35";
            this.groupBox35.Size = new System.Drawing.Size(152, 71);
            this.groupBox35.TabIndex = 9;
            this.groupBox35.TabStop = false;
            this.groupBox35.Text = "New Game";
            // 
            // TntWrsMpsList
            // 
            this.TntWrsMpsList.FormattingEnabled = true;
            this.TntWrsMpsList.Location = new System.Drawing.Point(7, 20);
            this.TntWrsMpsList.Name = "TntWrsMpsList";
            this.TntWrsMpsList.Size = new System.Drawing.Size(82, 30);
            this.TntWrsMpsList.TabIndex = 1;
            this.TntWrsMpsList.SelectedIndexChanged += new System.EventHandler(this.TntWrsMpsList_SelectedIndexChanged);
            // 
            // TntWrsCrtNwTntWrsBt
            // 
            this.TntWrsCrtNwTntWrsBt.Enabled = false;
            this.TntWrsCrtNwTntWrsBt.Location = new System.Drawing.Point(95, 20);
            this.TntWrsCrtNwTntWrsBt.Name = "TntWrsCrtNwTntWrsBt";
            this.TntWrsCrtNwTntWrsBt.Size = new System.Drawing.Size(51, 45);
            this.TntWrsCrtNwTntWrsBt.TabIndex = 0;
            this.TntWrsCrtNwTntWrsBt.Text = "Create";
            this.TntWrsCrtNwTntWrsBt.UseVisualStyleBackColor = true;
            this.TntWrsCrtNwTntWrsBt.Click += new System.EventHandler(this.TntWrsCrtNwTntWrsBt_Click);
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.TntWrsDiffSlctBt);
            this.groupBox34.Controls.Add(this.TntWrsDiffAboutBt);
            this.groupBox34.Controls.Add(this.TntWrsDiffCombo);
            this.groupBox34.Location = new System.Drawing.Point(6, 20);
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.Size = new System.Drawing.Size(178, 103);
            this.groupBox34.TabIndex = 8;
            this.groupBox34.TabStop = false;
            this.groupBox34.Text = "Difficulty";
            // 
            // TntWrsDiffSlctBt
            // 
            this.TntWrsDiffSlctBt.Location = new System.Drawing.Point(6, 45);
            this.TntWrsDiffSlctBt.Name = "TntWrsDiffSlctBt";
            this.TntWrsDiffSlctBt.Size = new System.Drawing.Size(166, 23);
            this.TntWrsDiffSlctBt.TabIndex = 2;
            this.TntWrsDiffSlctBt.Text = "Select Difficulty";
            this.TntWrsDiffSlctBt.UseVisualStyleBackColor = true;
            this.TntWrsDiffSlctBt.Click += new System.EventHandler(this.TntWrsDiffSlctBt_Click);
            // 
            // TntWrsDiffAboutBt
            // 
            this.TntWrsDiffAboutBt.Location = new System.Drawing.Point(6, 74);
            this.TntWrsDiffAboutBt.Name = "TntWrsDiffAboutBt";
            this.TntWrsDiffAboutBt.Size = new System.Drawing.Size(166, 23);
            this.TntWrsDiffAboutBt.TabIndex = 1;
            this.TntWrsDiffAboutBt.Text = "About Difficulties";
            this.TntWrsDiffAboutBt.UseVisualStyleBackColor = true;
            this.TntWrsDiffAboutBt.Click += new System.EventHandler(this.TntWrsDiffAboutBt_Click);
            // 
            // TntWrsDiffCombo
            // 
            this.TntWrsDiffCombo.FormattingEnabled = true;
            this.TntWrsDiffCombo.Items.AddRange(new object[] {
                                    "Easy",
                                    "Normal",
                                    "Hard",
                                    "Extreme"});
            this.TntWrsDiffCombo.Location = new System.Drawing.Point(6, 20);
            this.TntWrsDiffCombo.Name = "TntWrsDiffCombo";
            this.TntWrsDiffCombo.Size = new System.Drawing.Size(166, 21);
            this.TntWrsDiffCombo.TabIndex = 0;
            // 
            // groupBox33
            // 
            this.groupBox33.Controls.Add(this.TntWrsTKchck);
            this.groupBox33.Controls.Add(this.TntWrsBlnceTeamsChck);
            this.groupBox33.Controls.Add(this.TntWrsTmsChck);
            this.groupBox33.Location = new System.Drawing.Point(196, 175);
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.Size = new System.Drawing.Size(152, 87);
            this.groupBox33.TabIndex = 7;
            this.groupBox33.TabStop = false;
            this.groupBox33.Text = "Teams:";
            // 
            // TntWrsTKchck
            // 
            this.TntWrsTKchck.AutoSize = true;
            this.TntWrsTKchck.Location = new System.Drawing.Point(7, 68);
            this.TntWrsTKchck.Name = "TntWrsTKchck";
            this.TntWrsTKchck.Size = new System.Drawing.Size(73, 17);
            this.TntWrsTKchck.TabIndex = 2;
            this.TntWrsTKchck.Text = "Team Kills";
            this.TntWrsTKchck.UseVisualStyleBackColor = true;
            this.TntWrsTKchck.CheckedChanged += new System.EventHandler(this.TntWrsTKchck_CheckedChanged);
            // 
            // TntWrsBlnceTeamsChck
            // 
            this.TntWrsBlnceTeamsChck.AutoSize = true;
            this.TntWrsBlnceTeamsChck.Checked = true;
            this.TntWrsBlnceTeamsChck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TntWrsBlnceTeamsChck.Location = new System.Drawing.Point(7, 44);
            this.TntWrsBlnceTeamsChck.Name = "TntWrsBlnceTeamsChck";
            this.TntWrsBlnceTeamsChck.Size = new System.Drawing.Size(96, 17);
            this.TntWrsBlnceTeamsChck.TabIndex = 1;
            this.TntWrsBlnceTeamsChck.Text = "Balance Teams";
            this.TntWrsBlnceTeamsChck.UseVisualStyleBackColor = true;
            this.TntWrsBlnceTeamsChck.CheckedChanged += new System.EventHandler(this.TntWrsBlnceTeamsChck_CheckedChanged);
            // 
            // TntWrsTmsChck
            // 
            this.TntWrsTmsChck.AutoSize = true;
            this.TntWrsTmsChck.Checked = true;
            this.TntWrsTmsChck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TntWrsTmsChck.Location = new System.Drawing.Point(7, 21);
            this.TntWrsTmsChck.Name = "TntWrsTmsChck";
            this.TntWrsTmsChck.Size = new System.Drawing.Size(104, 17);
            this.TntWrsTmsChck.TabIndex = 0;
            this.TntWrsTmsChck.Text = "Teams (TDM/FFA)";
            this.TntWrsTmsChck.UseVisualStyleBackColor = true;
            this.TntWrsTmsChck.CheckedChanged += new System.EventHandler(this.TntWrsTmsChck_CheckedChanged);
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.label90);
            this.groupBox32.Controls.Add(this.TntWrsGraceTimeChck);
            this.groupBox32.Controls.Add(this.TntWrsGracePrdChck);
            this.groupBox32.Location = new System.Drawing.Point(196, 97);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(152, 73);
            this.groupBox32.TabIndex = 6;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Grace Period";
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(3, 45);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(80, 13);
            this.label90.TabIndex = 2;
            this.label90.Text = "Time (Seconds):";
            // 
            // TntWrsGraceTimeChck
            // 
            this.TntWrsGraceTimeChck.Location = new System.Drawing.Point(89, 43);
            this.TntWrsGraceTimeChck.Maximum = new decimal(new int[] {
                                    300,
                                    0,
                                    0,
                                    0});
            this.TntWrsGraceTimeChck.Minimum = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            this.TntWrsGraceTimeChck.Name = "TntWrsGraceTimeChck";
            this.TntWrsGraceTimeChck.Size = new System.Drawing.Size(57, 21);
            this.TntWrsGraceTimeChck.TabIndex = 1;
            this.TntWrsGraceTimeChck.Value = new decimal(new int[] {
                                    30,
                                    0,
                                    0,
                                    0});
            this.TntWrsGraceTimeChck.ValueChanged += new System.EventHandler(this.TntWrsGraceTimeChck_ValueChanged);
            // 
            // TntWrsGracePrdChck
            // 
            this.TntWrsGracePrdChck.AutoSize = true;
            this.TntWrsGracePrdChck.Checked = true;
            this.TntWrsGracePrdChck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TntWrsGracePrdChck.Location = new System.Drawing.Point(6, 20);
            this.TntWrsGracePrdChck.Name = "TntWrsGracePrdChck";
            this.TntWrsGracePrdChck.Size = new System.Drawing.Size(87, 17);
            this.TntWrsGracePrdChck.TabIndex = 0;
            this.TntWrsGracePrdChck.Text = "Grace Period";
            this.TntWrsGracePrdChck.UseVisualStyleBackColor = true;
            this.TntWrsGracePrdChck.CheckedChanged += new System.EventHandler(this.TntWrsGracePrdChck_CheckedChanged);
            // 
            // groupBox31
            // 
            this.groupBox31.Controls.Add(this.TntWrsMltiKlChck);
            this.groupBox31.Controls.Add(this.TntWrsMltiKlScPrUpDown);
            this.groupBox31.Controls.Add(this.TntWrsAsstChck);
            this.groupBox31.Controls.Add(this.TntWrsAstsScrUpDwn);
            this.groupBox31.Controls.Add(this.label89);
            this.groupBox31.Controls.Add(this.TntWrsScrPrKlUpDwn);
            this.groupBox31.Controls.Add(this.label88);
            this.groupBox31.Controls.Add(this.TntWrsScrLmtUpDwn);
            this.groupBox31.Location = new System.Drawing.Point(6, 129);
            this.groupBox31.Name = "groupBox31";
            this.groupBox31.Size = new System.Drawing.Size(184, 133);
            this.groupBox31.TabIndex = 5;
            this.groupBox31.TabStop = false;
            this.groupBox31.Text = "Scores";
            // 
            // TntWrsMltiKlChck
            // 
            this.TntWrsMltiKlChck.AutoSize = true;
            this.TntWrsMltiKlChck.Checked = true;
            this.TntWrsMltiKlChck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TntWrsMltiKlChck.Location = new System.Drawing.Point(11, 102);
            this.TntWrsMltiKlChck.Name = "TntWrsMltiKlChck";
            this.TntWrsMltiKlChck.Size = new System.Drawing.Size(122, 17);
            this.TntWrsMltiKlChck.TabIndex = 8;
            this.TntWrsMltiKlChck.Text = "MultiKills (Score Per:";
            this.TntWrsMltiKlChck.UseVisualStyleBackColor = true;
            this.TntWrsMltiKlChck.CheckedChanged += new System.EventHandler(this.TntWrsMltiKlChck_CheckedChanged);
            // 
            // TntWrsMltiKlScPrUpDown
            // 
            this.TntWrsMltiKlScPrUpDown.Location = new System.Drawing.Point(140, 101);
            this.TntWrsMltiKlScPrUpDown.Maximum = new decimal(new int[] {
                                    10000,
                                    0,
                                    0,
                                    0});
            this.TntWrsMltiKlScPrUpDown.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.TntWrsMltiKlScPrUpDown.Name = "TntWrsMltiKlScPrUpDown";
            this.TntWrsMltiKlScPrUpDown.Size = new System.Drawing.Size(38, 21);
            this.TntWrsMltiKlScPrUpDown.TabIndex = 7;
            this.TntWrsMltiKlScPrUpDown.Value = new decimal(new int[] {
                                    5,
                                    0,
                                    0,
                                    0});
            this.TntWrsMltiKlScPrUpDown.ValueChanged += new System.EventHandler(this.TntWrsMltiKlScPrUpDown_ValueChanged);
            // 
            // TntWrsAsstChck
            // 
            this.TntWrsAsstChck.AutoSize = true;
            this.TntWrsAsstChck.Checked = true;
            this.TntWrsAsstChck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TntWrsAsstChck.Location = new System.Drawing.Point(23, 75);
            this.TntWrsAsstChck.Name = "TntWrsAsstChck";
            this.TntWrsAsstChck.Size = new System.Drawing.Size(111, 17);
            this.TntWrsAsstChck.TabIndex = 6;
            this.TntWrsAsstChck.Text = "Assists (Score Per:";
            this.TntWrsAsstChck.UseVisualStyleBackColor = true;
            this.TntWrsAsstChck.CheckedChanged += new System.EventHandler(this.TntWrsAsstChck_CheckedChanged);
            // 
            // TntWrsAstsScrUpDwn
            // 
            this.TntWrsAstsScrUpDwn.Location = new System.Drawing.Point(140, 74);
            this.TntWrsAstsScrUpDwn.Maximum = new decimal(new int[] {
                                    10000,
                                    0,
                                    0,
                                    0});
            this.TntWrsAstsScrUpDwn.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.TntWrsAstsScrUpDwn.Name = "TntWrsAstsScrUpDwn";
            this.TntWrsAstsScrUpDwn.Size = new System.Drawing.Size(38, 21);
            this.TntWrsAstsScrUpDwn.TabIndex = 4;
            this.TntWrsAstsScrUpDwn.Value = new decimal(new int[] {
                                    5,
                                    0,
                                    0,
                                    0});
            this.TntWrsAstsScrUpDwn.ValueChanged += new System.EventHandler(this.TntWrsAstsScrUpDwn_ValueChanged);
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(63, 49);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(71, 13);
            this.label89.TabIndex = 3;
            this.label89.Text = "Score Per Kill:";
            // 
            // TntWrsScrPrKlUpDwn
            // 
            this.TntWrsScrPrKlUpDwn.Location = new System.Drawing.Point(140, 47);
            this.TntWrsScrPrKlUpDwn.Maximum = new decimal(new int[] {
                                    10000,
                                    0,
                                    0,
                                    0});
            this.TntWrsScrPrKlUpDwn.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.TntWrsScrPrKlUpDwn.Name = "TntWrsScrPrKlUpDwn";
            this.TntWrsScrPrKlUpDwn.Size = new System.Drawing.Size(38, 21);
            this.TntWrsScrPrKlUpDwn.TabIndex = 2;
            this.TntWrsScrPrKlUpDwn.Value = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            this.TntWrsScrPrKlUpDwn.ValueChanged += new System.EventHandler(this.TntWrsScrPrKlUpDwn_ValueChanged);
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Location = new System.Drawing.Point(72, 22);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(62, 13);
            this.label88.TabIndex = 1;
            this.label88.Text = "Score Limit:";
            // 
            // TntWrsScrLmtUpDwn
            // 
            this.TntWrsScrLmtUpDwn.Location = new System.Drawing.Point(140, 20);
            this.TntWrsScrLmtUpDwn.Maximum = new decimal(new int[] {
                                    10000000,
                                    0,
                                    0,
                                    0});
            this.TntWrsScrLmtUpDwn.Minimum = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            this.TntWrsScrLmtUpDwn.Name = "TntWrsScrLmtUpDwn";
            this.TntWrsScrLmtUpDwn.Size = new System.Drawing.Size(38, 21);
            this.TntWrsScrLmtUpDwn.TabIndex = 0;
            this.TntWrsScrLmtUpDwn.Value = new decimal(new int[] {
                                    150,
                                    0,
                                    0,
                                    0});
            this.TntWrsScrLmtUpDwn.ValueChanged += new System.EventHandler(this.TntWrsScrLmtUpDwn_ValueChanged);
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.TntWrsStrtGame);
            this.groupBox30.Controls.Add(this.TntWrsDltGame);
            this.groupBox30.Controls.Add(this.TntWrsEndGame);
            this.groupBox30.Controls.Add(this.TntWrsRstGame);
            this.groupBox30.Location = new System.Drawing.Point(354, 129);
            this.groupBox30.Name = "groupBox30";
            this.groupBox30.Size = new System.Drawing.Size(93, 133);
            this.groupBox30.TabIndex = 4;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "Status";
            // 
            // TntWrsStrtGame
            // 
            this.TntWrsStrtGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TntWrsStrtGame.Location = new System.Drawing.Point(6, 17);
            this.TntWrsStrtGame.Name = "TntWrsStrtGame";
            this.TntWrsStrtGame.Size = new System.Drawing.Size(80, 23);
            this.TntWrsStrtGame.TabIndex = 0;
            this.TntWrsStrtGame.Text = "Start Game";
            this.TntWrsStrtGame.UseVisualStyleBackColor = true;
            this.TntWrsStrtGame.Click += new System.EventHandler(this.TntWrsStrtGame_Click);
            // 
            // TntWrsDltGame
            // 
            this.TntWrsDltGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TntWrsDltGame.Location = new System.Drawing.Point(6, 104);
            this.TntWrsDltGame.Name = "TntWrsDltGame";
            this.TntWrsDltGame.Size = new System.Drawing.Size(80, 23);
            this.TntWrsDltGame.TabIndex = 3;
            this.TntWrsDltGame.Text = "Delete Game";
            this.TntWrsDltGame.UseVisualStyleBackColor = true;
            this.TntWrsDltGame.Click += new System.EventHandler(this.TntWrsDltGame_Click);
            // 
            // TntWrsEndGame
            // 
            this.TntWrsEndGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TntWrsEndGame.Location = new System.Drawing.Point(6, 46);
            this.TntWrsEndGame.Name = "TntWrsEndGame";
            this.TntWrsEndGame.Size = new System.Drawing.Size(80, 23);
            this.TntWrsEndGame.TabIndex = 1;
            this.TntWrsEndGame.Text = "End Game";
            this.TntWrsEndGame.UseVisualStyleBackColor = true;
            this.TntWrsEndGame.Click += new System.EventHandler(this.TntWrsEndGame_Click);
            // 
            // TntWrsRstGame
            // 
            this.TntWrsRstGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TntWrsRstGame.Location = new System.Drawing.Point(6, 75);
            this.TntWrsRstGame.Name = "TntWrsRstGame";
            this.TntWrsRstGame.Size = new System.Drawing.Size(80, 23);
            this.TntWrsRstGame.TabIndex = 2;
            this.TntWrsRstGame.Text = "Reset Game";
            this.TntWrsRstGame.UseVisualStyleBackColor = true;
            this.TntWrsRstGame.Click += new System.EventHandler(this.TntWrsRstGame_Click);
            // 
            // EditTntWarsGameBT
            // 
            this.EditTntWarsGameBT.Location = new System.Drawing.Point(9, 307);
            this.EditTntWarsGameBT.Name = "EditTntWarsGameBT";
            this.EditTntWarsGameBT.Size = new System.Drawing.Size(453, 23);
            this.EditTntWarsGameBT.TabIndex = 1;
            this.EditTntWarsGameBT.Text = "Edit";
            this.EditTntWarsGameBT.UseVisualStyleBackColor = true;
            this.EditTntWarsGameBT.Click += new System.EventHandler(this.EditTntWarsGameBT_Click);
            // 
            // TntWarsGamesList
            // 
            this.TntWarsGamesList.FormattingEnabled = true;
            this.TntWarsGamesList.Location = new System.Drawing.Point(9, 333);
            this.TntWarsGamesList.Name = "TntWarsGamesList";
            this.TntWarsGamesList.Size = new System.Drawing.Size(453, 121);
            this.TntWarsGamesList.TabIndex = 0;
            // 
            // tabPage11
            // 
            this.tabPage11.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage11.Controls.Add(this.propsZG);
            this.tabPage11.Location = new System.Drawing.Point(4, 22);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage11.Size = new System.Drawing.Size(484, 489);
            this.tabPage11.TabIndex = 1;
            this.tabPage11.Text = "Zombie survival";
            // 
            // propsZG
            // 
            this.propsZG.Location = new System.Drawing.Point(6, 3);
            this.propsZG.Name = "propsZG";
            this.propsZG.Size = new System.Drawing.Size(456, 464);
            this.propsZG.TabIndex = 42;
            this.propsZG.ToolbarVisible = false;
            // 
            // pageSecurity
            // 
            this.pageSecurity.BackColor = System.Drawing.SystemColors.Control;
            this.pageSecurity.Controls.Add(this.sec_gbBlocks);
            this.pageSecurity.Controls.Add(this.sec_gbCmd);
            this.pageSecurity.Controls.Add(this.sec_gbOther);
            this.pageSecurity.Controls.Add(this.sec_gbChat);
            this.pageSecurity.Location = new System.Drawing.Point(4, 22);
            this.pageSecurity.Name = "pageSecurity";
            this.pageSecurity.Padding = new System.Windows.Forms.Padding(3);
            this.pageSecurity.Size = new System.Drawing.Size(498, 521);
            this.pageSecurity.TabIndex = 7;
            this.pageSecurity.Text = "Security";
            // 
            // sec_gbBlocks
            // 
            this.sec_gbBlocks.Controls.Add(this.sec_cbBlocksAuto);
            this.sec_gbBlocks.Controls.Add(this.sec_lblBlocksOnMute);
            this.sec_gbBlocks.Controls.Add(this.sec_numBlocksMsgs);
            this.sec_gbBlocks.Controls.Add(this.sec_lblBlocksOnMsgs);
            this.sec_gbBlocks.Controls.Add(this.sec_numBlocksSecs);
            this.sec_gbBlocks.Controls.Add(this.sec_lblBlocksOnSecs);
            this.sec_gbBlocks.Location = new System.Drawing.Point(264, 150);
            this.sec_gbBlocks.Name = "sec_gbBlocks";
            this.sec_gbBlocks.Size = new System.Drawing.Size(217, 83);
            this.sec_gbBlocks.TabIndex = 36;
            this.sec_gbBlocks.TabStop = false;
            this.sec_gbBlocks.Text = "Block spam control";
            // 
            // sec_cbBlocksAuto
            // 
            this.sec_cbBlocksAuto.AutoSize = true;
            this.sec_cbBlocksAuto.Location = new System.Drawing.Point(10, 20);
            this.sec_cbBlocksAuto.Name = "sec_cbBlocksAuto";
            this.sec_cbBlocksAuto.Size = new System.Drawing.Size(142, 17);
            this.sec_cbBlocksAuto.TabIndex = 24;
            this.sec_cbBlocksAuto.Text = "Enable automatic kicking";
            this.sec_cbBlocksAuto.UseVisualStyleBackColor = true;
            this.sec_cbBlocksAuto.CheckedChanged += new System.EventHandler(this.sec_cbBlocksAuto_Checked);
            // 
            // sec_lblBlocksOnMute
            // 
            this.sec_lblBlocksOnMute.AutoSize = true;
            this.sec_lblBlocksOnMute.Location = new System.Drawing.Point(6, 48);
            this.sec_lblBlocksOnMute.Name = "sec_lblBlocksOnMute";
            this.sec_lblBlocksOnMute.Size = new System.Drawing.Size(40, 13);
            this.sec_lblBlocksOnMute.TabIndex = 25;
            this.sec_lblBlocksOnMute.Text = "Kick on";
            // 
            // sec_numBlocksMsgs
            // 
            this.sec_numBlocksMsgs.Location = new System.Drawing.Point(46, 45);
            this.sec_numBlocksMsgs.Maximum = new decimal(new int[] {
                                    500,
                                    0,
                                    0,
                                    0});
            this.sec_numBlocksMsgs.Name = "sec_numBlocksMsgs";
            this.sec_numBlocksMsgs.Size = new System.Drawing.Size(46, 21);
            this.sec_numBlocksMsgs.TabIndex = 30;
            this.sec_numBlocksMsgs.Value = new decimal(new int[] {
                                    200,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblBlocksOnMsgs
            // 
            this.sec_lblBlocksOnMsgs.AutoSize = true;
            this.sec_lblBlocksOnMsgs.Location = new System.Drawing.Point(93, 48);
            this.sec_lblBlocksOnMsgs.Name = "sec_lblBlocksOnMsgs";
            this.sec_lblBlocksOnMsgs.Size = new System.Drawing.Size(48, 13);
            this.sec_lblBlocksOnMsgs.TabIndex = 31;
            this.sec_lblBlocksOnMsgs.Text = "blocks in";
            // 
            // sec_numBlocksSecs
            // 
            this.sec_numBlocksSecs.Location = new System.Drawing.Point(142, 45);
            this.sec_numBlocksSecs.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.sec_numBlocksSecs.Name = "sec_numBlocksSecs";
            this.sec_numBlocksSecs.Size = new System.Drawing.Size(42, 21);
            this.sec_numBlocksSecs.TabIndex = 34;
            this.sec_numBlocksSecs.Value = new decimal(new int[] {
                                    5,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblBlocksOnSecs
            // 
            this.sec_lblBlocksOnSecs.AutoSize = true;
            this.sec_lblBlocksOnSecs.Location = new System.Drawing.Point(185, 48);
            this.sec_lblBlocksOnSecs.Name = "sec_lblBlocksOnSecs";
            this.sec_lblBlocksOnSecs.Size = new System.Drawing.Size(28, 13);
            this.sec_lblBlocksOnSecs.TabIndex = 33;
            this.sec_lblBlocksOnSecs.Text = "secs";
            // 
            // sec_gbCmd
            // 
            this.sec_gbCmd.Controls.Add(this.sec_cbCmdAuto);
            this.sec_gbCmd.Controls.Add(this.sec_lblCmdOnMute);
            this.sec_gbCmd.Controls.Add(this.sec_numCmdMsgs);
            this.sec_gbCmd.Controls.Add(this.sec_lblCmdOnMsgs);
            this.sec_gbCmd.Controls.Add(this.sec_numCmdSecs);
            this.sec_gbCmd.Controls.Add(this.sec_lblCmdOnSecs);
            this.sec_gbCmd.Controls.Add(this.sec_lblCmdForMute);
            this.sec_gbCmd.Controls.Add(this.sec_numCmdMute);
            this.sec_gbCmd.Controls.Add(this.sec_lblCmdForSecs);
            this.sec_gbCmd.Location = new System.Drawing.Point(14, 123);
            this.sec_gbCmd.Name = "sec_gbCmd";
            this.sec_gbCmd.Size = new System.Drawing.Size(238, 110);
            this.sec_gbCmd.TabIndex = 35;
            this.sec_gbCmd.TabStop = false;
            this.sec_gbCmd.Text = "Command spam control";
            // 
            // sec_cbCmdAuto
            // 
            this.sec_cbCmdAuto.AutoSize = true;
            this.sec_cbCmdAuto.Location = new System.Drawing.Point(10, 20);
            this.sec_cbCmdAuto.Name = "sec_cbCmdAuto";
            this.sec_cbCmdAuto.Size = new System.Drawing.Size(149, 17);
            this.sec_cbCmdAuto.TabIndex = 24;
            this.sec_cbCmdAuto.Text = "Enable automatic blocking";
            this.sec_cbCmdAuto.UseVisualStyleBackColor = true;
            this.sec_cbCmdAuto.CheckedChanged += new System.EventHandler(this.sec_cbCmdAuto_Checked);
            // 
            // sec_lblCmdOnMute
            // 
            this.sec_lblCmdOnMute.AutoSize = true;
            this.sec_lblCmdOnMute.Location = new System.Drawing.Point(6, 48);
            this.sec_lblCmdOnMute.Name = "sec_lblCmdOnMute";
            this.sec_lblCmdOnMute.Size = new System.Drawing.Size(46, 13);
            this.sec_lblCmdOnMute.TabIndex = 25;
            this.sec_lblCmdOnMute.Text = "Block on";
            // 
            // sec_numCmdMsgs
            // 
            this.sec_numCmdMsgs.Location = new System.Drawing.Point(53, 45);
            this.sec_numCmdMsgs.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.sec_numCmdMsgs.Name = "sec_numCmdMsgs";
            this.sec_numCmdMsgs.Size = new System.Drawing.Size(37, 21);
            this.sec_numCmdMsgs.TabIndex = 30;
            this.sec_numCmdMsgs.Value = new decimal(new int[] {
                                    25,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblCmdOnMsgs
            // 
            this.sec_lblCmdOnMsgs.AutoSize = true;
            this.sec_lblCmdOnMsgs.Location = new System.Drawing.Point(91, 48);
            this.sec_lblCmdOnMsgs.Name = "sec_lblCmdOnMsgs";
            this.sec_lblCmdOnMsgs.Size = new System.Drawing.Size(70, 13);
            this.sec_lblCmdOnMsgs.TabIndex = 31;
            this.sec_lblCmdOnMsgs.Text = "commands in";
            // 
            // sec_numCmdSecs
            // 
            this.sec_numCmdSecs.Location = new System.Drawing.Point(161, 45);
            this.sec_numCmdSecs.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.sec_numCmdSecs.Name = "sec_numCmdSecs";
            this.sec_numCmdSecs.Size = new System.Drawing.Size(42, 21);
            this.sec_numCmdSecs.TabIndex = 34;
            this.sec_numCmdSecs.Value = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblCmdOnSecs
            // 
            this.sec_lblCmdOnSecs.AutoSize = true;
            this.sec_lblCmdOnSecs.Location = new System.Drawing.Point(204, 48);
            this.sec_lblCmdOnSecs.Name = "sec_lblCmdOnSecs";
            this.sec_lblCmdOnSecs.Size = new System.Drawing.Size(28, 13);
            this.sec_lblCmdOnSecs.TabIndex = 33;
            this.sec_lblCmdOnSecs.Text = "secs";
            // 
            // sec_lblCmdForMute
            // 
            this.sec_lblCmdForMute.AutoSize = true;
            this.sec_lblCmdForMute.Location = new System.Drawing.Point(6, 83);
            this.sec_lblCmdForMute.Name = "sec_lblCmdForMute";
            this.sec_lblCmdForMute.Size = new System.Drawing.Size(47, 13);
            this.sec_lblCmdForMute.TabIndex = 25;
            this.sec_lblCmdForMute.Text = "Block for";
            // 
            // sec_numCmdMute
            // 
            this.sec_numCmdMute.Location = new System.Drawing.Point(53, 79);
            this.sec_numCmdMute.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.sec_numCmdMute.Name = "sec_numCmdMute";
            this.sec_numCmdMute.Size = new System.Drawing.Size(37, 21);
            this.sec_numCmdMute.TabIndex = 32;
            this.sec_numCmdMute.Value = new decimal(new int[] {
                                    60,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblCmdForSecs
            // 
            this.sec_lblCmdForSecs.AutoSize = true;
            this.sec_lblCmdForSecs.Location = new System.Drawing.Point(91, 83);
            this.sec_lblCmdForSecs.Name = "sec_lblCmdForSecs";
            this.sec_lblCmdForSecs.Size = new System.Drawing.Size(46, 13);
            this.sec_lblCmdForSecs.TabIndex = 33;
            this.sec_lblCmdForSecs.Text = "seconds";
            // 
            // sec_gbOther
            // 
            this.sec_gbOther.Controls.Add(this.sec_lblRank);
            this.sec_gbOther.Controls.Add(this.sec_cbWhitelist);
            this.sec_gbOther.Controls.Add(this.sec_cbLogNotes);
            this.sec_gbOther.Controls.Add(this.sec_cbVerifyAdmins);
            this.sec_gbOther.Controls.Add(this.sec_cmbVerifyRank);
            this.sec_gbOther.Location = new System.Drawing.Point(264, 6);
            this.sec_gbOther.Name = "sec_gbOther";
            this.sec_gbOther.Size = new System.Drawing.Size(217, 138);
            this.sec_gbOther.TabIndex = 2;
            this.sec_gbOther.TabStop = false;
            this.sec_gbOther.Text = "Other settings";
            // 
            // sec_lblRank
            // 
            this.sec_lblRank.AutoSize = true;
            this.sec_lblRank.Location = new System.Drawing.Point(33, 98);
            this.sec_lblRank.Name = "sec_lblRank";
            this.sec_lblRank.Size = new System.Drawing.Size(33, 13);
            this.sec_lblRank.TabIndex = 24;
            this.sec_lblRank.Text = "Rank:";
            // 
            // sec_gbChat
            // 
            this.sec_gbChat.Controls.Add(this.sec_cbChatAuto);
            this.sec_gbChat.Controls.Add(this.sec_lblChatOnMute);
            this.sec_gbChat.Controls.Add(this.sec_numChatMsgs);
            this.sec_gbChat.Controls.Add(this.sec_lblChatOnMsgs);
            this.sec_gbChat.Controls.Add(this.sec_numChatSecs);
            this.sec_gbChat.Controls.Add(this.sec_lblChatOnSecs);
            this.sec_gbChat.Controls.Add(this.sec_lblChatForMute);
            this.sec_gbChat.Controls.Add(this.sec_numChatMute);
            this.sec_gbChat.Controls.Add(this.sec_lblChatForSecs);
            this.sec_gbChat.Location = new System.Drawing.Point(14, 6);
            this.sec_gbChat.Name = "sec_gbChat";
            this.sec_gbChat.Size = new System.Drawing.Size(238, 111);
            this.sec_gbChat.TabIndex = 1;
            this.sec_gbChat.TabStop = false;
            this.sec_gbChat.Text = "Chat spam control";
            // 
            // sec_lblChatOnMute
            // 
            this.sec_lblChatOnMute.AutoSize = true;
            this.sec_lblChatOnMute.Location = new System.Drawing.Point(6, 48);
            this.sec_lblChatOnMute.Name = "sec_lblChatOnMute";
            this.sec_lblChatOnMute.Size = new System.Drawing.Size(46, 13);
            this.sec_lblChatOnMute.TabIndex = 25;
            this.sec_lblChatOnMute.Text = "Mute on";
            // 
            // sec_numChatMsgs
            // 
            this.sec_numChatMsgs.Location = new System.Drawing.Point(53, 45);
            this.sec_numChatMsgs.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.sec_numChatMsgs.Name = "sec_numChatMsgs";
            this.sec_numChatMsgs.Size = new System.Drawing.Size(37, 21);
            this.sec_numChatMsgs.TabIndex = 30;
            this.sec_numChatMsgs.Value = new decimal(new int[] {
                                    8,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblChatOnMsgs
            // 
            this.sec_lblChatOnMsgs.AutoSize = true;
            this.sec_lblChatOnMsgs.Location = new System.Drawing.Point(91, 48);
            this.sec_lblChatOnMsgs.Name = "sec_lblChatOnMsgs";
            this.sec_lblChatOnMsgs.Size = new System.Drawing.Size(65, 13);
            this.sec_lblChatOnMsgs.TabIndex = 31;
            this.sec_lblChatOnMsgs.Text = "messages in";
            // 
            // sec_numChatSecs
            // 
            this.sec_numChatSecs.Location = new System.Drawing.Point(156, 45);
            this.sec_numChatSecs.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.sec_numChatSecs.Name = "sec_numChatSecs";
            this.sec_numChatSecs.Size = new System.Drawing.Size(42, 21);
            this.sec_numChatSecs.TabIndex = 34;
            this.sec_numChatSecs.Value = new decimal(new int[] {
                                    5,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblChatOnSecs
            // 
            this.sec_lblChatOnSecs.AutoSize = true;
            this.sec_lblChatOnSecs.Location = new System.Drawing.Point(199, 48);
            this.sec_lblChatOnSecs.Name = "sec_lblChatOnSecs";
            this.sec_lblChatOnSecs.Size = new System.Drawing.Size(28, 13);
            this.sec_lblChatOnSecs.TabIndex = 33;
            this.sec_lblChatOnSecs.Text = "secs";
            // 
            // sec_lblChatForMute
            // 
            this.sec_lblChatForMute.AutoSize = true;
            this.sec_lblChatForMute.Location = new System.Drawing.Point(6, 83);
            this.sec_lblChatForMute.Name = "sec_lblChatForMute";
            this.sec_lblChatForMute.Size = new System.Drawing.Size(47, 13);
            this.sec_lblChatForMute.TabIndex = 25;
            this.sec_lblChatForMute.Text = "Mute for";
            // 
            // sec_numChatMute
            // 
            this.sec_numChatMute.Location = new System.Drawing.Point(53, 79);
            this.sec_numChatMute.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.sec_numChatMute.Name = "sec_numChatMute";
            this.sec_numChatMute.Size = new System.Drawing.Size(37, 21);
            this.sec_numChatMute.TabIndex = 32;
            this.sec_numChatMute.Value = new decimal(new int[] {
                                    60,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblChatForSecs
            // 
            this.sec_lblChatForSecs.AutoSize = true;
            this.sec_lblChatForSecs.Location = new System.Drawing.Point(91, 83);
            this.sec_lblChatForSecs.Name = "sec_lblChatForSecs";
            this.sec_lblChatForSecs.Size = new System.Drawing.Size(46, 13);
            this.sec_lblChatForSecs.TabIndex = 33;
            this.sec_lblChatForSecs.Text = "seconds";
            // 
            // PropertyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(507, 585);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnDiscard);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PropertyWindow";
            this.Text = "Properties";
            this.Load += new System.EventHandler(this.PropertyWindow_Load);
            this.Disposed += new System.EventHandler(this.PropertyWindow_Unload);
            this.tabChat.ResumeLayout(false);
            this.chat_gbTab.ResumeLayout(false);
            this.chat_gbTab.PerformLayout();
            this.chat_gbMessages.ResumeLayout(false);
            this.chat_gbMessages.PerformLayout();
            this.chat_gbOther.ResumeLayout(false);
            this.chat_gbOther.PerformLayout();
            this.chat_gbColors.ResumeLayout(false);
            this.chat_gbColors.PerformLayout();
            this.pageCommands.ResumeLayout(false);
            this.pageCommandsList.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.pageCommandsCustom.ResumeLayout(false);
            this.pageCommandsCustom.PerformLayout();
            this.groupBox24.ResumeLayout(false);
            this.groupBox24.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pageCommandPerms.ResumeLayout(false);
            this.pageCommandPerms.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.extracmdpermnumber)).EndInit();
            this.pageBlocks.ResumeLayout(false);
            this.pageBlocks.PerformLayout();
            this.pageRanks.ResumeLayout(false);
            this.pageRanks.PerformLayout();
            this.gbRankGeneral.ResumeLayout(false);
            this.gbRankGeneral.PerformLayout();
            this.gbRankSettings.ResumeLayout(false);
            this.gbRankSettings.PerformLayout();
            this.pageMisc.ResumeLayout(false);
            this.pageMisc.PerformLayout();
            this.economyGroupBox.ResumeLayout(false);
            this.grpExtra.ResumeLayout(false);
            this.grpExtra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldownTime)).EndInit();
            this.grpMessages.ResumeLayout(false);
            this.grpMessages.PerformLayout();
            this.grpPhysics.ResumeLayout(false);
            this.grpPhysics.PerformLayout();
            this.grpAFK.ResumeLayout(false);
            this.grpAFK.PerformLayout();
            this.grpBackups.ResumeLayout(false);
            this.grpBackups.PerformLayout();
            this.pageIRC.ResumeLayout(false);
            this.grpSQL.ResumeLayout(false);
            this.grpSQL.PerformLayout();
            this.grpIRC.ResumeLayout(false);
            this.grpIRC.PerformLayout();
            this.pageServer.ResumeLayout(false);
            this.grpLevels.ResumeLayout(false);
            this.grpLevels.PerformLayout();
            this.grpAdvanced.ResumeLayout(false);
            this.grpAdvanced.PerformLayout();
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.grpUpdate.ResumeLayout(false);
            this.grpUpdate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updateTimeNumeric)).EndInit();
            this.grpPlayers.ResumeLayout(false);
            this.grpPlayers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGuests)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.pageGames.ResumeLayout(false);
            this.tabGames.ResumeLayout(false);
            this.tabPage10.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.groupBox22.ResumeLayout(false);
            this.groupBox21.ResumeLayout(false);
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.tabPage14.ResumeLayout(false);
            this.tabPage14.PerformLayout();
            this.groupBox29.ResumeLayout(false);
            this.groupBox36.ResumeLayout(false);
            this.groupBox36.PerformLayout();
            this.groupBox35.ResumeLayout(false);
            this.groupBox34.ResumeLayout(false);
            this.groupBox33.ResumeLayout(false);
            this.groupBox33.PerformLayout();
            this.groupBox32.ResumeLayout(false);
            this.groupBox32.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsGraceTimeChck)).EndInit();
            this.groupBox31.ResumeLayout(false);
            this.groupBox31.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsMltiKlScPrUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsAstsScrUpDwn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsScrPrKlUpDwn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsScrLmtUpDwn)).EndInit();
            this.groupBox30.ResumeLayout(false);
            this.tabPage11.ResumeLayout(false);
            this.pageSecurity.ResumeLayout(false);
            this.sec_gbBlocks.ResumeLayout(false);
            this.sec_gbBlocks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numBlocksMsgs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numBlocksSecs)).EndInit();
            this.sec_gbCmd.ResumeLayout(false);
            this.sec_gbCmd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdMsgs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdSecs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdMute)).EndInit();
            this.sec_gbOther.ResumeLayout(false);
            this.sec_gbOther.PerformLayout();
            this.sec_gbChat.ResumeLayout(false);
            this.sec_gbChat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatMsgs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatSecs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatMute)).EndInit();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.Label sec_lblCmdForSecs;
        private System.Windows.Forms.NumericUpDown sec_numCmdMute;
        private System.Windows.Forms.Label sec_lblCmdForMute;
        private System.Windows.Forms.Label sec_lblCmdOnSecs;
        private System.Windows.Forms.NumericUpDown sec_numCmdSecs;
        private System.Windows.Forms.Label sec_lblCmdOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numCmdMsgs;
        private System.Windows.Forms.Label sec_lblCmdOnMute;
        private System.Windows.Forms.CheckBox sec_cbCmdAuto;
        private System.Windows.Forms.GroupBox sec_gbCmd;
        private System.Windows.Forms.Label sec_lblBlocksOnSecs;
        private System.Windows.Forms.NumericUpDown sec_numBlocksSecs;
        private System.Windows.Forms.Label sec_lblBlocksOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numBlocksMsgs;
        private System.Windows.Forms.Label sec_lblBlocksOnMute;
        private System.Windows.Forms.CheckBox sec_cbBlocksAuto;
        private System.Windows.Forms.GroupBox sec_gbBlocks;
        private System.Windows.Forms.PropertyGrid pg_lavaMap;
        private System.Windows.Forms.PropertyGrid pg_lava;
        private System.Windows.Forms.CheckBox sec_cbWhitelist;
        private System.Windows.Forms.Label sec_lblRank;
        private System.Windows.Forms.CheckBox chat_cbTabBots;
        private System.Windows.Forms.CheckBox chat_cbTabLevel;
        private System.Windows.Forms.CheckBox chat_cbTabRank;
        private System.Windows.Forms.GroupBox chat_gbTab;
        private System.Windows.Forms.ComboBox cmbOsMap;
        private System.Windows.Forms.Label lblOsMap;
        private System.Windows.Forms.CheckBox sec_cbLogNotes;
        private System.Windows.Forms.GroupBox sec_gbOther;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.TextBox txtOSMaps;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbRankSettings;
        private System.Windows.Forms.GroupBox gbRankGeneral;

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDiscard;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabPage pageBlocks;
        private System.Windows.Forms.Button btnBlHelp;
        private System.Windows.Forms.TextBox txtBlRanks;
        private System.Windows.Forms.TextBox txtBlAllow;
        private System.Windows.Forms.TextBox txtBlLowest;
        private System.Windows.Forms.TextBox txtBlDisallow;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ListBox listBlocks;
        private System.Windows.Forms.TabPage pageCommands;
        private System.Windows.Forms.TabControl pageCommandsList;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button btnCmdHelp;
        private System.Windows.Forms.TextBox txtCmdRanks;
        private System.Windows.Forms.TextBox txtCmdAllow;
        private System.Windows.Forms.TextBox txtCmdLowest;
        private System.Windows.Forms.TextBox txtCmdDisallow;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListBox listCommands;
        private System.Windows.Forms.TabPage pageCommandsCustom;
        private System.Windows.Forms.TabPage pageRanks;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.ComboBox cmbColor;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.TextBox txtLimit;
        private System.Windows.Forms.TextBox txtPermission;
        private System.Windows.Forms.TextBox txtRankName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnAddRank;
        private System.Windows.Forms.ListBox listRanks;
        private System.Windows.Forms.TabPage pageMisc;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox hackrank_kick_time;
        private System.Windows.Forms.TextBox chat_txtShutdown;
        private System.Windows.Forms.TextBox chat_txtBan;
        private System.Windows.Forms.TextBox txtNormRp;
        private System.Windows.Forms.TextBox txtRP;
        private System.Windows.Forms.TextBox txtAFKKick;
        private System.Windows.Forms.TextBox txtafk;
        private System.Windows.Forms.TextBox txtBackup;
        private System.Windows.Forms.TextBox txtBackupLocation;
        private System.Windows.Forms.TextBox txtMoneys;
        private System.Windows.Forms.TextBox chat_txtCheap;
        private System.Windows.Forms.TextBox txtRestartTime;
        private System.Windows.Forms.CheckBox hackrank_kick;
        private System.Windows.Forms.CheckBox chkProfanityFilter;
        private System.Windows.Forms.CheckBox chkRepeatMessages;
        private System.Windows.Forms.CheckBox chat_chkCheap;
        private System.Windows.Forms.CheckBox chkDeath;
        private System.Windows.Forms.CheckBox chk17Dollar;
        private System.Windows.Forms.CheckBox chkPhysicsRest;
        private System.Windows.Forms.CheckBox chkSmile;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.CheckBox chkRestartTime;
        private System.Windows.Forms.TabPage pageIRC;
        private System.Windows.Forms.TextBox txtOpChannel;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.TextBox txtIRCServer;
        private System.Windows.Forms.TextBox txtNick;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label chat_colIRC;
        private System.Windows.Forms.ComboBox chat_cmbIRC;
        private System.Windows.Forms.CheckBox chkIRC;
        private System.Windows.Forms.Label chat_lblIRC;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage pageServer;
        private System.Windows.Forms.NumericUpDown numPlayers;
        private System.Windows.Forms.NumericUpDown numGuests;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox txtMain;
        private System.Windows.Forms.TextBox chat_txtConsole;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtMOTD;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button ChkPort;
        private System.Windows.Forms.CheckBox chkLogBeat;
        private System.Windows.Forms.ComboBox cmbOpChat;
        private System.Windows.Forms.Label lblOpChat;
        private System.Windows.Forms.ComboBox cmbDefaultRank;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label chat_colDefault;
        private System.Windows.Forms.ComboBox chat_cmbDefault;
        private System.Windows.Forms.CheckBox chkRestart;
        private System.Windows.Forms.CheckBox chkPublic;
        private System.Windows.Forms.CheckBox chkAutoload;
        private System.Windows.Forms.CheckBox chkWorld;
        private System.Windows.Forms.CheckBox chkUpdates;
        private System.Windows.Forms.CheckBox chkVerify;
        private System.Windows.Forms.Label chat_lblDefault;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label chat_lblConsole;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.CheckBox chkAgreeToRules;
        private System.Windows.Forms.ComboBox cmbAdminChat;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.CheckBox chkAdminsJoinSilent;
        private System.Windows.Forms.Button editTxtsBt;
        private System.Windows.Forms.CheckBox chkTpToHigherRanks;
        private System.Windows.Forms.GroupBox grpIRC;
        private System.Windows.Forms.GroupBox grpGeneral;
        private System.Windows.Forms.GroupBox grpAdvanced;
        private System.Windows.Forms.GroupBox grpPlayers;
        private System.Windows.Forms.GroupBox grpLevels;
        private System.Windows.Forms.TextBox txtServerOwner;
        private System.Windows.Forms.GroupBox grpExtra;
        private System.Windows.Forms.GroupBox grpMessages;
        private System.Windows.Forms.GroupBox grpPhysics;
        private System.Windows.Forms.GroupBox grpAFK;
        private System.Windows.Forms.GroupBox grpBackups;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        public System.Windows.Forms.Button btnUnload;
        public System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.Button btnCreate;
        public System.Windows.Forms.Label label33;
        public System.Windows.Forms.TextBox txtCommandName;
        private System.Windows.Forms.GroupBox grpSQL;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox txtSQLHost;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.TextBox txtSQLDatabase;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.TextBox txtSQLPassword;
        private System.Windows.Forms.TextBox txtSQLUsername;
        private System.Windows.Forms.CheckBox chkUseSQL;
        private System.Windows.Forms.TabPage pageSecurity;
        private System.Windows.Forms.GroupBox sec_gbChat;
        private System.Windows.Forms.Label sec_lblChatForMute;
        private System.Windows.Forms.NumericUpDown sec_numChatMute;
        private System.Windows.Forms.Label sec_lblChatOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numChatMsgs;
        private System.Windows.Forms.Label sec_lblChatOnMute;
        private System.Windows.Forms.CheckBox sec_cbChatAuto;
        private System.Windows.Forms.CheckBox sec_cbVerifyAdmins;
        private System.Windows.Forms.ComboBox sec_cmbVerifyRank;
        private System.Windows.Forms.TabPage pageGames;
        private System.Windows.Forms.TabControl tabGames;
        private System.Windows.Forms.TabPage tabPage11;
        private System.Windows.Forms.TabPage tabPage10;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox txtIrcId;
        private System.Windows.Forms.CheckBox chkIrcId;
        private System.Windows.Forms.CheckBox irc_cbTitles;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.TextBox txtIRCPort;
        private System.Windows.Forms.CheckBox chkShowEmptyRanks;
        private System.Windows.Forms.TextBox txtMaxUndo;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.GroupBox groupBox20;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Button lsAddMap;
        private System.Windows.Forms.Button lsRemoveMap;
        private System.Windows.Forms.ListBox lsMapNoUse;
        private System.Windows.Forms.ListBox lsMapUse;
        private System.Windows.Forms.GroupBox groupBox21;
        private System.Windows.Forms.GroupBox groupBox22;
        private System.Windows.Forms.GroupBox groupBox23;
        private System.Windows.Forms.Button lsBtnEndRound;
        private System.Windows.Forms.Button lsBtnStopGame;
        private System.Windows.Forms.Button lsBtnStartGame;
        private System.Windows.Forms.Button lsBtnEndVote;
        private System.Windows.Forms.NumericUpDown sec_numChatSecs;
        private System.Windows.Forms.Label sec_lblChatOnSecs;
        private System.Windows.Forms.Label sec_lblChatForSecs;
        private System.Windows.Forms.TextBox txtSQLPort;
        private System.Windows.Forms.Label label70;
        private System.Windows.Forms.GroupBox grpUpdate;
        private System.Windows.Forms.CheckBox autoUpdate;
        private System.Windows.Forms.NumericUpDown updateTimeNumeric;
        private System.Windows.Forms.Label lblUpdateSeconds;
        private System.Windows.Forms.CheckBox notifyInGameUpdate;
        private System.Windows.Forms.Button forceUpdateBtn;
        private System.Windows.Forms.TabPage pageCommandPerms;
        private System.Windows.Forms.ListBox listCommandsExtraCmdPerms;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.Label label72;
        public System.Windows.Forms.NumericUpDown extracmdpermnumber;
        public System.Windows.Forms.TextBox extracmdpermdesc;
        public System.Windows.Forms.TextBox extracmdpermperm;
        private System.Windows.Forms.TextBox txtcmdranks2;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.ComboBox cmbAFKKickPerm;
        private System.Windows.Forms.CheckBox chkGuestLimitNotify;
        private System.Windows.Forms.NumericUpDown nudCooldownTime;
        private System.Windows.Forms.Label label84;
        private System.Windows.Forms.Label lblMOTD;
        private System.Windows.Forms.TextBox txtGrpMOTD;
        private System.Windows.Forms.TabPage tabPage14;
        private System.Windows.Forms.TextBox SlctdTntWrdStatus;
        private System.Windows.Forms.Label label86;
        private System.Windows.Forms.Label label85;
        private System.Windows.Forms.TextBox SlctdTntWrsLvl;
        private System.Windows.Forms.GroupBox groupBox29;
        private System.Windows.Forms.Button EditTntWarsGameBT;
        private System.Windows.Forms.ListBox TntWarsGamesList;
        private System.Windows.Forms.TextBox SlctdTntWrsPlyrs;
        private System.Windows.Forms.Label label87;
        private System.Windows.Forms.GroupBox groupBox30;
        private System.Windows.Forms.Button TntWrsStrtGame;
        private System.Windows.Forms.Button TntWrsDltGame;
        private System.Windows.Forms.Button TntWrsEndGame;
        private System.Windows.Forms.Button TntWrsRstGame;
        private System.Windows.Forms.GroupBox groupBox31;
        private System.Windows.Forms.CheckBox TntWrsAsstChck;
        private System.Windows.Forms.NumericUpDown TntWrsAstsScrUpDwn;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.NumericUpDown TntWrsScrPrKlUpDwn;
        private System.Windows.Forms.Label label88;
        private System.Windows.Forms.NumericUpDown TntWrsScrLmtUpDwn;
        private System.Windows.Forms.CheckBox TntWrsMltiKlChck;
        private System.Windows.Forms.NumericUpDown TntWrsMltiKlScPrUpDown;
        private System.Windows.Forms.GroupBox groupBox32;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.NumericUpDown TntWrsGraceTimeChck;
        private System.Windows.Forms.GroupBox groupBox33;
        private System.Windows.Forms.CheckBox TntWrsTKchck;
        private System.Windows.Forms.CheckBox TntWrsBlnceTeamsChck;
        private System.Windows.Forms.CheckBox TntWrsTmsChck;
        private System.Windows.Forms.CheckBox TntWrsGracePrdChck;
        private System.Windows.Forms.GroupBox groupBox34;
        private System.Windows.Forms.Button TntWrsDiffAboutBt;
        private System.Windows.Forms.ComboBox TntWrsDiffCombo;
        private System.Windows.Forms.GroupBox groupBox35;
        private System.Windows.Forms.Button TntWrsCrtNwTntWrsBt;
        private System.Windows.Forms.GroupBox groupBox36;
        private System.Windows.Forms.ListBox TntWrsMpsList;
        private System.Windows.Forms.Button TntWrsDiffSlctBt;
        private System.Windows.Forms.CheckBox TntWrsStreaksChck;
        private System.Windows.Forms.Button buttonEco;
        private System.Windows.Forms.Label lblLoadedCommands;
        private System.Windows.Forms.ListBox lstCommands;
        private System.Windows.Forms.GroupBox groupBox24;
        private System.Windows.Forms.GroupBox chat_gbMessages;
        private System.Windows.Forms.GroupBox chat_gbOther;
        private System.Windows.Forms.GroupBox chat_gbColors;
        private System.Windows.Forms.TabPage tabChat;
        private System.Windows.Forms.ComboBox chat_cmbDesc;
        private System.Windows.Forms.Label chat_lblDesc;
        private System.Windows.Forms.Label chat_lblSyntax;
        private System.Windows.Forms.ComboBox chat_cmbSyntax;
        private System.Windows.Forms.TextBox chat_txtDemote;
        private System.Windows.Forms.TextBox chat_txtPromote;
        private System.Windows.Forms.Label chat_lblShutdown;
        private System.Windows.Forms.Label chat_colSyntax;
        private System.Windows.Forms.Label chat_colDesc;
        private System.Windows.Forms.Label chat_lblDemote;
        private System.Windows.Forms.Label chat_lblPromote;
        private System.Windows.Forms.Label chat_lblBan;
        private System.Windows.Forms.PropertyGrid propsZG;
        private System.Windows.Forms.GroupBox economyGroupBox;
    }
}