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
            notifyIcon1.ContextMenuStrip = this.iconContext;
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Visible = true;
            notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon1_MouseClick);

            if (File.Exists("Changelog.txt")) {
                logs_txtChangelog.Text = "Changelog for " + Server.Version + ":";
                foreach (string line in File.ReadAllLines("Changelog.txt")) {
                    logs_txtChangelog.AppendText("\r\n           " + line);
                }
            }

            // Bind player list
            main_Players.DataSource = pc;
            main_Players.Font = new Font("Calibri", 8.25f);

            main_Maps.DataSource = new LevelCollection(); // Otherwise "-1 does not have a value" exception when clicking a row
            main_Maps.Font = new Font("Calibri", 8.25f);

            UpdateListTimer.Elapsed += delegate {
                try {
                    UpdateClientList(Player.players);
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
            s.OnError += newError;
            s.OnSystem += newSystem;

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

        void newError(string message) {
            try {
                if (logs_txtError.InvokeRequired) {
                    Invoke(new LogDelegate(newError), new object[] { message });
                } else {
                    logs_txtError.AppendText(Environment.NewLine + message);
                }
            } catch { 
            }
        }
        
        void newSystem(string message) {
            try {
                if (logs_txtSystem.InvokeRequired) {
                    Invoke(new LogDelegate(newSystem), new object[] { message });
                } else {
                    logs_txtSystem.AppendText(Environment.NewLine + message);
                }
            } catch { 
            }
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
        public void UpdateClientList(List<Player> players) {
            if (InvokeRequired) {
                Invoke(new PlayerListCallback(UpdateClientList), players);
            } else {
                if (main_Players.DataSource == null)
                    main_Players.DataSource = pc;

                // Try to keep the same selection on update
                string selected = null;
                if (pc.Count > 0 && main_Players.SelectedRows.Count > 0) {
                    selected = (from DataGridViewRow row in main_Players.Rows where row.Selected select pc[row.Index]).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();

                pc = new PlayerCollection();
                Player.players.ForEach(p => pc.Add(p));

                //dgvPlayers.Invalidate();
                main_Players.DataSource = pc;
                // Reselect player
                if (selected != null)
                {
                    foreach (Player t in Player.players)
                        for (int j = 0; j < main_Players.Rows.Count; j++)
                            if (Equals(main_Players.Rows[j].Cells[0].Value, selected))
                                main_Players.Rows[j].Selected = true;
                }

                main_Players.Refresh();
                //dgvPlayers.ResumeLayout();
            }

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

        void txtInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;
            e.SuppressKeyPress = true;            
            string text = main_txtInput.Text;
            if (text.Length == 0) return;
            
            if (text[0] == '/' && text.Length > 1 && text[1] == '/') {
                Handlers.HandleChat(text.Substring(1), WriteLine);
            } else if (text[0] == '/') {
                Handlers.HandleCommand(text.Substring(1), WriteCommand);
            } else {
                Handlers.HandleChat(text, WriteLine);
            }
            main_txtInput.Clear();
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
            foreach (TextBox txtBox in (from TabPage tP in tabControl1.TabPages from Control ctrl in tP.Controls select ctrl).OfType<TextBox>())
            {
                txtBox.Update();
            }
            tabControl1.Update();
        }

        void Restart_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to restart?", "Restart", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                MCGalaxy.Gui.App.ExitProgram(true);
            }
        }

        void restartServerToolStripMenuItem_Click(object sender, EventArgs e) {
            Restart_Click(sender, e);
        }

        void DatePicker1_ValueChanged(object sender, EventArgs e) {
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

        void txtUrl_DoubleClick(object sender, EventArgs e) {
            main_txtUrl.SelectAll();
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

        #region Tabs
        #region playersTab
        void LoadPLayerTabDetails(object sender, EventArgs e) {
            Player p = PlayerInfo.Find(PlyersListBox.Text);
            if (p == null) return;
            PlayersTextBox.AppendTextAndScroll("==" + p.name + "==");
            //Top Stuff
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
            
            //Check buttons
            if (p.joker) { pl_btnJoker.Text = "UnJoker"; } else { pl_btnJoker.Text = "Joker"; }
            if (p.frozen) { pl_btnFreeze.Text = "UnFreeze"; } else { pl_btnFreeze.Text = "Freeze"; }
            if (p.muted) { pl_btnMute.Text = "UnMute"; } else { pl_btnMute.Text = "Mute"; }
            if (p.voice) { pl_btnVoice.Text = "UnVoice"; } else { pl_btnVoice.Text = "Voice"; }
            if (p.hidden) { pl_btnHide.Text = "UnHide"; } else { pl_btnHide.Text = "Hide"; }
            if (p.jailed) { pl_btnJail.Text = "UnJail"; } else { pl_btnJail.Text = "Jail"; }
            
            //Text box stuff
            pl_txtLogin.Text = PlayerDB.GetLoginMessage(p);
            pl_txtLogout.Text = PlayerDB.GetLogoutMessage(p);
            pl_txtTitle.Text = p.title;
            pl_cmbColor.SelectedText = Colors.Name(p.color).Capitalize();
            
            //Map
            try {
                try {
                    UpdatePlayerMapCombo();
                } catch { }
                
                foreach (Object obj in pl_cmbMap.Items) {
                    if (LevelInfo.Find(obj.ToString()) == null) continue;
                    
                    if (p.level == LevelInfo.Find(obj.ToString()))
                        pl_cmbMap.SelectedItem = obj;
                }
            } catch { }
        }

        void UpdatePlayerMapCombo() {
            int selected = pl_cmbMap.SelectedIndex;
            pl_cmbMap.Items.Clear();
            foreach (Level level in Server.levels) {
                pl_cmbMap.Items.Add(level.name);
            }
            pl_cmbMap.SelectedIndex = selected;
        }

        void LoginBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            CP437Writer.WriteAllText("text/login/" + curPlayer.name + ".txt", pl_txtLogin.Text);
            PlayersTextBox.AppendTextAndScroll("The login message has been saved!");
        }

        void LogoutBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            CP437Writer.WriteAllText("text/logout/" + curPlayer.name + ".txt", pl_txtLogout.Text);
            PlayersTextBox.AppendTextAndScroll("The logout message has been saved!");
        }

        void TitleBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            if (pl_txtTitle.Text.Length > 17) { PlayersTextBox.AppendTextAndScroll("Title must be under 17 letters."); return; }
            curPlayer.prefix = "[" + pl_txtTitle.Text + "]";
            PlayersTextBox.AppendTextAndScroll("The title has been saved");
        }

        void ColorBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            curPlayer.color = Colors.Parse(pl_cmbColor.Text);
            PlayersTextBox.AppendTextAndScroll("Set color to " + pl_cmbColor.Text);
        }

        void MapBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            if (pl_cmbMap.Text.ToLower() == curPlayer.level.name.ToLower()) {
                PlayersTextBox.AppendTextAndScroll("The player is already on that map"); return;
            }
            if (!Server.levels.Contains(LevelInfo.Find(pl_cmbMap.Text))) {
                PlayersTextBox.AppendTextAndScroll("That map doesn't exist!!"); return;
            }

            try {
                Command.all.Find("goto").Use(curPlayer, pl_cmbMap.Text);
                PlayersTextBox.AppendTextAndScroll("Sent player to " + pl_cmbMap.Text);
            } catch {
                PlayersTextBox.AppendTextAndScroll("Something went wrong!!");
            }
        }

        void UndoBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            if (pl_txtUndo.Text.Trim() == "")  {
                PlayersTextBox.AppendTextAndScroll("You didn't specify a time"); return;
            }

            try {
                Command.core.Find("undo").Use(null, curPlayer.name + " " + pl_txtUndo.Text);
                PlayersTextBox.AppendTextAndScroll("Undid player for " + pl_txtUndo.Text + " Seconds");
            } catch {
                PlayersTextBox.AppendTextAndScroll("Something went wrong!!");
            }
        }

        void MessageBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            Player.SendMessage(curPlayer, "<CONSOLE> " + pl_txtMessage.Text);
            PlayersTextBox.AppendTextAndScroll("Sent player message '<CONSOLE> " + pl_txtMessage.Text + "'");
            pl_txtMessage.Text = "";
        }

        void ImpersonateORSendCmdBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            try {
                if (pl_txtImpersonate.Text.StartsWith("/")) {
                    string[] args = pl_txtImpersonate.Text.Trim().SplitSpaces(2);
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
                    Command.all.Find("impersonate").Use(null, curPlayer.name + " " + pl_txtImpersonate.Text);
                    PlayersTextBox.AppendTextAndScroll("Sent Message '" + pl_txtImpersonate.Text + "' as player");
                }
                pl_txtImpersonate.Text = "";
            } catch {
                PlayersTextBox.AppendTextAndScroll("Something went wrong");
            }
        }

        void HideBt_Click(object sender, EventArgs e) {
            DoToggle("ohide", pl_btnHide, "Hide", p => p.hidden, "Hid");
        }
        void JokerBt_Click(object sender, EventArgs e) {
            DoToggle("joker", pl_btnJoker, "Joker", p => p.joker, "Jokered");
        }
        void FreezeBt_Click(object sender, EventArgs e) {
            DoToggle("freeze", pl_btnFreeze, "Freeze", p => p.frozen, "Froze");
        }
        void MuteBt_Click(object sender, EventArgs e) {
            DoToggle("mute", pl_btnMute, "Mute", p => p.muted, "Muted");
        }
        void VoiceBt_Click(object sender, EventArgs e) {
            DoToggle("voice", pl_btnVoice, "Voice", p => p.voice, "Voiced");
        }
        void JailBt_Click(object sender, EventArgs e) {
            DoToggle("jail", pl_btnJail, "Jail", p => p.jailed, "Jailed");
        }

        void PromoteBt_Click(object sender, EventArgs e) { DoCmd("rank", "Promoted", "+up "); }
        void DemoteBt_Click(object sender, EventArgs e) { DoCmd("rank", "Demoted", "-down "); }
        void SlapBt_Click(object sender, EventArgs e) { DoCmd("slap", "Slapped"); }
        void KillBt_Click(object sender, EventArgs e) { DoCmd("kill", "Killed"); }
        void WarnBt_Click(object sender, EventArgs e) { DoCmd("warn", "Warned"); }
        void KickBt_Click(object sender, EventArgs e) { DoCmd("kick", "Kicked"); }
        void BanBt_Click(object sender, EventArgs e) { DoCmd("ban", "Banned"); }
        void IPBanBt_Click(object sender, EventArgs e) { DoCmd("banip", "IP-Banned"); }
        
        void DoCmd(string cmdName, string action) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            Command.all.Find(cmdName).Use(null, curPlayer.name);
            PlayersTextBox.AppendTextAndScroll(action + " player");
        }
        
        void DoCmd(string cmdName, string action, string prefix) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            Command.all.Find(cmdName).Use(null, prefix + curPlayer.name);
            PlayersTextBox.AppendTextAndScroll(action + " player");
        }
        
        void DoToggle(string cmdName, Button target, string targetDesc,
                      Predicate<Player> getter, string action) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
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

        void SendRulesTxt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected"); return;
            }
            Command.all.Find("rules").Use(curPlayer, "");
            PlayersTextBox.AppendTextAndScroll("Sent rules to player");
        }

        void SpawnBt_Click(object sender, EventArgs e) {
            if (curPlayer == null || !Player.players.Contains(curPlayer)) {
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
                delegate {
                    PlyersListBox.Items.Clear();
                    foreach (Player p in Player.players)
                    {
                        PlyersListBox.Items.Add(p.name);
                    }
                });

        }

        void PlyersListBox_Click(object sender, EventArgs e) {
            LoadPLayerTabDetails(sender, e);
        }

        void ImpersonateORSendCmdTxt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) ImpersonateORSendCmdBt_Click(sender, e);
        }
        void LoginTxt_KeyDown(object sender, KeyEventArgs e){
            if (e.KeyCode == Keys.Enter) LoginBt_Click(sender, e);
        }
        void LogoutTxt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) LogoutBt_Click(sender, e);
        }
        void TitleTxt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) TitleBt_Click(sender, e);
        }
        void UndoTxt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) UndoBt_Click(sender, e);
        }
        void PLayersMessageTxt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) MessageBt_Click(sender, e);
        }
        void ColorCombo_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) ColorBt_Click(sender, e);
        }
        void MapCombo_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) MapBt_Click(sender, e);
        }
        #endregion

        #endregion

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

        void loadOngotoToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " loadongoto"); }
        void instantBuildingToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " instant"); }
        void autpPhysicsToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " restartphysics"); }
        void gunsToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("allowguns"); }
        void unloadToolStripMenuItem1_Click(object sender, EventArgs e) { LevelCmd("map", " unload"); }
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
        void leafDecayToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " leafdecay"); }
        void randomFlowToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " randomflow"); }
        void treeGrowingToolStripMenuItem_Click(object sender, EventArgs e) { LevelCmd("map", " growtrees"); }

        #region Colored Reader Context Menu

        void nightModeToolStripMenuItem_Click_1(object sender, EventArgs e) {
            if (MessageBox.Show("Changing to and from night mode will clear your logs. Do you still want to change?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            main_txtLog.NightMode = nightModeToolStripMenuItem.Checked;
            nightModeToolStripMenuItem.Checked = !nightModeToolStripMenuItem.Checked;
        }

        void colorsToolStripMenuItem_Click_1(object sender, EventArgs e) {
            main_txtLog.Colorize = !colorsToolStripMenuItem.Checked;
            colorsToolStripMenuItem.Checked = !colorsToolStripMenuItem.Checked;
        }

        void dateStampToolStripMenuItem_Click(object sender, EventArgs e) {
            main_txtLog.DateStamp = !dateStampToolStripMenuItem.Checked;
            dateStampToolStripMenuItem.Checked = !dateStampToolStripMenuItem.Checked;
        }

        void autoScrollToolStripMenuItem_Click(object sender, EventArgs e) {
            main_txtLog.AutoScroll = !autoScrollToolStripMenuItem.Checked;
            autoScrollToolStripMenuItem.Checked = !autoScrollToolStripMenuItem.Checked;
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
            LevelProperties settings = new LevelProperties(lvl);
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

