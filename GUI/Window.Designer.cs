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

namespace MCGalaxy.Gui
{
    public partial class Window
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

        protected override void WndProc(ref Message msg)
        {
            /*const int WM_SIZE = 0x0005;
            const int SIZE_MINIMIZED = 1;
            if ((msg.Msg == WM_SIZE) && ((int)msg.WParam == SIZE_MINIMIZED) && (Window.Minimize != null))
            {
                this.Window_Minimize(this, EventArgs.Empty);
            }*/

            base.WndProc(ref msg);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.mapsStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.physicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.physicsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.finiteModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomFlowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeWaterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.growingGrassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeGrowingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leafDecayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autpPhysicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadOngotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miscToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animalAIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.survivalDeathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killerBlocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instantBuildingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rPChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gunsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actiondToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playerStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.whoisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.voiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.promoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.demoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iconContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdownServer = new System.Windows.Forms.ToolStripMenuItem();
            this.restartServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProperties = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.Restart = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabsLogs = new System.Windows.Forms.TabControl();
            this.tabLog_Err = new System.Windows.Forms.TabPage();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.tabLog_Gen = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.LogsTxtBox = new System.Windows.Forms.RichTextBox();
            this.tabLog_Sys = new System.Windows.Forms.TabPage();
            this.txtSystem = new System.Windows.Forms.TextBox();
            this.tabLog_Chg = new System.Windows.Forms.TabPage();
            this.txtChangelog = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Unloadempty_button = new System.Windows.Forms.Button();
            this.killphysics_button = new System.Windows.Forms.Button();
            this.button_saveall = new System.Windows.Forms.Button();
            this.gBCommands = new System.Windows.Forms.GroupBox();
            this.txtCommandsUsed = new MCGalaxy.Gui.AutoScrollTextBox();
            this.dgvMaps = new System.Windows.Forms.DataGridView();
            this.gBChat = new System.Windows.Forms.GroupBox();
            this.txtLog = new MCGalaxy.Gui.Components.ColoredTextBox();
            this.txtLogMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nightModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateStampToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoScrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCommands = new System.Windows.Forms.TextBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.dgvPlayers = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TreeGrowChk = new System.Windows.Forms.CheckBox();
            this.label39 = new System.Windows.Forms.Label();
            this.leafDecayChk = new System.Windows.Forms.CheckBox();
            this.label38 = new System.Windows.Forms.Label();
            this.chkRndFlow = new System.Windows.Forms.CheckBox();
            this.label37 = new System.Windows.Forms.Label();
            this.UnloadChk = new System.Windows.Forms.CheckBox();
            this.label36 = new System.Windows.Forms.Label();
            this.LoadOnGotoChk = new System.Windows.Forms.CheckBox();
            this.label35 = new System.Windows.Forms.Label();
            this.AutoLoadChk = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.drownNumeric = new System.Windows.Forms.NumericUpDown();
            this.Fallnumeric = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.Gunschk = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Aicombo = new System.Windows.Forms.ComboBox();
            this.edgewaterchk = new System.Windows.Forms.CheckBox();
            this.grasschk = new System.Windows.Forms.CheckBox();
            this.finitechk = new System.Windows.Forms.CheckBox();
            this.Killerbloxchk = new System.Windows.Forms.CheckBox();
            this.SurvivalStyleDeathchk = new System.Windows.Forms.CheckBox();
            this.chatlvlchk = new System.Windows.Forms.CheckBox();
            this.physlvlnumeric = new System.Windows.Forms.NumericUpDown();
            this.MOTDtxt = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.SaveMap = new System.Windows.Forms.Button();
            this.dgvMapsTab = new System.Windows.Forms.DataGridView();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.PlayersTextBox = new MCGalaxy.Gui.AutoScrollTextBox();
            this.PlyersListBox = new System.Windows.Forms.ListBox();
            this.StatusTxt = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.LoggedinForTxt = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.Kickstxt = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.TimesLoggedInTxt = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.Blockstxt = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.DeathsTxt = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.IPtxt = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.SpawnBt = new System.Windows.Forms.Button();
            this.UndoTxt = new System.Windows.Forms.TextBox();
            this.UndoBt = new System.Windows.Forms.Button();
            this.SlapBt = new System.Windows.Forms.Button();
            this.SendRulesTxt = new System.Windows.Forms.Button();
            this.ImpersonateORSendCmdTxt = new System.Windows.Forms.TextBox();
            this.ImpersonateORSendCmdBt = new System.Windows.Forms.Button();
            this.KillBt = new System.Windows.Forms.Button();
            this.JailBt = new System.Windows.Forms.Button();
            this.DemoteBt = new System.Windows.Forms.Button();
            this.PromoteBt = new System.Windows.Forms.Button();
            this.LoginTxt = new System.Windows.Forms.TextBox();
            this.LogoutTxt = new System.Windows.Forms.TextBox();
            this.TitleTxt = new System.Windows.Forms.TextBox();
            this.ColorCombo = new System.Windows.Forms.ComboBox();
            this.ColorBt = new System.Windows.Forms.Button();
            this.TitleBt = new System.Windows.Forms.Button();
            this.LogoutBt = new System.Windows.Forms.Button();
            this.LoginBt = new System.Windows.Forms.Button();
            this.FreezeBt = new System.Windows.Forms.Button();
            this.VoiceBt = new System.Windows.Forms.Button();
            this.JokerBt = new System.Windows.Forms.Button();
            this.WarnBt = new System.Windows.Forms.Button();
            this.MessageBt = new System.Windows.Forms.Button();
            this.PLayersMessageTxt = new System.Windows.Forms.TextBox();
            this.HideBt = new System.Windows.Forms.Button();
            this.IPBanBt = new System.Windows.Forms.Button();
            this.BanBt = new System.Windows.Forms.Button();
            this.KickBt = new System.Windows.Forms.Button();
            this.MapCombo = new System.Windows.Forms.ComboBox();
            this.MapBt = new System.Windows.Forms.Button();
            this.MuteBt = new System.Windows.Forms.Button();
            this.RankTxt = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.MapTxt = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.NameTxtPlayersTab = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.Chat = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txtGlobalLog = new MCGalaxy.Gui.AutoScrollTextBox();
            this.txtGlobalInput = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txtAdminLog = new MCGalaxy.Gui.AutoScrollTextBox();
            this.txtAdminInput = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txtOpInput = new System.Windows.Forms.TextBox();
            this.txtOpLog = new MCGalaxy.Gui.AutoScrollTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gbMap_Props = new System.Windows.Forms.GroupBox();
            this.pgMaps = new System.Windows.Forms.PropertyGrid();
            this.gbMap_Lded = new System.Windows.Forms.GroupBox();
            this.lbMap_Lded = new System.Windows.Forms.ListBox();
            this.gbMap_Unld = new System.Windows.Forms.GroupBox();
            this.btnMap_Load = new System.Windows.Forms.Button();
            this.lbMap_Unld = new System.Windows.Forms.ListBox();
            this.gbMap_New = new System.Windows.Forms.GroupBox();
            this.btnMap_Gen = new System.Windows.Forms.Button();
            this.lblMap_Type = new System.Windows.Forms.Label();
            this.lblMap_Seed = new System.Windows.Forms.Label();
            this.lblMap_Z = new System.Windows.Forms.Label();
            this.lblMap_X = new System.Windows.Forms.Label();
            this.lblMap_Y = new System.Windows.Forms.Label();
            this.txtMap_Seed = new System.Windows.Forms.TextBox();
            this.cmbMap_Type = new System.Windows.Forms.ComboBox();
            this.cmbMap_Z = new System.Windows.Forms.ComboBox();
            this.cmbMap_Y = new System.Windows.Forms.ComboBox();
            this.cmbMap_X = new System.Windows.Forms.ComboBox();
            this.lblMap_Name = new System.Windows.Forms.Label();
            this.txtMap_Name = new System.Windows.Forms.TextBox();
            this.mapsStrip.SuspendLayout();
            this.playerStrip.SuspendLayout();
            this.iconContext.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabsLogs.SuspendLayout();
            this.tabLog_Err.SuspendLayout();
            this.tabLog_Gen.SuspendLayout();
            this.tabLog_Sys.SuspendLayout();
            this.tabLog_Chg.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gBCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaps)).BeginInit();
            this.gBChat.SuspendLayout();
            this.txtLogMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drownNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Fallnumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.physlvlnumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMapsTab)).BeginInit();
            this.tabPage7.SuspendLayout();
            this.panel4.SuspendLayout();
            this.Chat.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gbMap_Props.SuspendLayout();
            this.gbMap_Lded.SuspendLayout();
            this.gbMap_Unld.SuspendLayout();
            this.gbMap_New.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapsStrip
            // 
            this.mapsStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.physicsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.actiondToolStripMenuItem,
            this.toolStripSeparator1,
            this.infoToolStripMenuItem});
            this.mapsStrip.Name = "mapsStrip";
            this.mapsStrip.Size = new System.Drawing.Size(144, 98);
            // 
            // physicsToolStripMenuItem
            // 
            this.physicsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.physicsToolStripMenuItem.Name = "physicsToolStripMenuItem";
            this.physicsToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.physicsToolStripMenuItem.Text = "Physics Level";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem2.Text = "Off";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click_1);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem3.Text = "Normal";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click_1);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem4.Text = "Advanced";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click_1);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem5.Text = "Hardcore";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click_1);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem6.Text = "Instant";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click_1);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem7.Text = "Doors-Only";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click_1);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.physicsToolStripMenuItem1,
            this.loadingToolStripMenuItem,
            this.miscToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // physicsToolStripMenuItem1
            // 
            this.physicsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.finiteModeToolStripMenuItem,
            this.randomFlowToolStripMenuItem,
            this.edgeWaterToolStripMenuItem,
            this.growingGrassToolStripMenuItem,
            this.treeGrowingToolStripMenuItem,
            this.leafDecayToolStripMenuItem,
            this.autpPhysicsToolStripMenuItem});
            this.physicsToolStripMenuItem1.Name = "physicsToolStripMenuItem1";
            this.physicsToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.physicsToolStripMenuItem1.Text = "Physics";
            // 
            // finiteModeToolStripMenuItem
            // 
            this.finiteModeToolStripMenuItem.Name = "finiteModeToolStripMenuItem";
            this.finiteModeToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.finiteModeToolStripMenuItem.Text = "Finite Mode";
            this.finiteModeToolStripMenuItem.Click += new System.EventHandler(this.finiteModeToolStripMenuItem_Click);
            // 
            // randomFlowToolStripMenuItem
            // 
            this.randomFlowToolStripMenuItem.Name = "randomFlowToolStripMenuItem";
            this.randomFlowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.randomFlowToolStripMenuItem.Text = "Random Flow";
            this.randomFlowToolStripMenuItem.Click += new System.EventHandler(this.randomFlowToolStripMenuItem_Click);
            // 
            // edgeWaterToolStripMenuItem
            // 
            this.edgeWaterToolStripMenuItem.Name = "edgeWaterToolStripMenuItem";
            this.edgeWaterToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.edgeWaterToolStripMenuItem.Text = "Edge Water";
            this.edgeWaterToolStripMenuItem.Click += new System.EventHandler(this.edgeWaterToolStripMenuItem_Click);
            // 
            // growingGrassToolStripMenuItem
            // 
            this.growingGrassToolStripMenuItem.Name = "growingGrassToolStripMenuItem";
            this.growingGrassToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.growingGrassToolStripMenuItem.Text = "Grass Growing";
            this.growingGrassToolStripMenuItem.Click += new System.EventHandler(this.growingGrassToolStripMenuItem_Click);
            // 
            // treeGrowingToolStripMenuItem
            // 
            this.treeGrowingToolStripMenuItem.Name = "treeGrowingToolStripMenuItem";
            this.treeGrowingToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.treeGrowingToolStripMenuItem.Text = "Tree Growing";
            this.treeGrowingToolStripMenuItem.Click += new System.EventHandler(this.treeGrowingToolStripMenuItem_Click);
            // 
            // leafDecayToolStripMenuItem
            // 
            this.leafDecayToolStripMenuItem.Name = "leafDecayToolStripMenuItem";
            this.leafDecayToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.leafDecayToolStripMenuItem.Text = "Leaf Decay";
            this.leafDecayToolStripMenuItem.Click += new System.EventHandler(this.leafDecayToolStripMenuItem_Click);
            // 
            // autpPhysicsToolStripMenuItem
            // 
            this.autpPhysicsToolStripMenuItem.Name = "autpPhysicsToolStripMenuItem";
            this.autpPhysicsToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.autpPhysicsToolStripMenuItem.Text = "Auto Physics";
            this.autpPhysicsToolStripMenuItem.Click += new System.EventHandler(this.autpPhysicsToolStripMenuItem_Click);
            // 
            // loadingToolStripMenuItem
            // 
            this.loadingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unloadToolStripMenuItem1,
            this.loadOngotoToolStripMenuItem});
            this.loadingToolStripMenuItem.Name = "loadingToolStripMenuItem";
            this.loadingToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.loadingToolStripMenuItem.Text = "Loading";
            // 
            // unloadToolStripMenuItem1
            // 
            this.unloadToolStripMenuItem1.Name = "unloadToolStripMenuItem1";
            this.unloadToolStripMenuItem1.Size = new System.Drawing.Size(150, 22);
            this.unloadToolStripMenuItem1.Text = "Auto Unload";
            this.unloadToolStripMenuItem1.Click += new System.EventHandler(this.unloadToolStripMenuItem1_Click);
            // 
            // loadOngotoToolStripMenuItem
            // 
            this.loadOngotoToolStripMenuItem.Name = "loadOngotoToolStripMenuItem";
            this.loadOngotoToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.loadOngotoToolStripMenuItem.Text = "Load on /goto";
            this.loadOngotoToolStripMenuItem.Click += new System.EventHandler(this.loadOngotoToolStripMenuItem_Click);
            // 
            // miscToolStripMenuItem
            // 
            this.miscToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animalAIToolStripMenuItem,
            this.survivalDeathToolStripMenuItem,
            this.killerBlocksToolStripMenuItem,
            this.instantBuildingToolStripMenuItem,
            this.rPChatToolStripMenuItem,
            this.gunsToolStripMenuItem});
            this.miscToolStripMenuItem.Name = "miscToolStripMenuItem";
            this.miscToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.miscToolStripMenuItem.Text = "Misc";
            // 
            // animalAIToolStripMenuItem
            // 
            this.animalAIToolStripMenuItem.Name = "animalAIToolStripMenuItem";
            this.animalAIToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.animalAIToolStripMenuItem.Text = "Animal AI";
            this.animalAIToolStripMenuItem.Click += new System.EventHandler(this.animalAIToolStripMenuItem_Click);
            // 
            // survivalDeathToolStripMenuItem
            // 
            this.survivalDeathToolStripMenuItem.Name = "survivalDeathToolStripMenuItem";
            this.survivalDeathToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.survivalDeathToolStripMenuItem.Text = "Survival Death";
            this.survivalDeathToolStripMenuItem.Click += new System.EventHandler(this.survivalDeathToolStripMenuItem_Click);
            // 
            // killerBlocksToolStripMenuItem
            // 
            this.killerBlocksToolStripMenuItem.Name = "killerBlocksToolStripMenuItem";
            this.killerBlocksToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.killerBlocksToolStripMenuItem.Text = "Killer Blocks";
            this.killerBlocksToolStripMenuItem.Click += new System.EventHandler(this.killerBlocksToolStripMenuItem_Click);
            // 
            // instantBuildingToolStripMenuItem
            // 
            this.instantBuildingToolStripMenuItem.Name = "instantBuildingToolStripMenuItem";
            this.instantBuildingToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.instantBuildingToolStripMenuItem.Text = "Instant Building";
            this.instantBuildingToolStripMenuItem.Click += new System.EventHandler(this.instantBuildingToolStripMenuItem_Click);
            // 
            // rPChatToolStripMenuItem
            // 
            this.rPChatToolStripMenuItem.Name = "rPChatToolStripMenuItem";
            this.rPChatToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.rPChatToolStripMenuItem.Text = "RP Chat";
            this.rPChatToolStripMenuItem.Click += new System.EventHandler(this.rPChatToolStripMenuItem_Click);
            // 
            // gunsToolStripMenuItem
            // 
            this.gunsToolStripMenuItem.Name = "gunsToolStripMenuItem";
            this.gunsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.gunsToolStripMenuItem.Text = "Guns";
            this.gunsToolStripMenuItem.Click += new System.EventHandler(this.gunsToolStripMenuItem_Click);
            // 
            // actiondToolStripMenuItem
            // 
            this.actiondToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.unloadToolStripMenuItem,
            this.moveAllToolStripMenuItem});
            this.actiondToolStripMenuItem.Name = "actiondToolStripMenuItem";
            this.actiondToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.actiondToolStripMenuItem.Text = "Actions";
            this.actiondToolStripMenuItem.Click += new System.EventHandler(this.actiondToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click_1);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // unloadToolStripMenuItem
            // 
            this.unloadToolStripMenuItem.Name = "unloadToolStripMenuItem";
            this.unloadToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.unloadToolStripMenuItem.Text = "Unload";
            this.unloadToolStripMenuItem.Click += new System.EventHandler(this.unloadToolStripMenuItem_Click_1);
            // 
            // moveAllToolStripMenuItem
            // 
            this.moveAllToolStripMenuItem.Name = "moveAllToolStripMenuItem";
            this.moveAllToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.moveAllToolStripMenuItem.Text = "Move All";
            this.moveAllToolStripMenuItem.Click += new System.EventHandler(this.moveAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // playerStrip
            // 
            this.playerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whoisToolStripMenuItem,
            this.kickToolStripMenuItem,
            this.banToolStripMenuItem,
            this.voiceToolStripMenuItem,
            this.clonesToolStripMenuItem,
            this.promoteToolStripMenuItem,
            this.demoteToolStripMenuItem});
            this.playerStrip.Name = "playerStrip";
            this.playerStrip.Size = new System.Drawing.Size(121, 158);
            // 
            // whoisToolStripMenuItem
            // 
            this.whoisToolStripMenuItem.Name = "whoisToolStripMenuItem";
            this.whoisToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.whoisToolStripMenuItem.Text = "Whois";
            this.whoisToolStripMenuItem.Click += new System.EventHandler(this.whoisToolStripMenuItem_Click);
            // 
            // kickToolStripMenuItem
            // 
            this.kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            this.kickToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.kickToolStripMenuItem.Text = "Kick";
            this.kickToolStripMenuItem.Click += new System.EventHandler(this.kickToolStripMenuItem_Click);
            // 
            // banToolStripMenuItem
            // 
            this.banToolStripMenuItem.Name = "banToolStripMenuItem";
            this.banToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.banToolStripMenuItem.Text = "Ban";
            this.banToolStripMenuItem.Click += new System.EventHandler(this.banToolStripMenuItem_Click);
            // 
            // voiceToolStripMenuItem
            // 
            this.voiceToolStripMenuItem.Name = "voiceToolStripMenuItem";
            this.voiceToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.voiceToolStripMenuItem.Text = "Voice";
            this.voiceToolStripMenuItem.Click += new System.EventHandler(this.voiceToolStripMenuItem_Click);
            // 
            // clonesToolStripMenuItem
            // 
            this.clonesToolStripMenuItem.Name = "clonesToolStripMenuItem";
            this.clonesToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.clonesToolStripMenuItem.Text = "Clones";
            this.clonesToolStripMenuItem.Click += new System.EventHandler(this.clonesToolStripMenuItem_Click);
            // 
            // promoteToolStripMenuItem
            // 
            this.promoteToolStripMenuItem.Name = "promoteToolStripMenuItem";
            this.promoteToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.promoteToolStripMenuItem.Text = "Promote";
            this.promoteToolStripMenuItem.Click += new System.EventHandler(this.promoteToolStripMenuItem_Click);
            // 
            // demoteToolStripMenuItem
            // 
            this.demoteToolStripMenuItem.Name = "demoteToolStripMenuItem";
            this.demoteToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.demoteToolStripMenuItem.Text = "Demote";
            this.demoteToolStripMenuItem.Click += new System.EventHandler(this.demoteToolStripMenuItem_Click);
            // 
            // iconContext
            // 
            this.iconContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openConsole,
            this.shutdownServer,
            this.restartServerToolStripMenuItem});
            this.iconContext.Name = "iconContext";
            this.iconContext.Size = new System.Drawing.Size(164, 70);
            // 
            // openConsole
            // 
            this.openConsole.Name = "openConsole";
            this.openConsole.Size = new System.Drawing.Size(163, 22);
            this.openConsole.Text = "Open Console";
            this.openConsole.Click += new System.EventHandler(this.openConsole_Click);
            // 
            // shutdownServer
            // 
            this.shutdownServer.Name = "shutdownServer";
            this.shutdownServer.Size = new System.Drawing.Size(163, 22);
            this.shutdownServer.Text = "Shutdown Server";
            this.shutdownServer.Click += new System.EventHandler(this.shutdownServer_Click);
            // 
            // restartServerToolStripMenuItem
            // 
            this.restartServerToolStripMenuItem.Name = "restartServerToolStripMenuItem";
            this.restartServerToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.restartServerToolStripMenuItem.Text = "Restart Server";
            this.restartServerToolStripMenuItem.Click += new System.EventHandler(this.restartServerToolStripMenuItem_Click);
            // 
            // btnProperties
            // 
            this.btnProperties.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProperties.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProperties.Location = new System.Drawing.Point(501, 5);
            this.btnProperties.Name = "btnProperties";
            this.btnProperties.Size = new System.Drawing.Size(80, 23);
            this.btnProperties.TabIndex = 34;
            this.btnProperties.Text = "Properties";
            this.btnProperties.UseVisualStyleBackColor = true;
            this.btnProperties.Click += new System.EventHandler(this.btnProperties_Click_1);
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(675, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 23);
            this.btnClose.TabIndex = 35;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // Restart
            // 
            this.Restart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Restart.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Restart.Location = new System.Drawing.Point(584, 5);
            this.Restart.Name = "Restart";
            this.Restart.Size = new System.Drawing.Size(88, 23);
            this.Restart.TabIndex = 36;
            this.Restart.Text = "Restart";
            this.Restart.UseVisualStyleBackColor = true;
            this.Restart.Click += new System.EventHandler(this.Restart_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage5.Controls.Add(this.tabsLogs);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(767, 488);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Logs";
            // 
            // tabsLogs
            // 
            this.tabsLogs.Controls.Add(this.tabLog_Err);
            this.tabsLogs.Controls.Add(this.tabLog_Gen);
            this.tabsLogs.Controls.Add(this.tabLog_Sys);
            this.tabsLogs.Controls.Add(this.tabLog_Chg);
            this.tabsLogs.Location = new System.Drawing.Point(-1, 1);
            this.tabsLogs.Name = "tabsLogs";
            this.tabsLogs.SelectedIndex = 0;
            this.tabsLogs.Size = new System.Drawing.Size(775, 491);
            this.tabsLogs.TabIndex = 0;
            // 
            // tabLog_Err
            // 
            this.tabLog_Err.Controls.Add(this.txtErrors);
            this.tabLog_Err.Location = new System.Drawing.Point(4, 22);
            this.tabLog_Err.Name = "tabLog_Err";
            this.tabLog_Err.Size = new System.Drawing.Size(767, 465);
            this.tabLog_Err.TabIndex = 2;
            this.tabLog_Err.Text = "Errors";
            this.tabLog_Err.UseVisualStyleBackColor = true;
            // 
            // txtErrors
            // 
            this.txtErrors.BackColor = System.Drawing.Color.White;
            this.txtErrors.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtErrors.Location = new System.Drawing.Point(-2, 0);
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.ReadOnly = true;
            this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrors.Size = new System.Drawing.Size(765, 465);
            this.txtErrors.TabIndex = 2;
            // 
            // tabLog_Gen
            // 
            this.tabLog_Gen.Controls.Add(this.label3);
            this.tabLog_Gen.Controls.Add(this.dateTimePicker1);
            this.tabLog_Gen.Controls.Add(this.LogsTxtBox);
            this.tabLog_Gen.Location = new System.Drawing.Point(4, 22);
            this.tabLog_Gen.Name = "tabLog_Gen";
            this.tabLog_Gen.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog_Gen.Size = new System.Drawing.Size(767, 465);
            this.tabLog_Gen.TabIndex = 0;
            this.tabLog_Gen.Text = "General";
            this.tabLog_Gen.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "View logs from:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(87, 4);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker1.TabIndex = 5;
            this.dateTimePicker1.Value = new System.DateTime(2011, 7, 20, 18, 31, 50, 0);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.DatePicker1_ValueChanged);
            // 
            // LogsTxtBox
            // 
            this.LogsTxtBox.BackColor = System.Drawing.SystemColors.Window;
            this.LogsTxtBox.Location = new System.Drawing.Point(-2, 30);
            this.LogsTxtBox.Name = "LogsTxtBox";
            this.LogsTxtBox.ReadOnly = true;
            this.LogsTxtBox.Size = new System.Drawing.Size(765, 436);
            this.LogsTxtBox.TabIndex = 4;
            this.LogsTxtBox.Text = "";
            // 
            // tabLog_Sys
            // 
            this.tabLog_Sys.Controls.Add(this.txtSystem);
            this.tabLog_Sys.Location = new System.Drawing.Point(4, 22);
            this.tabLog_Sys.Name = "tabLog_Sys";
            this.tabLog_Sys.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog_Sys.Size = new System.Drawing.Size(767, 465);
            this.tabLog_Sys.TabIndex = 1;
            this.tabLog_Sys.Text = "System";
            this.tabLog_Sys.UseVisualStyleBackColor = true;
            // 
            // txtSystem
            // 
            this.txtSystem.BackColor = System.Drawing.Color.White;
            this.txtSystem.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtSystem.Location = new System.Drawing.Point(-2, 0);
            this.txtSystem.Multiline = true;
            this.txtSystem.Name = "txtSystem";
            this.txtSystem.ReadOnly = true;
            this.txtSystem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSystem.Size = new System.Drawing.Size(765, 465);
            this.txtSystem.TabIndex = 2;
            // 
            // tabLog_Chg
            // 
            this.tabLog_Chg.Controls.Add(this.txtChangelog);
            this.tabLog_Chg.Location = new System.Drawing.Point(4, 22);
            this.tabLog_Chg.Name = "tabLog_Chg";
            this.tabLog_Chg.Size = new System.Drawing.Size(767, 465);
            this.tabLog_Chg.TabIndex = 3;
            this.tabLog_Chg.Text = "Changelog";
            this.tabLog_Chg.UseVisualStyleBackColor = true;
            // 
            // txtChangelog
            // 
            this.txtChangelog.BackColor = System.Drawing.Color.White;
            this.txtChangelog.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtChangelog.Location = new System.Drawing.Point(-2, 0);
            this.txtChangelog.Multiline = true;
            this.txtChangelog.Name = "txtChangelog";
            this.txtChangelog.ReadOnly = true;
            this.txtChangelog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChangelog.Size = new System.Drawing.Size(765, 465);
            this.txtChangelog.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.Unloadempty_button);
            this.tabPage1.Controls.Add(this.killphysics_button);
            this.tabPage1.Controls.Add(this.button_saveall);
            this.tabPage1.Controls.Add(this.gBCommands);
            this.tabPage1.Controls.Add(this.dgvMaps);
            this.tabPage1.Controls.Add(this.gBChat);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtCommands);
            this.tabPage1.Controls.Add(this.txtInput);
            this.tabPage1.Controls.Add(this.txtUrl);
            this.tabPage1.Controls.Add(this.dgvPlayers);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(767, 488);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            // 
            // Unloadempty_button
            // 
            this.Unloadempty_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Unloadempty_button.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Unloadempty_button.Location = new System.Drawing.Point(676, 263);
            this.Unloadempty_button.Name = "Unloadempty_button";
            this.Unloadempty_button.Size = new System.Drawing.Size(81, 23);
            this.Unloadempty_button.TabIndex = 41;
            this.Unloadempty_button.Text = "Unload Empty";
            this.Unloadempty_button.UseVisualStyleBackColor = true;
            this.Unloadempty_button.Click += new System.EventHandler(this.Unloadempty_button_Click);
            // 
            // killphysics_button
            // 
            this.killphysics_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.killphysics_button.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.killphysics_button.Location = new System.Drawing.Point(582, 263);
            this.killphysics_button.Name = "killphysics_button";
            this.killphysics_button.Size = new System.Drawing.Size(88, 23);
            this.killphysics_button.TabIndex = 40;
            this.killphysics_button.Text = "Kill All Physics";
            this.killphysics_button.UseVisualStyleBackColor = true;
            this.killphysics_button.Click += new System.EventHandler(this.killphysics_button_Click);
            // 
            // button_saveall
            // 
            this.button_saveall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_saveall.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_saveall.Location = new System.Drawing.Point(513, 263);
            this.button_saveall.Name = "button_saveall";
            this.button_saveall.Size = new System.Drawing.Size(63, 23);
            this.button_saveall.TabIndex = 39;
            this.button_saveall.Text = "Save All";
            this.button_saveall.UseVisualStyleBackColor = true;
            this.button_saveall.Click += new System.EventHandler(this.button_saveall_Click);
            // 
            // gBCommands
            // 
            this.gBCommands.Controls.Add(this.txtCommandsUsed);
            this.gBCommands.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBCommands.Location = new System.Drawing.Point(13, 326);
            this.gBCommands.Name = "gBCommands";
            this.gBCommands.Size = new System.Drawing.Size(493, 123);
            this.gBCommands.TabIndex = 34;
            this.gBCommands.TabStop = false;
            this.gBCommands.Text = "Commands";
            // 
            // txtCommandsUsed
            // 
            this.txtCommandsUsed.BackColor = System.Drawing.Color.White;
            this.txtCommandsUsed.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtCommandsUsed.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommandsUsed.Location = new System.Drawing.Point(9, 16);
            this.txtCommandsUsed.Multiline = true;
            this.txtCommandsUsed.Name = "txtCommandsUsed";
            this.txtCommandsUsed.ReadOnly = true;
            this.txtCommandsUsed.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCommandsUsed.Size = new System.Drawing.Size(478, 100);
            this.txtCommandsUsed.TabIndex = 0;
            // 
            // dgvMaps
            // 
            this.dgvMaps.AllowUserToAddRows = false;
            this.dgvMaps.AllowUserToDeleteRows = false;
            this.dgvMaps.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvMaps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvMaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMaps.ContextMenuStrip = this.mapsStrip;
            this.dgvMaps.Location = new System.Drawing.Point(512, 292);
            this.dgvMaps.MultiSelect = false;
            this.dgvMaps.Name = "dgvMaps";
            this.dgvMaps.ReadOnly = true;
            this.dgvMaps.RowHeadersVisible = false;
            this.dgvMaps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaps.Size = new System.Drawing.Size(246, 150);
            this.dgvMaps.TabIndex = 38;
            this.dgvMaps.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMaps_CellContentClick);
            // 
            // gBChat
            // 
            this.gBChat.Controls.Add(this.txtLog);
            this.gBChat.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBChat.Location = new System.Drawing.Point(13, 33);
            this.gBChat.Name = "gBChat";
            this.gBChat.Size = new System.Drawing.Size(493, 287);
            this.gBChat.TabIndex = 32;
            this.gBChat.TabStop = false;
            this.gBChat.Text = "Chat";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.ContextMenuStrip = this.txtLogMenuStrip;
            this.txtLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(7, 20);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtLog.Size = new System.Drawing.Size(480, 261);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // txtLogMenuStrip
            // 
            this.txtLogMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nightModeToolStripMenuItem,
            this.colorsToolStripMenuItem,
            this.dateStampToolStripMenuItem,
            this.autoScrollToolStripMenuItem,
            this.toolStripSeparator2,
            this.copySelectedToolStripMenuItem,
            this.copyAllToolStripMenuItem,
            this.toolStripSeparator3,
            this.clearToolStripMenuItem});
            this.txtLogMenuStrip.Name = "txtLogMenuStrip";
            this.txtLogMenuStrip.Size = new System.Drawing.Size(150, 170);
            // 
            // nightModeToolStripMenuItem
            // 
            this.nightModeToolStripMenuItem.Name = "nightModeToolStripMenuItem";
            this.nightModeToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.nightModeToolStripMenuItem.Text = "Night Theme";
            this.nightModeToolStripMenuItem.Click += new System.EventHandler(this.nightModeToolStripMenuItem_Click_1);
            // 
            // colorsToolStripMenuItem
            // 
            this.colorsToolStripMenuItem.Checked = true;
            this.colorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.colorsToolStripMenuItem.Name = "colorsToolStripMenuItem";
            this.colorsToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.colorsToolStripMenuItem.Text = "Colors";
            this.colorsToolStripMenuItem.Click += new System.EventHandler(this.colorsToolStripMenuItem_Click_1);
            // 
            // dateStampToolStripMenuItem
            // 
            this.dateStampToolStripMenuItem.Checked = true;
            this.dateStampToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dateStampToolStripMenuItem.Name = "dateStampToolStripMenuItem";
            this.dateStampToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.dateStampToolStripMenuItem.Text = "Date Stamp";
            this.dateStampToolStripMenuItem.Click += new System.EventHandler(this.dateStampToolStripMenuItem_Click);
            // 
            // autoScrollToolStripMenuItem
            // 
            this.autoScrollToolStripMenuItem.Checked = true;
            this.autoScrollToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoScrollToolStripMenuItem.Name = "autoScrollToolStripMenuItem";
            this.autoScrollToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.autoScrollToolStripMenuItem.Text = "Auto Scroll";
            this.autoScrollToolStripMenuItem.Click += new System.EventHandler(this.autoScrollToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(146, 6);
            // 
            // copySelectedToolStripMenuItem
            // 
            this.copySelectedToolStripMenuItem.Name = "copySelectedToolStripMenuItem";
            this.copySelectedToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.copySelectedToolStripMenuItem.Text = "Copy Selected";
            this.copySelectedToolStripMenuItem.Click += new System.EventHandler(this.copySelectedToolStripMenuItem_Click);
            // 
            // copyAllToolStripMenuItem
            // 
            this.copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
            this.copyAllToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.copyAllToolStripMenuItem.Text = "Copy All";
            this.copyAllToolStripMenuItem.Click += new System.EventHandler(this.copyAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(146, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(512, 462);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Command:";
            // 
            // txtCommands
            // 
            this.txtCommands.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommands.Location = new System.Drawing.Point(575, 459);
            this.txtCommands.Name = "txtCommands";
            this.txtCommands.Size = new System.Drawing.Size(183, 21);
            this.txtCommands.TabIndex = 28;
            this.txtCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCommands_KeyDown);
            // 
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(57, 459);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(449, 21);
            this.txtInput.TabIndex = 27;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // txtUrl
            // 
            this.txtUrl.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtUrl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUrl.Location = new System.Drawing.Point(13, 7);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.ReadOnly = true;
            this.txtUrl.Size = new System.Drawing.Size(493, 21);
            this.txtUrl.TabIndex = 25;
            this.txtUrl.DoubleClick += new System.EventHandler(this.txtUrl_DoubleClick);
            // 
            // dgvPlayers
            // 
            this.dgvPlayers.AllowUserToAddRows = false;
            this.dgvPlayers.AllowUserToDeleteRows = false;
            this.dgvPlayers.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvPlayers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlayers.ContextMenuStrip = this.playerStrip;
            this.dgvPlayers.Location = new System.Drawing.Point(512, 7);
            this.dgvPlayers.MultiSelect = false;
            this.dgvPlayers.Name = "dgvPlayers";
            this.dgvPlayers.ReadOnly = true;
            this.dgvPlayers.RowHeadersVisible = false;
            this.dgvPlayers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlayers.Size = new System.Drawing.Size(246, 250);
            this.dgvPlayers.TabIndex = 37;
            this.dgvPlayers.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvPlayers_RowPrePaint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 462);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Chat:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.Chat);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabControl1.Font = new System.Drawing.Font("Calibri", 8.25F);
            this.tabControl1.Location = new System.Drawing.Point(1, 11);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(775, 514);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.Click += new System.EventHandler(this.tabControl1_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.panel2);
            this.tabPage6.Controls.Add(this.dgvMapsTab);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(767, 488);
            this.tabPage6.TabIndex = 6;
            this.tabPage6.Text = "Maps";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.TreeGrowChk);
            this.panel2.Controls.Add(this.label39);
            this.panel2.Controls.Add(this.leafDecayChk);
            this.panel2.Controls.Add(this.label38);
            this.panel2.Controls.Add(this.chkRndFlow);
            this.panel2.Controls.Add(this.label37);
            this.panel2.Controls.Add(this.UnloadChk);
            this.panel2.Controls.Add(this.label36);
            this.panel2.Controls.Add(this.LoadOnGotoChk);
            this.panel2.Controls.Add(this.label35);
            this.panel2.Controls.Add(this.AutoLoadChk);
            this.panel2.Controls.Add(this.label23);
            this.panel2.Controls.Add(this.drownNumeric);
            this.panel2.Controls.Add(this.Fallnumeric);
            this.panel2.Controls.Add(this.label22);
            this.panel2.Controls.Add(this.Gunschk);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.Aicombo);
            this.panel2.Controls.Add(this.edgewaterchk);
            this.panel2.Controls.Add(this.grasschk);
            this.panel2.Controls.Add(this.finitechk);
            this.panel2.Controls.Add(this.Killerbloxchk);
            this.panel2.Controls.Add(this.SurvivalStyleDeathchk);
            this.panel2.Controls.Add(this.chatlvlchk);
            this.panel2.Controls.Add(this.physlvlnumeric);
            this.panel2.Controls.Add(this.MOTDtxt);
            this.panel2.Controls.Add(this.label21);
            this.panel2.Controls.Add(this.label20);
            this.panel2.Controls.Add(this.label19);
            this.panel2.Controls.Add(this.label18);
            this.panel2.Controls.Add(this.label17);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.SaveMap);
            this.panel2.Location = new System.Drawing.Point(390, 7);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(372, 207);
            this.panel2.TabIndex = 48;
            // 
            // TreeGrowChk
            // 
            this.TreeGrowChk.AutoSize = true;
            this.TreeGrowChk.Location = new System.Drawing.Point(168, 61);
            this.TreeGrowChk.Name = "TreeGrowChk";
            this.TreeGrowChk.Size = new System.Drawing.Size(15, 14);
            this.TreeGrowChk.TabIndex = 48;
            this.TreeGrowChk.UseVisualStyleBackColor = true;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(97, 62);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(69, 13);
            this.label39.TabIndex = 47;
            this.label39.Text = "Tree growing:";
            // 
            // leafDecayChk
            // 
            this.leafDecayChk.AutoSize = true;
            this.leafDecayChk.Location = new System.Drawing.Point(76, 125);
            this.leafDecayChk.Name = "leafDecayChk";
            this.leafDecayChk.Size = new System.Drawing.Size(15, 14);
            this.leafDecayChk.TabIndex = 46;
            this.leafDecayChk.UseVisualStyleBackColor = true;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(4, 125);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(60, 13);
            this.label38.TabIndex = 45;
            this.label38.Text = "Leaf decay:";
            // 
            // chkRndFlow
            // 
            this.chkRndFlow.AutoSize = true;
            this.chkRndFlow.Location = new System.Drawing.Point(343, 98);
            this.chkRndFlow.Name = "chkRndFlow";
            this.chkRndFlow.Size = new System.Drawing.Size(15, 14);
            this.chkRndFlow.TabIndex = 44;
            this.chkRndFlow.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(234, 97);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(71, 13);
            this.label37.TabIndex = 43;
            this.label37.Text = "Random flow:";
            // 
            // UnloadChk
            // 
            this.UnloadChk.AutoSize = true;
            this.UnloadChk.Location = new System.Drawing.Point(343, 82);
            this.UnloadChk.Name = "UnloadChk";
            this.UnloadChk.Size = new System.Drawing.Size(15, 14);
            this.UnloadChk.TabIndex = 42;
            this.UnloadChk.UseVisualStyleBackColor = true;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(234, 82);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(104, 13);
            this.label36.TabIndex = 41;
            this.label36.Text = "Unload when empty:";
            // 
            // LoadOnGotoChk
            // 
            this.LoadOnGotoChk.AutoSize = true;
            this.LoadOnGotoChk.Location = new System.Drawing.Point(343, 66);
            this.LoadOnGotoChk.Name = "LoadOnGotoChk";
            this.LoadOnGotoChk.Size = new System.Drawing.Size(15, 14);
            this.LoadOnGotoChk.TabIndex = 40;
            this.LoadOnGotoChk.UseVisualStyleBackColor = true;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(234, 67);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(74, 13);
            this.label35.TabIndex = 39;
            this.label35.Text = "Load on /goto:";
            // 
            // AutoLoadChk
            // 
            this.AutoLoadChk.AutoSize = true;
            this.AutoLoadChk.Location = new System.Drawing.Point(76, 109);
            this.AutoLoadChk.Name = "AutoLoadChk";
            this.AutoLoadChk.Size = new System.Drawing.Size(15, 14);
            this.AutoLoadChk.TabIndex = 38;
            this.AutoLoadChk.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(4, 109);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(58, 13);
            this.label23.TabIndex = 37;
            this.label23.Text = "Auto-Load:";
            // 
            // drownNumeric
            // 
            this.drownNumeric.Location = new System.Drawing.Point(281, 144);
            this.drownNumeric.Name = "drownNumeric";
            this.drownNumeric.Size = new System.Drawing.Size(77, 21);
            this.drownNumeric.TabIndex = 36;
            // 
            // Fallnumeric
            // 
            this.Fallnumeric.Location = new System.Drawing.Point(281, 116);
            this.Fallnumeric.Name = "Fallnumeric";
            this.Fallnumeric.Size = new System.Drawing.Size(77, 21);
            this.Fallnumeric.TabIndex = 35;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(234, 142);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(41, 13);
            this.label22.TabIndex = 34;
            this.label22.Text = "Drown:";
            // 
            // Gunschk
            // 
            this.Gunschk.AutoSize = true;
            this.Gunschk.Location = new System.Drawing.Point(76, 93);
            this.Gunschk.Name = "Gunschk";
            this.Gunschk.Size = new System.Drawing.Size(15, 14);
            this.Gunschk.TabIndex = 33;
            this.Gunschk.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(234, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "Fall:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Guns:";
            // 
            // Aicombo
            // 
            this.Aicombo.FormattingEnabled = true;
            this.Aicombo.Items.AddRange(new object[] {
            "Hunt",
            "Flee"});
            this.Aicombo.Location = new System.Drawing.Point(29, 142);
            this.Aicombo.Name = "Aicombo";
            this.Aicombo.Size = new System.Drawing.Size(62, 21);
            this.Aicombo.TabIndex = 30;
            // 
            // edgewaterchk
            // 
            this.edgewaterchk.AutoSize = true;
            this.edgewaterchk.Location = new System.Drawing.Point(343, 50);
            this.edgewaterchk.Name = "edgewaterchk";
            this.edgewaterchk.Size = new System.Drawing.Size(15, 14);
            this.edgewaterchk.TabIndex = 29;
            this.edgewaterchk.UseVisualStyleBackColor = true;
            // 
            // grasschk
            // 
            this.grasschk.AutoSize = true;
            this.grasschk.Location = new System.Drawing.Point(76, 61);
            this.grasschk.Name = "grasschk";
            this.grasschk.Size = new System.Drawing.Size(15, 14);
            this.grasschk.TabIndex = 28;
            this.grasschk.UseVisualStyleBackColor = true;
            // 
            // finitechk
            // 
            this.finitechk.AutoSize = true;
            this.finitechk.Location = new System.Drawing.Point(343, 34);
            this.finitechk.Name = "finitechk";
            this.finitechk.Size = new System.Drawing.Size(15, 14);
            this.finitechk.TabIndex = 27;
            this.finitechk.UseVisualStyleBackColor = true;
            // 
            // Killerbloxchk
            // 
            this.Killerbloxchk.AutoSize = true;
            this.Killerbloxchk.Location = new System.Drawing.Point(343, 2);
            this.Killerbloxchk.Name = "Killerbloxchk";
            this.Killerbloxchk.Size = new System.Drawing.Size(15, 14);
            this.Killerbloxchk.TabIndex = 26;
            this.Killerbloxchk.UseVisualStyleBackColor = true;
            // 
            // SurvivalStyleDeathchk
            // 
            this.SurvivalStyleDeathchk.AutoSize = true;
            this.SurvivalStyleDeathchk.Location = new System.Drawing.Point(343, 18);
            this.SurvivalStyleDeathchk.Name = "SurvivalStyleDeathchk";
            this.SurvivalStyleDeathchk.Size = new System.Drawing.Size(15, 14);
            this.SurvivalStyleDeathchk.TabIndex = 25;
            this.SurvivalStyleDeathchk.UseVisualStyleBackColor = true;
            // 
            // chatlvlchk
            // 
            this.chatlvlchk.AutoSize = true;
            this.chatlvlchk.Location = new System.Drawing.Point(76, 77);
            this.chatlvlchk.Name = "chatlvlchk";
            this.chatlvlchk.Size = new System.Drawing.Size(15, 14);
            this.chatlvlchk.TabIndex = 24;
            this.chatlvlchk.UseVisualStyleBackColor = true;
            // 
            // physlvlnumeric
            // 
            this.physlvlnumeric.Location = new System.Drawing.Point(76, 36);
            this.physlvlnumeric.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.physlvlnumeric.Name = "physlvlnumeric";
            this.physlvlnumeric.Size = new System.Drawing.Size(106, 21);
            this.physlvlnumeric.TabIndex = 22;
            // 
            // MOTDtxt
            // 
            this.MOTDtxt.Location = new System.Drawing.Point(76, 8);
            this.MOTDtxt.Name = "MOTDtxt";
            this.MOTDtxt.Size = new System.Drawing.Size(152, 21);
            this.MOTDtxt.TabIndex = 21;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(4, 146);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(19, 13);
            this.label21.TabIndex = 20;
            this.label21.Text = "AI:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(234, 51);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(89, 13);
            this.label20.TabIndex = 19;
            this.label20.Text = "Edge water flows:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(234, 35);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(68, 13);
            this.label19.TabIndex = 18;
            this.label19.Text = "Finite Liquid:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(234, 18);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(103, 13);
            this.label18.TabIndex = 17;
            this.label18.Text = "Survival-style death:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(234, 2);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(67, 13);
            this.label17.TabIndex = 16;
            this.label17.Text = "Killer blocks:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 62);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(37, 13);
            this.label16.TabIndex = 15;
            this.label16.Text = "Grass:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 11);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(38, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "MOTD:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 78);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 13);
            this.label13.TabIndex = 12;
            this.label13.Text = "World-Chat:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 41);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Physics Level:";
            // 
            // SaveMap
            // 
            this.SaveMap.Location = new System.Drawing.Point(3, 168);
            this.SaveMap.Name = "SaveMap";
            this.SaveMap.Size = new System.Drawing.Size(364, 35);
            this.SaveMap.TabIndex = 9;
            this.SaveMap.Text = "Save Map Properties";
            this.SaveMap.UseVisualStyleBackColor = true;
            this.SaveMap.Click += new System.EventHandler(this.SaveMap_Click);        
            // 
            // dgvMapsTab
            // 
            this.dgvMapsTab.AllowUserToAddRows = false;
            this.dgvMapsTab.AllowUserToDeleteRows = false;
            this.dgvMapsTab.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvMapsTab.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvMapsTab.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMapsTab.Location = new System.Drawing.Point(7, 220);
            this.dgvMapsTab.MultiSelect = false;
            this.dgvMapsTab.Name = "dgvMapsTab";
            this.dgvMapsTab.ReadOnly = true;
            this.dgvMapsTab.RowHeadersVisible = false;
            this.dgvMapsTab.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMapsTab.Size = new System.Drawing.Size(754, 262);
            this.dgvMapsTab.TabIndex = 39;
            this.dgvMapsTab.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMapsTab_CellClick);
            this.dgvMapsTab.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMapsTab_CellClick);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.PlayersTextBox);
            this.tabPage7.Controls.Add(this.PlyersListBox);
            this.tabPage7.Controls.Add(this.StatusTxt);
            this.tabPage7.Controls.Add(this.label25);
            this.tabPage7.Controls.Add(this.LoggedinForTxt);
            this.tabPage7.Controls.Add(this.label31);
            this.tabPage7.Controls.Add(this.Kickstxt);
            this.tabPage7.Controls.Add(this.label30);
            this.tabPage7.Controls.Add(this.TimesLoggedInTxt);
            this.tabPage7.Controls.Add(this.label29);
            this.tabPage7.Controls.Add(this.Blockstxt);
            this.tabPage7.Controls.Add(this.label28);
            this.tabPage7.Controls.Add(this.DeathsTxt);
            this.tabPage7.Controls.Add(this.label27);
            this.tabPage7.Controls.Add(this.IPtxt);
            this.tabPage7.Controls.Add(this.label26);
            this.tabPage7.Controls.Add(this.panel4);
            this.tabPage7.Controls.Add(this.RankTxt);
            this.tabPage7.Controls.Add(this.label24);
            this.tabPage7.Controls.Add(this.MapTxt);
            this.tabPage7.Controls.Add(this.label14);
            this.tabPage7.Controls.Add(this.NameTxtPlayersTab);
            this.tabPage7.Controls.Add(this.label12);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(767, 488);
            this.tabPage7.TabIndex = 7;
            this.tabPage7.Text = "Players";
            // 
            // PlayersTextBox
            // 
            this.PlayersTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.PlayersTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.PlayersTextBox.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayersTextBox.Location = new System.Drawing.Point(306, 304);
            this.PlayersTextBox.Multiline = true;
            this.PlayersTextBox.Name = "PlayersTextBox";
            this.PlayersTextBox.ReadOnly = true;
            this.PlayersTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.PlayersTextBox.Size = new System.Drawing.Size(452, 173);
            this.PlayersTextBox.TabIndex = 63;
            // 
            // PlyersListBox
            // 
            this.PlyersListBox.FormattingEnabled = true;
            this.PlyersListBox.Location = new System.Drawing.Point(8, 304);
            this.PlyersListBox.Name = "PlyersListBox";
            this.PlyersListBox.Size = new System.Drawing.Size(291, 173);
            this.PlyersListBox.TabIndex = 62;
            this.PlyersListBox.Click += new System.EventHandler(this.PlyersListBox_Click);
            // 
            // StatusTxt
            // 
            this.StatusTxt.Location = new System.Drawing.Point(612, 4);
            this.StatusTxt.Name = "StatusTxt";
            this.StatusTxt.ReadOnly = true;
            this.StatusTxt.Size = new System.Drawing.Size(145, 21);
            this.StatusTxt.TabIndex = 61;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(566, 7);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(40, 13);
            this.label25.TabIndex = 60;
            this.label25.Text = "Status:";
            // 
            // LoggedinForTxt
            // 
            this.LoggedinForTxt.Location = new System.Drawing.Point(537, 31);
            this.LoggedinForTxt.Name = "LoggedinForTxt";
            this.LoggedinForTxt.ReadOnly = true;
            this.LoggedinForTxt.Size = new System.Drawing.Size(76, 21);
            this.LoggedinForTxt.TabIndex = 59;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(505, 34);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(25, 13);
            this.label31.TabIndex = 58;
            this.label31.Text = "For:";
            // 
            // Kickstxt
            // 
            this.Kickstxt.Location = new System.Drawing.Point(658, 31);
            this.Kickstxt.Name = "Kickstxt";
            this.Kickstxt.ReadOnly = true;
            this.Kickstxt.Size = new System.Drawing.Size(99, 21);
            this.Kickstxt.TabIndex = 57;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(619, 34);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(34, 13);
            this.label30.TabIndex = 56;
            this.label30.Text = "Kicks:";
            // 
            // TimesLoggedInTxt
            // 
            this.TimesLoggedInTxt.Location = new System.Drawing.Point(412, 31);
            this.TimesLoggedInTxt.Name = "TimesLoggedInTxt";
            this.TimesLoggedInTxt.ReadOnly = true;
            this.TimesLoggedInTxt.Size = new System.Drawing.Size(92, 21);
            this.TimesLoggedInTxt.TabIndex = 55;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(352, 34);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(54, 13);
            this.label29.TabIndex = 54;
            this.label29.Text = "Logged in:";
            // 
            // Blockstxt
            // 
            this.Blockstxt.Location = new System.Drawing.Point(281, 31);
            this.Blockstxt.Name = "Blockstxt";
            this.Blockstxt.ReadOnly = true;
            this.Blockstxt.Size = new System.Drawing.Size(65, 21);
            this.Blockstxt.TabIndex = 53;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(228, 34);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(52, 13);
            this.label28.TabIndex = 52;
            this.label28.Text = "Modified:";
            // 
            // DeathsTxt
            // 
            this.DeathsTxt.Location = new System.Drawing.Point(188, 31);
            this.DeathsTxt.Name = "DeathsTxt";
            this.DeathsTxt.ReadOnly = true;
            this.DeathsTxt.Size = new System.Drawing.Size(34, 21);
            this.DeathsTxt.TabIndex = 51;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(137, 34);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(44, 13);
            this.label27.TabIndex = 50;
            this.label27.Text = "Deaths:";
            // 
            // IPtxt
            // 
            this.IPtxt.Location = new System.Drawing.Point(42, 31);
            this.IPtxt.Name = "IPtxt";
            this.IPtxt.ReadOnly = true;
            this.IPtxt.Size = new System.Drawing.Size(89, 21);
            this.IPtxt.TabIndex = 49;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(5, 34);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(19, 13);
            this.label26.TabIndex = 48;
            this.label26.Text = "IP:";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.SpawnBt);
            this.panel4.Controls.Add(this.UndoTxt);
            this.panel4.Controls.Add(this.UndoBt);
            this.panel4.Controls.Add(this.SlapBt);
            this.panel4.Controls.Add(this.SendRulesTxt);
            this.panel4.Controls.Add(this.ImpersonateORSendCmdTxt);
            this.panel4.Controls.Add(this.ImpersonateORSendCmdBt);
            this.panel4.Controls.Add(this.KillBt);
            this.panel4.Controls.Add(this.JailBt);
            this.panel4.Controls.Add(this.DemoteBt);
            this.panel4.Controls.Add(this.PromoteBt);
            this.panel4.Controls.Add(this.LoginTxt);
            this.panel4.Controls.Add(this.LogoutTxt);
            this.panel4.Controls.Add(this.TitleTxt);
            this.panel4.Controls.Add(this.ColorCombo);
            this.panel4.Controls.Add(this.ColorBt);
            this.panel4.Controls.Add(this.TitleBt);
            this.panel4.Controls.Add(this.LogoutBt);
            this.panel4.Controls.Add(this.LoginBt);
            this.panel4.Controls.Add(this.FreezeBt);
            this.panel4.Controls.Add(this.VoiceBt);
            this.panel4.Controls.Add(this.JokerBt);
            this.panel4.Controls.Add(this.WarnBt);
            this.panel4.Controls.Add(this.MessageBt);
            this.panel4.Controls.Add(this.PLayersMessageTxt);
            this.panel4.Controls.Add(this.HideBt);
            this.panel4.Controls.Add(this.IPBanBt);
            this.panel4.Controls.Add(this.BanBt);
            this.panel4.Controls.Add(this.KickBt);
            this.panel4.Controls.Add(this.MapCombo);
            this.panel4.Controls.Add(this.MapBt);
            this.panel4.Controls.Add(this.MuteBt);
            this.panel4.Location = new System.Drawing.Point(8, 59);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(753, 235);
            this.panel4.TabIndex = 47;
            // 
            // SpawnBt
            // 
            this.SpawnBt.Location = new System.Drawing.Point(627, 149);
            this.SpawnBt.Name = "SpawnBt";
            this.SpawnBt.Size = new System.Drawing.Size(122, 23);
            this.SpawnBt.TabIndex = 43;
            this.SpawnBt.Text = "Spawn";
            this.SpawnBt.UseVisualStyleBackColor = true;
            this.SpawnBt.Click += new System.EventHandler(this.SpawnBt_Click);
            // 
            // UndoTxt
            // 
            this.UndoTxt.Location = new System.Drawing.Point(131, 148);
            this.UndoTxt.Name = "UndoTxt";
            this.UndoTxt.Size = new System.Drawing.Size(234, 21);
            this.UndoTxt.TabIndex = 42;
            this.UndoTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UndoTxt_KeyDown);
            // 
            // UndoBt
            // 
            this.UndoBt.Location = new System.Drawing.Point(4, 148);
            this.UndoBt.Name = "UndoBt";
            this.UndoBt.Size = new System.Drawing.Size(121, 23);
            this.UndoBt.TabIndex = 41;
            this.UndoBt.Text = "Undo:";
            this.UndoBt.UseVisualStyleBackColor = true;
            this.UndoBt.Click += new System.EventHandler(this.UndoBt_Click);
            // 
            // SlapBt
            // 
            this.SlapBt.Location = new System.Drawing.Point(371, 146);
            this.SlapBt.Name = "SlapBt";
            this.SlapBt.Size = new System.Drawing.Size(122, 23);
            this.SlapBt.TabIndex = 40;
            this.SlapBt.Text = "Slap";
            this.SlapBt.UseVisualStyleBackColor = true;
            this.SlapBt.Click += new System.EventHandler(this.SlapBt_Click);
            // 
            // SendRulesTxt
            // 
            this.SendRulesTxt.Location = new System.Drawing.Point(627, 120);
            this.SendRulesTxt.Name = "SendRulesTxt";
            this.SendRulesTxt.Size = new System.Drawing.Size(122, 23);
            this.SendRulesTxt.TabIndex = 39;
            this.SendRulesTxt.Text = "Send Rules";
            this.SendRulesTxt.UseVisualStyleBackColor = true;
            this.SendRulesTxt.Click += new System.EventHandler(this.SendRulesTxt_Click);
            // 
            // ImpersonateORSendCmdTxt
            // 
            this.ImpersonateORSendCmdTxt.Location = new System.Drawing.Point(132, 207);
            this.ImpersonateORSendCmdTxt.Name = "ImpersonateORSendCmdTxt";
            this.ImpersonateORSendCmdTxt.Size = new System.Drawing.Size(616, 21);
            this.ImpersonateORSendCmdTxt.TabIndex = 38;
            this.ImpersonateORSendCmdTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImpersonateORSendCmdTxt_KeyDown);
            // 
            // ImpersonateORSendCmdBt
            // 
            this.ImpersonateORSendCmdBt.Location = new System.Drawing.Point(3, 206);
            this.ImpersonateORSendCmdBt.Name = "ImpersonateORSendCmdBt";
            this.ImpersonateORSendCmdBt.Size = new System.Drawing.Size(122, 23);
            this.ImpersonateORSendCmdBt.TabIndex = 37;
            this.ImpersonateORSendCmdBt.Text = "Impersonate/Cmd:";
            this.ImpersonateORSendCmdBt.UseVisualStyleBackColor = true;
            this.ImpersonateORSendCmdBt.Click += new System.EventHandler(this.ImpersonateORSendCmdBt_Click);
            // 
            // KillBt
            // 
            this.KillBt.Location = new System.Drawing.Point(499, 120);
            this.KillBt.Name = "KillBt";
            this.KillBt.Size = new System.Drawing.Size(122, 23);
            this.KillBt.TabIndex = 36;
            this.KillBt.Text = "Kill";
            this.KillBt.UseVisualStyleBackColor = true;
            this.KillBt.Click += new System.EventHandler(this.KillBt_Click);
            // 
            // JailBt
            // 
            this.JailBt.Location = new System.Drawing.Point(499, 149);
            this.JailBt.Name = "JailBt";
            this.JailBt.Size = new System.Drawing.Size(122, 23);
            this.JailBt.TabIndex = 34;
            this.JailBt.Text = "Jail";
            this.JailBt.UseVisualStyleBackColor = true;
            this.JailBt.Click += new System.EventHandler(this.JailBt_Click);
            // 
            // DemoteBt
            // 
            this.DemoteBt.Location = new System.Drawing.Point(371, 87);
            this.DemoteBt.Name = "DemoteBt";
            this.DemoteBt.Size = new System.Drawing.Size(122, 23);
            this.DemoteBt.TabIndex = 33;
            this.DemoteBt.Text = "Demote";
            this.DemoteBt.UseVisualStyleBackColor = true;
            this.DemoteBt.Click += new System.EventHandler(this.DemoteBt_Click);
            // 
            // PromoteBt
            // 
            this.PromoteBt.Location = new System.Drawing.Point(371, 58);
            this.PromoteBt.Name = "PromoteBt";
            this.PromoteBt.Size = new System.Drawing.Size(122, 23);
            this.PromoteBt.TabIndex = 32;
            this.PromoteBt.Text = "Promote";
            this.PromoteBt.UseVisualStyleBackColor = true;
            this.PromoteBt.Click += new System.EventHandler(this.PromoteBt_Click);
            // 
            // LoginTxt
            // 
            this.LoginTxt.Location = new System.Drawing.Point(131, 3);
            this.LoginTxt.Name = "LoginTxt";
            this.LoginTxt.Size = new System.Drawing.Size(362, 21);
            this.LoginTxt.TabIndex = 31;
            this.LoginTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LoginTxt_KeyDown);
            // 
            // LogoutTxt
            // 
            this.LogoutTxt.Location = new System.Drawing.Point(131, 31);
            this.LogoutTxt.Name = "LogoutTxt";
            this.LogoutTxt.Size = new System.Drawing.Size(362, 21);
            this.LogoutTxt.TabIndex = 30;
            this.LogoutTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LogoutTxt_KeyDown);
            // 
            // TitleTxt
            // 
            this.TitleTxt.Location = new System.Drawing.Point(131, 60);
            this.TitleTxt.Name = "TitleTxt";
            this.TitleTxt.Size = new System.Drawing.Size(234, 21);
            this.TitleTxt.TabIndex = 29;
            this.TitleTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TitleTxt_KeyDown);
            // 
            // ColorCombo
            // 
            this.ColorCombo.FormattingEnabled = true;
            this.ColorCombo.Items.AddRange(new object[] {
            "",
            "Black",
            "Navy",
            "Green",
            "Teal",
            "Maroon",
            "Purple",
            "Gold",
            "Silver",
            "Gray",
            "Blue",
            "Lime",
            "Aqua",
            "Red",
            "Pink",
            "Yellow",
            "White"});
            this.ColorCombo.Location = new System.Drawing.Point(131, 89);
            this.ColorCombo.Name = "ColorCombo";
            this.ColorCombo.Size = new System.Drawing.Size(234, 21);
            this.ColorCombo.TabIndex = 28;
            this.ColorCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ColorCombo_KeyDown);
            // 
            // ColorBt
            // 
            this.ColorBt.Location = new System.Drawing.Point(3, 89);
            this.ColorBt.Name = "ColorBt";
            this.ColorBt.Size = new System.Drawing.Size(122, 23);
            this.ColorBt.TabIndex = 27;
            this.ColorBt.Text = "Color:";
            this.ColorBt.UseVisualStyleBackColor = true;
            this.ColorBt.Click += new System.EventHandler(this.ColorBt_Click);
            // 
            // TitleBt
            // 
            this.TitleBt.Location = new System.Drawing.Point(3, 60);
            this.TitleBt.Name = "TitleBt";
            this.TitleBt.Size = new System.Drawing.Size(122, 23);
            this.TitleBt.TabIndex = 26;
            this.TitleBt.Text = "Title:";
            this.TitleBt.UseVisualStyleBackColor = true;
            this.TitleBt.Click += new System.EventHandler(this.TitleBt_Click);
            // 
            // LogoutBt
            // 
            this.LogoutBt.Location = new System.Drawing.Point(3, 31);
            this.LogoutBt.Name = "LogoutBt";
            this.LogoutBt.Size = new System.Drawing.Size(122, 23);
            this.LogoutBt.TabIndex = 25;
            this.LogoutBt.Text = "Logout:";
            this.LogoutBt.UseVisualStyleBackColor = true;
            this.LogoutBt.Click += new System.EventHandler(this.LogoutBt_Click);
            // 
            // LoginBt
            // 
            this.LoginBt.Location = new System.Drawing.Point(3, 3);
            this.LoginBt.Name = "LoginBt";
            this.LoginBt.Size = new System.Drawing.Size(122, 23);
            this.LoginBt.TabIndex = 24;
            this.LoginBt.Text = "Login:";
            this.LoginBt.UseVisualStyleBackColor = true;
            this.LoginBt.Click += new System.EventHandler(this.LoginBt_Click);
            // 
            // FreezeBt
            // 
            this.FreezeBt.Location = new System.Drawing.Point(499, 32);
            this.FreezeBt.Name = "FreezeBt";
            this.FreezeBt.Size = new System.Drawing.Size(122, 23);
            this.FreezeBt.TabIndex = 14;
            this.FreezeBt.Text = "Freeze";
            this.FreezeBt.UseVisualStyleBackColor = true;
            this.FreezeBt.Click += new System.EventHandler(this.FreezeBt_Click);
            // 
            // VoiceBt
            // 
            this.VoiceBt.Location = new System.Drawing.Point(499, 90);
            this.VoiceBt.Name = "VoiceBt";
            this.VoiceBt.Size = new System.Drawing.Size(122, 23);
            this.VoiceBt.TabIndex = 12;
            this.VoiceBt.Text = "Voice";
            this.VoiceBt.UseVisualStyleBackColor = true;
            this.VoiceBt.Click += new System.EventHandler(this.VoiceBt_Click);
            // 
            // JokerBt
            // 
            this.JokerBt.Location = new System.Drawing.Point(499, 3);
            this.JokerBt.Name = "JokerBt";
            this.JokerBt.Size = new System.Drawing.Size(122, 23);
            this.JokerBt.TabIndex = 11;
            this.JokerBt.Text = "Joker";
            this.JokerBt.UseVisualStyleBackColor = true;
            this.JokerBt.Click += new System.EventHandler(this.JokerBt_Click);
            // 
            // WarnBt
            // 
            this.WarnBt.Location = new System.Drawing.Point(627, 3);
            this.WarnBt.Name = "WarnBt";
            this.WarnBt.Size = new System.Drawing.Size(122, 23);
            this.WarnBt.TabIndex = 10;
            this.WarnBt.Text = "Warn";
            this.WarnBt.UseVisualStyleBackColor = true;
            this.WarnBt.Click += new System.EventHandler(this.WarnBt_Click);
            // 
            // MessageBt
            // 
            this.MessageBt.Location = new System.Drawing.Point(3, 177);
            this.MessageBt.Name = "MessageBt";
            this.MessageBt.Size = new System.Drawing.Size(122, 23);
            this.MessageBt.TabIndex = 9;
            this.MessageBt.Text = "Message:";
            this.MessageBt.UseVisualStyleBackColor = true;
            this.MessageBt.Click += new System.EventHandler(this.MessageBt_Click);
            // 
            // PLayersMessageTxt
            // 
            this.PLayersMessageTxt.Location = new System.Drawing.Point(131, 179);
            this.PLayersMessageTxt.Name = "PLayersMessageTxt";
            this.PLayersMessageTxt.Size = new System.Drawing.Size(617, 21);
            this.PLayersMessageTxt.TabIndex = 8;
            this.PLayersMessageTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PLayersMessageTxt_KeyDown);
            // 
            // HideBt
            // 
            this.HideBt.Location = new System.Drawing.Point(371, 117);
            this.HideBt.Name = "HideBt";
            this.HideBt.Size = new System.Drawing.Size(122, 23);
            this.HideBt.TabIndex = 7;
            this.HideBt.Text = "Hide";
            this.HideBt.UseVisualStyleBackColor = true;
            this.HideBt.Click += new System.EventHandler(this.HideBt_Click);
            // 
            // IPBanBt
            // 
            this.IPBanBt.Location = new System.Drawing.Point(627, 90);
            this.IPBanBt.Name = "IPBanBt";
            this.IPBanBt.Size = new System.Drawing.Size(122, 23);
            this.IPBanBt.TabIndex = 6;
            this.IPBanBt.Text = "IP Ban";
            this.IPBanBt.UseVisualStyleBackColor = true;
            this.IPBanBt.Click += new System.EventHandler(this.IPBanBt_Click);
            // 
            // BanBt
            // 
            this.BanBt.Location = new System.Drawing.Point(627, 61);
            this.BanBt.Name = "BanBt";
            this.BanBt.Size = new System.Drawing.Size(122, 23);
            this.BanBt.TabIndex = 5;
            this.BanBt.Text = "Ban";
            this.BanBt.UseVisualStyleBackColor = true;
            this.BanBt.Click += new System.EventHandler(this.BanBt_Click);
            // 
            // KickBt
            // 
            this.KickBt.Location = new System.Drawing.Point(627, 32);
            this.KickBt.Name = "KickBt";
            this.KickBt.Size = new System.Drawing.Size(122, 23);
            this.KickBt.TabIndex = 4;
            this.KickBt.Text = "Kick";
            this.KickBt.UseVisualStyleBackColor = true;
            this.KickBt.Click += new System.EventHandler(this.KickBt_Click);
            // 
            // MapCombo
            // 
            this.MapCombo.FormattingEnabled = true;
            this.MapCombo.Location = new System.Drawing.Point(131, 119);
            this.MapCombo.Name = "MapCombo";
            this.MapCombo.Size = new System.Drawing.Size(234, 21);
            this.MapCombo.TabIndex = 3;
            this.MapCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapCombo_KeyDown);
            // 
            // MapBt
            // 
            this.MapBt.Location = new System.Drawing.Point(3, 119);
            this.MapBt.Name = "MapBt";
            this.MapBt.Size = new System.Drawing.Size(122, 23);
            this.MapBt.TabIndex = 2;
            this.MapBt.Text = "Map:";
            this.MapBt.UseVisualStyleBackColor = true;
            this.MapBt.Click += new System.EventHandler(this.MapBt_Click);
            // 
            // MuteBt
            // 
            this.MuteBt.Location = new System.Drawing.Point(499, 61);
            this.MuteBt.Name = "MuteBt";
            this.MuteBt.Size = new System.Drawing.Size(122, 23);
            this.MuteBt.TabIndex = 13;
            this.MuteBt.Text = "Mute";
            this.MuteBt.UseVisualStyleBackColor = true;
            this.MuteBt.Click += new System.EventHandler(this.MuteBt_Click);
            // 
            // RankTxt
            // 
            this.RankTxt.Location = new System.Drawing.Point(426, 4);
            this.RankTxt.Name = "RankTxt";
            this.RankTxt.ReadOnly = true;
            this.RankTxt.Size = new System.Drawing.Size(134, 21);
            this.RankTxt.TabIndex = 44;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(387, 7);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(33, 13);
            this.label24.TabIndex = 43;
            this.label24.Text = "Rank:";
            // 
            // MapTxt
            // 
            this.MapTxt.Location = new System.Drawing.Point(238, 4);
            this.MapTxt.Name = "MapTxt";
            this.MapTxt.ReadOnly = true;
            this.MapTxt.Size = new System.Drawing.Size(143, 21);
            this.MapTxt.TabIndex = 42;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(201, 7);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(31, 13);
            this.label14.TabIndex = 41;
            this.label14.Text = "Map:";
            // 
            // NameTxtPlayersTab
            // 
            this.NameTxtPlayersTab.Location = new System.Drawing.Point(45, 4);
            this.NameTxtPlayersTab.Name = "NameTxtPlayersTab";
            this.NameTxtPlayersTab.ReadOnly = true;
            this.NameTxtPlayersTab.Size = new System.Drawing.Size(150, 21);
            this.NameTxtPlayersTab.TabIndex = 40;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(5, 7);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 39;
            this.label12.Text = "Name:";
            // 
            // Chat
            // 
            this.Chat.Controls.Add(this.groupBox3);
            this.Chat.Controls.Add(this.groupBox2);
            this.Chat.Controls.Add(this.groupBox1);
            this.Chat.Location = new System.Drawing.Point(4, 22);
            this.Chat.Name = "Chat";
            this.Chat.Padding = new System.Windows.Forms.Padding(3);
            this.Chat.Size = new System.Drawing.Size(767, 488);
            this.Chat.TabIndex = 8;
            this.Chat.Text = "Chat";
            this.Chat.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label40);
            this.groupBox3.Controls.Add(this.txtGlobalLog);
            this.groupBox3.Controls.Add(this.txtGlobalInput);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(8, 327);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(753, 155);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Global Chat";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(7, 131);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(61, 13);
            this.label40.TabIndex = 32;
            this.label40.Text = "GlobalChat:";
            // 
            // txtGlobalLog
            // 
            this.txtGlobalLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtGlobalLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtGlobalLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGlobalLog.Location = new System.Drawing.Point(6, 20);
            this.txtGlobalLog.Multiline = true;
            this.txtGlobalLog.Name = "txtGlobalLog";
            this.txtGlobalLog.ReadOnly = true;
            this.txtGlobalLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtGlobalLog.Size = new System.Drawing.Size(741, 102);
            this.txtGlobalLog.TabIndex = 2;
            // 
            // txtGlobalInput
            // 
            this.txtGlobalInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGlobalInput.Location = new System.Drawing.Point(74, 128);
            this.txtGlobalInput.Name = "txtGlobalInput";
            this.txtGlobalInput.Size = new System.Drawing.Size(673, 21);
            this.txtGlobalInput.TabIndex = 28;
            this.txtGlobalInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGlobalInput_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label32);
            this.groupBox2.Controls.Add(this.txtAdminLog);
            this.groupBox2.Controls.Add(this.txtAdminInput);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(8, 166);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(753, 155);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Admin Chat";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(7, 131);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(62, 13);
            this.label32.TabIndex = 32;
            this.label32.Text = "AdminChat:";
            // 
            // txtAdminLog
            // 
            this.txtAdminLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtAdminLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtAdminLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdminLog.Location = new System.Drawing.Point(6, 20);
            this.txtAdminLog.Multiline = true;
            this.txtAdminLog.Name = "txtAdminLog";
            this.txtAdminLog.ReadOnly = true;
            this.txtAdminLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAdminLog.Size = new System.Drawing.Size(741, 102);
            this.txtAdminLog.TabIndex = 2;
            // 
            // txtAdminInput
            // 
            this.txtAdminInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdminInput.Location = new System.Drawing.Point(75, 128);
            this.txtAdminInput.Name = "txtAdminInput";
            this.txtAdminInput.Size = new System.Drawing.Size(672, 21);
            this.txtAdminInput.TabIndex = 28;
            this.txtAdminInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdminInput_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label33);
            this.groupBox1.Controls.Add(this.txtOpInput);
            this.groupBox1.Controls.Add(this.txtOpLog);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(8, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(753, 155);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Op Chat";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(7, 131);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(44, 13);
            this.label33.TabIndex = 31;
            this.label33.Text = "OpChat:";
            // 
            // txtOpInput
            // 
            this.txtOpInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOpInput.Location = new System.Drawing.Point(57, 128);
            this.txtOpInput.Name = "txtOpInput";
            this.txtOpInput.Size = new System.Drawing.Size(690, 21);
            this.txtOpInput.TabIndex = 30;
            this.txtOpInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOpInput_KeyDown);
            // 
            // txtOpLog
            // 
            this.txtOpLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtOpLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtOpLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOpLog.Location = new System.Drawing.Point(6, 26);
            this.txtOpLog.Multiline = true;
            this.txtOpLog.Name = "txtOpLog";
            this.txtOpLog.ReadOnly = true;
            this.txtOpLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOpLog.Size = new System.Drawing.Size(741, 96);
            this.txtOpLog.TabIndex = 29;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gbMap_Props);
            this.tabPage2.Controls.Add(this.gbMap_Lded);
            this.tabPage2.Controls.Add(this.gbMap_Unld);
            this.tabPage2.Controls.Add(this.gbMap_New);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(767, 488);
            this.tabPage2.TabIndex = 9;
            this.tabPage2.Text = "Maps";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gbMap_Props
            // 
            this.gbMap_Props.Controls.Add(this.pgMaps);
            this.gbMap_Props.Location = new System.Drawing.Point(415, 3);
            this.gbMap_Props.Name = "gbMap_Props";
            this.gbMap_Props.Size = new System.Drawing.Size(343, 349);
            this.gbMap_Props.TabIndex = 5;
            this.gbMap_Props.TabStop = false;
            this.gbMap_Props.Text = "Properties for (none selected)";
            // 
            // pgMaps
            // 
            this.pgMaps.Location = new System.Drawing.Point(7, 20);
            this.pgMaps.Name = "pgMaps";
            this.pgMaps.Size = new System.Drawing.Size(330, 323);
            this.pgMaps.TabIndex = 0;
            this.pgMaps.ToolbarVisible = false;
            // 
            // gbMap_Lded
            // 
            this.gbMap_Lded.Controls.Add(this.lbMap_Lded);
            this.gbMap_Lded.Location = new System.Drawing.Point(7, 3);
            this.gbMap_Lded.Name = "gbMap_Lded";
            this.gbMap_Lded.Size = new System.Drawing.Size(390, 221);
            this.gbMap_Lded.TabIndex = 4;
            this.gbMap_Lded.TabStop = false;
            this.gbMap_Lded.Text = "Loaded levels";
            // 
            // lbMap_Lded
            // 
            this.lbMap_Lded.FormattingEnabled = true;
            this.lbMap_Lded.Location = new System.Drawing.Point(5, 15);
            this.lbMap_Lded.MultiColumn = true;
            this.lbMap_Lded.Name = "lbMap_Lded";
            this.lbMap_Lded.Size = new System.Drawing.Size(379, 199);
            this.lbMap_Lded.TabIndex = 0;
            // 
            // gbMap_Unld
            // 
            this.gbMap_Unld.Controls.Add(this.btnMap_Load);
            this.gbMap_Unld.Controls.Add(this.lbMap_Unld);
            this.gbMap_Unld.Location = new System.Drawing.Point(7, 227);
            this.gbMap_Unld.Name = "gbMap_Unld";
            this.gbMap_Unld.Size = new System.Drawing.Size(390, 258);
            this.gbMap_Unld.TabIndex = 3;
            this.gbMap_Unld.TabStop = false;
            this.gbMap_Unld.Text = "Unloaded levels";
            // 
            // btnMap_Load
            // 
            this.btnMap_Load.Location = new System.Drawing.Point(150, 230);
            this.btnMap_Load.Name = "btnMap_Load";
            this.btnMap_Load.Size = new System.Drawing.Size(75, 23);
            this.btnMap_Load.TabIndex = 1;
            this.btnMap_Load.Text = "Load map";
            this.btnMap_Load.UseVisualStyleBackColor = true;
            this.btnMap_Load.Click += new System.EventHandler(this.MapLoadClick);            
            // 
            // lb_MapUnld
            // 
            this.lbMap_Unld.FormattingEnabled = true;
            this.lbMap_Unld.Location = new System.Drawing.Point(5, 15);
            this.lbMap_Unld.MultiColumn = true;
            this.lbMap_Unld.Name = "lbMap_Unld";
            this.lbMap_Unld.Size = new System.Drawing.Size(379, 212);
            this.lbMap_Unld.TabIndex = 0;
            // 
            // gbMap_New
            // 
            this.gbMap_New.Controls.Add(this.btnMap_Gen);
            this.gbMap_New.Controls.Add(this.lblMap_Type);
            this.gbMap_New.Controls.Add(this.lblMap_Seed);
            this.gbMap_New.Controls.Add(this.lblMap_Z);
            this.gbMap_New.Controls.Add(this.lblMap_X);
            this.gbMap_New.Controls.Add(this.lblMap_Y);
            this.gbMap_New.Controls.Add(this.txtMap_Seed);
            this.gbMap_New.Controls.Add(this.cmbMap_Type);
            this.gbMap_New.Controls.Add(this.cmbMap_Z);
            this.gbMap_New.Controls.Add(this.cmbMap_Y);
            this.gbMap_New.Controls.Add(this.cmbMap_X);
            this.gbMap_New.Controls.Add(this.lblMap_Name);
            this.gbMap_New.Controls.Add(this.txtMap_Name);
            this.gbMap_New.Location = new System.Drawing.Point(415, 358);
            this.gbMap_New.Name = "gbMap_New";
            this.gbMap_New.Size = new System.Drawing.Size(343, 127);
            this.gbMap_New.TabIndex = 0;
            this.gbMap_New.TabStop = false;
            this.gbMap_New.Text = "Create new map";
            // 
            // btnMap_Gen
            // 
            this.btnMap_Gen.Location = new System.Drawing.Point(150, 99);
            this.btnMap_Gen.Name = "btnMap_Gen";
            this.btnMap_Gen.Size = new System.Drawing.Size(75, 23);
            this.btnMap_Gen.TabIndex = 17;
            this.btnMap_Gen.Text = "Generate";
            this.btnMap_Gen.UseVisualStyleBackColor = true;
            this.btnMap_Gen.Click += new System.EventHandler(this.MapGenClick);
            // 
            // lblMap_Type
            // 
            this.lblMap_Type.AutoSize = true;
            this.lblMap_Type.Location = new System.Drawing.Point(13, 78);
            this.lblMap_Type.Name = "lblMap_Type";
            this.lblMap_Type.Size = new System.Drawing.Size(32, 13);
            this.lblMap_Type.TabIndex = 16;
            this.lblMap_Type.Text = "Type:";
            // 
            // lblMap_Seed
            // 
            this.lblMap_Seed.AutoSize = true;
            this.lblMap_Seed.Location = new System.Drawing.Point(192, 78);
            this.lblMap_Seed.Name = "lblMap_Seed";
            this.lblMap_Seed.Size = new System.Drawing.Size(33, 13);
            this.lblMap_Seed.TabIndex = 15;
            this.lblMap_Seed.Text = "Seed:";
            // 
            // lblMap_Z
            // 
            this.lblMap_Z.AutoSize = true;
            this.lblMap_Z.Location = new System.Drawing.Point(231, 51);
            this.lblMap_Z.Name = "lblMap_Z";
            this.lblMap_Z.Size = new System.Drawing.Size(42, 13);
            this.lblMap_Z.TabIndex = 14;
            this.lblMap_Z.Text = "Length:";
            // 
            // lblMap_X
            // 
            this.lblMap_X.AutoSize = true;
            this.lblMap_X.Location = new System.Drawing.Point(7, 51);
            this.lblMap_X.Name = "lblMap_X";
            this.lblMap_X.Size = new System.Drawing.Size(39, 13);
            this.lblMap_X.TabIndex = 13;
            this.lblMap_X.Text = "Width:";
            // 
            // lblMap_Y
            // 
            this.lblMap_Y.AutoSize = true;
            this.lblMap_Y.Location = new System.Drawing.Point(118, 51);
            this.lblMap_Y.Name = "lblMap_Y";
            this.lblMap_Y.Size = new System.Drawing.Size(41, 13);
            this.lblMap_Y.TabIndex = 12;
            this.lblMap_Y.Text = "Height:";
            // 
            // txtMap_Seed
            // 
            this.txtMap_Seed.Location = new System.Drawing.Point(231, 75);
            this.txtMap_Seed.Name = "txtMap_Seed";
            this.txtMap_Seed.Size = new System.Drawing.Size(107, 21);
            this.txtMap_Seed.TabIndex = 11;
            // 
            // cmbMap_Type
            // 
            this.cmbMap_Type.FormattingEnabled = true;
            this.cmbMap_Type.Items.AddRange(new object[] {
            "Island",
            "Mountains",
            "Forest",
            "Ocean",
            "Flat",
            "Pixel",
            "Desert",
            "Space",
            "Rainbow",
            "Hell"});
            this.cmbMap_Type.Location = new System.Drawing.Point(51, 75);
            this.cmbMap_Type.Name = "cmbMap_Type";
            this.cmbMap_Type.Size = new System.Drawing.Size(121, 21);
            this.cmbMap_Type.TabIndex = 10;
            // 
            // cmbMap_Z
            // 
            this.cmbMap_Z.FormattingEnabled = true;
            this.cmbMap_Z.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.cmbMap_Z.Location = new System.Drawing.Point(279, 48);
            this.cmbMap_Z.Name = "cmbMap_Z";
            this.cmbMap_Z.Size = new System.Drawing.Size(60, 21);
            this.cmbMap_Z.TabIndex = 9;
            // 
            // cmbMap_Y
            // 
            this.cmbMap_Y.FormattingEnabled = true;
            this.cmbMap_Y.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.cmbMap_Y.Location = new System.Drawing.Point(165, 48);
            this.cmbMap_Y.Name = "cmbMap_Y";
            this.cmbMap_Y.Size = new System.Drawing.Size(60, 21);
            this.cmbMap_Y.TabIndex = 8;
            // 
            // cmbMap_X
            // 
            this.cmbMap_X.FormattingEnabled = true;
            this.cmbMap_X.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.cmbMap_X.Location = new System.Drawing.Point(52, 48);
            this.cmbMap_X.Name = "cmbMap_X";
            this.cmbMap_X.Size = new System.Drawing.Size(60, 21);
            this.cmbMap_X.TabIndex = 7;
            // 
            // lblMap_Name
            // 
            this.lblMap_Name.AutoSize = true;
            this.lblMap_Name.Location = new System.Drawing.Point(7, 24);
            this.lblMap_Name.Name = "lblMap_Name";
            this.lblMap_Name.Size = new System.Drawing.Size(38, 13);
            this.lblMap_Name.TabIndex = 6;
            this.lblMap_Name.Text = "Name:";
            // 
            // txtMap_Name
            // 
            this.txtMap_Name.Location = new System.Drawing.Point(51, 21);
            this.txtMap_Name.Name = "txtMap_Name";
            this.txtMap_Name.Size = new System.Drawing.Size(287, 21);
            this.txtMap_Name.TabIndex = 0;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 523);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnProperties);
            this.Controls.Add(this.Restart);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Window_FormClosing);
            this.Load += new System.EventHandler(this.Window_Load);
            this.Resize += new System.EventHandler(this.Window_Resize);
            this.mapsStrip.ResumeLayout(false);
            this.playerStrip.ResumeLayout(false);
            this.iconContext.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabsLogs.ResumeLayout(false);
            this.tabLog_Err.ResumeLayout(false);
            this.tabLog_Err.PerformLayout();
            this.tabLog_Gen.ResumeLayout(false);
            this.tabLog_Gen.PerformLayout();
            this.tabLog_Sys.ResumeLayout(false);
            this.tabLog_Sys.PerformLayout();
            this.tabLog_Chg.ResumeLayout(false);
            this.tabLog_Chg.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.gBCommands.ResumeLayout(false);
            this.gBCommands.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaps)).EndInit();
            this.gBChat.ResumeLayout(false);
            this.txtLogMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drownNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Fallnumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.physlvlnumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMapsTab)).EndInit();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.Chat.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.gbMap_Props.ResumeLayout(false);
            this.gbMap_Lded.ResumeLayout(false);
            this.gbMap_Unld.ResumeLayout(false);
            this.gbMap_New.ResumeLayout(false);
            this.gbMap_New.PerformLayout();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.TextBox txtMap_Name;
        private System.Windows.Forms.Label lblMap_Name;
        private System.Windows.Forms.ComboBox cmbMap_X;
        private System.Windows.Forms.ComboBox cmbMap_Y;
        private System.Windows.Forms.ComboBox cmbMap_Z;
        private System.Windows.Forms.ComboBox cmbMap_Type;
        private System.Windows.Forms.TextBox txtMap_Seed;
        private System.Windows.Forms.Label lblMap_Y;
        private System.Windows.Forms.Label lblMap_X;
        private System.Windows.Forms.Label lblMap_Z;
        private System.Windows.Forms.Label lblMap_Seed;
        private System.Windows.Forms.Label lblMap_Type;
        private System.Windows.Forms.Button btnMap_Gen;
        private System.Windows.Forms.GroupBox gbMap_New;
        private System.Windows.Forms.ListBox lbMap_Unld;
        private System.Windows.Forms.Button btnMap_Load;
        private System.Windows.Forms.GroupBox gbMap_Unld;
        private System.Windows.Forms.ListBox lbMap_Lded;
        private System.Windows.Forms.GroupBox gbMap_Lded;
        private System.Windows.Forms.PropertyGrid pgMaps;
        private System.Windows.Forms.GroupBox gbMap_Props;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabLog_Chg;
        private System.Windows.Forms.TabPage tabLog_Sys;
        private System.Windows.Forms.TabPage tabLog_Err;
        private System.Windows.Forms.TabPage tabLog_Gen;
        private System.Windows.Forms.TabControl tabsLogs;

        #endregion

        private Button btnClose;
        private ContextMenuStrip iconContext;
        private ToolStripMenuItem openConsole;
        private ToolStripMenuItem shutdownServer;
        private ContextMenuStrip playerStrip;
        private ToolStripMenuItem whoisToolStripMenuItem;
        private ToolStripMenuItem kickToolStripMenuItem;
        private ToolStripMenuItem banToolStripMenuItem;
        private ToolStripMenuItem voiceToolStripMenuItem;
        private ContextMenuStrip mapsStrip;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem finiteModeToolStripMenuItem;
        private ToolStripMenuItem animalAIToolStripMenuItem;
        private ToolStripMenuItem edgeWaterToolStripMenuItem;
        private ToolStripMenuItem growingGrassToolStripMenuItem;
        private ToolStripMenuItem survivalDeathToolStripMenuItem;
        private ToolStripMenuItem killerBlocksToolStripMenuItem;
        private ToolStripMenuItem rPChatToolStripMenuItem;
        private ToolStripMenuItem clonesToolStripMenuItem;
        private Button Restart;
        private ToolStripMenuItem restartServerToolStripMenuItem;
        private TabPage tabPage5;
        private Label label3;
        private TextBox txtErrors;
        private TextBox txtChangelog;
        private TextBox txtSystem;
        private TabPage tabPage1;
        private GroupBox gBCommands;
        private AutoScrollTextBox txtCommandsUsed;
        private DataGridView dgvMaps;
        private GroupBox gBChat;
        private Label label2;
        private TextBox txtCommands;
        private TextBox txtInput;
        private TextBox txtUrl;
        private DataGridView dgvPlayers;
        private Label label1;
        private TabControl tabControl1;
        private TabPage tabPage6;
        private Label label11;
        private Button SaveMap;
        private DataGridView dgvMapsTab;
        private Panel panel2;
        private Label label13;
        private Label label17;
        private Label label15;
        private Label label19;
        private Label label18;
        private Label label20;
        private Label label21;
        private ToolStripMenuItem promoteToolStripMenuItem;
        private ToolStripMenuItem demoteToolStripMenuItem;
        private ComboBox Aicombo;
        private CheckBox edgewaterchk;
        private CheckBox grasschk;
        private CheckBox finitechk;
        private CheckBox Killerbloxchk;
        private CheckBox SurvivalStyleDeathchk;
        private CheckBox chatlvlchk;
        private NumericUpDown physlvlnumeric;
        private TextBox MOTDtxt;
        private Label label5;
        private NumericUpDown drownNumeric;
        private NumericUpDown Fallnumeric;
        private Label label22;
        private CheckBox Gunschk;
        private Label label6;
        private CheckBox AutoLoadChk;
        private Label label23;
        private TabPage tabPage7;
        internal RichTextBox LogsTxtBox;
        private DateTimePicker dateTimePicker1;
        private Panel panel4;
        private TextBox RankTxt;
        private Label label24;
        private TextBox MapTxt;
        private Label label14;
        private TextBox NameTxtPlayersTab;
        private Label label12;
        private Button IPBanBt;
        private Button BanBt;
        private Button KickBt;
        private ComboBox MapCombo;
        private Button MapBt;
        private TextBox IPtxt;
        private Label label26;
        private Button MessageBt;
        private TextBox PLayersMessageTxt;
        private Button HideBt;
        private Label label30;
        private TextBox TimesLoggedInTxt;
        private Label label29;
        private TextBox Blockstxt;
        private Label label28;
        private TextBox DeathsTxt;
        private Label label27;
        private TextBox LoggedinForTxt;
        private Label label31;
        private TextBox Kickstxt;
        private Button WarnBt;
        private Button VoiceBt;
        private Button JokerBt;
        private Button MuteBt;
        private Button FreezeBt;
        private Button LogoutBt;
        private Button LoginBt;
        private TextBox LoginTxt;
        private TextBox LogoutTxt;
        private TextBox TitleTxt;
        private ComboBox ColorCombo;
        private Button ColorBt;
        private Button TitleBt;
        private Button JailBt;
        private Button DemoteBt;
        private Button PromoteBt;
        private Button KillBt;
        private TextBox StatusTxt;
        private Label label25;
        private TextBox ImpersonateORSendCmdTxt;
        private Button ImpersonateORSendCmdBt;
        private Button SpawnBt;
        private TextBox UndoTxt;
        private Button UndoBt;
        private Button SlapBt;
        private Button SendRulesTxt;
        private AutoScrollTextBox PlayersTextBox;
        private ListBox PlyersListBox;
        private TabPage Chat;
        private GroupBox groupBox2;
        private Label label32;
        private AutoScrollTextBox txtAdminLog;
        private TextBox txtAdminInput;
        private GroupBox groupBox1;
        private Label label33;
        private TextBox txtOpInput;
        private AutoScrollTextBox txtOpLog;
        private Button button_saveall;
        private Button Unloadempty_button;
        private Button killphysics_button;
        private ToolStripMenuItem unloadToolStripMenuItem1;
        private ToolStripMenuItem loadOngotoToolStripMenuItem;
        private ToolStripMenuItem autpPhysicsToolStripMenuItem;
        private ToolStripMenuItem instantBuildingToolStripMenuItem;
        private ToolStripMenuItem gunsToolStripMenuItem;
        private ToolStripMenuItem infoToolStripMenuItem;
        private ToolStripMenuItem actiondToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem unloadToolStripMenuItem;
        private ToolStripMenuItem moveAllToolStripMenuItem;
        private ToolStripMenuItem reloadToolStripMenuItem;
        private Label label16;
        private Label label35;
        private CheckBox LoadOnGotoChk;
        private CheckBox UnloadChk;
        private Label label36;
        private ToolStripMenuItem randomFlowToolStripMenuItem;
        private ToolStripMenuItem leafDecayToolStripMenuItem;
        private CheckBox chkRndFlow;
        private Label label37;
        private CheckBox leafDecayChk;
        private Label label38;
        private ToolStripMenuItem treeGrowingToolStripMenuItem;
        private CheckBox TreeGrowChk;
        private Label label39;
        private ToolStripMenuItem physicsToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem physicsToolStripMenuItem1;
        private ToolStripMenuItem loadingToolStripMenuItem;
        private ToolStripMenuItem miscToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private GroupBox groupBox3;
        private Label label40;
        private AutoScrollTextBox txtGlobalLog;
        private TextBox txtGlobalInput;
        private Components.ColoredTextBox txtLog;
        private ContextMenuStrip txtLogMenuStrip;
        private ToolStripMenuItem nightModeToolStripMenuItem;
        private ToolStripMenuItem colorsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem copySelectedToolStripMenuItem;
        private ToolStripMenuItem copyAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem dateStampToolStripMenuItem;
        private ToolStripMenuItem autoScrollToolStripMenuItem;
        private Button btnProperties;
    }
}
