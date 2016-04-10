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
            this.chat_chkGlobal = new System.Windows.Forms.CheckBox();
            this.chat_lblConsole = new System.Windows.Forms.Label();
            this.chat_txtConsole = new System.Windows.Forms.TextBox();
            this.chat_gbColors = new System.Windows.Forms.GroupBox();
            this.chat_colDesc = new System.Windows.Forms.Label();
            this.chat_cmbDesc = new System.Windows.Forms.ComboBox();
            this.chat_lblDesc = new System.Windows.Forms.Label();
            this.chat_colSyntax = new System.Windows.Forms.Label();
            this.chat_cmbSyntax = new System.Windows.Forms.ComboBox();
            this.chat_lblSyntax = new System.Windows.Forms.Label();
            this.chat_colGlobal = new System.Windows.Forms.Label();
            this.chat_cmbGlobal = new System.Windows.Forms.ComboBox();
            this.chat_lblGlobal = new System.Windows.Forms.Label();
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
            this.chkForceCuboid = new System.Windows.Forms.CheckBox();
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
            this.cmbVerificationRank = new System.Windows.Forms.ComboBox();
            this.chkEnableVerification = new System.Windows.Forms.CheckBox();
            this.chkSpamControl = new System.Windows.Forms.CheckBox();
            this.lsChkSendAFKMain = new System.Windows.Forms.CheckBox();
            this.lsChkStartOnStartup = new System.Windows.Forms.CheckBox();
            this.lsNudVoteCount = new System.Windows.Forms.NumericUpDown();
            this.lsNudVoteTime = new System.Windows.Forms.NumericUpDown();
            this.lsCmbSetupRank = new System.Windows.Forms.ComboBox();
            this.lsNudFastLava = new System.Windows.Forms.NumericUpDown();
            this.lsNudKiller = new System.Windows.Forms.NumericUpDown();
            this.lsNudDestroy = new System.Windows.Forms.NumericUpDown();
            this.lsNudWater = new System.Windows.Forms.NumericUpDown();
            this.lsNudLayer = new System.Windows.Forms.NumericUpDown();
            this.lsNudLayerHeight = new System.Windows.Forms.NumericUpDown();
            this.lsNudLayerCount = new System.Windows.Forms.NumericUpDown();
            this.lsNudLayerTime = new System.Windows.Forms.NumericUpDown();
            this.lsNudRoundTime = new System.Windows.Forms.NumericUpDown();
            this.lsNudFloodTime = new System.Windows.Forms.NumericUpDown();
            this.lsCmbControlRank = new System.Windows.Forms.ComboBox();
            this.lsNudLives = new System.Windows.Forms.NumericUpDown();
            this.cmbAFKKickPerm = new System.Windows.Forms.ComboBox();
            this.chkGuestLimitNotify = new System.Windows.Forms.CheckBox();
            this.cmbAdminChat = new System.Windows.Forms.ComboBox();
            this.cmbOpChat = new System.Windows.Forms.ComboBox();
            this.chkTpToHigherRanks = new System.Windows.Forms.CheckBox();
            this.cmbDefaultRank = new System.Windows.Forms.ComboBox();
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
            this.lsBtnSaveSettings = new System.Windows.Forms.Button();
            this.label67 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.label63 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.label75 = new System.Windows.Forms.Label();
            this.label68 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
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
            this.gbSpamSettings = new System.Windows.Forms.GroupBox();
            this.numCountReset = new System.Windows.Forms.NumericUpDown();
            this.label69 = new System.Windows.Forms.Label();
            this.numSpamMute = new System.Windows.Forms.NumericUpDown();
            this.label45 = new System.Windows.Forms.Label();
            this.numSpamMessages = new System.Windows.Forms.NumericUpDown();
            this.label44 = new System.Windows.Forms.Label();
            this.gbPasswords = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.listPasswords = new System.Windows.Forms.ListBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.pageReview = new System.Windows.Forms.TabPage();
            this.gbReviewOptions = new System.Windows.Forms.GroupBox();
            this.nudCooldownTime = new System.Windows.Forms.NumericUpDown();
            this.label84 = new System.Windows.Forms.Label();
            this.gbReviewActions = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.gbReviewPerms = new System.Windows.Forms.GroupBox();
            this.label83 = new System.Windows.Forms.Label();
            this.label82 = new System.Windows.Forms.Label();
            this.label81 = new System.Windows.Forms.Label();
            this.label80 = new System.Windows.Forms.Label();
            this.cmbGotoNext = new System.Windows.Forms.ComboBox();
            this.cmbClearQueue = new System.Windows.Forms.ComboBox();
            this.cmbLeaveQueue = new System.Windows.Forms.ComboBox();
            this.cmbEnterQueue = new System.Windows.Forms.ComboBox();
            this.cmbViewQueue = new System.Windows.Forms.ComboBox();
            this.label79 = new System.Windows.Forms.Label();
            this.gbReviewQueue = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabChat.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.lsNudVoteCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudVoteTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudFastLava)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudKiller)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudDestroy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudWater)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLayerHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLayerCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLayerTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudRoundTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudFloodTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLives)).BeginInit();
            this.pageBlocks.SuspendLayout();
            this.pageRanks.SuspendLayout();
            this.gbRankGeneral.SuspendLayout();
            this.gbRankSettings.SuspendLayout();
            this.pageMisc.SuspendLayout();
            this.economyGroupBox.SuspendLayout();
            this.grpExtra.SuspendLayout();
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
            this.gbSpamSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCountReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpamMute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpamMessages)).BeginInit();
            this.gbPasswords.SuspendLayout();
            this.pageReview.SuspendLayout();
            this.gbReviewOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldownTime)).BeginInit();
            this.gbReviewActions.SuspendLayout();
            this.gbReviewPerms.SuspendLayout();
            this.gbReviewQueue.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabChat
            // 
            this.tabChat.BackColor = System.Drawing.Color.White;
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
            this.chat_gbMessages.Location = new System.Drawing.Point(8, 187);
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
            this.chat_gbOther.Controls.Add(this.chat_chkGlobal);
            this.chat_gbOther.Controls.Add(this.chat_lblConsole);
            this.chat_gbOther.Controls.Add(this.chat_txtConsole);
            this.chat_gbOther.Location = new System.Drawing.Point(235, 6);
            this.chat_gbOther.Name = "chat_gbOther";
            this.chat_gbOther.Size = new System.Drawing.Size(256, 175);
            this.chat_gbOther.TabIndex = 1;
            this.chat_gbOther.TabStop = false;
            this.chat_gbOther.Text = "Other";
            // 
            // chat_chkGlobal
            // 
            this.chat_chkGlobal.AutoSize = true;
            this.chat_chkGlobal.Location = new System.Drawing.Point(9, 46);
            this.chat_chkGlobal.Name = "chat_chkGlobal";
            this.chat_chkGlobal.Size = new System.Drawing.Size(101, 17);
            this.chat_chkGlobal.TabIndex = 5;
            this.chat_chkGlobal.Text = "Use Global Chat";
            this.chat_chkGlobal.UseVisualStyleBackColor = true;
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
            this.chat_gbColors.Controls.Add(this.chat_colGlobal);
            this.chat_gbColors.Controls.Add(this.chat_cmbGlobal);
            this.chat_gbColors.Controls.Add(this.chat_lblGlobal);
            this.chat_gbColors.Controls.Add(this.chat_colIRC);
            this.chat_gbColors.Controls.Add(this.chat_cmbIRC);
            this.chat_gbColors.Controls.Add(this.chat_lblIRC);
            this.chat_gbColors.Controls.Add(this.chat_colDefault);
            this.chat_gbColors.Controls.Add(this.chat_cmbDefault);
            this.chat_gbColors.Controls.Add(this.chat_lblDefault);
            this.chat_gbColors.Location = new System.Drawing.Point(8, 6);
            this.chat_gbColors.Name = "chat_gbColors";
            this.chat_gbColors.Size = new System.Drawing.Size(221, 175);
            this.chat_gbColors.TabIndex = 0;
            this.chat_gbColors.TabStop = false;
            this.chat_gbColors.Text = "Colors";
            // 
            // chat_colDesc
            // 
            this.chat_colDesc.Location = new System.Drawing.Point(99, 125);
            this.chat_colDesc.Name = "chat_colDesc";
            this.chat_colDesc.Size = new System.Drawing.Size(18, 18);
            this.chat_colDesc.TabIndex = 34;
            // 
            // chat_cmbDesc
            // 
            this.chat_cmbDesc.FormattingEnabled = true;
            this.chat_cmbDesc.Location = new System.Drawing.Point(120, 125);
            this.chat_cmbDesc.Name = "chat_cmbDesc";
            this.chat_cmbDesc.Size = new System.Drawing.Size(95, 21);
            this.chat_cmbDesc.TabIndex = 33;
            this.toolTip.SetToolTip(this.chat_cmbDesc, "The colour of the /cmdname [args] in /help.");
            this.chat_cmbDesc.SelectedIndexChanged += new System.EventHandler(this.chat_cmbDesc_SelectedIndexChanged);
            // 
            // chat_lblDesc
            // 
            this.chat_lblDesc.AutoSize = true;
            this.chat_lblDesc.Location = new System.Drawing.Point(6, 128);
            this.chat_lblDesc.Name = "chat_lblDesc";
            this.chat_lblDesc.Size = new System.Drawing.Size(90, 13);
            this.chat_lblDesc.TabIndex = 32;
            this.chat_lblDesc.Text = "/help description:";
            // 
            // chat_colSyntax
            // 
            this.chat_colSyntax.Location = new System.Drawing.Point(99, 98);
            this.chat_colSyntax.Name = "chat_colSyntax";
            this.chat_colSyntax.Size = new System.Drawing.Size(18, 18);
            this.chat_colSyntax.TabIndex = 35;
            // 
            // chat_cmbSyntax
            // 
            this.chat_cmbSyntax.FormattingEnabled = true;
            this.chat_cmbSyntax.Location = new System.Drawing.Point(120, 98);
            this.chat_cmbSyntax.Name = "chat_cmbSyntax";
            this.chat_cmbSyntax.Size = new System.Drawing.Size(95, 21);
            this.chat_cmbSyntax.TabIndex = 30;
            this.toolTip.SetToolTip(this.chat_cmbSyntax, "The colour for the description of a command in /help.");
            this.chat_cmbSyntax.SelectedIndexChanged += new System.EventHandler(this.chat_cmbSyntax_SelectedIndexChanged);
            // 
            // chat_lblSyntax
            // 
            this.chat_lblSyntax.AutoSize = true;
            this.chat_lblSyntax.Location = new System.Drawing.Point(6, 101);
            this.chat_lblSyntax.Name = "chat_lblSyntax";
            this.chat_lblSyntax.Size = new System.Drawing.Size(68, 13);
            this.chat_lblSyntax.TabIndex = 31;
            this.chat_lblSyntax.Text = "/help syntax:";
            // 
            // chat_colGlobal
            // 
            this.chat_colGlobal.BackColor = System.Drawing.Color.Black;
            this.chat_colGlobal.Location = new System.Drawing.Point(99, 71);
            this.chat_colGlobal.Name = "chat_colGlobal";
            this.chat_colGlobal.Size = new System.Drawing.Size(18, 18);
            this.chat_colGlobal.TabIndex = 28;
            // 
            // chat_cmbGlobal
            // 
            this.chat_cmbGlobal.FormattingEnabled = true;
            this.chat_cmbGlobal.Location = new System.Drawing.Point(120, 71);
            this.chat_cmbGlobal.Name = "chat_cmbGlobal";
            this.chat_cmbGlobal.Size = new System.Drawing.Size(95, 21);
            this.chat_cmbGlobal.TabIndex = 27;
            this.toolTip.SetToolTip(this.chat_cmbGlobal, "The color of Global Chat text in-game.");
            this.chat_cmbGlobal.SelectedIndexChanged += new System.EventHandler(this.chat_cmbGlobal_SelectedIndexChanged);
            // 
            // chat_lblGlobal
            // 
            this.chat_lblGlobal.AutoSize = true;
            this.chat_lblGlobal.Location = new System.Drawing.Point(6, 74);
            this.chat_lblGlobal.Name = "chat_lblGlobal";
            this.chat_lblGlobal.Size = new System.Drawing.Size(64, 13);
            this.chat_lblGlobal.TabIndex = 29;
            this.chat_lblGlobal.Text = "Global chat:";
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
            this.pageCommandsCustom.Text = "Custom Commands";
            // 
            // lblLoadedCommands
            // 
            this.lblLoadedCommands.AutoSize = true;
            this.lblLoadedCommands.Location = new System.Drawing.Point(7, 120);
            this.lblLoadedCommands.Name = "lblLoadedCommands";
            this.lblLoadedCommands.Size = new System.Drawing.Size(97, 13);
            this.lblLoadedCommands.TabIndex = 40;
            this.lblLoadedCommands.Text = "Loaded Commands";
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
            this.groupBox24.Text = "Quick Command";
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
            this.btnCreate.Text = "Create Command";
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
            this.pageCommandPerms.Text = "Additional Command Permissions";
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
            this.label73.Size = new System.Drawing.Size(179, 13);
            this.label73.TabIndex = 44;
            this.label73.Text = "Command Extra Permission Number:";
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
            // chkForceCuboid
            // 
            this.chkForceCuboid.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkForceCuboid.AutoSize = true;
            this.chkForceCuboid.Location = new System.Drawing.Point(523, 266);
            this.chkForceCuboid.Name = "chkForceCuboid";
            this.chkForceCuboid.Size = new System.Drawing.Size(78, 23);
            this.chkForceCuboid.TabIndex = 29;
            this.chkForceCuboid.Text = "Force Cuboid";
            this.toolTip.SetToolTip(this.chkForceCuboid, "When true, runs an attempted cuboid despite cuboid limits, until it hits the grou" +
        "p limit for that user.");
            this.chkForceCuboid.UseVisualStyleBackColor = true;
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
            this.chkWorld.Size = new System.Drawing.Size(78, 17);
            this.chkWorld.TabIndex = 4;
            this.chkWorld.Text = "World chat";
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
            // cmbVerificationRank
            // 
            this.cmbVerificationRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVerificationRank.FormattingEnabled = true;
            this.cmbVerificationRank.Location = new System.Drawing.Point(60, 64);
            this.cmbVerificationRank.Name = "cmbVerificationRank";
            this.cmbVerificationRank.Size = new System.Drawing.Size(103, 21);
            this.cmbVerificationRank.TabIndex = 22;
            this.toolTip.SetToolTip(this.cmbVerificationRank, "The rank that verification is required for admins to gain access to commands.");
            // 
            // chkEnableVerification
            // 
            this.chkEnableVerification.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkEnableVerification.AutoSize = true;
            this.chkEnableVerification.Location = new System.Drawing.Point(24, 20);
            this.chkEnableVerification.Name = "chkEnableVerification";
            this.chkEnableVerification.Size = new System.Drawing.Size(49, 23);
            this.chkEnableVerification.TabIndex = 23;
            this.chkEnableVerification.Text = "Enable";
            this.toolTip.SetToolTip(this.chkEnableVerification, "Whether or not the server will ask for verification from admins before they can u" +
        "se commands.");
            this.chkEnableVerification.UseVisualStyleBackColor = true;
            // 
            // chkSpamControl
            // 
            this.chkSpamControl.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkSpamControl.AutoSize = true;
            this.chkSpamControl.Location = new System.Drawing.Point(15, 20);
            this.chkSpamControl.Name = "chkSpamControl";
            this.chkSpamControl.Size = new System.Drawing.Size(49, 23);
            this.chkSpamControl.TabIndex = 24;
            this.chkSpamControl.Text = "Enable";
            this.toolTip.SetToolTip(this.chkSpamControl, "If enabled it mutes a player for spamming. Default false.\r\n");
            this.chkSpamControl.UseVisualStyleBackColor = true;
            // 
            // lsChkSendAFKMain
            // 
            this.lsChkSendAFKMain.Appearance = System.Windows.Forms.Appearance.Button;
            this.lsChkSendAFKMain.AutoSize = true;
            this.lsChkSendAFKMain.Location = new System.Drawing.Point(19, 49);
            this.lsChkSendAFKMain.Name = "lsChkSendAFKMain";
            this.lsChkSendAFKMain.Size = new System.Drawing.Size(134, 23);
            this.lsChkSendAFKMain.TabIndex = 1;
            this.lsChkSendAFKMain.Text = "Send AFK Players To Main";
            this.toolTip.SetToolTip(this.lsChkSendAFKMain, "Send AFK players to the main map on a map change?");
            this.lsChkSendAFKMain.UseVisualStyleBackColor = true;
            // 
            // lsChkStartOnStartup
            // 
            this.lsChkStartOnStartup.Appearance = System.Windows.Forms.Appearance.Button;
            this.lsChkStartOnStartup.AutoSize = true;
            this.lsChkStartOnStartup.Location = new System.Drawing.Point(8, 20);
            this.lsChkStartOnStartup.Name = "lsChkStartOnStartup";
            this.lsChkStartOnStartup.Size = new System.Drawing.Size(156, 23);
            this.lsChkStartOnStartup.TabIndex = 0;
            this.lsChkStartOnStartup.Text = "Start Round On Server Startup";
            this.toolTip.SetToolTip(this.lsChkStartOnStartup, "Start Lava Survival when the server starts up?");
            this.lsChkStartOnStartup.UseVisualStyleBackColor = true;
            // 
            // lsNudVoteCount
            // 
            this.lsNudVoteCount.Location = new System.Drawing.Point(71, 78);
            this.lsNudVoteCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.lsNudVoteCount.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.lsNudVoteCount.Name = "lsNudVoteCount";
            this.lsNudVoteCount.Size = new System.Drawing.Size(95, 21);
            this.lsNudVoteCount.TabIndex = 3;
            this.toolTip.SetToolTip(this.lsNudVoteCount, "How many maps to put in the next map vote.");
            this.lsNudVoteCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lsNudVoteTime
            // 
            this.lsNudVoteTime.Location = new System.Drawing.Point(71, 106);
            this.lsNudVoteTime.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.lsNudVoteTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lsNudVoteTime.Name = "lsNudVoteTime";
            this.lsNudVoteTime.Size = new System.Drawing.Size(95, 21);
            this.lsNudVoteTime.TabIndex = 5;
            this.toolTip.SetToolTip(this.lsNudVoteTime, "Time until the next map vote ends.");
            this.lsNudVoteTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lsCmbSetupRank
            // 
            this.lsCmbSetupRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lsCmbSetupRank.FormattingEnabled = true;
            this.lsCmbSetupRank.Location = new System.Drawing.Point(71, 160);
            this.lsCmbSetupRank.Name = "lsCmbSetupRank";
            this.lsCmbSetupRank.Size = new System.Drawing.Size(95, 21);
            this.lsCmbSetupRank.TabIndex = 7;
            this.toolTip.SetToolTip(this.lsCmbSetupRank, "Minimum rank required to configure Lava Survival.");
            // 
            // lsNudFastLava
            // 
            this.lsNudFastLava.Location = new System.Drawing.Point(67, 20);
            this.lsNudFastLava.Name = "lsNudFastLava";
            this.lsNudFastLava.Size = new System.Drawing.Size(61, 21);
            this.lsNudFastLava.TabIndex = 1;
            this.toolTip.SetToolTip(this.lsNudFastLava, "Percent chance for fast lava.");
            // 
            // lsNudKiller
            // 
            this.lsNudKiller.Location = new System.Drawing.Point(67, 47);
            this.lsNudKiller.Name = "lsNudKiller";
            this.lsNudKiller.Size = new System.Drawing.Size(61, 21);
            this.lsNudKiller.TabIndex = 3;
            this.toolTip.SetToolTip(this.lsNudKiller, "Percent chance for killer lava/water.");
            // 
            // lsNudDestroy
            // 
            this.lsNudDestroy.Location = new System.Drawing.Point(67, 74);
            this.lsNudDestroy.Name = "lsNudDestroy";
            this.lsNudDestroy.Size = new System.Drawing.Size(61, 21);
            this.lsNudDestroy.TabIndex = 5;
            this.toolTip.SetToolTip(this.lsNudDestroy, "Percent chance for lava/water to destroy blocks.");
            // 
            // lsNudWater
            // 
            this.lsNudWater.Location = new System.Drawing.Point(67, 101);
            this.lsNudWater.Name = "lsNudWater";
            this.lsNudWater.Size = new System.Drawing.Size(61, 21);
            this.lsNudWater.TabIndex = 7;
            this.toolTip.SetToolTip(this.lsNudWater, "Percent chance for water flood.");
            // 
            // lsNudLayer
            // 
            this.lsNudLayer.Location = new System.Drawing.Point(67, 128);
            this.lsNudLayer.Name = "lsNudLayer";
            this.lsNudLayer.Size = new System.Drawing.Size(61, 21);
            this.lsNudLayer.TabIndex = 9;
            this.toolTip.SetToolTip(this.lsNudLayer, "Percent chance for layer flood.");
            // 
            // lsNudLayerHeight
            // 
            this.lsNudLayerHeight.Location = new System.Drawing.Point(211, 20);
            this.lsNudLayerHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lsNudLayerHeight.Name = "lsNudLayerHeight";
            this.lsNudLayerHeight.Size = new System.Drawing.Size(61, 21);
            this.lsNudLayerHeight.TabIndex = 12;
            this.toolTip.SetToolTip(this.lsNudLayerHeight, "Height of each layer.");
            this.lsNudLayerHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lsNudLayerCount
            // 
            this.lsNudLayerCount.Location = new System.Drawing.Point(211, 47);
            this.lsNudLayerCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.lsNudLayerCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lsNudLayerCount.Name = "lsNudLayerCount";
            this.lsNudLayerCount.Size = new System.Drawing.Size(61, 21);
            this.lsNudLayerCount.TabIndex = 14;
            this.toolTip.SetToolTip(this.lsNudLayerCount, "Number of layers to flood.");
            this.lsNudLayerCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lsNudLayerTime
            // 
            this.lsNudLayerTime.Location = new System.Drawing.Point(211, 74);
            this.lsNudLayerTime.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.lsNudLayerTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lsNudLayerTime.Name = "lsNudLayerTime";
            this.lsNudLayerTime.Size = new System.Drawing.Size(61, 21);
            this.lsNudLayerTime.TabIndex = 16;
            this.toolTip.SetToolTip(this.lsNudLayerTime, "Time between each layer.");
            this.lsNudLayerTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lsNudRoundTime
            // 
            this.lsNudRoundTime.Location = new System.Drawing.Point(211, 102);
            this.lsNudRoundTime.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.lsNudRoundTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lsNudRoundTime.Name = "lsNudRoundTime";
            this.lsNudRoundTime.Size = new System.Drawing.Size(61, 21);
            this.lsNudRoundTime.TabIndex = 18;
            this.toolTip.SetToolTip(this.lsNudRoundTime, "Time until the round ends.");
            this.lsNudRoundTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lsNudFloodTime
            // 
            this.lsNudFloodTime.Location = new System.Drawing.Point(211, 128);
            this.lsNudFloodTime.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.lsNudFloodTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lsNudFloodTime.Name = "lsNudFloodTime";
            this.lsNudFloodTime.Size = new System.Drawing.Size(61, 21);
            this.lsNudFloodTime.TabIndex = 20;
            this.toolTip.SetToolTip(this.lsNudFloodTime, "Time until the map is flooded.");
            this.lsNudFloodTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lsCmbControlRank
            // 
            this.lsCmbControlRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lsCmbControlRank.FormattingEnabled = true;
            this.lsCmbControlRank.Location = new System.Drawing.Point(84, 187);
            this.lsCmbControlRank.Name = "lsCmbControlRank";
            this.lsCmbControlRank.Size = new System.Drawing.Size(82, 21);
            this.lsCmbControlRank.TabIndex = 9;
            this.toolTip.SetToolTip(this.lsCmbControlRank, "Minimum rank required to administrate Lava Survival.");
            // 
            // lsNudLives
            // 
            this.lsNudLives.Location = new System.Drawing.Point(71, 133);
            this.lsNudLives.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.lsNudLives.Name = "lsNudLives";
            this.lsNudLives.Size = new System.Drawing.Size(95, 21);
            this.lsNudLives.TabIndex = 10;
            this.toolTip.SetToolTip(this.lsNudLives, "The number of times a player can die before being out of the round. Set to 0 for " +
        "unlimited.");
            this.lsNudLives.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
            this.chkTpToHigherRanks.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkTpToHigherRanks.AutoSize = true;
            this.chkTpToHigherRanks.Location = new System.Drawing.Point(11, 142);
            this.chkTpToHigherRanks.Name = "chkTpToHigherRanks";
            this.chkTpToHigherRanks.Size = new System.Drawing.Size(127, 23);
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
            // pageBlocks
            // 
            this.pageBlocks.BackColor = System.Drawing.Color.Transparent;
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
            this.pageRanks.BackColor = System.Drawing.Color.Transparent;
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
            this.gbRankGeneral.Size = new System.Drawing.Size(191, 185);
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
            this.chkAdminsJoinSilent.Location = new System.Drawing.Point(14, 119);
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
            this.gbRankSettings.Size = new System.Drawing.Size(197, 250);
            this.gbRankSettings.TabIndex = 18;
            this.gbRankSettings.TabStop = false;
            this.gbRankSettings.Text = "Rank settings";
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
            this.lblColor.Size = new System.Drawing.Size(21, 21);
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
            this.label14.Location = new System.Drawing.Point(23, 212);
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
            this.txtFileName.Location = new System.Drawing.Point(85, 209);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(100, 21);
            this.txtFileName.TabIndex = 4;
            this.txtFileName.TextChanged += new System.EventHandler(this.txtFileName_TextChanged);
            // 
            // lblMOTD
            // 
            this.lblMOTD.AutoSize = true;
            this.lblMOTD.Location = new System.Drawing.Point(39, 158);
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
            this.pageMisc.BackColor = System.Drawing.Color.White;
            this.pageMisc.Controls.Add(this.economyGroupBox);
            this.pageMisc.Controls.Add(this.grpExtra);
            this.pageMisc.Controls.Add(this.grpMessages);
            this.pageMisc.Controls.Add(this.grpPhysics);
            this.pageMisc.Controls.Add(this.grpAFK);
            this.pageMisc.Controls.Add(this.grpBackups);
            this.pageMisc.Controls.Add(this.chkProfanityFilter);
            this.pageMisc.Controls.Add(this.chkForceCuboid);
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
            this.economyGroupBox.Size = new System.Drawing.Size(133, 120);
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
            this.grpExtra.Size = new System.Drawing.Size(332, 210);
            this.grpExtra.TabIndex = 40;
            this.grpExtra.TabStop = false;
            this.grpExtra.Text = "Extra";
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
            this.label34.Location = new System.Drawing.Point(3, 183);
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
            this.pageIRC.BackColor = System.Drawing.Color.White;
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
            this.grpSQL.Size = new System.Drawing.Size(227, 252);
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
            this.grpIRC.Location = new System.Drawing.Point(8, 3);
            this.grpIRC.Name = "grpIRC";
            this.grpIRC.Size = new System.Drawing.Size(250, 252);
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
            this.label49.Location = new System.Drawing.Point(6, 213);
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
            // pageServer
            // 
            this.pageServer.BackColor = System.Drawing.Color.White;
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
            this.numPlayers.Location = new System.Drawing.Point(84, 20);
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
            this.tabControl.Controls.Add(this.pageReview);
            this.tabControl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(506, 547);
            this.tabControl.TabIndex = 1;
            this.tabControl.Click += new System.EventHandler(this.tabControl_Click);
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
            this.tabGames.Location = new System.Drawing.Point(9, 7);
            this.tabGames.Name = "tabGames";
            this.tabGames.SelectedIndex = 0;
            this.tabGames.Size = new System.Drawing.Size(476, 499);
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
            this.tabPage10.Size = new System.Drawing.Size(468, 473);
            this.tabPage10.TabIndex = 0;
            this.tabPage10.Text = "Lava Survival";
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.lsBtnEndVote);
            this.groupBox23.Controls.Add(this.lsBtnEndRound);
            this.groupBox23.Controls.Add(this.lsBtnStopGame);
            this.groupBox23.Controls.Add(this.lsBtnStartGame);
            this.groupBox23.Location = new System.Drawing.Point(291, 226);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(171, 165);
            this.groupBox23.TabIndex = 4;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Controls";
            // 
            // lsBtnEndVote
            // 
            this.lsBtnEndVote.Location = new System.Drawing.Point(6, 129);
            this.lsBtnEndVote.Name = "lsBtnEndVote";
            this.lsBtnEndVote.Size = new System.Drawing.Size(159, 30);
            this.lsBtnEndVote.TabIndex = 3;
            this.lsBtnEndVote.Text = "End Vote";
            this.lsBtnEndVote.UseVisualStyleBackColor = true;
            this.lsBtnEndVote.Click += new System.EventHandler(this.lsBtnEndVote_Click);
            // 
            // lsBtnEndRound
            // 
            this.lsBtnEndRound.Location = new System.Drawing.Point(6, 93);
            this.lsBtnEndRound.Name = "lsBtnEndRound";
            this.lsBtnEndRound.Size = new System.Drawing.Size(159, 30);
            this.lsBtnEndRound.TabIndex = 2;
            this.lsBtnEndRound.Text = "End Round";
            this.lsBtnEndRound.UseVisualStyleBackColor = true;
            this.lsBtnEndRound.Click += new System.EventHandler(this.lsBtnEndRound_Click);
            // 
            // lsBtnStopGame
            // 
            this.lsBtnStopGame.Location = new System.Drawing.Point(6, 57);
            this.lsBtnStopGame.Name = "lsBtnStopGame";
            this.lsBtnStopGame.Size = new System.Drawing.Size(159, 30);
            this.lsBtnStopGame.TabIndex = 1;
            this.lsBtnStopGame.Text = "Stop Game";
            this.lsBtnStopGame.UseVisualStyleBackColor = true;
            this.lsBtnStopGame.Click += new System.EventHandler(this.lsBtnStopGame_Click);
            // 
            // lsBtnStartGame
            // 
            this.lsBtnStartGame.Location = new System.Drawing.Point(6, 21);
            this.lsBtnStartGame.Name = "lsBtnStartGame";
            this.lsBtnStartGame.Size = new System.Drawing.Size(159, 30);
            this.lsBtnStartGame.TabIndex = 0;
            this.lsBtnStartGame.Text = "Start Game";
            this.lsBtnStartGame.UseVisualStyleBackColor = true;
            this.lsBtnStartGame.Click += new System.EventHandler(this.lsBtnStartGame_Click);
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.lsBtnSaveSettings);
            this.groupBox22.Controls.Add(this.lsNudFloodTime);
            this.groupBox22.Controls.Add(this.label67);
            this.groupBox22.Controls.Add(this.lsNudRoundTime);
            this.groupBox22.Controls.Add(this.label66);
            this.groupBox22.Controls.Add(this.lsNudLayerTime);
            this.groupBox22.Controls.Add(this.label65);
            this.groupBox22.Controls.Add(this.lsNudLayerCount);
            this.groupBox22.Controls.Add(this.label64);
            this.groupBox22.Controls.Add(this.lsNudLayerHeight);
            this.groupBox22.Controls.Add(this.label63);
            this.groupBox22.Controls.Add(this.lsNudLayer);
            this.groupBox22.Controls.Add(this.label62);
            this.groupBox22.Controls.Add(this.lsNudWater);
            this.groupBox22.Controls.Add(this.label61);
            this.groupBox22.Controls.Add(this.lsNudDestroy);
            this.groupBox22.Controls.Add(this.label60);
            this.groupBox22.Controls.Add(this.lsNudKiller);
            this.groupBox22.Controls.Add(this.label59);
            this.groupBox22.Controls.Add(this.lsNudFastLava);
            this.groupBox22.Controls.Add(this.label58);
            this.groupBox22.Location = new System.Drawing.Point(7, 199);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(278, 195);
            this.groupBox22.TabIndex = 3;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Map Settings";
            // 
            // lsBtnSaveSettings
            // 
            this.lsBtnSaveSettings.Location = new System.Drawing.Point(6, 155);
            this.lsBtnSaveSettings.Name = "lsBtnSaveSettings";
            this.lsBtnSaveSettings.Size = new System.Drawing.Size(266, 34);
            this.lsBtnSaveSettings.TabIndex = 21;
            this.lsBtnSaveSettings.Text = "Save Map Settings";
            this.lsBtnSaveSettings.UseVisualStyleBackColor = true;
            this.lsBtnSaveSettings.Click += new System.EventHandler(this.lsBtnSaveSettings_Click);
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(139, 131);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(61, 13);
            this.label67.TabIndex = 19;
            this.label67.Text = "Flood Time:";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(139, 105);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(65, 13);
            this.label66.TabIndex = 17;
            this.label66.Text = "Round Time:";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(139, 77);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(61, 13);
            this.label65.TabIndex = 15;
            this.label65.Text = "Layer Time:";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(139, 50);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(66, 13);
            this.label64.TabIndex = 13;
            this.label64.Text = "Layer Count:";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(139, 23);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(69, 13);
            this.label63.TabIndex = 11;
            this.label63.Text = "Layer Height:";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(7, 131);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(36, 13);
            this.label62.TabIndex = 8;
            this.label62.Text = "Layer:";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(7, 104);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(40, 13);
            this.label61.TabIndex = 6;
            this.label61.Text = "Water:";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(7, 77);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(47, 13);
            this.label60.TabIndex = 4;
            this.label60.Text = "Destroy:";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(7, 50);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(35, 13);
            this.label59.TabIndex = 2;
            this.label59.Text = "Killer:";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(7, 23);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(54, 13);
            this.label58.TabIndex = 0;
            this.label58.Text = "Fast Lava:";
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.label75);
            this.groupBox21.Controls.Add(this.lsNudLives);
            this.groupBox21.Controls.Add(this.lsCmbControlRank);
            this.groupBox21.Controls.Add(this.label68);
            this.groupBox21.Controls.Add(this.lsCmbSetupRank);
            this.groupBox21.Controls.Add(this.label57);
            this.groupBox21.Controls.Add(this.lsNudVoteTime);
            this.groupBox21.Controls.Add(this.label56);
            this.groupBox21.Controls.Add(this.lsNudVoteCount);
            this.groupBox21.Controls.Add(this.label55);
            this.groupBox21.Controls.Add(this.lsChkSendAFKMain);
            this.groupBox21.Controls.Add(this.lsChkStartOnStartup);
            this.groupBox21.Location = new System.Drawing.Point(291, 6);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(172, 214);
            this.groupBox21.TabIndex = 2;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Settings";
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(8, 136);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(34, 13);
            this.label75.TabIndex = 11;
            this.label75.Text = "Lives:";
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(8, 191);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(70, 13);
            this.label68.TabIndex = 8;
            this.label68.Text = "Control Rank:";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(8, 164);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(62, 13);
            this.label57.TabIndex = 6;
            this.label57.Text = "Setup Rank:";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(8, 109);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(57, 13);
            this.label56.TabIndex = 4;
            this.label56.Text = "Vote Time:";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(8, 82);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(62, 13);
            this.label55.TabIndex = 2;
            this.label55.Text = "Vote Count:";
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
            this.groupBox20.Size = new System.Drawing.Size(279, 186);
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
            this.label53.Location = new System.Drawing.Point(19, 17);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(64, 13);
            this.label53.TabIndex = 5;
            this.label53.Text = "Maps In Use";
            // 
            // lsAddMap
            // 
            this.lsAddMap.Location = new System.Drawing.Point(102, 81);
            this.lsAddMap.Name = "lsAddMap";
            this.lsAddMap.Size = new System.Drawing.Size(75, 23);
            this.lsAddMap.TabIndex = 4;
            this.lsAddMap.Text = "<< Add";
            this.lsAddMap.UseVisualStyleBackColor = true;
            this.lsAddMap.Click += new System.EventHandler(this.lsAddMap_Click);
            // 
            // lsRemoveMap
            // 
            this.lsRemoveMap.Location = new System.Drawing.Point(102, 110);
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
            this.lsMapNoUse.Location = new System.Drawing.Point(183, 33);
            this.lsMapNoUse.Name = "lsMapNoUse";
            this.lsMapNoUse.Size = new System.Drawing.Size(90, 134);
            this.lsMapNoUse.TabIndex = 2;
            // 
            // lsMapUse
            // 
            this.lsMapUse.FormattingEnabled = true;
            this.lsMapUse.Location = new System.Drawing.Point(6, 33);
            this.lsMapUse.Name = "lsMapUse";
            this.lsMapUse.Size = new System.Drawing.Size(90, 134);
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
            this.tabPage14.Size = new System.Drawing.Size(468, 473);
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
            this.tabPage11.Size = new System.Drawing.Size(468, 473);
            this.tabPage11.TabIndex = 1;
            this.tabPage11.Text = "Zombie survival";
            // 
            // propsZG
            // 
            this.propsZG.Location = new System.Drawing.Point(6, 3);
            this.propsZG.Name = "propsZG";
            this.propsZG.Size = new System.Drawing.Size(456, 464);
            this.propsZG.TabIndex = 42;
            // 
            // pageSecurity
            // 
            this.pageSecurity.BackColor = System.Drawing.Color.Transparent;
            this.pageSecurity.Controls.Add(this.gbSpamSettings);
            this.pageSecurity.Controls.Add(this.gbPasswords);
            this.pageSecurity.Location = new System.Drawing.Point(4, 22);
            this.pageSecurity.Name = "pageSecurity";
            this.pageSecurity.Padding = new System.Windows.Forms.Padding(3);
            this.pageSecurity.Size = new System.Drawing.Size(498, 521);
            this.pageSecurity.TabIndex = 7;
            this.pageSecurity.Text = "Security";
            // 
            // gbSpamSettings
            // 
            this.gbSpamSettings.Controls.Add(this.numCountReset);
            this.gbSpamSettings.Controls.Add(this.label69);
            this.gbSpamSettings.Controls.Add(this.numSpamMute);
            this.gbSpamSettings.Controls.Add(this.label45);
            this.gbSpamSettings.Controls.Add(this.numSpamMessages);
            this.gbSpamSettings.Controls.Add(this.label44);
            this.gbSpamSettings.Controls.Add(this.chkSpamControl);
            this.gbSpamSettings.Location = new System.Drawing.Point(225, 18);
            this.gbSpamSettings.Name = "gbSpamSettings";
            this.gbSpamSettings.Size = new System.Drawing.Size(248, 189);
            this.gbSpamSettings.TabIndex = 1;
            this.gbSpamSettings.TabStop = false;
            this.gbSpamSettings.Text = "Spam Control";
            // 
            // numCountReset
            // 
            this.numCountReset.Location = new System.Drawing.Point(170, 143);
            this.numCountReset.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.numCountReset.Name = "numCountReset";
            this.numCountReset.Size = new System.Drawing.Size(60, 21);
            this.numCountReset.TabIndex = 34;
            this.numCountReset.Tag = "The number of seconds before the message counter is reset.";
            this.numCountReset.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(15, 145);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(149, 13);
            this.label69.TabIndex = 33;
            this.label69.Text = "Counter Reset Time (seconds):";
            // 
            // numSpamMute
            // 
            this.numSpamMute.Location = new System.Drawing.Point(158, 104);
            this.numSpamMute.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.numSpamMute.Name = "numSpamMute";
            this.numSpamMute.Size = new System.Drawing.Size(60, 21);
            this.numSpamMute.TabIndex = 32;
            this.numSpamMute.Tag = "The number of seconds a player is muted for, for spamming.";
            this.numSpamMute.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(15, 106);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(137, 13);
            this.label45.TabIndex = 31;
            this.label45.Text = "Spam Mute Time (seconds) :";
            // 
            // numSpamMessages
            // 
            this.numSpamMessages.Location = new System.Drawing.Point(106, 67);
            this.numSpamMessages.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.numSpamMessages.Name = "numSpamMessages";
            this.numSpamMessages.Size = new System.Drawing.Size(60, 21);
            this.numSpamMessages.TabIndex = 30;
            this.numSpamMessages.Tag = "The amount of messages that have to be sent before a player is muted.";
            this.numSpamMessages.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(15, 71);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(85, 13);
            this.label44.TabIndex = 25;
            this.label44.Text = "Spam Messages:";
            // 
            // gbPasswords
            // 
            this.gbPasswords.Controls.Add(this.btnReset);
            this.gbPasswords.Controls.Add(this.listPasswords);
            this.gbPasswords.Controls.Add(this.label39);
            this.gbPasswords.Controls.Add(this.chkEnableVerification);
            this.gbPasswords.Controls.Add(this.cmbVerificationRank);
            this.gbPasswords.Controls.Add(this.label38);
            this.gbPasswords.Location = new System.Drawing.Point(19, 18);
            this.gbPasswords.Name = "gbPasswords";
            this.gbPasswords.Size = new System.Drawing.Size(191, 322);
            this.gbPasswords.TabIndex = 0;
            this.gbPasswords.TabStop = false;
            this.gbPasswords.Text = "Admin Verification";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(45, 276);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(91, 27);
            this.btnReset.TabIndex = 25;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // listPasswords
            // 
            this.listPasswords.FormattingEnabled = true;
            this.listPasswords.Location = new System.Drawing.Point(24, 132);
            this.listPasswords.Name = "listPasswords";
            this.listPasswords.Size = new System.Drawing.Size(139, 121);
            this.listPasswords.TabIndex = 1;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(21, 106);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(98, 13);
            this.label39.TabIndex = 24;
            this.label39.Text = "Remove Passwords";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(21, 67);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(33, 39);
            this.label38.TabIndex = 1;
            this.label38.Text = "Rank:\r\n\r\n\r\n";
            // 
            // pageReview
            // 
            this.pageReview.BackColor = System.Drawing.Color.Transparent;
            this.pageReview.Controls.Add(this.gbReviewOptions);
            this.pageReview.Controls.Add(this.gbReviewActions);
            this.pageReview.Controls.Add(this.gbReviewPerms);
            this.pageReview.Controls.Add(this.gbReviewQueue);
            this.pageReview.Location = new System.Drawing.Point(4, 22);
            this.pageReview.Name = "pageReview";
            this.pageReview.Padding = new System.Windows.Forms.Padding(3);
            this.pageReview.Size = new System.Drawing.Size(498, 521);
            this.pageReview.TabIndex = 9;
            this.pageReview.Text = "Review";
            // 
            // gbReviewOptions
            // 
            this.gbReviewOptions.Controls.Add(this.nudCooldownTime);
            this.gbReviewOptions.Controls.Add(this.label84);
            this.gbReviewOptions.Location = new System.Drawing.Point(154, 235);
            this.gbReviewOptions.Name = "gbReviewOptions";
            this.gbReviewOptions.Size = new System.Drawing.Size(328, 51);
            this.gbReviewOptions.TabIndex = 4;
            this.gbReviewOptions.TabStop = false;
            this.gbReviewOptions.Text = "Options";
            // 
            // nudCooldownTime
            // 
            this.nudCooldownTime.Location = new System.Drawing.Point(202, 19);
            this.nudCooldownTime.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.nudCooldownTime.Name = "nudCooldownTime";
            this.nudCooldownTime.Size = new System.Drawing.Size(120, 21);
            this.nudCooldownTime.TabIndex = 1;
            this.nudCooldownTime.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(7, 21);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(77, 13);
            this.label84.TabIndex = 0;
            this.label84.Text = "Cooldown time";
            // 
            // gbReviewActions
            // 
            this.gbReviewActions.Controls.Add(this.button4);
            this.gbReviewActions.Location = new System.Drawing.Point(154, 175);
            this.gbReviewActions.Name = "gbReviewActions";
            this.gbReviewActions.Size = new System.Drawing.Size(328, 53);
            this.gbReviewActions.TabIndex = 3;
            this.gbReviewActions.TabStop = false;
            this.gbReviewActions.Text = "Actions";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(7, 21);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(315, 23);
            this.button4.TabIndex = 0;
            this.button4.Text = "Clear queue";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // gbReviewPerms
            // 
            this.gbReviewPerms.Controls.Add(this.label83);
            this.gbReviewPerms.Controls.Add(this.label82);
            this.gbReviewPerms.Controls.Add(this.label81);
            this.gbReviewPerms.Controls.Add(this.label80);
            this.gbReviewPerms.Controls.Add(this.cmbGotoNext);
            this.gbReviewPerms.Controls.Add(this.cmbClearQueue);
            this.gbReviewPerms.Controls.Add(this.cmbLeaveQueue);
            this.gbReviewPerms.Controls.Add(this.cmbEnterQueue);
            this.gbReviewPerms.Controls.Add(this.cmbViewQueue);
            this.gbReviewPerms.Controls.Add(this.label79);
            this.gbReviewPerms.Location = new System.Drawing.Point(154, 7);
            this.gbReviewPerms.Name = "gbReviewPerms";
            this.gbReviewPerms.Size = new System.Drawing.Size(328, 161);
            this.gbReviewPerms.TabIndex = 2;
            this.gbReviewPerms.TabStop = false;
            this.gbReviewPerms.Text = "Permissions";
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Location = new System.Drawing.Point(6, 131);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(53, 13);
            this.label83.TabIndex = 9;
            this.label83.Text = "Goto next";
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Location = new System.Drawing.Point(6, 103);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(64, 13);
            this.label82.TabIndex = 8;
            this.label82.Text = "Clear queue";
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Location = new System.Drawing.Point(6, 75);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(67, 13);
            this.label81.TabIndex = 7;
            this.label81.Text = "Leave queue";
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Location = new System.Drawing.Point(6, 47);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(64, 13);
            this.label80.TabIndex = 6;
            this.label80.Text = "Enter queue";
            // 
            // cmbGotoNext
            // 
            this.cmbGotoNext.FormattingEnabled = true;
            this.cmbGotoNext.Location = new System.Drawing.Point(178, 131);
            this.cmbGotoNext.Name = "cmbGotoNext";
            this.cmbGotoNext.Size = new System.Drawing.Size(144, 21);
            this.cmbGotoNext.TabIndex = 5;
            // 
            // cmbClearQueue
            // 
            this.cmbClearQueue.FormattingEnabled = true;
            this.cmbClearQueue.Location = new System.Drawing.Point(178, 103);
            this.cmbClearQueue.Name = "cmbClearQueue";
            this.cmbClearQueue.Size = new System.Drawing.Size(144, 21);
            this.cmbClearQueue.TabIndex = 4;
            // 
            // cmbLeaveQueue
            // 
            this.cmbLeaveQueue.FormattingEnabled = true;
            this.cmbLeaveQueue.Location = new System.Drawing.Point(178, 75);
            this.cmbLeaveQueue.Name = "cmbLeaveQueue";
            this.cmbLeaveQueue.Size = new System.Drawing.Size(144, 21);
            this.cmbLeaveQueue.TabIndex = 3;
            // 
            // cmbEnterQueue
            // 
            this.cmbEnterQueue.FormattingEnabled = true;
            this.cmbEnterQueue.Location = new System.Drawing.Point(178, 47);
            this.cmbEnterQueue.Name = "cmbEnterQueue";
            this.cmbEnterQueue.Size = new System.Drawing.Size(144, 21);
            this.cmbEnterQueue.TabIndex = 2;
            // 
            // cmbViewQueue
            // 
            this.cmbViewQueue.FormattingEnabled = true;
            this.cmbViewQueue.Location = new System.Drawing.Point(178, 19);
            this.cmbViewQueue.Name = "cmbViewQueue";
            this.cmbViewQueue.Size = new System.Drawing.Size(144, 21);
            this.cmbViewQueue.TabIndex = 1;
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Location = new System.Drawing.Point(6, 22);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(62, 13);
            this.label79.TabIndex = 0;
            this.label79.Text = "View queue";
            // 
            // gbReviewQueue
            // 
            this.gbReviewQueue.Controls.Add(this.listBox1);
            this.gbReviewQueue.Location = new System.Drawing.Point(6, 6);
            this.gbReviewQueue.Name = "gbReviewQueue";
            this.gbReviewQueue.Size = new System.Drawing.Size(141, 497);
            this.gbReviewQueue.TabIndex = 1;
            this.gbReviewQueue.TabStop = false;
            this.gbReviewQueue.Text = "Review Queue";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 20);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(129, 459);
            this.listBox1.TabIndex = 0;
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
            ((System.ComponentModel.ISupportInitialize)(this.lsNudVoteCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudVoteTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudFastLava)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudKiller)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudDestroy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudWater)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLayerHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLayerCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLayerTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudRoundTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudFloodTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsNudLives)).EndInit();
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
            this.groupBox22.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            this.groupBox21.PerformLayout();
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
            this.gbSpamSettings.ResumeLayout(false);
            this.gbSpamSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCountReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpamMute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpamMessages)).EndInit();
            this.gbPasswords.ResumeLayout(false);
            this.gbPasswords.PerformLayout();
            this.pageReview.ResumeLayout(false);
            this.gbReviewOptions.ResumeLayout(false);
            this.gbReviewOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldownTime)).EndInit();
            this.gbReviewActions.ResumeLayout(false);
            this.gbReviewPerms.ResumeLayout(false);
            this.gbReviewPerms.PerformLayout();
            this.gbReviewQueue.ResumeLayout(false);
            this.ResumeLayout(false);

        }
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
        private System.Windows.Forms.CheckBox chkForceCuboid;
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
        private System.Windows.Forms.GroupBox gbSpamSettings;
        private System.Windows.Forms.NumericUpDown numSpamMute;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.NumericUpDown numSpamMessages;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.CheckBox chkSpamControl;
        private System.Windows.Forms.GroupBox gbPasswords;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ListBox listPasswords;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.CheckBox chkEnableVerification;
        private System.Windows.Forms.ComboBox cmbVerificationRank;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TabPage pageGames;
        private System.Windows.Forms.TabControl tabGames;
        private System.Windows.Forms.TabPage tabPage11;
        private System.Windows.Forms.TabPage tabPage10;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox txtIrcId;
        private System.Windows.Forms.CheckBox chkIrcId;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.TextBox txtIRCPort;
        private System.Windows.Forms.CheckBox chkShowEmptyRanks;
        private System.Windows.Forms.CheckBox chat_chkGlobal;
        private System.Windows.Forms.ComboBox chat_cmbGlobal;
        private System.Windows.Forms.Label chat_colGlobal;
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
        private System.Windows.Forms.CheckBox lsChkSendAFKMain;
        private System.Windows.Forms.CheckBox lsChkStartOnStartup;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.NumericUpDown lsNudVoteTime;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.NumericUpDown lsNudVoteCount;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.ComboBox lsCmbSetupRank;
        private System.Windows.Forms.GroupBox groupBox22;
        private System.Windows.Forms.NumericUpDown lsNudKiller;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.NumericUpDown lsNudFastLava;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.NumericUpDown lsNudLayer;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.NumericUpDown lsNudWater;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.NumericUpDown lsNudDestroy;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.NumericUpDown lsNudFloodTime;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.NumericUpDown lsNudRoundTime;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.NumericUpDown lsNudLayerTime;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.NumericUpDown lsNudLayerCount;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.NumericUpDown lsNudLayerHeight;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.Button lsBtnSaveSettings;
        private System.Windows.Forms.GroupBox groupBox23;
        private System.Windows.Forms.Button lsBtnEndRound;
        private System.Windows.Forms.Button lsBtnStopGame;
        private System.Windows.Forms.Button lsBtnStartGame;
        private System.Windows.Forms.ComboBox lsCmbControlRank;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.Button lsBtnEndVote;
        private System.Windows.Forms.NumericUpDown numCountReset;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.NumericUpDown lsNudLives;
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
        private System.Windows.Forms.Label label75;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.ComboBox cmbAFKKickPerm;
        private System.Windows.Forms.CheckBox chkGuestLimitNotify;
        private System.Windows.Forms.TabPage pageReview;
        private System.Windows.Forms.GroupBox gbReviewQueue;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox gbReviewPerms;
        private System.Windows.Forms.Label label79;
        private System.Windows.Forms.GroupBox gbReviewActions;
        private System.Windows.Forms.Label label83;
        private System.Windows.Forms.Label label82;
        private System.Windows.Forms.Label label81;
        private System.Windows.Forms.Label label80;
        private System.Windows.Forms.ComboBox cmbGotoNext;
        private System.Windows.Forms.ComboBox cmbClearQueue;
        private System.Windows.Forms.ComboBox cmbLeaveQueue;
        private System.Windows.Forms.ComboBox cmbEnterQueue;
        private System.Windows.Forms.ComboBox cmbViewQueue;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox gbReviewOptions;
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
        private System.Windows.Forms.Label chat_lblGlobal;
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