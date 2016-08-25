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
using System.Linq;
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
            Text = "Starting MCGalaxy...";
            Show();
            BringToFront();
            WindowState = FormWindowState.Normal;
            
            InitServer();

            notifyIcon1.Text = ("MCGalaxy Server: " + Server.name).Truncate(64);
            notifyIcon1.ContextMenuStrip = this.icon_context;
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Visible = true;
            notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon1_MouseClick);
            LoadChangelog();
            
            // Bind player list
            main_Players.DataSource = pc;
            main_Players.Font = new Font("Calibri", 8.25f);

            main_Maps.DataSource = new LevelCollection(); // Otherwise "-1 does not have a value" exception when clicking a row
            main_Maps.Font = new Font("Calibri", 8.25f);

            UpdateListTimer.Elapsed += delegate {
                try {
                    UpdateClientList(null);
                    UpdateMapList();
                }
                catch { } // needed for slower computers
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

            Player.PlayerConnect += Player_PlayerConnect;
            Player.PlayerDisconnect += Player_PlayerDisconnect;
            Player.OnSendMap += Player_OnSendMap;

            Level.LevelLoaded += Level_LevelLoaded;
            Level.LevelUnload += Level_LevelUnload;

            RunOnUiThread(() => main_btnProps.Enabled = true);
        }

        public void RunOnUiThread(Action act) { Invoke(act); }
        
        void Player_PlayerConnect(Player p) {
            UpdatePlyersListBox();
        }
        
        void Player_PlayerDisconnect(Player p, string reason) {
            UpdatePlyersListBox();
        }
        
        void Player_OnSendMap(Player p, byte[] buffer) {
            UpdatePlayerMapCombo();
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
                Text = Server.name + " - MCGalaxy " + Server.VersionString;
                notifyIcon1.Text = ("MCGalaxy Server: " + Server.name).Truncate(64);
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
                Invoke(new UpdateList(UpdateMapList));
            } else {

                if (main_Maps.DataSource == null)
                    main_Maps.DataSource = lc;

                // Try to keep the same selection on update
                string selected = null;
                if (lc.Count > 0 && main_Maps.SelectedRows.Count > 0) {
                    selected = (from DataGridViewRow row in main_Maps.Rows where row.Selected select lc[row.Index]).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();
                lc.Clear();
                string selectedLvl = null;
                if (map_lbLoaded.SelectedItem != null)
                    selectedLvl = map_lbLoaded.SelectedItem.ToString();
                
                map_lbLoaded.Items.Clear();
                //lc = new LevelCollection(new LevelListView());
                Server.levels.ForEach(l => lc.Add(l));
                Server.levels.ForEach(l => map_lbLoaded.Items.Add(l.name));
                
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
                    foreach (DataGridViewRow row in Server.levels.SelectMany(l => main_Maps.Rows.Cast<DataGridViewRow>().Where(row => (string)row.Cells[0].Value == selected)))
                        row.Selected = true;
                }

                main_Maps.Refresh();
                //dgvPlayers.ResumeLayout();

                // Update the data source and control
                //dgvPlayers.SuspendLayout();
            }
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
                if (!Server.shuttingDown) {
                    MCGalaxy.Gui.App.ExitProgram(false);
                    notifyIcon1.Dispose();
                }
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
        

        void finiteModeToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " finite"); }
        void animalAIToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " ai"); }
        void edgeWaterToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " edge"); }
        void growingGrassToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " grass"); }
        void survivalDeathToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " death"); }
        void killerBlocksToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " killer"); }
        void rPChatToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " chat"); }
        
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

       void tabControl1_Click(object sender, EventArgs e)  {
            try { UpdateUnloadedList(); }
            catch { }
            try { UpdatePlyersListBox(); }
            catch { }
            
            try {
                if (logs_txtGeneral.Text == "")
                    logs_dateGeneral.Value = DateTime.Now;
            }
            catch { }
            foreach (TextBox txtBox in (from TabPage tP in tabs.TabPages from Control ctrl in tP.Controls select ctrl).OfType<TextBox>())
            {
                txtBox.Update();
            }
            tabs.Update();
        }

        void restartServerToolStripMenuItem_Click(object sender, EventArgs e) {
            Restart_Click(sender, e);
        }

        void dgvPlayers_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            e.PaintParts &= ~DataGridViewPaintParts.Focus;
        }

        void promoteToolStripMenuItem_Click(object sender, EventArgs e) {
            PlayerCmd("rank", "+up ", "");
        }

        void demoteToolStripMenuItem_Click(object sender, EventArgs e) {
            PlayerCmd("rank", "-down ", "");
        }
        
        #region Main tab
        List<string> inputLog = new List<string>(21);
        int inputIndex = -1;
        
        void txtInput_KeyDown(object sender, KeyEventArgs e) {
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
        
        void Restart_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to restart?", "Restart", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                MCGalaxy.Gui.App.ExitProgram(true);
            }
        }
        
        void txtUrl_DoubleClick(object sender, EventArgs e) {
            main_txtUrl.SelectAll();
        }
        
        void button_saveall_Click(object sender, EventArgs e) {
            Command.all.Find("save").Use(null, "all");
        }

        void killphysics_button_Click(object sender, EventArgs e) {
            Command.all.Find("physics").Use(null, "kill");
            try { UpdateMapList(); }
            catch { }
        }

        void Unloadempty_button_Click(object sender, EventArgs e) {
            Command.all.Find("unload").Use(null, "empty");
            try { UpdateMapList(); }
            catch { }
        }
        
        #endregion
        

        #region Logs tab
        
        void LoadChangelog() {
            if (!File.Exists("Changelog.txt")) return;
            logs_txtChangelog.Text = "Changelog for " + Server.Version + ":";
            foreach (string line in File.ReadAllLines("Changelog.txt")) {
                logs_txtChangelog.AppendText("\r\n           " + line);
            }
        }
        
        void logs_dateGeneralValueChanged(object sender, EventArgs e) {
            string dayofmonth = logs_dateGeneral.Value.Day.ToString().PadLeft(2, '0');
            string year = logs_dateGeneral.Value.Year.ToString();
            string month = logs_dateGeneral.Value.Month.ToString().PadLeft(2, '0');

            string ymd = year + "-" + month + "-" + dayofmonth;
            string filename = ymd + ".txt";

            if (!File.Exists(Path.Combine("logs/", filename))) {
                logs_txtGeneral.Text = "No logs found for: " + ymd;
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
        
        void MapGenClick(object sender, EventArgs e) {
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
                } catch {
                    MessageBox.Show("Level Creation Failed. Are  you sure you didn't leave a box empty?");
                }

                if (LevelInfo.ExistsOffline(name)) {
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
        
        void MapLoadClick(object sender, EventArgs e) {
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
                string[] files = Directory.GetFiles("levels", "*.lvl");
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
         
        public void UpdatePlyersListBox() {
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
        
        void LoadPlayerTabDetails(object sender, EventArgs e) {
            Player p = PlayerInfo.FindExact(pl_listBox.Text);
            if (p == null) return;
            
            pl_statusBox.AppendTextAndScroll("==" + p.name + "==");
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

        void UndoBt_Click(object sender, EventArgs e) {
            if (curPlayer == null) { pl_statusBox.AppendTextAndScroll("No player selected"); return; }
            if (pl_txtUndo.Text.Trim() == "")  {
                pl_statusBox.AppendTextAndScroll("You didn't specify a time"); return;
            }

            try {
                Command.core.Find("undo").Use(null, curPlayer.name + " " + pl_txtUndo.Text);
                pl_statusBox.AppendTextAndScroll("Undid player for " + pl_txtUndo.Text + " Seconds");
            } catch {
                pl_statusBox.AppendTextAndScroll("Something went wrong!!");
            }
        }

        void MessageBt_Click(object sender, EventArgs e) {
            if (curPlayer == null) { pl_statusBox.AppendTextAndScroll("No player selected"); return; }
            Player.SendMessage(curPlayer, "<CONSOLE> " + pl_txtMessage.Text);
            pl_statusBox.AppendTextAndScroll("Sent player message '<CONSOLE> " + pl_txtMessage.Text + "'");
            pl_txtMessage.Text = "";
        }

        void ImpersonateORSendCmdBt_Click(object sender, EventArgs e) {
            if (curPlayer == null) { pl_statusBox.AppendTextAndScroll("No player selected"); return; }
            
            try {
                if (pl_txtImpersonate.Text.StartsWith("/")) {
                    string[] args = pl_txtImpersonate.Text.Trim().SplitSpaces(2);
                    Command cmd = Command.all.Find(args[0].Replace("/", ""));
                    if (cmd == null) {
                        pl_statusBox.AppendTextAndScroll("That isn't a command!!"); return;
                    }
                    
                    cmd.Use(curPlayer, args.Length > 1 ? args[1] : "");
                    if (args.Length > 1) {
                        pl_statusBox.AppendTextAndScroll("Used command '" + args[0] + "' with parameters '" + args[1] + "' as player");
                    } else {
                        pl_statusBox.AppendTextAndScroll("Used command '" + args[0] + "' with no parameters as player");
                    }
                } else {
                    Command.all.Find("impersonate").Use(null, curPlayer.name + " " + pl_txtImpersonate.Text);
                    pl_statusBox.AppendTextAndScroll("Sent Message '" + pl_txtImpersonate.Text + "' as player");
                }
                pl_txtImpersonate.Text = "";
            } catch {
                pl_statusBox.AppendTextAndScroll("Something went wrong");
            }
        }

        void SlapBt_Click(object sender, EventArgs e) { DoCmd("slap", "Slapped"); }
        void KillBt_Click(object sender, EventArgs e) { DoCmd("kill", "Killed"); }
        void WarnBt_Click(object sender, EventArgs e) { DoCmd("warn", "Warned"); }
        void KickBt_Click(object sender, EventArgs e) { DoCmd("kick", "Kicked"); }
        void BanBt_Click(object sender, EventArgs e) { DoCmd("ban", "Banned"); }
        void IPBanBt_Click(object sender, EventArgs e) { DoCmd("banip", "IP-Banned"); }
        
        void DoCmd(string cmdName, string action) {
            if (curPlayer == null) { pl_statusBox.AppendTextAndScroll("No player selected"); return; }
            Command.all.Find(cmdName).Use(null, curPlayer.name);
            pl_statusBox.AppendTextAndScroll(action + " player");
        }

        void SendRulesTxt_Click(object sender, EventArgs e) {
            if (curPlayer == null) { pl_statusBox.AppendTextAndScroll("No Player Selected"); return; }
            Command.all.Find("rules").Use(curPlayer, "");
            pl_statusBox.AppendTextAndScroll("Sent rules to player");
        }

        void SpawnBt_Click(object sender, EventArgs e) {
            if (curPlayer == null) { pl_statusBox.AppendTextAndScroll("No Player Selected"); return; }          
            Command.all.Find("spawn").Use(curPlayer, "");
            pl_statusBox.AppendTextAndScroll("Sent player to spawn");
        }

        void PlyersListBox_Click(object sender, EventArgs e) {
            LoadPlayerTabDetails(sender, e);
        }

        void ImpersonateORSendCmdTxt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) ImpersonateORSendCmdBt_Click(sender, e);
        }
        void UndoTxt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) UndoBt_Click(sender, e);
        }
        void PLayersMessageTxt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) MessageBt_Click(sender, e);
        }
        #endregion

        void infoToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map"); LevelCmd("mapinfo"); }
        void moveAllToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("moveall"); }
        void toolStripMenuItem2_Click_1(object sender, EventArgs e) { LevelCmd("physics", " 0"); }
        void toolStripMenuItem3_Click_1(object sender, EventArgs e) { LevelCmd("physics", " 1"); }
        void toolStripMenuItem4_Click_1(object sender, EventArgs e) { LevelCmd("physics", " 2"); }
        void toolStripMenuItem5_Click_1(object sender, EventArgs e) { LevelCmd("physics", " 3"); }
        void toolStripMenuItem6_Click_1(object sender, EventArgs e) { LevelCmd("physics", " 4"); }
        void toolStripMenuItem7_Click_1(object sender, EventArgs e) { LevelCmd("physics", " 5"); }
        void saveToolStripMenuItem_Click_1(object sender, EventArgs e) { LevelCmd("save"); }
        void unloadToolStripMenuItem_Click_1(object sender, EventArgs e) { LevelCmd("unload"); }
        void reloadToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("reload"); }

        #region Colored Reader Context Menu

        void nightModeToolStripMenuItem_Click_1(object sender, EventArgs e) {
            if (MessageBox.Show("Changing to and from night mode will clear your logs. Do you still want to change?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            main_txtLog.NightMode = tsLog_night.Checked;
            tsLog_night.Checked = !tsLog_night.Checked;
        }

        void colorsToolStripMenuItem_Click_1(object sender, EventArgs e) {
            main_txtLog.Colorize = !tsLog_Colored.Checked;
            tsLog_Colored.Checked = !tsLog_Colored.Checked;
        }

        void dateStampToolStripMenuItem_Click(object sender, EventArgs e) {
            main_txtLog.DateStamp = !tsLog_dateStamp.Checked;
            tsLog_dateStamp.Checked = !tsLog_dateStamp.Checked;
        }

        void autoScrollToolStripMenuItem_Click(object sender, EventArgs e) {
            main_txtLog.AutoScroll = !tsLog_autoScroll.Checked;
            tsLog_autoScroll.Checked = !tsLog_autoScroll.Checked;
        }

        void copySelectedToolStripMenuItem_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(main_txtLog.SelectedText)) return;
            Clipboard.SetText(main_txtLog.SelectedText, TextDataFormat.Text);
        }
        
        void copyAllToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText(main_txtLog.Text, TextDataFormat.Text);
        }
        
        void clearToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to clear logs?", "You sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                main_txtLog.Clear();
            }
        }
        #endregion
    }
}
