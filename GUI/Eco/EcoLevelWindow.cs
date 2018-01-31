using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MCGalaxy.Eco;
using MCGalaxy.Generator;
using Microsoft.Win32;

namespace MCGalaxy.Gui.Eco {
    public partial class EcoLevelWindow : Form {

        EconomyWindow _eco;
        string _name, _x, _y, _z, _type, _origName;
        int _price;

        public EcoLevelWindow(EconomyWindow eco, string name, int price, string x, string y, string z, string type) {
            _eco = eco;
            _name = name; _origName = name;
            _x = x; _y = y; _z = z;
            _price = price; _type = type;

            InitializeComponent();
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            this.Font = SystemFonts.IconTitleFont;
        }

        void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) {
            if (e.Category == UserPreferenceCategory.Window) {
                this.Font = SystemFonts.IconTitleFont;
            }
        }

        void EcoLevelWindow_FormClosing(object sender, FormClosingEventArgs e) {
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
        }

        static string[] dimensions = new string[] { "16", "32", "64", "128", "256", "512" };
        void EcoLevelWindow_Load(object sender, EventArgs e) {
            cmbX.DataSource = dimensions;
            cmbY.DataSource = dimensions;
            cmbZ.DataSource = dimensions;
            
            List<string> types = new List<string>();
            foreach (string theme in MapGen.SimpleThemeNames) {
                types.Add(theme);
            }
            cmbType.DataSource = types;

            txtName.Text = _name;
            numPrice.Value = _price;
            cmbX.SelectedItem = _x;
            cmbY.SelectedItem = _y;
            cmbZ.SelectedItem = _z;
            cmbType.SelectedItem = _type;
            btnOk.Enabled = _name.Length > 0;
        }

        void btnCancel_Click(object sender, EventArgs e) { Close(); }

        void btnOk_Click(object sender, EventArgs e) {
            LevelItem item = Economy.Levels;
            if (_origName.Length > 0) item.Presets.Remove(item.FindPreset(_origName));
            
            LevelItem.LevelPreset preset = new LevelItem.LevelPreset();
            preset.name = txtName.Text.Split()[0];
            preset.price = (int)numPrice.Value;
            preset.x = cmbX.SelectedItem.ToString();
            preset.y = cmbY.SelectedItem.ToString();
            preset.z = cmbZ.SelectedItem.ToString();
            preset.type = cmbType.SelectedItem.ToString().ToLower();
            
            item.Presets.Add(preset);
            _eco.UpdateLevels();
            _eco.CheckLevelEnables();
            Close();
        }

        void txtName_TextChanged(object sender, EventArgs e) {
            btnOk.Enabled = txtName.Text != null && txtName.Text.Length > 0;
        }
    }
}
