/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
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
            this.tsMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMap_physicsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_physics0 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_physics1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_physics2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_physics3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_physics4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_physics5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_actionsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_Reload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_Unload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_moveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMap_separator = new System.Windows.Forms.ToolStripSeparator();
            this.tsMap_info = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlayer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsPlayer_whois = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlayer_kick = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlayer_ban = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlayer_voice = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlayer_clones = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlayer_promote = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlayer_demote = new System.Windows.Forms.ToolStripMenuItem();
            this.icon_context = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.icon_hideWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.icon_separator = new System.Windows.Forms.ToolStripSeparator();
            this.icon_openConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.icon_shutdown = new System.Windows.Forms.ToolStripMenuItem();
            this.icon_restart = new System.Windows.Forms.ToolStripMenuItem();
            this.main_btnProps = new System.Windows.Forms.Button();
            this.main_btnClose = new System.Windows.Forms.Button();
            this.main_btnRestart = new System.Windows.Forms.Button();
            this.logs_tp = new System.Windows.Forms.TabPage();
            this.logs_tab = new System.Windows.Forms.TabControl();
            this.logs_tabErr = new System.Windows.Forms.TabPage();
            this.logs_txtError = new System.Windows.Forms.TextBox();
            this.logs_tabGen = new System.Windows.Forms.TabPage();
            this.panel_tabGenTop = new System.Windows.Forms.Panel();
            this.logs_lblGeneral = new System.Windows.Forms.Label();
            this.logs_dateGeneral = new System.Windows.Forms.DateTimePicker();
            this.logs_txtGeneral = new System.Windows.Forms.RichTextBox();
            this.tabLog_Sys = new System.Windows.Forms.TabPage();
            this.logs_txtSystem = new System.Windows.Forms.TextBox();
            this.tp_Main = new System.Windows.Forms.TabPage();
            this.panel_mainRight = new System.Windows.Forms.Panel();
            this.main_Players = new System.Windows.Forms.DataGridView();
            this.main_colPlName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.main_colPlMap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.main_colPlRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.main_Maps = new System.Windows.Forms.DataGridView();
            this.main_colLvlName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.main_colLvlPlayers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.main_colLvlPhysics = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.main_btnSaveAll = new System.Windows.Forms.Button();
            this.main_btnUnloadEmpty = new System.Windows.Forms.Button();
            this.main_btnKillPhysics = new System.Windows.Forms.Button();
            this.panel_mainLeft = new System.Windows.Forms.Panel();
            this.main_txtUrl = new System.Windows.Forms.TextBox();
            this.main_txtLog = new MCGalaxy.Gui.Components.ColoredTextBox();
            this.tsLog_Menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsLog_night = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLog_Colored = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLog_dateStamp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLog_autoScroll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLog_separator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsLog_copySelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLog_copyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLog_separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsLog_clear = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_mainBottom = new System.Windows.Forms.Panel();
            this.main_txtInput = new System.Windows.Forms.TextBox();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tp_Maps = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel_Maps = new System.Windows.Forms.TableLayoutPanel();
            this.panel_mapsRight = new System.Windows.Forms.Panel();
            this.panel_mapsRight_Bottom = new System.Windows.Forms.Panel();
            this.map_gbNew = new System.Windows.Forms.GroupBox();
            this.map_btnGen = new System.Windows.Forms.Button();
            this.map_lblType = new System.Windows.Forms.Label();
            this.map_lblSeed = new System.Windows.Forms.Label();
            this.map_lblZ = new System.Windows.Forms.Label();
            this.map_lblX = new System.Windows.Forms.Label();
            this.map_lblY = new System.Windows.Forms.Label();
            this.map_txtSeed = new System.Windows.Forms.TextBox();
            this.map_cmbType = new System.Windows.Forms.ComboBox();
            this.map_cmbZ = new System.Windows.Forms.ComboBox();
            this.map_cmbY = new System.Windows.Forms.ComboBox();
            this.map_cmbX = new System.Windows.Forms.ComboBox();
            this.map_lblName = new System.Windows.Forms.Label();
            this.map_txtName = new System.Windows.Forms.TextBox();
            this.panel_mapsRight_Top = new System.Windows.Forms.Panel();
            this.map_gbProps = new System.Windows.Forms.GroupBox();
            this.map_pgProps = new MCGalaxy.Gui.HackyPropertyGrid();
            this.panel_mapsLeft = new System.Windows.Forms.Panel();
            this.panel_mapsLeft_Middle = new System.Windows.Forms.Panel();
            this.panel_mapsLeft_Bottom = new System.Windows.Forms.Panel();
            this.map_btnLoad = new System.Windows.Forms.Button();
            this.map_gbUnloaded = new System.Windows.Forms.GroupBox();
            this.map_lbUnloaded = new System.Windows.Forms.ListBox();
            this.panel_mapsLeft_Top = new System.Windows.Forms.Panel();
            this.map_gbLoaded = new System.Windows.Forms.GroupBox();
            this.map_lbLoaded = new System.Windows.Forms.ListBox();
            this.tp_Players = new System.Windows.Forms.TabPage();
            this.panel_playersBottom = new System.Windows.Forms.Panel();
            this.pl_gbOther = new System.Windows.Forms.GroupBox();
            this.pl_txtSendCommand = new System.Windows.Forms.TextBox();
            this.pl_btnSendCommand = new System.Windows.Forms.Button();
            this.pl_txtMessage = new System.Windows.Forms.TextBox();
            this.pl_btnMessage = new System.Windows.Forms.Button();
            this.panel_playersRight = new System.Windows.Forms.Panel();
            this.pl_gbActions = new System.Windows.Forms.GroupBox();
            this.pl_btnKill = new System.Windows.Forms.Button();
            this.pl_statusBox = new System.Windows.Forms.TextBox();
            this.pl_numUndo = new MCGalaxy.Gui.TimespanUpDown();
            this.pl_btnWarn = new System.Windows.Forms.Button();
            this.pl_btnRules = new System.Windows.Forms.Button();
            this.pl_btnKick = new System.Windows.Forms.Button();
            this.pl_btnBanIP = new System.Windows.Forms.Button();
            this.pl_btnUndo = new System.Windows.Forms.Button();
            this.pl_btnMute = new System.Windows.Forms.Button();
            this.pl_btnBan = new System.Windows.Forms.Button();
            this.pl_btnFreeze = new System.Windows.Forms.Button();
            this.panel_playersCenter = new System.Windows.Forms.Panel();
            this.pl_gbProps = new System.Windows.Forms.GroupBox();
            this.pl_pgProps = new MCGalaxy.Gui.HackyPropertyGrid();
            this.panel_playersLeft = new System.Windows.Forms.Panel();
            this.pl_listBox = new System.Windows.Forms.ListBox();
            this.pl_lblOnline = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel_mainBtns = new System.Windows.Forms.Panel();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsMap.SuspendLayout();
            this.tsPlayer.SuspendLayout();
            this.icon_context.SuspendLayout();
            this.logs_tp.SuspendLayout();
            this.logs_tab.SuspendLayout();
            this.logs_tabErr.SuspendLayout();
            this.logs_tabGen.SuspendLayout();
            this.panel_tabGenTop.SuspendLayout();
            this.tabLog_Sys.SuspendLayout();
            this.tp_Main.SuspendLayout();
            this.panel_mainRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.main_Players)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.main_Maps)).BeginInit();
            this.panel_mainLeft.SuspendLayout();
            this.tsLog_Menu.SuspendLayout();
            this.panel_mainBottom.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tp_Maps.SuspendLayout();
            this.tableLayoutPanel_Maps.SuspendLayout();
            this.panel_mapsRight.SuspendLayout();
            this.panel_mapsRight_Bottom.SuspendLayout();
            this.map_gbNew.SuspendLayout();
            this.panel_mapsRight_Top.SuspendLayout();
            this.map_gbProps.SuspendLayout();
            this.panel_mapsLeft.SuspendLayout();
            this.panel_mapsLeft_Middle.SuspendLayout();
            this.panel_mapsLeft_Bottom.SuspendLayout();
            this.map_gbUnloaded.SuspendLayout();
            this.panel_mapsLeft_Top.SuspendLayout();
            this.map_gbLoaded.SuspendLayout();
            this.tp_Players.SuspendLayout();
            this.panel_playersBottom.SuspendLayout();
            this.pl_gbOther.SuspendLayout();
            this.panel_playersRight.SuspendLayout();
            this.pl_gbActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pl_numUndo)).BeginInit();
            this.panel_playersCenter.SuspendLayout();
            this.pl_gbProps.SuspendLayout();
            this.panel_playersLeft.SuspendLayout();
            this.panel_mainBtns.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMap
            // 
            this.tsMap.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMap_physicsMenu,
            this.tsMap_actionsMenu,
            this.tsMap_separator,
            this.tsMap_info});
            this.tsMap.Name = "mapsStrip";
            this.tsMap.Size = new System.Drawing.Size(144, 76);
            // 
            // tsMap_physicsMenu
            // 
            this.tsMap_physicsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMap_physics0,
            this.tsMap_physics1,
            this.tsMap_physics2,
            this.tsMap_physics3,
            this.tsMap_physics4,
            this.tsMap_physics5});
            this.tsMap_physicsMenu.Name = "tsMap_physicsMenu";
            this.tsMap_physicsMenu.Size = new System.Drawing.Size(143, 22);
            this.tsMap_physicsMenu.Text = "Physics Level";
            // 
            // tsMap_physics0
            // 
            this.tsMap_physics0.Name = "tsMap_physics0";
            this.tsMap_physics0.Size = new System.Drawing.Size(135, 22);
            this.tsMap_physics0.Text = "Off";
            this.tsMap_physics0.Click += new System.EventHandler(this.tsMap_Physics0_Click);
            // 
            // tsMap_physics1
            // 
            this.tsMap_physics1.Name = "tsMap_physics1";
            this.tsMap_physics1.Size = new System.Drawing.Size(135, 22);
            this.tsMap_physics1.Text = "Normal";
            this.tsMap_physics1.Click += new System.EventHandler(this.tsMap_Physics1_Click);
            // 
            // tsMap_physics2
            // 
            this.tsMap_physics2.Name = "tsMap_physics2";
            this.tsMap_physics2.Size = new System.Drawing.Size(135, 22);
            this.tsMap_physics2.Text = "Advanced";
            this.tsMap_physics2.Click += new System.EventHandler(this.tsMap_Physics2_Click);
            // 
            // tsMap_physics3
            // 
            this.tsMap_physics3.Name = "tsMap_physics3";
            this.tsMap_physics3.Size = new System.Drawing.Size(135, 22);
            this.tsMap_physics3.Text = "Hardcore";
            this.tsMap_physics3.Click += new System.EventHandler(this.tsMap_Physics3_Click);
            // 
            // tsMap_physics4
            // 
            this.tsMap_physics4.Name = "tsMap_physics4";
            this.tsMap_physics4.Size = new System.Drawing.Size(135, 22);
            this.tsMap_physics4.Text = "Instant";
            this.tsMap_physics4.Click += new System.EventHandler(this.tsMap_Physics4_Click);
            // 
            // tsMap_physics5
            // 
            this.tsMap_physics5.Name = "tsMap_physics5";
            this.tsMap_physics5.Size = new System.Drawing.Size(135, 22);
            this.tsMap_physics5.Text = "Doors-Only";
            this.tsMap_physics5.Click += new System.EventHandler(this.tsMap_Physics5_Click);
            // 
            // tsMap_actionsMenu
            // 
            this.tsMap_actionsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMap_Save,
            this.tsMap_Reload,
            this.tsMap_Unload,
            this.tsMap_moveAll});
            this.tsMap_actionsMenu.Name = "tsMap_actionsMenu";
            this.tsMap_actionsMenu.Size = new System.Drawing.Size(143, 22);
            this.tsMap_actionsMenu.Text = "Actions";
            // 
            // tsMap_Save
            // 
            this.tsMap_Save.Name = "tsMap_Save";
            this.tsMap_Save.Size = new System.Drawing.Size(121, 22);
            this.tsMap_Save.Text = "Save";
            this.tsMap_Save.Click += new System.EventHandler(this.tsMap_Save_Click);
            // 
            // tsMap_Reload
            // 
            this.tsMap_Reload.Name = "tsMap_Reload";
            this.tsMap_Reload.Size = new System.Drawing.Size(121, 22);
            this.tsMap_Reload.Text = "Reload";
            this.tsMap_Reload.Click += new System.EventHandler(this.tsMap_Reload_Click);
            // 
            // tsMap_Unload
            // 
            this.tsMap_Unload.Name = "tsMap_Unload";
            this.tsMap_Unload.Size = new System.Drawing.Size(121, 22);
            this.tsMap_Unload.Text = "Unload";
            this.tsMap_Unload.Click += new System.EventHandler(this.tsMap_Unload_Click);
            // 
            // tsMap_moveAll
            // 
            this.tsMap_moveAll.Name = "tsMap_moveAll";
            this.tsMap_moveAll.Size = new System.Drawing.Size(121, 22);
            this.tsMap_moveAll.Text = "Move All";
            this.tsMap_moveAll.Click += new System.EventHandler(this.tsMap_MoveAll_Click);
            // 
            // tsMap_separator
            // 
            this.tsMap_separator.Name = "tsMap_separator";
            this.tsMap_separator.Size = new System.Drawing.Size(140, 6);
            // 
            // tsMap_info
            // 
            this.tsMap_info.Name = "tsMap_info";
            this.tsMap_info.Size = new System.Drawing.Size(143, 22);
            this.tsMap_info.Text = "Info";
            this.tsMap_info.Click += new System.EventHandler(this.tsMap_Info_Click);
            // 
            // tsPlayer
            // 
            this.tsPlayer.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsPlayer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsPlayer_whois,
            this.tsPlayer_kick,
            this.tsPlayer_ban,
            this.tsPlayer_voice,
            this.tsPlayer_clones,
            this.tsPlayer_promote,
            this.tsPlayer_demote});
            this.tsPlayer.Name = "playerStrip";
            this.tsPlayer.Size = new System.Drawing.Size(121, 158);
            // 
            // tsPlayer_whois
            // 
            this.tsPlayer_whois.Name = "tsPlayer_whois";
            this.tsPlayer_whois.Size = new System.Drawing.Size(120, 22);
            this.tsPlayer_whois.Text = "Whois";
            this.tsPlayer_whois.Click += new System.EventHandler(this.tsPlayer_Whois_Click);
            // 
            // tsPlayer_kick
            // 
            this.tsPlayer_kick.Name = "tsPlayer_kick";
            this.tsPlayer_kick.Size = new System.Drawing.Size(120, 22);
            this.tsPlayer_kick.Text = "Kick";
            this.tsPlayer_kick.Click += new System.EventHandler(this.tsPlayer_Kick_Click);
            // 
            // tsPlayer_ban
            // 
            this.tsPlayer_ban.Name = "tsPlayer_ban";
            this.tsPlayer_ban.Size = new System.Drawing.Size(120, 22);
            this.tsPlayer_ban.Text = "Ban";
            this.tsPlayer_ban.Click += new System.EventHandler(this.tsPlayer_Ban_Click);
            // 
            // tsPlayer_voice
            // 
            this.tsPlayer_voice.Name = "tsPlayer_voice";
            this.tsPlayer_voice.Size = new System.Drawing.Size(120, 22);
            this.tsPlayer_voice.Text = "Voice";
            this.tsPlayer_voice.Click += new System.EventHandler(this.tsPlayer_Voice_Click);
            // 
            // tsPlayer_clones
            // 
            this.tsPlayer_clones.Name = "tsPlayer_clones";
            this.tsPlayer_clones.Size = new System.Drawing.Size(120, 22);
            this.tsPlayer_clones.Text = "Clones";
            this.tsPlayer_clones.Click += new System.EventHandler(this.tsPlayer_Clones_Click);
            // 
            // tsPlayer_promote
            // 
            this.tsPlayer_promote.Name = "tsPlayer_promote";
            this.tsPlayer_promote.Size = new System.Drawing.Size(120, 22);
            this.tsPlayer_promote.Text = "Promote";
            this.tsPlayer_promote.Click += new System.EventHandler(this.tsPlayer_Promote_Click);
            // 
            // tsPlayer_demote
            // 
            this.tsPlayer_demote.Name = "tsPlayer_demote";
            this.tsPlayer_demote.Size = new System.Drawing.Size(120, 22);
            this.tsPlayer_demote.Text = "Demote";
            this.tsPlayer_demote.Click += new System.EventHandler(this.tsPlayer_Demote_Click);
            // 
            // icon_context
            // 
            this.icon_context.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.icon_context.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.icon_hideWindow,
            this.icon_separator,
            this.icon_openConsole,
            this.icon_shutdown,
            this.icon_restart});
            this.icon_context.Name = "iconContext";
            this.icon_context.Size = new System.Drawing.Size(170, 98);
            // 
            // icon_hideWindow
            // 
            this.icon_hideWindow.Name = "icon_hideWindow";
            this.icon_hideWindow.Size = new System.Drawing.Size(169, 22);
            this.icon_hideWindow.Text = "Hide from taskbar";
            this.icon_hideWindow.Click += new System.EventHandler(this.icon_HideWindow_Click);
            // 
            // icon_separator
            // 
            this.icon_separator.Name = "icon_separator";
            this.icon_separator.Size = new System.Drawing.Size(166, 6);
            // 
            // icon_openConsole
            // 
            this.icon_openConsole.Name = "icon_openConsole";
            this.icon_openConsole.Size = new System.Drawing.Size(169, 22);
            this.icon_openConsole.Text = "Open console";
            this.icon_openConsole.Click += new System.EventHandler(this.icon_OpenConsole_Click);
            // 
            // icon_shutdown
            // 
            this.icon_shutdown.Name = "icon_shutdown";
            this.icon_shutdown.Size = new System.Drawing.Size(169, 22);
            this.icon_shutdown.Text = "Shutdown server";
            this.icon_shutdown.Click += new System.EventHandler(this.icon_Shutdown_Click);
            // 
            // icon_restart
            // 
            this.icon_restart.Name = "icon_restart";
            this.icon_restart.Size = new System.Drawing.Size(169, 22);
            this.icon_restart.Text = "Restart server";
            this.icon_restart.Click += new System.EventHandler(this.icon_restart_Click);
            // 
            // main_btnProps
            // 
            this.main_btnProps.Cursor = System.Windows.Forms.Cursors.Default;
            this.main_btnProps.Enabled = false;
            this.main_btnProps.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnProps.Location = new System.Drawing.Point(3, 4);
            this.main_btnProps.Name = "main_btnProps";
            this.main_btnProps.Size = new System.Drawing.Size(88, 23);
            this.main_btnProps.TabIndex = 34;
            this.main_btnProps.Text = "Settings";
            this.main_btnProps.UseVisualStyleBackColor = true;
            this.main_btnProps.Click += new System.EventHandler(this.btnProperties_Click);
            // 
            // main_btnClose
            // 
            this.main_btnClose.Cursor = System.Windows.Forms.Cursors.Default;
            this.main_btnClose.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnClose.Location = new System.Drawing.Point(179, 4);
            this.main_btnClose.Name = "main_btnClose";
            this.main_btnClose.Size = new System.Drawing.Size(88, 23);
            this.main_btnClose.TabIndex = 35;
            this.main_btnClose.Text = "Close";
            this.main_btnClose.UseVisualStyleBackColor = true;
            this.main_btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // main_btnRestart
            // 
            this.main_btnRestart.Cursor = System.Windows.Forms.Cursors.Default;
            this.main_btnRestart.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnRestart.Location = new System.Drawing.Point(91, 4);
            this.main_btnRestart.Name = "main_btnRestart";
            this.main_btnRestart.Size = new System.Drawing.Size(88, 23);
            this.main_btnRestart.TabIndex = 36;
            this.main_btnRestart.Text = "Restart";
            this.main_btnRestart.UseVisualStyleBackColor = true;
            this.main_btnRestart.Click += new System.EventHandler(this.main_BtnRestart_Click);
            // 
            // logs_tp
            // 
            this.logs_tp.BackColor = System.Drawing.SystemColors.Control;
            this.logs_tp.Controls.Add(this.logs_tab);
            this.logs_tp.Location = new System.Drawing.Point(4, 22);
            this.logs_tp.Name = "logs_tp";
            this.logs_tp.Padding = new System.Windows.Forms.Padding(3);
            this.logs_tp.Size = new System.Drawing.Size(766, 488);
            this.logs_tp.TabIndex = 4;
            this.logs_tp.Text = "Logs";
            // 
            // logs_tab
            // 
            this.logs_tab.Controls.Add(this.logs_tabErr);
            this.logs_tab.Controls.Add(this.logs_tabGen);
            this.logs_tab.Controls.Add(this.tabLog_Sys);
            this.logs_tab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logs_tab.Location = new System.Drawing.Point(3, 3);
            this.logs_tab.Name = "logs_tab";
            this.logs_tab.SelectedIndex = 0;
            this.logs_tab.Size = new System.Drawing.Size(760, 482);
            this.logs_tab.TabIndex = 0;
            // 
            // logs_tabErr
            // 
            this.logs_tabErr.Controls.Add(this.logs_txtError);
            this.logs_tabErr.Location = new System.Drawing.Point(4, 22);
            this.logs_tabErr.Name = "logs_tabErr";
            this.logs_tabErr.Size = new System.Drawing.Size(752, 456);
            this.logs_tabErr.TabIndex = 2;
            this.logs_tabErr.Text = "Errors";
            this.logs_tabErr.UseVisualStyleBackColor = true;
            // 
            // logs_txtError
            // 
            this.logs_txtError.BackColor = System.Drawing.SystemColors.Window;
            this.logs_txtError.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.logs_txtError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logs_txtError.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logs_txtError.Location = new System.Drawing.Point(0, 0);
            this.logs_txtError.Multiline = true;
            this.logs_txtError.Name = "logs_txtError";
            this.logs_txtError.ReadOnly = true;
            this.logs_txtError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logs_txtError.Size = new System.Drawing.Size(752, 456);
            this.logs_txtError.TabIndex = 2;
            // 
            // logs_tabGen
            // 
            this.logs_tabGen.Controls.Add(this.panel_tabGenTop);
            this.logs_tabGen.Controls.Add(this.logs_txtGeneral);
            this.logs_tabGen.Location = new System.Drawing.Point(4, 22);
            this.logs_tabGen.Name = "logs_tabGen";
            this.logs_tabGen.Padding = new System.Windows.Forms.Padding(3);
            this.logs_tabGen.Size = new System.Drawing.Size(752, 456);
            this.logs_tabGen.TabIndex = 0;
            this.logs_tabGen.Text = "General";
            this.logs_tabGen.UseVisualStyleBackColor = true;
            // 
            // panel_tabGenTop
            // 
            this.panel_tabGenTop.Controls.Add(this.logs_lblGeneral);
            this.panel_tabGenTop.Controls.Add(this.logs_dateGeneral);
            this.panel_tabGenTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_tabGenTop.Location = new System.Drawing.Point(3, 3);
            this.panel_tabGenTop.Name = "panel_tabGenTop";
            this.panel_tabGenTop.Size = new System.Drawing.Size(746, 24);
            this.panel_tabGenTop.TabIndex = 7;
            // 
            // logs_lblGeneral
            // 
            this.logs_lblGeneral.AutoSize = true;
            this.logs_lblGeneral.Location = new System.Drawing.Point(12, 7);
            this.logs_lblGeneral.Name = "logs_lblGeneral";
            this.logs_lblGeneral.Size = new System.Drawing.Size(78, 13);
            this.logs_lblGeneral.TabIndex = 6;
            this.logs_lblGeneral.Text = "View logs from:";
            // 
            // logs_dateGeneral
            // 
            this.logs_dateGeneral.CalendarForeColor = System.Drawing.SystemColors.WindowText;
            this.logs_dateGeneral.Location = new System.Drawing.Point(96, 2);
            this.logs_dateGeneral.Name = "logs_dateGeneral";
            this.logs_dateGeneral.Size = new System.Drawing.Size(180, 21);
            this.logs_dateGeneral.TabIndex = 5;
            this.logs_dateGeneral.Value = new System.DateTime(2011, 7, 20, 18, 31, 50, 0);
            this.logs_dateGeneral.ValueChanged += new System.EventHandler(this.logs_dateGeneral_Changed);
            // 
            // logs_txtGeneral
            // 
            this.logs_txtGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logs_txtGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.logs_txtGeneral.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logs_txtGeneral.Location = new System.Drawing.Point(0, 30);
            this.logs_txtGeneral.Name = "logs_txtGeneral";
            this.logs_txtGeneral.ReadOnly = true;
            this.logs_txtGeneral.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.logs_txtGeneral.Size = new System.Drawing.Size(753, 427);
            this.logs_txtGeneral.TabIndex = 4;
            this.logs_txtGeneral.Text = "";
            // 
            // tabLog_Sys
            // 
            this.tabLog_Sys.Controls.Add(this.logs_txtSystem);
            this.tabLog_Sys.Location = new System.Drawing.Point(4, 22);
            this.tabLog_Sys.Name = "tabLog_Sys";
            this.tabLog_Sys.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog_Sys.Size = new System.Drawing.Size(752, 456);
            this.tabLog_Sys.TabIndex = 1;
            this.tabLog_Sys.Text = "System";
            this.tabLog_Sys.UseVisualStyleBackColor = true;
            // 
            // logs_txtSystem
            // 
            this.logs_txtSystem.BackColor = System.Drawing.SystemColors.Window;
            this.logs_txtSystem.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.logs_txtSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logs_txtSystem.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logs_txtSystem.Location = new System.Drawing.Point(3, 3);
            this.logs_txtSystem.Multiline = true;
            this.logs_txtSystem.Name = "logs_txtSystem";
            this.logs_txtSystem.ReadOnly = true;
            this.logs_txtSystem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logs_txtSystem.Size = new System.Drawing.Size(746, 450);
            this.logs_txtSystem.TabIndex = 2;
            // 
            // tp_Main
            // 
            this.tp_Main.BackColor = System.Drawing.SystemColors.Control;
            this.tp_Main.Controls.Add(this.panel_mainRight);
            this.tp_Main.Controls.Add(this.panel_mainLeft);
            this.tp_Main.Controls.Add(this.panel_mainBottom);
            this.tp_Main.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tp_Main.Location = new System.Drawing.Point(4, 22);
            this.tp_Main.Name = "tp_Main";
            this.tp_Main.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Main.Size = new System.Drawing.Size(766, 488);
            this.tp_Main.TabIndex = 0;
            this.tp_Main.Text = "Main";
            // 
            // panel_mainRight
            // 
            this.panel_mainRight.BackColor = System.Drawing.Color.Transparent;
            this.panel_mainRight.Controls.Add(this.main_Players);
            this.panel_mainRight.Controls.Add(this.main_Maps);
            this.panel_mainRight.Controls.Add(this.main_btnSaveAll);
            this.panel_mainRight.Controls.Add(this.main_btnUnloadEmpty);
            this.panel_mainRight.Controls.Add(this.main_btnKillPhysics);
            this.panel_mainRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_mainRight.Location = new System.Drawing.Point(508, 3);
            this.panel_mainRight.Name = "panel_mainRight";
            this.panel_mainRight.Size = new System.Drawing.Size(255, 449);
            this.panel_mainRight.TabIndex = 44;
            // 
            // main_Players
            // 
            this.main_Players.AllowUserToAddRows = false;
            this.main_Players.AllowUserToDeleteRows = false;
            this.main_Players.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.main_Players.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.main_Players.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.main_Players.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.main_colPlName,
            this.main_colPlMap,
            this.main_colPlRank});
            this.main_Players.ContextMenuStrip = this.tsPlayer;
            this.main_Players.Location = new System.Drawing.Point(4, 5);
            this.main_Players.MultiSelect = false;
            this.main_Players.Name = "main_Players";
            this.main_Players.ReadOnly = true;
            this.main_Players.RowHeadersVisible = false;
            this.main_Players.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.main_Players.Size = new System.Drawing.Size(246, 250);
            this.main_Players.TabIndex = 37;
            this.main_Players.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.main_players_RowPrePaint);
            // 
            // main_colPlName
            // 
            this.main_colPlName.HeaderText = "Name";
            this.main_colPlName.Name = "main_colPlName";
            this.main_colPlName.ReadOnly = true;
            // 
            // main_colPlMap
            // 
            this.main_colPlMap.HeaderText = "Map";
            this.main_colPlMap.Name = "main_colPlMap";
            this.main_colPlMap.ReadOnly = true;
            // 
            // main_colPlRank
            // 
            this.main_colPlRank.HeaderText = "Rank";
            this.main_colPlRank.Name = "main_colPlRank";
            this.main_colPlRank.ReadOnly = true;
            // 
            // main_Maps
            // 
            this.main_Maps.AllowUserToAddRows = false;
            this.main_Maps.AllowUserToDeleteRows = false;
            this.main_Maps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.main_Maps.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.main_Maps.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.main_Maps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.main_Maps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.main_Maps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.main_colLvlName,
            this.main_colLvlPlayers,
            this.main_colLvlPhysics});
            this.main_Maps.ContextMenuStrip = this.tsMap;
            this.main_Maps.Location = new System.Drawing.Point(4, 290);
            this.main_Maps.MultiSelect = false;
            this.main_Maps.Name = "main_Maps";
            this.main_Maps.ReadOnly = true;
            this.main_Maps.RowHeadersVisible = false;
            this.main_Maps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.main_Maps.Size = new System.Drawing.Size(246, 156);
            this.main_Maps.TabIndex = 38;
            // 
            // main_colLvlName
            // 
            this.main_colLvlName.HeaderText = "Name";
            this.main_colLvlName.Name = "main_colLvlName";
            this.main_colLvlName.ReadOnly = true;
            // 
            // main_colLvlPlayers
            // 
            this.main_colLvlPlayers.FillWeight = 70F;
            this.main_colLvlPlayers.HeaderText = "Players";
            this.main_colLvlPlayers.Name = "main_colLvlPlayers";
            this.main_colLvlPlayers.ReadOnly = true;
            // 
            // main_colLvlPhysics
            // 
            this.main_colLvlPhysics.FillWeight = 70F;
            this.main_colLvlPhysics.HeaderText = "Physics";
            this.main_colLvlPhysics.Name = "main_colLvlPhysics";
            this.main_colLvlPhysics.ReadOnly = true;
            // 
            // main_btnSaveAll
            // 
            this.main_btnSaveAll.Cursor = System.Windows.Forms.Cursors.Default;
            this.main_btnSaveAll.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnSaveAll.Location = new System.Drawing.Point(5, 261);
            this.main_btnSaveAll.Name = "main_btnSaveAll";
            this.main_btnSaveAll.Size = new System.Drawing.Size(63, 23);
            this.main_btnSaveAll.TabIndex = 39;
            this.main_btnSaveAll.Text = "Save All";
            this.main_btnSaveAll.UseVisualStyleBackColor = true;
            this.main_btnSaveAll.Click += new System.EventHandler(this.main_BtnSaveAll_Click);
            // 
            // main_btnUnloadEmpty
            // 
            this.main_btnUnloadEmpty.Cursor = System.Windows.Forms.Cursors.Default;
            this.main_btnUnloadEmpty.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnUnloadEmpty.Location = new System.Drawing.Point(168, 261);
            this.main_btnUnloadEmpty.Name = "main_btnUnloadEmpty";
            this.main_btnUnloadEmpty.Size = new System.Drawing.Size(81, 23);
            this.main_btnUnloadEmpty.TabIndex = 41;
            this.main_btnUnloadEmpty.Text = "Unload Empty";
            this.main_btnUnloadEmpty.UseVisualStyleBackColor = true;
            this.main_btnUnloadEmpty.Click += new System.EventHandler(this.main_BtnUnloadEmpty_Click);
            // 
            // main_btnKillPhysics
            // 
            this.main_btnKillPhysics.Cursor = System.Windows.Forms.Cursors.Default;
            this.main_btnKillPhysics.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_btnKillPhysics.Location = new System.Drawing.Point(74, 261);
            this.main_btnKillPhysics.Name = "main_btnKillPhysics";
            this.main_btnKillPhysics.Size = new System.Drawing.Size(88, 23);
            this.main_btnKillPhysics.TabIndex = 40;
            this.main_btnKillPhysics.Text = "Kill All Physics";
            this.main_btnKillPhysics.UseVisualStyleBackColor = true;
            this.main_btnKillPhysics.Click += new System.EventHandler(this.main_BtnKillPhysics_Click);
            // 
            // panel_mainLeft
            // 
            this.panel_mainLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mainLeft.BackColor = System.Drawing.Color.Transparent;
            this.panel_mainLeft.Controls.Add(this.main_txtUrl);
            this.panel_mainLeft.Controls.Add(this.main_txtLog);
            this.panel_mainLeft.Location = new System.Drawing.Point(4, 5);
            this.panel_mainLeft.Name = "panel_mainLeft";
            this.panel_mainLeft.Size = new System.Drawing.Size(503, 447);
            this.panel_mainLeft.TabIndex = 43;
            // 
            // main_txtUrl
            // 
            this.main_txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_txtUrl.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.main_txtUrl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_txtUrl.Location = new System.Drawing.Point(2, 9);
            this.main_txtUrl.Name = "main_txtUrl";
            this.main_txtUrl.ReadOnly = true;
            this.main_txtUrl.Size = new System.Drawing.Size(498, 21);
            this.main_txtUrl.TabIndex = 25;
            this.main_txtUrl.Text = "Starting server..";
            this.main_txtUrl.DoubleClick += new System.EventHandler(this.main_TxtUrl_DoubleClick);
            // 
            // main_txtLog
            // 
            this.main_txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_txtLog.AutoScroll = true;
            this.main_txtLog.BackColor = System.Drawing.Color.White;
            this.main_txtLog.Colorize = true;
            this.main_txtLog.ContextMenuStrip = this.tsLog_Menu;
            this.main_txtLog.DateStamp = true;
            this.main_txtLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_txtLog.ForeColor = System.Drawing.Color.Black;
            this.main_txtLog.Location = new System.Drawing.Point(2, 36);
            this.main_txtLog.Name = "main_txtLog";
            this.main_txtLog.NightMode = false;
            this.main_txtLog.ReadOnly = true;
            this.main_txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.main_txtLog.Size = new System.Drawing.Size(498, 408);
            this.main_txtLog.TabIndex = 0;
            this.main_txtLog.Text = "";
            // 
            // tsLog_Menu
            // 
            this.tsLog_Menu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsLog_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLog_night,
            this.tsLog_Colored,
            this.tsLog_dateStamp,
            this.tsLog_autoScroll,
            this.tsLog_separator1,
            this.tsLog_copySelected,
            this.tsLog_copyAll,
            this.tsLog_separator2,
            this.tsLog_clear});
            this.tsLog_Menu.Name = "txtLogMenuStrip";
            this.tsLog_Menu.Size = new System.Drawing.Size(150, 170);
            // 
            // tsLog_night
            // 
            this.tsLog_night.Name = "tsLog_night";
            this.tsLog_night.Size = new System.Drawing.Size(149, 22);
            this.tsLog_night.Text = "Night Theme";
            this.tsLog_night.Click += new System.EventHandler(this.tsLog_Night_Click);
            // 
            // tsLog_Colored
            // 
            this.tsLog_Colored.Checked = true;
            this.tsLog_Colored.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsLog_Colored.Name = "tsLog_Colored";
            this.tsLog_Colored.Size = new System.Drawing.Size(149, 22);
            this.tsLog_Colored.Text = "Colors";
            this.tsLog_Colored.Click += new System.EventHandler(this.tsLog_Colored_Click);
            // 
            // tsLog_dateStamp
            // 
            this.tsLog_dateStamp.Checked = true;
            this.tsLog_dateStamp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsLog_dateStamp.Name = "tsLog_dateStamp";
            this.tsLog_dateStamp.Size = new System.Drawing.Size(149, 22);
            this.tsLog_dateStamp.Text = "Date Stamp";
            this.tsLog_dateStamp.Click += new System.EventHandler(this.tsLog_DateStamp_Click);
            // 
            // tsLog_autoScroll
            // 
            this.tsLog_autoScroll.Checked = true;
            this.tsLog_autoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsLog_autoScroll.Name = "tsLog_autoScroll";
            this.tsLog_autoScroll.Size = new System.Drawing.Size(149, 22);
            this.tsLog_autoScroll.Text = "Auto Scroll";
            this.tsLog_autoScroll.Click += new System.EventHandler(this.tsLog_AutoScroll_Click);
            // 
            // tsLog_separator1
            // 
            this.tsLog_separator1.Name = "tsLog_separator1";
            this.tsLog_separator1.Size = new System.Drawing.Size(146, 6);
            // 
            // tsLog_copySelected
            // 
            this.tsLog_copySelected.Name = "tsLog_copySelected";
            this.tsLog_copySelected.Size = new System.Drawing.Size(149, 22);
            this.tsLog_copySelected.Text = "Copy Selected";
            this.tsLog_copySelected.Click += new System.EventHandler(this.tsLog_CopySelected_Click);
            // 
            // tsLog_copyAll
            // 
            this.tsLog_copyAll.Name = "tsLog_copyAll";
            this.tsLog_copyAll.Size = new System.Drawing.Size(149, 22);
            this.tsLog_copyAll.Text = "Copy All";
            this.tsLog_copyAll.Click += new System.EventHandler(this.tsLog_CopyAll_Click);
            // 
            // tsLog_separator2
            // 
            this.tsLog_separator2.Name = "tsLog_separator2";
            this.tsLog_separator2.Size = new System.Drawing.Size(146, 6);
            // 
            // tsLog_clear
            // 
            this.tsLog_clear.Name = "tsLog_clear";
            this.tsLog_clear.Size = new System.Drawing.Size(149, 22);
            this.tsLog_clear.Text = "Clear";
            this.tsLog_clear.Click += new System.EventHandler(this.tsLog_Clear_Click);
            // 
            // panel_mainBottom
            // 
            this.panel_mainBottom.BackColor = System.Drawing.Color.Transparent;
            this.panel_mainBottom.Controls.Add(this.main_txtInput);
            this.panel_mainBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_mainBottom.Location = new System.Drawing.Point(3, 452);
            this.panel_mainBottom.Name = "panel_mainBottom";
            this.panel_mainBottom.Size = new System.Drawing.Size(760, 33);
            this.panel_mainBottom.TabIndex = 42;
            // 
            // main_txtInput
            // 
            this.main_txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.main_txtInput.BackColor = System.Drawing.SystemColors.Window;
            this.main_txtInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_txtInput.Location = new System.Drawing.Point(5, 6);
            this.main_txtInput.Name = "main_txtInput";
            this.main_txtInput.Size = new System.Drawing.Size(749, 21);
            this.main_txtInput.TabIndex = 27;
            this.toolTip.SetToolTip(this.main_txtInput, "To send chat to players, just type the message in.\nTo enter a command, put a / be" +
        "fore it. (e.g. /help commands)");
            this.main_txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.main_TxtInput_KeyDown);
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tp_Main);
            this.tabs.Controls.Add(this.logs_tp);
            this.tabs.Controls.Add(this.tp_Maps);
            this.tabs.Controls.Add(this.tp_Players);
            this.tabs.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabs.Font = new System.Drawing.Font("Calibri", 8.25F);
            this.tabs.Location = new System.Drawing.Point(1, 11);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(774, 514);
            this.tabs.TabIndex = 2;
            this.tabs.Click += new System.EventHandler(this.tabs_Click);
            // 
            // tp_Maps
            // 
            this.tp_Maps.BackColor = System.Drawing.SystemColors.Control;
            this.tp_Maps.Controls.Add(this.tableLayoutPanel_Maps);
            this.tp_Maps.Location = new System.Drawing.Point(4, 22);
            this.tp_Maps.Name = "tp_Maps";
            this.tp_Maps.Size = new System.Drawing.Size(766, 488);
            this.tp_Maps.TabIndex = 9;
            this.tp_Maps.Text = "Maps";
            // 
            // tableLayoutPanel_Maps
            // 
            this.tableLayoutPanel_Maps.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel_Maps.ColumnCount = 2;
            this.tableLayoutPanel_Maps.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Maps.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Maps.Controls.Add(this.panel_mapsRight, 1, 0);
            this.tableLayoutPanel_Maps.Controls.Add(this.panel_mapsLeft, 0, 0);
            this.tableLayoutPanel_Maps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Maps.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel_Maps.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Maps.Name = "tableLayoutPanel_Maps";
            this.tableLayoutPanel_Maps.RowCount = 1;
            this.tableLayoutPanel_Maps.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Maps.Size = new System.Drawing.Size(766, 488);
            this.tableLayoutPanel_Maps.TabIndex = 1;
            // 
            // panel_mapsRight
            // 
            this.panel_mapsRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mapsRight.Controls.Add(this.panel_mapsRight_Bottom);
            this.panel_mapsRight.Controls.Add(this.panel_mapsRight_Top);
            this.panel_mapsRight.Location = new System.Drawing.Point(386, 3);
            this.panel_mapsRight.Name = "panel_mapsRight";
            this.panel_mapsRight.Size = new System.Drawing.Size(377, 482);
            this.panel_mapsRight.TabIndex = 7;
            // 
            // panel_mapsRight_Bottom
            // 
            this.panel_mapsRight_Bottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mapsRight_Bottom.BackColor = System.Drawing.Color.Transparent;
            this.panel_mapsRight_Bottom.Controls.Add(this.map_gbNew);
            this.panel_mapsRight_Bottom.Location = new System.Drawing.Point(0, 345);
            this.panel_mapsRight_Bottom.Name = "panel_mapsRight_Bottom";
            this.panel_mapsRight_Bottom.Size = new System.Drawing.Size(376, 135);
            this.panel_mapsRight_Bottom.TabIndex = 7;
            // 
            // map_gbNew
            // 
            this.map_gbNew.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.map_gbNew.Controls.Add(this.map_btnGen);
            this.map_gbNew.Controls.Add(this.map_lblType);
            this.map_gbNew.Controls.Add(this.map_lblSeed);
            this.map_gbNew.Controls.Add(this.map_lblZ);
            this.map_gbNew.Controls.Add(this.map_lblX);
            this.map_gbNew.Controls.Add(this.map_lblY);
            this.map_gbNew.Controls.Add(this.map_txtSeed);
            this.map_gbNew.Controls.Add(this.map_cmbType);
            this.map_gbNew.Controls.Add(this.map_cmbZ);
            this.map_gbNew.Controls.Add(this.map_cmbY);
            this.map_gbNew.Controls.Add(this.map_cmbX);
            this.map_gbNew.Controls.Add(this.map_lblName);
            this.map_gbNew.Controls.Add(this.map_txtName);
            this.map_gbNew.Location = new System.Drawing.Point(5, 3);
            this.map_gbNew.Name = "map_gbNew";
            this.map_gbNew.Size = new System.Drawing.Size(365, 129);
            this.map_gbNew.TabIndex = 0;
            this.map_gbNew.TabStop = false;
            this.map_gbNew.Text = "Create new map";
            // 
            // map_btnGen
            // 
            this.map_btnGen.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_btnGen.Location = new System.Drawing.Point(145, 99);
            this.map_btnGen.Name = "map_btnGen";
            this.map_btnGen.Size = new System.Drawing.Size(75, 23);
            this.map_btnGen.TabIndex = 17;
            this.map_btnGen.Text = "Generate";
            this.map_btnGen.UseVisualStyleBackColor = true;
            this.map_btnGen.Click += new System.EventHandler(this.map_BtnGen_Click);
            // 
            // map_lblType
            // 
            this.map_lblType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_lblType.AutoSize = true;
            this.map_lblType.Location = new System.Drawing.Point(22, 78);
            this.map_lblType.Name = "map_lblType";
            this.map_lblType.Size = new System.Drawing.Size(32, 13);
            this.map_lblType.TabIndex = 16;
            this.map_lblType.Text = "Type:";
            // 
            // map_lblSeed
            // 
            this.map_lblSeed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_lblSeed.AutoSize = true;
            this.map_lblSeed.Location = new System.Drawing.Point(201, 78);
            this.map_lblSeed.Name = "map_lblSeed";
            this.map_lblSeed.Size = new System.Drawing.Size(33, 13);
            this.map_lblSeed.TabIndex = 15;
            this.map_lblSeed.Text = "Seed:";
            // 
            // map_lblZ
            // 
            this.map_lblZ.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_lblZ.AutoSize = true;
            this.map_lblZ.Location = new System.Drawing.Point(240, 51);
            this.map_lblZ.Name = "map_lblZ";
            this.map_lblZ.Size = new System.Drawing.Size(42, 13);
            this.map_lblZ.TabIndex = 14;
            this.map_lblZ.Text = "Length:";
            // 
            // map_lblX
            // 
            this.map_lblX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_lblX.AutoSize = true;
            this.map_lblX.Location = new System.Drawing.Point(16, 51);
            this.map_lblX.Name = "map_lblX";
            this.map_lblX.Size = new System.Drawing.Size(39, 13);
            this.map_lblX.TabIndex = 13;
            this.map_lblX.Text = "Width:";
            // 
            // map_lblY
            // 
            this.map_lblY.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_lblY.AutoSize = true;
            this.map_lblY.Location = new System.Drawing.Point(127, 51);
            this.map_lblY.Name = "map_lblY";
            this.map_lblY.Size = new System.Drawing.Size(41, 13);
            this.map_lblY.TabIndex = 12;
            this.map_lblY.Text = "Height:";
            // 
            // map_txtSeed
            // 
            this.map_txtSeed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_txtSeed.BackColor = System.Drawing.SystemColors.Window;
            this.map_txtSeed.Location = new System.Drawing.Point(240, 75);
            this.map_txtSeed.Name = "map_txtSeed";
            this.map_txtSeed.Size = new System.Drawing.Size(108, 21);
            this.map_txtSeed.TabIndex = 11;
            // 
            // map_cmbType
            // 
            this.map_cmbType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_cmbType.BackColor = System.Drawing.SystemColors.Window;
            this.map_cmbType.FormattingEnabled = true;
            this.map_cmbType.Location = new System.Drawing.Point(61, 75);
            this.map_cmbType.Name = "map_cmbType";
            this.map_cmbType.Size = new System.Drawing.Size(120, 21);
            this.map_cmbType.TabIndex = 10;
            // 
            // map_cmbZ
            // 
            this.map_cmbZ.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_cmbZ.BackColor = System.Drawing.SystemColors.Window;
            this.map_cmbZ.FormattingEnabled = true;
            this.map_cmbZ.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.map_cmbZ.Location = new System.Drawing.Point(288, 48);
            this.map_cmbZ.Name = "map_cmbZ";
            this.map_cmbZ.Size = new System.Drawing.Size(60, 21);
            this.map_cmbZ.TabIndex = 9;
            // 
            // map_cmbY
            // 
            this.map_cmbY.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_cmbY.BackColor = System.Drawing.SystemColors.Window;
            this.map_cmbY.FormattingEnabled = true;
            this.map_cmbY.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.map_cmbY.Location = new System.Drawing.Point(174, 48);
            this.map_cmbY.Name = "map_cmbY";
            this.map_cmbY.Size = new System.Drawing.Size(60, 21);
            this.map_cmbY.TabIndex = 8;
            // 
            // map_cmbX
            // 
            this.map_cmbX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_cmbX.BackColor = System.Drawing.SystemColors.Window;
            this.map_cmbX.FormattingEnabled = true;
            this.map_cmbX.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.map_cmbX.Location = new System.Drawing.Point(61, 48);
            this.map_cmbX.Name = "map_cmbX";
            this.map_cmbX.Size = new System.Drawing.Size(60, 21);
            this.map_cmbX.TabIndex = 7;
            // 
            // map_lblName
            // 
            this.map_lblName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_lblName.AutoSize = true;
            this.map_lblName.Location = new System.Drawing.Point(16, 24);
            this.map_lblName.Name = "map_lblName";
            this.map_lblName.Size = new System.Drawing.Size(38, 13);
            this.map_lblName.TabIndex = 6;
            this.map_lblName.Text = "Name:";
            // 
            // map_txtName
            // 
            this.map_txtName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_txtName.BackColor = System.Drawing.SystemColors.Window;
            this.map_txtName.Location = new System.Drawing.Point(60, 21);
            this.map_txtName.Name = "map_txtName";
            this.map_txtName.Size = new System.Drawing.Size(288, 21);
            this.map_txtName.TabIndex = 0;
            // 
            // panel_mapsRight_Top
            // 
            this.panel_mapsRight_Top.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mapsRight_Top.BackColor = System.Drawing.Color.Transparent;
            this.panel_mapsRight_Top.Controls.Add(this.map_gbProps);
            this.panel_mapsRight_Top.Location = new System.Drawing.Point(0, 3);
            this.panel_mapsRight_Top.Name = "panel_mapsRight_Top";
            this.panel_mapsRight_Top.Size = new System.Drawing.Size(376, 339);
            this.panel_mapsRight_Top.TabIndex = 6;
            // 
            // map_gbProps
            // 
            this.map_gbProps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.map_gbProps.Controls.Add(this.map_pgProps);
            this.map_gbProps.Location = new System.Drawing.Point(5, 3);
            this.map_gbProps.Name = "map_gbProps";
            this.map_gbProps.Size = new System.Drawing.Size(365, 333);
            this.map_gbProps.TabIndex = 5;
            this.map_gbProps.TabStop = false;
            this.map_gbProps.Text = "Properties for (none selected)";
            // 
            // map_pgProps
            // 
            this.map_pgProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map_pgProps.Location = new System.Drawing.Point(3, 17);
            this.map_pgProps.Name = "map_pgProps";
            this.map_pgProps.Size = new System.Drawing.Size(359, 313);
            this.map_pgProps.TabIndex = 0;
            this.map_pgProps.ToolbarVisible = false;
            // 
            // panel_mapsLeft
            // 
            this.panel_mapsLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mapsLeft.BackColor = System.Drawing.Color.Transparent;
            this.panel_mapsLeft.Controls.Add(this.panel_mapsLeft_Middle);
            this.panel_mapsLeft.Controls.Add(this.panel_mapsLeft_Top);
            this.panel_mapsLeft.Location = new System.Drawing.Point(3, 3);
            this.panel_mapsLeft.Name = "panel_mapsLeft";
            this.panel_mapsLeft.Size = new System.Drawing.Size(377, 482);
            this.panel_mapsLeft.TabIndex = 6;
            // 
            // panel_mapsLeft_Middle
            // 
            this.panel_mapsLeft_Middle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mapsLeft_Middle.BackColor = System.Drawing.Color.Transparent;
            this.panel_mapsLeft_Middle.Controls.Add(this.panel_mapsLeft_Bottom);
            this.panel_mapsLeft_Middle.Controls.Add(this.map_gbUnloaded);
            this.panel_mapsLeft_Middle.Location = new System.Drawing.Point(2, 229);
            this.panel_mapsLeft_Middle.Name = "panel_mapsLeft_Middle";
            this.panel_mapsLeft_Middle.Size = new System.Drawing.Size(375, 264);
            this.panel_mapsLeft_Middle.TabIndex = 3;
            // 
            // panel_mapsLeft_Bottom
            // 
            this.panel_mapsLeft_Bottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mapsLeft_Bottom.Controls.Add(this.map_btnLoad);
            this.panel_mapsLeft_Bottom.Location = new System.Drawing.Point(0, 213);
            this.panel_mapsLeft_Bottom.Name = "panel_mapsLeft_Bottom";
            this.panel_mapsLeft_Bottom.Size = new System.Drawing.Size(375, 40);
            this.panel_mapsLeft_Bottom.TabIndex = 2;
            // 
            // map_btnLoad
            // 
            this.map_btnLoad.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.map_btnLoad.Location = new System.Drawing.Point(150, 5);
            this.map_btnLoad.Name = "map_btnLoad";
            this.map_btnLoad.Size = new System.Drawing.Size(75, 23);
            this.map_btnLoad.TabIndex = 1;
            this.map_btnLoad.Text = "Load map";
            this.map_btnLoad.UseVisualStyleBackColor = true;
            this.map_btnLoad.Click += new System.EventHandler(this.map_BtnLoad_Click);
            // 
            // map_gbUnloaded
            // 
            this.map_gbUnloaded.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.map_gbUnloaded.Controls.Add(this.map_lbUnloaded);
            this.map_gbUnloaded.Location = new System.Drawing.Point(6, 3);
            this.map_gbUnloaded.Name = "map_gbUnloaded";
            this.map_gbUnloaded.Size = new System.Drawing.Size(364, 207);
            this.map_gbUnloaded.TabIndex = 3;
            this.map_gbUnloaded.TabStop = false;
            this.map_gbUnloaded.Text = "Unloaded levels";
            // 
            // map_lbUnloaded
            // 
            this.map_lbUnloaded.BackColor = System.Drawing.SystemColors.Window;
            this.map_lbUnloaded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map_lbUnloaded.ForeColor = System.Drawing.SystemColors.WindowText;
            this.map_lbUnloaded.FormattingEnabled = true;
            this.map_lbUnloaded.Location = new System.Drawing.Point(3, 17);
            this.map_lbUnloaded.MultiColumn = true;
            this.map_lbUnloaded.Name = "map_lbUnloaded";
            this.map_lbUnloaded.Size = new System.Drawing.Size(358, 187);
            this.map_lbUnloaded.TabIndex = 0;
            // 
            // panel_mapsLeft_Top
            // 
            this.panel_mapsLeft_Top.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mapsLeft_Top.Controls.Add(this.map_gbLoaded);
            this.panel_mapsLeft_Top.Location = new System.Drawing.Point(2, 2);
            this.panel_mapsLeft_Top.Name = "panel_mapsLeft_Top";
            this.panel_mapsLeft_Top.Size = new System.Drawing.Size(375, 226);
            this.panel_mapsLeft_Top.TabIndex = 1;
            // 
            // map_gbLoaded
            // 
            this.map_gbLoaded.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.map_gbLoaded.Controls.Add(this.map_lbLoaded);
            this.map_gbLoaded.Location = new System.Drawing.Point(4, 4);
            this.map_gbLoaded.Name = "map_gbLoaded";
            this.map_gbLoaded.Size = new System.Drawing.Size(366, 219);
            this.map_gbLoaded.TabIndex = 4;
            this.map_gbLoaded.TabStop = false;
            this.map_gbLoaded.Text = "Loaded levels";
            // 
            // map_lbLoaded
            // 
            this.map_lbLoaded.BackColor = System.Drawing.SystemColors.Window;
            this.map_lbLoaded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map_lbLoaded.ForeColor = System.Drawing.SystemColors.WindowText;
            this.map_lbLoaded.FormattingEnabled = true;
            this.map_lbLoaded.Location = new System.Drawing.Point(3, 17);
            this.map_lbLoaded.MultiColumn = true;
            this.map_lbLoaded.Name = "map_lbLoaded";
            this.map_lbLoaded.Size = new System.Drawing.Size(360, 199);
            this.map_lbLoaded.TabIndex = 0;
            this.map_lbLoaded.SelectedIndexChanged += new System.EventHandler(this.Map_UpdateSelected);
            // 
            // tp_Players
            // 
            this.tp_Players.Controls.Add(this.panel_playersBottom);
            this.tp_Players.Controls.Add(this.panel_playersRight);
            this.tp_Players.Controls.Add(this.panel_playersCenter);
            this.tp_Players.Controls.Add(this.panel_playersLeft);
            this.tp_Players.Location = new System.Drawing.Point(4, 22);
            this.tp_Players.Name = "tp_Players";
            this.tp_Players.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Players.Size = new System.Drawing.Size(766, 488);
            this.tp_Players.TabIndex = 7;
            this.tp_Players.Text = "Players";
            // 
            // panel_playersBottom
            // 
            this.panel_playersBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_playersBottom.BackColor = System.Drawing.Color.Transparent;
            this.panel_playersBottom.Controls.Add(this.pl_gbOther);
            this.panel_playersBottom.Location = new System.Drawing.Point(138, 395);
            this.panel_playersBottom.Name = "panel_playersBottom";
            this.panel_playersBottom.Size = new System.Drawing.Size(621, 90);
            this.panel_playersBottom.TabIndex = 72;
            // 
            // pl_gbOther
            // 
            this.pl_gbOther.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pl_gbOther.Controls.Add(this.pl_txtSendCommand);
            this.pl_gbOther.Controls.Add(this.pl_btnSendCommand);
            this.pl_gbOther.Controls.Add(this.pl_txtMessage);
            this.pl_gbOther.Controls.Add(this.pl_btnMessage);
            this.pl_gbOther.Location = new System.Drawing.Point(4, 1);
            this.pl_gbOther.Name = "pl_gbOther";
            this.pl_gbOther.Size = new System.Drawing.Size(611, 78);
            this.pl_gbOther.TabIndex = 66;
            this.pl_gbOther.TabStop = false;
            this.pl_gbOther.Text = "Other";
            // 
            // pl_txtSendCommand
            // 
            this.pl_txtSendCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pl_txtSendCommand.BackColor = System.Drawing.SystemColors.Window;
            this.pl_txtSendCommand.Location = new System.Drawing.Point(115, 50);
            this.pl_txtSendCommand.Name = "pl_txtSendCommand";
            this.pl_txtSendCommand.Size = new System.Drawing.Size(483, 21);
            this.pl_txtSendCommand.TabIndex = 38;
            this.pl_txtSendCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pl_txtSendCommand_KeyDown);
            // 
            // pl_btnSendCommand
            // 
            this.pl_btnSendCommand.Location = new System.Drawing.Point(6, 48);
            this.pl_btnSendCommand.Name = "pl_btnSendCommand";
            this.pl_btnSendCommand.Size = new System.Drawing.Size(98, 23);
            this.pl_btnSendCommand.TabIndex = 37;
            this.pl_btnSendCommand.Text = "Do command:";
            this.pl_btnSendCommand.UseVisualStyleBackColor = true;
            this.pl_btnSendCommand.Click += new System.EventHandler(this.pl_BtnSendCommand_Click);
            // 
            // pl_txtMessage
            // 
            this.pl_txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pl_txtMessage.BackColor = System.Drawing.SystemColors.Window;
            this.pl_txtMessage.Location = new System.Drawing.Point(115, 18);
            this.pl_txtMessage.Name = "pl_txtMessage";
            this.pl_txtMessage.Size = new System.Drawing.Size(483, 21);
            this.pl_txtMessage.TabIndex = 8;
            this.pl_txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pl_txtMessage_KeyDown);
            // 
            // pl_btnMessage
            // 
            this.pl_btnMessage.Location = new System.Drawing.Point(6, 16);
            this.pl_btnMessage.Name = "pl_btnMessage";
            this.pl_btnMessage.Size = new System.Drawing.Size(98, 23);
            this.pl_btnMessage.TabIndex = 9;
            this.pl_btnMessage.Text = "Send message:";
            this.pl_btnMessage.UseVisualStyleBackColor = true;
            this.pl_btnMessage.Click += new System.EventHandler(this.pl_BtnMessage_Click);
            // 
            // panel_playersRight
            // 
            this.panel_playersRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_playersRight.BackColor = System.Drawing.Color.Transparent;
            this.panel_playersRight.Controls.Add(this.pl_gbActions);
            this.panel_playersRight.Location = new System.Drawing.Point(521, 4);
            this.panel_playersRight.Name = "panel_playersRight";
            this.panel_playersRight.Size = new System.Drawing.Size(238, 390);
            this.panel_playersRight.TabIndex = 71;
            // 
            // pl_gbActions
            // 
            this.pl_gbActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pl_gbActions.Controls.Add(this.pl_btnKill);
            this.pl_gbActions.Controls.Add(this.pl_statusBox);
            this.pl_gbActions.Controls.Add(this.pl_numUndo);
            this.pl_gbActions.Controls.Add(this.pl_btnWarn);
            this.pl_gbActions.Controls.Add(this.pl_btnRules);
            this.pl_gbActions.Controls.Add(this.pl_btnKick);
            this.pl_gbActions.Controls.Add(this.pl_btnBanIP);
            this.pl_gbActions.Controls.Add(this.pl_btnUndo);
            this.pl_gbActions.Controls.Add(this.pl_btnMute);
            this.pl_gbActions.Controls.Add(this.pl_btnBan);
            this.pl_gbActions.Controls.Add(this.pl_btnFreeze);
            this.pl_gbActions.Location = new System.Drawing.Point(5, 5);
            this.pl_gbActions.Name = "pl_gbActions";
            this.pl_gbActions.Size = new System.Drawing.Size(228, 379);
            this.pl_gbActions.TabIndex = 65;
            this.pl_gbActions.TabStop = false;
            this.pl_gbActions.Text = "Actions";
            // 
            // pl_btnKill
            // 
            this.pl_btnKill.Location = new System.Drawing.Point(8, 105);
            this.pl_btnKill.Name = "pl_btnKill";
            this.pl_btnKill.Size = new System.Drawing.Size(98, 23);
            this.pl_btnKill.TabIndex = 43;
            this.pl_btnKill.Text = "Kill";
            this.pl_btnKill.UseVisualStyleBackColor = true;
            this.pl_btnKill.Click += new System.EventHandler(this.pl_BtnKill_Click);
            // 
            // pl_statusBox
            // 
            this.pl_statusBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pl_statusBox.BackColor = System.Drawing.SystemColors.Window;
            this.pl_statusBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.pl_statusBox.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pl_statusBox.Location = new System.Drawing.Point(6, 177);
            this.pl_statusBox.Multiline = true;
            this.pl_statusBox.Name = "pl_statusBox";
            this.pl_statusBox.ReadOnly = true;
            this.pl_statusBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.pl_statusBox.Size = new System.Drawing.Size(216, 196);
            this.pl_statusBox.TabIndex = 63;
            // 
            // pl_numUndo
            // 
            this.pl_numUndo.BackColor = System.Drawing.SystemColors.Window;
            this.pl_numUndo.Location = new System.Drawing.Point(122, 149);
            this.pl_numUndo.Name = "pl_numUndo";
            this.pl_numUndo.Seconds = ((long)(1800));
            this.pl_numUndo.Size = new System.Drawing.Size(51, 21);
            this.pl_numUndo.TabIndex = 42;
            this.pl_numUndo.Text = "30m";
            this.pl_numUndo.Value = System.TimeSpan.Parse("00:30:00");
            this.pl_numUndo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pl_numUndo_KeyDown);
            // 
            // pl_btnWarn
            // 
            this.pl_btnWarn.Location = new System.Drawing.Point(8, 18);
            this.pl_btnWarn.Name = "pl_btnWarn";
            this.pl_btnWarn.Size = new System.Drawing.Size(98, 23);
            this.pl_btnWarn.TabIndex = 10;
            this.pl_btnWarn.Text = "Warn";
            this.pl_btnWarn.UseVisualStyleBackColor = true;
            this.pl_btnWarn.Click += new System.EventHandler(this.pl_BtnWarn_Click);
            // 
            // pl_btnRules
            // 
            this.pl_btnRules.Location = new System.Drawing.Point(122, 105);
            this.pl_btnRules.Name = "pl_btnRules";
            this.pl_btnRules.Size = new System.Drawing.Size(98, 23);
            this.pl_btnRules.TabIndex = 39;
            this.pl_btnRules.Text = "Send Rules";
            this.pl_btnRules.UseVisualStyleBackColor = true;
            this.pl_btnRules.Click += new System.EventHandler(this.pl_BtnRules_Click);
            // 
            // pl_btnKick
            // 
            this.pl_btnKick.Location = new System.Drawing.Point(122, 18);
            this.pl_btnKick.Name = "pl_btnKick";
            this.pl_btnKick.Size = new System.Drawing.Size(98, 23);
            this.pl_btnKick.TabIndex = 4;
            this.pl_btnKick.Text = "Kick";
            this.pl_btnKick.UseVisualStyleBackColor = true;
            this.pl_btnKick.Click += new System.EventHandler(this.pl_BtnKick_Click);
            // 
            // pl_btnBanIP
            // 
            this.pl_btnBanIP.Location = new System.Drawing.Point(122, 47);
            this.pl_btnBanIP.Name = "pl_btnBanIP";
            this.pl_btnBanIP.Size = new System.Drawing.Size(98, 23);
            this.pl_btnBanIP.TabIndex = 6;
            this.pl_btnBanIP.Text = "IP Ban";
            this.pl_btnBanIP.UseVisualStyleBackColor = true;
            this.pl_btnBanIP.Click += new System.EventHandler(this.pl_BtnIPBan_Click);
            // 
            // pl_btnUndo
            // 
            this.pl_btnUndo.Location = new System.Drawing.Point(8, 148);
            this.pl_btnUndo.Name = "pl_btnUndo";
            this.pl_btnUndo.Size = new System.Drawing.Size(98, 23);
            this.pl_btnUndo.TabIndex = 41;
            this.pl_btnUndo.Text = "Undo:";
            this.pl_btnUndo.UseVisualStyleBackColor = true;
            this.pl_btnUndo.Click += new System.EventHandler(this.pl_BtnUndo_Click);
            // 
            // pl_btnMute
            // 
            this.pl_btnMute.Location = new System.Drawing.Point(8, 76);
            this.pl_btnMute.Name = "pl_btnMute";
            this.pl_btnMute.Size = new System.Drawing.Size(98, 23);
            this.pl_btnMute.TabIndex = 40;
            this.pl_btnMute.Text = "Mute";
            this.pl_btnMute.UseVisualStyleBackColor = true;
            this.pl_btnMute.Click += new System.EventHandler(this.pl_BtnMute_Click);
            // 
            // pl_btnBan
            // 
            this.pl_btnBan.Location = new System.Drawing.Point(8, 47);
            this.pl_btnBan.Name = "pl_btnBan";
            this.pl_btnBan.Size = new System.Drawing.Size(98, 23);
            this.pl_btnBan.TabIndex = 5;
            this.pl_btnBan.Text = "Ban";
            this.pl_btnBan.UseVisualStyleBackColor = true;
            this.pl_btnBan.Click += new System.EventHandler(this.pl_BtnBan_Click);
            // 
            // pl_btnFreeze
            // 
            this.pl_btnFreeze.Location = new System.Drawing.Point(122, 76);
            this.pl_btnFreeze.Name = "pl_btnFreeze";
            this.pl_btnFreeze.Size = new System.Drawing.Size(98, 23);
            this.pl_btnFreeze.TabIndex = 36;
            this.pl_btnFreeze.Text = "Freeze";
            this.pl_btnFreeze.UseVisualStyleBackColor = true;
            this.pl_btnFreeze.Click += new System.EventHandler(this.pl_BtnFreeze_Click);
            // 
            // panel_playersCenter
            // 
            this.panel_playersCenter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_playersCenter.BackColor = System.Drawing.Color.Transparent;
            this.panel_playersCenter.Controls.Add(this.pl_gbProps);
            this.panel_playersCenter.Location = new System.Drawing.Point(138, 4);
            this.panel_playersCenter.Name = "panel_playersCenter";
            this.panel_playersCenter.Size = new System.Drawing.Size(382, 390);
            this.panel_playersCenter.TabIndex = 70;
            // 
            // pl_gbProps
            // 
            this.pl_gbProps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pl_gbProps.Controls.Add(this.pl_pgProps);
            this.pl_gbProps.Location = new System.Drawing.Point(5, 5);
            this.pl_gbProps.Name = "pl_gbProps";
            this.pl_gbProps.Size = new System.Drawing.Size(372, 378);
            this.pl_gbProps.TabIndex = 67;
            this.pl_gbProps.TabStop = false;
            this.pl_gbProps.Text = "Properties for (none selected)";
            // 
            // pl_pgProps
            // 
            this.pl_pgProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pl_pgProps.HelpVisible = false;
            this.pl_pgProps.Location = new System.Drawing.Point(3, 17);
            this.pl_pgProps.Name = "pl_pgProps";
            this.pl_pgProps.Size = new System.Drawing.Size(366, 358);
            this.pl_pgProps.TabIndex = 64;
            this.pl_pgProps.ToolbarVisible = false;
            // 
            // panel_playersLeft
            // 
            this.panel_playersLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel_playersLeft.BackColor = System.Drawing.Color.Transparent;
            this.panel_playersLeft.Controls.Add(this.pl_listBox);
            this.panel_playersLeft.Controls.Add(this.pl_lblOnline);
            this.panel_playersLeft.Location = new System.Drawing.Point(3, 4);
            this.panel_playersLeft.Name = "panel_playersLeft";
            this.panel_playersLeft.Size = new System.Drawing.Size(134, 481);
            this.panel_playersLeft.TabIndex = 69;
            // 
            // pl_listBox
            // 
            this.pl_listBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pl_listBox.BackColor = System.Drawing.SystemColors.Window;
            this.pl_listBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.pl_listBox.FormattingEnabled = true;
            this.pl_listBox.Location = new System.Drawing.Point(6, 22);
            this.pl_listBox.Name = "pl_listBox";
            this.pl_listBox.Size = new System.Drawing.Size(123, 446);
            this.pl_listBox.TabIndex = 62;
            this.pl_listBox.Click += new System.EventHandler(this.pl_listBox_Click);
            // 
            // pl_lblOnline
            // 
            this.pl_lblOnline.AutoSize = true;
            this.pl_lblOnline.Location = new System.Drawing.Point(6, 5);
            this.pl_lblOnline.Name = "pl_lblOnline";
            this.pl_lblOnline.Size = new System.Drawing.Size(78, 13);
            this.pl_lblOnline.TabIndex = 68;
            this.pl_lblOnline.Text = "Online players:";
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
            // panel_mainBtns
            // 
            this.panel_mainBtns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_mainBtns.BackColor = System.Drawing.Color.Transparent;
            this.panel_mainBtns.Controls.Add(this.main_btnClose);
            this.panel_mainBtns.Controls.Add(this.main_btnRestart);
            this.panel_mainBtns.Controls.Add(this.main_btnProps);
            this.panel_mainBtns.Location = new System.Drawing.Point(501, 0);
            this.panel_mainBtns.Name = "panel_mainBtns";
            this.panel_mainBtns.Size = new System.Drawing.Size(270, 31);
            this.panel_mainBtns.TabIndex = 37;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Map";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Rank";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Name";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 102;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.FillWeight = 70F;
            this.dataGridViewTextBoxColumn5.HeaderText = "Players";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 71;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.FillWeight = 70F;
            this.dataGridViewTextBoxColumn6.HeaderText = "Physics";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 71;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(775, 523);
            this.Controls.Add(this.panel_mainBtns);
            this.Controls.Add(this.tabs);
            this.Name = "Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Window_FormClosing);
            this.Load += new System.EventHandler(this.Window_Load);
            this.Resize += new System.EventHandler(this.Window_Resize);
            this.tsMap.ResumeLayout(false);
            this.tsPlayer.ResumeLayout(false);
            this.icon_context.ResumeLayout(false);
            this.logs_tp.ResumeLayout(false);
            this.logs_tab.ResumeLayout(false);
            this.logs_tabErr.ResumeLayout(false);
            this.logs_tabErr.PerformLayout();
            this.logs_tabGen.ResumeLayout(false);
            this.panel_tabGenTop.ResumeLayout(false);
            this.panel_tabGenTop.PerformLayout();
            this.tabLog_Sys.ResumeLayout(false);
            this.tabLog_Sys.PerformLayout();
            this.tp_Main.ResumeLayout(false);
            this.panel_mainRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.main_Players)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.main_Maps)).EndInit();
            this.panel_mainLeft.ResumeLayout(false);
            this.panel_mainLeft.PerformLayout();
            this.tsLog_Menu.ResumeLayout(false);
            this.panel_mainBottom.ResumeLayout(false);
            this.panel_mainBottom.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tp_Maps.ResumeLayout(false);
            this.tableLayoutPanel_Maps.ResumeLayout(false);
            this.panel_mapsRight.ResumeLayout(false);
            this.panel_mapsRight_Bottom.ResumeLayout(false);
            this.map_gbNew.ResumeLayout(false);
            this.map_gbNew.PerformLayout();
            this.panel_mapsRight_Top.ResumeLayout(false);
            this.map_gbProps.ResumeLayout(false);
            this.panel_mapsLeft.ResumeLayout(false);
            this.panel_mapsLeft_Middle.ResumeLayout(false);
            this.panel_mapsLeft_Bottom.ResumeLayout(false);
            this.map_gbUnloaded.ResumeLayout(false);
            this.panel_mapsLeft_Top.ResumeLayout(false);
            this.map_gbLoaded.ResumeLayout(false);
            this.tp_Players.ResumeLayout(false);
            this.panel_playersBottom.ResumeLayout(false);
            this.pl_gbOther.ResumeLayout(false);
            this.pl_gbOther.PerformLayout();
            this.panel_playersRight.ResumeLayout(false);
            this.pl_gbActions.ResumeLayout(false);
            this.pl_gbActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pl_numUndo)).EndInit();
            this.panel_playersCenter.ResumeLayout(false);
            this.pl_gbProps.ResumeLayout(false);
            this.panel_playersLeft.ResumeLayout(false);
            this.panel_playersLeft.PerformLayout();
            this.panel_mainBtns.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.DataGridViewTextBoxColumn main_colPlName;
        private System.Windows.Forms.DataGridViewTextBoxColumn main_colPlMap;
        private System.Windows.Forms.DataGridViewTextBoxColumn main_colPlRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn main_colLvlPhysics;
        private System.Windows.Forms.DataGridViewTextBoxColumn main_colLvlPlayers;
        private System.Windows.Forms.DataGridViewTextBoxColumn main_colLvlName;
        private System.Windows.Forms.Label pl_lblOnline;
        private System.Windows.Forms.GroupBox pl_gbProps;
        private System.Windows.Forms.GroupBox pl_gbActions;
        private System.Windows.Forms.GroupBox pl_gbOther;
        private System.Windows.Forms.TextBox map_txtName;
        private System.Windows.Forms.Label map_lblName;
        private System.Windows.Forms.ComboBox map_cmbX;
        private System.Windows.Forms.ComboBox map_cmbY;
        private System.Windows.Forms.ComboBox map_cmbZ;
        private System.Windows.Forms.ComboBox map_cmbType;
        private System.Windows.Forms.TextBox map_txtSeed;
        private System.Windows.Forms.Label map_lblY;
        private System.Windows.Forms.Label map_lblX;
        private System.Windows.Forms.Label map_lblZ;
        private System.Windows.Forms.Label map_lblSeed;
        private System.Windows.Forms.Label map_lblType;
        private System.Windows.Forms.Button map_btnGen;
        private System.Windows.Forms.GroupBox map_gbNew;
        private System.Windows.Forms.ListBox map_lbUnloaded;
        private System.Windows.Forms.Button map_btnLoad;
        private System.Windows.Forms.GroupBox map_gbUnloaded;
        private System.Windows.Forms.GroupBox map_gbProps;
        private System.Windows.Forms.TabPage tp_Main;
        private System.Windows.Forms.TabPage tabLog_Sys;
        private System.Windows.Forms.TabPage logs_tabErr;
        private System.Windows.Forms.TabPage logs_tabGen;
        private System.Windows.Forms.TabControl logs_tab;

        #endregion

        private Button main_btnClose;
        private ContextMenuStrip icon_context;
        private ToolStripMenuItem icon_hideWindow;
        private ToolStripSeparator icon_separator;
        private ToolStripMenuItem icon_openConsole;
        private ToolStripMenuItem icon_shutdown;
        private ContextMenuStrip tsPlayer;
        private ToolStripMenuItem tsPlayer_whois;
        private ToolStripMenuItem tsPlayer_kick;
        private ToolStripMenuItem tsPlayer_ban;
        private ToolStripMenuItem tsPlayer_voice;
        private ContextMenuStrip tsMap;
        private ToolStripMenuItem tsPlayer_clones;
        private Button main_btnRestart;
        private ToolStripMenuItem icon_restart;
        private TabPage logs_tp;
        private Label logs_lblGeneral;
        private TextBox logs_txtError;
        private TextBox logs_txtSystem;
        private TabPage tp_Maps;
        private DataGridView main_Maps;
        private TextBox main_txtInput;
        private TextBox main_txtUrl;
        private DataGridView main_Players;
        private TabControl tabs;
        private ToolStripMenuItem tsPlayer_promote;
        private ToolStripMenuItem tsPlayer_demote;
        private TabPage tp_Players;
        private RichTextBox logs_txtGeneral;
        private DateTimePicker logs_dateGeneral;
        private Button pl_btnBanIP;
        private Button pl_btnBan;
        private Button pl_btnKick;
        private Button pl_btnMessage;
        private TextBox pl_txtMessage;
        private Button pl_btnWarn;
        private Button pl_btnFreeze;
        private TextBox pl_txtSendCommand;
        private Button pl_btnSendCommand;
        private Button pl_btnKill;
        private MCGalaxy.Gui.TimespanUpDown pl_numUndo;
        private Button pl_btnUndo;
        private Button pl_btnMute;
        private Button pl_btnRules;
        private TextBox pl_statusBox;
        private ListBox pl_listBox;
        private Button main_btnSaveAll;
        private Button main_btnUnloadEmpty;
        private Button main_btnKillPhysics;
        private ToolStripMenuItem tsMap_info;
        private ToolStripMenuItem tsMap_actionsMenu;
        private ToolStripMenuItem tsMap_Save;
        private ToolStripMenuItem tsMap_Unload;
        private ToolStripMenuItem tsMap_moveAll;
        private ToolStripMenuItem tsMap_Reload;
        private ToolStripMenuItem tsMap_physicsMenu;
        private ToolStripMenuItem tsMap_physics0;
        private ToolStripMenuItem tsMap_physics1;
        private ToolStripMenuItem tsMap_physics2;
        private ToolStripMenuItem tsMap_physics3;
        private ToolStripMenuItem tsMap_physics4;
        private ToolStripMenuItem tsMap_physics5;
        private ToolStripSeparator tsMap_separator;
        private Components.ColoredTextBox main_txtLog;
        private ContextMenuStrip tsLog_Menu;
        private ToolStripMenuItem tsLog_night;
        private ToolStripMenuItem tsLog_Colored;
        private ToolStripSeparator tsLog_separator1;
        private ToolStripMenuItem tsLog_copySelected;
        private ToolStripMenuItem tsLog_copyAll;
        private ToolStripSeparator tsLog_separator2;
        private ToolStripMenuItem tsLog_clear;
        private ToolStripMenuItem tsLog_dateStamp;
        private ToolStripMenuItem tsLog_autoScroll;
        private Button main_btnProps;
        private System.Windows.Forms.ToolTip toolTip;
        private HackyPropertyGrid pl_pgProps;
        private HackyPropertyGrid map_pgProps;
        private Panel panel_mainBtns;
        private Panel panel_mainBottom;
        private Panel panel_mainLeft;
        private Panel panel_mainRight;
        private Panel panel_tabGenTop;
        private Panel panel_mapsRight;
        private Panel panel_mapsLeft;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private Panel panel_mapsLeft_Bottom;
        private GroupBox map_gbLoaded;
        private ListBox map_lbLoaded;
        private Panel panel_playersLeft;
        private Panel panel_playersRight;
        private Panel panel_playersCenter;
        private Panel panel_playersBottom;
        private Panel panel_mapsLeft_Top;
        private Panel panel_mapsRight_Bottom;
        private Panel panel_mapsRight_Top;
        private Panel panel_mapsLeft_Middle;
        private TableLayoutPanel tableLayoutPanel_Maps;
    }
}
