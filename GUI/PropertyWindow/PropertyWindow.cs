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
using MCGalaxy.Commands;
using MCGalaxy.Eco;
using MCGalaxy.Events.GameEvents;
using MCGalaxy.Games;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {
        ZombieProperties zsSettings = new ZombieProperties();

        public PropertyWindow() {
            InitializeComponent();
            zsSettings.LoadFromServer();
            propsZG.SelectedObject = zsSettings;
        }
        
        public void RunOnUI_Async(Action act) { BeginInvoke(act); }

        void PropertyWindow_Load(object sender, EventArgs e) {
            OnMapsChangedEvent.Register(HandleMapsChanged, Priority.Low);
            OnStateChangedEvent.Register(HandleStateChanged, Priority.Low);
            GuiPerms.UpdateRankNames();
            rank_cmbDefault.Items.AddRange(GuiPerms.RankNames);
            rank_cmbOsMap.Items.AddRange(GuiPerms.RankNames);
            blk_cmbMin.Items.AddRange(GuiPerms.RankNames);
            cmd_cmbMin.Items.AddRange(GuiPerms.RankNames);

            //Load server stuff
            LoadProperties();
            LoadRanks();
            try {
                LoadCommands();
                LoadBlocks();
            } catch (Exception ex) {
                Logger.LogError("Error loading commands and blocks", ex);
            }

            LoadGameProps();
        }

        void PropertyWindow_Unload(object sender, EventArgs e) {
            OnMapsChangedEvent.Unregister(HandleMapsChanged);
            OnStateChangedEvent.Unregister(HandleStateChanged);
            Window.hasPropsForm = false;
        }

        void LoadProperties() {
            SrvProperties.Load();
            LoadGeneralProps();
            LoadChatProps();
            LoadRelayProps();
            LoadSqlProps();
            LoadEcoProps();
            LoadMiscProps();
            LoadRankProps();
            LoadSecurityProps();
            zsSettings.LoadFromServer();
        }

        void SaveProperties() {
            try {
                ApplyGeneralProps();
                ApplyChatProps();
                ApplyRelayProps();
                ApplySqlProps();
                ApplyEcoProps();
                ApplyMiscProps();
                ApplyRankProps();
                ApplySecurityProps();
                
                zsSettings.ApplyToServer();
                SrvProperties.Save();
                Economy.Save();                
            } catch (Exception ex) {
                Logger.LogError(ex);
                Logger.Log(LogType.Warning, "SAVE FAILED! properties/server.properties");
            }
            SaveDiscordProps();
        }

        void btnSave_Click(object sender, EventArgs e) { SaveChanges(); Dispose(); }
        void btnApply_Click(object sender, EventArgs e) { SaveChanges(); }

        void SaveChanges() {
            SaveProperties();
            SaveRanks();
            SaveCommands();
            SaveBlocks();
            SaveGameProps();
            
            try { ZSGame.Config.Save(); }
            catch { Logger.Log(LogType.Warning, "Error saving Zombie Survival settings!"); }

            SrvProperties.Load(); // loads when saving?
            CommandPerms.Load();
        }

        void btnDiscard_Click(object sender, EventArgs e) { Dispose(); }

        void GetHelp(string toHelp) {
            ConsoleHelpPlayer p = new ConsoleHelpPlayer();
            Command.Find("Help").Use(p, toHelp);
            Popup.Message(Colors.StripUsed(p.Messages), "Help for /" + toHelp);
        }
    }
    
    sealed class ConsoleHelpPlayer : Player {
        public string Messages = "";
            
        public ConsoleHelpPlayer() : base("(console)") {
            group = Group.NobodyRank;
            SuperName = "Console";
        }
            
        public override void Message(byte type, string message) {
            message = Chat.Format(message, this);
            Messages += message + "\r\n";
        }
    }
}
