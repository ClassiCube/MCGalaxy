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

namespace MCGalaxy.Gui {

    public partial class PropertyWindow : Form {
        
        void LoadSecurityProps() {
            sec_cbLogNotes.Checked = ServerConfig.LogNotes;            
            sec_cbVerifyAdmins.Checked = ServerConfig.verifyadmins;
            sec_cbWhitelist.Checked = ServerConfig.WhitelistedOnly;            
            sec_cmbVerifyRank.Items.AddRange(GuiPerms.RankNames);
            GuiPerms.SetDefaultIndex(sec_cmbVerifyRank, ServerConfig.VerifyAdminsRank);
            sec_cmbVerifyRank.Enabled = ServerConfig.verifyadmins;
            
            sec_cbChatAuto.Checked = ServerConfig.ChatSpamCheck;
            sec_numChatMsgs.Value = ServerConfig.ChatSpamCount;
            sec_numChatSecs.Value = ServerConfig.ChatSpamInterval;
            sec_numChatMute.Value = ServerConfig.ChatSpamMuteTime;
            ToggleChatSpamSettings(ServerConfig.ChatSpamCheck);
            
            sec_cbCmdAuto.Checked = ServerConfig.CmdSpamCheck;
            sec_numCmdMsgs.Value = ServerConfig.CmdSpamCount;
            sec_numCmdSecs.Value = ServerConfig.CmdSpamInterval;
            sec_numCmdMute.Value = ServerConfig.CmdSpamBlockTime;
            ToggleCmdSpamSettings(ServerConfig.CmdSpamCheck);
            
            sec_cbBlocksAuto.Checked = ServerConfig.BlockSpamCheck;
            sec_numBlocksMsgs.Value = ServerConfig.BlockSpamCount;
            sec_numBlocksSecs.Value = ServerConfig.BlockSpamInterval;
            ToggleBlocksSpamSettings(ServerConfig.BlockSpamCheck);
            
            sec_cbIPAuto.Checked = ServerConfig.IPSpamCheck;
            sec_numIPMsgs.Value = ServerConfig.IPSpamCount;
            sec_numIPSecs.Value = ServerConfig.IPSpamInterval;
            sec_numIPMute.Value = ServerConfig.IPSpamBlockTime;
            ToggleIPSpamSettings(sec_cbIPAuto.Checked);
        }

        void ApplySecurityProps() {
            ServerConfig.LogNotes = sec_cbLogNotes.Checked;
            ServerConfig.verifyadmins = sec_cbVerifyAdmins.Checked;
            ServerConfig.VerifyAdminsRank = GuiPerms.GetPermission(sec_cmbVerifyRank, LevelPermission.Operator);
            ServerConfig.WhitelistedOnly = sec_cbWhitelist.Checked;
            if (ServerConfig.WhitelistedOnly && Server.whiteList == null)
                Server.whiteList = PlayerList.Load("whitelist.txt");
            
            ServerConfig.ChatSpamCheck = sec_cbChatAuto.Checked;
            ServerConfig.ChatSpamCount = (int)sec_numChatMsgs.Value;
            ServerConfig.ChatSpamInterval = (int)sec_numChatSecs.Value;
            ServerConfig.ChatSpamMuteTime = (int)sec_numChatMute.Value;
            
            ServerConfig.CmdSpamCheck = sec_cbCmdAuto.Checked;
            ServerConfig.CmdSpamCount = (int)sec_numCmdMsgs.Value;
            ServerConfig.CmdSpamInterval = (int)sec_numCmdSecs.Value;
            ServerConfig.CmdSpamBlockTime = (int)sec_numCmdMute.Value;
            
            ServerConfig.BlockSpamCheck = sec_cbBlocksAuto.Checked;
            ServerConfig.BlockSpamCount = (int)sec_numBlocksMsgs.Value;
            ServerConfig.BlockSpamInterval = (int)sec_numBlocksSecs.Value;
            
            ServerConfig.IPSpamCheck = sec_cbIPAuto.Checked;
            ServerConfig.IPSpamCount = (int)sec_numIPMsgs.Value;
            ServerConfig.IPSpamInterval = (int)sec_numIPSecs.Value;
            ServerConfig.IPSpamBlockTime = (int)sec_numIPMute.Value;
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
