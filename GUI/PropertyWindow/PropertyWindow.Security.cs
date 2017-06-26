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
            sec_cbLogNotes.Checked = Server.LogNotes;            
            sec_cbVerifyAdmins.Checked = Server.verifyadmins;
            sec_cbWhitelist.Checked = Server.useWhitelist;            
            sec_cmbVerifyRank.Items.AddRange(GuiPerms.RankNames);
            GuiPerms.SetDefaultIndex(sec_cmbVerifyRank, Server.verifyadminsrank);
            sec_cmbVerifyRank.Enabled = Server.verifyadmins;
            
            sec_cbChatAuto.Checked = Server.checkspam;
            sec_numChatMsgs.Value = Server.spamcounter;
            sec_numChatSecs.Value = Server.spamcountreset;
            sec_numChatMute.Value = Server.mutespamtime;
            
            sec_cbCmdAuto.Checked = Server.CmdSpamCheck;
            sec_numCmdMsgs.Value = Server.CmdSpamCount;
            sec_numCmdSecs.Value = Server.CmdSpamInterval;
            sec_numCmdMute.Value = Server.CmdSpamBlockTime;
            
            sec_cbBlocksAuto.Checked = Server.BlockSpamCheck;
            sec_numBlocksMsgs.Value = Server.BlockSpamCount;
            sec_numBlocksSecs.Value = Server.BlockSpamInterval;
            
            sec_cbIPAuto.Checked = Server.IPSpamCheck;
            sec_numIPMsgs.Value = Server.IPSpamCount;
            sec_numIPSecs.Value = Server.IPSpamInterval;
            sec_numIPMute.Value = Server.IPSpamBlockTime;
        }

        void ApplySecurityProps() {
            Server.LogNotes = sec_cbLogNotes.Checked;
            Server.verifyadmins = sec_cbVerifyAdmins.Checked;
            Server.verifyadminsrank = GuiPerms.GetPermission(sec_cmbVerifyRank, LevelPermission.Operator);
            Server.useWhitelist = sec_cbWhitelist.Checked;
            if (Server.useWhitelist && Server.whiteList == null)
                Server.whiteList = PlayerList.Load("whitelist.txt");
            
            Server.checkspam = sec_cbChatAuto.Checked;
            Server.spamcounter = (int)sec_numChatMsgs.Value;
            Server.spamcountreset = (int)sec_numChatSecs.Value;
            Server.mutespamtime = (int)sec_numChatMute.Value;
            
            Server.CmdSpamCheck = sec_cbCmdAuto.Checked;
            Server.CmdSpamCount = (int)sec_numCmdMsgs.Value;
            Server.CmdSpamInterval = (int)sec_numCmdSecs.Value;
            Server.CmdSpamBlockTime = (int)sec_numCmdMute.Value;
            
            Server.BlockSpamCheck = sec_cbBlocksAuto.Checked;
            Server.BlockSpamCount = (int)sec_numBlocksMsgs.Value;
            Server.BlockSpamInterval = (int)sec_numBlocksSecs.Value;
            
            Server.IPSpamCheck = sec_cbIPAuto.Checked;
            Server.IPSpamCount = (int)sec_numIPMsgs.Value;
            Server.IPSpamInterval = (int)sec_numIPSecs.Value;
            Server.IPSpamBlockTime = (int)sec_numIPMute.Value;
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
