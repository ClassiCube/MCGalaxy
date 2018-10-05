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
            GuiPerms.SetDefaultIndex(rank_cmbDefault, Group.DefaultRank.Permission);
            GuiPerms.SetDefaultIndex(rank_cmbOsMap, Server.Config.OSPerbuildDefault);
            rank_cbTPHigher.Checked = Server.Config.HigherRankTP;
            rank_cbSilentAdmins.Checked = Server.Config.AdminsJoinSilently;
            rank_cbEmpty.Checked = Server.Config.ListEmptyRanks;
        }
        
        void ApplyRankProps() {
            Server.Config.DefaultRankName = rank_cmbDefault.SelectedItem.ToString();
            Server.Config.OSPerbuildDefault = GuiPerms.GetPermission(rank_cmbOsMap, LevelPermission.Nobody);
            Server.Config.HigherRankTP = rank_cbTPHigher.Checked;
            Server.Config.AdminsJoinSilently = rank_cbSilentAdmins.Checked;
            Server.Config.ListEmptyRanks = rank_cbEmpty.Checked;
        }
        
        
        List<Group> copiedGroups = new List<Group>();
        Group curGroup;
        void LoadRanks() {
            rank_list.Items.Clear();
            copiedGroups.Clear();
            curGroup = null;
            
            foreach (Group grp in Group.GroupList) {
                copiedGroups.Add(grp.CopyConfig());
                rank_list.Items.Add(grp.Name + " = " + (int)grp.Permission);
            }
            rank_list.SelectedIndex = 0;
        }
        
        void SaveRanks() {
            Group.SaveAll(copiedGroups);
            Group.LoadAll();
            LoadRanks();
        }
        
        
        void rank_btnColor_Click(object sender, EventArgs e) {
            chat_ShowColorDialog(rank_btnColor, curGroup.Name + " rank color");
            curGroup.Color = Colors.Parse(rank_btnColor.Text);
        }

        void rank_list_SelectedIndexChanged(object sender, EventArgs e) {
            if (rankSupressEvents) return;
            curGroup = null;
            if (rank_list.SelectedIndex == -1) return;
            
            Group grp = copiedGroups[rank_list.SelectedIndex];
            curGroup = grp;
            
            rank_txtName.Text = grp.Name;
            rank_numPerm.Value = (int)grp.Permission;
            chat_ParseColor(grp.Color, rank_btnColor);
            rank_txtMOTD.Text = grp.MOTD;
            rank_txtPrefix.Text = grp.Prefix;
            rank_cbAfk.Checked = grp.AfkKicked;
            rank_numAfk.Value = grp.AfkKickTime;
            
            rank_numDraw.Value = grp.DrawLimit;
            rank_numUndo.Value = grp.MaxUndo;
            rank_numMaps.Value = grp.OverseerMaps;
            rank_numGen.Value = grp.GenVolume;
            rank_numCopy.Value = grp.CopySlots;
        }

        void rank_txtName_TextChanged(object sender, EventArgs e) {
            if (rank_txtName.Text.IndexOf(' ') > 0) {
                rank_txtName.Text = rank_txtName.Text.Replace(" ", "");
                return;
            }
            if (rank_txtName.Text.Length == 0) return;
            
            curGroup.Name = rank_txtName.Text;
            rankSupressEvents = true;
            rank_list.Items[rank_list.SelectedIndex] = rank_txtName.Text + " = " + (int)curGroup.Permission;
            rankSupressEvents = false;
        }

        void rank_numPerm_ValueChanged(object sender, EventArgs e) {
            int perm = (int)rank_numPerm.Value;
            curGroup.Permission = (LevelPermission)perm;
            rankSupressEvents = true;
            rank_list.Items[rank_list.SelectedIndex] = curGroup.Name + " = " + perm;
            rankSupressEvents = false;
        }

        
        void rank_txtMOTD_TextChanged(object sender, EventArgs e) {
            curGroup.MOTD = rank_txtMOTD.Text;
        }
        
        void rank_txtPrefix_TextChanged(object sender, EventArgs e) {
            curGroup.Prefix = rank_txtPrefix.Text;
        }
        
        void rank_cbAfk_CheckedChanged(object sender, EventArgs e) {
            curGroup.AfkKicked = rank_cbAfk.Checked;
            rank_numAfk.Enabled = rank_cbAfk.Checked;
        }
        
        void rank_numAfk_ValueChanged(object sender, EventArgs e) {
            curGroup.AfkKickTime = rank_numAfk.Value;
        }       
        
        void rank_numDraw_ValueChanged(object sender, EventArgs e) {
            curGroup.DrawLimit = (int)rank_numDraw.Value;
        }
        
        void rank_numUndo_ValueChanged(object sender, EventArgs e) {
            curGroup.MaxUndo = rank_numUndo.Value;
        }
        
        void rank_numMaps_ValueChanged(object sender, EventArgs e) {
            curGroup.OverseerMaps = (int)rank_numMaps.Value;
        }
        
        void rank_numGen_ValueChanged(object sender, EventArgs e) {
            curGroup.GenVolume = (int)rank_numGen.Value;
        }
        
        void rank_numCopy_ValueChanged(object sender, EventArgs e) {
            curGroup.CopySlots = (int)rank_numCopy.Value;
        }
        
        void rank_btnAdd_Click(object sender, EventArgs e) {
            // Find first free rank permission
            int perm = 5;
            for (int i = (int)LevelPermission.Guest; i <= (int)LevelPermission.Nobody; i++) {
                if (PermissionFree(i)) { perm = i; break; }
            }
            
            Group newGroup = Group.DefaultRank.CopyConfig();
            newGroup.Permission = (LevelPermission)perm;
            newGroup.Name = "CHANGEME_" + perm;
            newGroup.Color = "&1";
            
            copiedGroups.Add(newGroup);
            rank_list.Items.Add(newGroup.Name + " = " + (int)newGroup.Permission);
        }

        void rank_btnDel_Click(object sender, EventArgs e) {
            if (rank_list.Items.Count == 0) return;
            
            copiedGroups.RemoveAt(rank_list.SelectedIndex);
            rankSupressEvents = true;
            rank_list.Items.RemoveAt(rank_list.SelectedIndex);
            rankSupressEvents = false;

            int i = rank_list.Items.Count > 0 ? 0 : -1;
            rank_list.SelectedIndex = i;
        }
        
        bool PermissionFree(int i) {
            foreach (Group grp in copiedGroups) {
                if (grp.Permission == (LevelPermission)i) return false;
            }
            return true;
        }
    }
}
