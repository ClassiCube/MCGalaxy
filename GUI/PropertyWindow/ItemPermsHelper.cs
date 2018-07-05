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

namespace MCGalaxy.Gui {
    public sealed class ItemPermsHelper {
        public ComboBox MinBox;
        public ComboBox[] AllowBoxes, DisallowBoxes;
        public bool SupressEvents = true;
        public Func<ItemPerms> GetCurPerms;
        
        public void Update(ItemPerms perms) {
            SupressEvents = true;
            GuiPerms.SetDefaultIndex(MinBox, perms.MinRank);
            SetSpecificPerms(perms.Allowed, AllowBoxes);
            SetSpecificPerms(perms.Disallowed, DisallowBoxes);
            SupressEvents = false;
        }
        
        public void FillInitial() {
            GuiPerms.FillRanks(AllowBoxes);
            GuiPerms.FillRanks(DisallowBoxes);
        }
        
        public void OnMinRankChanged(ComboBox box) {
            int idx = box.SelectedIndex;
            if (idx == -1 || SupressEvents) return;
            ItemPerms curPerms = GetCurPerms();
            
            curPerms.MinRank = GuiPerms.RankPerms[idx];
        }
        
        public void OnSpecificChanged(ComboBox box) {
            int idx = box.SelectedIndex;
            if (idx == -1 || SupressEvents) return;
            ItemPerms curPerms = GetCurPerms();
            
            List<LevelPermission> perms;
            ComboBox[] boxes;            
            int boxIdx = Array.IndexOf<ComboBox>(AllowBoxes, box);
            
            if (boxIdx == -1) {
                if (curPerms.Disallowed == null)
                    curPerms.Disallowed = new List<LevelPermission>();
                
                perms = curPerms.Disallowed;
                boxes = DisallowBoxes;
                boxIdx = Array.IndexOf<ComboBox>(DisallowBoxes, box);
            } else {
                if (curPerms.Allowed == null)
                    curPerms.Allowed = new List<LevelPermission>();
                
                perms = curPerms.Allowed;
                boxes = AllowBoxes;
            }
            
            if (idx == box.Items.Count - 1) {
                if (boxIdx >= perms.Count) return;
                perms.RemoveAt(boxIdx);
                
                SupressEvents = true;
                SetSpecificPerms(perms, boxes);
                SupressEvents = false;
            } else {
                SetSpecific(boxes, boxIdx, perms, idx);
            }
        }
        
        static void SetSpecific(ComboBox[] boxes, int boxIdx, List<LevelPermission> perms, int idx) {
            if (boxIdx < perms.Count) {
                perms[boxIdx] = GuiPerms.RankPerms[idx];
            } else {
                perms.Add(GuiPerms.RankPerms[idx]);
            }
            
            // Activate next box
            if (boxIdx < boxes.Length - 1 && !boxes[boxIdx + 1].Visible) {
                SetAddRank(boxes[boxIdx + 1]);
            }
        }
        
        static void SetAddRank(ComboBox box) {
            box.Visible = true;
            box.Enabled = true;
            box.Text = "(add rank)";
        }
        
        static void SetSpecificPerms(List<LevelPermission> perms, ComboBox[] boxes) {
            ComboBox box = null;
            int permsCount = perms == null ? 0 : perms.Count;
            
            for (int i = 0; i < boxes.Length; i++) {
                box = boxes[i];
                // Hide the non-visible specific permissions
                box.Text = "";
                box.Enabled = false;
                box.Visible = false;
                box.SelectedIndex = -1;
                
                // Show the non-visible specific permissions previously set
                if (permsCount > i) {
                    box.Visible = true;
                    box.Enabled = true;
                    GuiPerms.SetDefaultIndex(box, perms[i]);
                }
            }
            
            // Show (add rank) for the last item
            if (permsCount >= boxes.Length) return;
            SetAddRank(boxes[permsCount]);
        }
    }
}
