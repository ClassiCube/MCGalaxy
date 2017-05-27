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
using MCGalaxy.Blocks;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {

        bool blockSupressEvents = true;
        ComboBox[] blockAllowBoxes, blockDisallowBoxes;
        BlockPerms blockPerms;
        
        void listBlocks_SelectedIndexChanged(object sender, EventArgs e) {
            byte b = Block.Byte(blk_list.SelectedItem.ToString());
            blockPerms = storedBlocks.Find(bS => bS.BlockID == b);
            BlockInitSpecificArrays();
            
            blockSupressEvents = true;
            GuiPerms.SetDefaultIndex(blk_cmbMin, blockPerms.MinRank);
            BlockSetSpecificPerms(blockPerms.Allowed, blockAllowBoxes);
            BlockSetSpecificPerms(blockPerms.Disallowed, blockDisallowBoxes);
            blockSupressEvents = false;
        }
        
        void BlockInitSpecificArrays() {
            if (blockAllowBoxes != null) return;
            blockAllowBoxes = new ComboBox[] { blk_cmbAlw1, blk_cmbAlw2, blk_cmbAlw3 };
            blockDisallowBoxes = new ComboBox[] { blk_cmbDis1, blk_cmbDis2, blk_cmbDis3 };
            
            for (int i = 0; i < blockAllowBoxes.Length; i++) {
                blockAllowBoxes[i].Items.Add("(add rank)");
                blockDisallowBoxes[i].Items.Add("(add rank)");
            }
        }
        
        void BlockSetSpecificPerms(List<LevelPermission> perms, ComboBox[] boxes) {
            ComboBox box = null;
            for (int i = 0; i < boxes.Length; i++) {
                box = boxes[i];
                // Hide the non-visible specific permissions
                box.Text = "";
                box.Enabled = false;
                box.Visible = false;
                
                // Show the non-visible specific permissions previously set
                if (perms.Count > i) {
                    box.Visible = true;
                    box.Enabled = true;
                    GuiPerms.SetDefaultIndex(box, perms[i]);
                }
            }
            
            // Show (add rank) for the last item
            if (perms.Count >= boxes.Length) return;
            BlockSetAddRank(boxes[perms.Count]);
        }
        
        void BlockSetAddRank(ComboBox box) {
            box.Visible = true;
            box.Enabled = true;
            box.SelectedIndex = box.Items.Count - 1;
        }       
        
        
        void blk_cmbMin_SelectedIndexChanged(object sender, EventArgs e) {
            int idx = blk_cmbMin.SelectedIndex;
            if (idx == -1 || blockSupressEvents) return;
            
            blockPerms.MinRank = GuiPerms.RankPerms[idx];
        }
        
        void blk_cmbSpecific_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox box = (ComboBox)sender;
            if (blockSupressEvents) return;
            int idx = box.SelectedIndex;
            if (idx == box.Items.Count - 1) return;
            
            List<LevelPermission> perms = blockPerms.Allowed;
            ComboBox[] boxes = blockAllowBoxes;
            int boxIdx = Array.IndexOf<ComboBox>(boxes, box);
            if (boxIdx == -1) {
                perms = blockPerms.Disallowed;
                boxes = blockDisallowBoxes;
                boxIdx = Array.IndexOf<ComboBox>(boxes, box);
            }
            
            if (boxIdx < perms.Count) {
                perms[boxIdx] = GuiPerms.RankPerms[idx];
            } else {
                perms.Add(GuiPerms.RankPerms[idx]);
            }
            
            // Activate next box
            if (boxIdx < boxes.Length - 1 && !boxes[boxIdx + 1].Visible) {
                BlockSetAddRank(boxes[boxIdx + 1]);
            }
        }

        void btnBlHelp_Click(object sender, EventArgs e) {
            getHelp(blk_list.SelectedItem.ToString());
        }
    }
}
