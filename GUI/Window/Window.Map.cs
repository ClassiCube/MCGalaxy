/*    
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.UI;

namespace MCGalaxy.Gui {
    public partial class Window : Form {
        
        void map_BtnGen_Click(object sender, EventArgs e) {
            if (mapgen) { MessageBox.Show("Another map is already being generated."); return; }

            string name = map_txtName.Text.ToLower();
            string seed = map_txtSeed.Text.ToLower();
            if (String.IsNullOrEmpty(name)) { MessageBox.Show("Map name cannot be blank."); return; }
            
            string x = Map_GetComboboxItem(map_cmbX, "width");
            if (x == null) return;           
            string y = Map_GetComboboxItem(map_cmbY, "height");
            if (y == null) return;            
            string z = Map_GetComboboxItem(map_cmbZ, "length");
            if (z == null) return;            
            string type = Map_GetComboboxItem(map_cmbType, "type");
            if (type == null) return;            

            Thread genThread = new Thread(() =>
            {
                mapgen = true;
                try {
                    string args = name + " " + x + " " + y + " " + z + " " + type;
                    if (!String.IsNullOrEmpty(seed)) args += " " + seed;
                    Command.all.FindByName("NewLvl").Use(null, args);
                } catch (Exception ex) {
                    Logger.LogError(ex);
                    MessageBox.Show("Failed to generate level. Check error logs for details.");
                    mapgen = false;
                    return;
                }

                if (LevelInfo.MapExists(name)) {
                    MessageBox.Show("Level successfully generated.");
                    RunOnUI_Async(() => {
                        Map_UpdateUnloadedList();
                        Map_UpdateLoadedList();
                        Main_UpdateMapList();
                    });
                } else {
                    MessageBox.Show("Level was not generated. Check main log for details.");
                }
                mapgen = false;
            });
            genThread.Name = "MCG_GuiGenMap";
            genThread.Start();
        }
        
        string Map_GetComboboxItem(ComboBox box, string propName) {
            object selected = box.SelectedItem;
            string value = selected == null ? "" : selected.ToString().ToLower();
            
            if (value.Length == 0) {
                MessageBox.Show("Map " + propName + " cannot be blank.");
                return null;
            }
            return value;
        }
        
        void map_BtnLoad_Click(object sender, EventArgs e) {
            object selected = map_lbUnloaded.SelectedItem;
            if (selected == null) { MessageBox.Show("No map file selected"); return; }

            UIHelpers.HandleCommand("Load " +selected.ToString());
        }
        
        string last = null;
        void Map_UpdateSelected(object sender, EventArgs e) {
            if (map_lbLoaded.SelectedItem == null) {
                if (map_pgProps.SelectedObject == null) return;
                map_pgProps.SelectedObject = null; last = null;
                map_gbProps.Text = "Properties for (none selected)"; return;
            }
            
            string name = map_lbLoaded.SelectedItem.ToString();
            Level lvl = LevelInfo.FindExact(name);
            if (lvl == null) {
                if (map_pgProps.SelectedObject == null) return;
                map_pgProps.SelectedObject = null; last = null;
                map_gbProps.Text = "Properties for (none selected)"; return;
            }
            
            if (name == last) return;
            last = name;
            LevelProperties settings = new LevelProperties(lvl);
            map_pgProps.SelectedObject = settings;
            map_gbProps.Text = "Properties for " + name;
        }
        
        void Map_UpdateUnloadedList() {
            string selectedLvl = null;
            if (map_lbUnloaded.SelectedItem != null)
                selectedLvl = map_lbUnloaded.SelectedItem.ToString();
            
            map_lbUnloaded.Items.Clear();
            string[] allMaps = LevelInfo.AllMapNames();
            foreach (string map in allMaps) {
                if (LevelInfo.FindExact(map) == null)
                    map_lbUnloaded.Items.Add(map);
            }
            
            if (selectedLvl != null) {
                int index = map_lbUnloaded.Items.IndexOf(selectedLvl);
                map_lbUnloaded.SelectedIndex = index;
            } else {
                map_lbUnloaded.SelectedIndex = -1;
            }
        }
        
        void Map_UpdateLoadedList() {
            Level[] loaded = LevelInfo.Loaded.Items;
            // Try to keep the same selection on update
            string selected = null;
            if (map_lbLoaded.SelectedItem != null) {
                selected = map_lbLoaded.SelectedItem.ToString();
            }
            
            map_lbLoaded.Items.Clear();
            foreach (Level lvl in loaded) {
                map_lbLoaded.Items.Add(lvl.name);
            }
            
            int index = -1;
            if (selected != null) {
                index = map_lbLoaded.Items.IndexOf(selected);                
            }
            
            map_lbLoaded.SelectedIndex = index;
            Map_UpdateSelected(null, null);
        }
    }
}