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
            this.main_btnProps = new System.Windows.Forms.Button();
            this.main_btnClose = new System.Windows.Forms.Button();
            this.main_btnRestart = new System.Windows.Forms.Button();
            this.logs_tp = new System.Windows.Forms.TabPage();
            this.logs_tab = new System.Windows.Forms.TabControl();
            this.logs_tabErr = new System.Windows.Forms.TabPage();
            this.logs_txtError = new System.Windows.Forms.TextBox();
            this.logs_tabGen = new System.Windows.Forms.TabPage();
            this.logs_lblGeneral = new System.Windows.Forms.Label();
            this.logs_dateGeneral = new System.Windows.Forms.DateTimePicker();
            this.logs_txtGeneral = new System.Windows.Forms.RichTextBox();
            this.tabLog_Sys = new System.Windows.Forms.TabPage();
            this.logs_txtSystem = new System.Windows.Forms.TextBox();
            this.tabLog_Chg = new System.Windows.Forms.TabPage();
            this.logs_txtChangelog = new System.Windows.Forms.TextBox();
            this.tp_Main = new System.Windows.Forms.TabPage();
            this.main_btnUnloadEmpty = new System.Windows.Forms.Button();
            this.main_btnKillPhysics = new System.Windows.Forms.Button();
            this.main_btnSaveAll = new System.Windows.Forms.Button();
            this.main_Maps = new System.Windows.Forms.DataGridView();
            this.main_txtLog = new MCGalaxy.Gui.Components.ColoredTextBox();
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
            this.main_txtInput = new System.Windows.Forms.TextBox();
            this.main_txtUrl = new System.Windows.Forms.TextBox();
            this.main_Players = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tp_Players = new System.Windows.Forms.TabPage();
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
            this.tp_Maps = new System.Windows.Forms.TabPage();
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mapsStrip.SuspendLayout();
            this.playerStrip.SuspendLayout();
            this.iconContext.SuspendLayout();
            this.logs_tp.SuspendLayout();
            this.logs_tab.SuspendLayout();
            this.logs_tabErr.SuspendLayout();
            this.logs_tabGen.SuspendLayout();
            this.tabLog_Sys.SuspendLayout();
            this.tabLog_Chg.SuspendLayout();
            this.tp_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.main_Maps)).BeginInit();
            this.txtLogMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.main_Players)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tp_Players.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tp_Maps.SuspendLayout();
            this.gbMap_Props.SuspendLayout();
            this.gbMap_Lded.SuspendLayout();
            this.gbMap_Unld.SuspendLayout();
            this.gbMap_New.SuspendLayout();
            this.SuspendLayout();
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
            // main_btnProps
            // 
            this.main_btnProps.Cursor = System.Windows.Forms.Cursors.Hand;
            this.main_btnProps.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnProps.Location = new System.Drawing.Point(501, 5);
            this.main_btnProps.Name = "main_btnProps";
            this.main_btnProps.Size = new System.Drawing.Size(80, 23);
            this.main_btnProps.TabIndex = 34;
            this.main_btnProps.Text = "Properties";
            this.main_btnProps.UseVisualStyleBackColor = true;
            this.main_btnProps.Click += new System.EventHandler(this.btnProperties_Click_1);
            // 
            // main_btnClose
            // 
            this.main_btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.main_btnClose.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnClose.Location = new System.Drawing.Point(675, 5);
            this.main_btnClose.Name = "main_btnClose";
            this.main_btnClose.Size = new System.Drawing.Size(88, 23);
            this.main_btnClose.TabIndex = 35;
            this.main_btnClose.Text = "Close";
            this.main_btnClose.UseVisualStyleBackColor = true;
            this.main_btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // main_btnRestart
            // 
            this.main_btnRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.main_btnRestart.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnRestart.Location = new System.Drawing.Point(584, 5);
            this.main_btnRestart.Name = "main_btnRestart";
            this.main_btnRestart.Size = new System.Drawing.Size(88, 23);
            this.main_btnRestart.TabIndex = 36;
            this.main_btnRestart.Text = "Restart";
            this.main_btnRestart.UseVisualStyleBackColor = true;
            this.main_btnRestart.Click += new System.EventHandler(this.Restart_Click);
            // 
            // logs_tp
            // 
            this.logs_tp.BackColor = System.Drawing.SystemColors.Control;
            this.logs_tp.Controls.Add(this.logs_tab);
            this.logs_tp.Location = new System.Drawing.Point(4, 22);
            this.logs_tp.Name = "logs_tp";
            this.logs_tp.Padding = new System.Windows.Forms.Padding(3);
            this.logs_tp.Size = new System.Drawing.Size(767, 488);
            this.logs_tp.TabIndex = 4;
            this.logs_tp.Text = "Logs";
            // 
            // logs_tab
            // 
            this.logs_tab.Controls.Add(this.logs_tabErr);
            this.logs_tab.Controls.Add(this.logs_tabGen);
            this.logs_tab.Controls.Add(this.tabLog_Sys);
            this.logs_tab.Controls.Add(this.tabLog_Chg);
            this.logs_tab.Location = new System.Drawing.Point(-1, 1);
            this.logs_tab.Name = "logs_tab";
            this.logs_tab.SelectedIndex = 0;
            this.logs_tab.Size = new System.Drawing.Size(775, 491);
            this.logs_tab.TabIndex = 0;
            // 
            // logs_tabErr
            // 
            this.logs_tabErr.Controls.Add(this.logs_txtError);
            this.logs_tabErr.Location = new System.Drawing.Point(4, 22);
            this.logs_tabErr.Name = "logs_tabErr";
            this.logs_tabErr.Size = new System.Drawing.Size(767, 465);
            this.logs_tabErr.TabIndex = 2;
            this.logs_tabErr.Text = "Errors";
            this.logs_tabErr.UseVisualStyleBackColor = true;
            // 
            // logs_txtError
            // 
            this.logs_txtError.BackColor = System.Drawing.Color.White;
            this.logs_txtError.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.logs_txtError.Location = new System.Drawing.Point(-2, 0);
            this.logs_txtError.Multiline = true;
            this.logs_txtError.Name = "logs_txtError";
            this.logs_txtError.ReadOnly = true;
            this.logs_txtError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logs_txtError.Size = new System.Drawing.Size(765, 465);
            this.logs_txtError.TabIndex = 2;
            // 
            // logs_tabGen
            // 
            this.logs_tabGen.Controls.Add(this.logs_lblGeneral);
            this.logs_tabGen.Controls.Add(this.logs_dateGeneral);
            this.logs_tabGen.Controls.Add(this.logs_txtGeneral);
            this.logs_tabGen.Location = new System.Drawing.Point(4, 22);
            this.logs_tabGen.Name = "logs_tabGen";
            this.logs_tabGen.Padding = new System.Windows.Forms.Padding(3);
            this.logs_tabGen.Size = new System.Drawing.Size(767, 465);
            this.logs_tabGen.TabIndex = 0;
            this.logs_tabGen.Text = "General";
            this.logs_tabGen.UseVisualStyleBackColor = true;
            // 
            // logs_lblGeneral
            // 
            this.logs_lblGeneral.AutoSize = true;
            this.logs_lblGeneral.Location = new System.Drawing.Point(3, 9);
            this.logs_lblGeneral.Name = "logs_lblGeneral";
            this.logs_lblGeneral.Size = new System.Drawing.Size(78, 13);
            this.logs_lblGeneral.TabIndex = 6;
            this.logs_lblGeneral.Text = "View logs from:";
            // 
            // logs_dateGeneral
            // 
            this.logs_dateGeneral.Location = new System.Drawing.Point(87, 4);
            this.logs_dateGeneral.Name = "logs_dateGeneral";
            this.logs_dateGeneral.Size = new System.Drawing.Size(200, 21);
            this.logs_dateGeneral.TabIndex = 5;
            this.logs_dateGeneral.Value = new System.DateTime(2011, 7, 20, 18, 31, 50, 0);
            this.logs_dateGeneral.ValueChanged += new System.EventHandler(this.DatePicker1_ValueChanged);
            // 
            // logs_txtGeneral
            // 
            this.logs_txtGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.logs_txtGeneral.Location = new System.Drawing.Point(-2, 30);
            this.logs_txtGeneral.Name = "logs_txtGeneral";
            this.logs_txtGeneral.ReadOnly = true;
            this.logs_txtGeneral.Size = new System.Drawing.Size(765, 436);
            this.logs_txtGeneral.TabIndex = 4;
            this.logs_txtGeneral.Text = "";
            // 
            // tabLog_Sys
            // 
            this.tabLog_Sys.Controls.Add(this.logs_txtSystem);
            this.tabLog_Sys.Location = new System.Drawing.Point(4, 22);
            this.tabLog_Sys.Name = "tabLog_Sys";
            this.tabLog_Sys.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog_Sys.Size = new System.Drawing.Size(767, 465);
            this.tabLog_Sys.TabIndex = 1;
            this.tabLog_Sys.Text = "System";
            this.tabLog_Sys.UseVisualStyleBackColor = true;
            // 
            // logs_txtSystem
            // 
            this.logs_txtSystem.BackColor = System.Drawing.Color.White;
            this.logs_txtSystem.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.logs_txtSystem.Location = new System.Drawing.Point(-2, 0);
            this.logs_txtSystem.Multiline = true;
            this.logs_txtSystem.Name = "logs_txtSystem";
            this.logs_txtSystem.ReadOnly = true;
            this.logs_txtSystem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logs_txtSystem.Size = new System.Drawing.Size(765, 465);
            this.logs_txtSystem.TabIndex = 2;
            // 
            // tabLog_Chg
            // 
            this.tabLog_Chg.Controls.Add(this.logs_txtChangelog);
            this.tabLog_Chg.Location = new System.Drawing.Point(4, 22);
            this.tabLog_Chg.Name = "tabLog_Chg";
            this.tabLog_Chg.Size = new System.Drawing.Size(767, 465);
            this.tabLog_Chg.TabIndex = 3;
            this.tabLog_Chg.Text = "Changelog";
            this.tabLog_Chg.UseVisualStyleBackColor = true;
            // 
            // logs_txtChangelog
            // 
            this.logs_txtChangelog.BackColor = System.Drawing.Color.White;
            this.logs_txtChangelog.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.logs_txtChangelog.Location = new System.Drawing.Point(-2, 0);
            this.logs_txtChangelog.Multiline = true;
            this.logs_txtChangelog.Name = "logs_txtChangelog";
            this.logs_txtChangelog.ReadOnly = true;
            this.logs_txtChangelog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logs_txtChangelog.Size = new System.Drawing.Size(765, 465);
            this.logs_txtChangelog.TabIndex = 1;
            // 
            // tp_Main
            // 
            this.tp_Main.BackColor = System.Drawing.SystemColors.Control;
            this.tp_Main.Controls.Add(this.main_btnUnloadEmpty);
            this.tp_Main.Controls.Add(this.main_btnKillPhysics);
            this.tp_Main.Controls.Add(this.main_btnSaveAll);
            this.tp_Main.Controls.Add(this.main_Maps);
            this.tp_Main.Controls.Add(this.main_txtLog);
            this.tp_Main.Controls.Add(this.main_txtInput);
            this.tp_Main.Controls.Add(this.main_txtUrl);
            this.tp_Main.Controls.Add(this.main_Players);
            this.tp_Main.Location = new System.Drawing.Point(4, 22);
            this.tp_Main.Name = "tp_Main";
            this.tp_Main.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Main.Size = new System.Drawing.Size(767, 488);
            this.tp_Main.TabIndex = 0;
            this.tp_Main.Text = "Main";
            // 
            // main_btnUnloadEmpty
            // 
            this.main_btnUnloadEmpty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.main_btnUnloadEmpty.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnUnloadEmpty.Location = new System.Drawing.Point(676, 263);
            this.main_btnUnloadEmpty.Name = "main_btnUnloadEmpty";
            this.main_btnUnloadEmpty.Size = new System.Drawing.Size(81, 23);
            this.main_btnUnloadEmpty.TabIndex = 41;
            this.main_btnUnloadEmpty.Text = "Unload Empty";
            this.main_btnUnloadEmpty.UseVisualStyleBackColor = true;
            this.main_btnUnloadEmpty.Click += new System.EventHandler(this.Unloadempty_button_Click);
            // 
            // main_btnKillPhysics
            // 
            this.main_btnKillPhysics.Cursor = System.Windows.Forms.Cursors.Hand;
            this.main_btnKillPhysics.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnKillPhysics.Location = new System.Drawing.Point(582, 263);
            this.main_btnKillPhysics.Name = "main_btnKillPhysics";
            this.main_btnKillPhysics.Size = new System.Drawing.Size(88, 23);
            this.main_btnKillPhysics.TabIndex = 40;
            this.main_btnKillPhysics.Text = "Kill All Physics";
            this.main_btnKillPhysics.UseVisualStyleBackColor = true;
            this.main_btnKillPhysics.Click += new System.EventHandler(this.killphysics_button_Click);
            // 
            // main_btnSaveAll
            // 
            this.main_btnSaveAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.main_btnSaveAll.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnSaveAll.Location = new System.Drawing.Point(513, 263);
            this.main_btnSaveAll.Name = "main_btnSaveAll";
            this.main_btnSaveAll.Size = new System.Drawing.Size(63, 23);
            this.main_btnSaveAll.TabIndex = 39;
            this.main_btnSaveAll.Text = "Save All";
            this.main_btnSaveAll.UseVisualStyleBackColor = true;
            this.main_btnSaveAll.Click += new System.EventHandler(this.button_saveall_Click);
            // 
            // main_Maps
            // 
            this.main_Maps.AllowUserToAddRows = false;
            this.main_Maps.AllowUserToDeleteRows = false;
            this.main_Maps.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.main_Maps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.main_Maps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.main_Maps.ContextMenuStrip = this.mapsStrip;
            this.main_Maps.Location = new System.Drawing.Point(512, 292);
            this.main_Maps.MultiSelect = false;
            this.main_Maps.Name = "main_Maps";
            this.main_Maps.ReadOnly = true;
            this.main_Maps.RowHeadersVisible = false;
            this.main_Maps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.main_Maps.Size = new System.Drawing.Size(246, 150);
            this.main_Maps.TabIndex = 38;
            // 
            // main_txtLog
            // 
            this.main_txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.main_txtLog.ContextMenuStrip = this.txtLogMenuStrip;
            this.main_txtLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_txtLog.Location = new System.Drawing.Point(8, 38);
            this.main_txtLog.Name = "main_txtLog";
            this.main_txtLog.ReadOnly = true;
            this.main_txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.main_txtLog.Size = new System.Drawing.Size(498, 404);
            this.main_txtLog.TabIndex = 0;
            this.main_txtLog.Text = "";
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
            // main_txtInput
            // 
            this.main_txtInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_txtInput.Location = new System.Drawing.Point(8, 454);
            this.main_txtInput.Name = "main_txtInput";
            this.main_txtInput.Size = new System.Drawing.Size(750, 21);
            this.main_txtInput.TabIndex = 27;
            this.main_txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            this.toolTip.SetToolTip(this.main_txtInput, "To send chat to players, just type the message in.\nTo enter a command, put a / before it. (e.g. /help commands)");
            // 
            // main_txtUrl
            // 
            this.main_txtUrl.Cursor = System.Windows.Forms.Cursors.Default;
            this.main_txtUrl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_txtUrl.Location = new System.Drawing.Point(8, 7);
            this.main_txtUrl.Name = "main_txtUrl";
            this.main_txtUrl.ReadOnly = true;
            this.main_txtUrl.Size = new System.Drawing.Size(498, 21);
            this.main_txtUrl.TabIndex = 25;
            this.main_txtUrl.Text = "Finding classicube.net url..";
            this.main_txtUrl.DoubleClick += new System.EventHandler(this.txtUrl_DoubleClick);
            // 
            // main_Players
            // 
            this.main_Players.AllowUserToAddRows = false;
            this.main_Players.AllowUserToDeleteRows = false;
            this.main_Players.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.main_Players.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.main_Players.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.main_Players.ContextMenuStrip = this.playerStrip;
            this.main_Players.Location = new System.Drawing.Point(512, 7);
            this.main_Players.MultiSelect = false;
            this.main_Players.Name = "main_Players";
            this.main_Players.ReadOnly = true;
            this.main_Players.RowHeadersVisible = false;
            this.main_Players.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.main_Players.Size = new System.Drawing.Size(246, 250);
            this.main_Players.TabIndex = 37;
            this.main_Players.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvPlayers_RowPrePaint);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tp_Main);
            this.tabControl1.Controls.Add(this.logs_tp);
            this.tabControl1.Controls.Add(this.tp_Maps);
            this.tabControl1.Controls.Add(this.tp_Players);
            this.tabControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabControl1.Font = new System.Drawing.Font("Calibri", 8.25F);
            this.tabControl1.Location = new System.Drawing.Point(1, 11);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(775, 514);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.Click += new System.EventHandler(this.tabControl1_Click);
            //
            // tp_Players
            // 
            this.tp_Players.Controls.Add(this.PlayersTextBox);
            this.tp_Players.Controls.Add(this.PlyersListBox);
            this.tp_Players.Controls.Add(this.StatusTxt);
            this.tp_Players.Controls.Add(this.label25);
            this.tp_Players.Controls.Add(this.LoggedinForTxt);
            this.tp_Players.Controls.Add(this.label31);
            this.tp_Players.Controls.Add(this.Kickstxt);
            this.tp_Players.Controls.Add(this.label30);
            this.tp_Players.Controls.Add(this.TimesLoggedInTxt);
            this.tp_Players.Controls.Add(this.label29);
            this.tp_Players.Controls.Add(this.Blockstxt);
            this.tp_Players.Controls.Add(this.label28);
            this.tp_Players.Controls.Add(this.DeathsTxt);
            this.tp_Players.Controls.Add(this.label27);
            this.tp_Players.Controls.Add(this.IPtxt);
            this.tp_Players.Controls.Add(this.label26);
            this.tp_Players.Controls.Add(this.panel4);
            this.tp_Players.Controls.Add(this.RankTxt);
            this.tp_Players.Controls.Add(this.label24);
            this.tp_Players.Controls.Add(this.MapTxt);
            this.tp_Players.Controls.Add(this.label14);
            this.tp_Players.Controls.Add(this.NameTxtPlayersTab);
            this.tp_Players.Controls.Add(this.label12);
            this.tp_Players.Location = new System.Drawing.Point(4, 22);
            this.tp_Players.Name = "tp_Players";
            this.tp_Players.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Players.Size = new System.Drawing.Size(767, 488);
            this.tp_Players.TabIndex = 7;
            this.tp_Players.Text = "Players";
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
            // tp_Maps
            // 
            this.tp_Maps.BackColor = System.Drawing.SystemColors.Control;
            this.tp_Maps.Controls.Add(this.gbMap_Props);
            this.tp_Maps.Controls.Add(this.gbMap_Lded);
            this.tp_Maps.Controls.Add(this.gbMap_Unld);
            this.tp_Maps.Controls.Add(this.gbMap_New);
            this.tp_Maps.Location = new System.Drawing.Point(4, 22);
            this.tp_Maps.Name = "tp_Maps";
            this.tp_Maps.Size = new System.Drawing.Size(767, 488);
            this.tp_Maps.TabIndex = 9;
            this.tp_Maps.Text = "Maps";
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
            this.lbMap_Lded.SelectedIndexChanged += new System.EventHandler(this.UpdateSelectedMap);
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
            this.Controls.Add(this.main_btnClose);
            this.Controls.Add(this.main_btnProps);
            this.Controls.Add(this.main_btnRestart);
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
            this.logs_tp.ResumeLayout(false);
            this.logs_tab.ResumeLayout(false);
            this.logs_tabErr.ResumeLayout(false);
            this.logs_tabErr.PerformLayout();
            this.logs_tabGen.ResumeLayout(false);
            this.logs_tabGen.PerformLayout();
            this.tabLog_Sys.ResumeLayout(false);
            this.tabLog_Sys.PerformLayout();
            this.tabLog_Chg.ResumeLayout(false);
            this.tabLog_Chg.PerformLayout();
            this.tp_Main.ResumeLayout(false);
            this.tp_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.main_Maps)).EndInit();
            this.txtLogMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.main_Players)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tp_Players.ResumeLayout(false);
            this.tp_Players.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tp_Maps.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage tp_Main;
        private System.Windows.Forms.TabPage tabLog_Chg;
        private System.Windows.Forms.TabPage tabLog_Sys;
        private System.Windows.Forms.TabPage logs_tabErr;
        private System.Windows.Forms.TabPage logs_tabGen;
        private System.Windows.Forms.TabControl logs_tab;

        #endregion

        private Button main_btnClose;
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
        private Button main_btnRestart;
        private ToolStripMenuItem restartServerToolStripMenuItem;
        private TabPage logs_tp;
        private Label logs_lblGeneral;
        private TextBox logs_txtError;
        private TextBox logs_txtChangelog;
        private TextBox logs_txtSystem;
        private TabPage tp_Maps;
        private DataGridView main_Maps;
        private TextBox main_txtInput;
        private TextBox main_txtUrl;
        private DataGridView main_Players;
        private TabControl tabControl1;
        private ToolStripMenuItem promoteToolStripMenuItem;
        private ToolStripMenuItem demoteToolStripMenuItem;
        private TabPage tp_Players;
        internal RichTextBox logs_txtGeneral;
        private DateTimePicker logs_dateGeneral;
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
        private Button main_btnSaveAll;
        private Button main_btnUnloadEmpty;
        private Button main_btnKillPhysics;
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
        private ToolStripMenuItem randomFlowToolStripMenuItem;
        private ToolStripMenuItem leafDecayToolStripMenuItem;
        private ToolStripMenuItem treeGrowingToolStripMenuItem;
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
        private Components.ColoredTextBox main_txtLog;
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
        private Button main_btnProps;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
