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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Generator;
using MCGalaxy.Tasks;

namespace MCGalaxy.Gui {
    public partial class Window : Form {
        // for cross thread use
        delegate void StringCallback(string s);
        delegate void PlayerListCallback(List<Player> players);
        delegate void VoidDelegate();
        bool mapgen, loaded;

        NotifyIcon notifyIcon = new NotifyIcon();
        Player curPlayer;

        public Window() {
            logCallback = LogMessage;
            InitializeComponent();
        }
        
        // warn user if they're using the GUI with a DLL for different server version
        static void CheckVersions() {
            string gui_version = Server.InternalVersion;
            string dll_version = Server.Version;
            if (gui_version.CaselessEq(dll_version)) return;
            
            const string fmt = 
@"Currently you are using:
  {2} for {0} {1}
  {4} for {0} {3}

Trying to mix two versions is unsupported - you may experience issues";
            string msg = string.Format(fmt, Server.SoftwareName, 
                                       gui_version, AssemblyFile(typeof(Window), "MCGalaxy.exe"),
                                       dll_version, AssemblyFile(typeof(Server), "MCGalaxy_.dll"));
            RunAsync(() => Popup.Warning(msg));
        }
        
        static string AssemblyFile(Type type, string defPath) {
            try {
                string path = type.Assembly.CodeBase;
                return Path.GetFileName(path);
            } catch {
                return defPath;
            }
        }

        void Window_Load(object sender, EventArgs e) {
            LoadIcon();
            // Necessary as some versions of WINE may call Window_Load multiple times
            //  (however icon must still be reloaded each time)
            if (loaded) return;
            loaded = true;
            
            Text = "Starting " + Server.SoftwareNameVersioned + "...";
            Show();
            BringToFront();
            WindowState = FormWindowState.Normal;
            CheckVersions();

            InitServer();
            foreach (MapGen gen in MapGen.Generators) {
                if (gen.Type == GenType.Advanced) continue;
                map_cmbType.Items.Add(gen.Theme);
            }
            
            Text = Server.Config.Name + " - " + Server.SoftwareNameVersioned;
            MakeNotifyIcon();
            
            main_Players.Font = new Font("Calibri", 8.25f);
            main_Maps.Font = new Font("Calibri", 8.25f);
        }
        
