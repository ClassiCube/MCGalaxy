/*	
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

using System.Net;

namespace MCGalaxy.Gui
{
    public partial class Window : Form
    {
        // What is this???
        /*Regex regex = new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\." +
                                "([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");*/

        // for cross thread use
        delegate void StringCallback(string s);
        delegate void PlayerListCallback(List<Player> players);
        delegate void VoidDelegate();
        public static bool fileexists = false;
        bool mapgen = false;

        PlayerCollection pc = new PlayerCollection(new PlayerListView());
        LevelCollection lc = new LevelCollection(new LevelListView());

        //public static event EventHandler Minimize;
        public NotifyIcon notifyIcon1 = new NotifyIcon();
        //  public static bool Minimized = false;

        Level prpertiesoflvl;
        Player curPlayer;

        internal static Server s;

        readonly System.Timers.Timer UpdateListTimer = new System.Timers.Timer(10000);

        public Window()
        {
            InitializeComponent();
        }

        private void Window_Load(object sender, EventArgs e)
        {
            btnProperties.Enabled = false;
            //thisWindow = this;
            MaximizeBox = false;
            this.Text = "Starting MCGalaxy...";
            this.Show();
            this.BringToFront();
            WindowState = FormWindowState.Normal;
            Thread loadThread = new Thread(() =>
            {
                s = new Server();
                s.OnLog += WriteLine;
                s.OnCommand += newCommand;
                s.OnError += newError;
                s.OnSystem += newSystem;
                s.OnAdmin += WriteAdmin;
                s.OnOp += WriteOp;


                s.HeartBeatFail += HeartBeatFail;
                s.OnURLChange += UpdateUrl;
                s.OnPlayerListChange += UpdateClientList;
                s.OnSettingsUpdate += SettingsUpdate;
                s.Start();

                Player.PlayerConnect += Player_PlayerConnect;
                Player.PlayerDisconnect += Player_PlayerDisconnect;

                Level.LevelLoaded += Level_LevelLoaded;
                Level.LevelUnload += Level_LevelUnload;

                RunOnUiThread(() => btnProperties.Enabled = true);

            });
            loadThread.Name = "MCG_ServerSetup";
            loadThread.Start();


            notifyIcon1.Text = ("MCGalaxy Server: " + Server.name).Truncate(64);

            this.notifyIcon1.ContextMenuStrip = this.iconContext;
            this.notifyIcon1.Icon = this.Icon;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);

            if (File.Exists("Changelog.txt"))
            {
                txtChangelog.Text = "Changelog for " + Server.Version + ":";
                foreach (string line in File.ReadAllLines(("Changelog.txt")))
                {
                    txtChangelog.AppendText("\r\n           " + line);
                }
            }

            // Bind player list
            dgvPlayers.DataSource = pc;
            dgvPlayers.Font = new Font("Calibri", 8.25f);

            dgvMaps.DataSource = new LevelCollection(new LevelListView());
            dgvMaps.Font = new Font("Calibri", 8.25f);

            UpdateListTimer.Elapsed += delegate
            {
                try
                {
                    UpdateClientList(PlayerInfo.players);
                    UpdateMapList();
                }
                catch { } // needed for slower computers
                //Server.s.Log("Lists updated!");
            }; UpdateListTimer.Start();

        }

        public void RunOnUiThread(Action act)
        {
            var d = new VoidDelegate(() => Invoke(new VoidDelegate(act)));  //SOME ADVANCED STUFF RIGHT HERR
            d.Invoke();
        }
        void Player_PlayerConnect(Player p)
        {
            UpdatePlyersListBox();
        }
        void Player_PlayerDisconnect(Player p, string reason)
        {
            UpdatePlyersListBox();
        }
        void Level_LevelUnload(Level l)
        {
            RunOnUiThread(() =>
            {
                UpdateMapList();
                UpdatePlayerMapCombo();
                UpdateUnloadedList();

            });

        }
        void Level_LevelLoaded(Level l)
        {
            RunOnUiThread(() =>
            {
                UpdatePlayerMapCombo();
                UpdateUnloadedList();

            });
        }

        void SettingsUpdate()
        {
            if (Server.shuttingDown) return;
            if (txtLog.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(SettingsUpdate));
            }
            else
            {
                this.Text = Server.name + " - MCGalaxy " + Server.VersionString;
                notifyIcon1.Text = ("MCGalaxy Server: " + Server.name).Truncate(64);
            }
        }

        void HeartBeatFail()
        {
            WriteLine("Recent Heartbeat Failed");
        }

        void newError(string message)
        {
            try
            {
                if (txtErrors.InvokeRequired)
                {
                    this.Invoke(new LogDelegate(newError), new object[] { message });
                }
                else
                {
                    txtErrors.AppendText(Environment.NewLine + message);
                }
            }
            catch { }
        }
        void newSystem(string message)
        {
            try
            {
                if (txtSystem.InvokeRequired)
                {
                    this.Invoke(new LogDelegate(newSystem), new object[] { message });
                }
                else
                {
                    txtSystem.AppendText(Environment.NewLine + message);
                }
            }
            catch { }
        }

        delegate void LogDelegate(string message);

        /// <summary> Does the same as Console.WriteLine() only in the form </summary>
        /// <param name="s">The line to write</param>
        public void WriteLine(string s)
        {
            if (Server.shuttingDown) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new LogDelegate(WriteLine), new object[] { s });
            }
            else
            {

                string cleaned = s;
                //Begin substring of crappy date stamp

                int substr = s.IndexOf(')');
                if (substr == -1)
                {
                    cleaned = s;
                }
                else
                {
                    cleaned = s.Substring(substr + 1);
                }

                //end substring

                txtLog.AppendLog(cleaned + Environment.NewLine);
                // ColorBoxes(txtLog);
            }
        }


        public void WriteOp(string s)
        {
            if (Server.shuttingDown) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new LogDelegate(WriteOp), new object[] { s });
            }
            else
            {
                //txtLog.AppendText(Environment.NewLine + s);
                txtOpLog.AppendTextAndScroll(s);
            }
        }

        public void WriteAdmin(string s)
        {
            if (Server.shuttingDown) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new LogDelegate(WriteAdmin), new object[] { s });
            }
            else
            {
                //txtLog.AppendText(Environment.NewLine + s);
                txtAdminLog.AppendTextAndScroll(s);
            }
        }

        /// <summary> Updates the list of client names in the window </summary>
        /// <param name="players">The list of players to add</param>
        public void UpdateClientList(List<Player> players)
        {

            if (InvokeRequired)
            {
                Invoke(new PlayerListCallback(UpdateClientList), players);
            }
            else
            {

                if (dgvPlayers.DataSource == null)
                    dgvPlayers.DataSource = pc;

                // Try to keep the same selection on update
                string selected = null;
                if (pc.Count > 0 && dgvPlayers.SelectedRows.Count > 0)
                {
                    selected = (from DataGridViewRow row in dgvPlayers.Rows where row.Selected select pc[row.Index]).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();

                pc = new PlayerCollection(new PlayerListView());
                PlayerInfo.players.ForEach(p => pc.Add(p));

                //dgvPlayers.Invalidate();
                dgvPlayers.DataSource = pc;
                // Reselect player
                if (selected != null)
                {
                    foreach (Player t in PlayerInfo.players)
                        for (int j = 0; j < dgvPlayers.Rows.Count; j++)
                            if (Equals(dgvPlayers.Rows[j].Cells[0].Value, selected))
                                dgvPlayers.Rows[j].Selected = true;
                }

                dgvPlayers.Refresh();
                //dgvPlayers.ResumeLayout();
            }

        }

        public void PopupNotify(string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            notifyIcon1.ShowBalloonTip(3000, Server.name, message, icon);
        }

        public delegate void UpdateList();

        public void UpdateMapList()
        {
            if (InvokeRequired)
                Invoke(new UpdateList(UpdateMapList));
            else
            {

                if (dgvMaps.DataSource == null)
                    dgvMaps.DataSource = lc;

                // Try to keep the same selection on update
                string selected = null;
                if (lc.Count > 0 && dgvMaps.SelectedRows.Count > 0)
                {
                    selected = (from DataGridViewRow row in dgvMaps.Rows where row.Selected select lc[row.Index]).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();
                lc.Clear();
                string selectedLvl = null;
                if (lbMap_Lded.SelectedItem != null)
                    selectedLvl = lbMap_Lded.SelectedItem.ToString();
                
                lbMap_Lded.Items.Clear();
                //lc = new LevelCollection(new LevelListView());
                Server.levels.ForEach(l => lc.Add(l));
                Server.levels.ForEach(l => lbMap_Lded.Items.Add(l.name));
                
                if (selectedLvl != null) {
                    int index = lbMap_Lded.Items.IndexOf(selectedLvl);
                    lbMap_Lded.SelectedIndex = index;
                } else {
                    lbMap_Lded.SelectedIndex = -1;
                }
                UpdateSelectedMap(null, null);

                //dgvPlayers.Invalidate();
                dgvMaps.DataSource = null;
                dgvMaps.DataSource = lc;
                // Reselect map
                if (selected != null)
                {
                    foreach (DataGridViewRow row in Server.levels.SelectMany(l => dgvMaps.Rows.Cast<DataGridViewRow>().Where(row => (string)row.Cells[0].Value == selected)))
                        row.Selected = true;
                }

                dgvMaps.Refresh();
                //dgvPlayers.ResumeLayout();

                // Update the data source and control
                //dgvPlayers.SuspendLayout();
            }
        }

        /// <summary> Places the server's URL at the top of the window </summary>
        /// <param name="s">The URL to display</param>
        public void UpdateUrl(string s)
        {
            if (this.InvokeRequired)
            {
                StringCallback d = UpdateUrl;
                this.Invoke(d, new object[] { s });
            }
            else
                txtUrl.Text = s;
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                MCGalaxy.Gui.App.ExitProgram(false);
            }
            if (Server.shuttingDown || MessageBox.Show("Really Shutdown the Server? All Connections will break!", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (!Server.shuttingDown)
                {
                    MCGalaxy.Gui.App.ExitProgram(false);
                }
            }
            else
            {
                // Prevents form from closing when user clicks the X and then hits 'cancel'
                e.Cancel = true;
            }
        }

        void txtInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            Handlers.HandleChat(txtInput.Text, WriteLine);
            txtInput.Clear();
        }

        void txtCommands_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            Handlers.HandleCommand(txtCommands.Text, newCommand);
            txtCommands.Clear();
        }

        void btnClose_Click_1(object sender, EventArgs e) { Close(); }

        public void newCommand(string p)
        {
            if (txtCommandsUsed.InvokeRequired)
            {
                LogDelegate d = newCommand;
                this.Invoke(d, new object[] { p });
            }
            else
            {
                txtCommandsUsed.AppendTextAndScroll(p);
            }
        }

        private void btnProperties_Click_1(object sender, EventArgs e)
        {
            if (!prevLoaded) { PropertyForm = new PropertyWindow(); prevLoaded = true; }
            PropertyForm.Show();
            if (!PropertyForm.Focused)
            {
                PropertyForm.Focus();
            }
        }

        public static bool prevLoaded = false;
        Form PropertyForm;

        private void Window_Resize(object sender, EventArgs e)
        {
            this.ShowInTaskbar = (this.WindowState != FormWindowState.Minimized);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.BringToFront();
            WindowState = FormWindowState.Normal;
        }

        private void openConsole_Click(object sender, EventArgs e)
        {
            this.Show();
            this.BringToFront();
            WindowState = FormWindowState.Normal;
        }

        private void shutdownServer_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Player GetSelectedPlayer()
        {

            if (this.dgvPlayers.SelectedRows.Count <= 0)
                return null;

            return (Player)(this.dgvPlayers.SelectedRows[0].DataBoundItem);
        }

        private Level GetSelectedLevel()
        {

            if (this.dgvMaps.SelectedRows.Count <= 0)
                return null;

            return (Level)(this.dgvMaps.SelectedRows[0].DataBoundItem);
        }

        private void clonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerselect("clones");
        }

        private void voiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerselect("voice");
        }

        private void whoisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerselect("whois");
        }

        private void kickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerselect("kick", " You have been kicked by the console.");
        }


        private void banToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerselect("ban");
        }

        private void playerselect(string com)
        {
            if (GetSelectedPlayer() != null)
                Command.all.Find(com).Use(null, GetSelectedPlayer().name);
        }
        private void playerselect(string com, string args)
        {
            if (GetSelectedPlayer() != null)
                Command.all.Find(com).Use(null, GetSelectedPlayer().name + args);
        }

        private void finiteModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " finite");
        }

        private void animalAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " ai");
        }

        private void edgeWaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " edge");
        }

        private void growingGrassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " grass");
        }

        private void survivalDeathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " death");
        }

        private void killerBlocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " killer");
        }

        private void rPChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " chat");
        }

        private void levelcommand(string com)
        {
            if (GetSelectedLevel() != null)
                Command.all.Find(com).Use(null, GetSelectedLevel().name);
        }

        private void levelcommand(string com, string args)
        {
            if (GetSelectedLevel() != null)
                Command.all.Find(com).Use(null, GetSelectedLevel().name + args);
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            try { UpdateUnloadedList(); }
            catch { }
            try { UpdatePlyersListBox(); }
            catch { }
            try
            {
                if (LogsTxtBox.Text == "")
                {
                    dateTimePicker1.Value = DateTime.Now;
                }
            }
            catch { }
            foreach (TextBox txtBox in (from TabPage tP in tabControl1.TabPages from Control ctrl in tP.Controls select ctrl).OfType<TextBox>())
            {
                txtBox.Update();
            }
            tabControl1.Update();
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to restart?", "Restart", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                MCGalaxy.Gui.App.ExitProgram(true);
            }

        }

        private void restartServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart_Click(sender, e);
        }

        private void DatePicker1_ValueChanged(object sender, EventArgs e)
        {
            string dayofmonth = dateTimePicker1.Value.Day.ToString().PadLeft(2, '0');
            string year = dateTimePicker1.Value.Year.ToString();
            string month = dateTimePicker1.Value.Month.ToString().PadLeft(2, '0');

            string ymd = year + "-" + month + "-" + dayofmonth;
            string filename = ymd + ".txt";

            if (!File.Exists(Path.Combine("logs/", filename)))
            {
                //MessageBox.Show("Sorry, the log for " + ymd + " doesn't exist, please select another one");
                LogsTxtBox.Text = "No logs found for: " + ymd;
            }
            else
            {
                LogsTxtBox.Text = null;
                LogsTxtBox.Text = File.ReadAllText(Path.Combine("logs/", filename));
            }

        }

        private void txtUrl_DoubleClick(object sender, EventArgs e)
        {
            txtUrl.SelectAll();
        }

        private void dgvPlayers_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts &= ~DataGridViewPaintParts.Focus;
        }

        private void promoteToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("promote");
        }

        private void demoteToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("demote");
        }

        #region Tabs
        #region playersTab
        private void LoadPLayerTabDetails(object sender, EventArgs e)
        {
            Player p = PlayerInfo.Find(PlyersListBox.Text);
            if (p != null)
            {
                PlayersTextBox.AppendTextAndScroll("==" + p.name + "==");
                { //Top Stuff
                    curPlayer = p;
                    NameTxtPlayersTab.Text = p.name;
                    MapTxt.Text = p.level.name;
                    RankTxt.Text = p.group.name;
                    StatusTxt.Text = Player.CheckPlayerStatus(p);
                    IPtxt.Text = p.ip;
                    DeathsTxt.Text = p.fallCount.ToString();
                    Blockstxt.Text = p.overallBlocks.ToString();
                    TimesLoggedInTxt.Text = p.totalLogins.ToString();
                    LoggedinForTxt.Text = Convert.ToDateTime(DateTime.Now.Subtract(p.timeLogged).ToString()).ToString("HH:mm:ss");
                    Kickstxt.Text = p.totalKicked.ToString();
                }
                { //Check buttons
                    if (p.joker) { JokerBt.Text = "UnJoker"; } else { JokerBt.Text = "Joker"; }
                    if (p.frozen) { FreezeBt.Text = "UnFreeze"; } else { FreezeBt.Text = "Freeze"; }
                    if (p.muted) { MuteBt.Text = "UnMute"; } else { MuteBt.Text = "Mute"; }
                    if (p.voice) { VoiceBt.Text = "UnVoice"; } else { VoiceBt.Text = "Voice"; }
                    if (p.hidden) { HideBt.Text = "UnHide"; } else { HideBt.Text = "Hide"; }
                    if (p.jailed) { JailBt.Text = "UnJail"; } else { JailBt.Text = "Jail"; }
                }
                { //Text box stuff
                    LoginTxt.Text = PlayerDB.GetLoginMessage(p);
                    LogoutTxt.Text = PlayerDB.GetLogoutMessage(p);
                    TitleTxt.Text = p.title;
                    ColorCombo.SelectedText = Colors.Name(p.color).Capitalize();
                    //Map
                    {
                        try
                        {
                            try
                            {
                                UpdatePlayerMapCombo();
                            }
                            catch { }
                            foreach (Object obj in MapCombo.Items)
                            {
                                if (LevelInfo.Find(obj.ToString()) != null)
                                {
                                    if (p.level == LevelInfo.Find(obj.ToString()))
                                    {
                                        MapCombo.SelectedItem = obj;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

            }
        }

        public void UpdatePlayerMapCombo()
        {
            int selected = MapCombo.SelectedIndex;
            MapCombo.Items.Clear();
            foreach (Level level in Server.levels)
            {
                MapCombo.Items.Add(level.name);
            }
            MapCombo.SelectedIndex = selected;
        }

        private void LoginBt_Click(object sender, EventArgs e)
        {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer))
            {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            CP437Writer.WriteAllText("text/login/" + curPlayer.name + ".txt", null);
            CP437Writer.WriteAllText("text/login/" + curPlayer.name + ".txt", LoginTxt.Text);
            PlayersTextBox.AppendTextAndScroll("The login message has been saved!");
        }

        private void LogoutBt_Click(object sender, EventArgs e)
        {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer))
            {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            CP437Writer.WriteAllText("text/logout/" + curPlayer.name + ".txt", null);
            CP437Writer.WriteAllText("text/logout/" + curPlayer.name + ".txt", LogoutTxt.Text);
            PlayersTextBox.AppendTextAndScroll("The logout message has been saved!");
        }

        private void TitleBt_Click(object sender, EventArgs e)
        {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer))
            {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            if (TitleTxt.Text.Length > 17) { PlayersTextBox.AppendTextAndScroll("Title must be under 17 letters."); return; }
            curPlayer.prefix = "[" + TitleTxt.Text + "]";
            PlayersTextBox.AppendTextAndScroll("The title has been saved");
        }

        private void ColorBt_Click(object sender, EventArgs e)
        {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer))
            {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            curPlayer.color = Colors.Parse(ColorCombo.Text);
            PlayersTextBox.AppendTextAndScroll("Set color to " + ColorCombo.Text);
        }

        private void MapBt_Click(object sender, EventArgs e)
        {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer))
            {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            if (MapCombo.Text.ToLower() == curPlayer.level.name.ToLower())
            {
                PlayersTextBox.AppendTextAndScroll("The player is already on that map");
                return;
            }
            if (!Server.levels.Contains(LevelInfo.Find(MapCombo.Text)))
            {
                PlayersTextBox.AppendTextAndScroll("That map doesn't exist!!");
                return;
            }
            else
            {
                try
                {
                    Command.all.Find("goto").Use(curPlayer, MapCombo.Text);
                    PlayersTextBox.AppendTextAndScroll("Sent player to " + MapCombo.Text);
                }
                catch
                {
                    PlayersTextBox.AppendTextAndScroll("Something went wrong!!");
                    return;
                }
            }
        }

        private void UndoBt_Click(object sender, EventArgs e)
        {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer))
            {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            if (UndoTxt.Text.Trim() == "")
            {
                PlayersTextBox.AppendTextAndScroll("You didn't specify a time");
                return;
            }
            else
            {
                try
                {
                    Command.core.Find("undo").Use(null, curPlayer.name + " " + UndoTxt.Text);
                    PlayersTextBox.AppendTextAndScroll("Undid player for " + UndoTxt.Text + " Seconds");
                }
                catch
                {
                    PlayersTextBox.AppendTextAndScroll("Something went wrong!!");
                    return;
                }
            }
        }

        private void MessageBt_Click(object sender, EventArgs e)
        {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer))
            {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Player.SendMessage(curPlayer, "<CONSOLE> " + PLayersMessageTxt.Text);
            PlayersTextBox.AppendTextAndScroll("Sent player message '<CONSOLE> " + PLayersMessageTxt.Text + "'");
            PLayersMessageTxt.Text = "";
        }

        private void ImpersonateORSendCmdBt_Click(object sender, EventArgs e)
        {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            try {
                if (ImpersonateORSendCmdTxt.Text.StartsWith("/")) {
                	string[] args = ImpersonateORSendCmdTxt.Text.Trim().SplitSpaces(2);
                    Command cmd = Command.all.Find(args[0].Replace("/", ""));
                    if (cmd == null) {
                        PlayersTextBox.AppendTextAndScroll("That isn't a command!!"); return;
                    }
                    
                    cmd.Use(curPlayer, args.Length > 1 ? args[1] : "");
                    if (args.Length > 1) {
                    	PlayersTextBox.AppendTextAndScroll("Used command '" + args[0] + "' with parameters '" + args[1] + "' as player");
                    } else {
                        PlayersTextBox.AppendTextAndScroll("Used command '" + args[0] + "' with no parameters as player");
                    }
                } else {
                    Command.all.Find("impersonate").Use(null, curPlayer.name + " " + ImpersonateORSendCmdTxt.Text);
                    PlayersTextBox.AppendTextAndScroll("Sent Message '" + ImpersonateORSendCmdTxt.Text + "' as player");
                }
                ImpersonateORSendCmdTxt.Text = "";
            } catch {
                PlayersTextBox.AppendTextAndScroll("Something went wrong");
            }
        }

        void PromoteBt_Click(object sender, EventArgs e) { DoSimple("promote", "Promoted"); }

        void DemoteBt_Click(object sender, EventArgs e) { DoSimple("demote", "Demoted"); }

        void HideBt_Click(object sender, EventArgs e) {
            DoToggle("ohide", HideBt, "Hide", p => p.hidden, "Hid");
        }

        void SlapBt_Click(object sender, EventArgs e) { DoSimple("slap", "Slapped"); }

        void JokerBt_Click(object sender, EventArgs e) {
            DoToggle("joker", JokerBt, "Joker", p => p.joker, "Jokered");
        }

        void FreezeBt_Click(object sender, EventArgs e) {
            DoToggle("freeze", FreezeBt, "Freeze", p => p.frozen, "Froze");
        }

        void MuteBt_Click(object sender, EventArgs e) {
            DoToggle("mute", MuteBt, "Mute", p => p.muted, "Muted");
        }

        void VoiceBt_Click(object sender, EventArgs e) {
            DoToggle("voice", VoiceBt, "Voice", p => p.voice, "Voiced");
        }

        void KillBt_Click(object sender, EventArgs e) { DoSimple("kill", "Killed"); }

        void JailBt_Click(object sender, EventArgs e) {
            DoToggle("jail", JailBt, "Jail", p => p.jailed, "Jailed");
        }

        void WarnBt_Click(object sender, EventArgs e) { DoSimple("warn", "Warned"); }

        void KickBt_Click(object sender, EventArgs e) { DoSimple("kick", "Kicked"); }

        void BanBt_Click(object sender, EventArgs e) { DoSimple("ban", "Banned"); }

        void IPBanBt_Click(object sender, EventArgs e) { DoSimple("banip", "IP-Banned"); }
        
        void DoSimple(string cmdName, string action) {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            Command.all.Find(cmdName).Use(null, curPlayer.name);
            PlayersTextBox.AppendTextAndScroll(action + " player");
        }
        
        void DoToggle(string cmdName, Button target, string targetDesc,
                      Predicate<Player> getter, string action) {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            Command.all.Find(cmdName).Use(null, curPlayer.name);
            
            if (getter(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll(action + " player");
                target.Text = "Un" + targetDesc;
            } else {
                PlayersTextBox.AppendTextAndScroll("Un" + action + " player");
                target.Text = targetDesc;
            }
        }

        private void SendRulesTxt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            Command.all.Find("rules").Use(curPlayer, "");
            PlayersTextBox.AppendTextAndScroll("Sent rules to player");
        }

        private void SpawnBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !PlayerInfo.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            Player p = curPlayer;
            ushort x = (ushort)((0.5 + p.level.spawnx) * 32);
            ushort y = (ushort)((1 + p.level.spawny) * 32);
            ushort z = (ushort)((0.5 + p.level.spawnz) * 32);
            p.SendPos(0xFF, x, y, z, p.level.rotx, p.level.roty);
            PlayersTextBox.AppendTextAndScroll("Sent player to spawn");
        }

        public void UpdatePlyersListBox() {
            RunOnUiThread(
                delegate
                {

                    PlyersListBox.Items.Clear();
                    foreach (Player p in PlayerInfo.players)
                    {
                        PlyersListBox.Items.Add(p.name);
                    }
                });

        }

        private void PlyersListBox_Click(object sender, EventArgs e)
        {
            LoadPLayerTabDetails(sender, e);
        }

        private void ImpersonateORSendCmdTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ImpersonateORSendCmdBt_Click(sender, e);
            }
        }

        private void LoginTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginBt_Click(sender, e);
            }
        }

        private void LogoutTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LogoutBt_Click(sender, e);
            }
        }

        private void TitleTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TitleBt_Click(sender, e);
            }
        }

        private void UndoTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UndoBt_Click(sender, e);
            }
        }

        private void PLayersMessageTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MessageBt_Click(sender, e);
            }
        }

        private void ColorCombo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ColorBt_Click(sender, e);
            }
        }

        private void MapCombo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MapBt_Click(sender, e);
            }
        }
        #endregion

        #endregion

        private void txtOpInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtOpInput.Text == null || txtOpInput.Text.Trim() == "") { return; }
                string optext = txtOpInput.Text.Trim();
                string opnewtext = optext;
                MCGalaxy.Chat.GlobalMessageOps("To Ops &f-" + Server.DefaultColor + "Console [&a" + Server.ZallState + Server.DefaultColor + "]&f- " + opnewtext);
                Server.s.OpLog("(OPs): Console: " + opnewtext);
                txtOpInput.Clear();
            }

        }

        private void txtAdminInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtAdminInput.Text == null || txtAdminInput.Text.Trim() == "") { return; }
                string admintext = txtAdminInput.Text.Trim();
                string adminnewtext = admintext;
                MCGalaxy.Chat.GlobalMessageAdmins("To Admins &f-" + Server.DefaultColor + "Console [&a" + Server.ZallState + Server.DefaultColor + "]&f- " + adminnewtext);
                Server.s.AdminLog("(Admins): Console: " + adminnewtext);
                txtAdminInput.Clear();
            }
        }

        private void button_saveall_Click(object sender, EventArgs e)
        {
            Command.all.Find("save").Use(null, "all");
        }

        private void killphysics_button_Click(object sender, EventArgs e)
        {
            Command.all.Find("killphysics").Use(null, "");
            try { UpdateMapList(); }
            catch { }
        }

        private void Unloadempty_button_Click(object sender, EventArgs e)
        {
            Command.all.Find("unload").Use(null, "empty");
            try { UpdateMapList(); }
            catch { }
        }

        private void dgvMaps_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void loadOngotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " loadongoto");
        }

        private void instantBuildingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " instant");
        }

        private void autpPhysicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " restartphysics");
        }

        private void gunsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("allowguns");
        }

        private void unloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            levelcommand("map", " unload");
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map");
            levelcommand("mapinfo");
        }

        private void actiondToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void moveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("moveall");
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            levelcommand("physics", " 0");
        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            levelcommand("physics", " 1");
        }

        private void toolStripMenuItem4_Click_1(object sender, EventArgs e)
        {
            levelcommand("physics", " 2");
        }

        private void toolStripMenuItem5_Click_1(object sender, EventArgs e)
        {
            levelcommand("physics", " 3");
        }

        private void toolStripMenuItem6_Click_1(object sender, EventArgs e)
        {
            levelcommand("physics", " 4");
        }

        private void toolStripMenuItem7_Click_1(object sender, EventArgs e)
        {
            levelcommand("physics", " 5");
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            levelcommand("save");
        }

        private void unloadToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            levelcommand("unload");
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("reload");
        }

        private void leafDecayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " leafdecay");
        }

        private void randomFlowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " randomflow");
        }

        private void treeGrowingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            levelcommand("map", " growtrees");
        }

        #region Colored Reader Context Menu

        private void nightModeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Changing to and from night mode will clear your logs. Do you still want to change?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            txtLog.NightMode = nightModeToolStripMenuItem.Checked;
            nightModeToolStripMenuItem.Checked = !nightModeToolStripMenuItem.Checked;
        }

        private void colorsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            txtLog.Colorize = !colorsToolStripMenuItem.Checked;
            colorsToolStripMenuItem.Checked = !colorsToolStripMenuItem.Checked;

        }

        private void dateStampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtLog.DateStamp = !dateStampToolStripMenuItem.Checked;
            dateStampToolStripMenuItem.Checked = !dateStampToolStripMenuItem.Checked;
        }

        private void autoScrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtLog.AutoScroll = !autoScrollToolStripMenuItem.Checked;
            autoScrollToolStripMenuItem.Checked = !autoScrollToolStripMenuItem.Checked;
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtLog.SelectedText))
                return;

            Clipboard.SetText(txtLog.SelectedText, TextDataFormat.Text);
        }
        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtLog.Text, TextDataFormat.Text);
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear logs?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                txtLog.Clear();
            }
        }
        #endregion

        #region Map tab
        
        void MapGenClick(object sender, EventArgs e) {
            if (mapgen) { MessageBox.Show("A map is already being generated."); return; }
            string name, x, y, z, type, seed;

            try { name = txtMap_Name.Text.ToLower(); }
            catch { name = ""; }
            if (String.IsNullOrEmpty(name)) { MessageBox.Show("Map name cannot be blank."); return; }
            try { x = cmbMap_X.SelectedItem.ToString(); }
            catch { x = ""; }
            if (String.IsNullOrEmpty(x)) { MessageBox.Show("Map width cannot be blank."); return; }
            
            try { y = cmbMap_Y.SelectedItem.ToString(); }
            catch { y = ""; }
            if (String.IsNullOrEmpty(y)) { MessageBox.Show("Map height cannot be blank."); return; }
            
            try { z = cmbMap_Z.SelectedItem.ToString(); }
            catch { z = ""; }
            if (String.IsNullOrEmpty(z)) { MessageBox.Show("Map length cannot be blank."); return; }
            
            try { type = cmbMap_Type.SelectedItem.ToString().ToLower(); }
            catch { type = ""; }
            if (String.IsNullOrEmpty(type)) { MessageBox.Show("Map type cannot be blank."); return; }
            
            try { seed = txtMap_Seed.Text; }
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
                Command.all.Find("load").Use(null, lbMap_Unld.SelectedItem.ToString());
            } catch { 
            }
        	UpdateUnloadedList();
        	UpdateMapList();
        }
        
        string last = null;
        void UpdateSelectedMap(object sender, EventArgs e) {
            if (lbMap_Lded.SelectedItem == null) {
        		if (pgMaps.SelectedObject == null) return;
                pgMaps.SelectedObject = null; last = null;
                gbMap_Props.Text = "Properties for (none selected)"; return;
            }
            
            string name = lbMap_Lded.SelectedItem.ToString();
            Level lvl = LevelInfo.FindExact(name);
            if (lvl == null) {
            	if (pgMaps.SelectedObject == null) return;
                pgMaps.SelectedObject = null; last = null;
                gbMap_Props.Text = "Properties for (none selected)"; return;
            }
            
            if (name == last) return;
            last = name;
            LevelSettings settings = new LevelSettings(lvl);
            pgMaps.SelectedObject = settings;
            gbMap_Props.Text = "Properties for " + name;
        }
        
        public void UpdateUnloadedList()
        {
            RunOnUiThread(() =>
            {
        	    string selectedLvl = null;
                if (lbMap_Unld.SelectedItem != null)
                    selectedLvl = lbMap_Unld.SelectedItem.ToString();
                
                lbMap_Unld.Items.Clear();
                string[] files = Directory.GetFiles("levels", "*.lvl");
                foreach (string file in files) {
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (LevelInfo.FindExact(name) == null)
                        lbMap_Unld.Items.Add(name);
                }
                
                if (selectedLvl != null) {
                    int index = lbMap_Unld.Items.IndexOf(selectedLvl);
                    lbMap_Unld.SelectedIndex = index;
                } else {
                    lbMap_Unld.SelectedIndex = -1;
                }
            });
        }
        #endregion
    }
}

