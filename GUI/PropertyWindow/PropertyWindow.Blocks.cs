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
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {

        bool blockSupressEvents = true;
        ComboBox[] blockAllowBoxes, blockDisallowBoxes;
        BlockID curBlock;
        List<BlockID> blockIDMap;
        
        // need to keep a list of changed block perms, because we don't want
        // to modify the server's live permissions if user clicks 'discard'
        BlockPerms blockPermsOrig, blockPerms;
        List<BlockPerms> blockPermsChanged = new List<BlockPerms>();
        BlockProps[] blockPropsChanged = new BlockProps[Block.Props.Length];
        
        void LoadBlocks() {
            blk_list.Items.Clear();
            blockPermsChanged.Clear();
            blockIDMap = new List<BlockID>();
            
            for (int b = 0; b < blockPropsChanged.Length; b++) {
                blockPropsChanged[b] = Block.Props[b];
                blockPropsChanged[b].ChangedScope = 0;
                
                BlockID block = (BlockID)b;
                if (!Block.ExistsGlobal(block)) continue;
                
                string name = Block.GetName(null, block);
                blk_list.Items.Add(name);
                blockIDMap.Add(block);
            }
            
            if (blk_list.SelectedIndex == -1) {
                blk_list.SelectedIndex = 0;
            }
        }

        void SaveBlocks() {
            if (!BlocksChanged()) { LoadBlocks(); return; }
            
            for (int b = 0; b < blockPropsChanged.Length; b++) {
                if (blockPropsChanged[b].ChangedScope == 0) continue;
                Block.Props[b] = blockPropsChanged[b];
            }
            
            foreach (BlockPerms perms in blockPermsChanged) {
                BlockPerms.List[perms.ID] = perms;
            }
            BlockPerms.ResendAllBlockPermissions();
            
            BlockProps.Save("default", Block.Props, Block.PropsLock, 1);
            BlockPerms.Save();
            Block.SetBlocks();
            LoadBlocks();
        }
        
        bool BlocksChanged() {
            for (int b = 0; b < blockPropsChanged.Length; b++) {
                if (blockPropsChanged[b].ChangedScope != 0) return true;
            }
            return blockPermsChanged.Count > 0;
        }
        
        
        void blk_list_SelectedIndexChanged(object sender, EventArgs e) {
            curBlock = blockIDMap[blk_list.SelectedIndex];
            blockPermsOrig = BlockPerms.List[curBlock];
            blockPerms = blockPermsChanged.Find(p => p.ID == curBlock);
            BlockInitSpecificArrays();
            blockSupressEvents = true;
            
            BlockProps props = blockPropsChanged[curBlock];
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
            blockPropsChanged[curBlock].IsMessageBlock = blk_cbMsgBlock.Checked;
            MarkBlockPropsChanged();
        }
        
        void blk_cbPortal_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[curBlock].IsPortal = blk_cbPortal.Checked;
            MarkBlockPropsChanged();
        }
        
        void blk_cbDeath_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[curBlock].KillerBlock = blk_cbDeath.Checked;
            blk_txtDeath.Enabled = blk_cbDeath.Checked;
            MarkBlockPropsChanged();
        }
        
        void blk_txtDeath_TextChanged(object sender, EventArgs e) {
            blockPropsChanged[curBlock].DeathMessage = blk_txtDeath.Text;
            MarkBlockPropsChanged();
        }
        
        void blk_cbDoor_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[curBlock].IsDoor = blk_cbDoor.Checked;
            MarkBlockPropsChanged();
        }
        
        void blk_cbTdoor_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[curBlock].IsTDoor = blk_cbTdoor.Checked;
            MarkBlockPropsChanged();
        }
        
        void blk_cbRails_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[curBlock].IsRails = blk_cbRails.Checked;
            MarkBlockPropsChanged();
        }
        
        void blk_cbLava_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[curBlock].LavaKills = blk_cbLava.Checked;
            MarkBlockPropsChanged();
        }
        
        void blk_cbWater_CheckedChanged(object sender, EventArgs e) {
            blockPropsChanged[curBlock].WaterKills = blk_cbWater.Checked;
            MarkBlockPropsChanged();
        }
        
        void MarkBlockPropsChanged() {
            // don't mark props as changed when supressing events
            int changed = blockSupressEvents ? 0 : 1;
            blockPropsChanged[curBlock].ChangedScope = (byte)changed;
        }
    }
}
