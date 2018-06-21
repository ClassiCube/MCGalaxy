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
    internal static class GuiPerms {
        
        internal static string[] RankNames;
        internal static LevelPermission[] RankPerms;
        
        internal static void UpdateRankNames() {
            List<string> names = new List<string>(Group.GroupList.Count);
            List<LevelPermission> perms = new List<LevelPermission>(Group.GroupList.Count);
            
            foreach (Group group in Group.GroupList) {
                names.Add(group.Name);
                perms.Add(group.Permission);
            }
            RankNames = names.ToArray();
            RankPerms = perms.ToArray();
        }
        
        internal static LevelPermission GetPermission(ComboBox box, LevelPermission defPerm) {
            Group grp = Group.Find(box.SelectedItem.ToString());
            return grp == null ? defPerm : grp.Permission;
        }
        
        internal static void SetDefaultIndex(ComboBox box, LevelPermission perm) {
            Group grp = Group.Find(perm);
            if (grp == null) {
                box.SelectedIndex = 1;
            } else {
                int idx = Array.IndexOf<string>(RankNames, grp.Name);
                box.SelectedIndex = idx >= 0 ? idx : 1;
            }
        }
        
        internal static void FillRanks(ComboBox[] boxes, bool removeRank = true) {
            for (int i = 0; i < boxes.Length; i++) {
                boxes[i].Items.AddRange(RankNames);
                if (!removeRank) continue;
                boxes[i].Items.Add("(remove rank)");
            }
        }
    }
}

