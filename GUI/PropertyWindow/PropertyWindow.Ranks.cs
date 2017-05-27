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

namespace MCGalaxy.Gui {

    public partial class PropertyWindow : Form {  
        
        void LoadRankProps() {
            GuiPerms.SetDefaultIndex(rank_cmbDefault, Group.standard.Permission);
            GuiPerms.SetDefaultIndex(rank_cmbOsMap, Server.osPerbuildDefault);
            
            LevelPermission adminChatRank =
                CommandExtraPerms.MinPerm("adminchat", LevelPermission.Admin);
            LevelPermission opChatRank =
                CommandExtraPerms.MinPerm("opchat", LevelPermission.Operator);
            GuiPerms.SetDefaultIndex(rank_cmbOpChat, opChatRank);
            GuiPerms.SetDefaultIndex(rank_cmbAdminChat, adminChatRank);            

            rank_chkTpToHigherRanks.Checked = Server.higherranktp;
            chkAdminsJoinSilent.Checked = Server.adminsjoinsilent;
        }
        
        void ApplyRankProps() {
            Server.defaultRank = rank_cmbDefault.SelectedItem.ToString();
            Server.higherranktp = rank_chkTpToHigherRanks.Checked;
            Server.adminsjoinsilent = chkAdminsJoinSilent.Checked;
            
            Server.osPerbuildDefault = GuiPerms.GetPermission(rank_cmbOsMap, LevelPermission.Nobody);
            var perms = CommandExtraPerms.Find("opchat");
            perms.MinRank = GuiPerms.GetPermission(rank_cmbOpChat, LevelPermission.Operator);
            perms = CommandExtraPerms.Find("adminchat");
            perms.MinRank = GuiPerms.GetPermission(rank_cmbAdminChat, LevelPermission.Admin);
        }
		
		
		
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
        
        private void txtPrefix_TextChanged(object sender, EventArgs e) {
            storedRanks[listRanks.SelectedIndex].prefix = txtPrefix.Text;
        }

        private void btnAddRank_Click(object sender, EventArgs e) {
            // Find first free rank permission
            int freePerm = 5;
            for (int i = (int)LevelPermission.Guest; i <= (int)LevelPermission.Nobody; i++) {
                if (Group.findPermInt(i) != null) continue;
                
                freePerm = i; break;
            }
            
            Group newGroup = new Group((LevelPermission)freePerm, 600, 30, "CHANGEME", '1', "", null);
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
    }
}
