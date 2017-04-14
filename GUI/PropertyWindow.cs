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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using MCGalaxy.Games;
using MCGalaxy.Gui.Popups;
using MCGalaxy.SQL;
using MCGalaxy.Util;

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
                cmbDefaultRank.Items.Add(grp.name);
                cmbOpChat.Items.Add(grp.name);
                cmbAdminChat.Items.Add(grp.name);
                sec_cmbVerifyRank.Items.Add(grp.name);
                cmbAFKKickPerm.Items.Add(grp.name);
                cmbOsMap.Items.Add(grp.name);

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
            
            cmbDefaultRank.SelectedIndex = 1;
            cmbOpChat.SelectedIndex = ( opchatperm != String.Empty ? cmbOpChat.Items.IndexOf(opchatperm) : 1 );
            cmbAdminChat.SelectedIndex = ( adminchatperm != String.Empty ? cmbAdminChat.Items.IndexOf(adminchatperm) : 1 );
            sec_cmbVerifyRank.SelectedIndex = ( verifyadminsperm != String.Empty ? sec_cmbVerifyRank.Items.IndexOf(verifyadminsperm) : 1 );
            cmbAFKKickPerm.SelectedIndex = ( afkkickrank != String.Empty ? cmbAFKKickPerm.Items.IndexOf(afkkickrank) : 1 );
            cmbOsMap.SelectedIndex = ( osmaprank != String.Empty ? cmbOsMap.Items.IndexOf(osmaprank) : 1 );

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
        List<GrpCommands.rankAllowance> storedCommands = new List<GrpCommands.rankAllowance>();
        List<Block.Blocks> storedBlocks = new List<Block.Blocks>();

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
            storedCommands.Clear();
            foreach ( GrpCommands.rankAllowance aV in GrpCommands.allowedCommands ) {
                storedCommands.Add(aV);
                listCommands.Items.Add(aV.commandName);
            }
            if ( listCommands.SelectedIndex == -1 )
                listCommands.SelectedIndex = 0;
            // Sort the commands list
            listCommands.Sorted = true;
            listCommands.Sorted = false;
        }
        public void SaveCommands() {
            GrpCommands.Save(storedCommands);
            GrpCommands.fillRanks();
            LoadCommands();
        }

        public void LoadBlocks() {
            listBlocks.Items.Clear();
            storedBlocks.Clear();
            storedBlocks.AddRange(Block.BlockList);
            foreach ( Block.Blocks bs in storedBlocks ) {
                if ( Block.Name(bs.type) != "unknown" )
                    listBlocks.Items.Add(Block.Name(bs.type));
            }
            if ( listBlocks.SelectedIndex == -1 )
                listBlocks.SelectedIndex = 0;
        }

        public void SaveBlocks() {
            Block.SaveBlocks(storedBlocks);
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


        private void chat_cmbDefault_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colDefault.BackColor = GetColor(chat_cmbDefault.Items[chat_cmbDefault.SelectedIndex].ToString());
        }

        private void chat_cmbIRC_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colIRC.BackColor = GetColor(chat_cmbIRC.Items[chat_cmbIRC.SelectedIndex].ToString());
        }
        
        private void chat_cmbSyntax_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colSyntax.BackColor = GetColor(chat_cmbSyntax.Items[chat_cmbSyntax.SelectedIndex].ToString());
        }

        private void chat_cmbDesc_SelectedIndexChanged(object sender, EventArgs e) {
            chat_colDesc.BackColor = GetColor(chat_cmbDesc.Items[chat_cmbDesc.SelectedIndex].ToString());
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
            }
            catch {
                foundTxt.Text = "";
            }
        }

        private void txtPort_TextChanged(object sender, EventArgs e) { removeDigit(txtPort); }
        private void txtBackup_TextChanged(object sender, EventArgs e) { removeDigit(txtBackup); }

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

            SrvProperties.Load("properties/server.properties"); // loads when saving?
            GrpCommands.fillRanks();

            // Trigger profanity filter reload
            // Not the best way of doing things, but it kinda works
            ProfanityFilter.Init();
        }

        private void btnDiscard_Click(object sender, EventArgs e) {
            this.Dispose();
        }

        private void toolTip_Popup(object sender, PopupEventArgs e) {

        }

        private void tabPage2_Click(object sender, EventArgs e) {

        }

        private void tabPage1_Click(object sender, EventArgs e) {

        }

        private void chkPhysicsRest_CheckedChanged(object sender, EventArgs e) {
        }

        private void chkGC_CheckedChanged(object sender, EventArgs e) {
        }

        private void chkIRC_CheckedChanged(object sender, EventArgs e) {
            ToggleIrcSettings(chkIRC.Checked);
        }

        private void btnBackup_Click(object sender, EventArgs e) {
            /*FolderBrowserDialog folderDialog = new FolderBrowserDialog();
folderDialog.Description = "Select Folder";
if (folderDialog.ShowDialog() == DialogResult.OK) {
txtBackupLocation.Text = folderDialog.SelectedPath;
}*/
            MessageBox.Show("Currently glitchy! Just type in the location by hand.");
        }

        #region rankTab
        private void cmbColor_SelectedIndexChanged(object sender, EventArgs e) {
            lblColor.BackColor = GetColor(cmbColor.Items[cmbColor.SelectedIndex].ToString());
            storedRanks[listRanks.SelectedIndex].color = Colors.Parse(cmbColor.Items[cmbColor.SelectedIndex].ToString());
        }

        bool skip = false;
        private void listRanks_SelectedIndexChanged(object sender, EventArgs e) {
            if ( skip ) return;
            Group grp = storedRanks.Find(G => G.trueName == listRanks.Items[listRanks.SelectedIndex].ToString().Split('=')[0].Trim());
            if ( grp.Permission == LevelPermission.Nobody ) { listRanks.SelectedIndex = 0; return; }

            txtRankName.Text = grp.trueName;
            txtPermission.Text = ( (int)grp.Permission ).ToString();
            txtLimit.Text = grp.maxBlocks.ToString();
            txtMaxUndo.Text = grp.maxUndo.ToString();
            cmbColor.SelectedIndex = cmbColor.Items.IndexOf(Colors.Name(grp.color));
            txtGrpMOTD.Text = String.IsNullOrEmpty(grp.MOTD) ? String.Empty : grp.MOTD;
            txtFileName.Text = grp.fileName;
            txtOSMaps.Text = grp.OverseerMaps.ToString();
            txtPrefix.Text = grp.prefix;
        }

        private void txtRankName_TextChanged(object sender, EventArgs e) {
            if ( txtRankName.Text != "" && txtRankName.Text.ToLower() != "nobody" ) {
                storedRanks[listRanks.SelectedIndex].trueName = txtRankName.Text;
                skip = true;
                listRanks.Items[listRanks.SelectedIndex] = txtRankName.Text + " = " + (int)storedRanks[listRanks.SelectedIndex].Permission;
                skip = false;
            }
        }

        private void txtPermission_TextChanged(object sender, EventArgs e) {
            if ( txtPermission.Text != "" ) {
                int foundPerm;
                if (!int.TryParse(txtPermission.Text, out foundPerm)) {
                    if ( txtPermission.Text != "-" )
                        txtPermission.Text = txtPermission.Text.Remove(txtPermission.Text.Length - 1);
                    return;
                }

                if ( foundPerm < -50 ) { txtPermission.Text = "-50"; return; }
                else if ( foundPerm > 119 ) { txtPermission.Text = "119"; return; }

                storedRanks[listRanks.SelectedIndex].Permission = (LevelPermission)foundPerm;
                skip = true;
                listRanks.Items[listRanks.SelectedIndex] = storedRanks[listRanks.SelectedIndex].trueName + " = " + foundPerm;
                skip = false;
            }
        }

        private void txtLimit_TextChanged(object sender, EventArgs e) {
            if ( txtLimit.Text != "" ) {
                int drawLimit;
                if (!int.TryParse(txtLimit.Text, out drawLimit)) {
                    txtLimit.Text = txtLimit.Text.Remove(txtLimit.Text.Length - 1);
                    return;
                }

                if ( drawLimit < 1 ) { txtLimit.Text = "1"; return; }

                storedRanks[listRanks.SelectedIndex].maxBlocks = drawLimit;
            }
        }

        private void txtMaxUndo_TextChanged(object sender, EventArgs e) {
            if ( txtMaxUndo.Text != "" ) {
                long maxUndo;
                if (!long.TryParse(txtMaxUndo.Text, out maxUndo)) {
                    txtMaxUndo.Text = txtMaxUndo.Text.Remove(txtMaxUndo.Text.Length - 1);
                    return;
                }

                if ( maxUndo < -1 ) { txtMaxUndo.Text = "0"; return; }

                storedRanks[listRanks.SelectedIndex].maxUndo = maxUndo;
            }
        }
        
        private void txtOSMaps_TextChanged(object sender, EventArgs e) {
            if ( txtOSMaps.Text != "" ) {
                byte maxMaps;
                if (!byte.TryParse(txtOSMaps.Text, out maxMaps)) {
                    txtOSMaps.Text = txtOSMaps.Text.Remove(txtOSMaps.Text.Length - 1);
                    return;
                }
                storedRanks[listRanks.SelectedIndex].OverseerMaps = maxMaps;
            }
        }

        private void txtFileName_TextChanged(object sender, EventArgs e) {
            if ( txtFileName.Text != "" ) {
                storedRanks[listRanks.SelectedIndex].fileName = txtFileName.Text;
            }
        }
        
        private void txtPrefix_TextChanged(object sender, EventArgs e) {
            storedRanks[listRanks.SelectedIndex].prefix = txtPrefix.Text;
        }

        private void btnAddRank_Click(object sender, EventArgs e) {
            Group newGroup = new Group((LevelPermission)5, 600, 30, "CHANGEME", '1', String.Empty, "CHANGEME.txt");
            storedRanks.Add(newGroup);
            listRanks.Items.Add(newGroup.trueName + " = " + (int)newGroup.Permission);
        }

        private void button1_Click(object sender, EventArgs e) {
            if ( listRanks.Items.Count > 1 ) {
                storedRanks.RemoveAt(listRanks.SelectedIndex);
                skip = true;
                listRanks.Items.RemoveAt(listRanks.SelectedIndex);
                skip = false;

                listRanks.SelectedIndex = 0;
            }
        }
        #endregion

        #region commandTab
        private void listCommands_SelectedIndexChanged(object sender, EventArgs e) {
            Command cmd = Command.all.Find(listCommands.SelectedItem.ToString());
            GrpCommands.rankAllowance allowVar = storedCommands.Find(aV => aV.commandName == cmd.name);

            if ( Group.findPerm(allowVar.lowestRank) == null ) allowVar.lowestRank = cmd.defaultRank;
            txtCmdLowest.Text = (int)allowVar.lowestRank + "";

            bool foundOne = false;
            txtCmdDisallow.Text = "";
            foreach ( LevelPermission perm in allowVar.disallow ) {
                foundOne = true;
                txtCmdDisallow.Text += "," + (int)perm;
            }
            if ( foundOne ) txtCmdDisallow.Text = txtCmdDisallow.Text.Remove(0, 1);

            foundOne = false;
            txtCmdAllow.Text = "";
            foreach ( LevelPermission perm in allowVar.allow ) {
                foundOne = true;
                txtCmdAllow.Text += "," + (int)perm;
            }
            if ( foundOne ) txtCmdAllow.Text = txtCmdAllow.Text.Remove(0, 1);
        }
        private void txtCmdLowest_TextChanged(object sender, EventArgs e) {
            fillLowest(ref txtCmdLowest, ref storedCommands[listCommands.SelectedIndex].lowestRank);
        }
        private void txtCmdDisallow_TextChanged(object sender, EventArgs e) {
            fillAllowance(ref txtCmdDisallow, ref storedCommands[listCommands.SelectedIndex].disallow);
        }
        private void txtCmdAllow_TextChanged(object sender, EventArgs e) {
            fillAllowance(ref txtCmdAllow, ref storedCommands[listCommands.SelectedIndex].allow);
        }
        #endregion

        #region BlockTab
        private void listBlocks_SelectedIndexChanged(object sender, EventArgs e) {
            byte b = Block.Byte(listBlocks.SelectedItem.ToString());
            Block.Blocks bs = storedBlocks.Find(bS => bS.type == b);

            txtBlLowest.Text = (int)bs.lowestRank + "";

            bool foundOne = false;
            txtBlDisallow.Text = "";
            if (bs.disallow != null) {
                foreach ( LevelPermission perm in bs.disallow ) {
                    foundOne = true;
                    txtBlDisallow.Text += "," + (int)perm;
                }
            }
            if ( foundOne ) txtBlDisallow.Text = txtBlDisallow.Text.Remove(0, 1);

            foundOne = false;
            txtBlAllow.Text = "";
            if (bs.allow != null) {
                foreach ( LevelPermission perm in bs.allow ) {
                    foundOne = true;
                    txtBlAllow.Text += "," + (int)perm;
                }
            }
            if ( foundOne ) txtBlAllow.Text = txtBlAllow.Text.Remove(0, 1);
        }
        private void txtBlLowest_TextChanged(object sender, EventArgs e) {
            fillLowest(ref txtBlLowest, ref storedBlocks[Block.Byte(listBlocks.SelectedItem.ToString())].lowestRank);
        }
        private void txtBlDisallow_TextChanged(object sender, EventArgs e) {
            if (storedBlocks[listBlocks.SelectedIndex].disallow == null)
                storedBlocks[listBlocks.SelectedIndex].disallow = new List<LevelPermission>();
            fillAllowance(ref txtBlDisallow, ref storedBlocks[listBlocks.SelectedIndex].disallow);
        }
        private void txtBlAllow_TextChanged(object sender, EventArgs e) {
            if (storedBlocks[listBlocks.SelectedIndex].allow == null)
                storedBlocks[listBlocks.SelectedIndex].allow = new List<LevelPermission>();
            fillAllowance(ref txtBlAllow, ref storedBlocks[listBlocks.SelectedIndex].allow);
        }
        #endregion
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

        private void btnBlHelp_Click(object sender, EventArgs e) {
            getHelp(listBlocks.SelectedItem.ToString());
        }
        private void btnCmdHelp_Click(object sender, EventArgs e) {
            getHelp(listCommands.SelectedItem.ToString());
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


        private void ChkPort_Click(object sender, EventArgs e) {
            using ( var form = new PortTools() ) {
                form.ShowDialog();
            }
        }



        private void CrtCustCmd_Click(object sender, EventArgs e) {
            if ( txtCommandName.Text != null ) {
                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                    MessageBox.Show("Sorry, That command already exists!!");
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text);
                    MessageBox.Show("Command Created!!");
                }
            }
            else {
                MessageBox.Show("You didnt specify a name for the command!!");
            }
        }

        private void CompileCustCmd_Click(object sender, EventArgs e) {
            if ( txtCommandName.Text != null ) {
                if ( File.Exists("extra/commands/dll/Cmd" + txtCommandName.Text + ".dll") ) {
                    MessageBox.Show("Sorry, That command already exists!!");
                }
                else {
                    Command.all.Find("compile").Use(null, txtCommandName.Text);
                    MessageBox.Show("Command Compiled!!");
                }
            }
            else {
                MessageBox.Show("You didnt specify a name for the command!!");
            }
        }



        private void numPlayers_ValueChanged(object sender, EventArgs e) {
            // Ensure that number of guests is never more than number of players
            if ( numGuests.Value > numPlayers.Value ) {
                numGuests.Value = numPlayers.Value;
            }
            numGuests.Maximum = numPlayers.Value;
        }

        private void editTxtsBt_Click_1(object sender, EventArgs e) {
            new EditText().Show();
        }

        private void btnCreate_Click(object sender, EventArgs e) {
            if(String.IsNullOrEmpty(txtCommandName.Text.Trim())) {
                MessageBox.Show ( "Command must have a name" );
                return;
            }

            if ( radioButton1.Checked ) {
                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") || File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                    MessageBox.Show("Command already exists", "", MessageBoxButtons.OK);
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text.ToLower() + " vb");
                    MessageBox.Show("New Command Created: " + txtCommandName.Text.ToLower() + " Created.");
                }
            }
            else {



                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") || File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") ) {
                    MessageBox.Show("Command already exists", "", MessageBoxButtons.OK);
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text.ToLower());
                    MessageBox.Show("New Command Created: " + txtCommandName.Text.ToLower() + " Created.");
                }
            }
        }


        private void btnLoad_Click(object sender, EventArgs e) {
            Command[] commands = null;
            using (FileDialog dialog = new OpenFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.Filter = "Accepted File Types (*.cs, *.vb, *.dll)|*.cs;*.vb;*.dll|C# Source (*.cs)|*.cs|Visual Basic Source (*.vb)|*.vb|.NET Assemblies (*.dll)|*.dll";
                if (dialog.ShowDialog() != DialogResult.OK) return;

                string fileName = dialog.FileName;
                if (fileName.EndsWith(".dll")) {
                    commands = MCGalaxyScripter.FromAssemblyFile(fileName);
                } else {
                    ScriptLanguage language = fileName.EndsWith(".cs") ? ScriptLanguage.CSharp : ScriptLanguage.VB;
                    if (!File.Exists(fileName)) return;
                    
                    var result = MCGalaxyScripter.Compile(File.ReadAllText(fileName), language);
                    if (result == null) { MessageBox.Show ( "Error compiling files" ); return; }

                    if (result.CompilerErrors != null) {
                        foreach (CompilerError err in result.CompilerErrors) {
                            Server.s.ErrorCase("Error #" + err.ErrorNumber);
                            Server.s.ErrorCase("Message: " + err.ErrorText);
                            Server.s.ErrorCase("Line: " + err.Line);
                            Server.s.ErrorCase( "=================================" );
                        }
                        MessageBox.Show("Error compiling from source. Check logs for error");
                        return;
                    }
                    commands = result.Commands;
                }
            }

            if (commands == null) { MessageBox.Show("Error compiling files"); return; }
            for (int i = 0; i < commands.Length; i++) {
                Command cmd = commands[i];

                if (lstCommands.Items.Contains(cmd.name)) {
                    MessageBox.Show("Command " + cmd.name + " already exists. As a result, it was not loaded");
                    continue;
                }

                lstCommands.Items.Add(cmd.name);
                Command.all.Add(cmd);
                Server.s.Log("Added " + cmd.name + " to commands");
            }
            GrpCommands.fillRanks();
        }

        private void btnUnload_Click(object sender, EventArgs e) {
            Command cmd = Command.all.Find(lstCommands.SelectedItem.ToString());
            if (cmd == null) {
                MessageBox.Show(txtCommandName.Text + " is not a valid or loaded command.", ""); return;
            }

            lstCommands.Items.Remove( cmd.name );
            Command.all.Remove(cmd);
            GrpCommands.fillRanks();
            MessageBox.Show("Command was successfully unloaded.", "");
        }


        private void btnDiscardcmd_Click(object sender, EventArgs e) {
            switch ( MessageBox.Show("Are you sure you want to discard this whole file?", "Discard?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ) {
                case DialogResult.Yes:
                    if ( radioButton1.Checked ) {

                        if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") ) {
                            File.Delete("extra/commands/source/Cmd" + txtCommandName.Text + ".vb");
                        }
                        else { MessageBox.Show("File: " + txtCommandName.Text + ".vb Doesnt Exist."); }
                    }
                    else {
                        if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                            File.Delete("extra/commands/source/Cmd" + txtCommandName.Text + ".cs");
                        }
                        else { MessageBox.Show("File: " + txtCommandName.Text + ".cs Doesnt Exist."); }
                    }
                    break;

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start("http://dev.mysql.com/downloads/");
            }
            catch {
                MessageBox.Show("Failed to open link!");
            }
        }

        private void chkUseSQL_CheckedChanged(object sender, EventArgs e) {
            ToggleMySQLSettings(chkUseSQL.Checked);
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

        private bool skipExtraPermChanges;
        private int oldnumber;
        private Command oldcmd;
        private void listCommandsExtraCmdPerms_SelectedIndexChanged(object sender, EventArgs e) {
            SaveOldExtraCustomCmdChanges();
            Command cmd = Command.all.Find(listCommandsExtraCmdPerms.SelectedItem.ToString());
            oldcmd = cmd;
            skipExtraPermChanges = true;
            extracmdpermnumber.Maximum = MaxExtraPermissionNumber(cmd);
            extracmdpermnumber.ReadOnly = extracmdpermnumber.Maximum == 1;
            extracmdpermnumber.Value = 1;
            skipExtraPermChanges = false;
            
            ExtraPermSetDescriptions(cmd, 1);
            oldnumber = (int)extracmdpermnumber.Value;
        }
        
        private int MaxExtraPermissionNumber(Command cmd) {
            var all = CommandExtraPerms.FindAll(cmd.name);
            int maxNum = 0;
            
            foreach (CommandExtraPerms.ExtraPerms perms in all) {
                maxNum = Math.Max(maxNum, perms.Number);
            }
            return maxNum;
        }

        private void SaveOldExtraCustomCmdChanges() {
            if (oldcmd == null || skipExtraPermChanges) return;
            
            CommandExtraPerms.Find(oldcmd.name, oldnumber).MinRank = (LevelPermission)int.Parse(extracmdpermperm.Text);
            CommandExtraPerms.Save();
        }

        private void extracmdpermnumber_ValueChanged(object sender, EventArgs e) {
            SaveOldExtraCustomCmdChanges();
            oldnumber = (int)extracmdpermnumber.Value;
            ExtraPermSetDescriptions(oldcmd, (int)extracmdpermnumber.Value);
        }
        
        private void ExtraPermSetDescriptions(Command cmd, int number) {
            CommandExtraPerms.ExtraPerms perms =  CommandExtraPerms.Find(cmd.name, number);
            extracmdpermdesc.Text = perms.Description;
            extracmdpermperm.Text = ((int)perms.MinRank).ToString();
        }
        
        private void LoadExtraCmdCmds() {
            listCommandsExtraCmdPerms.Items.Clear();
            foreach ( Command cmd in Command.all.commands ) {
                if ( CommandExtraPerms.Find(cmd.name) != null ) {
                    listCommandsExtraCmdPerms.Items.Add(cmd.name);
                }
            }
            listCommandsExtraCmdPerms.Sorted = true;
            listCommandsExtraCmdPerms.Sorted = false;
        }

        private void txtGrpMOTD_TextChanged(object sender, EventArgs e) {
            if ( txtGrpMOTD.Text != null ) storedRanks[listRanks.SelectedIndex].MOTD = txtGrpMOTD.Text;
        }

        private void buttonEco_Click(object sender, EventArgs e) {
            new Gui.Eco.EconomyWindow().ShowDialog();
        }

        
        private void lstCommands_SelectedIndexChanged ( object sender, EventArgs e ) {
            btnUnload.Enabled = lstCommands.SelectedIndex != -1;
        }
        
        void sec_cbChatAuto_Checked(object sender, EventArgs e) {
            ToggleChatSpamSettings(sec_cbChatAuto.Checked);
        }

        void sec_cbCmdAuto_Checked(object sender, EventArgs e) {
            ToggleCmdSpamSettings(sec_cbCmdAuto.Checked);
        }
        
        void sec_cbBlocksAuto_Checked(object sender, EventArgs e) {
            ToggleBlocksSpamSettings(sec_cbBlocksAuto.Checked);
        }

        void sec_cbIPAuto_Checked(object sender, EventArgs e) {
            ToggleIPSpamSettings(sec_cbIPAuto.Checked);
        }
        
        void ToggleIrcSettings(bool enabled) {
            txtIRCServer.Enabled = enabled;
            txtIRCPort.Enabled = enabled;
            txtNick.Enabled = enabled;
            txtChannel.Enabled = enabled;
            txtOpChannel.Enabled = enabled;
            txtIrcId.Enabled = enabled;
            chkIrcId.Enabled = enabled;
            irc_cbTitles.Enabled = enabled;
        }

        void ToggleMySQLSettings(bool enabled) {
            txtSQLUsername.Enabled = enabled;
            txtSQLPassword.Enabled = enabled;
            txtSQLPort.Enabled = enabled;
            txtSQLHost.Enabled = enabled;
            txtSQLDatabase.Enabled = enabled;
        }
        
        void ToggleChatSpamSettings(bool enabled) {
            sec_numChatMsgs.Enabled = enabled;
            sec_numChatMute.Enabled = enabled;
            sec_numChatSecs.Enabled = enabled;
        }
        
        void ToggleCmdSpamSettings(bool enabled) {
            sec_numCmdMsgs.Enabled = enabled;
            sec_numCmdMute.Enabled = enabled;
            sec_numCmdSecs.Enabled = enabled;
        }
        
        void ToggleBlocksSpamSettings(bool enabled) {
            sec_numBlocksMsgs.Enabled = enabled;
            sec_numBlocksSecs.Enabled = enabled;
        }

        void ToggleIPSpamSettings(bool enabled) {
            sec_numIPMsgs.Enabled = enabled;
            sec_numIPMute.Enabled = enabled;
            sec_numIPSecs.Enabled = enabled;
        }
        
        void VerifyAdminsChecked(object sender, System.EventArgs e) {
            sec_cmbVerifyRank.Enabled = sec_cbVerifyAdmins.Checked;
        }
    }
}
