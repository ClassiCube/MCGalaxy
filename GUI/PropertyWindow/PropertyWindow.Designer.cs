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
            this.pageChat = new System.Windows.Forms.TabPage();
            this.chat_grpTab = new System.Windows.Forms.GroupBox();
            this.chat_cbTabRank = new System.Windows.Forms.CheckBox();
            this.chat_cbTabLevel = new System.Windows.Forms.CheckBox();
            this.chat_cbTabBots = new System.Windows.Forms.CheckBox();
            this.chat_grpMessages = new System.Windows.Forms.GroupBox();
            this.chat_lblShutdown = new System.Windows.Forms.Label();
            this.chat_txtShutdown = new System.Windows.Forms.TextBox();
            this.chat_chkCheap = new System.Windows.Forms.CheckBox();
            this.chat_txtCheap = new System.Windows.Forms.TextBox();
            this.chat_lblBan = new System.Windows.Forms.Label();
            this.chat_txtBan = new System.Windows.Forms.TextBox();
            this.chat_lblPromote = new System.Windows.Forms.Label();
            this.chat_txtPromote = new System.Windows.Forms.TextBox();
            this.chat_lblDemote = new System.Windows.Forms.Label();
            this.chat_txtDemote = new System.Windows.Forms.TextBox();
            this.chat_grpOther = new System.Windows.Forms.GroupBox();
            this.chat_lblConsole = new System.Windows.Forms.Label();
            this.chat_txtConsole = new System.Windows.Forms.TextBox();
            this.chat_grpColors = new System.Windows.Forms.GroupBox();
            this.chat_lblDefault = new System.Windows.Forms.Label();
            this.chat_btnDefault = new System.Windows.Forms.Button();
            this.chat_lblIRC = new System.Windows.Forms.Label();
            this.chat_btnIRC = new System.Windows.Forms.Button();
            this.chat_lblSyntax = new System.Windows.Forms.Label();
            this.chat_btnSyntax = new System.Windows.Forms.Button();
            this.chat_lblDesc = new System.Windows.Forms.Label();
            this.chat_btnDesc = new System.Windows.Forms.Button();
            this.main_btnSave = new System.Windows.Forms.Button();
            this.main_btnDiscard = new System.Windows.Forms.Button();
            this.main_btnApply = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label24 = new System.Windows.Forms.Label();
            this.chkPhysicsRest = new System.Windows.Forms.CheckBox();
            this.chkDeath = new System.Windows.Forms.CheckBox();
            this.hackrank_kick = new System.Windows.Forms.CheckBox();
            this.sec_cmbVerifyRank = new System.Windows.Forms.ComboBox();
            this.sec_cbVerifyAdmins = new System.Windows.Forms.CheckBox();
            this.chkGuestLimitNotify = new System.Windows.Forms.CheckBox();
            this.rank_cbTPHigher = new System.Windows.Forms.CheckBox();
            this.rank_cmbDefault = new System.Windows.Forms.ComboBox();
            this.sec_cbWhitelist = new System.Windows.Forms.CheckBox();
            this.afk_txtTimer = new System.Windows.Forms.TextBox();
            this.bak_numTime = new System.Windows.Forms.NumericUpDown();
            this.sql_chkUseSQL = new System.Windows.Forms.CheckBox();
            this.irc_chkEnabled = new System.Windows.Forms.CheckBox();
            this.irc_txtServer = new System.Windows.Forms.TextBox();
            this.irc_txtNick = new System.Windows.Forms.TextBox();
            this.irc_txtChannel = new System.Windows.Forms.TextBox();
            this.irc_txtOpChannel = new System.Windows.Forms.TextBox();
            this.lvl_chkAutoload = new System.Windows.Forms.CheckBox();
            this.lvl_chkWorld = new System.Windows.Forms.CheckBox();
            this.adv_chkVerify = new System.Windows.Forms.CheckBox();
            this.srv_txtName = new System.Windows.Forms.TextBox();
            this.srv_txtMOTD = new System.Windows.Forms.TextBox();
            this.srv_numPort = new System.Windows.Forms.NumericUpDown();
            this.srv_chkPublic = new System.Windows.Forms.CheckBox();
            this.rank_cbSilentAdmins = new System.Windows.Forms.CheckBox();
            this.rank_txtPrefix = new System.Windows.Forms.TextBox();
            this.rank_txtMOTD = new System.Windows.Forms.TextBox();
            this.rank_numPerm = new System.Windows.Forms.NumericUpDown();
            this.rank_txtName = new System.Windows.Forms.TextBox();
            this.rank_btnColor = new System.Windows.Forms.Button();
            this.rank_cmbOsMap = new System.Windows.Forms.ComboBox();
            this.irc_chkPass = new System.Windows.Forms.CheckBox();
            this.irc_txtPass = new System.Windows.Forms.TextBox();
            this.rank_numMaps = new System.Windows.Forms.NumericUpDown();
            this.rank_numDraw = new System.Windows.Forms.NumericUpDown();
            this.rank_numUndo = new System.Windows.Forms.NumericUpDown();
            this.rank_numGen = new System.Windows.Forms.NumericUpDown();
            this.rank_numAfk = new System.Windows.Forms.NumericUpDown();
            this.sec_cbLogNotes = new System.Windows.Forms.CheckBox();
            this.sec_cbChatAuto = new System.Windows.Forms.CheckBox();
            this.pageBlocks = new System.Windows.Forms.TabPage();
            this.blk_grpPhysics = new System.Windows.Forms.GroupBox();
            this.blk_cbWater = new System.Windows.Forms.CheckBox();
            this.blk_cbLava = new System.Windows.Forms.CheckBox();
            this.blk_cbRails = new System.Windows.Forms.CheckBox();
            this.blk_cbTdoor = new System.Windows.Forms.CheckBox();
            this.blk_cbDoor = new System.Windows.Forms.CheckBox();
            this.blk_grpBehaviour = new System.Windows.Forms.GroupBox();
            this.blk_txtDeath = new System.Windows.Forms.TextBox();
            this.blk_cbDeath = new System.Windows.Forms.CheckBox();
            this.blk_cbPortal = new System.Windows.Forms.CheckBox();
            this.blk_cbMsgBlock = new System.Windows.Forms.CheckBox();
            this.blk_grpPermissions = new System.Windows.Forms.GroupBox();
            this.blk_cmbAlw3 = new System.Windows.Forms.ComboBox();
            this.blk_cmbAlw2 = new System.Windows.Forms.ComboBox();
            this.blk_cmbDis3 = new System.Windows.Forms.ComboBox();
            this.blk_cmbDis2 = new System.Windows.Forms.ComboBox();
            this.blk_cmbAlw1 = new System.Windows.Forms.ComboBox();
            this.blk_cmbDis1 = new System.Windows.Forms.ComboBox();
            this.blk_cmbMin = new System.Windows.Forms.ComboBox();
            this.blk_lblMin = new System.Windows.Forms.Label();
            this.blk_lblAllow = new System.Windows.Forms.Label();
            this.blk_lblDisallow = new System.Windows.Forms.Label();
            this.blk_btnHelp = new System.Windows.Forms.Button();
            this.blk_list = new System.Windows.Forms.ListBox();
            this.pageRanks = new System.Windows.Forms.TabPage();
            this.rank_grpLimits = new System.Windows.Forms.GroupBox();
            this.rank_lblGen = new System.Windows.Forms.Label();
            this.rank_lblMaps = new System.Windows.Forms.Label();
            this.rank_lblDraw = new System.Windows.Forms.Label();
            this.rank_lblUndo = new System.Windows.Forms.Label();
            this.rank_grpGeneral = new System.Windows.Forms.GroupBox();
            this.rank_lblOsMap = new System.Windows.Forms.Label();
            this.rank_cbEmpty = new System.Windows.Forms.CheckBox();
            this.rank_lblDefault = new System.Windows.Forms.Label();
            this.rank_grpMisc = new System.Windows.Forms.GroupBox();
            this.rank_cbAfk = new System.Windows.Forms.CheckBox();
            this.rank_lblAfk = new System.Windows.Forms.Label();
            this.rank_lblPrefix = new System.Windows.Forms.Label();
            this.rank_lblPerm = new System.Windows.Forms.Label();
            this.rank_lblMOTD = new System.Windows.Forms.Label();
            this.rank_lblName = new System.Windows.Forms.Label();
            this.rank_lblColor = new System.Windows.Forms.Label();
            this.rank_btnDel = new System.Windows.Forms.Button();
            this.rank_btnAdd = new System.Windows.Forms.Button();
            this.rank_list = new System.Windows.Forms.ListBox();
            this.label85 = new System.Windows.Forms.Label();
            this.pageMisc = new System.Windows.Forms.TabPage();
            this.eco_grpEco = new System.Windows.Forms.GroupBox();
            this.eco_btnEco = new System.Windows.Forms.Button();
            this.grpExtra = new System.Windows.Forms.GroupBox();
            this.nudCooldownTime = new System.Windows.Forms.NumericUpDown();
            this.misc_lblReview = new System.Windows.Forms.Label();
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
            this.afk_grp = new System.Windows.Forms.GroupBox();
            this.afk_lblTimer = new System.Windows.Forms.Label();
            this.bak_grp = new System.Windows.Forms.GroupBox();
            this.bak_lblLocation = new System.Windows.Forms.Label();
            this.bak_txtLocation = new System.Windows.Forms.TextBox();
            this.bak_lblTime = new System.Windows.Forms.Label();
            this.chkProfanityFilter = new System.Windows.Forms.CheckBox();
            this.pageIRC = new System.Windows.Forms.TabPage();
            this.gb_ircSettings = new System.Windows.Forms.GroupBox();
            this.irc_txtPrefix = new System.Windows.Forms.TextBox();
            this.irc_lblPrefix = new System.Windows.Forms.Label();
            this.irc_cbVerify = new System.Windows.Forms.ComboBox();
            this.irc_lblVerify = new System.Windows.Forms.Label();
            this.irc_cbRank = new System.Windows.Forms.ComboBox();
            this.irc_lblRank = new System.Windows.Forms.Label();
            this.irc_cbAFK = new System.Windows.Forms.CheckBox();
            this.irc_cbWorldChanges = new System.Windows.Forms.CheckBox();
            this.irc_cbTitles = new System.Windows.Forms.CheckBox();
            this.sql_grp = new System.Windows.Forms.GroupBox();
            this.sql_linkDownload = new System.Windows.Forms.LinkLabel();
            this.sql_lblUser = new System.Windows.Forms.Label();
            this.sql_txtUser = new System.Windows.Forms.TextBox();
            this.sql_lblPass = new System.Windows.Forms.Label();
            this.sql_txtPass = new System.Windows.Forms.TextBox();
            this.sql_lblDBName = new System.Windows.Forms.Label();
            this.sql_txtDBName = new System.Windows.Forms.TextBox();
            this.sql_lblHost = new System.Windows.Forms.Label();
            this.sql_txtHost = new System.Windows.Forms.TextBox();
            this.sql_lblPort = new System.Windows.Forms.Label();
            this.sql_txtPort = new System.Windows.Forms.TextBox();
            this.irc_grp = new System.Windows.Forms.GroupBox();
            this.irc_lblServer = new System.Windows.Forms.Label();
            this.irc_lblPort = new System.Windows.Forms.Label();
            this.irc_txtPort = new System.Windows.Forms.TextBox();
            this.irc_lblNick = new System.Windows.Forms.Label();
            this.irc_lblChannel = new System.Windows.Forms.Label();
            this.irc_lblOpChannel = new System.Windows.Forms.Label();
            this.pageServer = new System.Windows.Forms.TabPage();
            this.lvl_grp = new System.Windows.Forms.GroupBox();
            this.lvl_lblMain = new System.Windows.Forms.Label();
            this.lvl_txtMain = new System.Windows.Forms.TextBox();
            this.adv_grp = new System.Windows.Forms.GroupBox();
            this.adv_chkRestart = new System.Windows.Forms.CheckBox();
            this.adv_btnEditTexts = new System.Windows.Forms.Button();
            this.srv_grp = new System.Windows.Forms.GroupBox();
            this.srv_lblName = new System.Windows.Forms.Label();
            this.srv_lblMotd = new System.Windows.Forms.Label();
            this.srv_lblPort = new System.Windows.Forms.Label();
            this.srv_btnPort = new System.Windows.Forms.Button();
            this.srv_lblOwner = new System.Windows.Forms.Label();
            this.srv_txtOwner = new System.Windows.Forms.TextBox();
            this.srv_grpUpdate = new System.Windows.Forms.GroupBox();
            this.srv_btnForceUpdate = new System.Windows.Forms.Button();
            this.chkUpdates = new System.Windows.Forms.CheckBox();
            this.grpPlayers = new System.Windows.Forms.GroupBox();
            this.srv_lblPlayers = new System.Windows.Forms.Label();
            this.srv_numPlayers = new System.Windows.Forms.NumericUpDown();
            this.srv_cbMustAgree = new System.Windows.Forms.CheckBox();
            this.srv_lblGuests = new System.Windows.Forms.Label();
            this.srv_numGuests = new System.Windows.Forms.NumericUpDown();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.pageGames = new System.Windows.Forms.TabPage();
            this.tabGames = new System.Windows.Forms.TabControl();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.lsBtnEndVote = new System.Windows.Forms.Button();
            this.ls_btnEndRound = new System.Windows.Forms.Button();
            this.ls_btnStopGame = new System.Windows.Forms.Button();
            this.ls_btnStartGame = new System.Windows.Forms.Button();
            this.ls_grpMapSettings = new System.Windows.Forms.GroupBox();
            this.pg_lavaMap = new System.Windows.Forms.PropertyGrid();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.pg_lava = new System.Windows.Forms.PropertyGrid();
            this.ls_grpMaps = new System.Windows.Forms.GroupBox();
            this.ls_lblNotUsed = new System.Windows.Forms.Label();
            this.ls_lblUsed = new System.Windows.Forms.Label();
            this.ls_btnAdd = new System.Windows.Forms.Button();
            this.ls_btnRemove = new System.Windows.Forms.Button();
            this.ls_lstNotUsed = new System.Windows.Forms.ListBox();
            this.ls_lstUsed = new System.Windows.Forms.ListBox();
            this.tabTntWars = new System.Windows.Forms.TabPage();
            this.tw_txtPlayers = new System.Windows.Forms.TextBox();
            this.tw_lblPlayers = new System.Windows.Forms.Label();
            this.tw_txtStatus = new System.Windows.Forms.TextBox();
            this.tw_lblStatus = new System.Windows.Forms.Label();
            this.SlctdTntWrsLvl = new System.Windows.Forms.TextBox();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.groupBox36 = new System.Windows.Forms.GroupBox();
            this.tw_cbStreaks = new System.Windows.Forms.CheckBox();
            this.groupBox35 = new System.Windows.Forms.GroupBox();
            this.TntWrsMpsList = new System.Windows.Forms.ListBox();
            this.TntWrsCrtNwTntWrsBt = new System.Windows.Forms.Button();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.TntWrsDiffSlctBt = new System.Windows.Forms.Button();
            this.TntWrsDiffAboutBt = new System.Windows.Forms.Button();
            this.TntWrsDiffCombo = new System.Windows.Forms.ComboBox();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.tw_cbTeamKills = new System.Windows.Forms.CheckBox();
            this.tw_cbBalanceTeams = new System.Windows.Forms.CheckBox();
            this.TntWrsTmsChck = new System.Windows.Forms.CheckBox();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.label90 = new System.Windows.Forms.Label();
            this.TntWrsGraceTimeChck = new System.Windows.Forms.NumericUpDown();
            this.TntWrsGracePrdChck = new System.Windows.Forms.CheckBox();
            this.tw_grpScores = new System.Windows.Forms.GroupBox();
            this.tw_cbMultiKills = new System.Windows.Forms.CheckBox();
            this.tw_numMultiKills = new System.Windows.Forms.NumericUpDown();
            this.tw_cbScoreAssists = new System.Windows.Forms.CheckBox();
            this.tw_numScoreAssists = new System.Windows.Forms.NumericUpDown();
            this.tw_lblScorePerKill = new System.Windows.Forms.Label();
            this.tw_numScorePerKill = new System.Windows.Forms.NumericUpDown();
            this.tw_lblScoreLimit = new System.Windows.Forms.Label();
            this.tw_numScoreLimit = new System.Windows.Forms.NumericUpDown();
            this.tw_grpStatus = new System.Windows.Forms.GroupBox();
            this.tw_btnStartGame = new System.Windows.Forms.Button();
            this.tw_btnDeleteGame = new System.Windows.Forms.Button();
            this.tw_btnEndGame = new System.Windows.Forms.Button();
            this.tw_btnResetGame = new System.Windows.Forms.Button();
            this.tw_btnEditGame = new System.Windows.Forms.Button();
            this.tw_lstGames = new System.Windows.Forms.ListBox();
            this.tabZS = new System.Windows.Forms.TabPage();
            this.propsZG = new System.Windows.Forms.PropertyGrid();
            this.pageCommands = new System.Windows.Forms.TabPage();
            this.cmd_grpExtra = new System.Windows.Forms.GroupBox();
            this.cmd_cmbExtra7 = new System.Windows.Forms.ComboBox();
            this.cmd_lblExtra7 = new System.Windows.Forms.Label();
            this.cmd_cmbExtra6 = new System.Windows.Forms.ComboBox();
            this.cmd_lblExtra6 = new System.Windows.Forms.Label();
            this.cmd_cmbExtra5 = new System.Windows.Forms.ComboBox();
            this.cmd_lblExtra5 = new System.Windows.Forms.Label();
            this.cmd_cmbExtra4 = new System.Windows.Forms.ComboBox();
            this.cmd_lblExtra4 = new System.Windows.Forms.Label();
            this.cmd_cmbExtra3 = new System.Windows.Forms.ComboBox();
            this.cmd_lblExtra3 = new System.Windows.Forms.Label();
            this.cmd_cmbExtra2 = new System.Windows.Forms.ComboBox();
            this.cmd_lblExtra2 = new System.Windows.Forms.Label();
            this.cmd_cmbExtra1 = new System.Windows.Forms.ComboBox();
            this.cmd_lblExtra1 = new System.Windows.Forms.Label();
            this.cmd_grpPermissions = new System.Windows.Forms.GroupBox();
            this.cmd_cmbAlw3 = new System.Windows.Forms.ComboBox();
            this.cmd_cmbAlw2 = new System.Windows.Forms.ComboBox();
            this.cmd_cmbDis3 = new System.Windows.Forms.ComboBox();
            this.cmd_cmbDis2 = new System.Windows.Forms.ComboBox();
            this.cmd_cmbAlw1 = new System.Windows.Forms.ComboBox();
            this.cmd_cmbDis1 = new System.Windows.Forms.ComboBox();
            this.cmd_cmbMin = new System.Windows.Forms.ComboBox();
            this.cmd_lblMin = new System.Windows.Forms.Label();
            this.cmd_lblDisallow = new System.Windows.Forms.Label();
            this.cmd_lblAllow = new System.Windows.Forms.Label();
            this.cmd_btnCustom = new System.Windows.Forms.Button();
            this.cmd_btnHelp = new System.Windows.Forms.Button();
            this.cmd_list = new System.Windows.Forms.ListBox();
            this.pageSecurity = new System.Windows.Forms.TabPage();
            this.sec_grpChat = new System.Windows.Forms.GroupBox();
            this.sec_lblChatOnMute = new System.Windows.Forms.Label();
            this.sec_numChatMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblChatOnMsgs = new System.Windows.Forms.Label();
            this.sec_numChatSecs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblChatOnSecs = new System.Windows.Forms.Label();
            this.sec_lblChatForMute = new System.Windows.Forms.Label();
            this.sec_numChatMute = new System.Windows.Forms.NumericUpDown();
            this.sec_lblChatForSecs = new System.Windows.Forms.Label();
            this.sec_grpCmd = new System.Windows.Forms.GroupBox();
            this.sec_cbCmdAuto = new System.Windows.Forms.CheckBox();
            this.sec_lblCmdOnMute = new System.Windows.Forms.Label();
            this.sec_numCmdMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblCmdOnMsgs = new System.Windows.Forms.Label();
            this.sec_numCmdSecs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblCmdOnSecs = new System.Windows.Forms.Label();
            this.sec_lblCmdForMute = new System.Windows.Forms.Label();
            this.sec_numCmdMute = new System.Windows.Forms.NumericUpDown();
            this.sec_lblCmdForSecs = new System.Windows.Forms.Label();
            this.sec_grpIP = new System.Windows.Forms.GroupBox();
            this.sec_cbIPAuto = new System.Windows.Forms.CheckBox();
            this.sec_lblIPOnMute = new System.Windows.Forms.Label();
            this.sec_numIPMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblIPOnMsgs = new System.Windows.Forms.Label();
            this.sec_numIPSecs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblIPOnSecs = new System.Windows.Forms.Label();
            this.sec_lblIPForMute = new System.Windows.Forms.Label();
            this.sec_numIPMute = new System.Windows.Forms.NumericUpDown();
            this.sec_lblIPForSecs = new System.Windows.Forms.Label();
            this.sec_grpOther = new System.Windows.Forms.GroupBox();
            this.sec_lblRank = new System.Windows.Forms.Label();
            this.sec_grpBlocks = new System.Windows.Forms.GroupBox();
            this.sec_cbBlocksAuto = new System.Windows.Forms.CheckBox();
            this.sec_lblBlocksOnMute = new System.Windows.Forms.Label();
            this.sec_numBlocksMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblBlocksOnMsgs = new System.Windows.Forms.Label();
            this.sec_numBlocksSecs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblBlocksOnSecs = new System.Windows.Forms.Label();
            this.pageChat.SuspendLayout();
            this.chat_grpTab.SuspendLayout();
            this.chat_grpMessages.SuspendLayout();
            this.chat_grpOther.SuspendLayout();
            this.chat_grpColors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bak_numTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.srv_numPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numPerm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numMaps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numDraw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numUndo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numGen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numAfk)).BeginInit();
            this.pageBlocks.SuspendLayout();
            this.blk_grpPhysics.SuspendLayout();
            this.blk_grpBehaviour.SuspendLayout();
            this.blk_grpPermissions.SuspendLayout();
            this.pageRanks.SuspendLayout();
            this.rank_grpLimits.SuspendLayout();
            this.rank_grpGeneral.SuspendLayout();
            this.rank_grpMisc.SuspendLayout();
            this.pageMisc.SuspendLayout();
            this.eco_grpEco.SuspendLayout();
            this.grpExtra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldownTime)).BeginInit();
            this.grpMessages.SuspendLayout();
            this.grpPhysics.SuspendLayout();
            this.afk_grp.SuspendLayout();
            this.bak_grp.SuspendLayout();
            this.pageIRC.SuspendLayout();
            this.gb_ircSettings.SuspendLayout();
            this.sql_grp.SuspendLayout();
            this.irc_grp.SuspendLayout();
            this.pageServer.SuspendLayout();
            this.lvl_grp.SuspendLayout();
            this.adv_grp.SuspendLayout();
            this.srv_grp.SuspendLayout();
            this.srv_grpUpdate.SuspendLayout();
            this.grpPlayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srv_numPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.srv_numGuests)).BeginInit();
            this.tabControl.SuspendLayout();
            this.pageGames.SuspendLayout();
            this.tabGames.SuspendLayout();
            this.tabPage10.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.ls_grpMapSettings.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.ls_grpMaps.SuspendLayout();
            this.tabTntWars.SuspendLayout();
            this.groupBox29.SuspendLayout();
            this.groupBox36.SuspendLayout();
            this.groupBox35.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.groupBox33.SuspendLayout();
            this.groupBox32.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TntWrsGraceTimeChck)).BeginInit();
            this.tw_grpScores.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numMultiKills)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScoreAssists)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScorePerKill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScoreLimit)).BeginInit();
            this.tw_grpStatus.SuspendLayout();
            this.tabZS.SuspendLayout();
            this.pageCommands.SuspendLayout();
            this.cmd_grpExtra.SuspendLayout();
            this.cmd_grpPermissions.SuspendLayout();
            this.pageSecurity.SuspendLayout();
            this.sec_grpChat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatMsgs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatSecs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatMute)).BeginInit();
            this.sec_grpCmd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdMsgs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdSecs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdMute)).BeginInit();
            this.sec_grpIP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numIPMsgs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numIPSecs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numIPMute)).BeginInit();
            this.sec_grpOther.SuspendLayout();
            this.sec_grpBlocks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numBlocksMsgs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numBlocksSecs)).BeginInit();
            this.SuspendLayout();
            // 
            // pageChat
            // 
            this.pageChat.BackColor = System.Drawing.SystemColors.Control;
            this.pageChat.Controls.Add(this.chat_grpTab);
            this.pageChat.Controls.Add(this.chat_grpMessages);
            this.pageChat.Controls.Add(this.chat_grpOther);
            this.pageChat.Controls.Add(this.chat_grpColors);
            this.pageChat.Location = new System.Drawing.Point(4, 22);
            this.pageChat.Name = "pageChat";
            this.pageChat.Padding = new System.Windows.Forms.Padding(3);
            this.pageChat.Size = new System.Drawing.Size(498, 521);
            this.pageChat.TabIndex = 10;
            this.pageChat.Text = "Chat";
            // 
            // chat_grpTab
            // 
            this.chat_grpTab.Controls.Add(this.chat_cbTabRank);
            this.chat_grpTab.Controls.Add(this.chat_cbTabLevel);
            this.chat_grpTab.Controls.Add(this.chat_cbTabBots);
            this.chat_grpTab.Location = new System.Drawing.Point(235, 59);
            this.chat_grpTab.Name = "chat_grpTab";
            this.chat_grpTab.Size = new System.Drawing.Size(256, 92);
            this.chat_grpTab.TabIndex = 3;
            this.chat_grpTab.TabStop = false;
            this.chat_grpTab.Text = "Tab list";
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
            // chat_grpMessages
            // 
            this.chat_grpMessages.Controls.Add(this.chat_lblShutdown);
            this.chat_grpMessages.Controls.Add(this.chat_txtShutdown);
            this.chat_grpMessages.Controls.Add(this.chat_chkCheap);
            this.chat_grpMessages.Controls.Add(this.chat_txtCheap);
            this.chat_grpMessages.Controls.Add(this.chat_lblBan);
            this.chat_grpMessages.Controls.Add(this.chat_txtBan);
            this.chat_grpMessages.Controls.Add(this.chat_lblPromote);
            this.chat_grpMessages.Controls.Add(this.chat_txtPromote);
            this.chat_grpMessages.Controls.Add(this.chat_lblDemote);
            this.chat_grpMessages.Controls.Add(this.chat_txtDemote);
            this.chat_grpMessages.Location = new System.Drawing.Point(8, 157);
            this.chat_grpMessages.Name = "chat_grpMessages";
            this.chat_grpMessages.Size = new System.Drawing.Size(483, 180);
            this.chat_grpMessages.TabIndex = 2;
            this.chat_grpMessages.TabStop = false;
            this.chat_grpMessages.Text = "Messages";
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
            // chat_txtShutdown
            // 
            this.chat_txtShutdown.Location = new System.Drawing.Point(134, 20);
            this.chat_txtShutdown.MaxLength = 128;
            this.chat_txtShutdown.Name = "chat_txtShutdown";
            this.chat_txtShutdown.Size = new System.Drawing.Size(343, 21);
            this.chat_txtShutdown.TabIndex = 29;
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
            // chat_txtCheap
            // 
            this.chat_txtCheap.Location = new System.Drawing.Point(134, 50);
            this.chat_txtCheap.Name = "chat_txtCheap";
            this.chat_txtCheap.Size = new System.Drawing.Size(343, 21);
            this.chat_txtCheap.TabIndex = 31;
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
            // chat_txtBan
            // 
            this.chat_txtBan.Location = new System.Drawing.Point(134, 80);
            this.chat_txtBan.MaxLength = 64;
            this.chat_txtBan.Name = "chat_txtBan";
            this.chat_txtBan.Size = new System.Drawing.Size(343, 21);
            this.chat_txtBan.TabIndex = 33;
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
            // chat_txtPromote
            // 
            this.chat_txtPromote.Location = new System.Drawing.Point(134, 110);
            this.chat_txtPromote.MaxLength = 64;
            this.chat_txtPromote.Name = "chat_txtPromote";
            this.chat_txtPromote.Size = new System.Drawing.Size(343, 21);
            this.chat_txtPromote.TabIndex = 36;
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
            // chat_txtDemote
            // 
            this.chat_txtDemote.Location = new System.Drawing.Point(134, 144);
            this.chat_txtDemote.MaxLength = 64;
            this.chat_txtDemote.Name = "chat_txtDemote";
            this.chat_txtDemote.Size = new System.Drawing.Size(343, 21);
            this.chat_txtDemote.TabIndex = 38;
            // 
            // chat_grpOther
            // 
            this.chat_grpOther.Controls.Add(this.chat_lblConsole);
            this.chat_grpOther.Controls.Add(this.chat_txtConsole);
            this.chat_grpOther.Location = new System.Drawing.Point(235, 6);
            this.chat_grpOther.Name = "chat_grpOther";
            this.chat_grpOther.Size = new System.Drawing.Size(256, 47);
            this.chat_grpOther.TabIndex = 1;
            this.chat_grpOther.TabStop = false;
            this.chat_grpOther.Text = "Other";
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
            // 
            // chat_grpColors
            // 
            this.chat_grpColors.Controls.Add(this.chat_lblDefault);
            this.chat_grpColors.Controls.Add(this.chat_btnDefault);
            this.chat_grpColors.Controls.Add(this.chat_lblIRC);
            this.chat_grpColors.Controls.Add(this.chat_btnIRC);
            this.chat_grpColors.Controls.Add(this.chat_lblSyntax);
            this.chat_grpColors.Controls.Add(this.chat_btnSyntax);
            this.chat_grpColors.Controls.Add(this.chat_lblDesc);
            this.chat_grpColors.Controls.Add(this.chat_btnDesc);
            this.chat_grpColors.Location = new System.Drawing.Point(8, 6);
            this.chat_grpColors.Name = "chat_grpColors";
            this.chat_grpColors.Size = new System.Drawing.Size(221, 145);
            this.chat_grpColors.TabIndex = 0;
            this.chat_grpColors.TabStop = false;
            this.chat_grpColors.Text = "Colors";
            // 
            // chat_lblDefault
            // 
            this.chat_lblDefault.AutoSize = true;
            this.chat_lblDefault.Location = new System.Drawing.Point(38, 25);
            this.chat_lblDefault.Name = "chat_lblDefault";
            this.chat_lblDefault.Size = new System.Drawing.Size(71, 13);
            this.chat_lblDefault.TabIndex = 11;
            this.chat_lblDefault.Text = "Default color:";
            // 
            // chat_btnDefault
            // 
            this.chat_btnDefault.Location = new System.Drawing.Point(113, 20);
            this.chat_btnDefault.Name = "chat_btnDefault";
            this.chat_btnDefault.Size = new System.Drawing.Size(95, 23);
            this.chat_btnDefault.TabIndex = 10;
            this.toolTip.SetToolTip(this.chat_btnDefault, "The default color of server messages (excluding player chat).\nFor example, when you are aske" +
                        "d to select two corners in a cuboid.");
            this.chat_btnDefault.Click += new System.EventHandler(this.chat_cmbDefault_Click);
            // 
            // chat_lblIRC
            // 
            this.chat_lblIRC.AutoSize = true;
            this.chat_lblIRC.Location = new System.Drawing.Point(36, 56);
            this.chat_lblIRC.Name = "chat_lblIRC";
            this.chat_lblIRC.Size = new System.Drawing.Size(74, 13);
            this.chat_lblIRC.TabIndex = 22;
            this.chat_lblIRC.Text = "IRC messages:";
            // 
            // chat_btnIRC
            // 
            this.chat_btnIRC.Location = new System.Drawing.Point(113, 51);
            this.chat_btnIRC.Name = "chat_btnIRC";
            this.chat_btnIRC.Size = new System.Drawing.Size(95, 23);
            this.chat_btnIRC.TabIndex = 24;
            this.toolTip.SetToolTip(this.chat_btnIRC, "The color of messages from IRC, and nicknames of IRC users.");
            this.chat_btnIRC.Click += new System.EventHandler(this.chat_btnIRC_Click);
            // 
            // chat_lblSyntax
            // 
            this.chat_lblSyntax.AutoSize = true;
            this.chat_lblSyntax.Location = new System.Drawing.Point(41, 85);
            this.chat_lblSyntax.Name = "chat_lblSyntax";
            this.chat_lblSyntax.Size = new System.Drawing.Size(68, 13);
            this.chat_lblSyntax.TabIndex = 31;
            this.chat_lblSyntax.Text = "/help syntax:";
            // 
            // chat_btnSyntax
            // 
            this.chat_btnSyntax.Location = new System.Drawing.Point(113, 80);
            this.chat_btnSyntax.Name = "chat_btnSyntax";
            this.chat_btnSyntax.Size = new System.Drawing.Size(95, 23);
            this.chat_btnSyntax.TabIndex = 30;
            this.toolTip.SetToolTip(this.chat_btnSyntax, "The color of the /cmdname [args] in /help.");
            this.chat_btnSyntax.Click += new System.EventHandler(this.chat_btnSyntax_Click);
            // 
            // chat_lblDesc
            // 
            this.chat_lblDesc.AutoSize = true;
            this.chat_lblDesc.Location = new System.Drawing.Point(19, 114);
            this.chat_lblDesc.Name = "chat_lblDesc";
            this.chat_lblDesc.Size = new System.Drawing.Size(90, 13);
            this.chat_lblDesc.TabIndex = 32;
            this.chat_lblDesc.Text = "/help description:";
            // 
            // chat_btnDesc
            // 
            this.chat_btnDesc.Location = new System.Drawing.Point(113, 109);
            this.chat_btnDesc.Name = "chat_btnDesc";
            this.chat_btnDesc.Size = new System.Drawing.Size(95, 23);
            this.chat_btnDesc.TabIndex = 33;
            this.toolTip.SetToolTip(this.chat_btnDesc, "The color for the description of a command in /help.");
            this.chat_btnDesc.Click += new System.EventHandler(this.chat_btnDesc_Click);
            // 
            // main_btnSave
            // 
            this.main_btnSave.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnSave.Location = new System.Drawing.Point(346, 553);
            this.main_btnSave.Name = "main_btnSave";
            this.main_btnSave.Size = new System.Drawing.Size(75, 23);
            this.main_btnSave.TabIndex = 1;
            this.main_btnSave.Text = "Save";
            this.main_btnSave.UseVisualStyleBackColor = true;
            this.main_btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // main_btnDiscard
            // 
            this.main_btnDiscard.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnDiscard.Location = new System.Drawing.Point(4, 553);
            this.main_btnDiscard.Name = "main_btnDiscard";
            this.main_btnDiscard.Size = new System.Drawing.Size(75, 23);
            this.main_btnDiscard.TabIndex = 1;
            this.main_btnDiscard.Text = "Discard";
            this.main_btnDiscard.UseVisualStyleBackColor = true;
            this.main_btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
            // 
            // main_btnApply
            // 
            this.main_btnApply.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnApply.Location = new System.Drawing.Point(427, 553);
            this.main_btnApply.Name = "main_btnApply";
            this.main_btnApply.Size = new System.Drawing.Size(75, 23);
            this.main_btnApply.TabIndex = 1;
            this.main_btnApply.Text = "Apply";
            this.main_btnApply.UseVisualStyleBackColor = true;
            this.main_btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 8000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Information";
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
            // rank_cbTPHigher
            // 
            this.rank_cbTPHigher.AutoSize = true;
            this.rank_cbTPHigher.Location = new System.Drawing.Point(11, 75);
            this.rank_cbTPHigher.Name = "rank_cbTPHigher";
            this.rank_cbTPHigher.Size = new System.Drawing.Size(136, 17);
            this.rank_cbTPHigher.TabIndex = 42;
            this.rank_cbTPHigher.Text = "Allow tp to higher ranks";
            this.toolTip.SetToolTip(this.rank_cbTPHigher, "Allows players to /tp to higher ranked players");
            this.rank_cbTPHigher.UseVisualStyleBackColor = true;
            // 
            // rank_cmbDefault
            // 
            this.rank_cmbDefault.FormattingEnabled = true;
            this.rank_cmbDefault.Location = new System.Drawing.Point(85, 20);
            this.rank_cmbDefault.Name = "rank_cmbDefault";
            this.rank_cmbDefault.Size = new System.Drawing.Size(81, 21);
            this.rank_cmbDefault.TabIndex = 44;
            this.toolTip.SetToolTip(this.rank_cmbDefault, "Default rank assigned to new players.");
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
            // afk_txtTimer
            // 
            this.afk_txtTimer.Location = new System.Drawing.Point(61, 16);
            this.afk_txtTimer.Name = "afk_txtTimer";
            this.afk_txtTimer.Size = new System.Drawing.Size(66, 21);
            this.afk_txtTimer.TabIndex = 10;
            this.toolTip.SetToolTip(this.afk_txtTimer, "How many minutes a player can idle before server announces auto afk. (0 = disable" +
                        "d)");
            // 
            // bak_numTime
            // 
            this.bak_numTime.Location = new System.Drawing.Point(81, 43);
            this.bak_numTime.Maximum = new decimal(new int[] {
                                    1000000,
                                    0,
                                    0,
                                    0});
            this.bak_numTime.Name = "bak_numTime";
            this.bak_numTime.Size = new System.Drawing.Size(41, 21);
            this.bak_numTime.TabIndex = 5;
            this.toolTip.SetToolTip(this.bak_numTime, "How often should backups be taken, in seconds.\nDefault = 300");
            this.bak_numTime.Value = new decimal(new int[] {
                                    300,
                                    0,
                                    0,
                                    0});
            // 
            // sql_chkUseSQL
            // 
            this.sql_chkUseSQL.AutoSize = true;
            this.sql_chkUseSQL.Location = new System.Drawing.Point(12, 20);
            this.sql_chkUseSQL.Name = "sql_chkUseSQL";
            this.sql_chkUseSQL.Size = new System.Drawing.Size(77, 17);
            this.sql_chkUseSQL.TabIndex = 28;
            this.sql_chkUseSQL.Text = "Use MySQL";
            this.toolTip.SetToolTip(this.sql_chkUseSQL, "Whether to use MySQL instead of SQLite for database storage. You will need to hav" +
                        "e installed it for this to work.");
            this.sql_chkUseSQL.UseVisualStyleBackColor = true;
            this.sql_chkUseSQL.CheckedChanged += new System.EventHandler(this.sql_chkUseSQL_CheckedChanged);
            // 
            // irc_chkEnabled
            // 
            this.irc_chkEnabled.AutoSize = true;
            this.irc_chkEnabled.Location = new System.Drawing.Point(9, 20);
            this.irc_chkEnabled.Name = "irc_chkEnabled";
            this.irc_chkEnabled.Size = new System.Drawing.Size(61, 17);
            this.irc_chkEnabled.TabIndex = 22;
            this.irc_chkEnabled.Text = "Use IRC";
            this.toolTip.SetToolTip(this.irc_chkEnabled, "Whether to use the IRC bot or not.\nIRC stands for Internet Relay Chat and allows " +
                        "for communication with the server while outside Minecraft.");
            this.irc_chkEnabled.UseVisualStyleBackColor = true;
            this.irc_chkEnabled.CheckedChanged += new System.EventHandler(this.irc_chkEnabled_CheckedChanged);
            // 
            // irc_txtServer
            // 
            this.irc_txtServer.Location = new System.Drawing.Point(82, 47);
            this.irc_txtServer.Name = "irc_txtServer";
            this.irc_txtServer.Size = new System.Drawing.Size(106, 21);
            this.irc_txtServer.TabIndex = 15;
            this.toolTip.SetToolTip(this.irc_txtServer, "IRC server hostname.\nDefault = irc.esper.net\nAnother choice = irc.geekshed.net");
            // 
            // irc_txtNick
            // 
            this.irc_txtNick.Location = new System.Drawing.Point(82, 101);
            this.irc_txtNick.Name = "irc_txtNick";
            this.irc_txtNick.Size = new System.Drawing.Size(106, 21);
            this.irc_txtNick.TabIndex = 16;
            this.toolTip.SetToolTip(this.irc_txtNick, "The Nick that the IRC bot will try and use.");
            // 
            // irc_txtChannel
            // 
            this.irc_txtChannel.Location = new System.Drawing.Point(82, 128);
            this.irc_txtChannel.Name = "irc_txtChannel";
            this.irc_txtChannel.Size = new System.Drawing.Size(106, 21);
            this.irc_txtChannel.TabIndex = 17;
            this.toolTip.SetToolTip(this.irc_txtChannel, "The IRC channel to be used.");
            // 
            // irc_txtOpChannel
            // 
            this.irc_txtOpChannel.Location = new System.Drawing.Point(82, 155);
            this.irc_txtOpChannel.Name = "irc_txtOpChannel";
            this.irc_txtOpChannel.Size = new System.Drawing.Size(106, 21);
            this.irc_txtOpChannel.TabIndex = 26;
            this.toolTip.SetToolTip(this.irc_txtOpChannel, "The IRC channel to be used.");
            // 
            // lvl_chkAutoload
            // 
            this.lvl_chkAutoload.AutoSize = true;
            this.lvl_chkAutoload.Location = new System.Drawing.Point(9, 49);
            this.lvl_chkAutoload.Name = "lvl_chkAutoload";
            this.lvl_chkAutoload.Size = new System.Drawing.Size(90, 17);
            this.lvl_chkAutoload.TabIndex = 4;
            this.lvl_chkAutoload.Text = "Load on /goto";
            this.toolTip.SetToolTip(this.lvl_chkAutoload, "Load a map when a user wishes to go to it, and unload empty maps");
            this.lvl_chkAutoload.UseVisualStyleBackColor = true;
            // 
            // lvl_chkWorld
            // 
            this.lvl_chkWorld.AutoSize = true;
            this.lvl_chkWorld.Location = new System.Drawing.Point(9, 72);
            this.lvl_chkWorld.Name = "lvl_chkWorld";
            this.lvl_chkWorld.Size = new System.Drawing.Size(105, 17);
            this.lvl_chkWorld.TabIndex = 4;
            this.lvl_chkWorld.Text = "Server-wide chat";
            this.toolTip.SetToolTip(this.lvl_chkWorld, "If disabled, every map has isolated chat.\nIf enabled, every map is able to commun" +
                        "icate without special letters.");
            this.lvl_chkWorld.UseVisualStyleBackColor = true;
            // 
            // adv_chkVerify
            // 
            this.adv_chkVerify.AutoSize = true;
            this.adv_chkVerify.Location = new System.Drawing.Point(9, 20);
            this.adv_chkVerify.Name = "adv_chkVerify";
            this.adv_chkVerify.Size = new System.Drawing.Size(87, 17);
            this.adv_chkVerify.TabIndex = 4;
            this.adv_chkVerify.Text = "Verify Names";
            this.toolTip.SetToolTip(this.adv_chkVerify, "Make sure the user is who they claim to be.");
            this.adv_chkVerify.UseVisualStyleBackColor = true;
            // 
            // srv_txtName
            // 
            this.srv_txtName.Location = new System.Drawing.Point(83, 19);
            this.srv_txtName.MaxLength = 64;
            this.srv_txtName.Name = "srv_txtName";
            this.srv_txtName.Size = new System.Drawing.Size(387, 21);
            this.srv_txtName.TabIndex = 0;
            this.toolTip.SetToolTip(this.srv_txtName, "The name of the server.\nPick something good!");
            // 
            // srv_txtMOTD
            // 
            this.srv_txtMOTD.Location = new System.Drawing.Point(83, 46);
            this.srv_txtMOTD.MaxLength = 64;
            this.srv_txtMOTD.Name = "srv_txtMOTD";
            this.srv_txtMOTD.Size = new System.Drawing.Size(387, 21);
            this.srv_txtMOTD.TabIndex = 1;
            this.toolTip.SetToolTip(this.srv_txtMOTD, "The MOTD of the server.\nUse \"+hax\" to allow any WoM hack, \"-hax\" to disallow any " +
                        "hacks at all and use \"-fly\" and whatnot to disallow other things.");
            // 
            // srv_numPort
            // 
            this.srv_numPort.Location = new System.Drawing.Point(83, 73);
            this.srv_numPort.Maximum = new decimal(new int[] {
                                    65535,
                                    0,
                                    0,
                                    0});
            this.srv_numPort.Name = "srv_numPort";
            this.srv_numPort.Size = new System.Drawing.Size(60, 21);
            this.srv_numPort.TabIndex = 2;
            this.toolTip.SetToolTip(this.srv_numPort, "The port that the server will output on.\nDefault = 25565\n\nChanging will reset you" +
                        "r ExternalURL.");
            this.srv_numPort.Value = new decimal(new int[] {
                                    25565,
                                    0,
                                    0,
                                    0});
            // 
            // srv_chkPublic
            // 
            this.srv_chkPublic.AutoSize = true;
            this.srv_chkPublic.Location = new System.Drawing.Point(9, 124);
            this.srv_chkPublic.Name = "srv_chkPublic";
            this.srv_chkPublic.Size = new System.Drawing.Size(55, 17);
            this.srv_chkPublic.TabIndex = 5;
            this.srv_chkPublic.Text = "Public";
            this.toolTip.SetToolTip(this.srv_chkPublic, "Whether or not the server will appear on the server list.");
            this.srv_chkPublic.UseVisualStyleBackColor = true;
            // 
            // rank_cbSilentAdmins
            // 
            this.rank_cbSilentAdmins.AutoSize = true;
            this.rank_cbSilentAdmins.Location = new System.Drawing.Point(11, 52);
            this.rank_cbSilentAdmins.Name = "rank_cbSilentAdmins";
            this.rank_cbSilentAdmins.Size = new System.Drawing.Size(118, 17);
            this.rank_cbSilentAdmins.TabIndex = 41;
            this.rank_cbSilentAdmins.Text = "Admins join silently";
            this.toolTip.SetToolTip(this.rank_cbSilentAdmins, "Players who can read adminchat also join the game silently");
            this.rank_cbSilentAdmins.UseVisualStyleBackColor = true;
            // 
            // rank_txtPrefix
            // 
            this.rank_txtPrefix.Location = new System.Drawing.Point(259, 47);
            this.rank_txtPrefix.Name = "rank_txtPrefix";
            this.rank_txtPrefix.Size = new System.Drawing.Size(81, 21);
            this.rank_txtPrefix.TabIndex = 21;
            this.toolTip.SetToolTip(this.rank_txtPrefix, "Short prefix showed before player names in chat.");
            this.rank_txtPrefix.TextChanged += new System.EventHandler(this.rank_txtPrefix_TextChanged);
            // 
            // rank_txtMOTD
            // 
            this.rank_txtMOTD.Location = new System.Drawing.Point(85, 74);
            this.rank_txtMOTD.Name = "rank_txtMOTD";
            this.rank_txtMOTD.Size = new System.Drawing.Size(255, 21);
            this.rank_txtMOTD.TabIndex = 17;
            this.toolTip.SetToolTip(this.rank_txtMOTD, "MOTD shown to players of this rank.\r\nIf left blank, the server MOTD is shown to t" +
                        "hem.");
            this.rank_txtMOTD.TextChanged += new System.EventHandler(this.rank_txtMOTD_TextChanged);
            // 
            // rank_numPerm
            // 
            this.rank_numPerm.Location = new System.Drawing.Point(259, 20);
            this.rank_numPerm.Maximum = new decimal(new int[] {
                                    120,
                                    0,
                                    0,
                                    0});
            this.rank_numPerm.Minimum = new decimal(new int[] {
                                    50,
                                    0,
                                    0,
                                    -2147483648});
            this.rank_numPerm.Name = "rank_numPerm";
            this.rank_numPerm.Size = new System.Drawing.Size(81, 21);
            this.rank_numPerm.TabIndex = 6;
            this.toolTip.SetToolTip(this.rank_numPerm, "Permission level of this rank.");
            this.rank_numPerm.ValueChanged += new System.EventHandler(this.rank_numPerm_ValueChanged);
            // 
            // rank_txtName
            // 
            this.rank_txtName.Location = new System.Drawing.Point(85, 20);
            this.rank_txtName.Name = "rank_txtName";
            this.rank_txtName.Size = new System.Drawing.Size(81, 21);
            this.rank_txtName.TabIndex = 5;
            this.toolTip.SetToolTip(this.rank_txtName, "Name of this rank");
            this.rank_txtName.TextChanged += new System.EventHandler(this.rank_txtName_TextChanged);
            // 
            // rank_btnColor
            // 
            this.rank_btnColor.Location = new System.Drawing.Point(85, 47);
            this.rank_btnColor.Name = "rank_btnColor";
            this.rank_btnColor.Size = new System.Drawing.Size(81, 23);
            this.rank_btnColor.TabIndex = 12;
            this.toolTip.SetToolTip(this.rank_btnColor, "Color of this rank name in chat and the tab list");
            this.rank_btnColor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rank_btnColor_Click);
            // 
            // rank_cmbOsMap
            // 
            this.rank_cmbOsMap.FormattingEnabled = true;
            this.rank_cmbOsMap.Location = new System.Drawing.Point(259, 20);
            this.rank_cmbOsMap.Name = "rank_cmbOsMap";
            this.rank_cmbOsMap.Size = new System.Drawing.Size(80, 21);
            this.rank_cmbOsMap.TabIndex = 49;
            this.toolTip.SetToolTip(this.rank_cmbOsMap, "Default minimum rank required to build on maps made with /os map add.\nIf \'nobody\'" +
                        " is selected, rank required is the minimum rank that can use /os.");
            // 
            // irc_chkPass
            // 
            this.irc_chkPass.AutoSize = true;
            this.irc_chkPass.Location = new System.Drawing.Point(9, 185);
            this.irc_chkPass.Name = "irc_chkPass";
            this.irc_chkPass.Size = new System.Drawing.Size(72, 17);
            this.irc_chkPass.TabIndex = 27;
            this.irc_chkPass.Text = "Password";
            this.toolTip.SetToolTip(this.irc_chkPass, "NickServ password set for the username");
            this.irc_chkPass.UseVisualStyleBackColor = true;
            this.irc_chkPass.CheckedChanged += new System.EventHandler(this.irc_chkPass_CheckedChanged);
            // 
            // irc_txtPass
            // 
            this.irc_txtPass.Location = new System.Drawing.Point(82, 182);
            this.irc_txtPass.Name = "irc_txtPass";
            this.irc_txtPass.PasswordChar = '*';
            this.irc_txtPass.Size = new System.Drawing.Size(106, 21);
            this.irc_txtPass.TabIndex = 28;
            this.toolTip.SetToolTip(this.irc_txtPass, "NickServ password set for the username");
            // 
            // rank_numMaps
            // 
            this.rank_numMaps.Location = new System.Drawing.Point(259, 20);
            this.rank_numMaps.Maximum = new decimal(new int[] {
                                    255,
                                    0,
                                    0,
                                    0});
            this.rank_numMaps.Name = "rank_numMaps";
            this.rank_numMaps.Size = new System.Drawing.Size(81, 21);
            this.rank_numMaps.TabIndex = 19;
            this.toolTip.SetToolTip(this.rank_numMaps, "Maximum number of /os maps players are allowed");
            this.rank_numMaps.ValueChanged += new System.EventHandler(this.rank_numMaps_ValueChanged);
            // 
            // rank_numDraw
            // 
            this.rank_numDraw.Location = new System.Drawing.Point(85, 20);
            this.rank_numDraw.Maximum = new decimal(new int[] {
                                    2147483647,
                                    0,
                                    0,
                                    0});
            this.rank_numDraw.Name = "rank_numDraw";
            this.rank_numDraw.Size = new System.Drawing.Size(81, 21);
            this.rank_numDraw.TabIndex = 4;
            this.toolTip.SetToolTip(this.rank_numDraw, "Maximum number of blocks players can affect in draw commands.");
            this.rank_numDraw.ValueChanged += new System.EventHandler(this.rank_numDraw_ValueChanged);
            // 
            // rank_numUndo
            // 
            this.rank_numUndo.Location = new System.Drawing.Point(85, 47);
            this.rank_numUndo.Maximum = new decimal(new int[] {
                                    2147483647,
                                    0,
                                    0,
                                    0});
            this.rank_numUndo.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    -2147483648});
            this.rank_numUndo.Name = "rank_numUndo";
            this.rank_numUndo.Size = new System.Drawing.Size(81, 21);
            this.rank_numUndo.TabIndex = 15;
            this.toolTip.SetToolTip(this.rank_numUndo, "Maximum number of seconds players can undo up to in the past with /undo");
            this.rank_numUndo.ValueChanged += new System.EventHandler(this.rank_numUndo_ValueChanged);
            // 
            // rank_numGen
            // 
            this.rank_numGen.Location = new System.Drawing.Point(259, 47);
            this.rank_numGen.Maximum = new decimal(new int[] {
                                    2147483647,
                                    0,
                                    0,
                                    0});
            this.rank_numGen.Name = "rank_numGen";
            this.rank_numGen.Size = new System.Drawing.Size(81, 21);
            this.rank_numGen.TabIndex = 21;
            this.toolTip.SetToolTip(this.rank_numGen, "Maximum volume of (number of blocks in) a map that players can generate");
            this.rank_numGen.ValueChanged += new System.EventHandler(this.rank_numGen_ValueChanged);
            // 
            // rank_numAfk
            // 
            this.rank_numAfk.Location = new System.Drawing.Point(113, 102);
            this.rank_numAfk.Maximum = new decimal(new int[] {
                                    100000,
                                    0,
                                    0,
                                    0});
            this.rank_numAfk.Name = "rank_numAfk";
            this.rank_numAfk.Size = new System.Drawing.Size(61, 21);
            this.rank_numAfk.TabIndex = 23;
            this.rank_numAfk.ValueChanged += new System.EventHandler(this.rank_numAfk_ValueChanged);
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
            this.pageBlocks.Controls.Add(this.blk_grpPhysics);
            this.pageBlocks.Controls.Add(this.blk_grpBehaviour);
            this.pageBlocks.Controls.Add(this.blk_grpPermissions);
            this.pageBlocks.Controls.Add(this.blk_btnHelp);
            this.pageBlocks.Controls.Add(this.blk_list);
            this.pageBlocks.Location = new System.Drawing.Point(4, 22);
            this.pageBlocks.Name = "pageBlocks";
            this.pageBlocks.Padding = new System.Windows.Forms.Padding(3);
            this.pageBlocks.Size = new System.Drawing.Size(498, 521);
            this.pageBlocks.TabIndex = 5;
            this.pageBlocks.Text = "Blocks";
            // 
            // blk_grpPhysics
            // 
            this.blk_grpPhysics.Controls.Add(this.blk_cbWater);
            this.blk_grpPhysics.Controls.Add(this.blk_cbLava);
            this.blk_grpPhysics.Controls.Add(this.blk_cbRails);
            this.blk_grpPhysics.Controls.Add(this.blk_cbTdoor);
            this.blk_grpPhysics.Controls.Add(this.blk_cbDoor);
            this.blk_grpPhysics.Location = new System.Drawing.Point(134, 180);
            this.blk_grpPhysics.Name = "blk_grpPhysics";
            this.blk_grpPhysics.Size = new System.Drawing.Size(360, 92);
            this.blk_grpPhysics.TabIndex = 26;
            this.blk_grpPhysics.TabStop = false;
            this.blk_grpPhysics.Text = "Physics behaviour";
            // 
            // blk_cbWater
            // 
            this.blk_cbWater.Location = new System.Drawing.Point(186, 65);
            this.blk_cbWater.Name = "blk_cbWater";
            this.blk_cbWater.Size = new System.Drawing.Size(104, 24);
            this.blk_cbWater.TabIndex = 7;
            this.blk_cbWater.Text = "Killed by water";
            this.blk_cbWater.UseVisualStyleBackColor = true;
            this.blk_cbWater.CheckedChanged += new System.EventHandler(this.blk_cbWater_CheckedChanged);
            // 
            // blk_cbLava
            // 
            this.blk_cbLava.Location = new System.Drawing.Point(10, 65);
            this.blk_cbLava.Name = "blk_cbLava";
            this.blk_cbLava.Size = new System.Drawing.Size(116, 24);
            this.blk_cbLava.TabIndex = 6;
            this.blk_cbLava.Text = "Killed by lava";
            this.blk_cbLava.UseVisualStyleBackColor = true;
            this.blk_cbLava.CheckedChanged += new System.EventHandler(this.blk_cbLava_CheckedChanged);
            // 
            // blk_cbRails
            // 
            this.blk_cbRails.Location = new System.Drawing.Point(10, 40);
            this.blk_cbRails.Name = "blk_cbRails";
            this.blk_cbRails.Size = new System.Drawing.Size(89, 24);
            this.blk_cbRails.TabIndex = 5;
            this.blk_cbRails.Text = "Is train rails";
            this.blk_cbRails.UseVisualStyleBackColor = true;
            this.blk_cbRails.CheckedChanged += new System.EventHandler(this.blk_cbRails_CheckedChanged);
            // 
            // blk_cbTdoor
            // 
            this.blk_cbTdoor.Location = new System.Drawing.Point(187, 15);
            this.blk_cbTdoor.Name = "blk_cbTdoor";
            this.blk_cbTdoor.Size = new System.Drawing.Size(104, 24);
            this.blk_cbTdoor.TabIndex = 4;
            this.blk_cbTdoor.Text = "Is a tDoor";
            this.blk_cbTdoor.UseVisualStyleBackColor = true;
            this.blk_cbTdoor.CheckedChanged += new System.EventHandler(this.blk_cbTdoor_CheckedChanged);
            // 
            // blk_cbDoor
            // 
            this.blk_cbDoor.Location = new System.Drawing.Point(10, 15);
            this.blk_cbDoor.Name = "blk_cbDoor";
            this.blk_cbDoor.Size = new System.Drawing.Size(116, 24);
            this.blk_cbDoor.TabIndex = 3;
            this.blk_cbDoor.Text = "Is a door";
            this.blk_cbDoor.UseVisualStyleBackColor = true;
            this.blk_cbDoor.CheckedChanged += new System.EventHandler(this.blk_cbDoor_CheckedChanged);
            // 
            // blk_grpBehaviour
            // 
            this.blk_grpBehaviour.Controls.Add(this.blk_txtDeath);
            this.blk_grpBehaviour.Controls.Add(this.blk_cbDeath);
            this.blk_grpBehaviour.Controls.Add(this.blk_cbPortal);
            this.blk_grpBehaviour.Controls.Add(this.blk_cbMsgBlock);
            this.blk_grpBehaviour.Location = new System.Drawing.Point(133, 105);
            this.blk_grpBehaviour.Name = "blk_grpBehaviour";
            this.blk_grpBehaviour.Size = new System.Drawing.Size(360, 70);
            this.blk_grpBehaviour.TabIndex = 25;
            this.blk_grpBehaviour.TabStop = false;
            this.blk_grpBehaviour.Text = "Behaviour";
            // 
            // blk_txtDeath
            // 
            this.blk_txtDeath.Location = new System.Drawing.Point(100, 42);
            this.blk_txtDeath.Name = "blk_txtDeath";
            this.blk_txtDeath.Size = new System.Drawing.Size(254, 21);
            this.blk_txtDeath.TabIndex = 3;
            this.blk_txtDeath.TextChanged += new System.EventHandler(this.blk_txtDeath_TextChanged);
            // 
            // blk_cbDeath
            // 
            this.blk_cbDeath.Location = new System.Drawing.Point(10, 40);
            this.blk_cbDeath.Name = "blk_cbDeath";
            this.blk_cbDeath.Size = new System.Drawing.Size(89, 24);
            this.blk_cbDeath.TabIndex = 2;
            this.blk_cbDeath.Text = "Kills players";
            this.blk_cbDeath.UseVisualStyleBackColor = true;
            this.blk_cbDeath.CheckedChanged += new System.EventHandler(this.blk_cbDeath_CheckedChanged);
            // 
            // blk_cbPortal
            // 
            this.blk_cbPortal.Location = new System.Drawing.Point(187, 15);
            this.blk_cbPortal.Name = "blk_cbPortal";
            this.blk_cbPortal.Size = new System.Drawing.Size(104, 24);
            this.blk_cbPortal.TabIndex = 1;
            this.blk_cbPortal.Text = "Is a portal";
            this.blk_cbPortal.UseVisualStyleBackColor = true;
            this.blk_cbPortal.CheckedChanged += new System.EventHandler(this.blk_cbPortal_CheckedChanged);
            // 
            // blk_cbMsgBlock
            // 
            this.blk_cbMsgBlock.Location = new System.Drawing.Point(10, 15);
            this.blk_cbMsgBlock.Name = "blk_cbMsgBlock";
            this.blk_cbMsgBlock.Size = new System.Drawing.Size(116, 24);
            this.blk_cbMsgBlock.TabIndex = 0;
            this.blk_cbMsgBlock.Text = "Is a message block";
            this.blk_cbMsgBlock.UseVisualStyleBackColor = true;
            this.blk_cbMsgBlock.CheckedChanged += new System.EventHandler(this.blk_cbMsgBlock_CheckedChanged);
            // 
            // blk_grpPermissions
            // 
            this.blk_grpPermissions.Controls.Add(this.blk_cmbAlw3);
            this.blk_grpPermissions.Controls.Add(this.blk_cmbAlw2);
            this.blk_grpPermissions.Controls.Add(this.blk_cmbDis3);
            this.blk_grpPermissions.Controls.Add(this.blk_cmbDis2);
            this.blk_grpPermissions.Controls.Add(this.blk_cmbAlw1);
            this.blk_grpPermissions.Controls.Add(this.blk_cmbDis1);
            this.blk_grpPermissions.Controls.Add(this.blk_cmbMin);
            this.blk_grpPermissions.Controls.Add(this.blk_lblMin);
            this.blk_grpPermissions.Controls.Add(this.blk_lblAllow);
            this.blk_grpPermissions.Controls.Add(this.blk_lblDisallow);
            this.blk_grpPermissions.Location = new System.Drawing.Point(133, 6);
            this.blk_grpPermissions.Name = "blk_grpPermissions";
            this.blk_grpPermissions.Size = new System.Drawing.Size(360, 94);
            this.blk_grpPermissions.TabIndex = 24;
            this.blk_grpPermissions.TabStop = false;
            this.blk_grpPermissions.Text = "Permissions";
            // 
            // blk_cmbAlw3
            // 
            this.blk_cmbAlw3.FormattingEnabled = true;
            this.blk_cmbAlw3.Location = new System.Drawing.Point(274, 67);
            this.blk_cmbAlw3.Name = "blk_cmbAlw3";
            this.blk_cmbAlw3.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbAlw3.TabIndex = 28;
            this.blk_cmbAlw3.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbAlw2
            // 
            this.blk_cmbAlw2.FormattingEnabled = true;
            this.blk_cmbAlw2.Location = new System.Drawing.Point(187, 67);
            this.blk_cmbAlw2.Name = "blk_cmbAlw2";
            this.blk_cmbAlw2.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbAlw2.TabIndex = 27;
            this.blk_cmbAlw2.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbDis3
            // 
            this.blk_cmbDis3.FormattingEnabled = true;
            this.blk_cmbDis3.Location = new System.Drawing.Point(274, 41);
            this.blk_cmbDis3.Name = "blk_cmbDis3";
            this.blk_cmbDis3.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbDis3.TabIndex = 26;
            this.blk_cmbDis3.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbDis2
            // 
            this.blk_cmbDis2.FormattingEnabled = true;
            this.blk_cmbDis2.Location = new System.Drawing.Point(187, 41);
            this.blk_cmbDis2.Name = "blk_cmbDis2";
            this.blk_cmbDis2.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbDis2.TabIndex = 25;
            this.blk_cmbDis2.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbAlw1
            // 
            this.blk_cmbAlw1.FormattingEnabled = true;
            this.blk_cmbAlw1.Location = new System.Drawing.Point(100, 67);
            this.blk_cmbAlw1.Name = "blk_cmbAlw1";
            this.blk_cmbAlw1.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbAlw1.TabIndex = 24;
            this.blk_cmbAlw1.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbDis1
            // 
            this.blk_cmbDis1.FormattingEnabled = true;
            this.blk_cmbDis1.Location = new System.Drawing.Point(100, 41);
            this.blk_cmbDis1.Name = "blk_cmbDis1";
            this.blk_cmbDis1.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbDis1.TabIndex = 23;
            this.blk_cmbDis1.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbMin
            // 
            this.blk_cmbMin.FormattingEnabled = true;
            this.blk_cmbMin.Location = new System.Drawing.Point(100, 14);
            this.blk_cmbMin.Name = "blk_cmbMin";
            this.blk_cmbMin.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbMin.TabIndex = 22;
            this.blk_cmbMin.SelectedIndexChanged += new System.EventHandler(this.blk_cmbMin_SelectedIndexChanged);
            // 
            // blk_lblMin
            // 
            this.blk_lblMin.AutoSize = true;
            this.blk_lblMin.Location = new System.Drawing.Point(10, 17);
            this.blk_lblMin.Name = "blk_lblMin";
            this.blk_lblMin.Size = new System.Drawing.Size(89, 13);
            this.blk_lblMin.TabIndex = 16;
            this.blk_lblMin.Text = "Min rank needed:";
            // 
            // blk_lblAllow
            // 
            this.blk_lblAllow.AutoSize = true;
            this.blk_lblAllow.Location = new System.Drawing.Point(10, 71);
            this.blk_lblAllow.Name = "blk_lblAllow";
            this.blk_lblAllow.Size = new System.Drawing.Size(91, 13);
            this.blk_lblAllow.TabIndex = 18;
            this.blk_lblAllow.Text = "Specifically allow:";
            // 
            // blk_lblDisallow
            // 
            this.blk_lblDisallow.AutoSize = true;
            this.blk_lblDisallow.Location = new System.Drawing.Point(10, 44);
            this.blk_lblDisallow.Name = "blk_lblDisallow";
            this.blk_lblDisallow.Size = new System.Drawing.Size(80, 13);
            this.blk_lblDisallow.TabIndex = 17;
            this.blk_lblDisallow.Text = "But don\'t allow:";
            // 
            // blk_btnHelp
            // 
            this.blk_btnHelp.Location = new System.Drawing.Point(6, 485);
            this.blk_btnHelp.Name = "blk_btnHelp";
            this.blk_btnHelp.Size = new System.Drawing.Size(121, 29);
            this.blk_btnHelp.TabIndex = 23;
            this.blk_btnHelp.Text = "Help information";
            this.blk_btnHelp.UseVisualStyleBackColor = true;
            this.blk_btnHelp.Click += new System.EventHandler(this.blk_btnHelp_Click);
            // 
            // blk_list
            // 
            this.blk_list.FormattingEnabled = true;
            this.blk_list.Location = new System.Drawing.Point(6, 6);
            this.blk_list.Name = "blk_list";
            this.blk_list.Size = new System.Drawing.Size(121, 472);
            this.blk_list.TabIndex = 15;
            this.blk_list.SelectedIndexChanged += new System.EventHandler(this.blk_list_SelectedIndexChanged);
            // 
            // pageRanks
            // 
            this.pageRanks.BackColor = System.Drawing.SystemColors.Control;
            this.pageRanks.Controls.Add(this.rank_grpLimits);
            this.pageRanks.Controls.Add(this.rank_grpGeneral);
            this.pageRanks.Controls.Add(this.rank_grpMisc);
            this.pageRanks.Controls.Add(this.rank_btnDel);
            this.pageRanks.Controls.Add(this.rank_btnAdd);
            this.pageRanks.Controls.Add(this.rank_list);
            this.pageRanks.Location = new System.Drawing.Point(4, 22);
            this.pageRanks.Name = "pageRanks";
            this.pageRanks.Padding = new System.Windows.Forms.Padding(3);
            this.pageRanks.Size = new System.Drawing.Size(498, 521);
            this.pageRanks.TabIndex = 4;
            this.pageRanks.Text = "Ranks";
            // 
            // rank_grpLimits
            // 
            this.rank_grpLimits.Controls.Add(this.rank_lblGen);
            this.rank_grpLimits.Controls.Add(this.rank_numGen);
            this.rank_grpLimits.Controls.Add(this.rank_lblMaps);
            this.rank_grpLimits.Controls.Add(this.rank_numMaps);
            this.rank_grpLimits.Controls.Add(this.rank_numDraw);
            this.rank_grpLimits.Controls.Add(this.rank_lblDraw);
            this.rank_grpLimits.Controls.Add(this.rank_numUndo);
            this.rank_grpLimits.Controls.Add(this.rank_lblUndo);
            this.rank_grpLimits.Location = new System.Drawing.Point(142, 143);
            this.rank_grpLimits.Name = "rank_grpLimits";
            this.rank_grpLimits.Size = new System.Drawing.Size(349, 79);
            this.rank_grpLimits.TabIndex = 22;
            this.rank_grpLimits.TabStop = false;
            this.rank_grpLimits.Text = "Rank limits";
            // 
            // rank_lblGen
            // 
            this.rank_lblGen.AutoSize = true;
            this.rank_lblGen.Location = new System.Drawing.Point(185, 50);
            this.rank_lblGen.Name = "rank_lblGen";
            this.rank_lblGen.Size = new System.Drawing.Size(68, 13);
            this.rank_lblGen.TabIndex = 20;
            this.rank_lblGen.Text = "/gen volume:";
            this.rank_lblGen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rank_lblMaps
            // 
            this.rank_lblMaps.AutoSize = true;
            this.rank_lblMaps.Location = new System.Drawing.Point(200, 23);
            this.rank_lblMaps.Name = "rank_lblMaps";
            this.rank_lblMaps.Size = new System.Drawing.Size(53, 13);
            this.rank_lblMaps.TabIndex = 18;
            this.rank_lblMaps.Text = "/os maps:";
            this.rank_lblMaps.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rank_lblDraw
            // 
            this.rank_lblDraw.AutoSize = true;
            this.rank_lblDraw.Location = new System.Drawing.Point(20, 23);
            this.rank_lblDraw.Name = "rank_lblDraw";
            this.rank_lblDraw.Size = new System.Drawing.Size(59, 13);
            this.rank_lblDraw.TabIndex = 3;
            this.rank_lblDraw.Text = "Draw limit:";
            this.rank_lblDraw.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rank_lblUndo
            // 
            this.rank_lblUndo.AutoSize = true;
            this.rank_lblUndo.Location = new System.Drawing.Point(19, 50);
            this.rank_lblUndo.Name = "rank_lblUndo";
            this.rank_lblUndo.Size = new System.Drawing.Size(60, 13);
            this.rank_lblUndo.TabIndex = 14;
            this.rank_lblUndo.Text = "Max /undo:";
            this.rank_lblUndo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rank_grpGeneral
            // 
            this.rank_grpGeneral.Controls.Add(this.rank_lblOsMap);
            this.rank_grpGeneral.Controls.Add(this.rank_cmbOsMap);
            this.rank_grpGeneral.Controls.Add(this.rank_cbEmpty);
            this.rank_grpGeneral.Controls.Add(this.rank_lblDefault);
            this.rank_grpGeneral.Controls.Add(this.rank_cmbDefault);
            this.rank_grpGeneral.Controls.Add(this.rank_cbSilentAdmins);
            this.rank_grpGeneral.Controls.Add(this.rank_cbTPHigher);
            this.rank_grpGeneral.Location = new System.Drawing.Point(142, 228);
            this.rank_grpGeneral.Name = "rank_grpGeneral";
            this.rank_grpGeneral.Size = new System.Drawing.Size(349, 121);
            this.rank_grpGeneral.TabIndex = 19;
            this.rank_grpGeneral.TabStop = false;
            this.rank_grpGeneral.Text = "General settings";
            // 
            // rank_lblOsMap
            // 
            this.rank_lblOsMap.AutoSize = true;
            this.rank_lblOsMap.Location = new System.Drawing.Point(186, 23);
            this.rank_lblOsMap.Name = "rank_lblOsMap";
            this.rank_lblOsMap.Size = new System.Drawing.Size(67, 13);
            this.rank_lblOsMap.TabIndex = 50;
            this.rank_lblOsMap.Text = "/os perbuild:";
            // 
            // rank_cbEmpty
            // 
            this.rank_cbEmpty.AutoSize = true;
            this.rank_cbEmpty.Location = new System.Drawing.Point(11, 98);
            this.rank_cbEmpty.Name = "rank_cbEmpty";
            this.rank_cbEmpty.Size = new System.Drawing.Size(163, 17);
            this.rank_cbEmpty.TabIndex = 45;
            this.rank_cbEmpty.Text = "Show empty ranks in /players";
            this.rank_cbEmpty.UseVisualStyleBackColor = true;
            // 
            // rank_lblDefault
            // 
            this.rank_lblDefault.AutoSize = true;
            this.rank_lblDefault.Location = new System.Drawing.Point(11, 23);
            this.rank_lblDefault.Name = "rank_lblDefault";
            this.rank_lblDefault.Size = new System.Drawing.Size(68, 13);
            this.rank_lblDefault.TabIndex = 43;
            this.rank_lblDefault.Text = "Default rank:";
            // 
            // rank_grpMisc
            // 
            this.rank_grpMisc.Controls.Add(this.rank_numAfk);
            this.rank_grpMisc.Controls.Add(this.rank_cbAfk);
            this.rank_grpMisc.Controls.Add(this.rank_lblAfk);
            this.rank_grpMisc.Controls.Add(this.rank_lblPrefix);
            this.rank_grpMisc.Controls.Add(this.rank_txtPrefix);
            this.rank_grpMisc.Controls.Add(this.rank_lblPerm);
            this.rank_grpMisc.Controls.Add(this.rank_txtMOTD);
            this.rank_grpMisc.Controls.Add(this.rank_numPerm);
            this.rank_grpMisc.Controls.Add(this.rank_txtName);
            this.rank_grpMisc.Controls.Add(this.rank_btnColor);
            this.rank_grpMisc.Controls.Add(this.rank_lblMOTD);
            this.rank_grpMisc.Controls.Add(this.rank_lblName);
            this.rank_grpMisc.Controls.Add(this.rank_lblColor);
            this.rank_grpMisc.Location = new System.Drawing.Point(142, 6);
            this.rank_grpMisc.Name = "rank_grpMisc";
            this.rank_grpMisc.Size = new System.Drawing.Size(349, 131);
            this.rank_grpMisc.TabIndex = 18;
            this.rank_grpMisc.TabStop = false;
            this.rank_grpMisc.Text = "Rank settings";
            // 
            // rank_cbAfk
            // 
            this.rank_cbAfk.Location = new System.Drawing.Point(11, 102);
            this.rank_cbAfk.Name = "rank_cbAfk";
            this.rank_cbAfk.Size = new System.Drawing.Size(108, 24);
            this.rank_cbAfk.TabIndex = 22;
            this.rank_cbAfk.Text = "Kick after AFK for";
            this.rank_cbAfk.UseVisualStyleBackColor = true;
            this.rank_cbAfk.CheckedChanged += new System.EventHandler(this.rank_cbAfk_CheckedChanged);
            // 
            // rank_lblAfk
            // 
            this.rank_lblAfk.AutoSize = true;
            this.rank_lblAfk.Location = new System.Drawing.Point(176, 106);
            this.rank_lblAfk.Name = "rank_lblAfk";
            this.rank_lblAfk.Size = new System.Drawing.Size(37, 13);
            this.rank_lblAfk.TabIndex = 23;
            this.rank_lblAfk.Text = "minutes";
            this.rank_lblAfk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rank_lblPrefix
            // 
            this.rank_lblPrefix.AutoSize = true;
            this.rank_lblPrefix.Location = new System.Drawing.Point(216, 50);
            this.rank_lblPrefix.Name = "rank_lblPrefix";
            this.rank_lblPrefix.Size = new System.Drawing.Size(37, 13);
            this.rank_lblPrefix.TabIndex = 20;
            this.rank_lblPrefix.Text = "Prefix:";
            this.rank_lblPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rank_lblPerm
            // 
            this.rank_lblPerm.AutoSize = true;
            this.rank_lblPerm.Location = new System.Drawing.Point(190, 23);
            this.rank_lblPerm.Name = "rank_lblPerm";
            this.rank_lblPerm.Size = new System.Drawing.Size(63, 13);
            this.rank_lblPerm.TabIndex = 7;
            this.rank_lblPerm.Text = "Permission:";
            this.rank_lblPerm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rank_lblMOTD
            // 
            this.rank_lblMOTD.AutoSize = true;
            this.rank_lblMOTD.Location = new System.Drawing.Point(41, 78);
            this.rank_lblMOTD.Name = "rank_lblMOTD";
            this.rank_lblMOTD.Size = new System.Drawing.Size(38, 13);
            this.rank_lblMOTD.TabIndex = 16;
            this.rank_lblMOTD.Text = "MOTD:";
            // 
            // rank_lblName
            // 
            this.rank_lblName.AutoSize = true;
            this.rank_lblName.Location = new System.Drawing.Point(41, 23);
            this.rank_lblName.Name = "rank_lblName";
            this.rank_lblName.Size = new System.Drawing.Size(38, 13);
            this.rank_lblName.TabIndex = 4;
            this.rank_lblName.Text = "Name:";
            this.rank_lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rank_lblColor
            // 
            this.rank_lblColor.AutoSize = true;
            this.rank_lblColor.Location = new System.Drawing.Point(44, 50);
            this.rank_lblColor.Name = "rank_lblColor";
            this.rank_lblColor.Size = new System.Drawing.Size(35, 13);
            this.rank_lblColor.TabIndex = 11;
            this.rank_lblColor.Text = "Color:";
            // 
            // rank_btnDel
            // 
            this.rank_btnDel.Location = new System.Drawing.Point(79, 326);
            this.rank_btnDel.Name = "rank_btnDel";
            this.rank_btnDel.Size = new System.Drawing.Size(57, 23);
            this.rank_btnDel.TabIndex = 2;
            this.rank_btnDel.Text = "Delete";
            this.rank_btnDel.UseVisualStyleBackColor = true;
            this.rank_btnDel.Click += new System.EventHandler(this.rank_btnDel_Click);
            // 
            // rank_btnAdd
            // 
            this.rank_btnAdd.Location = new System.Drawing.Point(6, 326);
            this.rank_btnAdd.Name = "rank_btnAdd";
            this.rank_btnAdd.Size = new System.Drawing.Size(57, 23);
            this.rank_btnAdd.TabIndex = 1;
            this.rank_btnAdd.Text = "Add";
            this.rank_btnAdd.UseVisualStyleBackColor = true;
            this.rank_btnAdd.Click += new System.EventHandler(this.rank_btnAdd_Click);
            // 
            // rank_list
            // 
            this.rank_list.FormattingEnabled = true;
            this.rank_list.Location = new System.Drawing.Point(6, 6);
            this.rank_list.Name = "rank_list";
            this.rank_list.Size = new System.Drawing.Size(130, 325);
            this.rank_list.TabIndex = 0;
            this.rank_list.SelectedIndexChanged += new System.EventHandler(this.rank_list_SelectedIndexChanged);
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
            this.pageMisc.Controls.Add(this.eco_grpEco);
            this.pageMisc.Controls.Add(this.grpExtra);
            this.pageMisc.Controls.Add(this.grpMessages);
            this.pageMisc.Controls.Add(this.grpPhysics);
            this.pageMisc.Controls.Add(this.afk_grp);
            this.pageMisc.Controls.Add(this.bak_grp);
            this.pageMisc.Controls.Add(this.chkProfanityFilter);
            this.pageMisc.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageMisc.Location = new System.Drawing.Point(4, 22);
            this.pageMisc.Name = "pageMisc";
            this.pageMisc.Size = new System.Drawing.Size(498, 521);
            this.pageMisc.TabIndex = 3;
            this.pageMisc.Text = "Misc";
            // 
            // eco_grpEco
            // 
            this.eco_grpEco.Controls.Add(this.eco_btnEco);
            this.eco_grpEco.Location = new System.Drawing.Point(352, 248);
            this.eco_grpEco.Name = "eco_grpEco";
            this.eco_grpEco.Size = new System.Drawing.Size(133, 144);
            this.eco_grpEco.TabIndex = 44;
            this.eco_grpEco.TabStop = false;
            this.eco_grpEco.Text = "Economy";
            // 
            // eco_btnEco
            // 
            this.eco_btnEco.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.eco_btnEco.Location = new System.Drawing.Point(8, 17);
            this.eco_btnEco.Name = "eco_btnEco";
            this.eco_btnEco.Size = new System.Drawing.Size(119, 23);
            this.eco_btnEco.TabIndex = 43;
            this.eco_btnEco.Text = "Economy Settings";
            this.eco_btnEco.UseVisualStyleBackColor = true;
            this.eco_btnEco.Click += new System.EventHandler(this.buttonEco_Click);
            // 
            // grpExtra
            // 
            this.grpExtra.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpExtra.Controls.Add(this.nudCooldownTime);
            this.grpExtra.Controls.Add(this.misc_lblReview);
            this.grpExtra.Controls.Add(this.chkGuestLimitNotify);
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
            // misc_lblReview
            // 
            this.misc_lblReview.AutoSize = true;
            this.misc_lblReview.Location = new System.Drawing.Point(23, 238);
            this.misc_lblReview.Name = "misc_lblReview";
            this.misc_lblReview.Size = new System.Drawing.Size(115, 13);
            this.misc_lblReview.TabIndex = 49;
            this.misc_lblReview.Text = "Review cooldown time:";
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
            // afk_grp
            // 
            this.afk_grp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.afk_grp.Controls.Add(this.afk_lblTimer);
            this.afk_grp.Controls.Add(this.afk_txtTimer);
            this.afk_grp.Location = new System.Drawing.Point(352, 13);
            this.afk_grp.Name = "afk_grp";
            this.afk_grp.Size = new System.Drawing.Size(133, 100);
            this.afk_grp.TabIndex = 37;
            this.afk_grp.TabStop = false;
            this.afk_grp.Text = "AFK";
            // 
            // afk_lblTimer
            // 
            this.afk_lblTimer.AutoSize = true;
            this.afk_lblTimer.Location = new System.Drawing.Point(5, 21);
            this.afk_lblTimer.Name = "afk_lblTimer";
            this.afk_lblTimer.Size = new System.Drawing.Size(54, 13);
            this.afk_lblTimer.TabIndex = 12;
            this.afk_lblTimer.Text = "AFK timer:";
            // 
            // bak_grp
            // 
            this.bak_grp.AutoSize = true;
            this.bak_grp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bak_grp.Controls.Add(this.bak_lblLocation);
            this.bak_grp.Controls.Add(this.bak_txtLocation);
            this.bak_grp.Controls.Add(this.bak_lblTime);
            this.bak_grp.Controls.Add(this.bak_numTime);
            this.bak_grp.Location = new System.Drawing.Point(10, 13);
            this.bak_grp.Name = "bak_grp";
            this.bak_grp.Size = new System.Drawing.Size(332, 84);
            this.bak_grp.TabIndex = 36;
            this.bak_grp.TabStop = false;
            this.bak_grp.Text = "Backups";
            // 
            // bak_lblLocation
            // 
            this.bak_lblLocation.AutoSize = true;
            this.bak_lblLocation.Location = new System.Drawing.Point(5, 21);
            this.bak_lblLocation.Name = "bak_lblLocation";
            this.bak_lblLocation.Size = new System.Drawing.Size(44, 13);
            this.bak_lblLocation.TabIndex = 3;
            this.bak_lblLocation.Text = "Backup:";
            // 
            // bak_txtLocation
            // 
            this.bak_txtLocation.Location = new System.Drawing.Point(81, 17);
            this.bak_txtLocation.Name = "bak_txtLocation";
            this.bak_txtLocation.Size = new System.Drawing.Size(245, 21);
            this.bak_txtLocation.TabIndex = 2;
            // 
            // bak_lblTime
            // 
            this.bak_lblTime.AutoSize = true;
            this.bak_lblTime.Location = new System.Drawing.Point(5, 47);
            this.bak_lblTime.Name = "bak_lblTime";
            this.bak_lblTime.Size = new System.Drawing.Size(67, 13);
            this.bak_lblTime.TabIndex = 7;
            this.bak_lblTime.Text = "Backup time:";
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
            this.pageIRC.Controls.Add(this.gb_ircSettings);
            this.pageIRC.Controls.Add(this.sql_grp);
            this.pageIRC.Controls.Add(this.irc_grp);
            this.pageIRC.Location = new System.Drawing.Point(4, 22);
            this.pageIRC.Name = "pageIRC";
            this.pageIRC.Size = new System.Drawing.Size(498, 521);
            this.pageIRC.TabIndex = 6;
            this.pageIRC.Text = "IRC/SQL";
            // 
            // gb_ircSettings
            // 
            this.gb_ircSettings.Controls.Add(this.irc_txtPrefix);
            this.gb_ircSettings.Controls.Add(this.irc_lblPrefix);
            this.gb_ircSettings.Controls.Add(this.irc_cbVerify);
            this.gb_ircSettings.Controls.Add(this.irc_lblVerify);
            this.gb_ircSettings.Controls.Add(this.irc_cbRank);
            this.gb_ircSettings.Controls.Add(this.irc_lblRank);
            this.gb_ircSettings.Controls.Add(this.irc_cbAFK);
            this.gb_ircSettings.Controls.Add(this.irc_cbWorldChanges);
            this.gb_ircSettings.Controls.Add(this.irc_cbTitles);
            this.gb_ircSettings.Location = new System.Drawing.Point(8, 223);
            this.gb_ircSettings.Name = "gb_ircSettings";
            this.gb_ircSettings.Size = new System.Drawing.Size(483, 95);
            this.gb_ircSettings.TabIndex = 33;
            this.gb_ircSettings.TabStop = false;
            this.gb_ircSettings.Text = "IRC settings";
            // 
            // irc_txtPrefix
            // 
            this.irc_txtPrefix.Location = new System.Drawing.Point(367, 68);
            this.irc_txtPrefix.Name = "irc_txtPrefix";
            this.irc_txtPrefix.Size = new System.Drawing.Size(100, 21);
            this.irc_txtPrefix.TabIndex = 32;
            // 
            // irc_lblPrefix
            // 
            this.irc_lblPrefix.AutoSize = true;
            this.irc_lblPrefix.Location = new System.Drawing.Point(265, 70);
            this.irc_lblPrefix.Name = "irc_lblPrefix";
            this.irc_lblPrefix.Size = new System.Drawing.Size(87, 13);
            this.irc_lblPrefix.TabIndex = 39;
            this.irc_lblPrefix.Text = "Command prefix:";
            // 
            // irc_cbVerify
            // 
            this.irc_cbVerify.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.irc_cbVerify.FormattingEnabled = true;
            this.irc_cbVerify.Location = new System.Drawing.Point(387, 42);
            this.irc_cbVerify.Name = "irc_cbVerify";
            this.irc_cbVerify.Size = new System.Drawing.Size(80, 21);
            this.irc_cbVerify.TabIndex = 38;
            // 
            // irc_lblVerify
            // 
            this.irc_lblVerify.AutoSize = true;
            this.irc_lblVerify.Location = new System.Drawing.Point(284, 45);
            this.irc_lblVerify.Name = "irc_lblVerify";
            this.irc_lblVerify.Size = new System.Drawing.Size(99, 13);
            this.irc_lblVerify.TabIndex = 37;
            this.irc_lblVerify.Text = "Verifcation method:";
            // 
            // irc_cbRank
            // 
            this.irc_cbRank.FormattingEnabled = true;
            this.irc_cbRank.Location = new System.Drawing.Point(367, 17);
            this.irc_cbRank.Name = "irc_cbRank";
            this.irc_cbRank.Size = new System.Drawing.Size(100, 21);
            this.irc_cbRank.TabIndex = 36;
            // 
            // irc_lblRank
            // 
            this.irc_lblRank.AutoSize = true;
            this.irc_lblRank.Location = new System.Drawing.Point(265, 21);
            this.irc_lblRank.Name = "irc_lblRank";
            this.irc_lblRank.Size = new System.Drawing.Size(97, 13);
            this.irc_lblRank.TabIndex = 35;
            this.irc_lblRank.Text = "IRC controller rank:";
            // 
            // irc_cbAFK
            // 
            this.irc_cbAFK.AutoSize = true;
            this.irc_cbAFK.Location = new System.Drawing.Point(6, 70);
            this.irc_cbAFK.Name = "irc_cbAFK";
            this.irc_cbAFK.Size = new System.Drawing.Size(176, 17);
            this.irc_cbAFK.TabIndex = 34;
            this.irc_cbAFK.Text = "Announce when player goes AFK";
            this.irc_cbAFK.UseVisualStyleBackColor = true;
            // 
            // irc_cbWorldChanges
            // 
            this.irc_cbWorldChanges.AutoSize = true;
            this.irc_cbWorldChanges.Location = new System.Drawing.Point(6, 45);
            this.irc_cbWorldChanges.Name = "irc_cbWorldChanges";
            this.irc_cbWorldChanges.Size = new System.Drawing.Size(199, 17);
            this.irc_cbWorldChanges.TabIndex = 33;
            this.irc_cbWorldChanges.Text = "Announce when player changes level";
            this.irc_cbWorldChanges.UseVisualStyleBackColor = true;
            // 
            // irc_cbTitles
            // 
            this.irc_cbTitles.AutoSize = true;
            this.irc_cbTitles.Location = new System.Drawing.Point(6, 20);
            this.irc_cbTitles.Name = "irc_cbTitles";
            this.irc_cbTitles.Size = new System.Drawing.Size(171, 17);
            this.irc_cbTitles.TabIndex = 32;
            this.irc_cbTitles.Text = "Show player\'s title in messages";
            this.irc_cbTitles.UseVisualStyleBackColor = true;
            // 
            // sql_grp
            // 
            this.sql_grp.Controls.Add(this.sql_chkUseSQL);
            this.sql_grp.Controls.Add(this.sql_linkDownload);
            this.sql_grp.Controls.Add(this.sql_lblUser);
            this.sql_grp.Controls.Add(this.sql_txtUser);
            this.sql_grp.Controls.Add(this.sql_lblPass);
            this.sql_grp.Controls.Add(this.sql_txtPass);
            this.sql_grp.Controls.Add(this.sql_lblDBName);
            this.sql_grp.Controls.Add(this.sql_txtDBName);
            this.sql_grp.Controls.Add(this.sql_lblHost);
            this.sql_grp.Controls.Add(this.sql_txtHost);
            this.sql_grp.Controls.Add(this.sql_lblPort);
            this.sql_grp.Controls.Add(this.sql_txtPort);
            this.sql_grp.Location = new System.Drawing.Point(264, 3);
            this.sql_grp.Name = "sql_grp";
            this.sql_grp.Size = new System.Drawing.Size(227, 214);
            this.sql_grp.TabIndex = 29;
            this.sql_grp.TabStop = false;
            this.sql_grp.Text = "MySQL";
            // 
            // sql_linkDownload
            // 
            this.sql_linkDownload.AutoSize = true;
            this.sql_linkDownload.Location = new System.Drawing.Point(108, 21);
            this.sql_linkDownload.Name = "sql_linkDownload";
            this.sql_linkDownload.Size = new System.Drawing.Size(113, 13);
            this.sql_linkDownload.TabIndex = 30;
            this.sql_linkDownload.TabStop = true;
            this.sql_linkDownload.Tag = "Click here to go to the download page for MySQL.";
            this.sql_linkDownload.Text = "MySQL Download Page";
            this.sql_linkDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.sql_linkDownload_LinkClicked);
            // 
            // sql_lblUser
            // 
            this.sql_lblUser.AutoSize = true;
            this.sql_lblUser.Location = new System.Drawing.Point(9, 50);
            this.sql_lblUser.Name = "sql_lblUser";
            this.sql_lblUser.Size = new System.Drawing.Size(59, 13);
            this.sql_lblUser.TabIndex = 3;
            this.sql_lblUser.Text = "Username:";
            // 
            // sql_txtUser
            // 
            this.sql_txtUser.Location = new System.Drawing.Point(111, 47);
            this.sql_txtUser.Name = "sql_txtUser";
            this.sql_txtUser.Size = new System.Drawing.Size(100, 21);
            this.sql_txtUser.TabIndex = 1;
            this.sql_txtUser.Tag = "The username set while installing MySQL";
            // 
            // sql_lblPass
            // 
            this.sql_lblPass.AutoSize = true;
            this.sql_lblPass.Location = new System.Drawing.Point(9, 77);
            this.sql_lblPass.Name = "sql_lblPass";
            this.sql_lblPass.Size = new System.Drawing.Size(56, 13);
            this.sql_lblPass.TabIndex = 4;
            this.sql_lblPass.Text = "Password:";
            // 
            // sql_txtPass
            // 
            this.sql_txtPass.Location = new System.Drawing.Point(111, 74);
            this.sql_txtPass.Name = "sql_txtPass";
            this.sql_txtPass.PasswordChar = '*';
            this.sql_txtPass.Size = new System.Drawing.Size(100, 21);
            this.sql_txtPass.TabIndex = 2;
            this.sql_txtPass.Tag = "The password set while installing MySQL";
            // 
            // sql_lblDBName
            // 
            this.sql_lblDBName.AutoSize = true;
            this.sql_lblDBName.Location = new System.Drawing.Point(9, 104);
            this.sql_lblDBName.Name = "sql_lblDBName";
            this.sql_lblDBName.Size = new System.Drawing.Size(86, 13);
            this.sql_lblDBName.TabIndex = 5;
            this.sql_lblDBName.Text = "Database Name:";
            // 
            // sql_txtDBName
            // 
            this.sql_txtDBName.Location = new System.Drawing.Point(111, 101);
            this.sql_txtDBName.Name = "sql_txtDBName";
            this.sql_txtDBName.Size = new System.Drawing.Size(100, 21);
            this.sql_txtDBName.TabIndex = 6;
            this.sql_txtDBName.Tag = "The name of the database stored (Default = MCZall)";
            // 
            // sql_lblHost
            // 
            this.sql_lblHost.AutoSize = true;
            this.sql_lblHost.Location = new System.Drawing.Point(9, 131);
            this.sql_lblHost.Name = "sql_lblHost";
            this.sql_lblHost.Size = new System.Drawing.Size(32, 13);
            this.sql_lblHost.TabIndex = 7;
            this.sql_lblHost.Text = "Host:";
            // 
            // sql_txtHost
            // 
            this.sql_txtHost.Location = new System.Drawing.Point(111, 128);
            this.sql_txtHost.Name = "sql_txtHost";
            this.sql_txtHost.Size = new System.Drawing.Size(100, 21);
            this.sql_txtHost.TabIndex = 8;
            this.sql_txtHost.Tag = "The host name for the database. Leave this unless problems occur.";
            // 
            // sql_lblPort
            // 
            this.sql_lblPort.AutoSize = true;
            this.sql_lblPort.Location = new System.Drawing.Point(9, 158);
            this.sql_lblPort.Name = "sql_lblPort";
            this.sql_lblPort.Size = new System.Drawing.Size(30, 13);
            this.sql_lblPort.TabIndex = 31;
            this.sql_lblPort.Text = "Port:";
            // 
            // sql_txtPort
            // 
            this.sql_txtPort.Location = new System.Drawing.Point(111, 155);
            this.sql_txtPort.Name = "sql_txtPort";
            this.sql_txtPort.Size = new System.Drawing.Size(100, 21);
            this.sql_txtPort.TabIndex = 32;
            // 
            // irc_grp
            // 
            this.irc_grp.Controls.Add(this.irc_chkEnabled);
            this.irc_grp.Controls.Add(this.irc_lblServer);
            this.irc_grp.Controls.Add(this.irc_txtServer);
            this.irc_grp.Controls.Add(this.irc_lblPort);
            this.irc_grp.Controls.Add(this.irc_txtPort);
            this.irc_grp.Controls.Add(this.irc_lblNick);
            this.irc_grp.Controls.Add(this.irc_txtNick);
            this.irc_grp.Controls.Add(this.irc_lblChannel);
            this.irc_grp.Controls.Add(this.irc_txtChannel);
            this.irc_grp.Controls.Add(this.irc_lblOpChannel);
            this.irc_grp.Controls.Add(this.irc_chkPass);
            this.irc_grp.Controls.Add(this.irc_txtPass);
            this.irc_grp.Controls.Add(this.irc_txtOpChannel);
            this.irc_grp.Location = new System.Drawing.Point(8, 3);
            this.irc_grp.Name = "irc_grp";
            this.irc_grp.Size = new System.Drawing.Size(250, 214);
            this.irc_grp.TabIndex = 27;
            this.irc_grp.TabStop = false;
            this.irc_grp.Text = "IRC";
            // 
            // irc_lblServer
            // 
            this.irc_lblServer.AutoSize = true;
            this.irc_lblServer.Location = new System.Drawing.Point(6, 50);
            this.irc_lblServer.Name = "irc_lblServer";
            this.irc_lblServer.Size = new System.Drawing.Size(40, 13);
            this.irc_lblServer.TabIndex = 19;
            this.irc_lblServer.Text = "Server:";
            // 
            // irc_lblPort
            // 
            this.irc_lblPort.AutoSize = true;
            this.irc_lblPort.Location = new System.Drawing.Point(6, 77);
            this.irc_lblPort.Name = "irc_lblPort";
            this.irc_lblPort.Size = new System.Drawing.Size(30, 13);
            this.irc_lblPort.TabIndex = 30;
            this.irc_lblPort.Text = "Port:";
            // 
            // irc_txtPort
            // 
            this.irc_txtPort.Location = new System.Drawing.Point(82, 74);
            this.irc_txtPort.Name = "irc_txtPort";
            this.irc_txtPort.Size = new System.Drawing.Size(63, 21);
            this.irc_txtPort.TabIndex = 31;
            // 
            // irc_lblNick
            // 
            this.irc_lblNick.AutoSize = true;
            this.irc_lblNick.Location = new System.Drawing.Point(6, 104);
            this.irc_lblNick.Name = "irc_lblNick";
            this.irc_lblNick.Size = new System.Drawing.Size(30, 13);
            this.irc_lblNick.TabIndex = 20;
            this.irc_lblNick.Text = "Nick:";
            // 
            // irc_lblChannel
            // 
            this.irc_lblChannel.AutoSize = true;
            this.irc_lblChannel.Location = new System.Drawing.Point(6, 131);
            this.irc_lblChannel.Name = "irc_lblChannel";
            this.irc_lblChannel.Size = new System.Drawing.Size(49, 13);
            this.irc_lblChannel.TabIndex = 18;
            this.irc_lblChannel.Text = "Channel:";
            // 
            // irc_lblOpChannel
            // 
            this.irc_lblOpChannel.AutoSize = true;
            this.irc_lblOpChannel.Location = new System.Drawing.Point(6, 158);
            this.irc_lblOpChannel.Name = "irc_lblOpChannel";
            this.irc_lblOpChannel.Size = new System.Drawing.Size(64, 13);
            this.irc_lblOpChannel.TabIndex = 25;
            this.irc_lblOpChannel.Text = "Op Channel:";
            // 
            // pageServer
            // 
            this.pageServer.BackColor = System.Drawing.SystemColors.Control;
            this.pageServer.Controls.Add(this.lvl_grp);
            this.pageServer.Controls.Add(this.adv_grp);
            this.pageServer.Controls.Add(this.srv_grp);
            this.pageServer.Controls.Add(this.srv_grpUpdate);
            this.pageServer.Controls.Add(this.grpPlayers);
            this.pageServer.Location = new System.Drawing.Point(4, 22);
            this.pageServer.Name = "pageServer";
            this.pageServer.Padding = new System.Windows.Forms.Padding(3);
            this.pageServer.Size = new System.Drawing.Size(498, 521);
            this.pageServer.TabIndex = 0;
            this.pageServer.Text = "Server";
            // 
            // lvl_grp
            // 
            this.lvl_grp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.lvl_grp.Controls.Add(this.lvl_lblMain);
            this.lvl_grp.Controls.Add(this.lvl_txtMain);
            this.lvl_grp.Controls.Add(this.lvl_chkAutoload);
            this.lvl_grp.Controls.Add(this.lvl_chkWorld);
            this.lvl_grp.Location = new System.Drawing.Point(314, 160);
            this.lvl_grp.Name = "lvl_grp";
            this.lvl_grp.Size = new System.Drawing.Size(177, 105);
            this.lvl_grp.TabIndex = 44;
            this.lvl_grp.TabStop = false;
            this.lvl_grp.Text = "Level Settings";
            // 
            // lvl_lblMain
            // 
            this.lvl_lblMain.AutoSize = true;
            this.lvl_lblMain.Location = new System.Drawing.Point(6, 22);
            this.lvl_lblMain.Name = "lvl_lblMain";
            this.lvl_lblMain.Size = new System.Drawing.Size(63, 13);
            this.lvl_lblMain.TabIndex = 3;
            this.lvl_lblMain.Text = "Main name:";
            // 
            // lvl_txtMain
            // 
            this.lvl_txtMain.Location = new System.Drawing.Point(75, 19);
            this.lvl_txtMain.Name = "lvl_txtMain";
            this.lvl_txtMain.Size = new System.Drawing.Size(87, 21);
            this.lvl_txtMain.TabIndex = 2;
            // 
            // adv_grp
            // 
            this.adv_grp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.adv_grp.Controls.Add(this.adv_chkVerify);
            this.adv_grp.Controls.Add(this.adv_chkRestart);
            this.adv_grp.Controls.Add(this.adv_btnEditTexts);
            this.adv_grp.Location = new System.Drawing.Point(8, 271);
            this.adv_grp.Name = "adv_grp";
            this.adv_grp.Size = new System.Drawing.Size(206, 98);
            this.adv_grp.TabIndex = 42;
            this.adv_grp.TabStop = false;
            this.adv_grp.Text = "Advanced Configuration";
            // 
            // adv_chkRestart
            // 
            this.adv_chkRestart.AutoSize = true;
            this.adv_chkRestart.Location = new System.Drawing.Point(9, 43);
            this.adv_chkRestart.Name = "adv_chkRestart";
            this.adv_chkRestart.Size = new System.Drawing.Size(101, 17);
            this.adv_chkRestart.TabIndex = 4;
            this.adv_chkRestart.Text = "Restart on error";
            this.adv_chkRestart.UseVisualStyleBackColor = true;
            // 
            // adv_btnEditTexts
            // 
            this.adv_btnEditTexts.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.adv_btnEditTexts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.adv_btnEditTexts.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.adv_btnEditTexts.Location = new System.Drawing.Point(6, 68);
            this.adv_btnEditTexts.Name = "adv_btnEditTexts";
            this.adv_btnEditTexts.Size = new System.Drawing.Size(80, 23);
            this.adv_btnEditTexts.TabIndex = 35;
            this.adv_btnEditTexts.Text = "Edit Text Files";
            this.adv_btnEditTexts.UseVisualStyleBackColor = true;
            this.adv_btnEditTexts.Click += new System.EventHandler(this.adv_btnEditTexts_Click);
            // 
            // srv_grp
            // 
            this.srv_grp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.srv_grp.Controls.Add(this.srv_lblName);
            this.srv_grp.Controls.Add(this.srv_txtName);
            this.srv_grp.Controls.Add(this.srv_lblMotd);
            this.srv_grp.Controls.Add(this.srv_txtMOTD);
            this.srv_grp.Controls.Add(this.srv_lblPort);
            this.srv_grp.Controls.Add(this.srv_numPort);
            this.srv_grp.Controls.Add(this.srv_btnPort);
            this.srv_grp.Controls.Add(this.srv_lblOwner);
            this.srv_grp.Controls.Add(this.srv_txtOwner);
            this.srv_grp.Controls.Add(this.srv_chkPublic);
            this.srv_grp.Location = new System.Drawing.Point(8, 6);
            this.srv_grp.Name = "srv_grp";
            this.srv_grp.Size = new System.Drawing.Size(483, 148);
            this.srv_grp.TabIndex = 41;
            this.srv_grp.TabStop = false;
            this.srv_grp.Text = "General Configuration";
            // 
            // srv_lblName
            // 
            this.srv_lblName.AutoSize = true;
            this.srv_lblName.Location = new System.Drawing.Point(6, 22);
            this.srv_lblName.Name = "srv_lblName";
            this.srv_lblName.Size = new System.Drawing.Size(38, 13);
            this.srv_lblName.TabIndex = 100;
            this.srv_lblName.Text = "Name:";
            // 
            // srv_lblMotd
            // 
            this.srv_lblMotd.AutoSize = true;
            this.srv_lblMotd.Location = new System.Drawing.Point(6, 49);
            this.srv_lblMotd.Name = "srv_lblMotd";
            this.srv_lblMotd.Size = new System.Drawing.Size(38, 13);
            this.srv_lblMotd.TabIndex = 101;
            this.srv_lblMotd.Text = "MOTD:";
            // 
            // srv_lblPort
            // 
            this.srv_lblPort.AutoSize = true;
            this.srv_lblPort.Location = new System.Drawing.Point(6, 76);
            this.srv_lblPort.Name = "srv_lblPort";
            this.srv_lblPort.Size = new System.Drawing.Size(30, 13);
            this.srv_lblPort.TabIndex = 102;
            this.srv_lblPort.Text = "Port:";
            // 
            // srv_btnPort
            // 
            this.srv_btnPort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.srv_btnPort.Location = new System.Drawing.Point(152, 71);
            this.srv_btnPort.Name = "srv_btnPort";
            this.srv_btnPort.Size = new System.Drawing.Size(110, 23);
            this.srv_btnPort.TabIndex = 3;
            this.srv_btnPort.Text = "Server Port Utilities";
            this.srv_btnPort.UseVisualStyleBackColor = true;
            this.srv_btnPort.Click += new System.EventHandler(this.ChkPort_Click);
            // 
            // srv_lblOwner
            // 
            this.srv_lblOwner.AutoSize = true;
            this.srv_lblOwner.Location = new System.Drawing.Point(6, 103);
            this.srv_lblOwner.Name = "srv_lblOwner";
            this.srv_lblOwner.Size = new System.Drawing.Size(72, 13);
            this.srv_lblOwner.TabIndex = 104;
            this.srv_lblOwner.Text = "Server owner:";
            // 
            // srv_txtOwner
            // 
            this.srv_txtOwner.Location = new System.Drawing.Point(83, 100);
            this.srv_txtOwner.MaxLength = 64;
            this.srv_txtOwner.Name = "srv_txtOwner";
            this.srv_txtOwner.Size = new System.Drawing.Size(119, 21);
            this.srv_txtOwner.TabIndex = 4;
            // 
            // srv_grpUpdate
            // 
            this.srv_grpUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.srv_grpUpdate.Controls.Add(this.srv_btnForceUpdate);
            this.srv_grpUpdate.Controls.Add(this.chkUpdates);
            this.srv_grpUpdate.Location = new System.Drawing.Point(220, 271);
            this.srv_grpUpdate.Name = "srv_grpUpdate";
            this.srv_grpUpdate.Size = new System.Drawing.Size(271, 98);
            this.srv_grpUpdate.TabIndex = 44;
            this.srv_grpUpdate.TabStop = false;
            this.srv_grpUpdate.Text = "Update Settings";
            // 
            // srv_btnForceUpdate
            // 
            this.srv_btnForceUpdate.AutoSize = true;
            this.srv_btnForceUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.srv_btnForceUpdate.Location = new System.Drawing.Point(186, 16);
            this.srv_btnForceUpdate.Name = "srv_btnForceUpdate";
            this.srv_btnForceUpdate.Size = new System.Drawing.Size(79, 23);
            this.srv_btnForceUpdate.TabIndex = 6;
            this.srv_btnForceUpdate.Text = "Force update";
            this.srv_btnForceUpdate.UseVisualStyleBackColor = true;
            this.srv_btnForceUpdate.Click += new System.EventHandler(this.forceUpdateBtn_Click);
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
            this.grpPlayers.Controls.Add(this.srv_lblPlayers);
            this.grpPlayers.Controls.Add(this.srv_numPlayers);
            this.grpPlayers.Controls.Add(this.srv_cbMustAgree);
            this.grpPlayers.Controls.Add(this.srv_lblGuests);
            this.grpPlayers.Controls.Add(this.srv_numGuests);
            this.grpPlayers.Location = new System.Drawing.Point(8, 160);
            this.grpPlayers.Name = "grpPlayers";
            this.grpPlayers.Size = new System.Drawing.Size(300, 105);
            this.grpPlayers.TabIndex = 46;
            this.grpPlayers.TabStop = false;
            this.grpPlayers.Text = "Players";
            // 
            // srv_lblPlayers
            // 
            this.srv_lblPlayers.AutoSize = true;
            this.srv_lblPlayers.Location = new System.Drawing.Point(6, 22);
            this.srv_lblPlayers.Name = "srv_lblPlayers";
            this.srv_lblPlayers.Size = new System.Drawing.Size(67, 13);
            this.srv_lblPlayers.TabIndex = 3;
            this.srv_lblPlayers.Text = "Max Players:";
            // 
            // srv_numPlayers
            // 
            this.srv_numPlayers.Location = new System.Drawing.Point(83, 20);
            this.srv_numPlayers.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.srv_numPlayers.Name = "srv_numPlayers";
            this.srv_numPlayers.Size = new System.Drawing.Size(60, 21);
            this.srv_numPlayers.TabIndex = 29;
            this.srv_numPlayers.Value = new decimal(new int[] {
                                    12,
                                    0,
                                    0,
                                    0});
            this.srv_numPlayers.ValueChanged += new System.EventHandler(this.numPlayers_ValueChanged);
            // 
            // srv_cbMustAgree
            // 
            this.srv_cbMustAgree.AutoSize = true;
            this.srv_cbMustAgree.Location = new System.Drawing.Point(9, 74);
            this.srv_cbMustAgree.Name = "srv_cbMustAgree";
            this.srv_cbMustAgree.Size = new System.Drawing.Size(169, 17);
            this.srv_cbMustAgree.TabIndex = 32;
            this.srv_cbMustAgree.Tag = "Forces guests to use /agree on entry to the server";
            this.srv_cbMustAgree.Text = "Force new guests to read rules";
            this.srv_cbMustAgree.UseVisualStyleBackColor = true;
            // 
            // srv_lblGuests
            // 
            this.srv_lblGuests.AutoSize = true;
            this.srv_lblGuests.Location = new System.Drawing.Point(6, 49);
            this.srv_lblGuests.Name = "srv_lblGuests";
            this.srv_lblGuests.Size = new System.Drawing.Size(65, 13);
            this.srv_lblGuests.TabIndex = 27;
            this.srv_lblGuests.Text = "Max Guests:";
            // 
            // srv_numGuests
            // 
            this.srv_numGuests.Location = new System.Drawing.Point(83, 47);
            this.srv_numGuests.Maximum = new decimal(new int[] {
                                    128,
                                    0,
                                    0,
                                    0});
            this.srv_numGuests.Name = "srv_numGuests";
            this.srv_numGuests.Size = new System.Drawing.Size(60, 21);
            this.srv_numGuests.TabIndex = 28;
            this.srv_numGuests.Value = new decimal(new int[] {
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
            this.tabControl.Controls.Add(this.pageChat);
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
            this.tabGames.Controls.Add(this.tabTntWars);
            this.tabGames.Controls.Add(this.tabZS);
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
            this.tabPage10.Controls.Add(this.ls_grpMapSettings);
            this.tabPage10.Controls.Add(this.groupBox21);
            this.tabPage10.Controls.Add(this.ls_grpMaps);
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
            this.groupBox23.Controls.Add(this.ls_btnEndRound);
            this.groupBox23.Controls.Add(this.ls_btnStopGame);
            this.groupBox23.Controls.Add(this.ls_btnStartGame);
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
            // ls_btnEndRound
            // 
            this.ls_btnEndRound.Location = new System.Drawing.Point(190, 20);
            this.ls_btnEndRound.Name = "ls_btnEndRound";
            this.ls_btnEndRound.Size = new System.Drawing.Size(80, 23);
            this.ls_btnEndRound.TabIndex = 2;
            this.ls_btnEndRound.Text = "End Round";
            this.ls_btnEndRound.UseVisualStyleBackColor = true;
            this.ls_btnEndRound.Click += new System.EventHandler(this.lsBtnEndRound_Click);
            // 
            // ls_btnStopGame
            // 
            this.ls_btnStopGame.Location = new System.Drawing.Point(100, 20);
            this.ls_btnStopGame.Name = "ls_btnStopGame";
            this.ls_btnStopGame.Size = new System.Drawing.Size(80, 23);
            this.ls_btnStopGame.TabIndex = 1;
            this.ls_btnStopGame.Text = "Stop Game";
            this.ls_btnStopGame.UseVisualStyleBackColor = true;
            this.ls_btnStopGame.Click += new System.EventHandler(this.lsBtnStopGame_Click);
            // 
            // ls_btnStartGame
            // 
            this.ls_btnStartGame.Location = new System.Drawing.Point(10, 20);
            this.ls_btnStartGame.Name = "ls_btnStartGame";
            this.ls_btnStartGame.Size = new System.Drawing.Size(80, 23);
            this.ls_btnStartGame.TabIndex = 0;
            this.ls_btnStartGame.Text = "Start Game";
            this.ls_btnStartGame.UseVisualStyleBackColor = true;
            this.ls_btnStartGame.Click += new System.EventHandler(this.lsBtnStartGame_Click);
            // 
            // ls_grpMapSettings
            // 
            this.ls_grpMapSettings.Controls.Add(this.pg_lavaMap);
            this.ls_grpMapSettings.Location = new System.Drawing.Point(193, 174);
            this.ls_grpMapSettings.Name = "ls_grpMapSettings";
            this.ls_grpMapSettings.Size = new System.Drawing.Size(285, 219);
            this.ls_grpMapSettings.TabIndex = 3;
            this.ls_grpMapSettings.TabStop = false;
            this.ls_grpMapSettings.Text = "Map Settings";
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
            // ls_grpMaps
            // 
            this.ls_grpMaps.Controls.Add(this.ls_lblNotUsed);
            this.ls_grpMaps.Controls.Add(this.ls_lblUsed);
            this.ls_grpMaps.Controls.Add(this.ls_btnAdd);
            this.ls_grpMaps.Controls.Add(this.ls_btnRemove);
            this.ls_grpMaps.Controls.Add(this.ls_lstNotUsed);
            this.ls_grpMaps.Controls.Add(this.ls_lstUsed);
            this.ls_grpMaps.Location = new System.Drawing.Point(6, 6);
            this.ls_grpMaps.Name = "ls_grpMaps";
            this.ls_grpMaps.Size = new System.Drawing.Size(181, 387);
            this.ls_grpMaps.TabIndex = 1;
            this.ls_grpMaps.TabStop = false;
            this.ls_grpMaps.Text = "Maps";
            // 
            // ls_lblNotUsed
            // 
            this.ls_lblNotUsed.AutoSize = true;
            this.ls_lblNotUsed.Location = new System.Drawing.Point(187, 17);
            this.ls_lblNotUsed.Name = "ls_lblNotUsed";
            this.ls_lblNotUsed.Size = new System.Drawing.Size(83, 13);
            this.ls_lblNotUsed.TabIndex = 6;
            this.ls_lblNotUsed.Text = "Maps Not In Use";
            // 
            // ls_lblUsed
            // 
            this.ls_lblUsed.AutoSize = true;
            this.ls_lblUsed.Location = new System.Drawing.Point(6, 17);
            this.ls_lblUsed.Name = "ls_lblUsed";
            this.ls_lblUsed.Size = new System.Drawing.Size(38, 13);
            this.ls_lblUsed.TabIndex = 5;
            this.ls_lblUsed.Text = "In use:";
            // 
            // ls_btnAdd
            // 
            this.ls_btnAdd.Location = new System.Drawing.Point(6, 188);
            this.ls_btnAdd.Name = "ls_btnAdd";
            this.ls_btnAdd.Size = new System.Drawing.Size(77, 23);
            this.ls_btnAdd.TabIndex = 4;
            this.ls_btnAdd.Text = "<< Add";
            this.ls_btnAdd.UseVisualStyleBackColor = true;
            this.ls_btnAdd.Click += new System.EventHandler(this.lsAddMap_Click);
            // 
            // ls_btnRemove
            // 
            this.ls_btnRemove.Location = new System.Drawing.Point(100, 188);
            this.ls_btnRemove.Name = "ls_btnRemove";
            this.ls_btnRemove.Size = new System.Drawing.Size(75, 23);
            this.ls_btnRemove.TabIndex = 3;
            this.ls_btnRemove.Text = "Remove >>";
            this.ls_btnRemove.UseVisualStyleBackColor = true;
            this.ls_btnRemove.Click += new System.EventHandler(this.lsRemoveMap_Click);
            // 
            // ls_lstNotUsed
            // 
            this.ls_lstNotUsed.FormattingEnabled = true;
            this.ls_lstNotUsed.Location = new System.Drawing.Point(6, 219);
            this.ls_lstNotUsed.Name = "ls_lstNotUsed";
            this.ls_lstNotUsed.Size = new System.Drawing.Size(169, 160);
            this.ls_lstNotUsed.TabIndex = 2;
            // 
            // ls_lstUsed
            // 
            this.ls_lstUsed.FormattingEnabled = true;
            this.ls_lstUsed.Location = new System.Drawing.Point(6, 33);
            this.ls_lstUsed.Name = "ls_lstUsed";
            this.ls_lstUsed.Size = new System.Drawing.Size(169, 147);
            this.ls_lstUsed.TabIndex = 0;
            this.ls_lstUsed.SelectedIndexChanged += new System.EventHandler(this.lsMapUse_SelectedIndexChanged);
            // 
            // tabTntWars
            // 
            this.tabTntWars.BackColor = System.Drawing.SystemColors.Control;
            this.tabTntWars.Controls.Add(this.tw_txtPlayers);
            this.tabTntWars.Controls.Add(this.tw_lblPlayers);
            this.tabTntWars.Controls.Add(this.tw_txtStatus);
            this.tabTntWars.Controls.Add(this.tw_lblStatus);
            this.tabTntWars.Controls.Add(this.label85);
            this.tabTntWars.Controls.Add(this.SlctdTntWrsLvl);
            this.tabTntWars.Controls.Add(this.groupBox29);
            this.tabTntWars.Controls.Add(this.tw_btnEditGame);
            this.tabTntWars.Controls.Add(this.tw_lstGames);
            this.tabTntWars.Location = new System.Drawing.Point(4, 22);
            this.tabTntWars.Name = "tabTntWars";
            this.tabTntWars.Padding = new System.Windows.Forms.Padding(3);
            this.tabTntWars.Size = new System.Drawing.Size(484, 489);
            this.tabTntWars.TabIndex = 2;
            this.tabTntWars.Text = "TNT Wars";
            // 
            // tw_txtPlayers
            // 
            this.tw_txtPlayers.Location = new System.Drawing.Point(426, 6);
            this.tw_txtPlayers.Name = "tw_txtPlayers";
            this.tw_txtPlayers.ReadOnly = true;
            this.tw_txtPlayers.Size = new System.Drawing.Size(36, 21);
            this.tw_txtPlayers.TabIndex = 9;
            // 
            // tw_lblPlayers
            // 
            this.tw_lblPlayers.AutoSize = true;
            this.tw_lblPlayers.Location = new System.Drawing.Point(375, 9);
            this.tw_lblPlayers.Name = "tw_lblPlayers";
            this.tw_lblPlayers.Size = new System.Drawing.Size(45, 13);
            this.tw_lblPlayers.TabIndex = 8;
            this.tw_lblPlayers.Text = "Players:";
            // 
            // tw_txtStatus
            // 
            this.tw_txtStatus.Location = new System.Drawing.Point(234, 6);
            this.tw_txtStatus.Name = "tw_txtStatus";
            this.tw_txtStatus.ReadOnly = true;
            this.tw_txtStatus.Size = new System.Drawing.Size(135, 21);
            this.tw_txtStatus.TabIndex = 7;
            // 
            // tw_lblStatus
            // 
            this.tw_lblStatus.AutoSize = true;
            this.tw_lblStatus.Location = new System.Drawing.Point(188, 9);
            this.tw_lblStatus.Name = "tw_lblStatus";
            this.tw_lblStatus.Size = new System.Drawing.Size(40, 13);
            this.tw_lblStatus.TabIndex = 6;
            this.tw_lblStatus.Text = "Status:";
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
            this.groupBox29.Controls.Add(this.tw_grpScores);
            this.groupBox29.Controls.Add(this.tw_grpStatus);
            this.groupBox29.Location = new System.Drawing.Point(9, 33);
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.Size = new System.Drawing.Size(453, 268);
            this.groupBox29.TabIndex = 3;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Edit Selected Game";
            // 
            // groupBox36
            // 
            this.groupBox36.Controls.Add(this.tw_cbStreaks);
            this.groupBox36.Location = new System.Drawing.Point(355, 20);
            this.groupBox36.Name = "groupBox36";
            this.groupBox36.Size = new System.Drawing.Size(92, 103);
            this.groupBox36.TabIndex = 10;
            this.groupBox36.TabStop = false;
            this.groupBox36.Text = "Other:";
            // 
            // tw_cbStreaks
            // 
            this.tw_cbStreaks.AutoSize = true;
            this.tw_cbStreaks.Checked = true;
            this.tw_cbStreaks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tw_cbStreaks.Location = new System.Drawing.Point(7, 21);
            this.tw_cbStreaks.Name = "tw_cbStreaks";
            this.tw_cbStreaks.Size = new System.Drawing.Size(61, 17);
            this.tw_cbStreaks.TabIndex = 0;
            this.tw_cbStreaks.Text = "Streaks";
            this.tw_cbStreaks.UseVisualStyleBackColor = true;
            this.tw_cbStreaks.CheckedChanged += new System.EventHandler(this.TntWrsStreaksChck_CheckedChanged);
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
            this.groupBox33.Controls.Add(this.tw_cbTeamKills);
            this.groupBox33.Controls.Add(this.tw_cbBalanceTeams);
            this.groupBox33.Controls.Add(this.TntWrsTmsChck);
            this.groupBox33.Location = new System.Drawing.Point(196, 175);
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.Size = new System.Drawing.Size(152, 87);
            this.groupBox33.TabIndex = 7;
            this.groupBox33.TabStop = false;
            this.groupBox33.Text = "Teams:";
            // 
            // tw_cbTeamKills
            // 
            this.tw_cbTeamKills.AutoSize = true;
            this.tw_cbTeamKills.Location = new System.Drawing.Point(7, 68);
            this.tw_cbTeamKills.Name = "tw_cbTeamKills";
            this.tw_cbTeamKills.Size = new System.Drawing.Size(73, 17);
            this.tw_cbTeamKills.TabIndex = 2;
            this.tw_cbTeamKills.Text = "Team Kills";
            this.tw_cbTeamKills.UseVisualStyleBackColor = true;
            this.tw_cbTeamKills.CheckedChanged += new System.EventHandler(this.TntWrsTKchck_CheckedChanged);
            // 
            // tw_cbBalanceTeams
            // 
            this.tw_cbBalanceTeams.AutoSize = true;
            this.tw_cbBalanceTeams.Checked = true;
            this.tw_cbBalanceTeams.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tw_cbBalanceTeams.Location = new System.Drawing.Point(7, 44);
            this.tw_cbBalanceTeams.Name = "tw_cbBalanceTeams";
            this.tw_cbBalanceTeams.Size = new System.Drawing.Size(96, 17);
            this.tw_cbBalanceTeams.TabIndex = 1;
            this.tw_cbBalanceTeams.Text = "Balance Teams";
            this.tw_cbBalanceTeams.UseVisualStyleBackColor = true;
            this.tw_cbBalanceTeams.CheckedChanged += new System.EventHandler(this.TntWrsBlnceTeamsChck_CheckedChanged);
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
            // tw_grpScores
            // 
            this.tw_grpScores.Controls.Add(this.tw_cbMultiKills);
            this.tw_grpScores.Controls.Add(this.tw_numMultiKills);
            this.tw_grpScores.Controls.Add(this.tw_cbScoreAssists);
            this.tw_grpScores.Controls.Add(this.tw_numScoreAssists);
            this.tw_grpScores.Controls.Add(this.tw_lblScorePerKill);
            this.tw_grpScores.Controls.Add(this.tw_numScorePerKill);
            this.tw_grpScores.Controls.Add(this.tw_lblScoreLimit);
            this.tw_grpScores.Controls.Add(this.tw_numScoreLimit);
            this.tw_grpScores.Location = new System.Drawing.Point(6, 129);
            this.tw_grpScores.Name = "tw_grpScores";
            this.tw_grpScores.Size = new System.Drawing.Size(184, 133);
            this.tw_grpScores.TabIndex = 5;
            this.tw_grpScores.TabStop = false;
            this.tw_grpScores.Text = "Scores";
            // 
            // tw_cbMultiKills
            // 
            this.tw_cbMultiKills.AutoSize = true;
            this.tw_cbMultiKills.Checked = true;
            this.tw_cbMultiKills.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tw_cbMultiKills.Location = new System.Drawing.Point(11, 102);
            this.tw_cbMultiKills.Name = "tw_cbMultiKills";
            this.tw_cbMultiKills.Size = new System.Drawing.Size(122, 17);
            this.tw_cbMultiKills.TabIndex = 8;
            this.tw_cbMultiKills.Text = "MultiKills (Score Per:";
            this.tw_cbMultiKills.UseVisualStyleBackColor = true;
            this.tw_cbMultiKills.CheckedChanged += new System.EventHandler(this.TntWrsMltiKlChck_CheckedChanged);
            // 
            // tw_numMultiKills
            // 
            this.tw_numMultiKills.Location = new System.Drawing.Point(140, 101);
            this.tw_numMultiKills.Maximum = new decimal(new int[] {
                                    10000,
                                    0,
                                    0,
                                    0});
            this.tw_numMultiKills.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.tw_numMultiKills.Name = "tw_numMultiKills";
            this.tw_numMultiKills.Size = new System.Drawing.Size(38, 21);
            this.tw_numMultiKills.TabIndex = 7;
            this.tw_numMultiKills.Value = new decimal(new int[] {
                                    5,
                                    0,
                                    0,
                                    0});
            this.tw_numMultiKills.ValueChanged += new System.EventHandler(this.TntWrsMltiKlScPrUpDown_ValueChanged);
            // 
            // tw_cbScoreAssists
            // 
            this.tw_cbScoreAssists.AutoSize = true;
            this.tw_cbScoreAssists.Checked = true;
            this.tw_cbScoreAssists.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tw_cbScoreAssists.Location = new System.Drawing.Point(23, 75);
            this.tw_cbScoreAssists.Name = "tw_cbScoreAssists";
            this.tw_cbScoreAssists.Size = new System.Drawing.Size(111, 17);
            this.tw_cbScoreAssists.TabIndex = 6;
            this.tw_cbScoreAssists.Text = "Assists (Score Per:";
            this.tw_cbScoreAssists.UseVisualStyleBackColor = true;
            this.tw_cbScoreAssists.CheckedChanged += new System.EventHandler(this.TntWrsAsstChck_CheckedChanged);
            // 
            // tw_numScoreAssists
            // 
            this.tw_numScoreAssists.Location = new System.Drawing.Point(140, 74);
            this.tw_numScoreAssists.Maximum = new decimal(new int[] {
                                    10000,
                                    0,
                                    0,
                                    0});
            this.tw_numScoreAssists.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.tw_numScoreAssists.Name = "tw_numScoreAssists";
            this.tw_numScoreAssists.Size = new System.Drawing.Size(38, 21);
            this.tw_numScoreAssists.TabIndex = 4;
            this.tw_numScoreAssists.Value = new decimal(new int[] {
                                    5,
                                    0,
                                    0,
                                    0});
            this.tw_numScoreAssists.ValueChanged += new System.EventHandler(this.TntWrsAstsScrUpDwn_ValueChanged);
            // 
            // tw_lblScorePerKill
            // 
            this.tw_lblScorePerKill.AutoSize = true;
            this.tw_lblScorePerKill.Location = new System.Drawing.Point(63, 49);
            this.tw_lblScorePerKill.Name = "tw_lblScorePerKill";
            this.tw_lblScorePerKill.Size = new System.Drawing.Size(71, 13);
            this.tw_lblScorePerKill.TabIndex = 3;
            this.tw_lblScorePerKill.Text = "Score Per Kill:";
            // 
            // tw_numScorePerKill
            // 
            this.tw_numScorePerKill.Location = new System.Drawing.Point(140, 47);
            this.tw_numScorePerKill.Maximum = new decimal(new int[] {
                                    10000,
                                    0,
                                    0,
                                    0});
            this.tw_numScorePerKill.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.tw_numScorePerKill.Name = "tw_numScorePerKill";
            this.tw_numScorePerKill.Size = new System.Drawing.Size(38, 21);
            this.tw_numScorePerKill.TabIndex = 2;
            this.tw_numScorePerKill.Value = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            this.tw_numScorePerKill.ValueChanged += new System.EventHandler(this.TntWrsScrPrKlUpDwn_ValueChanged);
            // 
            // tw_lblScoreLimit
            // 
            this.tw_lblScoreLimit.AutoSize = true;
            this.tw_lblScoreLimit.Location = new System.Drawing.Point(72, 22);
            this.tw_lblScoreLimit.Name = "tw_lblScoreLimit";
            this.tw_lblScoreLimit.Size = new System.Drawing.Size(62, 13);
            this.tw_lblScoreLimit.TabIndex = 1;
            this.tw_lblScoreLimit.Text = "Score Limit:";
            // 
            // tw_numScoreLimit
            // 
            this.tw_numScoreLimit.Location = new System.Drawing.Point(140, 20);
            this.tw_numScoreLimit.Maximum = new decimal(new int[] {
                                    10000000,
                                    0,
                                    0,
                                    0});
            this.tw_numScoreLimit.Minimum = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            this.tw_numScoreLimit.Name = "tw_numScoreLimit";
            this.tw_numScoreLimit.Size = new System.Drawing.Size(38, 21);
            this.tw_numScoreLimit.TabIndex = 0;
            this.tw_numScoreLimit.Value = new decimal(new int[] {
                                    150,
                                    0,
                                    0,
                                    0});
            this.tw_numScoreLimit.ValueChanged += new System.EventHandler(this.TntWrsScrLmtUpDwn_ValueChanged);
            // 
            // tw_grpStatus
            // 
            this.tw_grpStatus.Controls.Add(this.tw_btnStartGame);
            this.tw_grpStatus.Controls.Add(this.tw_btnDeleteGame);
            this.tw_grpStatus.Controls.Add(this.tw_btnEndGame);
            this.tw_grpStatus.Controls.Add(this.tw_btnResetGame);
            this.tw_grpStatus.Location = new System.Drawing.Point(354, 129);
            this.tw_grpStatus.Name = "tw_grpStatus";
            this.tw_grpStatus.Size = new System.Drawing.Size(93, 133);
            this.tw_grpStatus.TabIndex = 4;
            this.tw_grpStatus.TabStop = false;
            this.tw_grpStatus.Text = "Status";
            // 
            // tw_btnStartGame
            // 
            this.tw_btnStartGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tw_btnStartGame.Location = new System.Drawing.Point(6, 17);
            this.tw_btnStartGame.Name = "tw_btnStartGame";
            this.tw_btnStartGame.Size = new System.Drawing.Size(80, 23);
            this.tw_btnStartGame.TabIndex = 0;
            this.tw_btnStartGame.Text = "Start Game";
            this.tw_btnStartGame.UseVisualStyleBackColor = true;
            this.tw_btnStartGame.Click += new System.EventHandler(this.TntWrsStrtGame_Click);
            // 
            // tw_btnDeleteGame
            // 
            this.tw_btnDeleteGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tw_btnDeleteGame.Location = new System.Drawing.Point(6, 104);
            this.tw_btnDeleteGame.Name = "tw_btnDeleteGame";
            this.tw_btnDeleteGame.Size = new System.Drawing.Size(80, 23);
            this.tw_btnDeleteGame.TabIndex = 3;
            this.tw_btnDeleteGame.Text = "Delete Game";
            this.tw_btnDeleteGame.UseVisualStyleBackColor = true;
            this.tw_btnDeleteGame.Click += new System.EventHandler(this.TntWrsDltGame_Click);
            // 
            // tw_btnEndGame
            // 
            this.tw_btnEndGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tw_btnEndGame.Location = new System.Drawing.Point(6, 46);
            this.tw_btnEndGame.Name = "tw_btnEndGame";
            this.tw_btnEndGame.Size = new System.Drawing.Size(80, 23);
            this.tw_btnEndGame.TabIndex = 1;
            this.tw_btnEndGame.Text = "End Game";
            this.tw_btnEndGame.UseVisualStyleBackColor = true;
            this.tw_btnEndGame.Click += new System.EventHandler(this.TntWrsEndGame_Click);
            // 
            // tw_btnResetGame
            // 
            this.tw_btnResetGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tw_btnResetGame.Location = new System.Drawing.Point(6, 75);
            this.tw_btnResetGame.Name = "tw_btnResetGame";
            this.tw_btnResetGame.Size = new System.Drawing.Size(80, 23);
            this.tw_btnResetGame.TabIndex = 2;
            this.tw_btnResetGame.Text = "Reset Game";
            this.tw_btnResetGame.UseVisualStyleBackColor = true;
            this.tw_btnResetGame.Click += new System.EventHandler(this.TntWrsRstGame_Click);
            // 
            // tw_btnEditGame
            // 
            this.tw_btnEditGame.Location = new System.Drawing.Point(9, 307);
            this.tw_btnEditGame.Name = "tw_btnEditGame";
            this.tw_btnEditGame.Size = new System.Drawing.Size(453, 23);
            this.tw_btnEditGame.TabIndex = 1;
            this.tw_btnEditGame.Text = "Edit";
            this.tw_btnEditGame.UseVisualStyleBackColor = true;
            this.tw_btnEditGame.Click += new System.EventHandler(this.EditTntWarsGameBT_Click);
            // 
            // tw_lstGames
            // 
            this.tw_lstGames.FormattingEnabled = true;
            this.tw_lstGames.Location = new System.Drawing.Point(9, 333);
            this.tw_lstGames.Name = "tw_lstGames";
            this.tw_lstGames.Size = new System.Drawing.Size(453, 121);
            this.tw_lstGames.TabIndex = 0;
            // 
            // tabZS
            // 
            this.tabZS.BackColor = System.Drawing.SystemColors.Control;
            this.tabZS.Controls.Add(this.propsZG);
            this.tabZS.Location = new System.Drawing.Point(4, 22);
            this.tabZS.Name = "tabZS";
            this.tabZS.Padding = new System.Windows.Forms.Padding(3);
            this.tabZS.Size = new System.Drawing.Size(484, 489);
            this.tabZS.TabIndex = 1;
            this.tabZS.Text = "Zombie survival";
            // 
            // propsZG
            // 
            this.propsZG.Location = new System.Drawing.Point(6, 3);
            this.propsZG.Name = "propsZG";
            this.propsZG.Size = new System.Drawing.Size(456, 464);
            this.propsZG.TabIndex = 42;
            this.propsZG.ToolbarVisible = false;
            // 
            // pageCommands
            // 
            this.pageCommands.AutoScroll = true;
            this.pageCommands.Controls.Add(this.cmd_grpExtra);
            this.pageCommands.Controls.Add(this.cmd_grpPermissions);
            this.pageCommands.Controls.Add(this.cmd_btnCustom);
            this.pageCommands.Controls.Add(this.cmd_btnHelp);
            this.pageCommands.Controls.Add(this.cmd_list);
            this.pageCommands.Location = new System.Drawing.Point(4, 22);
            this.pageCommands.Name = "pageCommands";
            this.pageCommands.Size = new System.Drawing.Size(498, 521);
            this.pageCommands.TabIndex = 2;
            this.pageCommands.Text = "Commands";
            // 
            // cmd_grpExtra
            // 
            this.cmd_grpExtra.Controls.Add(this.cmd_cmbExtra7);
            this.cmd_grpExtra.Controls.Add(this.cmd_lblExtra7);
            this.cmd_grpExtra.Controls.Add(this.cmd_cmbExtra6);
            this.cmd_grpExtra.Controls.Add(this.cmd_lblExtra6);
            this.cmd_grpExtra.Controls.Add(this.cmd_cmbExtra5);
            this.cmd_grpExtra.Controls.Add(this.cmd_lblExtra5);
            this.cmd_grpExtra.Controls.Add(this.cmd_cmbExtra4);
            this.cmd_grpExtra.Controls.Add(this.cmd_lblExtra4);
            this.cmd_grpExtra.Controls.Add(this.cmd_cmbExtra3);
            this.cmd_grpExtra.Controls.Add(this.cmd_lblExtra3);
            this.cmd_grpExtra.Controls.Add(this.cmd_cmbExtra2);
            this.cmd_grpExtra.Controls.Add(this.cmd_lblExtra2);
            this.cmd_grpExtra.Controls.Add(this.cmd_cmbExtra1);
            this.cmd_grpExtra.Controls.Add(this.cmd_lblExtra1);
            this.cmd_grpExtra.Location = new System.Drawing.Point(133, 105);
            this.cmd_grpExtra.Name = "cmd_grpExtra";
            this.cmd_grpExtra.Size = new System.Drawing.Size(360, 205);
            this.cmd_grpExtra.TabIndex = 28;
            this.cmd_grpExtra.TabStop = false;
            this.cmd_grpExtra.Text = "Extra permissions";
            // 
            // cmd_cmbExtra7
            // 
            this.cmd_cmbExtra7.FormattingEnabled = true;
            this.cmd_cmbExtra7.Location = new System.Drawing.Point(10, 173);
            this.cmd_cmbExtra7.Name = "cmd_cmbExtra7";
            this.cmd_cmbExtra7.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbExtra7.TabIndex = 42;
            this.cmd_cmbExtra7.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbExtra_SelectedIndexChanged);
            // 
            // cmd_lblExtra7
            // 
            this.cmd_lblExtra7.AutoSize = true;
            this.cmd_lblExtra7.Location = new System.Drawing.Point(91, 176);
            this.cmd_lblExtra7.Name = "cmd_lblExtra7";
            this.cmd_lblExtra7.Size = new System.Drawing.Size(12, 13);
            this.cmd_lblExtra7.TabIndex = 41;
            this.cmd_lblExtra7.Text = "+";
            // 
            // cmd_cmbExtra6
            // 
            this.cmd_cmbExtra6.FormattingEnabled = true;
            this.cmd_cmbExtra6.Location = new System.Drawing.Point(10, 147);
            this.cmd_cmbExtra6.Name = "cmd_cmbExtra6";
            this.cmd_cmbExtra6.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbExtra6.TabIndex = 40;
            this.cmd_cmbExtra6.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbExtra_SelectedIndexChanged);
            // 
            // cmd_lblExtra6
            // 
            this.cmd_lblExtra6.AutoSize = true;
            this.cmd_lblExtra6.Location = new System.Drawing.Point(91, 150);
            this.cmd_lblExtra6.Name = "cmd_lblExtra6";
            this.cmd_lblExtra6.Size = new System.Drawing.Size(12, 13);
            this.cmd_lblExtra6.TabIndex = 39;
            this.cmd_lblExtra6.Text = "+";
            // 
            // cmd_cmbExtra5
            // 
            this.cmd_cmbExtra5.FormattingEnabled = true;
            this.cmd_cmbExtra5.Location = new System.Drawing.Point(10, 121);
            this.cmd_cmbExtra5.Name = "cmd_cmbExtra5";
            this.cmd_cmbExtra5.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbExtra5.TabIndex = 38;
            this.cmd_cmbExtra5.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbExtra_SelectedIndexChanged);
            // 
            // cmd_lblExtra5
            // 
            this.cmd_lblExtra5.AutoSize = true;
            this.cmd_lblExtra5.Location = new System.Drawing.Point(91, 124);
            this.cmd_lblExtra5.Name = "cmd_lblExtra5";
            this.cmd_lblExtra5.Size = new System.Drawing.Size(12, 13);
            this.cmd_lblExtra5.TabIndex = 37;
            this.cmd_lblExtra5.Text = "+";
            // 
            // cmd_cmbExtra4
            // 
            this.cmd_cmbExtra4.FormattingEnabled = true;
            this.cmd_cmbExtra4.Location = new System.Drawing.Point(10, 95);
            this.cmd_cmbExtra4.Name = "cmd_cmbExtra4";
            this.cmd_cmbExtra4.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbExtra4.TabIndex = 36;
            this.cmd_cmbExtra4.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbExtra_SelectedIndexChanged);
            // 
            // cmd_lblExtra4
            // 
            this.cmd_lblExtra4.AutoSize = true;
            this.cmd_lblExtra4.Location = new System.Drawing.Point(91, 98);
            this.cmd_lblExtra4.Name = "cmd_lblExtra4";
            this.cmd_lblExtra4.Size = new System.Drawing.Size(12, 13);
            this.cmd_lblExtra4.TabIndex = 35;
            this.cmd_lblExtra4.Text = "+";
            // 
            // cmd_cmbExtra3
            // 
            this.cmd_cmbExtra3.FormattingEnabled = true;
            this.cmd_cmbExtra3.Location = new System.Drawing.Point(10, 69);
            this.cmd_cmbExtra3.Name = "cmd_cmbExtra3";
            this.cmd_cmbExtra3.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbExtra3.TabIndex = 34;
            this.cmd_cmbExtra3.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbExtra_SelectedIndexChanged);
            // 
            // cmd_lblExtra3
            // 
            this.cmd_lblExtra3.AutoSize = true;
            this.cmd_lblExtra3.Location = new System.Drawing.Point(91, 72);
            this.cmd_lblExtra3.Name = "cmd_lblExtra3";
            this.cmd_lblExtra3.Size = new System.Drawing.Size(12, 13);
            this.cmd_lblExtra3.TabIndex = 33;
            this.cmd_lblExtra3.Text = "+";
            // 
            // cmd_cmbExtra2
            // 
            this.cmd_cmbExtra2.FormattingEnabled = true;
            this.cmd_cmbExtra2.Location = new System.Drawing.Point(10, 43);
            this.cmd_cmbExtra2.Name = "cmd_cmbExtra2";
            this.cmd_cmbExtra2.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbExtra2.TabIndex = 32;
            this.cmd_cmbExtra2.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbExtra_SelectedIndexChanged);
            // 
            // cmd_lblExtra2
            // 
            this.cmd_lblExtra2.AutoSize = true;
            this.cmd_lblExtra2.Location = new System.Drawing.Point(91, 46);
            this.cmd_lblExtra2.Name = "cmd_lblExtra2";
            this.cmd_lblExtra2.Size = new System.Drawing.Size(12, 13);
            this.cmd_lblExtra2.TabIndex = 31;
            this.cmd_lblExtra2.Text = "+";
            // 
            // cmd_cmbExtra1
            // 
            this.cmd_cmbExtra1.FormattingEnabled = true;
            this.cmd_cmbExtra1.Location = new System.Drawing.Point(10, 17);
            this.cmd_cmbExtra1.Name = "cmd_cmbExtra1";
            this.cmd_cmbExtra1.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbExtra1.TabIndex = 30;
            this.cmd_cmbExtra1.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbExtra_SelectedIndexChanged);
            // 
            // cmd_lblExtra1
            // 
            this.cmd_lblExtra1.AutoSize = true;
            this.cmd_lblExtra1.Location = new System.Drawing.Point(91, 20);
            this.cmd_lblExtra1.Name = "cmd_lblExtra1";
            this.cmd_lblExtra1.Size = new System.Drawing.Size(12, 13);
            this.cmd_lblExtra1.TabIndex = 29;
            this.cmd_lblExtra1.Text = "+";
            // 
            // cmd_grpPermissions
            // 
            this.cmd_grpPermissions.Controls.Add(this.cmd_cmbAlw3);
            this.cmd_grpPermissions.Controls.Add(this.cmd_cmbAlw2);
            this.cmd_grpPermissions.Controls.Add(this.cmd_cmbDis3);
            this.cmd_grpPermissions.Controls.Add(this.cmd_cmbDis2);
            this.cmd_grpPermissions.Controls.Add(this.cmd_cmbAlw1);
            this.cmd_grpPermissions.Controls.Add(this.cmd_cmbDis1);
            this.cmd_grpPermissions.Controls.Add(this.cmd_cmbMin);
            this.cmd_grpPermissions.Controls.Add(this.cmd_lblMin);
            this.cmd_grpPermissions.Controls.Add(this.cmd_lblDisallow);
            this.cmd_grpPermissions.Controls.Add(this.cmd_lblAllow);
            this.cmd_grpPermissions.Location = new System.Drawing.Point(133, 6);
            this.cmd_grpPermissions.Name = "cmd_grpPermissions";
            this.cmd_grpPermissions.Size = new System.Drawing.Size(360, 94);
            this.cmd_grpPermissions.TabIndex = 27;
            this.cmd_grpPermissions.TabStop = false;
            this.cmd_grpPermissions.Text = "Permissions";
            // 
            // cmd_cmbAlw3
            // 
            this.cmd_cmbAlw3.FormattingEnabled = true;
            this.cmd_cmbAlw3.Location = new System.Drawing.Point(274, 67);
            this.cmd_cmbAlw3.Name = "cmd_cmbAlw3";
            this.cmd_cmbAlw3.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbAlw3.TabIndex = 28;
            this.cmd_cmbAlw3.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbAlw2
            // 
            this.cmd_cmbAlw2.FormattingEnabled = true;
            this.cmd_cmbAlw2.Location = new System.Drawing.Point(187, 67);
            this.cmd_cmbAlw2.Name = "cmd_cmbAlw2";
            this.cmd_cmbAlw2.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbAlw2.TabIndex = 27;
            this.cmd_cmbAlw2.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbDis3
            // 
            this.cmd_cmbDis3.FormattingEnabled = true;
            this.cmd_cmbDis3.Location = new System.Drawing.Point(274, 41);
            this.cmd_cmbDis3.Name = "cmd_cmbDis3";
            this.cmd_cmbDis3.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbDis3.TabIndex = 26;
            this.cmd_cmbDis3.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbDis2
            // 
            this.cmd_cmbDis2.FormattingEnabled = true;
            this.cmd_cmbDis2.Location = new System.Drawing.Point(187, 41);
            this.cmd_cmbDis2.Name = "cmd_cmbDis2";
            this.cmd_cmbDis2.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbDis2.TabIndex = 25;
            this.cmd_cmbDis2.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbAlw1
            // 
            this.cmd_cmbAlw1.FormattingEnabled = true;
            this.cmd_cmbAlw1.Location = new System.Drawing.Point(100, 67);
            this.cmd_cmbAlw1.Name = "cmd_cmbAlw1";
            this.cmd_cmbAlw1.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbAlw1.TabIndex = 24;
            this.cmd_cmbAlw1.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbDis1
            // 
            this.cmd_cmbDis1.FormattingEnabled = true;
            this.cmd_cmbDis1.Location = new System.Drawing.Point(100, 41);
            this.cmd_cmbDis1.Name = "cmd_cmbDis1";
            this.cmd_cmbDis1.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbDis1.TabIndex = 23;
            this.cmd_cmbDis1.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbMin
            // 
            this.cmd_cmbMin.FormattingEnabled = true;
            this.cmd_cmbMin.Location = new System.Drawing.Point(100, 14);
            this.cmd_cmbMin.Name = "cmd_cmbMin";
            this.cmd_cmbMin.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbMin.TabIndex = 22;
            this.cmd_cmbMin.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbMin_SelectedIndexChanged);
            // 
            // cmd_lblMin
            // 
            this.cmd_lblMin.AutoSize = true;
            this.cmd_lblMin.Location = new System.Drawing.Point(10, 17);
            this.cmd_lblMin.Name = "cmd_lblMin";
            this.cmd_lblMin.Size = new System.Drawing.Size(89, 13);
            this.cmd_lblMin.TabIndex = 16;
            this.cmd_lblMin.Text = "Min rank needed:";
            // 
            // cmd_lblDisallow
            // 
            this.cmd_lblDisallow.AutoSize = true;
            this.cmd_lblDisallow.Location = new System.Drawing.Point(10, 71);
            this.cmd_lblDisallow.Name = "cmd_lblDisallow";
            this.cmd_lblDisallow.Size = new System.Drawing.Size(91, 13);
            this.cmd_lblDisallow.TabIndex = 18;
            this.cmd_lblDisallow.Text = "Specifically allow:";
            // 
            // cmd_lblAllow
            // 
            this.cmd_lblAllow.AutoSize = true;
            this.cmd_lblAllow.Location = new System.Drawing.Point(10, 44);
            this.cmd_lblAllow.Name = "cmd_lblAllow";
            this.cmd_lblAllow.Size = new System.Drawing.Size(80, 13);
            this.cmd_lblAllow.TabIndex = 17;
            this.cmd_lblAllow.Text = "But don\'t allow:";
            // 
            // cmd_btnCustom
            // 
            this.cmd_btnCustom.Location = new System.Drawing.Point(370, 485);
            this.cmd_btnCustom.Name = "cmd_btnCustom";
            this.cmd_btnCustom.Size = new System.Drawing.Size(121, 29);
            this.cmd_btnCustom.TabIndex = 26;
            this.cmd_btnCustom.Text = "Custom commands";
            this.cmd_btnCustom.UseVisualStyleBackColor = true;
            this.cmd_btnCustom.Click += new System.EventHandler(this.cmd_btnCustom_Click);
            // 
            // cmd_btnHelp
            // 
            this.cmd_btnHelp.Location = new System.Drawing.Point(6, 485);
            this.cmd_btnHelp.Name = "cmd_btnHelp";
            this.cmd_btnHelp.Size = new System.Drawing.Size(121, 29);
            this.cmd_btnHelp.TabIndex = 25;
            this.cmd_btnHelp.Text = "Help information";
            this.cmd_btnHelp.UseVisualStyleBackColor = true;
            this.cmd_btnHelp.Click += new System.EventHandler(this.cmd_btnHelp_Click);
            // 
            // cmd_list
            // 
            this.cmd_list.FormattingEnabled = true;
            this.cmd_list.Location = new System.Drawing.Point(6, 6);
            this.cmd_list.Name = "cmd_list";
            this.cmd_list.Size = new System.Drawing.Size(121, 472);
            this.cmd_list.TabIndex = 24;
            this.cmd_list.SelectedIndexChanged += new System.EventHandler(this.cmd_list_SelectedIndexChanged);
            // 
            // pageSecurity
            // 
            this.pageSecurity.BackColor = System.Drawing.SystemColors.Control;
            this.pageSecurity.Controls.Add(this.sec_grpChat);
            this.pageSecurity.Controls.Add(this.sec_grpCmd);
            this.pageSecurity.Controls.Add(this.sec_grpIP);
            this.pageSecurity.Controls.Add(this.sec_grpOther);
            this.pageSecurity.Controls.Add(this.sec_grpBlocks);
            this.pageSecurity.Location = new System.Drawing.Point(4, 22);
            this.pageSecurity.Name = "pageSecurity";
            this.pageSecurity.Padding = new System.Windows.Forms.Padding(3);
            this.pageSecurity.Size = new System.Drawing.Size(498, 521);
            this.pageSecurity.TabIndex = 7;
            this.pageSecurity.Text = "Security";
            // 
            // sec_grpChat
            // 
            this.sec_grpChat.Controls.Add(this.sec_cbChatAuto);
            this.sec_grpChat.Controls.Add(this.sec_lblChatOnMute);
            this.sec_grpChat.Controls.Add(this.sec_numChatMsgs);
            this.sec_grpChat.Controls.Add(this.sec_lblChatOnMsgs);
            this.sec_grpChat.Controls.Add(this.sec_numChatSecs);
            this.sec_grpChat.Controls.Add(this.sec_lblChatOnSecs);
            this.sec_grpChat.Controls.Add(this.sec_lblChatForMute);
            this.sec_grpChat.Controls.Add(this.sec_numChatMute);
            this.sec_grpChat.Controls.Add(this.sec_lblChatForSecs);
            this.sec_grpChat.Location = new System.Drawing.Point(14, 6);
            this.sec_grpChat.Name = "sec_grpChat";
            this.sec_grpChat.Size = new System.Drawing.Size(238, 111);
            this.sec_grpChat.TabIndex = 1;
            this.sec_grpChat.TabStop = false;
            this.sec_grpChat.Text = "Chat spam control";
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
                                    1000,
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
                                    1000,
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
                                    1000,
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
            // sec_grpCmd
            // 
            this.sec_grpCmd.Controls.Add(this.sec_cbCmdAuto);
            this.sec_grpCmd.Controls.Add(this.sec_lblCmdOnMute);
            this.sec_grpCmd.Controls.Add(this.sec_numCmdMsgs);
            this.sec_grpCmd.Controls.Add(this.sec_lblCmdOnMsgs);
            this.sec_grpCmd.Controls.Add(this.sec_numCmdSecs);
            this.sec_grpCmd.Controls.Add(this.sec_lblCmdOnSecs);
            this.sec_grpCmd.Controls.Add(this.sec_lblCmdForMute);
            this.sec_grpCmd.Controls.Add(this.sec_numCmdMute);
            this.sec_grpCmd.Controls.Add(this.sec_lblCmdForSecs);
            this.sec_grpCmd.Location = new System.Drawing.Point(14, 123);
            this.sec_grpCmd.Name = "sec_grpCmd";
            this.sec_grpCmd.Size = new System.Drawing.Size(238, 110);
            this.sec_grpCmd.TabIndex = 35;
            this.sec_grpCmd.TabStop = false;
            this.sec_grpCmd.Text = "Command spam control";
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
                                    1000,
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
                                    1000,
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
                                    1000,
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
            // sec_grpIP
            // 
            this.sec_grpIP.Controls.Add(this.sec_cbIPAuto);
            this.sec_grpIP.Controls.Add(this.sec_lblIPOnMute);
            this.sec_grpIP.Controls.Add(this.sec_numIPMsgs);
            this.sec_grpIP.Controls.Add(this.sec_lblIPOnMsgs);
            this.sec_grpIP.Controls.Add(this.sec_numIPSecs);
            this.sec_grpIP.Controls.Add(this.sec_lblIPOnSecs);
            this.sec_grpIP.Controls.Add(this.sec_lblIPForMute);
            this.sec_grpIP.Controls.Add(this.sec_numIPMute);
            this.sec_grpIP.Controls.Add(this.sec_lblIPForSecs);
            this.sec_grpIP.Location = new System.Drawing.Point(14, 240);
            this.sec_grpIP.Name = "sec_grpIP";
            this.sec_grpIP.Size = new System.Drawing.Size(238, 110);
            this.sec_grpIP.TabIndex = 37;
            this.sec_grpIP.TabStop = false;
            this.sec_grpIP.Text = "Connection spam control";
            // 
            // sec_cbIPAuto
            // 
            this.sec_cbIPAuto.AutoSize = true;
            this.sec_cbIPAuto.Location = new System.Drawing.Point(10, 20);
            this.sec_cbIPAuto.Name = "sec_cbIPAuto";
            this.sec_cbIPAuto.Size = new System.Drawing.Size(149, 17);
            this.sec_cbIPAuto.TabIndex = 24;
            this.sec_cbIPAuto.Text = "Enable automatic blocking";
            this.sec_cbIPAuto.UseVisualStyleBackColor = true;
            this.sec_cbIPAuto.CheckedChanged += new System.EventHandler(this.sec_cbIPAuto_Checked);
            // 
            // sec_lblIPOnMute
            // 
            this.sec_lblIPOnMute.AutoSize = true;
            this.sec_lblIPOnMute.Location = new System.Drawing.Point(6, 48);
            this.sec_lblIPOnMute.Name = "sec_lblIPOnMute";
            this.sec_lblIPOnMute.Size = new System.Drawing.Size(46, 13);
            this.sec_lblIPOnMute.TabIndex = 25;
            this.sec_lblIPOnMute.Text = "Block on";
            // 
            // sec_numIPMsgs
            // 
            this.sec_numIPMsgs.Location = new System.Drawing.Point(53, 45);
            this.sec_numIPMsgs.Maximum = new decimal(new int[] {
                                    1000,
                                    0,
                                    0,
                                    0});
            this.sec_numIPMsgs.Name = "sec_numIPMsgs";
            this.sec_numIPMsgs.Size = new System.Drawing.Size(37, 21);
            this.sec_numIPMsgs.TabIndex = 30;
            this.sec_numIPMsgs.Value = new decimal(new int[] {
                                    25,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblIPOnMsgs
            // 
            this.sec_lblIPOnMsgs.AutoSize = true;
            this.sec_lblIPOnMsgs.Location = new System.Drawing.Point(91, 48);
            this.sec_lblIPOnMsgs.Name = "sec_lblIPOnMsgs";
            this.sec_lblIPOnMsgs.Size = new System.Drawing.Size(75, 13);
            this.sec_lblIPOnMsgs.TabIndex = 31;
            this.sec_lblIPOnMsgs.Text = "connections in";
            // 
            // sec_numIPSecs
            // 
            this.sec_numIPSecs.Location = new System.Drawing.Point(166, 45);
            this.sec_numIPSecs.Maximum = new decimal(new int[] {
                                    1000,
                                    0,
                                    0,
                                    0});
            this.sec_numIPSecs.Name = "sec_numIPSecs";
            this.sec_numIPSecs.Size = new System.Drawing.Size(42, 21);
            this.sec_numIPSecs.TabIndex = 34;
            this.sec_numIPSecs.Value = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblIPOnSecs
            // 
            this.sec_lblIPOnSecs.AutoSize = true;
            this.sec_lblIPOnSecs.Location = new System.Drawing.Point(209, 48);
            this.sec_lblIPOnSecs.Name = "sec_lblIPOnSecs";
            this.sec_lblIPOnSecs.Size = new System.Drawing.Size(28, 13);
            this.sec_lblIPOnSecs.TabIndex = 33;
            this.sec_lblIPOnSecs.Text = "secs";
            // 
            // sec_lblIPForMute
            // 
            this.sec_lblIPForMute.AutoSize = true;
            this.sec_lblIPForMute.Location = new System.Drawing.Point(6, 83);
            this.sec_lblIPForMute.Name = "sec_lblIPForMute";
            this.sec_lblIPForMute.Size = new System.Drawing.Size(47, 13);
            this.sec_lblIPForMute.TabIndex = 25;
            this.sec_lblIPForMute.Text = "Block for";
            // 
            // sec_numIPMute
            // 
            this.sec_numIPMute.Location = new System.Drawing.Point(53, 79);
            this.sec_numIPMute.Maximum = new decimal(new int[] {
                                    1000,
                                    0,
                                    0,
                                    0});
            this.sec_numIPMute.Name = "sec_numIPMute";
            this.sec_numIPMute.Size = new System.Drawing.Size(42, 21);
            this.sec_numIPMute.TabIndex = 32;
            this.sec_numIPMute.Value = new decimal(new int[] {
                                    300,
                                    0,
                                    0,
                                    0});
            // 
            // sec_lblIPForSecs
            // 
            this.sec_lblIPForSecs.AutoSize = true;
            this.sec_lblIPForSecs.Location = new System.Drawing.Point(96, 83);
            this.sec_lblIPForSecs.Name = "sec_lblIPForSecs";
            this.sec_lblIPForSecs.Size = new System.Drawing.Size(46, 13);
            this.sec_lblIPForSecs.TabIndex = 33;
            this.sec_lblIPForSecs.Text = "seconds";
            // 
            // sec_grpOther
            // 
            this.sec_grpOther.Controls.Add(this.sec_lblRank);
            this.sec_grpOther.Controls.Add(this.sec_cbWhitelist);
            this.sec_grpOther.Controls.Add(this.sec_cbLogNotes);
            this.sec_grpOther.Controls.Add(this.sec_cbVerifyAdmins);
            this.sec_grpOther.Controls.Add(this.sec_cmbVerifyRank);
            this.sec_grpOther.Location = new System.Drawing.Point(264, 6);
            this.sec_grpOther.Name = "sec_grpOther";
            this.sec_grpOther.Size = new System.Drawing.Size(217, 138);
            this.sec_grpOther.TabIndex = 2;
            this.sec_grpOther.TabStop = false;
            this.sec_grpOther.Text = "Other settings";
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
            // sec_grpBlocks
            // 
            this.sec_grpBlocks.Controls.Add(this.sec_cbBlocksAuto);
            this.sec_grpBlocks.Controls.Add(this.sec_lblBlocksOnMute);
            this.sec_grpBlocks.Controls.Add(this.sec_numBlocksMsgs);
            this.sec_grpBlocks.Controls.Add(this.sec_lblBlocksOnMsgs);
            this.sec_grpBlocks.Controls.Add(this.sec_numBlocksSecs);
            this.sec_grpBlocks.Controls.Add(this.sec_lblBlocksOnSecs);
            this.sec_grpBlocks.Location = new System.Drawing.Point(264, 150);
            this.sec_grpBlocks.Name = "sec_grpBlocks";
            this.sec_grpBlocks.Size = new System.Drawing.Size(217, 83);
            this.sec_grpBlocks.TabIndex = 36;
            this.sec_grpBlocks.TabStop = false;
            this.sec_grpBlocks.Text = "Block spam control";
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
                                    1000,
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
                                    1000,
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
            // PropertyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(507, 585);
            this.Controls.Add(this.main_btnApply);
            this.Controls.Add(this.main_btnDiscard);
            this.Controls.Add(this.main_btnSave);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PropertyWindow";
            this.Text = "Properties";
            this.Load += new System.EventHandler(this.PropertyWindow_Load);
            this.Disposed += new System.EventHandler(this.PropertyWindow_Unload);
            this.pageChat.ResumeLayout(false);
            this.chat_grpTab.ResumeLayout(false);
            this.chat_grpTab.PerformLayout();
            this.chat_grpMessages.ResumeLayout(false);
            this.chat_grpMessages.PerformLayout();
            this.chat_grpOther.ResumeLayout(false);
            this.chat_grpOther.PerformLayout();
            this.chat_grpColors.ResumeLayout(false);
            this.chat_grpColors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bak_numTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.srv_numPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numPerm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numMaps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numDraw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numUndo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numGen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numAfk)).EndInit();
            this.pageBlocks.ResumeLayout(false);
            this.blk_grpPhysics.ResumeLayout(false);
            this.blk_grpBehaviour.ResumeLayout(false);
            this.blk_grpBehaviour.PerformLayout();
            this.blk_grpPermissions.ResumeLayout(false);
            this.blk_grpPermissions.PerformLayout();
            this.pageRanks.ResumeLayout(false);
            this.rank_grpLimits.ResumeLayout(false);
            this.rank_grpLimits.PerformLayout();
            this.rank_grpGeneral.ResumeLayout(false);
            this.rank_grpGeneral.PerformLayout();
            this.rank_grpMisc.ResumeLayout(false);
            this.rank_grpMisc.PerformLayout();
            this.pageMisc.ResumeLayout(false);
            this.pageMisc.PerformLayout();
            this.eco_grpEco.ResumeLayout(false);
            this.grpExtra.ResumeLayout(false);
            this.grpExtra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldownTime)).EndInit();
            this.grpMessages.ResumeLayout(false);
            this.grpMessages.PerformLayout();
            this.grpPhysics.ResumeLayout(false);
            this.grpPhysics.PerformLayout();
            this.afk_grp.ResumeLayout(false);
            this.afk_grp.PerformLayout();
            this.bak_grp.ResumeLayout(false);
            this.bak_grp.PerformLayout();
            this.pageIRC.ResumeLayout(false);
            this.gb_ircSettings.ResumeLayout(false);
            this.gb_ircSettings.PerformLayout();
            this.sql_grp.ResumeLayout(false);
            this.sql_grp.PerformLayout();
            this.irc_grp.ResumeLayout(false);
            this.irc_grp.PerformLayout();
            this.pageServer.ResumeLayout(false);
            this.lvl_grp.ResumeLayout(false);
            this.lvl_grp.PerformLayout();
            this.adv_grp.ResumeLayout(false);
            this.adv_grp.PerformLayout();
            this.srv_grp.ResumeLayout(false);
            this.srv_grp.PerformLayout();
            this.srv_grpUpdate.ResumeLayout(false);
            this.srv_grpUpdate.PerformLayout();
            this.grpPlayers.ResumeLayout(false);
            this.grpPlayers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srv_numPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.srv_numGuests)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.pageGames.ResumeLayout(false);
            this.tabGames.ResumeLayout(false);
            this.tabPage10.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.ls_grpMapSettings.ResumeLayout(false);
            this.groupBox21.ResumeLayout(false);
            this.ls_grpMaps.ResumeLayout(false);
            this.ls_grpMaps.PerformLayout();
            this.tabTntWars.ResumeLayout(false);
            this.tabTntWars.PerformLayout();
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
            this.tw_grpScores.ResumeLayout(false);
            this.tw_grpScores.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numMultiKills)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScoreAssists)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScorePerKill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScoreLimit)).EndInit();
            this.tw_grpStatus.ResumeLayout(false);
            this.tabZS.ResumeLayout(false);
            this.pageCommands.ResumeLayout(false);
            this.cmd_grpExtra.ResumeLayout(false);
            this.cmd_grpExtra.PerformLayout();
            this.cmd_grpPermissions.ResumeLayout(false);
            this.cmd_grpPermissions.PerformLayout();
            this.pageSecurity.ResumeLayout(false);
            this.sec_grpChat.ResumeLayout(false);
            this.sec_grpChat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatMsgs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatSecs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numChatMute)).EndInit();
            this.sec_grpCmd.ResumeLayout(false);
            this.sec_grpCmd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdMsgs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdSecs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numCmdMute)).EndInit();
            this.sec_grpIP.ResumeLayout(false);
            this.sec_grpIP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numIPMsgs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numIPSecs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numIPMute)).EndInit();
            this.sec_grpOther.ResumeLayout(false);
            this.sec_grpOther.PerformLayout();
            this.sec_grpBlocks.ResumeLayout(false);
            this.sec_grpBlocks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numBlocksMsgs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sec_numBlocksSecs)).EndInit();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.CheckBox rank_cbAfk;
        private System.Windows.Forms.NumericUpDown rank_numAfk;
        private System.Windows.Forms.Label rank_lblAfk;
        private System.Windows.Forms.Label rank_lblUndo;
        private System.Windows.Forms.NumericUpDown rank_numUndo;
        private System.Windows.Forms.Label rank_lblDraw;
        private System.Windows.Forms.NumericUpDown rank_numDraw;
        private System.Windows.Forms.NumericUpDown rank_numMaps;
        private System.Windows.Forms.Label rank_lblMaps;
        private System.Windows.Forms.NumericUpDown rank_numGen;
        private System.Windows.Forms.Label rank_lblGen;
        private System.Windows.Forms.GroupBox rank_grpLimits;
        private System.Windows.Forms.Label irc_lblRank;
        private System.Windows.Forms.ComboBox irc_cbRank;
        private System.Windows.Forms.Label irc_lblVerify;
        private System.Windows.Forms.ComboBox irc_cbVerify;
        private System.Windows.Forms.Label irc_lblPrefix;
        private System.Windows.Forms.TextBox irc_txtPrefix;
        private System.Windows.Forms.CheckBox irc_cbAFK;
        private System.Windows.Forms.CheckBox irc_cbWorldChanges;
        private System.Windows.Forms.GroupBox gb_ircSettings;
        private System.Windows.Forms.Label cmd_lblExtra1;
        private System.Windows.Forms.ComboBox cmd_cmbExtra1;
        private System.Windows.Forms.Label cmd_lblExtra2;
        private System.Windows.Forms.ComboBox cmd_cmbExtra2;
        private System.Windows.Forms.Label cmd_lblExtra3;
        private System.Windows.Forms.ComboBox cmd_cmbExtra3;
        private System.Windows.Forms.Label cmd_lblExtra4;
        private System.Windows.Forms.ComboBox cmd_cmbExtra4;
        private System.Windows.Forms.Label cmd_lblExtra5;
        private System.Windows.Forms.ComboBox cmd_cmbExtra5;
        private System.Windows.Forms.Label cmd_lblExtra6;
        private System.Windows.Forms.ComboBox cmd_cmbExtra6;
        private System.Windows.Forms.Label cmd_lblExtra7;
        private System.Windows.Forms.ComboBox cmd_cmbExtra7;
        private System.Windows.Forms.GroupBox cmd_grpExtra;
        private System.Windows.Forms.ListBox cmd_list;
        private System.Windows.Forms.Button cmd_btnHelp;
        private System.Windows.Forms.Label cmd_lblAllow;
        private System.Windows.Forms.Label cmd_lblDisallow;
        private System.Windows.Forms.Label cmd_lblMin;
        private System.Windows.Forms.ComboBox cmd_cmbMin;
        private System.Windows.Forms.ComboBox cmd_cmbDis1;
        private System.Windows.Forms.ComboBox cmd_cmbAlw1;
        private System.Windows.Forms.ComboBox cmd_cmbDis2;
        private System.Windows.Forms.ComboBox cmd_cmbDis3;
        private System.Windows.Forms.ComboBox cmd_cmbAlw2;
        private System.Windows.Forms.ComboBox cmd_cmbAlw3;
        private System.Windows.Forms.GroupBox cmd_grpPermissions;
        private System.Windows.Forms.Button cmd_btnCustom;
        private System.Windows.Forms.CheckBox blk_cbLava;
        private System.Windows.Forms.CheckBox blk_cbWater;
        private System.Windows.Forms.CheckBox blk_cbDoor;
        private System.Windows.Forms.CheckBox blk_cbTdoor;
        private System.Windows.Forms.CheckBox blk_cbRails;
        private System.Windows.Forms.CheckBox blk_cbMsgBlock;
        private System.Windows.Forms.CheckBox blk_cbPortal;
        private System.Windows.Forms.CheckBox blk_cbDeath;
        private System.Windows.Forms.TextBox blk_txtDeath;
        private System.Windows.Forms.GroupBox blk_grpBehaviour;
        private System.Windows.Forms.GroupBox blk_grpPhysics;
        private System.Windows.Forms.ComboBox blk_cmbMin;
        private System.Windows.Forms.ComboBox blk_cmbDis1;
        private System.Windows.Forms.ComboBox blk_cmbAlw1;
        private System.Windows.Forms.ComboBox blk_cmbDis2;
        private System.Windows.Forms.ComboBox blk_cmbDis3;
        private System.Windows.Forms.ComboBox blk_cmbAlw2;
        private System.Windows.Forms.ComboBox blk_cmbAlw3;
        private System.Windows.Forms.GroupBox blk_grpPermissions;
        #endregion
        
        private System.Windows.Forms.ComboBox rank_cmbOsMap;
        private System.Windows.Forms.Label rank_lblOsMap;
        private System.Windows.Forms.PropertyGrid pg_lavaMap;
        private System.Windows.Forms.PropertyGrid pg_lava;
        private System.Windows.Forms.TextBox rank_txtPrefix;
        private System.Windows.Forms.Label rank_lblPrefix;
        private System.Windows.Forms.Label srv_lblOwner;
        private System.Windows.Forms.GroupBox rank_grpMisc;
        private System.Windows.Forms.GroupBox rank_grpGeneral;
        
        private System.Windows.Forms.TabPage pageChat;
        private System.Windows.Forms.GroupBox chat_grpTab;
        private System.Windows.Forms.CheckBox chat_cbTabRank;
        private System.Windows.Forms.CheckBox chat_cbTabLevel;
        private System.Windows.Forms.CheckBox chat_cbTabBots;
        
        private System.Windows.Forms.GroupBox chat_grpMessages;
        private System.Windows.Forms.Label chat_lblShutdown;
        private System.Windows.Forms.TextBox chat_txtShutdown;
        private System.Windows.Forms.CheckBox chat_chkCheap;
        private System.Windows.Forms.TextBox chat_txtCheap;
        private System.Windows.Forms.Label chat_lblBan;
        private System.Windows.Forms.TextBox chat_txtBan;
        private System.Windows.Forms.Label chat_lblPromote;
        private System.Windows.Forms.TextBox chat_txtPromote;
        private System.Windows.Forms.Label chat_lblDemote;
        private System.Windows.Forms.TextBox chat_txtDemote;
        
        private System.Windows.Forms.GroupBox chat_grpColors;
        private System.Windows.Forms.Label chat_lblDefault;
        private System.Windows.Forms.Button chat_btnDefault;
        private System.Windows.Forms.Label chat_lblIRC;
        private System.Windows.Forms.Button chat_btnIRC;
        private System.Windows.Forms.Label chat_lblSyntax;
        private System.Windows.Forms.Button chat_btnSyntax;
        private System.Windows.Forms.Label chat_lblDesc;
        private System.Windows.Forms.Button chat_btnDesc;
        
        private System.Windows.Forms.GroupBox chat_grpOther;
        private System.Windows.Forms.Label chat_lblConsole;
        private System.Windows.Forms.TextBox chat_txtConsole;
        
        
        private System.Windows.Forms.TabPage pageSecurity;
        private System.Windows.Forms.GroupBox sec_grpChat;
        private System.Windows.Forms.CheckBox sec_cbChatAuto;
        private System.Windows.Forms.Label sec_lblChatForMute;
        private System.Windows.Forms.NumericUpDown sec_numChatMute;
        private System.Windows.Forms.Label sec_lblChatOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numChatMsgs;
        private System.Windows.Forms.Label sec_lblChatOnMute;
        private System.Windows.Forms.NumericUpDown sec_numChatSecs;
        private System.Windows.Forms.Label sec_lblChatOnSecs;
        private System.Windows.Forms.Label sec_lblChatForSecs;
        
        private System.Windows.Forms.GroupBox sec_grpCmd;
        private System.Windows.Forms.CheckBox sec_cbCmdAuto;
        private System.Windows.Forms.Label sec_lblCmdForSecs;
        private System.Windows.Forms.NumericUpDown sec_numCmdMute;
        private System.Windows.Forms.Label sec_lblCmdForMute;
        private System.Windows.Forms.Label sec_lblCmdOnSecs;
        private System.Windows.Forms.NumericUpDown sec_numCmdSecs;
        private System.Windows.Forms.Label sec_lblCmdOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numCmdMsgs;
        private System.Windows.Forms.Label sec_lblCmdOnMute;
        
        private System.Windows.Forms.GroupBox sec_grpIP;
        private System.Windows.Forms.CheckBox sec_cbIPAuto;
        private System.Windows.Forms.Label sec_lblIPForSecs;
        private System.Windows.Forms.NumericUpDown sec_numIPMute;
        private System.Windows.Forms.Label sec_lblIPForMute;
        private System.Windows.Forms.Label sec_lblIPOnSecs;
        private System.Windows.Forms.NumericUpDown sec_numIPSecs;
        private System.Windows.Forms.Label sec_lblIPOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numIPMsgs;
        private System.Windows.Forms.Label sec_lblIPOnMute;
        
        private System.Windows.Forms.GroupBox sec_grpOther;
        private System.Windows.Forms.CheckBox sec_cbLogNotes;
        private System.Windows.Forms.CheckBox sec_cbWhitelist;
        private System.Windows.Forms.CheckBox sec_cbVerifyAdmins;
        private System.Windows.Forms.Label sec_lblRank;
        private System.Windows.Forms.ComboBox sec_cmbVerifyRank;
        
        private System.Windows.Forms.GroupBox sec_grpBlocks;
        private System.Windows.Forms.CheckBox sec_cbBlocksAuto;
        private System.Windows.Forms.Label sec_lblBlocksOnSecs;
        private System.Windows.Forms.NumericUpDown sec_numBlocksSecs;
        private System.Windows.Forms.Label sec_lblBlocksOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numBlocksMsgs;
        private System.Windows.Forms.Label sec_lblBlocksOnMute;
        
        private System.Windows.Forms.Button main_btnSave;
        private System.Windows.Forms.Button main_btnDiscard;
        private System.Windows.Forms.Button main_btnApply;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabPage pageBlocks;
        private System.Windows.Forms.Button blk_btnHelp;
        private System.Windows.Forms.Label blk_lblAllow;
        private System.Windows.Forms.Label blk_lblDisallow;
        private System.Windows.Forms.Label blk_lblMin;
        private System.Windows.Forms.ListBox blk_list;
        private System.Windows.Forms.TabPage pageCommands;
        private System.Windows.Forms.TabPage pageRanks;
        private System.Windows.Forms.Button rank_btnColor;
        private System.Windows.Forms.Label rank_lblColor;
        private System.Windows.Forms.NumericUpDown rank_numPerm;
        private System.Windows.Forms.TextBox rank_txtName;
        private System.Windows.Forms.Label rank_lblPerm;
        private System.Windows.Forms.Label rank_lblName;
        private System.Windows.Forms.Button rank_btnDel;
        private System.Windows.Forms.Button rank_btnAdd;
        private System.Windows.Forms.ListBox rank_list;
        private System.Windows.Forms.TabPage pageMisc;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox hackrank_kick_time;
        private System.Windows.Forms.TextBox txtNormRp;
        private System.Windows.Forms.TextBox txtRP;
        private System.Windows.Forms.TextBox afk_txtTimer;
        private System.Windows.Forms.NumericUpDown bak_numTime;
        private System.Windows.Forms.TextBox bak_txtLocation;
        private System.Windows.Forms.TextBox txtMoneys;
        private System.Windows.Forms.TextBox txtRestartTime;
        private System.Windows.Forms.CheckBox hackrank_kick;
        private System.Windows.Forms.CheckBox chkProfanityFilter;
        private System.Windows.Forms.CheckBox chkRepeatMessages;
        private System.Windows.Forms.CheckBox chkDeath;
        private System.Windows.Forms.CheckBox chk17Dollar;
        private System.Windows.Forms.CheckBox chkPhysicsRest;
        private System.Windows.Forms.CheckBox chkSmile;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label afk_lblTimer;
        private System.Windows.Forms.Label bak_lblTime;
        private System.Windows.Forms.Label bak_lblLocation;
        private System.Windows.Forms.CheckBox chkRestartTime;
        private System.Windows.Forms.TabPage pageIRC;
        private System.Windows.Forms.TextBox irc_txtOpChannel;
        private System.Windows.Forms.TextBox irc_txtChannel;
        private System.Windows.Forms.TextBox irc_txtServer;
        private System.Windows.Forms.TextBox irc_txtNick;
        private System.Windows.Forms.Label irc_lblOpChannel;
        private System.Windows.Forms.CheckBox irc_chkEnabled;
        private System.Windows.Forms.Label irc_lblChannel;
        private System.Windows.Forms.Label irc_lblServer;
        private System.Windows.Forms.Label irc_lblNick;
        private System.Windows.Forms.TabPage pageServer;
        private System.Windows.Forms.NumericUpDown srv_numPlayers;
        private System.Windows.Forms.NumericUpDown srv_numGuests;
        private System.Windows.Forms.Label srv_lblGuests;
        private System.Windows.Forms.TextBox lvl_txtMain;
        private System.Windows.Forms.NumericUpDown srv_numPort;
        private System.Windows.Forms.TextBox srv_txtMOTD;
        private System.Windows.Forms.TextBox srv_txtName;
        private System.Windows.Forms.Button srv_btnPort;
        private System.Windows.Forms.ComboBox rank_cmbDefault;
        private System.Windows.Forms.Label rank_lblDefault;
        private System.Windows.Forms.CheckBox adv_chkRestart;
        private System.Windows.Forms.CheckBox srv_chkPublic;
        private System.Windows.Forms.CheckBox lvl_chkAutoload;
        private System.Windows.Forms.CheckBox lvl_chkWorld;
        private System.Windows.Forms.CheckBox chkUpdates;
        private System.Windows.Forms.CheckBox adv_chkVerify;
        private System.Windows.Forms.Label lvl_lblMain;
        private System.Windows.Forms.Label srv_lblPlayers;
        private System.Windows.Forms.Label srv_lblPort;
        private System.Windows.Forms.Label srv_lblMotd;
        private System.Windows.Forms.Label srv_lblName;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.CheckBox srv_cbMustAgree;
        private System.Windows.Forms.CheckBox rank_cbSilentAdmins;
        private System.Windows.Forms.Button adv_btnEditTexts;
        private System.Windows.Forms.CheckBox rank_cbTPHigher;
        private System.Windows.Forms.GroupBox irc_grp;
        private System.Windows.Forms.GroupBox srv_grp;
        private System.Windows.Forms.GroupBox adv_grp;
        private System.Windows.Forms.GroupBox grpPlayers;
        private System.Windows.Forms.GroupBox lvl_grp;
        private System.Windows.Forms.TextBox srv_txtOwner;
        private System.Windows.Forms.GroupBox grpExtra;
        private System.Windows.Forms.GroupBox grpMessages;
        private System.Windows.Forms.GroupBox grpPhysics;
        private System.Windows.Forms.GroupBox afk_grp;
        private System.Windows.Forms.GroupBox bak_grp;
        private System.Windows.Forms.GroupBox sql_grp;
        private System.Windows.Forms.LinkLabel sql_linkDownload;
        private System.Windows.Forms.TextBox sql_txtHost;
        private System.Windows.Forms.Label sql_lblHost;
        private System.Windows.Forms.TextBox sql_txtDBName;
        private System.Windows.Forms.Label sql_lblDBName;
        private System.Windows.Forms.Label sql_lblPass;
        private System.Windows.Forms.Label sql_lblUser;
        private System.Windows.Forms.TextBox sql_txtPass;
        private System.Windows.Forms.TextBox sql_txtUser;
        private System.Windows.Forms.CheckBox sql_chkUseSQL;
        private System.Windows.Forms.TabPage pageGames;
        private System.Windows.Forms.TabControl tabGames;
        private System.Windows.Forms.TabPage tabZS;
        private System.Windows.Forms.TabPage tabPage10;
        private System.Windows.Forms.TextBox irc_txtPass;
        private System.Windows.Forms.CheckBox irc_chkPass;
        private System.Windows.Forms.CheckBox irc_cbTitles;
        private System.Windows.Forms.Label irc_lblPort;
        private System.Windows.Forms.TextBox irc_txtPort;
        private System.Windows.Forms.CheckBox rank_cbEmpty;
        private System.Windows.Forms.GroupBox ls_grpMaps;
        private System.Windows.Forms.Label ls_lblNotUsed;
        private System.Windows.Forms.Label ls_lblUsed;
        private System.Windows.Forms.Button ls_btnAdd;
        private System.Windows.Forms.Button ls_btnRemove;
        private System.Windows.Forms.ListBox ls_lstNotUsed;
        private System.Windows.Forms.ListBox ls_lstUsed;
        private System.Windows.Forms.GroupBox groupBox21;
        private System.Windows.Forms.GroupBox ls_grpMapSettings;
        private System.Windows.Forms.GroupBox groupBox23;
        private System.Windows.Forms.Button ls_btnEndRound;
        private System.Windows.Forms.Button ls_btnStopGame;
        private System.Windows.Forms.Button ls_btnStartGame;
        private System.Windows.Forms.Button lsBtnEndVote;
        private System.Windows.Forms.TextBox sql_txtPort;
        private System.Windows.Forms.Label sql_lblPort;
        private System.Windows.Forms.GroupBox srv_grpUpdate;
        private System.Windows.Forms.Button srv_btnForceUpdate;
        private System.Windows.Forms.CheckBox chkGuestLimitNotify;
        private System.Windows.Forms.NumericUpDown nudCooldownTime;
        private System.Windows.Forms.Label misc_lblReview;
        private System.Windows.Forms.Label rank_lblMOTD;
        private System.Windows.Forms.TextBox rank_txtMOTD;
        private System.Windows.Forms.TabPage tabTntWars;
        private System.Windows.Forms.TextBox tw_txtStatus;
        private System.Windows.Forms.Label tw_lblStatus;
        private System.Windows.Forms.Label label85;
        private System.Windows.Forms.TextBox SlctdTntWrsLvl;
        private System.Windows.Forms.GroupBox groupBox29;
        private System.Windows.Forms.Button tw_btnEditGame;
        private System.Windows.Forms.ListBox tw_lstGames;
        private System.Windows.Forms.TextBox tw_txtPlayers;
        private System.Windows.Forms.Label tw_lblPlayers;
        private System.Windows.Forms.GroupBox tw_grpStatus;
        private System.Windows.Forms.Button tw_btnStartGame;
        private System.Windows.Forms.Button tw_btnDeleteGame;
        private System.Windows.Forms.Button tw_btnEndGame;
        private System.Windows.Forms.Button tw_btnResetGame;
        private System.Windows.Forms.GroupBox tw_grpScores;
        private System.Windows.Forms.CheckBox tw_cbScoreAssists;
        private System.Windows.Forms.NumericUpDown tw_numScoreAssists;
        private System.Windows.Forms.Label tw_lblScorePerKill;
        private System.Windows.Forms.NumericUpDown tw_numScorePerKill;
        private System.Windows.Forms.Label tw_lblScoreLimit;
        private System.Windows.Forms.NumericUpDown tw_numScoreLimit;
        private System.Windows.Forms.CheckBox tw_cbMultiKills;
        private System.Windows.Forms.NumericUpDown tw_numMultiKills;
        private System.Windows.Forms.GroupBox groupBox32;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.NumericUpDown TntWrsGraceTimeChck;
        private System.Windows.Forms.GroupBox groupBox33;
        private System.Windows.Forms.CheckBox tw_cbTeamKills;
        private System.Windows.Forms.CheckBox tw_cbBalanceTeams;
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
        private System.Windows.Forms.CheckBox tw_cbStreaks;
        private System.Windows.Forms.Button eco_btnEco;
        private System.Windows.Forms.PropertyGrid propsZG;
        private System.Windows.Forms.GroupBox eco_grpEco;
    }
}