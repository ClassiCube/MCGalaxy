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
using System.Windows.Forms;
using MCGalaxy.Commands;

namespace MCGalaxy.Gui {

    public partial class PropertyWindow : Form {
        
        bool rankSupressEvents = false;
        
        void LoadRankProps() {
            GuiPerms.SetDefaultIndex(rank_cmbDefault, Group.standard.Permission);
            GuiPerms.SetDefaultIndex(rank_cmbOsMap, ServerConfig.OSPerbuildDefault);
            rank_cbTPHigher.Checked = ServerConfig.HigherRankTP;
            rank_cbSilentAdmins.Checked = ServerConfig.AdminsJoinSilently;
            rank_cbEmpty.Checked = ServerConfig.ListEmptyRanks;
        }
        
        void ApplyRankProps() {
            ServerConfig.DefaultRankName = rank_cmbDefault.SelectedItem.ToString();
            ServerConfig.OSPerbuildDefault = GuiPerms.GetPermission(rank_cmbOsMap, LevelPermission.Nobody);
            ServerConfig.HigherRankTP = rank_cbTPHigher.Checked;
            ServerConfig.AdminsJoinSilently = rank_cbSilentAdmins.Checked;
            ServerConfig.ListEmptyRanks = rank_cbEmpty.Checked;
        }
        
        
        List<Group> storedRanks = new List<Group>();
        void LoadRanks() {
            rank_list.Items.Clear();
            storedRanks.Clear();
            storedRanks.AddRange(Group.GroupList);
            foreach ( Group grp in storedRanks ) {
                rank_list.Items.Add(grp.Name + " = " + (int)grp.Permission);
            }
            rank_list.SelectedIndex = 0;
        }
        
        void SaveRanks() {
            Group.SaveList(storedRanks);
            Group.InitAll();
            LoadRanks();
        }
        
        
        void rank_btnColor_Click(object sender, EventArgs e) {
            chat_ShowColorDialog(rank_btnColor, storedRanks[rank_list.SelectedIndex].Name + " rank color");
            storedRanks[rank_list.SelectedIndex].Color = Colors.Parse(rank_btnColor.Text);
        }

        void rank_list_SelectedIndexChanged(object sender, EventArgs e) {
            if ( rankSupressEvents ) return;
            Group grp = storedRanks.Find(G => G.Name == rank_list.Items[rank_list.SelectedIndex].ToString().Split('=')[0].Trim());
            if ( grp.Permission == LevelPermission.Nobody ) { rank_list.SelectedIndex = 0; return; }

            rank_txtName.Text = grp.Name;
            rank_txtPerm.Text = ( (int)grp.Permission ).ToString();
            rank_numLimit.Value = grp.MaxBlocks;
            rank_numUndo.Value = grp.MaxUndo;
            chat_ParseColor(grp.Color, rank_btnColor);
            rank_txtMOTD.Text = String.IsNullOrEmpty(grp.MOTD) ? "" : grp.MOTD;
            rank_numOSMaps.Value = grp.OverseerMaps;
            rank_txtPrefix.Text = grp.Prefix;
        }

        void rank_txtName_TextChanged(object sender, EventArgs e) {
            if (rank_txtName.Text.IndexOf(' ') > 0) {
                rank_txtName.Text = rank_txtName.Text.Replace(" ", "");
                return;
            }
            
            if ( rank_txtName.Text != "" && rank_txtName.Text.ToLower() != "nobody" ) {
                storedRanks[rank_list.SelectedIndex].Name = rank_txtName.Text;
                rankSupressEvents = true;
                rank_list.Items[rank_list.SelectedIndex] = rank_txtName.Text + " = " + (int)storedRanks[rank_list.SelectedIndex].Permission;
                rankSupressEvents = false;
            }
        }

       void rank_txtPermission_TextChanged(object sender, EventArgs e) {
            if ( rank_txtPerm.Text != "" ) {
                int foundPerm;
                if (!int.TryParse(rank_txtPerm.Text, out foundPerm)) {
                    if ( rank_txtPerm.Text != "-" )
                        rank_txtPerm.Text = rank_txtPerm.Text.Remove(rank_txtPerm.Text.Length - 1);
                    return;
                }

                if ( foundPerm < -50 ) { rank_txtPerm.Text = "-50"; return; }
                else if ( foundPerm > 119 ) { rank_txtPerm.Text = "119"; return; }

                storedRanks[rank_list.SelectedIndex].Permission = (LevelPermission)foundPerm;
                rankSupressEvents = true;
                rank_list.Items[rank_list.SelectedIndex] = storedRanks[rank_list.SelectedIndex].Name + " = " + foundPerm;
                rankSupressEvents = false;
            }
        }
        
        void rank_numLimit_ValueChanged(object sender, EventArgs e) {
            storedRanks[rank_list.SelectedIndex].MaxBlocks = (int)rank_numLimit.Value;
        }
        
        void rank_numOSMaps_ValueChanged(object sender, EventArgs e) {
            storedRanks[rank_list.SelectedIndex].OverseerMaps = (byte)rank_numOSMaps.Value;
        }
        
        void rank_numUndo_ValueChanged(object sender, EventArgs e) {
            storedRanks[rank_list.SelectedIndex].MaxUndo = (int)rank_numUndo.Value;
        }
        
        void rank_txtPrefix_TextChanged(object sender, EventArgs e) {
            storedRanks[rank_list.SelectedIndex].Prefix = rank_txtPrefix.Text;
        }
        
        void rank_txtMOTD_TextChanged(object sender, EventArgs e) {
            storedRanks[rank_list.SelectedIndex].MOTD = rank_txtMOTD.Text;
        }

        void rank_btnAdd_Click(object sender, EventArgs e) {
            // Find first free rank permission
            int freePerm = 5;
            for (int i = (int)LevelPermission.Guest; i <= (int)LevelPermission.Nobody; i++) {
                if (Group.Find(i) != null) continue;
                
                freePerm = i; break;
            }
            
            Group newGroup = new Group((LevelPermission)freePerm, 600, 30, "CHANGEME", '1', "", null);
            storedRanks.Add(newGroup);
            rank_list.Items.Add(newGroup.Name + " = " + (int)newGroup.Permission);
        }

        void rank_btnDel_Click(object sender, EventArgs e) {
            if (rank_list.Items.Count <= 1) return;
            
            storedRanks.RemoveAt(rank_list.SelectedIndex);
            rankSupressEvents = true;
            rank_list.Items.RemoveAt(rank_list.SelectedIndex);
            rankSupressEvents = false;

            rank_list.SelectedIndex = 0;
        }
    }
}
