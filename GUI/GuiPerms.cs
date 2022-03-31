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

namespace MCGalaxy.Gui
{
    public class GuiRank
    {
        public string Name;
        public LevelPermission Permission;
        public GuiRank(string name, LevelPermission perm) { Name = name; Permission = perm; }

        public override string ToString() { return Name; }
    }

    internal static class GuiPerms 
    {
        static List<GuiRank> Ranks, RanksRemove;
        
        internal static void UpdateRanks() {
            Ranks = new List<GuiRank>(Group.AllRanks.Count);
            
            foreach (Group group in Group.AllRanks) 
            {
                Ranks.Add(new GuiRank(group.Name, group.Permission));
            }

            RanksRemove = new List<GuiRank>(Ranks);
            RanksRemove.Add(new GuiRank("(remove rank)", LevelPermission.Null));
        }
        
        internal static LevelPermission GetSelectedRank(ComboBox box, LevelPermission defPerm) {
            GuiRank rank = (GuiRank)box.SelectedItem;
            return rank == null ? defPerm : rank.Permission;
        }
        
        internal static void SetSelectedRank(ComboBox box, LevelPermission perm) {
            List<GuiRank> ranks = (List<GuiRank>)box.DataSource;
            GuiRank rank = ranks.Find(r => r.Permission == perm);

            if (rank == null) {
                box.SelectedIndex = 1;
            } else {
                box.SelectedItem  = rank;
            }
        }

        internal static void SetRanks(ComboBox box, bool removeRank = false) {
            List<GuiRank> ranks = removeRank ? RanksRemove : Ranks;
            box.DisplayMember = "Name";
            box.ValueMember   = "Permission";
            // run into issues otherwise if multiple combo boxes share same source
            box.DataSource    = new List<GuiRank>(ranks);
        }
        
        internal static void SetRanks(ComboBox[] boxes, bool removeRank = false) {
            foreach (ComboBox box in boxes)
            {
                SetRanks(box, removeRank);
            }
        }
    }
}

