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
            sec_cbLogNotes.Checked = Server.Config.LogNotes;            
            sec_cbVerifyAdmins.Checked = Server.Config.verifyadmins;
            sec_cbWhitelist.Checked = Server.Config.WhitelistedOnly;            
            sec_cmbVerifyRank.Items.AddRange(GuiPerms.RankNames);
            GuiPerms.SetDefaultIndex(sec_cmbVerifyRank, Server.Config.VerifyAdminsRank);
            sec_cmbVerifyRank.Enabled = Server.Config.verifyadmins;
            
            sec_cbChatAuto.Checked = Server.Config.ChatSpamCheck;
            sec_numChatMsgs.Value = Server.Config.ChatSpamCount;
            sec_numChatSecs.Value = Server.Config.ChatSpamInterval;
            sec_numChatMute.Value = Server.Config.ChatSpamMuteTime;
            ToggleChatSpamSettings(Server.Config.ChatSpamCheck);
            
            sec_cbCmdAuto.Checked = Server.Config.CmdSpamCheck;
            sec_numCmdMsgs.Value = Server.Config.CmdSpamCount;
            sec_numCmdSecs.Value = Server.Config.CmdSpamInterval;
            sec_numCmdMute.Value = Server.Config.CmdSpamBlockTime;
            ToggleCmdSpamSettings(Server.Config.CmdSpamCheck);
            
            sec_cbBlocksAuto.Checked = Server.Config.BlockSpamCheck;
            sec_numBlocksMsgs.Value = Server.Config.BlockSpamCount;
            sec_numBlocksSecs.Value = Server.Config.BlockSpamInterval;
            ToggleBlocksSpamSettings(Server.Config.BlockSpamCheck);
            
            sec_cbIPAuto.Checked = Server.Config.IPSpamCheck;
            sec_numIPMsgs.Value = Server.Config.IPSpamCount;
            sec_numIPSecs.Value = Server.Config.IPSpamInterval;
            sec_numIPMute.Value = Server.Config.IPSpamBlockTime;
            ToggleIPSpamSettings(sec_cbIPAuto.Checked);
        }

        void ApplySecurityProps() {
            Server.Config.LogNotes = sec_cbLogNotes.Checked;
            Server.Config.verifyadmins = sec_cbVerifyAdmins.Checked;
            Server.Config.VerifyAdminsRank = GuiPerms.GetPermission(sec_cmbVerifyRank, LevelPermission.Operator);
            Server.Config.WhitelistedOnly  = sec_cbWhitelist.Checked;

            Server.Config.ChatSpamCheck = sec_cbChatAuto.Checked;
            Server.Config.ChatSpamCount = (int)sec_numChatMsgs.Value;
            Server.Config.ChatSpamInterval = sec_numChatSecs.Value;
            Server.Config.ChatSpamMuteTime = sec_numChatMute.Value;
            
            Server.Config.CmdSpamCheck = sec_cbCmdAuto.Checked;
            Server.Config.CmdSpamCount = (int)sec_numCmdMsgs.Value;
            Server.Config.CmdSpamInterval = sec_numCmdSecs.Value;
            Server.Config.CmdSpamBlockTime = sec_numCmdMute.Value;
            
            Server.Config.BlockSpamCheck = sec_cbBlocksAuto.Checked;
            Server.Config.BlockSpamCount = (int)sec_numBlocksMsgs.Value;
            Server.Config.BlockSpamInterval = sec_numBlocksSecs.Value;
            
            Server.Config.IPSpamCheck = sec_cbIPAuto.Checked;
            Server.Config.IPSpamCount = (int)sec_numIPMsgs.Value;
            Server.Config.IPSpamInterval = sec_numIPSecs.Value;
            Server.Config.IPSpamBlockTime = sec_numIPMute.Value;
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
