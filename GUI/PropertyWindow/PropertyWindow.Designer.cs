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
            this.chat_chkFilter = new System.Windows.Forms.CheckBox();
            this.chat_lblConsole = new System.Windows.Forms.Label();
            this.chat_txtConsole = new System.Windows.Forms.TextBox();
            this.chat_grpColors = new System.Windows.Forms.GroupBox();
            this.chat_lblWarn = new System.Windows.Forms.Label();
            this.chat_btnWarn = new System.Windows.Forms.Button();
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
            this.chkRpLimit = new System.Windows.Forms.Label();
            this.chkDeath = new System.Windows.Forms.CheckBox();
            this.hack_lbl = new System.Windows.Forms.CheckBox();
            this.sec_cmbVerifyRank = new System.Windows.Forms.ComboBox();
            this.sec_cbVerifyAdmins = new System.Windows.Forms.CheckBox();
            this.chkGuestLimitNotify = new System.Windows.Forms.CheckBox();
            this.rank_cbTPHigher = new System.Windows.Forms.CheckBox();
            this.rank_cmbDefault = new System.Windows.Forms.ComboBox();
            this.sec_cbWhitelist = new System.Windows.Forms.CheckBox();
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
            this.rank_numGen = new System.Windows.Forms.NumericUpDown();
            this.rank_numCopy = new System.Windows.Forms.NumericUpDown();
            this.dis_grp = new System.Windows.Forms.GroupBox();
            this.dis_lblToken = new System.Windows.Forms.Label();
            this.dis_lblChannel = new System.Windows.Forms.Label();
            this.dis_txtChannel = new System.Windows.Forms.TextBox();
            this.dis_lblOpChannel = new System.Windows.Forms.Label();
            this.dis_chkEnabled = new System.Windows.Forms.CheckBox();
            this.dis_chkNicks = new System.Windows.Forms.CheckBox();
            this.dis_txtToken = new System.Windows.Forms.TextBox();
            this.dis_txtOpChannel = new System.Windows.Forms.TextBox();
            this.dis_linkHelp = new System.Windows.Forms.LinkLabel();
            this.adv_chkCPE = new System.Windows.Forms.CheckBox();
            this.eco_cmbItemRank = new System.Windows.Forms.ComboBox();
            this.rank_numUndo = new MCGalaxy.Gui.TimespanUpDown();
            this.chkPhysRestart = new System.Windows.Forms.CheckBox();
            this.ls_numMax = new System.Windows.Forms.NumericUpDown();
            this.ls_numKiller = new System.Windows.Forms.NumericUpDown();
            this.ls_numFast = new System.Windows.Forms.NumericUpDown();
            this.ls_numWater = new System.Windows.Forms.NumericUpDown();
            this.ls_numDestroy = new System.Windows.Forms.NumericUpDown();
            this.ls_numLayer = new System.Windows.Forms.NumericUpDown();
            this.ls_numCount = new System.Windows.Forms.NumericUpDown();
            this.ls_numHeight = new System.Windows.Forms.NumericUpDown();
            this.ls_cbMain = new System.Windows.Forms.CheckBox();
            this.ls_cbStart = new System.Windows.Forms.CheckBox();
            this.rank_numAfk = new MCGalaxy.Gui.TimespanUpDown();
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
            this.rank_lblCopy = new System.Windows.Forms.Label();
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
            this.rank_lblPrefix = new System.Windows.Forms.Label();
            this.rank_lblPerm = new System.Windows.Forms.Label();
            this.rank_lblMOTD = new System.Windows.Forms.Label();
            this.rank_lblName = new System.Windows.Forms.Label();
            this.rank_lblColor = new System.Windows.Forms.Label();
            this.rank_btnDel = new System.Windows.Forms.Button();
            this.rank_btnAdd = new System.Windows.Forms.Button();
            this.rank_list = new System.Windows.Forms.ListBox();
            this.pageMisc = new System.Windows.Forms.TabPage();
            this.grpExtra = new System.Windows.Forms.GroupBox();
            this.misc_numReview = new MCGalaxy.Gui.TimespanUpDown();
            this.chkRestart = new System.Windows.Forms.CheckBox();
            this.misc_lblReview = new System.Windows.Forms.Label();
            this.chkRepeatMessages = new System.Windows.Forms.CheckBox();
            this.chk17Dollar = new System.Windows.Forms.CheckBox();
            this.chkSmile = new System.Windows.Forms.CheckBox();
            this.grpMessages = new System.Windows.Forms.GroupBox();
            this.hack_num = new MCGalaxy.Gui.TimespanUpDown();
            this.grpPhysics = new System.Windows.Forms.GroupBox();
            this.txtRP = new System.Windows.Forms.TextBox();
            this.chkRpNorm = new System.Windows.Forms.Label();
            this.txtNormRp = new System.Windows.Forms.TextBox();
            this.afk_grp = new System.Windows.Forms.GroupBox();
            this.afk_numTimer = new MCGalaxy.Gui.TimespanUpDown();
            this.afk_lblTimer = new System.Windows.Forms.Label();
            this.bak_grp = new System.Windows.Forms.GroupBox();
            this.bak_numTime = new MCGalaxy.Gui.TimespanUpDown();
            this.bak_lblLocation = new System.Windows.Forms.Label();
            this.bak_txtLocation = new System.Windows.Forms.TextBox();
            this.bak_lblTime = new System.Windows.Forms.Label();
            this.pageRelay = new System.Windows.Forms.TabPage();
            this.gb_ircSettings = new System.Windows.Forms.GroupBox();
            this.irc_txtPrefix = new System.Windows.Forms.TextBox();
            this.irc_lblPrefix = new System.Windows.Forms.Label();
            this.irc_cmbVerify = new System.Windows.Forms.ComboBox();
            this.irc_lblVerify = new System.Windows.Forms.Label();
            this.irc_cmbRank = new System.Windows.Forms.ComboBox();
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
            this.irc_numPort = new System.Windows.Forms.NumericUpDown();
            this.irc_lblNick = new System.Windows.Forms.Label();
            this.irc_lblChannel = new System.Windows.Forms.Label();
            this.irc_lblOpChannel = new System.Windows.Forms.Label();
            this.pageServer = new System.Windows.Forms.TabPage();
            this.lvl_grp = new System.Windows.Forms.GroupBox();
            this.lvl_lblMain = new System.Windows.Forms.Label();
            this.lvl_txtMain = new System.Windows.Forms.TextBox();
            this.adv_grp = new System.Windows.Forms.GroupBox();
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
            this.pageEco = new System.Windows.Forms.TabPage();
            this.eco_gbRank = new System.Windows.Forms.GroupBox();
            this.eco_dgvRanks = new System.Windows.Forms.DataGridView();
            this.eco_colRankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eco_colRankPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eco_cbRank = new System.Windows.Forms.CheckBox();
            this.eco_gbLvl = new System.Windows.Forms.GroupBox();
            this.eco_dgvMaps = new System.Windows.Forms.DataGridView();
            this.eco_colLvlName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eco_colLvlPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eco_colLvlX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eco_colLvlY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eco_colLvlZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eco_colLvlTheme = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.eco_btnLvlDel = new System.Windows.Forms.Button();
            this.eco_btnLvlAdd = new System.Windows.Forms.Button();
            this.eco_cbLvl = new System.Windows.Forms.CheckBox();
            this.eco_gbItem = new System.Windows.Forms.GroupBox();
            this.eco_lblItemRank = new System.Windows.Forms.Label();
            this.eco_numItemPrice = new System.Windows.Forms.NumericUpDown();
            this.eco_lblItemPrice = new System.Windows.Forms.Label();
            this.eco_cbItem = new System.Windows.Forms.CheckBox();
            this.eco_gb = new System.Windows.Forms.GroupBox();
            this.eco_cmbCfg = new System.Windows.Forms.ComboBox();
            this.eco_lblCfg = new System.Windows.Forms.Label();
            this.eco_cbEnabled = new System.Windows.Forms.CheckBox();
            this.eco_txtCurrency = new System.Windows.Forms.TextBox();
            this.eco_lblCurrency = new System.Windows.Forms.Label();
            this.pageGames = new System.Windows.Forms.TabPage();
            this.tabGames = new System.Windows.Forms.TabControl();
            this.tabLS = new System.Windows.Forms.TabPage();
            this.ls_grpControls = new System.Windows.Forms.GroupBox();
            this.ls_btnEnd = new System.Windows.Forms.Button();
            this.ls_btnStop = new System.Windows.Forms.Button();
            this.ls_btnStart = new System.Windows.Forms.Button();
            this.ls_grpMapSettings = new System.Windows.Forms.GroupBox();
            this.ls_grpTime = new System.Windows.Forms.GroupBox();
            this.ls_numFlood = new MCGalaxy.Gui.TimespanUpDown();
            this.ls_numLayerTime = new MCGalaxy.Gui.TimespanUpDown();
            this.ls_numRound = new MCGalaxy.Gui.TimespanUpDown();
            this.ls_lblLayerTime = new System.Windows.Forms.Label();
            this.ls_lblFlood = new System.Windows.Forms.Label();
            this.ls_lblRound = new System.Windows.Forms.Label();
            this.ls_grpLayer = new System.Windows.Forms.GroupBox();
            this.ls_lblBlocksTall = new System.Windows.Forms.Label();
            this.ls_lblLayersEach = new System.Windows.Forms.Label();
            this.ls_lblLayer = new System.Windows.Forms.Label();
            this.ls_grpBlock = new System.Windows.Forms.GroupBox();
            this.ls_lblDestroy = new System.Windows.Forms.Label();
            this.ls_lblFast = new System.Windows.Forms.Label();
            this.ls_lblWater = new System.Windows.Forms.Label();
            this.ls_lblKill = new System.Windows.Forms.Label();
            this.ls_grpSettings = new System.Windows.Forms.GroupBox();
            this.ls_lblMax = new System.Windows.Forms.Label();
            this.ls_cbMap = new System.Windows.Forms.CheckBox();
            this.ls_grpMaps = new System.Windows.Forms.GroupBox();
            this.ls_lblNotUsed = new System.Windows.Forms.Label();
            this.ls_lblUsed = new System.Windows.Forms.Label();
            this.ls_btnAdd = new System.Windows.Forms.Button();
            this.ls_btnRemove = new System.Windows.Forms.Button();
            this.ls_lstNotUsed = new System.Windows.Forms.ListBox();
            this.ls_lstUsed = new System.Windows.Forms.ListBox();
            this.tabZS = new System.Windows.Forms.TabPage();
            this.zs_grpControls = new System.Windows.Forms.GroupBox();
            this.zs_btnEnd = new System.Windows.Forms.Button();
            this.zs_btnStop = new System.Windows.Forms.Button();
            this.zs_btnStart = new System.Windows.Forms.Button();
            this.zs_grpMap = new System.Windows.Forms.GroupBox();
            this.zs_grpTime = new System.Windows.Forms.GroupBox();
            this.timespanUpDown1 = new MCGalaxy.Gui.TimespanUpDown();
            this.timespanUpDown2 = new MCGalaxy.Gui.TimespanUpDown();
            this.timespanUpDown3 = new MCGalaxy.Gui.TimespanUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.zs_grpSettings = new System.Windows.Forms.GroupBox();
            this.zs_grpZombie = new System.Windows.Forms.GroupBox();
            this.zs_txtModel = new System.Windows.Forms.TextBox();
            this.zs_txtName = new System.Windows.Forms.TextBox();
            this.zs_lblModel = new System.Windows.Forms.Label();
            this.zs_lblName = new System.Windows.Forms.Label();
            this.zs_grpRevive = new System.Windows.Forms.GroupBox();
            this.zs_lblReviveEff = new System.Windows.Forms.Label();
            this.zs_numReviveEff = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.zs_lblReviveLimitFtr = new System.Windows.Forms.Label();
            this.zs_lblReviveLimitHdr = new System.Windows.Forms.Label();
            this.zs_numReviveLimit = new System.Windows.Forms.NumericUpDown();
            this.zs_numReviveMax = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.zs_cbMain = new System.Windows.Forms.CheckBox();
            this.zs_cbMap = new System.Windows.Forms.CheckBox();
            this.zs_grpInv = new System.Windows.Forms.GroupBox();
            this.zs_numInvZombieDur = new System.Windows.Forms.NumericUpDown();
            this.zs_numInvHumanDur = new System.Windows.Forms.NumericUpDown();
            this.zs_numInvZombieMax = new System.Windows.Forms.NumericUpDown();
            this.zs_numInvHumanMax = new System.Windows.Forms.NumericUpDown();
            this.zs_lblInvZombieDur = new System.Windows.Forms.Label();
            this.zs_lblInvZombieMax = new System.Windows.Forms.Label();
            this.zs_lblInvHumanDur = new System.Windows.Forms.Label();
            this.zs_lblInvHumanMax = new System.Windows.Forms.Label();
            this.zs_cbStart = new System.Windows.Forms.CheckBox();
            this.zs_grpMaps = new System.Windows.Forms.GroupBox();
            this.zs_lblNotUsed = new System.Windows.Forms.Label();
            this.zs_lblUsed = new System.Windows.Forms.Label();
            this.zs_btnAdd = new System.Windows.Forms.Button();
            this.zs_btnRemove = new System.Windows.Forms.Button();
            this.zs_lstNotUsed = new System.Windows.Forms.ListBox();
            this.zs_lstUsed = new System.Windows.Forms.ListBox();
            this.tabZS_old = new System.Windows.Forms.TabPage();
            this.propsZG = new MCGalaxy.Gui.HackyPropertyGrid();
            this.tabCTF = new System.Windows.Forms.TabPage();
            this.ctf_grpControls = new System.Windows.Forms.GroupBox();
            this.ctf_btnEnd = new System.Windows.Forms.Button();
            this.ctf_btnStop = new System.Windows.Forms.Button();
            this.ctf_btnStart = new System.Windows.Forms.Button();
            this.ctf_grpSettings = new System.Windows.Forms.GroupBox();
            this.ctf_cbMain = new System.Windows.Forms.CheckBox();
            this.ctf_cbMap = new System.Windows.Forms.CheckBox();
            this.ctf_cbStart = new System.Windows.Forms.CheckBox();
            this.ctf_grpMaps = new System.Windows.Forms.GroupBox();
            this.ctf_lblNotUsed = new System.Windows.Forms.Label();
            this.ctf_lblUsed = new System.Windows.Forms.Label();
            this.ctf_btnAdd = new System.Windows.Forms.Button();
            this.ctf_btnRemove = new System.Windows.Forms.Button();
            this.ctf_lstNotUsed = new System.Windows.Forms.ListBox();
            this.ctf_lstUsed = new System.Windows.Forms.ListBox();
            this.tabTW = new System.Windows.Forms.TabPage();
            this.tw_grpControls = new System.Windows.Forms.GroupBox();
            this.tw_btnEnd = new System.Windows.Forms.Button();
            this.tw_btnStop = new System.Windows.Forms.Button();
            this.tw_btnStart = new System.Windows.Forms.Button();
            this.tw_grpMapSettings = new System.Windows.Forms.GroupBox();
            this.tw_grpTeams = new System.Windows.Forms.GroupBox();
            this.tw_cbKills = new System.Windows.Forms.CheckBox();
            this.tw_cbBalance = new System.Windows.Forms.CheckBox();
            this.tw_grpGrace = new System.Windows.Forms.GroupBox();
            this.tw_numGrace = new MCGalaxy.Gui.TimespanUpDown();
            this.tw_lblGrace = new System.Windows.Forms.Label();
            this.tw_cbGrace = new System.Windows.Forms.CheckBox();
            this.tw_grpScores = new System.Windows.Forms.GroupBox();
            this.tw_lblMulti = new System.Windows.Forms.Label();
            this.tw_lblAssist = new System.Windows.Forms.Label();
            this.tw_cbStreaks = new System.Windows.Forms.CheckBox();
            this.tw_numMultiKills = new System.Windows.Forms.NumericUpDown();
            this.tw_numScoreAssists = new System.Windows.Forms.NumericUpDown();
            this.tw_lblScorePerKill = new System.Windows.Forms.Label();
            this.tw_numScorePerKill = new System.Windows.Forms.NumericUpDown();
            this.tw_lblScoreLimit = new System.Windows.Forms.Label();
            this.tw_numScoreLimit = new System.Windows.Forms.NumericUpDown();
            this.tw_grpSettings = new System.Windows.Forms.GroupBox();
            this.tw_cmbMode = new System.Windows.Forms.ComboBox();
            this.tw_cmbDiff = new System.Windows.Forms.ComboBox();
            this.tw_lblMode = new System.Windows.Forms.Label();
            this.tw_lblDiff = new System.Windows.Forms.Label();
            this.tw_cbMain = new System.Windows.Forms.CheckBox();
            this.tw_cbMap = new System.Windows.Forms.CheckBox();
            this.tw_cbStart = new System.Windows.Forms.CheckBox();
            this.tw_gbMaps = new System.Windows.Forms.GroupBox();
            this.tw_lblInUse = new System.Windows.Forms.Label();
            this.tw_btnAdd = new System.Windows.Forms.Button();
            this.tw_btnRemove = new System.Windows.Forms.Button();
            this.tw_lstNotUsed = new System.Windows.Forms.ListBox();
            this.tw_lstUsed = new System.Windows.Forms.ListBox();
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
            this.sec_numChatSecs = new MCGalaxy.Gui.TimespanUpDown();
            this.sec_lblChatForMute = new System.Windows.Forms.Label();
            this.sec_numChatMute = new MCGalaxy.Gui.TimespanUpDown();
            this.sec_grpCmd = new System.Windows.Forms.GroupBox();
            this.sec_cbCmdAuto = new System.Windows.Forms.CheckBox();
            this.sec_lblCmdOnMute = new System.Windows.Forms.Label();
            this.sec_numCmdMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblCmdOnMsgs = new System.Windows.Forms.Label();
            this.sec_numCmdSecs = new MCGalaxy.Gui.TimespanUpDown();
            this.sec_lblCmdForMute = new System.Windows.Forms.Label();
            this.sec_numCmdMute = new MCGalaxy.Gui.TimespanUpDown();
            this.sec_grpIP = new System.Windows.Forms.GroupBox();
            this.sec_cbIPAuto = new System.Windows.Forms.CheckBox();
            this.sec_lblIPOnMute = new System.Windows.Forms.Label();
            this.sec_numIPMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblIPOnMsgs = new System.Windows.Forms.Label();
            this.sec_numIPSecs = new MCGalaxy.Gui.TimespanUpDown();
            this.sec_lblIPForMute = new System.Windows.Forms.Label();
            this.sec_numIPMute = new MCGalaxy.Gui.TimespanUpDown();
            this.sec_grpOther = new System.Windows.Forms.GroupBox();
            this.sec_lblRank = new System.Windows.Forms.Label();
            this.sec_grpBlocks = new System.Windows.Forms.GroupBox();
            this.sec_cbBlocksAuto = new System.Windows.Forms.CheckBox();
            this.sec_lblBlocksOnMute = new System.Windows.Forms.Label();
            this.sec_numBlocksMsgs = new System.Windows.Forms.NumericUpDown();
            this.sec_lblBlocksOnMsgs = new System.Windows.Forms.Label();
            this.sec_numBlocksSecs = new MCGalaxy.Gui.TimespanUpDown();
            this.pageChat.SuspendLayout();
            this.chat_grpTab.SuspendLayout();
            this.chat_grpMessages.SuspendLayout();
            this.chat_grpOther.SuspendLayout();
            this.chat_grpColors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srv_numPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numPerm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numMaps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numDraw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numGen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numUndo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numKiller)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numFast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numWater)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numDestroy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numLayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numHeight)).BeginInit();
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
            this.grpExtra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.misc_numReview)).BeginInit();
            this.grpMessages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hack_num)).BeginInit();
            this.grpPhysics.SuspendLayout();
            this.afk_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.afk_numTimer)).BeginInit();
            this.bak_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bak_numTime)).BeginInit();
            this.pageRelay.SuspendLayout();
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
            this.pageEco.SuspendLayout();
            this.eco_gbRank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eco_dgvRanks)).BeginInit();
            this.eco_gbLvl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eco_dgvMaps)).BeginInit();
            this.eco_gbItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eco_numItemPrice)).BeginInit();
            this.eco_gb.SuspendLayout();
            this.pageGames.SuspendLayout();
            this.tabGames.SuspendLayout();
            this.tabLS.SuspendLayout();
            this.ls_grpControls.SuspendLayout();
            this.ls_grpMapSettings.SuspendLayout();
            this.ls_grpTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numFlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numLayerTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numRound)).BeginInit();
            this.ls_grpLayer.SuspendLayout();
            this.ls_grpBlock.SuspendLayout();
            this.ls_grpSettings.SuspendLayout();
            this.ls_grpMaps.SuspendLayout();
            this.tabZS.SuspendLayout();
            this.zs_grpControls.SuspendLayout();
            this.zs_grpMap.SuspendLayout();
            this.zs_grpTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timespanUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timespanUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timespanUpDown3)).BeginInit();
            this.zs_grpSettings.SuspendLayout();
            this.zs_grpZombie.SuspendLayout();
            this.zs_grpRevive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numReviveEff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numReviveLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numReviveMax)).BeginInit();
            this.zs_grpInv.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numInvZombieDur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numInvHumanDur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numInvZombieMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numInvHumanMax)).BeginInit();
            this.zs_grpMaps.SuspendLayout();
            this.tabZS_old.SuspendLayout();
            this.tabCTF.SuspendLayout();
            this.ctf_grpControls.SuspendLayout();
            this.ctf_grpSettings.SuspendLayout();
            this.ctf_grpMaps.SuspendLayout();
            this.tabTW.SuspendLayout();
            this.tw_grpControls.SuspendLayout();
            this.tw_grpMapSettings.SuspendLayout();
            this.tw_grpTeams.SuspendLayout();
            this.tw_grpGrace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numGrace)).BeginInit();
            this.tw_grpScores.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numMultiKills)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScoreAssists)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScorePerKill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScoreLimit)).BeginInit();
            this.tw_grpSettings.SuspendLayout();
            this.tw_gbMaps.SuspendLayout();
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
            this.chat_grpTab.Location = new System.Drawing.Point(235, 88);
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
            this.chat_grpMessages.Location = new System.Drawing.Point(8, 186);
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
            this.chat_txtShutdown.BackColor = System.Drawing.SystemColors.Window;
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
            this.chat_txtCheap.BackColor = System.Drawing.SystemColors.Window;
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
            this.chat_txtBan.BackColor = System.Drawing.SystemColors.Window;
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
            this.chat_txtPromote.BackColor = System.Drawing.SystemColors.Window;
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
            this.chat_txtDemote.BackColor = System.Drawing.SystemColors.Window;
            this.chat_txtDemote.Location = new System.Drawing.Point(134, 144);
            this.chat_txtDemote.MaxLength = 64;
            this.chat_txtDemote.Name = "chat_txtDemote";
            this.chat_txtDemote.Size = new System.Drawing.Size(343, 21);
            this.chat_txtDemote.TabIndex = 38;
            // 
            // chat_grpOther
            // 
            this.chat_grpOther.Controls.Add(this.chat_chkFilter);
            this.chat_grpOther.Controls.Add(this.chat_lblConsole);
            this.chat_grpOther.Controls.Add(this.chat_txtConsole);
            this.chat_grpOther.Location = new System.Drawing.Point(235, 6);
            this.chat_grpOther.Name = "chat_grpOther";
            this.chat_grpOther.Size = new System.Drawing.Size(256, 76);
            this.chat_grpOther.TabIndex = 1;
            this.chat_grpOther.TabStop = false;
            this.chat_grpOther.Text = "Other";
            // 
            // chat_chkFilter
            // 
            this.chat_chkFilter.AutoSize = true;
            this.chat_chkFilter.Location = new System.Drawing.Point(6, 49);
            this.chat_chkFilter.Name = "chat_chkFilter";
            this.chat_chkFilter.Size = new System.Drawing.Size(96, 17);
            this.chat_chkFilter.TabIndex = 31;
            this.chat_chkFilter.Text = "Profanity Filter";
            this.chat_chkFilter.UseVisualStyleBackColor = true;
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
            this.chat_txtConsole.BackColor = System.Drawing.SystemColors.Window;
            this.chat_txtConsole.Location = new System.Drawing.Point(89, 17);
            this.chat_txtConsole.Name = "chat_txtConsole";
            this.chat_txtConsole.Size = new System.Drawing.Size(161, 21);
            this.chat_txtConsole.TabIndex = 3;
            // 
            // chat_grpColors
            // 
            this.chat_grpColors.Controls.Add(this.chat_lblWarn);
            this.chat_grpColors.Controls.Add(this.chat_btnWarn);
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
            this.chat_grpColors.Size = new System.Drawing.Size(221, 174);
            this.chat_grpColors.TabIndex = 0;
            this.chat_grpColors.TabStop = false;
            this.chat_grpColors.Text = "Colors";
            // 
            // chat_lblWarn
            // 
            this.chat_lblWarn.AutoSize = true;
            this.chat_lblWarn.Location = new System.Drawing.Point(20, 141);
            this.chat_lblWarn.Name = "chat_lblWarn";
            this.chat_lblWarn.Size = new System.Drawing.Size(88, 13);
            this.chat_lblWarn.TabIndex = 35;
            this.chat_lblWarn.Text = "Warnings/errors:";
            // 
            // chat_btnWarn
            // 
            this.chat_btnWarn.Location = new System.Drawing.Point(113, 136);
            this.chat_btnWarn.Name = "chat_btnWarn";
            this.chat_btnWarn.Size = new System.Drawing.Size(95, 23);
            this.chat_btnWarn.TabIndex = 34;
            this.toolTip.SetToolTip(this.chat_btnWarn, "The color of warning/error messages produced by commands");
            this.chat_btnWarn.Click += new System.EventHandler(this.chat_btnWarn_Click);
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
            this.toolTip.SetToolTip(this.chat_btnDefault, "The default color of server messages (excluding player chat).\nFor example, when y" +
                        "ou are asked to select two corners in a cuboid.");
            this.chat_btnDefault.Click += new System.EventHandler(this.chat_cmbDefault_Click);
            // 
            // chat_lblIRC
            // 
            this.chat_lblIRC.AutoSize = true;
            this.chat_lblIRC.Location = new System.Drawing.Point(36, 54);
            this.chat_lblIRC.Name = "chat_lblIRC";
            this.chat_lblIRC.Size = new System.Drawing.Size(74, 13);
            this.chat_lblIRC.TabIndex = 22;
            this.chat_lblIRC.Text = "IRC messages:";
            // 
            // chat_btnIRC
            // 
            this.chat_btnIRC.Location = new System.Drawing.Point(113, 49);
            this.chat_btnIRC.Name = "chat_btnIRC";
            this.chat_btnIRC.Size = new System.Drawing.Size(95, 23);
            this.chat_btnIRC.TabIndex = 24;
            this.toolTip.SetToolTip(this.chat_btnIRC, "The color of messages from IRC, and nicknames of IRC users.");
            this.chat_btnIRC.Click += new System.EventHandler(this.chat_btnIRC_Click);
            // 
            // chat_lblSyntax
            // 
            this.chat_lblSyntax.AutoSize = true;
            this.chat_lblSyntax.Location = new System.Drawing.Point(41, 83);
            this.chat_lblSyntax.Name = "chat_lblSyntax";
            this.chat_lblSyntax.Size = new System.Drawing.Size(68, 13);
            this.chat_lblSyntax.TabIndex = 31;
            this.chat_lblSyntax.Text = "/help syntax:";
            // 
            // chat_btnSyntax
            // 
            this.chat_btnSyntax.Location = new System.Drawing.Point(113, 78);
            this.chat_btnSyntax.Name = "chat_btnSyntax";
            this.chat_btnSyntax.Size = new System.Drawing.Size(95, 23);
            this.chat_btnSyntax.TabIndex = 30;
            this.toolTip.SetToolTip(this.chat_btnSyntax, "The color of the /cmdname [args] in /help.");
            this.chat_btnSyntax.Click += new System.EventHandler(this.chat_btnSyntax_Click);
            // 
            // chat_lblDesc
            // 
            this.chat_lblDesc.AutoSize = true;
            this.chat_lblDesc.Location = new System.Drawing.Point(19, 112);
            this.chat_lblDesc.Name = "chat_lblDesc";
            this.chat_lblDesc.Size = new System.Drawing.Size(90, 13);
            this.chat_lblDesc.TabIndex = 32;
            this.chat_lblDesc.Text = "/help description:";
            // 
            // chat_btnDesc
            // 
            this.chat_btnDesc.Location = new System.Drawing.Point(113, 107);
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
            // chkRpLimit
            // 
            this.chkRpLimit.AutoSize = true;
            this.chkRpLimit.Location = new System.Drawing.Point(5, 52);
            this.chkRpLimit.Name = "chkRpLimit";
            this.chkRpLimit.Size = new System.Drawing.Size(48, 13);
            this.chkRpLimit.TabIndex = 15;
            this.chkRpLimit.Text = "/rp limit:";
            this.toolTip.SetToolTip(this.chkRpLimit, "Limit for custom physics set by /rp");
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
            // hack_lbl
            // 
            this.hack_lbl.AutoSize = true;
            this.hack_lbl.Location = new System.Drawing.Point(7, 20);
            this.hack_lbl.Name = "hack_lbl";
            this.hack_lbl.Size = new System.Drawing.Size(193, 17);
            this.hack_lbl.TabIndex = 32;
            this.hack_lbl.Text = "Kick people who use hackrank after ";
            this.toolTip.SetToolTip(this.hack_lbl, "Hackrank kicks people? Or not?");
            this.hack_lbl.UseVisualStyleBackColor = true;
            // 
            // sec_cmbVerifyRank
            // 
            this.sec_cmbVerifyRank.BackColor = System.Drawing.SystemColors.Window;
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
            this.rank_cmbDefault.BackColor = System.Drawing.SystemColors.Window;
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
            this.irc_txtServer.BackColor = System.Drawing.SystemColors.Window;
            this.irc_txtServer.Location = new System.Drawing.Point(82, 47);
            this.irc_txtServer.Name = "irc_txtServer";
            this.irc_txtServer.Size = new System.Drawing.Size(106, 21);
            this.irc_txtServer.TabIndex = 15;
            this.toolTip.SetToolTip(this.irc_txtServer, "IRC server hostname.\nDefault = irc.esper.net\nAnother choice = irc.geekshed.net");
            // 
            // irc_txtNick
            // 
            this.irc_txtNick.BackColor = System.Drawing.SystemColors.Window;
            this.irc_txtNick.Location = new System.Drawing.Point(82, 101);
            this.irc_txtNick.Name = "irc_txtNick";
            this.irc_txtNick.Size = new System.Drawing.Size(106, 21);
            this.irc_txtNick.TabIndex = 16;
            this.toolTip.SetToolTip(this.irc_txtNick, "The Nick that the IRC bot will try and use.");
            // 
            // irc_txtChannel
            // 
            this.irc_txtChannel.BackColor = System.Drawing.SystemColors.Window;
            this.irc_txtChannel.Location = new System.Drawing.Point(82, 128);
            this.irc_txtChannel.Name = "irc_txtChannel";
            this.irc_txtChannel.Size = new System.Drawing.Size(106, 21);
            this.irc_txtChannel.TabIndex = 17;
            this.toolTip.SetToolTip(this.irc_txtChannel, "The IRC channel to be used.");
            // 
            // irc_txtOpChannel
            // 
            this.irc_txtOpChannel.BackColor = System.Drawing.SystemColors.Window;
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
            this.adv_chkVerify.CheckedChanged += new System.EventHandler(this.chkVerify_CheckedChanged);
            // 
            // srv_txtName
            // 
            this.srv_txtName.BackColor = System.Drawing.SystemColors.Window;
            this.srv_txtName.Location = new System.Drawing.Point(83, 19);
            this.srv_txtName.MaxLength = 64;
            this.srv_txtName.Name = "srv_txtName";
            this.srv_txtName.Size = new System.Drawing.Size(387, 21);
            this.srv_txtName.TabIndex = 0;
            this.toolTip.SetToolTip(this.srv_txtName, "The name of the server.\nPick something good!");
            // 
            // srv_txtMOTD
            // 
            this.srv_txtMOTD.BackColor = System.Drawing.SystemColors.Window;
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
            this.srv_numPort.BackColor = System.Drawing.SystemColors.Window;
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
            this.toolTip.SetToolTip(this.rank_cbSilentAdmins, "Players who can read adminchat also join the server silently");
            this.rank_cbSilentAdmins.UseVisualStyleBackColor = true;
            // 
            // rank_txtPrefix
            // 
            this.rank_txtPrefix.BackColor = System.Drawing.SystemColors.Window;
            this.rank_txtPrefix.Location = new System.Drawing.Point(259, 47);
            this.rank_txtPrefix.Name = "rank_txtPrefix";
            this.rank_txtPrefix.Size = new System.Drawing.Size(81, 21);
            this.rank_txtPrefix.TabIndex = 21;
            this.toolTip.SetToolTip(this.rank_txtPrefix, "Short prefix showed before player names in chat.");
            this.rank_txtPrefix.TextChanged += new System.EventHandler(this.rank_txtPrefix_TextChanged);
            // 
            // rank_txtMOTD
            // 
            this.rank_txtMOTD.BackColor = System.Drawing.SystemColors.Window;
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
            this.rank_numPerm.BackColor = System.Drawing.SystemColors.Window;
            this.rank_numPerm.Location = new System.Drawing.Point(259, 20);
            this.rank_numPerm.Maximum = new decimal(new int[] {
                                    120,
                                    0,
                                    0,
                                    0});
            this.rank_numPerm.Minimum = new decimal(new int[] {
                                    20,
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
            this.rank_txtName.BackColor = System.Drawing.SystemColors.Window;
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
            this.rank_cmbOsMap.BackColor = System.Drawing.SystemColors.Window;
            this.rank_cmbOsMap.FormattingEnabled = true;
            this.rank_cmbOsMap.Location = new System.Drawing.Point(259, 20);
            this.rank_cmbOsMap.Name = "rank_cmbOsMap";
            this.rank_cmbOsMap.Size = new System.Drawing.Size(80, 21);
            this.rank_cmbOsMap.TabIndex = 49;
            this.toolTip.SetToolTip(this.rank_cmbOsMap, "Default minimum rank required to build on maps made with /os map add.");
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
            this.irc_txtPass.BackColor = System.Drawing.SystemColors.Window;
            this.irc_txtPass.Location = new System.Drawing.Point(82, 182);
            this.irc_txtPass.Name = "irc_txtPass";
            this.irc_txtPass.PasswordChar = '*';
            this.irc_txtPass.Size = new System.Drawing.Size(106, 21);
            this.irc_txtPass.TabIndex = 28;
            this.toolTip.SetToolTip(this.irc_txtPass, "NickServ password set for the username");
            // 
            // rank_numMaps
            // 
            this.rank_numMaps.BackColor = System.Drawing.SystemColors.Window;
            this.rank_numMaps.Location = new System.Drawing.Point(259, 20);
            this.rank_numMaps.Maximum = new decimal(new int[] {
                                    2147483647,
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
            this.rank_numDraw.BackColor = System.Drawing.SystemColors.Window;
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
            // rank_numGen
            // 
            this.rank_numGen.BackColor = System.Drawing.SystemColors.Window;
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
            // rank_numCopy
            // 
            this.rank_numCopy.BackColor = System.Drawing.SystemColors.Window;
            this.rank_numCopy.Location = new System.Drawing.Point(85, 74);
            this.rank_numCopy.Maximum = new decimal(new int[] {
                                    255,
                                    0,
                                    0,
                                    0});
            this.rank_numCopy.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.rank_numCopy.Name = "rank_numCopy";
            this.rank_numCopy.Size = new System.Drawing.Size(81, 21);
            this.rank_numCopy.TabIndex = 23;
            this.toolTip.SetToolTip(this.rank_numCopy, "Maximum number of copies player can select in /copyslot");
            this.rank_numCopy.Value = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.rank_numCopy.ValueChanged += new System.EventHandler(this.rank_numCopy_ValueChanged);
            // 
            // adv_chkCPE
            // 
            this.adv_chkCPE.AutoSize = true;
            this.adv_chkCPE.Location = new System.Drawing.Point(9, 43);
            this.adv_chkCPE.Name = "adv_chkCPE";
            this.adv_chkCPE.Size = new System.Drawing.Size(122, 17);
            this.adv_chkCPE.TabIndex = 4;
            this.adv_chkCPE.Text = "Non-classic features";
            this.toolTip.SetToolTip(this.adv_chkCPE, "Enables custom blocks, multiline chat, changing env settings, etc");
            this.adv_chkCPE.UseVisualStyleBackColor = true;
            // 
            // eco_cmbItemRank
            // 
            this.eco_cmbItemRank.BackColor = System.Drawing.SystemColors.Window;
            this.eco_cmbItemRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.eco_cmbItemRank.FormattingEnabled = true;
            this.eco_cmbItemRank.Location = new System.Drawing.Point(368, 43);
            this.eco_cmbItemRank.Name = "eco_cmbItemRank";
            this.eco_cmbItemRank.Size = new System.Drawing.Size(110, 21);
            this.eco_cmbItemRank.TabIndex = 23;
            this.toolTip.SetToolTip(this.eco_cmbItemRank, "Minimum rank a player must have to purchase this item.");
            this.eco_cmbItemRank.SelectedIndexChanged += new System.EventHandler(this.eco_cmbItemRank_SelectedIndexChanged);
            // 
            // rank_numUndo
            // 
            this.rank_numUndo.BackColor = System.Drawing.SystemColors.Window;
            this.rank_numUndo.Location = new System.Drawing.Point(85, 47);
            this.rank_numUndo.Name = "rank_numUndo";
            this.rank_numUndo.Seconds = ((long)(0));
            this.rank_numUndo.Size = new System.Drawing.Size(81, 21);
            this.rank_numUndo.TabIndex = 24;
            this.rank_numUndo.Text = "0s";
            this.toolTip.SetToolTip(this.rank_numUndo, "Maximum time players can undo up to in the past with /undo");
            this.rank_numUndo.Value = System.TimeSpan.Parse("00:00:00");
            this.rank_numUndo.ValueChanged += new System.EventHandler(this.rank_numUndo_ValueChanged);
            // 
            // chkPhysRestart
            // 
            this.chkPhysRestart.AutoSize = true;
            this.chkPhysRestart.Location = new System.Drawing.Point(6, 20);
            this.chkPhysRestart.Name = "chkPhysRestart";
            this.chkPhysRestart.Size = new System.Drawing.Size(124, 17);
            this.chkPhysRestart.TabIndex = 52;
            this.chkPhysRestart.Text = "Restart on shutdown";
            this.chkPhysRestart.UseVisualStyleBackColor = true;
            // 
            // ls_numMax
            // 
            this.ls_numMax.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numMax.Location = new System.Drawing.Point(71, 89);
            this.ls_numMax.Maximum = new decimal(new int[] {
                                    1000000,
                                    0,
                                    0,
                                    0});
            this.ls_numMax.Name = "ls_numMax";
            this.ls_numMax.Size = new System.Drawing.Size(52, 21);
            this.ls_numMax.TabIndex = 25;
            this.ls_numMax.Value = new decimal(new int[] {
                                    3,
                                    0,
                                    0,
                                    0});
            // 
            // ls_numKiller
            // 
            this.ls_numKiller.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numKiller.Location = new System.Drawing.Point(79, 20);
            this.ls_numKiller.Name = "ls_numKiller";
            this.ls_numKiller.Size = new System.Drawing.Size(52, 21);
            this.ls_numKiller.TabIndex = 27;
            this.ls_numKiller.Value = new decimal(new int[] {
                                    100,
                                    0,
                                    0,
                                    0});
            // 
            // ls_numFast
            // 
            this.ls_numFast.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numFast.Location = new System.Drawing.Point(79, 45);
            this.ls_numFast.Name = "ls_numFast";
            this.ls_numFast.Size = new System.Drawing.Size(52, 21);
            this.ls_numFast.TabIndex = 31;
            // 
            // ls_numWater
            // 
            this.ls_numWater.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numWater.Location = new System.Drawing.Point(226, 20);
            this.ls_numWater.Name = "ls_numWater";
            this.ls_numWater.Size = new System.Drawing.Size(52, 21);
            this.ls_numWater.TabIndex = 32;
            // 
            // ls_numDestroy
            // 
            this.ls_numDestroy.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numDestroy.Location = new System.Drawing.Point(226, 45);
            this.ls_numDestroy.Name = "ls_numDestroy";
            this.ls_numDestroy.Size = new System.Drawing.Size(52, 21);
            this.ls_numDestroy.TabIndex = 33;
            // 
            // ls_numLayer
            // 
            this.ls_numLayer.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numLayer.Location = new System.Drawing.Point(128, 16);
            this.ls_numLayer.Name = "ls_numLayer";
            this.ls_numLayer.Size = new System.Drawing.Size(52, 21);
            this.ls_numLayer.TabIndex = 34;
            // 
            // ls_numCount
            // 
            this.ls_numCount.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numCount.Location = new System.Drawing.Point(7, 44);
            this.ls_numCount.Maximum = new decimal(new int[] {
                                    1000000,
                                    0,
                                    0,
                                    0});
            this.ls_numCount.Name = "ls_numCount";
            this.ls_numCount.Size = new System.Drawing.Size(52, 21);
            this.ls_numCount.TabIndex = 35;
            this.ls_numCount.Value = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            // 
            // ls_numHeight
            // 
            this.ls_numHeight.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numHeight.Location = new System.Drawing.Point(128, 44);
            this.ls_numHeight.Maximum = new decimal(new int[] {
                                    1000000,
                                    0,
                                    0,
                                    0});
            this.ls_numHeight.Name = "ls_numHeight";
            this.ls_numHeight.Size = new System.Drawing.Size(52, 21);
            this.ls_numHeight.TabIndex = 37;
            this.ls_numHeight.Value = new decimal(new int[] {
                                    3,
                                    0,
                                    0,
                                    0});
            // 
            // ls_cbMain
            // 
            this.ls_cbMain.AutoSize = true;
            this.ls_cbMain.Location = new System.Drawing.Point(11, 66);
            this.ls_cbMain.Name = "ls_cbMain";
            this.ls_cbMain.Size = new System.Drawing.Size(112, 17);
            this.ls_cbMain.TabIndex = 24;
            this.ls_cbMain.Text = "Change main level";
            this.ls_cbMain.UseVisualStyleBackColor = true;
            // 
            // ls_cbStart
            // 
            this.ls_cbStart.AutoSize = true;
            this.ls_cbStart.Location = new System.Drawing.Point(11, 20);
            this.ls_cbStart.Name = "ls_cbStart";
            this.ls_cbStart.Size = new System.Drawing.Size(139, 17);
            this.ls_cbStart.TabIndex = 22;
            this.ls_cbStart.Text = "Start when server starts";
            this.ls_cbStart.UseVisualStyleBackColor = true;
            // 
            // rank_numAfk
            // 
            this.rank_numAfk.BackColor = System.Drawing.SystemColors.Window;
            this.rank_numAfk.Location = new System.Drawing.Point(113, 102);
            this.rank_numAfk.Name = "rank_numAfk";
            this.rank_numAfk.Seconds = ((long)(0));
            this.rank_numAfk.Size = new System.Drawing.Size(62, 21);
            this.rank_numAfk.TabIndex = 23;
            this.rank_numAfk.Text = "0s";
            this.rank_numAfk.Value = System.TimeSpan.Parse("00:00:00");
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
            this.blk_txtDeath.BackColor = System.Drawing.SystemColors.Window;
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
            this.blk_cmbAlw3.BackColor = System.Drawing.SystemColors.Window;
            this.blk_cmbAlw3.FormattingEnabled = true;
            this.blk_cmbAlw3.Location = new System.Drawing.Point(274, 67);
            this.blk_cmbAlw3.Name = "blk_cmbAlw3";
            this.blk_cmbAlw3.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbAlw3.TabIndex = 28;
            this.blk_cmbAlw3.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbAlw2
            // 
            this.blk_cmbAlw2.BackColor = System.Drawing.SystemColors.Window;
            this.blk_cmbAlw2.FormattingEnabled = true;
            this.blk_cmbAlw2.Location = new System.Drawing.Point(187, 67);
            this.blk_cmbAlw2.Name = "blk_cmbAlw2";
            this.blk_cmbAlw2.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbAlw2.TabIndex = 27;
            this.blk_cmbAlw2.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbDis3
            // 
            this.blk_cmbDis3.BackColor = System.Drawing.SystemColors.Window;
            this.blk_cmbDis3.FormattingEnabled = true;
            this.blk_cmbDis3.Location = new System.Drawing.Point(274, 41);
            this.blk_cmbDis3.Name = "blk_cmbDis3";
            this.blk_cmbDis3.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbDis3.TabIndex = 26;
            this.blk_cmbDis3.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbDis2
            // 
            this.blk_cmbDis2.BackColor = System.Drawing.SystemColors.Window;
            this.blk_cmbDis2.FormattingEnabled = true;
            this.blk_cmbDis2.Location = new System.Drawing.Point(187, 41);
            this.blk_cmbDis2.Name = "blk_cmbDis2";
            this.blk_cmbDis2.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbDis2.TabIndex = 25;
            this.blk_cmbDis2.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbAlw1
            // 
            this.blk_cmbAlw1.BackColor = System.Drawing.SystemColors.Window;
            this.blk_cmbAlw1.FormattingEnabled = true;
            this.blk_cmbAlw1.Location = new System.Drawing.Point(100, 67);
            this.blk_cmbAlw1.Name = "blk_cmbAlw1";
            this.blk_cmbAlw1.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbAlw1.TabIndex = 24;
            this.blk_cmbAlw1.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbDis1
            // 
            this.blk_cmbDis1.BackColor = System.Drawing.SystemColors.Window;
            this.blk_cmbDis1.FormattingEnabled = true;
            this.blk_cmbDis1.Location = new System.Drawing.Point(100, 41);
            this.blk_cmbDis1.Name = "blk_cmbDis1";
            this.blk_cmbDis1.Size = new System.Drawing.Size(81, 21);
            this.blk_cmbDis1.TabIndex = 23;
            this.blk_cmbDis1.SelectedIndexChanged += new System.EventHandler(this.blk_cmbSpecific_SelectedIndexChanged);
            // 
            // blk_cmbMin
            // 
            this.blk_cmbMin.BackColor = System.Drawing.SystemColors.Window;
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
            this.blk_list.BackColor = System.Drawing.SystemColors.Window;
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
            this.rank_grpLimits.Controls.Add(this.rank_numUndo);
            this.rank_grpLimits.Controls.Add(this.rank_numCopy);
            this.rank_grpLimits.Controls.Add(this.rank_lblCopy);
            this.rank_grpLimits.Controls.Add(this.rank_lblGen);
            this.rank_grpLimits.Controls.Add(this.rank_numGen);
            this.rank_grpLimits.Controls.Add(this.rank_lblMaps);
            this.rank_grpLimits.Controls.Add(this.rank_numMaps);
            this.rank_grpLimits.Controls.Add(this.rank_numDraw);
            this.rank_grpLimits.Controls.Add(this.rank_lblDraw);
            this.rank_grpLimits.Controls.Add(this.rank_lblUndo);
            this.rank_grpLimits.Location = new System.Drawing.Point(142, 143);
            this.rank_grpLimits.Name = "rank_grpLimits";
            this.rank_grpLimits.Size = new System.Drawing.Size(349, 106);
            this.rank_grpLimits.TabIndex = 22;
            this.rank_grpLimits.TabStop = false;
            this.rank_grpLimits.Text = "Rank limits";
            // 
            // rank_lblCopy
            // 
            this.rank_lblCopy.AutoSize = true;
            this.rank_lblCopy.Location = new System.Drawing.Point(18, 77);
            this.rank_lblCopy.Name = "rank_lblCopy";
            this.rank_lblCopy.Size = new System.Drawing.Size(61, 13);
            this.rank_lblCopy.TabIndex = 22;
            this.rank_lblCopy.Text = "/copy slots:";
            this.rank_lblCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.rank_grpGeneral.Location = new System.Drawing.Point(142, 255);
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
            this.rank_btnDel.Location = new System.Drawing.Point(79, 353);
            this.rank_btnDel.Name = "rank_btnDel";
            this.rank_btnDel.Size = new System.Drawing.Size(57, 23);
            this.rank_btnDel.TabIndex = 2;
            this.rank_btnDel.Text = "Delete";
            this.rank_btnDel.UseVisualStyleBackColor = true;
            this.rank_btnDel.Click += new System.EventHandler(this.rank_btnDel_Click);
            // 
            // rank_btnAdd
            // 
            this.rank_btnAdd.Location = new System.Drawing.Point(6, 353);
            this.rank_btnAdd.Name = "rank_btnAdd";
            this.rank_btnAdd.Size = new System.Drawing.Size(57, 23);
            this.rank_btnAdd.TabIndex = 1;
            this.rank_btnAdd.Text = "Add";
            this.rank_btnAdd.UseVisualStyleBackColor = true;
            this.rank_btnAdd.Click += new System.EventHandler(this.rank_btnAdd_Click);
            // 
            // rank_list
            // 
            this.rank_list.BackColor = System.Drawing.SystemColors.Window;
            this.rank_list.FormattingEnabled = true;
            this.rank_list.Location = new System.Drawing.Point(6, 6);
            this.rank_list.Name = "rank_list";
            this.rank_list.Size = new System.Drawing.Size(130, 342);
            this.rank_list.TabIndex = 0;
            this.rank_list.SelectedIndexChanged += new System.EventHandler(this.rank_list_SelectedIndexChanged);
            // 
            // pageMisc
            // 
            this.pageMisc.BackColor = System.Drawing.SystemColors.Control;
            this.pageMisc.Controls.Add(this.grpExtra);
            this.pageMisc.Controls.Add(this.grpMessages);
            this.pageMisc.Controls.Add(this.grpPhysics);
            this.pageMisc.Controls.Add(this.afk_grp);
            this.pageMisc.Controls.Add(this.bak_grp);
            this.pageMisc.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageMisc.Location = new System.Drawing.Point(4, 22);
            this.pageMisc.Name = "pageMisc";
            this.pageMisc.Size = new System.Drawing.Size(498, 521);
            this.pageMisc.TabIndex = 3;
            this.pageMisc.Text = "Misc";
            // 
            // grpExtra
            // 
            this.grpExtra.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpExtra.Controls.Add(this.misc_numReview);
            this.grpExtra.Controls.Add(this.chkRestart);
            this.grpExtra.Controls.Add(this.misc_lblReview);
            this.grpExtra.Controls.Add(this.chkGuestLimitNotify);
            this.grpExtra.Controls.Add(this.chkRepeatMessages);
            this.grpExtra.Controls.Add(this.chkDeath);
            this.grpExtra.Controls.Add(this.chk17Dollar);
            this.grpExtra.Controls.Add(this.chkSmile);
            this.grpExtra.Location = new System.Drawing.Point(10, 158);
            this.grpExtra.Name = "grpExtra";
            this.grpExtra.Size = new System.Drawing.Size(332, 124);
            this.grpExtra.TabIndex = 40;
            this.grpExtra.TabStop = false;
            this.grpExtra.Text = "Extra";
            // 
            // misc_numReview
            // 
            this.misc_numReview.BackColor = System.Drawing.SystemColors.Window;
            this.misc_numReview.Location = new System.Drawing.Point(126, 89);
            this.misc_numReview.Name = "misc_numReview";
            this.misc_numReview.Seconds = ((long)(300));
            this.misc_numReview.Size = new System.Drawing.Size(66, 21);
            this.misc_numReview.TabIndex = 35;
            this.misc_numReview.Text = "5m";
            this.misc_numReview.Value = System.TimeSpan.Parse("00:05:00");
            // 
            // chkRestart
            // 
            this.chkRestart.AutoSize = true;
            this.chkRestart.Location = new System.Drawing.Point(190, 20);
            this.chkRestart.Name = "chkRestart";
            this.chkRestart.Size = new System.Drawing.Size(101, 17);
            this.chkRestart.TabIndex = 51;
            this.chkRestart.Text = "Restart on error";
            this.chkRestart.UseVisualStyleBackColor = true;
            // 
            // misc_lblReview
            // 
            this.misc_lblReview.AutoSize = true;
            this.misc_lblReview.Location = new System.Drawing.Point(6, 91);
            this.misc_lblReview.Name = "misc_lblReview";
            this.misc_lblReview.Size = new System.Drawing.Size(115, 13);
            this.misc_lblReview.TabIndex = 49;
            this.misc_lblReview.Text = "Review cooldown time:";
            // 
            // chkRepeatMessages
            // 
            this.chkRepeatMessages.AutoSize = true;
            this.chkRepeatMessages.Location = new System.Drawing.Point(190, 43);
            this.chkRepeatMessages.Name = "chkRepeatMessages";
            this.chkRepeatMessages.Size = new System.Drawing.Size(136, 17);
            this.chkRepeatMessages.TabIndex = 29;
            this.chkRepeatMessages.Text = "Repeat message blocks";
            this.chkRepeatMessages.UseVisualStyleBackColor = true;
            // 
            // chk17Dollar
            // 
            this.chk17Dollar.AutoSize = true;
            this.chk17Dollar.Location = new System.Drawing.Point(6, 66);
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
            // grpMessages
            // 
            this.grpMessages.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpMessages.Controls.Add(this.hack_num);
            this.grpMessages.Controls.Add(this.hack_lbl);
            this.grpMessages.Location = new System.Drawing.Point(10, 103);
            this.grpMessages.Name = "grpMessages";
            this.grpMessages.Size = new System.Drawing.Size(332, 49);
            this.grpMessages.TabIndex = 39;
            this.grpMessages.TabStop = false;
            this.grpMessages.Text = "Messages";
            // 
            // hack_num
            // 
            this.hack_num.BackColor = System.Drawing.SystemColors.Window;
            this.hack_num.Location = new System.Drawing.Point(201, 18);
            this.hack_num.Name = "hack_num";
            this.hack_num.Seconds = ((long)(5));
            this.hack_num.Size = new System.Drawing.Size(66, 21);
            this.hack_num.TabIndex = 34;
            this.hack_num.Text = "5s";
            this.hack_num.Value = System.TimeSpan.Parse("00:00:05");
            // 
            // grpPhysics
            // 
            this.grpPhysics.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpPhysics.Controls.Add(this.chkPhysRestart);
            this.grpPhysics.Controls.Add(this.chkRpLimit);
            this.grpPhysics.Controls.Add(this.txtRP);
            this.grpPhysics.Controls.Add(this.chkRpNorm);
            this.grpPhysics.Controls.Add(this.txtNormRp);
            this.grpPhysics.Location = new System.Drawing.Point(352, 124);
            this.grpPhysics.Name = "grpPhysics";
            this.grpPhysics.Size = new System.Drawing.Size(133, 117);
            this.grpPhysics.TabIndex = 38;
            this.grpPhysics.TabStop = false;
            this.grpPhysics.Text = "Physics Restart";
            // 
            // txtRP
            // 
            this.txtRP.BackColor = System.Drawing.SystemColors.Window;
            this.txtRP.Location = new System.Drawing.Point(72, 49);
            this.txtRP.Name = "txtRP";
            this.txtRP.Size = new System.Drawing.Size(55, 21);
            this.txtRP.TabIndex = 14;
            // 
            // chkRpNorm
            // 
            this.chkRpNorm.AutoSize = true;
            this.chkRpNorm.Location = new System.Drawing.Point(5, 79);
            this.chkRpNorm.Name = "chkRpNorm";
            this.chkRpNorm.Size = new System.Drawing.Size(61, 13);
            this.chkRpNorm.TabIndex = 16;
            this.chkRpNorm.Text = "Normal /rp:";
            // 
            // txtNormRp
            // 
            this.txtNormRp.BackColor = System.Drawing.SystemColors.Window;
            this.txtNormRp.Location = new System.Drawing.Point(72, 76);
            this.txtNormRp.Name = "txtNormRp";
            this.txtNormRp.Size = new System.Drawing.Size(55, 21);
            this.txtNormRp.TabIndex = 13;
            // 
            // afk_grp
            // 
            this.afk_grp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.afk_grp.Controls.Add(this.afk_numTimer);
            this.afk_grp.Controls.Add(this.afk_lblTimer);
            this.afk_grp.Location = new System.Drawing.Point(352, 13);
            this.afk_grp.Name = "afk_grp";
            this.afk_grp.Size = new System.Drawing.Size(133, 100);
            this.afk_grp.TabIndex = 37;
            this.afk_grp.TabStop = false;
            this.afk_grp.Text = "AFK";
            // 
            // afk_numTimer
            // 
            this.afk_numTimer.BackColor = System.Drawing.SystemColors.Window;
            this.afk_numTimer.Location = new System.Drawing.Point(61, 16);
            this.afk_numTimer.Name = "afk_numTimer";
            this.afk_numTimer.Seconds = ((long)(600));
            this.afk_numTimer.Size = new System.Drawing.Size(66, 21);
            this.afk_numTimer.TabIndex = 33;
            this.afk_numTimer.Text = "10m";
            this.afk_numTimer.Value = System.TimeSpan.Parse("00:10:00");
            // 
            // afk_lblTimer
            // 
            this.afk_lblTimer.AutoSize = true;
            this.afk_lblTimer.Location = new System.Drawing.Point(5, 20);
            this.afk_lblTimer.Name = "afk_lblTimer";
            this.afk_lblTimer.Size = new System.Drawing.Size(54, 13);
            this.afk_lblTimer.TabIndex = 12;
            this.afk_lblTimer.Text = "AFK timer:";
            // 
            // bak_grp
            // 
            this.bak_grp.AutoSize = true;
            this.bak_grp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bak_grp.Controls.Add(this.bak_numTime);
            this.bak_grp.Controls.Add(this.bak_lblLocation);
            this.bak_grp.Controls.Add(this.bak_txtLocation);
            this.bak_grp.Controls.Add(this.bak_lblTime);
            this.bak_grp.Location = new System.Drawing.Point(10, 13);
            this.bak_grp.Name = "bak_grp";
            this.bak_grp.Size = new System.Drawing.Size(332, 84);
            this.bak_grp.TabIndex = 36;
            this.bak_grp.TabStop = false;
            this.bak_grp.Text = "Backups";
            // 
            // bak_numTime
            // 
            this.bak_numTime.BackColor = System.Drawing.SystemColors.Window;
            this.bak_numTime.Location = new System.Drawing.Point(81, 43);
            this.bak_numTime.Name = "bak_numTime";
            this.bak_numTime.Seconds = ((long)(300));
            this.bak_numTime.Size = new System.Drawing.Size(66, 21);
            this.bak_numTime.TabIndex = 34;
            this.bak_numTime.Text = "5m";
            this.bak_numTime.Value = System.TimeSpan.Parse("00:05:00");
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
            this.bak_txtLocation.BackColor = System.Drawing.SystemColors.Window;
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
            // pageRelay
            // 
            this.pageRelay.BackColor = System.Drawing.SystemColors.Control;
            this.pageRelay.Controls.Add(this.dis_grp);
            this.pageRelay.Controls.Add(this.gb_ircSettings);
            this.pageMisc.Controls.Add(this.sql_grp);
            this.pageRelay.Controls.Add(this.irc_grp);
            this.pageRelay.Location = new System.Drawing.Point(4, 22);
            this.pageRelay.Name = "pageRelay";
            this.pageRelay.Size = new System.Drawing.Size(498, 521);
            this.pageRelay.TabIndex = 6;
            this.pageRelay.Text = "IRC";
            // 
            // gb_ircSettings
            // 
            this.gb_ircSettings.Controls.Add(this.irc_txtPrefix);
            this.gb_ircSettings.Controls.Add(this.irc_lblPrefix);
            this.gb_ircSettings.Controls.Add(this.irc_cmbVerify);
            this.gb_ircSettings.Controls.Add(this.irc_lblVerify);
            this.gb_ircSettings.Controls.Add(this.irc_cmbRank);
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
            this.irc_txtPrefix.BackColor = System.Drawing.SystemColors.Window;
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
            // irc_cmbVerify
            // 
            this.irc_cmbVerify.BackColor = System.Drawing.SystemColors.Window;
            this.irc_cmbVerify.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.irc_cmbVerify.FormattingEnabled = true;
            this.irc_cmbVerify.Location = new System.Drawing.Point(387, 42);
            this.irc_cmbVerify.Name = "irc_cmbVerify";
            this.irc_cmbVerify.Size = new System.Drawing.Size(80, 21);
            this.irc_cmbVerify.TabIndex = 38;
            // 
            // irc_lblVerify
            // 
            this.irc_lblVerify.AutoSize = true;
            this.irc_lblVerify.Location = new System.Drawing.Point(284, 45);
            this.irc_lblVerify.Name = "irc_lblVerify";
            this.irc_lblVerify.Size = new System.Drawing.Size(99, 13);
            this.irc_lblVerify.TabIndex = 37;
            this.irc_lblVerify.Text = "Verification method:";
            // 
            // irc_cmbRank
            // 
            this.irc_cmbRank.BackColor = System.Drawing.SystemColors.Window;
            this.irc_cmbRank.FormattingEnabled = true;
            this.irc_cmbRank.Location = new System.Drawing.Point(367, 17);
            this.irc_cmbRank.Name = "irc_cmbRank";
            this.irc_cmbRank.Size = new System.Drawing.Size(100, 21);
            this.irc_cmbRank.TabIndex = 36;
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
            // dis_grp
            // 
            this.dis_grp.Controls.Add(this.dis_linkHelp);
            this.dis_grp.Controls.Add(this.dis_chkEnabled);
            this.dis_grp.Controls.Add(this.dis_lblToken);
            this.dis_grp.Controls.Add(this.dis_txtToken);
            this.dis_grp.Controls.Add(this.dis_lblChannel);
            this.dis_grp.Controls.Add(this.dis_txtChannel);
            this.dis_grp.Controls.Add(this.dis_lblOpChannel);
            this.dis_grp.Controls.Add(this.dis_txtOpChannel);
            this.dis_grp.Controls.Add(this.dis_chkNicks);
            this.dis_grp.Location = new System.Drawing.Point(241, 3);
            this.dis_grp.Name = "dis_grp";
            this.dis_grp.Size = new System.Drawing.Size(250, 158);
            this.dis_grp.TabIndex = 34;
            this.dis_grp.TabStop = false;
            this.dis_grp.Text = "Discord";
            // 
            // dis_lblToken
            // 
            this.dis_lblToken.AutoSize = true;
            this.dis_lblToken.Location = new System.Drawing.Point(6, 50);
            this.dis_lblToken.Name = "dis_lblToken";
            this.dis_lblToken.Size = new System.Drawing.Size(55, 13);
            this.dis_lblToken.TabIndex = 19;
            this.dis_lblToken.Text = "Bot token:";
            // 
            // dis_lblChannel
            // 
            this.dis_lblChannel.AutoSize = true;
            this.dis_lblChannel.Location = new System.Drawing.Point(6, 77);
            this.dis_lblChannel.Name = "dis_lblChannel";
            this.dis_lblChannel.Size = new System.Drawing.Size(61, 13);
            this.dis_lblChannel.TabIndex = 30;
            this.dis_lblChannel.Text = "Channel ID:";
            // 
            // dis_txtChannel
            // 
            this.dis_txtChannel.BackColor = System.Drawing.SystemColors.Window;
            this.dis_txtChannel.Location = new System.Drawing.Point(82, 74);
            this.dis_txtChannel.Name = "dis_txtChannel";
            this.dis_txtChannel.Size = new System.Drawing.Size(152, 21);
            this.dis_txtChannel.TabIndex = 31;
            this.toolTip.SetToolTip(this.dis_txtOpChannel, "The ID of the channel that chat is sent to and read from.\n\n" +
                                    "To get the ID of a channel on Discord, right click it and then click Copy ID on the dropdown menu");
            // 
            // dis_lblOpChannel
            // 
            this.dis_lblOpChannel.AutoSize = true;
            this.dis_lblOpChannel.Location = new System.Drawing.Point(6, 104);
            this.dis_lblOpChannel.Name = "dis_lblOpChannel";
            this.dis_lblOpChannel.Size = new System.Drawing.Size(67, 13);
            this.dis_lblOpChannel.TabIndex = 20;
            this.dis_lblOpChannel.Text = "OpChannel ID:";
            // 
            // dis_chkEnabled
            // 
            this.dis_chkEnabled.AutoSize = true;
            this.dis_chkEnabled.Location = new System.Drawing.Point(9, 20);
            this.dis_chkEnabled.Name = "dis_chkEnabled";
            this.dis_chkEnabled.Size = new System.Drawing.Size(96, 17);
            this.dis_chkEnabled.TabIndex = 22;
            this.dis_chkEnabled.Text = "Enable Discord integration";
            this.toolTip.SetToolTip(this.dis_chkEnabled, "Enables sending chat to and reading chat from Discord channel(s) using a bot account");
            this.dis_chkEnabled.UseVisualStyleBackColor = true;
            this.dis_chkEnabled.CheckedChanged += new System.EventHandler(this.dis_chkEnabled_CheckedChanged);
            // 
            // dis_txtToken
            // 
            this.dis_txtToken.BackColor = System.Drawing.SystemColors.Window;
            this.dis_txtToken.Location = new System.Drawing.Point(82, 47);
            this.dis_txtToken.Name = "dis_txtToken";
            this.dis_txtToken.PasswordChar = '*';
            this.dis_txtToken.Size = new System.Drawing.Size(152, 21);
            this.dis_txtToken.TabIndex = 15;
            this.toolTip.SetToolTip(this.dis_txtToken, "The token for the bot account. To find this token:\n" +
                                    "Go to Developer portal -> go to the bot application -> Settings -> Bot -> click Copy under TOKEN\n\n" +
                                    "Note: This token allows full access to the bot - NEVER SHARE THIS TOKEN WITH ANYONE ELSE");
            // 
            // dis_txtOpChannel
            // 
            this.dis_txtOpChannel.BackColor = System.Drawing.SystemColors.Window;
            this.dis_txtOpChannel.Location = new System.Drawing.Point(82, 101);
            this.dis_txtOpChannel.Name = "dis_txtOpChannel";
            this.dis_txtOpChannel.Size = new System.Drawing.Size(152, 21);
            this.dis_txtOpChannel.TabIndex = 16;
            this.toolTip.SetToolTip(this.dis_txtOpChannel, "The ID of the channel that staff only chat is sent to and read from. Can be left blank.\n\n" +
                                    "To get the ID of a channel on Discord, right click it and then click Copy ID on the dropdown menu");
            // 
            // dis_chkNicks
            // 
            this.dis_chkNicks.AutoSize = true;
            this.dis_chkNicks.Location = new System.Drawing.Point(9, 131);
            this.dis_chkNicks.Name = "dis_chkNicks";
            this.dis_chkNicks.Size = new System.Drawing.Size(171, 17);
            this.dis_chkNicks.TabIndex = 32;
            this.dis_chkNicks.Text = "Prefer nicknames to usernames";
            this.dis_chkNicks.UseVisualStyleBackColor = true;
            // 
            // dis_linkHelp
            // 
            this.dis_linkHelp.AutoSize = true;
            this.dis_linkHelp.Location = new System.Drawing.Point(207, 21);
            this.dis_linkHelp.Name = "dis_linkHelp";
            this.dis_linkHelp.Size = new System.Drawing.Size(28, 13);
            this.dis_linkHelp.TabIndex = 33;
            this.dis_linkHelp.TabStop = true;
            this.dis_linkHelp.Text = "Help";
            this.dis_linkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dis_lnkHelp_LinkClicked);
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
            this.sql_grp.Location = new System.Drawing.Point(10, 288);
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
            this.sql_txtUser.BackColor = System.Drawing.SystemColors.Window;
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
            this.sql_txtPass.BackColor = System.Drawing.SystemColors.Window;
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
            this.sql_txtDBName.BackColor = System.Drawing.SystemColors.Window;
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
            this.sql_txtHost.BackColor = System.Drawing.SystemColors.Window;
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
            this.sql_txtPort.BackColor = System.Drawing.SystemColors.Window;
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
            this.irc_grp.Controls.Add(this.irc_numPort);
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
            this.irc_grp.Size = new System.Drawing.Size(225, 214);
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
            // irc_numPort
            // 
            this.irc_numPort.BackColor = System.Drawing.SystemColors.Window;
            this.irc_numPort.Location = new System.Drawing.Point(82, 74);
            this.irc_numPort.Name = "irc_numPort";
            this.irc_numPort.Size = new System.Drawing.Size(63, 21);
            this.irc_numPort.TabIndex = 31;
            this.irc_numPort.Maximum = new decimal(new int[] {
                                    65535,
                                    0,
                                    0,
                                    0});
            this.irc_numPort.Value = new decimal(new int[] {
                                    6667,
                                    0,
                                    0,
                                    0});
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
            this.lvl_txtMain.BackColor = System.Drawing.SystemColors.Window;
            this.lvl_txtMain.Location = new System.Drawing.Point(75, 19);
            this.lvl_txtMain.Name = "lvl_txtMain";
            this.lvl_txtMain.Size = new System.Drawing.Size(87, 21);
            this.lvl_txtMain.TabIndex = 2;
            // 
            // adv_grp
            // 
            this.adv_grp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.adv_grp.Controls.Add(this.adv_chkVerify);
            this.adv_grp.Controls.Add(this.adv_chkCPE);
            this.adv_grp.Controls.Add(this.adv_btnEditTexts);
            this.adv_grp.Location = new System.Drawing.Point(8, 271);
            this.adv_grp.Name = "adv_grp";
            this.adv_grp.Size = new System.Drawing.Size(206, 98);
            this.adv_grp.TabIndex = 42;
            this.adv_grp.TabStop = false;
            this.adv_grp.Text = "Advanced Configuration";
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
            this.srv_btnPort.Location = new System.Drawing.Point(152, 72);
            this.srv_btnPort.Name = "srv_btnPort";
            this.srv_btnPort.Size = new System.Drawing.Size(95, 23);
            this.srv_btnPort.TabIndex = 3;
            this.srv_btnPort.Text = "Port forwarding";
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
            this.srv_txtOwner.BackColor = System.Drawing.SystemColors.Window;
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
            this.srv_numPlayers.BackColor = System.Drawing.SystemColors.Window;
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
            this.srv_numGuests.BackColor = System.Drawing.SystemColors.Window;
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
            this.tabControl.Controls.Add(this.pageRelay);
            this.tabControl.Controls.Add(this.pageEco);
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
            // pageEco
            // 
            this.pageEco.Controls.Add(this.eco_gbRank);
            this.pageEco.Controls.Add(this.eco_gbLvl);
            this.pageEco.Controls.Add(this.eco_gbItem);
            this.pageEco.Controls.Add(this.eco_gb);
            this.pageEco.Location = new System.Drawing.Point(4, 22);
            this.pageEco.Name = "pageEco";
            this.pageEco.Size = new System.Drawing.Size(498, 521);
            this.pageEco.TabIndex = 11;
            this.pageEco.Text = "Eco";
            // 
            // eco_gbRank
            // 
            this.eco_gbRank.Controls.Add(this.eco_dgvRanks);
            this.eco_gbRank.Controls.Add(this.eco_cbRank);
            this.eco_gbRank.Enabled = false;
            this.eco_gbRank.Location = new System.Drawing.Point(8, 85);
            this.eco_gbRank.Margin = new System.Windows.Forms.Padding(2);
            this.eco_gbRank.Name = "eco_gbRank";
            this.eco_gbRank.Padding = new System.Windows.Forms.Padding(2);
            this.eco_gbRank.Size = new System.Drawing.Size(484, 197);
            this.eco_gbRank.TabIndex = 43;
            this.eco_gbRank.TabStop = false;
            this.eco_gbRank.Text = "Rank";
            this.eco_gbRank.Visible = false;
            // 
            // eco_dgvRanks
            // 
            this.eco_dgvRanks.AllowUserToAddRows = false;
            this.eco_dgvRanks.AllowUserToDeleteRows = false;
            this.eco_dgvRanks.AllowUserToResizeRows = false;
            this.eco_dgvRanks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eco_dgvRanks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                                    this.eco_colRankName,
                                    this.eco_colRankPrice});
            this.eco_dgvRanks.Location = new System.Drawing.Point(6, 38);
            this.eco_dgvRanks.Margin = new System.Windows.Forms.Padding(2);
            this.eco_dgvRanks.MultiSelect = false;
            this.eco_dgvRanks.Name = "eco_dgvRanks";
            this.eco_dgvRanks.RowHeadersVisible = false;
            this.eco_dgvRanks.RowTemplate.Height = 18;
            this.eco_dgvRanks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.eco_dgvRanks.Size = new System.Drawing.Size(472, 152);
            this.eco_dgvRanks.TabIndex = 10;
            this.eco_dgvRanks.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.eco_dgvRanks_CellValueChanged);
            // 
            // eco_colRankName
            // 
            this.eco_colRankName.HeaderText = "Rank";
            this.eco_colRankName.Name = "eco_colRankName";
            this.eco_colRankName.ReadOnly = true;
            this.eco_colRankName.Width = 140;
            // 
            // eco_colRankPrice
            // 
            this.eco_colRankPrice.HeaderText = "Price";
            this.eco_colRankPrice.Name = "eco_colRankPrice";
            this.eco_colRankPrice.Width = 60;
            // 
            // eco_cbRank
            // 
            this.eco_cbRank.AutoSize = true;
            this.eco_cbRank.Location = new System.Drawing.Point(6, 17);
            this.eco_cbRank.Margin = new System.Windows.Forms.Padding(2);
            this.eco_cbRank.Name = "eco_cbRank";
            this.eco_cbRank.Size = new System.Drawing.Size(64, 17);
            this.eco_cbRank.TabIndex = 5;
            this.eco_cbRank.Text = "Enabled";
            this.eco_cbRank.UseVisualStyleBackColor = true;
            this.eco_cbRank.CheckedChanged += new System.EventHandler(this.eco_cbRank_CheckedChanged);
            // 
            // eco_gbLvl
            // 
            this.eco_gbLvl.Controls.Add(this.eco_dgvMaps);
            this.eco_gbLvl.Controls.Add(this.eco_btnLvlDel);
            this.eco_gbLvl.Controls.Add(this.eco_btnLvlAdd);
            this.eco_gbLvl.Controls.Add(this.eco_cbLvl);
            this.eco_gbLvl.Enabled = false;
            this.eco_gbLvl.Location = new System.Drawing.Point(8, 85);
            this.eco_gbLvl.Margin = new System.Windows.Forms.Padding(2);
            this.eco_gbLvl.Name = "eco_gbLvl";
            this.eco_gbLvl.Padding = new System.Windows.Forms.Padding(2);
            this.eco_gbLvl.Size = new System.Drawing.Size(484, 222);
            this.eco_gbLvl.TabIndex = 44;
            this.eco_gbLvl.TabStop = false;
            this.eco_gbLvl.Text = "Level";
            this.eco_gbLvl.Visible = false;
            // 
            // eco_dgvMaps
            // 
            this.eco_dgvMaps.AllowUserToAddRows = false;
            this.eco_dgvMaps.AllowUserToDeleteRows = false;
            this.eco_dgvMaps.AllowUserToResizeRows = false;
            this.eco_dgvMaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eco_dgvMaps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                                    this.eco_colLvlName,
                                    this.eco_colLvlPrice,
                                    this.eco_colLvlX,
                                    this.eco_colLvlY,
                                    this.eco_colLvlZ,
                                    this.eco_colLvlTheme});
            this.eco_dgvMaps.Location = new System.Drawing.Point(6, 39);
            this.eco_dgvMaps.Margin = new System.Windows.Forms.Padding(2);
            this.eco_dgvMaps.MultiSelect = false;
            this.eco_dgvMaps.Name = "eco_dgvMaps";
            this.eco_dgvMaps.RowHeadersVisible = false;
            this.eco_dgvMaps.RowTemplate.Height = 24;
            this.eco_dgvMaps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.eco_dgvMaps.Size = new System.Drawing.Size(472, 151);
            this.eco_dgvMaps.TabIndex = 9;
            this.eco_dgvMaps.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.eco_dgvMaps_CellValueChanged);
            // 
            // eco_colLvlName
            // 
            this.eco_colLvlName.HeaderText = "Name";
            this.eco_colLvlName.Name = "eco_colLvlName";
            this.eco_colLvlName.Width = 140;
            // 
            // eco_colLvlPrice
            // 
            this.eco_colLvlPrice.HeaderText = "Price";
            this.eco_colLvlPrice.Name = "eco_colLvlPrice";
            this.eco_colLvlPrice.Width = 60;
            // 
            // eco_colLvlX
            // 
            this.eco_colLvlX.HeaderText = "Width";
            this.eco_colLvlX.Name = "eco_colLvlX";
            this.eco_colLvlX.Width = 50;
            // 
            // eco_colLvlY
            // 
            this.eco_colLvlY.HeaderText = "Height";
            this.eco_colLvlY.Name = "eco_colLvlY";
            this.eco_colLvlY.Width = 50;
            // 
            // eco_colLvlZ
            // 
            this.eco_colLvlZ.HeaderText = "Length";
            this.eco_colLvlZ.Name = "eco_colLvlZ";
            this.eco_colLvlZ.Width = 50;
            // 
            // eco_colLvlTheme
            // 
            this.eco_colLvlTheme.HeaderText = "Theme";
            this.eco_colLvlTheme.Name = "eco_colLvlTheme";
            this.eco_colLvlTheme.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.eco_colLvlTheme.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // eco_btnLvlDel
            // 
            this.eco_btnLvlDel.Enabled = false;
            this.eco_btnLvlDel.Location = new System.Drawing.Point(406, 194);
            this.eco_btnLvlDel.Margin = new System.Windows.Forms.Padding(2);
            this.eco_btnLvlDel.Name = "eco_btnLvlDel";
            this.eco_btnLvlDel.Size = new System.Drawing.Size(72, 23);
            this.eco_btnLvlDel.TabIndex = 8;
            this.eco_btnLvlDel.Text = "Remove";
            this.eco_btnLvlDel.UseVisualStyleBackColor = true;
            this.eco_btnLvlDel.Click += new System.EventHandler(this.eco_lvlDelete_Click);
            // 
            // eco_btnLvlAdd
            // 
            this.eco_btnLvlAdd.Location = new System.Drawing.Point(4, 194);
            this.eco_btnLvlAdd.Margin = new System.Windows.Forms.Padding(2);
            this.eco_btnLvlAdd.Name = "eco_btnLvlAdd";
            this.eco_btnLvlAdd.Size = new System.Drawing.Size(72, 23);
            this.eco_btnLvlAdd.TabIndex = 7;
            this.eco_btnLvlAdd.Text = "Add";
            this.eco_btnLvlAdd.UseVisualStyleBackColor = true;
            this.eco_btnLvlAdd.Click += new System.EventHandler(this.eco_lvlAdd_Click);
            // 
            // eco_cbLvl
            // 
            this.eco_cbLvl.AutoSize = true;
            this.eco_cbLvl.Location = new System.Drawing.Point(6, 17);
            this.eco_cbLvl.Margin = new System.Windows.Forms.Padding(2);
            this.eco_cbLvl.Name = "eco_cbLvl";
            this.eco_cbLvl.Size = new System.Drawing.Size(64, 17);
            this.eco_cbLvl.TabIndex = 6;
            this.eco_cbLvl.Text = "Enabled";
            this.eco_cbLvl.UseVisualStyleBackColor = true;
            this.eco_cbLvl.CheckedChanged += new System.EventHandler(this.eco_lvlEnabled_CheckedChanged);
            // 
            // eco_gbItem
            // 
            this.eco_gbItem.Controls.Add(this.eco_lblItemRank);
            this.eco_gbItem.Controls.Add(this.eco_cmbItemRank);
            this.eco_gbItem.Controls.Add(this.eco_numItemPrice);
            this.eco_gbItem.Controls.Add(this.eco_lblItemPrice);
            this.eco_gbItem.Controls.Add(this.eco_cbItem);
            this.eco_gbItem.Enabled = false;
            this.eco_gbItem.Location = new System.Drawing.Point(8, 85);
            this.eco_gbItem.Margin = new System.Windows.Forms.Padding(2);
            this.eco_gbItem.Name = "eco_gbItem";
            this.eco_gbItem.Padding = new System.Windows.Forms.Padding(2);
            this.eco_gbItem.Size = new System.Drawing.Size(484, 70);
            this.eco_gbItem.TabIndex = 42;
            this.eco_gbItem.TabStop = false;
            this.eco_gbItem.Text = "Titlecolor";
            this.eco_gbItem.Visible = false;
            // 
            // eco_lblItemRank
            // 
            this.eco_lblItemRank.AutoSize = true;
            this.eco_lblItemRank.Location = new System.Drawing.Point(311, 46);
            this.eco_lblItemRank.Name = "eco_lblItemRank";
            this.eco_lblItemRank.Size = new System.Drawing.Size(51, 13);
            this.eco_lblItemRank.TabIndex = 24;
            this.eco_lblItemRank.Text = "Min rank:";
            // 
            // eco_numItemPrice
            // 
            this.eco_numItemPrice.BackColor = System.Drawing.SystemColors.Window;
            this.eco_numItemPrice.Location = new System.Drawing.Point(76, 43);
            this.eco_numItemPrice.Margin = new System.Windows.Forms.Padding(2);
            this.eco_numItemPrice.Maximum = new decimal(new int[] {
                                    16777215,
                                    0,
                                    0,
                                    0});
            this.eco_numItemPrice.Name = "eco_numItemPrice";
            this.eco_numItemPrice.Size = new System.Drawing.Size(78, 21);
            this.eco_numItemPrice.TabIndex = 6;
            this.eco_numItemPrice.Value = new decimal(new int[] {
                                    100,
                                    0,
                                    0,
                                    0});
            this.eco_numItemPrice.ValueChanged += new System.EventHandler(this.eco_numItemPrice_ValueChanged);
            // 
            // eco_lblItemPrice
            // 
            this.eco_lblItemPrice.AutoSize = true;
            this.eco_lblItemPrice.Location = new System.Drawing.Point(38, 45);
            this.eco_lblItemPrice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.eco_lblItemPrice.Name = "eco_lblItemPrice";
            this.eco_lblItemPrice.Size = new System.Drawing.Size(34, 13);
            this.eco_lblItemPrice.TabIndex = 6;
            this.eco_lblItemPrice.Text = "Price:";
            // 
            // eco_cbItem
            // 
            this.eco_cbItem.AutoSize = true;
            this.eco_cbItem.Location = new System.Drawing.Point(6, 17);
            this.eco_cbItem.Margin = new System.Windows.Forms.Padding(2);
            this.eco_cbItem.Name = "eco_cbItem";
            this.eco_cbItem.Size = new System.Drawing.Size(64, 17);
            this.eco_cbItem.TabIndex = 6;
            this.eco_cbItem.Text = "Enabled";
            this.eco_cbItem.UseVisualStyleBackColor = true;
            this.eco_cbItem.CheckedChanged += new System.EventHandler(this.eco_cbItem_CheckedChanged);
            // 
            // eco_gb
            // 
            this.eco_gb.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.eco_gb.Controls.Add(this.eco_cmbCfg);
            this.eco_gb.Controls.Add(this.eco_lblCfg);
            this.eco_gb.Controls.Add(this.eco_cbEnabled);
            this.eco_gb.Controls.Add(this.eco_txtCurrency);
            this.eco_gb.Controls.Add(this.eco_lblCurrency);
            this.eco_gb.Location = new System.Drawing.Point(8, 3);
            this.eco_gb.Name = "eco_gb";
            this.eco_gb.Size = new System.Drawing.Size(484, 70);
            this.eco_gb.TabIndex = 41;
            this.eco_gb.TabStop = false;
            this.eco_gb.Text = "Economy";
            // 
            // eco_cmbCfg
            // 
            this.eco_cmbCfg.BackColor = System.Drawing.SystemColors.Window;
            this.eco_cmbCfg.FormattingEnabled = true;
            this.eco_cmbCfg.Location = new System.Drawing.Point(368, 42);
            this.eco_cmbCfg.Name = "eco_cmbCfg";
            this.eco_cmbCfg.Size = new System.Drawing.Size(110, 21);
            this.eco_cmbCfg.TabIndex = 23;
            this.eco_cmbCfg.SelectedIndexChanged += new System.EventHandler(this.Eco_cmbCfg_SelectedIndexChanged);
            // 
            // eco_lblCfg
            // 
            this.eco_lblCfg.AutoSize = true;
            this.eco_lblCfg.Location = new System.Drawing.Point(286, 45);
            this.eco_lblCfg.Name = "eco_lblCfg";
            this.eco_lblCfg.Size = new System.Drawing.Size(79, 13);
            this.eco_lblCfg.TabIndex = 22;
            this.eco_lblCfg.Text = "Configure item:";
            // 
            // eco_cbEnabled
            // 
            this.eco_cbEnabled.AutoSize = true;
            this.eco_cbEnabled.Location = new System.Drawing.Point(6, 20);
            this.eco_cbEnabled.Name = "eco_cbEnabled";
            this.eco_cbEnabled.Size = new System.Drawing.Size(64, 17);
            this.eco_cbEnabled.TabIndex = 21;
            this.eco_cbEnabled.Text = "Enabled";
            this.eco_cbEnabled.UseVisualStyleBackColor = true;
            this.eco_cbEnabled.CheckedChanged += new System.EventHandler(this.eco_cbEnabled_CheckedChanged);
            // 
            // eco_txtCurrency
            // 
            this.eco_txtCurrency.BackColor = System.Drawing.SystemColors.Window;
            this.eco_txtCurrency.Location = new System.Drawing.Point(76, 42);
            this.eco_txtCurrency.Name = "eco_txtCurrency";
            this.eco_txtCurrency.Size = new System.Drawing.Size(118, 21);
            this.eco_txtCurrency.TabIndex = 1;
            // 
            // eco_lblCurrency
            // 
            this.eco_lblCurrency.AutoSize = true;
            this.eco_lblCurrency.Location = new System.Drawing.Point(18, 45);
            this.eco_lblCurrency.Name = "eco_lblCurrency";
            this.eco_lblCurrency.Size = new System.Drawing.Size(52, 13);
            this.eco_lblCurrency.TabIndex = 11;
            this.eco_lblCurrency.Text = "Currency:";
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
            this.tabGames.Controls.Add(this.tabLS);
            this.tabGames.Controls.Add(this.tabZS);
            this.tabGames.Controls.Add(this.tabZS_old);
            this.tabGames.Controls.Add(this.tabCTF);
            this.tabGames.Controls.Add(this.tabTW);
            this.tabGames.Location = new System.Drawing.Point(3, 3);
            this.tabGames.Name = "tabGames";
            this.tabGames.SelectedIndex = 0;
            this.tabGames.Size = new System.Drawing.Size(492, 515);
            this.tabGames.TabIndex = 0;
            // 
            // tabLS
            // 
            this.tabLS.BackColor = System.Drawing.SystemColors.Control;
            this.tabLS.Controls.Add(this.ls_grpControls);
            this.tabLS.Controls.Add(this.ls_grpMapSettings);
            this.tabLS.Controls.Add(this.ls_grpSettings);
            this.tabLS.Controls.Add(this.ls_grpMaps);
            this.tabLS.Location = new System.Drawing.Point(4, 22);
            this.tabLS.Name = "tabLS";
            this.tabLS.Padding = new System.Windows.Forms.Padding(3);
            this.tabLS.Size = new System.Drawing.Size(484, 489);
            this.tabLS.TabIndex = 0;
            this.tabLS.Text = "Lava Survival";
            // 
            // ls_grpControls
            // 
            this.ls_grpControls.Controls.Add(this.ls_btnEnd);
            this.ls_grpControls.Controls.Add(this.ls_btnStop);
            this.ls_grpControls.Controls.Add(this.ls_btnStart);
            this.ls_grpControls.Location = new System.Drawing.Point(110, 5);
            this.ls_grpControls.Name = "ls_grpControls";
            this.ls_grpControls.Size = new System.Drawing.Size(279, 51);
            this.ls_grpControls.TabIndex = 4;
            this.ls_grpControls.TabStop = false;
            this.ls_grpControls.Text = "Controls";
            // 
            // ls_btnEnd
            // 
            this.ls_btnEnd.Location = new System.Drawing.Point(190, 19);
            this.ls_btnEnd.Name = "ls_btnEnd";
            this.ls_btnEnd.Size = new System.Drawing.Size(80, 23);
            this.ls_btnEnd.TabIndex = 2;
            this.ls_btnEnd.Text = "End Round";
            this.ls_btnEnd.UseVisualStyleBackColor = true;
            // 
            // ls_btnStop
            // 
            this.ls_btnStop.Location = new System.Drawing.Point(100, 19);
            this.ls_btnStop.Name = "ls_btnStop";
            this.ls_btnStop.Size = new System.Drawing.Size(80, 23);
            this.ls_btnStop.TabIndex = 1;
            this.ls_btnStop.Text = "Stop Game";
            this.ls_btnStop.UseVisualStyleBackColor = true;
            // 
            // ls_btnStart
            // 
            this.ls_btnStart.Location = new System.Drawing.Point(10, 19);
            this.ls_btnStart.Name = "ls_btnStart";
            this.ls_btnStart.Size = new System.Drawing.Size(80, 23);
            this.ls_btnStart.TabIndex = 0;
            this.ls_btnStart.Text = "Start Game";
            this.ls_btnStart.UseVisualStyleBackColor = true;
            // 
            // ls_grpMapSettings
            // 
            this.ls_grpMapSettings.Controls.Add(this.ls_grpTime);
            this.ls_grpMapSettings.Controls.Add(this.ls_grpLayer);
            this.ls_grpMapSettings.Controls.Add(this.ls_grpBlock);
            this.ls_grpMapSettings.Enabled = false;
            this.ls_grpMapSettings.Location = new System.Drawing.Point(182, 184);
            this.ls_grpMapSettings.Name = "ls_grpMapSettings";
            this.ls_grpMapSettings.Size = new System.Drawing.Size(296, 287);
            this.ls_grpMapSettings.TabIndex = 3;
            this.ls_grpMapSettings.TabStop = false;
            this.ls_grpMapSettings.Text = "Map Settings";
            // 
            // ls_grpTime
            // 
            this.ls_grpTime.Controls.Add(this.ls_numFlood);
            this.ls_grpTime.Controls.Add(this.ls_numLayerTime);
            this.ls_grpTime.Controls.Add(this.ls_numRound);
            this.ls_grpTime.Controls.Add(this.ls_lblLayerTime);
            this.ls_grpTime.Controls.Add(this.ls_lblFlood);
            this.ls_grpTime.Controls.Add(this.ls_lblRound);
            this.ls_grpTime.Location = new System.Drawing.Point(6, 180);
            this.ls_grpTime.Name = "ls_grpTime";
            this.ls_grpTime.Size = new System.Drawing.Size(284, 71);
            this.ls_grpTime.TabIndex = 39;
            this.ls_grpTime.TabStop = false;
            this.ls_grpTime.Text = "Time settings";
            // 
            // ls_numFlood
            // 
            this.ls_numFlood.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numFlood.Location = new System.Drawing.Point(69, 43);
            this.ls_numFlood.Name = "ls_numFlood";
            this.ls_numFlood.Seconds = ((long)(300));
            this.ls_numFlood.Size = new System.Drawing.Size(62, 21);
            this.ls_numFlood.TabIndex = 38;
            this.ls_numFlood.Text = "5m";
            this.ls_numFlood.Value = System.TimeSpan.Parse("00:05:00");
            // 
            // ls_numLayerTime
            // 
            this.ls_numLayerTime.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numLayerTime.Location = new System.Drawing.Point(216, 16);
            this.ls_numLayerTime.Name = "ls_numLayerTime";
            this.ls_numLayerTime.Seconds = ((long)(120));
            this.ls_numLayerTime.Size = new System.Drawing.Size(62, 21);
            this.ls_numLayerTime.TabIndex = 37;
            this.ls_numLayerTime.Text = "2m";
            this.ls_numLayerTime.Value = System.TimeSpan.Parse("00:02:00");
            // 
            // ls_numRound
            // 
            this.ls_numRound.BackColor = System.Drawing.SystemColors.Window;
            this.ls_numRound.Location = new System.Drawing.Point(69, 16);
            this.ls_numRound.Name = "ls_numRound";
            this.ls_numRound.Seconds = ((long)(900));
            this.ls_numRound.Size = new System.Drawing.Size(62, 21);
            this.ls_numRound.TabIndex = 36;
            this.ls_numRound.Text = "15m";
            this.ls_numRound.Value = System.TimeSpan.Parse("00:15:00");
            // 
            // ls_lblLayerTime
            // 
            this.ls_lblLayerTime.AutoSize = true;
            this.ls_lblLayerTime.Location = new System.Drawing.Point(154, 20);
            this.ls_lblLayerTime.Name = "ls_lblLayerTime";
            this.ls_lblLayerTime.Size = new System.Drawing.Size(59, 13);
            this.ls_lblLayerTime.TabIndex = 35;
            this.ls_lblLayerTime.Text = "Layer time:";
            // 
            // ls_lblFlood
            // 
            this.ls_lblFlood.AutoSize = true;
            this.ls_lblFlood.Location = new System.Drawing.Point(8, 46);
            this.ls_lblFlood.Name = "ls_lblFlood";
            this.ls_lblFlood.Size = new System.Drawing.Size(59, 13);
            this.ls_lblFlood.TabIndex = 34;
            this.ls_lblFlood.Text = "Flood time:";
            // 
            // ls_lblRound
            // 
            this.ls_lblRound.AutoSize = true;
            this.ls_lblRound.Location = new System.Drawing.Point(5, 20);
            this.ls_lblRound.Name = "ls_lblRound";
            this.ls_lblRound.Size = new System.Drawing.Size(63, 13);
            this.ls_lblRound.TabIndex = 34;
            this.ls_lblRound.Text = "Round time:";
            // 
            // ls_grpLayer
            // 
            this.ls_grpLayer.Controls.Add(this.ls_lblBlocksTall);
            this.ls_grpLayer.Controls.Add(this.ls_numHeight);
            this.ls_grpLayer.Controls.Add(this.ls_lblLayersEach);
            this.ls_grpLayer.Controls.Add(this.ls_numCount);
            this.ls_grpLayer.Controls.Add(this.ls_numLayer);
            this.ls_grpLayer.Controls.Add(this.ls_lblLayer);
            this.ls_grpLayer.Location = new System.Drawing.Point(6, 100);
            this.ls_grpLayer.Name = "ls_grpLayer";
            this.ls_grpLayer.Size = new System.Drawing.Size(284, 74);
            this.ls_grpLayer.TabIndex = 1;
            this.ls_grpLayer.TabStop = false;
            this.ls_grpLayer.Text = "Layer flood settings";
            // 
            // ls_lblBlocksTall
            // 
            this.ls_lblBlocksTall.AutoSize = true;
            this.ls_lblBlocksTall.Location = new System.Drawing.Point(183, 48);
            this.ls_lblBlocksTall.Name = "ls_lblBlocksTall";
            this.ls_lblBlocksTall.Size = new System.Drawing.Size(55, 13);
            this.ls_lblBlocksTall.TabIndex = 38;
            this.ls_lblBlocksTall.Text = "blocks tall";
            // 
            // ls_lblLayersEach
            // 
            this.ls_lblLayersEach.AutoSize = true;
            this.ls_lblLayersEach.Location = new System.Drawing.Point(62, 48);
            this.ls_lblLayersEach.Name = "ls_lblLayersEach";
            this.ls_lblLayersEach.Size = new System.Drawing.Size(64, 13);
            this.ls_lblLayersEach.TabIndex = 36;
            this.ls_lblLayersEach.Text = "layers, each";
            // 
            // ls_lblLayer
            // 
            this.ls_lblLayer.AutoSize = true;
            this.ls_lblLayer.Location = new System.Drawing.Point(28, 20);
            this.ls_lblLayer.Name = "ls_lblLayer";
            this.ls_lblLayer.Size = new System.Drawing.Size(98, 13);
            this.ls_lblLayer.TabIndex = 34;
            this.ls_lblLayer.Text = "Layer flood chance:";
            // 
            // ls_grpBlock
            // 
            this.ls_grpBlock.Controls.Add(this.ls_numDestroy);
            this.ls_grpBlock.Controls.Add(this.ls_numWater);
            this.ls_grpBlock.Controls.Add(this.ls_numFast);
            this.ls_grpBlock.Controls.Add(this.ls_numKiller);
            this.ls_grpBlock.Controls.Add(this.ls_lblDestroy);
            this.ls_grpBlock.Controls.Add(this.ls_lblFast);
            this.ls_grpBlock.Controls.Add(this.ls_lblWater);
            this.ls_grpBlock.Controls.Add(this.ls_lblKill);
            this.ls_grpBlock.Location = new System.Drawing.Point(6, 20);
            this.ls_grpBlock.Name = "ls_grpBlock";
            this.ls_grpBlock.Size = new System.Drawing.Size(284, 74);
            this.ls_grpBlock.TabIndex = 0;
            this.ls_grpBlock.TabStop = false;
            this.ls_grpBlock.Text = "Flood block type";
            // 
            // ls_lblDestroy
            // 
            this.ls_lblDestroy.AutoSize = true;
            this.ls_lblDestroy.Location = new System.Drawing.Point(135, 48);
            this.ls_lblDestroy.Name = "ls_lblDestroy";
            this.ls_lblDestroy.Size = new System.Drawing.Size(88, 13);
            this.ls_lblDestroy.TabIndex = 30;
            this.ls_lblDestroy.Text = "Destroys chance:";
            // 
            // ls_lblFast
            // 
            this.ls_lblFast.AutoSize = true;
            this.ls_lblFast.Location = new System.Drawing.Point(11, 48);
            this.ls_lblFast.Name = "ls_lblFast";
            this.ls_lblFast.Size = new System.Drawing.Size(66, 13);
            this.ls_lblFast.TabIndex = 29;
            this.ls_lblFast.Text = "Fast chance:";
            // 
            // ls_lblWater
            // 
            this.ls_lblWater.AutoSize = true;
            this.ls_lblWater.Location = new System.Drawing.Point(147, 23);
            this.ls_lblWater.Name = "ls_lblWater";
            this.ls_lblWater.Size = new System.Drawing.Size(76, 13);
            this.ls_lblWater.TabIndex = 28;
            this.ls_lblWater.Text = "Water chance:";
            // 
            // ls_lblKill
            // 
            this.ls_lblKill.AutoSize = true;
            this.ls_lblKill.Location = new System.Drawing.Point(6, 23);
            this.ls_lblKill.Name = "ls_lblKill";
            this.ls_lblKill.Size = new System.Drawing.Size(71, 13);
            this.ls_lblKill.TabIndex = 27;
            this.ls_lblKill.Text = "Killer chance:";
            // 
            // ls_grpSettings
            // 
            this.ls_grpSettings.Controls.Add(this.ls_lblMax);
            this.ls_grpSettings.Controls.Add(this.ls_numMax);
            this.ls_grpSettings.Controls.Add(this.ls_cbMain);
            this.ls_grpSettings.Controls.Add(this.ls_cbMap);
            this.ls_grpSettings.Controls.Add(this.ls_cbStart);
            this.ls_grpSettings.Location = new System.Drawing.Point(182, 59);
            this.ls_grpSettings.Name = "ls_grpSettings";
            this.ls_grpSettings.Size = new System.Drawing.Size(296, 119);
            this.ls_grpSettings.TabIndex = 2;
            this.ls_grpSettings.TabStop = false;
            this.ls_grpSettings.Text = "Settings";
            // 
            // ls_lblMax
            // 
            this.ls_lblMax.AutoSize = true;
            this.ls_lblMax.Location = new System.Drawing.Point(14, 92);
            this.ls_lblMax.Name = "ls_lblMax";
            this.ls_lblMax.Size = new System.Drawing.Size(54, 13);
            this.ls_lblMax.TabIndex = 26;
            this.ls_lblMax.Text = "Max lives:";
            // 
            // ls_cbMap
            // 
            this.ls_cbMap.AutoSize = true;
            this.ls_cbMap.Location = new System.Drawing.Point(11, 43);
            this.ls_cbMap.Name = "ls_cbMap";
            this.ls_cbMap.Size = new System.Drawing.Size(136, 17);
            this.ls_cbMap.TabIndex = 23;
            this.ls_cbMap.Text = "Map name in server list";
            this.ls_cbMap.UseVisualStyleBackColor = true;
            // 
            // ls_grpMaps
            // 
            this.ls_grpMaps.Controls.Add(this.ls_lblNotUsed);
            this.ls_grpMaps.Controls.Add(this.ls_lblUsed);
            this.ls_grpMaps.Controls.Add(this.ls_btnAdd);
            this.ls_grpMaps.Controls.Add(this.ls_btnRemove);
            this.ls_grpMaps.Controls.Add(this.ls_lstNotUsed);
            this.ls_grpMaps.Controls.Add(this.ls_lstUsed);
            this.ls_grpMaps.Location = new System.Drawing.Point(6, 59);
            this.ls_grpMaps.Name = "ls_grpMaps";
            this.ls_grpMaps.Size = new System.Drawing.Size(170, 412);
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
            // 
            // ls_btnRemove
            // 
            this.ls_btnRemove.Location = new System.Drawing.Point(89, 188);
            this.ls_btnRemove.Name = "ls_btnRemove";
            this.ls_btnRemove.Size = new System.Drawing.Size(75, 23);
            this.ls_btnRemove.TabIndex = 3;
            this.ls_btnRemove.Text = "Remove >>";
            this.ls_btnRemove.UseVisualStyleBackColor = true;
            // 
            // ls_lstNotUsed
            // 
            this.ls_lstNotUsed.BackColor = System.Drawing.SystemColors.Window;
            this.ls_lstNotUsed.FormattingEnabled = true;
            this.ls_lstNotUsed.Location = new System.Drawing.Point(6, 219);
            this.ls_lstNotUsed.Name = "ls_lstNotUsed";
            this.ls_lstNotUsed.Size = new System.Drawing.Size(158, 186);
            this.ls_lstNotUsed.TabIndex = 2;
            // 
            // ls_lstUsed
            // 
            this.ls_lstUsed.BackColor = System.Drawing.SystemColors.Window;
            this.ls_lstUsed.FormattingEnabled = true;
            this.ls_lstUsed.Location = new System.Drawing.Point(6, 33);
            this.ls_lstUsed.Name = "ls_lstUsed";
            this.ls_lstUsed.Size = new System.Drawing.Size(158, 147);
            this.ls_lstUsed.TabIndex = 0;
            this.ls_lstUsed.SelectedIndexChanged += new System.EventHandler(this.lsMapUse_SelectedIndexChanged);
            // 
            // tabZS
            // 
            this.tabZS.BackColor = System.Drawing.SystemColors.Control;
            this.tabZS.Controls.Add(this.zs_grpControls);
            this.tabZS.Controls.Add(this.zs_grpMap);
            this.tabZS.Controls.Add(this.zs_grpSettings);
            this.tabZS.Controls.Add(this.zs_grpMaps);
            this.tabZS.Location = new System.Drawing.Point(4, 22);
            this.tabZS.Name = "tabZS";
            this.tabZS.Padding = new System.Windows.Forms.Padding(3);
            this.tabZS.Size = new System.Drawing.Size(484, 489);
            this.tabZS.TabIndex = 6;
            this.tabZS.Text = "Zombie Survival";
            // 
            // zs_grpControls
            // 
            this.zs_grpControls.Controls.Add(this.zs_btnEnd);
            this.zs_grpControls.Controls.Add(this.zs_btnStop);
            this.zs_grpControls.Controls.Add(this.zs_btnStart);
            this.zs_grpControls.Location = new System.Drawing.Point(110, 5);
            this.zs_grpControls.Name = "zs_grpControls";
            this.zs_grpControls.Size = new System.Drawing.Size(279, 51);
            this.zs_grpControls.TabIndex = 4;
            this.zs_grpControls.TabStop = false;
            this.zs_grpControls.Text = "Controls";
            // 
            // zs_btnEnd
            // 
            this.zs_btnEnd.Location = new System.Drawing.Point(190, 19);
            this.zs_btnEnd.Name = "zs_btnEnd";
            this.zs_btnEnd.Size = new System.Drawing.Size(80, 23);
            this.zs_btnEnd.TabIndex = 2;
            this.zs_btnEnd.Text = "End Round";
            this.zs_btnEnd.UseVisualStyleBackColor = true;
            // 
            // zs_btnStop
            // 
            this.zs_btnStop.Location = new System.Drawing.Point(100, 19);
            this.zs_btnStop.Name = "zs_btnStop";
            this.zs_btnStop.Size = new System.Drawing.Size(80, 23);
            this.zs_btnStop.TabIndex = 1;
            this.zs_btnStop.Text = "Stop Game";
            this.zs_btnStop.UseVisualStyleBackColor = true;
            // 
            // zs_btnStart
            // 
            this.zs_btnStart.Location = new System.Drawing.Point(10, 19);
            this.zs_btnStart.Name = "zs_btnStart";
            this.zs_btnStart.Size = new System.Drawing.Size(80, 23);
            this.zs_btnStart.TabIndex = 0;
            this.zs_btnStart.Text = "Start Game";
            this.zs_btnStart.UseVisualStyleBackColor = true;
            // 
            // zs_grpMap
            // 
            this.zs_grpMap.Controls.Add(this.zs_grpTime);
            this.zs_grpMap.Enabled = false;
            this.zs_grpMap.Location = new System.Drawing.Point(182, 374);
            this.zs_grpMap.Name = "zs_grpMap";
            this.zs_grpMap.Size = new System.Drawing.Size(296, 97);
            this.zs_grpMap.TabIndex = 3;
            this.zs_grpMap.TabStop = false;
            this.zs_grpMap.Text = "Map Settings";
            // 
            // zs_grpTime
            // 
            this.zs_grpTime.Controls.Add(this.timespanUpDown1);
            this.zs_grpTime.Controls.Add(this.timespanUpDown2);
            this.zs_grpTime.Controls.Add(this.timespanUpDown3);
            this.zs_grpTime.Controls.Add(this.label1);
            this.zs_grpTime.Controls.Add(this.label2);
            this.zs_grpTime.Controls.Add(this.label3);
            this.zs_grpTime.Location = new System.Drawing.Point(6, 16);
            this.zs_grpTime.Name = "zs_grpTime";
            this.zs_grpTime.Size = new System.Drawing.Size(284, 71);
            this.zs_grpTime.TabIndex = 39;
            this.zs_grpTime.TabStop = false;
            this.zs_grpTime.Text = "Time settings";
            // 
            // timespanUpDown1
            // 
            this.timespanUpDown1.BackColor = System.Drawing.SystemColors.Window;
            this.timespanUpDown1.Location = new System.Drawing.Point(69, 43);
            this.timespanUpDown1.Name = "timespanUpDown1";
            this.timespanUpDown1.Seconds = ((long)(300));
            this.timespanUpDown1.Size = new System.Drawing.Size(62, 21);
            this.timespanUpDown1.TabIndex = 38;
            this.timespanUpDown1.Text = "5m";
            this.timespanUpDown1.Value = System.TimeSpan.Parse("00:05:00");
            // 
            // timespanUpDown2
            // 
            this.timespanUpDown2.BackColor = System.Drawing.SystemColors.Window;
            this.timespanUpDown2.Location = new System.Drawing.Point(216, 16);
            this.timespanUpDown2.Name = "timespanUpDown2";
            this.timespanUpDown2.Seconds = ((long)(120));
            this.timespanUpDown2.Size = new System.Drawing.Size(62, 21);
            this.timespanUpDown2.TabIndex = 37;
            this.timespanUpDown2.Text = "2m";
            this.timespanUpDown2.Value = System.TimeSpan.Parse("00:02:00");
            // 
            // timespanUpDown3
            // 
            this.timespanUpDown3.BackColor = System.Drawing.SystemColors.Window;
            this.timespanUpDown3.Location = new System.Drawing.Point(69, 16);
            this.timespanUpDown3.Name = "timespanUpDown3";
            this.timespanUpDown3.Seconds = ((long)(900));
            this.timespanUpDown3.Size = new System.Drawing.Size(62, 21);
            this.timespanUpDown3.TabIndex = 36;
            this.timespanUpDown3.Text = "15m";
            this.timespanUpDown3.Value = System.TimeSpan.Parse("00:15:00");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(154, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Layer time:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Flood time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Round time:";
            // 
            // zs_grpSettings
            // 
            this.zs_grpSettings.Controls.Add(this.zs_grpZombie);
            this.zs_grpSettings.Controls.Add(this.zs_grpRevive);
            this.zs_grpSettings.Controls.Add(this.zs_cbMain);
            this.zs_grpSettings.Controls.Add(this.zs_cbMap);
            this.zs_grpSettings.Controls.Add(this.zs_grpInv);
            this.zs_grpSettings.Controls.Add(this.zs_cbStart);
            this.zs_grpSettings.Location = new System.Drawing.Point(182, 59);
            this.zs_grpSettings.Name = "zs_grpSettings";
            this.zs_grpSettings.Size = new System.Drawing.Size(296, 309);
            this.zs_grpSettings.TabIndex = 2;
            this.zs_grpSettings.TabStop = false;
            this.zs_grpSettings.Text = "Settings";
            // 
            // zs_grpZombie
            // 
            this.zs_grpZombie.Controls.Add(this.zs_txtModel);
            this.zs_grpZombie.Controls.Add(this.zs_txtName);
            this.zs_grpZombie.Controls.Add(this.zs_lblModel);
            this.zs_grpZombie.Controls.Add(this.zs_lblName);
            this.zs_grpZombie.Location = new System.Drawing.Point(6, 250);
            this.zs_grpZombie.Name = "zs_grpZombie";
            this.zs_grpZombie.Size = new System.Drawing.Size(284, 46);
            this.zs_grpZombie.TabIndex = 40;
            this.zs_grpZombie.TabStop = false;
            this.zs_grpZombie.Text = "Zombie settings";
            // 
            // zs_txtModel
            // 
            this.zs_txtModel.BackColor = System.Drawing.SystemColors.Window;
            this.zs_txtModel.Location = new System.Drawing.Point(200, 17);
            this.zs_txtModel.Name = "zs_txtModel";
            this.zs_txtModel.Size = new System.Drawing.Size(76, 21);
            this.zs_txtModel.TabIndex = 39;
            this.toolTip.SetToolTip(this.zs_txtModel, "Model to use for infected players.\nIf left blank, then 'zombie' model is used.");
            // 
            // zs_txtName
            // 
            this.zs_txtName.BackColor = System.Drawing.SystemColors.Window;
            this.zs_txtName.Location = new System.Drawing.Point(49, 17);
            this.zs_txtName.Name = "zs_txtName";
            this.zs_txtName.Size = new System.Drawing.Size(80, 21);
            this.zs_txtName.TabIndex = 38;
            this.toolTip.SetToolTip(this.zs_txtName, "Name to show above head of infected players.\nIf left blank, then the player's name is shown instead.");
            // 
            // zs_lblModel
            // 
            this.zs_lblModel.AutoSize = true;
            this.zs_lblModel.Location = new System.Drawing.Point(154, 20);
            this.zs_lblModel.Name = "zs_lblModel";
            this.zs_lblModel.Size = new System.Drawing.Size(40, 13);
            this.zs_lblModel.TabIndex = 35;
            this.zs_lblModel.Text = "Model:";
            // 
            // zs_lblName
            // 
            this.zs_lblName.AutoSize = true;
            this.zs_lblName.Location = new System.Drawing.Point(5, 20);
            this.zs_lblName.Name = "zs_lblName";
            this.zs_lblName.Size = new System.Drawing.Size(38, 13);
            this.zs_lblName.TabIndex = 34;
            this.zs_lblName.Text = "Name:";
            // 
            // zs_grpRevive
            // 
            this.zs_grpRevive.Controls.Add(this.zs_lblReviveEff);
            this.zs_grpRevive.Controls.Add(this.zs_numReviveEff);
            this.zs_grpRevive.Controls.Add(this.label4);
            this.zs_grpRevive.Controls.Add(this.zs_lblReviveLimitFtr);
            this.zs_grpRevive.Controls.Add(this.zs_lblReviveLimitHdr);
            this.zs_grpRevive.Controls.Add(this.zs_numReviveLimit);
            this.zs_grpRevive.Controls.Add(this.zs_numReviveMax);
            this.zs_grpRevive.Controls.Add(this.label9);
            this.zs_grpRevive.Location = new System.Drawing.Point(6, 169);
            this.zs_grpRevive.Name = "zs_grpRevive";
            this.zs_grpRevive.Size = new System.Drawing.Size(284, 73);
            this.zs_grpRevive.TabIndex = 25;
            this.zs_grpRevive.TabStop = false;
            this.zs_grpRevive.Text = "Revive settings";
            // 
            // zs_lblReviveEff
            // 
            this.zs_lblReviveEff.AutoSize = true;
            this.zs_lblReviveEff.Location = new System.Drawing.Point(202, 20);
            this.zs_lblReviveEff.Name = "zs_lblReviveEff";
            this.zs_lblReviveEff.Size = new System.Drawing.Size(79, 13);
            this.zs_lblReviveEff.TabIndex = 40;
            this.zs_lblReviveEff.Text = "% effectiveness";
            // 
            // zs_numReviveEff
            // 
            this.zs_numReviveEff.BackColor = System.Drawing.SystemColors.Window;
            this.zs_numReviveEff.Location = new System.Drawing.Point(150, 16);
            this.zs_numReviveEff.Maximum = new decimal(new int[] {
                                    1000000,
                                    0,
                                    0,
                                    0});
            this.zs_numReviveEff.Name = "zs_numReviveEff";
            this.zs_numReviveEff.Size = new System.Drawing.Size(52, 21);
            this.zs_numReviveEff.TabIndex = 39;
            this.zs_numReviveEff.Value = new decimal(new int[] {
                                    3,
                                    0,
                                    0,
                                    0});
            this.toolTip.SetToolTip(this.zs_numReviveEff, "Likelihood that /buy revive will disinfect a zombie");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(91, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "times, with";
            // 
            // zs_lblReviveLimitFtr
            // 
            this.zs_lblReviveLimitFtr.AutoSize = true;
            this.zs_lblReviveLimitFtr.Location = new System.Drawing.Point(167, 48);
            this.zs_lblReviveLimitFtr.Name = "zs_lblReviveLimitFtr";
            this.zs_lblReviveLimitFtr.Size = new System.Drawing.Size(100, 13);
            this.zs_lblReviveLimitFtr.TabIndex = 38;
            this.zs_lblReviveLimitFtr.Text = "seconds of infection";
            // 
            // zs_lblReviveLimitHdr
            // 
            this.zs_lblReviveLimitHdr.AutoSize = true;
            this.zs_lblReviveLimitHdr.Location = new System.Drawing.Point(7, 48);
            this.zs_lblReviveLimitHdr.Name = "zs_lblReviveLimitHdr";
            this.zs_lblReviveLimitHdr.Size = new System.Drawing.Size(102, 13);
            this.zs_lblReviveLimitHdr.TabIndex = 36;
            this.zs_lblReviveLimitHdr.Text = "Must be used within";
            // 
            // zs_numReviveLimit
            // 
            this.zs_numReviveLimit.BackColor = System.Drawing.SystemColors.Window;
            this.zs_numReviveLimit.Location = new System.Drawing.Point(112, 45);
            this.zs_numReviveLimit.Maximum = new decimal(new int[] {
                                    1000000,
                                    0,
                                    0,
                                    0});
            this.zs_numReviveLimit.Name = "zs_numReviveLimit";
            this.zs_numReviveLimit.Size = new System.Drawing.Size(52, 21);
            this.zs_numReviveLimit.TabIndex = 35;
            this.zs_numReviveLimit.Value = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            this.toolTip.SetToolTip(this.zs_numReviveLimit, "The time limit after a human is infected that /buy revive must be used within");
            // 
            // zs_numReviveMax
            // 
            this.zs_numReviveMax.BackColor = System.Drawing.SystemColors.Window;
            this.zs_numReviveMax.Location = new System.Drawing.Point(36, 17);
            this.zs_numReviveMax.Name = "zs_numReviveMax";
            this.zs_numReviveMax.Size = new System.Drawing.Size(52, 21);
            this.zs_numReviveMax.TabIndex = 34;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 34;
            this.label9.Text = "Max";
            // 
            // zs_cbMain
            // 
            this.zs_cbMain.AutoSize = true;
            this.zs_cbMain.Location = new System.Drawing.Point(11, 66);
            this.zs_cbMain.Name = "zs_cbMain";
            this.zs_cbMain.Size = new System.Drawing.Size(112, 17);
            this.zs_cbMain.TabIndex = 24;
            this.zs_cbMain.Text = "Change main level";
            this.zs_cbMain.UseVisualStyleBackColor = true;
            // 
            // zs_cbMap
            // 
            this.zs_cbMap.AutoSize = true;
            this.zs_cbMap.Location = new System.Drawing.Point(11, 43);
            this.zs_cbMap.Name = "zs_cbMap";
            this.zs_cbMap.Size = new System.Drawing.Size(136, 17);
            this.zs_cbMap.TabIndex = 23;
            this.zs_cbMap.Text = "Map name in server list";
            this.zs_cbMap.UseVisualStyleBackColor = true;
            // 
            // zs_grpInv
            // 
            this.zs_grpInv.Controls.Add(this.zs_numInvZombieDur);
            this.zs_grpInv.Controls.Add(this.zs_numInvHumanDur);
            this.zs_grpInv.Controls.Add(this.zs_numInvZombieMax);
            this.zs_grpInv.Controls.Add(this.zs_numInvHumanMax);
            this.zs_grpInv.Controls.Add(this.zs_lblInvZombieDur);
            this.zs_grpInv.Controls.Add(this.zs_lblInvZombieMax);
            this.zs_grpInv.Controls.Add(this.zs_lblInvHumanDur);
            this.zs_grpInv.Controls.Add(this.zs_lblInvHumanMax);
            this.zs_grpInv.Location = new System.Drawing.Point(6, 89);
            this.zs_grpInv.Name = "zs_grpInv";
            this.zs_grpInv.Size = new System.Drawing.Size(284, 74);
            this.zs_grpInv.TabIndex = 0;
            this.zs_grpInv.TabStop = false;
            this.zs_grpInv.Text = "Invisibility settings";
            // 
            // zs_numInvZombieDur
            // 
            this.zs_numInvZombieDur.BackColor = System.Drawing.SystemColors.Window;
            this.zs_numInvZombieDur.Location = new System.Drawing.Point(227, 45);
            this.zs_numInvZombieDur.Name = "zs_numInvZombieDur";
            this.zs_numInvZombieDur.Size = new System.Drawing.Size(52, 21);
            this.zs_numInvZombieDur.TabIndex = 33;
            this.toolTip.SetToolTip(this.zs_numInvZombieDur, "How many seconds a zombie is invisible for after using /buy invisibility");
            // 
            // zs_numInvHumanDur
            // 
            this.zs_numInvHumanDur.BackColor = System.Drawing.SystemColors.Window;
            this.zs_numInvHumanDur.Location = new System.Drawing.Point(227, 20);
            this.zs_numInvHumanDur.Name = "zs_numInvHumanDur";
            this.zs_numInvHumanDur.Size = new System.Drawing.Size(52, 21);
            this.zs_numInvHumanDur.TabIndex = 32;            
            this.toolTip.SetToolTip(this.zs_numInvHumanDur, "How many seconds a human is invisible for after using /buy invisibility");
            // 
            // zs_numInvZombieMax
            // 
            this.zs_numInvZombieMax.BackColor = System.Drawing.SystemColors.Window;
            this.zs_numInvZombieMax.Location = new System.Drawing.Point(77, 45);
            this.zs_numInvZombieMax.Name = "zs_numInvZombieMax";
            this.zs_numInvZombieMax.Size = new System.Drawing.Size(52, 21);
            this.zs_numInvZombieMax.TabIndex = 31;
            this.toolTip.SetToolTip(this.zs_numInvZombieMax, "Maximum number of times a zombie can use /buy invisibility in a round");
            // 
            // zs_numInvHumanMax
            // 
            this.zs_numInvHumanMax.BackColor = System.Drawing.SystemColors.Window;
            this.zs_numInvHumanMax.Location = new System.Drawing.Point(78, 20);
            this.zs_numInvHumanMax.Name = "zs_numInvHumanMax";
            this.zs_numInvHumanMax.Size = new System.Drawing.Size(52, 21);
            this.zs_numInvHumanMax.TabIndex = 27;
            this.toolTip.SetToolTip(this.zs_numInvHumanMax, "Maximum number of times a human can use /buy invisibility in a round");
            // 
            // zs_lblInvZombieDur
            // 
            this.zs_lblInvZombieDur.AutoSize = true;
            this.zs_lblInvZombieDur.Location = new System.Drawing.Point(129, 48);
            this.zs_lblInvZombieDur.Name = "zs_lblInvZombieDur";
            this.zs_lblInvZombieDur.Size = new System.Drawing.Size(100, 13);
            this.zs_lblInvZombieDur.TabIndex = 30;
            this.zs_lblInvZombieDur.Text = "times, which last for";
            // 
            // zs_lblInvZombieMax
            // 
            this.zs_lblInvZombieMax.AutoSize = true;
            this.zs_lblInvZombieMax.Location = new System.Drawing.Point(6, 48);
            this.zs_lblInvZombieMax.Name = "zs_lblInvZombieMax";
            this.zs_lblInvZombieMax.Size = new System.Drawing.Size(72, 13);
            this.zs_lblInvZombieMax.TabIndex = 29;
            this.zs_lblInvZombieMax.Text = "Zombies: max";
            // 
            // zs_lblInvHumanDur
            // 
            this.zs_lblInvHumanDur.AutoSize = true;
            this.zs_lblInvHumanDur.Location = new System.Drawing.Point(129, 22);
            this.zs_lblInvHumanDur.Name = "zs_lblInvHumanDur";
            this.zs_lblInvHumanDur.Size = new System.Drawing.Size(101, 13);
            this.zs_lblInvHumanDur.TabIndex = 28;
            this.zs_lblInvHumanDur.Text = "times, which last for";
            // 
            // zs_lblInvHumanMax
            // 
            this.zs_lblInvHumanMax.AutoSize = true;
            this.zs_lblInvHumanMax.Location = new System.Drawing.Point(7, 23);
            this.zs_lblInvHumanMax.Name = "zs_lblInvHumanMax";
            this.zs_lblInvHumanMax.Size = new System.Drawing.Size(71, 13);
            this.zs_lblInvHumanMax.TabIndex = 27;
            this.zs_lblInvHumanMax.Text = "Humans: max";
            // 
            // zs_cbStart
            // 
            this.zs_cbStart.AutoSize = true;
            this.zs_cbStart.Location = new System.Drawing.Point(11, 20);
            this.zs_cbStart.Name = "zs_cbStart";
            this.zs_cbStart.Size = new System.Drawing.Size(139, 17);
            this.zs_cbStart.TabIndex = 22;
            this.zs_cbStart.Text = "Start when server starts";
            this.zs_cbStart.UseVisualStyleBackColor = true;
            // 
            // zs_grpMaps
            // 
            this.zs_grpMaps.Controls.Add(this.zs_lblNotUsed);
            this.zs_grpMaps.Controls.Add(this.zs_lblUsed);
            this.zs_grpMaps.Controls.Add(this.zs_btnAdd);
            this.zs_grpMaps.Controls.Add(this.zs_btnRemove);
            this.zs_grpMaps.Controls.Add(this.zs_lstNotUsed);
            this.zs_grpMaps.Controls.Add(this.zs_lstUsed);
            this.zs_grpMaps.Location = new System.Drawing.Point(6, 59);
            this.zs_grpMaps.Name = "zs_grpMaps";
            this.zs_grpMaps.Size = new System.Drawing.Size(170, 412);
            this.zs_grpMaps.TabIndex = 1;
            this.zs_grpMaps.TabStop = false;
            this.zs_grpMaps.Text = "Maps";
            // 
            // zs_lblNotUsed
            // 
            this.zs_lblNotUsed.AutoSize = true;
            this.zs_lblNotUsed.Location = new System.Drawing.Point(187, 17);
            this.zs_lblNotUsed.Name = "zs_lblNotUsed";
            this.zs_lblNotUsed.Size = new System.Drawing.Size(83, 13);
            this.zs_lblNotUsed.TabIndex = 6;
            this.zs_lblNotUsed.Text = "Maps Not In Use";
            // 
            // zs_lblUsed
            // 
            this.zs_lblUsed.AutoSize = true;
            this.zs_lblUsed.Location = new System.Drawing.Point(6, 17);
            this.zs_lblUsed.Name = "zs_lblUsed";
            this.zs_lblUsed.Size = new System.Drawing.Size(38, 13);
            this.zs_lblUsed.TabIndex = 5;
            this.zs_lblUsed.Text = "In use:";
            // 
            // zs_btnAdd
            // 
            this.zs_btnAdd.Location = new System.Drawing.Point(6, 188);
            this.zs_btnAdd.Name = "zs_btnAdd";
            this.zs_btnAdd.Size = new System.Drawing.Size(77, 23);
            this.zs_btnAdd.TabIndex = 4;
            this.zs_btnAdd.Text = "<< Add";
            this.zs_btnAdd.UseVisualStyleBackColor = true;
            // 
            // zs_btnRemove
            // 
            this.zs_btnRemove.Location = new System.Drawing.Point(89, 188);
            this.zs_btnRemove.Name = "zs_btnRemove";
            this.zs_btnRemove.Size = new System.Drawing.Size(75, 23);
            this.zs_btnRemove.TabIndex = 3;
            this.zs_btnRemove.Text = "Remove >>";
            this.zs_btnRemove.UseVisualStyleBackColor = true;
            // 
            // zs_lstNotUsed
            // 
            this.zs_lstNotUsed.BackColor = System.Drawing.SystemColors.Window;
            this.zs_lstNotUsed.FormattingEnabled = true;
            this.zs_lstNotUsed.Location = new System.Drawing.Point(6, 219);
            this.zs_lstNotUsed.Name = "zs_lstNotUsed";
            this.zs_lstNotUsed.Size = new System.Drawing.Size(158, 186);
            this.zs_lstNotUsed.TabIndex = 2;
            // 
            // zs_lstUsed
            // 
            this.zs_lstUsed.BackColor = System.Drawing.SystemColors.Window;
            this.zs_lstUsed.FormattingEnabled = true;
            this.zs_lstUsed.Location = new System.Drawing.Point(6, 33);
            this.zs_lstUsed.Name = "zs_lstUsed";
            this.zs_lstUsed.Size = new System.Drawing.Size(158, 147);
            this.zs_lstUsed.TabIndex = 0;
            // 
            // tabZS_old
            // 
            this.tabZS_old.BackColor = System.Drawing.SystemColors.Control;
            this.tabZS_old.Controls.Add(this.propsZG);
            this.tabZS_old.Location = new System.Drawing.Point(4, 22);
            this.tabZS_old.Name = "tabZS_old";
            this.tabZS_old.Padding = new System.Windows.Forms.Padding(3);
            this.tabZS_old.Size = new System.Drawing.Size(484, 489);
            this.tabZS_old.TabIndex = 1;
            this.tabZS_old.Text = "Zombie old";
            // 
            // propsZG
            // 
            this.propsZG.Location = new System.Drawing.Point(6, 3);
            this.propsZG.Name = "propsZG";
            this.propsZG.Size = new System.Drawing.Size(456, 464);
            this.propsZG.TabIndex = 42;
            this.propsZG.ToolbarVisible = false;
            // 
            // tabCTF
            // 
            this.tabCTF.BackColor = System.Drawing.SystemColors.Control;
            this.tabCTF.Controls.Add(this.ctf_grpControls);
            this.tabCTF.Controls.Add(this.ctf_grpSettings);
            this.tabCTF.Controls.Add(this.ctf_grpMaps);
            this.tabCTF.Location = new System.Drawing.Point(4, 22);
            this.tabCTF.Name = "tabCTF";
            this.tabCTF.Size = new System.Drawing.Size(484, 489);
            this.tabCTF.TabIndex = 3;
            this.tabCTF.Text = "CTF";
            // 
            // ctf_grpControls
            // 
            this.ctf_grpControls.Controls.Add(this.ctf_btnEnd);
            this.ctf_grpControls.Controls.Add(this.ctf_btnStop);
            this.ctf_grpControls.Controls.Add(this.ctf_btnStart);
            this.ctf_grpControls.Location = new System.Drawing.Point(110, 5);
            this.ctf_grpControls.Name = "ctf_grpControls";
            this.ctf_grpControls.Size = new System.Drawing.Size(279, 51);
            this.ctf_grpControls.TabIndex = 7;
            this.ctf_grpControls.TabStop = false;
            this.ctf_grpControls.Text = "Controls";
            // 
            // ctf_btnEnd
            // 
            this.ctf_btnEnd.Location = new System.Drawing.Point(190, 19);
            this.ctf_btnEnd.Name = "ctf_btnEnd";
            this.ctf_btnEnd.Size = new System.Drawing.Size(80, 23);
            this.ctf_btnEnd.TabIndex = 2;
            this.ctf_btnEnd.Text = "End Round";
            this.ctf_btnEnd.UseVisualStyleBackColor = true;
            // 
            // ctf_btnStop
            // 
            this.ctf_btnStop.Location = new System.Drawing.Point(100, 19);
            this.ctf_btnStop.Name = "ctf_btnStop";
            this.ctf_btnStop.Size = new System.Drawing.Size(80, 23);
            this.ctf_btnStop.TabIndex = 1;
            this.ctf_btnStop.Text = "Stop Game";
            this.ctf_btnStop.UseVisualStyleBackColor = true;
            // 
            // ctf_btnStart
            // 
            this.ctf_btnStart.Location = new System.Drawing.Point(10, 19);
            this.ctf_btnStart.Name = "ctf_btnStart";
            this.ctf_btnStart.Size = new System.Drawing.Size(80, 23);
            this.ctf_btnStart.TabIndex = 0;
            this.ctf_btnStart.Text = "Start Game";
            this.ctf_btnStart.UseVisualStyleBackColor = true;
            // 
            // ctf_grpSettings
            // 
            this.ctf_grpSettings.Controls.Add(this.ctf_cbMain);
            this.ctf_grpSettings.Controls.Add(this.ctf_cbMap);
            this.ctf_grpSettings.Controls.Add(this.ctf_cbStart);
            this.ctf_grpSettings.Location = new System.Drawing.Point(182, 59);
            this.ctf_grpSettings.Name = "ctf_grpSettings";
            this.ctf_grpSettings.Size = new System.Drawing.Size(296, 89);
            this.ctf_grpSettings.TabIndex = 6;
            this.ctf_grpSettings.TabStop = false;
            this.ctf_grpSettings.Text = "Settings";
            // 
            // ctf_cbMain
            // 
            this.ctf_cbMain.AutoSize = true;
            this.ctf_cbMain.Location = new System.Drawing.Point(11, 66);
            this.ctf_cbMain.Name = "ctf_cbMain";
            this.ctf_cbMain.Size = new System.Drawing.Size(112, 17);
            this.ctf_cbMain.TabIndex = 24;
            this.ctf_cbMain.Text = "Change main level";
            this.ctf_cbMain.UseVisualStyleBackColor = true;
            // 
            // ctf_cbMap
            // 
            this.ctf_cbMap.AutoSize = true;
            this.ctf_cbMap.Location = new System.Drawing.Point(11, 43);
            this.ctf_cbMap.Name = "ctf_cbMap";
            this.ctf_cbMap.Size = new System.Drawing.Size(136, 17);
            this.ctf_cbMap.TabIndex = 23;
            this.ctf_cbMap.Text = "Map name in server list";
            this.ctf_cbMap.UseVisualStyleBackColor = true;
            // 
            // ctf_cbStart
            // 
            this.ctf_cbStart.AutoSize = true;
            this.ctf_cbStart.Location = new System.Drawing.Point(11, 20);
            this.ctf_cbStart.Name = "ctf_cbStart";
            this.ctf_cbStart.Size = new System.Drawing.Size(139, 17);
            this.ctf_cbStart.TabIndex = 22;
            this.ctf_cbStart.Text = "Start when server starts";
            this.ctf_cbStart.UseVisualStyleBackColor = true;
            // 
            // ctf_grpMaps
            // 
            this.ctf_grpMaps.Controls.Add(this.ctf_lblNotUsed);
            this.ctf_grpMaps.Controls.Add(this.ctf_lblUsed);
            this.ctf_grpMaps.Controls.Add(this.ctf_btnAdd);
            this.ctf_grpMaps.Controls.Add(this.ctf_btnRemove);
            this.ctf_grpMaps.Controls.Add(this.ctf_lstNotUsed);
            this.ctf_grpMaps.Controls.Add(this.ctf_lstUsed);
            this.ctf_grpMaps.Location = new System.Drawing.Point(6, 59);
            this.ctf_grpMaps.Name = "ctf_grpMaps";
            this.ctf_grpMaps.Size = new System.Drawing.Size(170, 412);
            this.ctf_grpMaps.TabIndex = 5;
            this.ctf_grpMaps.TabStop = false;
            this.ctf_grpMaps.Text = "Maps";
            // 
            // ctf_lblNotUsed
            // 
            this.ctf_lblNotUsed.AutoSize = true;
            this.ctf_lblNotUsed.Location = new System.Drawing.Point(187, 17);
            this.ctf_lblNotUsed.Name = "ctf_lblNotUsed";
            this.ctf_lblNotUsed.Size = new System.Drawing.Size(83, 13);
            this.ctf_lblNotUsed.TabIndex = 6;
            this.ctf_lblNotUsed.Text = "Maps Not In Use";
            // 
            // ctf_lblUsed
            // 
            this.ctf_lblUsed.AutoSize = true;
            this.ctf_lblUsed.Location = new System.Drawing.Point(6, 17);
            this.ctf_lblUsed.Name = "ctf_lblUsed";
            this.ctf_lblUsed.Size = new System.Drawing.Size(38, 13);
            this.ctf_lblUsed.TabIndex = 5;
            this.ctf_lblUsed.Text = "In use:";
            // 
            // ctf_btnAdd
            // 
            this.ctf_btnAdd.Location = new System.Drawing.Point(6, 188);
            this.ctf_btnAdd.Name = "ctf_btnAdd";
            this.ctf_btnAdd.Size = new System.Drawing.Size(77, 23);
            this.ctf_btnAdd.TabIndex = 4;
            this.ctf_btnAdd.Text = "<< Add";
            this.ctf_btnAdd.UseVisualStyleBackColor = true;
            // 
            // ctf_btnRemove
            // 
            this.ctf_btnRemove.Location = new System.Drawing.Point(89, 188);
            this.ctf_btnRemove.Name = "ctf_btnRemove";
            this.ctf_btnRemove.Size = new System.Drawing.Size(75, 23);
            this.ctf_btnRemove.TabIndex = 3;
            this.ctf_btnRemove.Text = "Remove >>";
            this.ctf_btnRemove.UseVisualStyleBackColor = true;
            // 
            // ctf_lstNotUsed
            // 
            this.ctf_lstNotUsed.BackColor = System.Drawing.SystemColors.Window;
            this.ctf_lstNotUsed.FormattingEnabled = true;
            this.ctf_lstNotUsed.Location = new System.Drawing.Point(6, 219);
            this.ctf_lstNotUsed.Name = "ctf_lstNotUsed";
            this.ctf_lstNotUsed.Size = new System.Drawing.Size(158, 186);
            this.ctf_lstNotUsed.TabIndex = 2;
            // 
            // ctf_lstUsed
            // 
            this.ctf_lstUsed.BackColor = System.Drawing.SystemColors.Window;
            this.ctf_lstUsed.FormattingEnabled = true;
            this.ctf_lstUsed.Location = new System.Drawing.Point(6, 33);
            this.ctf_lstUsed.Name = "ctf_lstUsed";
            this.ctf_lstUsed.Size = new System.Drawing.Size(158, 147);
            this.ctf_lstUsed.TabIndex = 0;
            // 
            // tabTW
            // 
            this.tabTW.BackColor = System.Drawing.SystemColors.Control;
            this.tabTW.Controls.Add(this.tw_grpControls);
            this.tabTW.Controls.Add(this.tw_grpMapSettings);
            this.tabTW.Controls.Add(this.tw_grpSettings);
            this.tabTW.Controls.Add(this.tw_gbMaps);
            this.tabTW.Location = new System.Drawing.Point(4, 22);
            this.tabTW.Name = "tabTW";
            this.tabTW.Size = new System.Drawing.Size(484, 489);
            this.tabTW.TabIndex = 4;
            this.tabTW.Text = "TNT Wars";
            // 
            // tw_grpControls
            // 
            this.tw_grpControls.Controls.Add(this.tw_btnEnd);
            this.tw_grpControls.Controls.Add(this.tw_btnStop);
            this.tw_grpControls.Controls.Add(this.tw_btnStart);
            this.tw_grpControls.Location = new System.Drawing.Point(110, 5);
            this.tw_grpControls.Name = "tw_grpControls";
            this.tw_grpControls.Size = new System.Drawing.Size(279, 51);
            this.tw_grpControls.TabIndex = 8;
            this.tw_grpControls.TabStop = false;
            this.tw_grpControls.Text = "Controls";
            // 
            // tw_btnEnd
            // 
            this.tw_btnEnd.Location = new System.Drawing.Point(190, 19);
            this.tw_btnEnd.Name = "tw_btnEnd";
            this.tw_btnEnd.Size = new System.Drawing.Size(80, 23);
            this.tw_btnEnd.TabIndex = 2;
            this.tw_btnEnd.Text = "End Round";
            this.tw_btnEnd.UseVisualStyleBackColor = true;
            // 
            // tw_btnStop
            // 
            this.tw_btnStop.Location = new System.Drawing.Point(100, 19);
            this.tw_btnStop.Name = "tw_btnStop";
            this.tw_btnStop.Size = new System.Drawing.Size(80, 23);
            this.tw_btnStop.TabIndex = 1;
            this.tw_btnStop.Text = "Stop Game";
            this.tw_btnStop.UseVisualStyleBackColor = true;
            // 
            // tw_btnStart
            // 
            this.tw_btnStart.Location = new System.Drawing.Point(10, 19);
            this.tw_btnStart.Name = "tw_btnStart";
            this.tw_btnStart.Size = new System.Drawing.Size(80, 23);
            this.tw_btnStart.TabIndex = 0;
            this.tw_btnStart.Text = "Start Game";
            this.tw_btnStart.UseVisualStyleBackColor = true;
            // 
            // tw_grpMapSettings
            // 
            this.tw_grpMapSettings.Controls.Add(this.tw_grpTeams);
            this.tw_grpMapSettings.Controls.Add(this.tw_grpGrace);
            this.tw_grpMapSettings.Controls.Add(this.tw_grpScores);
            this.tw_grpMapSettings.Enabled = false;
            this.tw_grpMapSettings.Location = new System.Drawing.Point(182, 213);
            this.tw_grpMapSettings.Name = "tw_grpMapSettings";
            this.tw_grpMapSettings.Size = new System.Drawing.Size(296, 207);
            this.tw_grpMapSettings.TabIndex = 7;
            this.tw_grpMapSettings.TabStop = false;
            this.tw_grpMapSettings.Text = "Map Settings";
            // 
            // tw_grpTeams
            // 
            this.tw_grpTeams.Controls.Add(this.tw_cbKills);
            this.tw_grpTeams.Controls.Add(this.tw_cbBalance);
            this.tw_grpTeams.Location = new System.Drawing.Point(172, 125);
            this.tw_grpTeams.Name = "tw_grpTeams";
            this.tw_grpTeams.Size = new System.Drawing.Size(118, 73);
            this.tw_grpTeams.TabIndex = 8;
            this.tw_grpTeams.TabStop = false;
            this.tw_grpTeams.Text = "Teams:";
            // 
            // tw_cbKills
            // 
            this.tw_cbKills.AutoSize = true;
            this.tw_cbKills.Location = new System.Drawing.Point(6, 43);
            this.tw_cbKills.Name = "tw_cbKills";
            this.tw_cbKills.Size = new System.Drawing.Size(81, 17);
            this.tw_cbKills.TabIndex = 2;
            this.tw_cbKills.Text = "Team killing";
            this.tw_cbKills.UseVisualStyleBackColor = true;
            // 
            // tw_cbBalance
            // 
            this.tw_cbBalance.AutoSize = true;
            this.tw_cbBalance.Checked = true;
            this.tw_cbBalance.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tw_cbBalance.Location = new System.Drawing.Point(6, 20);
            this.tw_cbBalance.Name = "tw_cbBalance";
            this.tw_cbBalance.Size = new System.Drawing.Size(96, 17);
            this.tw_cbBalance.TabIndex = 1;
            this.tw_cbBalance.Text = "Balance teams";
            this.tw_cbBalance.UseVisualStyleBackColor = true;
            // 
            // tw_grpGrace
            // 
            this.tw_grpGrace.Controls.Add(this.tw_numGrace);
            this.tw_grpGrace.Controls.Add(this.tw_lblGrace);
            this.tw_grpGrace.Controls.Add(this.tw_cbGrace);
            this.tw_grpGrace.Location = new System.Drawing.Point(6, 125);
            this.tw_grpGrace.Name = "tw_grpGrace";
            this.tw_grpGrace.Size = new System.Drawing.Size(160, 73);
            this.tw_grpGrace.TabIndex = 7;
            this.tw_grpGrace.TabStop = false;
            this.tw_grpGrace.Text = "Grace Period";
            // 
            // tw_numGrace
            // 
            this.tw_numGrace.BackColor = System.Drawing.SystemColors.Window;
            this.tw_numGrace.Location = new System.Drawing.Point(59, 41);
            this.tw_numGrace.Name = "tw_numGrace";
            this.tw_numGrace.Seconds = ((long)(30));
            this.tw_numGrace.Size = new System.Drawing.Size(66, 21);
            this.tw_numGrace.TabIndex = 35;
            this.tw_numGrace.Text = "30s";
            this.tw_numGrace.Value = System.TimeSpan.Parse("00:00:30");
            // 
            // tw_lblGrace
            // 
            this.tw_lblGrace.AutoSize = true;
            this.tw_lblGrace.Location = new System.Drawing.Point(23, 44);
            this.tw_lblGrace.Name = "tw_lblGrace";
            this.tw_lblGrace.Size = new System.Drawing.Size(33, 13);
            this.tw_lblGrace.TabIndex = 2;
            this.tw_lblGrace.Text = "Time:";
            // 
            // tw_cbGrace
            // 
            this.tw_cbGrace.AutoSize = true;
            this.tw_cbGrace.Checked = true;
            this.tw_cbGrace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tw_cbGrace.Location = new System.Drawing.Point(6, 20);
            this.tw_cbGrace.Name = "tw_cbGrace";
            this.tw_cbGrace.Size = new System.Drawing.Size(64, 17);
            this.tw_cbGrace.TabIndex = 0;
            this.tw_cbGrace.Text = "Enabled";
            this.tw_cbGrace.UseVisualStyleBackColor = true;
            // 
            // tw_grpScores
            // 
            this.tw_grpScores.Controls.Add(this.tw_lblMulti);
            this.tw_grpScores.Controls.Add(this.tw_lblAssist);
            this.tw_grpScores.Controls.Add(this.tw_cbStreaks);
            this.tw_grpScores.Controls.Add(this.tw_numMultiKills);
            this.tw_grpScores.Controls.Add(this.tw_numScoreAssists);
            this.tw_grpScores.Controls.Add(this.tw_lblScorePerKill);
            this.tw_grpScores.Controls.Add(this.tw_numScorePerKill);
            this.tw_grpScores.Controls.Add(this.tw_lblScoreLimit);
            this.tw_grpScores.Controls.Add(this.tw_numScoreLimit);
            this.tw_grpScores.Location = new System.Drawing.Point(6, 20);
            this.tw_grpScores.Name = "tw_grpScores";
            this.tw_grpScores.Size = new System.Drawing.Size(284, 99);
            this.tw_grpScores.TabIndex = 6;
            this.tw_grpScores.TabStop = false;
            this.tw_grpScores.Text = "Scores";
            // 
            // tw_lblMulti
            // 
            this.tw_lblMulti.AutoSize = true;
            this.tw_lblMulti.Location = new System.Drawing.Point(157, 47);
            this.tw_lblMulti.Name = "tw_lblMulti";
            this.tw_lblMulti.Size = new System.Drawing.Size(79, 13);
            this.tw_lblMulti.TabIndex = 10;
            this.tw_lblMulti.Text = "Multikill bonus:";
            // 
            // tw_lblAssist
            // 
            this.tw_lblAssist.AutoSize = true;
            this.tw_lblAssist.Location = new System.Drawing.Point(168, 20);
            this.tw_lblAssist.Name = "tw_lblAssist";
            this.tw_lblAssist.Size = new System.Drawing.Size(69, 13);
            this.tw_lblAssist.TabIndex = 9;
            this.tw_lblAssist.Text = "Assist bonus:";
            // 
            // tw_cbStreaks
            // 
            this.tw_cbStreaks.AutoSize = true;
            this.tw_cbStreaks.Checked = true;
            this.tw_cbStreaks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tw_cbStreaks.Location = new System.Drawing.Point(11, 72);
            this.tw_cbStreaks.Name = "tw_cbStreaks";
            this.tw_cbStreaks.Size = new System.Drawing.Size(61, 17);
            this.tw_cbStreaks.TabIndex = 0;
            this.tw_cbStreaks.Text = "Streaks";
            this.tw_cbStreaks.UseVisualStyleBackColor = true;
            // 
            // tw_numMultiKills
            // 
            this.tw_numMultiKills.BackColor = System.Drawing.SystemColors.Window;
            this.tw_numMultiKills.Location = new System.Drawing.Point(240, 44);
            this.tw_numMultiKills.Maximum = new decimal(new int[] {
                                    100000,
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
            // 
            // tw_numScoreAssists
            // 
            this.tw_numScoreAssists.BackColor = System.Drawing.SystemColors.Window;
            this.tw_numScoreAssists.Location = new System.Drawing.Point(240, 17);
            this.tw_numScoreAssists.Maximum = new decimal(new int[] {
                                    100000,
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
            // 
            // tw_lblScorePerKill
            // 
            this.tw_lblScorePerKill.AutoSize = true;
            this.tw_lblScorePerKill.Location = new System.Drawing.Point(9, 46);
            this.tw_lblScorePerKill.Name = "tw_lblScorePerKill";
            this.tw_lblScorePerKill.Size = new System.Drawing.Size(70, 13);
            this.tw_lblScorePerKill.TabIndex = 3;
            this.tw_lblScorePerKill.Text = "Score per kill:";
            // 
            // tw_numScorePerKill
            // 
            this.tw_numScorePerKill.BackColor = System.Drawing.SystemColors.Window;
            this.tw_numScorePerKill.Location = new System.Drawing.Point(82, 44);
            this.tw_numScorePerKill.Maximum = new decimal(new int[] {
                                    100000,
                                    0,
                                    0,
                                    0});
            this.tw_numScorePerKill.Name = "tw_numScorePerKill";
            this.tw_numScorePerKill.Size = new System.Drawing.Size(50, 21);
            this.tw_numScorePerKill.TabIndex = 2;
            this.tw_numScorePerKill.Value = new decimal(new int[] {
                                    10,
                                    0,
                                    0,
                                    0});
            // 
            // tw_lblScoreLimit
            // 
            this.tw_lblScoreLimit.AutoSize = true;
            this.tw_lblScoreLimit.Location = new System.Drawing.Point(5, 19);
            this.tw_lblScoreLimit.Name = "tw_lblScoreLimit";
            this.tw_lblScoreLimit.Size = new System.Drawing.Size(74, 13);
            this.tw_lblScoreLimit.TabIndex = 1;
            this.tw_lblScoreLimit.Text = "Score needed:";
            // 
            // tw_numScoreLimit
            // 
            this.tw_numScoreLimit.BackColor = System.Drawing.SystemColors.Window;
            this.tw_numScoreLimit.Location = new System.Drawing.Point(82, 17);
            this.tw_numScoreLimit.Maximum = new decimal(new int[] {
                                    100000,
                                    0,
                                    0,
                                    0});
            this.tw_numScoreLimit.Name = "tw_numScoreLimit";
            this.tw_numScoreLimit.Size = new System.Drawing.Size(50, 21);
            this.tw_numScoreLimit.TabIndex = 0;
            this.tw_numScoreLimit.Value = new decimal(new int[] {
                                    150,
                                    0,
                                    0,
                                    0});
            // 
            // tw_grpSettings
            // 
            this.tw_grpSettings.Controls.Add(this.tw_cmbMode);
            this.tw_grpSettings.Controls.Add(this.tw_cmbDiff);
            this.tw_grpSettings.Controls.Add(this.tw_lblMode);
            this.tw_grpSettings.Controls.Add(this.tw_lblDiff);
            this.tw_grpSettings.Controls.Add(this.tw_cbMain);
            this.tw_grpSettings.Controls.Add(this.tw_cbMap);
            this.tw_grpSettings.Controls.Add(this.tw_cbStart);
            this.tw_grpSettings.Location = new System.Drawing.Point(182, 59);
            this.tw_grpSettings.Name = "tw_grpSettings";
            this.tw_grpSettings.Size = new System.Drawing.Size(296, 148);
            this.tw_grpSettings.TabIndex = 6;
            this.tw_grpSettings.TabStop = false;
            this.tw_grpSettings.Text = "Settings";
            // 
            // tw_cmbMode
            // 
            this.tw_cmbMode.BackColor = System.Drawing.SystemColors.Window;
            this.tw_cmbMode.FormattingEnabled = true;
            this.tw_cmbMode.Items.AddRange(new object[] {
                                    "FFA",
                                    "TDM"});
            this.tw_cmbMode.Location = new System.Drawing.Point(74, 116);
            this.tw_cmbMode.Name = "tw_cmbMode";
            this.tw_cmbMode.Size = new System.Drawing.Size(76, 21);
            this.tw_cmbMode.TabIndex = 29;
            // 
            // tw_cmbDiff
            // 
            this.tw_cmbDiff.BackColor = System.Drawing.SystemColors.Window;
            this.tw_cmbDiff.FormattingEnabled = true;
            this.tw_cmbDiff.Items.AddRange(new object[] {
                                    "Easy",
                                    "Normal",
                                    "Hard",
                                    "Extreme"});
            this.tw_cmbDiff.Location = new System.Drawing.Point(74, 89);
            this.tw_cmbDiff.Name = "tw_cmbDiff";
            this.tw_cmbDiff.Size = new System.Drawing.Size(76, 21);
            this.tw_cmbDiff.TabIndex = 28;
            this.toolTip.SetToolTip(this.tw_cmbDiff, "Easy (2 Hits to die, TNT has long delay)\nNormal (2 Hits to die, TNT has normal delay)\n" +
                                    "Hard (1 Hit to die, TNT has short delay and team kills on)\nExtreme (1 Hit to die, TNT has short delay, big explosion and team kills on)");
            // 
            // tw_lblMode
            // 
            this.tw_lblMode.AutoSize = true;
            this.tw_lblMode.Location = new System.Drawing.Point(8, 119);
            this.tw_lblMode.Name = "tw_lblMode";
            this.tw_lblMode.Size = new System.Drawing.Size(65, 13);
            this.tw_lblMode.TabIndex = 27;
            this.tw_lblMode.Text = "Gamemode:";
            // 
            // tw_lblDiff
            // 
            this.tw_lblDiff.AutoSize = true;
            this.tw_lblDiff.Location = new System.Drawing.Point(21, 94);
            this.tw_lblDiff.Name = "tw_lblDiff";
            this.tw_lblDiff.Size = new System.Drawing.Size(52, 13);
            this.tw_lblDiff.TabIndex = 26;
            this.tw_lblDiff.Text = "Difficulty:";
            // 
            // tw_cbMain
            // 
            this.tw_cbMain.AutoSize = true;
            this.tw_cbMain.Location = new System.Drawing.Point(11, 66);
            this.tw_cbMain.Name = "tw_cbMain";
            this.tw_cbMain.Size = new System.Drawing.Size(112, 17);
            this.tw_cbMain.TabIndex = 24;
            this.tw_cbMain.Text = "Change main level";
            this.tw_cbMain.UseVisualStyleBackColor = true;
            // 
            // tw_cbMap
            // 
            this.tw_cbMap.AutoSize = true;
            this.tw_cbMap.Location = new System.Drawing.Point(11, 43);
            this.tw_cbMap.Name = "tw_cbMap";
            this.tw_cbMap.Size = new System.Drawing.Size(136, 17);
            this.tw_cbMap.TabIndex = 23;
            this.tw_cbMap.Text = "Map name in server list";
            this.tw_cbMap.UseVisualStyleBackColor = true;
            // 
            // tw_cbStart
            // 
            this.tw_cbStart.AutoSize = true;
            this.tw_cbStart.Location = new System.Drawing.Point(11, 20);
            this.tw_cbStart.Name = "tw_cbStart";
            this.tw_cbStart.Size = new System.Drawing.Size(139, 17);
            this.tw_cbStart.TabIndex = 22;
            this.tw_cbStart.Text = "Start when server starts";
            this.tw_cbStart.UseVisualStyleBackColor = true;
            // 
            // tw_gbMaps
            // 
            this.tw_gbMaps.Controls.Add(this.tw_lblInUse);
            this.tw_gbMaps.Controls.Add(this.tw_btnAdd);
            this.tw_gbMaps.Controls.Add(this.tw_btnRemove);
            this.tw_gbMaps.Controls.Add(this.tw_lstNotUsed);
            this.tw_gbMaps.Controls.Add(this.tw_lstUsed);
            this.tw_gbMaps.Location = new System.Drawing.Point(6, 59);
            this.tw_gbMaps.Name = "tw_gbMaps";
            this.tw_gbMaps.Size = new System.Drawing.Size(170, 412);
            this.tw_gbMaps.TabIndex = 5;
            this.tw_gbMaps.TabStop = false;
            this.tw_gbMaps.Text = "Maps";
            // 
            // tw_lblInUse
            // 
            this.tw_lblInUse.AutoSize = true;
            this.tw_lblInUse.Location = new System.Drawing.Point(6, 17);
            this.tw_lblInUse.Name = "tw_lblInUse";
            this.tw_lblInUse.Size = new System.Drawing.Size(38, 13);
            this.tw_lblInUse.TabIndex = 5;
            this.tw_lblInUse.Text = "In use:";
            // 
            // tw_btnAdd
            // 
            this.tw_btnAdd.Location = new System.Drawing.Point(6, 188);
            this.tw_btnAdd.Name = "tw_btnAdd";
            this.tw_btnAdd.Size = new System.Drawing.Size(77, 23);
            this.tw_btnAdd.TabIndex = 4;
            this.tw_btnAdd.Text = "<< Add";
            this.tw_btnAdd.UseVisualStyleBackColor = true;
            // 
            // tw_btnRemove
            // 
            this.tw_btnRemove.Location = new System.Drawing.Point(89, 188);
            this.tw_btnRemove.Name = "tw_btnRemove";
            this.tw_btnRemove.Size = new System.Drawing.Size(75, 23);
            this.tw_btnRemove.TabIndex = 3;
            this.tw_btnRemove.Text = "Remove >>";
            this.tw_btnRemove.UseVisualStyleBackColor = true;
            // 
            // tw_lstNotUsed
            // 
            this.tw_lstNotUsed.BackColor = System.Drawing.SystemColors.Window;
            this.tw_lstNotUsed.FormattingEnabled = true;
            this.tw_lstNotUsed.Location = new System.Drawing.Point(6, 219);
            this.tw_lstNotUsed.Name = "tw_lstNotUsed";
            this.tw_lstNotUsed.Size = new System.Drawing.Size(158, 186);
            this.tw_lstNotUsed.TabIndex = 2;
            // 
            // tw_lstUsed
            // 
            this.tw_lstUsed.BackColor = System.Drawing.SystemColors.Window;
            this.tw_lstUsed.FormattingEnabled = true;
            this.tw_lstUsed.Location = new System.Drawing.Point(6, 33);
            this.tw_lstUsed.Name = "tw_lstUsed";
            this.tw_lstUsed.Size = new System.Drawing.Size(158, 147);
            this.tw_lstUsed.TabIndex = 0;
            this.tw_lstUsed.SelectedIndexChanged += new System.EventHandler(this.twMapUse_SelectedIndexChanged);
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
            this.cmd_cmbExtra7.BackColor = System.Drawing.SystemColors.Window;
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
            this.cmd_cmbExtra6.BackColor = System.Drawing.SystemColors.Window;
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
            this.cmd_cmbExtra5.BackColor = System.Drawing.SystemColors.Window;
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
            this.cmd_cmbExtra4.BackColor = System.Drawing.SystemColors.Window;
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
            this.cmd_cmbExtra3.BackColor = System.Drawing.SystemColors.Window;
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
            this.cmd_cmbExtra2.BackColor = System.Drawing.SystemColors.Window;
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
            this.cmd_cmbExtra1.BackColor = System.Drawing.SystemColors.Window;
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
            this.cmd_cmbAlw3.BackColor = System.Drawing.SystemColors.Window;
            this.cmd_cmbAlw3.FormattingEnabled = true;
            this.cmd_cmbAlw3.Location = new System.Drawing.Point(274, 67);
            this.cmd_cmbAlw3.Name = "cmd_cmbAlw3";
            this.cmd_cmbAlw3.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbAlw3.TabIndex = 28;
            this.cmd_cmbAlw3.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbAlw2
            // 
            this.cmd_cmbAlw2.BackColor = System.Drawing.SystemColors.Window;
            this.cmd_cmbAlw2.FormattingEnabled = true;
            this.cmd_cmbAlw2.Location = new System.Drawing.Point(187, 67);
            this.cmd_cmbAlw2.Name = "cmd_cmbAlw2";
            this.cmd_cmbAlw2.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbAlw2.TabIndex = 27;
            this.cmd_cmbAlw2.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbDis3
            // 
            this.cmd_cmbDis3.BackColor = System.Drawing.SystemColors.Window;
            this.cmd_cmbDis3.FormattingEnabled = true;
            this.cmd_cmbDis3.Location = new System.Drawing.Point(274, 41);
            this.cmd_cmbDis3.Name = "cmd_cmbDis3";
            this.cmd_cmbDis3.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbDis3.TabIndex = 26;
            this.cmd_cmbDis3.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbDis2
            // 
            this.cmd_cmbDis2.BackColor = System.Drawing.SystemColors.Window;
            this.cmd_cmbDis2.FormattingEnabled = true;
            this.cmd_cmbDis2.Location = new System.Drawing.Point(187, 41);
            this.cmd_cmbDis2.Name = "cmd_cmbDis2";
            this.cmd_cmbDis2.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbDis2.TabIndex = 25;
            this.cmd_cmbDis2.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbAlw1
            // 
            this.cmd_cmbAlw1.BackColor = System.Drawing.SystemColors.Window;
            this.cmd_cmbAlw1.FormattingEnabled = true;
            this.cmd_cmbAlw1.Location = new System.Drawing.Point(100, 67);
            this.cmd_cmbAlw1.Name = "cmd_cmbAlw1";
            this.cmd_cmbAlw1.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbAlw1.TabIndex = 24;
            this.cmd_cmbAlw1.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbDis1
            // 
            this.cmd_cmbDis1.BackColor = System.Drawing.SystemColors.Window;
            this.cmd_cmbDis1.FormattingEnabled = true;
            this.cmd_cmbDis1.Location = new System.Drawing.Point(100, 41);
            this.cmd_cmbDis1.Name = "cmd_cmbDis1";
            this.cmd_cmbDis1.Size = new System.Drawing.Size(81, 21);
            this.cmd_cmbDis1.TabIndex = 23;
            this.cmd_cmbDis1.SelectedIndexChanged += new System.EventHandler(this.cmd_cmbSpecific_SelectedIndexChanged);
            // 
            // cmd_cmbMin
            // 
            this.cmd_cmbMin.BackColor = System.Drawing.SystemColors.Window;
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
            this.cmd_list.BackColor = System.Drawing.SystemColors.Window;
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
            this.sec_grpChat.Controls.Add(this.sec_lblChatForMute);
            this.sec_grpChat.Controls.Add(this.sec_numChatMute);
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
            this.sec_numChatMsgs.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numChatMsgs.Location = new System.Drawing.Point(53, 45);
            this.sec_numChatMsgs.Maximum = new decimal(new int[] {
                                    10000,
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
            this.sec_numChatSecs.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numChatSecs.Location = new System.Drawing.Point(156, 45);
            this.sec_numChatSecs.Name = "sec_numChatSecs";
            this.sec_numChatSecs.Seconds = ((long)(5));
            this.sec_numChatSecs.Size = new System.Drawing.Size(62, 21);
            this.sec_numChatSecs.TabIndex = 34;
            this.sec_numChatSecs.Text = "5s";
            this.sec_numChatSecs.Value = System.TimeSpan.Parse("00:00:05");
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
            this.sec_numChatMute.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numChatMute.Location = new System.Drawing.Point(53, 79);
            this.sec_numChatMute.Name = "sec_numChatMute";
            this.sec_numChatMute.Seconds = ((long)(60));
            this.sec_numChatMute.Size = new System.Drawing.Size(62, 21);
            this.sec_numChatMute.TabIndex = 32;
            this.sec_numChatMute.Text = "1m";
            this.sec_numChatMute.Value = System.TimeSpan.Parse("00:01:00");
            // 
            // sec_grpCmd
            // 
            this.sec_grpCmd.Controls.Add(this.sec_cbCmdAuto);
            this.sec_grpCmd.Controls.Add(this.sec_lblCmdOnMute);
            this.sec_grpCmd.Controls.Add(this.sec_numCmdMsgs);
            this.sec_grpCmd.Controls.Add(this.sec_lblCmdOnMsgs);
            this.sec_grpCmd.Controls.Add(this.sec_numCmdSecs);
            this.sec_grpCmd.Controls.Add(this.sec_lblCmdForMute);
            this.sec_grpCmd.Controls.Add(this.sec_numCmdMute);
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
            this.sec_numCmdMsgs.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numCmdMsgs.Location = new System.Drawing.Point(53, 45);
            this.sec_numCmdMsgs.Maximum = new decimal(new int[] {
                                    10000,
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
            this.sec_numCmdSecs.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numCmdSecs.Location = new System.Drawing.Point(161, 45);
            this.sec_numCmdSecs.Name = "sec_numCmdSecs";
            this.sec_numCmdSecs.Seconds = ((long)(1));
            this.sec_numCmdSecs.Size = new System.Drawing.Size(62, 21);
            this.sec_numCmdSecs.TabIndex = 34;
            this.sec_numCmdSecs.Text = "1s";
            this.sec_numCmdSecs.Value = System.TimeSpan.Parse("00:00:01");
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
            this.sec_numCmdMute.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numCmdMute.Location = new System.Drawing.Point(53, 79);
            this.sec_numCmdMute.Name = "sec_numCmdMute";
            this.sec_numCmdMute.Seconds = ((long)(60));
            this.sec_numCmdMute.Size = new System.Drawing.Size(62, 21);
            this.sec_numCmdMute.TabIndex = 32;
            this.sec_numCmdMute.Text = "1m";
            this.sec_numCmdMute.Value = System.TimeSpan.Parse("00:01:00");
            // 
            // sec_grpIP
            // 
            this.sec_grpIP.Controls.Add(this.sec_cbIPAuto);
            this.sec_grpIP.Controls.Add(this.sec_lblIPOnMute);
            this.sec_grpIP.Controls.Add(this.sec_numIPMsgs);
            this.sec_grpIP.Controls.Add(this.sec_lblIPOnMsgs);
            this.sec_grpIP.Controls.Add(this.sec_numIPSecs);
            this.sec_grpIP.Controls.Add(this.sec_lblIPForMute);
            this.sec_grpIP.Controls.Add(this.sec_numIPMute);
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
            this.sec_numIPMsgs.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numIPMsgs.Location = new System.Drawing.Point(53, 45);
            this.sec_numIPMsgs.Maximum = new decimal(new int[] {
                                    10000,
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
            this.sec_numIPSecs.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numIPSecs.Location = new System.Drawing.Point(166, 45);
            this.sec_numIPSecs.Name = "sec_numIPSecs";
            this.sec_numIPSecs.Seconds = ((long)(1));
            this.sec_numIPSecs.Size = new System.Drawing.Size(62, 21);
            this.sec_numIPSecs.TabIndex = 34;
            this.sec_numIPSecs.Text = "1s";
            this.sec_numIPSecs.Value = System.TimeSpan.Parse("00:00:01");
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
            this.sec_numIPMute.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numIPMute.Location = new System.Drawing.Point(53, 79);
            this.sec_numIPMute.Name = "sec_numIPMute";
            this.sec_numIPMute.Seconds = ((long)(300));
            this.sec_numIPMute.Size = new System.Drawing.Size(62, 21);
            this.sec_numIPMute.TabIndex = 32;
            this.sec_numIPMute.Text = "5m";
            this.sec_numIPMute.Value = System.TimeSpan.Parse("00:05:00");
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
            this.sec_numBlocksMsgs.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numBlocksMsgs.Location = new System.Drawing.Point(46, 45);
            this.sec_numBlocksMsgs.Maximum = new decimal(new int[] {
                                    10000,
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
            this.sec_numBlocksSecs.BackColor = System.Drawing.SystemColors.Window;
            this.sec_numBlocksSecs.Location = new System.Drawing.Point(142, 45);
            this.sec_numBlocksSecs.Name = "sec_numBlocksSecs";
            this.sec_numBlocksSecs.Seconds = ((long)(5));
            this.sec_numBlocksSecs.Size = new System.Drawing.Size(62, 21);
            this.sec_numBlocksSecs.TabIndex = 34;
            this.sec_numBlocksSecs.Text = "5s";
            this.sec_numBlocksSecs.Value = System.TimeSpan.Parse("00:00:05");
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
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.PropertyWindow_Load);
            this.Disposed += new System.EventHandler(this.PropertyWindow_Unload);
            this.pageChat.ResumeLayout(false);
            this.dis_grp.ResumeLayout(false);
            this.dis_grp.PerformLayout();
            this.chat_grpTab.ResumeLayout(false);
            this.chat_grpTab.PerformLayout();
            this.chat_grpMessages.ResumeLayout(false);
            this.chat_grpMessages.PerformLayout();
            this.chat_grpOther.ResumeLayout(false);
            this.chat_grpOther.PerformLayout();
            this.chat_grpColors.ResumeLayout(false);
            this.chat_grpColors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srv_numPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.irc_numPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numPerm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numMaps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numDraw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numGen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rank_numUndo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numKiller)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numFast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numWater)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numDestroy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numLayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numHeight)).EndInit();
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
            this.grpExtra.ResumeLayout(false);
            this.grpExtra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.misc_numReview)).EndInit();
            this.grpMessages.ResumeLayout(false);
            this.grpMessages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hack_num)).EndInit();
            this.grpPhysics.ResumeLayout(false);
            this.grpPhysics.PerformLayout();
            this.afk_grp.ResumeLayout(false);
            this.afk_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.afk_numTimer)).EndInit();
            this.bak_grp.ResumeLayout(false);
            this.bak_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bak_numTime)).EndInit();
            this.pageRelay.ResumeLayout(false);
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
            this.pageEco.ResumeLayout(false);
            this.eco_gbRank.ResumeLayout(false);
            this.eco_gbRank.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eco_dgvRanks)).EndInit();
            this.eco_gbLvl.ResumeLayout(false);
            this.eco_gbLvl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eco_dgvMaps)).EndInit();
            this.eco_gbItem.ResumeLayout(false);
            this.eco_gbItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eco_numItemPrice)).EndInit();
            this.eco_gb.ResumeLayout(false);
            this.eco_gb.PerformLayout();
            this.pageGames.ResumeLayout(false);
            this.tabGames.ResumeLayout(false);
            this.tabLS.ResumeLayout(false);
            this.ls_grpControls.ResumeLayout(false);
            this.ls_grpMapSettings.ResumeLayout(false);
            this.ls_grpTime.ResumeLayout(false);
            this.ls_grpTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numFlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numLayerTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ls_numRound)).EndInit();
            this.ls_grpLayer.ResumeLayout(false);
            this.ls_grpLayer.PerformLayout();
            this.ls_grpBlock.ResumeLayout(false);
            this.ls_grpBlock.PerformLayout();
            this.ls_grpSettings.ResumeLayout(false);
            this.ls_grpSettings.PerformLayout();
            this.ls_grpMaps.ResumeLayout(false);
            this.ls_grpMaps.PerformLayout();
            this.tabZS.ResumeLayout(false);
            this.zs_grpControls.ResumeLayout(false);
            this.zs_grpMap.ResumeLayout(false);
            this.zs_grpTime.ResumeLayout(false);
            this.zs_grpTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timespanUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timespanUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timespanUpDown3)).EndInit();
            this.zs_grpSettings.ResumeLayout(false);
            this.zs_grpSettings.PerformLayout();
            this.zs_grpZombie.ResumeLayout(false);
            this.zs_grpZombie.PerformLayout();
            this.zs_grpRevive.ResumeLayout(false);
            this.zs_grpRevive.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numReviveEff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numReviveLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numReviveMax)).EndInit();
            this.zs_grpInv.ResumeLayout(false);
            this.zs_grpInv.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numInvZombieDur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numInvHumanDur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numInvZombieMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zs_numInvHumanMax)).EndInit();
            this.zs_grpMaps.ResumeLayout(false);
            this.zs_grpMaps.PerformLayout();
            this.tabZS_old.ResumeLayout(false);
            this.tabCTF.ResumeLayout(false);
            this.ctf_grpControls.ResumeLayout(false);
            this.ctf_grpSettings.ResumeLayout(false);
            this.ctf_grpSettings.PerformLayout();
            this.ctf_grpMaps.ResumeLayout(false);
            this.ctf_grpMaps.PerformLayout();
            this.tabTW.ResumeLayout(false);
            this.tw_grpControls.ResumeLayout(false);
            this.tw_grpMapSettings.ResumeLayout(false);
            this.tw_grpTeams.ResumeLayout(false);
            this.tw_grpTeams.PerformLayout();
            this.tw_grpGrace.ResumeLayout(false);
            this.tw_grpGrace.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numGrace)).EndInit();
            this.tw_grpScores.ResumeLayout(false);
            this.tw_grpScores.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numMultiKills)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScoreAssists)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScorePerKill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tw_numScoreLimit)).EndInit();
            this.tw_grpSettings.ResumeLayout(false);
            this.tw_grpSettings.PerformLayout();
            this.tw_gbMaps.ResumeLayout(false);
            this.tw_gbMaps.PerformLayout();
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
        private System.Windows.Forms.TextBox zs_txtName;
        private System.Windows.Forms.TextBox zs_txtModel;
        private System.Windows.Forms.Label zs_lblName;
        private System.Windows.Forms.Label zs_lblModel;
        private System.Windows.Forms.GroupBox zs_grpZombie;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown zs_numReviveMax;
        private System.Windows.Forms.NumericUpDown zs_numReviveLimit;
        private System.Windows.Forms.Label zs_lblReviveLimitHdr;
        private System.Windows.Forms.Label zs_lblReviveLimitFtr;
        private System.Windows.Forms.GroupBox zs_grpRevive;
        private System.Windows.Forms.ListBox zs_lstUsed;
        private System.Windows.Forms.ListBox zs_lstNotUsed;
        private System.Windows.Forms.Button zs_btnRemove;
        private System.Windows.Forms.Button zs_btnAdd;
        private System.Windows.Forms.Label zs_lblUsed;
        private System.Windows.Forms.Label zs_lblNotUsed;
        private System.Windows.Forms.GroupBox zs_grpMaps;
        private System.Windows.Forms.CheckBox zs_cbStart;
        private System.Windows.Forms.CheckBox zs_cbMap;
        private System.Windows.Forms.CheckBox zs_cbMain;
        private System.Windows.Forms.GroupBox zs_grpSettings;
        private System.Windows.Forms.Label zs_lblInvHumanMax;
        private System.Windows.Forms.Label zs_lblInvHumanDur;
        private System.Windows.Forms.Label zs_lblInvZombieMax;
        private System.Windows.Forms.Label zs_lblInvZombieDur;
        private System.Windows.Forms.NumericUpDown zs_numInvHumanMax;
        private System.Windows.Forms.NumericUpDown zs_numInvZombieMax;
        private System.Windows.Forms.NumericUpDown zs_numInvHumanDur;
        private System.Windows.Forms.NumericUpDown zs_numInvZombieDur;
        private System.Windows.Forms.GroupBox zs_grpInv;
        private System.Windows.Forms.Label zs_lblReviveEff;
        private System.Windows.Forms.NumericUpDown zs_numReviveEff;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private MCGalaxy.Gui.TimespanUpDown timespanUpDown3;
        private MCGalaxy.Gui.TimespanUpDown timespanUpDown2;
        private MCGalaxy.Gui.TimespanUpDown timespanUpDown1;
        private System.Windows.Forms.GroupBox zs_grpTime;
        private System.Windows.Forms.GroupBox zs_grpMap;
        private System.Windows.Forms.Button zs_btnStart;
        private System.Windows.Forms.Button zs_btnStop;
        private System.Windows.Forms.Button zs_btnEnd;
        private System.Windows.Forms.GroupBox zs_grpControls;
        private System.Windows.Forms.TabPage tabZS;
        private System.Windows.Forms.CheckBox chkPhysRestart;
        private System.Windows.Forms.Label ls_lblRound;
        private System.Windows.Forms.Label ls_lblFlood;
        private System.Windows.Forms.Label ls_lblLayerTime;
        private MCGalaxy.Gui.TimespanUpDown ls_numRound;
        private MCGalaxy.Gui.TimespanUpDown ls_numLayerTime;
        private MCGalaxy.Gui.TimespanUpDown ls_numFlood;
        private MCGalaxy.Gui.TimespanUpDown misc_numReview;
        private MCGalaxy.Gui.TimespanUpDown hack_num;
        private MCGalaxy.Gui.TimespanUpDown afk_numTimer;
        private System.Windows.Forms.Label tw_lblAssist;
        private System.Windows.Forms.Label tw_lblMulti;
        private System.Windows.Forms.Button chat_btnWarn;
        private System.Windows.Forms.Label chat_lblWarn;
        private System.Windows.Forms.Label tw_lblMode;
        private System.Windows.Forms.ComboBox tw_cmbDiff;
        private System.Windows.Forms.ComboBox tw_cmbMode;
        private System.Windows.Forms.ListBox tw_lstUsed;
        private System.Windows.Forms.ListBox tw_lstNotUsed;
        private System.Windows.Forms.Button tw_btnRemove;
        private System.Windows.Forms.Button tw_btnAdd;
        private System.Windows.Forms.Label tw_lblInUse;
        private System.Windows.Forms.GroupBox tw_gbMaps;
        private System.Windows.Forms.CheckBox tw_cbStart;
        private System.Windows.Forms.CheckBox tw_cbMap;
        private System.Windows.Forms.CheckBox tw_cbMain;
        private System.Windows.Forms.Label tw_lblDiff;
        private System.Windows.Forms.GroupBox tw_grpSettings;
        private System.Windows.Forms.GroupBox tw_grpMapSettings;
        private System.Windows.Forms.Button tw_btnStart;
        private System.Windows.Forms.Button tw_btnStop;
        private System.Windows.Forms.Button tw_btnEnd;
        private System.Windows.Forms.GroupBox tw_grpControls;
        private System.Windows.Forms.TabPage tabTW;
        private System.Windows.Forms.ListBox ctf_lstUsed;
        private System.Windows.Forms.ListBox ctf_lstNotUsed;
        private System.Windows.Forms.Button ctf_btnRemove;
        private System.Windows.Forms.Button ctf_btnAdd;
        private System.Windows.Forms.Label ctf_lblUsed;
        private System.Windows.Forms.Label ctf_lblNotUsed;
        private System.Windows.Forms.GroupBox ctf_grpMaps;
        private System.Windows.Forms.CheckBox ctf_cbStart;
        private System.Windows.Forms.CheckBox ctf_cbMap;
        private System.Windows.Forms.CheckBox ctf_cbMain;
        private System.Windows.Forms.GroupBox ctf_grpSettings;
        private System.Windows.Forms.Button ctf_btnStart;
        private System.Windows.Forms.Button ctf_btnStop;
        private System.Windows.Forms.Button ctf_btnEnd;
        private System.Windows.Forms.GroupBox ctf_grpControls;
        private System.Windows.Forms.TabPage tabCTF;
        private System.Windows.Forms.GroupBox ls_grpTime;
        private System.Windows.Forms.Label ls_lblLayer;
        private System.Windows.Forms.NumericUpDown ls_numLayer;
        private System.Windows.Forms.NumericUpDown ls_numCount;
        private System.Windows.Forms.Label ls_lblLayersEach;
        private System.Windows.Forms.NumericUpDown ls_numHeight;
        private System.Windows.Forms.Label ls_lblBlocksTall;
        private System.Windows.Forms.Label ls_lblKill;
        private System.Windows.Forms.Label ls_lblWater;
        private System.Windows.Forms.Label ls_lblFast;
        private System.Windows.Forms.Label ls_lblDestroy;
        private System.Windows.Forms.NumericUpDown ls_numKiller;
        private System.Windows.Forms.NumericUpDown ls_numFast;
        private System.Windows.Forms.NumericUpDown ls_numWater;
        private System.Windows.Forms.NumericUpDown ls_numDestroy;
        private System.Windows.Forms.GroupBox ls_grpBlock;
        private System.Windows.Forms.GroupBox ls_grpLayer;
        private System.Windows.Forms.CheckBox ls_cbMain;
        private System.Windows.Forms.NumericUpDown ls_numMax;
        private System.Windows.Forms.Label ls_lblMax;
        private System.Windows.Forms.CheckBox ls_cbStart;
        private System.Windows.Forms.CheckBox ls_cbMap;
        private System.Windows.Forms.DataGridViewTextBoxColumn eco_colRankPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn eco_colRankName;
        private System.Windows.Forms.DataGridView eco_dgvRanks;

        private System.Windows.Forms.Label eco_lblItemRank;
        private System.Windows.Forms.ComboBox eco_cmbItemRank;
        private System.Windows.Forms.Label eco_lblCfg;
        private System.Windows.Forms.ComboBox eco_cmbCfg;
        private System.Windows.Forms.CheckBox eco_cbItem;
        private System.Windows.Forms.Label eco_lblItemPrice;
        private System.Windows.Forms.NumericUpDown eco_numItemPrice;
        private System.Windows.Forms.GroupBox eco_gbItem;
        private System.Windows.Forms.CheckBox eco_cbRank;
        private System.Windows.Forms.GroupBox eco_gbRank;
        private System.Windows.Forms.CheckBox eco_cbLvl;
        private System.Windows.Forms.Button eco_btnLvlAdd;
        private System.Windows.Forms.Button eco_btnLvlDel;
        private System.Windows.Forms.DataGridViewComboBoxColumn eco_colLvlTheme;
        private System.Windows.Forms.DataGridViewTextBoxColumn eco_colLvlZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn eco_colLvlY;
        private System.Windows.Forms.DataGridViewTextBoxColumn eco_colLvlX;
        private System.Windows.Forms.DataGridViewTextBoxColumn eco_colLvlPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn eco_colLvlName;
        private System.Windows.Forms.DataGridView eco_dgvMaps;
        private System.Windows.Forms.GroupBox eco_gbLvl;
        private System.Windows.Forms.Label eco_lblCurrency;
        private System.Windows.Forms.TextBox eco_txtCurrency;
        private System.Windows.Forms.CheckBox eco_cbEnabled;
        private System.Windows.Forms.GroupBox eco_gb;
        private System.Windows.Forms.TabPage pageEco;
        private System.Windows.Forms.CheckBox chkRestart;
        private System.Windows.Forms.Label rank_lblCopy;
        private System.Windows.Forms.NumericUpDown rank_numCopy;
        private System.Windows.Forms.CheckBox rank_cbAfk;
        private MCGalaxy.Gui.TimespanUpDown rank_numAfk;
        private System.Windows.Forms.Label rank_lblUndo;
        private MCGalaxy.Gui.TimespanUpDown rank_numUndo;
        private System.Windows.Forms.Label rank_lblDraw;
        private System.Windows.Forms.NumericUpDown rank_numDraw;
        private System.Windows.Forms.NumericUpDown rank_numMaps;
        private System.Windows.Forms.Label rank_lblMaps;
        private System.Windows.Forms.NumericUpDown rank_numGen;
        private System.Windows.Forms.Label rank_lblGen;
        private System.Windows.Forms.GroupBox rank_grpLimits;
        private System.Windows.Forms.Label irc_lblRank;
        private System.Windows.Forms.ComboBox irc_cmbRank;
        private System.Windows.Forms.Label irc_lblVerify;
        private System.Windows.Forms.ComboBox irc_cmbVerify;
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
        
        private System.Windows.Forms.TextBox dis_txtOpChannel;
        private System.Windows.Forms.Label dis_lblOpChannel;
        private System.Windows.Forms.TextBox dis_txtChannel;
        private System.Windows.Forms.Label dis_lblChannel;
        private System.Windows.Forms.TextBox dis_txtToken;
        private System.Windows.Forms.Label dis_lblToken;
        private System.Windows.Forms.CheckBox dis_chkEnabled;
        private System.Windows.Forms.CheckBox dis_chkNicks;
        private System.Windows.Forms.GroupBox dis_grp;
        private System.Windows.Forms.LinkLabel dis_linkHelp;
        private System.Windows.Forms.ComboBox rank_cmbOsMap;
        private System.Windows.Forms.Label rank_lblOsMap;
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
        private MCGalaxy.Gui.TimespanUpDown sec_numChatMute;
        private System.Windows.Forms.Label sec_lblChatOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numChatMsgs;
        private System.Windows.Forms.Label sec_lblChatOnMute;
        private MCGalaxy.Gui.TimespanUpDown sec_numChatSecs;
        
        private System.Windows.Forms.GroupBox sec_grpCmd;
        private System.Windows.Forms.CheckBox sec_cbCmdAuto;
        private MCGalaxy.Gui.TimespanUpDown sec_numCmdMute;
        private System.Windows.Forms.Label sec_lblCmdForMute;
        private MCGalaxy.Gui.TimespanUpDown sec_numCmdSecs;
        private System.Windows.Forms.Label sec_lblCmdOnMsgs;
        private System.Windows.Forms.NumericUpDown sec_numCmdMsgs;
        private System.Windows.Forms.Label sec_lblCmdOnMute;
        
        private System.Windows.Forms.GroupBox sec_grpIP;
        private System.Windows.Forms.CheckBox sec_cbIPAuto;
        private MCGalaxy.Gui.TimespanUpDown sec_numIPMute;
        private System.Windows.Forms.Label sec_lblIPForMute;
        private MCGalaxy.Gui.TimespanUpDown sec_numIPSecs;
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
        private MCGalaxy.Gui.TimespanUpDown sec_numBlocksSecs;
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
        private System.Windows.Forms.TextBox txtNormRp;
        private System.Windows.Forms.TextBox txtRP;
        private MCGalaxy.Gui.TimespanUpDown bak_numTime;
        private System.Windows.Forms.TextBox bak_txtLocation;
        private System.Windows.Forms.CheckBox hack_lbl;
        private System.Windows.Forms.CheckBox chat_chkFilter;
        private System.Windows.Forms.CheckBox chkRepeatMessages;
        private System.Windows.Forms.CheckBox chkDeath;
        private System.Windows.Forms.CheckBox chk17Dollar;
        private System.Windows.Forms.CheckBox chkSmile;
        private System.Windows.Forms.Label chkRpNorm;
        private System.Windows.Forms.Label chkRpLimit;
        private System.Windows.Forms.Label afk_lblTimer;
        private System.Windows.Forms.Label bak_lblTime;
        private System.Windows.Forms.Label bak_lblLocation;
        private System.Windows.Forms.TabPage pageRelay;
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
        private System.Windows.Forms.CheckBox adv_chkCPE;
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
        private System.Windows.Forms.TabPage tabZS_old;
        private System.Windows.Forms.TabPage tabLS;
        private System.Windows.Forms.TextBox irc_txtPass;
        private System.Windows.Forms.CheckBox irc_chkPass;
        private System.Windows.Forms.CheckBox irc_cbTitles;
        private System.Windows.Forms.Label irc_lblPort;
        private System.Windows.Forms.NumericUpDown irc_numPort;
        private System.Windows.Forms.CheckBox rank_cbEmpty;
        private System.Windows.Forms.GroupBox ls_grpMaps;
        private System.Windows.Forms.Label ls_lblNotUsed;
        private System.Windows.Forms.Label ls_lblUsed;
        private System.Windows.Forms.Button ls_btnAdd;
        private System.Windows.Forms.Button ls_btnRemove;
        private System.Windows.Forms.ListBox ls_lstNotUsed;
        private System.Windows.Forms.ListBox ls_lstUsed;
        private System.Windows.Forms.GroupBox ls_grpSettings;
        private System.Windows.Forms.GroupBox ls_grpMapSettings;
        private System.Windows.Forms.GroupBox ls_grpControls;
        private System.Windows.Forms.Button ls_btnEnd;
        private System.Windows.Forms.Button ls_btnStop;
        private System.Windows.Forms.Button ls_btnStart;
        private System.Windows.Forms.TextBox sql_txtPort;
        private System.Windows.Forms.Label sql_lblPort;
        private System.Windows.Forms.GroupBox srv_grpUpdate;
        private System.Windows.Forms.Button srv_btnForceUpdate;
        private System.Windows.Forms.CheckBox chkGuestLimitNotify;
        private System.Windows.Forms.Label misc_lblReview;
        private System.Windows.Forms.Label rank_lblMOTD;
        private System.Windows.Forms.TextBox rank_txtMOTD;
        private System.Windows.Forms.GroupBox tw_grpScores;
        private System.Windows.Forms.NumericUpDown tw_numScoreAssists;
        private System.Windows.Forms.Label tw_lblScorePerKill;
        private System.Windows.Forms.NumericUpDown tw_numScorePerKill;
        private System.Windows.Forms.Label tw_lblScoreLimit;
        private System.Windows.Forms.NumericUpDown tw_numScoreLimit;
        private System.Windows.Forms.NumericUpDown tw_numMultiKills;
        private System.Windows.Forms.GroupBox tw_grpGrace;
        private System.Windows.Forms.Label tw_lblGrace;
        private MCGalaxy.Gui.TimespanUpDown tw_numGrace;
        private System.Windows.Forms.GroupBox tw_grpTeams;
        private System.Windows.Forms.CheckBox tw_cbKills;
        private System.Windows.Forms.CheckBox tw_cbBalance;
        private System.Windows.Forms.CheckBox tw_cbGrace;
        private System.Windows.Forms.CheckBox tw_cbStreaks;
        private System.Windows.Forms.PropertyGrid propsZG;
    }
}