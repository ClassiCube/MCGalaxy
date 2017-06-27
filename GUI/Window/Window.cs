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
using MCGalaxy.Generator;
using MCGalaxy.Tasks;

namespace MCGalaxy.Gui {
    public partial class Window : Form {
        // for cross thread use
        delegate void StringCallback(string s);
        delegate void PlayerListCallback(List<Player> players);
        delegate void VoidDelegate();
        bool mapgen = false;

        PlayerCollection pc = new PlayerCollection();
        LevelCollection lc = new LevelCollection();
        public NotifyIcon notifyIcon = new NotifyIcon();
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
            foreach (string theme in MapGen.SimpleThemeNames) {
                map_cmbType.Items.Add(theme);
            }
            
            Text = ServerConfig.Name + " - " + Server.SoftwareNameVersioned;
            MakeNotifyIcon();
            
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
        
        void UpdateNotifyIconText() {
            int playerCount = PlayerInfo.Online.Count;
            string players = " (" + playerCount + " players)";
            notifyIcon.Text = (ServerConfig.Name + players).Truncate(63);
        }
        
        void MakeNotifyIcon() {
            UpdateNotifyIconText();
            notifyIcon.ContextMenuStrip = icon_context;
            notifyIcon.Icon = Icon;
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += notifyIcon_MouseClick;
        }

        void notifyIcon_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) openConsole_Click(sender, e);
        }
        
        void InitServer() {
            Server s = new Server();
            Logger.LogHandler += LogMessage;

            Server.HeartBeatFail += HeartBeatFail;
            Server.OnURLChange += UpdateUrl;
            Server.OnPlayerListChange += UpdateClientList;
            Server.OnSettingsUpdate += SettingsUpdate;
            Server.Background.QueueOnce(InitServerTask);
        }
        
        void LogMessage(LogType type, string message) {
            string now;
            switch (type) {
                case LogType.Error:
                    WriteLine("!!!Error! See " + FileLogger.ErrorLogPath + " for more information.");
                    LogErrorMessage(message); 
                    break;
                case LogType.BackgroundActivity:
                    now = DateTime.Now.ToString("(HH:mm:ss) ");
                    LogSystemMessage(now + message); 
                    break;
                case LogType.CommandUsage:
                    now = DateTime.Now.ToString("(HH:mm:ss) ");
                    WriteCommand(now + message); 
                    break;
                default:
                    WriteLine(message);
                    break;
            }
        }
        
        void InitServerTask(SchedulerTask task) {
            Server.Start();
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
            UpdatePlayers();
        }
        
        void Player_PlayerDisconnect(Player p, string reason) {
            UpdatePlayers();
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
                Text = ServerConfig.Name + " - " + Server.SoftwareNameVersioned;
                UpdateNotifyIconText();
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
        public void UpdateClientList() {
            if (InvokeRequired) { Invoke(new VoidDelegate(UpdateClientList)); return; }
            
            UpdateNotifyIconText();
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
            notifyIcon.ShowBalloonTip(3000, ServerConfig.Name, message, icon);
        }

        public void UpdateMapList() {
            if (InvokeRequired) {
                Invoke(new VoidDelegate(UpdateMapList)); return;
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
                notifyIcon.Dispose();
            }
            
            if (Server.shuttingDown || MessageBox.Show("Really shutdown the server? All players will be disconnected!", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                if (!Server.shuttingDown) MCGalaxy.Gui.App.ExitProgram(false);
                notifyIcon.Dispose();
            } else {
                // Prevents form from closing when user clicks the X and then hits 'cancel'
                e.Cancel = true;
            }
        }

        void btnClose_Click(object sender, EventArgs e) { Close(); }

        void btnProperties_Click(object sender, EventArgs e) {
            if (!prevLoaded) { PropertyForm = new PropertyWindow(); prevLoaded = true; }
            PropertyForm.Show();
            if (!PropertyForm.Focused) PropertyForm.Focus();
        }

        public static bool prevLoaded = false;
        Form PropertyForm;

        void Window_Resize(object sender, EventArgs e) {
            ShowInTaskbar = WindowState != FormWindowState.Minimized;
        }

        void openConsole_Click(object sender, EventArgs e) {
            Show();
            BringToFront();
            WindowState = FormWindowState.Normal;
        }

        void shutdownServer_Click(object sender, EventArgs e) {
            Close();
        }      

       void tabs_Click(object sender, EventArgs e)  {
            try { UpdateUnloadedList(); }
            catch { }
            try { UpdatePlayers(); }
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
    }
}
