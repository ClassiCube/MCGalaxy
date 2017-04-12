/*    
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MCGalaxy.Gui {
    public partial class Window : Form {
        // for cross thread use
        delegate void StringCallback(string s);
        delegate void PlayerListCallback(List<Player> players);
        delegate void VoidDelegate();
        bool mapgen = false;

        PlayerCollection pc = new PlayerCollection();
        LevelCollection lc = new LevelCollection();
        public NotifyIcon notifyIcon1 = new NotifyIcon();
        Player curPlayer;

        readonly System.Timers.Timer UpdateListTimer = new System.Timers.Timer(10000);

        public Window() {
            InitializeComponent();
        }

        void Window_Load(object sender, EventArgs e) {
            main_btnProps.Enabled = false;
            MaximizeBox = false;
            Text = "Starting " + Server.SoftwareNameVersioned + "...";
            Show();
            BringToFront();
            WindowState = FormWindowState.Normal;
            
            InitServer();

            Text = Server.name + " - " + Server.SoftwareNameVersioned;
            notifyIcon1.Text =  Server.name.Truncate(63);
            notifyIcon1.ContextMenuStrip = this.icon_context;
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Visible = true;
            notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon1_MouseClick);
            
            // Bind player list
            main_Players.DataSource = pc;
            main_Players.Font = new Font("Calibri", 8.25f);

            main_Maps.DataSource = new LevelCollection(); // Otherwise "-1 does not have a value" exception when clicking a row
            main_Maps.Font = new Font("Calibri", 8.25f);

            UpdateListTimer.Elapsed += delegate {
                try {
                    UpdateMapList();
                } catch { } // needed for slower computers
                //Server.s.Log("Lists updated!");
            }; UpdateListTimer.Start();

        }
        
        void InitServer() {
            Server s = new Server();
            s.OnLog += WriteLine;
            s.OnCommand += WriteCommand;
            s.OnError += LogErrorMessage;
            s.OnSystem += LogSystemMessage;

            s.HeartBeatFail += HeartBeatFail;
            s.OnURLChange += UpdateUrl;
            s.OnPlayerListChange += UpdateClientList;
            s.OnSettingsUpdate += SettingsUpdate;
            Server.Background.QueueOnce(InitServerTask);
        }
        
        void InitServerTask() {
            Server.s.Start();
            // The first check for updates is run after 10 seconds, subsequent ones every two hours
            Server.Background.QueueRepeat(Updater.UpdaterTask, null, TimeSpan.FromSeconds(10));

            Player.PlayerConnect += Player_PlayerConnect;
            Player.PlayerDisconnect += Player_PlayerDisconnect;
            Player.OnSendMap += Player_OnSendMap;

            Level.LevelLoaded += Level_LevelLoaded;
            Level.LevelUnload += Level_LevelUnload;

            RunOnUiThread(() => main_btnProps.Enabled = true);
        }

        public void RunOnUiThread(Action act) { Invoke(act); }
        
        void Player_PlayerConnect(Player p) {
            UpdatePlayersListBox();
        }
        
        void Player_PlayerDisconnect(Player p, string reason) {
            UpdatePlayersListBox();
        }
        
        void Player_OnSendMap(Player p, byte[] buffer) {
            RunOnUiThread(() => {
                UpdatePlayerMapCombo();
            });
        }
        
        void Level_LevelUnload(Level l) {
            RunOnUiThread(() => {
                UpdateMapList();
                UpdatePlayerMapCombo();
                UpdateUnloadedList();
            });
        }
        
        void Level_LevelLoaded(Level l) {
            RunOnUiThread(() => {
                UpdatePlayerMapCombo();
                UpdateUnloadedList();
            });
        }

        void SettingsUpdate() {
            if (Server.shuttingDown) return;
            
            if (main_txtLog.InvokeRequired) {
                Invoke(new VoidDelegate(SettingsUpdate));
            } else {
                Text = Server.name + " -" + Server.SoftwareNameVersioned;
                notifyIcon1.Text = Server.name.Truncate(63);
            }
        }

        void HeartBeatFail() {
            WriteLine("Recent Heartbeat Failed");
        }

        delegate void LogDelegate(string message);

        /// <summary> Does the same as Console.WriteLine() only in the form </summary>
        /// <param name="s">The line to write</param>
        public void WriteLine(string s) {
            if (Server.shuttingDown) return;
            
            if (InvokeRequired) {
                Invoke(new LogDelegate(WriteLine), new object[] { s });
            } else {
                //Begin substring of crappy date stamp
                int index = s.IndexOf(')');
                s = index == -1 ? s : s.Substring(index + 1);
                //end substring

                main_txtLog.AppendLog(s + Environment.NewLine);
            }
        }
        
        void WriteCommand(string s) {
            if (Server.shuttingDown) return;
            
            if (InvokeRequired) {
                Invoke(new LogDelegate(WriteCommand), new object[] { s });
            } else {
                main_txtLog.AppendLog(s + Environment.NewLine, main_txtLog.ForeColor, false);
            }
        }

        /// <summary> Updates the list of client names in the window </summary>
        /// <param name="players">The list of players to add</param>
        public void UpdateClientList(List<Player> playerList) {
            if (InvokeRequired) {
                Invoke(new PlayerListCallback(UpdateClientList), playerList); return;
            }
            
            if (main_Players.DataSource == null)
                main_Players.DataSource = pc;

            // Try to keep the same selection on update
            string selected = null;
            var selectedRows = main_Players.SelectedRows;
            if (pc.Count > 0 && selectedRows.Count > 0)
                selected = pc[selectedRows[0].Index].name;

            // Update the data source and control
            pc = new PlayerCollection();
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players)
                pc.Add(pl);

            main_Players.DataSource = pc;
            
            // Reselect player
            if (selected != null) {
                for (int i = 0; i < main_Players.Rows.Count; i++) {
                    if (Equals(main_Players.Rows[i].Cells[0].Value, selected))
                        main_Players.Rows[i].Selected = true;
                }
            }

            main_Players.Refresh();
        }

        public void PopupNotify(string message, ToolTipIcon icon = ToolTipIcon.Info) {
            notifyIcon1.ShowBalloonTip(3000, Server.name, message, icon);
        }

        public delegate void UpdateList();

        public void UpdateMapList() {
            if (InvokeRequired) {
                Invoke(new UpdateList(UpdateMapList)); return;
            }
            
            if (main_Maps.DataSource == null)
                main_Maps.DataSource = lc;

            // Try to keep the same selection on update
            List<string> selected = null;
            if (lc.Count > 0 && main_Maps.SelectedRows.Count > 0) {
                selected = new List<string>();
                foreach (DataGridViewRow row in main_Maps.SelectedRows) {
                    string lvlName = (string)row.Cells[0].Value;
                    selected.Add(lvlName);
                }
            }

            // Update the data source and control
            //dgvPlayers.SuspendLayout();
            lc.Clear();
            string selectedLvl = null;
            if (map_lbLoaded.SelectedItem != null)
                selectedLvl = map_lbLoaded.SelectedItem.ToString();
            
            map_lbLoaded.Items.Clear();
            //lc = new LevelCollection(new LevelListView());
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                lc.Add(lvl);
                map_lbLoaded.Items.Add(lvl.name);
            }
            
            if (selectedLvl != null) {
                int index = map_lbLoaded.Items.IndexOf(selectedLvl);
                map_lbLoaded.SelectedIndex = index;
            } else {
                map_lbLoaded.SelectedIndex = -1;
            }
            UpdateSelectedMap(null, null);

            //dgvPlayers.Invalidate();
            main_Maps.DataSource = null;
            main_Maps.DataSource = lc;
            
            // Reselect map
            if (selected != null) {
                foreach (DataGridViewRow row in main_Maps.Rows) {
                    string lvlName = (string)row.Cells[0].Value;
                    if (selected.Contains(lvlName)) row.Selected = true;
                }
            }

            main_Maps.Refresh();
            //dgvPlayers.ResumeLayout();

            // Update the data source and control
            //dgvPlayers.SuspendLayout();
        }

        /// <summary> Places the server's URL at the top of the window </summary>
        /// <param name="s">The URL to display</param>
        public void UpdateUrl(string s) {
            if (InvokeRequired) {
                StringCallback d = UpdateUrl;
                Invoke(d, new object[] { s });
            } else {
                main_txtUrl.Text = s;
            }
        }

        void Window_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.WindowsShutDown) {
                MCGalaxy.Gui.App.ExitProgram(false);
                notifyIcon1.Dispose();
            }
            if (Server.shuttingDown || MessageBox.Show("Really Shutdown the Server? All Connections will break!", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                if (!Server.shuttingDown) MCGalaxy.Gui.App.ExitProgram(false);
                notifyIcon1.Dispose();
            } else {
                // Prevents form from closing when user clicks the X and then hits 'cancel'
                e.Cancel = true;
            }
        }

        void btnClose_Click_1(object sender, EventArgs e) { Close(); }

        void btnProperties_Click_1(object sender, EventArgs e) {
            if (!prevLoaded) { PropertyForm = new PropertyWindow(); prevLoaded = true; }
            PropertyForm.Show();
            if (!PropertyForm.Focused) PropertyForm.Focus();
        }

        public static bool prevLoaded = false;
        Form PropertyForm;

        void Window_Resize(object sender, EventArgs e) {
            ShowInTaskbar = WindowState != FormWindowState.Minimized;
        }

        void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            Show();
            BringToFront();
            WindowState = FormWindowState.Normal;
        }

        void openConsole_Click(object sender, EventArgs e) {
            Show();
            BringToFront();
            WindowState = FormWindowState.Normal;
        }

        void shutdownServer_Click(object sender, EventArgs e) {
            Close();
        }

        void clonesToolStripMenuItem_Click(object sender, EventArgs e) { PlayerCmd("clones"); }
        void voiceToolStripMenuItem_Click(object sender, EventArgs e) { PlayerCmd("voice"); }
        void whoisToolStripMenuItem_Click(object sender, EventArgs e) { PlayerCmd("whois"); }       
        void banToolStripMenuItem_Click(object sender, EventArgs e) { PlayerCmd("ban"); }
        void kickToolStripMenuItem_Click(object sender, EventArgs e) {
            PlayerCmd("kick", "", " You have been kicked by the console.");
        }

        Player GetSelectedPlayer() {
            if (main_Players.SelectedRows.Count <= 0) return null;
            return (Player)(main_Players.SelectedRows[0].DataBoundItem);
        }
        
        void PlayerCmd(string com) {
            if (GetSelectedPlayer() != null)
                Command.all.Find(com).Use(null, GetSelectedPlayer().name);
        }
        
        void PlayerCmd(string com, string prefix, string suffix) {
            if (GetSelectedPlayer() != null)
                Command.all.Find(com).Use(null, prefix + GetSelectedPlayer().name + suffix);
        }
        
        Level GetSelectedLevel() {
            if (main_Maps.SelectedRows.Count <= 0) return null;
            return (Level)(main_Maps.SelectedRows[0].DataBoundItem);
        }
        
        void LevelCmd(string com) {
            if (GetSelectedLevel() != null)
                Command.all.Find(com).Use(null, GetSelectedLevel().name);
        }

        void LevelCmd(string com, string args) {
            if (GetSelectedLevel() != null)
                Command.all.Find(com).Use(null, GetSelectedLevel().name + args);
        }        

       void tabs_Click(object sender, EventArgs e)  {
            try { UpdateUnloadedList(); }
            catch { }
            try { UpdatePlayersListBox(); }
            catch { }
            
            try {
                if (logs_txtGeneral.Text == "")
                    logs_dateGeneral.Value = DateTime.Now;
            } catch { }
            
            foreach (TabPage page in tabs.TabPages)
                foreach (Control control in page.Controls)
            {
                if (!control.GetType().IsSubclassOf(typeof(TextBox))) continue;
                control.Update();
            }
            tabs.Update();
        }

        void icon_restart_Click(object sender, EventArgs e) {
            main_BtnRestart_Click(sender, e);
        }

        void main_players_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            e.PaintParts &= ~DataGridViewPaintParts.Focus;
        }

        void tsPlayer_promote_Click(object sender, EventArgs e) {
            PlayerCmd("rank", "+up ", "");
        }

        void tsPlayer_demote_Click(object sender, EventArgs e) {
            PlayerCmd("rank", "-down ", "");
        }
        
        #region Main tab
        void tsMap_Info_Click(object sender, EventArgs e) { LevelCmd("map"); LevelCmd("mapinfo"); }
        void tsMap_MoveAll_Click(object sender, EventArgs e) { LevelCmd("moveall"); }
        void tsMap_Physics0_Click(object sender, EventArgs e) { LevelCmd("physics", " 0"); }
        void tsMap_Physics1_Click(object sender, EventArgs e) { LevelCmd("physics", " 1"); }
        void tsMap_Physics2_Click(object sender, EventArgs e) { LevelCmd("physics", " 2"); }
        void tsMap_Physics3_Click(object sender, EventArgs e) { LevelCmd("physics", " 3"); }
        void tsMap_Physics4_Click(object sender, EventArgs e) { LevelCmd("physics", " 4"); }
        void tsMap_Physics5_Click(object sender, EventArgs e) { LevelCmd("physics", " 5"); }
        void tsMap_Save_Click(object sender, EventArgs e) { LevelCmd("save"); }
        void tsMap_Unload_Click(object sender, EventArgs e) { LevelCmd("unload"); }
        void tsMap_Reload_Click(object sender, EventArgs e) { LevelCmd("reload"); }
        
        List<string> inputLog = new List<string>(21);
        int inputIndex = -1;
        
        void main_TxtInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Up) {
                inputIndex = Math.Min(inputIndex + 1, inputLog.Count - 1);
                if (inputIndex > -1) SetInputText();
            } else if (e.KeyCode == Keys.Down) {
                inputIndex = Math.Max(inputIndex - 1, -1);
                if (inputIndex > -1) SetInputText();
            } else if (e.KeyCode == Keys.Enter) {
                InputText();
            } else {
                inputIndex = -1; return;
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        
        void SetInputText() {
            if (inputIndex == -1) return;
            main_txtInput.Text = inputLog[inputIndex];
            main_txtInput.SelectionLength = 0;
            main_txtInput.SelectionStart = main_txtInput.Text.Length;
        }
        
        void InputText() {
            string text = main_txtInput.Text;
            if (text.Length == 0) return;
            
            inputLog.Insert(0, text);
            if (inputLog.Count > 20) 
                inputLog.RemoveAt(20);
            
            if (text[0] == '/' && text.Length > 1 && text[1] == '/') {
                Handlers.HandleChat(text.Substring(1), WriteLine);
            } else if (text[0] == '/') {
                Handlers.HandleCommand(text.Substring(1), WriteCommand);
            } else {
                Handlers.HandleChat(text, WriteLine);
            }
            main_txtInput.Clear();
        }
        
        void main_BtnRestart_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to restart?", "Restart", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                MCGalaxy.Gui.App.ExitProgram(true);
            }
        }
        
        void main_TxtUrl_DoubleClick(object sender, EventArgs e) {
            main_txtUrl.SelectAll();
        }
        
        void main_BtnSaveAll_Click(object sender, EventArgs e) {
            Command.all.Find("save").Use(null, "all");
        }

        void main_BtnKillPhysics_Click(object sender, EventArgs e) {
            Command.all.Find("physics").Use(null, "kill");
            try { UpdateMapList(); }
            catch { }
        }

        void main_BtnUnloadEmpty_Click(object sender, EventArgs e) {
            Command.all.Find("unload").Use(null, "empty");
            try { UpdateMapList(); }
            catch { }
        }
        
        #endregion
        

        #region Logs tab
        
        void logs_dateGeneral_Changed(object sender, EventArgs e) {
            string day = logs_dateGeneral.Value.Day.ToString().PadLeft(2, '0');
            string year = logs_dateGeneral.Value.Year.ToString();
            string month = logs_dateGeneral.Value.Month.ToString().PadLeft(2, '0');

            string date = year + "-" + month + "-" + day;
            string filename = date + ".txt";

            if (!File.Exists(Path.Combine("logs/", filename))) {
                logs_txtGeneral.Text = "No logs found for: " + date;
            } else {
                logs_txtGeneral.Text = null;
                logs_txtGeneral.Text = File.ReadAllText(Path.Combine("logs/", filename));
            }
        }

        void LogErrorMessage(string message) {
            try {
                if (logs_txtError.InvokeRequired) {
                    Invoke(new LogDelegate(LogErrorMessage), new object[] { message });
                } else {
                    logs_txtError.AppendText(Environment.NewLine + message);
                }
            } catch { 
            }
        }
        
        void LogSystemMessage(string message) {
            try {
                if (logs_txtSystem.InvokeRequired) {
                    Invoke(new LogDelegate(LogSystemMessage), new object[] { message });
                } else {
                    logs_txtSystem.AppendText(Environment.NewLine + message);
                }
            } catch { 
            }
        }
        
        #endregion
        
        
        #region Map tab
        
        void map_BtnGen_Click(object sender, EventArgs e) {
            if (mapgen) { MessageBox.Show("A map is already being generated."); return; }
            string name, x, y, z, type, seed;

            try { name = map_txtName.Text.ToLower(); }
            catch { name = ""; }
            if (String.IsNullOrEmpty(name)) { MessageBox.Show("Map name cannot be blank."); return; }
            try { x = map_cmbX.SelectedItem.ToString(); }
            catch { x = ""; }
            if (String.IsNullOrEmpty(x)) { MessageBox.Show("Map width cannot be blank."); return; }
            
            try { y = map_cmbY.SelectedItem.ToString(); }
            catch { y = ""; }
            if (String.IsNullOrEmpty(y)) { MessageBox.Show("Map height cannot be blank."); return; }
            
            try { z = map_cmbZ.SelectedItem.ToString(); }
            catch { z = ""; }
            if (String.IsNullOrEmpty(z)) { MessageBox.Show("Map length cannot be blank."); return; }
            
            try { type = map_cmbType.SelectedItem.ToString().ToLower(); }
            catch { type = ""; }
            if (String.IsNullOrEmpty(type)) { MessageBox.Show("Map type cannot be blank."); return; }
            
            try { seed = map_txtSeed.Text; }
            catch { seed = ""; }

            Thread genThread = new Thread(() =>
            {
                mapgen = true;
                try {
                    string args = name + " " + x + " " + y + " " + z + " " + type;
                    if (!String.IsNullOrEmpty(seed)) args += " " + seed;
                    Command.all.Find("newlvl").Use(null, args);
                } catch (Exception ex) {
                    Server.ErrorLog(ex);
                    MessageBox.Show("Level creation failed. Check error logs for details.");
                }

                if (LevelInfo.MapExists(name)) {
                    MessageBox.Show("Created Level");
                    try {
                        UpdateUnloadedList();
                        UpdateMapList();
                    } catch { 
                    }
                } else {
                    MessageBox.Show("Level may not have been created.");
                }
                mapgen = false;
            });
            genThread.Name = "MCG_GuiGenMap";
            genThread.Start();
        }
        
        void mao_BtnLoad_Click(object sender, EventArgs e) {
            try {
                Command.all.Find("load").Use(null, map_lbUnloaded.SelectedItem.ToString());
            } catch { 
            }
            UpdateUnloadedList();
            UpdateMapList();
        }
        
        string last = null;
        void UpdateSelectedMap(object sender, EventArgs e) {
            if (map_lbLoaded.SelectedItem == null) {
                if (map_pgProps.SelectedObject == null) return;
                map_pgProps.SelectedObject = null; last = null;
                map_gbProps.Text = "Properties for (none selected)"; return;
            }
            
            string name = map_lbLoaded.SelectedItem.ToString();
            Level lvl = LevelInfo.FindExact(name);
            if (lvl == null) {
                if (map_pgProps.SelectedObject == null) return;
                map_pgProps.SelectedObject = null; last = null;
                map_gbProps.Text = "Properties for (none selected)"; return;
            }
            
            if (name == last) return;
            last = name;
            LevelProperties settings = new LevelProperties(lvl);
            map_pgProps.SelectedObject = settings;
            map_gbProps.Text = "Properties for " + name;
        }
        
        public void UpdateUnloadedList() {
            RunOnUiThread(() =>
            {
                string selectedLvl = null;
                if (map_lbUnloaded.SelectedItem != null)
                    selectedLvl = map_lbUnloaded.SelectedItem.ToString();
                
                map_lbUnloaded.Items.Clear();
                string[] files = LevelInfo.AllMapFiles();
                foreach (string file in files) {
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (LevelInfo.FindExact(name) == null)
                        map_lbUnloaded.Items.Add(name);
                }
                
                if (selectedLvl != null) {
                    int index = map_lbUnloaded.Items.IndexOf(selectedLvl);
                    map_lbUnloaded.SelectedIndex = index;
                } else {
                    map_lbUnloaded.SelectedIndex = -1;
                }
            });
        }
        #endregion
        

        #region Players tab
        PlayerProperties playerProps;
         
        public void UpdatePlayersListBox() {
            RunOnUiThread(
                delegate {
                    pl_listBox.Items.Clear();
                    Player[] players = PlayerInfo.Online.Items;
                    foreach (Player p in players)
                        pl_listBox.Items.Add(p.name);
                    
                    if (curPlayer == null) return;
                    if (PlayerInfo.FindExact(curPlayer.name) != null) return;
                    
                    curPlayer = null;
                    playerProps = null;
                    pl_gbProps.Text = "Properties for (none selected)";
                    pl_pgProps.SelectedObject = null;
                });
        }
        
        void AppendPlayerStatus(string text) {
            if (InvokeRequired) {
                Action<string> d = AppendPlayerStatus;
                Invoke(d, new object[] { text, true });
            } else {
                pl_statusBox.AppendText(text + Environment.NewLine);
            }
        }
        
        void LoadPlayerTabDetails(object sender, EventArgs e) {
            Player p = PlayerInfo.FindExact(pl_listBox.Text);
            if (p == null || p == curPlayer) return;
            
            pl_statusBox.Text = "";
            AppendPlayerStatus("==" + p.name + "==");
            playerProps = new PlayerProperties(p);
            pl_gbProps.Text = "Properties for " + p.name;
            pl_pgProps.SelectedObject = playerProps;
            curPlayer = p;
            
            try {
                UpdatePlayerMapCombo();
            } catch { }
        }

        void UpdatePlayerMapCombo() {          
            if (tabs.SelectedTab != tp_Players) return;
            pl_pgProps.Refresh();
        }

        void pl_BtnUndo_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No player selected"); return; }
            if (pl_txtUndo.Text.Trim() == "")  {
                AppendPlayerStatus("You didn't specify a time"); return;
            }

            try {
                Command.core.Find("undo").Use(null, curPlayer.name + " " + pl_txtUndo.Text);
                AppendPlayerStatus("Undid player for " + pl_txtUndo.Text + " seconds");
            } catch {
                AppendPlayerStatus("Something went wrong!!");
            }
        }

        void pl_BtnMessage_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No player selected"); return; }
            Player.Message(curPlayer, "<CONSOLE> " + pl_txtMessage.Text);
            AppendPlayerStatus("Sent player message '<CONSOLE> " + pl_txtMessage.Text + "'");
            pl_txtMessage.Text = "";
        }

        void pl_BtnSendCommand_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No player selected"); return; }
            
            try {
                string[] args = pl_txtImpersonate.Text.Trim().SplitSpaces(2);
                args[0] = args[0].Replace("/", "");
                Command cmd = Command.all.Find(args[0]);
                if (cmd == null) {
                    AppendPlayerStatus("There is no command '" + args[0] + "'"); return;
                }
                
                cmd.Use(curPlayer, args.Length > 1 ? args[1] : "");
                if (args.Length > 1) {
                    AppendPlayerStatus("Used command '" + args[0] + "' with parameters '" + args[1] + "' as player");
                } else {
                    AppendPlayerStatus("Used command '" + args[0] + "' with no parameters as player");
                }
                pl_txtImpersonate.Text = "";
            } catch {
                AppendPlayerStatus("Something went wrong");
            }
        }

        void pl_BtnSlap_Click(object sender, EventArgs e) { DoCmd("slap", "Slapped"); }
        void pl_BtnKill_Click(object sender, EventArgs e) { DoCmd("kill", "Killed"); }
        void pl_BtnWarn_Click(object sender, EventArgs e) { DoCmd("warn", "Warned"); }
        void pl_BtnKick_Click(object sender, EventArgs e) { DoCmd("kick", "Kicked"); }
        void pl_BtnBan_Click(object sender, EventArgs e) { DoCmd("ban", "Banned"); }
        void pl_BtnIPBan_Click(object sender, EventArgs e) { DoCmd("banip", "IP-Banned"); }
        
        void DoCmd(string cmdName, string action) {
            if (curPlayer == null) { AppendPlayerStatus("No player selected"); return; }
            Command.all.Find(cmdName).Use(null, curPlayer.name);
            AppendPlayerStatus(action + " player");
        }

        void pl_BtnRules_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No Player Selected"); return; }
            Command.all.Find("rules").Use(curPlayer, "");
            AppendPlayerStatus("Sent rules to player");
        }

        void pl_BtnSpawn_Click(object sender, EventArgs e) {
            if (curPlayer == null) { AppendPlayerStatus("No Player Selected"); return; }
            Command.all.Find("spawn").Use(curPlayer, "");
            AppendPlayerStatus("Sent player to spawn");
        }

        void pl_listBox_Click(object sender, EventArgs e) {
            LoadPlayerTabDetails(sender, e);
        }

        void pl_txtImpersonate_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) pl_BtnSendCommand_Click(sender, e);
        }
        void pl_txtUndo_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) pl_BtnUndo_Click(sender, e);
        }
        void pl_txtMessage_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) pl_BtnMessage_Click(sender, e);
        }
        
        #endregion
        

        #region Main tab log - context menu

        void tsLog_Night_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Changing to and from night mode will clear your logs. Do you still want to change?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            main_txtLog.NightMode = tsLog_night.Checked;
            tsLog_night.Checked = !tsLog_night.Checked;
        }

        void tsLog_Colored_Click(object sender, EventArgs e) {
            main_txtLog.Colorize = !tsLog_Colored.Checked;
            tsLog_Colored.Checked = !tsLog_Colored.Checked;
        }

        void tsLog_DateStamp_Click(object sender, EventArgs e) {
            main_txtLog.DateStamp = !tsLog_dateStamp.Checked;
            tsLog_dateStamp.Checked = !tsLog_dateStamp.Checked;
        }

        void tsLog_AutoScroll_Click(object sender, EventArgs e) {
            main_txtLog.AutoScroll = !tsLog_autoScroll.Checked;
            tsLog_autoScroll.Checked = !tsLog_autoScroll.Checked;
        }

        void tsLog_CopySelected_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(main_txtLog.SelectedText)) return;
            Clipboard.SetText(main_txtLog.SelectedText, TextDataFormat.Text);
        }
        
        void tsLog_CopyAll_Click(object sender, EventArgs e) {
            Clipboard.SetText(main_txtLog.Text, TextDataFormat.Text);
        }
        
        void tsLog_Clear_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to clear logs?", "You sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
        		main_txtLog.ClearLog();
            }
        }
        #endregion
    }
}