        void LoadIcon() {
            // Normally this code would be in InitializeComponent method in Window.Designer.cs,
            //  however that doesn't work properly with some WINE versions (you get WINE icon instead)
            try {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Window));
                Icon = (Icon)(resources.GetObject("$this.Icon"));
            } catch { }
        }
        
        void UpdateNotifyIconText() {
            int playerCount = PlayerInfo.Online.Count;
            string players = " (" + playerCount + " players)";
            
            // ArgumentException thrown if text length is > 63
            string text = (Server.Config.Name + players);
            if (text.Length > 63) text = text.Substring(0, 63);
            notifyIcon.Text = text;
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
            Logger.LogHandler += LogMessage;
            Updater.NewerVersionDetected += LogNewerVersionDetected;

            Server.OnURLChange += UpdateUrl;
            Server.OnSettingsUpdate += SettingsUpdate;
            Server.Background.QueueOnce(InitServerTask);
        }
        
        // cache LogMessage, avoids new object being allocated every time
        delegate void LogCallback(LogType type, string message);
        LogCallback logCallback;
        
        void LogMessage(LogType type, string message) {
            if (!Server.Config.FileLogging[(int)type]) return;
            
            if (InvokeRequired) {
                try {
                    BeginInvoke(logCallback, type, message); 
                } catch (InvalidOperationException) {
                    // This exception is thrown when trying to log
                    //  messages after window has already been closed
                }
                return;
            }
            if (Server.shuttingDown) return;
            string newline = Environment.NewLine;
            
            switch (type) {
                case LogType.Error:
                    main_txtLog.AppendLog("&c!!!Error" + ExtractErrorMessage(message) 
                                          + " - See Logs tab for more details" + newline);
                    message = FormatError(message);
                    logs_txtError.AppendText(message + newline);
                    break;
                case LogType.BackgroundActivity:
                    message = DateTime.Now.ToString("(HH:mm:ss) ") + message;
                    logs_txtSystem.AppendText(message + newline);
                    break;
                case LogType.CommandUsage:
                    message = DateTime.Now.ToString("(HH:mm:ss) ") + message;
                    main_txtLog.AppendLog(message + newline, main_txtLog.ForeColor, false);
                    break;
                default:
                    main_txtLog.AppendLog(message + newline);
                    break;
            }
        }
        
        static string FormatError(string message) {
            string date = "----" + DateTime.Now + "----";
            return date + Environment.NewLine + message + Environment.NewLine + "-------------------------";
        }
        
        static string msgPrefix = Environment.NewLine + "Message: ";
        static string ExtractErrorMessage(string raw) {
            // Error messages are usually structured like so:
            //   Type: whatever
            //   Message: whatever
            //   Something: whatever
            // this code extracts the Message line from the raw message
            int beg = raw.IndexOf(msgPrefix);
            if (beg == -1) return "";
            
            beg += msgPrefix.Length;
            int end = raw.IndexOf(Environment.NewLine, beg);
            if (end == -1) return "";
            
            return " (" + raw.Substring(beg, end - beg) + ")";
        }
        

        static volatile bool msgOpen = false;
        static void LogNewerVersionDetected(object sender, EventArgs e) {
            if (msgOpen) return;
            // don't want message box blocking background scheduler thread
            RunAsync(ShowUpdateMessageBox);
        }
        
        static void RunAsync(ThreadStart func) {
            Thread thread = new Thread(func);
            thread.Name = "MCGalaxy_MsgBox";
            thread.Start();
        }
        
        static void ShowUpdateMessageBox() {
            msgOpen = true;
            if (Popup.YesNo("New version found. Would you like to update?", "Update?")) {
                Updater.PerformUpdate();
            }
            msgOpen = false;
        }
        
        void InitServerTask(SchedulerTask task) {
            Server.Start();
            // The first check for updates is run after 10 seconds, subsequent ones every two hours
            Server.Background.QueueRepeat(Updater.UpdaterTask, null, TimeSpan.FromSeconds(10));

            OnPlayerConnectEvent.Register(Player_PlayerConnect, Priority.Low);
            OnPlayerDisconnectEvent.Register(Player_PlayerDisconnect, Priority.Low);
            OnSentMapEvent.Register(Player_OnJoinedLevel, Priority.Low);

            OnLevelAddedEvent.Register(Level_LevelAdded, Priority.Low);
            OnLevelRemovedEvent.Register(Level_LevelRemoved, Priority.Low);
            OnPhysicsLevelChangedEvent.Register(Level_PhysicsLevelChanged, Priority.Low);

            RunOnUI_Async(() => main_btnProps.Enabled = true);
        }

        public void RunOnUI_Async(Action act) { BeginInvoke(act); }
        
        void Player_PlayerConnect(Player p) {
            RunOnUI_Async(() => {
                Main_UpdatePlayersList();
                Players_UpdateList(); 
            });
        }
        
        void Player_PlayerDisconnect(Player p, string reason) {
            RunOnUI_Async(() => {
                Main_UpdateMapList();
                Main_UpdatePlayersList();
                Players_UpdateList(); 
            });
        }
        
        void Player_OnJoinedLevel(Player p, Level prevLevel, Level lvl) {
            RunOnUI_Async(() => {
                Main_UpdateMapList();
                Main_UpdatePlayersList();
                Players_UpdateSelected(); 
            });
        }
        
        void Level_LevelAdded(Level lvl) {
            RunOnUI_Async(() => {
                Main_UpdateMapList();
                Map_UpdateLoadedList();
                Map_UpdateUnloadedList();
            });
        }
        
        void Level_LevelRemoved(Level lvl) {
            RunOnUI_Async(() => {
                Main_UpdateMapList();
                Map_UpdateLoadedList();
                Map_UpdateUnloadedList();
            });
        }
        
        void Level_PhysicsLevelChanged(Level lvl, int level) {
            RunOnUI_Async(() => {
                Main_UpdateMapList();
                Map_UpdateLoadedList();
            });
        }


        void SettingsUpdate() {
            RunOnUI_Async(() => {
                if (Server.shuttingDown) return;
                Text = Server.Config.Name + " - " + Server.SoftwareNameVersioned;
                UpdateNotifyIconText();
            });
        }

        public void PopupNotify(string message, ToolTipIcon icon = ToolTipIcon.Info) {
            notifyIcon.ShowBalloonTip(3000, Server.Config.Name, message, icon);
        }

        void UpdateUrl(string s) {
            RunOnUI_Async(() => { Main_UpdateUrl(s); });
        }

        void Window_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.WindowsShutDown) {
                Server.Stop(false, "Server shutdown - PC turning off");
                notifyIcon.Dispose();
            }
            
            if (Server.shuttingDown || Popup.OKCancel("Really shutdown the server? All players will be disconnected!", "Exit")) {
                Server.Stop(false, Server.Config.DefaultShutdownMessage);
                notifyIcon.Dispose();
            } else {
                // Prevents form from closing when user clicks the X and then hits 'cancel'
                e.Cancel = true;
            }
        }

        void btnClose_Click(object sender, EventArgs e) { Close(); }

        void btnProperties_Click(object sender, EventArgs e) {
            if (!hasPropsForm) {
                propsForm = new PropertyWindow();
                // just doing propForms.Icon = Icon; here doesn't show on Mono
                try { propsForm._icon = Icon; } catch { }
                hasPropsForm = true; 
            }
            
            propsForm.Show();
            if (!propsForm.Focused) propsForm.Focus();
        }

        public static bool hasPropsForm;
        PropertyWindow propsForm;

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
            try { Map_UpdateUnloadedList(); }
            catch { }
            try { Players_UpdateList(); }
            catch { }
            
            try {
                if (logs_txtGeneral.Text.Length == 0)
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
