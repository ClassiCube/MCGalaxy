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
            if (mapgen) { Popup.Warning("Another map is already being generated."); return; }

            string name = map_txtName.Text;
            string seed = map_txtSeed.Text;
            if (String.IsNullOrEmpty(name)) { Popup.Warning("Map name cannot be blank."); return; }
            
            string x = Map_GetComboboxSize(map_cmbX, "width");
            if (x == null) return;           
            string y = Map_GetComboboxSize(map_cmbY, "height");
            if (y == null) return;            
            string z = Map_GetComboboxSize(map_cmbZ, "length");
            if (z == null) return;            
            string type = Map_GetComboboxItem(map_cmbType, "type");
            if (type == null) return;            

            string args = name + " " + x + " " + y + " " + z + " " + type;
            if (!String.IsNullOrEmpty(seed)) args += " " + seed;
            
            Thread genThread = new Thread(() => DoGen(name, args));
            genThread.Name = "MCG_GuiGenMap";
            genThread.Start();
        }
        
        void DoGen(string name, string args) {
            mapgen = true;
            try {
                Command.Find("NewLvl").Use(Player.Console, args);
            } catch (Exception ex) {
                Logger.LogError(ex);
                Popup.Error("Failed to generate level. Check error logs for details.");
                mapgen = false;
                return;
            }
            
            if (LevelInfo.MapExists(name)) {
                Popup.Message("Level successfully generated.");
                RunOnUI_Async(() => {
                    Map_UpdateUnloadedList();
                    Map_UpdateLoadedList();
                    Main_UpdateMapList();
                });
            } else {
               Popup.Error("Level was not generated. Check main log for details.");
            }
            mapgen = false;
        }
        
        string Map_GetComboboxItem(ComboBox box, string propName) {
            object selected = box.SelectedItem;
            string value = selected == null ? "" : selected.ToString();
            
            if (value.Length == 0) {
                Popup.Warning("Map " + propName + " cannot be blank.");
                return null;
            }
            return value;
        }
        
        string Map_GetComboboxSize(ComboBox box, string propName) {
            string value = box.Text; 
            
            if (value.Length == 0) {
                Popup.Warning("Map " + propName + " cannot be blank.");
                return null;
            }
            
            ushort size;
            if (!ushort.TryParse(value, out size) || size == 0 || size > 16384) {
                Popup.Warning("Map " + propName + " must be an integer between 1 and 16384");
                return null;
            }
            return value;
        }
        
        void map_BtnLoad_Click(object sender, EventArgs e) {
            object selected = map_lbUnloaded.SelectedItem;
            if (selected == null) { Popup.Warning("No map file selected."); return; }

            UIHelpers.HandleCommand("Load " + selected.ToString());
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
            object selected = map_lbUnloaded.SelectedItem;
            map_lbUnloaded.Items.Clear();
            
            string[] allMaps = LevelInfo.AllMapNames();
            foreach (string map in allMaps) {
                if (LevelInfo.FindExact(map) == null)
                    map_lbUnloaded.Items.Add(map);
            }
            
            Map_Reselect(map_lbUnloaded, selected);
        }
        
        void Map_UpdateLoadedList() {
            object selected = map_lbLoaded.SelectedItem;
            map_lbLoaded.Items.Clear();
            
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                map_lbLoaded.Items.Add(lvl.name);
            }
            
            Map_Reselect(map_lbLoaded, selected);
            Map_UpdateSelected(null, null);
        }
        
        void Map_Reselect(ListBox box, object selected) {
            int i = -1;
            if (selected != null) i = box.Items.IndexOf(selected);
            box.SelectedIndex = i;
        }
    }
}