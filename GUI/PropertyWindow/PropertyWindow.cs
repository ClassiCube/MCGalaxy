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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MCGalaxy.Blocks;
using MCGalaxy.Commands;
using MCGalaxy.Games;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {
        ZombieProperties zsSettings = new ZombieProperties();
        LavaProperties lsSettings = new LavaProperties();

        public PropertyWindow() {
            InitializeComponent();
            lsSettings.LoadFromServer();
            zsSettings.LoadFromServer();
            propsZG.SelectedObject = zsSettings;
            pg_lava.SelectedObject = lsSettings;
        }

        void PropertyWindow_Load(object sender, EventArgs e) {
            string[] colors = LineFormatter.GetColorsList().ToArray();
            chat_cmbDefault.Items.AddRange(colors);
            chat_cmbIRC.Items.AddRange(colors);
            chat_cmbSyntax.Items.AddRange(colors);
            chat_cmbDesc.Items.AddRange(colors);
            cmbColor.Items.AddRange(colors);

            sec_cmbVerifyRank.Enabled = Server.verifyadmins;
            ToggleIrcSettings(Server.irc);
            ToggleMySQLSettings(Server.useMySQL);
            ToggleChatSpamSettings(Server.checkspam);
            ToggleCmdSpamSettings(Server.CmdSpamCheck);
            ToggleBlocksSpamSettings(Server.BlockSpamCheck);

            string opchatperm = "", adminchatperm = "";
            string verifyadminsperm = "", afkkickrank = "", osmaprank = "";
            LevelPermission adminChatRank =
                CommandExtraPerms.MinPerm("adminchat", LevelPermission.Admin);
            LevelPermission opChatRank =
                CommandExtraPerms.MinPerm("opchat", LevelPermission.Operator);

            foreach (Group grp in Group.GroupList) {
                rank_cmbDefault.Items.Add(grp.name);
                rank_cmbOpChat.Items.Add(grp.name);
                rank_cmbAdminChat.Items.Add(grp.name);
                sec_cmbVerifyRank.Items.Add(grp.name);
                afk_cmbKickPerm.Items.Add(grp.name);
                rank_cmbOsMap.Items.Add(grp.name);

                if (grp.Permission == opChatRank)
                    opchatperm = grp.name;
                if (grp.Permission == adminChatRank)
                    adminchatperm = grp.name;
                if (grp.Permission == Server.verifyadminsrank)
                    verifyadminsperm = grp.name;
                if (grp.Permission == Server.afkkickperm)
                    afkkickrank = grp.name;
                if (grp.Permission == Server.osPerbuildDefault)
                    osmaprank = grp.name;
            }
            
            rank_cmbDefault.SelectedIndex = 1;
            rank_cmbOpChat.SelectedIndex = ( opchatperm != String.Empty ? rank_cmbOpChat.Items.IndexOf(opchatperm) : 1 );
            rank_cmbAdminChat.SelectedIndex = ( adminchatperm != String.Empty ? rank_cmbAdminChat.Items.IndexOf(adminchatperm) : 1 );
            sec_cmbVerifyRank.SelectedIndex = ( verifyadminsperm != String.Empty ? sec_cmbVerifyRank.Items.IndexOf(verifyadminsperm) : 1 );
            afk_cmbKickPerm.SelectedIndex = ( afkkickrank != String.Empty ? afk_cmbKickPerm.Items.IndexOf(afkkickrank) : 1 );
            rank_cmbOsMap.SelectedIndex = ( osmaprank != String.Empty ? rank_cmbOsMap.Items.IndexOf(osmaprank) : 1 );

            //Load server stuff
            LoadProp("properties/server.properties");
            LoadRanks();
            try {
                LoadCommands();
                LoadBlocks();
                LoadExtraCmdCmds();
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Failed to load commands and blocks!");
            }

            try {
                LoadLavaSettings();
                UpdateLavaMapList();
                UpdateLavaControls();
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Failed to load Lava Survival settings!");
            }

            try {
                lavaUpdateTimer = new System.Timers.Timer(10000) { AutoReset = true };
                lavaUpdateTimer.Elapsed += delegate {
                    UpdateLavaControls();
                    UpdateLavaMapList(false);
                };
                lavaUpdateTimer.Start();
            }
            catch {
                Server.s.Log("Failed to start lava control update timer!");
            }

            //Sigh. I wish there were SOME event to help me.
            foreach(var command in Command.all.commands) {
                if ( Command.core.commands.Contains( command ) )
                    continue;

                lstCommands.Items.Add ( command.name );
            }
        }

        private void PropertyWindow_Unload(object sender, EventArgs e) {
            lavaUpdateTimer.Dispose();
            Window.prevLoaded = false;
            TntWarsGame.GuiLoaded = null;
        }

        List<Group> storedRanks = new List<Group>();
        List<CommandPerms> storedCommands = new List<CommandPerms>();
        List<BlockPerms> storedBlocks = new List<BlockPerms>();

        public void LoadRanks() {
            txtCmdRanks.Text = "The following ranks are available: \r\n\r\n";
            txtcmdranks2.Text = "The following ranks are available: \r\n\r\n";
            listRanks.Items.Clear();
            storedRanks.Clear();
            storedRanks.AddRange(Group.GroupList);
            foreach ( Group grp in storedRanks ) {
                txtCmdRanks.Text += "    " + grp.name + " (" + (int)grp.Permission + ")\r\n";
                txtcmdranks2.Text += "    " + grp.name + " (" + (int)grp.Permission + ")\r\n";
                listRanks.Items.Add(grp.trueName + " = " + (int)grp.Permission);
            }
            txtBlRanks.Text = txtCmdRanks.Text;
            listRanks.SelectedIndex = 0;
        }
        public void SaveRanks() {
            Group.saveGroups(storedRanks);
            Group.InitAll();
            LoadRanks();
        }

        public void LoadCommands() {
            listCommands.Items.Clear();
            storedCommands = CommandPerms.CopyAll();
            foreach (CommandPerms perms in storedCommands) {
                listCommands.Items.Add(perms.CmdName);
            }
            
            if ( listCommands.SelectedIndex == -1 )
                listCommands.SelectedIndex = 0;
            // Sort the commands list
            listCommands.Sorted = true;
            listCommands.Sorted = false;
        }
        public void SaveCommands() {
            CommandPerms.Save();
            CommandPerms.Load();
            LoadCommands();
        }

        public void LoadBlocks() {
            listBlocks.Items.Clear();
            storedBlocks.Clear();
            storedBlocks.AddRange(BlockPerms.List);
            foreach ( BlockPerms bs in storedBlocks ) {
                if ( Block.Name(bs.BlockID) != "unknown" )
                    listBlocks.Items.Add(Block.Name(bs.BlockID));
            }
            if ( listBlocks.SelectedIndex == -1 )
                listBlocks.SelectedIndex = 0;
        }

        public void SaveBlocks() {
            BlockPerms.Save(storedBlocks);
            Block.SetBlocks();
            LoadBlocks();
        }

        public void LoadProp(string givenPath) {
            SrvProperties.Load(givenPath);
            LoadGeneralProps();
            LoadChatProps();
            LoadIrcSqlProps();
            LoadMiscProps();
            LoadRankProps();
            LoadSecurityProps();
            zsSettings.LoadFromServer();
        }
        
        void ParseColor(string value, ComboBox target) {
            target.SelectedIndex = target.Items.IndexOf(Colors.Name(value));
        }

        void SaveProperties() {
            try {
                ApplyGeneralProps();
                ApplyChatProps();
                ApplyIrcSqlProps();
                ApplyMiscProps();
                ApplyRankProps(); 
                ApplySecurityProps();
                zsSettings.ApplyToServer();
                lsSettings.ApplyToServer();
                
                SrvProperties.Save();
                CommandExtraPerms.Save();
            } catch( Exception ex ) {
                Server.ErrorLog(ex);
                Server.s.Log("SAVE FAILED! properties/server.properties");
            }
        }

        
        Color GetColor(string name) {
            string code = Colors.Parse(name);
            if (code == "") return SystemColors.Control;
            if (Colors.IsStandardColor(code[1])) return Color.FromName(name);
            
            CustomColor col = Colors.ExtColors[code[1]];
            return Color.FromArgb(col.R, col.G, col.B);
        }

        void removeDigit(TextBox foundTxt) {
            try {
                int lastChar = int.Parse(foundTxt.Text[foundTxt.Text.Length - 1].ToString());
            } catch {
                foundTxt.Text = "";
            }
        }

        private void txtPort_TextChanged(object sender, EventArgs e) { removeDigit(srv_txtPort); }
        private void txtBackup_TextChanged(object sender, EventArgs e) { removeDigit(bak_txtTime); }

        private void btnSave_Click(object sender, EventArgs e) { saveStuff(); Dispose(); }
        private void btnApply_Click(object sender, EventArgs e) { saveStuff(); }

        void saveStuff() {
            foreach ( Control tP in tabControl.Controls )
                if ( tP is TabPage && tP != pageCommands && tP != pageBlocks )
                    foreach ( Control ctrl in tP.Controls )
                        if ( ctrl is TextBox && ctrl.Name.ToLower() != "txtgrpmotd" )
                            if ( ctrl.Text == "" ) {
                MessageBox.Show("A textbox has been left empty. It must be filled.\n" + ctrl.Name);
                return;
            }

            SaveProperties();
            SaveRanks();
            SaveCommands();
            SaveOldExtraCustomCmdChanges();
            SaveBlocks();
            try { SaveLavaSettings(); }
            catch { Server.s.Log("Error saving Lava Survival settings!"); }
            try { ZombieGameProps.SaveSettings(); }
            catch { Server.s.Log("Error saving Zombie Survival settings!"); }

            SrvProperties.Load("properties/server.properties"); // loads when saving?
            CommandPerms.Load();

            // Trigger profanity filter reload
            // Not the best way of doing things, but it kinda works
            ProfanityFilter.Init();
        }

        private void btnDiscard_Click(object sender, EventArgs e) {
            this.Dispose();
        }

        private void fillAllowance(ref TextBox txtBox, ref List<LevelPermission> addTo) {
            addTo.Clear();
            if ( txtBox.Text != "" ) {
                string[] perms = txtBox.Text.Split(',');
                for ( int i = 0; i < perms.Length; i++ ) {
                    perms[i] = perms[i].Trim().ToLower();
                    int foundPerm;
                    try {
                        foundPerm = int.Parse(perms[i]);
                    }
                    catch {
                        Group foundGroup = Group.Find(perms[i]);
                        if ( foundGroup != null ) foundPerm = (int)foundGroup.Permission;
                        else { Server.s.Log("Could not find " + perms[i]); continue; }
                    }
                    addTo.Add((LevelPermission)foundPerm);
                }

                txtBox.Text = "";
                foreach ( LevelPermission p in addTo ) {
                    txtBox.Text += "," + (int)p;
                }
                if ( txtBox.Text != "" ) txtBox.Text = txtBox.Text.Remove(0, 1);
            }
        }
        private void fillLowest(ref TextBox txtBox, ref LevelPermission toChange) {
            if ( txtBox.Text != "" ) {
                txtBox.Text = txtBox.Text.Trim().ToLower();
                int foundPerm = -100;
                try {
                    foundPerm = int.Parse(txtBox.Text);
                }
                catch {
                    Group foundGroup = Group.Find(txtBox.Text);
                    if ( foundGroup != null ) foundPerm = (int)foundGroup.Permission;
                    else { Server.s.Log("Could not find " + txtBox.Text); }
                }

                txtBox.Text = "";
                if ( foundPerm < -99 ) txtBox.Text = (int)toChange + "";
                else txtBox.Text = foundPerm + "";

                toChange = (LevelPermission)Convert.ToInt16(txtBox.Text);
            }
        }

        private void getHelp(string toHelp) {
            Player.storedHelp = "";
            Player.storeHelp = true;
            Command.all.Find("help").Use(null, toHelp);
            Player.storeHelp = false;
            string messageInfo = "Help information for " + toHelp + ":\r\n\r\n";
            messageInfo += Player.storedHelp;
            MessageBox.Show(messageInfo);
        }

        private void forceUpdateBtn_Click(object sender, EventArgs e) {
            forceUpdateBtn.Enabled = false;
            DialogResult result = MessageBox.Show("Would you like to force update " + Server.SoftwareName + " now?", "Force Update",
                                                  MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK) {
                saveStuff();
                Updater.PerformUpdate();
                Dispose();
            } else {
                forceUpdateBtn.Enabled = true;
            }
        }
        
        private void txtGrpMOTD_TextChanged(object sender, EventArgs e) {
            if ( txtGrpMOTD.Text != null ) storedRanks[listRanks.SelectedIndex].MOTD = txtGrpMOTD.Text;
        }
    }
}
