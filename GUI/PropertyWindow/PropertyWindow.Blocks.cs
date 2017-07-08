/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Blocks;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {

        bool blockSupressEvents = true;
        ComboBox[] blockAllowBoxes, blockDisallowBoxes;
        byte blockID;
        
        // need to keep a list of changed block perms, because we don't want
        // to modify the server's live permissions if user clicks 'discard'
        BlockPerms blockPermsOrig, blockPerms;
        List<BlockPerms> blockPermsChanged = new List<BlockPerms>();
        BlockProps[] blockPropsChanged = new BlockProps[256];        
        
        void LoadBlocks() {
            blk_list.Items.Clear();
            blockPermsChanged.Clear();
            for (int i = 0; i < Block.Props.Length; i++) {
                blockPropsChanged[i] = Block.Props[i];
                blockPropsChanged[i].Changed = false;
                
                if (Block.Props[i].Name != "unknown") {
                    blk_list.Items.Add(Block.Props[i].Name);
                }
            }
            
            if (blk_list.SelectedIndex == -1)
                blk_list.SelectedIndex = 0;
        }

       void SaveBlocks() {
            if (!BlocksChanged()) { LoadBlocks(); return; }
            
            for (int i = 0; i < blockPropsChanged.Length; i++) {
                if (!blockPropsChanged[i].Changed) continue;
                Block.Props[i] = blockPropsChanged[i];
            }
            foreach (BlockPerms perms in blockPermsChanged) {
                BlockPerms.List[perms.BlockID] = perms;
                if (perms.BlockID < Block.CpeCount) {
                    BlockPerms.ResendBlockPermissions(perms.BlockID);
                }
            }
            
            BlockProps.Save("core", Block.Props, i => true);
            BlockPerms.Save();
            Block.SetBlocks();
            LoadBlocks();
        }
        
        bool BlocksChanged() {
            for (int i = 0; i < blockPropsChanged.Length; i++) {
                if (blockPropsChanged[i].Changed) return true;
            }
            return blockPermsChanged.Count > 0;
        }
        
        
        void blk_list_SelectedIndexChanged(object sender, EventArgs e) {
            blockID = Block.Byte(blk_list.SelectedItem.ToString());
            blockPermsOrig = BlockPerms.List[blockID];
            blockPerms = blockPermsChanged.Find(p => p.BlockID == blockID);
            BlockInitSpecificArrays();
            blockSupressEvents = true;
            
            BlockProps props = blockPropsChanged[blockID];
            blk_cbMsgBlock.Checked = props.IsMessageBlock;
            blk_cbPortal.Checked = props.IsPortal;
            blk_cbDeath.Checked = props.KillerBlock;
            blk_txtDeath.Text = props.DeathMessage;
            blk_txtDeath.Enabled = blk_cbDeath.Checked;
            
            blk_cbDoor.Checked = props.IsDoor;
            blk_cbTdoor.Checked = props.IsTDoor;
            blk_cbRails.Checked = props.IsRails;
            blk_cbLava.Checked = props.LavaKills;
            blk_cbWater.Checked = props.WaterKills;
                        
            BlockPerms perms = blockPerms != null ? blockPerms : blockPermsOrig;
            GuiPerms.SetDefaultIndex(blk_cmbMin, perms.MinRank);
            GuiPerms.SetSpecificPerms(perms.Allowed, blockAllowBoxes);
            GuiPerms.SetSpecificPerms(perms.Disallowed, blockDisallowBoxes);
            blockSupressEvents = false;
        }
        
        void BlockInitSpecificArrays() {
            if (blockAllowBoxes != null) return;
            blockAllowBoxes = new ComboBox[] { blk_cmbAlw1, blk_cmbAlw2, blk_cmbAlw3 };
            blockDisallowBoxes = new ComboBox[] { blk_cmbDis1, blk_cmbDis2, blk_cmbDis3 };
            GuiPerms.FillRanks(blockAllowBoxes);
            GuiPerms.FillRanks(blockDisallowBoxes);
        }
        
        void BlockGetOrAddPermsChanged() {
            if (blockPerms != null) return;
            blockPerms = blockPermsOrig.Copy();
            blockPermsChanged.Add(blockPerms);
        }
        
        
        void blk_cmbMin_SelectedIndexChanged(object sender, EventArgs e) {
            int idx = blk_cmbMin.SelectedIndex;
            if (idx == -1 || blockSupressEvents) return;
            BlockGetOrAddPermsChanged();
            
            blockPerms.MinRank = GuiPerms.RankPerms[idx];
        }
        
        void blk_cmbSpecific_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox box = (ComboBox)sender;
            if (blockSupressEvents) return;
            int idx = box.SelectedIndex;
            if (idx == -1) return;
            BlockGetOrAddPermsChanged();
            
            List<LevelPermission> perms = blockPerms.Allowed;
            ComboBox[] boxes = blockAllowBoxes;
            int boxIdx = Array.IndexOf<ComboBox>(boxes, box);
            if (boxIdx == -1) {
                perms = blockPerms.Disallowed;
                boxes = blockDisallowBoxes;
                boxIdx = Array.IndexOf<ComboBox>(boxes, box);
            }
            
            if (idx == box.Items.Count - 1) {
                if (boxIdx >= perms.Count) return;
                perms.RemoveAt(boxIdx);
                
                blockSupressEvents = true;
                GuiPerms.SetSpecificPerms(perms, boxes);
                blockSupressEvents = false;
            } else {
                GuiPerms.SetSpecific(boxes, boxIdx, perms, idx);
            }
        }

        void blk_btnHelp_Click(object sender, EventArgs e) {
            GetHelp(blk_list.SelectedItem.ToString());
        }
        
        
        void blk_cbMsgBlock_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].IsMessageBlock = blk_cbMsgBlock.Checked;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
        
        void blk_cbPortal_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].IsPortal = blk_cbPortal.Checked;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
        
        void blk_cbDeath_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].KillerBlock = blk_cbDeath.Checked;
            blk_txtDeath.Enabled = blk_cbDeath.Checked;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
        
        void blk_txtDeath_TextChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].DeathMessage = blk_txtDeath.Text;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
        
        void blk_cbDoor_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].IsDoor = blk_cbDoor.Checked;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
        
        void blk_cbTdoor_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].IsTDoor = blk_cbTdoor.Checked;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
        
        void blk_cbRails_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].IsRails = blk_cbRails.Checked;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
        
        void blk_cbLava_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].LavaKills = blk_cbLava.Checked;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
        
        void blk_cbWater_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[blockID].WaterKills = blk_cbWater.Checked;
            blockPropsChanged[blockID].Changed = !blockSupressEvents;
        }
    }
}
