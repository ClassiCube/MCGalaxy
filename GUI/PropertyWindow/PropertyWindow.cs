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
            
            ToggleIrcSettings(Server.irc);
            ToggleMySQLSettings(Server.useMySQL);
            ToggleChatSpamSettings(Server.checkspam);
            ToggleCmdSpamSettings(Server.CmdSpamCheck);
            ToggleBlocksSpamSettings(Server.BlockSpamCheck);

            GuiPerms.UpdateRankNames();
            rank_cmbDefault.Items.AddRange(GuiPerms.RankNames);
            rank_cmbOpChat.Items.AddRange(GuiPerms.RankNames);
            rank_cmbAdminChat.Items.AddRange(GuiPerms.RankNames);
            rank_cmbOsMap.Items.AddRange(GuiPerms.RankNames);
            sec_cmbVerifyRank.Items.AddRange(GuiPerms.RankNames);
            afk_cmbKickPerm.Items.AddRange(GuiPerms.RankNames);
            blk_cmbMin.Items.AddRange(GuiPerms.RankNames);
            cmd_cmbMin.Items.AddRange(GuiPerms.RankNames);

            //Load server stuff
            LoadProperties("properties/server.properties");
            LoadRanks();
            try {
                LoadCommands();
                LoadBlocks();
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
        }

        void PropertyWindow_Unload(object sender, EventArgs e) {
            lavaUpdateTimer.Dispose();
            Window.prevLoaded = false;
            TntWarsGame.GuiLoaded = null;
        }

        void LoadProperties(string givenPath) {
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

        void OnlyAddDigit(TextBox box) {
        	if (box.TextLength == 0) return;
        	
        	string lastChar = box.Text[box.TextLength - 1].ToString();
        	byte ignored;
        	if (byte.TryParse(lastChar, out ignored)) return;
        	
        	box.Text = box.Text.Substring(0, box.TextLength - 1);
        }

        void btnSave_Click(object sender, EventArgs e) { SaveChanges(); Dispose(); }
        void btnApply_Click(object sender, EventArgs e) { SaveChanges(); }

        void SaveChanges() {
            SaveProperties();
            SaveRanks();
            SaveCommands();
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

        void btnDiscard_Click(object sender, EventArgs e) { Dispose(); }

       void GetHelp(string toHelp) {
            Player.storedHelp = "";
            Player.storeHelp = true;
            Command.all.Find("help").Use(null, toHelp);
            Player.storeHelp = false;
            
            MessageBox.Show(Colors.StripColors(Player.storedHelp),
                            "Help information for " + toHelp);
        }
    }
}
